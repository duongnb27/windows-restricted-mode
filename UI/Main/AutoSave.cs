using System;
using System.Drawing;
using System.Windows.Forms;
namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private Label _saveStatus;
        private bool _autoSaveReady;
        private void InitializeAutoSave()
        {
            // Attach only after initial configuration has populated all controls.
            foreach (var check in new[] { chkCtrl, chkShift, chkAlt, chkAlsoAllowDefaultHotkey,
                chkHotCornerEnabled, chkHideTaskbar, chkHideStartMenu })
                check.CheckedChanged += AutoSaveChanged;
            txtRestrictedPassword.TextChanged += AutoSaveChanged;
            cboExitKey.SelectedIndexChanged += AutoSaveChanged;
            cboHotCornerPosition.SelectedIndexChanged += AutoSaveChanged;
            numHotCornerSize.ValueChanged += AutoSaveChanged;
            numIntervalSeconds.ValueChanged += AutoSaveChanged;
            _autoSaveReady = true;
        }
        private void AutoSaveChanged(object sender, EventArgs args)
        { if (_autoSaveReady) SaveChanges(); }
        private bool SaveChanges()
        {
            UIToConfig();
            string error;
            bool saved = ConfigManager.TrySave(_config, out error);
            _saveStatus.Visible = !saved;
            _saveStatus.Text = saved ? "" : "Save failed. Check folder permissions and try again.";
            _saveStatus.ForeColor = saved ? Color.FromArgb(100, 116, 139) : Color.Firebrick;
            if (!saved) System.Diagnostics.Trace.TraceError(error);
            return saved;
        }
    }
}
