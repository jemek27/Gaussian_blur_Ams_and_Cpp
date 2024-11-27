namespace JA_projekt_sem5 {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            inputPicture = new PictureBox();
            loadPictureButton = new Button();
            openFileDialog1 = new OpenFileDialog();
            processedPicture = new PictureBox();
            processPictureButton = new Button();
            kernelSizeTextBox = new TextBox();
            sigmaTextBox = new TextBox();
            blureConfigButton = new Button();
            labelKernelSize = new Label();
            labelSigmaSize = new Label();
            labelStatus = new Label();
            radioButtonAsm = new RadioButton();
            radioButtonCpp = new RadioButton();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            runTestButton = new Button();
            timeAsmLabel = new Label();
            timeCppLabel = new Label();
            label6 = new Label();
            label7 = new Label();
            checkBoxAll = new CheckBox();
            checkBoxSmall = new CheckBox();
            checkBoxMedium = new CheckBox();
            checkBoxBig = new CheckBox();
            trackBar1 = new TrackBar();
            labelXTimes = new Label();
            iterationsTextBox = new TextBox();
            SaveButton = new Button();
            threadsLabel = new Label();
            button2 = new Button();
            label4 = new Label();
            label5 = new Label();
            label8 = new Label();
            textBoxStartThreadsNum = new TextBox();
            textBoxEndThreadsNum = new TextBox();
            checkBoxDoublingEachIter = new CheckBox();
            label9 = new Label();
            label10 = new Label();
            trackBarRadius = new TrackBar();
            label11 = new Label();
            textRadiusVal = new TextBox();
            ((System.ComponentModel.ISupportInitialize)inputPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)processedPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).BeginInit();
            SuspendLayout();
            // 
            // inputPicture
            // 
            inputPicture.BackColor = SystemColors.ButtonShadow;
            inputPicture.BorderStyle = BorderStyle.FixedSingle;
            inputPicture.Location = new Point(269, 44);
            inputPicture.Margin = new Padding(3, 2, 3, 2);
            inputPicture.Name = "inputPicture";
            inputPicture.Size = new Size(300, 200);
            inputPicture.TabIndex = 5;
            inputPicture.TabStop = false;
            // 
            // loadPictureButton
            // 
            loadPictureButton.Font = new Font("Segoe UI", 11F);
            loadPictureButton.Location = new Point(31, 44);
            loadPictureButton.Margin = new Padding(3, 2, 3, 2);
            loadPictureButton.Name = "loadPictureButton";
            loadPictureButton.Size = new Size(120, 30);
            loadPictureButton.TabIndex = 6;
            loadPictureButton.Text = "Load Picture";
            loadPictureButton.UseVisualStyleBackColor = true;
            loadPictureButton.Click += buttonLoadPicture_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // processedPicture
            // 
            processedPicture.BackColor = SystemColors.ButtonShadow;
            processedPicture.BorderStyle = BorderStyle.FixedSingle;
            processedPicture.Location = new Point(606, 44);
            processedPicture.Margin = new Padding(3, 2, 3, 2);
            processedPicture.Name = "processedPicture";
            processedPicture.Size = new Size(1050, 700);
            processedPicture.TabIndex = 7;
            processedPicture.TabStop = false;
            // 
            // processPictureButton
            // 
            processPictureButton.Font = new Font("Segoe UI", 11F);
            processPictureButton.Location = new Point(31, 78);
            processPictureButton.Margin = new Padding(3, 2, 3, 2);
            processPictureButton.Name = "processPictureButton";
            processPictureButton.Size = new Size(120, 30);
            processPictureButton.TabIndex = 8;
            processPictureButton.Text = "Process Picture";
            processPictureButton.UseVisualStyleBackColor = true;
            processPictureButton.Click += processPictureButton_Click;
            // 
            // kernelSizeTextBox
            // 
            kernelSizeTextBox.Location = new Point(269, 323);
            kernelSizeTextBox.Margin = new Padding(3, 2, 3, 2);
            kernelSizeTextBox.Name = "kernelSizeTextBox";
            kernelSizeTextBox.Size = new Size(140, 23);
            kernelSizeTextBox.TabIndex = 9;
            // 
            // sigmaTextBox
            // 
            sigmaTextBox.Location = new Point(437, 323);
            sigmaTextBox.Margin = new Padding(3, 2, 3, 2);
            sigmaTextBox.Name = "sigmaTextBox";
            sigmaTextBox.Size = new Size(140, 23);
            sigmaTextBox.TabIndex = 10;
            // 
            // blureConfigButton
            // 
            blureConfigButton.Font = new Font("Segoe UI", 11F);
            blureConfigButton.Location = new Point(269, 350);
            blureConfigButton.Margin = new Padding(3, 2, 3, 2);
            blureConfigButton.Name = "blureConfigButton";
            blureConfigButton.Size = new Size(100, 30);
            blureConfigButton.TabIndex = 11;
            blureConfigButton.Text = "Apply";
            blureConfigButton.UseVisualStyleBackColor = true;
            blureConfigButton.Click += blurConfigButton_Click;
            // 
            // labelKernelSize
            // 
            labelKernelSize.AutoSize = true;
            labelKernelSize.Font = new Font("Segoe UI", 11F);
            labelKernelSize.Location = new Point(269, 297);
            labelKernelSize.Name = "labelKernelSize";
            labelKernelSize.Size = new Size(107, 20);
            labelKernelSize.TabIndex = 16;
            labelKernelSize.Text = "Set kernel size ";
            // 
            // labelSigmaSize
            // 
            labelSigmaSize.AutoSize = true;
            labelSigmaSize.Font = new Font("Segoe UI", 11F);
            labelSigmaSize.Location = new Point(436, 297);
            labelSigmaSize.Name = "labelSigmaSize";
            labelSigmaSize.Size = new Size(113, 20);
            labelSigmaSize.TabIndex = 13;
            labelSigmaSize.Text = "Set sigma value";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.ForeColor = SystemColors.ControlDark;
            labelStatus.Location = new Point(42, 757);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 15);
            labelStatus.TabIndex = 15;
            // 
            // radioButtonAsm
            // 
            radioButtonAsm.AutoSize = true;
            radioButtonAsm.Checked = true;
            radioButtonAsm.Font = new Font("Segoe UI", 11F);
            radioButtonAsm.Location = new Point(170, 46);
            radioButtonAsm.Name = "radioButtonAsm";
            radioButtonAsm.Size = new Size(56, 24);
            radioButtonAsm.TabIndex = 16;
            radioButtonAsm.TabStop = true;
            radioButtonAsm.Text = "Asm";
            radioButtonAsm.UseVisualStyleBackColor = true;
            radioButtonAsm.CheckedChanged += radioButtonAsm_CheckedChanged;
            // 
            // radioButtonCpp
            // 
            radioButtonCpp.AutoSize = true;
            radioButtonCpp.Font = new Font("Segoe UI", 11F);
            radioButtonCpp.Location = new Point(170, 72);
            radioButtonCpp.Name = "radioButtonCpp";
            radioButtonCpp.Size = new Size(56, 24);
            radioButtonCpp.TabIndex = 17;
            radioButtonCpp.Text = "C++";
            radioButtonCpp.UseVisualStyleBackColor = true;
            radioButtonCpp.CheckedChanged += radioButtonCpp_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F);
            label1.Location = new Point(37, 408);
            label1.Name = "label1";
            label1.Size = new Size(96, 25);
            label1.TabIndex = 18;
            label1.Text = "Speed test";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(37, 433);
            label2.Name = "label2";
            label2.Size = new Size(352, 40);
            label2.TabIndex = 19;
            label2.Text = "Compare execution times of Assembly and C++ Dll \r\nSetted kernel size and sigma value will be used\r\n";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 13F);
            label3.Location = new Point(37, 481);
            label3.Name = "label3";
            label3.Size = new Size(210, 25);
            label3.TabIndex = 24;
            label3.Text = "Select testing picture size";
            // 
            // runTestButton
            // 
            runTestButton.Font = new Font("Segoe UI", 11F);
            runTestButton.Location = new Point(42, 590);
            runTestButton.Margin = new Padding(3, 2, 3, 2);
            runTestButton.Name = "runTestButton";
            runTestButton.Size = new Size(120, 30);
            runTestButton.TabIndex = 25;
            runTestButton.Text = "Run test";
            runTestButton.UseVisualStyleBackColor = true;
            runTestButton.Click += runTestButton_Click;
            // 
            // timeAsmLabel
            // 
            timeAsmLabel.AutoSize = true;
            timeAsmLabel.Font = new Font("Segoe UI", 16F);
            timeAsmLabel.Location = new Point(124, 626);
            timeAsmLabel.Name = "timeAsmLabel";
            timeAsmLabel.Size = new Size(107, 30);
            timeAsmLabel.TabIndex = 26;
            timeAsmLabel.Text = "00:00:000";
            // 
            // timeCppLabel
            // 
            timeCppLabel.AutoSize = true;
            timeCppLabel.Font = new Font("Segoe UI", 16F);
            timeCppLabel.Location = new Point(124, 666);
            timeCppLabel.Name = "timeCppLabel";
            timeCppLabel.Size = new Size(107, 30);
            timeCppLabel.TabIndex = 27;
            timeCppLabel.Text = "00:00:000";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 16F);
            label6.Location = new Point(42, 626);
            label6.Name = "label6";
            label6.Size = new Size(55, 30);
            label6.TabIndex = 28;
            label6.Text = "Asm";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 16F);
            label7.Location = new Point(42, 666);
            label7.Name = "label7";
            label7.Size = new Size(57, 30);
            label7.TabIndex = 29;
            label7.Text = "C++";
            // 
            // checkBoxAll
            // 
            checkBoxAll.AutoSize = true;
            checkBoxAll.Font = new Font("Segoe UI", 11F);
            checkBoxAll.Location = new Point(42, 511);
            checkBoxAll.Name = "checkBoxAll";
            checkBoxAll.Size = new Size(46, 24);
            checkBoxAll.TabIndex = 30;
            checkBoxAll.Text = "All";
            checkBoxAll.UseVisualStyleBackColor = true;
            checkBoxAll.CheckedChanged += checkBoxAll_CheckedChanged;
            // 
            // checkBoxSmall
            // 
            checkBoxSmall.AutoSize = true;
            checkBoxSmall.Font = new Font("Segoe UI", 11F);
            checkBoxSmall.Location = new Point(93, 511);
            checkBoxSmall.Name = "checkBoxSmall";
            checkBoxSmall.Size = new Size(65, 24);
            checkBoxSmall.TabIndex = 31;
            checkBoxSmall.Text = "Small";
            checkBoxSmall.UseVisualStyleBackColor = true;
            // 
            // checkBoxMedium
            // 
            checkBoxMedium.AutoSize = true;
            checkBoxMedium.Font = new Font("Segoe UI", 11F);
            checkBoxMedium.Location = new Point(164, 511);
            checkBoxMedium.Name = "checkBoxMedium";
            checkBoxMedium.Size = new Size(83, 24);
            checkBoxMedium.TabIndex = 32;
            checkBoxMedium.Text = "Medium";
            checkBoxMedium.UseVisualStyleBackColor = true;
            // 
            // checkBoxBig
            // 
            checkBoxBig.AutoSize = true;
            checkBoxBig.Font = new Font("Segoe UI", 11F);
            checkBoxBig.Location = new Point(251, 511);
            checkBoxBig.Name = "checkBoxBig";
            checkBoxBig.Size = new Size(50, 24);
            checkBoxBig.TabIndex = 33;
            checkBoxBig.Text = "Big";
            checkBoxBig.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.BackColor = SystemColors.Control;
            trackBar1.LargeChange = 8;
            trackBar1.Location = new Point(31, 197);
            trackBar1.Maximum = 64;
            trackBar1.Minimum = 1;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(195, 45);
            trackBar1.TabIndex = 34;
            trackBar1.Value = 1;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // labelXTimes
            // 
            labelXTimes.AutoSize = true;
            labelXTimes.Font = new Font("Segoe UI", 13F);
            labelXTimes.Location = new Point(307, 481);
            labelXTimes.Name = "labelXTimes";
            labelXTimes.Size = new Size(127, 25);
            labelXTimes.TabIndex = 36;
            labelXTimes.Text = "Repeat x times";
            // 
            // iterationsTextBox
            // 
            iterationsTextBox.Location = new Point(307, 513);
            iterationsTextBox.Margin = new Padding(3, 2, 3, 2);
            iterationsTextBox.Name = "iterationsTextBox";
            iterationsTextBox.Size = new Size(140, 23);
            iterationsTextBox.TabIndex = 35;
            iterationsTextBox.TextChanged += iterationsTextBox_TextChanged;
            // 
            // SaveButton
            // 
            SaveButton.Font = new Font("Segoe UI", 11F);
            SaveButton.Location = new Point(31, 119);
            SaveButton.Margin = new Padding(3, 2, 3, 2);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(120, 30);
            SaveButton.TabIndex = 37;
            SaveButton.Text = "Save Picture";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // threadsLabel
            // 
            threadsLabel.AutoSize = true;
            threadsLabel.Font = new Font("Segoe UI", 13F);
            threadsLabel.Location = new Point(31, 169);
            threadsLabel.Name = "threadsLabel";
            threadsLabel.Size = new Size(182, 25);
            threadsLabel.TabIndex = 38;
            threadsLabel.Text = "Number of threads: 1";
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 11F);
            button2.Location = new Point(37, 236);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(179, 30);
            button2.TabIndex = 39;
            button2.Text = "Set to number of cores";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 13F);
            label4.Location = new Point(42, 552);
            label4.Name = "label4";
            label4.Size = new Size(103, 25);
            label4.TabIndex = 40;
            label4.Text = "Run test on";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 13F);
            label5.Location = new Point(187, 552);
            label5.Name = "label5";
            label5.Size = new Size(29, 25);
            label5.TabIndex = 41;
            label5.Text = "to";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 13F);
            label8.Location = new Point(256, 552);
            label8.Name = "label8";
            label8.Size = new Size(71, 25);
            label8.TabIndex = 42;
            label8.Text = "threads";
            // 
            // textBoxStartThreadsNum
            // 
            textBoxStartThreadsNum.Location = new Point(151, 555);
            textBoxStartThreadsNum.Margin = new Padding(3, 2, 3, 2);
            textBoxStartThreadsNum.Name = "textBoxStartThreadsNum";
            textBoxStartThreadsNum.Size = new Size(30, 23);
            textBoxStartThreadsNum.TabIndex = 43;
            textBoxStartThreadsNum.Text = "1";
            textBoxStartThreadsNum.TextAlign = HorizontalAlignment.Center;
            textBoxStartThreadsNum.TextChanged += textBoxStartThreadsNum_TextChanged;
            // 
            // textBoxEndThreadsNum
            // 
            textBoxEndThreadsNum.Location = new Point(217, 555);
            textBoxEndThreadsNum.Margin = new Padding(3, 2, 3, 2);
            textBoxEndThreadsNum.Name = "textBoxEndThreadsNum";
            textBoxEndThreadsNum.Size = new Size(30, 23);
            textBoxEndThreadsNum.TabIndex = 44;
            textBoxEndThreadsNum.Text = "64";
            textBoxEndThreadsNum.TextAlign = HorizontalAlignment.Center;
            textBoxEndThreadsNum.TextChanged += textBoxEndThreadsNum_TextChanged;
            // 
            // checkBoxDoublingEachIter
            // 
            checkBoxDoublingEachIter.AutoSize = true;
            checkBoxDoublingEachIter.Font = new Font("Segoe UI", 11F);
            checkBoxDoublingEachIter.Location = new Point(333, 555);
            checkBoxDoublingEachIter.Name = "checkBoxDoublingEachIter";
            checkBoxDoublingEachIter.Size = new Size(183, 24);
            checkBoxDoublingEachIter.TabIndex = 45;
            checkBoxDoublingEachIter.Text = "doubling each iteration";
            checkBoxDoublingEachIter.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11F);
            label9.Location = new Point(269, 382);
            label9.Name = "label9";
            label9.Size = new Size(283, 40);
            label9.TabIndex = 46;
            label9.Text = "If applied radius value will be overwritten\r\nUse with caution or for experimenting.\r\n";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 13F);
            label10.Location = new Point(34, 292);
            label10.Name = "label10";
            label10.Size = new Size(95, 25);
            label10.TabIndex = 48;
            label10.Text = "Blur radius";
            // 
            // trackBarRadius
            // 
            trackBarRadius.BackColor = SystemColors.Control;
            trackBarRadius.LargeChange = 8;
            trackBarRadius.Location = new Point(34, 320);
            trackBarRadius.Maximum = 200;
            trackBarRadius.Minimum = 1;
            trackBarRadius.Name = "trackBarRadius";
            trackBarRadius.Size = new Size(195, 45);
            trackBarRadius.TabIndex = 47;
            trackBarRadius.Value = 10;
            trackBarRadius.Scroll += trackBar2_Scroll;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 13F);
            label11.Location = new Point(269, 264);
            label11.Name = "label11";
            label11.Size = new Size(188, 25);
            label11.TabIndex = 49;
            label11.Text = "Additional parameters";
            // 
            // textRadiusVal
            // 
            textRadiusVal.Location = new Point(63, 356);
            textRadiusVal.Margin = new Padding(3, 2, 3, 2);
            textRadiusVal.Name = "textRadiusVal";
            textRadiusVal.Size = new Size(140, 23);
            textRadiusVal.TabIndex = 51;
            textRadiusVal.TextAlign = HorizontalAlignment.Center;
            textRadiusVal.TextChanged += textRadiusVal_TextChanged;
            textRadiusVal.Leave += textRadiusVal_Leave;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1680, 781);
            Controls.Add(textRadiusVal);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(trackBarRadius);
            Controls.Add(label9);
            Controls.Add(checkBoxDoublingEachIter);
            Controls.Add(textBoxEndThreadsNum);
            Controls.Add(textBoxStartThreadsNum);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(button2);
            Controls.Add(threadsLabel);
            Controls.Add(SaveButton);
            Controls.Add(labelXTimes);
            Controls.Add(iterationsTextBox);
            Controls.Add(trackBar1);
            Controls.Add(checkBoxBig);
            Controls.Add(checkBoxMedium);
            Controls.Add(checkBoxSmall);
            Controls.Add(checkBoxAll);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(timeCppLabel);
            Controls.Add(timeAsmLabel);
            Controls.Add(runTestButton);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(radioButtonCpp);
            Controls.Add(radioButtonAsm);
            Controls.Add(labelStatus);
            Controls.Add(labelSigmaSize);
            Controls.Add(labelKernelSize);
            Controls.Add(blureConfigButton);
            Controls.Add(sigmaTextBox);
            Controls.Add(kernelSizeTextBox);
            Controls.Add(processPictureButton);
            Controls.Add(processedPicture);
            Controls.Add(loadPictureButton);
            Controls.Add(inputPicture);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)inputPicture).EndInit();
            ((System.ComponentModel.ISupportInitialize)processedPicture).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox inputPicture;
        private Button loadPictureButton;
        private OpenFileDialog openFileDialog1;
        private PictureBox processedPicture;
        private Button processPictureButton;
        private TextBox kernelSizeTextBox;
        private TextBox sigmaTextBox;
        private Button blureConfigButton;
        private Label labelKernelSize;
        private Label labelSigmaSize;
        private Label labelStatus;
        private RadioButton radioButtonAsm;
        private RadioButton radioButtonCpp;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button runTestButton;
        private Label timeAsmLabel;
        private Label timeCppLabel;
        private Label label6;
        private Label label7;
        private CheckBox checkBoxAll;
        private CheckBox checkBoxSmall;
        private CheckBox checkBoxMedium;
        private CheckBox checkBoxBig;
        private TrackBar trackBar1;
        private Label labelXTimes;
        private TextBox iterationsTextBox;
        private Button SaveButton;
        private Label threadsLabel;
        private Button button2;
        private Label label4;
        private Label label5;
        private Label label8;
        private TextBox textBoxStartThreadsNum;
        private TextBox textBoxEndThreadsNum;
        private CheckBox checkBoxDoublingEachIter;
        private Label label9;
        private Label label10;
        private TrackBar trackBarRadius;
        private Label label11;
        private TextBox textRadiusVal;
    }
}