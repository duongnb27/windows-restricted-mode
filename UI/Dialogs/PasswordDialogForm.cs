using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Password input dialog with show/hide toggle.
    /// </summary>
    public partial class PasswordDialogForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        public string ExpectedPassword { private get; set; }

        public string EnteredPassword => txtPassword.Text ?? "";

        public PasswordDialogForm(string title, string prompt)
        {
            InitializeComponent();
            ApplyModernLayout(title);
            Text = title;
            lblPrompt.Text = prompt;
            ActiveControl = txtPassword;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TopMost = true;
            BeginInvoke(new Action(() =>
            {
                if (IsDisposed || !Visible) return;
                SetForegroundWindow(Handle);
                Activate();
                txtPassword.Select();
            }));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // Focus on activation only; do not poll or fight other windows with a timer.
            if (txtPassword != null) ActiveControl = txtPassword;
        }

        private void btnToggleVisibility_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnToggleVisibility.Text = txtPassword.UseSystemPasswordChar ? "Show" : "Hide";
            txtPassword.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ExpectedPassword != null && EnteredPassword != ExpectedPassword)
            {
                _passwordError.Text = "Incorrect password. Please try again.";
                txtPassword.Focus();
                txtPassword.SelectAll();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
