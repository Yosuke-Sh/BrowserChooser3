using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// About画面フォーム
    /// アプリケーション情報、貢献者、診断情報を表示します
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// AboutFormの新しいインスタンスを初期化します
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            
            // メインコントロール
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.llHome = new System.Windows.Forms.LinkLabel();
            this.llLicense = new System.Windows.Forms.LinkLabel();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpInfo = new System.Windows.Forms.TabPage();
            this.tpContrib = new System.Windows.Forms.TabPage();
            this.tpAttributions = new System.Windows.Forms.TabPage();
            this.tpDiagnostics = new System.Windows.Forms.TabPage();
            this.lblDiagnostics = new System.Windows.Forms.Label();
            this.lblContributors = new System.Windows.Forms.Label();
            this.llSebCboLb = new System.Windows.Forms.LinkLabel();

            this.lblAttributions = new System.Windows.Forms.Label();
            this.cmdSaveLogs = new System.Windows.Forms.Button();
            this.lblForkedFrom = new System.Windows.Forms.Label();
            this.lblThisVersionHere = new System.Windows.Forms.Label();
            this.lblOriginalVersion = new System.Windows.Forms.LinkLabel();
            this.OKButton = new System.Windows.Forms.Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpInfo.SuspendLayout();
            this.tpContrib.SuspendLayout();
            this.tpAttributions.SuspendLayout();
            this.tpDiagnostics.SuspendLayout();
            this.SuspendLayout();
            
            // pictureBox1
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            

            
            // pictureBox2
            this.pictureBox2.Location = new System.Drawing.Point(12, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(48, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            

            
            // labelProductName
            this.labelProductName.AutoSize = true;
            this.labelProductName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelProductName.Location = new System.Drawing.Point(66, 12);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(120, 21);
            this.labelProductName.TabIndex = 2;
            this.labelProductName.Text = "BrowserChooser3";
            
            // labelVersion
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(66, 33);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(45, 13);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version 3.0.0";
            
            // labelCopyright
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(66, 46);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(200, 13);
            this.labelCopyright.TabIndex = 4;
            this.labelCopyright.Text = "Copyright © 2024 BrowserChooser3 Team";
            
            // llHome
            this.llHome.AutoSize = true;
            this.llHome.Location = new System.Drawing.Point(66, 66);
            this.llHome.Name = "llHome";
            this.llHome.Size = new System.Drawing.Size(35, 13);
            this.llHome.TabIndex = 5;
            this.llHome.TabStop = true;
            this.llHome.Text = "Home";
            this.llHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llHome_LinkClicked);
            
            // llLicense
            this.llLicense.AutoSize = true;
            this.llLicense.Location = new System.Drawing.Point(66, 79);
            this.llLicense.Name = "llLicense";
            this.llLicense.Size = new System.Drawing.Size(44, 13);
            this.llLicense.TabIndex = 6;
            this.llLicense.TabStop = true;
            this.llLicense.Text = "License";
            this.llLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llLicense_LinkClicked);
            
            // btnCopy
            this.btnCopy.Location = new System.Drawing.Point(216, 194);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(133, 32);
            this.btnCopy.TabIndex = 7;
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            
            // tabControl1
            this.tabControl1.Controls.Add(this.tpInfo);
            this.tabControl1.Controls.Add(this.tpContrib);
            this.tabControl1.Controls.Add(this.tpAttributions);
            this.tabControl1.Controls.Add(this.tpDiagnostics);
            this.tabControl1.Location = new System.Drawing.Point(12, 120);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(527, 265);
            this.tabControl1.TabIndex = 8;
            
            // tpInfo
            this.tpInfo.Controls.Add(this.lblForkedFrom);
            this.tpInfo.Controls.Add(this.lblThisVersionHere);
            this.tpInfo.Controls.Add(this.lblOriginalVersion);
            this.tpInfo.Location = new System.Drawing.Point(4, 22);
            this.tpInfo.Name = "tpInfo";
            this.tpInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tpInfo.Size = new System.Drawing.Size(519, 239);
            this.tpInfo.TabIndex = 0;
            this.tpInfo.Text = "Information";
            this.tpInfo.UseVisualStyleBackColor = true;
            
            // lblForkedFrom
            this.lblForkedFrom.AutoSize = true;
            this.lblForkedFrom.Location = new System.Drawing.Point(3, 12);
            this.lblForkedFrom.Name = "lblForkedFrom";
            this.lblForkedFrom.Size = new System.Drawing.Size(70, 13);
            this.lblForkedFrom.TabIndex = 0;
            this.lblForkedFrom.Text = "Forked from:";
            
            // lblThisVersionHere
            this.lblThisVersionHere.AutoSize = true;
            this.lblThisVersionHere.Location = new System.Drawing.Point(3, 35);
            this.lblThisVersionHere.Name = "lblThisVersionHere";
            this.lblThisVersionHere.Size = new System.Drawing.Size(90, 13);
            this.lblThisVersionHere.TabIndex = 1;
            this.lblThisVersionHere.Text = "Latest version: BrowserChooser3 v3.0.0";
            
            // lblOriginalVersion
            this.lblOriginalVersion.AutoSize = true;
            this.lblOriginalVersion.Location = new System.Drawing.Point(79, 12);
            this.lblOriginalVersion.Name = "lblOriginalVersion";
            this.lblOriginalVersion.Size = new System.Drawing.Size(120, 13);
            this.lblOriginalVersion.TabIndex = 2;
            this.lblOriginalVersion.TabStop = true;
            this.lblOriginalVersion.Text = "Browser Chooser 2";
            this.lblOriginalVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblOriginalVersion_LinkClicked);
            
            // tpContrib
            this.tpContrib.Controls.Add(this.lblContributors);
            this.tpContrib.Location = new System.Drawing.Point(4, 22);
            this.tpContrib.Name = "tpContrib";
            this.tpContrib.Padding = new System.Windows.Forms.Padding(3);
            this.tpContrib.Size = new System.Drawing.Size(519, 239);
            this.tpContrib.TabIndex = 1;
            this.tpContrib.Text = "Contributors";
            this.tpContrib.UseVisualStyleBackColor = true;
            
            // lblContributors
            this.lblContributors.AutoSize = true;
            this.lblContributors.Location = new System.Drawing.Point(3, 12);
            this.lblContributors.Name = "lblContributors";
            this.lblContributors.Size = new System.Drawing.Size(300, 150);
            this.lblContributors.TabIndex = 0;
            this.lblContributors.Text = @"BrowserChooser3 Development Team
