using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace JA_projekt_sem5 {
    internal class ImageProcessor {
        const string dllPath = "..\\..\\..\\..\\..\\x64\\Release\\"; //Debug Release

        [DllImport(dllPath + "C_functions.dll")]
        private static extern void createGaussianKernel(Byte kernelSize, float sigma, float[] kernel);

        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlur(byte[] bitmapData, byte[] tempBitmapData, float[] kernel,
                                             int width, int height, int stride, int kernelSize);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlurStage1(byte[] bitmapData, byte[] tempBitmapData, float[] kernel,
                                                    int width, int height, int stride, int kernelSize,
                                                    int startHeight, int endHeight);
        [DllImport(dllPath + "C_functions.dll")]
        private static extern void gaussBlurStage2(byte[] bitmapData, byte[] tempBitmapData, float[] kernel,
                                                    int width, int height, int stride, int kernelSize,
                                                    int startHeight, int endHeight);


        [DllImport(dllPath + "JAAsm.dll")]
        private static extern void createGaussianKernelAsm(Byte kernelSize, float sigma, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern void gaussBlurAsm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern void gaussBlurStage1Asm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel);
        [DllImport(dllPath + "JAAsm.dll")]
        private static extern void gaussBlurStage2Asm(byte[] bitmapData, int[] packedArguments, byte[] tempBitmapData, float[] kernel);


        private Byte kernelSize = 11;
        private float sigma = 4;

        public ImageProcessor() {}

        public ImageProcessor(Byte kernelSize, float sigma) {
            this.kernelSize = kernelSize;
            this.sigma = sigma;
        }

        public Byte getKernelSize() { return kernelSize; }

        public void updateKernelConfig(Byte newKernelSize, float newSigma) {
            if (newKernelSize > 0 && newSigma > 0) {
                kernelSize = (byte)(newKernelSize % 2 == 0 ? newKernelSize + 1 : newKernelSize); // force odd parity
                sigma = newSigma;
            }
        }

        public void updateKernelConfig(float radius) {
            sigma = radius / 3.0f;
            int temp = (int)(  ( 2 * Math.Ceiling(radius) ) + 1  );
            kernelSize = (byte)(temp > 255 ? 255 : temp);
        }

            public Bitmap applyGaussianBlurCpp(Bitmap bmpIn, int numOfThreads) {
            if (bmpIn != null) {
                Bitmap bitmapBlurred = new Bitmap(bmpIn);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapBlurred.LockBits(
                    new Rectangle(0, 0, bitmapBlurred.Width, bitmapBlurred.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                int height = bitmapBlurred.Height;
                int width = bitmapBlurred.Width;
                int stride = bmpData.Stride;

                // 1st stage saves output here and 2nd stage uses it as input
                byte[] tempCanvas = new byte[stride * height];

                // Working on buffer because of multithreading
                byte[] bmpDataBuffer = new byte[stride * height];
                Marshal.Copy(bmpData.Scan0, bmpDataBuffer, 0, bmpDataBuffer.Length);

                float[] kernel = new float[kernelSize];
                createGaussianKernel(kernelSize, sigma, kernel);

                if (numOfThreads == 1) {
                    gaussBlur(bmpDataBuffer, tempCanvas, kernel, width, height,
                            stride, kernelSize);
                } else {       
                    int segmentHeight = height / numOfThreads;

                    // Assign starting and ending values for each segment
                    Thread[] threads = new Thread[numOfThreads];
                    int[] startHeights = new int[numOfThreads];
                    int[] endHeights = new int[numOfThreads];

                    // Segment the image
                    for (int i = 0; i < numOfThreads; ++i) {
                        startHeights[i] = i * segmentHeight;
                        endHeights[i] = (i + 1) * segmentHeight;

                        if (i == numOfThreads - 1) { endHeights[i] = height; }
                    }

                    // Vertical blur
                    for (int i = 0; i < numOfThreads; ++i) {
                        int start = startHeights[i];
                        int end = endHeights[i];

                        threads[i] = new Thread(() => {
                            gaussBlurStage1(bmpDataBuffer, tempCanvas, kernel, width, height,
                                            stride, kernelSize, start, end);
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
                            gaussBlurStage2(bmpDataBuffer, tempCanvas, kernel, width, height,
                                            stride, kernelSize, start, end);
                        });
                        threads[i].Start();
                    }

                    // Wait for all horizontal blur threads to finish
                    foreach (var thread in threads) {
                        thread.Join();
                    }
                }

                // Copy processed data back to the bitmap
                Marshal.Copy(bmpDataBuffer, 0, bmpData.Scan0, bmpDataBuffer.Length);
                bitmapBlurred.UnlockBits(bmpData);
                return bitmapBlurred;
            } else {
                return null;
            }
        }

        public Bitmap applyGaussianBlurAsm(Bitmap bmp, int numOfThreads) {
            if (bmp != null) {
                Bitmap bitmapBlurred = new Bitmap(bmp);

                // Lock the bitmap's bits to get access to its pixel data
                BitmapData bmpData = bitmapBlurred.LockBits(
                    new Rectangle(0, 0, bitmapBlurred.Width, bitmapBlurred.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                int height = bitmapBlurred.Height;
                int width = bitmapBlurred.Width;
                int stride = bmpData.Stride;

                // 1st stage saves output here and 2nd stage uses it as input
                byte[] tempCanvas = new byte[stride * height];

                // Working on buffer because of multithreading
                byte[] bmpDataBuffer = new byte[stride * height];
                Marshal.Copy(bmpData.Scan0, bmpDataBuffer, 0, bmpDataBuffer.Length);

                float[] kernel = new float[kernelSize];
                createGaussianKernelAsm(kernelSize, sigma, kernel);

                if (numOfThreads == 1) {
                    int[] gaussBlurAsmArguments = new int[5];
                    gaussBlurAsmArguments[0] = width;
                    gaussBlurAsmArguments[1] = height;
                    gaussBlurAsmArguments[2] = stride;
                    gaussBlurAsmArguments[3] = kernelSize;

                    gaussBlurAsm(bmpDataBuffer, gaussBlurAsmArguments, tempCanvas, kernel);
                } else {
                    int segmentHeight = height / numOfThreads;

                    // Assign starting and ending values for each segment
                    Thread[] threads = new Thread[numOfThreads];
                    int[] startHeights = new int[numOfThreads];
                    int[] endHeights = new int[numOfThreads];

                    // Segment the image
                    for (int i = 0; i < numOfThreads; ++i) {
                        startHeights[i] = i * segmentHeight;
                        endHeights[i] = (i + 1) * segmentHeight;

                        if (i == numOfThreads - 1) { endHeights[i] = height; }
                    }

                    // Vertical blur
                    for (int i = 0; i < numOfThreads; ++i) {
                        int start = startHeights[i];
                        int end = endHeights[i];

                        int[] gaussBlurAsmArguments = new int[5];
                        gaussBlurAsmArguments[0] = width;
                        gaussBlurAsmArguments[1] = end;
                        gaussBlurAsmArguments[2] = stride;
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
                        gaussBlurAsmArguments[0] = width;
                        gaussBlurAsmArguments[1] = height;
                        gaussBlurAsmArguments[2] = stride;
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
                }

                // Copy processed data back to the bitmap
                Marshal.Copy(bmpDataBuffer, 0, bmpData.Scan0, bmpDataBuffer.Length);

                bitmapBlurred.UnlockBits(bmpData);
                return bitmapBlurred;
            } else {
                return null;
            }
        }
    }
}
