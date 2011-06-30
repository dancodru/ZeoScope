namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.DirectX.Direct3D;

    internal partial class MainScreen : Form
    {
        #region Privates
        private string fileName;

        private ManualResetEvent exitEvent = new ManualResetEvent(false);

        private Stopwatch timer;

        private ZeoStream zeoStream;

        private int eegLastPosition = 0;
        private int freqLastPosition = 0;
        private int stageLastPosition = 0;
        #endregion

        private delegate void StopDelegate(object sender, EventArgs e);

        #region Drawing methods
        internal void Render()
        {
            if (this.zeoStream != null)
            {
                this.eegScopePanel.ScopeData = this.zeoStream.ReadEegFromLastPosition(ref this.eegLastPosition, this.eegScopePanel.ScopeLength);
                this.freqScopePanel.ScopeData = this.zeoStream.ReadFrequencyDataFromLastPosition(ref this.freqLastPosition, this.freqScopePanel.ScopeLength);
                this.stageScopePanel.ScopeData = this.zeoStream.ReadStageDataFromLastPosition(ref this.stageLastPosition, this.stageScopePanel.ScopeLength);

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

                DateTime? time = this.zeoStream.GetTimeFromIndex(this.eegLastPosition);
                if (time != null)
                {
                    time.Value.AddSeconds(this.eegScopePanel.ScopeX / ZeoStream.SamplesPerSec);
                    this.eegScopePanel.TimeString = string.Format("t: {0}  +{1:0.000}s", time.Value.ToString("HH:mm:ss"), (double)this.eegScopePanel.ScopeX / ZeoMessage.SamplesPerMessage);
                    this.freqScopePanel.TimeString = string.Format("t: {0}", time.Value.ToString("HH:mm:ss"));
                    this.stageScopePanel.TimeString = string.Format("t: {0}", time.Value.AddSeconds(2).ToString("HH:mm:ss")); // TODO: fix AddSeconds(2)
                }

                if (this.stageScopePanel.ScopeX < this.stageScopePanel.ScopeData.Length)
                {
                    this.stageScopePanel.LabelFormatStrings[0] = Enum.GetName(typeof(ZeoSleepStage), (ZeoSleepStage)(-this.stageScopePanel.ScopeData[this.stageScopePanel.ScopeX].Values[0]));
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
            this.SetFormText(Path.GetFileName(this.fileName));
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

        private void StartToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.zeoStream != null)
            {
                this.zeoStream.Dispose();
                this.zeoStream = null;
                this.exitEvent.Reset();

                this.eegLastPosition = 0;
                this.eegScopePanel.ScrollBarMaximum = 0;
                this.eegScopePanel.ScrollBarValue = 0;
            }

            this.timer.Reset();
            this.timer.Start();

            this.zeoStream = new ZeoStream(this.exitEvent);
            this.zeoStream.HeadbandDocked += new EventHandler(this.ZeoStream_HeadbandDocked);
            this.zeoStream.OpenLiveStream(this.comPortToolStripComboBox.Text, this.fileNameToolStripComboBox.Text);

            this.SetFormText(this.zeoStream.FileName);

            this.startToolStripButton.Enabled = false;
            this.stopToolStripButton.Enabled = true;
        }

        private void ZeoStream_HeadbandDocked(object sender, EventArgs e)
        {
            StopDelegate stopDelegate = new StopDelegate(this.StopToolStripButton_Click);
            this.Invoke(stopDelegate, sender, e);
        }

        private void StopToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.zeoStream.LiveStream == true)
            {
                this.zeoStream.LiveStream = false;
                this.exitEvent.Set();
                this.startToolStripButton.Enabled = true;
                this.stopToolStripButton.Enabled = false;
                this.SetFormText(this.zeoStream.FileName);
                return;
            }
        }

        private void LoadToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.loadScopeFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.LoadFile(this.loadScopeFileDialog.FileName);
            }
        }

        private void EegLevelToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            float eegLevel;
            float.TryParse(this.eegLevelToolStripComboBox.Text, out eegLevel);

            this.eegScopePanel.MaxValueDisplay = new float[] { eegLevel };
            this.eegScopePanel.MinValueDisplay = new float[] { -eegLevel };
        }

        private void EegScopePanel_ScrollScope(object sender, ScrollEventArgs e)
        {
            if (this.zeoStream != null && this.zeoStream.LiveStream == false)
            {
                if (e.OldValue != e.NewValue)
                {
                    this.eegLastPosition = e.NewValue;
                    this.freqScopePanel.ScrollBarValue = this.eegLastPosition;
                    this.freqLastPosition = this.freqScopePanel.ScrollBarValue;
                    this.stageScopePanel.ScopeX = this.eegLastPosition / ZeoStream.SleepStageSeconds;
                    this.Render();
                }
            }
        }

        private void FreqScopePanel_ScrollScope(object sender, ScrollEventArgs e)
        {
            if (this.zeoStream != null && this.zeoStream.LiveStream == false)
            {
                if (e.OldValue != e.NewValue)
                {
                    this.freqLastPosition = e.NewValue;
                    this.eegScopePanel.ScrollBarValue = this.freqLastPosition;
                    this.eegLastPosition = this.freqLastPosition + this.freqScopePanel.ScopeX;
                    this.stageScopePanel.ScopeX = this.freqLastPosition / ZeoStream.SleepStageSeconds;
                    this.Render();
                }
            }
        }

        private void FreqScopePanel_DeviceMouseMove(object sender, MouseEventArgs e)
        {
            if (this.zeoStream != null && this.zeoStream.LiveStream == false)
            {
                int second = this.freqLastPosition + this.freqScopePanel.ScopeX;
                this.stageScopePanel.ScopeX = second / ZeoStream.SleepStageSeconds;
                this.eegScopePanel.ScrollBarValue = second;
                this.eegLastPosition = this.eegScopePanel.ScrollBarValue;
            }
        }

        private void StageScopePanel_DeviceMouseMove(object sender, MouseEventArgs e)
        {
            if (this.zeoStream != null && this.zeoStream.LiveStream == false)
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
            this.freqScopePanel.LabelFormatStrings = new string[] { "D: {0:0.00}", "T: {0:0.00}", "A: {0:0.00}", "B1: {0:0.00}", "B2: {0:0.00}", "B3: {0:0.00}", "G: {0:0.00}", "Imp: {0:0.0}" };
            this.freqScopePanel.HorizontalLinesCount = 0;
        }

        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            // TODO: is there a better way to treat OutOfVideoMemoryException
            OutOfVideoMemoryException.IgnoreExceptions();

            Application.EnableVisualStyles();
            using (MainScreen frm = new MainScreen())
            {
                try
                {
                    frm.InitializeComponent();
                }
                catch (ZeoException ex)
                {
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show(ex.Message, "Install DirectX Runtime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }

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
            this.fileName = file;

            if (this.zeoStream != null)
            {
                this.zeoStream.Dispose();
                this.zeoStream = null;
            }

            this.eegLastPosition = 0;

            this.zeoStream = new ZeoStream(this.exitEvent);
            try
            {
                this.zeoStream.OpenFileStream(file);
            }
            catch (ZeoException ex)
            {
                this.fileName = string.Empty;
                this.zeoStream = null;
                this.eegScopePanel.ScopeData = null;
                this.freqScopePanel.ScopeData = null;
                this.stageScopePanel.ScopeData = null;
                MessageBox.Show(ex.Message);
                return;
            }

            this.eegScopePanel.ScrollBarMaximum = this.zeoStream.Length;
            this.freqScopePanel.ScrollBarMaximum = this.zeoStream.Length;

            this.SetFormText(Path.GetFileName(this.fileName));
        }

        private void SetFormText(string fileName)
        {
            string formText = "ZeoScope ::";
            if (this.zeoStream != null && this.zeoStream.LiveStream)
            {
                formText += " Live signal :: " + this.comPortToolStripComboBox.Text + " ::";
            }

            if (string.IsNullOrEmpty(this.fileName) == false)
            {
                formText += " " + this.fileName;
            }

            this.Text = formText;
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
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