• Main Developer - C# Port and Enhancement
• Original Browser Chooser 2 Authors
• .NET Community Contributors
• Icon and UI Design Contributors

Special Thanks to:
• Browser Chooser 2 original team for the excellent foundation
• Microsoft for the .NET platform
• Open source community for feedback and contributions

Join us on GitHub to contribute to BrowserChooser3!";
            
            // tpAttributions
            this.tpAttributions.Controls.Add(this.lblAttributions);
            this.tpAttributions.Controls.Add(this.llSebCboLb);
            this.tpAttributions.Location = new System.Drawing.Point(4, 22);
            this.tpAttributions.Name = "tpAttributions";
            this.tpAttributions.Padding = new System.Windows.Forms.Padding(3);
            this.tpAttributions.Size = new System.Drawing.Size(519, 239);
            this.tpAttributions.TabIndex = 2;
            this.tpAttributions.Text = "Attributions";
            this.tpAttributions.UseVisualStyleBackColor = true;
            
            // lblAttributions
            this.lblAttributions.AutoSize = true;
            this.lblAttributions.Location = new System.Drawing.Point(3, 12);
            this.lblAttributions.Name = "lblAttributions";
            this.lblAttributions.Size = new System.Drawing.Size(400, 80);
            this.lblAttributions.TabIndex = 0;
            this.lblAttributions.Text = @"BrowserChooser3 uses the following third-party components:

• .NET 8.0 Framework - Microsoft Corporation
• Windows Forms - Microsoft Corporation  
• System.Drawing - Microsoft Corporation
• Icon resources from Browser Chooser 2 project

