

using System.Runtime.InteropServices;

namespace JA_projekt_sem5
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        [DllImport(@"C:\Users\Robert\Downloads\Gaussian_blur_Ams_and_Cpp-5bd6c511ae9b01d538affa27a0b279b7c4800d91\Gaussian_blur_Ams_and_Cpp-5bd6c511ae9b01d538affa27a0b279b7c4800d91\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);
        
        [STAThread]
        static void Main()
        {
            int x = 5, y = 3;
            int retVal = MyProc1(x, y);
            Console.Write("Moja pierwsza wartosc obliczona w asm to:");
            Console.WriteLine(retVal);
            Console.ReadLine();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}