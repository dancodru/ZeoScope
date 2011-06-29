namespace ZeoScope
{
    using System;
    using System.Text;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using Microsoft.DirectX.Direct3D;

    class MainScreen : Form
    {
        #region Privates
        private string fileName;

        ManualResetEvent exitEvent = new ManualResetEvent(false);

        private Stopwatch timer;

        private ZeoStream zeoStream;

        ManualResetEvent _drawSin = new ManualResetEvent(false);
        private OpenFileDialog loadScopeFileDialog;

        private SaveFileDialog saveFileDialog;

        private ScopePanel eegScopePanel;
        private ScopePanel freqScopePanel;
        private ScopePanel stageScopePanel;

        private int eegLastPosition = 0;
        private int freqLastPosition = 0;
        private Splitter splitter;
        private ToolStrip toolStrip;
        private ToolStripButton startToolStripButton;
        private ToolStripButton stopToolStripButton;
        private ToolStripComboBox comPortToolStripComboBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton loadToolStripButton;
        private ToolStripLabel toolStripLabel2;
        private ToolStripLabel toolStripLabel1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripLabel toolStripLabel3;
        private ToolStripComboBox eegLevelToolStripComboBox;
        private ToolStripLabel toolStripLabel4;
        private ToolStripComboBox fileNameToolStripComboBox;
        private int stageLastPosition = 0;
        #endregion

        #region Drawing methods
        internal void Render()
        {
            if (this.zeoStream != null)
            {
                this.eegScopePanel.ScopeData = zeoStream.ReadEegFromLastPosition(ref eegLastPosition, this.eegScopePanel.ScopeLength);
                this.freqScopePanel.ScopeData = zeoStream.ReadFrequencyDataFromLastPosition(ref freqLastPosition, this.freqScopePanel.ScopeLength);
                this.stageScopePanel.ScopeData = zeoStream.ReadStageDataFromLastPosition(ref stageLastPosition, this.stageScopePanel.ScopeLength);

                if (this.zeoStream.LiveStream == true)
                {
                    int len = this.zeoStream.Length;
                    this.eegScopePanel.SuspendLayout();
                    this.freqScopePanel.SuspendLayout();
                    this.eegScopePanel.ScrollBarValue = len;
                    this.freqScopePanel.ScrollBarValue = len;
                    this.eegScopePanel.ScrollBarMaximum = len;
                    this.freqScopePanel.ScrollBarMaximum = len;
                    this.freqScopePanel.ResumeLayout();
                    this.eegScopePanel.ResumeLayout();
                }

                DateTime? time = zeoStream.GetTimeFromIndex(eegLastPosition);
                if (time != null)
                {
                    time.Value.AddSeconds(this.eegScopePanel.ScopeX / ZeoStream.SamplesPerSec);
                    this.eegScopePanel.TimeString = string.Format("t: {0}  +{1:0.000}s", time.Value.ToString("HH:mm:ss"), (double)this.eegScopePanel.ScopeX / ZeoMessage.SamplesPerMessage);
                    this.freqScopePanel.TimeString = string.Format("t: {0}", time.Value.ToString("HH:mm:ss"));
                    this.stageScopePanel.TimeString = string.Format("t: {0}", time.Value.ToString("HH:mm:ss"));
                }

                if (this.stageScopePanel.ScopeX < this.stageScopePanel.ScopeData.Length)
                {
                    this.stageScopePanel.LabelFormatStrings[0] = Enum.GetName(typeof(ZeoSleepStage), (ZeoSleepStage)(-stageScopePanel.ScopeData[stageScopePanel.ScopeX].Values[0]));
                }
            }

            this.eegScopePanel.RenderDevice();
            this.freqScopePanel.RenderDevice();
            this.stageScopePanel.RenderDevice();

            Thread.Sleep(15);
        }
        #endregion

        #region Events
        protected override void OnClosed(EventArgs e)
        {
            this.exitEvent.Set();

            base.OnClosed(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.Render();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Render();
        }

        private void MainScreen_Shown(object sender, EventArgs e)
        {
            SetFormText(Path.GetFileName(fileName));
        }

        private void MainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.zeoStream != null)
            {
                this.exitEvent.Set();
                Thread.Sleep(100);
                this.zeoStream.Dispose();
                this.zeoStream = null;
            }
        }

        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            if (zeoStream != null)
            {
                    zeoStream.Dispose();
                    zeoStream = null;
                    exitEvent.Reset();

                    eegLastPosition = 0;
                    this.eegScopePanel.ScrollBarMaximum = 0;
                    this.eegScopePanel.ScrollBarValue = 0;
            }

            this.timer.Reset();
            this.timer.Start();

            zeoStream = new ZeoStream(this.exitEvent);
            zeoStream.HeadbandDocked += new EventHandler(zeoStream_HeadbandDocked);
            zeoStream.OpenLiveStream(this.comPortToolStripComboBox.Text, this.fileNameToolStripComboBox.Text);

            this.SetFormText(zeoStream.FileName);

            startToolStripButton.Enabled = false;
            stopToolStripButton.Enabled = true;
        }

        private delegate void StopDelegate(object sender, EventArgs e);

        private void zeoStream_HeadbandDocked(object sender, EventArgs e)
        {
            StopDelegate stopDelegate = new StopDelegate(stopToolStripButton_Click);
            this.Invoke(stopDelegate, sender, e);
        }

        private void stopToolStripButton_Click(object sender, EventArgs e)
        {
            if (zeoStream.LiveStream == true)
            {
                zeoStream.LiveStream = false;
                exitEvent.Set();
                startToolStripButton.Enabled = true;
                stopToolStripButton.Enabled = false;
                this.SetFormText(zeoStream.FileName);
                return;
            }
        }

        private void loadToolStripButton_Click(object sender, EventArgs e)
        {
            if (loadScopeFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadFile(loadScopeFileDialog.FileName);
            }
        }

        private void eegLevelToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            float eegLevel;
            float.TryParse(this.eegLevelToolStripComboBox.Text, out eegLevel);

            this.eegScopePanel.MaxValueDisplay = new float[] { eegLevel };
            this.eegScopePanel.MinValueDisplay = new float[] { -eegLevel };
        }

        private void eegScopePanel_ScrollScope(object sender, ScrollEventArgs e)
        {
            if (zeoStream != null && zeoStream.LiveStream == false)
            {
                if (e.OldValue != e.NewValue)
                {
                    eegLastPosition = e.NewValue;
                    this.freqScopePanel.ScrollBarValue = eegLastPosition;
                    this.freqLastPosition = this.freqScopePanel.ScrollBarValue;
                    this.stageScopePanel.ScopeX = eegLastPosition / ZeoStream.SleepStageSeconds;
                    this.Render();
                }
            }
        }

        private void freqScopePanel_ScrollScope(object sender, ScrollEventArgs e)
        {
            if (zeoStream != null && zeoStream.LiveStream == false)
                if (e.OldValue != e.NewValue)
                {
                    freqLastPosition = e.NewValue;
                    this.eegScopePanel.ScrollBarValue = freqLastPosition;
                    this.eegLastPosition = freqLastPosition + this.freqScopePanel.ScopeX;
                    this.stageScopePanel.ScopeX = freqLastPosition / ZeoStream.SleepStageSeconds;
                    this.Render();
                }
        }

        private void freqScopePanel_DeviceMouseMove(object sender, MouseEventArgs e)
        {
            if (zeoStream != null && zeoStream.LiveStream == false)
            {
                int second = freqLastPosition + freqScopePanel.ScopeX;
                this.stageScopePanel.ScopeX = second / ZeoStream.SleepStageSeconds;
                this.eegScopePanel.ScrollBarValue = second;
                this.eegLastPosition = this.eegScopePanel.ScrollBarValue;
            }
        }

        private void stageScopePanel_DeviceMouseMove(object sender, MouseEventArgs e)
        {
            if (zeoStream != null && zeoStream.LiveStream == false)
            {
                int second = this.stageScopePanel.ScopeX * ZeoStream.SleepStageSeconds;

                this.eegScopePanel.ScrollBarValue = second;
                this.eegLastPosition = this.eegScopePanel.ScrollBarValue;
                this.freqScopePanel.ScrollBarValue = second;
                this.freqLastPosition = this.freqScopePanel.ScrollBarValue;
            }
        }

        private void MainScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        #endregion

        #region Methods
        public void InitializeGraphics()
        {
            this.MinimumSize = new Size((int)(ZeoStream.SamplesPerSec * 2), 100);

            this.timer = new Stopwatch();

            this.comPortToolStripComboBox.Items.Clear();
            
            // TODO: this takes a lot of time, need to create a thread/delegate here
            this.comPortToolStripComboBox.Items.AddRange(Ftdi.GetComPortList());
            if (this.comPortToolStripComboBox.Items.Count > 0)
            {
                this.comPortToolStripComboBox.SelectedIndex = 0;
            }

            this.eegLevelToolStripComboBox.SelectedIndex = 3;
            float eegLevel;
            float.TryParse(this.eegLevelToolStripComboBox.Text, out eegLevel);

            this.eegScopePanel.MaxValueDisplay = new float[] { eegLevel };
            this.eegScopePanel.MinValueDisplay = new float[] { -eegLevel };

            this.freqScopePanel.SamplesPerSecond = 1.0;
            this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 1;
            this.freqScopePanel.MaxValueDisplay = new float[] { 50.0f, 50.0f, 50.0f, 50.0f, 50.0f, 50.0f, 3.0f, 1500.0f };
            this.freqScopePanel.MinValueDisplay = new float[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            this.freqScopePanel.GraphColors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Magenta, Color.Yellow, Color.Coral, Color.FromArgb(80, 80, 80) };
            this.freqScopePanel.LabelFormatStrings = new string[] { "D: {0:0.00}", "T: {0:0.00}", "A: {0:0.00}", "B1: {0:0.00}", "B2: {0:0.00}", "B3: {0:0.00}", "G: {0:0.00}", "Imp: {0:0.0}",};
            this.freqScopePanel.HorizontalLinesCount = 0;
        }

        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.loadScopeFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitter = new System.Windows.Forms.Splitter();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.eegScopePanel = new ZeoScope.ScopePanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.startToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.comPortToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.fileNameToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.eegLevelToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.freqScopePanel = new ZeoScope.ScopePanel();
            this.stageScopePanel = new ZeoScope.ScopePanel();
            this.eegScopePanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadScopeFileDialog
            // 
            this.loadScopeFileDialog.Filter = "Zeo files (*.zeo)|*.zeo";
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.Color.DimGray;
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 176);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(784, 3);
            this.splitter.TabIndex = 15;
            this.splitter.TabStop = false;
            // 
            // eegScopePanel
            // 
            this.eegScopePanel.Controls.Add(this.toolStrip);
            this.eegScopePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eegScopePanel.GraphColors = new System.Drawing.Color[] {
        System.Drawing.Color.Green};
            this.eegScopePanel.HorizontalLinesCount = 1;
            this.eegScopePanel.LabelFormatStrings = new string[] {
        "\t{0:0.000}mv"};
            this.eegScopePanel.LabelSpacing = 0;
            this.eegScopePanel.Location = new System.Drawing.Point(0, 0);
            this.eegScopePanel.MaxValueDisplay = null;
            this.eegScopePanel.MinValueDisplay = null;
            this.eegScopePanel.Name = "eegScopePanel";
            this.eegScopePanel.NumberOfChannels = 1;
            this.eegScopePanel.SamplesPerSecond = 128D;
            this.eegScopePanel.ScopeData = null;
            this.eegScopePanel.ScopeX = 0;
            this.eegScopePanel.ScrollBarMaximum = 0;
            this.eegScopePanel.ScrollBarValue = 0;
            this.eegScopePanel.Size = new System.Drawing.Size(784, 179);
            this.eegScopePanel.TabIndex = 0;
            this.eegScopePanel.TimeString = null;
            this.eegScopePanel.Title = "EEG";
            this.eegScopePanel.ScrollScope += new System.Windows.Forms.ScrollEventHandler(this.eegScopePanel_ScrollScope);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripButton,
            this.stopToolStripButton,
            this.toolStripLabel4,
            this.comPortToolStripComboBox,
            this.toolStripSeparator1,
            this.loadToolStripButton,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.fileNameToolStripComboBox,
            this.toolStripLabel1,
            this.toolStripSeparator3,
            this.toolStripLabel3,
            this.eegLevelToolStripComboBox});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(784, 25);
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
            this.startToolStripButton.Click += new System.EventHandler(this.startToolStripButton_Click);
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
            this.stopToolStripButton.Click += new System.EventHandler(this.stopToolStripButton_Click);
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
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // loadToolStripButton
            // 
            this.loadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("loadToolStripButton.Image")));
            this.loadToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadToolStripButton.Name = "loadToolStripButton";
            this.loadToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.loadToolStripButton.Text = "Open Zeo File";
            this.loadToolStripButton.Click += new System.EventHandler(this.loadToolStripButton_Click);
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
            this.fileNameToolStripComboBox.AutoToolTip = true;
            this.fileNameToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.fileNameToolStripComboBox.Name = "fileNameToolStripComboBox";
            this.fileNameToolStripComboBox.Size = new System.Drawing.Size(75, 25);
            this.fileNameToolStripComboBox.Text = "ZeoData";
            this.fileNameToolStripComboBox.ToolTipText = "File Name";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(76, 22);
            this.toolStripLabel1.Text = "_<Time>.zeo";
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
            this.eegLevelToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.eegLevelToolStripComboBox_SelectedIndexChanged);
            // 
            // freqScopePanel
            // 
            this.freqScopePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.freqScopePanel.GraphColors = null;
            this.freqScopePanel.HorizontalLinesCount = 0;
            this.freqScopePanel.LabelFormatStrings = null;
            this.freqScopePanel.LabelSpacing = 100;
            this.freqScopePanel.Location = new System.Drawing.Point(0, 179);
            this.freqScopePanel.MaxValueDisplay = null;
            this.freqScopePanel.MinValueDisplay = null;
            this.freqScopePanel.Name = "freqScopePanel";
            this.freqScopePanel.NumberOfChannels = 0;
            this.freqScopePanel.SamplesPerSecond = 1D;
            this.freqScopePanel.ScopeData = null;
            this.freqScopePanel.ScopeX = 0;
            this.freqScopePanel.ScrollBarMaximum = 0;
            this.freqScopePanel.ScrollBarValue = 0;
            this.freqScopePanel.Size = new System.Drawing.Size(784, 200);
            this.freqScopePanel.SplitterPanelVisible = false;
            this.freqScopePanel.TabIndex = 3;
            this.freqScopePanel.TimeString = null;
            this.freqScopePanel.Title = "Frequency";
            this.freqScopePanel.ScrollScope += new System.Windows.Forms.ScrollEventHandler(this.freqScopePanel_ScrollScope);
            this.freqScopePanel.DeviceMouseMove += new System.Windows.Forms.MouseEventHandler(this.freqScopePanel_DeviceMouseMove);
            // 
            // stageScopePanel
            // 
            this.stageScopePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stageScopePanel.GraphColors = new System.Drawing.Color[] {
        System.Drawing.Color.Cyan};
            this.stageScopePanel.HorizontalLinesCount = 4;
            this.stageScopePanel.LabelFormatStrings = new string[] {
        "{0}"};
            this.stageScopePanel.LabelSpacing = 0;
            this.stageScopePanel.Location = new System.Drawing.Point(0, 379);
            this.stageScopePanel.MaxValueDisplay = new float[] {
        0F};
            this.stageScopePanel.MinValueDisplay = new float[] {
        -5F};
            this.stageScopePanel.Name = "stageScopePanel";
            this.stageScopePanel.NumberOfChannels = 1;
            this.stageScopePanel.SamplesPerSecond = 0.0333D;
            this.stageScopePanel.ScopeData = null;
            this.stageScopePanel.ScopeX = 0;
            this.stageScopePanel.ScrollBarMaximum = 0;
            this.stageScopePanel.ScrollBarValue = 0;
            this.stageScopePanel.ScrollBarVisible = false;
            this.stageScopePanel.Size = new System.Drawing.Size(784, 183);
            this.stageScopePanel.SplitterPanelVisible = false;
            this.stageScopePanel.TabIndex = 5;
            this.stageScopePanel.TimeString = null;
            this.stageScopePanel.Title = "Stage";
            this.stageScopePanel.DeviceMouseMove += new System.Windows.Forms.MouseEventHandler(this.stageScopePanel_DeviceMouseMove);
            // 
            // MainScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.eegScopePanel);
            this.Controls.Add(this.freqScopePanel);
            this.Controls.Add(this.stageScopePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            // TODO: is there a better way to treat OutOfVideoMemoryException
            OutOfVideoMemoryException.IgnoreExceptions();

            Application.EnableVisualStyles();
            using (MainScreen frm = new MainScreen())
            {
                frm.InitializeComponent();
                frm.InitializeGraphics();
                frm.Show();
                frm.Render();
                if (args.Length > 0)
                {
                    if (File.Exists(args[0]))
                    {
                        frm.LoadFile(args[0]);
                    }
                }

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    if (frm.WindowState != FormWindowState.Minimized)
                    {
                        frm.Render();
                    }

                    Application.DoEvents();
                }
            }
        }

        private void LoadFile(string file)
        {
            fileName = file;

            if (zeoStream != null)
            {
                zeoStream.Dispose();
                zeoStream = null;
            }

            eegLastPosition = 0;

            zeoStream = new ZeoStream(this.exitEvent);
            zeoStream.OpenFileStream(file);
            this.eegScopePanel.ScrollBarMaximum = zeoStream.Length;
            this.freqScopePanel.ScrollBarMaximum = zeoStream.Length;

            SetFormText(Path.GetFileName(fileName));
        }

        private void SetFormText(string fileName)
        {
            string formText = "ZeoScope ::";
            if (zeoStream != null && zeoStream.LiveStream)
            {
                formText += " Live signal :: " + this.comPortToolStripComboBox.Text + " ::";
            }

            if (string.IsNullOrEmpty(fileName) == false)
            {
                formText += " " + fileName;
            }

            this.Text = formText;
        }

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            TextWriter exceptionLog = new StreamWriter("ExceptionLog.log", true);
            exceptionLog.WriteLine("\r\n==============================================================");
            exceptionLog.WriteLine("\r\n==============================================================");
            exceptionLog.WriteLine("Date: {0}", DateTime.Now);
            exceptionLog.WriteLine(ex.ToString());
            exceptionLog.Close();

            MessageBox.Show(ex.ToString());
        }
        #endregion
    }
}
