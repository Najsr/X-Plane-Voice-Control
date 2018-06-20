using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace X_Plane_Voice_Control
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] params_)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (params_.Length > 0 && params_[0].Equals("--console", StringComparison.InvariantCultureIgnoreCase))
            {
                AllocConsole();
                Console.SetOut(new PrefixedWriter());
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine(@"Console Initialized");
                Console.Title = @"X-Plane Voice Control made by Nicer";
            }

            Application.Run(new MainForm());
        }

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern bool AllocConsole();

    }
}
