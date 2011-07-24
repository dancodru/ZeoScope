namespace ZeoScope
{
    internal partial class SettingsForm
    {
        private System.Windows.Forms.GroupBox lucidAlarmGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog mp3FileDialog;
        private System.Windows.Forms.Label mp3FileNameLabel;
        private System.Windows.Forms.TextBox toTimeTextBox;
        private System.Windows.Forms.TextBox fromTimeTextBox;
        private System.Windows.Forms.CheckBox enableAlarmCheckBox;
        private System.Windows.Forms.ComboBox alarmCueComboBox;
        private System.Windows.Forms.TextBox durationTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox fadeOutTextBox;
        private System.Windows.Forms.TextBox fadeInTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button alarmPreviewButton;

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
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                    this.components = null;
                }

                if (this.alarmThread != null)
                {
                    this.alarmThread.Dispose();
                    this.alarmThread = null;
                }

                if (this.soundAlarm != null)
                {
                    this.soundAlarm.Dispose();
                    this.soundAlarm = null;
                }
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
            this.lucidAlarmGroupBox = new System.Windows.Forms.GroupBox();
            this.restoreButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.alarmCueComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.mp3FileNameLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.snoozeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fromTimeTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.toTimeTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maxVolumeComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.alarmPreviewButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fadeInTextBox = new System.Windows.Forms.TextBox();
            this.fadeOutTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.durationTextBox = new System.Windows.Forms.TextBox();
            this.mp3FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.enableAlarmCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lucidAlarmGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lucidAlarmGroupBox
            // 
            this.lucidAlarmGroupBox.Controls.Add(this.restoreButton);
            this.lucidAlarmGroupBox.Controls.Add(this.label2);
            this.lucidAlarmGroupBox.Controls.Add(this.alarmCueComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.groupBox3);
            this.lucidAlarmGroupBox.Controls.Add(this.groupBox2);
            this.lucidAlarmGroupBox.Controls.Add(this.groupBox1);
            this.lucidAlarmGroupBox.Location = new System.Drawing.Point(12, 37);
            this.lucidAlarmGroupBox.Name = "lucidAlarmGroupBox";
            this.lucidAlarmGroupBox.Size = new System.Drawing.Size(368, 411);
            this.lucidAlarmGroupBox.TabIndex = 1;
            this.lucidAlarmGroupBox.TabStop = false;
            this.lucidAlarmGroupBox.Text = "Lucid Alarm";
            // 
            // restoreButton
            // 
            this.restoreButton.Location = new System.Drawing.Point(12, 377);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(154, 23);
            this.restoreButton.TabIndex = 4;
            this.restoreButton.Text = "Restore Default Settings";
            this.restoreButton.UseVisualStyleBackColor = true;
            this.restoreButton.Click += new System.EventHandler(this.RestoreButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Alarm Cue:";
            // 
            // alarmCueComboBox
            // 
            this.alarmCueComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.alarmCueComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.alarmCueComboBox.Location = new System.Drawing.Point(76, 350);
            this.alarmCueComboBox.MaxLength = 50;
            this.alarmCueComboBox.Name = "alarmCueComboBox";
            this.alarmCueComboBox.Size = new System.Drawing.Size(211, 21);
            this.alarmCueComboBox.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.browseButton);
            this.groupBox3.Controls.Add(this.mp3FileNameLabel);
            this.groupBox3.Location = new System.Drawing.Point(6, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(352, 53);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MP3 File";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(6, 19);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(69, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // mp3FileNameLabel
            // 
            this.mp3FileNameLabel.AutoSize = true;
            this.mp3FileNameLabel.ForeColor = System.Drawing.Color.Navy;
            this.mp3FileNameLabel.Location = new System.Drawing.Point(81, 24);
            this.mp3FileNameLabel.Name = "mp3FileNameLabel";
            this.mp3FileNameLabel.Size = new System.Drawing.Size(71, 13);
            this.mp3FileNameLabel.TabIndex = 1;
            this.mp3FileNameLabel.Text = "mp3FileName";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.snoozeTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.fromTimeTextBox);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.toTimeTextBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 208);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 136);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Alarm Enabled Time Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Snooze:";
            // 
            // snoozeTextBox
            // 
            this.snoozeTextBox.Location = new System.Drawing.Point(100, 78);
            this.snoozeTextBox.Name = "snoozeTextBox";
            this.snoozeTextBox.Size = new System.Drawing.Size(35, 20);
            this.snoozeTextBox.TabIndex = 8;
            this.snoozeTextBox.Text = "00:30";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "From:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "To:";
            // 
            // fromTimeTextBox
            // 
            this.fromTimeTextBox.Location = new System.Drawing.Point(100, 26);
            this.fromTimeTextBox.Name = "fromTimeTextBox";
            this.fromTimeTextBox.Size = new System.Drawing.Size(35, 20);
            this.fromTimeTextBox.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(6, 110);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(318, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Optional fileds, empty means no restriction; 24-hour format (hh:mm)";
            // 
            // toTimeTextBox
            // 
            this.toTimeTextBox.Location = new System.Drawing.Point(100, 52);
            this.toTimeTextBox.Name = "toTimeTextBox";
            this.toTimeTextBox.Size = new System.Drawing.Size(35, 20);
            this.toTimeTextBox.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.maxVolumeComboBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.alarmPreviewButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.fadeInTextBox);
            this.groupBox1.Controls.Add(this.fadeOutTextBox);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.durationTextBox);
            this.groupBox1.Location = new System.Drawing.Point(6, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 124);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sound duration and fade";
            // 
            // maxVolumeComboBox
            // 
            this.maxVolumeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maxVolumeComboBox.FormattingEnabled = true;
            this.maxVolumeComboBox.Items.AddRange(new object[] {
            "5",
            "4",
            "3",
            "2",
            "1"});
            this.maxVolumeComboBox.Location = new System.Drawing.Point(254, 17);
            this.maxVolumeComboBox.Name = "maxVolumeComboBox";
            this.maxVolumeComboBox.Size = new System.Drawing.Size(35, 21);
            this.maxVolumeComboBox.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(180, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Max Volume:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Fade-In:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Fade-Out:";
            // 
            // alarmPreviewButton
            // 
            this.alarmPreviewButton.Location = new System.Drawing.Point(254, 90);
            this.alarmPreviewButton.Name = "alarmPreviewButton";
            this.alarmPreviewButton.Size = new System.Drawing.Size(92, 23);
            this.alarmPreviewButton.TabIndex = 4;
            this.alarmPreviewButton.Text = "Alarm Preview";
            this.alarmPreviewButton.UseVisualStyleBackColor = true;
            this.alarmPreviewButton.Click += new System.EventHandler(this.AlarmPreviewButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Required fileds, minutes (mm:ss)";
            // 
            // fadeInTextBox
            // 
            this.fadeInTextBox.Location = new System.Drawing.Point(100, 44);
            this.fadeInTextBox.Name = "fadeInTextBox";
            this.fadeInTextBox.Size = new System.Drawing.Size(35, 20);
            this.fadeInTextBox.TabIndex = 1;
            // 
            // fadeOutTextBox
            // 
            this.fadeOutTextBox.Location = new System.Drawing.Point(100, 70);
            this.fadeOutTextBox.Name = "fadeOutTextBox";
            this.fadeOutTextBox.Size = new System.Drawing.Size(35, 20);
            this.fadeOutTextBox.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Duration:";
            // 
            // durationTextBox
            // 
            this.durationTextBox.Location = new System.Drawing.Point(100, 18);
            this.durationTextBox.Name = "durationTextBox";
            this.durationTextBox.Size = new System.Drawing.Size(35, 20);
            this.durationTextBox.TabIndex = 0;
            // 
            // mp3FileDialog
            // 
            this.mp3FileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            // 
            // enableAlarmCheckBox
            // 
            this.enableAlarmCheckBox.AutoSize = true;
            this.enableAlarmCheckBox.Checked = true;
            this.enableAlarmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableAlarmCheckBox.Location = new System.Drawing.Point(22, 13);
            this.enableAlarmCheckBox.Name = "enableAlarmCheckBox";
            this.enableAlarmCheckBox.Size = new System.Drawing.Size(149, 17);
            this.enableAlarmCheckBox.TabIndex = 0;
            this.enableAlarmCheckBox.Text = "Enable Sleep Stage Alarm";
            this.enableAlarmCheckBox.UseVisualStyleBackColor = true;
            this.enableAlarmCheckBox.CheckedChanged += new System.EventHandler(this.EnableLucidAlarmCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(224, 454);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(305, 454);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(389, 486);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.enableAlarmCheckBox);
            this.Controls.Add(this.lucidAlarmGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ZeoScope :: Settings";
            this.lucidAlarmGroupBox.ResumeLayout(false);
            this.lucidAlarmGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox snoozeTextBox;
        private System.Windows.Forms.ComboBox maxVolumeComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button restoreButton;
    }
}