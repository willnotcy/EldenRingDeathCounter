using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EldenRingDeathCounter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NativeMethods.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static class NativeMethods
        {
            [DllImport("user32")]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            public static extern bool SetProcessDPIAware();
        }
    }
}
