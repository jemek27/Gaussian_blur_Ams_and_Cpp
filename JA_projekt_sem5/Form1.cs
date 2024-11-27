using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

//TODO cleaning
namespace JA_projekt_sem5 {
    public partial class Form1 : Form {

        const string dllPath = "..\\..\\..\\..\\..\\x64\\Release\\"; //Debug Release

        [DllImport(dllPath + "JAAsm.dll")]
        private static extern float expAsm(float x);

        [DllImport(dllPath + "JAAsm.dll")]
        private static extern void createGaussianKernelAsm(Byte kernelSize, float sigma, float[] kernel);

        [DllImport(dllPath + "C_functions.dll")]
        private static extern void createGaussianKernel(Byte kernelSize, float sigma, float[] kernel);

        private ImageProcessor imageProcessor;

        private Bitmap bitmap;
        private Bitmap bitmapOutput;

        private OpenFileDialog openFileDialog;


        private int numOfThreads = 1;
        private float radius = 1;
        bool useRadius = false;

        private long totalTestTimeCpp = 0;
        private long totalTestTimeAsm = 0;
        private long tempTestTimeCpp = 0;
        private long tempTestTimeAsm = 0;
        private int testIterations = 1;
        private int testStartThreadsNum = 1;
        private int testEndThreadsNum = 64;

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

        private static Bitmap ConvertImageToBitmap(string imagePath) {
            using (Image image = Image.FromFile(imagePath)) {
                return new Bitmap(image);
            }
        }

