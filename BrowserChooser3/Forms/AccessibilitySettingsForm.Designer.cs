namespace BrowserChooser3.Forms
{
    partial class AccessibilitySettingsForm
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
            this.chkShowFocus = new System.Windows.Forms.CheckBox();
            this.lblFocusColor = new System.Windows.Forms.Label();
            this.pbFocusColor = new System.Windows.Forms.Panel();
            this.lblFocusWidth = new System.Windows.Forms.Label();
            this.nudFocusWidth = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudFocusWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // chkShowFocus
            // 
            this.chkShowFocus.AutoSize = true;
            this.chkShowFocus.Location = new System.Drawing.Point(20, 20);
            this.chkShowFocus.Name = "chkShowFocus";
            this.chkShowFocus.Size = new System.Drawing.Size(95, 19);
            this.chkShowFocus.TabIndex = 0;
            this.chkShowFocus.Text = "Show Focus Box";
            this.chkShowFocus.UseVisualStyleBackColor = true;
            // 
            // lblFocusColor
            // 
            this.lblFocusColor.AutoSize = true;
            this.lblFocusColor.Location = new System.Drawing.Point(20, 60);
            this.lblFocusColor.Name = "lblFocusColor";
            this.lblFocusColor.Size = new System.Drawing.Size(85, 15);
            this.lblFocusColor.TabIndex = 1;
            this.lblFocusColor.Text = "Focus Box Color:";
            // 
            // pbFocusColor
            // 
            this.pbFocusColor.BackColor = System.Drawing.Color.Red;
            this.pbFocusColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFocusColor.Location = new System.Drawing.Point(120, 57);
            this.pbFocusColor.Name = "pbFocusColor";
            this.pbFocusColor.Size = new System.Drawing.Size(50, 23);
            this.pbFocusColor.TabIndex = 2;
            this.pbFocusColor.Click += new System.EventHandler(this.pbFocusColor_Click);
            // 
            // lblFocusWidth
            // 
            this.lblFocusWidth.AutoSize = true;
            this.lblFocusWidth.Location = new System.Drawing.Point(20, 100);
            this.lblFocusWidth.Name = "lblFocusWidth";
            this.lblFocusWidth.Size = new System.Drawing.Size(89, 15);
            this.lblFocusWidth.TabIndex = 3;
            this.lblFocusWidth.Text = "Focus Box Width:";
            // 
            // nudFocusWidth
            // 
            this.nudFocusWidth.Location = new System.Drawing.Point(120, 97);
            this.nudFocusWidth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudFocusWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFocusWidth.Name = "nudFocusWidth";
            this.nudFocusWidth.Size = new System.Drawing.Size(80, 23);
            this.nudFocusWidth.TabIndex = 4;
            this.nudFocusWidth.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(200, 220);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(285, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AccessibilitySettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.nudFocusWidth);
            this.Controls.Add(this.lblFocusWidth);
            this.Controls.Add(this.pbFocusColor);
            this.Controls.Add(this.lblFocusColor);
            this.Controls.Add(this.chkShowFocus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessibilitySettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Accessibility Settings";
            ((System.ComponentModel.ISupportInitialize)(this.nudFocusWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowFocus;
        private System.Windows.Forms.Label lblFocusColor;
        private System.Windows.Forms.Panel pbFocusColor;
        private System.Windows.Forms.Label lblFocusWidth;
        private System.Windows.Forms.NumericUpDown nudFocusWidth;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
