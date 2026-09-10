using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication : Form
    {
        private KeyboardHook _keyboardHook;
        private ServicesWatchDog _watchDog;
        private ExitHotCorners _exitHotCorners;
        private AppConfig _config;
        /// <summary>
        /// When true, form can show (after exit); blocked while restricted.
        /// </summary>
        private bool _allowShowForm;
        private bool _initialLoadDone;
        /// <summary>
        /// When true, password dialog is already visible; ignore further hotkey/hot corner until it closes.
        /// </summary>
        private bool _passwordDialogShowing;
        private static readonly string[] ExitKeyNames = { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "Escape", "Tab", "Pause" };

        public RestrictedModeApplication()
        {
            InitializeComponent();
            ApplyModernLayout();
            ShowInTaskbar = false;
            _allowShowForm = false;
            RestrictedState.EnterRestrictedMode();
            CreateHandle();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!_initialLoadDone)
                BeginInvoke(new Action(InitialLoadAndStartRestricted));
        }

        protected override void SetVisibleCore(bool value)
        {
            if (value && RestrictedState.IsRestrictedMode && !_allowShowForm)
                value = false;
            base.SetVisibleCore(value);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateControl();
            InitialLoadAndStartRestricted();
        }

        /// <summary>
        /// Runs once; form may run hidden so OnLoad might not run.
        /// </summary>
        private void InitialLoadAndStartRestricted()
        {
            if (_initialLoadDone) return;
            _initialLoadDone = true;

            foreach (var k in ExitKeyNames)
                cboExitKey.Items.Add(k);
            cboExitKey.SelectedIndex = 0;

            cboHotCornerPosition.Items.Clear();
            cboHotCornerPosition.Items.Add("Top left");
            cboHotCornerPosition.Items.Add("Top right");
            cboHotCornerPosition.Items.Add("Bottom left");
            cboHotCornerPosition.Items.Add("Bottom right");
            cboHotCornerPosition.SelectedIndex = 0;

            _config = ConfigManager.Load();
            if (!File.Exists(ConfigManager.ConfigPath))
                ConfigManager.Save(ConfigManager.GetDefault());

            ConfigToUI(_config);
            InitializeStartupOption();
            InitializeAutoSave();
            RestrictedState.RestrictedModeExited += OnRestrictedModeExited;
            RestrictedState.ExitRestrictedRequested += OnExitRestrictedRequested;

            StartRestricted();
        }

        private void StartRestricted()
        {
            _allowShowForm = false;
            RestrictedState.ApplyExitHotkeyConfig(_config.ExitHotkey);
            RestrictedState.EnterRestrictedMode();

            if (_keyboardHook == null)
                _keyboardHook = new KeyboardHook();
            _keyboardHook.Install();
            TaskManagerPolicy.Disable();
            bool edgePolicyApplied = EdgeSwipePolicy.Disable();
            EdgeSwipePolicy.CloseFolderWindows();
            string edgePolicyError = EdgeSwipePolicy.LastError;

            _watchDog?.Stop();
            _watchDog = new ServicesWatchDog { CheckIntervalMs = _config.WatchDog?.CheckIntervalMs ?? 5000 };
            if (_config.WatchDog?.Processes != null)
            {
                foreach (var p in _config.WatchDog.Processes)
                {
                    if (!string.IsNullOrWhiteSpace(p?.ExePath))
                        _watchDog.AddProcess(p.ExePath, p.Arguments, p.WorkingDirectory);
                }
            }
            _watchDog.Start();

            if (_exitHotCorners == null)
                _exitHotCorners = new ExitHotCorners(this);
            _exitHotCorners.Start(_config.ExitHotCornerEnabled, _config.ExitHotCornerCorner, _config.ExitHotCornerSizePx);

            if (_config.UtilityHideStartMenu)
                StartMenuPolicy.Hide();
            if (_config.UtilityHideTaskbar)
                TaskbarPolicy.Hide();

            Hide();
            if (!edgePolicyApplied || !string.IsNullOrEmpty(edgePolicyError))
            {
                MessageBox.Show(
                    "Restricted mode protection is incomplete. Windows edge gestures may still work.\n\n" +
                    "Run RestrictedMode as Administrator under the kiosk account, then retry. " +
                    "Reboot after installing policies.\n\n" + edgePolicyError +
                    "\n\nLog (if writable): " + EdgeSwipePolicy.LogPath,
                    "Restricted mode policy error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void OnExitRestrictedRequested()
        {
            if (IsDisposed || Disposing || _passwordDialogShowing) return;
            // Return from the low-level keyboard hook before entering a modal loop.
            _passwordDialogShowing = true;
            BeginInvoke(new Action(ShowExitPasswordDialog));
        }

        private void ShowExitPasswordDialog()
        {
            try { ShowExitPasswordDialogCore(); }
            finally { _passwordDialogShowing = false; }
        }

        private void ShowExitPasswordDialogCore()
        {
            string requiredPassword = string.IsNullOrWhiteSpace(_config?.RestrictedPassword) ? null : _config.RestrictedPassword;
            if (requiredPassword == null)
            {
                RestrictedState.ConfirmExitRestricted();
                return;
            }
            _exitHotCorners?.Stop();
            _watchDog?.Stop();
            try
            {
                using (var dlg = new PasswordDialogForm("Exit Restricted Mode", "Enter password to exit Restricted Mode:"))
                {
                    dlg.ExpectedPassword = requiredPassword;
                    dlg.TopMost = true;
                    dlg.StartPosition = FormStartPosition.CenterScreen;
                    dlg.ShowInTaskbar = false;
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;
                    RestrictedState.ConfirmExitRestricted();
                }
            }
            finally
            {
                if (RestrictedState.IsRestrictedMode && !IsDisposed && !Disposing)
                {
                    _watchDog?.Start();
                    _exitHotCorners?.Start(_config.ExitHotCornerEnabled,
                        _config.ExitHotCornerCorner, _config.ExitHotCornerSizePx);
                }
            }
        }

        private void OnRestrictedModeExited()
        {
            RefreshStartupOption();
            _exitHotCorners?.Stop();
            _keyboardHook?.Uninstall();
            TaskManagerPolicy.Enable();
            if (_config != null)
            {
                if (_config.UtilityHideTaskbar)
                    TaskbarPolicy.Show();
                if (_config.UtilityHideStartMenu)
                    StartMenuPolicy.Show();
            }
            RestoreEdgePolicies();
            _watchDog?.Stop();
            _allowShowForm = true;
            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            BringToFront();
        }

        private void RestoreEdgePolicies()
        {
            if (!EdgeSwipePolicy.Enable())
                MessageBox.Show("Could not restore Windows policies.\n\n" + EdgeSwipePolicy.LastError +
                    "\n\nLog: " + EdgeSwipePolicy.LogPath, "Policy restore error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            RestrictedState.RestrictedModeExited -= OnRestrictedModeExited;
            RestrictedState.ExitRestrictedRequested -= OnExitRestrictedRequested;
            TaskManagerPolicy.Enable();
            TaskbarPolicy.Show();
            StartMenuPolicy.Show();
            RestoreEdgePolicies();
            _watchDog?.Stop();
            UIToConfig();
            if (_config != null)
                ConfigManager.Save(_config);
            _keyboardHook?.Dispose();
            _exitHotCorners?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
