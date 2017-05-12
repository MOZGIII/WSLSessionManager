using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.ComponentModel;

namespace WSLSessionManager
{
    public class SettingsControl : UserControl
    {
        private class PropertyControlsContainer : Container
        {
            private Label label = null;
            private TextBox value = null;

            public PropertyControlsContainer()
            {
                InitializeComponent();
            }

            public Label Label { get => label; }
            public TextBox Value { get => value; }

            private void InitializeComponent()
            {
                label = new Label();
                value = new TextBox();

                Add(label);
                Add(value);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (settings != null)
                {
                    DisposeSettingsComponents();
                }
                layout.Dispose();
            }
            base.Dispose(disposing);
        }

        private ApplicationSettingsBase settings = null;
        private TableLayoutPanel layout = null;
        private OrderedDictionary settingsControls = null;

        public ApplicationSettingsBase Settings { get => settings; set => BindToSettings(value); }

        public SettingsControl()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            settingsControls = new OrderedDictionary();

            layout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                AutoScroll = true
            };

            Controls.Add(layout);
        }

        public void ClearSettingsBinding()
        {
            if (settings == null)
            {
                return;
            }
            DisposeSettingsComponents();
            settings = null;
        }

        public void BindToSettings(ApplicationSettingsBase newSettings)
        {
            ClearSettingsBinding();
            if (newSettings != null)
            {
                settings = newSettings;
                InitializeSettingsComponents(settings);
            }
        }

        public static string GetPropertyDescription(SettingsProperty property)
        {
            System.Type search = typeof(SettingsDescriptionAttribute);
            object attribute = property.Attributes[search];
            if (attribute == null) { return null; }
            return (attribute as SettingsDescriptionAttribute).Description;
        }

        private void InitializeSettingsComponents(ApplicationSettingsBase settings)
        {
            foreach (SettingsProperty currentProperty in settings.Properties)
            {
                PropertyControlsContainer container = new PropertyControlsContainer();
                settingsControls.Add(currentProperty.Name, container);

                string description = GetPropertyDescription(currentProperty);
                container.Label.Text = description ?? currentProperty.Name;
                container.Value.Text = settings.PropertyValues[currentProperty.Name]?.SerializedValue?.ToString() ?? "";
                container.Value.DataBindings.Add(new Binding("Text", settings, currentProperty.Name));
            }

            layout.SuspendLayout();
            layout.BackColor = new System.Drawing.Color();

            layout.RowCount = settingsControls.Values.Count + 1;

            foreach (PropertyControlsContainer container in settingsControls.Values)
            {
                container.Label.Dock = DockStyle.Fill;
                container.Label.AutoSize = true;
                container.Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                container.Value.Dock = DockStyle.Fill;

                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.Controls.AddRange(new Control[] { container.Label, container.Value });
            }

            layout.RowStyles.Add(new RowStyle(SizeType.Percent) { Height = 100 });

            layout.ResumeLayout(false);
            layout.PerformLayout();
        }

        private void DisposeSettingsComponents()
        {
            layout.SuspendLayout();
            layout.Controls.Clear();
            layout.RowStyles.Clear();
            layout.RowCount = 0;
            layout.ResumeLayout(false);
            layout.PerformLayout();

            foreach (PropertyControlsContainer container in settingsControls.Values)
            {
                container.Dispose();
            }
            settingsControls.Clear();
        }
    }
}
