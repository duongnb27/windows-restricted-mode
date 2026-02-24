using System;
using System.IO;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class ProcessEditForm : Form
    {
        public WatchDogProcessConfig Result { get; private set; }

        public ProcessEditForm(WatchDogProcessConfig edit = null)
        {
            InitializeComponent();
            if (edit != null)
            {
                txtExePath.Text = edit.ExePath ?? "";
                txtArguments.Text = edit.Arguments ?? "";
                txtWorkingDir.Text = edit.WorkingDirectory ?? "";
            }
        }

        private void btnBrowseExe_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Executable (*.exe)|*.exe|All files (*.*)|*.*";
                dlg.Title = "Select .exe file";
                if (!string.IsNullOrWhiteSpace(txtExePath.Text) && File.Exists(txtExePath.Text))
                    dlg.InitialDirectory = Path.GetDirectoryName(txtExePath.Text);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtExePath.Text = dlg.FileName;
                    if (string.IsNullOrWhiteSpace(txtWorkingDir.Text))
                        txtWorkingDir.Text = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select working directory";
                if (!string.IsNullOrWhiteSpace(txtWorkingDir.Text) && Directory.Exists(txtWorkingDir.Text))
                    dlg.SelectedPath = txtWorkingDir.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtWorkingDir.Text = dlg.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string exe = (txtExePath.Text ?? "").Trim();
            if (string.IsNullOrEmpty(exe))
            {
                MessageBox.Show(this, "Please select the .exe file path.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtExePath.Focus();
                return;
            }
            if (!File.Exists(exe))
            {
                MessageBox.Show(this, "The .exe file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtExePath.Focus();
                return;
            }
            string workDir = (txtWorkingDir.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(workDir) && !Directory.Exists(workDir))
            {
                MessageBox.Show(this, "Working directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWorkingDir.Focus();
                return;
            }
            Result = new WatchDogProcessConfig
            {
                ExePath = exe,
                Arguments = (txtArguments.Text ?? "").Trim(),
                WorkingDirectory = string.IsNullOrEmpty(workDir) ? null : workDir
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
