using System;
using System.Windows.Forms;

namespace WSLSessionManager
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var p = ParseOptions(args);
            if (p.LaunchMode == LaunchMode.Settings)
            {
                Application.Run(new SettingsAppContext());
            }
            else
            {
                Application.Run(new ManagerAppContext());
            }
        }

        public enum LaunchMode { Manager = 0x0, Settings = 0x1 }

        private struct LaunchParams
        {
            public LaunchMode LaunchMode;
        }

        private static LaunchParams ParseOptions(string[] args)
        {
            var p = new LaunchParams();
            foreach (string arg in args)
            {
                if (arg == "/settings" || arg == "--settings" || arg == "-s")
                {
                    p.LaunchMode = LaunchMode.Settings;
                }
            }
            return p;
        }
    }
}
