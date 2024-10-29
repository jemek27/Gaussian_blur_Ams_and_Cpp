using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

//TODO push used registers in asm
namespace JA_projekt_sem5 {

    public partial class Form1 : Form {
        const string dllPath = "..\\..\\..\\..\\..\\x64\\Debug\\"; //Debug Release

        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlur(   IntPtr bitmapData, byte[] tempBitmapData, double[] kernel, 
                                                int width, int height, int stride, int kernelSize,
                                                int startHeight, int endHeight);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void createGaussianKernel(Byte kernelSize, float sigma, double[] kernel);


        [DllImport(dllPath + "JAAsm.dll")]
        private static extern float createGaussianKernelAsm(Byte kernelSize, float sigma, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern int gaussBlurAsm(IntPtr bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel); //TODO into void


        private Bitmap bitmap;
        private Bitmap bitmapOutput;

        private OpenFileDialog openFileDialog;

        private Byte kernelSize = 11;
        private float sigma = 4;

        private long    totalTestTimeCpp = 0;
        private long    totalTestTimeAsm = 0;
        private long    tempTestTimeCpp = 0;
        private long    tempTestTimeAsm = 0;
        private int     testIterations = 1;
        private Byte    numOfThreads = 2;

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

        private async void processPictureButton_Click(object sender, EventArgs e) {
            if (bitmap != null) {
                bitmapOutput = new Bitmap(bitmap);
                Bitmap copyBitmap = new Bitmap(bitmap);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapOutput.LockBits(
                    new Rectangle(0, 0, bitmapOutput.Width, bitmapOutput.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                // 1st stage saves output here and 2nd stage uses it as input
                byte[] tempCanvas = new byte[bitmapOutput.Width * bitmapOutput.Height * 3];



                int segmentHight = bitmapOutput.Height / numOfThreads;

                if (radioButtonAsm.Checked) {
                    float[] kernel = new float[kernelSize];

                    int[] gaussBlurAsmArguments = new int[4];
                    gaussBlurAsmArguments[0] = bitmapOutput.Width;
                    gaussBlurAsmArguments[1] = bitmapOutput.Height;
                    gaussBlurAsmArguments[2] = bmpData.Stride;
                    gaussBlurAsmArguments[3] = kernelSize;

                    float a = createGaussianKernelAsm(kernelSize, sigma, kernel);
                    labelAsmTestResult.Text = $"retVal = \n {kernel}";
                    int testRet = gaussBlurAsm(bmpData.Scan0, gaussBlurAsmArguments, tempCanvas, kernel);
                    labelAsmTestResult.Text = $"retVal = {testRet} | {kernelSize} | {sigma}  \n {string.Join(", ", kernel)}";

                }

                if (radioButtonCpp.Checked) {
                    double[] kernel = new double[kernelSize];
                    createGaussianKernel(kernelSize, sigma, kernel);

                    Task[] tasks = new Task[numOfThreads];

                    for (int i = 0; i < numOfThreads; ++i) {
                        int startHeight = i * segmentHight;
                        int endHeight = (i + 1) * segmentHight;

                        if (i == numOfThreads - 1) { endHeight = bitmapOutput.Height; }
                        //TODO enable 2nd stage after neighbour are ready 
                        tasks[i] = Task.Run(() =>
                        {
                            gaussBlur(bmpData.Scan0, tempCanvas, kernel, bitmapOutput.Width, bitmapOutput.Height,
                                      bmpData.Stride, kernelSize, startHeight, endHeight);
                        });

                        
                    }

                    await Task.WhenAll(tasks);
                }


                bitmapOutput.UnlockBits(bmpData);
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
            Bitmap bitmapSmall = ConvertImageToBitmap(  imgPath + "sum-ryba-900x450.bmp");
            Bitmap bitmapMedium = ConvertImageToBitmap( imgPath + "krolWod.bmp");
            Bitmap bitmapBig = ConvertImageToBitmap(    imgPath + "PXL_20240915_075912464.bmp");

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
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm);
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
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm);
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
                SaveToCsv(csvPath, tempTestTimeCpp, tempTestTimeAsm);
                totalTestTimeCpp += tempTestTimeCpp;
                totalTestTimeAsm += tempTestTimeAsm;
                tempTestTimeCpp = 0;
                tempTestTimeAsm = 0;
                counter = testIterations;
            }


            timeCppLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeCpp);
            timeAsmLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeAsm);
            totalTestTimeCpp = 0;
            totalTestTimeAsm = 0;
        }

