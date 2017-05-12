using System.Windows.Forms;

namespace WSLSessionManager
{
    internal class SettingsAppContext : ApplicationContext
    {
        public SettingsAppContext() : this(new SettingsForm())
        {
        }

        private SettingsAppContext(Form settingsForm) : base(settingsForm)
        {
        }
    }
}
