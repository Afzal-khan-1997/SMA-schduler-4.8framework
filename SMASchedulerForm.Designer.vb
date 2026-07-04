<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SMASchedulerForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.commandBar = New System.Windows.Forms.ToolStrip()
        Me.btnSave = New System.Windows.Forms.ToolStripButton()
        Me.btnRefreshCapacity = New System.Windows.Forms.ToolStripButton()
        Me.sepFile = New System.Windows.Forms.ToolStripSeparator()
        Me.btnAddTask = New System.Windows.Forms.ToolStripButton()
        Me.btnDelete = New System.Windows.Forms.ToolStripButton()
        Me.btnMoveUp = New System.Windows.Forms.ToolStripButton()
        Me.btnMoveDown = New System.Windows.Forms.ToolStripButton()
        Me.sepTasks = New System.Windows.Forms.ToolStripSeparator()
        Me.btnLink = New System.Windows.Forms.ToolStripButton()
        Me.btnUnlink = New System.Windows.Forms.ToolStripButton()
        Me.btnMilestone = New System.Windows.Forms.ToolStripButton()
        Me.sepTheme = New System.Windows.Forms.ToolStripSeparator()
        Me.btnChangeTheme = New System.Windows.Forms.ToolStripButton()
        Me.btnNew = New System.Windows.Forms.ToolStripButton()
        Me.btnTaskUsage = New System.Windows.Forms.ToolStripButton()
        Me.btnResourceUsage = New System.Windows.Forms.ToolStripButton()
        Me.headerPanel = New System.Windows.Forms.Panel()
        Me.appTitle = New System.Windows.Forms.Label()
        Me.projectLabel = New System.Windows.Forms.Label()
        Me._projectName = New System.Windows.Forms.TextBox()
        Me.versionLabel = New System.Windows.Forms.Label()
        Me._versionNumber = New System.Windows.Forms.TextBox()
        Me.totalHoursLabel = New System.Windows.Forms.Label()
        Me._totalProjectHours = New SMAScheduler.BlankNumericUpDown()
        Me.projectSizeLabel = New System.Windows.Forms.Label()
        Me._projectSizeSelector = New System.Windows.Forms.ComboBox()
        Me._includeSaturdays = New System.Windows.Forms.CheckBox()
        Me._summaryTitle = New System.Windows.Forms.Label()
        Me._summaryDates = New System.Windows.Forms.Label()
        Me._summaryProgress = New System.Windows.Forms.Label()
        Me._summaryResources = New System.Windows.Forms.Label()
        Me.taskCatalogLabel = New System.Windows.Forms.Label()
        Me._taskCatalogSelector = New System.Windows.Forms.ComboBox()
        Me.resourcesNeededLabel = New System.Windows.Forms.Label()
        Me._resourcesNeeded = New SMAScheduler.BlankNumericUpDown()
        Me._remainingHoursLabel = New System.Windows.Forms.Label()
        Me.contentSplit = New System.Windows.Forms.SplitContainer()
        Me._workspaceTabs = New System.Windows.Forms.TabControl()
        Me.taskAllocationTab = New System.Windows.Forms.TabPage()
        Me.mainSplit = New System.Windows.Forms.SplitContainer()
        Me._grid = New System.Windows.Forms.DataGridView()
        Me.ganttPreviewSplit = New System.Windows.Forms.SplitContainer()
        Me._gantt = New SMAScheduler.GanttPanel()
        Me.allocationPreviewPanel = New System.Windows.Forms.Panel()
        Me.allocationPreviewTitle = New System.Windows.Forms.Label()
        Me.allocationPreviewBadges = New System.Windows.Forms.FlowLayoutPanel()
        Me.allocationPrimaryLabel = New System.Windows.Forms.Label()
        Me.allocationSecondaryLabel = New System.Windows.Forms.Label()
        Me.allocationPreviewBodySplit = New System.Windows.Forms.SplitContainer()
        Me.allocationPreviewChart = New SMAScheduler.PlannerPieChartPanel()
        Me.allocationLegendGrid = New System.Windows.Forms.DataGridView()
        Me.taskUsageTab = New System.Windows.Forms.TabPage()
        Me.taskUsageSplit = New System.Windows.Forms.SplitContainer()
        Me._taskUsageGrid = New System.Windows.Forms.DataGridView()
        Me.taskUsagePreviewPanel = New System.Windows.Forms.Panel()
        Me.taskUsagePreviewTitle = New System.Windows.Forms.Label()
        Me.taskUsagePreviewBadges = New System.Windows.Forms.FlowLayoutPanel()
        Me.taskUsagePrimaryLabel = New System.Windows.Forms.Label()
        Me.taskUsageSecondaryLabel = New System.Windows.Forms.Label()
        Me.taskUsagePreviewBodySplit = New System.Windows.Forms.SplitContainer()
        Me.taskUsagePreviewChart = New SMAScheduler.PlannerPieChartPanel()
        Me.taskUsageLegendGrid = New System.Windows.Forms.DataGridView()
        Me.resourceUsageTab = New System.Windows.Forms.TabPage()
        Me.resourceUsageSplit = New System.Windows.Forms.SplitContainer()
        Me._resourceUsageGrid = New System.Windows.Forms.DataGridView()
        Me.resourceUsagePreviewPanel = New System.Windows.Forms.Panel()
        Me.resourceUsagePreviewTitle = New System.Windows.Forms.Label()
        Me.resourceUsagePreviewBadges = New System.Windows.Forms.FlowLayoutPanel()
        Me.resourceUsagePrimaryLabel = New System.Windows.Forms.Label()
        Me.resourceUsageSecondaryLabel = New System.Windows.Forms.Label()
        Me.resourceUsagePreviewBodySplit = New System.Windows.Forms.SplitContainer()
        Me.resourceUsagePreviewChart = New SMAScheduler.PlannerPieChartPanel()
        Me.resourceUsageLegendGrid = New System.Windows.Forms.DataGridView()
        Me.capacityPlanningTab = New System.Windows.Forms.TabPage()
        Me._capacityGrid = New System.Windows.Forms.DataGridView()
        Me.resourceUtilizationTab = New System.Windows.Forms.TabPage()
        Me.resourceUtilizationHost = New System.Windows.Forms.Panel()
        Me.resourceUtilizationToolbar = New System.Windows.Forms.FlowLayoutPanel()
        Me._resourceUtilizationRefreshButton = New System.Windows.Forms.Button()
        Me._resourceUtilizationColorSelector = New System.Windows.Forms.ComboBox()
        Me._resourceUtilizationApplyButton = New System.Windows.Forms.Button()
        Me._resourceUtilizationClearButton = New System.Windows.Forms.Button()
        Me._resourceUtilizationMailButton = New System.Windows.Forms.Button()
        Me._resourceUtilizationGrid = New System.Windows.Forms.DataGridView()
        Me._detailsPanel = New System.Windows.Forms.Panel()
        Me.taskWorkspaceTitle = New System.Windows.Forms.Label()
        Me.statusBar = New System.Windows.Forms.StatusStrip()
        Me._status = New System.Windows.Forms.ToolStripStatusLabel()
        Me.commandBar.SuspendLayout()
        Me.headerPanel.SuspendLayout()
        CType(Me._totalProjectHours, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me._resourcesNeeded, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.contentSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.contentSplit.Panel1.SuspendLayout()
        Me.contentSplit.Panel2.SuspendLayout()
        Me.contentSplit.SuspendLayout()
        Me._workspaceTabs.SuspendLayout()
        Me.taskAllocationTab.SuspendLayout()
        CType(Me.mainSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mainSplit.Panel1.SuspendLayout()
        Me.mainSplit.Panel2.SuspendLayout()
        Me.mainSplit.SuspendLayout()
        CType(Me._grid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ganttPreviewSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ganttPreviewSplit.Panel1.SuspendLayout()
        Me.ganttPreviewSplit.Panel2.SuspendLayout()
        Me.ganttPreviewSplit.SuspendLayout()
        Me.allocationPreviewPanel.SuspendLayout()
        Me.allocationPreviewBadges.SuspendLayout()
        CType(Me.allocationPreviewBodySplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.allocationPreviewBodySplit.Panel1.SuspendLayout()
        Me.allocationPreviewBodySplit.Panel2.SuspendLayout()
        Me.allocationPreviewBodySplit.SuspendLayout()
        CType(Me.allocationLegendGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.taskUsageTab.SuspendLayout()
        CType(Me.taskUsageSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.taskUsageSplit.Panel1.SuspendLayout()
        Me.taskUsageSplit.Panel2.SuspendLayout()
        Me.taskUsageSplit.SuspendLayout()
        CType(Me._taskUsageGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.taskUsagePreviewPanel.SuspendLayout()
        Me.taskUsagePreviewBadges.SuspendLayout()
        CType(Me.taskUsagePreviewBodySplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.taskUsagePreviewBodySplit.Panel1.SuspendLayout()
        Me.taskUsagePreviewBodySplit.Panel2.SuspendLayout()
        Me.taskUsagePreviewBodySplit.SuspendLayout()
        CType(Me.taskUsageLegendGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.resourceUsageTab.SuspendLayout()
        CType(Me.resourceUsageSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.resourceUsageSplit.Panel1.SuspendLayout()
        Me.resourceUsageSplit.Panel2.SuspendLayout()
        Me.resourceUsageSplit.SuspendLayout()
        CType(Me._resourceUsageGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.resourceUsagePreviewPanel.SuspendLayout()
        Me.resourceUsagePreviewBadges.SuspendLayout()
        CType(Me.resourceUsagePreviewBodySplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.resourceUsagePreviewBodySplit.Panel1.SuspendLayout()
        Me.resourceUsagePreviewBodySplit.Panel2.SuspendLayout()
        Me.resourceUsagePreviewBodySplit.SuspendLayout()
        CType(Me.resourceUsageLegendGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.capacityPlanningTab.SuspendLayout()
        CType(Me._capacityGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.resourceUtilizationTab.SuspendLayout()
        Me.resourceUtilizationHost.SuspendLayout()
        Me.resourceUtilizationToolbar.SuspendLayout()
        CType(Me._resourceUtilizationGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._detailsPanel.SuspendLayout()
        Me.statusBar.SuspendLayout()
        Me.SuspendLayout()
        '
        'commandBar
        '
        Me.commandBar.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.commandBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.commandBar.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.commandBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnSave, Me.btnRefreshCapacity, Me.sepFile, Me.btnAddTask, Me.btnDelete, Me.btnMoveUp, Me.btnMoveDown, Me.sepTasks, Me.btnLink, Me.btnUnlink, Me.btnMilestone})
        Me.commandBar.Location = New System.Drawing.Point(0, 0)
        Me.commandBar.Name = "commandBar"
        Me.commandBar.Padding = New System.Windows.Forms.Padding(11, 9, 11, 9)
        Me.commandBar.Size = New System.Drawing.Size(1577, 45)
        Me.commandBar.TabIndex = 0
        '
        'btnSave
        '
        Me.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnSave.ForeColor = System.Drawing.Color.White
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(44, 24)
        Me.btnSave.Text = "Save"
        '
        'btnRefreshCapacity
        '
        Me.btnRefreshCapacity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnRefreshCapacity.ForeColor = System.Drawing.Color.White
        Me.btnRefreshCapacity.Name = "btnRefreshCapacity"
        Me.btnRefreshCapacity.Size = New System.Drawing.Size(184, 24)
        Me.btnRefreshCapacity.Text = "Refresh Capacity Planning"
        '
        'sepFile
        '
        Me.sepFile.Name = "sepFile"
        Me.sepFile.Size = New System.Drawing.Size(6, 27)
        '
        'btnAddTask
        '
        Me.btnAddTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnAddTask.ForeColor = System.Drawing.Color.White
        Me.btnAddTask.Name = "btnAddTask"
        Me.btnAddTask.Size = New System.Drawing.Size(72, 24)
        Me.btnAddTask.Text = "Add Task"
        '
        'btnDelete
        '
        Me.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnDelete.ForeColor = System.Drawing.Color.White
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(57, 24)
        Me.btnDelete.Text = "Delete"
        '
        'btnMoveUp
        '
        Me.btnMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnMoveUp.ForeColor = System.Drawing.Color.White
        Me.btnMoveUp.Name = "btnMoveUp"
        Me.btnMoveUp.Size = New System.Drawing.Size(73, 24)
        Me.btnMoveUp.Text = "Move Up"
        '
        'btnMoveDown
        '
        Me.btnMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnMoveDown.ForeColor = System.Drawing.Color.White
        Me.btnMoveDown.Name = "btnMoveDown"
        Me.btnMoveDown.Size = New System.Drawing.Size(93, 24)
        Me.btnMoveDown.Text = "Move Down"
        '
        'sepTasks
        '
        Me.sepTasks.Name = "sepTasks"
        Me.sepTasks.Size = New System.Drawing.Size(6, 27)
        '
        'btnLink
        '
        Me.btnLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnLink.ForeColor = System.Drawing.Color.White
        Me.btnLink.Name = "btnLink"
        Me.btnLink.Size = New System.Drawing.Size(39, 24)
        Me.btnLink.Text = "Link"
        '
        'btnUnlink
        '
        Me.btnUnlink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnUnlink.ForeColor = System.Drawing.Color.White
        Me.btnUnlink.Name = "btnUnlink"
        Me.btnUnlink.Size = New System.Drawing.Size(54, 24)
        Me.btnUnlink.Text = "Unlink"
        '
        'btnMilestone
        '
        Me.btnMilestone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnMilestone.ForeColor = System.Drawing.Color.White
        Me.btnMilestone.Name = "btnMilestone"
        Me.btnMilestone.Size = New System.Drawing.Size(78, 24)
        Me.btnMilestone.Text = "Milestone"
        '
        'sepTheme
        '
        Me.sepTheme.Name = "sepTheme"
        Me.sepTheme.Size = New System.Drawing.Size(6, 27)
        '
        'btnChangeTheme
        '
        Me.btnChangeTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnChangeTheme.ForeColor = System.Drawing.Color.White
        Me.btnChangeTheme.Name = "btnChangeTheme"
        Me.btnChangeTheme.Size = New System.Drawing.Size(112, 24)
        Me.btnChangeTheme.Text = "Change Theme"
        '
        'btnNew
        '
        Me.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnNew.ForeColor = System.Drawing.Color.White
        Me.btnNew.Name = "btnNew"
        Me.btnNew.Size = New System.Drawing.Size(43, 24)
        Me.btnNew.Text = "New"
        '
        'btnTaskUsage
        '
        Me.btnTaskUsage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnTaskUsage.ForeColor = System.Drawing.Color.White
        Me.btnTaskUsage.Name = "btnTaskUsage"
        Me.btnTaskUsage.Size = New System.Drawing.Size(84, 24)
        Me.btnTaskUsage.Text = "Task Usage"
        '
        'btnResourceUsage
        '
        Me.btnResourceUsage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnResourceUsage.ForeColor = System.Drawing.Color.White
        Me.btnResourceUsage.Name = "btnResourceUsage"
        Me.btnResourceUsage.Size = New System.Drawing.Size(112, 24)
        Me.btnResourceUsage.Text = "Resource Usage"
        '
        'headerPanel
        '
        Me.headerPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(229, Byte), Integer), CType(CType(241, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.headerPanel.Controls.Add(Me.appTitle)
        Me.headerPanel.Controls.Add(Me.projectLabel)
        Me.headerPanel.Controls.Add(Me._projectName)
        Me.headerPanel.Controls.Add(Me.versionLabel)
        Me.headerPanel.Controls.Add(Me._versionNumber)
        Me.headerPanel.Controls.Add(Me.totalHoursLabel)
        Me.headerPanel.Controls.Add(Me._totalProjectHours)
        Me.headerPanel.Controls.Add(Me.projectSizeLabel)
        Me.headerPanel.Controls.Add(Me._projectSizeSelector)
        Me.headerPanel.Controls.Add(Me._includeSaturdays)
        Me.headerPanel.Controls.Add(Me._summaryTitle)
        Me.headerPanel.Controls.Add(Me._summaryDates)
        Me.headerPanel.Controls.Add(Me._summaryProgress)
        Me.headerPanel.Controls.Add(Me._summaryResources)
        Me.headerPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.headerPanel.Location = New System.Drawing.Point(0, 45)
        Me.headerPanel.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.headerPanel.Name = "headerPanel"
        Me.headerPanel.Padding = New System.Windows.Forms.Padding(18, 16, 18, 16)
        Me.headerPanel.Size = New System.Drawing.Size(1577, 252)
        Me.headerPanel.TabIndex = 1
        '
        'appTitle
        '
        Me.appTitle.AutoSize = True
        Me.appTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 16.0!)
        Me.appTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.appTitle.Location = New System.Drawing.Point(0, 0)
        Me.appTitle.Name = "appTitle"
        Me.appTitle.Size = New System.Drawing.Size(203, 37)
        Me.appTitle.TabIndex = 0
        Me.appTitle.Text = "SMA Scheduler"
        '
        'projectLabel
        '
        Me.projectLabel.AutoSize = True
        Me.projectLabel.ForeColor = System.Drawing.Color.DimGray
        Me.projectLabel.Location = New System.Drawing.Point(13, 42)
        Me.projectLabel.Name = "projectLabel"
        Me.projectLabel.Size = New System.Drawing.Size(55, 20)
        Me.projectLabel.TabIndex = 1
        Me.projectLabel.Text = "Project"
        '
        '_projectName
        '
        Me._projectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._projectName.Location = New System.Drawing.Point(13, 66)
        Me._projectName.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._projectName.Name = "_projectName"
        Me._projectName.Size = New System.Drawing.Size(263, 27)
        Me._projectName.TabIndex = 2
        Me._projectName.Text = "SMA Scheduler"
        '
        'versionLabel
        '
        Me.versionLabel.AutoSize = True
        Me.versionLabel.ForeColor = System.Drawing.Color.DimGray
        Me.versionLabel.Location = New System.Drawing.Point(291, 42)
        Me.versionLabel.Name = "versionLabel"
        Me.versionLabel.Size = New System.Drawing.Size(57, 20)
        Me.versionLabel.TabIndex = 15
        Me.versionLabel.Text = "Version"
        '
        '_versionNumber
        '
        Me._versionNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._versionNumber.Location = New System.Drawing.Point(291, 66)
        Me._versionNumber.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._versionNumber.Name = "_versionNumber"
        Me._versionNumber.Size = New System.Drawing.Size(86, 27)
        Me._versionNumber.TabIndex = 16
        Me._versionNumber.Text = "1.0"
        '
        'totalHoursLabel
        '
        Me.totalHoursLabel.AutoSize = True
        Me.totalHoursLabel.ForeColor = System.Drawing.Color.DimGray
        Me.totalHoursLabel.Location = New System.Drawing.Point(412, 42)
        Me.totalHoursLabel.Name = "totalHoursLabel"
        Me.totalHoursLabel.Size = New System.Drawing.Size(135, 20)
        Me.totalHoursLabel.TabIndex = 9
        Me.totalHoursLabel.Text = "Total Project Hours"
        '
        '_totalProjectHours
        '
        Me._totalProjectHours.DecimalPlaces = 1
        Me._totalProjectHours.Location = New System.Drawing.Point(412, 66)
        Me._totalProjectHours.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._totalProjectHours.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me._totalProjectHours.Name = "_totalProjectHours"
        Me._totalProjectHours.Size = New System.Drawing.Size(149, 27)
        Me._totalProjectHours.TabIndex = 10
        Me._totalProjectHours.ThousandsSeparator = True
        '
        'projectSizeLabel
        '
        Me.projectSizeLabel.AutoSize = True
        Me.projectSizeLabel.ForeColor = System.Drawing.Color.DimGray
        Me.projectSizeLabel.Location = New System.Drawing.Point(13, 108)
        Me.projectSizeLabel.Name = "projectSizeLabel"
        Me.projectSizeLabel.Size = New System.Drawing.Size(86, 20)
        Me.projectSizeLabel.TabIndex = 3
        Me.projectSizeLabel.Text = "Project Size"
        '
        '_projectSizeSelector
        '
        Me._projectSizeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._projectSizeSelector.FormattingEnabled = True
        Me._projectSizeSelector.Location = New System.Drawing.Point(13, 132)
        Me._projectSizeSelector.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._projectSizeSelector.Name = "_projectSizeSelector"
        Me._projectSizeSelector.Size = New System.Drawing.Size(134, 28)
        Me._projectSizeSelector.TabIndex = 4
        '
        '_includeSaturdays
        '
        Me._includeSaturdays.AutoSize = True
        Me._includeSaturdays.Location = New System.Drawing.Point(166, 132)
        Me._includeSaturdays.Name = "_includeSaturdays"
        Me._includeSaturdays.Size = New System.Drawing.Size(124, 24)
        Me._includeSaturdays.TabIndex = 20
        Me._includeSaturdays.Text = "Weekend Plan"
        Me._includeSaturdays.UseVisualStyleBackColor = True
        '
        '_summaryTitle
        '
        Me._summaryTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(232, Byte), Integer))
        Me._summaryTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me._summaryTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._summaryTitle.Location = New System.Drawing.Point(13, 170)
        Me._summaryTitle.Name = "_summaryTitle"
        Me._summaryTitle.Size = New System.Drawing.Size(189, 56)
        Me._summaryTitle.TabIndex = 5
        Me._summaryTitle.Text = "0 tasks"
        Me._summaryTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        '_summaryDates
        '
        Me._summaryDates.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(243, Byte), Integer), CType(CType(205, Byte), Integer))
        Me._summaryDates.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me._summaryDates.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._summaryDates.Location = New System.Drawing.Point(216, 170)
        Me._summaryDates.Name = "_summaryDates"
        Me._summaryDates.Size = New System.Drawing.Size(206, 56)
        Me._summaryDates.TabIndex = 6
        Me._summaryDates.Text = "No dates"
        Me._summaryDates.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        '_summaryProgress
        '
        Me._summaryProgress.BackColor = System.Drawing.Color.FromArgb(CType(CType(225, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(255, Byte), Integer))
        Me._summaryProgress.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me._summaryProgress.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._summaryProgress.Location = New System.Drawing.Point(442, 170)
        Me._summaryProgress.Name = "_summaryProgress"
        Me._summaryProgress.Size = New System.Drawing.Size(171, 56)
        Me._summaryProgress.TabIndex = 7
        Me._summaryProgress.Text = "0% complete"
        Me._summaryProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me._summaryProgress.Visible = False
        '
        '_summaryResources
        '
        Me._summaryResources.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(222, Byte), Integer), CType(CType(234, Byte), Integer))
        Me._summaryResources.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me._summaryResources.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._summaryResources.Location = New System.Drawing.Point(442, 170)
        Me._summaryResources.Name = "_summaryResources"
        Me._summaryResources.Size = New System.Drawing.Size(224, 56)
        Me._summaryResources.TabIndex = 8
        Me._summaryResources.Text = "No resources"
        Me._summaryResources.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'taskCatalogLabel
        '
        Me.taskCatalogLabel.AutoSize = True
        Me.taskCatalogLabel.ForeColor = System.Drawing.Color.DimGray
        Me.taskCatalogLabel.Location = New System.Drawing.Point(13, 108)
        Me.taskCatalogLabel.Name = "taskCatalogLabel"
        Me.taskCatalogLabel.Size = New System.Drawing.Size(147, 20)
        Me.taskCatalogLabel.TabIndex = 1
        Me.taskCatalogLabel.Text = "Database Task Name"
        '
        '_taskCatalogSelector
        '
        Me._taskCatalogSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._taskCatalogSelector.FormattingEnabled = True
        Me._taskCatalogSelector.Location = New System.Drawing.Point(13, 132)
        Me._taskCatalogSelector.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._taskCatalogSelector.Name = "_taskCatalogSelector"
        Me._taskCatalogSelector.Size = New System.Drawing.Size(427, 24)
        Me._taskCatalogSelector.TabIndex = 2
        '
        'resourcesNeededLabel
        '
        Me.resourcesNeededLabel.AutoSize = True
        Me.resourcesNeededLabel.ForeColor = System.Drawing.Color.DimGray
        Me.resourcesNeededLabel.Location = New System.Drawing.Point(596, 56)
        Me.resourcesNeededLabel.Name = "resourcesNeededLabel"
        Me.resourcesNeededLabel.Size = New System.Drawing.Size(132, 20)
        Me.resourcesNeededLabel.TabIndex = 11
        Me.resourcesNeededLabel.Text = "Resources Needed"
        '
        '_resourcesNeeded
        '
        Me._resourcesNeeded.Location = New System.Drawing.Point(596, 80)
        Me._resourcesNeeded.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._resourcesNeeded.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        Me._resourcesNeeded.Name = "_resourcesNeeded"
        Me._resourcesNeeded.Size = New System.Drawing.Size(137, 22)
        Me._resourcesNeeded.TabIndex = 12
        '
        '_remainingHoursLabel
        '
        Me._remainingHoursLabel.AutoSize = True
        Me._remainingHoursLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._remainingHoursLabel.Location = New System.Drawing.Point(810, 122)
        Me._remainingHoursLabel.Name = "_remainingHoursLabel"
        Me._remainingHoursLabel.Size = New System.Drawing.Size(118, 20)
        Me._remainingHoursLabel.TabIndex = 21
        Me._remainingHoursLabel.Text = "Remaining: 8 hrs"
        '
        'contentSplit
        '
        Me.contentSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(229, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.contentSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.contentSplit.Location = New System.Drawing.Point(0, 297)
        Me.contentSplit.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.contentSplit.Name = "contentSplit"
        Me.contentSplit.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'contentSplit.Panel1
        '
        Me.contentSplit.Panel1.Controls.Add(Me._workspaceTabs)
        Me.contentSplit.Panel1MinSize = 430
        '
        'contentSplit.Panel2
        '
        Me.contentSplit.Panel2.Controls.Add(Me._detailsPanel)
        Me.contentSplit.Panel2Collapsed = True
        Me.contentSplit.Panel2MinSize = 130
        Me.contentSplit.Size = New System.Drawing.Size(1577, 732)
        Me.contentSplit.SplitterDistance = 430
        Me.contentSplit.SplitterWidth = 5
        Me.contentSplit.TabIndex = 2
        '
        '_workspaceTabs
        '
        Me._workspaceTabs.Controls.Add(Me.taskAllocationTab)
        Me._workspaceTabs.Controls.Add(Me.taskUsageTab)
        Me._workspaceTabs.Controls.Add(Me.resourceUsageTab)
        Me._workspaceTabs.Controls.Add(Me.capacityPlanningTab)
        Me._workspaceTabs.Controls.Add(Me.resourceUtilizationTab)
        Me._workspaceTabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me._workspaceTabs.Location = New System.Drawing.Point(0, 0)
        Me._workspaceTabs.Name = "_workspaceTabs"
        Me._workspaceTabs.Padding = New System.Drawing.Point(18, 6)
        Me._workspaceTabs.SelectedIndex = 0
        Me._workspaceTabs.Size = New System.Drawing.Size(1577, 732)
        Me._workspaceTabs.TabIndex = 0
        '
        'taskAllocationTab
        '
        Me.taskAllocationTab.BackColor = System.Drawing.Color.White
        Me.taskAllocationTab.Controls.Add(Me.mainSplit)
        Me.taskAllocationTab.Location = New System.Drawing.Point(4, 32)
        Me.taskAllocationTab.Name = "taskAllocationTab"
        Me.taskAllocationTab.Padding = New System.Windows.Forms.Padding(3)
        Me.taskAllocationTab.Size = New System.Drawing.Size(1569, 696)
        Me.taskAllocationTab.TabIndex = 0
        Me.taskAllocationTab.Text = "Task Allocation"
        'mainSplit
        '
        Me.mainSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(229, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mainSplit.Location = New System.Drawing.Point(3, 3)
        Me.mainSplit.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.mainSplit.Name = "mainSplit"
        '
        'mainSplit.Panel1
        '
        Me.mainSplit.Panel1.Controls.Add(Me._grid)
        Me.mainSplit.Panel1MinSize = 620
        '
        'mainSplit.Panel2
        '
        Me.mainSplit.Panel2.Controls.Add(Me.ganttPreviewSplit)
        Me.mainSplit.Panel2MinSize = 380
        Me.mainSplit.Size = New System.Drawing.Size(1563, 690)
        Me.mainSplit.SplitterDistance = 934
        Me.mainSplit.SplitterWidth = 5
        Me.mainSplit.TabIndex = 0
        '
        '_grid
        '
        Me._grid.AllowUserToAddRows = False
        Me._grid.AllowUserToDeleteRows = False
        Me._grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me._grid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me._grid.BackgroundColor = System.Drawing.Color.White
        Me._grid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._grid.ColumnHeadersHeight = 34
        Me._grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._grid.EnableHeadersVisualStyles = False
        Me._grid.GridColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(242, Byte), Integer))
        Me._grid.Location = New System.Drawing.Point(0, 0)
        Me._grid.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._grid.MultiSelect = False
        Me._grid.Name = "_grid"
        Me._grid.RowHeadersVisible = False
        Me._grid.RowHeadersWidth = 51
        Me._grid.RowTemplate.Height = 30
        Me._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me._grid.Size = New System.Drawing.Size(934, 690)
        Me._grid.TabIndex = 0
        '
        'ganttPreviewSplit
        '
        Me.ganttPreviewSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(229, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.ganttPreviewSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ganttPreviewSplit.Location = New System.Drawing.Point(0, 0)
        Me.ganttPreviewSplit.Name = "ganttPreviewSplit"
        Me.ganttPreviewSplit.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'ganttPreviewSplit.Panel1
        '
        Me.ganttPreviewSplit.Panel1.Controls.Add(Me._gantt)
        Me.ganttPreviewSplit.Panel1MinSize = 260
        '
        'ganttPreviewSplit.Panel2
        '
        Me.ganttPreviewSplit.Panel2.Controls.Add(Me.allocationPreviewPanel)
        Me.ganttPreviewSplit.Panel2MinSize = 200
        Me.ganttPreviewSplit.Size = New System.Drawing.Size(624, 690)
        Me.ganttPreviewSplit.SplitterDistance = 420
        Me.ganttPreviewSplit.SplitterWidth = 5
        Me.ganttPreviewSplit.TabIndex = 0
        '
        '_gantt
        '
        Me._gantt.AutoScroll = True
        Me._gantt.BackColor = System.Drawing.Color.White
        Me._gantt.Dock = System.Windows.Forms.DockStyle.Fill
        Me._gantt.Location = New System.Drawing.Point(0, 0)
        Me._gantt.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._gantt.Name = "_gantt"
        Me._gantt.Size = New System.Drawing.Size(624, 420)
        Me._gantt.TabIndex = 0
        '
        'allocationPreviewPanel
        '
        Me.allocationPreviewPanel.BackColor = System.Drawing.Color.White
        Me.allocationPreviewPanel.Controls.Add(Me.allocationPreviewBodySplit)
        Me.allocationPreviewPanel.Controls.Add(Me.allocationPreviewBadges)
        Me.allocationPreviewPanel.Controls.Add(Me.allocationPreviewTitle)
        Me.allocationPreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.allocationPreviewPanel.Location = New System.Drawing.Point(0, 0)
        Me.allocationPreviewPanel.Name = "allocationPreviewPanel"
        Me.allocationPreviewPanel.Padding = New System.Windows.Forms.Padding(8)
        Me.allocationPreviewPanel.Size = New System.Drawing.Size(624, 265)
        Me.allocationPreviewPanel.TabIndex = 0
        '
        'allocationPreviewTitle
        '
        Me.allocationPreviewTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.allocationPreviewTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
        Me.allocationPreviewTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.allocationPreviewTitle.Location = New System.Drawing.Point(8, 8)
        Me.allocationPreviewTitle.Name = "allocationPreviewTitle"
        Me.allocationPreviewTitle.Size = New System.Drawing.Size(608, 28)
        Me.allocationPreviewTitle.TabIndex = 0
        Me.allocationPreviewTitle.Text = "Planner Preview - Resources Used"
        '
        'allocationPreviewBadges
        '
        Me.allocationPreviewBadges.Controls.Add(Me.allocationPrimaryLabel)
        Me.allocationPreviewBadges.Controls.Add(Me.allocationSecondaryLabel)
        Me.allocationPreviewBadges.Dock = System.Windows.Forms.DockStyle.Top
        Me.allocationPreviewBadges.Location = New System.Drawing.Point(8, 36)
        Me.allocationPreviewBadges.Name = "allocationPreviewBadges"
        Me.allocationPreviewBadges.Padding = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.allocationPreviewBadges.Size = New System.Drawing.Size(608, 34)
        Me.allocationPreviewBadges.TabIndex = 1
        Me.allocationPreviewBadges.WrapContents = False
        '
        'allocationPrimaryLabel
        '
        Me.allocationPrimaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.allocationPrimaryLabel.Location = New System.Drawing.Point(3, 3)
        Me.allocationPrimaryLabel.Name = "allocationPrimaryLabel"
        Me.allocationPrimaryLabel.Size = New System.Drawing.Size(170, 27)
        Me.allocationPrimaryLabel.TabIndex = 0
        Me.allocationPrimaryLabel.Text = "Resources Selected: 3"
        Me.allocationPrimaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'allocationSecondaryLabel
        '
        Me.allocationSecondaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(234, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.allocationSecondaryLabel.Location = New System.Drawing.Point(179, 3)
        Me.allocationSecondaryLabel.Name = "allocationSecondaryLabel"
        Me.allocationSecondaryLabel.Size = New System.Drawing.Size(170, 27)
        Me.allocationSecondaryLabel.TabIndex = 1
        Me.allocationSecondaryLabel.Text = "Assigned Hours: 24 hrs"
        Me.allocationSecondaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'allocationPreviewBodySplit
        '
        Me.allocationPreviewBodySplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.allocationPreviewBodySplit.Location = New System.Drawing.Point(8, 70)
        Me.allocationPreviewBodySplit.Name = "allocationPreviewBodySplit"
        '
        'allocationPreviewBodySplit.Panel1
        '
        Me.allocationPreviewBodySplit.Panel1.Controls.Add(Me.allocationPreviewChart)
        '
        'allocationPreviewBodySplit.Panel2
        '
        Me.allocationPreviewBodySplit.Panel2.Controls.Add(Me.allocationLegendGrid)
        Me.allocationPreviewBodySplit.Size = New System.Drawing.Size(608, 187)
        Me.allocationPreviewBodySplit.SplitterDistance = 260
        Me.allocationPreviewBodySplit.SplitterWidth = 4
        Me.allocationPreviewBodySplit.TabIndex = 2
        '
        'allocationPreviewChart
        '
        Me.allocationPreviewChart.BackColor = System.Drawing.Color.White
        Me.allocationPreviewChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.allocationPreviewChart.Location = New System.Drawing.Point(0, 0)
        Me.allocationPreviewChart.Name = "allocationPreviewChart"
        Me.allocationPreviewChart.PreviewMode = SMAScheduler.PlannerPreviewMode.ResourcesUsed
        Me.allocationPreviewChart.Size = New System.Drawing.Size(260, 187)
        Me.allocationPreviewChart.TabIndex = 0
        '
        'allocationLegendGrid
        '
        Me.allocationLegendGrid.AllowUserToAddRows = False
        Me.allocationLegendGrid.AllowUserToDeleteRows = False
        Me.allocationLegendGrid.BackgroundColor = System.Drawing.Color.White
        Me.allocationLegendGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.allocationLegendGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.allocationLegendGrid.Location = New System.Drawing.Point(0, 0)
        Me.allocationLegendGrid.Name = "allocationLegendGrid"
        Me.allocationLegendGrid.ReadOnly = True
        Me.allocationLegendGrid.RowHeadersVisible = False
        Me.allocationLegendGrid.Size = New System.Drawing.Size(344, 187)
        Me.allocationLegendGrid.TabIndex = 0
        '
        'taskUsageTab
        '
        Me.taskUsageTab.BackColor = System.Drawing.Color.White
        Me.taskUsageTab.Controls.Add(Me.taskUsageSplit)
        Me.taskUsageTab.Location = New System.Drawing.Point(4, 32)
        Me.taskUsageTab.Name = "taskUsageTab"
        Me.taskUsageTab.Padding = New System.Windows.Forms.Padding(3)
        Me.taskUsageTab.Size = New System.Drawing.Size(1569, 696)
        Me.taskUsageTab.TabIndex = 1
        Me.taskUsageTab.Text = "Task Usage View"
        '
        'taskUsageSplit
        '
        Me.taskUsageSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.taskUsageSplit.Location = New System.Drawing.Point(3, 3)
        Me.taskUsageSplit.Name = "taskUsageSplit"
        '
        'taskUsageSplit.Panel1
        '
        Me.taskUsageSplit.Panel1.Controls.Add(Me._taskUsageGrid)
        Me.taskUsageSplit.Panel1MinSize = 700
        '
        'taskUsageSplit.Panel2
        '
        Me.taskUsageSplit.Panel2.Controls.Add(Me.taskUsagePreviewPanel)
        Me.taskUsageSplit.Panel2MinSize = 360
        Me.taskUsageSplit.Size = New System.Drawing.Size(1563, 690)
        Me.taskUsageSplit.SplitterDistance = 1040
        Me.taskUsageSplit.SplitterWidth = 5
        Me.taskUsageSplit.TabIndex = 0
        '
        '_taskUsageGrid
        '
        Me._taskUsageGrid.AllowUserToAddRows = False
        Me._taskUsageGrid.AllowUserToDeleteRows = False
        Me._taskUsageGrid.BackgroundColor = System.Drawing.Color.White
        Me._taskUsageGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._taskUsageGrid.ColumnHeadersHeight = 32
        Me._taskUsageGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._taskUsageGrid.EnableHeadersVisualStyles = False
        Me._taskUsageGrid.Location = New System.Drawing.Point(0, 0)
        Me._taskUsageGrid.Name = "_taskUsageGrid"
        Me._taskUsageGrid.RowHeadersVisible = False
        Me._taskUsageGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me._taskUsageGrid.Size = New System.Drawing.Size(1040, 690)
        Me._taskUsageGrid.TabIndex = 0
        '
        'taskUsagePreviewPanel
        '
        Me.taskUsagePreviewPanel.BackColor = System.Drawing.Color.White
        Me.taskUsagePreviewPanel.Controls.Add(Me.taskUsagePreviewBodySplit)
        Me.taskUsagePreviewPanel.Controls.Add(Me.taskUsagePreviewBadges)
        Me.taskUsagePreviewPanel.Controls.Add(Me.taskUsagePreviewTitle)
        Me.taskUsagePreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.taskUsagePreviewPanel.Location = New System.Drawing.Point(0, 0)
        Me.taskUsagePreviewPanel.Name = "taskUsagePreviewPanel"
        Me.taskUsagePreviewPanel.Padding = New System.Windows.Forms.Padding(12)
        Me.taskUsagePreviewPanel.Size = New System.Drawing.Size(518, 690)
        Me.taskUsagePreviewPanel.TabIndex = 0
        '
        'taskUsagePreviewTitle
        '
        Me.taskUsagePreviewTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.taskUsagePreviewTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.taskUsagePreviewTitle.Location = New System.Drawing.Point(12, 12)
        Me.taskUsagePreviewTitle.Name = "taskUsagePreviewTitle"
        Me.taskUsagePreviewTitle.Size = New System.Drawing.Size(494, 32)
        Me.taskUsagePreviewTitle.TabIndex = 0
        Me.taskUsagePreviewTitle.Text = "Planner Preview - Task Count"
        '
        'taskUsagePreviewBadges
        '
        Me.taskUsagePreviewBadges.Controls.Add(Me.taskUsagePrimaryLabel)
        Me.taskUsagePreviewBadges.Controls.Add(Me.taskUsageSecondaryLabel)
        Me.taskUsagePreviewBadges.Dock = System.Windows.Forms.DockStyle.Top
        Me.taskUsagePreviewBadges.Location = New System.Drawing.Point(12, 44)
        Me.taskUsagePreviewBadges.Name = "taskUsagePreviewBadges"
        Me.taskUsagePreviewBadges.Padding = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.taskUsagePreviewBadges.Size = New System.Drawing.Size(494, 40)
        Me.taskUsagePreviewBadges.TabIndex = 1
        '
        'taskUsagePrimaryLabel
        '
        Me.taskUsagePrimaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.taskUsagePrimaryLabel.Location = New System.Drawing.Point(3, 4)
        Me.taskUsagePrimaryLabel.Name = "taskUsagePrimaryLabel"
        Me.taskUsagePrimaryLabel.Size = New System.Drawing.Size(180, 30)
        Me.taskUsagePrimaryLabel.TabIndex = 0
        Me.taskUsagePrimaryLabel.Text = "Scheduled Tasks: 3"
        Me.taskUsagePrimaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'taskUsageSecondaryLabel
        '
        Me.taskUsageSecondaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(234, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.taskUsageSecondaryLabel.Location = New System.Drawing.Point(189, 4)
        Me.taskUsageSecondaryLabel.Name = "taskUsageSecondaryLabel"
        Me.taskUsageSecondaryLabel.Size = New System.Drawing.Size(190, 30)
        Me.taskUsageSecondaryLabel.TabIndex = 1
        Me.taskUsageSecondaryLabel.Text = "Project Duration: 3 days"
        Me.taskUsageSecondaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'taskUsagePreviewBodySplit
        '
        Me.taskUsagePreviewBodySplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.taskUsagePreviewBodySplit.Location = New System.Drawing.Point(12, 84)
        Me.taskUsagePreviewBodySplit.Name = "taskUsagePreviewBodySplit"
        Me.taskUsagePreviewBodySplit.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.taskUsagePreviewBodySplit.Panel1.Controls.Add(Me.taskUsagePreviewChart)
        Me.taskUsagePreviewBodySplit.Panel2.Controls.Add(Me.taskUsageLegendGrid)
        Me.taskUsagePreviewBodySplit.Size = New System.Drawing.Size(494, 594)
        Me.taskUsagePreviewBodySplit.SplitterDistance = 350
        Me.taskUsagePreviewBodySplit.SplitterWidth = 4
        Me.taskUsagePreviewBodySplit.TabIndex = 2
        '
        'taskUsagePreviewChart
        '
        Me.taskUsagePreviewChart.BackColor = System.Drawing.Color.White
        Me.taskUsagePreviewChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.taskUsagePreviewChart.Location = New System.Drawing.Point(0, 0)
        Me.taskUsagePreviewChart.Name = "taskUsagePreviewChart"
        Me.taskUsagePreviewChart.PreviewMode = SMAScheduler.PlannerPreviewMode.TaskDuration
        Me.taskUsagePreviewChart.Size = New System.Drawing.Size(494, 350)
        Me.taskUsagePreviewChart.TabIndex = 0
        '
        'taskUsageLegendGrid
        '
        Me.taskUsageLegendGrid.AllowUserToAddRows = False
        Me.taskUsageLegendGrid.AllowUserToDeleteRows = False
        Me.taskUsageLegendGrid.BackgroundColor = System.Drawing.Color.White
        Me.taskUsageLegendGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.taskUsageLegendGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.taskUsageLegendGrid.Location = New System.Drawing.Point(0, 0)
        Me.taskUsageLegendGrid.Name = "taskUsageLegendGrid"
        Me.taskUsageLegendGrid.ReadOnly = True
        Me.taskUsageLegendGrid.RowHeadersVisible = False
        Me.taskUsageLegendGrid.Size = New System.Drawing.Size(494, 240)
        Me.taskUsageLegendGrid.TabIndex = 0
        '
        'resourceUsageTab
        '
        Me.resourceUsageTab.BackColor = System.Drawing.Color.White
        Me.resourceUsageTab.Controls.Add(Me.resourceUsageSplit)
        Me.resourceUsageTab.Location = New System.Drawing.Point(4, 32)
        Me.resourceUsageTab.Name = "resourceUsageTab"
        Me.resourceUsageTab.Padding = New System.Windows.Forms.Padding(3)
        Me.resourceUsageTab.Size = New System.Drawing.Size(1569, 696)
        Me.resourceUsageTab.TabIndex = 2
        Me.resourceUsageTab.Text = "Resource Usage View"
        '
        'resourceUsageSplit
        '
        Me.resourceUsageSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUsageSplit.Location = New System.Drawing.Point(3, 3)
        Me.resourceUsageSplit.Name = "resourceUsageSplit"
        Me.resourceUsageSplit.Panel1.Controls.Add(Me._resourceUsageGrid)
        Me.resourceUsageSplit.Panel1MinSize = 700
        Me.resourceUsageSplit.Panel2.Controls.Add(Me.resourceUsagePreviewPanel)
        Me.resourceUsageSplit.Panel2MinSize = 360
        Me.resourceUsageSplit.Size = New System.Drawing.Size(1563, 690)
        Me.resourceUsageSplit.SplitterDistance = 1040
        Me.resourceUsageSplit.SplitterWidth = 5
        Me.resourceUsageSplit.TabIndex = 0
        '
        '_resourceUsageGrid
        '
        Me._resourceUsageGrid.AllowUserToAddRows = False
        Me._resourceUsageGrid.AllowUserToDeleteRows = False
        Me._resourceUsageGrid.BackgroundColor = System.Drawing.Color.White
        Me._resourceUsageGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._resourceUsageGrid.ColumnHeadersHeight = 32
        Me._resourceUsageGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._resourceUsageGrid.EnableHeadersVisualStyles = False
        Me._resourceUsageGrid.Location = New System.Drawing.Point(0, 0)
        Me._resourceUsageGrid.Name = "_resourceUsageGrid"
        Me._resourceUsageGrid.RowHeadersVisible = False
        Me._resourceUsageGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me._resourceUsageGrid.Size = New System.Drawing.Size(1040, 690)
        Me._resourceUsageGrid.TabIndex = 0
        '
        'resourceUsagePreviewPanel
        '
        Me.resourceUsagePreviewPanel.BackColor = System.Drawing.Color.White
        Me.resourceUsagePreviewPanel.Controls.Add(Me.resourceUsagePreviewBodySplit)
        Me.resourceUsagePreviewPanel.Controls.Add(Me.resourceUsagePreviewBadges)
        Me.resourceUsagePreviewPanel.Controls.Add(Me.resourceUsagePreviewTitle)
        Me.resourceUsagePreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUsagePreviewPanel.Location = New System.Drawing.Point(0, 0)
        Me.resourceUsagePreviewPanel.Name = "resourceUsagePreviewPanel"
        Me.resourceUsagePreviewPanel.Padding = New System.Windows.Forms.Padding(12)
        Me.resourceUsagePreviewPanel.Size = New System.Drawing.Size(518, 690)
        Me.resourceUsagePreviewPanel.TabIndex = 0
        '
        'resourceUsagePreviewTitle
        '
        Me.resourceUsagePreviewTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.resourceUsagePreviewTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.resourceUsagePreviewTitle.Location = New System.Drawing.Point(12, 12)
        Me.resourceUsagePreviewTitle.Name = "resourceUsagePreviewTitle"
        Me.resourceUsagePreviewTitle.Size = New System.Drawing.Size(494, 32)
        Me.resourceUsagePreviewTitle.TabIndex = 0
        Me.resourceUsagePreviewTitle.Text = "Planner Preview - Resource Contribution"
        '
        'resourceUsagePreviewBadges
        '
        Me.resourceUsagePreviewBadges.Controls.Add(Me.resourceUsagePrimaryLabel)
        Me.resourceUsagePreviewBadges.Controls.Add(Me.resourceUsageSecondaryLabel)
        Me.resourceUsagePreviewBadges.Dock = System.Windows.Forms.DockStyle.Top
        Me.resourceUsagePreviewBadges.Location = New System.Drawing.Point(12, 44)
        Me.resourceUsagePreviewBadges.Name = "resourceUsagePreviewBadges"
        Me.resourceUsagePreviewBadges.Padding = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.resourceUsagePreviewBadges.Size = New System.Drawing.Size(494, 40)
        Me.resourceUsagePreviewBadges.TabIndex = 1
        '
        'resourceUsagePrimaryLabel
        '
        Me.resourceUsagePrimaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(222, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.resourceUsagePrimaryLabel.Location = New System.Drawing.Point(3, 4)
        Me.resourceUsagePrimaryLabel.Name = "resourceUsagePrimaryLabel"
        Me.resourceUsagePrimaryLabel.Size = New System.Drawing.Size(180, 30)
        Me.resourceUsagePrimaryLabel.TabIndex = 0
        Me.resourceUsagePrimaryLabel.Text = "Contributors: 3"
        Me.resourceUsagePrimaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'resourceUsageSecondaryLabel
        '
        Me.resourceUsageSecondaryLabel.BackColor = System.Drawing.Color.FromArgb(CType(CType(234, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.resourceUsageSecondaryLabel.Location = New System.Drawing.Point(189, 4)
        Me.resourceUsageSecondaryLabel.Name = "resourceUsageSecondaryLabel"
        Me.resourceUsageSecondaryLabel.Size = New System.Drawing.Size(190, 30)
        Me.resourceUsageSecondaryLabel.TabIndex = 1
        Me.resourceUsageSecondaryLabel.Text = "Assigned Hours: 24 hrs"
        Me.resourceUsageSecondaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'resourceUsagePreviewBodySplit
        '
        Me.resourceUsagePreviewBodySplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUsagePreviewBodySplit.Location = New System.Drawing.Point(12, 84)
        Me.resourceUsagePreviewBodySplit.Name = "resourceUsagePreviewBodySplit"
        Me.resourceUsagePreviewBodySplit.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.resourceUsagePreviewBodySplit.Panel1.Controls.Add(Me.resourceUsagePreviewChart)
        Me.resourceUsagePreviewBodySplit.Panel2.Controls.Add(Me.resourceUsageLegendGrid)
        Me.resourceUsagePreviewBodySplit.Size = New System.Drawing.Size(494, 594)
        Me.resourceUsagePreviewBodySplit.SplitterDistance = 350
        Me.resourceUsagePreviewBodySplit.SplitterWidth = 4
        Me.resourceUsagePreviewBodySplit.TabIndex = 2
        '
        'resourceUsagePreviewChart
        '
        Me.resourceUsagePreviewChart.BackColor = System.Drawing.Color.White
        Me.resourceUsagePreviewChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUsagePreviewChart.Location = New System.Drawing.Point(0, 0)
        Me.resourceUsagePreviewChart.Name = "resourceUsagePreviewChart"
        Me.resourceUsagePreviewChart.PreviewMode = SMAScheduler.PlannerPreviewMode.ResourceContribution
        Me.resourceUsagePreviewChart.Size = New System.Drawing.Size(494, 350)
        Me.resourceUsagePreviewChart.TabIndex = 0
        '
        'resourceUsageLegendGrid
        '
        Me.resourceUsageLegendGrid.AllowUserToAddRows = False
        Me.resourceUsageLegendGrid.AllowUserToDeleteRows = False
        Me.resourceUsageLegendGrid.BackgroundColor = System.Drawing.Color.White
        Me.resourceUsageLegendGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.resourceUsageLegendGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUsageLegendGrid.Location = New System.Drawing.Point(0, 0)
        Me.resourceUsageLegendGrid.Name = "resourceUsageLegendGrid"
        Me.resourceUsageLegendGrid.ReadOnly = True
        Me.resourceUsageLegendGrid.RowHeadersVisible = False
        Me.resourceUsageLegendGrid.Size = New System.Drawing.Size(494, 240)
        Me.resourceUsageLegendGrid.TabIndex = 0
        '
        'capacityPlanningTab
        '
        Me.capacityPlanningTab.BackColor = System.Drawing.Color.White
        Me.capacityPlanningTab.Controls.Add(Me._capacityGrid)
        Me.capacityPlanningTab.Location = New System.Drawing.Point(4, 32)
        Me.capacityPlanningTab.Name = "capacityPlanningTab"
        Me.capacityPlanningTab.Padding = New System.Windows.Forms.Padding(3)
        Me.capacityPlanningTab.Size = New System.Drawing.Size(1569, 696)
        Me.capacityPlanningTab.TabIndex = 3
        Me.capacityPlanningTab.Text = "Capacity Planning"
        '
        '_capacityGrid
        '
        Me._capacityGrid.AllowUserToAddRows = False
        Me._capacityGrid.AllowUserToDeleteRows = False
        Me._capacityGrid.BackgroundColor = System.Drawing.Color.White
        Me._capacityGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._capacityGrid.ColumnHeadersHeight = 32
        Me._capacityGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._capacityGrid.EnableHeadersVisualStyles = False
        Me._capacityGrid.Location = New System.Drawing.Point(3, 3)
        Me._capacityGrid.Name = "_capacityGrid"
        Me._capacityGrid.ReadOnly = True
        Me._capacityGrid.RowHeadersVisible = False
        Me._capacityGrid.Size = New System.Drawing.Size(1563, 690)
        Me._capacityGrid.TabIndex = 0
        '
        'resourceUtilizationTab
        '
        Me.resourceUtilizationTab.BackColor = System.Drawing.Color.White
        Me.resourceUtilizationTab.Controls.Add(Me.resourceUtilizationHost)
        Me.resourceUtilizationTab.Location = New System.Drawing.Point(4, 32)
        Me.resourceUtilizationTab.Name = "resourceUtilizationTab"
        Me.resourceUtilizationTab.Padding = New System.Windows.Forms.Padding(3)
        Me.resourceUtilizationTab.Size = New System.Drawing.Size(1569, 696)
        Me.resourceUtilizationTab.TabIndex = 4
        Me.resourceUtilizationTab.Text = "Resource Utilization"
        '
        'resourceUtilizationHost
        '
        Me.resourceUtilizationHost.Controls.Add(Me._resourceUtilizationGrid)
        Me.resourceUtilizationHost.Controls.Add(Me.resourceUtilizationToolbar)
        Me.resourceUtilizationHost.Dock = System.Windows.Forms.DockStyle.Fill
        Me.resourceUtilizationHost.Location = New System.Drawing.Point(3, 3)
        Me.resourceUtilizationHost.Name = "resourceUtilizationHost"
        Me.resourceUtilizationHost.Size = New System.Drawing.Size(1563, 690)
        Me.resourceUtilizationHost.TabIndex = 0
        '
        'resourceUtilizationToolbar
        '
        Me.resourceUtilizationToolbar.BackColor = System.Drawing.Color.White
        Me.resourceUtilizationToolbar.Controls.Add(Me._resourceUtilizationRefreshButton)
        Me.resourceUtilizationToolbar.Controls.Add(Me._resourceUtilizationColorSelector)
        Me.resourceUtilizationToolbar.Controls.Add(Me._resourceUtilizationApplyButton)
        Me.resourceUtilizationToolbar.Controls.Add(Me._resourceUtilizationClearButton)
        Me.resourceUtilizationToolbar.Controls.Add(Me._resourceUtilizationMailButton)
        Me.resourceUtilizationToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.resourceUtilizationToolbar.Location = New System.Drawing.Point(0, 0)
        Me.resourceUtilizationToolbar.Name = "resourceUtilizationToolbar"
        Me.resourceUtilizationToolbar.Padding = New System.Windows.Forms.Padding(10, 8, 10, 6)
        Me.resourceUtilizationToolbar.Size = New System.Drawing.Size(1563, 48)
        Me.resourceUtilizationToolbar.TabIndex = 0
        Me.resourceUtilizationToolbar.WrapContents = False
        '
        '_resourceUtilizationRefreshButton
        '
        Me._resourceUtilizationRefreshButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(95, Byte), Integer), CType(CType(160, Byte), Integer))
        Me._resourceUtilizationRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._resourceUtilizationRefreshButton.ForeColor = System.Drawing.Color.White
        Me._resourceUtilizationRefreshButton.Location = New System.Drawing.Point(13, 11)
        Me._resourceUtilizationRefreshButton.Name = "_resourceUtilizationRefreshButton"
        Me._resourceUtilizationRefreshButton.Size = New System.Drawing.Size(130, 28)
        Me._resourceUtilizationRefreshButton.TabIndex = 0
        Me._resourceUtilizationRefreshButton.Text = "Refresh SQL Hours"
        Me._resourceUtilizationRefreshButton.UseVisualStyleBackColor = False
        '
        '_resourceUtilizationColorSelector
        '
        Me._resourceUtilizationColorSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._resourceUtilizationColorSelector.FormattingEnabled = True
        Me._resourceUtilizationColorSelector.Items.AddRange(New Object() {"Blue - Planned Leave", "Dark Blue - Unplanned Leave", "Yellow - Training", "Green - Weekend Work", "Orange - Pending Work", "Red - Unassigned Hours"})
        Me._resourceUtilizationColorSelector.Location = New System.Drawing.Point(149, 11)
        Me._resourceUtilizationColorSelector.Name = "_resourceUtilizationColorSelector"
        Me._resourceUtilizationColorSelector.Size = New System.Drawing.Size(180, 28)
        Me._resourceUtilizationColorSelector.TabIndex = 1
        '
        '_resourceUtilizationApplyButton
        '
        Me._resourceUtilizationApplyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._resourceUtilizationApplyButton.Location = New System.Drawing.Point(335, 11)
        Me._resourceUtilizationApplyButton.Name = "_resourceUtilizationApplyButton"
        Me._resourceUtilizationApplyButton.Size = New System.Drawing.Size(120, 28)
        Me._resourceUtilizationApplyButton.TabIndex = 2
        Me._resourceUtilizationApplyButton.Text = "Apply Highlight"
        '
        '_resourceUtilizationClearButton
        '
        Me._resourceUtilizationClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._resourceUtilizationClearButton.Location = New System.Drawing.Point(461, 11)
        Me._resourceUtilizationClearButton.Name = "_resourceUtilizationClearButton"
        Me._resourceUtilizationClearButton.Size = New System.Drawing.Size(120, 28)
        Me._resourceUtilizationClearButton.TabIndex = 3
        Me._resourceUtilizationClearButton.Text = "Clear Highlight"
        '
        '_resourceUtilizationMailButton
        '
        Me._resourceUtilizationMailButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(34, Byte), Integer), CType(CType(169, Byte), Integer), CType(CType(105, Byte), Integer))
        Me._resourceUtilizationMailButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._resourceUtilizationMailButton.ForeColor = System.Drawing.Color.White
        Me._resourceUtilizationMailButton.Location = New System.Drawing.Point(587, 11)
        Me._resourceUtilizationMailButton.Name = "_resourceUtilizationMailButton"
        Me._resourceUtilizationMailButton.Size = New System.Drawing.Size(170, 28)
        Me._resourceUtilizationMailButton.TabIndex = 4
        Me._resourceUtilizationMailButton.Text = "Send Availability Snip"
        Me._resourceUtilizationMailButton.UseVisualStyleBackColor = False
        '
        '_resourceUtilizationGrid
        '
        Me._resourceUtilizationGrid.AllowUserToAddRows = False
        Me._resourceUtilizationGrid.AllowUserToDeleteRows = False
        Me._resourceUtilizationGrid.BackgroundColor = System.Drawing.Color.White
        Me._resourceUtilizationGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._resourceUtilizationGrid.ColumnHeadersHeight = 32
        Me._resourceUtilizationGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._resourceUtilizationGrid.EnableHeadersVisualStyles = False
        Me._resourceUtilizationGrid.Location = New System.Drawing.Point(0, 48)
        Me._resourceUtilizationGrid.Name = "_resourceUtilizationGrid"
        Me._resourceUtilizationGrid.RowHeadersVisible = False
        Me._resourceUtilizationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me._resourceUtilizationGrid.Size = New System.Drawing.Size(1563, 642)
        Me._resourceUtilizationGrid.TabIndex = 1
        '
        '_detailsPanel
        '
        Me._detailsPanel.BackColor = System.Drawing.Color.White
        Me._detailsPanel.Controls.Add(Me.taskWorkspaceTitle)
        Me._detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me._detailsPanel.Location = New System.Drawing.Point(0, 0)
        Me._detailsPanel.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._detailsPanel.Name = "_detailsPanel"
        Me._detailsPanel.Padding = New System.Windows.Forms.Padding(16, 19, 16, 19)
        Me._detailsPanel.Size = New System.Drawing.Size(150, 46)
        Me._detailsPanel.TabIndex = 0
        '
        'taskWorkspaceTitle
        '
        Me.taskWorkspaceTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.taskWorkspaceTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 10.5!)
        Me.taskWorkspaceTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.taskWorkspaceTitle.Location = New System.Drawing.Point(16, 19)
        Me.taskWorkspaceTitle.Name = "taskWorkspaceTitle"
        Me.taskWorkspaceTitle.Size = New System.Drawing.Size(118, 37)
        Me.taskWorkspaceTitle.TabIndex = 0
        Me.taskWorkspaceTitle.Text = "Task allocation"
        '
        'statusBar
        '
        Me.statusBar.BackColor = System.Drawing.Color.White
        Me.statusBar.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.statusBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me._status})
        Me.statusBar.Location = New System.Drawing.Point(0, 1029)
        Me.statusBar.Name = "statusBar"
        Me.statusBar.Padding = New System.Windows.Forms.Padding(1, 0, 16, 0)
        Me.statusBar.Size = New System.Drawing.Size(1577, 26)
        Me.statusBar.TabIndex = 3
        '
        '_status
        '
        Me._status.Name = "_status"
        Me._status.Size = New System.Drawing.Size(50, 20)
        Me._status.Text = "Ready"
        '
        'SMASchedulerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(244, Byte), Integer), CType(CType(246, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1577, 1055)
        Me.Controls.Add(Me.contentSplit)
        Me.Controls.Add(Me.statusBar)
        Me.Controls.Add(Me.headerPanel)
        Me.Controls.Add(Me.commandBar)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MinimumSize = New System.Drawing.Size(1323, 891)
        Me.Name = "SMASchedulerForm"
        Me.Text = "SMA Scheduler"
        Me.commandBar.ResumeLayout(False)
        Me.commandBar.PerformLayout()
        Me.headerPanel.ResumeLayout(False)
        Me.headerPanel.PerformLayout()
        CType(Me._totalProjectHours, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._resourcesNeeded, System.ComponentModel.ISupportInitialize).EndInit()
        Me.contentSplit.Panel1.ResumeLayout(False)
        Me.contentSplit.Panel2.ResumeLayout(False)
        CType(Me.contentSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.contentSplit.ResumeLayout(False)
        Me._workspaceTabs.ResumeLayout(False)
        Me.taskAllocationTab.ResumeLayout(False)
        Me.mainSplit.Panel1.ResumeLayout(False)
        Me.mainSplit.Panel2.ResumeLayout(False)
        CType(Me.mainSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mainSplit.ResumeLayout(False)
        CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ganttPreviewSplit.Panel1.ResumeLayout(False)
        Me.ganttPreviewSplit.Panel2.ResumeLayout(False)
        CType(Me.ganttPreviewSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ganttPreviewSplit.ResumeLayout(False)
        Me.allocationPreviewPanel.ResumeLayout(False)
        Me.allocationPreviewBadges.ResumeLayout(False)
        Me.allocationPreviewBodySplit.Panel1.ResumeLayout(False)
        Me.allocationPreviewBodySplit.Panel2.ResumeLayout(False)
        CType(Me.allocationPreviewBodySplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.allocationPreviewBodySplit.ResumeLayout(False)
        CType(Me.allocationLegendGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.taskUsageTab.ResumeLayout(False)
        Me.taskUsageSplit.Panel1.ResumeLayout(False)
        Me.taskUsageSplit.Panel2.ResumeLayout(False)
        CType(Me.taskUsageSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.taskUsageSplit.ResumeLayout(False)
        CType(Me._taskUsageGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.taskUsagePreviewPanel.ResumeLayout(False)
        Me.taskUsagePreviewBadges.ResumeLayout(False)
        Me.taskUsagePreviewBodySplit.Panel1.ResumeLayout(False)
        Me.taskUsagePreviewBodySplit.Panel2.ResumeLayout(False)
        CType(Me.taskUsagePreviewBodySplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.taskUsagePreviewBodySplit.ResumeLayout(False)
        CType(Me.taskUsageLegendGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.resourceUsageTab.ResumeLayout(False)
        Me.resourceUsageSplit.Panel1.ResumeLayout(False)
        Me.resourceUsageSplit.Panel2.ResumeLayout(False)
        CType(Me.resourceUsageSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.resourceUsageSplit.ResumeLayout(False)
        CType(Me._resourceUsageGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.resourceUsagePreviewPanel.ResumeLayout(False)
        Me.resourceUsagePreviewBadges.ResumeLayout(False)
        Me.resourceUsagePreviewBodySplit.Panel1.ResumeLayout(False)
        Me.resourceUsagePreviewBodySplit.Panel2.ResumeLayout(False)
        CType(Me.resourceUsagePreviewBodySplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.resourceUsagePreviewBodySplit.ResumeLayout(False)
        CType(Me.resourceUsageLegendGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.capacityPlanningTab.ResumeLayout(False)
        CType(Me._capacityGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.resourceUtilizationTab.ResumeLayout(False)
        Me.resourceUtilizationHost.ResumeLayout(False)
        Me.resourceUtilizationToolbar.ResumeLayout(False)
        CType(Me._resourceUtilizationGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me._detailsPanel.ResumeLayout(False)
        Me.statusBar.ResumeLayout(False)
        Me.statusBar.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private commandBar As ToolStrip
    Private btnNew As ToolStripButton
    Private btnSave As ToolStripButton
    Private btnRefreshCapacity As ToolStripButton
    Private btnTaskUsage As ToolStripButton
    Private btnResourceUsage As ToolStripButton
    Private sepFile As ToolStripSeparator
    Private btnAddTask As ToolStripButton
    Private btnDelete As ToolStripButton
    Private btnMoveUp As ToolStripButton
    Private btnMoveDown As ToolStripButton
    Private sepTasks As ToolStripSeparator
    Private btnLink As ToolStripButton
    Private btnUnlink As ToolStripButton
    Private btnMilestone As ToolStripButton
    Private sepTheme As ToolStripSeparator
    Private btnChangeTheme As ToolStripButton
    Private headerPanel As Panel
    Private appTitle As Label
    Private projectLabel As Label
    Private versionLabel As Label
    Private totalHoursLabel As Label
    Private resourcesNeededLabel As Label
    Private contentSplit As SplitContainer
    Private _workspaceTabs As TabControl
    Private taskAllocationTab As TabPage
    Private taskUsageTab As TabPage
    Private resourceUsageTab As TabPage
    Private capacityPlanningTab As TabPage
    Private resourceUtilizationTab As TabPage
    Private mainSplit As SplitContainer
    Private ganttPreviewSplit As SplitContainer
    Private allocationPreviewPanel As Panel
    Private allocationPreviewTitle As Label
    Private allocationPreviewBadges As FlowLayoutPanel
    Private allocationPrimaryLabel As Label
    Private allocationSecondaryLabel As Label
    Private allocationPreviewBodySplit As SplitContainer
    Private allocationPreviewChart As PlannerPieChartPanel
    Private allocationLegendGrid As DataGridView
    Private taskUsageSplit As SplitContainer
    Private _taskUsageGrid As DataGridView
    Private taskUsagePreviewPanel As Panel
    Private taskUsagePreviewTitle As Label
    Private taskUsagePreviewBadges As FlowLayoutPanel
    Private taskUsagePrimaryLabel As Label
    Private taskUsageSecondaryLabel As Label
    Private taskUsagePreviewBodySplit As SplitContainer
    Private taskUsagePreviewChart As PlannerPieChartPanel
    Private taskUsageLegendGrid As DataGridView
    Private resourceUsageSplit As SplitContainer
    Private _resourceUsageGrid As DataGridView
    Private resourceUsagePreviewPanel As Panel
    Private resourceUsagePreviewTitle As Label
    Private resourceUsagePreviewBadges As FlowLayoutPanel
    Private resourceUsagePrimaryLabel As Label
    Private resourceUsageSecondaryLabel As Label
    Private resourceUsagePreviewBodySplit As SplitContainer
    Private resourceUsagePreviewChart As PlannerPieChartPanel
    Private resourceUsageLegendGrid As DataGridView
    Private _capacityGrid As DataGridView
    Private resourceUtilizationHost As Panel
    Private resourceUtilizationToolbar As FlowLayoutPanel
    Private _resourceUtilizationRefreshButton As Button
    Private _resourceUtilizationColorSelector As ComboBox
    Private _resourceUtilizationApplyButton As Button
    Private _resourceUtilizationClearButton As Button
    Private _resourceUtilizationMailButton As Button
    Private _resourceUtilizationGrid As DataGridView
    Private projectSizeLabel As Label
    Private taskCatalogLabel As Label
    Private taskWorkspaceTitle As Label
    Private statusBar As StatusStrip
    Private _projectName As TextBox
    Private _versionNumber As TextBox
    Private _totalProjectHours As BlankNumericUpDown
    Private _taskCatalogSelector As ComboBox
    Private _projectSizeSelector As ComboBox
    Private _includeSaturdays As CheckBox
    Private _summaryTitle As Label
    Private _summaryDates As Label
    Private _summaryProgress As Label
    Private _summaryResources As Label
    Private _resourcesNeeded As BlankNumericUpDown
    Private _remainingHoursLabel As Label
    Private WithEvents _grid As DataGridView
    Private _gantt As GanttPanel
    Private _detailsPanel As Panel
    Private _status As ToolStripStatusLabel
End Class
