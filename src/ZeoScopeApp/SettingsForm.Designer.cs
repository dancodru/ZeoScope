namespace ZeoScope
{
    partial class SettingsForm
    {
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lucidAlarmGroupBox = new System.Windows.Forms.GroupBox();
            this.alarmCueComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.toTimeMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.fromTimeMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
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
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.fadeInComboBox = new System.Windows.Forms.ComboBox();
            this.fadeOutComboBox = new System.Windows.Forms.ComboBox();
            this.durationComboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lucidAlarmGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // lucidAlarmGroupBox
            // 
            this.lucidAlarmGroupBox.Controls.Add(this.label14);
            this.lucidAlarmGroupBox.Controls.Add(this.label13);
            this.lucidAlarmGroupBox.Controls.Add(this.label12);
            this.lucidAlarmGroupBox.Controls.Add(this.label11);
            this.lucidAlarmGroupBox.Controls.Add(this.durationComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label10);
            this.lucidAlarmGroupBox.Controls.Add(this.fadeOutComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.fadeInComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label9);
            this.lucidAlarmGroupBox.Controls.Add(this.label8);
            this.lucidAlarmGroupBox.Controls.Add(this.alarmCueComboBox);
            this.lucidAlarmGroupBox.Controls.Add(this.label6);
            this.lucidAlarmGroupBox.Controls.Add(this.toTimeMaskedTextBox);
            this.lucidAlarmGroupBox.Controls.Add(this.fromTimeMaskedTextBox);
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
            // alarmCueComboBox
            // 
            this.alarmCueComboBox.FormattingEnabled = true;
            this.alarmCueComboBox.Items.AddRange(new object[] {
            "40R",
            "30R",
            "20R",
            "20R;1L OR 20L1R"});
            this.alarmCueComboBox.Location = new System.Drawing.Point(95, 225);
            this.alarmCueComboBox.Name = "alarmCueComboBox";
            this.alarmCueComboBox.Size = new System.Drawing.Size(168, 21);
            this.alarmCueComboBox.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(137, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "(Optional; 24-hour format)";
            // 
            // toTimeMaskedTextBox
            // 
            this.toTimeMaskedTextBox.Location = new System.Drawing.Point(95, 184);
            this.toTimeMaskedTextBox.Mask = "00:00";
            this.toTimeMaskedTextBox.Name = "toTimeMaskedTextBox";
            this.toTimeMaskedTextBox.Size = new System.Drawing.Size(35, 20);
            this.toTimeMaskedTextBox.TabIndex = 7;
            this.toTimeMaskedTextBox.ValidatingType = typeof(System.DateTime);
            // 
            // fromTimeMaskedTextBox
            // 
            this.fromTimeMaskedTextBox.Location = new System.Drawing.Point(95, 158);
            this.fromTimeMaskedTextBox.Mask = "00:00";
            this.fromTimeMaskedTextBox.Name = "fromTimeMaskedTextBox";
            this.fromTimeMaskedTextBox.Size = new System.Drawing.Size(35, 20);
            this.fromTimeMaskedTextBox.TabIndex = 6;
            this.fromTimeMaskedTextBox.ValidatingType = typeof(System.DateTime);
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
            this.label4.Size = new System.Drawing.Size(310, 104);
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
            this.browseButton.Location = new System.Drawing.Point(95, 16);
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
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MP3 File Name:";
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
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Fade-In:";
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
            // fadeInComboBox
            // 
            this.fadeInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fadeInComboBox.FormattingEnabled = true;
            this.fadeInComboBox.Items.AddRange(new object[] {
            "15",
            "30",
            "60",
            "90",
            "120"});
            this.fadeInComboBox.Location = new System.Drawing.Point(95, 65);
            this.fadeInComboBox.Name = "fadeInComboBox";
            this.fadeInComboBox.Size = new System.Drawing.Size(50, 21);
            this.fadeInComboBox.TabIndex = 3;
            // 
            // fadeOutComboBox
            // 
            this.fadeOutComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fadeOutComboBox.FormattingEnabled = true;
            this.fadeOutComboBox.Items.AddRange(new object[] {
            "15",
            "30",
            "60",
            "90",
            "120"});
            this.fadeOutComboBox.Location = new System.Drawing.Point(95, 92);
            this.fadeOutComboBox.Name = "fadeOutComboBox";
            this.fadeOutComboBox.Size = new System.Drawing.Size(50, 21);
            this.fadeOutComboBox.TabIndex = 4;
            // 
            // durationComboBox
            // 
            this.durationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.durationComboBox.FormattingEnabled = true;
            this.durationComboBox.Items.AddRange(new object[] {
            "15",
            "30",
            "60",
            "90",
            "120",
            "300",
            "600",
            "900",
            "1200"});
            this.durationComboBox.Location = new System.Drawing.Point(95, 119);
            this.durationComboBox.Name = "durationComboBox";
            this.durationComboBox.Size = new System.Drawing.Size(50, 21);
            this.durationComboBox.TabIndex = 5;
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(151, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "seconds";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(151, 95);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "seconds";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(151, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "seconds";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(137, 187);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(126, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "(Optional; 24-hour format)";
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.MaskedTextBox toTimeMaskedTextBox;
        private System.Windows.Forms.MaskedTextBox fromTimeMaskedTextBox;
        private System.Windows.Forms.CheckBox enableLucidAlarmCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox alarmCueComboBox;
        private System.Windows.Forms.ComboBox durationComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox fadeOutComboBox;
        private System.Windows.Forms.ComboBox fadeInComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
    }
}