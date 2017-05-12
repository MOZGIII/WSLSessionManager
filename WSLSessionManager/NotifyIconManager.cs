using System;
using System.Windows.Forms;

namespace WSLSessionManager
{
    internal class NotifyIconManager : IDisposable
    {
        public NotifyIcon NotifyIcon { get; private set; }
        public ContextMenuStrip ContextMenu { get; private set; }
        public ToolStripMenuItem ContextMenu_Settings { get; private set; }
        public ToolStripMenuItem ContextMenu_Exit { get; private set; }

        public NotifyIconManager()
        {
            ContextMenu = new ContextMenuStrip();
            ContextMenu_Settings = new ToolStripMenuItem() { Text = "Settings" };
            ContextMenu_Exit = new ToolStripMenuItem() { Text = "Exit" };

            ContextMenu.SuspendLayout();
            ContextMenu.Items.AddRange(new ToolStripItem[] { ContextMenu_Settings, ContextMenu_Exit });
            ContextMenu.ResumeLayout(false);

            NotifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.Icon,
                Text = Application.ProductName,
                ContextMenuStrip = ContextMenu
            };
        }

        public void Dispose()
        {
            if (NotifyIcon != null)
            {
                // Despite we're just hiding the icon here, we're really removing it from
                // the tray bar, hence real disposure happends. Notify icon is indeed a
                // sort of unmanaged resource.
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                ContextMenu.Dispose();
            }
        }
    }
}
