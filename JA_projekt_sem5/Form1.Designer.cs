namespace JA_projekt_sem5
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            ((System.ComponentModel.ISupportInitialize)inputPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)processedPicture).BeginInit();
            SuspendLayout();
            // 
            // inputPicture
            // 
            inputPicture.Location = new Point(102, 162);
            inputPicture.Margin = new Padding(3, 2, 3, 2);
            inputPicture.Name = "inputPicture";
            inputPicture.Size = new Size(525, 300);
            inputPicture.TabIndex = 5;
            inputPicture.TabStop = false;
            // 
            // loadPictureButton
            // 
            loadPictureButton.Location = new Point(102, 480);
            loadPictureButton.Margin = new Padding(3, 2, 3, 2);
            loadPictureButton.Name = "loadPictureButton";
            loadPictureButton.Size = new Size(105, 22);
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
            processedPicture.Location = new Point(653, 162);
            processedPicture.Margin = new Padding(3, 2, 3, 2);
            processedPicture.Name = "processedPicture";
            processedPicture.Size = new Size(525, 300);
            processedPicture.TabIndex = 7;
            processedPicture.TabStop = false;
            // 
            // processPictureButton
            // 
            processPictureButton.Location = new Point(222, 481);
            processPictureButton.Margin = new Padding(3, 2, 3, 2);
            processPictureButton.Name = "processPictureButton";
            processPictureButton.Size = new Size(106, 22);
            processPictureButton.TabIndex = 8;
            processPictureButton.Text = "Process Picture";
            processPictureButton.UseVisualStyleBackColor = true;
            processPictureButton.Click += processPictureButton_Click;
            // 
            // kernelSizeTextBox
            // 
            kernelSizeTextBox.Location = new Point(102, 31);
            kernelSizeTextBox.Margin = new Padding(3, 2, 3, 2);
            kernelSizeTextBox.Name = "kernelSizeTextBox";
            kernelSizeTextBox.Size = new Size(178, 23);
            kernelSizeTextBox.TabIndex = 9;
            // 
            // sigmaTextBox
            // 
            sigmaTextBox.Location = new Point(303, 31);
            sigmaTextBox.Margin = new Padding(3, 2, 3, 2);
            sigmaTextBox.Name = "sigmaTextBox";
            sigmaTextBox.Size = new Size(178, 23);
            sigmaTextBox.TabIndex = 10;
            // 
            // blureConfigButton
            // 
            blureConfigButton.Location = new Point(102, 72);
            blureConfigButton.Margin = new Padding(3, 2, 3, 2);
            blureConfigButton.Name = "blureConfigButton";
            blureConfigButton.Size = new Size(82, 22);
            blureConfigButton.TabIndex = 11;
            blureConfigButton.Text = "Apply";
            blureConfigButton.UseVisualStyleBackColor = true;
            blureConfigButton.Click += blurConfigButton_Click;
            // 
            // labelKernelSize
            // 
            labelKernelSize.AutoSize = true;
            labelKernelSize.Location = new Point(102, 14);
            labelKernelSize.Name = "labelKernelSize";
            labelKernelSize.Size = new Size(83, 15);
            labelKernelSize.TabIndex = 12;
            labelKernelSize.Text = "Set kernel size ";
            // 
            // labelSigmaSize
            // 
            labelSigmaSize.AutoSize = true;
            labelSigmaSize.Location = new Point(303, 14);
            labelSigmaSize.Name = "labelSigmaSize";
            labelSigmaSize.Size = new Size(89, 15);
            labelSigmaSize.TabIndex = 13;
            labelSigmaSize.Text = "Set sigma value";
            // 
            // button1
            // 
            button1.Location = new Point(653, 72);
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
            labelAsmTestResult.Location = new Point(752, 75);
            labelAsmTestResult.Name = "labelAsmTestResult";
            labelAsmTestResult.Size = new Size(0, 15);
            labelAsmTestResult.TabIndex = 15;
            // 
            // radioButtonAsm
            // 
            radioButtonAsm.AutoSize = true;
            radioButtonAsm.Checked = true;
            radioButtonAsm.Location = new Point(360, 483);
            radioButtonAsm.Name = "radioButtonAsm";
            radioButtonAsm.Size = new Size(49, 19);
            radioButtonAsm.TabIndex = 16;
            radioButtonAsm.TabStop = true;
            radioButtonAsm.Text = "Asm";
            radioButtonAsm.UseVisualStyleBackColor = true;
            radioButtonAsm.CheckedChanged += radioButtonAsm_CheckedChanged;
            // 
            // radioButtonCpp
            // 
            radioButtonCpp.AutoSize = true;
            radioButtonCpp.Location = new Point(360, 508);
            radioButtonCpp.Name = "radioButtonCpp";
            radioButtonCpp.Size = new Size(49, 19);
            radioButtonCpp.TabIndex = 17;
            radioButtonCpp.Text = "C++";
            radioButtonCpp.UseVisualStyleBackColor = true;
            radioButtonCpp.CheckedChanged += radioButtonCpp_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1243, 565);
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
    }
}
