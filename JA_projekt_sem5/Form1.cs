using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

//TODO push used registers in asm
//TODO auto kernel = 6*sigma + 1 albo jakiœ switch od rozmiaru
//TODO asm wêtki 
namespace JA_projekt_sem5 {

    public partial class Form1 : Form {
        const string dllPath = "..\\..\\..\\..\\..\\x64\\Debug\\"; //Debug Release
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void createGaussianKernel(Byte kernelSize, float sigma, double[] kernel);

        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlur(IntPtr bitmapData, byte[] tempBitmapData, double[] kernel,
                                             int width, int height, int stride, int kernelSize,
                                             int startHeight, int endHeight);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlurStage1(byte[] bitmapData, byte[] tempBitmapData, double[] kernel,
                                                    int width, int height, int stride, int kernelSize,
                                                    int startHeight, int endHeight);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlurStage2(byte[] bitmapData, byte[] tempBitmapData, double[] kernel,
                                                    int width, int height, int stride, int kernelSize,
                                                    int startHeight, int endHeight);



        [DllImport(dllPath + "JAAsm.dll")]
        private static extern float createGaussianKernelAsm(Byte kernelSize, float sigma, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern int gaussBlurAsm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel); //TODO into void
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern int gaussBlurStage1Asm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel); //TODO into void
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern int gaussBlurStage2Asm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel); //TODO into void


        private Bitmap bitmap;
        private Bitmap bitmapOutput;

        private OpenFileDialog openFileDialog;

        private Byte kernelSize = 11;
        private float sigma = 4;

        private long totalTestTimeCpp = 0;
        private long totalTestTimeAsm = 0;
        private long tempTestTimeCpp = 0;
        private long tempTestTimeAsm = 0;
        private int testIterations = 1;
        private int numOfThreads = 1;

        public Form1() {
            InitializeComponent();
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
                //TODO not <=0 
                kernelSize = Convert.ToByte(kernelSizeTextBox.Text);
                sigma = float.Parse(sigmaTextBox.Text);
                if (kernelSize % 2 == 0) { kernelSize += 1; } // must be uneven  
            } catch (FormatException ex) {
                MessageBox.Show("Input format is incorrect: " + ex.Message);
            }
        }

        private void buttonLoadPicture_Click(object sender, EventArgs e) {

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                bitmap = ConvertImageToBitmap(openFileDialog.FileName);
                DisplayImage(bitmap, inputPicture);
            }
        }

        private void processPictureButton_Click(object sender, EventArgs e) {
            if (radioButtonAsm.Checked) {
                bitmapOutput = applyGaussianBlurAsm(bitmap);
            } else if (radioButtonCpp.Checked) {
                bitmapOutput = applyGaussianBlurCpp(bitmap);
            }

            if (bitmapOutput != null) {
                DisplayImage(bitmapOutput, processedPicture);
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
        [DllImport(dllPath + "JAAsm.dll")]

        private static extern float expAsm(float x);//, double[] tabX, int[] tabN);


        private void buttonTestAsm_Click(object sender, EventArgs e) {
            double[] tabX = new double[14];
            int[] tabN = new int[14];

            // Call the Assembly function to fill the arrays and calculate the sum
            float sum = expAsm(-0.78125f);//, tabX, tabN); -0.78125f
            double controlSum = 0;

            for (int i = 0; i < 14; i++) {
                controlSum += tabX[i] / Convert.ToDouble(tabN[i]); ;
            }

            float[] tabK = new float[kernelSize];
            float a = createGaussianKernelAsm(kernelSize, sigma, tabK);

            //Display the result
            //labelAsmTestResult.Text = $"Sum of tabX[i] / tabN[i] = {sum}  | constrol sum = {controlSum}";
            labelAsmTestResult.Text = $"Sum of tabX[i] / tabN[i] = {sum}  |   controlSum = {controlSum}    " +
                $"                      \n {string.Join(", ", tabX)}  " +
                $"                      \n {string.Join(", ", tabN)}\n" +
                $"                      Kernal = {a} | {string.Join(", ", tabK)}";
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

        private void runTestButton_Click(object sender, EventArgs e) {
            const string imgPath = "..\\..\\..\\..\\..\\assets\\";
            const string csvPath = "..\\..\\..\\..\\..\\testCSV.csv";
            Bitmap bitmapSmall = ConvertImageToBitmap(imgPath + "sum-ryba-900x450.bmp");
            Bitmap bitmapMedium = ConvertImageToBitmap(imgPath + "krolWod.bmp");
            Bitmap bitmapBig = ConvertImageToBitmap(imgPath + "PXL_20240915_075912464.bmp");

            Stopwatch stopwatch = new Stopwatch();
            int counter = testIterations;

            if (checkBoxSmall.Checked) {
                while (counter > 0) {
                    --counter;
                    stopwatch.Start();
                    applyGaussianBlurCpp(bitmapSmall);
                    stopwatch.Stop();

                    tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapSmall);
                    stopwatch.Stop();

                    tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm, "small");
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
                    applyGaussianBlurCpp(bitmapMedium);
                    stopwatch.Stop();

                    tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapMedium);
                    stopwatch.Stop();

                    tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm, "medium");
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
                    applyGaussianBlurCpp(bitmapBig);
                    stopwatch.Stop();

                    tempTestTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapBig);
                    stopwatch.Stop();

                    tempTestTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm, "big");
                totalTestTimeCpp += tempTestTimeCpp;
                totalTestTimeAsm += tempTestTimeAsm;
                tempTestTimeCpp = 0;
                tempTestTimeAsm = 0;
                counter = testIterations;
            }

            SaveToCsv(csvPath, totalTestTimeCpp, totalTestTimeAsm, "all");
            timeCppLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeCpp);
            timeAsmLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeAsm);
            totalTestTimeCpp = 0;
            totalTestTimeAsm = 0;
        }

        private Bitmap applyGaussianBlurCpp(Bitmap bmpIn) {
            if (bmpIn != null) {
                Bitmap bitmapBlurred = new Bitmap(bmpIn);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapBlurred.LockBits(
                    new Rectangle(0, 0, bitmapBlurred.Width, bitmapBlurred.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                // 1st stage saves output here and 2nd stage uses it as input
                byte[] tempCanvas = new byte[bmpData.Stride * bitmapBlurred.Height];

                // Working on buffer because of multithreading
                byte[] bmpDataBuffer = new byte[bmpData.Stride * bitmapBlurred.Height];
                Marshal.Copy(bmpData.Scan0, bmpDataBuffer, 0, bmpDataBuffer.Length);

                int segmentHeight = bitmapBlurred.Height / numOfThreads;


                double[] kernel = new double[kernelSize];
                createGaussianKernel(kernelSize, sigma, kernel);

                //TODO remove in relese
                double controlSum = 0;
                for (int i = 0; i < kernelSize; ++i) {
                    controlSum += kernel[i];
                }
                labelAsmTestResult.Text = $"retValcpp = {controlSum} | {kernelSize} | {sigma}  \n {string.Join(", ", kernel)}";

                // Assign starting and ending values for each segment
                Thread[] threads = new Thread[numOfThreads];
                int[] startHeights = new int[numOfThreads];
                int[] endHeights = new int[numOfThreads];

                // Segment the image
                for (int i = 0; i < numOfThreads; ++i) {
                    startHeights[i] = i * segmentHeight;
                    endHeights[i] = (i + 1) * segmentHeight;

                    if (i == numOfThreads - 1) { endHeights[i] = bitmapBlurred.Height; }
                }

                // Vertical blur
                for (int i = 0; i < numOfThreads; ++i) {
                    int start = startHeights[i];
                    int end = endHeights[i];

                    threads[i] = new Thread(() => {
                        gaussBlurStage1(bmpDataBuffer, tempCanvas, kernel, bitmapBlurred.Width, bitmapBlurred.Height,
                                        bmpData.Stride, kernelSize, start, end);
                    });
                    threads[i].Start();
                }

                // Wait for all vertical blur threads to finish
                foreach (var thread in threads) {
                    thread.Join();
                }

                // Horizontal blur
                for (int i = 0; i < numOfThreads; ++i) {
                    int start = startHeights[i];
                    int end = endHeights[i];
                    threads[i] = new Thread(() => {
                        gaussBlurStage2(bmpDataBuffer, tempCanvas, kernel, bitmapBlurred.Width, bitmapBlurred.Height,
                                        bmpData.Stride, kernelSize, start, end);
                    });
                    threads[i].Start();
                }

                // Wait for all horizontal blur threads to finish
                foreach (var thread in threads) {
                    thread.Join();
                }


                // Copy processed data back to the bitmap
                Marshal.Copy(bmpDataBuffer, 0, bmpData.Scan0, bmpDataBuffer.Length);
                bitmapBlurred.UnlockBits(bmpData);
                return bitmapBlurred;
            } else {
                return null;
            }
        }

        private Bitmap applyGaussianBlurAsm(Bitmap bmp) {
            if (bmp != null) {
                Bitmap bitmapBlurred = new Bitmap(bmp);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapBlurred.LockBits(
                    new Rectangle(0, 0, bitmapBlurred.Width, bitmapBlurred.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                // 1st stage saves output here and 2nd stage uses it as input
                byte[] tempCanvas = new byte[bmpData.Stride * bitmapBlurred.Height];

                // Working on buffer because of multithreading
                byte[] bmpDataBuffer = new byte[bmpData.Stride * bitmapBlurred.Height];
                Marshal.Copy(bmpData.Scan0, bmpDataBuffer, 0, bmpDataBuffer.Length);

                int segmentHeight = bitmapBlurred.Height / numOfThreads;

                float[] kernel = new float[kernelSize];
                float a = createGaussianKernelAsm(kernelSize, sigma, kernel);
                ///////////

                // Assign starting and ending values for each segment
                Thread[] threads = new Thread[numOfThreads];
                int[] startHeights = new int[numOfThreads];
                int[] endHeights = new int[numOfThreads];

                // Segment the image
                for (int i = 0; i < numOfThreads; ++i) {
                    startHeights[i] = i * segmentHeight;
                    endHeights[i] = (i + 1) * segmentHeight;

                    if (i == numOfThreads - 1) { endHeights[i] = bitmapBlurred.Height; }
                }

                // Vertical blur
                for (int i = 0; i < numOfThreads; ++i) {
                    int start = startHeights[i];
                    int end = endHeights[i];

                    int[] gaussBlurAsmArguments = new int[5];
                    gaussBlurAsmArguments[0] = bitmapBlurred.Width;
                    gaussBlurAsmArguments[1] = end;
                    gaussBlurAsmArguments[2] = bmpData.Stride;
                    gaussBlurAsmArguments[3] = kernelSize;
                    gaussBlurAsmArguments[4] = start;

                    threads[i] = new Thread(() => {
                        gaussBlurStage1Asm(bmpDataBuffer, gaussBlurAsmArguments, tempCanvas, kernel);
                    });
                    threads[i].Start();
                }

                // Wait for all vertical blur threads to finish
                foreach (var thread in threads) {
                    thread.Join();
                }

                // Horizontal blur
                for (int i = 0; i < numOfThreads; ++i) {
                    int start = startHeights[i];
                    int end = endHeights[i];

                    int[] gaussBlurAsmArguments = new int[6];
                    gaussBlurAsmArguments[0] = bitmapBlurred.Width;
                    gaussBlurAsmArguments[1] = bitmapBlurred.Height;
                    gaussBlurAsmArguments[2] = bmpData.Stride;
                    gaussBlurAsmArguments[3] = kernelSize;
                    gaussBlurAsmArguments[4] = start;
                    gaussBlurAsmArguments[5] = end;

                    threads[i] = new Thread(() => {
                        gaussBlurStage2Asm(bmpDataBuffer, gaussBlurAsmArguments, tempCanvas, kernel);
                    });
                    threads[i].Start();
                }

                // Wait for all horizontal blur threads to finish
                foreach (var thread in threads) {
                    thread.Join();
                }


                // Copy processed data back to the bitmap
                Marshal.Copy(bmpDataBuffer, 0, bmpData.Scan0, bmpDataBuffer.Length);

                bitmapBlurred.UnlockBits(bmpData);
                return bitmapBlurred;
            } else {
                return null;
            }
        }

        private string ConvertMillisecondsToTimeFormat(long milliseconds) {

            long minutes = milliseconds / 60000;
            long seconds = (milliseconds % 60000) / 1000;
            long remainingMilliseconds = milliseconds % 1000;

            // Format as MM:SS:FFF
            return $"{minutes:D2}:{seconds:D2}:{remainingMilliseconds:D3}";
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            numOfThreads = ((byte)trackBar1.Value);
            threadsLabel.Text = "Number of threads: " + numOfThreads.ToString();
        }

        private void iterationsTextBox_TextChanged(object sender, EventArgs e) {
            try {
                testIterations = Convert.ToInt32(iterationsTextBox.Text);
                labelXTimes.Text = $"Repeat {testIterations} times";
            } catch (FormatException ex) {
                labelXTimes.Text = labelXTimes.Text + " Incorrect format!";
            }
        }

        private void SaveToCsv(string filePath, long cppTime, long asmTime, String sizeName) {
            bool fileExists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, append: true)) {
                if (!fileExists) {
                    writer.WriteLine("Time-cpp-ms,Time-asm-ms,sizeName,Threads,Number-of-iterations");
                }

                writer.WriteLine($"{cppTime},{asmTime},{sizeName},{numOfThreads},{testIterations}");
                labelAsmTestResult.Text = "saved to csv";
            }
        }

        public void SaveBitmapAsBmp(Bitmap bitmap, string filePath) {
            try {
                bitmap.Save(filePath, ImageFormat.Bmp);
                labelAsmTestResult.Text = $"Obraz zapisano pod œcie¿k¹: {filePath}";
            } catch (Exception ex) {
                labelAsmTestResult.Text = $"B³¹d przy zapisywaniu obrazu: {ex.Message}";
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            numOfThreads = Environment.ProcessorCount;
            trackBar1.Value = numOfThreads;
            threadsLabel.Text = "Number of threads: " + numOfThreads.ToString();
        }
    }
}
