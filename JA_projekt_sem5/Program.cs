

using System.Runtime.InteropServices;

namespace JA_projekt_sem5
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        [DllImport(@"C:\Users\jemek\source\repos\Gaussian_blur_Ams_and_Cpp\x64\Debug\JAAsm.dll")]
        static extern int MyProc1();
        
        [STAThread]
        static void Main()
        {

            

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}