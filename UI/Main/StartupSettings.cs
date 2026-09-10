using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private CheckBox _startupCheckBox;
        private bool _readingStartup;

        private void InitializeStartupOption()
        {
            _startupCheckBox = new CheckBox
            {
                Text = "Start with Windows",
                AutoSize = true,
                Location = new System.Drawing.Point(250, 22),
                TabIndex = 2
            };
            grpUtility.Controls.Add(_startupCheckBox);
            _startupCheckBox.CheckedChanged += (sender, args) =>
            {
                if (_readingStartup) return;
                try
                {
                    using (var task = new StartupTask()) task.SetEnabled(_startupCheckBox.Checked);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not update Windows startup: " + ex.Message,
                        "Windows startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                RefreshStartupOption();
            };
            RefreshStartupOption();
        }

        private void RefreshStartupOption()
        {
            _readingStartup = true;
            try
            {
                using (var task = new StartupTask()) _startupCheckBox.Checked = task.IsEnabled();
                _startupCheckBox.Enabled = true;
                _startupCheckBox.Text = "Start with Windows";
            }
            catch (Exception ex)
            {
                _startupCheckBox.CheckState = CheckState.Indeterminate;
                _startupCheckBox.Enabled = false;
                _startupCheckBox.Text = "Startup status unavailable";
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
            finally { _readingStartup = false; }
        }

    }
}
