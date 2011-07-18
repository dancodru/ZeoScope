namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    internal partial class SettingsForm : Form
    {
        private static string regexTime = @"\d{1,2}:\d{2}";

        private string mp3FileName;

        private bool alarmPreviewStarted = false;

        private System.Threading.Timer alarmThread;
        private SoundAlarm soundAlarm;

        private ErrorProvider[] errorProviders;

        private ToolTip[] toolTips;

        public SettingsForm()
        {
            this.InitializeComponent();
            this.lucidAlarmGroupBox.Enabled = ZeoSettings.Default.AlarmEnabled;
            this.enableAlarmCheckBox.Checked = ZeoSettings.Default.AlarmEnabled;

            this.mp3FileName = ZeoSettings.Default.MP3FileName;
            this.mp3FileNameLabel.Text = Path.GetFileName(this.mp3FileName);

            this.fadeInTextBox.Text = ZeoSettings.Default.AlarmFadeIn;
            this.fadeOutTextBox.Text = ZeoSettings.Default.AlarmFadeOut;
            this.durationTextBox.Text = ZeoSettings.Default.AlarmDuration;
            this.maxVolumeComboBox.Text = ZeoSettings.Default.MaxVolume.ToString();

            this.fromTimeTextBox.Text = ZeoSettings.Default.AlarmFromTime;
            this.toTimeTextBox.Text = ZeoSettings.Default.AlarmToTime;
            this.snoozeTextBox.Text = ZeoSettings.Default.AlarmSnooze;

            this.SetToolTips();
            this.LoadAlarmCues();

            this.errorProviders = new ErrorProvider[8];
            for (int i = 0; i < this.errorProviders.Length; i++)
            {
                this.errorProviders[i] = new ErrorProvider();
                this.errorProviders[i].BlinkStyle = ErrorBlinkStyle.NeverBlink;
            }
        }

        private void SetToolTips()
        {
            this.toolTips = new ToolTip[9];
            for (int i = 0; i < this.toolTips.Length; i++)
            {
                this.toolTips[i] = new ToolTip();
                this.toolTips[i].AutoPopDelay = 30000;
                this.toolTips[i].InitialDelay = 200;
                this.toolTips[i].ReshowDelay = 100;
                //this.toolTips[i].ShowAlways = true;
                this.toolTips[i].ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            }

            this.toolTips[0].ToolTipTitle = "Alarm Duration";
            this.toolTips[0].SetToolTip(this.durationTextBox, "MP3 file will be played in a loop until the duration is reached");
            this.toolTips[1].ToolTipTitle = "Fade-In";
            this.toolTips[1].SetToolTip(this.fadeInTextBox, "Sound fade-in time");
            this.toolTips[2].ToolTipTitle = "Fade-Out";
            this.toolTips[2].SetToolTip(this.fadeOutTextBox, "Sound fade-out time");
            this.toolTips[3].ToolTipTitle = "Alarm Max Volume";
            this.toolTips[3].SetToolTip(this.maxVolumeComboBox, "Maximum volume that will be reached by alarm sound");
            this.toolTips[4].ToolTipTitle = "Alarm Preview";
            this.toolTips[4].SetToolTip(this.alarmPreviewButton, "Preview alarm playback with fade-in and fade-out efects");

            this.toolTips[5].ToolTipTitle = "Alarm disabled untill";
            this.toolTips[5].SetToolTip(this.fromTimeTextBox, "Time when alarm is activated, (empty) - active from start");
            this.toolTips[6].ToolTipTitle = "Alarm active untill";
            this.toolTips[6].SetToolTip(this.toTimeTextBox, "Time when alarm is deactivated, (empty) - never deactivated");
            this.toolTips[7].ToolTipTitle = "Snooze Time";
            this.toolTips[7].SetToolTip(this.snoozeTextBox, "Minimum time interval between alarms");

            this.toolTips[8].ToolTipTitle = "Alarm Cue Format";
            this.toolTips[8].SetToolTip(this.alarmCueComboBox,
                "State1[;State2] [ OR State3[;State4]]\r\n" +
                "       StateN :: nnn<A,R,L,D,S,U>\r\n" +
                "                       nnn - number of half-minutes, 1-999\r\n" +
                "                       A - Awake, R - REM, L - Light,\r\n" +
                "                       D - Deep, S - any kind of sleep, U - Undefined\r\n" +
                "\r\n" +
                "Example1: 20R;1L OR 30L;1R\r\n" +
                "    Will start the alarm after 1 half-minute (L)ight sleep preceeded by\r\n" +
                "    20 half-minutes of (R)EM sleep\r\n" +
                "    OR\r\n" +
                "    1 half-minute (R)EM sleep preceeded by 30 half-minutes (L)ight sleep\r\n" +
                "Example2: 40S\r\n" +
                "    20 minutes of sleep (REM, Light or Deep); Good for napping\r\n" +
                "Example2: 40R\r\n" +
                "    20 minutes of REM sleep; Good for Lucid Alarm\r\n");
        }

        private void LoadAlarmCues()
        {
            string[] alarmCues = ZeoSettings.Default.AlarmCues.Split('|');
            this.alarmCueComboBox.Items.Clear();
            this.alarmCueComboBox.Items.AddRange(alarmCues);
            this.alarmCueComboBox.Text = ZeoSettings.Default.AlarmCue;
        }

        private void SaveAlarmCues()
        {
            List<string> alarmCues = new List<string>();
            alarmCues.AddRange(ZeoSettings.Default.AlarmCues.Split('|'));
            alarmCues.Remove(this.alarmCueComboBox.Text);
            alarmCues.Insert(0, this.alarmCueComboBox.Text);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string alarmCue in alarmCues)
            {
                stringBuilder.Append(alarmCue + "|");
            }

            ZeoSettings.Default.AlarmCues = stringBuilder.ToString().Trim('|');
            ZeoSettings.Default.AlarmCue = this.alarmCueComboBox.Text;
            ZeoSettings.Default.Save();
        }

        private void EnableLucidAlarmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.lucidAlarmGroupBox.Enabled = this.enableAlarmCheckBox.Checked;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.mp3FileDialog.InitialDirectory = Path.GetDirectoryName(this.mp3FileName);
            }
            catch (ArgumentException)
            {
            }

            if (this.mp3FileDialog.ShowDialog() == DialogResult.OK)
            {
                this.mp3FileName = this.mp3FileDialog.FileName;
                this.mp3FileNameLabel.Text = Path.GetFileName(this.mp3FileName);
                this.ValidateFields();
            }
        }

        private void AlarmPreviewButton_Click(object sender, EventArgs e)
        {
            if (this.alarmPreviewStarted == false)
            {
                if (this.ValidateFields() == true)
                {
                    SoundAlarm.SetVolumes(Convert.ToInt32(this.maxVolumeComboBox.Text));

                    this.alarmPreviewStarted = true;
                    this.alarmPreviewButton.Text = "Stop";
                    this.alarmThread = new System.Threading.Timer(new TimerCallback(this.RunAlarm), null, 0, 1000);
                }
            }
            else
            {
                this.alarmPreviewStarted = false;
                this.alarmPreviewButton.Text = "Alarm Preview";

                this.alarmThread.Dispose();
                this.alarmThread = null;

                this.soundAlarm.AlarmStarted = false;
                this.soundAlarm.ProcessZeoMessage(new ZeoMessage());
                this.soundAlarm.Dispose();
                this.soundAlarm = null;
            }
        }

        private void RunAlarm(object obj)
        {
            if (this.soundAlarm == null)
            {
                this.soundAlarm = new SoundAlarm(this.mp3FileName, this.fadeInTextBox.Text, this.fadeOutTextBox.Text, this.durationTextBox.Text, "", "", "", "1A");
                this.soundAlarm.AlarmStarted = true;
            }

            this.soundAlarm.ProcessZeoMessage(new ZeoMessage());

            if (this.soundAlarm.AlarmStarted == false)
            {
                this.Invoke(new EventHandler(this.AlarmPreviewButton_Click), null, null);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateFields() == true)
            {
                ZeoSettings.Default.AlarmEnabled = this.enableAlarmCheckBox.Checked;
                ZeoSettings.Default.MP3FileName = this.mp3FileName;

                ZeoSettings.Default.AlarmDuration = this.durationTextBox.Text;
                ZeoSettings.Default.AlarmFadeIn = this.fadeInTextBox.Text;
                ZeoSettings.Default.AlarmFadeOut = this.fadeOutTextBox.Text;
                ZeoSettings.Default.MaxVolume = Convert.ToInt32(this.maxVolumeComboBox.Text);

                ZeoSettings.Default.AlarmFromTime = this.fromTimeTextBox.Text;
                ZeoSettings.Default.AlarmToTime = this.toTimeTextBox.Text;
                ZeoSettings.Default.AlarmSnooze = this.snoozeTextBox.Text;

                this.SaveAlarmCues();

                ZeoSettings.Default.Save();
                ZeoSettings.Default.Reload();

                SoundAlarm.SetVolumes(ZeoSettings.Default.MaxVolume);

                this.Close();
            }
        }

        private bool ValidateFields()
        {
            bool validFields = true;

            Control[] requiredFields = new Control[] { this.durationTextBox, this.fadeInTextBox, this.fadeOutTextBox };
            int errorProviderId = 0;
            foreach (Control control in requiredFields)
            {
                if (string.IsNullOrEmpty(control.Text) == true || Regex.Match(control.Text, SettingsForm.regexTime).Value != control.Text)
                {
                    this.errorProviders[errorProviderId].SetError(control, "Invalid or empty time string (mm:ss)");
                    validFields = false;
                }
                else
                {
                    this.errorProviders[errorProviderId].SetError(control, string.Empty);
                }

                errorProviderId++;
            }

            Control[] optionalFields = new Control[] { this.fromTimeTextBox, this.toTimeTextBox, this.snoozeTextBox };
            foreach (Control control in optionalFields)
            {
                if (Regex.Match(control.Text, SettingsForm.regexTime).Value != control.Text)
                {
                    this.errorProviders[errorProviderId].SetError(control, "Invalid time string (hh:mm)");
                    validFields = false;
                }
                else
                {
                    this.errorProviders[errorProviderId].SetError(control, string.Empty);
                }

                errorProviderId++;
            }

            if (string.IsNullOrEmpty(this.alarmCueComboBox.Text) == true || SoundAlarm.ValidateAlarmCue(this.alarmCueComboBox.Text) == false)
            {
                this.errorProviders[errorProviderId].SetError(this.alarmCueComboBox, "Invalid or empty Alarm Cue");
                validFields = false;
            }
            else
            {
                this.errorProviders[errorProviderId].SetError(this.alarmCueComboBox, string.Empty);
            }

            errorProviderId++;

            if (string.IsNullOrEmpty(this.mp3FileName) == true && this.enableAlarmCheckBox.Checked == true)
            {
                this.errorProviders[errorProviderId].SetError(this.mp3FileNameLabel, "Empty MP3 file name");
                validFields = false;
            }
            else
            {
                this.errorProviders[errorProviderId].SetError(this.mp3FileNameLabel, string.Empty);
            }

            errorProviderId++;

            return validFields;
        }
    }
}
