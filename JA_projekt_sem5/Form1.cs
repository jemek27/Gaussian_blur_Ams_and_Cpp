using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace JA_projekt_sem5 {

    public partial class Form1 : Form {
        [DllImport(@"C:\Users\jemek\source\repos\JA_projekt_sem5\x64\Debug\C_functions.dll")]
        private static extern void ProcessBitmap(IntPtr bitmapData, int width, int height, int stride);
        [DllImport(@"C:\Users\jemek\source\repos\JA_projekt_sem5\x64\Debug\C_functions.dll")]
        private static extern void gaussBlur(IntPtr bitmapData, int width, int height, int stride, int kernelSize, float sigma);

        [DllImport(@"C:\Users\jemek\source\repos\JA_projekt_sem5\x64\Debug\JAAsm.dll")]
        private static extern long gaussBlurAsm(IntPtr bitmapData, int width, int height, int stride, int kernelSize, float sigma);

        private Bitmap bitmap;

        private OpenFileDialog openFileDialog;

        private Byte kernelSize = 11;
        private float sigma = 4;

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

        private void button2_Click(object sender, EventArgs e) {

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                bitmap = ConvertImageToBitmap(openFileDialog.FileName);
                DisplayImage(bitmap, inputPicture);
            }
        }

        private void processPictureButton_Click(object sender, EventArgs e) {
            if (bitmap != null) {
                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                long testRet = gaussBlurAsm(bmpData.Scan0, bitmap.Width, bitmap.Height, bmpData.Stride, kernelSize, sigma);
                labelAsmTestResult.Text = $"retVal = {testRet} | {bmpData.Scan0}";
                //gaussBlur(bmpData.Scan0, bitmap.Width, bitmap.Height, bmpData.Stride, kernelSize, sigma);
                //ProcessBitmap(bmpData.Scan0, bitmap.Width, bitmap.Height, bmpData.Stride);
                bitmap.UnlockBits(bmpData);
                DisplayImage(bitmap, processedPicture);
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
        [DllImport(@"C:\Users\jemek\source\repos\JA_projekt_sem5\x64\Debug\JAAsm.dll")]

        private static extern float expAsm(float x);//, double[] tabX, int[] tabN);
        [DllImport(@"C:\Users\jemek\source\repos\JA_projekt_sem5\x64\Debug\JAAsm.dll")]

        private static extern float createGaussianKernel(Byte kernelSize, float sigma, float[] kernel);

        private void button1_Click(object sender, EventArgs e) {
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
    }
}
