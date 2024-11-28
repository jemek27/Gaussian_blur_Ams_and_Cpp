using System.Diagnostics;


namespace JA_projekt_sem5 {
    public partial class Form1 : Form {

        private long totalTestTimeCpp = 0;
        private long totalTestTimeAsm = 0;
        private long tempTestTimeCpp = 0;
        private long tempTestTimeAsm = 0;
        private int testIterations = 1;
        private int testStartThreadsNum = 1;
        private int testEndThreadsNum = 64;


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
    }
}
