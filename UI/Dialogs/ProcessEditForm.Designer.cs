namespace RestrictedMode
{
    partial class ProcessEditForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblExe = new System.Windows.Forms.Label();
            this.txtExePath = new System.Windows.Forms.TextBox();
            this.btnBrowseExe = new System.Windows.Forms.Button();
            this.lblArgs = new System.Windows.Forms.Label();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.lblWorkDir = new System.Windows.Forms.Label();
            this.txtWorkingDir = new System.Windows.Forms.TextBox();
            this.btnBrowseDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblExe
            //
            this.lblExe.AutoSize = true;
            this.lblExe.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblExe.Location = new System.Drawing.Point(12, 14);
            this.lblExe.Name = "lblExe";
            this.lblExe.Size = new System.Drawing.Size(68, 13);
            this.lblExe.TabIndex = 0;
            this.lblExe.Text = "Exe path:";
            //
            // txtExePath
            //
            this.txtExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExePath.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.txtExePath.Location = new System.Drawing.Point(12, 32);
            this.txtExePath.Name = "txtExePath";
            this.txtExePath.Size = new System.Drawing.Size(348, 25);
            this.txtExePath.TabIndex = 1;
            //
            // btnBrowseExe
            //
            this.btnBrowseExe.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnBrowseExe.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnBrowseExe.Location = new System.Drawing.Point(366, 30);
            this.btnBrowseExe.Name = "btnBrowseExe";
            this.btnBrowseExe.Size = new System.Drawing.Size(28, 26);
            this.btnBrowseExe.TabIndex = 2;
            this.btnBrowseExe.Text = "...";
            this.btnBrowseExe.UseVisualStyleBackColor = true;
            this.btnBrowseExe.Click += new System.EventHandler(this.btnBrowseExe_Click);
            //
            // lblArgs
            //
            this.lblArgs.AutoSize = true;
            this.lblArgs.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblArgs.Location = new System.Drawing.Point(12, 58);
            this.lblArgs.Name = "lblArgs";
            this.lblArgs.Size = new System.Drawing.Size(50, 13);
            this.lblArgs.TabIndex = 3;
            this.lblArgs.Text = "Arguments:";
            //
            // txtArguments
            //
            this.txtArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArguments.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.txtArguments.Location = new System.Drawing.Point(12, 76);
            this.txtArguments.Name = "txtArguments";
            this.txtArguments.Size = new System.Drawing.Size(382, 25);
            this.txtArguments.TabIndex = 4;
            //
            // lblWorkDir
            //
            this.lblWorkDir.AutoSize = true;
            this.lblWorkDir.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblWorkDir.Location = new System.Drawing.Point(12, 102);
            this.lblWorkDir.Name = "lblWorkDir";
            this.lblWorkDir.Size = new System.Drawing.Size(97, 13);
            this.lblWorkDir.TabIndex = 5;
            this.lblWorkDir.Text = "Working directory (optional):";
            //
            // txtWorkingDir
            //
            this.txtWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWorkingDir.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.txtWorkingDir.Location = new System.Drawing.Point(12, 120);
            this.txtWorkingDir.Name = "txtWorkingDir";
            this.txtWorkingDir.Size = new System.Drawing.Size(348, 25);
            this.txtWorkingDir.TabIndex = 6;
            //
            // btnBrowseDir
            //
            this.btnBrowseDir.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnBrowseDir.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnBrowseDir.Location = new System.Drawing.Point(366, 118);
            this.btnBrowseDir.Name = "btnBrowseDir";
            this.btnBrowseDir.Size = new System.Drawing.Size(28, 26);
            this.btnBrowseDir.TabIndex = 7;
            this.btnBrowseDir.Text = "...";
            this.btnBrowseDir.UseVisualStyleBackColor = true;
            this.btnBrowseDir.Click += new System.EventHandler(this.btnBrowseDir_Click);
            //
            // btnOK
            //
            this.btnOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnOK.Location = new System.Drawing.Point(238, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnCancel.Location = new System.Drawing.Point(319, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // ProcessEditForm
            //
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.ClientSize = new System.Drawing.Size(406, 196);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowseDir);
            this.Controls.Add(this.txtWorkingDir);
            this.Controls.Add(this.lblWorkDir);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.lblArgs);
            this.Controls.Add(this.btnBrowseExe);
            this.Controls.Add(this.txtExePath);
            this.Controls.Add(this.lblExe);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add / Edit process";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblExe;
        private System.Windows.Forms.TextBox txtExePath;
        private System.Windows.Forms.Button btnBrowseExe;
        private System.Windows.Forms.Label lblArgs;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.Label lblWorkDir;
        private System.Windows.Forms.TextBox txtWorkingDir;
        private System.Windows.Forms.Button btnBrowseDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
