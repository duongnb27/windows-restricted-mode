using System.Drawing;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class PasswordDialogForm
    {
        private Label _passwordError;

        private void ApplyModernLayout(string title)
        {
            SuspendLayout();
            Font = new Font("Segoe UI", 10F);
            AutoScaleDimensions = new SizeF(7F, 17F);
            ClientSize = new Size(480, 286);
            BackColor = Color.FromArgb(243, 246, 250);
            ForeColor = Color.FromArgb(30, 41, 59);
            ShowInTaskbar = false;
            using (var stream = typeof(PasswordDialogForm).Assembly.GetManifestResourceStream("RestrictedMode.AppIcon.ico"))
                if (stream != null) Icon = new Icon(stream);

            var header = new Panel { Dock = DockStyle.Top, Height = 88, BackColor = Color.FromArgb(15, 23, 42) };
            header.Controls.Add(new PictureBox { Image = Icon.ToBitmap(), SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(24, 24), Size = new Size(40, 40) });
            header.Controls.Add(new Label { Text = title, AutoSize = false, Location = new Point(80, 16),
                Size = new Size(380, 32), ForeColor = Color.White, Font = new Font("Segoe UI", 17F, FontStyle.Bold) });
            header.Controls.Add(new Label { Text = UIText.AdministratorVerification, AutoSize = true,
                Location = new Point(82, 52), ForeColor = Color.FromArgb(180, 195, 215) });
            Controls.Add(header);

            lblPrompt.AutoSize = false;
            lblPrompt.SetBounds(24, 106, 432, 40);
            lblPrompt.TextAlign = ContentAlignment.TopLeft;
            txtPassword.AutoSize = false;
            txtPassword.SetBounds(24, 150, 340, 34);
            txtPassword.TabIndex = 0;
            btnToggleVisibility.SetBounds(376, 150, 80, 34);
            btnToggleVisibility.TabIndex = 1;
            _passwordError = new Label { Location = new Point(24, 190), Size = new Size(432, 24),
                ForeColor = Color.Firebrick, Text = "" };
            Controls.Add(_passwordError);
            txtPassword.TextChanged += (sender, args) => _passwordError.Text = "";
            btnCancel.SetBounds(114, 220, 120, 40);
            btnOK.SetBounds(246, 220, 120, 40);
            btnOK.TabIndex = 2;
            btnCancel.TabIndex = 3;
            foreach (var button in new[] { btnToggleVisibility, btnOK, btnCancel })
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                button.BackColor = Color.White;
                button.ForeColor = ForeColor;
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.Font = Font;
                button.Cursor = Cursors.Hand;
                button.UseVisualStyleBackColor = false;
            }
            btnOK.BackColor = Color.FromArgb(37, 99, 235);
            btnOK.ForeColor = Color.White;
            btnOK.FlatAppearance.BorderSize = 0;
            ResumeLayout(true);
        }
    }
}
