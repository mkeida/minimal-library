using Minimal;
using System;
using System.Windows.Forms;

namespace Demo
{
    static class Program
    {
        //ADD THESE TWO LINES !!
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Enables minimal library
            M.EnableMinimal();

            Application.Run(new Main());
        }
    }
}
