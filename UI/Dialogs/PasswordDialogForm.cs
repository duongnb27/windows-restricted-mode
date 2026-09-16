using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Password input dialog. Password characters always remain masked.
    /// </summary>
    public partial class PasswordDialogForm : Form
    {
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int dwProcessId);

        public string ExpectedPassword { private get; set; }
        public string AlternateExpectedPassword { private get; set; }

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
            ForceKeyboardFocus();
            // Retry after the previous app (e.g. a focused kiosk text box) finishes its focus cycle.
            BeginInvoke(new Action(() =>
            {
                if (IsDisposed || !Visible) return;
                ForceKeyboardFocus();
            }));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (txtPassword != null && !txtPassword.Focused)
                txtPassword.Focus();
        }

        /// <summary>
        /// Steal foreground from another process that currently owns keyboard focus
        /// (common when a managed kiosk webview has an active text field).
        /// </summary>
        private void ForceKeyboardFocus()
        {
            IntPtr hwnd = Handle;
            AllowSetForegroundWindow(-1); // ASFW_ANY
            IntPtr foreground = GetForegroundWindow();
            uint thisThread = GetCurrentThreadId();
            uint foreThread = foreground != IntPtr.Zero
                ? GetWindowThreadProcessId(foreground, IntPtr.Zero)
                : 0;
            bool attached = false;
            try
            {
                if (foreThread != 0 && foreThread != thisThread)
                    attached = AttachThreadInput(foreThread, thisThread, true);
                ShowWindow(hwnd, SW_RESTORE);
                BringWindowToTop(hwnd);
                SetForegroundWindow(hwnd);
                Activate();
                TopMost = true;
            }
            finally
            {
                if (attached) AttachThreadInput(foreThread, thisThread, false);
            }
            ActiveControl = txtPassword;
            txtPassword.Focus();
            txtPassword.Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool matchesPrimary = ExpectedPassword != null && EnteredPassword == ExpectedPassword;
            bool matchesAlternate = AlternateExpectedPassword != null && EnteredPassword == AlternateExpectedPassword;
            if (!matchesPrimary && !matchesAlternate)
            {
                _passwordError.Text = UIText.IncorrectPassword;
                txtPassword.Focus();
                txtPassword.SelectAll();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
