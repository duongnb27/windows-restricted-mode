using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Password input dialog with show/hide toggle.
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
