namespace ZeoScope
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public partial class SettingsForm : Form
    {
        private string mp3FileName;

        public SettingsForm()
        {
            InitializeComponent();
            this.lucidAlarmGroupBox.Enabled = ZeoSettings.Default.LucidAlarmEnabled;
            this.enableLucidAlarmCheckBox.Checked = ZeoSettings.Default.LucidAlarmEnabled;
            
            this.mp3FileName = ZeoSettings.Default.MP3FileName;
            this.mp3FileNameLabel.Text = Path.GetFileName(this.mp3FileName);

            this.fadeInComboBox.Text = ZeoSettings.Default.FadeIn.ToString();
            this.fadeOutComboBox.Text = ZeoSettings.Default.FadeOut.ToString();
            this.durationComboBox.Text = ZeoSettings.Default.AlarmDuration.ToString();

            this.fromTimeMaskedTextBox.Text = ZeoSettings.Default.LucidAlarmFromTime;
            this.toTimeMaskedTextBox.Text = ZeoSettings.Default.LucidAlarmToTime;
            this.alarmCueComboBox.Text = ZeoSettings.Default.LucidAlarmCue;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            ZeoSettings.Default.LucidAlarmEnabled = this.enableLucidAlarmCheckBox.Checked;
            ZeoSettings.Default.MP3FileName = this.mp3FileName;

            ZeoSettings.Default.FadeIn = Convert.ToInt32(this.fadeInComboBox.Text);
            ZeoSettings.Default.FadeOut = Convert.ToInt32(this.fadeOutComboBox.Text);
            ZeoSettings.Default.AlarmDuration = Convert.ToInt32(this.durationComboBox.Text);

            ZeoSettings.Default.LucidAlarmFromTime = this.fromTimeMaskedTextBox.Text.Trim(' ', ':');
            ZeoSettings.Default.LucidAlarmToTime = this.toTimeMaskedTextBox.Text.Trim(' ', ':');
            ZeoSettings.Default.LucidAlarmCue = this.alarmCueComboBox.Text.Trim();


            ZeoSettings.Default.Save();
            ZeoSettings.Default.Reload();
        }

        private void EnableLucidAlarmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.lucidAlarmGroupBox.Enabled = this.enableLucidAlarmCheckBox.Checked;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            this.mp3FileDialog.InitialDirectory = Path.GetDirectoryName(this.mp3FileName);
            if (this.mp3FileDialog.ShowDialog() == DialogResult.OK)
            {
                this.mp3FileName = this.mp3FileDialog.FileName;
                this.mp3FileNameLabel.Text = Path.GetFileName(this.mp3FileName);
            }
        }
    }
}