All third-party components are used under their respective licenses.";
            
            // llSebCboLb
            this.llSebCboLb.AutoSize = true;
            this.llSebCboLb.Location = new System.Drawing.Point(3, 100);
            this.llSebCboLb.Name = "llSebCboLb";
            this.llSebCboLb.Size = new System.Drawing.Size(120, 13);
            this.llSebCboLb.TabIndex = 1;
            this.llSebCboLb.TabStop = true;
            this.llSebCboLb.Text = "GitHub Contributors";
            this.llSebCboLb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llSebCboLb_LinkClicked);
            

            
            // tpDiagnostics
            this.tpDiagnostics.Controls.Add(this.cmdSaveLogs);
            this.tpDiagnostics.Controls.Add(this.btnCopy);
            this.tpDiagnostics.Controls.Add(this.lblDiagnostics);
            this.tpDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tpDiagnostics.Name = "tpDiagnostics";
            this.tpDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tpDiagnostics.Size = new System.Drawing.Size(519, 239);
            this.tpDiagnostics.TabIndex = 3;
            this.tpDiagnostics.Text = "Diagnostics";
            this.tpDiagnostics.UseVisualStyleBackColor = true;
            
            // lblDiagnostics
            this.lblDiagnostics.BackColor = System.Drawing.Color.White;
            this.lblDiagnostics.ForeColor = System.Drawing.Color.Navy;
            this.lblDiagnostics.Location = new System.Drawing.Point(5, 5);
            this.lblDiagnostics.Name = "lblDiagnostics";
            this.lblDiagnostics.Size = new System.Drawing.Size(511, 224);
            this.lblDiagnostics.TabIndex = 13;
            this.lblDiagnostics.Text = "Diagnostic information will be displayed here...";
            
            // cmdSaveLogs
            this.cmdSaveLogs.Location = new System.Drawing.Point(216, 194);
            this.cmdSaveLogs.Name = "cmdSaveLogs";
            this.cmdSaveLogs.Size = new System.Drawing.Size(133, 32);
            this.cmdSaveLogs.TabIndex = 12;
            this.cmdSaveLogs.Text = "Enable Logging";
            this.cmdSaveLogs.UseVisualStyleBackColor = true;
            this.cmdSaveLogs.Click += new System.EventHandler(this.cmdSaveLogs_Click);
            
            // OKButton
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(464, 391);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 9;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            
            // AboutForm
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.OKButton;
            this.ClientSize = new System.Drawing.Size(555, 426);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About BrowserChooser3";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AboutForm_Load);
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpInfo.ResumeLayout(false);
            this.tpInfo.PerformLayout();
            this.tpContrib.ResumeLayout(false);
            this.tpAttributions.ResumeLayout(false);
            this.tpAttributions.PerformLayout();
            this.tpDiagnostics.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox pictureBox1 = null!;
        private System.Windows.Forms.LinkLabel llHome = null!;
        private System.Windows.Forms.PictureBox pictureBox2 = null!;
        private System.Windows.Forms.Label labelProductName = null!;
        private System.Windows.Forms.Label labelVersion = null!;
        private System.Windows.Forms.Label labelCopyright = null!;
        private System.Windows.Forms.LinkLabel llLicense = null!;
        private System.Windows.Forms.Button btnCopy = null!;
        private System.Windows.Forms.TabControl tabControl1 = null!;
        private System.Windows.Forms.TabPage tpInfo = null!;
        private System.Windows.Forms.TabPage tpContrib = null!;
        private System.Windows.Forms.TabPage tpAttributions = null!;
        private System.Windows.Forms.TabPage tpDiagnostics = null!;
        private System.Windows.Forms.Label lblDiagnostics = null!;
        private System.Windows.Forms.Label lblContributors = null!;
        private System.Windows.Forms.LinkLabel llSebCboLb = null!;

        private System.Windows.Forms.Label lblAttributions = null!;
        private System.Windows.Forms.Button cmdSaveLogs = null!;
        private System.Windows.Forms.Label lblForkedFrom = null!;
        private System.Windows.Forms.Label lblThisVersionHere = null!;
        private System.Windows.Forms.LinkLabel lblOriginalVersion = null!;
        private System.Windows.Forms.Button OKButton = null!;
        private System.ComponentModel.IContainer components = null!;

        /// <summary>
        /// フォーム読み込み時の処理
        /// </summary>
        private void AboutForm_Load(object? sender, EventArgs e)
        {
            Logger.LogInfo("AboutForm.AboutForm_Load", "About画面を読み込み");
            
            try
            {
                // アイコンの読み込み
                LoadIcons();
                
                // 診断情報を表示
                var diagnostics = new System.Text.StringBuilder();
                diagnostics.AppendLine("BrowserChooser3 Diagnostic Information");
                diagnostics.AppendLine("=====================================");
                diagnostics.AppendLine($"Application: BrowserChooser3 v3.0.0");
                diagnostics.AppendLine($"Framework: .NET 8.0");
                diagnostics.AppendLine($"OS Version: {Environment.OSVersion}");
                diagnostics.AppendLine($".NET Runtime Version: {Environment.Version}");
                diagnostics.AppendLine($"Working Directory: {Environment.CurrentDirectory}");
                diagnostics.AppendLine($"User Name: {Environment.UserName}");
                diagnostics.AppendLine($"Machine Name: {Environment.MachineName}");
                diagnostics.AppendLine($"Processor Count: {Environment.ProcessorCount}");
                diagnostics.AppendLine($"System Page Size: {Environment.SystemPageSize}");
                diagnostics.AppendLine($"Is 64-bit Process: {Environment.Is64BitProcess}");
                diagnostics.AppendLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
                diagnostics.AppendLine($"CLR Version: {Environment.Version}");
                diagnostics.AppendLine($"System Directory: {Environment.SystemDirectory}");
                diagnostics.AppendLine($"Tick Count: {Environment.TickCount}");
                diagnostics.AppendLine();
                diagnostics.AppendLine("Configuration:");
                diagnostics.AppendLine($"Portable Mode: {Settings.Current.PortableMode}");
                diagnostics.AppendLine($"Logging Enabled: {Settings.Current.EnableLogging}");
                diagnostics.AppendLine($"Browser Count: {Settings.Current.Browsers.Count}");
                diagnostics.AppendLine($"Protocol Count: {Settings.Current.Protocols.Count}");
                diagnostics.AppendLine($"File Type Count: {Settings.Current.FileTypes.Count}");
                
                lblDiagnostics.Text = diagnostics.ToString();
                
                Logger.LogInfo("AboutForm.AboutForm_Load", "About画面読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.AboutForm_Load", "About画面読み込みエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// アイコンの読み込み
        /// </summary>
        private void LoadIcons()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                // pictureBox1のアイコン読み込み
                using var stream1 = assembly.GetManifestResourceStream("BrowserChooser3.Resources.BrowserChooser2.ico");
                if (stream1 != null)
                {
                    pictureBox1.Image = Image.FromStream(stream1);
                }
                
                // pictureBox2のアイコン読み込み
                using var stream2 = assembly.GetManifestResourceStream("BrowserChooser3.Resources.bclogo.ico");
                if (stream2 != null)
                {
                    pictureBox2.Image = Image.FromStream(stream2);
                }
                
                Logger.LogInfo("AboutForm.LoadIcons", "アイコン読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.LoadIcons", "アイコン読み込みエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// ホームリンクのクリックイベント
        /// </summary>
        private void llHome_LinkClicked(object? sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Logger.LogInfo("AboutForm.llHome_LinkClicked", "ホームページを開く");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BrowserChooser/BrowserChooser3",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.llHome_LinkClicked", "ホームページを開けませんでした", ex.Message);
            }
        }

        /// <summary>
        /// ライセンスリンクのクリックイベント
        /// </summary>
        private void llLicense_LinkClicked(object? sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Logger.LogInfo("AboutForm.llLicense_LinkClicked", "ライセンスページを開く");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BrowserChooser/BrowserChooser3/blob/main/LICENSE",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.llLicense_LinkClicked", "ライセンスページを開けませんでした", ex.Message);
            }
        }

        /// <summary>
        /// コピーボタンのクリックイベント
        /// </summary>
        private void btnCopy_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("AboutForm.btnCopy_Click", "診断情報をクリップボードにコピー");
            try
            {
                Clipboard.SetText(lblDiagnostics.Text);
                MessageBox.Show("診断情報をクリップボードにコピーしました", "情報", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.btnCopy_Click", "クリップボードコピーエラー", ex.Message);
                MessageBox.Show($"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ログ保存ボタンのクリックイベント
        /// </summary>
        private void cmdSaveLogs_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("AboutForm.cmdSaveLogs_Click", "ログ機能を有効化");
            try
            {
                Settings.Current.EnableLogging = true;
                Settings.Current.LogLevel = 3; // Info level
                Settings.Current.DoSave();
                Logger.InitializeLogLevel(3);
                MessageBox.Show("ログ機能を有効化しました", "情報", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.cmdSaveLogs_Click", "ログ機能有効化エラー", ex.Message);
                MessageBox.Show($"ログ機能の有効化に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 元バージョンリンクのクリックイベント
        /// </summary>
        private void lblOriginalVersion_LinkClicked(object? sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Logger.LogInfo("AboutForm.lblOriginalVersion_LinkClicked", "元バージョンのページを開く");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://bitbucket.org/Verbail/browserchooser2rrfork",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.lblOriginalVersion_LinkClicked", "元バージョンのページを開けませんでした", ex.Message);
            }
        }

        /// <summary>
        /// SebCboLbリンクのクリックイベント
        /// </summary>
        private void llSebCboLb_LinkClicked(object? sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Logger.LogInfo("AboutForm.llSebCboLb_LinkClicked", "SebCboLbのページを開く");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/BrowserChooser/BrowserChooser3/contributors",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("AboutForm.llSebCboLb_LinkClicked", "SebCboLbのページを開けませんでした", ex.Message);
            }
        }


    }
}
