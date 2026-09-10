using System;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private CheckBox _startupCheckBox;
        private bool _readingStartup;
        private CheckBox _edgeSwipeCheckBox;
        private bool _readingEdgeSwipe;

        private void InitializeEdgeSwipeOption()
        {
            _edgeSwipeCheckBox = new CheckBox
            {
                Text = UIText.BlockEdgeSwipes,
                AutoSize = true,
                Location = new System.Drawing.Point(13, 106),
                TabIndex = 3
            };
            grpUtility.Controls.Add(_edgeSwipeCheckBox);
            _edgeSwipeCheckBox.CheckedChanged += (sender, args) =>
            {
                if (_readingEdgeSwipe) return;
                try
                {
                    EdgeSwipePolicy.SetBlocked(_edgeSwipeCheckBox.Checked);
                    MessageBox.Show(this,
                        UIText.RestartMessage +
                        UIText.PersistentSettingMessage,
                        UIText.RestartTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, UIText.EdgeSwipesSaveFailed + ex.Message,
                        UIText.EdgeSwipesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                RefreshEdgeSwipeOption();
            };
            RefreshEdgeSwipeOption();
        }

        private void RefreshEdgeSwipeOption()
        {
            _readingEdgeSwipe = true;
            try
            {
                _edgeSwipeCheckBox.Checked = EdgeSwipePolicy.IsBlocked();
                _edgeSwipeCheckBox.Enabled = true;
                _edgeSwipeCheckBox.Text = UIText.BlockEdgeSwipes;
            }
            catch (Exception ex)
            {
                _edgeSwipeCheckBox.CheckState = CheckState.Indeterminate;
                _edgeSwipeCheckBox.Enabled = false;
                _edgeSwipeCheckBox.Text = UIText.EdgeSwipesUnavailable;
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
            finally { _readingEdgeSwipe = false; }
        }

        private void InitializeStartupOption()
        {
            // Initialize this settings group once, including persistent edge-swipe settings.
            if (_startupCheckBox != null) return;

            _startupCheckBox = new CheckBox
            {
                Text = UIText.StartWithWindows,
                AutoSize = true,
                Location = new System.Drawing.Point(13, 22),
                TabIndex = 0
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
                    MessageBox.Show(this, UIText.StartupSaveFailed + ex.Message,
                        UIText.StartupTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                RefreshStartupOption();
            };
            RefreshStartupOption();
            InitializeEdgeSwipeOption();
        }

        private void RefreshStartupOption()
        {
            _readingStartup = true;
            try
            {
                using (var task = new StartupTask()) _startupCheckBox.Checked = task.IsEnabled();
                _startupCheckBox.Enabled = true;
                _startupCheckBox.Text = UIText.StartWithWindows;
            }
            catch (Exception ex)
            {
                _startupCheckBox.CheckState = CheckState.Indeterminate;
                _startupCheckBox.Enabled = false;
                _startupCheckBox.Text = UIText.StartupUnavailable;
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
            finally { _readingStartup = false; }
        }

    }
}
