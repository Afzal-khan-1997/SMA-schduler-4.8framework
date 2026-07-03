Imports System.ComponentModel
Imports System.Data
Imports System.Globalization
Imports System.Reflection

Public Class SqlProjectRepository
    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Sub TestConnection()
        Using connection = CreateConnection()
            connection.Open()
        End Using
    End Sub

    Public Sub SaveProject(projectName As String, tasks As BindingList(Of ScheduleTask), Optional projectVersion As String = "1.0", Optional projectSize As String = "Small", Optional projectType As String = "New", Optional totalProjectHours As Decimal = 0D, Optional resourcesNeeded As Integer = 0, Optional resourceHours As Decimal = 0D)
        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            Dim projectId = GetOrCreateProject(connection, projectName, projectVersion, projectSize, projectType, totalProjectHours, resourcesNeeded, resourceHours)
            Execute(connection, "DELETE FROM dbo.SmaScheduleTasks WHERE ProjectId = @ProjectId", New Dictionary(Of String, Object) From {{"@ProjectId", projectId}})
            Execute(connection, "DELETE FROM dbo.TaskAssignment WHERE ProjectId = @ProjectId", New Dictionary(Of String, Object) From {{"@ProjectId", projectId}})

            For Each task In tasks
                Execute(connection,
                        "INSERT INTO dbo.SmaScheduleTasks (ProjectId, TaskId, DatabaseTaskId, TaskName, StartDate, DurationDays, FinishDate, PercentComplete, Predecessors, DependencyType, AssignedTo, AssignmentDate, ResourceNames, ResourceAllocations, DailyResourceAllocations, ResourceHours, ModuleId, PlannerTaskId) VALUES (@ProjectId, @TaskId, @DatabaseTaskId, @TaskName, @StartDate, @DurationDays, @FinishDate, @PercentComplete, @Predecessors, @DependencyType, @AssignedTo, @AssignmentDate, @ResourceNames, @ResourceAllocations, @DailyResourceAllocations, @ResourceHours, @ModuleId, @PlannerTaskId)",
                        New Dictionary(Of String, Object) From {
                            {"@ProjectId", projectId},
                            {"@TaskId", task.TaskId},
                            {"@DatabaseTaskId", task.DatabaseTaskId},
                            {"@TaskName", task.TaskName},
                            {"@StartDate", task.StartDate},
                            {"@DurationDays", task.DurationDays},
                            {"@FinishDate", task.FinishDate},
                            {"@PercentComplete", task.PercentComplete},
                            {"@Predecessors", task.Predecessors},
                            {"@DependencyType", task.DependencyType},
                            {"@AssignedTo", task.AssignedTo},
                            {"@AssignmentDate", task.AssignmentDate},
                            {"@ResourceNames", task.ResourceNames},
                            {"@ResourceAllocations", task.ResourceAllocations},
                            {"@DailyResourceAllocations", task.DailyResourceAllocations},
                            {"@ResourceHours", task.ResourceHours},
                            {"@ModuleId", task.ModuleId},
                            {"@PlannerTaskId", task.PlannerTaskId}
                        })
            Next

            SaveTaskAssignments(connection, projectId, projectName, projectVersion, projectSize, projectType, tasks)
        End Using
    End Sub

    Public Function ListProjects(Optional maxResults As Integer = 50) As List(Of ProjectLibraryItem)
        Dim projectsFromAssignments = ListProjectsFromAssignments(maxResults)
        If projectsFromAssignments.Count > 0 Then
            Return projectsFromAssignments
        End If

        Dim projects As New List(Of ProjectLibraryItem)()
        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT TOP " & Math.Max(1, maxResults).ToString(CultureInfo.InvariantCulture) & " " &
                    "p.ProjectId, p.ProjectName, p.ProjectVersion, p.ProjectSize, p.ProjectType, p.TotalProjectHours, p.ResourceHours, p.UpdatedAt, " &
                    "COUNT(t.Id) AS TaskCount, MIN(t.StartDate) AS StartDate, MAX(t.FinishDate) AS FinishDate " &
                    "FROM dbo.SmaScheduleProjects p " &
                    "LEFT JOIN dbo.SmaScheduleTasks t ON t.ProjectId = p.ProjectId " &
                    "GROUP BY p.ProjectId, p.ProjectName, p.ProjectVersion, p.ProjectSize, p.ProjectType, p.TotalProjectHours, p.ResourceHours, p.UpdatedAt " &
                    "ORDER BY p.ProjectId DESC"

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim item As New ProjectLibraryItem With {
                            .ProjectId = CInt(reader("ProjectId")),
                            .ProjectName = CStr(reader("ProjectName")),
                            .VersionNumber = If(reader("ProjectVersion") Is DBNull.Value, "1.0", CStr(reader("ProjectVersion"))),
                            .ProjectSize = If(reader("ProjectSize") Is DBNull.Value OrElse String.IsNullOrWhiteSpace(CStr(reader("ProjectSize"))), "Small", CStr(reader("ProjectSize"))),
                            .ProjectType = ResolveProjectType(If(reader("ProjectType") Is DBNull.Value, "", CStr(reader("ProjectType"))), CStr(reader("ProjectName"))),
                            .TaskCount = If(reader("TaskCount") Is DBNull.Value, 0, CInt(reader("TaskCount"))),
                            .ResourceHours = If(reader("ResourceHours") Is DBNull.Value, 0D, CDec(reader("ResourceHours"))),
                            .UpdatedOn = If(reader("UpdatedAt") Is DBNull.Value, Date.Now, CDate(reader("UpdatedAt")))
                        }

                        If reader("StartDate") IsNot DBNull.Value Then
                            item.StartDate = CDate(reader("StartDate")).Date
                            item.StartDateText = item.StartDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
                        End If

                        If reader("FinishDate") IsNot DBNull.Value Then
                            item.FinishDate = CDate(reader("FinishDate")).Date
                            item.FinishDateText = item.FinishDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
                        End If

                        projects.Add(item)
                    End While
                End Using
            End Using
        End Using

        Return projects
    End Function

    Public Function LoadProjectSnapshot(projectId As Integer) As ProjectSnapshot
        Dim assignmentSnapshot = LoadProjectSnapshotFromAssignments(projectId)
        If assignmentSnapshot IsNot Nothing Then
            Return assignmentSnapshot
        End If

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT ProjectId, ProjectName, ProjectVersion, ProjectSize, ProjectType, TotalProjectHours, ResourcesNeeded, ResourceHours, UpdatedAt " &
                    "FROM dbo.SmaScheduleProjects WHERE ProjectId = @ProjectId"
                AddParameter(command, "@ProjectId", projectId)

                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then
                        Return Nothing
                    End If

                    Dim snapshot As New ProjectSnapshot With {
                        .ProjectName = CStr(reader("ProjectName")),
                        .VersionNumber = If(reader("ProjectVersion") Is DBNull.Value, "1.0", CStr(reader("ProjectVersion"))),
                        .ProjectSize = If(reader("ProjectSize") Is DBNull.Value OrElse String.IsNullOrWhiteSpace(CStr(reader("ProjectSize"))), "Small", CStr(reader("ProjectSize"))),
                        .ProjectType = ResolveProjectType(If(reader("ProjectType") Is DBNull.Value, "", CStr(reader("ProjectType"))), CStr(reader("ProjectName"))),
                        .TotalProjectHours = If(reader("TotalProjectHours") Is DBNull.Value, 0D, CDec(reader("TotalProjectHours"))),
                        .ResourcesNeeded = If(reader("ResourcesNeeded") Is DBNull.Value, 0, CInt(reader("ResourcesNeeded"))),
                        .ResourceHours = If(reader("ResourceHours") Is DBNull.Value, 0D, CDec(reader("ResourceHours"))),
                        .UpdatedOn = If(reader("UpdatedAt") Is DBNull.Value, Date.Now, CDate(reader("UpdatedAt")))
                    }

                    reader.Close()
                    snapshot.Tasks = LoadTasksByProjectId(connection, projectId).ToList()
                    Return snapshot
                End Using
            End Using
        End Using
    End Function

    Public Function LoadProject(projectName As String) As BindingList(Of ScheduleTask)
        Dim snapshotFromAssignments = LoadProjectSnapshotByNameFromAssignments(projectName)
        If snapshotFromAssignments IsNot Nothing AndAlso snapshotFromAssignments.Tasks IsNot Nothing Then
            Return New BindingList(Of ScheduleTask)(snapshotFromAssignments.Tasks.OrderBy(Function(task) task.TaskId).ToList())
        End If

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            Using command = connection.CreateCommand()
                command.CommandText = "SELECT ProjectId FROM dbo.SmaScheduleProjects WHERE ProjectName = @ProjectName"
                AddParameter(command, "@ProjectName", projectName)
                Dim projectIdValue = command.ExecuteScalar()
                If projectIdValue Is Nothing OrElse projectIdValue Is DBNull.Value Then
                    Return New BindingList(Of ScheduleTask)()
                End If
                Return LoadTasksByProjectId(connection, CInt(projectIdValue))
            End Using
        End Using
    End Function

    Public Function LoadResourceAvailability(startDate As Date, finishDate As Date) As Dictionary(Of String, Dictionary(Of Date, Decimal))
        Dim result As New Dictionary(Of String, Dictionary(Of Date, Decimal))(StringComparer.OrdinalIgnoreCase)

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If TableHasRows(connection, "dbo.EmployeeDailyAvailability") Then
                LoadAvailabilityFromDailyAvailability(connection, result, startDate, finishDate)
                If result.Count > 0 Then
                    Return result
                End If
            End If

            Using command = connection.CreateCommand()
                command.CommandText =
                    "IF OBJECT_ID('dbo.SmaResourceAvailability', 'U') IS NULL " &
                    "SELECT CAST(NULL AS NVARCHAR(200)) AS ResourceName, CAST(NULL AS DATE) AS WorkDate, CAST(NULL AS DECIMAL(12,2)) AS AvailableHours WHERE 1 = 0 " &
                    "ELSE " &
                    "SELECT ResourceName, WorkDate, AvailableHours FROM dbo.SmaResourceAvailability WHERE WorkDate BETWEEN @StartDate AND @FinishDate ORDER BY ResourceName, WorkDate"
                AddParameter(command, "@StartDate", startDate.Date)
                AddParameter(command, "@FinishDate", finishDate.Date)

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        If reader("ResourceName") Is DBNull.Value OrElse reader("WorkDate") Is DBNull.Value Then
                            Continue While
                        End If

                        Dim resourceName = CStr(reader("ResourceName")).Trim()
                        If resourceName.Length = 0 Then
                            Continue While
                        End If

                        Dim workDate = CDate(reader("WorkDate")).Date
                        Dim hours = If(reader("AvailableHours") Is DBNull.Value, 8D, CDec(reader("AvailableHours")))

                        Dim byDate As Dictionary(Of Date, Decimal) = Nothing
                        If Not result.TryGetValue(resourceName, byDate) Then
                            byDate = New Dictionary(Of Date, Decimal)()
                            result(resourceName) = byDate
                        End If

                        byDate(workDate) = Math.Max(0D, hours)
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function

    Public Function LoadEmployees() As List(Of String)
        Dim result As New List(Of String)()

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableExists(connection, "dbo.Employees") Then
                Return result
            End If

            Using command = connection.CreateCommand()
                command.CommandText =
                    "IF COL_LENGTH('dbo.Employees', 'EmployeeName') IS NOT NULL " &
                    "SELECT DISTINCT LTRIM(RTRIM(EmployeeName)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(EmployeeName)), '') <> '' ORDER BY EmployeeName " &
                    "ELSE IF COL_LENGTH('dbo.Employees', 'firstname') IS NOT NULL " &
                    "SELECT DISTINCT LTRIM(RTRIM(firstname)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(firstname)), '') <> '' ORDER BY firstname " &
                    "ELSE " &
                    "SELECT CAST(NULL AS NVARCHAR(200)) AS EmployeeName WHERE 1 = 0"

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        If reader("EmployeeName") Is DBNull.Value Then
                            Continue While
                        End If

                        Dim employeeName = CStr(reader("EmployeeName")).Trim()
                        If employeeName.Length > 0 Then
                            result.Add(employeeName)
                        End If
                    End While
                End Using
            End Using
        End Using

        Return result.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(Function(name) name, StringComparer.OrdinalIgnoreCase).ToList()
    End Function

    Public Function LoadTaskCatalog() As List(Of TaskCatalogItem)
        Return LoadTaskTemplates("", "")
    End Function

    Public Function LoadTaskTemplates(templateName As String, projectSize As String) As List(Of TaskCatalogItem)
        Dim result As New List(Of TaskCatalogItem)()

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableExists(connection, "dbo.TaskTemplate") Then
                Return result
            End If

            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT TaskTemplateId, TemplateName, TaskOrder, TaskName, ProjectSize, ProjectType, DefaultHours, DependencyTaskOrder, DependencyType, ModuleId " &
                    "FROM dbo.TaskTemplate " &
                    "WHERE IsActive = 1 " &
                    "AND (@TemplateName = '' OR TemplateName = @TemplateName) " &
                    "AND (@ProjectSize = '' OR ISNULL(ProjectSize, '') = '' OR ProjectSize = @ProjectSize) " &
                    "ORDER BY TemplateName, TaskOrder"
                AddParameter(command, "@TemplateName", If(templateName, "").Trim())
                AddParameter(command, "@ProjectSize", If(projectSize, "").Trim())

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim defaultHours = If(reader("DefaultHours") Is DBNull.Value, 0D, CDec(reader("DefaultHours")))
                        Dim predecessorText = ""
                        If reader("DependencyTaskOrder") IsNot DBNull.Value Then
                            predecessorText = CInt(reader("DependencyTaskOrder")).ToString(CultureInfo.InvariantCulture)
                        End If

                        result.Add(New TaskCatalogItem With {
                            .DatabaseTaskId = CInt(reader("TaskTemplateId")),
                            .Title = CStr(reader("TaskName")),
                            .Predecessor = predecessorText,
                            .DependencyType = If(reader("DependencyType") Is DBNull.Value, "FS", CStr(reader("DependencyType"))),
                            .Summary = If(reader("TemplateName") Is DBNull.Value, "", CStr(reader("TemplateName"))),
                            .SmallHours = defaultHours,
                            .MediumHours = defaultHours,
                            .LargeHours = defaultHours,
                            .VeryLargeHours = defaultHours,
                            .Assignee = If(reader("ProjectType") Is DBNull.Value, "", CStr(reader("ProjectType"))),
                            .ModuleId = If(reader("ModuleId") Is DBNull.Value, 0, CInt(reader("ModuleId")))
                        })
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function

    Public Function LoadTemplateProjects(Optional searchText As String = "") As List(Of LiveProjectItem)
        Dim result As New List(Of LiveProjectItem)()

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableExists(connection, "dbo.TaskTemplate") Then
                Return result
            End If

            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT TemplateName, " &
                    "MAX(CASE WHEN ISNULL(ProjectSize, '') = '' THEN 'Small' ELSE ProjectSize END) AS ProjectSize, " &
                    "MAX(CASE WHEN ISNULL(ProjectType, '') = '' THEN 'New' ELSE ProjectType END) AS ProjectType " &
                    "FROM dbo.TaskTemplate " &
                    "WHERE IsActive = 1 AND (@SearchText = '' OR TemplateName LIKE '%' + @SearchText + '%' OR ProjectType LIKE '%' + @SearchText + '%' OR ProjectSize LIKE '%' + @SearchText + '%') " &
                    "GROUP BY TemplateName " &
                    "ORDER BY TemplateName"
                AddParameter(command, "@SearchText", If(searchText, "").Trim())

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim templateNameValue = If(reader("TemplateName") Is DBNull.Value, "", CStr(reader("TemplateName")).Trim())
                        If templateNameValue.Length = 0 Then
                            Continue While
                        End If

                        result.Add(New LiveProjectItem With {
                            .ProjectCode = "TPL-" & MakeSafeCode(templateNameValue),
                            .ProjectName = templateNameValue,
                            .ClientName = "SQL Template",
                            .VersionNumber = "1.0",
                            .ProjectSize = If(reader("ProjectSize") Is DBNull.Value, "Small", CStr(reader("ProjectSize"))),
                            .TemplateName = templateNameValue,
                            .ProjectType = If(reader("ProjectType") Is DBNull.Value, "New", CStr(reader("ProjectType"))),
                            .ReportType = If(reader("ProjectType") Is DBNull.Value, "New", CStr(reader("ProjectType")))
                        })
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function

    Private Function LoadTasksByProjectId(connection As IDbConnection, projectId As Integer) As BindingList(Of ScheduleTask)
        Dim tasks As New BindingList(Of ScheduleTask)
        Using command = connection.CreateCommand()
            command.CommandText = "SELECT TaskId, DatabaseTaskId, TaskName, StartDate, DurationDays, FinishDate, PercentComplete, Predecessors, DependencyType, AssignedTo, AssignmentDate, ResourceNames, ResourceAllocations, DailyResourceAllocations, ResourceHours, ModuleId, PlannerTaskId FROM dbo.SmaScheduleTasks WHERE ProjectId = @ProjectId ORDER BY TaskId"
            AddParameter(command, "@ProjectId", projectId)

            Using reader = command.ExecuteReader()
                While reader.Read()
                    tasks.Add(New ScheduleTask With {
                        .TaskId = CInt(reader("TaskId")),
                        .DatabaseTaskId = If(reader("DatabaseTaskId") Is DBNull.Value, 0, CInt(reader("DatabaseTaskId"))),
                        .TaskName = CStr(reader("TaskName")),
                        .StartDate = CDate(reader("StartDate")),
                        .DurationDays = If(reader("DurationDays") Is DBNull.Value, 0D, CDec(reader("DurationDays"))),
                        .FinishDate = CDate(reader("FinishDate")),
                        .PercentComplete = CInt(reader("PercentComplete")),
                        .Predecessors = If(reader("Predecessors") Is DBNull.Value, "", CStr(reader("Predecessors"))),
                        .DependencyType = If(reader("DependencyType") Is DBNull.Value, "FS", CStr(reader("DependencyType"))),
                        .AssignedTo = If(reader("AssignedTo") Is DBNull.Value, "", CStr(reader("AssignedTo"))),
                        .AssignmentDate = If(reader("AssignmentDate") Is DBNull.Value, CDate(reader("StartDate")), CDate(reader("AssignmentDate"))),
                        .ResourceNames = If(reader("ResourceNames") Is DBNull.Value, "", CStr(reader("ResourceNames"))),
                        .ResourceAllocations = If(reader("ResourceAllocations") Is DBNull.Value, "", CStr(reader("ResourceAllocations"))),
                        .DailyResourceAllocations = If(reader("DailyResourceAllocations") Is DBNull.Value, "", CStr(reader("DailyResourceAllocations"))),
                        .ResourceHours = If(reader("ResourceHours") Is DBNull.Value, 0D, CDec(reader("ResourceHours"))),
                        .ModuleId = If(reader("ModuleId") Is DBNull.Value, 0, CInt(reader("ModuleId"))),
                        .PlannerTaskId = If(reader("PlannerTaskId") Is DBNull.Value, "", CStr(reader("PlannerTaskId")))
                    })
                End While
            End Using
        End Using

        Return tasks
    End Function

    Private Function CreateConnection() As IDbConnection
        Dim connectionType = ResolveSqlConnectionType()
        Dim connection = DirectCast(Activator.CreateInstance(connectionType), IDbConnection)
        connection.ConnectionString = _connectionString
        Return connection
    End Function

    Private Function ResolveSqlConnectionType() As Type
        For Each name In {"Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient", "System.Data.SqlClient.SqlConnection, System.Data.SqlClient"}
            Dim resolved = Type.GetType(name, False)
            If resolved IsNot Nothing Then
                Return resolved
            End If
        Next

        Throw New InvalidOperationException("No SQL Server provider was found. Add Microsoft.Data.SqlClient to the project or install the SQL Server client provider on this machine.")
    End Function

    Private Sub EnsureSchema(connection As IDbConnection)
        Execute(connection,
                "IF OBJECT_ID('dbo.TaskTemplate', 'U') IS NULL CREATE TABLE dbo.TaskTemplate (TaskTemplateId INT IDENTITY(1,1) NOT NULL PRIMARY KEY, TemplateName NVARCHAR(200) NOT NULL, TaskOrder INT NOT NULL, TaskName NVARCHAR(300) NOT NULL, ProjectSize NVARCHAR(50) NULL, ProjectType NVARCHAR(50) NULL, DefaultHours DECIMAL(12,2) NOT NULL DEFAULT 0, DependencyTaskOrder INT NULL, DependencyType NVARCHAR(2) NULL, ModuleId INT NULL, IsActive BIT NOT NULL DEFAULT 1, CreatedOn DATETIME NOT NULL DEFAULT GETDATE())",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.EmployeeDailyAvailability', 'U') IS NULL CREATE TABLE dbo.EmployeeDailyAvailability (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, EmployeeId INT NOT NULL, WorkDate DATE NOT NULL, DefaultHours DECIMAL(5,2) NOT NULL DEFAULT 8, LeaveHours DECIMAL(5,2) NOT NULL DEFAULT 0, AvailableHours DECIMAL(5,2) NOT NULL DEFAULT 8, EntryType NVARCHAR(50) NULL, Remarks NVARCHAR(300) NULL, UpdatedOn DATETIME NULL, UpdatedBy NVARCHAR(100) NULL, CONSTRAINT UQ_EmployeeDailyAvailability UNIQUE(EmployeeId, WorkDate))",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.TaskAssignment', 'U') IS NULL CREATE TABLE dbo.TaskAssignment (AssignmentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ProjectId INT NOT NULL, ProjectName NVARCHAR(200) NOT NULL, VersionNumber NVARCHAR(50) NULL, ProjectSize NVARCHAR(50) NULL, ProjectType NVARCHAR(50) NULL, TaskId INT NOT NULL, TaskName NVARCHAR(300) NOT NULL, TaskOrder INT NOT NULL, EmployeeId INT NOT NULL, WorkDate DATE NOT NULL, AssignedHours DECIMAL(12,2) NOT NULL DEFAULT 0, StartDate DATE NULL, FinishDate DATE NULL, DependencyTaskOrder INT NULL, DependencyType NVARCHAR(2) NULL, Remarks NVARCHAR(300) NULL, UpdatedOn DATETIME NOT NULL DEFAULT GETDATE(), UpdatedBy NVARCHAR(100) NULL)",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleProjects', 'U') IS NULL CREATE TABLE dbo.SmaScheduleProjects (ProjectId INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ProjectName NVARCHAR(200) NOT NULL UNIQUE, ProjectVersion NVARCHAR(50) NOT NULL DEFAULT '1.0', ProjectSize NVARCHAR(50) NULL, ProjectType NVARCHAR(50) NULL, TotalProjectHours DECIMAL(12,2) NOT NULL DEFAULT 0, ResourcesNeeded INT NOT NULL DEFAULT 0, ResourceHours DECIMAL(12,2) NOT NULL DEFAULT 0, CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME())",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleTasks', 'U') IS NULL CREATE TABLE dbo.SmaScheduleTasks (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ProjectId INT NOT NULL, TaskId INT NOT NULL, DatabaseTaskId INT NULL, TaskName NVARCHAR(300) NOT NULL, StartDate DATE NOT NULL, DurationDays DECIMAL(12,3) NOT NULL, FinishDate DATE NOT NULL, PercentComplete INT NOT NULL, Predecessors NVARCHAR(200) NULL, DependencyType NVARCHAR(2) NOT NULL DEFAULT 'FS', AssignedTo NVARCHAR(200) NULL, AssignmentDate DATE NULL, ResourceNames NVARCHAR(500) NULL, ResourceAllocations NVARCHAR(1000) NULL, DailyResourceAllocations NVARCHAR(MAX) NULL, ResourceHours DECIMAL(12,2) NOT NULL DEFAULT 0, ModuleId INT NULL, PlannerTaskId NVARCHAR(200) NULL, CONSTRAINT FK_SmaScheduleTasks_Project FOREIGN KEY(ProjectId) REFERENCES dbo.SmaScheduleProjects(ProjectId), CONSTRAINT UQ_SmaScheduleTasks_Project_Task UNIQUE(ProjectId, TaskId))",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleProjects', 'U') IS NOT NULL AND COL_LENGTH('dbo.SmaScheduleProjects', 'ProjectSize') IS NULL ALTER TABLE dbo.SmaScheduleProjects ADD ProjectSize NVARCHAR(50) NULL",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleProjects', 'U') IS NOT NULL AND COL_LENGTH('dbo.SmaScheduleProjects', 'ProjectType') IS NULL ALTER TABLE dbo.SmaScheduleProjects ADD ProjectType NVARCHAR(50) NULL",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleTasks', 'U') IS NOT NULL AND COL_LENGTH('dbo.SmaScheduleTasks', 'DependencyType') IS NULL ALTER TABLE dbo.SmaScheduleTasks ADD DependencyType NVARCHAR(2) NOT NULL CONSTRAINT DF_SmaScheduleTasks_DependencyType DEFAULT 'FS'",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaScheduleTasks', 'U') IS NOT NULL AND COL_LENGTH('dbo.SmaScheduleTasks', 'DailyResourceAllocations') IS NULL ALTER TABLE dbo.SmaScheduleTasks ADD DailyResourceAllocations NVARCHAR(MAX) NULL",
                Nothing)
        Execute(connection,
                "IF OBJECT_ID('dbo.SmaResourceAvailability', 'U') IS NULL CREATE TABLE dbo.SmaResourceAvailability (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ResourceName NVARCHAR(200) NOT NULL, WorkDate DATE NOT NULL, AvailableHours DECIMAL(12,2) NOT NULL DEFAULT 8, UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), CONSTRAINT UQ_SmaResourceAvailability UNIQUE(ResourceName, WorkDate))",
                Nothing)
    End Sub

    Private Function GetOrCreateProject(connection As IDbConnection, projectName As String, projectVersion As String, projectSize As String, projectType As String, totalProjectHours As Decimal, resourcesNeeded As Integer, resourceHours As Decimal) As Integer
        Execute(connection,
                "IF NOT EXISTS (SELECT 1 FROM dbo.SmaScheduleProjects WHERE ProjectName = @ProjectName) INSERT INTO dbo.SmaScheduleProjects (ProjectName, ProjectVersion, ProjectSize, ProjectType, TotalProjectHours, ResourcesNeeded, ResourceHours) VALUES (@ProjectName, @ProjectVersion, @ProjectSize, @ProjectType, @TotalProjectHours, @ResourcesNeeded, @ResourceHours) ELSE UPDATE dbo.SmaScheduleProjects SET ProjectVersion = @ProjectVersion, ProjectSize = @ProjectSize, ProjectType = @ProjectType, TotalProjectHours = @TotalProjectHours, ResourcesNeeded = @ResourcesNeeded, ResourceHours = @ResourceHours, UpdatedAt = SYSUTCDATETIME() WHERE ProjectName = @ProjectName",
                New Dictionary(Of String, Object) From {
                    {"@ProjectName", projectName},
                    {"@ProjectVersion", If(String.IsNullOrWhiteSpace(projectVersion), "1.0", projectVersion)},
                    {"@ProjectSize", If(String.IsNullOrWhiteSpace(projectSize), "Small", projectSize)},
                    {"@ProjectType", If(String.IsNullOrWhiteSpace(projectType), "New", projectType)},
                    {"@TotalProjectHours", totalProjectHours},
                    {"@ResourcesNeeded", resourcesNeeded},
                    {"@ResourceHours", resourceHours}
                })

        Using command = connection.CreateCommand()
            command.CommandText = "SELECT ProjectId FROM dbo.SmaScheduleProjects WHERE ProjectName = @ProjectName"
            AddParameter(command, "@ProjectName", projectName)
            Return CInt(command.ExecuteScalar())
        End Using
    End Function

    Private Function ListProjectsFromAssignments(maxResults As Integer) As List(Of ProjectLibraryItem)
        Dim projects As New List(Of ProjectLibraryItem)()

        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableHasRows(connection, "dbo.TaskAssignment") Then
                Return projects
            End If

            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT TOP " & Math.Max(1, maxResults).ToString(CultureInfo.InvariantCulture) & " " &
                    "ProjectId, ProjectName, ISNULL(VersionNumber, '1.0') AS VersionNumber, ISNULL(ProjectSize, 'Small') AS ProjectSize, ISNULL(ProjectType, 'New') AS ProjectType, " &
                    "COUNT(DISTINCT TaskId) AS TaskCount, SUM(AssignedHours) AS ResourceHours, MIN(ISNULL(StartDate, WorkDate)) AS StartDate, MAX(ISNULL(FinishDate, WorkDate)) AS FinishDate, MAX(UpdatedOn) AS UpdatedOn " &
                    "FROM dbo.TaskAssignment GROUP BY ProjectId, ProjectName, VersionNumber, ProjectSize, ProjectType ORDER BY MAX(UpdatedOn) DESC, ProjectId DESC"

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim item As New ProjectLibraryItem With {
                            .ProjectId = CInt(reader("ProjectId")),
                            .ProjectName = CStr(reader("ProjectName")),
                            .VersionNumber = If(reader("VersionNumber") Is DBNull.Value, "1.0", CStr(reader("VersionNumber"))),
                            .ProjectSize = If(reader("ProjectSize") Is DBNull.Value, "Small", CStr(reader("ProjectSize"))),
                            .ProjectType = If(reader("ProjectType") Is DBNull.Value, "New", CStr(reader("ProjectType"))),
                            .TaskCount = If(reader("TaskCount") Is DBNull.Value, 0, CInt(reader("TaskCount"))),
                            .ResourceHours = If(reader("ResourceHours") Is DBNull.Value, 0D, CDec(reader("ResourceHours"))),
                            .UpdatedOn = If(reader("UpdatedOn") Is DBNull.Value, Date.Now, CDate(reader("UpdatedOn")))
                        }

                        If reader("StartDate") IsNot DBNull.Value Then
                            item.StartDate = CDate(reader("StartDate")).Date
                            item.StartDateText = item.StartDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
                        End If

                        If reader("FinishDate") IsNot DBNull.Value Then
                            item.FinishDate = CDate(reader("FinishDate")).Date
                            item.FinishDateText = item.FinishDate.Value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
                        End If

                        projects.Add(item)
                    End While
                End Using
            End Using
        End Using

        Return projects
    End Function

    Private Function LoadProjectSnapshotFromAssignments(projectId As Integer) As ProjectSnapshot
        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableHasRows(connection, "dbo.TaskAssignment") Then
                Return Nothing
            End If

            Return LoadProjectSnapshotFromAssignments(connection, projectId)
        End Using
    End Function

    Private Function LoadProjectSnapshotByNameFromAssignments(projectName As String) As ProjectSnapshot
        Using connection = CreateConnection()
            connection.Open()
            EnsureSchema(connection)

            If Not TableHasRows(connection, "dbo.TaskAssignment") Then
                Return Nothing
            End If

            Using command = connection.CreateCommand()
                command.CommandText = "SELECT TOP 1 ProjectId FROM dbo.TaskAssignment WHERE ProjectName = @ProjectName ORDER BY UpdatedOn DESC"
                AddParameter(command, "@ProjectName", projectName)
                Dim value = command.ExecuteScalar()
                If value Is Nothing OrElse value Is DBNull.Value Then
                    Return Nothing
                End If

                Return LoadProjectSnapshotFromAssignments(connection, CInt(value))
            End Using
        End Using
    End Function

    Private Function LoadProjectSnapshotFromAssignments(connection As IDbConnection, projectId As Integer) As ProjectSnapshot
        Using command = connection.CreateCommand()
            command.CommandText =
                "SELECT ProjectId, ProjectName, ISNULL(VersionNumber, '1.0') AS VersionNumber, ISNULL(ProjectSize, 'Small') AS ProjectSize, ISNULL(ProjectType, 'New') AS ProjectType, " &
                "TaskId, TaskName, TaskOrder, EmployeeId, WorkDate, AssignedHours, StartDate, FinishDate, DependencyTaskOrder, DependencyType, UpdatedOn " &
                "FROM dbo.TaskAssignment WHERE ProjectId = @ProjectId ORDER BY TaskOrder, WorkDate, EmployeeId"
            AddParameter(command, "@ProjectId", projectId)

            Dim rows As New List(Of AssignmentSnapshotRow)()
            Using reader = command.ExecuteReader()
                While reader.Read()
                    rows.Add(New AssignmentSnapshotRow With {
                        .ProjectId = CInt(reader("ProjectId")),
                        .ProjectName = CStr(reader("ProjectName")),
                        .VersionNumber = If(reader("VersionNumber") Is DBNull.Value, "1.0", CStr(reader("VersionNumber"))),
                        .ProjectSize = If(reader("ProjectSize") Is DBNull.Value, "Small", CStr(reader("ProjectSize"))),
                        .ProjectType = If(reader("ProjectType") Is DBNull.Value, "New", CStr(reader("ProjectType"))),
                        .TaskId = CInt(reader("TaskId")),
                        .TaskName = CStr(reader("TaskName")),
                        .TaskOrder = CInt(reader("TaskOrder")),
                        .EmployeeId = CInt(reader("EmployeeId")),
                        .WorkDate = CDate(reader("WorkDate")).Date,
                        .AssignedHours = If(reader("AssignedHours") Is DBNull.Value, 0D, CDec(reader("AssignedHours"))),
                        .StartDate = If(reader("StartDate") Is DBNull.Value, CType(Nothing, Date?), CDate(reader("StartDate")).Date),
                        .FinishDate = If(reader("FinishDate") Is DBNull.Value, CType(Nothing, Date?), CDate(reader("FinishDate")).Date),
                        .DependencyTaskOrder = If(reader("DependencyTaskOrder") Is DBNull.Value, CType(Nothing, Integer?), CInt(reader("DependencyTaskOrder"))),
                        .DependencyType = If(reader("DependencyType") Is DBNull.Value, "FS", CStr(reader("DependencyType"))),
                        .UpdatedOn = If(reader("UpdatedOn") Is DBNull.Value, Date.Now, CDate(reader("UpdatedOn")))
                    })
                End While
            End Using

            If rows.Count = 0 Then
                Return Nothing
            End If

            Dim employeeNames = LoadEmployeeNameLookup(connection)
            Dim firstRow = rows(0)
            Dim snapshot As New ProjectSnapshot With {
                .ProjectName = firstRow.ProjectName,
                .VersionNumber = firstRow.VersionNumber,
                .ProjectSize = firstRow.ProjectSize,
                .ProjectType = firstRow.ProjectType,
                .ResourceHours = rows.Sum(Function(row) row.AssignedHours),
                .UpdatedOn = rows.Max(Function(row) row.UpdatedOn)
            }

            Dim groupedTasks = rows.GroupBy(Function(row) row.TaskId).OrderBy(Function(group) group.Min(Function(item) item.TaskOrder))
            For Each taskGroup In groupedTasks
                Dim taskRows = taskGroup.OrderBy(Function(item) item.WorkDate).ToList()
                Dim taskNames = taskRows.
                    Select(Function(row)
                               If employeeNames.ContainsKey(row.EmployeeId) Then
                                   Return employeeNames(row.EmployeeId)
                               End If
                               Return "Employee " & row.EmployeeId.ToString(CultureInfo.InvariantCulture)
                           End Function).
                    Distinct(StringComparer.OrdinalIgnoreCase).
                    ToList()

                Dim allocationTotals As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
                Dim dailyAllocations As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

                For Each row In taskRows
                    Dim employeeName = If(employeeNames.ContainsKey(row.EmployeeId), employeeNames(row.EmployeeId), "Employee " & row.EmployeeId.ToString(CultureInfo.InvariantCulture))
                    If Not allocationTotals.ContainsKey(employeeName) Then
                        allocationTotals(employeeName) = 0D
                    End If
                    allocationTotals(employeeName) += row.AssignedHours
                    dailyAllocations(employeeName.Trim() & "|" & row.WorkDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) = row.AssignedHours
                Next

                Dim predecessors = taskRows.
                    Where(Function(row) row.DependencyTaskOrder.HasValue).
                    Select(Function(row) row.DependencyTaskOrder.Value.ToString(CultureInfo.InvariantCulture)).
                    Distinct().
                    ToList()

                snapshot.Tasks.Add(New ScheduleTask With {
                    .TaskId = taskGroup.Key,
                    .DatabaseTaskId = taskGroup.Key,
                    .TaskName = taskRows(0).TaskName,
                    .StartDate = taskRows.Min(Function(row) If(row.StartDate.HasValue, row.StartDate.Value, row.WorkDate)),
                    .FinishDate = taskRows.Max(Function(row) If(row.FinishDate.HasValue, row.FinishDate.Value, row.WorkDate)),
                    .DurationDays = taskRows.Max(Function(row) If(row.FinishDate.HasValue, row.FinishDate.Value, row.WorkDate)).Subtract(taskRows.Min(Function(row) If(row.StartDate.HasValue, row.StartDate.Value, row.WorkDate))).Days + 1D,
                    .PercentComplete = 0,
                    .Predecessors = String.Join(",", predecessors),
                    .DependencyType = taskRows.Select(Function(row) If(String.IsNullOrWhiteSpace(row.DependencyType), "FS", row.DependencyType)).FirstOrDefault(),
                    .AssignedTo = String.Join("; ", taskNames),
                    .AssignmentDate = taskRows.Min(Function(row) row.WorkDate),
                    .ResourceNames = String.Join("; ", taskNames),
                    .ResourceAllocations = BuildResourceAllocationsString(allocationTotals),
                    .DailyResourceAllocations = BuildDailyAllocationsString(dailyAllocations),
                    .ResourceHours = taskRows.Sum(Function(row) row.AssignedHours),
                    .ModuleId = 0
                })
            Next

            If snapshot.Tasks.Count > 0 Then
                snapshot.TotalProjectHours = snapshot.Tasks.Sum(Function(task) task.ResourceHours)
                snapshot.ResourcesNeeded = rows.Select(Function(row) row.EmployeeId).Distinct().Count()
            End If

            Return snapshot
        End Using
    End Function

    Private Sub SaveTaskAssignments(connection As IDbConnection, projectId As Integer, projectName As String, projectVersion As String, projectSize As String, projectType As String, tasks As BindingList(Of ScheduleTask))
        Dim employeeLookup = LoadEmployeeIdLookup(connection)
        Dim missingEmployees As New List(Of String)()

        For Each task In tasks
            Dim assignmentRows = BuildTaskAssignmentRows(task)
            For Each assignmentRow In assignmentRows
                If String.IsNullOrWhiteSpace(assignmentRow.EmployeeName) Then
                    Continue For
                End If

                If Not employeeLookup.ContainsKey(assignmentRow.EmployeeName) Then
                    If Not missingEmployees.Contains(assignmentRow.EmployeeName, StringComparer.OrdinalIgnoreCase) Then
                        missingEmployees.Add(assignmentRow.EmployeeName)
                    End If
                    Continue For
                End If

                Execute(connection,
                        "INSERT INTO dbo.TaskAssignment (ProjectId, ProjectName, VersionNumber, ProjectSize, ProjectType, TaskId, TaskName, TaskOrder, EmployeeId, WorkDate, AssignedHours, StartDate, FinishDate, DependencyTaskOrder, DependencyType, Remarks, UpdatedOn, UpdatedBy) " &
                        "VALUES (@ProjectId, @ProjectName, @VersionNumber, @ProjectSize, @ProjectType, @TaskId, @TaskName, @TaskOrder, @EmployeeId, @WorkDate, @AssignedHours, @StartDate, @FinishDate, @DependencyTaskOrder, @DependencyType, @Remarks, GETDATE(), @UpdatedBy)",
                        New Dictionary(Of String, Object) From {
                            {"@ProjectId", projectId},
                            {"@ProjectName", projectName},
                            {"@VersionNumber", If(String.IsNullOrWhiteSpace(projectVersion), "1.0", projectVersion)},
                            {"@ProjectSize", If(String.IsNullOrWhiteSpace(projectSize), "Small", projectSize)},
                            {"@ProjectType", If(String.IsNullOrWhiteSpace(projectType), "New", projectType)},
                            {"@TaskId", task.TaskId},
                            {"@TaskName", task.TaskName},
                            {"@TaskOrder", task.TaskId},
                            {"@EmployeeId", employeeLookup(assignmentRow.EmployeeName)},
                            {"@WorkDate", assignmentRow.WorkDate},
                            {"@AssignedHours", assignmentRow.AssignedHours},
                            {"@StartDate", task.StartDate},
                            {"@FinishDate", task.FinishDate},
                            {"@DependencyTaskOrder", ParseFirstPredecessor(task.Predecessors)},
                            {"@DependencyType", If(String.IsNullOrWhiteSpace(task.DependencyType), "FS", task.DependencyType)},
                            {"@Remarks", DBNull.Value},
                            {"@UpdatedBy", Environment.UserName}
                        })
            Next
        Next

        If missingEmployees.Count > 0 Then
            Throw New InvalidOperationException("Employee IDs were not found in SQL for: " & String.Join(", ", missingEmployees.OrderBy(Function(name) name, StringComparer.OrdinalIgnoreCase)))
        End If
    End Sub

    Private Function BuildTaskAssignmentRows(task As ScheduleTask) As List(Of AssignmentPersistRow)
        Dim rows As New List(Of AssignmentPersistRow)()
        If task Is Nothing Then
            Return rows
        End If

        Dim daily = ParseDailyAllocations(task.DailyResourceAllocations)
        If daily.Count = 0 Then
            daily = BuildFallbackDailyAllocations(task)
        End If

        For Each item In daily
            Dim pieces = item.Key.Split({"|"c}, 2, StringSplitOptions.None)
            If pieces.Length <> 2 Then
                Continue For
            End If

            Dim workDate As Date
            If Not Date.TryParseExact(pieces(1), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, workDate) Then
                Continue For
            End If

            rows.Add(New AssignmentPersistRow With {
                .EmployeeName = pieces(0).Trim(),
                .WorkDate = workDate.Date,
                .AssignedHours = Math.Max(0D, item.Value)
            })
        Next

        Return rows.Where(Function(row) row.AssignedHours > 0D).ToList()
    End Function

    Private Function BuildFallbackDailyAllocations(task As ScheduleTask) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        Dim resourceAllocations = ParseResourceAllocations(task.ResourceAllocations)
        Dim dates = EnumerateAllDates(task.StartDate.Date, task.FinishDate.Date)
        If resourceAllocations.Count = 0 OrElse dates.Count = 0 Then
            Return result
        End If

        For Each resourceEntry In resourceAllocations
            Dim remaining = Math.Max(0D, resourceEntry.Value)
            Dim dailyShare = Math.Round(remaining / dates.Count, 2, MidpointRounding.AwayFromZero)
            For index = 0 To dates.Count - 1
                Dim workDate = dates(index)
                Dim hours = If(index = dates.Count - 1, remaining, dailyShare)
                hours = Math.Max(0D, Math.Round(hours, 2, MidpointRounding.AwayFromZero))
                If hours > 0D Then
                    result(resourceEntry.Key.Trim() & "|" & workDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) = hours
                End If
                remaining -= hours
            Next
        Next

        Return result
    End Function

    Private Function LoadEmployeeIdLookup(connection As IDbConnection) As Dictionary(Of String, Integer)
        Dim result As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        If Not TableExists(connection, "dbo.Employees") Then
            Return result
        End If

        Using command = connection.CreateCommand()
            command.CommandText =
                "IF COL_LENGTH('dbo.Employees', 'EmployeeName') IS NOT NULL " &
                "SELECT EmployeeId, LTRIM(RTRIM(EmployeeName)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(EmployeeName)), '') <> '' " &
                "ELSE IF COL_LENGTH('dbo.Employees', 'firstname') IS NOT NULL " &
                "SELECT EmployeeId, LTRIM(RTRIM(firstname)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(firstname)), '') <> '' " &
                "ELSE " &
                "SELECT CAST(NULL AS INT) AS EmployeeId, CAST(NULL AS NVARCHAR(200)) AS EmployeeName WHERE 1 = 0"

            Using reader = command.ExecuteReader()
                While reader.Read()
                    If reader("EmployeeId") Is DBNull.Value OrElse reader("EmployeeName") Is DBNull.Value Then
                        Continue While
                    End If

                    Dim employeeName = CStr(reader("EmployeeName")).Trim()
                    If employeeName.Length > 0 AndAlso Not result.ContainsKey(employeeName) Then
                        result(employeeName) = CInt(reader("EmployeeId"))
                    End If
                End While
            End Using
        End Using

        Return result
    End Function

    Private Function LoadEmployeeNameLookup(connection As IDbConnection) As Dictionary(Of Integer, String)
        Dim result As New Dictionary(Of Integer, String)()
        If Not TableExists(connection, "dbo.Employees") Then
            Return result
        End If

        Using command = connection.CreateCommand()
            command.CommandText =
                "IF COL_LENGTH('dbo.Employees', 'EmployeeName') IS NOT NULL " &
                "SELECT EmployeeId, LTRIM(RTRIM(EmployeeName)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(EmployeeName)), '') <> '' " &
                "ELSE IF COL_LENGTH('dbo.Employees', 'firstname') IS NOT NULL " &
                "SELECT EmployeeId, LTRIM(RTRIM(firstname)) AS EmployeeName FROM dbo.Employees WHERE ISNULL(LTRIM(RTRIM(firstname)), '') <> '' " &
                "ELSE " &
                "SELECT CAST(NULL AS INT) AS EmployeeId, CAST(NULL AS NVARCHAR(200)) AS EmployeeName WHERE 1 = 0"

            Using reader = command.ExecuteReader()
                While reader.Read()
                    If reader("EmployeeId") Is DBNull.Value OrElse reader("EmployeeName") Is DBNull.Value Then
                        Continue While
                    End If

                    Dim employeeId = CInt(reader("EmployeeId"))
                    If Not result.ContainsKey(employeeId) Then
                        result(employeeId) = CStr(reader("EmployeeName")).Trim()
                    End If
                End While
            End Using
        End Using

        Return result
    End Function

    Private Sub LoadAvailabilityFromDailyAvailability(connection As IDbConnection, target As Dictionary(Of String, Dictionary(Of Date, Decimal)), startDate As Date, finishDate As Date)
        Using command = connection.CreateCommand()
            command.CommandText =
                "IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL AND COL_LENGTH('dbo.Employees', 'EmployeeName') IS NOT NULL " &
                "SELECT a.EmployeeId, LTRIM(RTRIM(e.EmployeeName)) AS EmployeeName, a.WorkDate, a.AvailableHours FROM dbo.EmployeeDailyAvailability a INNER JOIN dbo.Employees e ON e.EmployeeId = a.EmployeeId WHERE a.WorkDate BETWEEN @StartDate AND @FinishDate " &
                "ELSE IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL AND COL_LENGTH('dbo.Employees', 'firstname') IS NOT NULL " &
                "SELECT a.EmployeeId, LTRIM(RTRIM(e.firstname)) AS EmployeeName, a.WorkDate, a.AvailableHours FROM dbo.EmployeeDailyAvailability a INNER JOIN dbo.Employees e ON e.EmployeeId = a.EmployeeId WHERE a.WorkDate BETWEEN @StartDate AND @FinishDate " &
                "ELSE " &
                "SELECT CAST(NULL AS INT) AS EmployeeId, CAST(NULL AS NVARCHAR(200)) AS EmployeeName, CAST(NULL AS DATE) AS WorkDate, CAST(NULL AS DECIMAL(12,2)) AS AvailableHours WHERE 1 = 0"
            AddParameter(command, "@StartDate", startDate.Date)
            AddParameter(command, "@FinishDate", finishDate.Date)

            Using reader = command.ExecuteReader()
                While reader.Read()
                    If reader("EmployeeName") Is DBNull.Value OrElse reader("WorkDate") Is DBNull.Value Then
                        Continue While
                    End If

                    Dim resourceName = CStr(reader("EmployeeName")).Trim()
                    If resourceName.Length = 0 Then
                        Continue While
                    End If

                    Dim workDate = CDate(reader("WorkDate")).Date
                    Dim hours = If(reader("AvailableHours") Is DBNull.Value, 8D, CDec(reader("AvailableHours")))

                    Dim byDate As Dictionary(Of Date, Decimal) = Nothing
                    If Not target.TryGetValue(resourceName, byDate) Then
                        byDate = New Dictionary(Of Date, Decimal)()
                        target(resourceName) = byDate
                    End If

                    byDate(workDate) = Math.Max(0D, hours)
                End While
            End Using
        End Using
    End Sub

    Private Function TableExists(connection As IDbConnection, tableName As String) As Boolean
        Using command = connection.CreateCommand()
            command.CommandText = "SELECT CASE WHEN OBJECT_ID('" & tableName & "', 'U') IS NULL THEN 0 ELSE 1 END"
            Return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) = 1
        End Using
    End Function

    Private Function TableHasRows(connection As IDbConnection, tableName As String) As Boolean
        If Not TableExists(connection, tableName) Then
            Return False
        End If

        Using command = connection.CreateCommand()
            command.CommandText = "SELECT TOP 1 1 FROM " & tableName
            Dim value = command.ExecuteScalar()
            Return value IsNot Nothing AndAlso value IsNot DBNull.Value
        End Using
    End Function

    Private Shared Function ParseResourceAllocations(value As String) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        If String.IsNullOrWhiteSpace(value) Then
            Return result
        End If

        For Each part In value.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Split({"="c}, 2, StringSplitOptions.None)
            If pieces.Length <> 2 Then
                Continue For
            End If

            Dim hours As Decimal
            If Decimal.TryParse(pieces(1).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, hours) OrElse
                Decimal.TryParse(pieces(1).Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, hours) Then
                Dim name = pieces(0).Trim()
                If name.Length > 0 Then
                    result(name) = Math.Max(0D, hours)
                End If
            End If
        Next

        Return result
    End Function

    Private Shared Function ParseDailyAllocations(value As String) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        If String.IsNullOrWhiteSpace(value) Then
            Return result
        End If

        For Each part In value.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Split({"="c}, 2, StringSplitOptions.None)
            If pieces.Length <> 2 Then
                Continue For
            End If

            Dim hours As Decimal
            If Decimal.TryParse(pieces(1).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, hours) OrElse
                Decimal.TryParse(pieces(1).Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, hours) Then
                Dim key = pieces(0).Trim()
                If key.Length > 0 Then
                    result(key) = Math.Max(0D, hours)
                End If
            End If
        Next

        Return result
    End Function

    Private Shared Function BuildResourceAllocationsString(resourceHours As Dictionary(Of String, Decimal)) As String
        Return String.Join("; ", resourceHours.
            Where(Function(item) Not String.IsNullOrWhiteSpace(item.Key) AndAlso item.Value > 0D).
            Select(Function(item) item.Key.Trim() & "=" & item.Value.ToString("0.##", CultureInfo.InvariantCulture)))
    End Function

    Private Shared Function BuildDailyAllocationsString(daily As Dictionary(Of String, Decimal)) As String
        Return String.Join("; ", daily.
            Where(Function(item) Not String.IsNullOrWhiteSpace(item.Key) AndAlso item.Value > 0D).
            Select(Function(item) item.Key.Trim() & "=" & item.Value.ToString("0.##", CultureInfo.InvariantCulture)))
    End Function

    Private Shared Function EnumerateAllDates(startDate As Date, finishDate As Date) As List(Of Date)
        Dim dates As New List(Of Date)()
        Dim currentDate = startDate.Date
        Dim lastDate = If(finishDate.Date < currentDate, currentDate, finishDate.Date)
        While currentDate <= lastDate
            dates.Add(currentDate)
            currentDate = currentDate.AddDays(1)
        End While
        Return dates
    End Function

    Private Shared Function ParseFirstPredecessor(predecessors As String) As Object
        If String.IsNullOrWhiteSpace(predecessors) Then
            Return DBNull.Value
        End If

        Dim firstValue = predecessors.Split({","c}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
        Dim predecessorNumber As Integer
        If firstValue IsNot Nothing AndAlso Integer.TryParse(firstValue.Trim(), predecessorNumber) Then
            Return predecessorNumber
        End If

        Return DBNull.Value
    End Function

    Private Shared Function MakeSafeCode(value As String) As String
        Dim characters = value.ToUpperInvariant().Select(Function(ch) If(Char.IsLetterOrDigit(ch), ch, "_"c)).ToArray()
        Return New String(characters)
    End Function

    Private Sub Execute(connection As IDbConnection, sql As String, parameters As Dictionary(Of String, Object))
        Using command = connection.CreateCommand()
            command.CommandText = sql
            If parameters IsNot Nothing Then
                For Each item In parameters
                    AddParameter(command, item.Key, item.Value)
                Next
            End If
            command.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub AddParameter(command As IDbCommand, name As String, value As Object)
        Dim parameter = command.CreateParameter()
        parameter.ParameterName = name
        parameter.Value = If(value, DBNull.Value)
        command.Parameters.Add(parameter)
    End Sub

    Private Shared Function ResolveProjectType(savedType As String, projectName As String) As String
        If Not String.IsNullOrWhiteSpace(savedType) Then
            Return savedType.Trim()
        End If

        Dim normalizedName = If(projectName, "")
        If normalizedName.IndexOf("feedback", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Return "Feedback"
        End If
        If normalizedName.IndexOf("update", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            normalizedName.IndexOf("bre", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            normalizedName.IndexOf("rol", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Return "Update"
        End If

        Return "New"
    End Function
End Class

Friend Class AssignmentPersistRow
    Public Property EmployeeName As String = ""
    Public Property WorkDate As Date
    Public Property AssignedHours As Decimal
End Class

Friend Class AssignmentSnapshotRow
    Public Property ProjectId As Integer
    Public Property ProjectName As String = ""
    Public Property VersionNumber As String = "1.0"
    Public Property ProjectSize As String = "Small"
    Public Property ProjectType As String = "New"
    Public Property TaskId As Integer
    Public Property TaskName As String = ""
    Public Property TaskOrder As Integer
    Public Property EmployeeId As Integer
    Public Property WorkDate As Date
    Public Property AssignedHours As Decimal
    Public Property StartDate As Date?
    Public Property FinishDate As Date?
    Public Property DependencyTaskOrder As Integer?
    Public Property DependencyType As String = "FS"
    Public Property UpdatedOn As Date
End Class
