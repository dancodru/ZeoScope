namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.DirectX.Direct3D;

    internal partial class MainScreen : Form
    {
        #region Privates
        private readonly string alarmOnString = "Alarm: ON";
        private readonly string alarmOffString = string.Empty;

        private string fileName;

        private ManualResetEvent exitEvent = new ManualResetEvent(false);

        private Stopwatch timer;

        private ZeoStream zeoStream;

        private SoundAlarm soundAlarm;

        private int eegLastPosition = 0;
        private int freqLastPosition = 0;
        private int stageLastPosition = 0;
        #endregion

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

                DateTime? time = null;
                if (this.zeoStream.LiveStream == true)
                {
                    time = this.zeoStream.GetTimeFromIndex(this.zeoStream.Length - 1);
                }
                else
                {
                    time = this.zeoStream.GetTimeFromIndex(this.eegLastPosition);
                }

                if (time != null)
                {
                    time.Value.AddSeconds(this.eegScopePanel.ScopeX / ZeoStream.SamplesPerSec);
                    this.eegScopePanel.TimeString = string.Format("{0}  +{1:0.000}s", time.Value.ToString("HH:mm:ss"), (double)this.eegScopePanel.ScopeX / ZeoMessage.SamplesPerMessage);
                    this.freqScopePanel.TimeString = string.Format("{0}", time.Value.ToString("HH:mm:ss"));
                    this.stageScopePanel.TimeString = string.Format("{0}", time.Value.AddSeconds(2).ToString("HH:mm:ss")); // TODO: fix AddSeconds(2)
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            ZeoSettings.Default.FreqPanelHeight = this.freqScopePanel.Height;
            ZeoSettings.Default.WindowMaximized = this.WindowState == FormWindowState.Maximized;
            if (this.WindowState == FormWindowState.Normal)
            {
                ZeoSettings.Default.WindowHeight = this.Height;
                ZeoSettings.Default.WindowWidth = this.Width;
            }

            ZeoSettings.Default.Save();
        }

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
            // TODO: Fix this workaround
            this.eegScopePanel.Height++;
            this.eegScopePanel.Height--;

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

            if (ZeoSettings.Default.AlarmEnabled == true && File.Exists(ZeoSettings.Default.MP3FileName) == true)
            {
                this.soundAlarm = new SoundAlarm(ZeoSettings.Default.MP3FileName, ZeoSettings.Default.AlarmFadeIn,
                    ZeoSettings.Default.AlarmFadeOut, ZeoSettings.Default.AlarmDuration, ZeoSettings.Default.AlarmFromTime,
                    ZeoSettings.Default.AlarmToTime, ZeoSettings.Default.AlarmSnooze, ZeoSettings.Default.AlarmCue);

                // Add one more channel for Alarm sound volume
                this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 2;
                this.freqScopePanel.MaxValueDisplay[this.freqScopePanel.NumberOfChannels - 1] = SoundAlarm.MaxVolume + 2000;
                this.freqScopePanel.MinValueDisplay[this.freqScopePanel.NumberOfChannels - 1] = SoundAlarm.MinVolume - 200;

                this.stageScopePanel.MaxValueDisplay[this.stageScopePanel.NumberOfChannels - 1] = SoundAlarm.MaxVolume + 2000;
                this.stageScopePanel.MinValueDisplay[this.stageScopePanel.NumberOfChannels - 1] = SoundAlarm.MinVolume - 200;
            }
            else
            {
                this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 1;
            }

            this.zeoStream.OpenLiveStream(this.comPortToolStripComboBox.Text, this.fileNameToolStripComboBox.Text, this.soundAlarm);

            this.SetFormText(this.zeoStream.FileName);

            this.startToolStripButton.Enabled = false;
            this.settingsToolStripButton.Enabled = false;
            this.openToolStripButton.Enabled = false;
            this.stopToolStripButton.Enabled = true;

            this.SaveFileNames();
        }

        private void ZeoStream_HeadbandDocked(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(this.StopToolStripButton_Click), sender, e);
        }

        private void StopToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.zeoStream.LiveStream == true)
            {
                this.exitEvent.Set();

                this.startToolStripButton.Enabled = true;
                this.settingsToolStripButton.Enabled = true;
                this.openToolStripButton.Enabled = true;
                this.stopToolStripButton.Enabled = false;

                this.SetFormText(this.zeoStream.FileName);

                if (this.soundAlarm != null)
                {
                    this.soundAlarm.Dispose();
                    this.soundAlarm = null;
                }

                return;
            }
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.openScopeFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.LoadFile(this.openScopeFileDialog.FileName);
            }
        }

        private void EegLevelToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            float eegLevel;
            float.TryParse(this.eegLevelToolStripComboBox.Text, out eegLevel);

            this.eegScopePanel.MaxValueDisplay = new float[] { eegLevel };
            this.eegScopePanel.MinValueDisplay = new float[] { -eegLevel };

            ZeoSettings.Default.EegLevelSelecteIndex = this.eegLevelToolStripComboBox.SelectedIndex;
            ZeoSettings.Default.Save();
        }

        private void ComPortToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZeoSettings.Default.ComPortSelectedIndex = this.comPortToolStripComboBox.SelectedIndex;
            ZeoSettings.Default.Save();
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

        private void SettingsToolStripButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
            settingsForm.Dispose();

            if (ZeoSettings.Default.AlarmEnabled == true)
            {
                this.alarmStateToolStripLabel.Text = this.alarmOnString;
                this.alarmStateToolStripLabel.ForeColor = Color.DarkGreen;
            }
            else
            {
                this.alarmStateToolStripLabel.Text = this.alarmOffString;
                this.alarmStateToolStripLabel.ForeColor = SystemColors.ControlText;
            }
        }

        private void MainScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Stop the allarm if any key is pressed
            if (this.soundAlarm != null && this.soundAlarm.AlarmStarted == true)
            {
                this.soundAlarm.AlarmStarted = false;
                return;
            }

            if (this.fileNameToolStripComboBox.Focused == true)
            {
                return;
            }

            switch (e.KeyChar)
            {
                case ' ':
                    {
                        if (this.startToolStripButton.Enabled == true)
                        {
                            this.StartToolStripButton_Click(this.startToolStripButton, null);
                        }

                        break;
                    }

                default:
                    break;
            }
        }

        private void BluetoothSpeaker_RawInput(object sender, SlimDX.RawInput.RawInputEventArgs e)
        {
            if (e != null && e.RawData != null & e.RawData.Length == 5)
            {
                // Second byte value from RawData:
                // 181 Scan Next Track
                // 182 Scan Previous Track
                // 205 Play/Pause
                byte keyPressed = e.RawData[1];

                if (keyPressed == 181 || keyPressed == 182 || keyPressed == 205)
                {
                    // Stop the allarm if any key is pressed
                    if (this.soundAlarm != null && this.soundAlarm.AlarmStarted == true)
                    {
                        this.soundAlarm.AlarmStarted = false;
                        return;
                    }
                }

                if (keyPressed == 205)
                {
                    if (this.startToolStripButton.Enabled == true)
                    {
                        this.StartToolStripButton_Click(this, null);
                        return;
                    }

                    if (this.stopToolStripButton.Enabled == true)
                    {
                        this.StopToolStripButton_Click(this, null);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Methods
        public void InitializeGraphics()
        {
            this.MinimumSize = new Size((int)(ZeoStream.SamplesPerSec * 2), 100);

            this.timer = new Stopwatch();

            this.eegLevelToolStripComboBox.SelectedIndex = ZeoSettings.Default.EegLevelSelecteIndex;
            float eegLevel;
            float.TryParse(this.eegLevelToolStripComboBox.Text, out eegLevel);

            this.eegScopePanel.MaxValueDisplay = new float[] { eegLevel };
            this.eegScopePanel.MinValueDisplay = new float[] { -eegLevel };

            if (this.fileNameToolStripComboBox.Items.Count > 0)
            {
                this.fileNameToolStripComboBox.SelectedIndex = 0;
            }

            SoundAlarm.SetVolumes(ZeoSettings.Default.MaxVolume);

            this.freqScopePanel.SamplesPerSecond = 1.0;
            this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 2; // Include Impedance and Sound Alarm Volume
            this.freqScopePanel.MaxValueDisplay = new float[] { 50.0f, 50.0f, 50.0f, 50.0f, 50.0f, 50.0f, 2.0f, 1500.0f, SoundAlarm.MaxVolume + 2000 };
            this.freqScopePanel.MinValueDisplay = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, SoundAlarm.MinVolume - 200 };
            this.freqScopePanel.GraphColors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Magenta, Color.Yellow, Color.Coral, Color.FromArgb(80, 80, 80), Color.Brown };
            this.freqScopePanel.LabelFormatStrings = new string[] { "D: {0:0.00}", "T: {0:0.00}", "A: {0:0.00}", "B1: {0:0.00}", "B2: {0:0.00}", "B3: {0:0.00}", "G: {0:0.00}", "Im: {0:0.0}", "V: {0}" };
            this.freqScopePanel.HorizontalLinesCount = 0;

            // Restore from ZeoSettings
            this.eegLevelToolStripComboBox.SelectedIndex = ZeoSettings.Default.EegLevelSelecteIndex;
            this.WindowState = ZeoSettings.Default.WindowMaximized == true ? FormWindowState.Maximized : FormWindowState.Normal;
            this.Height = ZeoSettings.Default.WindowHeight;
            this.Width = ZeoSettings.Default.WindowWidth;
            this.freqScopePanel.Height = ZeoSettings.Default.FreqPanelHeight;

            if (ZeoSettings.Default.AlarmEnabled == true)
            {
                this.alarmStateToolStripLabel.Text = this.alarmOnString;
                this.alarmStateToolStripLabel.ForeColor = Color.DarkGreen;
            }
            else
            {
                this.alarmStateToolStripLabel.Text = this.alarmOffString;
                this.alarmStateToolStripLabel.ForeColor = SystemColors.ControlText;
            }

            this.LoadFileNames();

            Action<object> comPortsDetect = delegate(object obj)
            {
                Thread.Sleep(50); // sleep to make sure the form is created
                string[] comPorts = Ftdi.GetComPortList();
                this.Invoke(new Action<string[]>(this.ComPortsComboBoxItemsAdd), new object[] { comPorts });
            };

            this.InitBluetoothSpeaker();

            comPortsDetect.BeginInvoke(null, null, null);
        }

        private void InitBluetoothSpeaker()
        {
            SlimDX.RawInput.Device.RegisterDevice(SlimDX.Multimedia.UsagePage.Consumer, SlimDX.Multimedia.UsageId.TelephonyPhone, SlimDX.RawInput.DeviceFlags.InputSink, this.Handle);
            SlimDX.RawInput.Device.RawInput += new EventHandler<SlimDX.RawInput.RawInputEventArgs>(BluetoothSpeaker_RawInput);
        }

        private void ComPortsComboBoxItemsAdd(object obj)
        {
            this.comPortToolStripComboBox.Items.Clear();
            
            string[] comPorts = (string[])obj;
            this.comPortToolStripComboBox.Items.AddRange(comPorts);

            if (comPorts.Length == 0)
            {
                // if no FTDI ports exist, add all existing COM ports
                List<string> ports = new List<string>(SerialPort.GetPortNames());
                ports.Sort();
                this.comPortToolStripComboBox.Items.AddRange(ports.ToArray());
            }

            if (this.comPortToolStripComboBox.Items.Count > ZeoSettings.Default.ComPortSelectedIndex)
            {
                this.comPortToolStripComboBox.SelectedIndex = ZeoSettings.Default.ComPortSelectedIndex;
            }
        }

        private void LoadFileNames()
        {
            string[] fileNames = ZeoSettings.Default.FileNames.Split(';');
            this.fileNameToolStripComboBox.Items.Clear();
            this.fileNameToolStripComboBox.Items.AddRange(fileNames);
            if (this.fileNameToolStripComboBox.Items.Count > 0)
            {
                this.fileNameToolStripComboBox.SelectedIndex = 0;
            }
        }

        private void SaveFileNames()
        {
            string currentFileName = this.fileNameToolStripComboBox.Text;
            List<string> fileNames = new List<string>();
            fileNames.AddRange(ZeoSettings.Default.FileNames.Split(';'));
            fileNames.Remove(currentFileName);
            fileNames.Insert(0, currentFileName);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string fileName in fileNames)
            {
                stringBuilder.Append(fileName + ";");
            }

            ZeoSettings.Default.FileNames = stringBuilder.ToString().Trim(';');
            ZeoSettings.Default.Save();
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

                // TODO: Fix this workaround
                frm.eegScopePanel.Height++;
                frm.eegScopePanel.Height--;

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

            if (this.zeoStream.SoundAlarmEnabled == true)
            {
                // Add one more channel for Alarm sound volume
                this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 2;
                this.freqScopePanel.MaxValueDisplay[this.freqScopePanel.NumberOfChannels - 1] = SoundAlarm.MaxVolume + 2000;
                this.freqScopePanel.MinValueDisplay[this.freqScopePanel.NumberOfChannels - 1] = SoundAlarm.MinVolume - 200;

                this.stageScopePanel.MaxValueDisplay[this.stageScopePanel.NumberOfChannels - 1] = SoundAlarm.MaxVolume + 2000;
                this.stageScopePanel.MinValueDisplay[this.stageScopePanel.NumberOfChannels - 1] = SoundAlarm.MinVolume - 200;
            }
            else
            {
                this.freqScopePanel.NumberOfChannels = ZeoMessage.FrequencyBinsLength + 1;
            }

            this.eegScopePanel.ScrollBarMaximum = this.zeoStream.Length;
            this.freqScopePanel.ScrollBarMaximum = this.zeoStream.Length;

            this.SetFormText(Path.GetFileName(this.fileName));
        }

        private void SetFormText(string fileName)
        {
            this.fileName = fileName;

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
