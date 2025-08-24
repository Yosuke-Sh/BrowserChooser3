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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            
            // メインコントロール
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.btnCopyToClipboardAndClose = new System.Windows.Forms.Button();
            this.chkAutoClose = new System.Windows.Forms.CheckBox();
            this.chkAutoOpen = new System.Windows.Forms.CheckBox();
            this.lblShortcutMessage = new System.Windows.Forms.Label();
            this.tmrDelay = new System.Windows.Forms.Timer(this.components);
            this.cmOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miEditMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            
            this.cmOptions.SuspendLayout();
            this.SuspendLayout();
            
            // btnInfo
            this.btnInfo.AccessibleName = "About";
            this.btnInfo.AutoSize = true;
            this.btnInfo.BackColor = System.Drawing.Color.Transparent;
            this.btnInfo.FlatAppearance.BorderSize = 0;
            this.btnInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.Location = new System.Drawing.Point(14, 52);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(0);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(24, 24);
            this.btnInfo.TabIndex = 1;
            this.btnInfo.UseVisualStyleBackColor = false;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            
            // btnOptions
            this.btnOptions.AccessibleName = "Options";
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.AutoSize = true;
            this.btnOptions.BackColor = System.Drawing.Color.Transparent;
            this.btnOptions.FlatAppearance.BorderSize = 0;
            this.btnOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOptions.Location = new System.Drawing.Point(511, 12);
            this.btnOptions.Margin = new System.Windows.Forms.Padding(0);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(24, 24);
            this.btnOptions.TabIndex = 2;
            this.btnOptions.UseVisualStyleBackColor = false;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            
            // btnCancel
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(511, 40);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(24, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // btnCopyToClipboard
            this.btnCopyToClipboard.BackColor = System.Drawing.Color.Transparent;
            this.btnCopyToClipboard.FlatAppearance.BorderSize = 0;
            this.btnCopyToClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyToClipboard.Location = new System.Drawing.Point(511, 70);
            this.btnCopyToClipboard.Margin = new System.Windows.Forms.Padding(0);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToClipboard.TabIndex = 4;
            this.btnCopyToClipboard.UseVisualStyleBackColor = false;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            
            // btnCopyToClipboardAndClose
            this.btnCopyToClipboardAndClose.BackColor = System.Drawing.Color.Transparent;
            this.btnCopyToClipboardAndClose.FlatAppearance.BorderSize = 0;
            this.btnCopyToClipboardAndClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyToClipboardAndClose.Location = new System.Drawing.Point(511, 100);
            this.btnCopyToClipboardAndClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnCopyToClipboardAndClose.Name = "btnCopyToClipboardAndClose";
            this.btnCopyToClipboardAndClose.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToClipboardAndClose.TabIndex = 5;
            this.btnCopyToClipboardAndClose.UseVisualStyleBackColor = false;
            this.btnCopyToClipboardAndClose.Click += new System.EventHandler(this.btnCopyToClipboardAndClose_Click);
            
            // chkAutoClose
            this.chkAutoClose.AutoSize = true;
            this.chkAutoClose.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoClose.Location = new System.Drawing.Point(14, 120);
            this.chkAutoClose.Name = "chkAutoClose";
            this.chkAutoClose.Size = new System.Drawing.Size(80, 17);
            this.chkAutoClose.TabIndex = 6;
            this.chkAutoClose.Text = "Auto Close";
            this.chkAutoClose.UseVisualStyleBackColor = false;
            this.chkAutoClose.CheckedChanged += new System.EventHandler(this.chkAutoClose_CheckedChanged);
            
            // chkAutoOpen
            this.chkAutoOpen.AutoSize = true;
            this.chkAutoOpen.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoOpen.Location = new System.Drawing.Point(100, 120);
            this.chkAutoOpen.Name = "chkAutoOpen";
            this.chkAutoOpen.Size = new System.Drawing.Size(80, 17);
            this.chkAutoOpen.TabIndex = 7;
            this.chkAutoOpen.Text = "Auto Open";
            this.chkAutoOpen.UseVisualStyleBackColor = false;
            this.chkAutoOpen.CheckedChanged += new System.EventHandler(this.chkAutoOpen_CheckedChanged);
            
            // lblShortcutMessage
            this.lblShortcutMessage.AutoSize = true;
            this.lblShortcutMessage.BackColor = System.Drawing.Color.Black;
            this.lblShortcutMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblShortcutMessage.ForeColor = System.Drawing.Color.White;
            this.lblShortcutMessage.Location = new System.Drawing.Point(2, 1);
            this.lblShortcutMessage.Name = "lblShortcutMessage";
            this.lblShortcutMessage.Size = new System.Drawing.Size(49, 39);
            this.lblShortcutMessage.TabIndex = 8;
            this.lblShortcutMessage.Text = "Shortcut\r\n/ Default\r\n(if set):";
            this.lblShortcutMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // tmrDelay
            this.tmrDelay.Interval = 1000;
            this.tmrDelay.Tick += new System.EventHandler(this.tmrDelay_Tick);
            
            // cmOptions
            this.cmOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miEditMode,
                this.toolStripSeparator1,
                this.toolStripMenuItem2,
                this.toolStripMenuItem3});
            this.cmOptions.Name = "cmOptions";
            this.cmOptions.Size = new System.Drawing.Size(182, 76);
            
            // miEditMode
            this.miEditMode.Name = "miEditMode";
            this.miEditMode.Size = new System.Drawing.Size(181, 22);
            this.miEditMode.Text = "&Edit";
            this.miEditMode.Click += new System.EventHandler(this.miEditMode_Click);
            
            // toolStripSeparator1
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            
            // toolStripMenuItem2
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem2.Text = "ToolStripMenuItem2";
            
            // toolStripMenuItem3
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem3.Text = "ToolStripMenuItem3";
            
            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gold;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(542, 157);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.lblShortcutMessage);
            this.Controls.Add(this.btnCopyToClipboardAndClose);
            this.Controls.Add(this.chkAutoOpen);
            this.Controls.Add(this.chkAutoClose);
            this.Controls.Add(this.btnCopyToClipboard);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose a Browser";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.cmOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
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
        private System.Windows.Forms.Label lblShortcutMessage;
    }
}
