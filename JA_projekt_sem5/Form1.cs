using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

//TODO 
namespace JA_projekt_sem5 {

    public partial class Form1 : Form {
        const string dllPath = "C:\\Users\\Robert\\source\\repos\\Gaussian_blur_Ams_and_Cpp\\x64\\Debug\\"; //Debug Release
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void ProcessBitmap(IntPtr bitmapData, int width, int height, int stride);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlur(IntPtr bitmapData, int width, int height, int stride, int kernelSize, float sigma);

        [DllImport(dllPath + "JAAsm.dll")]
        private static extern float createGaussianKernel(Byte kernelSize, float sigma, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern int gaussBlurAsm(IntPtr bitmapData, int[] packedArguments, IntPtr tempBitmapData, float[] kernel); //TODO into void
                                                                                                         //        private static extern long gaussBlurAsm(IntPtr bitmapData, int width, int height, int stride, int kernelSize, float sigma);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern long ProcessBitmapAsm(IntPtr bitmapData, int width, int height, int stride);

        private Bitmap bitmap;
        private Bitmap bitmapOutput;

        private OpenFileDialog openFileDialog;

        private Byte kernelSize = 11;
        private float sigma = 4;

        private long testTimeCpp = 0;
        private long testTimeAsm = 0;
        private int testIterations = 1;

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
            if (bitmap != null) {
                bitmapOutput = new Bitmap(bitmap);
                Bitmap copyBitmap = new Bitmap(bitmap);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapOutput.LockBits(
                    new Rectangle(0, 0, bitmapOutput.Width, bitmapOutput.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                if(radioButtonAsm.Checked) {
                    float[] kernel = new float[kernelSize];

                    int[] gaussBlurAsmArguments = new int[4];
                    gaussBlurAsmArguments[0] = bitmapOutput.Width;
                    gaussBlurAsmArguments[1] = bitmapOutput.Height;
                    gaussBlurAsmArguments[2] = bmpData.Stride;
                    gaussBlurAsmArguments[3] = kernelSize;

                    

                    BitmapData copyBmpData = copyBitmap.LockBits(
                        new Rectangle(0, 0, copyBitmap.Width, copyBitmap.Height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                    float a = createGaussianKernel(kernelSize, sigma, kernel);
                    labelAsmTestResult.Text = $"retVal = \n {kernel}";
                    int testRet = gaussBlurAsm(bmpData.Scan0, gaussBlurAsmArguments, copyBmpData.Scan0, kernel);
                    labelAsmTestResult.Text = $"retVal = {testRet} | {kernelSize} | {sigma}  \n {string.Join(", ", kernel)}";

                    copyBitmap.UnlockBits(copyBmpData);
                }

                if (radioButtonCpp.Checked) {
                    gaussBlur(bmpData.Scan0, bitmapOutput.Width, bitmapOutput.Height, bmpData.Stride, kernelSize, sigma);
                    //ProcessBitmap(bmpData.Scan0, bitmap.Width, bitmap.Height, bmpData.Stride);
                }


                bitmapOutput.UnlockBits(bmpData);
                //DisplayImage(copyBitmap, processedPicture);
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
            float a = createGaussianKernel(kernelSize, sigma, tabK);

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

                    testTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapSmall);
                    stopwatch.Stop();

                    testTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                counter = testIterations;
            } 
            if (checkBoxMedium.Checked) {
                while (counter > 0) {
                    --counter;
                    stopwatch.Start();
                    applyGaussianBlurCpp(bitmapMedium);
                    stopwatch.Stop();

                    testTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapMedium);
                    stopwatch.Stop();

                    testTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                counter = testIterations;
            } 
            if (checkBoxBig.Checked) {
                while (counter > 0) {
                    --counter;
                    stopwatch.Start();
                    applyGaussianBlurCpp(bitmapBig);
                    stopwatch.Stop();

                    testTimeCpp += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    //////////////////////

                    stopwatch.Start();
                    applyGaussianBlurAsm(bitmapBig);
                    stopwatch.Stop();

                    testTimeAsm += stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                }
                counter = testIterations;
            }


            timeCppLabel.Text = ConvertMillisecondsToTimeFormat(testTimeCpp);
            timeAsmLabel.Text = ConvertMillisecondsToTimeFormat(testTimeAsm);
            testTimeCpp = 0;
            testTimeAsm = 0;
        }

        private void applyGaussianBlurCpp(Bitmap bmp) {
            BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

            gaussBlur(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, kernelSize, sigma);

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

            float a = createGaussianKernel(kernelSize, sigma, kernel);
            int testRet = gaussBlurAsm(bmpData.Scan0, gaussBlurAsmArguments, copyBmpData.Scan0, kernel);

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
            labelAsmTestResult.Text = "Threads: " + trackBar1.Value.ToString();
        }

        private void iterationsTextBox_TextChanged(object sender, EventArgs e) {
            try {
                testIterations = Convert.ToInt32(iterationsTextBox.Text);
                labelXTimes.Text = $"Repeat {testIterations} times";
            } catch (FormatException ex) {
                labelXTimes.Text = labelXTimes.Text + " Incorrect format!";
            }
        }
    }
}
