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
            RestrictedState.RestrictedModeExited += OnRestrictedModeExited;
            RestrictedState.ExitRestrictedRequested += OnExitRestrictedRequested;

            StartRestricted();
        }

        private void ConfigToUI(AppConfig c)
        {
            if (c == null) return;
            if (c.ExitHotkey != null)
            {
                chkCtrl.Checked = c.ExitHotkey.Ctrl;
                chkShift.Checked = c.ExitHotkey.Shift;
                chkAlt.Checked = c.ExitHotkey.Alt;
                var idx = cboExitKey.Items.IndexOf(c.ExitHotkey.Key ?? "F12");
                cboExitKey.SelectedIndex = idx >= 0 ? idx : 0;
            }
            txtRestrictedPassword.Text = c.RestrictedPassword ?? "";

            chkHotCornerEnabled.Checked = c.ExitHotCornerEnabled;
            var cornerIdx = Math.Max(0, Math.Min(3, c.ExitHotCornerCorner));
            if (cboHotCornerPosition.Items.Count >= 4)
                cboHotCornerPosition.SelectedIndex = cornerIdx;
            int sizePx = Math.Max(20, Math.Min(200, c.ExitHotCornerSizePx));
            numHotCornerSize.Value = sizePx;

            if (c.WatchDog != null)
            {
                numIntervalSeconds.Value = Math.Max(1, Math.Min(300, c.WatchDog.CheckIntervalMs / 1000));
                lstProcesses.Items.Clear();
                if (c.WatchDog.Processes != null)
                {
                    foreach (var p in c.WatchDog.Processes)
                    {
                        if (string.IsNullOrWhiteSpace(p?.ExePath)) continue;
                        var li = new ListViewItem(p.ExePath);
                        li.SubItems.Add(p.Arguments ?? "");
                        li.SubItems.Add(p.WorkingDirectory ?? "");
                        li.Tag = p;
                        lstProcesses.Items.Add(li);
                    }
                }
            }
        }

        private void UIToConfig()
        {
            if (_config == null) _config = new AppConfig();
            if (_config.ExitHotkey == null) _config.ExitHotkey = new ExitHotkeyConfig();
            _config.ExitHotkey.Ctrl = chkCtrl.Checked;
            _config.ExitHotkey.Shift = chkShift.Checked;
            _config.ExitHotkey.Alt = chkAlt.Checked;
            _config.ExitHotkey.Key = cboExitKey.SelectedItem?.ToString() ?? "F12";

            if (_config.WatchDog == null) _config.WatchDog = new WatchDogConfig();
            _config.WatchDog.CheckIntervalMs = (int)numIntervalSeconds.Value * 1000;
            var list = new List<WatchDogProcessConfig>();
            foreach (ListViewItem li in lstProcesses.Items)
            {
                var p = li.Tag as WatchDogProcessConfig;
                if (p != null)
                    list.Add(new WatchDogProcessConfig { ExePath = p.ExePath, Arguments = p.Arguments, WorkingDirectory = p.WorkingDirectory });
                else
                {
                    if (!string.IsNullOrWhiteSpace(li.Text))
                        list.Add(new WatchDogProcessConfig { ExePath = li.Text, Arguments = li.SubItems.Count > 0 ? li.SubItems[0].Text : "", WorkingDirectory = li.SubItems.Count > 1 ? li.SubItems[1].Text : "" });
                }
            }
            _config.WatchDog.Processes = list.ToArray();
            _config.RestrictedPassword = string.IsNullOrWhiteSpace(txtRestrictedPassword.Text) ? null : txtRestrictedPassword.Text.Trim();
            _config.ExitHotCornerEnabled = chkHotCornerEnabled.Checked;
            _config.ExitHotCornerCorner = cboHotCornerPosition.SelectedIndex >= 0 ? Math.Min(3, cboHotCornerPosition.SelectedIndex) : 0;
            _config.ExitHotCornerSizePx = (int)numHotCornerSize.Value;
        }

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            txtRestrictedPassword.UseSystemPasswordChar = !txtRestrictedPassword.UseSystemPasswordChar;
            btnShowPassword.Text = txtRestrictedPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UIToConfig();
            ConfigManager.Save(_config);
            MessageBox.Show(this, "Configuration saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnStartRestricted_Click(object sender, EventArgs e)
        {
            UIToConfig();
            ConfigManager.Save(_config);
            StartRestricted();
        }

        /// <summary>
        /// Applies config and enters restricted mode (no password prompt on start).
        /// </summary>
        private void StartRestricted()
        {
            _allowShowForm = false;
            RestrictedState.ApplyExitHotkeyConfig(_config.ExitHotkey);
            RestrictedState.EnterRestrictedMode();

            if (_keyboardHook == null)
                _keyboardHook = new KeyboardHook();
            _keyboardHook.Install();
            TaskManagerPolicy.Disable();

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

            Hide();
        }

        private void OnExitRestrictedRequested()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(OnExitRestrictedRequested));
                return;
            }
            if (_passwordDialogShowing)
                return;
            string requiredPassword = string.IsNullOrWhiteSpace(_config?.RestrictedPassword) ? null : _config.RestrictedPassword;
            if (requiredPassword == null)
            {
                RestrictedState.ConfirmExitRestricted();
                return;
            }
            _passwordDialogShowing = true;
            try
            {
                using (var dlg = new PasswordDialogForm("Exit Restricted Mode", "Enter password to exit Restricted Mode:"))
                {
                    dlg.TopMost = true;
                    dlg.StartPosition = FormStartPosition.CenterScreen;
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;
                    if (dlg.EnteredPassword != requiredPassword)
                    {
                        MessageBox.Show(dlg, "Wrong password!", "Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    RestrictedState.ConfirmExitRestricted();
                }
            }
            finally
            {
                _passwordDialogShowing = false;
            }
        }

        private void OnRestrictedModeExited()
        {
            _exitHotCorners?.Stop();
            _keyboardHook?.Uninstall();
            TaskManagerPolicy.Enable();
            _watchDog?.Stop();
            _allowShowForm = true;
            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            BringToFront();
        }

        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            using (var f = new ProcessEditForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK && f.Result != null)
                {
                    var li = new ListViewItem(f.Result.ExePath);
                    li.SubItems.Add(f.Result.Arguments ?? "");
                    li.SubItems.Add(f.Result.WorkingDirectory ?? "");
                    li.Tag = f.Result;
                    lstProcesses.Items.Add(li);
                }
            }
        }

        private void btnRemoveProcess_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItems.Count == 0) return;
            foreach (ListViewItem li in lstProcesses.SelectedItems.Cast<ListViewItem>().ToArray())
                lstProcesses.Items.Remove(li);
        }

        private void lstProcesses_DoubleClick(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItems.Count == 0) return;
            var li = lstProcesses.SelectedItems[0];
            var p = li.Tag as WatchDogProcessConfig ?? new WatchDogProcessConfig { ExePath = li.Text, Arguments = li.SubItems.Count > 1 ? li.SubItems[1].Text : "", WorkingDirectory = li.SubItems.Count > 2 ? li.SubItems[2].Text : "" };
            using (var f = new ProcessEditForm(p))
            {
                if (f.ShowDialog(this) == DialogResult.OK && f.Result != null)
                {
                    li.Text = f.Result.ExePath;
                    if (li.SubItems.Count < 2) { li.SubItems.Add(""); li.SubItems.Add(""); }
                    li.SubItems[0].Text = f.Result.Arguments ?? "";
                    li.SubItems[1].Text = f.Result.WorkingDirectory ?? "";
                    li.Tag = f.Result;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            RestrictedState.RestrictedModeExited -= OnRestrictedModeExited;
            RestrictedState.ExitRestrictedRequested -= OnExitRestrictedRequested;
            TaskManagerPolicy.Enable();
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
