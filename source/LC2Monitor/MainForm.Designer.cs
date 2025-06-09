namespace LC2Monitor
{
  partial class MainForm
  {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnStep = new System.Windows.Forms.Button();
      this.btnRun = new System.Windows.Forms.Button();
      this.btnStop = new System.Windows.Forms.Button();
      this.btnSendBinary = new System.Windows.Forms.Button();
      this.logsListBox = new System.Windows.Forms.ListBox();
      this.variableViewer = new DebugViews.VariableViewer();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblPlcStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblPLCCycle = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblPLCDuration = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblPLCDurationMax = new System.Windows.Forms.ToolStripStatusLabel();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.pLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.nONEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.saveProgramToFlashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.rTCSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.lCVMDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer1.Location = new System.Drawing.Point(20, 48);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.variableViewer);
      this.splitContainer1.Size = new System.Drawing.Size(1415, 604);
      this.splitContainer1.SplitterDistance = 1066;
      this.splitContainer1.SplitterWidth = 6;
      this.splitContainer1.TabIndex = 2;
      // 
      // splitContainer2
      // 
      this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel3);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.logsListBox);
      this.splitContainer2.Size = new System.Drawing.Size(1066, 604);
      this.splitContainer2.SplitterDistance = 113;
      this.splitContainer2.SplitterWidth = 6;
      this.splitContainer2.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(6);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 3;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 183F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1064, 111);
      this.tableLayoutPanel3.TabIndex = 5;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel4.Controls.Add(this.btnStep, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnRun, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnStop, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSendBinary, 4, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 6);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(6);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(1052, 94);
      this.tableLayoutPanel4.TabIndex = 5;
      // 
      // btnStep
      // 
      this.btnStep.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnStep.Location = new System.Drawing.Point(426, 6);
      this.btnStep.Margin = new System.Windows.Forms.Padding(6);
      this.btnStep.Name = "btnStep";
      this.btnStep.Size = new System.Drawing.Size(198, 82);
      this.btnStep.TabIndex = 4;
      this.btnStep.Text = "Step";
      this.btnStep.UseVisualStyleBackColor = true;
      // 
      // btnRun
      // 
      this.btnRun.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnRun.Location = new System.Drawing.Point(6, 6);
      this.btnRun.Margin = new System.Windows.Forms.Padding(6);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(198, 82);
      this.btnRun.TabIndex = 0;
      this.btnRun.Text = "Run";
      this.btnRun.UseVisualStyleBackColor = true;
      // 
      // btnStop
      // 
      this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnStop.Enabled = false;
      this.btnStop.Location = new System.Drawing.Point(216, 6);
      this.btnStop.Margin = new System.Windows.Forms.Padding(6);
      this.btnStop.Name = "btnStop";
      this.btnStop.Size = new System.Drawing.Size(198, 82);
      this.btnStop.TabIndex = 1;
      this.btnStop.Text = "Stop";
      this.btnStop.UseVisualStyleBackColor = true;
      // 
      // btnSendBinary
      // 
      this.btnSendBinary.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnSendBinary.Location = new System.Drawing.Point(846, 6);
      this.btnSendBinary.Margin = new System.Windows.Forms.Padding(6);
      this.btnSendBinary.Name = "btnSendBinary";
      this.btnSendBinary.Size = new System.Drawing.Size(200, 82);
      this.btnSendBinary.TabIndex = 3;
      this.btnSendBinary.Text = "Download";
      this.btnSendBinary.UseVisualStyleBackColor = true;
      // 
      // logsListBox
      // 
      this.logsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logsListBox.FormattingEnabled = true;
      this.logsListBox.ItemHeight = 24;
      this.logsListBox.Location = new System.Drawing.Point(0, 0);
      this.logsListBox.Margin = new System.Windows.Forms.Padding(4);
      this.logsListBox.Name = "logsListBox";
      this.logsListBox.Size = new System.Drawing.Size(1064, 483);
      this.logsListBox.TabIndex = 0;
      // 
      // variableViewer
      // 
      this.variableViewer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableViewer.Location = new System.Drawing.Point(0, 0);
      this.variableViewer.Margin = new System.Windows.Forms.Padding(4);
      this.variableViewer.Name = "variableViewer";
      this.variableViewer.Size = new System.Drawing.Size(341, 602);
      this.variableViewer.TabIndex = 1;
      this.variableViewer.VisibleLiveColumn = true;
      // 
      // statusStrip1
      // 
      this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblConnectionStatus,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.lblPlcStatus,
            this.toolStripStatusLabel6,
            this.toolStripStatusLabel4,
            this.lblPLCCycle,
            this.toolStripStatusLabel5,
            this.lblPLCDuration,
            this.toolStripStatusLabel7,
            this.lblPLCDurationMax});
      this.statusStrip1.Location = new System.Drawing.Point(0, 657);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 18, 0);
      this.statusStrip1.Size = new System.Drawing.Size(1454, 39);
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(74, 30);
      this.toolStripStatusLabel1.Text = "Status:";
      // 
      // lblConnectionStatus
      // 
      this.lblConnectionStatus.Name = "lblConnectionStatus";
      this.lblConnectionStatus.Size = new System.Drawing.Size(37, 30);
      this.lblConnectionStatus.Text = "---";
      // 
      // toolStripStatusLabel2
      // 
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new System.Drawing.Size(31, 30);
      this.toolStripStatusLabel2.Text = "   ";
      // 
      // toolStripStatusLabel3
      // 
      this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
      this.toolStripStatusLabel3.Size = new System.Drawing.Size(52, 30);
      this.toolStripStatusLabel3.Text = "PLC:";
      // 
      // lblPlcStatus
      // 
      this.lblPlcStatus.Name = "lblPlcStatus";
      this.lblPlcStatus.Size = new System.Drawing.Size(37, 30);
      this.lblPlcStatus.Text = "---";
      // 
      // toolStripStatusLabel6
      // 
      this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
      this.toolStripStatusLabel6.Size = new System.Drawing.Size(31, 30);
      this.toolStripStatusLabel6.Text = "   ";
      // 
      // toolStripStatusLabel4
      // 
      this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
      this.toolStripStatusLabel4.Size = new System.Drawing.Size(73, 30);
      this.toolStripStatusLabel4.Text = "Cycle: ";
      // 
      // lblPLCCycle
      // 
      this.lblPLCCycle.Name = "lblPLCCycle";
      this.lblPLCCycle.Size = new System.Drawing.Size(37, 30);
      this.lblPLCCycle.Text = "---";
      // 
      // toolStripStatusLabel5
      // 
      this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
      this.toolStripStatusLabel5.Size = new System.Drawing.Size(105, 30);
      this.toolStripStatusLabel5.Text = "Duration: ";
      // 
      // lblPLCDuration
      // 
      this.lblPLCDuration.Name = "lblPLCDuration";
      this.lblPLCDuration.Size = new System.Drawing.Size(37, 30);
      this.lblPLCDuration.Text = "---";
      // 
      // toolStripStatusLabel7
      // 
      this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
      this.toolStripStatusLabel7.Size = new System.Drawing.Size(151, 30);
      this.toolStripStatusLabel7.Text = "Max Duration: ";
      this.toolStripStatusLabel7.Visible = false;
      // 
      // lblPLCDurationMax
      // 
      this.lblPLCDurationMax.Name = "lblPLCDurationMax";
      this.lblPLCDurationMax.Size = new System.Drawing.Size(37, 30);
      this.lblPLCDurationMax.Text = "---";
      this.lblPLCDurationMax.Visible = false;
      // 
      // menuStrip1
      // 
      this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.pLCToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 4, 0, 4);
      this.menuStrip1.Size = new System.Drawing.Size(1454, 42);
      this.menuStrip1.TabIndex = 4;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // projectToolStripMenuItem
      // 
      this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
      this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
      this.projectToolStripMenuItem.Size = new System.Drawing.Size(95, 34);
      this.projectToolStripMenuItem.Text = "Project";
      // 
      // openToolStripMenuItem
      // 
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new System.Drawing.Size(182, 40);
      this.openToolStripMenuItem.Text = "Open";
      this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(182, 40);
      this.closeToolStripMenuItem.Text = "Close";
      // 
      // pLCToolStripMenuItem
      // 
      this.pLCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveProgramToFlashToolStripMenuItem,
            this.toolStripSeparator3,
            this.rTCSyncToolStripMenuItem,
            this.toolStripSeparator2,
            this.lCVMDumpToolStripMenuItem});
      this.pLCToolStripMenuItem.Name = "pLCToolStripMenuItem";
      this.pLCToolStripMenuItem.Size = new System.Drawing.Size(65, 34);
      this.pLCToolStripMenuItem.Text = "PLC";
      // 
      // connectToolStripMenuItem
      // 
      this.connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nONEToolStripMenuItem});
      this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
      this.connectToolStripMenuItem.Size = new System.Drawing.Size(337, 40);
      this.connectToolStripMenuItem.Text = "Connect";
      // 
      // nONEToolStripMenuItem
      // 
      this.nONEToolStripMenuItem.Name = "nONEToolStripMenuItem";
      this.nONEToolStripMenuItem.Size = new System.Drawing.Size(202, 40);
      this.nONEToolStripMenuItem.Text = "[NONE]";
      // 
      // disconnectToolStripMenuItem
      // 
      this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
      this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(337, 40);
      this.disconnectToolStripMenuItem.Text = "Disconnect";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(334, 6);
      // 
      // saveProgramToFlashToolStripMenuItem
      // 
      this.saveProgramToFlashToolStripMenuItem.Name = "saveProgramToFlashToolStripMenuItem";
      this.saveProgramToFlashToolStripMenuItem.Size = new System.Drawing.Size(337, 40);
      this.saveProgramToFlashToolStripMenuItem.Text = "Save program to Flash";
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(334, 6);
      // 
      // rTCSyncToolStripMenuItem
      // 
      this.rTCSyncToolStripMenuItem.Name = "rTCSyncToolStripMenuItem";
      this.rTCSyncToolStripMenuItem.Size = new System.Drawing.Size(337, 40);
      this.rTCSyncToolStripMenuItem.Text = "RTC Sync";
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(334, 6);
      // 
      // lCVMDumpToolStripMenuItem
      // 
      this.lCVMDumpToolStripMenuItem.Name = "lCVMDumpToolStripMenuItem";
      this.lCVMDumpToolStripMenuItem.Size = new System.Drawing.Size(337, 40);
      this.lCVMDumpToolStripMenuItem.Text = "LCVM Dump";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1454, 696);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.menuStrip1);
      this.Controls.Add(this.splitContainer1);
      this.MainMenuStrip = this.menuStrip1;
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "MainForm";
      this.Text = "LC2 Monitor";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private DebugViews.VariableViewer variableViewer;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.ToolStripStatusLabel lblConnectionStatus;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
    private System.Windows.Forms.ToolStripStatusLabel lblPlcStatus;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.Button btnSendBinary;
    private System.Windows.Forms.ListBox logsListBox;
    private System.Windows.Forms.Button btnStep;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem pLCToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem nONEToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem lCVMDumpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem rTCSyncToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveProgramToFlashToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
    private System.Windows.Forms.ToolStripStatusLabel lblPLCCycle;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
    private System.Windows.Forms.ToolStripStatusLabel lblPLCDuration;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel7;
    private System.Windows.Forms.ToolStripStatusLabel lblPLCDurationMax;
  }
}

