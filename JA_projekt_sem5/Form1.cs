using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace JA_projekt_sem5 {
    public partial class Form1 : Form {

        private ImageProcessor imageProcessor;
        private OpenFileDialog openFileDialog;

        private Bitmap bitmap;
        private Bitmap bitmapOutput;

        private int numOfThreads = 1;
        private float radius = 1;
        bool useRadius = true;


        public Form1() {
            InitializeComponent();
            imageProcessor = new ImageProcessor();
        }

        private void Form1_Load(object sender, EventArgs e) {
            openFileDialog = new OpenFileDialog {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Select Image"
            };
        }

        private void blurConfigButton_Click(object sender, EventArgs e) {
            try {
                Byte kernelSize = Convert.ToByte(kernelSizeTextBox.Text);
                float sigma = float.Parse(sigmaTextBox.Text);
                imageProcessor.updateKernelConfig(kernelSize, sigma);
                useRadius = false;
            } catch (FormatException ex) {
                MessageBox.Show("Input format is incorrect: " + ex.Message);
            }
        }

        private void buttonLoadPicture_Click(object sender, EventArgs e) {

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                bitmap = ConvertImageToBitmap(openFileDialog.FileName);
                DisplayImage(bitmap, inputPicture);
                labelStatus.Text = $"Loaded Image: {openFileDialog.FileName}";
            }
        }

        private void processPictureButton_Click(object sender, EventArgs e) {
            if (useRadius) {
                imageProcessor.updateKernelConfig(radius);
            }

            labelStatus.Text = "Image blur started";
            labelStatus.Refresh();

            if (radioButtonAsm.Checked) {
                bitmapOutput = imageProcessor.applyGaussianBlurAsm(bitmap, numOfThreads);
            } else if (radioButtonCpp.Checked) {
                bitmapOutput = imageProcessor.applyGaussianBlurCpp(bitmap, numOfThreads);
            }

            if (bitmapOutput != null) {
                DisplayImage(bitmapOutput, processedPicture);
                labelStatus.Text = "Image blur completed";
            } else {
                labelStatus.Text = "Image blur failed";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e) {
            if (bitmapOutput != null) {
                SaveBitmapAsBmp(bitmapOutput, "..\\..\\..\\..\\..\\output.bmp");
            }
        }

        private void radioButtonAsm_CheckedChanged(object sender, EventArgs e) {
            if (radioButtonAsm.Checked) {
                radioButtonCpp.Checked = false;
            }
        }

        private void radioButtonCpp_CheckedChanged(object sender, EventArgs e) {
            if (radioButtonCpp.Checked) {
                radioButtonAsm.Checked = false;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            numOfThreads = Environment.ProcessorCount;
            trackBar1.Value = numOfThreads;
            threadsLabel.Text = "Number of threads: " + numOfThreads.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e) {
            radius = ((float)trackBarRadius.Value) / 10.0f;
            textRadiusVal.Text = radius.ToString();
            useRadius = true;
        }

        private void textRadiusVal_TextChanged(object sender, EventArgs e) {

            if (float.TryParse(textRadiusVal.Text, out float newRadius)) {

                float minRadius = (float)trackBarRadius.Minimum / 10.0f;
                float maxRadius = (float)trackBarRadius.Maximum / 10.0f;

                if (newRadius < minRadius) {
                    trackBarRadius.Value = trackBarRadius.Minimum;
                    radius = 0.1f;
                } else if (newRadius > maxRadius) {
                    trackBarRadius.Value = trackBarRadius.Maximum;
                    radius = newRadius;
                } else {
                    trackBarRadius.Value = (int)(newRadius * 10);
                    radius = newRadius;
                }

                useRadius = true;
            } else {
                if (textRadiusVal.Text.Length != 0) {
                    textRadiusVal.Text = radius.ToString();
                }
            }
        }

        private void textRadiusVal_Leave(object sender, EventArgs e) {
            if (textRadiusVal.Text.Length == 0) {
                textRadiusVal.Text = radius.ToString();
            } else if (float.TryParse(textRadiusVal.Text, out float lastRadius)) {

                float minRadius = (float)trackBarRadius.Minimum / 10.0f;
                if (lastRadius < minRadius) {
                    textRadiusVal.Text = radius.ToString();
                }
            }
        }
    }
}
