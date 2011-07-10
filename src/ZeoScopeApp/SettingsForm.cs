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

        private bool validationError = false;

        private bool alarmPreviewStarted = false;

        private System.Threading.Timer alarmThread;
        private SoundAlarm soundAlarm;

        public SettingsForm()
        {
            this.InitializeComponent();
            this.lucidAlarmGroupBox.Enabled = ZeoSettings.Default.LucidAlarmEnabled;
            this.enableLucidAlarmCheckBox.Checked = ZeoSettings.Default.LucidAlarmEnabled;

            this.mp3FileName = ZeoSettings.Default.MP3FileName;
            this.mp3FileNameLabel.Text = Path.GetFileName(this.mp3FileName);

            this.fadeInTextBox.Text = ZeoSettings.Default.AlarmFadeIn.ToString();
            this.fadeOutTextBox.Text = ZeoSettings.Default.AlarmFadeOut.ToString();
            this.durationTextBox.Text = ZeoSettings.Default.AlarmDuration.ToString();

            this.fromTimeTextBox.Text = ZeoSettings.Default.AlarmFromTime;
            this.toTimeTextBox.Text = ZeoSettings.Default.AlarmToTime;

            this.LoadAlarmCues();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (this.validationError == false)
            {
                ZeoSettings.Default.LucidAlarmEnabled = this.enableLucidAlarmCheckBox.Checked;
                ZeoSettings.Default.MP3FileName = this.mp3FileName;

                ZeoSettings.Default.AlarmFadeIn = this.fadeInTextBox.Text;
                ZeoSettings.Default.AlarmFadeOut = this.fadeOutTextBox.Text;
                ZeoSettings.Default.AlarmDuration = this.durationTextBox.Text;

                ZeoSettings.Default.AlarmFromTime = this.fromTimeTextBox.Text.Trim(' ', ':');
                ZeoSettings.Default.AlarmToTime = this.toTimeTextBox.Text.Trim(' ', ':');

                this.SaveAlarmCues();

                ZeoSettings.Default.Save();
                ZeoSettings.Default.Reload();
            }
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
            alarmCues.AddRange(ZeoSettings.Default.AlarmCues.Split(';'));
            alarmCues.Remove(this.alarmCueComboBox.Text);
            alarmCues.Insert(0, this.alarmCueComboBox.Text);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string alarmCue in alarmCues)
            {
                stringBuilder.Append(alarmCue + ";");
            }

            ZeoSettings.Default.AlarmCues = stringBuilder.ToString().Trim('|');
            ZeoSettings.Default.AlarmCue = this.alarmCueComboBox.Text;
            ZeoSettings.Default.Save();
        }

        private void EnableLucidAlarmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.lucidAlarmGroupBox.Enabled = this.enableLucidAlarmCheckBox.Checked;
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
            }
        }

        private void TextBox_Validated(object sender, EventArgs e)
        {
            this.errorProvider.SetError((Control)sender, string.Empty);
            this.validationError = false;
        }

        private void TextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Control control = (Control)sender;
            if (control == this.alarmCueComboBox && SoundAlarm.ValidateAlarmCue(control.Text) == false)
            {
                e.Cancel = true;
                this.errorProvider.SetError(control, "Invalid Alarm Cue");
            }
            else if (Regex.Match(control.Text, SettingsForm.regexTime).Value != control.Text)
            {
                e.Cancel = true;
                this.errorProvider.SetError(control, "Invalid Time string (mm:ss or hh:mm)");
                this.validationError = true;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Control control = (Control)sender;
            if (control == this.alarmCueComboBox)
            {
                if (SoundAlarm.ValidateAlarmCue(control.Text) == false)
                {
                    this.errorProvider.SetError(control, "Invalid Alarm Cue");
                    this.validationError = true;
                }
                else
                {
                    this.errorProvider.SetError(control, string.Empty);
                    this.validationError = false;
                }
            }
            else
            {
                if (Regex.Match(control.Text, SettingsForm.regexTime).Value != control.Text)
                {
                    this.errorProvider.SetError(control, "Invalid Time string (mm:ss or hh:mm)");
                    this.validationError = true;
                }
                else
                {
                    this.errorProvider.SetError(control, string.Empty);
                    this.validationError = false;
                }
            }
        }

        private void AlarmPreviewButton_Click(object sender, EventArgs e)
        {
            if (this.alarmPreviewStarted == false)
            {
                this.alarmPreviewStarted = true;
                this.alarmPreviewButton.Text = "Stop";
                this.alarmThread = new System.Threading.Timer(new TimerCallback(this.RunAlarm), null, 0, 1000);
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
                this.soundAlarm = new SoundAlarm(this.mp3FileName, this.fadeInTextBox.Text, this.fadeOutTextBox.Text, this.durationTextBox.Text, "", "", "1A");
                this.soundAlarm.AlarmStarted = true;
            }

            this.soundAlarm.ProcessZeoMessage(new ZeoMessage());

            if (this.soundAlarm.AlarmStarted == false)
            {
                this.Invoke(new EventHandler(this.AlarmPreviewButton_Click), null, null);
            }
        }
    }
}
