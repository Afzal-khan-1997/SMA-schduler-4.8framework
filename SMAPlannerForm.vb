Imports System.ComponentModel
Imports System.Globalization

Public Class SMAPlannerForm
    Inherits Form

    Private ReadOnly _projectLibrary As New ProjectLibraryService()
    Private ReadOnly _liveProjectCatalog As New LiveProjectCatalogService()
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

    Private Sub PlannerActivated(sender As Object, e As EventArgs)
        ApplyCurrentTheme()
    End Sub

    Private Sub ApplyCurrentTheme()
        _currentTheme = SchedulerThemePalette.ThemeByName(SchedulerThemePreferences.LoadThemeName())
        Dim theme = _currentTheme

        BackColor = theme.WindowBack
        headerPanel.BackColor = theme.HeaderBack
        gridPanel.BackColor = theme.PanelBack
        planningSummaryPanel.BackColor = theme.PanelBack

        titleLabel.ForeColor = theme.Text
        promptLabel.ForeColor = theme.Text
        listTitle.ForeColor = theme.Text
        summaryTitleLabel.ForeColor = theme.Text
        _liveProjectSizeLabel.ForeColor = theme.Text
        _status.ForeColor = theme.MutedText
        For Each label In {searchLabel, selectorLabel, summaryPeriodLabel}
            label.ForeColor = theme.MutedText
        Next

        btnNewProject.BackColor = theme.Action
        btnScheduleProject.BackColor = theme.Action
        btnRefreshList.BackColor = theme.CommandBack
        For Each button In {btnNewProject, btnScheduleProject, btnRefreshList}
            button.ForeColor = Color.White
        Next

        newProjectsPanel.BackColor = theme.TileOne
        updateProjectsPanel.BackColor = theme.TileThree
        feedbackProjectsPanel.BackColor = theme.TileFour
        For Each label In {newProjectsLabel, newProjectsCountLabel, updateProjectsLabel, updateProjectsCountLabel, feedbackProjectsLabel, feedbackProjectsCountLabel}
            label.ForeColor = theme.Text
        Next

        _grid.BackgroundColor = theme.PanelBack
        _grid.GridColor = theme.GridLine
        _grid.ColumnHeadersDefaultCellStyle.BackColor = theme.GridHeader
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        _grid.DefaultCellStyle.BackColor = theme.PanelBack
        _grid.DefaultCellStyle.ForeColor = theme.Text
        _grid.DefaultCellStyle.SelectionBackColor = theme.Selection
        _grid.DefaultCellStyle.SelectionForeColor = theme.Text
        _grid.AlternatingRowsDefaultCellStyle.BackColor = theme.AlternatingRow
        _grid.EnableHeadersVisualStyles = False
    End Sub

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
        For Each project In _projectLibrary.ListProjects()
            _projects.Add(project)
        Next

        _status.Text = _projects.Count.ToString(CultureInfo.InvariantCulture) & " planned project(s). Double-click a project to update its schedule."
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

        Dim snapshot = _projectLibrary.LoadSnapshot(item.FilePath)
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
End Class
