using System;
using System.Windows;
using System.Windows.Forms;

namespace WSLSessionManager
{
    internal class ManagerAppContext : ApplicationContext, IDisposable
    {
        private ProcessManager processManager = null;
        private NotifyIconManager notifyIconMananger = null;
        private Wow64FsRedirectionDisabler wow64FsRedirectionDisabler = null;

        public ManagerAppContext() : base()
        {
            wow64FsRedirectionDisabler = new Wow64FsRedirectionDisabler();

            notifyIconMananger = new NotifyIconManager();
            notifyIconMananger.ContextMenu_Exit.Click += ContextMenu_Exit_Click;
            notifyIconMananger.ContextMenu_Settings.Click += ContextMenu_Settings_Click;
            notifyIconMananger.NotifyIcon.Visible = true;

            processManager = BuildProcessManager();
            processManager.Crashed += ProcessManager_Crashed;
            processManager.Start();
        }

        private static ProcessManager BuildProcessManager()
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = Properties.Settings.Default.CommandExecutable,
                Arguments = Properties.Settings.Default.CommandArguments,
                CreateNoWindow = true
            };
            if (startInfo.FileName == string.Empty)
            {
                throw new Exception("Command executable path is empty");
            }

            return new ProcessManager(startInfo);
        }

        private void ProcessManager_Crashed(object sender, EventArgs e)
        {
            if (notifyIconMananger != null && notifyIconMananger.NotifyIcon != null)
            {
                notifyIconMananger.NotifyIcon.ShowBalloonTip(15, "Crash", "Process crashed", System.Windows.Forms.ToolTipIcon.Warning);
            }
            else
            {
                throw new Exception("Process crashed and notification icon was not ready to display the message.");
            }
        }

        private void ContextMenu_Exit_Click(object sender, EventArgs e)
        {
            processManager.Stop();
            ExitThread();
        }

        private void ContextMenu_Settings_Click(object sender, EventArgs e)
        {
            if (notifyIconMananger?.NotifyIcon != null)
            {
                var message = "Settings are applied when manager starts.\n" +
                                "Restart WSL Session Manager to apply new settings.";
                notifyIconMananger.NotifyIcon.ShowBalloonTip(20000, "Relaunch required to apply settings", message, ToolTipIcon.Info);
            }
            System.Diagnostics.Process.Start(Application.ExecutablePath, "/settings");
        }
    }
}
