using System;
using System.Drawing;
using System.Windows.Forms;
namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private Label _saveStatus;
        private Label _passwordDirtyIndicator;
        private Panel _passwordInputBorder;
        private bool _autoSaveReady;
        private void InitializeAutoSave()
        {
            // Attach only after initial configuration has populated all controls.
            foreach (var check in new[] { chkCtrl, chkShift, chkAlt, chkAlsoAllowDefaultHotkey,
                // chkAlsoAllowDefaultPassword, // Restore with the default-password UI.
                chkHotCornerEnabled, chkHideTaskbar, chkHideStartMenu })
                check.CheckedChanged += AutoSaveChanged;
            txtRestrictedPassword.TextChanged += PasswordInputChanged;
            cboExitKey.SelectedIndexChanged += AutoSaveChanged;
            cboHotCornerPosition.SelectedIndexChanged += AutoSaveChanged;
            numHotCornerSize.ValueChanged += AutoSaveChanged;
            numIntervalSeconds.ValueChanged += AutoSaveChanged;
            _autoSaveReady = true;
            UpdatePasswordDirtyIndicator();
        }
        private void AutoSaveChanged(object sender, EventArgs args)
        { if (_autoSaveReady) SaveChanges(); }
        private bool SaveChanges()
        {
            UIToConfig();
            return PersistConfig();
        }
        private bool SavePasswordChange()
        {
            UIToConfig();
            string previousPassword = _config.RestrictedPassword;
            _config.RestrictedPassword = GetPasswordInput();
            bool saved = PersistConfig();
            if (!saved)
                _config.RestrictedPassword = previousPassword;
            UpdatePasswordDirtyIndicator();
            return saved;
        }
        private string GetPasswordInput()
        {
            return string.IsNullOrWhiteSpace(txtRestrictedPassword.Text)
                ? null
                : txtRestrictedPassword.Text.Trim();
        }
        private void PasswordInputChanged(object sender, EventArgs args)
        {
            if (_autoSaveReady) UpdatePasswordDirtyIndicator();
        }
        private void UpdatePasswordDirtyIndicator()
        {
            if (_passwordDirtyIndicator == null) return;
            bool hasUnsavedChange = !string.Equals(
                GetPasswordInput(), _config?.RestrictedPassword, StringComparison.Ordinal);
            _passwordDirtyIndicator.Visible = hasUnsavedChange;
            if (_passwordInputBorder != null)
                _passwordInputBorder.BackColor = hasUnsavedChange
                    ? Color.Firebrick
                    : Color.FromArgb(148, 163, 184);
        }
        private bool PersistConfig()
        {
            string error;
            bool saved = ConfigManager.TrySave(_config, out error);
            _saveStatus.Visible = !saved;
            _saveStatus.Text = saved ? "" : UIText.SaveFailed;
            _saveStatus.ForeColor = saved ? Color.FromArgb(100, 116, 139) : Color.Firebrick;
            if (!saved) System.Diagnostics.Trace.TraceError(error);
            return saved;
        }
    }
}
