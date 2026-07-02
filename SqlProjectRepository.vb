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
        End Using
    End Sub

    Public Function ListProjects(Optional maxResults As Integer = 50) As List(Of ProjectLibraryItem)
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
