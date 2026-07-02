Imports System.ComponentModel
Imports System.Configuration
Imports System.Globalization

Public Class SMAPlannerForm
    Inherits Form

    Private ReadOnly _projectLibrary As New ProjectLibraryService()
    Private ReadOnly _liveProjectCatalog As New LiveProjectCatalogService()
    Private ReadOnly _sqlRepository As SqlProjectRepository = CreateSqlRepository()
    Private ReadOnly _projects As New BindingList(Of ProjectLibraryItem)()
    Private ReadOnly _liveProjects As New BindingList(Of LiveProjectItem)()
    Private _currentTheme As SchedulerThemePalette = SchedulerThemePalette.ThemeByName(SchedulerThemePreferences.LoadThemeName())

    Public Sub New()
        InitializeComponent()
        ConfigurePlannerForm()
        ApplyCurrentTheme()
        LoadLiveProjectList()
        LoadProjectList()
    End Sub

    Private Sub ConfigurePlannerForm()
        _liveProjectSelector.DisplayMember = NameOf(LiveProjectItem.DisplayText)
        _liveProjectSelector.DataSource = _liveProjects

        _grid.DataSource = _projects
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        _grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        _grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)

        AddHandler btnNewProject.Click, AddressOf OpenNewProject
        AddHandler btnRefreshList.Click, Sub() LoadProjectList()
        AddHandler btnScheduleProject.Click, AddressOf OpenSelectedLiveProjectTemplate
        AddHandler _liveProjectSearchBox.TextChanged, Sub() LoadLiveProjectList()
        AddHandler _liveProjectSelector.SelectedIndexChanged, AddressOf LiveProjectSelectionChanged
        AddHandler _grid.CellDoubleClick, AddressOf OpenSelectedExistingProject
        AddHandler Activated, AddressOf PlannerActivated
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
        _currentTheme = SchedulerThemePalette.ThemeByName(SchedulerThemePreferences.LoadThemeName())
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

        For Each label In New Label() {titleLabel, promptLabel, listTitle, summaryTitleLabel, _liveProjectSizeLabel}
            If ControlReady(label) Then
                label.ForeColor = theme.Text
            End If
        Next

        If ControlReady(_status) Then
            _status.ForeColor = theme.MutedText
        End If

        For Each label In New Label() {searchLabel, selectorLabel, summaryPeriodLabel}
            If ControlReady(label) Then
                label.ForeColor = theme.MutedText
            End If
        Next

        If ControlReady(btnNewProject) Then
            btnNewProject.BackColor = theme.Action
        End If

        If ControlReady(btnScheduleProject) Then
            btnScheduleProject.BackColor = theme.Action
        End If

        If ControlReady(btnRefreshList) Then
            btnRefreshList.BackColor = theme.CommandBack
        End If

        For Each button In New Button() {btnNewProject, btnScheduleProject, btnRefreshList}
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

    Private Shared Function ControlReady(control As Control) As Boolean
        Return control IsNot Nothing AndAlso Not control.IsDisposed
    End Function

    Private Sub LoadLiveProjectList()
        Dim selectedCode = SelectedLiveProject()?.ProjectCode
        Dim matches = _liveProjectCatalog.SearchProjects(_liveProjectSearchBox.Text)

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
            If Not String.IsNullOrWhiteSpace(selectedCode) Then
                For i = 0 To _liveProjects.Count - 1
                    If String.Equals(_liveProjects(i).ProjectCode, selectedCode, StringComparison.OrdinalIgnoreCase) Then
                        restoreIndex = i
                        Exit For
                    End If
                Next
            End If
            _liveProjectSelector.SelectedIndex = If(restoreIndex >= 0, restoreIndex, 0)
        Else
            _liveProjectSelector.SelectedIndex = -1
        End If
        _liveProjectSelector.EndUpdate()
        UpdateLiveProjectSizeLabel()
    End Sub

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
        UpdatePlanningSummary()
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

    Private Sub LiveProjectSelectionChanged(sender As Object, e As EventArgs)
        UpdateLiveProjectSizeLabel()
    End Sub

    Private Sub UpdateLiveProjectSizeLabel()
        Dim selectedProject = SelectedLiveProject()
        If selectedProject Is Nothing Then
            _liveProjectSizeLabel.Text = "No project found"
            Return
        End If

        _liveProjectSizeLabel.Text = "Project size: " & selectedProject.ProjectSize
    End Sub

    Private Sub OpenNewProject(sender As Object, e As EventArgs)
        Using scheduler As New SMASchedulerForm()
            scheduler.LoadLiveProjectTemplate(LiveProjectCatalogService.CreateSmaNewProjectTemplate())
            FormTransitionService.ShowDialogWithMotion(Me, scheduler)
        End Using
        ApplyCurrentTheme()
        LoadProjectList()
    End Sub

    Private Sub OpenSelectedLiveProjectTemplate(sender As Object, e As EventArgs)
        Dim selectedProject = SelectedLiveProject()
        If selectedProject Is Nothing Then
            MessageBox.Show(Me, "No project is selected.", "Schedule Project", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using scheduler As New SMASchedulerForm()
            scheduler.LoadLiveProjectTemplate(selectedProject)
            FormTransitionService.ShowDialogWithMotion(Me, scheduler)
        End Using
        ApplyCurrentTheme()
        LoadProjectList()
    End Sub

    Private Sub OpenSelectedExistingProject(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        Dim item = TryCast(_grid.Rows(e.RowIndex).DataBoundItem, ProjectLibraryItem)
        If item Is Nothing Then
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
        LoadProjectList()
    End Sub

    Private Sub SetPlannerStatus(message As String)
        If String.IsNullOrWhiteSpace(message) Then
            Return
        End If

        _status.Text = message
    End Sub


End Class
