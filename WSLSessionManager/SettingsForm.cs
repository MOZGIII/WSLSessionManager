using System.Windows.Forms;

namespace WSLSessionManager
{
    internal class SettingsForm : Form
    {
        private TableLayoutPanel layout;
        private Button saveButton;
        private SettingsControl settingsControl;

        public SettingsForm()
        {
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Text = Application.ProductName;
            BackColor = System.Drawing.Color.White;
            Icon = Properties.Resources.Icon;
            Height = 250;
            Width = 500;

            layout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 0,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent) { Height = 100 });
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            settingsControl = new SettingsControl()
            {
                Dock = DockStyle.Fill,
                AutoScaleMode = AutoScaleMode.Inherit,
                Settings = Properties.Settings.Default
            };

            saveButton = new Button()
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Right),
                TabIndex = 1,
                TabStop = true,
                Text = "Save"
            };
            saveButton.Click += SaveButton_Click;

            SuspendLayout();

            layout.Controls.Add(settingsControl, 0, 0);
            layout.Controls.Add(saveButton, 0, 1);

            Controls.Add(layout);

            ResumeLayout(true);
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
