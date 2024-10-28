

using System.Runtime.InteropServices;

namespace JA_projekt_sem5
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        [DllImport(@"C:\Users\jemek\source\repos\Gaussian_blur_Ams_and_Cpp\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);
        
        [STAThread]
        static void Main()
        {
            //dont touch this code XD
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