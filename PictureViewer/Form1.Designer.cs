namespace MT3
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ShowButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ObsStart = new System.Windows.Forms.Button();
            this.ObsEndButton = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.ButtonSaveEnd = new System.Windows.Forms.Button();
            this.buttonMakeSettings = new System.Windows.Forms.Button();
            this.buttonMove = new System.Windows.Forms.Button();
            this.numericUpDownStarMin = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStarCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_daz = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_dalt = new System.Windows.Forms.NumericUpDown();
            this.checkBox_WideDR = new System.Windows.Forms.CheckBox();
            this.checkBoxObsAuto = new System.Windows.Forms.CheckBox();
            this.checkBoxDispAvg = new System.Windows.Forms.CheckBox();
            this.label_frame_rate = new System.Windows.Forms.Label();
            this.label_ID = new System.Windows.Forms.Label();
            this.label_X2Y2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.timerSaveTimeOver = new System.Windows.Forms.Timer(this.components);
            this.timerSavePost = new System.Windows.Forms.Timer(this.components);
            this.timerDisplay = new System.Windows.Forms.Timer(this.components);
            this.timerObsOnOff = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFramerate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelID = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelPixelClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelExposure = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelGain = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerMTmonSend = new System.Windows.Forms.Timer(this.components);
            this.timer1min = new System.Windows.Forms.Timer(this.components);
            this.timerWaitShutdown = new System.Windows.Forms.Timer(this.components);
            this.timerSave = new System.Windows.Forms.Timer(this.components);
            this.updateDeviceListTimer = new System.Windows.Forms.Timer(this.components);
            this.timerAutoStarData = new System.Windows.Forms.Timer(this.components);
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStarMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStarCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_daz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_dalt)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.08108F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.918927F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(646, 619);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 2);
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ShowButton);
            this.flowLayoutPanel1.Controls.Add(this.CloseButton);
            this.flowLayoutPanel1.Controls.Add(this.ObsStart);
            this.flowLayoutPanel1.Controls.Add(this.ObsEndButton);
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.ButtonSaveEnd);
            this.flowLayoutPanel1.Controls.Add(this.buttonMakeSettings);
            this.flowLayoutPanel1.Controls.Add(this.buttonMove);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownStarMin);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownStarCount);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_daz);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDown_dalt);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_WideDR);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxObsAuto);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxDispAvg);
            this.flowLayoutPanel1.Controls.Add(this.label_frame_rate);
            this.flowLayoutPanel1.Controls.Add(this.label_ID);
            this.flowLayoutPanel1.Controls.Add(this.label_X2Y2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 489);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(640, 47);
            this.flowLayoutPanel1.TabIndex = 2;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // ShowButton
            // 
            this.ShowButton.AutoSize = true;
            this.ShowButton.Location = new System.Drawing.Point(3, 3);
            this.ShowButton.Name = "ShowButton";
            this.ShowButton.Size = new System.Drawing.Size(42, 23);
            this.ShowButton.TabIndex = 0;
            this.ShowButton.Text = "Show";
            this.ShowButton.UseVisualStyleBackColor = true;
            this.ShowButton.Click += new System.EventHandler(this.ShowButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.AutoSize = true;
            this.CloseButton.Location = new System.Drawing.Point(51, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(44, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ObsStart
            // 
            this.ObsStart.Location = new System.Drawing.Point(101, 3);
            this.ObsStart.Name = "ObsStart";
            this.ObsStart.Size = new System.Drawing.Size(58, 23);
            this.ObsStart.TabIndex = 4;
            this.ObsStart.Text = "ObsStart";
            this.ObsStart.UseVisualStyleBackColor = true;
            this.ObsStart.Click += new System.EventHandler(this.ObsStart_Click);
            // 
            // ObsEndButton
            // 
            this.ObsEndButton.AutoSize = true;
            this.ObsEndButton.Location = new System.Drawing.Point(165, 3);
            this.ObsEndButton.Name = "ObsEndButton";
            this.ObsEndButton.Size = new System.Drawing.Size(58, 23);
            this.ObsEndButton.TabIndex = 1;
            this.ObsEndButton.Text = "Obs End";
            this.ObsEndButton.UseVisualStyleBackColor = true;
            this.ObsEndButton.Click += new System.EventHandler(this.ObsEndButton_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonSave.Location = new System.Drawing.Point(229, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(62, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "SaveStart";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ButtonSaveEnd
            // 
            this.ButtonSaveEnd.AutoSize = true;
            this.ButtonSaveEnd.Font = new System.Drawing.Font("MS UI Gothic", 8F);
            this.ButtonSaveEnd.Location = new System.Drawing.Point(297, 3);
            this.ButtonSaveEnd.Name = "ButtonSaveEnd";
            this.ButtonSaveEnd.Size = new System.Drawing.Size(55, 23);
            this.ButtonSaveEnd.TabIndex = 2;
            this.ButtonSaveEnd.Text = "SaveEnd";
            this.ButtonSaveEnd.UseVisualStyleBackColor = true;
            this.ButtonSaveEnd.Click += new System.EventHandler(this.ButtonSaveEnd_Click);
            // 
            // buttonMakeSettings
            // 
            this.buttonMakeSettings.Location = new System.Drawing.Point(358, 3);
            this.buttonMakeSettings.Name = "buttonMakeSettings";
            this.buttonMakeSettings.Size = new System.Drawing.Size(57, 23);
            this.buttonMakeSettings.TabIndex = 6;
            this.buttonMakeSettings.Text = "MakeSet";
            this.buttonMakeSettings.UseVisualStyleBackColor = true;
            this.buttonMakeSettings.Click += new System.EventHandler(this.buttonMakeDark_Click);
            // 
            // buttonMove
            // 
            this.buttonMove.AutoSize = true;
            this.buttonMove.Location = new System.Drawing.Point(421, 3);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(42, 23);
            this.buttonMove.TabIndex = 14;
            this.buttonMove.Text = "Move";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // numericUpDownStarMin
            // 
            this.numericUpDownStarMin.Location = new System.Drawing.Point(469, 3);
            this.numericUpDownStarMin.Maximum = new decimal(new int[] {
            98,
            0,
            0,
            0});
            this.numericUpDownStarMin.Name = "numericUpDownStarMin";
            this.numericUpDownStarMin.Size = new System.Drawing.Size(33, 19);
            this.numericUpDownStarMin.TabIndex = 15;
            this.numericUpDownStarMin.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // numericUpDownStarCount
            // 
            this.numericUpDownStarCount.Location = new System.Drawing.Point(508, 3);
            this.numericUpDownStarCount.Maximum = new decimal(new int[] {
            98,
            0,
            0,
            0});
            this.numericUpDownStarCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStarCount.Name = "numericUpDownStarCount";
            this.numericUpDownStarCount.Size = new System.Drawing.Size(33, 19);
            this.numericUpDownStarCount.TabIndex = 18;
            this.numericUpDownStarCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown_daz
            // 
            this.numericUpDown_daz.Font = new System.Drawing.Font("MS UI Gothic", 8F);
            this.numericUpDown_daz.Location = new System.Drawing.Point(547, 3);
            this.numericUpDown_daz.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDown_daz.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
            this.numericUpDown_daz.Name = "numericUpDown_daz";
            this.numericUpDown_daz.Size = new System.Drawing.Size(36, 18);
            this.numericUpDown_daz.TabIndex = 16;
            this.numericUpDown_daz.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numericUpDown_dalt
            // 
            this.numericUpDown_dalt.Font = new System.Drawing.Font("MS UI Gothic", 8F);
            this.numericUpDown_dalt.Location = new System.Drawing.Point(589, 3);
            this.numericUpDown_dalt.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDown_dalt.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
            this.numericUpDown_dalt.Name = "numericUpDown_dalt";
            this.numericUpDown_dalt.Size = new System.Drawing.Size(36, 18);
            this.numericUpDown_dalt.TabIndex = 17;
            this.numericUpDown_dalt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_WideDR
            // 
            this.checkBox_WideDR.AutoSize = true;
            this.checkBox_WideDR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox_WideDR.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.checkBox_WideDR.Location = new System.Drawing.Point(3, 32);
            this.checkBox_WideDR.Name = "checkBox_WideDR";
            this.checkBox_WideDR.Size = new System.Drawing.Size(95, 16);
            this.checkBox_WideDR.TabIndex = 1;
            this.checkBox_WideDR.Text = "WideDR Mode";
            this.checkBox_WideDR.UseVisualStyleBackColor = true;
            this.checkBox_WideDR.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxObsAuto
            // 
            this.checkBoxObsAuto.AutoSize = true;
            this.checkBoxObsAuto.Checked = true;
            this.checkBoxObsAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxObsAuto.Location = new System.Drawing.Point(104, 32);
            this.checkBoxObsAuto.Name = "checkBoxObsAuto";
            this.checkBoxObsAuto.Size = new System.Drawing.Size(68, 16);
            this.checkBoxObsAuto.TabIndex = 7;
            this.checkBoxObsAuto.Text = "ObsAuto";
            this.checkBoxObsAuto.UseVisualStyleBackColor = true;
            this.checkBoxObsAuto.CheckedChanged += new System.EventHandler(this.checkBoxObsAuto_CheckedChanged);
            // 
            // checkBoxDispAvg
            // 
            this.checkBoxDispAvg.AutoSize = true;
            this.checkBoxDispAvg.Location = new System.Drawing.Point(178, 32);
            this.checkBoxDispAvg.Name = "checkBoxDispAvg";
            this.checkBoxDispAvg.Size = new System.Drawing.Size(67, 16);
            this.checkBoxDispAvg.TabIndex = 11;
            this.checkBoxDispAvg.Text = "DispAvg";
            this.checkBoxDispAvg.UseVisualStyleBackColor = true;
            // 
            // label_frame_rate
            // 
            this.label_frame_rate.AutoSize = true;
            this.label_frame_rate.Location = new System.Drawing.Point(251, 29);
            this.label_frame_rate.Name = "label_frame_rate";
            this.label_frame_rate.Size = new System.Drawing.Size(35, 12);
            this.label_frame_rate.TabIndex = 9;
            this.label_frame_rate.Text = "label1";
            // 
            // label_ID
            // 
            this.label_ID.AutoSize = true;
            this.label_ID.Location = new System.Drawing.Point(292, 29);
            this.label_ID.Name = "label_ID";
            this.label_ID.Size = new System.Drawing.Size(44, 12);
            this.label_ID.TabIndex = 10;
            this.label_ID.Text = "label_ID";
            // 
            // label_X2Y2
            // 
            this.label_X2Y2.AutoSize = true;
            this.label_X2Y2.Location = new System.Drawing.Point(342, 29);
            this.label_X2Y2.Name = "label_X2Y2";
            this.label_X2Y2.Size = new System.Drawing.Size(85, 12);
            this.label_X2Y2.TabIndex = 13;
            this.label_X2Y2.Text = "(  X2,      Y2   )";
            // 
            // textBox1
            // 
            tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 601);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(640, 19);
            this.textBox1.TabIndex = 3;
            // 
            // richTextBox1
            // 
            tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 2);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 542);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(640, 53);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // timerSaveTimeOver
            // 
            this.timerSaveTimeOver.Interval = 10000;
            this.timerSaveTimeOver.Tick += new System.EventHandler(this.timerSaveTimeOver_Tick);
            // 
            // timerSavePost
            // 
            this.timerSavePost.Interval = 5000;
            this.timerSavePost.Tick += new System.EventHandler(this.timerSavePostTime_Tick);
            // 
            // timerDisplay
            // 
            this.timerDisplay.Enabled = true;
            this.timerDisplay.Interval = 200;
            this.timerDisplay.Tick += new System.EventHandler(this.timerDisplay_Tick);
            // 
            // timerObsOnOff
            // 
            this.timerObsOnOff.Enabled = true;
            this.timerObsOnOff.Interval = 10000;
            this.timerObsOnOff.Tick += new System.EventHandler(this.timerObsOnOff_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFramerate,
            this.toolStripStatusLabelFailed,
            this.toolStripStatusLabelID,
            this.toolStripStatusLabelPixelClock,
            this.toolStripStatusLabelExposure,
            this.toolStripStatusLabelGain});
            this.statusStrip1.Location = new System.Drawing.Point(0, 597);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(646, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFramerate
            // 
            this.toolStripStatusLabelFramerate.Name = "toolStripStatusLabelFramerate";
            this.toolStripStatusLabelFramerate.Size = new System.Drawing.Size(24, 17);
            this.toolStripStatusLabelFramerate.Text = "Fps";
            // 
            // toolStripStatusLabelFailed
            // 
            this.toolStripStatusLabelFailed.Name = "toolStripStatusLabelFailed";
            this.toolStripStatusLabelFailed.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelFailed.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabelID
            // 
            this.toolStripStatusLabelID.Name = "toolStripStatusLabelID";
            this.toolStripStatusLabelID.Size = new System.Drawing.Size(49, 17);
            this.toolStripStatusLabelID.Text = "FrameID";
            // 
            // toolStripStatusLabelPixelClock
            // 
            this.toolStripStatusLabelPixelClock.Name = "toolStripStatusLabelPixelClock";
            this.toolStripStatusLabelPixelClock.Size = new System.Drawing.Size(21, 17);
            this.toolStripStatusLabelPixelClock.Text = "PC";
            // 
            // toolStripStatusLabelExposure
            // 
            this.toolStripStatusLabelExposure.Name = "toolStripStatusLabelExposure";
            this.toolStripStatusLabelExposure.Size = new System.Drawing.Size(82, 17);
            this.toolStripStatusLabelExposure.Text = "Exp: xx.xx[ms]";
            // 
            // toolStripStatusLabelGain
            // 
            this.toolStripStatusLabelGain.Name = "toolStripStatusLabelGain";
            this.toolStripStatusLabelGain.Size = new System.Drawing.Size(46, 17);
            this.toolStripStatusLabelGain.Text = "Gain:xx";
            // 
            // timerMTmonSend
            // 
            this.timerMTmonSend.Interval = 2000;
            this.timerMTmonSend.Tick += new System.EventHandler(this.timerMTmonSend_Tick);
            // 
            // timer1min
            // 
            this.timer1min.Enabled = true;
            this.timer1min.Interval = 60000;
            this.timer1min.Tick += new System.EventHandler(this.timer1min_Tick);
            // 
            // timerWaitShutdown
            // 
            this.timerWaitShutdown.Interval = 20000;
            this.timerWaitShutdown.Tick += new System.EventHandler(this.timerWaitShutdown_Tick);
            // 
            // timerSave
            // 
            this.timerSave.Interval = 30000;
            this.timerSave.Tick += new System.EventHandler(this.timerSave_Tick);
            // 
            // updateDeviceListTimer
            // 
            this.updateDeviceListTimer.Interval = 5000;
            this.updateDeviceListTimer.Tick += new System.EventHandler(this.updateDeviceListTimer_Tick);
            // 
            // timerAutoStarData
            // 
            this.timerAutoStarData.Enabled = true;
            this.timerAutoStarData.Interval = 1200000;
            this.timerAutoStarData.Tick += new System.EventHandler(this.timerAutoStarData_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 619);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.RightToLeftLayout = true;
            this.Text = "MT3AVT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStarMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStarCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_daz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_dalt)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Timer timerSaveTimeOver;
        private System.Windows.Forms.Timer timerSavePost;
        private System.Windows.Forms.Timer timerDisplay;
        private System.Windows.Forms.Timer timerObsOnOff;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFramerate;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFailed;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelPixelClock;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelExposure;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelID;
        private System.Windows.Forms.Timer timerMTmonSend;
        private System.Windows.Forms.Timer timer1min;
        private System.Windows.Forms.Timer timerWaitShutdown;
        private System.Windows.Forms.Timer timerSave;
        private System.Windows.Forms.Timer updateDeviceListTimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelGain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button ShowButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button ObsStart;
        private System.Windows.Forms.Button ObsEndButton;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button ButtonSaveEnd;
        private System.Windows.Forms.Button buttonMakeSettings;
        private System.Windows.Forms.CheckBox checkBox_WideDR;
        private System.Windows.Forms.CheckBox checkBoxObsAuto;
        private System.Windows.Forms.CheckBox checkBoxDispAvg;
        private System.Windows.Forms.Label label_frame_rate;
        private System.Windows.Forms.Label label_ID;
        private System.Windows.Forms.Label label_X2Y2;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.NumericUpDown numericUpDownStarMin;
        private System.Windows.Forms.NumericUpDown numericUpDown_daz;
        private System.Windows.Forms.NumericUpDown numericUpDown_dalt;
        private System.Windows.Forms.NumericUpDown numericUpDownStarCount;
        private System.Windows.Forms.Timer timerAutoStarData;
        private TIS.Imaging.ICImagingControl icImagingControl1;
    }
}

