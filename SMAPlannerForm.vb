Imports System.ComponentModel
Imports System.Configuration
Imports System.Globalization
Imports System.ComponentModel.Design

Public Class SMAPlannerForm
    Inherits Form

    Private ReadOnly _projectLibrary As New ProjectLibraryService()
    Private ReadOnly _sqlRepository As SqlProjectRepository = CreateSqlRepository()
    Private ReadOnly _liveProjectCatalog As LiveProjectCatalogService
    Private ReadOnly _projects As New BindingList(Of ProjectLibraryItem)()
    Private ReadOnly _liveProjects As New BindingList(Of LiveProjectItem)()
    Private ReadOnly _searchProjectMatches As New List(Of LiveProjectItem)()
    Private _selectedSearchProject As LiveProjectItem
    Private _currentTheme As SchedulerThemePalette = SchedulerThemePalette.ThemeByName("Dusk")
    Private _isUpdatingSearchText As Boolean

    Public Sub New()
        _liveProjectCatalog = New LiveProjectCatalogService(_sqlRepository)
        InitializeComponent()
        ConfigurePlannerForm()
        If IsInDesignerHost() Then
            SeedPlannerDesignerData()
        Else
            LoadProjectList()
            LoadLiveProjectList()
        End If
        ApplyCurrentTheme()
    End Sub

    Private Sub ConfigurePlannerForm()
        _liveProjectSearchBox.AutoCompleteMode = AutoCompleteMode.Suggest
        _liveProjectSearchBox.AutoCompleteSource = AutoCompleteSource.CustomSource

        _liveProjectSelector.DisplayMember = NameOf(LiveProjectItem.DisplayText)
        _liveProjectSelector.DataSource = _liveProjects

        _grid.AutoGenerateColumns = False
        _grid.Columns.Clear()
        AddProjectGridColumns()
        _grid.DataSource = _projects
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        _grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        _grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)

        AddHandler btnNewProject.Click, AddressOf OpenNewProject
        AddHandler btnRefreshList.Click, Sub() RefreshPlannerLists()
        AddHandler btnScheduleProject.Click, AddressOf OpenSelectedLiveProjectTemplate
        AddHandler btnRescheduleProject.Click, AddressOf OpenSelectedSearchProject
        AddHandler _liveProjectSearchBox.TextChanged, AddressOf LiveProjectSearchTextChanged
        AddHandler _liveProjectSearchBox.KeyDown, AddressOf LiveProjectSearchKeyDown
        AddHandler _liveProjectSelector.SelectedIndexChanged, AddressOf LiveProjectSelectionChanged
        AddHandler _grid.CellDoubleClick, AddressOf OpenSelectedExistingProject
        AddHandler Activated, AddressOf PlannerActivated
    End Sub

    Private Sub AddProjectGridColumns()
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.ProjectId), .HeaderText = "ProjectId", .Width = 96, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.ProjectName), .HeaderText = "ProjectName", .Width = 180, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.VersionNumber), .HeaderText = "VersionNumber", .Width = 108, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.ProjectSize), .HeaderText = "ProjectSize", .Width = 108, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.ProjectType), .HeaderText = "ProjectType", .Width = 110, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.TaskCount), .HeaderText = "TaskCount", .Width = 94, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.ResourceHours), .HeaderText = "ResourceHours", .Width = 116, .ReadOnly = True, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "0.##"}})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.StartDate), .HeaderText = "StartDate", .Width = 108, .ReadOnly = True, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "dd-MMM-yyyy"}})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ProjectLibraryItem.FinishDate), .HeaderText = "FinishDate", .Width = 108, .ReadOnly = True, .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "dd-MMM-yyyy"}})
    End Sub

    Private Shared Function CreateSqlRepository() As SqlProjectRepository
        Dim connectionString = ""

        Try
            Dim settings = ConfigurationManager.ConnectionStrings("SmaSchedulerDb")
            If settings IsNot Nothing Then
                connectionString = settings.ConnectionString
            End If
        Catch ex As ConfigurationErrorsException
        End Try

        If String.IsNullOrWhiteSpace(connectionString) Then
            connectionString = Environment.GetEnvironmentVariable("SMA_SCHEDULER_SQL_CONNECTION")
        End If

        If String.IsNullOrWhiteSpace(connectionString) Then
            Return Nothing
        End If

        Return New SqlProjectRepository(connectionString)
    End Function

    Private Sub PlannerActivated(sender As Object, e As EventArgs)
        ApplyCurrentTheme()
    End Sub

    Private Sub ApplyCurrentTheme()
        _currentTheme = DeterminePlannerTheme()
        Dim theme = _currentTheme

        If theme Is Nothing OrElse IsDisposed Then
            Return
        End If

        BackColor = theme.WindowBack

        If ControlReady(headerPanel) Then
            headerPanel.BackColor = theme.HeaderBack
        End If

        If ControlReady(gridPanel) Then
            gridPanel.BackColor = theme.PanelBack
        End If

        If ControlReady(planningSummaryPanel) Then
            planningSummaryPanel.BackColor = theme.PanelBack
        End If

        For Each label In New Label() {titleLabel, listTitle, summaryTitleLabel, _liveProjectSizeLabel, projectTypeValueLabel}
            If ControlReady(label) Then
                label.ForeColor = theme.Text
            End If
        Next

        If ControlReady(_status) Then
            _status.ForeColor = theme.MutedText
        End If

        For Each label In New Label() {searchLabel, selectorLabel, summaryPeriodLabel, projectTypeCaptionLabel}
            If ControlReady(label) Then
                label.ForeColor = theme.MutedText
            End If
        Next

        If ControlReady(btnScheduleProject) Then
            btnScheduleProject.BackColor = theme.Action
        End If

        If ControlReady(btnRescheduleProject) Then
            btnRescheduleProject.BackColor = theme.CommandBack
        End If

        For Each button In New Button() {btnScheduleProject, btnRescheduleProject}
            If ControlReady(button) Then
                button.ForeColor = Color.White
            End If
        Next

        If ControlReady(newProjectsPanel) Then
            newProjectsPanel.BackColor = theme.TileOne
        End If

        If ControlReady(updateProjectsPanel) Then
            updateProjectsPanel.BackColor = theme.TileThree
        End If

        If ControlReady(feedbackProjectsPanel) Then
            feedbackProjectsPanel.BackColor = theme.TileFour
        End If

        For Each label In New Label() {newProjectsLabel, newProjectsCountLabel, updateProjectsLabel, updateProjectsCountLabel, feedbackProjectsLabel, feedbackProjectsCountLabel}
            If ControlReady(label) Then
                label.ForeColor = theme.Text
            End If
        Next

        If _grid IsNot Nothing AndAlso Not _grid.IsDisposed Then
            Dim headerStyle = If(_grid.ColumnHeadersDefaultCellStyle, New DataGridViewCellStyle())
            Dim defaultStyle = If(_grid.DefaultCellStyle, New DataGridViewCellStyle())
            Dim alternatingStyle = If(_grid.AlternatingRowsDefaultCellStyle, New DataGridViewCellStyle())

            _grid.BackgroundColor = theme.PanelBack
            _grid.GridColor = theme.GridLine

            headerStyle.BackColor = theme.GridHeader
            headerStyle.ForeColor = Color.White
            _grid.ColumnHeadersDefaultCellStyle = headerStyle

            defaultStyle.BackColor = theme.PanelBack
            defaultStyle.ForeColor = theme.Text
            defaultStyle.SelectionBackColor = theme.Selection
            defaultStyle.SelectionForeColor = theme.Text
            _grid.DefaultCellStyle = defaultStyle

            alternatingStyle.BackColor = theme.AlternatingRow
            _grid.AlternatingRowsDefaultCellStyle = alternatingStyle
            _grid.EnableHeadersVisualStyles = False
        End If
    End Sub

    Private Function DeterminePlannerTheme() As SchedulerThemePalette
        Return SchedulerThemePalette.ThemeByName("Dusk")
    End Function

    Private Shared Function ControlReady(control As Control) As Boolean
        Return control IsNot Nothing AndAlso Not control.IsDisposed
    End Function

    Private Sub LoadLiveProjectList()
        Dim selectedCode = SelectedLiveProject()?.ProjectCode
        Dim matches = _liveProjectCatalog.SearchProjects("").
            Where(Function(project) Not project.IsStoredProject).
            GroupBy(Function(project) project.ProjectCode, StringComparer.OrdinalIgnoreCase).
            Select(Function(group) group.First()).
            OrderBy(Function(project) project.ProjectName, StringComparer.OrdinalIgnoreCase).
            ToList()

        _liveProjectSelector.BeginUpdate()
        _liveProjectSelector.DataSource = Nothing
        _liveProjects.Clear()
        For Each project In matches
            _liveProjects.Add(project)
        Next

        _liveProjectSelector.DisplayMember = NameOf(LiveProjectItem.DisplayText)
        _liveProjectSelector.DataSource = _liveProjects

        If _liveProjectSelector.Items.Count > 0 Then
            Dim restoreIndex = -1
            For i = 0 To _liveProjects.Count - 1
                Dim liveProject = _liveProjects(i)
                If Not String.IsNullOrWhiteSpace(selectedCode) AndAlso String.Equals(liveProject.ProjectCode, selectedCode, StringComparison.OrdinalIgnoreCase) Then
                    restoreIndex = i
                    Exit For
                End If
            Next
            _liveProjectSelector.SelectedIndex = If(restoreIndex >= 0, restoreIndex, 0)
        Else
            _liveProjectSelector.SelectedIndex = -1
        End If
        _liveProjectSelector.EndUpdate()

        UpdateLiveProjectSizeLabel()
    End Sub

    Private Sub LiveProjectSearchTextChanged(sender As Object, e As EventArgs)
        If _isUpdatingSearchText Then
            Return
        End If
        RefreshSearchProjectSuggestions()
        UpdateLiveProjectSizeLabel()
    End Sub

    Private Sub RefreshSearchProjectSuggestions()
        Dim query = If(_liveProjectSearchBox.Text, "").Trim()
        Dim previousCode = If(_selectedSearchProject Is Nothing, "", _selectedSearchProject.ProjectCode)
        Dim autoComplete As New AutoCompleteStringCollection()

        _searchProjectMatches.Clear()
        For Each project In SearchStoredProjects(query).
            GroupBy(Function(item) item.ProjectCode, StringComparer.OrdinalIgnoreCase).
            Select(Function(group) group.First()).
            OrderBy(Function(item) item.ProjectName, StringComparer.OrdinalIgnoreCase)
            _searchProjectMatches.Add(project)
            autoComplete.Add(project.ProjectName)
            If Not String.IsNullOrWhiteSpace(project.ProjectCode) Then
                autoComplete.Add(project.ProjectCode)
            End If
        Next

        _liveProjectSearchBox.AutoCompleteCustomSource = autoComplete

        If query.Length = 0 Then
            _selectedSearchProject = Nothing
            Return
        End If

        _selectedSearchProject = _searchProjectMatches.FirstOrDefault(
            Function(project) Not String.IsNullOrWhiteSpace(previousCode) AndAlso
                String.Equals(project.ProjectCode, previousCode, StringComparison.OrdinalIgnoreCase))

        If _selectedSearchProject Is Nothing Then
            _selectedSearchProject = ResolveSearchProject(False)
        End If
    End Sub

    Private Function SearchStoredProjects(searchText As String) As IEnumerable(Of LiveProjectItem)
        Dim query = If(searchText, "").Trim()
        Dim matches = _projects.AsEnumerable()

        If query.Length > 0 Then
            matches = matches.Where(Function(project)
                                        Return project.ProjectId.ToString(CultureInfo.InvariantCulture).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ProjectName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ProjectSize.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                            project.ProjectType.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                                    End Function)
        End If

        Return matches.Select(Function(project) CreateStoredProjectOption(project))
    End Function

    Private Shared Function CreateStoredProjectOption(project As ProjectLibraryItem) As LiveProjectItem
        Return New LiveProjectItem With {
            .ProjectCode = "SQL-" & project.ProjectId.ToString(CultureInfo.InvariantCulture),
            .ProjectName = project.ProjectName,
            .ClientName = "SQL",
            .VersionNumber = If(String.IsNullOrWhiteSpace(project.VersionNumber), "1.0", project.VersionNumber),
            .ProjectSize = If(String.IsNullOrWhiteSpace(project.ProjectSize), "Small", project.ProjectSize),
            .TemplateName = project.ProjectName,
            .ProjectType = If(String.IsNullOrWhiteSpace(project.ProjectType), "New", project.ProjectType),
            .SavedProjectId = project.ProjectId,
            .SourceFilePath = project.FilePath,
            .ReportType = If(String.IsNullOrWhiteSpace(project.ProjectType), "New", project.ProjectType)
        }
    End Function

    Private Sub LoadProjectList()
        _projects.Clear()
        Dim loadedFromSql = False

        If _sqlRepository IsNot Nothing Then
            Try
                For Each project In _sqlRepository.ListProjects()
                    _projects.Add(project)
                Next
                loadedFromSql = True
            Catch ex As Exception
                SetPlannerStatus("SQL project list could not be loaded. Showing local backups.")
            End Try
        End If

        If Not loadedFromSql Then
            For Each project In _projectLibrary.ListProjects()
                _projects.Add(project)
            Next
        End If

        If loadedFromSql Then
            _status.Text = _projects.Count.ToString(CultureInfo.InvariantCulture) & " recent scheduled project(s) loaded from SQL. Double-click a project to update its schedule."
        Else
            _status.Text = _projects.Count.ToString(CultureInfo.InvariantCulture) & " planned project(s). Double-click a project to update its schedule."
        End If
        RefreshSearchProjectSuggestions()
        UpdatePlanningSummary()
    End Sub

    Private Sub RefreshPlannerLists()
        LoadProjectList()
        LoadLiveProjectList()
    End Sub

    Private Sub UpdatePlanningSummary()
        Dim periodStart = New Date(Date.Today.Year, Date.Today.Month, 20)
        If Date.Today.Day < 20 Then
            periodStart = periodStart.AddMonths(-1)
        End If
        Dim periodEnd = periodStart.AddMonths(1)

        summaryPeriodLabel.Text = periodStart.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) &
            " to " & periodEnd.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)

        Dim periodProjects = _projects.
            Where(Function(project) project.StartDate.HasValue AndAlso
                project.StartDate.Value.Date >= periodStart AndAlso
                project.StartDate.Value.Date <= periodEnd).
            ToList()

        newProjectsCountLabel.Text = CountProjects(periodProjects, "New").ToString(CultureInfo.InvariantCulture)
        updateProjectsCountLabel.Text = CountProjects(periodProjects, "Update").ToString(CultureInfo.InvariantCulture)
        feedbackProjectsCountLabel.Text = CountProjects(periodProjects, "Feedback").ToString(CultureInfo.InvariantCulture)
    End Sub

    Private Shared Function CountProjects(projects As IEnumerable(Of ProjectLibraryItem), projectType As String) As Integer
        Return projects.Count(Function(project) String.Equals(project.ProjectType, projectType, StringComparison.OrdinalIgnoreCase))
    End Function

    Private Function SelectedLiveProject() As LiveProjectItem
        Return TryCast(_liveProjectSelector.SelectedItem, LiveProjectItem)
    End Function

    Private Function ResolveSearchProject(preferFirstMatch As Boolean) As LiveProjectItem
        Dim query = If(_liveProjectSearchBox.Text, "").Trim()
        If query.Length = 0 Then
            Return Nothing
        End If

        Dim exactMatch = _searchProjectMatches.FirstOrDefault(
            Function(project)
                Return String.Equals(project.ProjectName, query, StringComparison.OrdinalIgnoreCase) OrElse
                    String.Equals(project.ProjectCode, query, StringComparison.OrdinalIgnoreCase)
            End Function)
        If exactMatch IsNot Nothing Then
            Return exactMatch
        End If

        If preferFirstMatch AndAlso _searchProjectMatches.Count > 0 Then
            Return _searchProjectMatches(0)
        End If

        Return Nothing
    End Function

    Private Sub LiveProjectSelectionChanged(sender As Object, e As EventArgs)
        UpdateLiveProjectSizeLabel()
    End Sub

    Private Sub UpdateLiveProjectSizeLabel()
        Dim selectedTemplate = SelectedLiveProject()
        Dim selectedProject = ResolveSearchProject(False)

        If selectedTemplate Is Nothing AndAlso selectedProject Is Nothing Then
            _liveProjectSizeLabel.Text = "Project size: Small"
            projectTypeValueLabel.Text = "New"
            Return
        End If

        Dim sizeText = FirstNonBlank(
            If(selectedProject Is Nothing, "", selectedProject.ProjectSize),
            If(selectedTemplate Is Nothing, "", selectedTemplate.ProjectSize),
            "Small")
        Dim typeText = FirstNonBlank(
            If(selectedTemplate Is Nothing, "", selectedTemplate.ProjectType),
            If(selectedProject Is Nothing, "", selectedProject.ProjectType),
            "New")

        _liveProjectSizeLabel.Text = "Project size: " & sizeText
        projectTypeValueLabel.Text = typeText
    End Sub

    Private Sub LiveProjectSearchKeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Back Then
            HandleSearchBoxBackspace(e)
            Return
        End If

        If e.KeyCode <> Keys.Enter Then
            Return
        End If

        e.Handled = True
        e.SuppressKeyPress = True

        Dim matchedProject = ResolveSearchProject(True)
        If matchedProject IsNot Nothing Then
            _selectedSearchProject = matchedProject
            ReplaceSearchText(matchedProject.ProjectName)
        End If

        UpdateLiveProjectSizeLabel()
    End Sub

    Private Sub HandleSearchBoxBackspace(e As KeyEventArgs)
        If _isUpdatingSearchText OrElse _liveProjectSearchBox.SelectionLength <= 0 Then
            Return
        End If

        Dim selectionStart = _liveProjectSearchBox.SelectionStart
        Dim currentText = _liveProjectSearchBox.Text
        Dim updatedText = If(selectionStart <= 0, "", currentText.Substring(0, selectionStart - 1))
        ReplaceSearchText(updatedText)
        e.Handled = True
        e.SuppressKeyPress = True
    End Sub

    Private Sub ReplaceSearchText(value As String)
        _isUpdatingSearchText = True
        Try
            _liveProjectSearchBox.Text = value
            _liveProjectSearchBox.SelectionStart = _liveProjectSearchBox.TextLength
            _liveProjectSearchBox.SelectionLength = 0
        Finally
            _isUpdatingSearchText = False
        End Try
    End Sub

    Private Sub OpenNewProject(sender As Object, e As EventArgs)
        Using scheduler As New SMASchedulerForm()
            scheduler.LoadLiveProjectTemplate(_liveProjectCatalog.GetDefaultNewProjectTemplate())
            FormTransitionService.ShowDialogWithMotion(Me, scheduler)
        End Using
        ApplyCurrentTheme()
        RefreshPlannerLists()
    End Sub

    Private Sub OpenSelectedSearchProject(sender As Object, e As EventArgs)
        Dim matchedProject = ResolveSearchProject(True)
        If matchedProject Is Nothing OrElse matchedProject.SavedProjectId <= 0 Then
            MessageBox.Show(Me, "Select a scheduled project from Search Projects first.", "Reschedule Project", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ReplaceSearchText(matchedProject.ProjectName)
        OpenStoredProject(matchedProject.SavedProjectId)
    End Sub

    Private Sub OpenSelectedLiveProjectTemplate(sender As Object, e As EventArgs)
        Dim selectedTemplate = SelectedLiveProject()
        If selectedTemplate Is Nothing Then
            MessageBox.Show(Me, "No template is selected.", "Schedule Project", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedProject = ResolveSearchProject(True)
        Dim projectName = If(selectedProject Is Nothing, _liveProjectSearchBox.Text.Trim(), selectedProject.ProjectName)

        If String.IsNullOrWhiteSpace(projectName) Then
            MessageBox.Show(Me, "Type or select a live project first.", "Schedule Project", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim projectToSchedule As New LiveProjectItem With {
            .ProjectCode = FirstNonBlank(If(selectedProject Is Nothing, "", selectedProject.ProjectCode), selectedTemplate.ProjectCode, projectName),
            .ProjectName = projectName,
            .ClientName = FirstNonBlank(If(selectedProject Is Nothing, "", selectedProject.ClientName), selectedTemplate.ClientName, "SQL"),
            .VersionNumber = FirstNonBlank(If(selectedProject Is Nothing, "", selectedProject.VersionNumber), selectedTemplate.VersionNumber, "1.0"),
            .ProjectSize = FirstNonBlank(If(selectedProject Is Nothing, "", selectedProject.ProjectSize), selectedTemplate.ProjectSize, "Small"),
            .TemplateName = FirstNonBlank(selectedTemplate.TemplateName, selectedTemplate.ProjectName, "SMA New Project"),
            .ProjectType = FirstNonBlank(selectedTemplate.ProjectType, If(selectedProject Is Nothing, "", selectedProject.ProjectType), "New"),
            .SavedProjectId = If(selectedProject Is Nothing, 0, selectedProject.SavedProjectId),
            .SourceFilePath = If(selectedProject Is Nothing, "", selectedProject.SourceFilePath),
            .ReportType = FirstNonBlank(selectedTemplate.ReportType, selectedTemplate.ProjectType, "New")
        }

        Using scheduler As New SMASchedulerForm()
            scheduler.LoadLiveProjectTemplate(projectToSchedule)
            FormTransitionService.ShowDialogWithMotion(Me, scheduler)
        End Using
        ApplyCurrentTheme()
        RefreshPlannerLists()
    End Sub

    Private Shared Function FirstNonBlank(ParamArray values() As String) As String
        For Each value In values
            If Not String.IsNullOrWhiteSpace(value) Then
                Return value.Trim()
            End If
        Next

        Return ""
    End Function

    Private Sub OpenSelectedExistingProject(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        Dim item = TryCast(_grid.Rows(e.RowIndex).DataBoundItem, ProjectLibraryItem)
        If item Is Nothing Then
            Return
        End If

        OpenStoredProject(item.ProjectId)
    End Sub

    Private Sub OpenStoredProject(projectId As Integer)
        If projectId <= 0 Then
            MessageBox.Show(Me, "This scheduled project does not have a saved identifier yet.", "Reschedule Project", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim item = _projects.FirstOrDefault(Function(project) project.ProjectId = projectId)
        If item Is Nothing Then
            MessageBox.Show(Me, "This scheduled project could not be found.", "Reschedule Project", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim snapshot As ProjectSnapshot = Nothing

        If _sqlRepository IsNot Nothing AndAlso item.ProjectId > 0 Then
            Try
                snapshot = _sqlRepository.LoadProjectSnapshot(item.ProjectId)
            Catch ex As Exception
                SetPlannerStatus("SQL project load failed. Trying local backup.")
            End Try
        End If

        If snapshot Is Nothing AndAlso Not String.IsNullOrWhiteSpace(item.FilePath) Then
            snapshot = _projectLibrary.LoadSnapshot(item.FilePath)
        End If

        If snapshot Is Nothing Then
            MessageBox.Show(Me, "This planned project could not be opened.", "Open Project", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            LoadProjectList()
            Return
        End If

        Using scheduler As New SMASchedulerForm()
            scheduler.LoadProjectSnapshot(snapshot)
            FormTransitionService.ShowDialogWithMotion(Me, scheduler)
        End Using
        ApplyCurrentTheme()
        RefreshPlannerLists()
    End Sub

    Private Sub SetPlannerStatus(message As String)
        If String.IsNullOrWhiteSpace(message) Then
            Return
        End If

        _status.Text = message
    End Sub

    Private Shared Function IsInDesignerHost() As Boolean
        Return LicenseManager.UsageMode = LicenseUsageMode.Designtime
    End Function

    Private Sub SeedPlannerDesignerData()
        _projects.Clear()
        _projects.Add(New ProjectLibraryItem With {
            .ProjectId = 1201,
            .ProjectName = "SMA BRE Project",
            .VersionNumber = "1.0",
            .ProjectSize = "Small",
            .ProjectType = "New",
            .TaskCount = 15,
            .ResourceHours = 42D,
            .StartDate = New Date(2026, 6, 24),
            .FinishDate = New Date(2026, 7, 2)
        })
        _projects.Add(New ProjectLibraryItem With {
            .ProjectId = 1202,
            .ProjectName = "SMA Within Project",
            .VersionNumber = "1.1",
            .ProjectSize = "Medium",
            .ProjectType = "Update",
            .TaskCount = 9,
            .ResourceHours = 28D,
            .StartDate = New Date(2026, 6, 27),
            .FinishDate = New Date(2026, 7, 4)
        })

        _liveProjects.Clear()
        _liveProjects.Add(New LiveProjectItem With {.ProjectCode = "TPL-BRE", .ProjectName = "SMA - BRE Project", .TemplateName = "SMA - BRE Project", .ProjectSize = "Small", .ProjectType = "New", .ReportType = "New"})
        _liveProjects.Add(New LiveProjectItem With {.ProjectCode = "TPL-ROL", .ProjectName = "SMA - ROL Project", .TemplateName = "SMA - ROL Project", .ProjectSize = "Medium", .ProjectType = "Update", .ReportType = "Update"})
        _liveProjects.Add(New LiveProjectItem With {.ProjectCode = "TPL-WITHIN", .ProjectName = "SMA - Within Project", .TemplateName = "SMA - Within Project", .ProjectSize = "Small", .ProjectType = "Update", .ReportType = "Update"})

        _grid.DataSource = _projects
        _liveProjectSelector.DataSource = Nothing
        _liveProjectSelector.DisplayMember = NameOf(LiveProjectItem.DisplayText)
        _liveProjectSelector.DataSource = _liveProjects
        If _liveProjects.Count > 0 Then
            _liveProjectSelector.SelectedIndex = 0
        End If

        _liveProjectSearchBox.Text = "SMA BRE Project"
        _status.Text = "Designer preview"
        UpdatePlanningSummary()
        UpdateLiveProjectSizeLabel()
    End Sub


End Class
