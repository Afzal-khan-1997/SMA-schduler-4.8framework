Imports System.ComponentModel
Imports System.Configuration
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO

Public Class SMASchedulerForm
    Private ReadOnly _tasks As New BindingList(Of ScheduleTask)
    Private ReadOnly _engine As New ScheduleEngine()
    Private ReadOnly _taskCatalogService As New TaskCatalogService()
    Private ReadOnly _employeeCatalogService As New EmployeeCatalogService()
    Private ReadOnly _xlsxExportService As New XlsxExportService()
    Private ReadOnly _projectLibrary As New ProjectLibraryService()
    Private ReadOnly _sqlRepository As SqlProjectRepository = CreateSqlRepository()
    Private ReadOnly _taskCatalog As New BindingList(Of TaskCatalogItem)
    Private ReadOnly _employees As New BindingList(Of String)

    Private _workspaceTabs As TabControl
    Private _capacityGrid As DataGridView
    Private _taskUsageGrid As DataGridView
    Private _resourceUsageGrid As DataGridView
    Private _plannerLegendGrid As DataGridView
    Private ReadOnly _plannerPieCharts As New List(Of PlannerPieChartPanel)
    Private ReadOnly _plannerTaskCountLabels As New List(Of Label)
    Private ReadOnly _plannerDurationLabels As New List(Of Label)
    Private _currentTheme As SchedulerThemePalette = SchedulerThemePalette.ThemeByName(SchedulerThemePreferences.LoadThemeName())
    Private _projectType As String = "New"
    Private ReadOnly _capacityDateColumns As New Dictionary(Of Integer, Date)
    Private ReadOnly _taskViewDateColumns As New Dictionary(Of Integer, Date)
    Private ReadOnly _resourceUsageDateColumns As New Dictionary(Of Integer, Date)
    Private _isRecalculating As Boolean
    Private _isRefreshingWorkspace As Boolean
    Private _isLoadingCatalogControls As Boolean
    Private _suspendTaskEvents As Boolean
    Private _isLoadingCapacityGrid As Boolean
    Private _isLoadingTaskViewGrid As Boolean
    Private _isLoadingResourceUsageGrid As Boolean
    Private _workspaceRefreshQueued As Boolean
    Private _lastSavedSignature As String = ""

    Public Sub New()
        InitializeComponent()
        ConfigureDesignerUi()
        AddHandler _tasks.ListChanged, AddressOf TasksChanged
        RecalculateAndRefresh("Ready")
        ClearPlanningInputDisplays()
        MarkCurrentStateSaved()
    End Sub

    Private Shared Function CreateSqlRepository() As SqlProjectRepository
        Dim connectionString = ""
        Try
            Dim settings = ConfigurationManager.ConnectionStrings("SmaSchedulerDb")
            If settings IsNot Nothing Then
                connectionString = settings.ConnectionString
            End If
        Catch
        End Try

        If String.IsNullOrWhiteSpace(connectionString) Then
            connectionString = Environment.GetEnvironmentVariable("SMA_SCHEDULER_SQL_CONNECTION")
        End If

        If String.IsNullOrWhiteSpace(connectionString) Then
            Return Nothing
        End If

        Return New SqlProjectRepository(connectionString.Trim())
    End Function

    Public Sub StartNewProject()
        ClearProjectForNewSchedule()
        SetStatus("New project ready")
    End Sub

    Public Sub LoadLiveProjectTemplate(liveProject As LiveProjectItem)
        If liveProject Is Nothing Then
            Return
        End If

        ClearProjectForNewSchedule()
        _projectName.Text = If(String.IsNullOrWhiteSpace(liveProject.ProjectName), "SMA Scheduler", liveProject.ProjectName.Trim())
        _versionNumber.Text = If(String.IsNullOrWhiteSpace(liveProject.VersionNumber), "1.0", liveProject.VersionNumber.Trim())
        _projectType = ProjectTypeFromTemplate(liveProject.TemplateName, liveProject.ProjectName)
        SelectProjectSize(liveProject.ProjectSize)

        Dim projectSize = CStr(_projectSizeSelector.SelectedItem)
        LoadTemplateTasks(liveProject.TemplateName, projectSize)
        If _tasks.Count = 0 Then
            ClearPlanningInputDisplays()
        End If
        RecalculateAndRefresh(liveProject.TemplateName & " template loaded for " & projectSize & " project")
    End Sub

    Public Sub LoadProjectSnapshot(snapshot As ProjectSnapshot)
        If snapshot Is Nothing Then
            Return
        End If

        _tasks.Clear()
        _projectName.Text = snapshot.ProjectName
        _versionNumber.Text = If(String.IsNullOrWhiteSpace(snapshot.VersionNumber), "1.0", snapshot.VersionNumber)
        _projectType = If(String.IsNullOrWhiteSpace(snapshot.ProjectType), ProjectTypeFromTemplate("", snapshot.ProjectName), snapshot.ProjectType)
        SelectProjectSize(snapshot.ProjectSize)
        _totalProjectHours.Value = ClampDecimal(snapshot.TotalProjectHours, _totalProjectHours.Minimum, _totalProjectHours.Maximum)
        _resourcesNeeded.Value = ClampDecimal(snapshot.ResourcesNeeded, _resourcesNeeded.Minimum, _resourcesNeeded.Maximum)
        If snapshot.Tasks IsNot Nothing Then
            For Each task In snapshot.Tasks
                _tasks.Add(task)
            Next
        End If
        RenumberTasks()
        RecalculateAndRefresh("Project opened")
        MarkCurrentStateSaved()
    End Sub

    Private Sub SelectProjectSize(sizeName As String)
        If _projectSizeSelector Is Nothing OrElse _projectSizeSelector.Items.Count = 0 Then
            Return
        End If

        Dim normalizedSize = If(String.IsNullOrWhiteSpace(sizeName), "Small", sizeName.Trim())
        For i = 0 To _projectSizeSelector.Items.Count - 1
            If String.Equals(Convert.ToString(_projectSizeSelector.Items(i), CultureInfo.InvariantCulture), normalizedSize, StringComparison.OrdinalIgnoreCase) Then
                _projectSizeSelector.SelectedIndex = i
                Return
            End If
        Next

        _projectSizeSelector.SelectedIndex = 0
    End Sub

    Private Shared Function ProjectTypeFromTemplate(templateName As String, projectName As String) As String
        Dim classificationText = (If(templateName, "") & " " & If(projectName, "")).Trim()
        If classificationText.IndexOf("feedback", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Return "Feedback"
        End If
        If classificationText.IndexOf("update", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            classificationText.IndexOf("bre", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            classificationText.IndexOf("rol", StringComparison.OrdinalIgnoreCase) >= 0 Then
            Return "Update"
        End If

        Return "New"
    End Function

    Private Sub ConfigureDesignerUi()
        _grid.AutoGenerateColumns = False
        _grid.Columns.Clear()
        _gantt.Tasks = _tasks
        StyleGrid()
        ConfigureWorkspaceTabs()
        LoadCatalogControls()
        AddGridColumns()
        _grid.DataSource = _tasks
        ApplySchedulerHeaderLayout()
        ApplyTheme()
        AddHandler _grid.SelectionChanged, AddressOf ScheduleSelectionChanged
        AddHandler btnSave.Click, AddressOf SaveProjectFile
        AddHandler btnRefreshCapacity.Click, AddressOf RefreshCapacityPlanning
        AddHandler btnAddTask.Click, AddressOf AddTask
        AddHandler btnDelete.Click, AddressOf DeleteTask
        AddHandler btnMoveUp.Click, AddressOf MoveTaskUp
        AddHandler btnMoveDown.Click, AddressOf MoveTaskDown
        AddHandler btnLink.Click, AddressOf LinkSelectedTask
        AddHandler btnUnlink.Click, AddressOf UnlinkSelectedTask
        AddHandler btnMilestone.Click, AddressOf ToggleMilestone
        AddHandler btnChangeTheme.Click, AddressOf ChangeTheme
        AddHandler _totalProjectHours.ValueChanged, AddressOf ProjectInputsChanged
        AddHandler _resourcesNeeded.ValueChanged, AddressOf ProjectInputsChanged
        AddHandler _includeSaturdays.CheckedChanged, AddressOf AllocationInputsChanged
        AddHandler _taskCatalogSelector.SelectedIndexChanged, AddressOf CatalogSelectionChanged
        AddHandler _projectSizeSelector.SelectedIndexChanged, AddressOf CatalogSelectionChanged
    End Sub

    Private Sub ChangeTheme(sender As Object, e As EventArgs)
        Dim themeNames = SchedulerThemePalette.AllNames().Select(Function(name) Convert.ToString(name, CultureInfo.InvariantCulture)).ToList()
        If themeNames.Count = 0 Then
            Return
        End If

        Dim currentIndex = themeNames.FindIndex(Function(name) String.Equals(name, _currentTheme.Name, StringComparison.OrdinalIgnoreCase))
        Dim nextIndex = If(currentIndex < 0, 0, (currentIndex + 1) Mod themeNames.Count)
        _currentTheme = SchedulerThemePalette.ThemeByName(themeNames(nextIndex))
        SchedulerThemePreferences.SaveThemeName(_currentTheme.Name)
        ApplyTheme()
        _gantt.Invalidate()
        For Each chart In _plannerPieCharts
            If chart IsNot Nothing Then
                chart.Invalidate()
            End If
        Next
        SetStatus("Theme changed to " & _currentTheme.Name)
    End Sub

    Private Sub ApplySchedulerHeaderLayout()
        If taskCatalogLabel.Parent IsNot Nothing Then
            taskCatalogLabel.Parent.Controls.Remove(taskCatalogLabel)
        End If
        If _taskCatalogSelector.Parent IsNot Nothing Then
            _taskCatalogSelector.Parent.Controls.Remove(_taskCatalogSelector)
        End If
        If resourcesNeededLabel.Parent IsNot Nothing Then
            resourcesNeededLabel.Parent.Controls.Remove(resourcesNeededLabel)
        End If
        If _resourcesNeeded.Parent IsNot Nothing Then
            _resourcesNeeded.Parent.Controls.Remove(_resourcesNeeded)
        End If
        If _remainingHoursLabel.Parent IsNot Nothing Then
            _remainingHoursLabel.Parent.Controls.Remove(_remainingHoursLabel)
        End If
    End Sub

    Private Sub ApplyTheme()
        Dim theme = _currentTheme
        BackColor = theme.WindowBack
        commandBar.BackColor = theme.CommandBack
        For Each item As ToolStripItem In commandBar.Items
            item.ForeColor = theme.CommandText
        Next

        headerPanel.BackColor = theme.HeaderBack
        appTitle.ForeColor = theme.Text

        For Each label In {projectLabel, versionLabel, totalHoursLabel, projectSizeLabel, resourcesNeededLabel}
            If label IsNot Nothing Then
                label.ForeColor = theme.MutedText
            End If
        Next

        _summaryTitle.BackColor = theme.TileOne
        _summaryDates.BackColor = theme.TileTwo
        _summaryProgress.BackColor = theme.TileThree
        _summaryResources.BackColor = theme.TileFour
        For Each label In {_summaryTitle, _summaryDates, _summaryProgress, _summaryResources}
            label.ForeColor = theme.Text
        Next

        contentSplit.BackColor = theme.Divider
        mainSplit.BackColor = theme.Divider
        _detailsPanel.BackColor = theme.PanelBack
        taskWorkspaceTitle.ForeColor = theme.Text
        statusBar.BackColor = theme.PanelBack
        _status.ForeColor = theme.MutedText
        _gantt.BackColor = theme.PanelBack
        If _workspaceTabs IsNot Nothing Then
            _workspaceTabs.BackColor = theme.PanelBack
            _workspaceTabs.ForeColor = theme.Text
        End If

        ApplyGridTheme(_grid, theme)
        If _capacityGrid IsNot Nothing Then
            ApplyGridTheme(_capacityGrid, theme)
        End If
        If _taskUsageGrid IsNot Nothing Then
            ApplyGridTheme(_taskUsageGrid, theme)
        End If
        If _resourceUsageGrid IsNot Nothing Then
            ApplyGridTheme(_resourceUsageGrid, theme)
        End If
        If _plannerLegendGrid IsNot Nothing Then
            ApplyGridTheme(_plannerLegendGrid, theme)
        End If
        For Each label In _plannerTaskCountLabels.Concat(_plannerDurationLabels)
            If label IsNot Nothing Then
                label.BackColor = theme.TileThree
                label.ForeColor = theme.Text
            End If
        Next
        For Each chart In _plannerPieCharts
            If chart IsNot Nothing Then
                chart.Invalidate()
            End If
        Next
    End Sub

    Private Sub ApplyGridTheme(grid As DataGridView, theme As SchedulerThemePalette)
        If grid Is Nothing Then
            Return
        End If

        grid.BackgroundColor = theme.PanelBack
        grid.GridColor = theme.GridLine
        grid.ColumnHeadersDefaultCellStyle.BackColor = theme.GridHeader
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        grid.DefaultCellStyle.BackColor = theme.PanelBack
        grid.DefaultCellStyle.ForeColor = theme.Text
        grid.DefaultCellStyle.SelectionBackColor = theme.Selection
        grid.DefaultCellStyle.SelectionForeColor = theme.Text
        grid.AlternatingRowsDefaultCellStyle.BackColor = theme.AlternatingRow
        grid.EnableHeadersVisualStyles = False
    End Sub

    Private Sub LoadCatalogControls()
        _isLoadingCatalogControls = True
        _taskCatalog.Clear()
        For Each item In _taskCatalogService.LoadAvailableTasks()
            _taskCatalog.Add(item)
        Next

        _employees.Clear()
        For Each employeeName In _employeeCatalogService.LoadEmployees()
            If Not _employees.Contains(employeeName) Then
                _employees.Add(employeeName)
            End If
        Next

        _taskCatalogSelector.DataSource = _taskCatalog
        _taskCatalogSelector.DisplayMember = NameOf(TaskCatalogItem.Title)
        _projectSizeSelector.Items.AddRange({"Small", "Medium", "Large", "Very Large"})
        _projectSizeSelector.SelectedIndex = 0

        _isLoadingCatalogControls = False
        UpdateAllocationHoursFromSelectedCatalog()
        RecalculateAndRefresh("Working days updated")
    End Sub

    Private Sub ProjectInputsChanged(sender As Object, e As EventArgs)
        UpdateSummary()
        SetStatus("Project planning values updated")
    End Sub

    Private Sub ClearPlanningInputDisplays()
        Dim totalHoursInput = TryCast(_totalProjectHours, BlankNumericUpDown)
        If totalHoursInput IsNot Nothing Then
            totalHoursInput.ClearDisplayedValue()
        ElseIf _totalProjectHours IsNot Nothing Then
            _totalProjectHours.Text = ""
        End If

        Dim resourcesInput = TryCast(_resourcesNeeded, BlankNumericUpDown)
        If resourcesInput IsNot Nothing Then
            resourcesInput.ClearDisplayedValue()
        ElseIf _resourcesNeeded IsNot Nothing Then
            _resourcesNeeded.Text = ""
        End If

        UpdateRemainingHoursDisplay()
    End Sub

    Private Sub CatalogSelectionChanged(sender As Object, e As EventArgs)
        If _isLoadingCatalogControls Then
            Return
        End If
        UpdateAllocationHoursFromSelectedCatalog()
    End Sub

    Private Sub AllocationInputsChanged(sender As Object, e As EventArgs)
        If _isLoadingCatalogControls OrElse _isRecalculating Then
            Return
        End If
        RecalculateAndRefresh("Working days updated")
    End Sub

    Private Sub UpdateAllocationHoursFromSelectedCatalog()
        Dim selectedCatalogTask = TryCast(_taskCatalogSelector.SelectedItem, TaskCatalogItem)
        If selectedCatalogTask Is Nothing OrElse _projectSizeSelector.SelectedItem Is Nothing Then
            Return
        End If

        UpdateRemainingHoursDisplay()
    End Sub

    Private Sub StyleGrid()
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        _grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        _grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)
        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255)
        _grid.DefaultCellStyle.BackColor = Color.White
        _grid.DefaultCellStyle.ForeColor = Color.FromArgb(37, 47, 63)
    End Sub

    Private Sub ConfigureWorkspaceTabs()
        If contentSplit Is Nothing OrElse mainSplit Is Nothing Then
            Return
        End If

        contentSplit.Panel2Collapsed = True
        _plannerPieCharts.Clear()
        _plannerTaskCountLabels.Clear()
        _plannerDurationLabels.Clear()

        If mainSplit.Parent IsNot Nothing Then
            mainSplit.Parent.Controls.Remove(mainSplit)
        End If

        _workspaceTabs = New TabControl With {
            .Dock = DockStyle.Fill,
            .Name = "_workspaceTabs",
            .Padding = New Point(18, 6)
        }

        Dim taskAllocationTab = BuildWorkspaceTabPage("Task Allocation")
        Dim taskViewTab = BuildWorkspaceTabPage("Task Usage View")
        Dim resourceUsageTab = BuildWorkspaceTabPage("Resource Usage View")
        Dim capacityPlanningTab = BuildWorkspaceTabPage("Capacity Planning")
        Dim taskAllocationHost As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White
        }
        Dim taskAllocationCanvas As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(10, 8, 10, 26),
            .BackColor = Color.White
        }
        Dim taskAllocationSurface As New Panel With {
            .Dock = DockStyle.Top,
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }
        Dim taskGridHost = BuildWorkspaceSurfaceHost()
        Dim ganttHost = BuildWorkspaceSurfaceHost()
        Dim allocationPreviewChart As PlannerPieChartPanel = Nothing
        Dim allocationTaskCountLabel As Label = Nothing
        Dim allocationDurationLabel As Label = Nothing
        Dim allocationPreview = BuildPlannerPreviewHost(allocationPreviewChart, allocationTaskCountLabel, allocationDurationLabel)
        Dim ganttCanvas As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White
        }

        taskAllocationTab.Padding = New Padding(0, 0, 0, 10)
        contentSplit.Panel1.Padding = New Padding(0, 0, 0, 10)

        mainSplit.Panel1.Controls.Clear()
        mainSplit.Panel2.Controls.Clear()
        mainSplit.Orientation = Orientation.Vertical
        taskGridHost.Controls.Add(_grid)
        ganttCanvas.Controls.Add(_gantt)
        ganttHost.Controls.Add(ganttCanvas)
        ganttHost.Controls.Add(allocationPreview)
        mainSplit.Panel1.Controls.Add(taskGridHost)
        mainSplit.Panel2.Controls.Add(ganttHost)
        mainSplit.Panel1MinSize = 620
        mainSplit.Panel2MinSize = 320
        mainSplit.Dock = DockStyle.Fill
        _grid.ScrollBars = ScrollBars.Both
        _grid.BorderStyle = BorderStyle.FixedSingle
        AddHandler mainSplit.SizeChanged, Sub()
                                              ApplyResponsiveSplitter(mainSplit, 720, 360, 0.62R)
                                          End Sub
        AddHandler taskAllocationCanvas.SizeChanged, Sub()
                                                         LayoutTaskAllocationSurface(taskAllocationCanvas, taskAllocationSurface)
                                                     End Sub
        taskAllocationSurface.Controls.Add(mainSplit)
        taskAllocationCanvas.Controls.Add(taskAllocationSurface)
        taskAllocationHost.Controls.Add(taskAllocationCanvas)
        taskAllocationTab.Controls.Add(taskAllocationHost)

        _taskUsageGrid = BuildTaskViewGrid()
        _resourceUsageGrid = BuildResourceUsageGrid()
        _capacityGrid = BuildCapacityPlanningSummaryGrid()
        Dim taskUsagePreviewChart As PlannerPieChartPanel = Nothing
        Dim taskUsageTaskCountLabel As Label = Nothing
        Dim taskUsageDurationLabel As Label = Nothing
        Dim resourceUsagePreviewChart As PlannerPieChartPanel = Nothing
        Dim resourceUsageTaskCountLabel As Label = Nothing
        Dim resourceUsageDurationLabel As Label = Nothing

        taskViewTab.Controls.Add(BuildWorkspacePreviewSplit(_taskUsageGrid, BuildPlannerPreviewHost(taskUsagePreviewChart, taskUsageTaskCountLabel, taskUsageDurationLabel)))
        resourceUsageTab.Controls.Add(BuildWorkspacePreviewSplit(_resourceUsageGrid, BuildPlannerPreviewHost(resourceUsagePreviewChart, resourceUsageTaskCountLabel, resourceUsageDurationLabel)))
        capacityPlanningTab.Controls.Add(_capacityGrid)

        _workspaceTabs.TabPages.Add(taskAllocationTab)
        _workspaceTabs.TabPages.Add(taskViewTab)
        _workspaceTabs.TabPages.Add(resourceUsageTab)
        _workspaceTabs.TabPages.Add(capacityPlanningTab)

        contentSplit.Panel1.Controls.Clear()
        contentSplit.Panel1.Controls.Add(_workspaceTabs)

        _plannerLegendGrid = Nothing

        AddHandler _taskUsageGrid.CellParsing, AddressOf CalendarGridCellParsing
        AddHandler _taskUsageGrid.CellEndEdit, AddressOf TaskViewGridCellEndEdit
        AddHandler _taskUsageGrid.DataError, AddressOf CalendarGridDataError

        AddHandler _resourceUsageGrid.CellParsing, AddressOf CalendarGridCellParsing
        AddHandler _resourceUsageGrid.CellEndEdit, AddressOf ResourceUsageGridCellEndEdit
        AddHandler _resourceUsageGrid.DataError, AddressOf CalendarGridDataError

        RefreshWorkspaceTabs()
        LayoutTaskAllocationSurface(taskAllocationCanvas, taskAllocationSurface)
        ApplyResponsiveSplitter(mainSplit, 720, 360, 0.62R)
    End Sub

    Private Sub LayoutTaskAllocationSurface(canvas As Panel, surface As Panel)
        If canvas Is Nothing OrElse surface Is Nothing OrElse canvas.IsDisposed OrElse surface.IsDisposed Then
            Return
        End If

        Dim availableHeight = canvas.ClientSize.Height - canvas.Padding.Vertical
        If availableHeight <= 0 Then
            Return
        End If

        Dim reservedBottomSpace As Integer
        If availableHeight >= 700 Then
            reservedBottomSpace = 22
        ElseIf availableHeight >= 560 Then
            reservedBottomSpace = 18
        Else
            reservedBottomSpace = 14
        End If

        Dim desiredHeight = availableHeight - reservedBottomSpace
        desiredHeight = Math.Max(300, Math.Min(desiredHeight, availableHeight))
        surface.Height = desiredHeight
    End Sub

    Private Sub ApplyResponsiveSplitter(split As SplitContainer, preferredPanel1Min As Integer, preferredPanel2Min As Integer, panel1Ratio As Double)
        If split Is Nothing OrElse split.IsDisposed Then
            Return
        End If

        Dim available = If(split.Orientation = Orientation.Vertical, split.Width, split.Height) - split.SplitterWidth
        If available < 20 Then
            Return
        End If

        Dim panel1Min = preferredPanel1Min
        Dim panel2Min = preferredPanel2Min
        Dim preferredTotal = Math.Max(1, panel1Min + panel2Min)
        If preferredTotal > available Then
            Dim scale = available / CDbl(preferredTotal)
            panel1Min = Math.Max(1, CInt(Math.Floor(panel1Min * scale)))
            panel2Min = Math.Max(1, available - panel1Min)
        End If

        If panel1Min + panel2Min > available Then
            panel2Min = Math.Max(1, available - panel1Min)
        End If

        Dim minDistance = Math.Max(1, panel1Min)
        Dim maxDistance = Math.Max(minDistance, available - Math.Max(1, panel2Min))
        Dim desired = CInt(Math.Round(available * Math.Max(0.1R, Math.Min(0.9R, panel1Ratio))))
        desired = Math.Max(minDistance, Math.Min(maxDistance, desired))

        Try
            split.Panel1MinSize = 1
            split.Panel2MinSize = 1
            split.SplitterDistance = desired
            split.Panel1MinSize = panel1Min
            split.Panel2MinSize = panel2Min
        Catch ex As InvalidOperationException
            ' WinForms can report temporary invalid sizes while the designer/runtime is still laying out.
        End Try
    End Sub

    Private Function PlannerPreviewBadge(text As String) As Label
        Return New Label With {
            .AutoSize = False,
            .Text = text,
            .Width = 210,
            .Height = 28,
            .Margin = New Padding(0, 2, 0, 4),
            .BackColor = Color.FromArgb(225, 239, 255),
            .ForeColor = Color.FromArgb(24, 31, 42),
            .Font = New Font("Segoe UI Semibold", 9.0F),
            .TextAlign = ContentAlignment.MiddleCenter
        }
    End Function

    Private Function BuildWorkspaceTabPage(title As String) As TabPage
        Return New TabPage(title) With {
            .BackColor = Color.White,
            .Padding = New Padding(0)
        }
    End Function

    Private Function BuildWorkspaceSurfaceHost() As Panel
        Return New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 0, 10, 16),
            .BackColor = Color.White
        }
    End Function

    Private Function BuildPlannerPreviewHost(ByRef chart As PlannerPieChartPanel, ByRef taskCountLabel As Label, ByRef durationLabel As Label) As Panel
        Dim host As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 310,
            .Padding = New Padding(10, 10, 10, 12),
            .BackColor = Color.White
        }

        Dim card As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Padding = New Padding(12)
        }

        Dim title As New Label With {
            .Dock = DockStyle.Top,
            .Height = 28,
            .Text = "Planner Preview",
            .Font = New Font("Segoe UI Semibold", 12.0F),
            .ForeColor = Color.FromArgb(24, 31, 42),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        taskCountLabel = PlannerPreviewBadge("Scheduled Tasks: 0")
        taskCountLabel.Dock = DockStyle.Top

        durationLabel = PlannerPreviewBadge("Duration: 0 days")
        durationLabel.Dock = DockStyle.Top

        chart = New PlannerPieChartPanel(_tasks) With {
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .MinimumSize = New Size(180, 180)
        }

        card.Controls.Add(chart)
        card.Controls.Add(durationLabel)
        card.Controls.Add(taskCountLabel)
        card.Controls.Add(title)
        host.Controls.Add(card)

        _plannerPieCharts.Add(chart)
        _plannerTaskCountLabels.Add(taskCountLabel)
        _plannerDurationLabels.Add(durationLabel)

        Return host
    End Function

    Private Function BuildWorkspacePreviewSplit(mainControl As Control, previewHost As Control) As SplitContainer
        Dim split As New SplitContainer With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Vertical,
            .BackColor = Color.FromArgb(232, 236, 242),
            .SplitterWidth = 6,
            .FixedPanel = FixedPanel.Panel2
        }

        split.Panel1.Padding = New Padding(0, 0, 10, 0)
        split.Panel2.Padding = New Padding(0)
        split.Panel1.Controls.Add(mainControl)

        Dim previewCanvas As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White
        }
        previewCanvas.Controls.Add(previewHost)
        split.Panel2.Controls.Add(previewCanvas)
        split.Panel1MinSize = 520
        split.Panel2MinSize = 250

        AddHandler split.SizeChanged,
            Sub()
                ApplyResponsiveSplitter(split, 620, 280, 0.76R)
            End Sub

        ApplyResponsiveSplitter(split, 620, 280, 0.76R)
        Return split
    End Function

    Private Function BuildTaskViewGrid() As DataGridView
        Dim usageGrid As New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoGenerateColumns = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ColumnHeadersHeight = 32,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .GridColor = Color.FromArgb(232, 236, 242),
            .MultiSelect = False,
            .Name = "_taskUsageGrid",
            .RowHeadersVisible = False,
            .ScrollBars = ScrollBars.Both,
            .SelectionMode = DataGridViewSelectionMode.CellSelect
        }
        usageGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        usageGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        usageGrid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        usageGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        usageGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)
        Return usageGrid
    End Function

    Private Function BuildResourceUsageGrid() As DataGridView
        Dim usageGrid As New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoGenerateColumns = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ColumnHeadersHeight = 32,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .GridColor = Color.FromArgb(232, 236, 242),
            .MultiSelect = False,
            .Name = "_resourceUsageGrid",
            .RowHeadersVisible = False,
            .ScrollBars = ScrollBars.Both,
            .SelectionMode = DataGridViewSelectionMode.CellSelect
        }
        usageGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        usageGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        usageGrid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        usageGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        usageGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)
        Return usageGrid
    End Function

    Private Function BuildCapacityPlanningSummaryGrid() As DataGridView
        Dim grid As New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoGenerateColumns = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ColumnHeadersHeight = 32,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .GridColor = Color.FromArgb(232, 236, 242),
            .MultiSelect = False,
            .Name = "_capacityGrid",
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .ScrollBars = ScrollBars.Both,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)
        Return grid
    End Function

    Private Sub UpdateTaskUsageGrid()
        If _taskUsageGrid Is Nothing Then
            Return
        End If

        _isLoadingTaskViewGrid = True
        Try
            _taskViewDateColumns.Clear()
            _taskUsageGrid.Columns.Clear()
            _taskUsageGrid.Rows.Clear()

            _taskUsageGrid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Task / Resource", .Width = 340, .ReadOnly = True, .Frozen = True})
            _taskUsageGrid.Columns.Add(New CalendarColumn With {.HeaderText = "Start", .Width = 96, .Frozen = True})
            _taskUsageGrid.Columns.Add(New CalendarColumn With {.HeaderText = "Finish", .Width = 96, .Frozen = True})
            _taskUsageGrid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Assigned To", .Width = 220, .ReadOnly = True, .Frozen = True})

            Dim projectStart As Date
            Dim projectFinish As Date
            If Not TryGetProjectDateRange(projectStart, projectFinish) Then
                _taskUsageGrid.Rows.Add("No tasks loaded", "", "", "")
                Return
            End If

            AddDateColumns(_taskUsageGrid, _taskViewDateColumns, projectStart, projectFinish)

            For Each task In _tasks.OrderBy(Function(item) item.TaskId)
                Dim taskRowIndex = _taskUsageGrid.Rows.Add()
                Dim taskRow = _taskUsageGrid.Rows(taskRowIndex)
                taskRow.Tag = New TaskRowTag(task.TaskId)
                taskRow.DefaultCellStyle.BackColor = Color.FromArgb(235, 243, 252)
                taskRow.DefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
                taskRow.Cells(0).Value = task.TaskId.ToString(CultureInfo.InvariantCulture) & ". " & task.TaskName
                taskRow.Cells(0).ReadOnly = True
                taskRow.Cells(1).Value = task.StartDate.Date
                taskRow.Cells(2).Value = task.FinishDate.Date
                taskRow.Cells(3).Value = task.AssignedTo
                taskRow.Cells(3).ReadOnly = True

                For Each dateColumn In _taskViewDateColumns
                    Dim workDate = dateColumn.Value
                    Dim cell = taskRow.Cells(dateColumn.Key)
                    If IsBlockedScheduleDate(workDate) Then
                        cell.Value = "HOLIDAY"
                        cell.ReadOnly = True
                        cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                        cell.Style.ForeColor = Color.DimGray
                    Else
                        Dim totalHours = TaskTotalHoursOnDate(task, workDate)
                        cell.Value = totalHours
                        cell.ReadOnly = True
                        StyleUsageSummaryCell(cell, totalHours)
                    End If
                Next

                For Each resourceName In ResourceNamesFromAssignment(task.AssignedTo)
                    Dim rowIndex = _taskUsageGrid.Rows.Add()
                    Dim row = _taskUsageGrid.Rows(rowIndex)
                    row.Tag = New ResourceTaskRowTag(resourceName, task.TaskId)
                    row.Cells(0).Value = "   " & resourceName
                    row.Cells(0).ReadOnly = True
                    row.Cells(1).Value = task.StartDate.Date
                    row.Cells(2).Value = task.FinishDate.Date
                    row.Cells(3).Value = ""
                    row.Cells(3).ReadOnly = True

                    For Each dateColumn In _taskViewDateColumns
                        Dim workDate = dateColumn.Value
                        Dim cell = row.Cells(dateColumn.Key)
                        If IsBlockedScheduleDate(workDate) Then
                            cell.Value = "HOLIDAY"
                            cell.ReadOnly = True
                            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                            cell.Style.ForeColor = Color.DimGray
                        Else
                            cell.Value = TaskHoursOnDate(task, resourceName, workDate)
                            cell.ReadOnly = False
                            StyleResourceTaskCell(cell, resourceName, workDate)
                        End If
                    Next
                Next
            Next
        Finally
            _isLoadingTaskViewGrid = False
        End Try
    End Sub

    Private Function PlannerLegendGrid() As DataGridView
        Dim legend As New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AutoGenerateColumns = False,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .ColumnHeadersHeight = 30,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .GridColor = Color.FromArgb(232, 236, 242),
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .RowTemplate = New DataGridViewRow With {.Height = 24},
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        legend.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 46, 66)
        legend.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        legend.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        legend.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 235, 255)
        legend.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 31, 42)
        legend.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(PlannerPreviewRow.ColorName), .HeaderText = "", .Width = 34, .Resizable = DataGridViewTriState.False})
        legend.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = NameOf(PlannerPreviewRow.TaskName),
            .HeaderText = "Task",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .MinimumWidth = 220,
            .DefaultCellStyle = New DataGridViewCellStyle With {.WrapMode = DataGridViewTriState.True}
        })
        legend.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(PlannerPreviewRow.DurationText), .HeaderText = "Duration", .Width = 86, .Resizable = DataGridViewTriState.False})
        AddHandler legend.CellFormatting, AddressOf PlannerLegendCellFormatting
        AddHandler legend.CellToolTipTextNeeded, AddressOf PlannerLegendToolTipTextNeeded
        Return legend
    End Function

    Private Sub AddGridColumns()
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ScheduleTask.TaskId), .HeaderText = "ID", .Width = 46, .ReadOnly = True})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ScheduleTask.TaskName), .HeaderText = "Task Name", .Width = 303})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ScheduleTask.DurationDays), .HeaderText = "Duration (Days)", .Width = 104, .ValueType = GetType(Decimal), .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "0.###"}})
        _grid.Columns.Add(New CalendarColumn With {.DataPropertyName = NameOf(ScheduleTask.StartDate), .HeaderText = "Start", .Width = 104})
        _grid.Columns.Add(New CalendarColumn With {.DataPropertyName = NameOf(ScheduleTask.FinishDate), .HeaderText = "Finish", .Width = 104})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ScheduleTask.PercentComplete), .HeaderText = "%", .Width = 52})
        Dim predecessorColumn As New DataGridViewComboBoxColumn With {
            .DataPropertyName = NameOf(ScheduleTask.PredecessorLink),
            .HeaderText = "Predecessors",
            .Width = 108,
            .FlatStyle = FlatStyle.Flat,
            .DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
            .ValueType = GetType(String)
        }
        predecessorColumn.Items.AddRange("", "FS", "SS", "FF", "SF")
        _grid.Columns.Add(predecessorColumn)
        _grid.Columns.Add(New ResourceChecklistColumn(_employees) With {.DataPropertyName = NameOf(ScheduleTask.AssignedTo), .HeaderText = "Assigned To", .Width = 210})
        _grid.Columns.Add(New CalendarColumn With {.DataPropertyName = NameOf(ScheduleTask.AssignmentDate), .HeaderText = "Assign Date", .Width = 104})
        _grid.Columns.Add(New DataGridViewTextBoxColumn With {.DataPropertyName = NameOf(ScheduleTask.ResourceHours), .HeaderText = "Resource Hours", .Width = 108, .ValueType = GetType(Decimal), .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "0.##"}})
    End Sub

    Private Sub SeedProject()
        _totalProjectHours.Value = 176
        _resourcesNeeded.Value = 5

        _tasks.Add(New ScheduleTask With {.TaskId = 1, .DatabaseTaskId = 1, .TaskName = "Input study and Copy files to M Files", .StartDate = Date.Today, .AssignmentDate = Date.Today, .DurationDays = 1, .PercentComplete = 0, .AssignedTo = "Devarajan", .ResourceNames = "Devarajan", .ResourceAllocations = "Devarajan=8", .ResourceHours = 8, .ModuleId = 1})
        _tasks.Add(New ScheduleTask With {.TaskId = 2, .DatabaseTaskId = 6, .TaskName = "Create Scope of work", .StartDate = Date.Today, .AssignmentDate = Date.Today, .DurationDays = DurationFromHours(16D), .PercentComplete = 0, .Predecessors = "1", .AssignedTo = "Mahaboob Basha", .ResourceNames = "Mahaboob Basha", .ResourceAllocations = "Mahaboob Basha=16", .ResourceHours = 16, .ModuleId = 1})
        _tasks.Add(New ScheduleTask With {.TaskId = 3, .DatabaseTaskId = 28, .TaskName = "3D Modeling Proposed", .StartDate = Date.Today, .AssignmentDate = Date.Today, .DurationDays = DurationFromHours(12D), .Predecessors = "2", .AssignedTo = "Aashiq Aliuddin", .ResourceNames = "Aashiq Aliuddin", .ResourceAllocations = "Aashiq Aliuddin=12", .ResourceHours = 12, .ModuleId = 4})
    End Sub

    Private Sub NewProject(sender As Object, e As EventArgs)
        Dim decision = PromptForUnsavedChanges()
        If decision = DialogResult.Cancel Then
            Return
        End If
        If decision = DialogResult.Yes AndAlso Not SaveProjectToSql(showSuccessMessage:=False) Then
            Return
        End If

        ClearProjectForNewSchedule()
    End Sub

    Private Sub ClearProjectForNewSchedule()
        _tasks.Clear()
        _projectType = "New"
        _projectName.Text = "SMA Scheduler"
        _versionNumber.Text = "1.0"
        _totalProjectHours.Value = 0
        _resourcesNeeded.Value = 0
        RecalculateAndRefresh("New project created")
        ClearPlanningInputDisplays()
        MarkCurrentStateSaved()
    End Sub

    Private Sub AddTask(sender As Object, e As EventArgs)
        Dim nextId = If(_tasks.Count = 0, 1, _tasks.Max(Function(t) t.TaskId) + 1)
        Dim startDate = NextWorkingDate(If(_tasks.Count = 0, Date.Today, _tasks.Max(Function(t) t.FinishDate).AddDays(1)))
        _tasks.Add(New ScheduleTask With {
                   .TaskId = nextId,
                   .TaskName = "New task " & nextId,
                   .StartDate = startDate,
                   .AssignmentDate = NextWorkingDate(Date.Today),
                   .DurationDays = 1,
                   .PercentComplete = 0,
                   .AssignedTo = "",
                   .ResourceNames = "",
                   .ResourceAllocations = "",
                   .DailyResourceAllocations = "",
                   .ResourceHours = 0D})
        SelectTask(nextId)
        RecalculateAndRefresh("Task added")
    End Sub

    Private Sub AddTaskFromCatalog(selectedCatalogTask As TaskCatalogItem)
        Dim nextId = If(_tasks.Count = 0, 1, _tasks.Max(Function(t) t.TaskId) + 1)
        Dim requestedHours = DefaultTaskHours(selectedCatalogTask)
        Dim startDate = PreferredTaskStartDate()

        _tasks.Add(New ScheduleTask With {
                   .TaskId = nextId,
                   .DatabaseTaskId = selectedCatalogTask.DatabaseTaskId,
                   .TaskName = selectedCatalogTask.Title,
                   .StartDate = startDate,
                   .AssignmentDate = startDate,
                   .DurationDays = DurationFromHours(requestedHours),
                   .PercentComplete = 0,
                   .Predecessors = If(_tasks.Count = 0, "", (_tasks.Count).ToString()),
                   .AssignedTo = "",
                   .ResourceNames = "",
                   .ResourceAllocations = "",
                   .DailyResourceAllocations = "",
                   .ResourceHours = 0D,
                   .ModuleId = selectedCatalogTask.ModuleId})

        SelectTask(nextId)
        RecalculateAndRefresh("Task added from allocation panel")
    End Sub

    Private Sub LoadTemplateTasks(templateName As String, projectSize As String)
        Dim templateTasks = _taskCatalogService.LoadTemplateTasks(templateName, projectSize)
        Dim startDate = NextWorkingDate(Date.Today)
        Dim totalTemplateHours = 0D

        _tasks.Clear()
        For Each catalogTask In templateTasks
            Dim requestedHours = catalogTask.HoursForSize(projectSize)
            If requestedHours <= 0D Then
                Continue For
            End If

            totalTemplateHours += requestedHours
            _tasks.Add(CreateTemplateScheduleTask(catalogTask, _tasks.Count + 1, requestedHours, startDate))
        Next

        _totalProjectHours.Value = ClampDecimal(totalTemplateHours, _totalProjectHours.Minimum, _totalProjectHours.Maximum)
        _resourcesNeeded.Value = 0D
        If _tasks.Count > 0 Then
            SelectTask(1)
        End If
    End Sub

    Private Function CreateTemplateScheduleTask(catalogTask As TaskCatalogItem, taskId As Integer, requestedHours As Decimal, startDate As Date) As ScheduleTask
        Return New ScheduleTask With {
            .TaskId = taskId,
            .DatabaseTaskId = catalogTask.DatabaseTaskId,
            .TaskName = catalogTask.Title,
            .StartDate = startDate,
            .AssignmentDate = startDate,
            .DurationDays = DurationFromHours(requestedHours),
            .PercentComplete = 0,
            .Predecessors = If(taskId <= 1, "", (taskId - 1).ToString(CultureInfo.InvariantCulture)),
            .DependencyType = "FS",
            .AssignedTo = "",
            .ResourceNames = "",
            .ResourceAllocations = "",
            .DailyResourceAllocations = "",
            .ResourceHours = 0D,
            .ModuleId = catalogTask.ModuleId
        }
    End Function

    Private Sub DeleteTask(sender As Object, e As EventArgs)
        Dim selected = SelectedTask()
        If selected Is Nothing Then
            Return
        End If

        Dim removedTaskId = selected.TaskId
        _tasks.Remove(selected)
        For Each task In _tasks
            Dim updatedPredecessors = _engine.ParsePredecessors(task.Predecessors).
                Where(Function(id) id <> removedTaskId).
                Select(Function(id) If(id > removedTaskId, id - 1, id)).
                ToList()
            task.Predecessors = String.Join(",", updatedPredecessors)
        Next
        RenumberTasks()
        RecalculateAndRefresh("Task deleted")
    End Sub

    Private Sub MoveTaskUp(sender As Object, e As EventArgs)
        MoveSelectedTask(-1)
    End Sub

    Private Sub MoveTaskDown(sender As Object, e As EventArgs)
        MoveSelectedTask(1)
    End Sub

    Private Sub MoveSelectedTask(direction As Integer)
        If _grid.CurrentRow Is Nothing Then
            Return
        End If

        Dim index = _grid.CurrentRow.Index
        Dim target = index + direction
        If target < 0 OrElse target >= _tasks.Count Then
            Return
        End If

        Dim task = _tasks(index)
        _suspendTaskEvents = True
        Try
            _tasks.RemoveAt(index)
            _tasks.Insert(target, task)
            RemapPredecessorsForCurrentOrder()
            RenumberTasks()
        Finally
            _suspendTaskEvents = False
        End Try

        _grid.ClearSelection()
        _grid.Rows(target).Selected = True
        _grid.CurrentCell = _grid.Rows(target).Cells(1)
        RecalculateAndRefresh("Task moved")
    End Sub

    Private Sub LinkSelectedTask(sender As Object, e As EventArgs)
        If _grid.CurrentRow Is Nothing OrElse _grid.CurrentRow.Index <= 0 Then
            Return
        End If

        Dim task = DirectCast(_grid.CurrentRow.DataBoundItem, ScheduleTask)
        Dim predecessor = _tasks(_grid.CurrentRow.Index - 1)
        task.Predecessors = predecessor.TaskId.ToString()
        task.DependencyType = "FS"
        RecalculateAndRefresh("Linked to previous task")
    End Sub

    Private Sub UnlinkSelectedTask(sender As Object, e As EventArgs)
        Dim task = SelectedTask()
        If task IsNot Nothing Then
            task.Predecessors = ""
            RecalculateAndRefresh("Dependencies cleared")
        End If
    End Sub

    Private Sub ToggleMilestone(sender As Object, e As EventArgs)
        Dim task = SelectedTask()
        If task Is Nothing Then
            Return
        End If

        task.DurationDays = 1
        task.TaskName = If(task.TaskName.StartsWith("Milestone: "), task.TaskName.Replace("Milestone: ", ""), "Milestone: " & task.TaskName)
        RecalculateAndRefresh("Milestone updated")
    End Sub

    Private Sub SaveProjectFile(sender As Object, e As EventArgs)
        If _tasks.Count = 0 Then
            MessageBox.Show(Me, "There is no task assigned for the project.", "No Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        SaveProjectToSql(showSuccessMessage:=True)
    End Sub

    Private Sub RefreshCapacityPlanning(sender As Object, e As EventArgs)
        CommitWorkspaceGridEdits()
        RecalculateAndRefresh("Capacity planning refreshed")
        Dim sqlSaved = SaveProjectToSql(showSuccessMessage:=False)
        RefreshWorkspaceTabs()
        Dim message = If(sqlSaved,
            "Capacity Planning has been refreshed and SQL has been updated.",
            "Capacity Planning has been refreshed. SQL was not updated.")
        MessageBox.Show(Me, message, "Capacity Planning", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Function SaveProjectToSql(showSuccessMessage As Boolean) As Boolean
        If _tasks.Count = 0 Then
            Return False
        End If

        CommitWorkspaceGridEdits()
        RecalculateAndRefresh("Saving project")

        If _sqlRepository Is Nothing Then
            MessageBox.Show(Me, "SQL connection is not configured. Update App.config with the SmaSchedulerDb connection string before saving.", "SQL Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SetStatus("SQL save skipped because no connection string is configured.")
            Return False
        End If

        Try
            Dim projectName = If(String.IsNullOrWhiteSpace(_projectName.Text), "SMA Scheduler", _projectName.Text.Trim())
            Dim version = If(String.IsNullOrWhiteSpace(_versionNumber.Text), "1.0", _versionNumber.Text.Trim())
            Dim projectSize = If(_projectSizeSelector.SelectedItem Is Nothing, "Small", Convert.ToString(_projectSizeSelector.SelectedItem, CultureInfo.InvariantCulture))
            Dim totalAssignedHours = _tasks.Sum(Function(task) task.ResourceHours)

            _sqlRepository.SaveProject(projectName, _tasks, version, projectSize, _projectType, _totalProjectHours.Value, CInt(_resourcesNeeded.Value), totalAssignedHours)

            SaveEmployeeWorkspaceWorkbook()
            SaveProjectSnapshotToLibrary()
            MarkCurrentStateSaved()

            If showSuccessMessage Then
                MessageBox.Show(Me, "Project has been saved to SQL and Capacity Planning has been updated.", "Project Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            SetStatus("SQL updated for " & projectName)
            Return True
        Catch ex As Exception
            MessageBox.Show(Me, "SQL update failed." & Environment.NewLine & ex.Message, "SQL Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            SetStatus("SQL save failed")
            Return False
        End Try
    End Function

    Private Function PromptForUnsavedChanges() As DialogResult
        If String.Equals(CurrentProjectSignature(), _lastSavedSignature, StringComparison.Ordinal) Then
            Return DialogResult.No
        End If

        Return MessageBox.Show(Me, "Do you want to save changes before creating a new project?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
    End Function

    Private Sub MarkCurrentStateSaved()
        _lastSavedSignature = CurrentProjectSignature()
    End Sub

    Private Function CurrentProjectSignature() As String
        Dim taskSignature = String.Join("|", _tasks.
            OrderBy(Function(task) task.TaskId).
            Select(Function(task) String.Join("~", {
                task.TaskId.ToString(CultureInfo.InvariantCulture),
                task.DatabaseTaskId.ToString(CultureInfo.InvariantCulture),
                task.TaskName,
                task.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                task.FinishDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                task.DurationDays.ToString("0.###", CultureInfo.InvariantCulture),
                task.PercentComplete.ToString(CultureInfo.InvariantCulture),
                task.Predecessors,
                task.DependencyType,
                task.AssignedTo,
                task.AssignmentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                task.ResourceAllocations,
                task.DailyResourceAllocations,
                task.ResourceHours.ToString("0.##", CultureInfo.InvariantCulture)
            })))

        Return String.Join("||", {
            _projectName.Text.Trim(),
            _versionNumber.Text.Trim(),
            Convert.ToString(If(_projectSizeSelector.SelectedItem, "Small"), CultureInfo.InvariantCulture),
            _projectType,
            _totalProjectHours.Value.ToString("0.##", CultureInfo.InvariantCulture),
            _resourcesNeeded.Value.ToString("0", CultureInfo.InvariantCulture),
            _includeSaturdays.Checked.ToString(),
            taskSignature
        })
    End Function

    Private Sub OpenProjectFile(sender As Object, e As EventArgs)
        Using dialog As New OpenFileDialog()
            dialog.Title = "Open SMA schedule"
            dialog.Filter = "SMA schedule (*.smaschedule;*.json)|*.smaschedule;*.json"

            If dialog.ShowDialog(Me) <> DialogResult.OK Then
                Return
            End If

            Dim snapshot = _projectLibrary.LoadSnapshot(dialog.FileName)
            If snapshot Is Nothing Then
                Return
            End If

            LoadProjectSnapshot(snapshot)
        End Using
    End Sub

    Private Sub GridCellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles _grid.CellValueChanged
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse _isRecalculating Then
            Return
        End If

        Dim task = TryCast(_grid.Rows(e.RowIndex).DataBoundItem, ScheduleTask)
        If task Is Nothing Then
            Return
        End If

        NormalizeGridEdit(task, _grid.Columns(e.ColumnIndex).DataPropertyName)
        If Not _suspendTaskEvents Then
            RecalculateAndRefresh("Schedule recalculated")
        End If
    End Sub

    Private Sub GridCellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles _grid.CellBeginEdit
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso IsScheduleEditColumn(_grid.Columns(e.ColumnIndex).DataPropertyName) Then
            _suspendTaskEvents = True
        End If
    End Sub

    Private Sub GridCellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles _grid.CellEndEdit
        If _suspendTaskEvents Then
            _suspendTaskEvents = False
            RecalculateAndRefresh("Schedule recalculated")
        End If
    End Sub

    Private Sub GridCellParsing(sender As Object, e As DataGridViewCellParsingEventArgs) Handles _grid.CellParsing
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim propertyName = _grid.Columns(e.ColumnIndex).DataPropertyName
        If propertyName = NameOf(ScheduleTask.ResourceHours) OrElse propertyName = NameOf(ScheduleTask.DurationDays) Then
            Dim parsedDecimal As Decimal
            If Decimal.TryParse(Convert.ToString(e.Value, CultureInfo.CurrentCulture), NumberStyles.Number, CultureInfo.CurrentCulture, parsedDecimal) OrElse
                Decimal.TryParse(Convert.ToString(e.Value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, parsedDecimal) Then
                e.Value = Math.Max(0D, parsedDecimal)
                e.ParsingApplied = True
            End If
            Return
        End If

        If propertyName <> NameOf(ScheduleTask.StartDate) AndAlso propertyName <> NameOf(ScheduleTask.FinishDate) AndAlso propertyName <> NameOf(ScheduleTask.AssignmentDate) Then
            Return
        End If

        Dim parsedDate As Date
        If TryParseGridDate(Convert.ToString(e.Value, CultureInfo.CurrentCulture), parsedDate) Then
            e.Value = parsedDate.Date
            e.ParsingApplied = True
        End If
    End Sub

    Private Sub GridCurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles _grid.CurrentCellDirtyStateChanged
        If _grid.IsCurrentCellDirty AndAlso (TypeOf _grid.EditingControl Is DataGridViewComboBoxEditingControl OrElse TypeOf _grid.EditingControl Is CalendarEditingControl) Then
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub GridDataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles _grid.DataError
        e.ThrowException = False
        SetStatus("Please enter a valid value")
    End Sub

    Private Sub TasksChanged(sender As Object, e As ListChangedEventArgs)
        If _suspendTaskEvents Then
            Return
        End If

        RecalculateAndRefresh("Schedule updated")
    End Sub

    Private Sub NormalizeGridEdit(task As ScheduleTask, propertyName As String)
        Select Case propertyName
            Case NameOf(ScheduleTask.StartDate)
                task.StartDate = NormalizePickedWorkingDate(task.StartDate)
                If task.FinishDate < task.StartDate Then
                    task.FinishDate = task.StartDate
                End If
                task.FinishDate = NormalizePickedWorkingDate(task.FinishDate)
                ApplyDurationFromDateRange(task)
                CapHoursToDuration(task)
                task.DailyResourceAllocations = BuildDefaultDailyAllocations(task, ParseResourceAllocations(task.ResourceAllocations))

            Case NameOf(ScheduleTask.FinishDate)
                task.FinishDate = NormalizePickedWorkingDate(task.FinishDate)
                If task.FinishDate < task.StartDate Then
                    task.FinishDate = task.StartDate
                End If
                ApplyDurationFromDateRange(task)
                CapHoursToDuration(task)
                task.DailyResourceAllocations = BuildDefaultDailyAllocations(task, ParseResourceAllocations(task.ResourceAllocations))

            Case NameOf(ScheduleTask.DurationDays)
                task.DurationDays = Math.Max(0D, task.DurationDays)
                task.StartDate = NextWorkingDate(task.StartDate)
                task.FinishDate = FinishFromWorkingDuration(task.StartDate, task.DurationDays)
                CapHoursToDuration(task)
                task.DailyResourceAllocations = BuildDefaultDailyAllocations(task, ParseResourceAllocations(task.ResourceAllocations))

            Case NameOf(ScheduleTask.AssignedTo)
                task.AssignedTo = NormalizeResourceList(task.AssignedTo)
                task.ResourceNames = task.AssignedTo
                KeepOnlySelectedResources(task)
                SyncTaskResourceDistribution(task)
                RefreshWorkspaceTabs()
                SetStatus("Resources selected. Edit planned hours in Task Usage View or Resource Usage View.")

            Case NameOf(ScheduleTask.AssignmentDate)
                task.AssignmentDate = NormalizePickedWorkingDate(task.AssignmentDate)

            Case NameOf(ScheduleTask.DependencyType), NameOf(ScheduleTask.PredecessorLink)
                task.DependencyType = NormalizeDependencyType(task.DependencyType)

            Case NameOf(ScheduleTask.ResourceHours)
                task.ResourceHours = Math.Max(0D, task.ResourceHours)
                task.DurationDays = DurationFromHours(task.ResourceHours)
                task.StartDate = NextWorkingDate(task.StartDate)
                task.FinishDate = FinishFromWorkingDuration(task.StartDate, task.DurationDays)
                SyncTaskResourceDistribution(task)
                SaveEmployeeWorkspaceWorkbook()
        End Select
    End Sub

    Private Sub ApplyDurationFromDateRange(task As ScheduleTask)
        task.DurationDays = WorkingDaysBetween(task.StartDate, task.FinishDate)
    End Sub

    Private Sub CapHoursToDuration(task As ScheduleTask)
        task.ResourceHours = ClampDecimal(task.ResourceHours, 0D, task.DurationDays * 8D)
    End Sub

    Private Function DurationFromHours(hours As Decimal) As Decimal
        If hours <= 0D Then
            Return 0D
        End If

        Return hours / 8D
    End Function

    Private Function NormalizePickedWorkingDate(value As Date) As Date
        Dim adjustedDate = NextWorkingDate(value)
        If adjustedDate <> value.Date Then
            MessageBox.Show(value.ToString("dd-MM-yyyy") & " is a non-working day. Date moved to " & adjustedDate.ToString("dd-MM-yyyy") & ".", "Non-working day", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Return adjustedDate
    End Function

    Private Function NextWorkingDate(value As Date) As Date
        Dim currentDate = value.Date
        While IsBlockedScheduleDate(currentDate)
            currentDate = currentDate.AddDays(1)
        End While
        Return currentDate
    End Function

    Private Function FinishFromWorkingDuration(startDate As Date, durationDays As Decimal) As Date
        Dim currentDate = NextWorkingDate(startDate)
        Dim remainingDays = Math.Max(1, CInt(Math.Ceiling(durationDays)))

        While remainingDays > 1
            currentDate = currentDate.AddDays(1)
            If Not IsBlockedScheduleDate(currentDate) Then
                remainingDays -= 1
            End If
        End While

        Return currentDate
    End Function

    Private Function WorkingDaysBetween(startDate As Date, finishDate As Date) As Decimal
        Dim currentDate = NextWorkingDate(startDate)
        Dim endDate = NextWorkingDate(finishDate)
        If endDate < currentDate Then
            Return 1
        End If

        Dim dayCount = 0
        While currentDate <= endDate
            If Not IsBlockedScheduleDate(currentDate) Then
                dayCount += 1
            End If
            currentDate = currentDate.AddDays(1)
        End While

        Return Math.Max(1, dayCount)
    End Function

    Private Function IsBlockedScheduleDate(value As Date) As Boolean
        Return Not _includeSaturdays.Checked AndAlso
            (value.DayOfWeek = DayOfWeek.Saturday OrElse value.DayOfWeek = DayOfWeek.Sunday)
    End Function

    Private Function NormalizeResourceList(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then
            Return ""
        End If

        Dim names = value.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(name) name.Trim()).
            Where(Function(name) name.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase)

        Return String.Join("; ", names)
    End Function

    Private Shared Function NormalizeDependencyType(value As String) As String
        Dim safeValue = If(value, "").Trim().ToUpperInvariant()
        Select Case safeValue
            Case "SS", "FF", "SF"
                Return safeValue
            Case Else
                Return "FS"
        End Select
    End Function

    Private Sub KeepOnlySelectedResources(task As ScheduleTask)
        Dim selectedNames = ResourceNamesFromAssignment(task.AssignedTo)
        If selectedNames.Count = 0 Then
            task.ResourceAllocations = ""
            task.DailyResourceAllocations = ""
            task.ResourceHours = 0D
            Return
        End If

        Dim selectedSet = New HashSet(Of String)(selectedNames, StringComparer.OrdinalIgnoreCase)
        Dim daily = ParseDailyAllocations(task.DailyResourceAllocations).
            Where(Function(item)
                      Dim resourceName = item.Key.Split({"|"c}, 2, StringSplitOptions.None)(0)
                      Return selectedSet.Contains(resourceName)
                  End Function).
            ToDictionary(Function(item) item.Key, Function(item) item.Value, StringComparer.OrdinalIgnoreCase)

        Dim totals = selectedNames.ToDictionary(Function(name) name, Function(name) 0D, StringComparer.OrdinalIgnoreCase)
        If daily.Count > 0 Then
            For Each item In daily
                Dim resourceName = item.Key.Split({"|"c}, 2, StringSplitOptions.None)(0)
                totals(resourceName) += item.Value
            Next
        Else
            For Each item In ParseResourceAllocations(task.ResourceAllocations)
                If selectedSet.Contains(item.Key) Then
                    totals(item.Key) = item.Value
                End If
            Next
        End If

        task.ResourceAllocations = BuildResourceAllocationsString(totals)
        task.ResourceHours = totals.Values.Sum()
        task.DailyResourceAllocations = BuildDailyAllocationsString(daily)
        If task.ResourceHours > 0D Then
            task.DurationDays = DurationFromHours(task.ResourceHours)
        End If
    End Sub

    Private Function ResourceNamesFromAssignment(value As String) As List(Of String)
        If String.IsNullOrWhiteSpace(value) Then
            Return New List(Of String)()
        End If

        Return value.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(name) name.Trim()).
            Where(Function(name) name.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
    End Function

    Private Sub SyncTaskResourceDistribution(task As ScheduleTask)
        Dim selectedNames = ResourceNamesFromAssignment(task.AssignedTo)
        If selectedNames.Count = 0 Then
            task.ResourceAllocations = ""
            task.DailyResourceAllocations = ""
            Return
        End If

        Dim totals = BuildEvenResourceAllocations(selectedNames, task.ResourceHours)
        task.ResourceAllocations = BuildResourceAllocationsString(totals)
        task.DailyResourceAllocations = BuildDefaultDailyAllocations(task, totals)
    End Sub

    Private Function BuildEvenResourceAllocations(resourceNames As List(Of String), totalHours As Decimal) As Dictionary(Of String, Decimal)
        Dim result As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        If resourceNames Is Nothing OrElse resourceNames.Count = 0 Then
            Return result
        End If

        totalHours = Math.Max(0D, totalHours)
        If totalHours <= 0D Then
            For Each resourceName In resourceNames
                result(resourceName) = 0D
            Next
            Return result
        End If

        Dim remaining = totalHours
        Dim baseShare = Math.Round(totalHours / resourceNames.Count, 2, MidpointRounding.AwayFromZero)
        For index = 0 To resourceNames.Count - 1
            Dim resourceName = resourceNames(index)
            Dim share = If(index = resourceNames.Count - 1, remaining, Math.Min(remaining, baseShare))
            share = Math.Max(0D, Math.Round(share, 2, MidpointRounding.AwayFromZero))
            result(resourceName) = share
            remaining -= share
        Next

        Return result
    End Function

    Private Function ParseResourceAllocations(value As String) As Dictionary(Of String, Decimal)
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

    Private Function BuildResourceAllocationsString(resourceHours As Dictionary(Of String, Decimal)) As String
        Return String.Join("; ", resourceHours.
            Where(Function(item) Not String.IsNullOrWhiteSpace(item.Key) AndAlso item.Value > 0D).
            Select(Function(item) item.Key.Trim() & "=" & item.Value.ToString("0.##", CultureInfo.InvariantCulture)))
    End Function

    Private Sub ScheduleSelectionChanged(sender As Object, e As EventArgs)
        RefreshWorkspaceTabs()
    End Sub

    Private Sub RefreshWorkspaceTabs()
        If _isRefreshingWorkspace Then
            Return
        End If

        If Not IsHandleCreated OrElse Disposing Then
            RefreshWorkspaceTabsCore()
            Return
        End If

        If _workspaceRefreshQueued Then
            Return
        End If

        _workspaceRefreshQueued = True
        BeginInvoke(New Action(
            Sub()
                _workspaceRefreshQueued = False
                RefreshWorkspaceTabsCore()
            End Sub))
    End Sub

    Private Sub RefreshWorkspaceTabsCore()
        If _isRefreshingWorkspace Then
            Return
        End If

        _isRefreshingWorkspace = True
        Try
            PrepareGridForRebuild(_taskUsageGrid)
            PrepareGridForRebuild(_resourceUsageGrid)
            PrepareGridForRebuild(_capacityGrid)

            UpdateTaskUsageGrid()
            UpdateResourceUsageGrid()
            UpdateCapacityPlanningGrid()
            UpdateEmbeddedPlannerPreview()
        Finally
            _isRefreshingWorkspace = False
        End Try
    End Sub

    Private Sub PrepareGridForRebuild(grid As DataGridView)
        If grid Is Nothing OrElse grid.IsDisposed Then
            Return
        End If

        Try
            If grid.IsCurrentCellInEditMode Then
                grid.EndEdit()
            End If
        Catch
        End Try

        Try
            grid.CancelEdit()
        Catch
        End Try

        Try
            If grid.CurrentCell IsNot Nothing Then
                grid.CurrentCell = Nothing
            End If
        Catch
        End Try

        Try
            grid.ClearSelection()
        Catch
        End Try
    End Sub

    Private Sub UpdateEmbeddedPlannerPreview()
        If _plannerPieCharts.Count = 0 Then
            Return
        End If

        Dim taskList = _tasks.ToList()
        Dim duration = taskList.Sum(Function(task) Math.Max(0D, task.DurationDays))

        For Each chart In _plannerPieCharts
            If chart IsNot Nothing Then
                chart.UpdateTasks(taskList)
            End If
        Next

        If _plannerLegendGrid IsNot Nothing Then
            _plannerLegendGrid.DataSource = taskList.Select(Function(task, index) PlannerPreviewRow.FromTask(task, index)).ToList()
        End If

        For Each label In _plannerTaskCountLabels
            If label IsNot Nothing Then
                label.Text = "Scheduled Tasks: " & taskList.Count.ToString(CultureInfo.InvariantCulture)
            End If
        Next

        For Each label In _plannerDurationLabels
            If label IsNot Nothing Then
                label.Text = "Duration: " & duration.ToString("0.##", CultureInfo.InvariantCulture) & " days"
            End If
        Next
    End Sub

    Private Sub PlannerLegendCellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex <> 0 Then
            Return
        End If

        Dim color = PlannerPieChartPanel.SliceColor(e.RowIndex)
        e.CellStyle.BackColor = color
        e.CellStyle.ForeColor = color
        e.Value = ""
        e.FormattingApplied = True
    End Sub

    Private Sub PlannerLegendToolTipTextNeeded(sender As Object, e As DataGridViewCellToolTipTextNeededEventArgs)
        If e.RowIndex < 0 OrElse _plannerLegendGrid Is Nothing Then
            Return
        End If

        Dim row = TryCast(_plannerLegendGrid.Rows(e.RowIndex).DataBoundItem, PlannerPreviewRow)
        If row Is Nothing Then
            Return
        End If

        e.ToolTipText = row.TaskName & " - " & row.DurationText
    End Sub

    Private Sub UpdateResourceUsageGrid()
        If _resourceUsageGrid Is Nothing Then
            Return
        End If

        _isLoadingResourceUsageGrid = True
        Try
            _resourceUsageDateColumns.Clear()
            _resourceUsageGrid.Columns.Clear()
            _resourceUsageGrid.Rows.Clear()

            _resourceUsageGrid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Resource / Task", .Width = 300, .ReadOnly = True, .Frozen = True})
            _resourceUsageGrid.Columns.Add(New CalendarColumn With {.HeaderText = "Start", .Width = 96, .Frozen = True})
            _resourceUsageGrid.Columns.Add(New CalendarColumn With {.HeaderText = "Finish", .Width = 96, .Frozen = True})

            Dim projectStart As Date
            Dim projectFinish As Date
            If Not TryGetProjectDateRange(projectStart, projectFinish) Then
                Return
            End If

            AddDateColumns(_resourceUsageGrid, _resourceUsageDateColumns, projectStart, projectFinish)

            Dim resources = AllAssignedResources()
            If resources.Count = 0 Then
                _resourceUsageGrid.Rows.Add("No resources assigned", "", "")
                Return
            End If

            For Each resourceName In resources
                Dim headerRowIndex = _resourceUsageGrid.Rows.Add()
                Dim headerRow = _resourceUsageGrid.Rows(headerRowIndex)
                headerRow.Tag = New ResourceHeaderRowTag(resourceName)
                headerRow.ReadOnly = True
                headerRow.DefaultCellStyle.BackColor = Color.FromArgb(235, 243, 252)
                headerRow.DefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
                headerRow.Cells(0).Value = resourceName
                headerRow.Cells(1).Value = ""
                headerRow.Cells(2).Value = ""
                For Each dateColumn In _resourceUsageDateColumns
                    Dim workDate = dateColumn.Value
                    Dim totalHours = _tasks.Sum(Function(task) TaskHoursOnDate(task, resourceName, workDate))
                    Dim cell = headerRow.Cells(dateColumn.Key)
                    If IsBlockedScheduleDate(workDate) Then
                        cell.Value = "HOLIDAY"
                        cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                        cell.Style.ForeColor = Color.DimGray
                    Else
                        cell.Value = totalHours
                        StyleUsageSummaryCell(cell, totalHours)
                    End If
                Next

                For Each task In _tasks.Where(Function(item) ResourceNamesFromAssignment(item.AssignedTo).Contains(resourceName, StringComparer.OrdinalIgnoreCase)).
                    OrderBy(Function(item) item.StartDate).
                    ThenBy(Function(item) item.TaskId)
                    Dim rowIndex = _resourceUsageGrid.Rows.Add()
                    Dim row = _resourceUsageGrid.Rows(rowIndex)
                    row.Tag = New ResourceTaskRowTag(resourceName, task.TaskId)
                    row.Cells(0).Value = "   " & task.TaskId.ToString(CultureInfo.InvariantCulture) & ". " & task.TaskName
                    row.Cells(0).ReadOnly = True
                    row.Cells(1).Value = task.StartDate.Date
                    row.Cells(2).Value = task.FinishDate.Date

                    For Each dateColumn In _resourceUsageDateColumns
                        Dim workDate = dateColumn.Value
                        Dim cell = row.Cells(dateColumn.Key)
                        If IsBlockedScheduleDate(workDate) Then
                            cell.Value = "HOLIDAY"
                            cell.ReadOnly = True
                            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                            cell.Style.ForeColor = Color.DimGray
                        Else
                            cell.Value = TaskHoursOnDate(task, resourceName, workDate)
                            cell.ReadOnly = False
                            StyleResourceTaskCell(cell, resourceName, workDate)
                        End If
                    Next
                Next
            Next
        Finally
            _isLoadingResourceUsageGrid = False
        End Try
    End Sub

    Private Sub UpdateCapacityPlanningGrid()
        If _capacityGrid Is Nothing Then
            Return
        End If

        _isLoadingCapacityGrid = True
        Try
            _capacityDateColumns.Clear()
            _capacityGrid.Columns.Clear()
            _capacityGrid.Rows.Clear()

            _capacityGrid.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Resource / Task", .Width = 320, .ReadOnly = True, .Frozen = True})

            Dim projectStart As Date
            Dim projectFinish As Date
            If Not TryGetProjectDateRange(projectStart, projectFinish) Then
                projectStart = Date.Today
                projectFinish = Date.Today
            End If

            AddDateColumns(_capacityGrid, _capacityDateColumns, projectStart.AddDays(-5), projectFinish)

            Dim resources = AllKnownEmployees()
            If resources.Count = 0 Then
                _capacityGrid.Rows.Add("No employees loaded")
                Return
            End If

            For Each resourceName In resources
                Dim availableRowIndex = _capacityGrid.Rows.Add()
                Dim availableRow = _capacityGrid.Rows(availableRowIndex)
                availableRow.Tag = New CapacityAvailableRowTag(resourceName)
                availableRow.ReadOnly = True
                availableRow.Cells(0).Value = resourceName & " - Available"
                availableRow.DefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252)
                availableRow.DefaultCellStyle.ForeColor = Color.FromArgb(37, 47, 63)
                availableRow.DefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Italic)
                For Each dateColumn In _capacityDateColumns
                    Dim workDate = dateColumn.Value
                    Dim cell = availableRow.Cells(dateColumn.Key)
                    If IsBlockedScheduleDate(workDate) Then
                        cell.Value = "HOLIDAY"
                        cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                        cell.Style.ForeColor = Color.DimGray
                    Else
                        Dim plannedHours = _tasks.Sum(Function(task) TaskHoursOnDate(task, resourceName, workDate))
                        Dim availableHours = Math.Max(0D, 8D - plannedHours)
                        cell.Value = availableHours
                        cell.Style.BackColor = If(availableHours <= 0D, Color.FromArgb(255, 238, 200), Color.FromArgb(209, 242, 224))
                        cell.Style.ForeColor = If(availableHours <= 0D, Color.FromArgb(120, 70, 0), Color.FromArgb(22, 101, 52))
                    End If
                    cell.ToolTipText = "Available hours for " & resourceName & " on " & workDate.ToString("dd-MMM-yyyy")
                Next

                Dim plannedRowIndex = _capacityGrid.Rows.Add()
                Dim plannedRow = _capacityGrid.Rows(plannedRowIndex)
                plannedRow.Tag = New CapacityPlannedRowTag(resourceName)
                plannedRow.ReadOnly = True
                plannedRow.Cells(0).Value = resourceName & " - Planned"
                For Each dateColumn In _capacityDateColumns
                    Dim workDate = dateColumn.Value
                    Dim cell = plannedRow.Cells(dateColumn.Key)
                    If IsBlockedScheduleDate(workDate) Then
                        cell.Value = "HOLIDAY"
                    Else
                        Dim plannedHours = _tasks.Sum(Function(task) TaskHoursOnDate(task, resourceName, workDate))
                        cell.Value = plannedHours
                        StyleUsageSummaryCell(cell, plannedHours)
                    End If
                Next

                For Each task In _tasks.Where(Function(item) ResourceNamesFromAssignment(item.AssignedTo).Contains(resourceName, StringComparer.OrdinalIgnoreCase)).
                    OrderBy(Function(item) item.StartDate).
                    ThenBy(Function(item) item.TaskId)
                    Dim rowIndex = _capacityGrid.Rows.Add()
                    Dim row = _capacityGrid.Rows(rowIndex)
                    row.Tag = New ResourceTaskRowTag(resourceName, task.TaskId)
                    row.ReadOnly = True
                    row.Cells(0).Value = "   " & task.TaskId.ToString(CultureInfo.InvariantCulture) & ". " & task.TaskName

                    For Each dateColumn In _capacityDateColumns
                        Dim workDate = dateColumn.Value
                        Dim cell = row.Cells(dateColumn.Key)
                        If IsBlockedScheduleDate(workDate) Then
                            cell.Value = "HOLIDAY"
                            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
                            cell.Style.ForeColor = Color.DimGray
                        Else
                            cell.Value = TaskHoursOnDate(task, resourceName, workDate)
                            StyleCapacityTaskCell(cell, resourceName, workDate)
                        End If
                    Next
                Next
            Next
        Finally
            _isLoadingCapacityGrid = False
        End Try
    End Sub

    Private Sub AddDateColumns(grid As DataGridView, dateColumns As Dictionary(Of Integer, Date), startDate As Date, finishDate As Date)
        Dim currentDate = startDate.Date
        While currentDate <= finishDate.Date
            Dim column As New DataGridViewTextBoxColumn With {
                .HeaderText = currentDate.ToString("dd-MMM"),
                .Width = 88,
                .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "0.##"}
            }
            Dim columnIndex = grid.Columns.Add(column)
            dateColumns(columnIndex) = currentDate
            currentDate = currentDate.AddDays(1)
        End While
    End Sub

    Private Sub StyleTaskViewCell(cell As DataGridViewCell, workDate As Date, task As ScheduleTask)
        If IsBlockedScheduleDate(workDate) Then
            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
            cell.Style.ForeColor = Color.DimGray
            Return
        End If

        Dim hours = DecimalCellValue(cell.Value)
        If workDate >= task.StartDate.Date AndAlso workDate <= task.FinishDate.Date Then
            cell.Style.BackColor = If(hours > 0D, Color.FromArgb(224, 245, 232), Color.FromArgb(244, 248, 252))
        Else
            cell.Style.BackColor = If(hours > 0D, Color.FromArgb(255, 244, 214), Color.White)
        End If
        cell.Style.ForeColor = Color.FromArgb(34, 94, 74)
        cell.ToolTipText = "Task hours on " & workDate.ToString("dd-MMM-yyyy")
    End Sub

    Private Sub StyleUsageSummaryCell(cell As DataGridViewCell, hours As Decimal)
        If hours > 8D Then
            cell.Style.BackColor = Color.FromArgb(255, 210, 210)
            cell.Style.ForeColor = Color.Firebrick
        ElseIf hours > 0D Then
            cell.Style.BackColor = Color.FromArgb(209, 242, 224)
            cell.Style.ForeColor = Color.FromArgb(22, 101, 52)
        Else
            cell.Style.BackColor = Color.FromArgb(245, 248, 252)
            cell.Style.ForeColor = Color.FromArgb(75, 85, 99)
        End If
    End Sub

    Private Sub StyleResourceTaskCell(cell As DataGridViewCell, resourceName As String, workDate As Date)
        If IsBlockedScheduleDate(workDate) Then
            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
            cell.Style.ForeColor = Color.DimGray
            Return
        End If

        Dim hours = DecimalCellValue(cell.Value)
        If hours > 8D Then
            cell.Style.BackColor = Color.FromArgb(255, 210, 210)
            cell.Style.ForeColor = Color.Firebrick
        ElseIf hours > 0D Then
            cell.Style.BackColor = Color.FromArgb(224, 245, 232)
            cell.Style.ForeColor = Color.FromArgb(34, 94, 74)
        Else
            cell.Style.BackColor = Color.White
            cell.Style.ForeColor = Color.FromArgb(75, 85, 99)
        End If
        cell.ToolTipText = resourceName & " on " & workDate.ToString("dd-MMM-yyyy")
    End Sub

    Private Sub StyleCapacityTaskCell(cell As DataGridViewCell, resourceName As String, workDate As Date)
        If IsBlockedScheduleDate(workDate) Then
            cell.Style.BackColor = Color.FromArgb(238, 240, 244)
            cell.Style.ForeColor = Color.DimGray
            Return
        End If

        Dim hours = DecimalCellValue(cell.Value)
        Dim remaining = Math.Max(0D, 8D - hours)
        If hours > 8D Then
            cell.Style.BackColor = Color.FromArgb(255, 210, 210)
            cell.Style.ForeColor = Color.Firebrick
        ElseIf hours > 0D Then
            cell.Style.BackColor = Color.FromArgb(224, 245, 232)
            cell.Style.ForeColor = Color.FromArgb(34, 94, 74)
        Else
            cell.Style.BackColor = Color.White
            cell.Style.ForeColor = Color.FromArgb(75, 85, 99)
        End If
        cell.ToolTipText = resourceName & " planned hours; remaining " & remaining.ToString("0.##", CultureInfo.InvariantCulture) & " hrs"
    End Sub

    Private Function TryGetProjectDateRange(ByRef startDate As Date, ByRef finishDate As Date) As Boolean
        If _tasks.Count = 0 Then
            startDate = Date.Today
            finishDate = Date.Today
            Return False
        End If

        startDate = _tasks.Min(Function(task) task.StartDate).Date
        finishDate = _tasks.Max(Function(task) If(task.FinishDate < task.StartDate, task.StartDate, task.FinishDate)).Date
        If finishDate < startDate Then
            finishDate = startDate
        End If
        Return True
    End Function

    Private Sub CalendarGridCellParsing(sender As Object, e As DataGridViewCellParsingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim grid = TryCast(sender, DataGridView)
        If grid Is _taskUsageGrid OrElse grid Is _resourceUsageGrid Then
            If e.ColumnIndex = 1 OrElse e.ColumnIndex = 2 Then
                Dim parsedDate As Date
                If TryParseGridDate(Convert.ToString(e.Value, CultureInfo.CurrentCulture), parsedDate) Then
                    e.Value = parsedDate.Date
                    e.ParsingApplied = True
                End If
                Return
            End If
        End If

        Dim parsedDecimal As Decimal
        If Decimal.TryParse(Convert.ToString(e.Value, CultureInfo.CurrentCulture), NumberStyles.Number, CultureInfo.CurrentCulture, parsedDecimal) OrElse
            Decimal.TryParse(Convert.ToString(e.Value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, parsedDecimal) Then
            e.Value = Math.Max(0D, parsedDecimal)
            e.ParsingApplied = True
        End If
    End Sub

    Private Sub TaskViewGridCellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If _isLoadingTaskViewGrid OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim row = _taskUsageGrid.Rows(e.RowIndex)
        Dim task = TaskFromWorkspaceRowTag(row.Tag)
        If task Is Nothing Then
            Return
        End If

        If e.ColumnIndex = 1 OrElse e.ColumnIndex = 2 Then
            Dim editedDate As Date
            If Not TryGetEditedDate(row.Cells(e.ColumnIndex).Value, editedDate) Then
                RefreshWorkspaceTabs()
                SetStatus("Please pick a valid date")
                Return
            End If

            If e.ColumnIndex = 1 Then
                task.StartDate = editedDate
                NormalizeGridEdit(task, NameOf(ScheduleTask.StartDate))
            Else
                task.FinishDate = editedDate
                NormalizeGridEdit(task, NameOf(ScheduleTask.FinishDate))
            End If

            RecalculateAndRefresh("Task dates updated")
            Return
        End If

        If Not _taskViewDateColumns.ContainsKey(e.ColumnIndex) Then
            Return
        End If

        Dim workDate = _taskViewDateColumns(e.ColumnIndex)
        Dim requestedHours = Math.Max(0D, DecimalCellValue(row.Cells(e.ColumnIndex).Value))

        Dim resourceTag = TryCast(row.Tag, ResourceTaskRowTag)
        If resourceTag IsNot Nothing Then
            SetTaskResourceHoursOnDate(task, resourceTag.ResourceName, workDate, requestedHours)
            RecalculateAndRefresh("Task usage updated")
            Return
        End If

        SetTaskTotalHoursOnDate(task, workDate, requestedHours)
        RecalculateAndRefresh("Task usage updated")
    End Sub

    Private Sub ResourceUsageGridCellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If _isLoadingResourceUsageGrid OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim row = _resourceUsageGrid.Rows(e.RowIndex)
        Dim tag = TryCast(row.Tag, ResourceTaskRowTag)
        If tag Is Nothing Then
            Return
        End If

        Dim task = _tasks.FirstOrDefault(Function(item) item.TaskId = tag.TaskId)
        If task Is Nothing Then
            Return
        End If

        If e.ColumnIndex = 1 OrElse e.ColumnIndex = 2 Then
            Dim editedDate As Date
            If Not TryGetEditedDate(row.Cells(e.ColumnIndex).Value, editedDate) Then
                RefreshWorkspaceTabs()
                SetStatus("Please pick a valid date")
                Return
            End If

            If e.ColumnIndex = 1 Then
                task.StartDate = editedDate
                NormalizeGridEdit(task, NameOf(ScheduleTask.StartDate))
            Else
                task.FinishDate = editedDate
                NormalizeGridEdit(task, NameOf(ScheduleTask.FinishDate))
            End If

            RecalculateAndRefresh("Resource usage dates updated")
            Return
        End If

        If Not _resourceUsageDateColumns.ContainsKey(e.ColumnIndex) Then
            Return
        End If

        Dim workDate = _resourceUsageDateColumns(e.ColumnIndex)
        Dim requestedHours = Math.Max(0D, DecimalCellValue(row.Cells(e.ColumnIndex).Value))
        SetTaskResourceHoursOnDate(task, tag.ResourceName, workDate, requestedHours)
        RecalculateAndRefresh("Resource usage updated")
    End Sub

    Private Sub CalendarGridDataError(sender As Object, e As DataGridViewDataErrorEventArgs)
        e.ThrowException = False
        SetStatus("Please enter a valid hour value")
    End Sub

    Private Function TaskFromWorkspaceRowTag(tag As Object) As ScheduleTask
        Dim taskTag = TryCast(tag, TaskRowTag)
        If taskTag IsNot Nothing Then
            Return _tasks.FirstOrDefault(Function(item) item.TaskId = taskTag.TaskId)
        End If

        Dim resourceTag = TryCast(tag, ResourceTaskRowTag)
        If resourceTag IsNot Nothing Then
            Return _tasks.FirstOrDefault(Function(item) item.TaskId = resourceTag.TaskId)
        End If

        Return Nothing
    End Function

    Private Function TryGetEditedDate(value As Object, ByRef parsedDate As Date) As Boolean
        If TypeOf value Is Date Then
            parsedDate = CType(value, Date).Date
            Return True
        End If

        Return TryParseGridDate(Convert.ToString(value, CultureInfo.CurrentCulture), parsedDate)
    End Function

    Private Function TaskHoursOnDate(task As ScheduleTask, resourceName As String, workDate As Date) As Decimal
        Dim daily = ParseDailyAllocations(task.DailyResourceAllocations)
        Dim value As Decimal
        If daily.TryGetValue(DailyAllocationKey(resourceName, workDate), value) Then
            Return value
        End If

        Return DistributedTaskHoursOnDate(task, resourceName, workDate)
    End Function

    Private Function DistributedTaskHoursOnDate(task As ScheduleTask, resourceName As String, workDate As Date) As Decimal
        If task Is Nothing OrElse task.ResourceHours <= 0D OrElse workDate < task.StartDate.Date OrElse workDate > task.FinishDate.Date OrElse IsBlockedScheduleDate(workDate) Then
            Return 0D
        End If

        Dim assigned = ParseResourceAllocations(task.ResourceAllocations)
        If assigned.Count = 0 OrElse Not assigned.ContainsKey(resourceName) Then
            Return 0D
        End If

        Dim previousWorkingDays = 0
        Dim cursor = task.StartDate.Date
        While cursor < workDate.Date
            If Not IsBlockedScheduleDate(cursor) Then
                previousWorkingDays += 1
            End If
            cursor = cursor.AddDays(1)
        End While

        Dim remainingForDate = assigned(resourceName) - previousWorkingDays * 8D
        Return ClampDecimal(remainingForDate, 0D, 8D)
    End Function

    Private Function TaskTotalHoursOnDate(task As ScheduleTask, workDate As Date) As Decimal
        If task Is Nothing Then
            Return 0D
        End If

        Return ResourceNamesFromAssignment(task.AssignedTo).
            Sum(Function(resourceName) TaskHoursOnDate(task, resourceName, workDate))
    End Function

    Private Function AllAssignedResources() As List(Of String)
        Return _tasks.
            SelectMany(Function(task) ResourceNamesFromAssignment(task.AssignedTo)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(name) name, StringComparer.OrdinalIgnoreCase).
            ToList()
    End Function

    Private Function AllKnownEmployees() As List(Of String)
        Dim employees = _employees.
            Select(Function(name) If(name, "").Trim()).
            Where(Function(name) name.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(name) name, StringComparer.OrdinalIgnoreCase).
            ToList()

        If employees.Count = 0 Then
            employees = AllAssignedResources()
        End If

        Return employees
    End Function

    Private Sub SetTaskTotalHoursOnDate(task As ScheduleTask, workDate As Date, totalHours As Decimal)
        If task Is Nothing OrElse IsBlockedScheduleDate(workDate) Then
            Return
        End If

        Dim resourceNames = ResourceNamesFromAssignment(task.AssignedTo)
        If resourceNames.Count = 0 Then
            Return
        End If

        Dim daily = ParseDailyAllocations(task.DailyResourceAllocations)
        Dim existing = resourceNames.ToDictionary(
            Function(name) name,
            Function(name)
                Dim value As Decimal = 0D
                daily.TryGetValue(DailyAllocationKey(name, workDate), value)
                Return value
            End Function,
            StringComparer.OrdinalIgnoreCase)

        For Each resourceName In resourceNames
            daily.Remove(DailyAllocationKey(resourceName, workDate))
        Next

        totalHours = Math.Max(0D, totalHours)
        If totalHours > 0D Then
            Dim existingTotal = existing.Values.Sum()
            Dim remaining = totalHours
            For index = 0 To resourceNames.Count - 1
                Dim resourceName = resourceNames(index)
                Dim ratio = If(existingTotal > 0D, existing(resourceName) / existingTotal, 1D / resourceNames.Count)
                Dim share = If(index = resourceNames.Count - 1, remaining, Math.Round(totalHours * CDec(ratio), 2, MidpointRounding.AwayFromZero))
                share = Math.Max(0D, Math.Min(8D, share))
                remaining -= share
                If share > 0D Then
                    daily(DailyAllocationKey(resourceName, workDate)) = share
                End If
            Next
        End If

        ApplyDailyAllocationsToTask(task, daily)
    End Sub

    Private Sub SetTaskResourceHoursOnDate(task As ScheduleTask, resourceName As String, workDate As Date, hours As Decimal)
        If task Is Nothing OrElse String.IsNullOrWhiteSpace(resourceName) OrElse IsBlockedScheduleDate(workDate) Then
            Return
        End If

        Dim daily = ParseDailyAllocations(task.DailyResourceAllocations)
        Dim key = DailyAllocationKey(resourceName, workDate)
        hours = Math.Max(0D, Math.Min(8D, hours))
        If hours > 0D Then
            daily(key) = hours
        Else
            daily.Remove(key)
        End If

        ApplyDailyAllocationsToTask(task, daily)
    End Sub

    Private Sub ApplyDailyAllocationsToTask(task As ScheduleTask, daily As Dictionary(Of String, Decimal))
        Dim selectedNames = ResourceNamesFromAssignment(task.AssignedTo)
        If selectedNames.Count = 0 Then
            _suspendTaskEvents = True
            Try
                task.DailyResourceAllocations = ""
                task.ResourceAllocations = ""
                task.ResourceHours = 0D
                task.DurationDays = 0D
            Finally
                _suspendTaskEvents = False
            End Try
            Return
        End If

        Dim selectedSet = New HashSet(Of String)(selectedNames, StringComparer.OrdinalIgnoreCase)
        Dim filteredDaily = daily.
            Where(Function(item)
                      Dim resourceName = item.Key.Split({"|"c}, 2, StringSplitOptions.None)(0)
                      Return selectedSet.Contains(resourceName) AndAlso item.Value > 0D
                  End Function).
            ToDictionary(Function(item) item.Key, Function(item) item.Value, StringComparer.OrdinalIgnoreCase)

        Dim totals = selectedNames.ToDictionary(Function(name) name, Function(name) 0D, StringComparer.OrdinalIgnoreCase)
        For Each item In filteredDaily
            Dim resourceName = item.Key.Split({"|"c}, 2, StringSplitOptions.None)(0)
            totals(resourceName) += item.Value
        Next

        _suspendTaskEvents = True
        Try
            task.ResourceAllocations = BuildResourceAllocationsString(totals)
            task.ResourceHours = totals.Values.Sum()
            task.DailyResourceAllocations = BuildDailyAllocationsString(filteredDaily)
            task.DurationDays = DurationFromHours(task.ResourceHours)

            Dim activeDates = filteredDaily.Keys.
                Select(Function(key) key.Substring(key.LastIndexOf("|"c) + 1)).
                Select(Function(value) Date.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture)).
                ToList()
            If activeDates.Count > 0 Then
                task.StartDate = activeDates.Min()
                task.FinishDate = activeDates.Max()
            End If
        Finally
            _suspendTaskEvents = False
        End Try
    End Sub

    Private Sub CommitWorkspaceGridEdits()
        Validate()
        _grid.EndEdit()
        If _taskUsageGrid IsNot Nothing Then
            _taskUsageGrid.EndEdit()
        End If
        If _resourceUsageGrid IsNot Nothing Then
            _resourceUsageGrid.EndEdit()
        End If
    End Sub

    Private Function BuildDefaultDailyAllocations(task As ScheduleTask, totals As Dictionary(Of String, Decimal)) As String
        Dim daily = New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        For Each item In totals
            Dim remaining = Math.Max(0D, item.Value)
            Dim workDate = task.StartDate.Date
            While remaining > 0D
                If Not IsBlockedScheduleDate(workDate) Then
                    Dim hours = Math.Min(8D, remaining)
                    daily(DailyAllocationKey(item.Key, workDate)) = hours
                    remaining -= hours
                End If
                workDate = workDate.AddDays(1)
            End While
        Next
        Return BuildDailyAllocationsString(daily)
    End Function

    Private Function ParseDailyAllocations(value As String) As Dictionary(Of String, Decimal)
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

    Private Function BuildDailyAllocationsString(daily As Dictionary(Of String, Decimal)) As String
        Return String.Join("; ", daily.
            Where(Function(item) item.Value > 0D).
            OrderBy(Function(item) item.Key).
            Select(Function(item) item.Key & "=" & item.Value.ToString("0.##", CultureInfo.InvariantCulture)))
    End Function

    Private Function DailyAllocationKey(resourceName As String, workDate As Date) As String
        Return resourceName.Trim() & "|" & workDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    End Function

    Private Function DecimalCellValue(value As Object) As Decimal
        If value Is Nothing Then
            Return 0D
        End If

        If TypeOf value Is Decimal OrElse TypeOf value Is Integer OrElse TypeOf value Is Double Then
            Return Convert.ToDecimal(value, CultureInfo.InvariantCulture)
        End If

        Dim parsed As Decimal
        If Decimal.TryParse(Convert.ToString(value, CultureInfo.CurrentCulture), NumberStyles.Number, CultureInfo.CurrentCulture, parsed) OrElse
            Decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, parsed) Then
            Return parsed
        End If

        Return 0D
    End Function

    Private Function SaveEmployeeWorkspaceWorkbook() As Boolean
        Try
            Dim resourceMonth = ResourceWorkbookMonth()
            Dim filePath = CapacityPlanningWorkbookPath(resourceMonth)
            Directory.CreateDirectory(Path.GetDirectoryName(filePath))
            Dim fileName = Path.GetFileName(filePath)
            _xlsxExportService.ExportResourceMonth(filePath, _projectName.Text, _versionNumber.Text, _employees, _tasks, resourceMonth, _includeSaturdays.Checked)
            SetStatus("Capacity Planning updated: " & fileName)
            Return True
        Catch ex As IOException
            SetStatus("Capacity Planning workbook is open or locked. Close the Excel file and save again.")
        Catch ex As UnauthorizedAccessException
            SetStatus("Capacity Planning workbook could not be saved. Check folder permission.")
        End Try
        Return False
    End Function

    Private Function BuildProjectSnapshot() As ProjectSnapshot
        Return New ProjectSnapshot With {
            .ProjectName = _projectName.Text,
            .VersionNumber = _versionNumber.Text,
            .ProjectSize = If(_projectSizeSelector.SelectedItem Is Nothing, "Small", CStr(_projectSizeSelector.SelectedItem)),
            .ProjectType = _projectType,
            .TotalProjectHours = _totalProjectHours.Value,
            .ResourcesNeeded = CInt(_resourcesNeeded.Value),
            .Tasks = _tasks.ToList(),
            .UpdatedOn = Date.Now
        }
    End Function

    Private Sub SaveProjectSnapshotToLibrary()
        Try
            _projectLibrary.SaveSnapshot(BuildProjectSnapshot())
        Catch ex As IOException
            SetStatus("Project schedule could not be stored in the planner library.")
        Catch ex As UnauthorizedAccessException
            SetStatus("Project schedule could not be stored. Check folder permission.")
        End Try
    End Sub

    Private Function CapacityPlanningWorkbookPath(monthDate As Date) As String
        Dim folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SMA Scheduler", "Capacity Planning")
        Return Path.Combine(folder, "Capacity Planning_" & monthDate.ToString("yyyyMM") & ".xlsx")
    End Function

    Private Function IsScheduleEditColumn(propertyName As String) As Boolean
        Return propertyName = NameOf(ScheduleTask.StartDate) OrElse
            propertyName = NameOf(ScheduleTask.FinishDate) OrElse
            propertyName = NameOf(ScheduleTask.DurationDays) OrElse
            propertyName = NameOf(ScheduleTask.DependencyType) OrElse
            propertyName = NameOf(ScheduleTask.PredecessorLink) OrElse
            propertyName = NameOf(ScheduleTask.AssignedTo) OrElse
            propertyName = NameOf(ScheduleTask.AssignmentDate) OrElse
            propertyName = NameOf(ScheduleTask.ResourceHours)
    End Function

    Private Function SelectedTask() As ScheduleTask
        If _grid.CurrentRow Is Nothing Then
            Return Nothing
        End If
        Return TryCast(_grid.CurrentRow.DataBoundItem, ScheduleTask)
    End Function

    Private Sub SelectTask(taskId As Integer)
        For Each row As DataGridViewRow In _grid.Rows
            Dim task = TryCast(row.DataBoundItem, ScheduleTask)
            If task IsNot Nothing AndAlso task.TaskId = taskId Then
                row.Selected = True
                _grid.CurrentCell = row.Cells(1)
                Exit For
            End If
        Next
    End Sub

    Private Sub RenumberTasks()
        For i = 0 To _tasks.Count - 1
            _tasks(i).TaskId = i + 1
        Next
    End Sub

    Private Sub RemapPredecessorsForCurrentOrder()
        Dim idMap As New Dictionary(Of Integer, Integer)
        For i = 0 To _tasks.Count - 1
            If Not idMap.ContainsKey(_tasks(i).TaskId) Then
                idMap.Add(_tasks(i).TaskId, i + 1)
            End If
        Next

        For Each task In _tasks
            Dim currentNewId = If(idMap.ContainsKey(task.TaskId), idMap(task.TaskId), task.TaskId)
            Dim updatedPredecessors = _engine.ParsePredecessors(task.Predecessors).
                Where(Function(id) idMap.ContainsKey(id)).
                Select(Function(id) idMap(id)).
                Where(Function(id) id <> currentNewId).
                Distinct().
                ToList()
            task.Predecessors = String.Join(",", updatedPredecessors)
        Next
    End Sub

    Private Sub RecalculateAndRefresh(message As String)
        If _isRecalculating Then
            Return
        End If

        Try
            _isRecalculating = True
            _engine.IncludeSaturdays = _includeSaturdays.Checked
            _engine.Recalculate(_tasks)
            _grid.Refresh()
            _gantt.Invalidate()
            UpdateSummary()
            RefreshWorkspaceTabs()
            UpdateEmbeddedPlannerPreview()
            SetStatus(message)
        Finally
            _isRecalculating = False
        End Try
    End Sub

    Private Sub UpdateSummary()
        If _tasks.Count = 0 Then
            _summaryTitle.Text = "0 tasks"
            _summaryDates.Text = "No dates"
            _summaryProgress.Text = "0% complete"
            _summaryResources.Text = "No resources"
            Return
        End If

        Dim startDate = _tasks.Min(Function(t) t.StartDate)
        Dim finishDate = _tasks.Max(Function(t) t.FinishDate)
        Dim progress = CInt(Math.Round(_tasks.Average(Function(t) t.PercentComplete)))
        Dim assignedHours = _tasks.Sum(Function(t) t.ResourceHours)
        Dim resourceCount = _tasks.SelectMany(Function(t) t.ResourceNames.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)).
            Select(Function(r) r.Trim()).
            Where(Function(r) r.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase).
            Count()

        _summaryTitle.Text = _tasks.Count & " tasks"
        _summaryDates.Text = startDate.ToString("dd MMM") & " - " & finishDate.ToString("dd MMM yyyy")
        _summaryProgress.Text = progress & "% complete / " & assignedHours.ToString("0.##") & " hrs"
        _summaryResources.Text = resourceCount & " resources / " & assignedHours.ToString("0.##") & " hrs"
        UpdateRemainingHoursDisplay()
    End Sub

    Private Sub SetStatus(message As String)
        If _status IsNot Nothing Then
            _status.Text = message & "  |  Tasks: " & _tasks.Count & "  |  Updated: " & Date.Now.ToString("HH:mm:ss")
        End If
    End Sub

    Private Function MakeSafeFileName(value As String) As String
        Dim safeValue = If(String.IsNullOrWhiteSpace(value), "SMA Scheduler", value)
        For Each character In Path.GetInvalidFileNameChars()
            safeValue = safeValue.Replace(character, "_"c)
        Next
        Return safeValue
    End Function

    Private Function ClampDecimal(value As Decimal, minimum As Decimal, maximum As Decimal) As Decimal
        Return Math.Min(maximum, Math.Max(minimum, value))
    End Function

    Private Function AvailableHoursForSelection(employeeName As String, currentDate As Date, ignoredTaskId As Integer) As Decimal
        If String.IsNullOrWhiteSpace(employeeName) OrElse IsBlockedScheduleDate(currentDate) Then
            Return 0D
        End If

        Dim currentProjectUsed = 8D - _xlsxExportService.RemainingHours(employeeName, currentDate.Date, _tasks, _includeSaturdays.Checked, ignoredTaskId)
        Dim capacityWorkbookPath = CapacityPlanningWorkbookPath(New Date(currentDate.Year, currentDate.Month, 1))
        Dim currentProjectLabel = _xlsxExportService.ProjectVersionLabel(_projectName.Text, _versionNumber.Text)
        Dim existingExternalUsed = _xlsxExportService.AllocatedHoursFromCapacityPlanning(capacityWorkbookPath, employeeName, currentDate.Date, currentProjectLabel)

        Return Math.Max(0D, 8D - currentProjectUsed - existingExternalUsed)
    End Function

    Private Sub UpdateRemainingHoursDisplay()
        If _remainingHoursLabel Is Nothing OrElse _totalProjectHours Is Nothing Then
            Return
        End If

        Dim remainingHours = Math.Max(0D, _totalProjectHours.Value - _tasks.Sum(Function(task) task.ResourceHours))

        _remainingHoursLabel.Text = "Remaining: " & remainingHours.ToString("0.##") & " hrs"
    End Sub

    Private Function NextTaskStartDate() As Date
        If _tasks.Count = 0 Then
            Return NextWorkingDate(Date.Today)
        End If

        Return NextWorkingDate(_tasks.Max(Function(t) t.FinishDate).AddDays(1))
    End Function

    Private Function PreferredTaskStartDate() As Date
        Return NextTaskStartDate()
    End Function

    Private Function FindAvailableAllocationDate(employeeName As String, proposedDate As Date) As Date
        For offset = 0 To 365
            Dim candidate = proposedDate.Date.AddDays(offset)
            If IsBlockedScheduleDate(candidate) Then
                Continue For
            End If
            If AvailableHoursForSelection(employeeName, candidate, 0) > 0D Then
                Return candidate
            End If
        Next

        Return NextWorkingDate(proposedDate)
    End Function

    Private Function DefaultTaskHours(selectedCatalogTask As TaskCatalogItem) As Decimal
        Dim requestedHours = selectedCatalogTask.HoursForSize(CStr(_projectSizeSelector.SelectedItem))
        If requestedHours > 0D Then
            Return requestedHours
        End If

        Return 8D
    End Function

    Private Function DefaultEmployeeForTask(selectedCatalogTask As TaskCatalogItem) As String
        If selectedCatalogTask IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(selectedCatalogTask.Assignee) AndAlso _employees.Contains(selectedCatalogTask.Assignee) Then
            Return selectedCatalogTask.Assignee
        End If

        If _employees.Count > 0 Then
            Return _employees(0)
        End If

        Return "Unassigned"
    End Function

    Private Function ResourceWorkbookMonth() As Date
        If _tasks.Count > 0 Then
            Dim startDate = _tasks.Min(Function(task) task.StartDate)
            Return New Date(startDate.Year, startDate.Month, 1)
        End If

        Return New Date(Date.Today.Year, Date.Today.Month, 1)
    End Function

    Private Function TryParseGridDate(value As String, ByRef parsedDate As Date) As Boolean
        Dim formats = {"dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy"}
        Return Date.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, parsedDate) OrElse
            Date.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, parsedDate)
    End Function


    Private Sub _grid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles _grid.CellContentClick

    End Sub

    Private NotInheritable Class TaskRowTag
        Public Sub New(taskId As Integer)
            Me.TaskId = taskId
        End Sub

        Public ReadOnly Property TaskId As Integer
    End Class

    Private NotInheritable Class ResourceHeaderRowTag
        Public Sub New(resourceName As String)
            Me.ResourceName = resourceName
        End Sub

        Public ReadOnly Property ResourceName As String
    End Class

    Private NotInheritable Class ResourceTaskRowTag
        Public Sub New(resourceName As String, taskId As Integer)
            Me.ResourceName = resourceName
            Me.TaskId = taskId
        End Sub

        Public ReadOnly Property ResourceName As String
        Public ReadOnly Property TaskId As Integer
    End Class

    Private NotInheritable Class CapacityAvailableRowTag
        Public Sub New(resourceName As String)
            Me.ResourceName = resourceName
        End Sub

        Public ReadOnly Property ResourceName As String
    End Class

    Private NotInheritable Class CapacityPlannedRowTag
        Public Sub New(resourceName As String)
            Me.ResourceName = resourceName
        End Sub

        Public ReadOnly Property ResourceName As String
    End Class

End Class

Public Class TaskUsageRow
    Public Property TaskIdText As String = ""
    Public Property TaskName As String = ""
    Public Property StartDateText As String = ""
    Public Property FinishDateText As String = ""
    Public Property DurationText As String = ""
    Public Property ResourceHoursText As String = ""
    Public Property AssignedTo As String = ""
    Public Property Predecessors As String = ""

    Public Shared Function FromTask(task As ScheduleTask) As TaskUsageRow
        Return New TaskUsageRow With {
            .TaskIdText = task.TaskId.ToString(CultureInfo.InvariantCulture),
            .TaskName = task.TaskName,
            .StartDateText = task.StartDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
            .FinishDateText = task.FinishDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
            .DurationText = task.DurationDays.ToString("0.###", CultureInfo.InvariantCulture),
            .ResourceHoursText = task.ResourceHours.ToString("0.##", CultureInfo.InvariantCulture),
            .AssignedTo = task.AssignedTo,
            .Predecessors = FormatPredecessors(task)
        }
    End Function

    Private Shared Function FormatPredecessors(task As ScheduleTask) As String
        Dim value = If(task.Predecessors, "").Trim()
        If value.Length = 0 Then
            Return ""
        End If
        If value.Any(Function(ch) Char.IsLetter(ch)) Then
            Return value.ToUpperInvariant()
        End If
        Return value & task.DependencyType
    End Function
End Class

Public NotInheritable Class SchedulerThemePreferences
    Private Sub New()
    End Sub

    Private Shared ReadOnly Property PreferenceFile As String
        Get
            Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SMA Scheduler", "theme.txt")
        End Get
    End Property

    Public Shared Function LoadThemeName() As String
        Try
            If File.Exists(PreferenceFile) Then
                Dim savedName = File.ReadAllText(PreferenceFile).Trim()
                If Not String.IsNullOrWhiteSpace(savedName) Then
                    Return savedName
                End If
            End If
        Catch ex As IOException
        Catch ex As UnauthorizedAccessException
        End Try

        Return "Fresh"
    End Function

    Public Shared Sub SaveThemeName(themeName As String)
        If String.IsNullOrWhiteSpace(themeName) Then
            Return
        End If

        Try
            Directory.CreateDirectory(Path.GetDirectoryName(PreferenceFile))
            File.WriteAllText(PreferenceFile, themeName.Trim())
        Catch ex As IOException
        Catch ex As UnauthorizedAccessException
        End Try
    End Sub
End Class

Public Class ProjectSnapshot
    Public Property ProjectName As String = "SMA Scheduler"
    Public Property VersionNumber As String = "1.0"
    Public Property PlannerPlan As String = "SMA Planner"
    Public Property ProjectSize As String = "Small"
    Public Property ProjectType As String = ""
    Public Property TotalProjectHours As Decimal
    Public Property ResourcesNeeded As Integer = 1
    Public Property ResourceHours As Decimal
    Public Property Tasks As New List(Of ScheduleTask)
    Public Property UpdatedOn As Date
End Class

Public Class SchedulerThemePalette
    Public Property Name As String = ""
    Public Property WindowBack As Color
    Public Property PanelBack As Color
    Public Property HeaderBack As Color
    Public Property CommandBack As Color
    Public Property CommandText As Color
    Public Property GridHeader As Color
    Public Property GridLine As Color
    Public Property Selection As Color
    Public Property AlternatingRow As Color
    Public Property Text As Color
    Public Property MutedText As Color
    Public Property Action As Color
    Public Property Divider As Color
    Public Property TileOne As Color
    Public Property TileTwo As Color
    Public Property TileThree As Color
    Public Property TileFour As Color

    Public Shared Function AllNames() As Object()
        Return Themes().Select(Function(theme) CObj(theme.Name)).ToArray()
    End Function

    Public Shared Function ThemeByName(themeName As String) As SchedulerThemePalette
        Dim selected = Themes().FirstOrDefault(Function(theme) String.Equals(theme.Name, themeName, StringComparison.OrdinalIgnoreCase))
        Return If(selected, Themes().First())
    End Function

    Private Shared Function Themes() As List(Of SchedulerThemePalette)
        Return New List(Of SchedulerThemePalette) From {
            New SchedulerThemePalette With {
                .Name = "Fresh",
                .WindowBack = Color.FromArgb(244, 246, 249),
                .PanelBack = Color.White,
                .HeaderBack = Color.FromArgb(229, 241, 255),
                .CommandBack = Color.FromArgb(35, 46, 66),
                .CommandText = Color.White,
                .GridHeader = Color.FromArgb(35, 46, 66),
                .GridLine = Color.FromArgb(232, 236, 242),
                .Selection = Color.FromArgb(219, 235, 255),
                .AlternatingRow = Color.FromArgb(250, 252, 255),
                .Text = Color.FromArgb(24, 31, 42),
                .MutedText = Color.FromArgb(75, 85, 99),
                .Action = Color.FromArgb(32, 164, 112),
                .Divider = Color.FromArgb(224, 229, 236),
                .TileOne = Color.FromArgb(223, 245, 232),
                .TileTwo = Color.FromArgb(255, 243, 205),
                .TileThree = Color.FromArgb(225, 239, 255),
                .TileFour = Color.FromArgb(248, 222, 234)
            },
            New SchedulerThemePalette With {
                .Name = "Sunrise",
                .WindowBack = Color.FromArgb(250, 247, 242),
                .PanelBack = Color.FromArgb(255, 253, 250),
                .HeaderBack = Color.FromArgb(255, 235, 214),
                .CommandBack = Color.FromArgb(95, 55, 50),
                .CommandText = Color.White,
                .GridHeader = Color.FromArgb(95, 55, 50),
                .GridLine = Color.FromArgb(236, 226, 214),
                .Selection = Color.FromArgb(255, 223, 186),
                .AlternatingRow = Color.FromArgb(255, 250, 245),
                .Text = Color.FromArgb(48, 38, 35),
                .MutedText = Color.FromArgb(102, 82, 75),
                .Action = Color.FromArgb(212, 95, 56),
                .Divider = Color.FromArgb(231, 216, 202),
                .TileOne = Color.FromArgb(225, 245, 232),
                .TileTwo = Color.FromArgb(255, 232, 190),
                .TileThree = Color.FromArgb(221, 238, 255),
                .TileFour = Color.FromArgb(255, 218, 226)
            },
            New SchedulerThemePalette With {
                .Name = "Mint",
                .WindowBack = Color.FromArgb(241, 248, 246),
                .PanelBack = Color.White,
                .HeaderBack = Color.FromArgb(215, 242, 235),
                .CommandBack = Color.FromArgb(26, 82, 83),
                .CommandText = Color.White,
                .GridHeader = Color.FromArgb(26, 82, 83),
                .GridLine = Color.FromArgb(220, 236, 234),
                .Selection = Color.FromArgb(202, 238, 230),
                .AlternatingRow = Color.FromArgb(248, 253, 252),
                .Text = Color.FromArgb(25, 43, 45),
                .MutedText = Color.FromArgb(71, 96, 97),
                .Action = Color.FromArgb(42, 157, 143),
                .Divider = Color.FromArgb(205, 225, 223),
                .TileOne = Color.FromArgb(211, 245, 229),
                .TileTwo = Color.FromArgb(255, 241, 194),
                .TileThree = Color.FromArgb(218, 234, 255),
                .TileFour = Color.FromArgb(240, 222, 255)
            },
            New SchedulerThemePalette With {
                .Name = "Graphite",
                .WindowBack = Color.FromArgb(241, 243, 245),
                .PanelBack = Color.FromArgb(252, 252, 253),
                .HeaderBack = Color.FromArgb(232, 235, 239),
                .CommandBack = Color.FromArgb(42, 45, 52),
                .CommandText = Color.White,
                .GridHeader = Color.FromArgb(42, 45, 52),
                .GridLine = Color.FromArgb(224, 228, 234),
                .Selection = Color.FromArgb(225, 233, 246),
                .AlternatingRow = Color.FromArgb(248, 249, 251),
                .Text = Color.FromArgb(24, 27, 32),
                .MutedText = Color.FromArgb(89, 96, 107),
                .Action = Color.FromArgb(50, 132, 120),
                .Divider = Color.FromArgb(212, 218, 226),
                .TileOne = Color.FromArgb(220, 240, 230),
                .TileTwo = Color.FromArgb(255, 239, 201),
                .TileThree = Color.FromArgb(226, 236, 251),
                .TileFour = Color.FromArgb(244, 225, 235)
            }
        }
    End Function
End Class

Public Class BlankNumericUpDown
    Inherits NumericUpDown

    Private _isDisplayBlank As Boolean

    Public Sub ClearDisplayedValue()
        _isDisplayBlank = True
        Text = ""
    End Sub

    Protected Overrides Sub UpdateEditText()
        If _isDisplayBlank Then
            Text = ""
            Return
        End If

        MyBase.UpdateEditText()
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        If _isDisplayBlank Then
            _isDisplayBlank = False
            Text = ""
        End If

        MyBase.OnKeyDown(e)
    End Sub

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        If _isDisplayBlank Then
            _isDisplayBlank = False
            Text = ""
        End If

        MyBase.OnKeyPress(e)
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        If _isDisplayBlank Then
            _isDisplayBlank = False
            UpdateEditText()
        End If

        MyBase.OnMouseDown(e)
    End Sub
End Class

Public Class ResourceChecklistColumn
    Inherits DataGridViewColumn

    Public Sub New(employeeNames As IEnumerable(Of String))
        MyBase.New(New ResourceChecklistCell())
        Me.EmployeeNames = employeeNames.ToList()
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EmployeeNames As List(Of String)

    Public Overrides Function Clone() As Object
        Dim clonedColumn = CType(MyBase.Clone(), ResourceChecklistColumn)
        clonedColumn.EmployeeNames = New List(Of String)(EmployeeNames)
        Return clonedColumn
    End Function

    Public Overrides Property CellTemplate As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            If value IsNot Nothing AndAlso Not value.GetType().IsAssignableFrom(GetType(ResourceChecklistCell)) Then
                Throw New InvalidCastException("ResourceChecklistColumn requires a ResourceChecklistCell template.")
            End If

            MyBase.CellTemplate = value
        End Set
    End Property
End Class

Public Class ResourceChecklistCell
    Inherits DataGridViewTextBoxCell

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
        Dim control = CType(DataGridView.EditingControl, ResourceChecklistEditingControl)
        Dim checklistColumn = TryCast(OwningColumn, ResourceChecklistColumn)
        control.SetEmployees(If(checklistColumn Is Nothing, Enumerable.Empty(Of String)(), checklistColumn.EmployeeNames))
        control.EditingControlFormattedValue = If(Value, "")
    End Sub

    Public Overrides ReadOnly Property EditType As Type
        Get
            Return GetType(ResourceChecklistEditingControl)
        End Get
    End Property

    Public Overrides ReadOnly Property ValueType As Type
        Get
            Return GetType(String)
        End Get
    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue As Object
        Get
            Return ""
        End Get
    End Property
End Class

Public Class ResourceChecklistEditingControl
    Inherits UserControl
    Implements IDataGridViewEditingControl

    Private ReadOnly _textBox As New TextBox()
    Private ReadOnly _dropButton As New Button()
    Private ReadOnly _employees As New List(Of String)()
    Private _dataGridView As DataGridView
    Private _dropDown As ToolStripDropDown
    Private _checkedList As CheckedListBox
    Private _rowIndex As Integer
    Private _valueChanged As Boolean
    Private _isClosingDropDown As Boolean

    Public Sub New()
        BorderStyle = BorderStyle.None
        Margin = Padding.Empty
        Padding = Padding.Empty

        _textBox.BorderStyle = BorderStyle.FixedSingle
        _textBox.Dock = DockStyle.Fill
        _textBox.ReadOnly = True

        _dropButton.Dock = DockStyle.Right
        _dropButton.Width = 24
        _dropButton.Text = "v"
        _dropButton.FlatStyle = FlatStyle.Flat
        _dropButton.FlatAppearance.BorderSize = 0

        Controls.Add(_textBox)
        Controls.Add(_dropButton)

        AddHandler _textBox.Click, Sub() ShowDropDown()
        AddHandler _dropButton.Click, Sub() ShowDropDown()
        AddHandler _textBox.KeyDown, AddressOf EditorKeyDown
        AddHandler _dropButton.KeyDown, AddressOf EditorKeyDown
    End Sub

    Public Sub SetEmployees(employeeNames As IEnumerable(Of String))
        _employees.Clear()
        _employees.AddRange(employeeNames.Where(Function(name) Not String.IsNullOrWhiteSpace(name)).Distinct(StringComparer.OrdinalIgnoreCase))
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlFormattedValue As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return _textBox.Text
        End Get
        Set(value As Object)
            _textBox.Text = NormalizeResourceValue(Convert.ToString(value, CultureInfo.CurrentCulture))
        End Set
    End Property

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return EditingControlFormattedValue
    End Function

    Public Sub ApplyCellStyleToEditingControl(dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        _textBox.Font = dataGridViewCellStyle.Font
        _textBox.ForeColor = dataGridViewCellStyle.ForeColor
        _textBox.BackColor = dataGridViewCellStyle.BackColor
        Font = dataGridViewCellStyle.Font
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlRowIndex As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return _rowIndex
        End Get
        Set(value As Integer)
            _rowIndex = value
        End Set
    End Property

    Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        Select Case keyData And Keys.KeyCode
            Case Keys.Enter, Keys.Space, Keys.Down, Keys.Up, Keys.Left, Keys.Right, Keys.Escape
                Return True
            Case Else
                Return Not dataGridViewWantsInputKey
        End Select
    End Function

    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        BeginInvoke(New MethodInvoker(AddressOf ShowDropDown))
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlDataGridView As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return _dataGridView
        End Get
        Set(value As DataGridView)
            _dataGridView = value
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlValueChanged As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return _valueChanged
        End Get
        Set(value As Boolean)
            _valueChanged = value
        End Set
    End Property

    Public ReadOnly Property EditingPanelCursor As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Cursors.IBeam
        End Get
    End Property

    Private Sub ShowDropDown()
        If _dropDown IsNot Nothing AndAlso Not _dropDown.IsDisposed AndAlso _dropDown.Visible Then
            Return
        End If

        _checkedList = New CheckedListBox With {
            .CheckOnClick = True,
            .BorderStyle = BorderStyle.None,
            .IntegralHeight = False,
            .Width = Math.Max(260, Width),
            .Height = Math.Min(320, Math.Max(120, _employees.Count * 24 + 8))
        }

        Dim selectedNames = SelectedResources(_textBox.Text)
        For Each employeeName In _employees
            _checkedList.Items.Add(employeeName, selectedNames.Contains(employeeName, StringComparer.OrdinalIgnoreCase))
        Next

        AddHandler _checkedList.ItemCheck, AddressOf CheckedListItemCheck
        AddHandler _checkedList.KeyDown, AddressOf EditorKeyDown

        Dim host As New ToolStripControlHost(_checkedList) With {
            .Margin = Padding.Empty,
            .Padding = Padding.Empty,
            .AutoSize = False,
            .Size = _checkedList.Size
        }

        _dropDown = New ToolStripDropDown With {
            .Padding = Padding.Empty,
            .AutoClose = True
        }
        _dropDown.Items.Add(host)
        AddHandler _dropDown.Closed, AddressOf DropDownClosed
        _dropDown.Show(Me, New Point(0, Height))
        _checkedList.Focus()
    End Sub

    Private Sub CheckedListItemCheck(sender As Object, e As ItemCheckEventArgs)
        BeginInvoke(New MethodInvoker(Sub()
                                          UpdateTextFromCheckedList()
                                          MarkDirty()
                                      End Sub))
    End Sub

    Private Sub DropDownClosed(sender As Object, e As ToolStripDropDownClosedEventArgs)
        If _isClosingDropDown Then
            Return
        End If

        _isClosingDropDown = True
        Dim closedDropDown = TryCast(sender, ToolStripDropDown)
        UpdateTextFromCheckedList()
        MarkDirty()

        If closedDropDown IsNot Nothing Then
            RemoveHandler closedDropDown.Closed, AddressOf DropDownClosed
        End If

        If Object.ReferenceEquals(_dropDown, closedDropDown) Then
            _dropDown = Nothing
        End If
        _checkedList = Nothing
        _isClosingDropDown = False
    End Sub

    Private Sub EditorKeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            CommitSelection()
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Escape Then
            If _dropDown IsNot Nothing AndAlso Not _dropDown.IsDisposed Then
                _dropDown.Close()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Space AndAlso Not (_dropDown IsNot Nothing AndAlso Not _dropDown.IsDisposed AndAlso _dropDown.Visible) Then
            ShowDropDown()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CommitSelection()
        UpdateTextFromCheckedList()
        MarkDirty()

        Dim activeDropDown = _dropDown
        If activeDropDown IsNot Nothing AndAlso Not activeDropDown.IsDisposed Then
            _dropDown = Nothing
            activeDropDown.Close()
        End If

        If _dataGridView IsNot Nothing Then
            _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
            _dataGridView.EndEdit()
        End If
    End Sub

    Private Sub UpdateTextFromCheckedList()
        If _checkedList Is Nothing Then
            Return
        End If

        _textBox.Text = String.Join("; ", _checkedList.CheckedItems.Cast(Of Object)().Select(Function(item) CStr(item)))
    End Sub

    Private Sub MarkDirty()
        _valueChanged = True
        If _dataGridView IsNot Nothing Then
            _dataGridView.NotifyCurrentCellDirty(True)
        End If
    End Sub

    Private Function NormalizeResourceValue(value As String) As String
        Return String.Join("; ", SelectedResources(value))
    End Function

    Private Function SelectedResources(value As String) As List(Of String)
        If String.IsNullOrWhiteSpace(value) Then
            Return New List(Of String)()
        End If

        Return value.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(name) name.Trim()).
            Where(Function(name) name.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
    End Function
End Class

Public Class CalendarColumn
    Inherits DataGridViewColumn

    Public Sub New()
        MyBase.New(New CalendarCell())
    End Sub

    Public Overrides Property CellTemplate As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            If value IsNot Nothing AndAlso Not value.GetType().IsAssignableFrom(GetType(CalendarCell)) Then
                Throw New InvalidCastException("CalendarColumn requires a CalendarCell template.")
            End If

            MyBase.CellTemplate = value
        End Set
    End Property
End Class

Public Class CalendarCell
    Inherits DataGridViewTextBoxCell

    Public Sub New()
        Style.Format = "dd-MM-yyyy"
    End Sub

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
        Dim control = CType(DataGridView.EditingControl, CalendarEditingControl)
        If Value Is Nothing OrElse Value Is DBNull.Value Then
            control.Value = Date.Today
        Else
            control.Value = CDate(Value)
        End If
    End Sub

    Public Overrides ReadOnly Property EditType As Type
        Get
            Return GetType(CalendarEditingControl)
        End Get
    End Property

    Public Overrides ReadOnly Property ValueType As Type
        Get
            Return GetType(Date)
        End Get
    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue As Object
        Get
            Return Date.Today
        End Get
    End Property
End Class

Public Class CalendarEditingControl
    Inherits DateTimePicker
    Implements IDataGridViewEditingControl

    Private _dataGridView As DataGridView
    Private _valueChanged As Boolean
    Private _rowIndex As Integer

    Public Sub New()
        Format = DateTimePickerFormat.Custom
        CustomFormat = "dd-MM-yyyy"
        ShowUpDown = False
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlFormattedValue As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
        End Get
        Set(value As Object)
            Dim parsedDate As Date
            If value IsNot Nothing AndAlso Date.TryParse(CStr(value), parsedDate) Then
                Value = parsedDate
            End If
        End Set
    End Property

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return EditingControlFormattedValue
    End Function

    Public Sub ApplyCellStyleToEditingControl(dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Font = dataGridViewCellStyle.Font
        CalendarForeColor = dataGridViewCellStyle.ForeColor
        CalendarMonthBackground = dataGridViewCellStyle.BackColor
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlRowIndex As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return _rowIndex
        End Get
        Set(value As Integer)
            _rowIndex = value
        End Set
    End Property

    Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        Select Case keyData And Keys.KeyCode
            Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp, Keys.F4, Keys.Enter, Keys.Escape, Keys.Tab
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        BeginInvoke(New MethodInvoker(Sub() SendKeys.SendWait("%{DOWN}")))
    End Sub

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        e.Handled = True
        MyBase.OnKeyPress(e)
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp, Keys.F4, Keys.Enter, Keys.Escape, Keys.Tab
                MyBase.OnKeyDown(e)
            Case Else
                e.Handled = True
                e.SuppressKeyPress = True
        End Select
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlDataGridView As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return _dataGridView
        End Get
        Set(value As DataGridView)
            _dataGridView = value
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EditingControlValueChanged As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return _valueChanged
        End Get
        Set(value As Boolean)
            _valueChanged = value
        End Set
    End Property

    Public ReadOnly Property EditingPanelCursor As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return MyBase.Cursor
        End Get
    End Property

    Protected Overrides Sub OnValueChanged(eventargs As EventArgs)
        _valueChanged = True
        If _dataGridView IsNot Nothing Then
            _dataGridView.NotifyCurrentCellDirty(True)
        End If

        MyBase.OnValueChanged(eventargs)
    End Sub
End Class

Public Class GanttPanel
    Inherits Panel

    Private ReadOnly _motionTimer As Timer
    Private _motionPhase As Single

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Tasks As BindingList(Of ScheduleTask)

    Public Sub New()
        DoubleBuffered = True
        BackColor = Color.White
        AutoScroll = True
        _motionTimer = New Timer With {.Interval = 45}
        AddHandler _motionTimer.Tick, AddressOf MotionTimerTick
        _motionTimer.Start()
    End Sub

    Private Sub MotionTimerTick(sender As Object, e As EventArgs)
        If IsDisposed Then
            _motionTimer.Stop()
            Return
        End If

        _motionPhase += 0.035F
        If _motionPhase > 1.0F Then
            _motionPhase -= 1.0F
        End If
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        If Tasks Is Nothing OrElse Tasks.Count = 0 Then
            TextRenderer.DrawText(e.Graphics, "No tasks", Font, New Point(24, 24), Color.DimGray)
            Return
        End If

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim minDate = Tasks.Min(Function(t) t.StartDate).Date
        Dim maxDate = Tasks.Max(Function(t) t.FinishDate).Date
        Dim totalDays = Math.Max(1, CInt((maxDate - minDate).TotalDays) + 1)
        Dim dayWidth = Math.Max(20, Math.Min(38, (ClientSize.Width - 110) \ Math.Max(1, totalDays)))
        Dim left = 96
        Dim top = 68
        Dim rowHeight = 34
        Dim chartWidth = left + totalDays * dayWidth + 160
        Dim chartHeight = top + Tasks.Count * rowHeight + 64
        If AutoScrollMinSize.Width <> chartWidth OrElse AutoScrollMinSize.Height <> chartHeight Then
            AutoScrollMinSize = New Size(chartWidth, chartHeight)
        End If

        Dim scrollOffset = AutoScrollPosition
        e.Graphics.TranslateTransform(scrollOffset.X, scrollOffset.Y)
        Dim visibleLeft = -scrollOffset.X
        Dim visibleTop = -scrollOffset.Y

        Using headerBrush As New SolidBrush(Color.FromArgb(248, 250, 252)),
              weekendBrush As New SolidBrush(Color.FromArgb(245, 247, 250)),
              gridPen As New Pen(Color.FromArgb(226, 230, 235)),
              todayPen As New Pen(Color.FromArgb(220, 53, 69), 2),
              barBrush As New SolidBrush(Color.FromArgb(43, 120, 228)),
              milestoneBrush As New SolidBrush(Color.FromArgb(255, 152, 0)),
              progressBrush As New SolidBrush(Color.FromArgb(32, 156, 110)),
              dependencyPen As New Pen(Color.FromArgb(88, 101, 125), 1.4F)

            e.Graphics.FillRectangle(headerBrush, visibleLeft, visibleTop, ClientSize.Width, top - 8)
            dependencyPen.CustomEndCap = New AdjustableArrowCap(4, 5)

            Using monthFont As New Font(Font, FontStyle.Bold)
                Dim monthStart = New Date(minDate.Year, minDate.Month, 1)
                While monthStart <= maxDate
                    Dim monthEnd = monthStart.AddMonths(1).AddDays(-1)
                    Dim segmentStart = If(monthStart < minDate, minDate, monthStart)
                    Dim segmentEnd = If(monthEnd > maxDate, maxDate, monthEnd)

                    Dim startOffset = CInt((segmentStart - minDate).TotalDays)
                    Dim endOffset = CInt((segmentEnd - minDate).TotalDays) + 1
                    Dim monthRectangle = New Rectangle(left + startOffset * dayWidth, 8, Math.Max(dayWidth, (endOffset - startOffset) * dayWidth), 22)
                    If monthRectangle.Width >= 64 Then
                        TextRenderer.DrawText(e.Graphics, monthStart.ToString("MMM yyyy"), monthFont, monthRectangle, Color.FromArgb(30, 36, 48), TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
                    ElseIf monthRectangle.Width >= 34 Then
                        TextRenderer.DrawText(e.Graphics, monthStart.ToString("MMM"), monthFont, monthRectangle, Color.FromArgb(30, 36, 48), TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
                    End If

                    monthStart = monthStart.AddMonths(1)
                End While
            End Using

            For day = 0 To totalDays - 1
                Dim dateValue = minDate.AddDays(day)
                Dim x = left + day * dayWidth
                If dateValue.DayOfWeek = DayOfWeek.Saturday OrElse dateValue.DayOfWeek = DayOfWeek.Sunday Then
                    e.Graphics.FillRectangle(weekendBrush, x, top - 8, dayWidth, chartHeight - top + 8)
                End If
                e.Graphics.DrawLine(gridPen, x, top - 8, x, chartHeight)
                If dayWidth >= 18 Then
                    TextRenderer.DrawText(e.Graphics, dateValue.ToString("dd"), Font, New Rectangle(x, 34, dayWidth, 20), Color.DimGray, TextFormatFlags.HorizontalCenter)
                End If
            Next

            Dim todayOffset = CInt((Date.Today - minDate).TotalDays)
            If todayOffset >= 0 AndAlso todayOffset <= totalDays Then
                Dim todayX = left + todayOffset * dayWidth
                e.Graphics.DrawLine(todayPen, todayX, top - 8, todayX, chartHeight)
            End If
            Dim bars As New Dictionary(Of Integer, Rectangle)
            For i = 0 To Tasks.Count - 1
                Dim task = Tasks(i)
                Dim y = top + i * rowHeight
                Dim x = left + CInt((task.StartDate.Date - minDate).TotalDays) * dayWidth
                Dim width = Math.Max(dayWidth, CInt(Math.Ceiling(task.DurationDays)) * dayWidth)
                Dim bar = New Rectangle(x, y + 9, width, 16)
                bars(task.TaskId) = bar

                e.Graphics.DrawLine(gridPen, visibleLeft, y, Math.Max(chartWidth, visibleLeft + ClientSize.Width), y)
                TextRenderer.DrawText(e.Graphics, task.TaskId.ToString(), Font, New Rectangle(8, y + 5, 34, 24), Color.DimGray, TextFormatFlags.Right)
                TextRenderer.DrawText(e.Graphics, task.TaskName, Font, New Rectangle(48, y + 5, Math.Max(20, left - 54), 24), Color.FromArgb(45, 48, 56), TextFormatFlags.EndEllipsis)

                If task.TaskName.StartsWith("Milestone: ") Then
                    DrawMilestone(e.Graphics, milestoneBrush, New Point(bar.Left + 8, bar.Top + 8))
                Else
                    Dim progressWidth = CInt(width * (task.PercentComplete / 100.0R))
                    e.Graphics.FillRoundedRectangle(barBrush, bar, 4)
                    DrawMovingBarHighlight(e.Graphics, bar)
                    If progressWidth > 0 Then
                        e.Graphics.FillRoundedRectangle(progressBrush, New Rectangle(bar.X, bar.Y, progressWidth, bar.Height), 4)
                    End If
                End If

                TextRenderer.DrawText(e.Graphics, task.PercentComplete & "%", Font, New Rectangle(bar.Right + 6, y + 4, 46, 24), Color.DimGray)
            Next

            Dim parser As New ScheduleEngine()
            For Each task In Tasks
                If Not bars.ContainsKey(task.TaskId) Then
                    Continue For
                End If

                For Each predecessorId In parser.ParsePredecessors(task.Predecessors)
                    If bars.ContainsKey(predecessorId) Then
                        Dim fromBar = bars(predecessorId)
                        Dim toBar = bars(task.TaskId)
                        Dim fromPoint = New Point(fromBar.Right, fromBar.Top + fromBar.Height \ 2)
                        Dim toPoint = New Point(toBar.Left, toBar.Top + toBar.Height \ 2)
                        e.Graphics.DrawLine(dependencyPen, fromPoint, toPoint)
                    End If
                Next
            Next
        End Using
    End Sub

    Private Sub DrawMilestone(graphics As Graphics, brush As Brush, center As Point)
        Dim points = {
            New Point(center.X, center.Y - 9),
            New Point(center.X + 9, center.Y),
            New Point(center.X, center.Y + 9),
            New Point(center.X - 9, center.Y)
        }
        graphics.FillPolygon(brush, points)
    End Sub

    Private Sub DrawMovingBarHighlight(graphics As Graphics, bar As Rectangle)
        If bar.Width < 18 OrElse bar.Height < 8 Then
            Return
        End If

        Dim sweepWidth = Math.Max(14, Math.Min(34, bar.Width \ 2))
        Dim travel = bar.Width + sweepWidth * 2
        Dim sweepLeft = bar.Left - sweepWidth + CInt(travel * _motionPhase)
        Dim sweepBounds = New Rectangle(sweepLeft, bar.Top, sweepWidth, bar.Height)

        Using path As New GraphicsPath()
            Dim radius = 8
            path.AddArc(bar.Left, bar.Top, radius, radius, 180, 90)
            path.AddArc(bar.Right - radius, bar.Top, radius, radius, 270, 90)
            path.AddArc(bar.Right - radius, bar.Bottom - radius, radius, radius, 0, 90)
            path.AddArc(bar.Left, bar.Bottom - radius, radius, radius, 90, 90)
            path.CloseFigure()

            Dim previousClip = graphics.Clip
            graphics.SetClip(path, CombineMode.Intersect)
            Using brush As New LinearGradientBrush(sweepBounds, Color.FromArgb(0, Color.White), Color.FromArgb(105, Color.White), LinearGradientMode.Horizontal)
                Dim blend As New Blend With {
                    .Positions = New Single() {0.0F, 0.5F, 1.0F},
                    .Factors = New Single() {0.0F, 1.0F, 0.0F}
                }
                brush.Blend = blend
                graphics.FillRectangle(brush, sweepBounds)
            End Using
            graphics.Clip = previousClip
        End Using
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso _motionTimer IsNot Nothing Then
            _motionTimer.Stop()
            RemoveHandler _motionTimer.Tick, AddressOf MotionTimerTick
            _motionTimer.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class

Public Module GraphicsExtensions
    <Runtime.CompilerServices.Extension>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, bounds As Rectangle, radius As Integer)
        Using path As New GraphicsPath()
            Dim diameter = radius * 2
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90)
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90)
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90)
            path.CloseFigure()
            graphics.FillPath(brush, path)
        End Using
    End Sub
End Module


Public Class PlannerPieChartPanel
    Inherits Panel

    Private _tasks As List(Of ScheduleTask)
    Private ReadOnly _motionTimer As System.Windows.Forms.Timer
    Private _animationProgress As Single = 1.0F
    Private _pulsePhase As Single = 0.0F
    Private _taskSignature As String = ""

    Private Shared ReadOnly SliceColors As Color() = {
        Color.FromArgb(45, 125, 221),
        Color.FromArgb(32, 164, 112),
        Color.FromArgb(245, 158, 11),
        Color.FromArgb(236, 72, 153),
        Color.FromArgb(99, 102, 241),
        Color.FromArgb(20, 184, 166),
        Color.FromArgb(239, 68, 68),
        Color.FromArgb(132, 204, 22)
    }

    Public Sub New(tasks As IEnumerable(Of ScheduleTask))
        DoubleBuffered = True
        _tasks = tasks.OrderBy(Function(task) task.TaskId).ToList()
        _taskSignature = BuildTaskSignature(_tasks)
        _motionTimer = New System.Windows.Forms.Timer With {.Interval = 35}
        AddHandler _motionTimer.Tick, AddressOf MotionTimerTick
        ResetMotion()
    End Sub

    Public Shared Function SliceColor(index As Integer) As Color
        Return SliceColors(index Mod SliceColors.Length)
    End Function

    Public Sub UpdateTasks(tasks As IEnumerable(Of ScheduleTask))
        Dim nextTasks = tasks.OrderBy(Function(task) task.TaskId).ToList()
        Dim nextSignature = BuildTaskSignature(nextTasks)
        Dim taskSetChanged = Not String.Equals(_taskSignature, nextSignature, StringComparison.Ordinal)
        _tasks = nextTasks
        _taskSignature = nextSignature
        If taskSetChanged Then
            ResetMotion()
        End If
        Invalidate()
    End Sub

    Private Shared Function BuildTaskSignature(tasks As IEnumerable(Of ScheduleTask)) As String
        Return String.Join("|", tasks.Select(Function(task) task.TaskId.ToString(CultureInfo.InvariantCulture) & ":" & task.TaskName & ":" & task.DurationDays.ToString("0.###", CultureInfo.InvariantCulture)))
    End Function

    Private Sub ResetMotion()
        _animationProgress = 0.0F
        _pulsePhase = 0.0F
        If _motionTimer IsNot Nothing AndAlso Not _motionTimer.Enabled Then
            _motionTimer.Start()
        End If
        Invalidate()
    End Sub

    Private Sub MotionTimerTick(sender As Object, e As EventArgs)
        If IsDisposed Then
            _motionTimer.Stop()
            Return
        End If

        If _animationProgress < 1.0F Then
            _animationProgress = Math.Min(1.0F, _animationProgress + 0.045F)
        End If
        _pulsePhase += 0.08F
        If _pulsePhase > CSng(Math.PI * 2) Then
            _pulsePhase -= CSng(Math.PI * 2)
        End If

        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.Clear(Color.White)

        Dim totalDuration = _tasks.Sum(Function(task) Math.Max(0.01F, CSng(task.DurationDays)))
        If totalDuration <= 0 Then
            DrawEmpty(e.Graphics)
            Return
        End If

        Dim compact = ClientSize.Height < 260 OrElse ClientSize.Width < 340
        Dim titleHeight = If(compact, 0, 28)
        Dim topPadding = If(compact, 0, 36)
        Dim bottomPadding = If(compact, 0, 10)
        Dim sidePadding = If(compact, 0, 24)
        Dim drawingHeight = Math.Max(1, ClientSize.Height - topPadding - bottomPadding)
        Dim diameter = Math.Min(ClientSize.Width - (sidePadding * 2), drawingHeight)

        If diameter < 28 Then
            DrawCenteredText(e.Graphics, _tasks.Count.ToString(CultureInfo.InvariantCulture) & " tasks", New Font("Segoe UI Semibold", 9.0F), Color.FromArgb(24, 31, 42), ClientRectangle)
            Return
        End If

        Dim bounds As New Rectangle((ClientSize.Width - diameter) \ 2, topPadding + ((drawingHeight - diameter) \ 2), Math.Max(1, diameter - 2), Math.Max(1, diameter - 2))

        Dim startAngle As Single = -90.0F
        Dim remainingSweep As Single = 360.0F * EaseOutCubic(_animationProgress)
        For i = 0 To _tasks.Count - 1
            Dim sweep = CSng(Math.Max(0.01D, _tasks(i).DurationDays) / CDec(totalDuration) * 360D)
            Dim visibleSweep = Math.Min(sweep, remainingSweep)
            If visibleSweep <= 0 Then
                Exit For
            End If
            Using brush As New SolidBrush(SliceColor(i))
                e.Graphics.FillPie(brush, bounds, startAngle, visibleSweep)
            End Using
            startAngle += sweep
            remainingSweep -= visibleSweep
        Next

        Using pen As New Pen(Color.White, 2.0F)
            e.Graphics.DrawEllipse(pen, bounds)
        End Using

        Dim pulseSize = CInt(Math.Round((Math.Sin(_pulsePhase) + 1.0R) * 2.0R))
        Dim pulseBounds = Rectangle.Inflate(bounds, pulseSize, pulseSize)
        Dim pulseAlpha = CInt(32 + ((Math.Sin(_pulsePhase) + 1.0R) * 16.0R))
        Using pen As New Pen(Color.FromArgb(pulseAlpha, 45, 125, 221), 2.0F)
            e.Graphics.DrawEllipse(pen, pulseBounds)
        End Using

        Dim center = New Point(bounds.Left + bounds.Width \ 2, bounds.Top + bounds.Height \ 2)
        Dim innerDiameter = Math.Max(28, CInt(diameter * 0.55))
        Dim innerBounds As New Rectangle(center.X - innerDiameter \ 2, center.Y - innerDiameter \ 2, innerDiameter, innerDiameter)
        Using brush As New SolidBrush(Color.White)
            e.Graphics.FillEllipse(brush, innerBounds)
        End Using

        Dim title = _tasks.Count.ToString(CultureInfo.InvariantCulture)
        Dim subtitle = totalDuration.ToString("0.##", CultureInfo.InvariantCulture) & " days"
        Dim titleFontSize = CSng(Math.Max(9.0R, Math.Min(28.0R, diameter / 5.6R)))
        Dim subtitleFontSize = CSng(Math.Max(7.0R, Math.Min(12.0R, diameter / 15.5R)))
        Dim centerTitleTop = innerBounds.Top + CInt(innerBounds.Height * If(compact, 0.16R, 0.24R))
        DrawCenteredText(e.Graphics, title, New Font("Segoe UI Semibold", titleFontSize), Color.FromArgb(24, 31, 42), New Rectangle(innerBounds.Left, centerTitleTop, innerBounds.Width, Math.Max(18, CInt(innerBounds.Height * 0.28R))))
        If diameter >= 62 Then
            DrawCenteredText(e.Graphics, subtitle, New Font("Segoe UI", subtitleFontSize), Color.FromArgb(75, 85, 99), New Rectangle(innerBounds.Left, centerTitleTop + Math.Max(18, CInt(innerBounds.Height * 0.25R)), innerBounds.Width, Math.Max(16, CInt(innerBounds.Height * 0.22R))))
        End If
        If Not compact Then
            DrawCenteredText(e.Graphics, "Task duration split", New Font("Segoe UI Semibold", 12.0F), Color.FromArgb(24, 31, 42), New Rectangle(0, 4, ClientSize.Width, titleHeight))
        End If
    End Sub

    Private Shared Function EaseOutCubic(value As Single) As Single
        Dim progress = Math.Max(0.0F, Math.Min(1.0F, value))
        Dim inverse = 1.0F - progress
        Return 1.0F - (inverse * inverse * inverse)
    End Function

    Private Sub DrawEmpty(graphics As Graphics)
        DrawCenteredText(graphics, "No scheduled tasks", New Font("Segoe UI Semibold", 13.0F), Color.DimGray, ClientRectangle)
    End Sub

    Private Sub DrawCenteredText(graphics As Graphics, text As String, font As Font, color As Color, bounds As Rectangle)
        Using brush As New SolidBrush(color)
            Using format As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                graphics.DrawString(text, font, brush, bounds, format)
            End Using
        End Using
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso _motionTimer IsNot Nothing Then
            _motionTimer.Stop()
            RemoveHandler _motionTimer.Tick, AddressOf MotionTimerTick
            _motionTimer.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class

Public Class PlannerPreviewRow
    Public Property ColorName As String = ""
    Public Property TaskName As String = ""
    Public Property DurationText As String = ""
    Public Property DateRange As String = ""

    Public Shared Function FromTask(task As ScheduleTask, index As Integer) As PlannerPreviewRow
        Return New PlannerPreviewRow With {
            .ColorName = "",
            .TaskName = task.TaskName,
            .DurationText = task.DurationDays.ToString("0.##", CultureInfo.InvariantCulture) & " days",
            .DateRange = task.StartDate.ToString("dd-MMM", CultureInfo.InvariantCulture) & " - " & task.FinishDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
        }
    End Function
End Class
