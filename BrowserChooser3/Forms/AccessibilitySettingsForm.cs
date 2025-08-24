using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// アクセシビリティ設定フォーム
    /// 視覚的フォーカス表示の設定を管理します
    /// </summary>
    public partial class AccessibilitySettingsForm : Form
    {
        private Settings _settings;

        /// <summary>
        /// AccessibilitySettingsFormの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public AccessibilitySettingsForm(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // メインコントロール
            this.chkShowVisualFocus = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.cmdChange = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.cdChooseBoxColor = new System.Windows.Forms.ColorDialog();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            this.SuspendLayout();
            
            // chkShowVisualFocus
            this.chkShowVisualFocus.AutoSize = true;
            this.chkShowVisualFocus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.chkShowVisualFocus.Location = new System.Drawing.Point(12, 12);
            this.chkShowVisualFocus.Name = "chkShowVisualFocus";
            this.chkShowVisualFocus.Size = new System.Drawing.Size(116, 17);
            this.chkShowVisualFocus.TabIndex = 2;
            this.chkShowVisualFocus.Text = "Show Visual Focus";
            this.chkShowVisualFocus.UseVisualStyleBackColor = true;
            this.chkShowVisualFocus.CheckedChanged += new System.EventHandler(this.chkShowVisualFocus_CheckedChanged);
            
            // groupBox1
            this.groupBox1.Controls.Add(this.txtColor);
            this.groupBox1.Controls.Add(this.cmdChange);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudWidth);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 80);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customize the Focus";
            
            // txtColor
            this.txtColor.Enabled = false;
            this.txtColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.txtColor.Location = new System.Drawing.Point(76, 43);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(30, 23);
            this.txtColor.TabIndex = 3;
            
            // cmdChange
            this.cmdChange.Enabled = false;
            this.cmdChange.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.cmdChange.Location = new System.Drawing.Point(112, 41);
            this.cmdChange.Name = "cmdChange";
            this.cmdChange.Size = new System.Drawing.Size(80, 25);
            this.cmdChange.TabIndex = 4;
            this.cmdChange.Text = "&Change";
            this.cmdChange.UseVisualStyleBackColor = true;
            this.cmdChange.Click += new System.EventHandler(this.cmdChange_Click);
            
            // label2
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Box Color :";
            
            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.label1.Location = new System.Drawing.Point(11, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Box width :";
            
            // nudWidth
            this.nudWidth.Enabled = false;
            this.nudWidth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.nudWidth.Location = new System.Drawing.Point(76, 18);
            this.nudWidth.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.nudWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(110, 23);
            this.nudWidth.TabIndex = 1;
            this.nudWidth.Value = new decimal(new int[] { 2, 0, 0, 0 });
            
            // cmdOK
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.cmdOK.Location = new System.Drawing.Point(12, 125);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(80, 28);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            
            // cmdCancel
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.cmdCancel.Location = new System.Drawing.Point(102, 125);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(80, 28);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            
            // AccessibilitySettingsForm
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(220, 165);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkShowVisualFocus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessibilitySettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Accessibility Settings";
            
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckBox chkShowVisualFocus = null!;
        private System.Windows.Forms.GroupBox groupBox1 = null!;
        private System.Windows.Forms.TextBox txtColor = null!;
        private System.Windows.Forms.Button cmdChange = null!;
        private System.Windows.Forms.Label label2 = null!;
        private System.Windows.Forms.Label label1 = null!;
        private System.Windows.Forms.NumericUpDown nudWidth = null!;
        private System.Windows.Forms.ColorDialog cdChooseBoxColor = null!;
        private System.Windows.Forms.Button cmdOK = null!;
        private System.Windows.Forms.Button cmdCancel = null!;
        private System.ComponentModel.IContainer components = null!;

        /// <summary>
        /// 設定値を読み込み
        /// </summary>
        private void LoadSettings()
        {
            Logger.LogInfo("AccessibilitySettingsForm.LoadSettings", "Start");
            
            try
            {
                // 設定値を各コントロールに設定
                chkShowVisualFocus.Checked = _settings.ShowVisualFocus;
                nudWidth.Value = _settings.FocusBoxWidth;
                
                // 色の設定
                var color = Color.FromArgb(_settings.FocusBoxColor);
                txtColor.BackColor = color;
                txtColor.Text = color.Name;
                
                // コントロールの有効/無効を設定
                UpdateControlStates();
                
                Logger.LogInfo("AccessibilitySettingsForm.LoadSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("AccessibilitySettingsForm.LoadSettings", "設定読み込みエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// 設定値を保存
        /// </summary>
        private void SaveSettings()
        {
            Logger.LogInfo("AccessibilitySettingsForm.SaveSettings", "Start");
            
            try
            {
                // 各コントロールから設定値を取得して保存
                _settings.ShowVisualFocus = chkShowVisualFocus.Checked;
                _settings.FocusBoxWidth = (int)nudWidth.Value;
                _settings.FocusBoxColor = txtColor.BackColor.ToArgb();
                
                Logger.LogInfo("AccessibilitySettingsForm.SaveSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("AccessibilitySettingsForm.SaveSettings", "設定保存エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// コントロールの有効/無効状態を更新
        /// </summary>
        private void UpdateControlStates()
        {
            bool enabled = chkShowVisualFocus.Checked;
            nudWidth.Enabled = enabled;
            txtColor.Enabled = enabled;
            cmdChange.Enabled = enabled;
        }

        /// <summary>
        /// 視覚的フォーカス表示チェックボックスの変更イベント
        /// </summary>
        private void chkShowVisualFocus_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("AccessibilitySettingsForm.chkShowVisualFocus_CheckedChanged", $"視覚的フォーカス表示: {chkShowVisualFocus.Checked}");
            UpdateControlStates();
        }

        /// <summary>
        /// 色変更ボタンのクリックイベント
        /// </summary>
        private void cmdChange_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("AccessibilitySettingsForm.cmdChange_Click", "色変更ダイアログを開く");
            
            try
            {
                cdChooseBoxColor.Color = txtColor.BackColor;
                if (cdChooseBoxColor.ShowDialog() == DialogResult.OK)
                {
                    txtColor.BackColor = cdChooseBoxColor.Color;
                    txtColor.Text = cdChooseBoxColor.Color.Name;
                    Logger.LogInfo("AccessibilitySettingsForm.cmdChange_Click", "色を変更", cdChooseBoxColor.Color.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("AccessibilitySettingsForm.cmdChange_Click", "色変更エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void cmdOK_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("AccessibilitySettingsForm.cmdOK_Click", "設定を保存");
            SaveSettings();
        }
    }
}

