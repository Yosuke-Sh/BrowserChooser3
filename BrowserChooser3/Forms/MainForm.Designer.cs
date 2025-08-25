namespace BrowserChooser3.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnInfo = new Button();
            btnOptions = new Button();
            btnCancel = new Button();
            btnCopyToClipboard = new Button();
            btnCopyToClipboardAndClose = new Button();
            chkAutoClose = new CheckBox();
            chkAutoOpen = new CheckBox();

            tmrDelay = new System.Windows.Forms.Timer(components);
            cmOptions = new ContextMenuStrip(components);
            miEditMode = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            cmOptions.SuspendLayout();
            SuspendLayout();
            // 
            // btnInfo
            // 
            btnInfo.AccessibleName = "About";
            btnInfo.AutoSize = true;
            btnInfo.BackColor = Color.Transparent;
            btnInfo.FlatAppearance.BorderSize = 0;
            btnInfo.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnInfo.FlatStyle = FlatStyle.Flat;
            btnInfo.Image = Properties.Resources.Icon122;
            btnInfo.Location = new Point(2, 1);
            btnInfo.Margin = new Padding(0);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(24, 24);
            btnInfo.TabIndex = 1;
            btnInfo.UseVisualStyleBackColor = false;
            btnInfo.Click += btnInfo_Click;
            // 
            // btnOptions
            // 
            btnOptions.AccessibleName = "Options";
            btnOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOptions.AutoSize = true;
            btnOptions.BackColor = Color.Transparent;
            btnOptions.FlatAppearance.BorderSize = 0;
            btnOptions.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnOptions.FlatStyle = FlatStyle.Flat;
            btnOptions.Image = Properties.Resources.SettingsIcon;
            btnOptions.Location = new Point(ClientSize.Width - 35, 15);
            btnOptions.Margin = new Padding(0);
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new Size(28, 28);
            btnOptions.TabIndex = 2;
            btnOptions.UseVisualStyleBackColor = false;
            btnOptions.Click += btnOptions_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Transparent;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Image = Properties.Resources.WorldGoIcon;
            btnCancel.Location = new Point(370, 12);
            btnCancel.Margin = new Padding(0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0, 0);
            btnCancel.TabIndex = 3;
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnCopyToClipboard
            // 
            btnCopyToClipboard.BackColor = Color.Transparent;
            btnCopyToClipboard.FlatAppearance.BorderSize = 0;
            btnCopyToClipboard.FlatStyle = FlatStyle.Flat;
            btnCopyToClipboard.Image = Properties.Resources.PasteIcon;
            btnCopyToClipboard.Location = new Point(ClientSize.Width - 35, 50);
            btnCopyToClipboard.Margin = new Padding(0);
            btnCopyToClipboard.Name = "btnCopyToClipboard";
            btnCopyToClipboard.Size = new Size(28, 28);
            btnCopyToClipboard.TabIndex = 4;
            btnCopyToClipboard.UseVisualStyleBackColor = false;
            btnCopyToClipboard.Click += btnCopyToClipboard_Click;
            // 
            // btnCopyToClipboardAndClose
            // 
            btnCopyToClipboardAndClose.BackColor = Color.Transparent;
            btnCopyToClipboardAndClose.FlatAppearance.BorderSize = 0;
            btnCopyToClipboardAndClose.FlatStyle = FlatStyle.Flat;
            btnCopyToClipboardAndClose.Image = Properties.Resources.PasteAndCloseIcon;
            btnCopyToClipboardAndClose.Location = new Point(ClientSize.Width - 35, 85);
            btnCopyToClipboardAndClose.Margin = new Padding(0);
            btnCopyToClipboardAndClose.Name = "btnCopyToClipboardAndClose";
            btnCopyToClipboardAndClose.Size = new Size(28, 28);
            btnCopyToClipboardAndClose.TabIndex = 5;
            btnCopyToClipboardAndClose.UseVisualStyleBackColor = false;
            btnCopyToClipboardAndClose.Click += btnCopyToClipboardAndClose_Click;
            // 
            // chkAutoClose
            // 
            chkAutoClose.AutoSize = true;
            chkAutoClose.BackColor = Color.Transparent;
            chkAutoClose.Location = new Point(20, ClientSize.Height - 80);
            chkAutoClose.Margin = new Padding(5, 6, 5, 6);
            chkAutoClose.Name = "chkAutoClose";
            chkAutoClose.Size = new Size(400, 24);
            chkAutoClose.TabIndex = 6;
            chkAutoClose.Text = "ブラウザを選択後に自動的に閉じる";
            chkAutoClose.UseVisualStyleBackColor = false;
            chkAutoClose.CheckedChanged += chkAutoClose_CheckedChanged;
            // 
            // chkAutoOpen
            // 
            chkAutoOpen.AutoSize = true;
            chkAutoOpen.BackColor = Color.Transparent;
            chkAutoOpen.Location = new Point(20, ClientSize.Height - 50);
            chkAutoOpen.Margin = new Padding(5, 6, 5, 6);
            chkAutoOpen.Name = "chkAutoOpen";
            chkAutoOpen.Size = new Size(450, 22);
            chkAutoOpen.TabIndex = 7;
            chkAutoOpen.Text = "指定秒数後にデフォルトブラウザを開く [space key:Timerの一時停止/再開]";
            chkAutoOpen.UseVisualStyleBackColor = false;
            chkAutoOpen.CheckedChanged += chkAutoOpen_CheckedChanged;
            // 

            // 
            // tmrDelay
            // 
            tmrDelay.Interval = 1000;
            tmrDelay.Tick += tmrDelay_Tick;
            // 
            // cmOptions
            // 
            cmOptions.ImageScalingSize = new Size(24, 24);
            cmOptions.Items.AddRange(new ToolStripItem[] { miEditMode, toolStripSeparator1, toolStripMenuItem2, toolStripMenuItem3 });
            cmOptions.Name = "cmOptions";
            cmOptions.Size = new Size(246, 106);
            // 
            // miEditMode
            // 
            miEditMode.Name = "miEditMode";
            miEditMode.Size = new Size(245, 32);
            miEditMode.Text = "&Edit";
            miEditMode.Click += miEditMode_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(242, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(245, 32);
            toolStripMenuItem2.Text = "ToolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(245, 32);
            toolStripMenuItem3.Text = "ToolStripMenuItem3";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gold;
            CancelButton = btnCancel;
            ClientSize = new Size(542, 157);
            Controls.Add(btnInfo);
            Controls.Add(btnCopyToClipboardAndClose);
            Controls.Add(chkAutoOpen);
            Controls.Add(chkAutoClose);
            Controls.Add(btnCopyToClipboard);
            Controls.Add(btnCancel);
            Controls.Add(btnOptions);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            KeyPreview = true;
            Margin = new Padding(5, 6, 5, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Choose a Browser";
            TopMost = true;
            Load += MainForm_Load;
            cmOptions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCopyToClipboard;
        private System.Windows.Forms.Button btnCopyToClipboardAndClose;
        private System.Windows.Forms.CheckBox chkAutoClose;
        private System.Windows.Forms.Timer tmrDelay;
        private System.Windows.Forms.CheckBox chkAutoOpen;
        private System.Windows.Forms.ContextMenuStrip cmOptions;
        private System.Windows.Forms.ToolStripMenuItem miEditMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;

    }
}