        private void applyGaussianBlurCpp(Bitmap bmp) {
            BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

            Bitmap copyBitmap = new Bitmap(bmp);

            //TODO mar normal byte tab
            BitmapData copyBmpData = copyBitmap.LockBits(
                new Rectangle(0, 0, copyBitmap.Width, copyBitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            int endIndex = bmpData.Stride * bitmapOutput.Height * 3;
            int segmentuSize = endIndex / numOfThreads;

            double[] kernel = new double[kernelSize];
            createGaussianKernel(kernelSize, sigma, kernel);
            for (int i = 0; i < numOfThreads; ++i) {
                int startID = i * segmentuSize;
                int endID = (i + 1) * segmentuSize;

                if (i == numOfThreads - 1) { endID = endIndex; }
                //preparing data for this format 
                //int pixelIndex = y * stride + x * 3; // BMP uses 3 bytes per pixel (BGR format)
                //int startHeight = startID / bmpData.Stride;
                //int startWidth = (startID % bmpData.Stride) / 3;
                //int workingHeight = endID / bmpData.Stride;
                //int workingWidth = (endID % bmpData.Stride) / 3;


                //gaussBlur(bmpData.Scan0, copyBmpData.Scan0, kernel, bitmapOutput.Width, bitmapOutput.Height,
                //            bmpData.Stride, kernelSize, workingWidth, workingHeight, startWidth, startHeight);
            }

            bmp.UnlockBits(bmpData);
        }

        private void applyGaussianBlurAsm(Bitmap bmp) {
            Bitmap copyBitmap = new Bitmap(bmp);

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            float[] kernel = new float[kernelSize];

            int[] gaussBlurAsmArguments = new int[4];
            gaussBlurAsmArguments[0] = bmp.Width;
            gaussBlurAsmArguments[1] = bmp.Height;
            gaussBlurAsmArguments[2] = bmpData.Stride;
            gaussBlurAsmArguments[3] = kernelSize;

            BitmapData copyBmpData = copyBitmap.LockBits(
                new Rectangle(0, 0, copyBitmap.Width, copyBitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

            float a = createGaussianKernelAsm(kernelSize, sigma, kernel);
            //int testRet = gaussBlurAsm(bmpData.Scan0, gaussBlurAsmArguments, copyBmpData.Scan0, kernel);

            copyBitmap.UnlockBits(copyBmpData);
            bmp.UnlockBits(bmpData);
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
            labelAsmTestResult.Text = "Threads: " + numOfThreads;
        }

        private void iterationsTextBox_TextChanged(object sender, EventArgs e) {
            try {
                testIterations = Convert.ToInt32(iterationsTextBox.Text);
                labelXTimes.Text = $"Repeat {testIterations} times";
            } catch (FormatException ex) {
                labelXTimes.Text = labelXTimes.Text + " Incorrect format!";
            }
        }

        private void SaveToCsv(string filePath, long cppTime, long asmTime) {
            bool fileExists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, append: true)) {
                if (!fileExists) {
                    writer.WriteLine("Time-cpp-ms,Time-asm-ms,Threads,Number-of-iterations");
                }

                writer.WriteLine($"{cppTime},{asmTime},{numOfThreads},{testIterations}");
                labelAsmTestResult.Text = "saved to csv";
            }
        }
    }
}
