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
            ((System.ComponentModel.ISupportInitialize)inputPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)processedPicture).BeginInit();
            SuspendLayout();
            // 
            // inputPicture
            // 
            inputPicture.Location = new Point(117, 216);
            inputPicture.Name = "inputPicture";
            inputPicture.Size = new Size(600, 400);
            inputPicture.TabIndex = 5;
            inputPicture.TabStop = false;
            // 
            // loadPictureButton
            // 
            loadPictureButton.Location = new Point(117, 640);
            loadPictureButton.Name = "loadPictureButton";
            loadPictureButton.Size = new Size(120, 29);
            loadPictureButton.TabIndex = 6;
            loadPictureButton.Text = "Load Picture";
            loadPictureButton.UseVisualStyleBackColor = true;
            loadPictureButton.Click += button2_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // processedPicture
            // 
            processedPicture.Location = new Point(746, 216);
            processedPicture.Name = "processedPicture";
            processedPicture.Size = new Size(600, 400);
            processedPicture.TabIndex = 7;
            processedPicture.TabStop = false;
            // 
            // processPictureButton
            // 
            processPictureButton.Location = new Point(254, 641);
            processPictureButton.Name = "processPictureButton";
            processPictureButton.Size = new Size(121, 29);
            processPictureButton.TabIndex = 8;
            processPictureButton.Text = "Process Picture";
            processPictureButton.UseVisualStyleBackColor = true;
            processPictureButton.Click += processPictureButton_Click;
            // 
            // kernelSizeTextBox
            // 
            kernelSizeTextBox.Location = new Point(117, 41);
            kernelSizeTextBox.Name = "kernelSizeTextBox";
            kernelSizeTextBox.Size = new Size(203, 27);
            kernelSizeTextBox.TabIndex = 9;
            // 
            // sigmaTextBox
            // 
            sigmaTextBox.Location = new Point(346, 41);
            sigmaTextBox.Name = "sigmaTextBox";
            sigmaTextBox.Size = new Size(203, 27);
            sigmaTextBox.TabIndex = 10;
            // 
            // blureConfigButton
            // 
            blureConfigButton.Location = new Point(117, 96);
            blureConfigButton.Name = "blureConfigButton";
            blureConfigButton.Size = new Size(94, 29);
            blureConfigButton.TabIndex = 11;
            blureConfigButton.Text = "Apply";
            blureConfigButton.UseVisualStyleBackColor = true;
            blureConfigButton.Click += blurConfigButton_Click;
            // 
            // labelKernelSize
            // 
            labelKernelSize.AutoSize = true;
            labelKernelSize.Location = new Point(117, 18);
            labelKernelSize.Name = "labelKernelSize";
            labelKernelSize.Size = new Size(107, 20);
            labelKernelSize.TabIndex = 12;
            labelKernelSize.Text = "Set kernel size ";
            // 
            // labelSigmaSize
            // 
            labelSigmaSize.AutoSize = true;
            labelSigmaSize.Location = new Point(346, 18);
            labelSigmaSize.Name = "labelSigmaSize";
            labelSigmaSize.Size = new Size(113, 20);
            labelSigmaSize.TabIndex = 13;
            labelSigmaSize.Text = "Set sigma value";
            // 
            // button1
            // 
            button1.Location = new Point(746, 96);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 14;
            button1.Text = "Test asm";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // labelAsmTestResult
            // 
            labelAsmTestResult.AutoSize = true;
            labelAsmTestResult.Location = new Point(860, 100);
            labelAsmTestResult.Name = "labelAsmTestResult";
            labelAsmTestResult.Size = new Size(0, 20);
            labelAsmTestResult.TabIndex = 15;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1421, 753);
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
    }
}
