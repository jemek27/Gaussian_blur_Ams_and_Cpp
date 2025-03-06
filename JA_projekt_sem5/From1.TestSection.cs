using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace JA_projekt_sem5 {
    public partial class Form1 : Form {

        const string basePath = "..\\..\\..\\..\\..\\";
        const string imgPath = basePath + "assets\\";
        const string csvPath = basePath + "test_results\\";
 

        private long totalTestTimeCpp = 0;
        private long totalTestTimeAsm = 0;
        private int testIterations = 1;
        private int testStartThreadsNum = 1;
        private int testEndThreadsNum = 64;

        private string lastDataFileName = "";


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
            string fileNameInfos = $"-K{imageProcessor.getKernelSize()}-I{testIterations}-S{testStartThreadsNum}-E{testEndThreadsNum}-D{checkBoxDoublingEachIter.Checked}.csv";
            string csvHistoryPath = csvPath + "testTimeHistory" + fileNameInfos;
            string csvTestTimePath = csvPath + "testTime" + fileNameInfos;

            Bitmap bitmapSmall = ConvertImageToBitmap(imgPath + "sum-ryba-900x450.bmp");
            Bitmap bitmapMedium = ConvertImageToBitmap(imgPath + "krolWod.bmp");
            Bitmap bitmapBig = ConvertImageToBitmap(imgPath + "PXL_20240915_075912464.bmp");

            Byte kernelSize = imageProcessor.getKernelSize();
            
            String csvDataBuffer = "";
            String csvDataBufferEachIteration = "";
            String testsPerformed = "";

            if (checkBoxSmall.Checked) { testsPerformed += "S"; }
            if (checkBoxMedium.Checked) { testsPerformed += "M"; }
            if (checkBoxBig.Checked) { testsPerformed += "B"; }

            int testNumOfThreads = testStartThreadsNum;

            // Select the appropriate lambda based on the checkbox state
            Action incrementAction = checkBoxDoublingEachIter.Checked
                ? () => testNumOfThreads *= 2
                : () => ++testNumOfThreads;

            labelStatus.Text = "Start Test";
            labelStatus.Refresh();

            bool testRunning = true;
            for (; testRunning; incrementAction()) {

                //last iteration
                if (testNumOfThreads >= testEndThreadsNum) {
                    testNumOfThreads = testEndThreadsNum;
                    testRunning = false;
                }

                int counter = testIterations;

                if (checkBoxSmall.Checked) {
                    runTest("small", counter, bitmapSmall, testNumOfThreads, ref csvDataBuffer, ref csvDataBufferEachIteration);
                }
                if (checkBoxMedium.Checked) {
                    runTest("medium", counter, bitmapMedium, testNumOfThreads, ref csvDataBuffer, ref csvDataBufferEachIteration);
                }
                if (checkBoxBig.Checked) {
                    runTest("big", counter, bitmapBig, testNumOfThreads, ref csvDataBuffer, ref csvDataBufferEachIteration);
                }
                csvDataBuffer += $"all,{totalTestTimeCpp},{totalTestTimeAsm},{testNumOfThreads},{testIterations}\n";

                totalTestTimeCpp = 0;
                totalTestTimeAsm = 0;
            }

            csvDataBuffer = csvDataBuffer.Remove(csvDataBuffer.Length - 1); // last \n is not needed

            SaveToCsv(csvPath + testsPerformed + fileNameInfos, csvDataBufferEachIteration, false);
            SaveToCsv(csvTestTimePath, csvDataBuffer, false);
            SaveToCsv(csvHistoryPath, csvDataBuffer);

            lastDataFileName = testsPerformed + fileNameInfos;
        }

        private void buttonGenerateChart_Click(object sender, EventArgs e) {
            labelStatus.Text = "Generating Chart";
            labelStatus.Refresh();
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = "python",
                    Arguments = $"{basePath}JA_projekt_sem5\\chart-generation.py {csvPath} {lastDataFileName}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (Process process = Process.Start(startInfo)) {
                    process.WaitForExit();

                    if (process.ExitCode == 0) { 
                        labelStatus.Text = $"Chart saved at: test_results\\{lastDataFileName}";
                    } else {
                        labelStatus.Text = "Error while executing script!";
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
                labelStatus.Text = "Error while executing chart generation";
            }
            labelStatus.Refresh();
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
                    writer.WriteLine("Size-name,Time-cpp-ms,Time-asm-ms,Threads,Iterations");
                }

                writer.WriteLine(dataRows);
                labelStatus.Text = "saved to csv";
            }
        }

        private void runTest(String sizeName, int counter, Bitmap bitmap, int testNumOfThreads, 
                             ref String csvDataBuffer, ref String csvDataBufferEachIteration) {
            Stopwatch stopwatch = new Stopwatch();
            long timeCpp = 0;
            long timeAsm = 0;
            long tempTestTimeCpp = 0;
            long tempTestTimeAsm = 0;

            while (counter > 0) {
                --counter;
                stopwatch.Start();
                imageProcessor.applyGaussianBlurCpp(bitmap, testNumOfThreads);
                stopwatch.Stop();

                timeCpp = stopwatch.ElapsedMilliseconds;
                tempTestTimeCpp += timeCpp;
                stopwatch.Restart();

                //////////////////////

                stopwatch.Start();
                imageProcessor.applyGaussianBlurAsm(bitmap, testNumOfThreads);
                stopwatch.Stop();

                timeAsm = stopwatch.ElapsedMilliseconds;
                tempTestTimeAsm += timeAsm;
                stopwatch.Restart();

                csvDataBufferEachIteration += $"{sizeName},{timeCpp},{timeAsm},{testNumOfThreads},{testIterations - counter} \n";

                labelStatus.Text = $"Test completed: {sizeName} image, iteration {counter}, threads = {testNumOfThreads}";
                labelStatus.Refresh();
            }
            csvDataBuffer += $"{sizeName},{tempTestTimeCpp},{tempTestTimeAsm},{testNumOfThreads},{testIterations}\n";
            totalTestTimeCpp += tempTestTimeCpp;
            totalTestTimeAsm += tempTestTimeAsm;
            tempTestTimeCpp = 0;
            tempTestTimeAsm = 0;
            counter = testIterations;
            timeCppLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeCpp);
            timeAsmLabel.Text = ConvertMillisecondsToTimeFormat(totalTestTimeAsm);
            timeCppLabel.Refresh();  // Force immediate label update
            timeAsmLabel.Refresh();
        }
    }
}
