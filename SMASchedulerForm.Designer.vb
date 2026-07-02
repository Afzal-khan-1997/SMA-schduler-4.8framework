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
        Me.mainSplit = New System.Windows.Forms.SplitContainer()
        Me._grid = New System.Windows.Forms.DataGridView()
        Me._gantt = New SMAScheduler.GanttPanel()
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
        CType(Me.mainSplit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mainSplit.Panel1.SuspendLayout()
        Me.mainSplit.Panel2.SuspendLayout()
        Me.mainSplit.SuspendLayout()
        CType(Me._grid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._detailsPanel.SuspendLayout()
        Me.statusBar.SuspendLayout()
        Me.SuspendLayout()
        '
        'commandBar
        '
        Me.commandBar.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.commandBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.commandBar.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.commandBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnSave, Me.btnRefreshCapacity, Me.sepFile, Me.btnAddTask, Me.btnDelete, Me.btnMoveUp, Me.btnMoveDown, Me.sepTasks, Me.btnLink, Me.btnUnlink, Me.btnMilestone, Me.sepTheme, Me.btnChangeTheme})
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
        '
        '_summaryResources
        '
        Me._summaryResources.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(222, Byte), Integer), CType(CType(234, Byte), Integer))
        Me._summaryResources.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me._summaryResources.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me._summaryResources.Location = New System.Drawing.Point(642, 170)
        Me._summaryResources.Name = "_summaryResources"
        Me._summaryResources.Size = New System.Drawing.Size(189, 56)
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
        Me.contentSplit.Panel1.Controls.Add(Me.mainSplit)
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
        'mainSplit
        '
        Me.mainSplit.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(229, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mainSplit.Location = New System.Drawing.Point(0, 0)
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
        Me.mainSplit.Panel2.Controls.Add(Me._gantt)
        Me.mainSplit.Panel2MinSize = 320
        Me.mainSplit.Size = New System.Drawing.Size(1577, 732)
        Me.mainSplit.SplitterDistance = 937
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
        Me._grid.Size = New System.Drawing.Size(937, 732)
        Me._grid.TabIndex = 0
        '
        '_gantt
        '
        Me._gantt.AutoScroll = True
        Me._gantt.BackColor = System.Drawing.Color.White
        Me._gantt.Dock = System.Windows.Forms.DockStyle.Fill
        Me._gantt.Location = New System.Drawing.Point(0, 0)
        Me._gantt.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._gantt.Name = "_gantt"
        Me._gantt.Size = New System.Drawing.Size(635, 732)
        Me._gantt.TabIndex = 0
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
        Me.mainSplit.Panel1.ResumeLayout(False)
        Me.mainSplit.Panel2.ResumeLayout(False)
        CType(Me.mainSplit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mainSplit.ResumeLayout(False)
        CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
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
    Private mainSplit As SplitContainer
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