        private static void DisplayImage(Bitmap picture, PictureBox picBox) {
            Bitmap img = picture;
            if (img.Width > picBox.Width || img.Height > picBox.Height) {
                img = ResizeImage(img, picBox.Width, picBox.Height);
            }

            picBox.Image = img;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void blurConfigButton_Click(object sender, EventArgs e) {
            try {
                Byte kernelSize = Convert.ToByte(kernelSizeTextBox.Text);
                float sigma = float.Parse(sigmaTextBox.Text);
                imageProcessor.updateKernelConfig(kernelSize, sigma);
                useRadius = false;
                //TODO remove after check
                float[] tabK = new float[kernelSize];
                float[] tabKC = new float[kernelSize];

                createGaussianKernel(kernelSize, sigma, tabKC);
                createGaussianKernelAsm(kernelSize, sigma, tabK);
                labelStatus.Text = $"tabK= [{string.Join(", ", tabK)}]\n" +
                                   $"tabC= [{string.Join(", ", tabKC)}]";
            } catch (FormatException ex) {
                MessageBox.Show("Input format is incorrect: " + ex.Message);
            }
            //////TODO remove after check
            //float a = 2.7f;
            //labelStatus.Text = $"exp(2,7)= {expAsm(a)}";
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

        private static Bitmap ResizeImage(Bitmap img, int maxWidth, int maxHeight) {

            float ratioX = (float)maxWidth / img.Width;
            float ratioY = (float)maxHeight / img.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            return newImage;
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

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxAll.Checked) {
                checkBoxSmall.Checked = true;
                checkBoxMedium.Checked = true;
                checkBoxBig.Checked = true;
            } else {
                checkBoxSmall.Checked = false;
                checkBoxMedium.Checked = false;
                checkBoxBig.Checked = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            numOfThreads = ((byte)trackBar1.Value);
            threadsLabel.Text = "Number of threads: " + numOfThreads.ToString();
        }

        private void iterationsTextBox_TextChanged(object sender, EventArgs e) {
            if (labelXTimes.Text.Length != 0) {
                try {
                    testIterations = Convert.ToInt32(iterationsTextBox.Text);
                    labelXTimes.Text = $"Repeat {testIterations} times";
                } catch (FormatException ex) {
                    labelXTimes.Text = $"Repeat {testIterations} times    Incorrect format!";
                }
            }
        }

        public void SaveBitmapAsBmp(Bitmap bitmap, string filePath) {
            try {
                bitmap.Save(filePath, ImageFormat.Bmp);
                labelStatus.Text = $"Image was saved at: {filePath}";
            } catch (Exception ex) {
                labelStatus.Text = $"Image saving error: {ex.Message}";
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            numOfThreads = Environment.ProcessorCount;
            trackBar1.Value = numOfThreads;
            threadsLabel.Text = "Number of threads: " + numOfThreads.ToString();
        }

        private void textBoxStartThreadsNum_TextChanged(object sender, EventArgs e) {
            if (textBoxStartThreadsNum.Text.Length != 0) {
                try {
                    testStartThreadsNum = Convert.ToInt32(textBoxStartThreadsNum.Text);

                    if (testStartThreadsNum == 0) {
                        textBoxStartThreadsNum.Text = testStartThreadsNum.ToString();
                        //Set cursor to end
                        textBoxStartThreadsNum.SelectionStart = textBoxStartThreadsNum.Text.Length;
                    }
                } catch (FormatException ex) {
                    textBoxStartThreadsNum.Text = testStartThreadsNum.ToString();
                    //Set cursor to end
                    textBoxStartThreadsNum.SelectionStart = textBoxStartThreadsNum.Text.Length;
                }
            }
        }

        private void textBoxEndThreadsNum_TextChanged(object sender, EventArgs e) {
            if (textBoxEndThreadsNum.Text.Length != 0) {
                try {
                    testEndThreadsNum = Convert.ToInt32(textBoxEndThreadsNum.Text);
                } catch (FormatException ex) {
                    textBoxEndThreadsNum.Text = testEndThreadsNum.ToString();
                    //Set cursor to end
                    textBoxEndThreadsNum.SelectionStart = textBoxEndThreadsNum.Text.Length;
                }
            }
        }

        private void runTestButton_Click(object sender, EventArgs e) {
            const string imgPath = "..\\..\\..\\..\\..\\assets\\";
            const string csvHistoryPath = "..\\..\\..\\..\\..\\testTimeHistory.csv";
            const string csvTestTimePath = "..\\..\\..\\..\\..\\testTime.csv";
            Bitmap bitmapSmall = ConvertImageToBitmap(imgPath + "sum-ryba-900x450.bmp");
            Bitmap bitmapMedium = ConvertImageToBitmap(imgPath + "krolWod.bmp");
            Bitmap bitmapBig = ConvertImageToBitmap(imgPath + "PXL_20240915_075912464.bmp");

            Byte kernelSize = imageProcessor.getKernelSize();
            Stopwatch stopwatch = new Stopwatch();
            String csvDataBuffer = "";
            int testNumOfThreads = testStartThreadsNum;

            // Select the appropriate lambda based on the checkbox state
            Action incrementAction = checkBoxDoublingEachIter.Checked
                ? () => testNumOfThreads *= 2
                : () => ++testNumOfThreads;

            bool testRunning = true;
            for (; testRunning; incrementAction()) {

                //last iteration
                if (testNumOfThreads >= testEndThreadsNum) {
                    testNumOfThreads = testEndThreadsNum;
                    testRunning = false;
                }

                int counter = testIterations;

                if (checkBoxSmall.Checked) {
                    while (counter > 0) {
                        --counter;
                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurCpp(bitmapSmall, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();

                        //////////////////////

                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurAsm(bitmapSmall, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();
                        labelStatus.Text = $"Test completed: small image, iteration {counter}, threads = {testNumOfThreads}";
                        labelStatus.Refresh();
                    }
                    csvDataBuffer += $"{tempTestTimeCpp},{tempTestTimeAsm},small,{testNumOfThreads},{testIterations},{kernelSize} \n";
                    totalTestTimeCpp += tempTestTimeCpp;
                    totalTestTimeAsm += tempTestTimeAsm;
                    tempTestTimeCpp = 0;
                    tempTestTimeAsm = 0;
                    counter = testIterations;
                }
                if (checkBoxMedium.Checked) {
                    while (counter > 0) {
                        --counter;
                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurCpp(bitmapMedium, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();

                        //////////////////////

                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurAsm(bitmapMedium, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();
                        labelStatus.Text = $"Test completed: small image, iteration {counter}, threads = {testNumOfThreads}";
                        labelStatus.Refresh();
                    }
                    csvDataBuffer += $"{tempTestTimeCpp},{tempTestTimeAsm},medium,{testNumOfThreads},{testIterations},{kernelSize} \n";
                    totalTestTimeCpp += tempTestTimeCpp;
                    totalTestTimeAsm += tempTestTimeAsm;
                    tempTestTimeCpp = 0;
                    tempTestTimeAsm = 0;
                    counter = testIterations;
                }
                if (checkBoxBig.Checked) {
                    while (counter > 0) {
                        --counter;
                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurCpp(bitmapBig, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();

                        //////////////////////

                        stopwatch.Start();
                        imageProcessor.applyGaussianBlurAsm(bitmapBig, testNumOfThreads);
                        stopwatch.Stop();

                        tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();
                        labelStatus.Text = $"Test completed: small image, iteration {counter}, threads = {testNumOfThreads}";
                        labelStatus.Refresh();
                    }
                    csvDataBuffer += $"{tempTestTimeCpp},{tempTestTimeAsm},big,{testNumOfThreads},{testIterations},{kernelSize} \n";
                    totalTestTimeCpp += tempTestTimeCpp;
                    totalTestTimeAsm += tempTestTimeAsm;
                    tempTestTimeCpp = 0;
                    tempTestTimeAsm = 0;
                    counter = testIterations;
                }
                csvDataBuffer += $"{totalTestTimeCpp},{totalTestTimeAsm},all,{testNumOfThreads},{testIterations},{kernelSize} \n";


                timeCppLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeCpp);
                timeAsmLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeAsm);
                timeCppLabel.Refresh();  // Force immediate label update
                timeAsmLabel.Refresh();
                totalTestTimeCpp = 0;
                totalTestTimeAsm = 0;
            }

            csvDataBuffer = csvDataBuffer.Remove(csvDataBuffer.Length - 1); // last \n is not needed

            SaveToCsv(csvTestTimePath, csvDataBuffer, false);
            SaveToCsv(csvHistoryPath, csvDataBuffer);
        }

        private string ConvertMillisecondsToTimeFormat(long milliseconds) {

            long minutes = milliseconds / 60000;
            long seconds = (milliseconds % 60000) / 1000;
            long remainingMilliseconds = milliseconds % 1000;

            // Format as MM:SS:FFF
            return $"{minutes:D2}:{seconds:D2}:{remainingMilliseconds:D3}";
        }
        private void SaveToCsv(string filePath, string dataRows, bool append = true) {
            bool fileExists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, append: append)) {
                if (!fileExists || !append) { //if not exists or when overwriting
                    writer.WriteLine("Time-cpp-ms,Time-asm-ms,sizeName,Threads,Number-of-iterations,Kernel-size");
                }

                writer.WriteLine(dataRows);
                labelStatus.Text = "saved to csv";
            }
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
