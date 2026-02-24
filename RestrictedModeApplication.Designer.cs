namespace RestrictedMode
{
    partial class RestrictedModeApplication
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
            this.grpHotkey = new System.Windows.Forms.GroupBox();
            this.cboExitKey = new System.Windows.Forms.ComboBox();
            this.chkAlt = new System.Windows.Forms.CheckBox();
            this.chkShift = new System.Windows.Forms.CheckBox();
            this.chkCtrl = new System.Windows.Forms.CheckBox();
            this.lblHotkey = new System.Windows.Forms.Label();
            this.grpWatchDog = new System.Windows.Forms.GroupBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.numIntervalSeconds = new System.Windows.Forms.NumericUpDown();
            this.lstProcesses = new System.Windows.Forms.ListView();
            this.colExe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colArgs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWorkDir = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddProcess = new System.Windows.Forms.Button();
            this.btnRemoveProcess = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnStartRestricted = new System.Windows.Forms.Button();
            this.grpPassword = new System.Windows.Forms.GroupBox();
            this.lblPasswordHint = new System.Windows.Forms.Label();
            this.txtRestrictedPassword = new System.Windows.Forms.TextBox();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.grpHotCorner = new System.Windows.Forms.GroupBox();
            this.chkHotCornerEnabled = new System.Windows.Forms.CheckBox();
            this.lblHotCornerPosition = new System.Windows.Forms.Label();
            this.cboHotCornerPosition = new System.Windows.Forms.ComboBox();
            this.lblHotCornerSize = new System.Windows.Forms.Label();
            this.numHotCornerSize = new System.Windows.Forms.NumericUpDown();
            this.grpHotkey.SuspendLayout();
            this.grpWatchDog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).BeginInit();
            this.grpPassword.SuspendLayout();
            this.grpHotCorner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHotCornerSize)).BeginInit();
            this.SuspendLayout();
            // 
            // grpHotkey
            // 
            this.grpHotkey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHotkey.Controls.Add(this.cboExitKey);
            this.grpHotkey.Controls.Add(this.chkAlt);
            this.grpHotkey.Controls.Add(this.chkShift);
            this.grpHotkey.Controls.Add(this.chkCtrl);
            this.grpHotkey.Controls.Add(this.lblHotkey);
            this.grpHotkey.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.grpHotkey.Location = new System.Drawing.Point(12, 96);
            this.grpHotkey.Name = "grpHotkey";
            this.grpHotkey.Size = new System.Drawing.Size(800, 72);
            this.grpHotkey.TabIndex = 0;
            this.grpHotkey.TabStop = false;
            this.grpHotkey.Text = "Exit Hotkey";
            // 
            // cboExitKey
            // 
            this.cboExitKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExitKey.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.cboExitKey.FormattingEnabled = true;
            this.cboExitKey.Location = new System.Drawing.Point(164, 40);
            this.cboExitKey.Name = "cboExitKey";
            this.cboExitKey.Size = new System.Drawing.Size(120, 25);
            this.cboExitKey.TabIndex = 4;
            // 
            // chkAlt
            // 
            this.chkAlt.AutoSize = true;
            this.chkAlt.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.chkAlt.Location = new System.Drawing.Point(119, 42);
            this.chkAlt.Name = "chkAlt";
            this.chkAlt.Size = new System.Drawing.Size(45, 23);
            this.chkAlt.TabIndex = 3;
            this.chkAlt.Text = "Alt";
            this.chkAlt.UseVisualStyleBackColor = true;
            // 
            // chkShift
            // 
            this.chkShift.AutoSize = true;
            this.chkShift.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.chkShift.Location = new System.Drawing.Point(64, 42);
            this.chkShift.Name = "chkShift";
            this.chkShift.Size = new System.Drawing.Size(55, 23);
            this.chkShift.TabIndex = 2;
            this.chkShift.Text = "Shift";
            this.chkShift.UseVisualStyleBackColor = true;
            // 
            // chkCtrl
            // 
            this.chkCtrl.AutoSize = true;
            this.chkCtrl.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.chkCtrl.Location = new System.Drawing.Point(13, 42);
            this.chkCtrl.Name = "chkCtrl";
            this.chkCtrl.Size = new System.Drawing.Size(50, 23);
            this.chkCtrl.TabIndex = 1;
            this.chkCtrl.Text = "Ctrl";
            this.chkCtrl.UseVisualStyleBackColor = true;
            // 
            // lblHotkey
            // 
            this.lblHotkey.AutoSize = true;
            this.lblHotkey.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblHotkey.Location = new System.Drawing.Point(10, 22);
            this.lblHotkey.Name = "lblHotkey";
            this.lblHotkey.Size = new System.Drawing.Size(248, 19);
            this.lblHotkey.TabIndex = 0;
            this.lblHotkey.Text = "Press this combination to exit Restricted Mode:";
            // 
            // grpWatchDog
            // 
            this.grpWatchDog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpWatchDog.Controls.Add(this.lblInterval);
            this.grpWatchDog.Controls.Add(this.numIntervalSeconds);
            this.grpWatchDog.Controls.Add(this.lstProcesses);
            this.grpWatchDog.Controls.Add(this.btnAddProcess);
            this.grpWatchDog.Controls.Add(this.btnRemoveProcess);
            this.grpWatchDog.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.grpWatchDog.Location = new System.Drawing.Point(12, 302);
            this.grpWatchDog.Name = "grpWatchDog";
            this.grpWatchDog.Size = new System.Drawing.Size(800, 284);
            this.grpWatchDog.TabIndex = 1;
            this.grpWatchDog.TabStop = false;
            this.grpWatchDog.Text = "Processes to keep running";
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblInterval.Location = new System.Drawing.Point(10, 24);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(127, 19);
            this.lblInterval.TabIndex = 0;
            this.lblInterval.Text = "Check every (seconds):";
            // 
            // numIntervalSeconds
            // 
            this.numIntervalSeconds.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.numIntervalSeconds.Location = new System.Drawing.Point(160, 22);
            this.numIntervalSeconds.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numIntervalSeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIntervalSeconds.Name = "numIntervalSeconds";
            this.numIntervalSeconds.Size = new System.Drawing.Size(60, 25);
            this.numIntervalSeconds.TabIndex = 1;
            this.numIntervalSeconds.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lstProcesses
            // 
            this.lstProcesses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colExe,
            this.colArgs,
            this.colWorkDir});
            this.lstProcesses.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lstProcesses.FullRowSelect = true;
            this.lstProcesses.GridLines = true;
            this.lstProcesses.HideSelection = false;
            this.lstProcesses.Location = new System.Drawing.Point(13, 50);
            this.lstProcesses.Name = "lstProcesses";
            this.lstProcesses.Size = new System.Drawing.Size(774, 196);
            this.lstProcesses.TabIndex = 2;
            this.lstProcesses.UseCompatibleStateImageBehavior = false;
            this.lstProcesses.View = System.Windows.Forms.View.Details;
            this.lstProcesses.DoubleClick += new System.EventHandler(this.lstProcesses_DoubleClick);
            // 
            // colExe
            // 
            this.colExe.Text = "Exe file";
            this.colExe.Width = 229;
            // 
            // colArgs
            // 
            this.colArgs.Text = "Arguments";
            this.colArgs.Width = 179;
            // 
            // colWorkDir
            // 
            this.colWorkDir.Text = "Working directory";
            this.colWorkDir.Width = 341;
            // 
            // btnAddProcess
            // 
            this.btnAddProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddProcess.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnAddProcess.Location = new System.Drawing.Point(13, 254);
            this.btnAddProcess.Name = "btnAddProcess";
            this.btnAddProcess.Size = new System.Drawing.Size(75, 26);
            this.btnAddProcess.TabIndex = 3;
            this.btnAddProcess.Text = "Add";
            this.btnAddProcess.UseVisualStyleBackColor = true;
            this.btnAddProcess.Click += new System.EventHandler(this.btnAddProcess_Click);
            // 
            // btnRemoveProcess
            // 
            this.btnRemoveProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveProcess.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnRemoveProcess.Location = new System.Drawing.Point(94, 254);
            this.btnRemoveProcess.Name = "btnRemoveProcess";
            this.btnRemoveProcess.Size = new System.Drawing.Size(75, 26);
            this.btnRemoveProcess.TabIndex = 4;
            this.btnRemoveProcess.Text = "Remove";
            this.btnRemoveProcess.UseVisualStyleBackColor = true;
            this.btnRemoveProcess.Click += new System.EventHandler(this.btnRemoveProcess_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeButtonLarge);
            this.btnSave.Location = new System.Drawing.Point(552, 600);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 38);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save configuration";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnStartRestricted
            // 
            this.btnStartRestricted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartRestricted.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnStartRestricted.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeButtonLarge, System.Drawing.FontStyle.Bold);
            this.btnStartRestricted.Location = new System.Drawing.Point(690, 600);
            this.btnStartRestricted.Name = "btnStartRestricted";
            this.btnStartRestricted.Size = new System.Drawing.Size(122, 38);
            this.btnStartRestricted.TabIndex = 3;
            this.btnStartRestricted.Text = "Start Restricted";
            this.btnStartRestricted.UseVisualStyleBackColor = false;
            this.btnStartRestricted.Click += new System.EventHandler(this.btnStartRestricted_Click);
            // 
            // grpPassword
            // 
            this.grpPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPassword.Controls.Add(this.lblPasswordHint);
            this.grpPassword.Controls.Add(this.txtRestrictedPassword);
            this.grpPassword.Controls.Add(this.btnShowPassword);
            this.grpPassword.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.grpPassword.Location = new System.Drawing.Point(12, 12);
            this.grpPassword.Name = "grpPassword";
            this.grpPassword.Size = new System.Drawing.Size(800, 76);
            this.grpPassword.TabIndex = 5;
            this.grpPassword.TabStop = false;
            this.grpPassword.Text = "Restricted Mode Password";
            // 
            // grpHotCorner
            // 
            this.grpHotCorner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHotCorner.Controls.Add(this.chkHotCornerEnabled);
            this.grpHotCorner.Controls.Add(this.lblHotCornerPosition);
            this.grpHotCorner.Controls.Add(this.cboHotCornerPosition);
            this.grpHotCorner.Controls.Add(this.lblHotCornerSize);
            this.grpHotCorner.Controls.Add(this.numHotCornerSize);
            this.grpHotCorner.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.grpHotCorner.Location = new System.Drawing.Point(12, 176);
            this.grpHotCorner.Name = "grpHotCorner";
            this.grpHotCorner.Size = new System.Drawing.Size(800, 118);
            this.grpHotCorner.TabIndex = 6;
            this.grpHotCorner.TabStop = false;
            this.grpHotCorner.Text = "Exit Hot Corner";
            // 
            // chkHotCornerEnabled
            // 
            this.chkHotCornerEnabled.AutoSize = true;
            this.chkHotCornerEnabled.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.chkHotCornerEnabled.Location = new System.Drawing.Point(13, 22);
            this.chkHotCornerEnabled.Name = "chkHotCornerEnabled";
            this.chkHotCornerEnabled.Size = new System.Drawing.Size(348, 23);
            this.chkHotCornerEnabled.TabIndex = 0;
            this.chkHotCornerEnabled.Text = "Enable";
            this.chkHotCornerEnabled.UseVisualStyleBackColor = true;
            // 
            // lblHotCornerPosition
            // 
            this.lblHotCornerPosition.AutoSize = true;
            this.lblHotCornerPosition.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblHotCornerPosition.Location = new System.Drawing.Point(10, 48);
            this.lblHotCornerPosition.Name = "lblHotCornerPosition";
            this.lblHotCornerPosition.Size = new System.Drawing.Size(38, 19);
            this.lblHotCornerPosition.TabIndex = 1;
            this.lblHotCornerPosition.Text = "Corner:";
            // 
            // cboHotCornerPosition
            // 
            this.cboHotCornerPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHotCornerPosition.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.cboHotCornerPosition.FormattingEnabled = true;
            this.cboHotCornerPosition.Location = new System.Drawing.Point(65, 45);
            this.cboHotCornerPosition.Name = "cboHotCornerPosition";
            this.cboHotCornerPosition.Size = new System.Drawing.Size(140, 25);
            this.cboHotCornerPosition.TabIndex = 2;
            // 
            // lblHotCornerSize
            // 
            this.lblHotCornerSize.AutoSize = true;
            this.lblHotCornerSize.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblHotCornerSize.Location = new System.Drawing.Point(10, 78);
            this.lblHotCornerSize.Name = "lblHotCornerSize";
            this.lblHotCornerSize.Size = new System.Drawing.Size(99, 19);
            this.lblHotCornerSize.TabIndex = 3;
            this.lblHotCornerSize.Text = "Size (pixels):";
            // 
            // numHotCornerSize
            // 
            this.numHotCornerSize.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.numHotCornerSize.Location = new System.Drawing.Point(90, 76);
            this.numHotCornerSize.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numHotCornerSize.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numHotCornerSize.Name = "numHotCornerSize";
            this.numHotCornerSize.Size = new System.Drawing.Size(70, 25);
            this.numHotCornerSize.TabIndex = 4;
            this.numHotCornerSize.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // lblPasswordHint
            // 
            this.lblPasswordHint.AutoSize = true;
            this.lblPasswordHint.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.lblPasswordHint.Location = new System.Drawing.Point(10, 22);
            this.lblPasswordHint.Name = "lblPasswordHint";
            this.lblPasswordHint.Size = new System.Drawing.Size(399, 19);
            this.lblPasswordHint.TabIndex = 0;
            this.lblPasswordHint.Text = "Leave empty to not require password when exiting Restricted Mode:";
            // 
            // txtRestrictedPassword
            // 
            this.txtRestrictedPassword.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.txtRestrictedPassword.Location = new System.Drawing.Point(10, 46);
            this.txtRestrictedPassword.Name = "txtRestrictedPassword";
            this.txtRestrictedPassword.Size = new System.Drawing.Size(173, 25);
            this.txtRestrictedPassword.TabIndex = 1;
            this.txtRestrictedPassword.UseSystemPasswordChar = true;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.btnShowPassword.Location = new System.Drawing.Point(189, 46);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(32, 26);
            this.btnShowPassword.TabIndex = 2;
            this.btnShowPassword.Text = "👁";
            this.btnShowPassword.UseVisualStyleBackColor = true;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // RestrictedModeApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 645);
            this.Controls.Add(this.btnStartRestricted);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpWatchDog);
            this.Controls.Add(this.grpHotCorner);
            this.Controls.Add(this.grpHotkey);
            this.Controls.Add(this.grpPassword);
            this.Font = new System.Drawing.Font(UIFonts.Family, UIFonts.SizeNormal);
            this.MinimumSize = new System.Drawing.Size(500, 430);
            this.Name = "RestrictedModeApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Restricted Mode — Configuration";
            this.grpHotkey.ResumeLayout(false);
            this.grpHotkey.PerformLayout();
            this.grpWatchDog.ResumeLayout(false);
            this.grpWatchDog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).EndInit();
            this.grpPassword.ResumeLayout(false);
            this.grpPassword.PerformLayout();
            this.grpHotCorner.ResumeLayout(false);
            this.grpHotCorner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHotCornerSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpHotkey;
        private System.Windows.Forms.ComboBox cboExitKey;
        private System.Windows.Forms.CheckBox chkAlt;
        private System.Windows.Forms.CheckBox chkShift;
        private System.Windows.Forms.CheckBox chkCtrl;
        private System.Windows.Forms.Label lblHotkey;
        private System.Windows.Forms.GroupBox grpWatchDog;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numIntervalSeconds;
        private System.Windows.Forms.ListView lstProcesses;
        private System.Windows.Forms.ColumnHeader colExe;
        private System.Windows.Forms.ColumnHeader colArgs;
        private System.Windows.Forms.ColumnHeader colWorkDir;
        private System.Windows.Forms.Button btnAddProcess;
        private System.Windows.Forms.Button btnRemoveProcess;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnStartRestricted;
        private System.Windows.Forms.GroupBox grpPassword;
        private System.Windows.Forms.Label lblPasswordHint;
        private System.Windows.Forms.TextBox txtRestrictedPassword;
        private System.Windows.Forms.Button btnShowPassword;
        private System.Windows.Forms.GroupBox grpHotCorner;
        private System.Windows.Forms.CheckBox chkHotCornerEnabled;
        private System.Windows.Forms.Label lblHotCornerPosition;
        private System.Windows.Forms.ComboBox cboHotCornerPosition;
        private System.Windows.Forms.Label lblHotCornerSize;
        private System.Windows.Forms.NumericUpDown numHotCornerSize;
    }
}
