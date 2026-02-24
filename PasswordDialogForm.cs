using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Dialog nhập mật khẩu — ô ẩn với nút con mắt để hiển thị/ẩn.
    /// </summary>
    public partial class PasswordDialogForm : Form
    {
        public string EnteredPassword => txtPassword.Text ?? "";

        public PasswordDialogForm(string title, string prompt)
        {
            InitializeComponent();
            Text = title;
            lblPrompt.Text = prompt;
        }

        private void btnToggleVisibility_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnToggleVisibility.Text = txtPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
