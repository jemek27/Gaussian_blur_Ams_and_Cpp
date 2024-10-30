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
            button1 = new Button();
            labelAsmTestResult = new Label();
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
            ((System.ComponentModel.ISupportInitialize)inputPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)processedPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // inputPicture
            // 
            inputPicture.Location = new Point(30, 182);
            inputPicture.Margin = new Padding(3, 2, 3, 2);
            inputPicture.Name = "inputPicture";
            inputPicture.Size = new Size(750, 500);
            inputPicture.TabIndex = 5;
            inputPicture.TabStop = false;
            // 
            // loadPictureButton
            // 
            loadPictureButton.Font = new Font("Segoe UI", 11F);
            loadPictureButton.Location = new Point(376, 56);
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
            processedPicture.Location = new Point(805, 182);
            processedPicture.Margin = new Padding(3, 2, 3, 2);
            processedPicture.Name = "processedPicture";
            processedPicture.Size = new Size(750, 500);
            processedPicture.TabIndex = 7;
            processedPicture.TabStop = false;
            // 
            // processPictureButton
            // 
            processPictureButton.Font = new Font("Segoe UI", 11F);
            processPictureButton.Location = new Point(376, 90);
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
            kernelSizeTextBox.Location = new Point(30, 58);
            kernelSizeTextBox.Margin = new Padding(3, 2, 3, 2);
            kernelSizeTextBox.Name = "kernelSizeTextBox";
            kernelSizeTextBox.Size = new Size(140, 23);
            kernelSizeTextBox.TabIndex = 9;
            // 
            // sigmaTextBox
            // 
            sigmaTextBox.Location = new Point(187, 58);
            sigmaTextBox.Margin = new Padding(3, 2, 3, 2);
            sigmaTextBox.Name = "sigmaTextBox";
            sigmaTextBox.Size = new Size(140, 23);
            sigmaTextBox.TabIndex = 10;
            // 
            // blureConfigButton
            // 
            blureConfigButton.Font = new Font("Segoe UI", 11F);
            blureConfigButton.Location = new Point(31, 95);
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
            labelKernelSize.Font = new Font("Segoe UI", 13F);
            labelKernelSize.Location = new Point(30, 27);
            labelKernelSize.Name = "labelKernelSize";
            labelKernelSize.Size = new Size(128, 25);
            labelKernelSize.TabIndex = 16;
            labelKernelSize.Text = "Set kernel size ";
            // 
            // labelSigmaSize
            // 
            labelSigmaSize.AutoSize = true;
            labelSigmaSize.Font = new Font("Segoe UI", 13F);
            labelSigmaSize.Location = new Point(187, 27);
            labelSigmaSize.Name = "labelSigmaSize";
            labelSigmaSize.Size = new Size(136, 25);
            labelSigmaSize.TabIndex = 13;
            labelSigmaSize.Text = "Set sigma value";
            // 
            // button1
            // 
            button1.Location = new Point(31, 686);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(82, 22);
            button1.TabIndex = 14;
            button1.Text = "Test asm";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonTestAsm_Click;
            // 
            // labelAsmTestResult
            // 
            labelAsmTestResult.AutoSize = true;
            labelAsmTestResult.Location = new Point(131, 690);
            labelAsmTestResult.Name = "labelAsmTestResult";
            labelAsmTestResult.Size = new Size(0, 15);
            labelAsmTestResult.TabIndex = 15;
            // 
            // radioButtonAsm
            // 
            radioButtonAsm.AutoSize = true;
            radioButtonAsm.Checked = true;
            radioButtonAsm.Font = new Font("Segoe UI", 11F);
            radioButtonAsm.Location = new Point(515, 58);
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
            radioButtonCpp.Location = new Point(515, 84);
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
            label1.Location = new Point(805, 27);
            label1.Name = "label1";
            label1.Size = new Size(96, 25);
            label1.TabIndex = 18;
            label1.Text = "Speed test";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(805, 52);
            label2.Name = "label2";
            label2.Size = new Size(352, 20);
            label2.TabIndex = 19;
            label2.Text = "Compare execution times of Assembly and C++ Dll ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 13F);
            label3.Location = new Point(805, 95);
            label3.Name = "label3";
            label3.Size = new Size(210, 25);
            label3.TabIndex = 24;
            label3.Text = "Select testing picture size";
            // 
            // runTestButton
            // 
            runTestButton.Font = new Font("Segoe UI", 11F);
            runTestButton.Location = new Point(1326, 42);
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
            timeAsmLabel.Location = new Point(1408, 78);
            timeAsmLabel.Name = "timeAsmLabel";
            timeAsmLabel.Size = new Size(107, 30);
            timeAsmLabel.TabIndex = 26;
            timeAsmLabel.Text = "00:00:000";
            // 
            // timeCppLabel
            // 
            timeCppLabel.AutoSize = true;
            timeCppLabel.Font = new Font("Segoe UI", 16F);
            timeCppLabel.Location = new Point(1408, 118);
            timeCppLabel.Name = "timeCppLabel";
            timeCppLabel.Size = new Size(107, 30);
            timeCppLabel.TabIndex = 27;
            timeCppLabel.Text = "00:00:000";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 16F);
            label6.Location = new Point(1326, 78);
            label6.Name = "label6";
            label6.Size = new Size(55, 30);
            label6.TabIndex = 28;
            label6.Text = "Asm";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 16F);
            label7.Location = new Point(1326, 118);
            label7.Name = "label7";
            label7.Size = new Size(57, 30);
            label7.TabIndex = 29;
            label7.Text = "C++";
            // 
            // checkBoxAll
            // 
            checkBoxAll.AutoSize = true;
            checkBoxAll.Font = new Font("Segoe UI", 11F);
            checkBoxAll.Location = new Point(810, 125);
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
            checkBoxSmall.Location = new Point(861, 125);
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
            checkBoxMedium.Location = new Point(932, 125);
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
            checkBoxBig.Location = new Point(1019, 125);
            checkBoxBig.Name = "checkBoxBig";
            checkBoxBig.Size = new Size(50, 24);
            checkBoxBig.TabIndex = 33;
            checkBoxBig.Text = "Big";
            checkBoxBig.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.LargeChange = 8;
            trackBar1.Location = new Point(376, 125);
            trackBar1.Maximum = 128;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(195, 45);
            trackBar1.TabIndex = 34;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // labelXTimes
            // 
            labelXTimes.AutoSize = true;
            labelXTimes.Font = new Font("Segoe UI", 13F);
            labelXTimes.Location = new Point(1075, 95);
            labelXTimes.Name = "labelXTimes";
            labelXTimes.Size = new Size(127, 25);
            labelXTimes.TabIndex = 36;
            labelXTimes.Text = "Repeat x times";
            // 
            // iterationsTextBox
            // 
            iterationsTextBox.Location = new Point(1075, 127);
            iterationsTextBox.Margin = new Padding(3, 2, 3, 2);
            iterationsTextBox.Name = "iterationsTextBox";
            iterationsTextBox.Size = new Size(140, 23);
            iterationsTextBox.TabIndex = 35;
            iterationsTextBox.TextChanged += iterationsTextBox_TextChanged;
            // 
            // SaveButton
            // 
            SaveButton.Font = new Font("Segoe UI", 11F);
            SaveButton.Location = new Point(603, 58);
            SaveButton.Margin = new Padding(3, 2, 3, 2);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(120, 30);
            SaveButton.TabIndex = 37;
            SaveButton.Text = "Save Picture";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 761);
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
            Controls.Add(labelAsmTestResult);
            Controls.Add(button1);
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
        private Button button1;
        private Label labelAsmTestResult;
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
    }
}