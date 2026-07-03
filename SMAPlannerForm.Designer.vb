<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SMAPlannerForm
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
        Me.headerPanel = New System.Windows.Forms.Panel()
        Me.titleLabel = New System.Windows.Forms.Label()
        Me.promptLabel = New System.Windows.Forms.Label()
        Me.btnNewProject = New System.Windows.Forms.Button()
        Me.btnRefreshList = New System.Windows.Forms.Button()
        Me.searchLabel = New System.Windows.Forms.Label()
        Me._liveProjectSearchBox = New System.Windows.Forms.TextBox()
        Me.selectorLabel = New System.Windows.Forms.Label()
        Me._liveProjectSelector = New System.Windows.Forms.ComboBox()
        Me.btnScheduleProject = New System.Windows.Forms.Button()
        Me._liveProjectSizeLabel = New System.Windows.Forms.Label()
        Me.btnRescheduleProject = New System.Windows.Forms.Button()
        Me.projectTypeCaptionLabel = New System.Windows.Forms.Label()
        Me.projectTypeValueLabel = New System.Windows.Forms.Label()
        Me.gridPanel = New System.Windows.Forms.Panel()
        Me._grid = New System.Windows.Forms.DataGridView()
        Me.listTitle = New System.Windows.Forms.Label()
        Me.planningSummaryPanel = New System.Windows.Forms.Panel()
        Me.summaryCards = New System.Windows.Forms.TableLayoutPanel()
        Me.newProjectsPanel = New System.Windows.Forms.Panel()
        Me.newProjectsCountLabel = New System.Windows.Forms.Label()
        Me.newProjectsLabel = New System.Windows.Forms.Label()
        Me.updateProjectsPanel = New System.Windows.Forms.Panel()
        Me.updateProjectsCountLabel = New System.Windows.Forms.Label()
        Me.updateProjectsLabel = New System.Windows.Forms.Label()
        Me.feedbackProjectsPanel = New System.Windows.Forms.Panel()
        Me.feedbackProjectsCountLabel = New System.Windows.Forms.Label()
        Me.feedbackProjectsLabel = New System.Windows.Forms.Label()
        Me.summaryPeriodLabel = New System.Windows.Forms.Label()
        Me.summaryTitleLabel = New System.Windows.Forms.Label()
        Me._status = New System.Windows.Forms.Label()
        Me.headerPanel.SuspendLayout()
        Me.gridPanel.SuspendLayout()
        CType(Me._grid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.planningSummaryPanel.SuspendLayout()
        Me.summaryCards.SuspendLayout()
        Me.newProjectsPanel.SuspendLayout()
        Me.updateProjectsPanel.SuspendLayout()
        Me.feedbackProjectsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'headerPanel
        '
        Me.headerPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(229, Byte), Integer), CType(CType(241, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.headerPanel.Controls.Add(Me.titleLabel)
        Me.headerPanel.Controls.Add(Me.promptLabel)
        Me.headerPanel.Controls.Add(Me.btnNewProject)
        Me.headerPanel.Controls.Add(Me.btnRefreshList)
        Me.headerPanel.Controls.Add(Me.searchLabel)
        Me.headerPanel.Controls.Add(Me._liveProjectSearchBox)
        Me.headerPanel.Controls.Add(Me.selectorLabel)
        Me.headerPanel.Controls.Add(Me._liveProjectSelector)
        Me.headerPanel.Controls.Add(Me.btnScheduleProject)
        Me.headerPanel.Controls.Add(Me._liveProjectSizeLabel)
        Me.headerPanel.Controls.Add(Me.btnRescheduleProject)
        Me.headerPanel.Controls.Add(Me.projectTypeCaptionLabel)
        Me.headerPanel.Controls.Add(Me.projectTypeValueLabel)
        Me.headerPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.headerPanel.Location = New System.Drawing.Point(0, 0)
        Me.headerPanel.Name = "headerPanel"
        Me.headerPanel.Padding = New System.Windows.Forms.Padding(24, 18, 24, 18)
        Me.headerPanel.Size = New System.Drawing.Size(1180, 190)
        Me.headerPanel.TabIndex = 0
        '
        'titleLabel
        '
        Me.titleLabel.AutoSize = True
        Me.titleLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 20.0!)
        Me.titleLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.titleLabel.Location = New System.Drawing.Point(24, 18)
        Me.titleLabel.Name = "titleLabel"
        Me.titleLabel.Size = New System.Drawing.Size(220, 46)
        Me.titleLabel.TabIndex = 0
        Me.titleLabel.Text = "SMA Planner"
        '
        'promptLabel
        '
        Me.promptLabel.AutoSize = True
        Me.promptLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
        Me.promptLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(63, Byte), Integer))
        Me.promptLabel.Location = New System.Drawing.Point(26, 76)
        Me.promptLabel.Name = "promptLabel"
        Me.promptLabel.Size = New System.Drawing.Size(343, 25)
        Me.promptLabel.TabIndex = 1
        Me.promptLabel.Text = "Do you want to plan for a new project?"
        Me.promptLabel.Visible = False
        '
        'btnNewProject
        '
        Me.btnNewProject.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(125, Byte), Integer), CType(CType(221, Byte), Integer))
        Me.btnNewProject.FlatAppearance.BorderSize = 0
        Me.btnNewProject.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNewProject.ForeColor = System.Drawing.Color.White
        Me.btnNewProject.Location = New System.Drawing.Point(390, 70)
        Me.btnNewProject.Name = "btnNewProject"
        Me.btnNewProject.Size = New System.Drawing.Size(140, 34)
        Me.btnNewProject.TabIndex = 2
        Me.btnNewProject.Text = "New Project"
        Me.btnNewProject.UseVisualStyleBackColor = False
        Me.btnNewProject.Visible = False
        '
        'btnRefreshList
        '
        Me.btnRefreshList.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.btnRefreshList.FlatAppearance.BorderSize = 0
        Me.btnRefreshList.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefreshList.ForeColor = System.Drawing.Color.White
        Me.btnRefreshList.Location = New System.Drawing.Point(546, 70)
        Me.btnRefreshList.Name = "btnRefreshList"
        Me.btnRefreshList.Size = New System.Drawing.Size(120, 34)
        Me.btnRefreshList.TabIndex = 3
        Me.btnRefreshList.Text = "Refresh List"
        Me.btnRefreshList.UseVisualStyleBackColor = False
        Me.btnRefreshList.Visible = False
        '
        'searchLabel
        '
        Me.searchLabel.AutoSize = True
        Me.searchLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
        Me.searchLabel.Location = New System.Drawing.Point(24, 76)
        Me.searchLabel.Name = "searchLabel"
        Me.searchLabel.Size = New System.Drawing.Size(109, 20)
        Me.searchLabel.TabIndex = 4
        Me.searchLabel.Text = "Search Projects"
        '
        '_liveProjectSearchBox
        '
        Me._liveProjectSearchBox.Location = New System.Drawing.Point(24, 100)
        Me._liveProjectSearchBox.Name = "_liveProjectSearchBox"
        Me._liveProjectSearchBox.Size = New System.Drawing.Size(290, 27)
        Me._liveProjectSearchBox.TabIndex = 5
        '
        'selectorLabel
        '
        Me.selectorLabel.AutoSize = True
        Me.selectorLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
        Me.selectorLabel.Location = New System.Drawing.Point(336, 76)
        Me.selectorLabel.Name = "selectorLabel"
        Me.selectorLabel.Size = New System.Drawing.Size(106, 20)
        Me.selectorLabel.TabIndex = 6
        Me.selectorLabel.Text = "Select Template"
        '
        '_liveProjectSelector
        '
        Me._liveProjectSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._liveProjectSelector.FormattingEnabled = True
        Me._liveProjectSelector.Location = New System.Drawing.Point(336, 100)
        Me._liveProjectSelector.Name = "_liveProjectSelector"
        Me._liveProjectSelector.Size = New System.Drawing.Size(360, 28)
        Me._liveProjectSelector.TabIndex = 7
        '
        'btnScheduleProject
        '
        Me.btnScheduleProject.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(164, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.btnScheduleProject.FlatAppearance.BorderSize = 0
        Me.btnScheduleProject.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnScheduleProject.ForeColor = System.Drawing.Color.White
        Me.btnScheduleProject.Location = New System.Drawing.Point(716, 96)
        Me.btnScheduleProject.Name = "btnScheduleProject"
        Me.btnScheduleProject.Size = New System.Drawing.Size(150, 34)
        Me.btnScheduleProject.TabIndex = 8
        Me.btnScheduleProject.Text = "Schedule Project"
        Me.btnScheduleProject.UseVisualStyleBackColor = False
        '
        '_liveProjectSizeLabel
        '
        Me._liveProjectSizeLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me._liveProjectSizeLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me._liveProjectSizeLabel.Location = New System.Drawing.Point(24, 142)
        Me._liveProjectSizeLabel.Name = "_liveProjectSizeLabel"
        Me._liveProjectSizeLabel.Size = New System.Drawing.Size(260, 28)
        Me._liveProjectSizeLabel.TabIndex = 9
        Me._liveProjectSizeLabel.Text = "Project size:"
        Me._liveProjectSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnRescheduleProject
        '
        Me.btnRescheduleProject.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.btnRescheduleProject.FlatAppearance.BorderSize = 0
        Me.btnRescheduleProject.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRescheduleProject.ForeColor = System.Drawing.Color.White
        Me.btnRescheduleProject.Location = New System.Drawing.Point(882, 96)
        Me.btnRescheduleProject.Name = "btnRescheduleProject"
        Me.btnRescheduleProject.Size = New System.Drawing.Size(162, 34)
        Me.btnRescheduleProject.TabIndex = 10
        Me.btnRescheduleProject.Text = "Reschedule Project"
        Me.btnRescheduleProject.UseVisualStyleBackColor = False
        '
        'projectTypeCaptionLabel
        '
        Me.projectTypeCaptionLabel.AutoSize = True
        Me.projectTypeCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
        Me.projectTypeCaptionLabel.Location = New System.Drawing.Point(336, 146)
        Me.projectTypeCaptionLabel.Name = "projectTypeCaptionLabel"
        Me.projectTypeCaptionLabel.Size = New System.Drawing.Size(88, 20)
        Me.projectTypeCaptionLabel.TabIndex = 11
        Me.projectTypeCaptionLabel.Text = "Report Type:"
        '
        'projectTypeValueLabel
        '
        Me.projectTypeValueLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.projectTypeValueLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.projectTypeValueLabel.Location = New System.Drawing.Point(430, 142)
        Me.projectTypeValueLabel.Name = "projectTypeValueLabel"
        Me.projectTypeValueLabel.Size = New System.Drawing.Size(266, 28)
        Me.projectTypeValueLabel.TabIndex = 12
        Me.projectTypeValueLabel.Text = "New"
        Me.projectTypeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gridPanel
        '
        Me.gridPanel.BackColor = System.Drawing.Color.White
        Me.gridPanel.Controls.Add(Me._grid)
        Me.gridPanel.Controls.Add(Me.listTitle)
        Me.gridPanel.Controls.Add(Me.planningSummaryPanel)
        Me.gridPanel.Controls.Add(Me._status)
        Me.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridPanel.Location = New System.Drawing.Point(0, 190)
        Me.gridPanel.Name = "gridPanel"
        Me.gridPanel.Padding = New System.Windows.Forms.Padding(24, 22, 24, 16)
        Me.gridPanel.Size = New System.Drawing.Size(1180, 630)
        Me.gridPanel.TabIndex = 1
        '
        '_grid
        '
        Me._grid.AllowUserToAddRows = False
        Me._grid.AllowUserToDeleteRows = False
        Me._grid.BackgroundColor = System.Drawing.Color.White
        Me._grid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._grid.ColumnHeadersHeight = 34
        Me._grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me._grid.EnableHeadersVisualStyles = False
        Me._grid.GridColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(242, Byte), Integer))
        Me._grid.Location = New System.Drawing.Point(24, 56)
        Me._grid.MultiSelect = False
        Me._grid.Name = "_grid"
        Me._grid.ReadOnly = True
        Me._grid.RowHeadersVisible = False
        Me._grid.RowHeadersWidth = 51
        Me._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me._grid.Size = New System.Drawing.Size(1132, 286)
        Me._grid.TabIndex = 1
        '
        'listTitle
        '
        Me.listTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.listTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.listTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.listTitle.Location = New System.Drawing.Point(24, 22)
        Me.listTitle.Name = "listTitle"
        Me.listTitle.Size = New System.Drawing.Size(1132, 34)
        Me.listTitle.TabIndex = 0
        Me.listTitle.Text = "Recent Scheduled Projects"
        '
        'planningSummaryPanel
        '
        Me.planningSummaryPanel.Controls.Add(Me.summaryCards)
        Me.planningSummaryPanel.Controls.Add(Me.summaryPeriodLabel)
        Me.planningSummaryPanel.Controls.Add(Me.summaryTitleLabel)
        Me.planningSummaryPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.planningSummaryPanel.Location = New System.Drawing.Point(24, 342)
        Me.planningSummaryPanel.Name = "planningSummaryPanel"
        Me.planningSummaryPanel.Padding = New System.Windows.Forms.Padding(0, 12, 0, 8)
        Me.planningSummaryPanel.Size = New System.Drawing.Size(1132, 214)
        Me.planningSummaryPanel.TabIndex = 2
        '
        'summaryCards
        '
        Me.summaryCards.ColumnCount = 3
        Me.summaryCards.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.summaryCards.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.summaryCards.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.summaryCards.Controls.Add(Me.newProjectsPanel, 0, 0)
        Me.summaryCards.Controls.Add(Me.updateProjectsPanel, 1, 0)
        Me.summaryCards.Controls.Add(Me.feedbackProjectsPanel, 2, 0)
        Me.summaryCards.Dock = System.Windows.Forms.DockStyle.Fill
        Me.summaryCards.Location = New System.Drawing.Point(0, 68)
        Me.summaryCards.Name = "summaryCards"
        Me.summaryCards.RowCount = 1
        Me.summaryCards.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.summaryCards.Size = New System.Drawing.Size(1132, 138)
        Me.summaryCards.TabIndex = 2
        '
        'newProjectsPanel
        '
        Me.newProjectsPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.newProjectsPanel.Controls.Add(Me.newProjectsCountLabel)
        Me.newProjectsPanel.Controls.Add(Me.newProjectsLabel)
        Me.newProjectsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.newProjectsPanel.Location = New System.Drawing.Point(0, 4)
        Me.newProjectsPanel.Margin = New System.Windows.Forms.Padding(0, 4, 12, 4)
        Me.newProjectsPanel.Name = "newProjectsPanel"
        Me.newProjectsPanel.Size = New System.Drawing.Size(365, 130)
        Me.newProjectsPanel.TabIndex = 0
        '
        'newProjectsCountLabel
        '
        Me.newProjectsCountLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.newProjectsCountLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 24.0!)
        Me.newProjectsCountLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.newProjectsCountLabel.Location = New System.Drawing.Point(0, 42)
        Me.newProjectsCountLabel.Name = "newProjectsCountLabel"
        Me.newProjectsCountLabel.Size = New System.Drawing.Size(365, 88)
        Me.newProjectsCountLabel.TabIndex = 1
        Me.newProjectsCountLabel.Text = "0"
        Me.newProjectsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'newProjectsLabel
        '
        Me.newProjectsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.newProjectsLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me.newProjectsLabel.Location = New System.Drawing.Point(0, 0)
        Me.newProjectsLabel.Name = "newProjectsLabel"
        Me.newProjectsLabel.Size = New System.Drawing.Size(365, 42)
        Me.newProjectsLabel.TabIndex = 0
        Me.newProjectsLabel.Text = "New Projects"
        Me.newProjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'updateProjectsPanel
        '
        Me.updateProjectsPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(225, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.updateProjectsPanel.Controls.Add(Me.updateProjectsCountLabel)
        Me.updateProjectsPanel.Controls.Add(Me.updateProjectsLabel)
        Me.updateProjectsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.updateProjectsPanel.Location = New System.Drawing.Point(389, 4)
        Me.updateProjectsPanel.Margin = New System.Windows.Forms.Padding(12, 4, 12, 4)
        Me.updateProjectsPanel.Name = "updateProjectsPanel"
        Me.updateProjectsPanel.Size = New System.Drawing.Size(353, 130)
        Me.updateProjectsPanel.TabIndex = 1
        '
        'updateProjectsCountLabel
        '
        Me.updateProjectsCountLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.updateProjectsCountLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 24.0!)
        Me.updateProjectsCountLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.updateProjectsCountLabel.Location = New System.Drawing.Point(0, 42)
        Me.updateProjectsCountLabel.Name = "updateProjectsCountLabel"
        Me.updateProjectsCountLabel.Size = New System.Drawing.Size(353, 88)
        Me.updateProjectsCountLabel.TabIndex = 1
        Me.updateProjectsCountLabel.Text = "0"
        Me.updateProjectsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'updateProjectsLabel
        '
        Me.updateProjectsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.updateProjectsLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me.updateProjectsLabel.Location = New System.Drawing.Point(0, 0)
        Me.updateProjectsLabel.Name = "updateProjectsLabel"
        Me.updateProjectsLabel.Size = New System.Drawing.Size(353, 42)
        Me.updateProjectsLabel.TabIndex = 0
        Me.updateProjectsLabel.Text = "Update Projects"
        Me.updateProjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'feedbackProjectsPanel
        '
        Me.feedbackProjectsPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(222, Byte), Integer), CType(CType(234, Byte), Integer))
        Me.feedbackProjectsPanel.Controls.Add(Me.feedbackProjectsCountLabel)
        Me.feedbackProjectsPanel.Controls.Add(Me.feedbackProjectsLabel)
        Me.feedbackProjectsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.feedbackProjectsPanel.Location = New System.Drawing.Point(766, 4)
        Me.feedbackProjectsPanel.Margin = New System.Windows.Forms.Padding(12, 4, 0, 4)
        Me.feedbackProjectsPanel.Name = "feedbackProjectsPanel"
        Me.feedbackProjectsPanel.Size = New System.Drawing.Size(366, 130)
        Me.feedbackProjectsPanel.TabIndex = 2
        '
        'feedbackProjectsCountLabel
        '
        Me.feedbackProjectsCountLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.feedbackProjectsCountLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 24.0!)
        Me.feedbackProjectsCountLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.feedbackProjectsCountLabel.Location = New System.Drawing.Point(0, 42)
        Me.feedbackProjectsCountLabel.Name = "feedbackProjectsCountLabel"
        Me.feedbackProjectsCountLabel.Size = New System.Drawing.Size(366, 88)
        Me.feedbackProjectsCountLabel.TabIndex = 1
        Me.feedbackProjectsCountLabel.Text = "0"
        Me.feedbackProjectsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'feedbackProjectsLabel
        '
        Me.feedbackProjectsLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.feedbackProjectsLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me.feedbackProjectsLabel.Location = New System.Drawing.Point(0, 0)
        Me.feedbackProjectsLabel.Name = "feedbackProjectsLabel"
        Me.feedbackProjectsLabel.Size = New System.Drawing.Size(366, 42)
        Me.feedbackProjectsLabel.TabIndex = 0
        Me.feedbackProjectsLabel.Text = "Feedback Projects"
        Me.feedbackProjectsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'summaryPeriodLabel
        '
        Me.summaryPeriodLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.summaryPeriodLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
        Me.summaryPeriodLabel.Location = New System.Drawing.Point(0, 42)
        Me.summaryPeriodLabel.Name = "summaryPeriodLabel"
        Me.summaryPeriodLabel.Size = New System.Drawing.Size(1132, 26)
        Me.summaryPeriodLabel.TabIndex = 1
        Me.summaryPeriodLabel.Text = "20-Jun-2026 to 20-Jul-2026"
        '
        'summaryTitleLabel
        '
        Me.summaryTitleLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.summaryTitleLabel.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.summaryTitleLabel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.summaryTitleLabel.Location = New System.Drawing.Point(0, 12)
        Me.summaryTitleLabel.Name = "summaryTitleLabel"
        Me.summaryTitleLabel.Size = New System.Drawing.Size(1132, 30)
        Me.summaryTitleLabel.TabIndex = 0
        Me.summaryTitleLabel.Text = "Projects Planned For This Period"
        '
        '_status
        '
        Me._status.Dock = System.Windows.Forms.DockStyle.Bottom
        Me._status.ForeColor = System.Drawing.Color.DimGray
        Me._status.Location = New System.Drawing.Point(24, 556)
        Me._status.Name = "_status"
        Me._status.Size = New System.Drawing.Size(1132, 28)
        Me._status.TabIndex = 2
        '
        'SMAPlannerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(244, Byte), Integer), CType(CType(246, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1180, 820)
        Me.Controls.Add(Me.gridPanel)
        Me.Controls.Add(Me.headerPanel)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.MinimumSize = New System.Drawing.Size(980, 640)
        Me.Name = "SMAPlannerForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SMA Planner"
        Me.headerPanel.ResumeLayout(False)
        Me.headerPanel.PerformLayout()
        Me.gridPanel.ResumeLayout(False)
        CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.planningSummaryPanel.ResumeLayout(False)
        Me.summaryCards.ResumeLayout(False)
        Me.newProjectsPanel.ResumeLayout(False)
        Me.updateProjectsPanel.ResumeLayout(False)
        Me.feedbackProjectsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private headerPanel As Panel
    Private titleLabel As Label
    Private promptLabel As Label
    Private btnNewProject As Button
    Private btnRefreshList As Button
    Private searchLabel As Label
    Private _liveProjectSearchBox As TextBox
    Private selectorLabel As Label
    Private _liveProjectSelector As ComboBox
    Private btnScheduleProject As Button
    Private _liveProjectSizeLabel As Label
    Private btnRescheduleProject As Button
    Private projectTypeCaptionLabel As Label
    Private projectTypeValueLabel As Label
    Private gridPanel As Panel
    Private _grid As DataGridView
    Private listTitle As Label
    Private planningSummaryPanel As Panel
    Private summaryTitleLabel As Label
    Private summaryPeriodLabel As Label
    Private summaryCards As TableLayoutPanel
    Private newProjectsPanel As Panel
    Private newProjectsCountLabel As Label
    Private newProjectsLabel As Label
    Private updateProjectsPanel As Panel
    Private updateProjectsCountLabel As Label
    Private updateProjectsLabel As Label
    Private feedbackProjectsPanel As Panel
    Private feedbackProjectsCountLabel As Label
    Private feedbackProjectsLabel As Label
    Private _status As Label
    Private idColumn As DataGridViewTextBoxColumn
    Private projectColumn As DataGridViewTextBoxColumn
    Private versionColumn As DataGridViewTextBoxColumn
    Private sizeColumn As DataGridViewTextBoxColumn
    Private tasksColumn As DataGridViewTextBoxColumn
    Private hoursColumn As DataGridViewTextBoxColumn
    Private startColumn As DataGridViewTextBoxColumn
    Private finishColumn As DataGridViewTextBoxColumn
    Private updatedColumn As DataGridViewTextBoxColumn
End Class
