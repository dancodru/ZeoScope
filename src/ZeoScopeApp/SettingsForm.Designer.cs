namespace ZeoScope
{
    internal partial class SettingsForm
    {
        private System.Windows.Forms.GroupBox lucidAlarmGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog mp3FileDialog;
        private System.Windows.Forms.Label mp3FileNameLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox toTimeTextBox;
        private System.Windows.Forms.TextBox fromTimeTextBox;
        private System.Windows.Forms.CheckBox enableLucidAlarmCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox alarmCueComboBox;
        private System.Windows.Forms.TextBox durationTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox fadeOutTextBox;
        private System.Windows.Forms.TextBox fadeInTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ErrorProvider errorProvider;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lucidAlarmGroupBox = new System.Windows.Forms.GroupBox();
            this.alarmPreviewButton = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.durationTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.fadeOutTextBox = new System.Windows.Forms.TextBox();
            this.fadeInTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.alarmCueComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.toTimeTextBox = new System.Windows.Forms.TextBox();
            this.fromTimeTextBox = new System.Windows.Forms.TextBox();
            this.mp3FileNameLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mp3FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.enableLucidAlarmCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.lucidAlarmGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lucidAlarmGroupBox
            // 
            this.lucidAlarmGroupBox.Controls.Add(this.alarmPreviewButton);
            this.lucidAlarmGroupBox.Controls.Add(this.label14);
            this.lucidAlarmGroupBox.Controls.Add(this.label13);
            this.lucidAlarmGroupBox.Controls.Add(this.label12);
            this.lucidAlarmGroupBox.Controls.Add(this.label11);
            this.lucidAlarmGroupBox.Controls.Add(this.durationTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label10);
            this.lucidAlarmGroupBox.Controls.Add(this.fadeOutTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.fadeInTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label9);
            this.lucidAlarmGroupBox.Controls.Add(this.label8);
            this.lucidAlarmGroupBox.Controls.Add(this.alarmCueComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label6);
            this.lucidAlarmGroupBox.Controls.Add(this.toTimeTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.fromTimeTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.mp3FileNameLabel);
            this.lucidAlarmGroupBox.Controls.Add(this.label5);
            this.lucidAlarmGroupBox.Controls.Add(this.label4);
            this.lucidAlarmGroupBox.Controls.Add(this.label3);
            this.lucidAlarmGroupBox.Controls.Add(this.label2);
            this.lucidAlarmGroupBox.Controls.Add(this.browseButton);
            this.lucidAlarmGroupBox.Controls.Add(this.label1);
            this.lucidAlarmGroupBox.Location = new System.Drawing.Point(12, 37);
            this.lucidAlarmGroupBox.Name = "lucidAlarmGroupBox";
            this.lucidAlarmGroupBox.Size = new System.Drawing.Size(375, 375);
            this.lucidAlarmGroupBox.TabIndex = 1;
            this.lucidAlarmGroupBox.TabStop = false;
            this.lucidAlarmGroupBox.Text = "Lucid Alarm";
            // 
            // alarmPreviewButton
            // 
            this.alarmPreviewButton.Location = new System.Drawing.Point(231, 117);
            this.alarmPreviewButton.Name = "alarmPreviewButton";
            this.alarmPreviewButton.Size = new System.Drawing.Size(92, 23);
            this.alarmPreviewButton.TabIndex = 26;
            this.alarmPreviewButton.Text = "Alarm Preview";
            this.alarmPreviewButton.UseVisualStyleBackColor = true;
            this.alarmPreviewButton.Click += new System.EventHandler(this.AlarmPreviewButton_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(130, 187);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(157, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "Optional; 24-hour format, hh:mm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(130, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(78, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "minutes, mm:ss";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(130, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "minutes, mm:ss";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(130, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "minutes, mm:ss";
            // 
            // durationTextBox
            // 
            this.durationTextBox.Location = new System.Drawing.Point(71, 119);
            this.durationTextBox.Name = "durationTextBox";
            this.durationTextBox.Size = new System.Drawing.Size(35, 20);
            this.durationTextBox.TabIndex = 5;
            this.durationTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.durationTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.durationTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 122);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Duration:";
            // 
            // fadeOutTextBox
            // 
            this.fadeOutTextBox.Location = new System.Drawing.Point(71, 94);
            this.fadeOutTextBox.Name = "fadeOutTextBox";
            this.fadeOutTextBox.Size = new System.Drawing.Size(35, 20);
            this.fadeOutTextBox.TabIndex = 4;
            this.fadeOutTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.fadeOutTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.fadeOutTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // fadeInTextBox
            // 
            this.fadeInTextBox.Location = new System.Drawing.Point(71, 65);
            this.fadeInTextBox.Name = "fadeInTextBox";
            this.fadeInTextBox.Size = new System.Drawing.Size(35, 20);
            this.fadeInTextBox.TabIndex = 3;
            this.fadeInTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.fadeInTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.fadeInTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Fade-Out:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Fade-In:";
            // 
            // alarmCueComboBox
            // 
            this.alarmCueComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.alarmCueComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.alarmCueComboBox.FormattingEnabled = true;
            this.alarmCueComboBox.Location = new System.Drawing.Point(71, 225);
            this.alarmCueComboBox.MaxLength = 50;
            this.alarmCueComboBox.Name = "alarmCueComboBox";
            this.alarmCueComboBox.Size = new System.Drawing.Size(228, 21);
            this.alarmCueComboBox.TabIndex = 8;
            this.alarmCueComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.alarmCueComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.alarmCueComboBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(130, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Optional; 24-hour format, hh:mm";
            // 
            // toTimeTextBox
            // 
            this.toTimeTextBox.Location = new System.Drawing.Point(71, 184);
            this.toTimeTextBox.Name = "toTimeTextBox";
            this.toTimeTextBox.Size = new System.Drawing.Size(35, 20);
            this.toTimeTextBox.TabIndex = 7;
            this.toTimeTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.toTimeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.toTimeTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // fromTimeTextBox
            // 
            this.fromTimeTextBox.Location = new System.Drawing.Point(71, 158);
            this.fromTimeTextBox.Name = "fromTimeTextBox";
            this.fromTimeTextBox.Size = new System.Drawing.Size(35, 20);
            this.fromTimeTextBox.TabIndex = 6;
            this.fromTimeTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.fromTimeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            this.fromTimeTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
            // 
            // mp3FileNameLabel
            // 
            this.mp3FileNameLabel.AutoSize = true;
            this.mp3FileNameLabel.ForeColor = System.Drawing.Color.Navy;
            this.mp3FileNameLabel.Location = new System.Drawing.Point(20, 42);
            this.mp3FileNameLabel.Name = "mp3FileNameLabel";
            this.mp3FileNameLabel.Size = new System.Drawing.Size(71, 13);
            this.mp3FileNameLabel.TabIndex = 11;
            this.mp3FileNameLabel.Text = "mp3FileName";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "To:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(7, 265);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(316, 104);
            this.label4.TabIndex = 7;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "From:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 228);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Alarm Cue:";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(71, 16);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(69, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MP3 File:";
            // 
            // mp3FileDialog
            // 
            this.mp3FileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            // 
            // enableLucidAlarmCheckBox
            // 
            this.enableLucidAlarmCheckBox.AutoSize = true;
            this.enableLucidAlarmCheckBox.Checked = true;
            this.enableLucidAlarmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableLucidAlarmCheckBox.Location = new System.Drawing.Point(22, 13);
            this.enableLucidAlarmCheckBox.Name = "enableLucidAlarmCheckBox";
            this.enableLucidAlarmCheckBox.Size = new System.Drawing.Size(117, 17);
            this.enableLucidAlarmCheckBox.TabIndex = 1;
            this.enableLucidAlarmCheckBox.Text = "Enable Lucid Alarm";
            this.enableLucidAlarmCheckBox.UseVisualStyleBackColor = true;
            this.enableLucidAlarmCheckBox.CheckedChanged += new System.EventHandler(this.EnableLucidAlarmCheckBox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(173, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(197, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Warning: Do Not use as an actual Alarm";
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 424);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.enableLucidAlarmCheckBox);
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}