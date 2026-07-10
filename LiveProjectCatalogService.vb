Imports System.Globalization

Public Class LiveProjectCatalogService
    Private ReadOnly _sqlRepository As SqlProjectRepository
    Private ReadOnly _projects As List(Of LiveProjectItem)

    Public Sub New(Optional sqlRepository As SqlProjectRepository = Nothing)
        _sqlRepository = sqlRepository
        ' SQL connection will replace this seed list later. The form and scheduler already use this service boundary.
        _projects = New List(Of LiveProjectItem) From {
            CreateSmaBreProjectTemplate(),
            CreateSmaRolProjectTemplate(),
            CreateSmaWithinProjectTemplate(),
            New LiveProjectItem With {.ProjectCode = "TPL-BRE-ROL", .ProjectName = "SMA BRE/ROL Update", .ClientName = "Template", .VersionNumber = "1.0", .ProjectSize = "Small", .TemplateName = "BRE/ROL Update template", .ProjectType = "Update"},
            New LiveProjectItem With {.ProjectCode = "TPL-BRE-WITHIN", .ProjectName = "SMA BRE Within Update", .ClientName = "Template", .VersionNumber = "1.0", .ProjectSize = "Small", .TemplateName = "BRE Within Update", .ProjectType = "Update"},
            New LiveProjectItem With {.ProjectCode = "TPL-FEEDBACK", .ProjectName = "SMA Feedback update", .ClientName = "Template", .VersionNumber = "1.0", .ProjectSize = "Small", .TemplateName = "Feedback Change", .ProjectType = "Feedback"}
        }
    End Sub

    Public Shared Function CreateSmaNewProjectTemplate() As LiveProjectItem
        Return CreateSmaBreProjectTemplate()
    End Function

    Public Shared Function CreateSmaBreProjectTemplate() As LiveProjectItem
        Return New LiveProjectItem With {
            .ProjectCode = "TPL-NEW-BRE",
            .ProjectName = "SMA - BRE Project",
            .ClientName = "Template",
            .VersionNumber = "1.0",
            .ProjectSize = "Small",
            .TemplateName = "SMA New Project",
            .ProjectType = "New"
        }
    End Function

    Public Shared Function CreateSmaRolProjectTemplate() As LiveProjectItem
        Return New LiveProjectItem With {
            .ProjectCode = "TPL-NEW-ROL",
            .ProjectName = "SMA - ROL Project",
            .ClientName = "Template",
            .VersionNumber = "1.0",
            .ProjectSize = "Small",
            .TemplateName = "SMA New Project",
            .ProjectType = "New"
        }
    End Function

    Public Shared Function CreateSmaWithinProjectTemplate() As LiveProjectItem
        Return New LiveProjectItem With {
            .ProjectCode = "TPL-NEW-WITHIN",
            .ProjectName = "SMA - Within Project",
            .ClientName = "Template",
            .VersionNumber = "1.0",
            .ProjectSize = "Small",
            .TemplateName = "SMA New Project",
            .ProjectType = "New"
        }
    End Function

    Public Function SearchProjects(searchText As String) As List(Of LiveProjectItem)
        Dim query = If(searchText, "").Trim()
        Dim results As New List(Of LiveProjectItem)()

        If _sqlRepository IsNot Nothing Then
            Try
                results.AddRange(_sqlRepository.LoadTemplateProjects(query))
            Catch
            End Try
        End If

        Dim matches = _projects.AsEnumerable()

        If query.Length > 0 Then
            matches = matches.Where(Function(project)
                                        Return project.ProjectCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ProjectName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ClientName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ProjectSize.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.TemplateName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                                    End Function)
        End If

        results.AddRange(matches)

        Return results.
            GroupBy(Function(project) project.ProjectName, StringComparer.OrdinalIgnoreCase).
            Select(Function(group) group.First()).
            OrderBy(Function(project) project.ProjectName).
            ToList()
    End Function

    Public Function GetDefaultNewProjectTemplate() As LiveProjectItem
        Dim availableProjects = SearchProjects("")
        Dim preferred = availableProjects.FirstOrDefault(
            Function(project) project.ProjectType.Equals("New", StringComparison.OrdinalIgnoreCase) AndAlso
                (project.ProjectName.IndexOf("new", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                 project.TemplateName.IndexOf("new", StringComparison.OrdinalIgnoreCase) >= 0))

        If preferred IsNot Nothing Then
            Return preferred
        End If

        preferred = availableProjects.FirstOrDefault(Function(project) project.ProjectType.Equals("New", StringComparison.OrdinalIgnoreCase))
        If preferred IsNot Nothing Then
            Return preferred
        End If

        Return CreateSmaNewProjectTemplate()
    End Function
End Class

Public Class LiveProjectItem
    Public Property ProjectCode As String = ""
    Public Property ProjectName As String = ""
    Public Property ClientName As String = ""
    Public Property VersionNumber As String = "1.0"
    Public Property ProjectSize As String = "Small"
    Public Property TemplateName As String = "New Project"
    Public Property ProjectType As String = "New"
    Public Property SavedProjectId As Integer
    Public Property SourceFilePath As String = ""
    Public Property ReportType As String = ""
    Public Property TaskReportFilter As String = ""
    Public Property ProjectDetailsText As String = ""
    Public Property FinalCompletionDate As Date?
    Public Property PlanningMessage As String = ""
    Public Property ControllerAtRolc As String = ""
    Public Property ClientType As String = ""
    Public Property IsPointcloud As Boolean
    Public Property TechPack As Boolean
    Public Property DeedProfile As Boolean
    Public Property ShadowAnalysis As Boolean
    Public Property UrgentSmallProjects As Boolean

    Public ReadOnly Property IsStoredProject As Boolean
        Get
            Return SavedProjectId > 0 OrElse Not String.IsNullOrWhiteSpace(SourceFilePath)
        End Get
    End Property

    Public ReadOnly Property DisplayText As String
        Get
            Return ProjectName
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return DisplayText
    End Function
End Class
