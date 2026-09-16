using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
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
                chkAlsoAllowDefaultHotkey.Checked = c.ExitHotkey.AlsoAllowDefaultHotkey;
            }
            txtRestrictedPassword.Text = c.RestrictedPassword ?? "";
            chkAlsoAllowDefaultPassword.Checked = c.AlsoAllowDefaultPassword;

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

            chkHideTaskbar.Checked = c.UtilityHideTaskbar;
            chkHideStartMenu.Checked = c.UtilityHideStartMenu;
        }

        private void UIToConfig()
        {
            if (_config == null) _config = new AppConfig();
            if (_config.ExitHotkey == null) _config.ExitHotkey = new ExitHotkeyConfig();
            _config.ExitHotkey.Ctrl = chkCtrl.Checked;
            _config.ExitHotkey.Shift = chkShift.Checked;
            _config.ExitHotkey.Alt = chkAlt.Checked;
            _config.ExitHotkey.Key = cboExitKey.SelectedItem?.ToString() ?? "F12";
            _config.ExitHotkey.AlsoAllowDefaultHotkey = chkAlsoAllowDefaultHotkey.Checked;

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
                        list.Add(new WatchDogProcessConfig { ExePath = li.Text, Arguments = li.SubItems.Count > 1 ? li.SubItems[1].Text : "", WorkingDirectory = li.SubItems.Count > 2 ? li.SubItems[2].Text : "" });
                }
            }
            _config.WatchDog.Processes = list.ToArray();
            _config.AlsoAllowDefaultPassword = chkAlsoAllowDefaultPassword.Checked;
            _config.ExitHotCornerEnabled = chkHotCornerEnabled.Checked;
            _config.ExitHotCornerCorner = cboHotCornerPosition.SelectedIndex >= 0 ? Math.Min(3, cboHotCornerPosition.SelectedIndex) : 0;
            _config.ExitHotCornerSizePx = (int)numHotCornerSize.Value;
            _config.UtilityHideTaskbar = chkHideTaskbar.Checked;
            _config.UtilityHideStartMenu = chkHideStartMenu.Checked;
        }

        private void btnStartRestricted_Click(object sender, EventArgs e)
        {
            if (SaveChanges()) StartRestricted();
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            SavePasswordChange();
        }

    }
}
