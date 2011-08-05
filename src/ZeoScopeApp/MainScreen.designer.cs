namespace ZeoScope
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal partial class MainScreen
    {
        private ScopePanel eegScopePanel;
        private ScopePanel freqScopePanel;
        private ScopePanel stageScopePanel;
        private Splitter splitter;
        private ToolStrip toolStrip;
        private ToolStripButton startToolStripButton;
        private ToolStripButton stopToolStripButton;
        private ToolStripComboBox comPortToolStripComboBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton openToolStripButton;
        private ToolStripLabel toolStripLabel2;
        private ToolStripLabel toolStripLabel1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripLabel toolStripLabel3;
        private ToolStripComboBox eegLevelToolStripComboBox;
        private ToolStripLabel toolStripLabel4;
        private ToolStripComboBox fileNameToolStripComboBox;
        private OpenFileDialog openScopeFileDialog;
        private SaveFileDialog saveFileDialog;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton settingsToolStripButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.openScopeFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitter = new System.Windows.Forms.Splitter();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.eegScopePanel = new ZeoScope.ScopePanel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.startToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.comPortToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.fileNameToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.eegLevelToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.alarmStateToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.freqScopePanel = new ZeoScope.ScopePanel();
            this.stageScopePanel = new ZeoScope.ScopePanel();
            this.eegScopePanel.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openScopeFileDialog
            // 
            this.openScopeFileDialog.Filter = "Zeo files (*.zeo)|*.zeo";
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.Color.DimGray;
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 276);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(967, 3);
            this.splitter.TabIndex = 15;
            this.splitter.TabStop = false;
            // 
            // eegScopePanel
            // 
            this.eegScopePanel.ContextMenuStrip = this.contextMenuStrip;
            this.eegScopePanel.Controls.Add(this.toolStrip);
            this.eegScopePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eegScopePanel.GraphColors = new System.Drawing.Color[] {
        System.Drawing.Color.Green};
            this.eegScopePanel.HorizontalLinesCount = 1;
            this.eegScopePanel.LabelFormatStrings = new string[] {
        "\t{0:0.000}mv"};
            this.eegScopePanel.LabelSpacing = 90;
            this.eegScopePanel.Location = new System.Drawing.Point(0, 0);
            this.eegScopePanel.MaxValueDisplay = null;
            this.eegScopePanel.MinValueDisplay = null;
            this.eegScopePanel.Name = "eegScopePanel";
            this.eegScopePanel.NumberOfChannels = 1;
            this.eegScopePanel.SamplesPerSecond = 128D;
            this.eegScopePanel.ScopeData = null;
            this.eegScopePanel.ScopeX = 95;
            this.eegScopePanel.ScrollBarMaximum = 0;
            this.eegScopePanel.ScrollBarValue = 0;
            this.eegScopePanel.Size = new System.Drawing.Size(967, 279);
            this.eegScopePanel.TabIndex = 0;
            this.eegScopePanel.TimeString = null;
            this.eegScopePanel.Title = "EEG";
            this.eegScopePanel.ScrollScope += new System.Windows.Forms.ScrollEventHandler(this.EegScopePanel_ScrollScope);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(103, 136);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem1.Text = "2%";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem2.Text = "5%";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem3.Text = "10%";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem4.Text = "20%";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem5.Text = "50%";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(102, 22);
            this.toolStripMenuItem6.Text = "100%";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripButton,
            this.stopToolStripButton,
            this.toolStripLabel4,
            this.comPortToolStripComboBox,
            this.toolStripSeparator1,
            this.openToolStripButton,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.fileNameToolStripComboBox,
            this.toolStripLabel1,
            this.toolStripSeparator3,
            this.toolStripLabel3,
            this.eegLevelToolStripComboBox,
            this.toolStripSeparator4,
            this.settingsToolStripButton,
            this.alarmStateToolStripLabel});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip.Size = new System.Drawing.Size(967, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip";
            // 
            // startToolStripButton
            // 
            this.startToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("startToolStripButton.Image")));
            this.startToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startToolStripButton.Name = "startToolStripButton";
            this.startToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.startToolStripButton.Text = "Start Recording";
            this.startToolStripButton.ToolTipText = "Start Recording";
            this.startToolStripButton.Click += new System.EventHandler(this.StartToolStripButton_Click);
            // 
            // stopToolStripButton
            // 
            this.stopToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopToolStripButton.Enabled = false;
            this.stopToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripButton.Image")));
            this.stopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopToolStripButton.Name = "stopToolStripButton";
            this.stopToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.stopToolStripButton.Text = "Stop Recording";
            this.stopToolStripButton.ToolTipText = "Stop Recording";
            this.stopToolStripButton.Click += new System.EventHandler(this.StopToolStripButton_Click);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(13, 22);
            this.toolStripLabel4.Text = "  ";
            // 
            // comPortToolStripComboBox
            // 
            this.comPortToolStripComboBox.AutoToolTip = true;
            this.comPortToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPortToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comPortToolStripComboBox.Name = "comPortToolStripComboBox";
            this.comPortToolStripComboBox.Size = new System.Drawing.Size(75, 25);
            this.comPortToolStripComboBox.ToolTipText = "COM Port";
            this.comPortToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.ComPortToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "Open ZeoScope File";
            this.openToolStripButton.ToolTipText = "Open Zeo File";
            this.openToolStripButton.Click += new System.EventHandler(this.OpenToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(69, 22);
            this.toolStripLabel2.Text = "   FileName:";
            // 
            // fileNameToolStripComboBox
            // 
            this.fileNameToolStripComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.fileNameToolStripComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fileNameToolStripComboBox.AutoToolTip = true;
            this.fileNameToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.fileNameToolStripComboBox.MaxDropDownItems = 20;
            this.fileNameToolStripComboBox.Name = "fileNameToolStripComboBox";
            this.fileNameToolStripComboBox.Size = new System.Drawing.Size(100, 25);
            this.fileNameToolStripComboBox.Text = "ZeoData";
            this.fileNameToolStripComboBox.ToolTipText = "File Name (editable)";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(85, 22);
            this.toolStripLabel1.Text = "_<Time>.zeo   ";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(69, 22);
            this.toolStripLabel3.Text = "   EEG Level:";
            // 
            // eegLevelToolStripComboBox
            // 
            this.eegLevelToolStripComboBox.AutoToolTip = true;
            this.eegLevelToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eegLevelToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.eegLevelToolStripComboBox.Items.AddRange(new object[] {
            "10",
            "25",
            "50",
            "100",
            "200",
            "500"});
            this.eegLevelToolStripComboBox.Name = "eegLevelToolStripComboBox";
            this.eegLevelToolStripComboBox.Size = new System.Drawing.Size(75, 25);
            this.eegLevelToolStripComboBox.ToolTipText = "EEG Level";
            this.eegLevelToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.EegLevelToolStripComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // settingsToolStripButton
            // 
            this.settingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripButton.Image")));
            this.settingsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsToolStripButton.Name = "settingsToolStripButton";
            this.settingsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.settingsToolStripButton.Text = "Settings";
            this.settingsToolStripButton.ToolTipText = "Settings\r\nAlarm";
            this.settingsToolStripButton.Click += new System.EventHandler(this.SettingsToolStripButton_Click);
            // 
            // alarmStateToolStripLabel
            // 
            this.alarmStateToolStripLabel.Name = "alarmStateToolStripLabel";
            this.alarmStateToolStripLabel.Size = new System.Drawing.Size(66, 22);
            this.alarmStateToolStripLabel.Text = "Alarm: OFF";
            // 
            // freqScopePanel
            // 
            this.freqScopePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.freqScopePanel.GraphColors = null;
            this.freqScopePanel.HorizontalLinesCount = 0;
            this.freqScopePanel.LabelFormatStrings = null;
            this.freqScopePanel.LabelSpacing = 90;
            this.freqScopePanel.Location = new System.Drawing.Point(0, 279);
            this.freqScopePanel.MaxValueDisplay = null;
            this.freqScopePanel.MinValueDisplay = null;
            this.freqScopePanel.Name = "freqScopePanel";
            this.freqScopePanel.NumberOfChannels = 0;
            this.freqScopePanel.SamplesPerSecond = 1D;
            this.freqScopePanel.ScopeData = null;
            this.freqScopePanel.ScopeX = 0;
            this.freqScopePanel.ScrollBarMaximum = 0;
            this.freqScopePanel.ScrollBarValue = 0;
            this.freqScopePanel.Size = new System.Drawing.Size(967, 100);
            this.freqScopePanel.SplitterPanelVisible = false;
            this.freqScopePanel.TabIndex = 3;
            this.freqScopePanel.TimeString = null;
            this.freqScopePanel.Title = "Frequency";
            this.freqScopePanel.ScrollScope += new System.Windows.Forms.ScrollEventHandler(this.FreqScopePanel_ScrollScope);
            this.freqScopePanel.DeviceMouseMove += new System.Windows.Forms.MouseEventHandler(this.FreqScopePanel_DeviceMouseMove);
            // 
            // stageScopePanel
            // 
            this.stageScopePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stageScopePanel.GraphColors = new System.Drawing.Color[] {
        System.Drawing.Color.Cyan,
        System.Drawing.Color.Brown};
            this.stageScopePanel.HorizontalLinesCount = 4;
            this.stageScopePanel.LabelFormatStrings = new string[] {
        "{0}",
        " "};
            this.stageScopePanel.LabelSpacing = 90;
            this.stageScopePanel.Location = new System.Drawing.Point(0, 379);
            this.stageScopePanel.MaxValueDisplay = new float[] {
        0F,
        2000F};
            this.stageScopePanel.MinValueDisplay = new float[] {
        -5F,
        -9000F};
            this.stageScopePanel.Name = "stageScopePanel";
            this.stageScopePanel.NumberOfChannels = 2;
            this.stageScopePanel.SamplesPerSecond = 0.0333D;
            this.stageScopePanel.ScopeData = null;
            this.stageScopePanel.ScopeX = 0;
            this.stageScopePanel.ScrollBarMaximum = 0;
            this.stageScopePanel.ScrollBarValue = 0;
            this.stageScopePanel.ScrollBarVisible = false;
            this.stageScopePanel.Size = new System.Drawing.Size(967, 183);
            this.stageScopePanel.SplitterPanelVisible = false;
            this.stageScopePanel.TabIndex = 5;
            this.stageScopePanel.TimeString = null;
            this.stageScopePanel.Title = "Stage";
            this.stageScopePanel.DeviceMouseMove += new System.Windows.Forms.MouseEventHandler(this.StageScopePanel_DeviceMouseMove);
            // 
            // MainScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(967, 562);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.eegScopePanel);
            this.Controls.Add(this.freqScopePanel);
            this.Controls.Add(this.stageScopePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.Name = "MainScreen";
            this.Tag = "Zeo Scope";
            this.Text = "Zeo Scope";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainScreen_FormClosed);
            this.Shown += new System.EventHandler(this.MainScreen_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainScreen_KeyPress);
            this.eegScopePanel.ResumeLayout(false);
            this.eegScopePanel.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ToolStripLabel alarmStateToolStripLabel;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
    }
}
