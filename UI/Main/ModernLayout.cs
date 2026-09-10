using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private void ApplyModernLayout()
        {
            SuspendLayout();
            BackColor = Color.FromArgb(243, 246, 250);
            ForeColor = Color.FromArgb(30, 41, 59);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(900, 700);
            MinimumSize = new Size(850, 650);
            Text = "Restricted Mode | Settings";
            using (var stream = typeof(RestrictedModeApplication).Assembly.GetManifestResourceStream("RestrictedMode.AppIcon.ico"))
                if (stream != null) Icon = new Icon(stream);

            var header = new Panel { Dock = DockStyle.Top, Height = 104, BackColor = Color.FromArgb(15, 23, 42) };
            var icon = new PictureBox { Location = new Point(26, 26), Size = new Size(48, 48),
                SizeMode = PictureBoxSizeMode.Zoom, Image = Icon.ToBitmap() };
            header.Controls.Add(icon);
            header.Controls.Add(new Label { Text = "Restricted Mode", AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(88, 18) });
            header.Controls.Add(new Label { Text = "Configure access, managed apps and Windows startup.", AutoSize = true,
                ForeColor = Color.FromArgb(180, 195, 215), Location = new Point(92, 65) });

            var tabs = new SettingsNavigation { Dock = DockStyle.Fill, Font = Font };
            var access = CreateSettingsPage(tabs, "Access & security");
            var apps = CreateSettingsPage(tabs, "Managed apps");
            var system = CreateSettingsPage(tabs, "Settings");
            AddSection(access, grpPassword, "Administrator access", 110);
            AddSection(access, grpHotkey, "Exit shortcut", 125);
            AddSection(access, grpHotCorner, "Touch exit corner", 145);
            AddSection(apps, grpWatchDog, "Keep applications running", 355);
            AddSection(system, grpUtility, "Desktop & startup", 135);
            lblPasswordHint.Text = "Set an exit password. Leave empty to allow access without a password.";
            txtRestrictedPassword.AutoSize = false;
            txtRestrictedPassword.SetBounds(10, 48, 260, 32);
            btnShowPassword.Location = new Point(282, 48);
            btnShowPassword.Size = new Size(80, 32);
            btnShowPassword.Text = "Show";
            chkAlsoAllowDefaultHotkey.Text = "Also allow Ctrl + Shift + F12";
            chkHotCornerEnabled.Text = "Enable touch corner to request exit";
            lstProcesses.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lstProcesses.Size = new Size(770, 235);
            lstProcesses.BorderStyle = BorderStyle.FixedSingle;
            lstProcesses.FullRowSelect = true;
            btnAddProcess.Location = new Point(13, 298);
            btnRemoveProcess.Location = new Point(145, 298);
            btnAddProcess.Size = btnRemoveProcess.Size = new Size(120, 36);
            btnAddProcess.Text = "Add application";
            btnRemoveProcess.Text = "Remove";
            colExe.Width = 285;
            colArgs.Width = 190;
            colWorkDir.Width = 265;

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 86, BackColor = Color.White };
            btnStartRestricted.Text = "Start";
            btnStartRestricted.Size = new Size(180, 40);
            btnStartRestricted.Anchor = AnchorStyles.None;
            footer.Controls.Add(btnStartRestricted);
            _saveStatus = new Label { AutoSize = false, Height = 22, Dock = DockStyle.Bottom,
                Text = "", Visible = false, TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 116, 139) };
            footer.Controls.Add(_saveStatus);
            footer.Resize += (sender, args) => btnStartRestricted.Location =
                new Point((footer.ClientSize.Width - btnStartRestricted.Width) / 2, (footer.ClientSize.Height - btnStartRestricted.Height) / 2);
            StyleControls(this);
            StyleControls(tabs);
            tabs.SelectPage(0);
            StyleControls(footer);
            btnStartRestricted.BackColor = Color.FromArgb(37, 99, 235);
            btnStartRestricted.ForeColor = Color.White;
            btnStartRestricted.FlatAppearance.BorderSize = 0;
            Controls.Add(tabs);
            Controls.Add(footer);
            Controls.Add(header);
            tabs.BringToFront();
            ResumeLayout(true);
        }

        private static FlowLayoutPanel CreateSettingsPage(SettingsNavigation tabs, string title)
        {
            var page = tabs.AddPage(title);
            page.BackColor = Color.FromArgb(243, 246, 250);
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, WrapContents = false };
            page.Controls.Add(flow);

            flow.SizeChanged += (sender, args) =>
            {
                foreach (Control child in flow.Controls)
                    child.Width = Math.Max(750, flow.ClientSize.Width - 26);
            };
            return flow;
        }

        private static void AddSection(FlowLayoutPanel page, GroupBox group, string title, int height)
        {
            group.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            group.Dock = DockStyle.None;
            group.Text = title;
            group.BackColor = Color.White;
            group.ForeColor = Color.FromArgb(30, 41, 59);
            group.Size = new Size(810, height);
            group.Margin = new Padding(0, 0, 0, 14);
            page.Controls.Add(group);
        }

        private static void StyleControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                var button = child as Button;
                if (button != null)
                {
                    button.TextAlign = ContentAlignment.MiddleCenter;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    button.BackColor = Color.White;
                    button.ForeColor = Color.FromArgb(30, 41, 59);
                    button.Font = new Font("Segoe UI", 10F);
                    button.Cursor = Cursors.Hand;
                }
                var list = child as ListView;
                if (list != null) { list.BackColor = Color.White; list.ForeColor = Color.FromArgb(30, 41, 59); }
                StyleControls(child);
            }
        }
    }
}
