using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            labelProductName = new Label();
            labelVersion = new Label();
            labelCopyright = new Label();
            llHome = new LinkLabel();
            llLicense = new LinkLabel();
            btnCopy = new Button();
            tabControl1 = new TabControl();
            tpInfo = new TabPage();
            lblForkedFrom = new Label();
            lblThisVersionHere = new Label();
            lblOriginalVersion = new LinkLabel();
            tpContrib = new TabPage();
            lblContributors = new Label();
            tpAttributions = new TabPage();
            lblAttributions = new Label();
            llSebCboLb = new LinkLabel();
            tpDiagnostics = new TabPage();
            cmdSaveLogs = new Button();
            lblDiagnostics = new Label();
            OKButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabControl1.SuspendLayout();
            tpInfo.SuspendLayout();
            tpContrib.SuspendLayout();
            tpAttributions.SuspendLayout();
            tpDiagnostics.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.ErrorImage = Properties.Resources.BrowserChooserIcon;
            pictureBox1.InitialImage = Properties.Resources.BrowserChooserIcon;
            pictureBox1.Location = new Point(38, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(612, 273);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.ErrorImage = Properties.Resources.BrowserChooser3;
            pictureBox2.InitialImage = Properties.Resources.BrowserChooser3;
            pictureBox2.Location = new Point(12, 66);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(48, 48);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // labelProductName
            // 
            labelProductName.AutoSize = true;
            labelProductName.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelProductName.Location = new Point(66, 12);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new Size(120, 21);
            labelProductName.TabIndex = 2;
            labelProductName.Text = "BrowserChooser3";
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelVersion.Location = new Point(66, 33);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(45, 13);
            labelVersion.TabIndex = 3;
            labelVersion.Text = "Version 3.0.0";
            // 
            // labelCopyright
            // 
            labelCopyright.AutoSize = true;
            labelCopyright.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelCopyright.Location = new Point(66, 46);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new Size(200, 13);
            labelCopyright.TabIndex = 4;
            labelCopyright.Text = "Copyright © 2024 BrowserChooser3 Team";
            // 
            // llHome
            // 
            llHome.AutoSize = true;
            llHome.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            llHome.Location = new Point(66, 66);
            llHome.Name = "llHome";
            llHome.Size = new Size(35, 13);
            llHome.TabIndex = 5;
            llHome.TabStop = true;
            llHome.Text = "Home";
            llHome.LinkClicked += llHome_LinkClicked;
            // 
            // llLicense
            // 
            llLicense.AutoSize = true;
            llLicense.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            llLicense.Location = new Point(66, 79);
            llLicense.Name = "llLicense";
            llLicense.Size = new Size(44, 13);
            llLicense.TabIndex = 6;
            llLicense.TabStop = true;
            llLicense.Text = "License";
            llLicense.LinkClicked += llLicense_LinkClicked;
            // 
            // btnCopy
            // 
            btnCopy.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCopy.Location = new Point(216, 194);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(140, 32);
            btnCopy.TabIndex = 7;
            btnCopy.Text = "Copy to Clipboard";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tpInfo);
            tabControl1.Controls.Add(tpContrib);
            tabControl1.Controls.Add(tpAttributions);
            tabControl1.Controls.Add(tpDiagnostics);
            tabControl1.Location = new Point(38, 295);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(600, 300);
            tabControl1.TabIndex = 8;
            // 
            // tpInfo
            // 
            tpInfo.Controls.Add(lblForkedFrom);
            tpInfo.Controls.Add(lblThisVersionHere);
            tpInfo.Controls.Add(lblOriginalVersion);
            tpInfo.Location = new Point(4, 32);
            tpInfo.Name = "tpInfo";
            tpInfo.Padding = new Padding(3);
            tpInfo.Size = new Size(592, 264);
            tpInfo.TabIndex = 0;
            tpInfo.Text = "Information";
            tpInfo.UseVisualStyleBackColor = true;
            // 
            // lblForkedFrom
            // 
            lblForkedFrom.AutoSize = true;
            lblForkedFrom.Location = new Point(3, 12);
            lblForkedFrom.Name = "lblForkedFrom";
            lblForkedFrom.Size = new Size(106, 23);
            lblForkedFrom.TabIndex = 0;
            lblForkedFrom.Text = "Forked from:";
            // 
            // lblThisVersionHere
            // 
            lblThisVersionHere.AutoSize = true;
            lblThisVersionHere.Location = new Point(3, 35);
            lblThisVersionHere.Name = "lblThisVersionHere";
            lblThisVersionHere.Size = new Size(303, 23);
            lblThisVersionHere.TabIndex = 1;
            lblThisVersionHere.Text = "Latest version: BrowserChooser3 v3.0.0";
            // 
            // lblOriginalVersion
            // 
            lblOriginalVersion.AutoSize = true;
            lblOriginalVersion.Location = new Point(79, 12);
            lblOriginalVersion.Name = "lblOriginalVersion";
            lblOriginalVersion.Size = new Size(152, 23);
            lblOriginalVersion.TabIndex = 2;
            lblOriginalVersion.TabStop = true;
            lblOriginalVersion.Text = "Browser Chooser 2";
            lblOriginalVersion.LinkClicked += lblOriginalVersion_LinkClicked;
            // 
            // tpContrib
            // 
            tpContrib.Controls.Add(lblContributors);
            tpContrib.Location = new Point(4, 34);
            tpContrib.Name = "tpContrib";
            tpContrib.Padding = new Padding(3);
            tpContrib.Size = new Size(592, 262);
            tpContrib.TabIndex = 1;
            tpContrib.Text = "Contributors";
            tpContrib.UseVisualStyleBackColor = true;
            // 
            // lblContributors
            // 
            lblContributors.AutoSize = true;
            lblContributors.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContributors.Location = new Point(3, 12);
            lblContributors.Name = "lblContributors";
            lblContributors.Size = new Size(505, 300);
            lblContributors.TabIndex = 0;
            lblContributors.Text = resources.GetString("lblContributors.Text");
            // 
            // tpAttributions
            // 
            tpAttributions.Controls.Add(lblAttributions);
            tpAttributions.Controls.Add(llSebCboLb);
            tpAttributions.Location = new Point(4, 34);
            tpAttributions.Name = "tpAttributions";
            tpAttributions.Padding = new Padding(3);
            tpAttributions.Size = new Size(592, 262);
            tpAttributions.TabIndex = 2;
            tpAttributions.Text = "Attributions";
            tpAttributions.UseVisualStyleBackColor = true;
            // 
            // lblAttributions
            // 
            lblAttributions.AutoSize = true;
            lblAttributions.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAttributions.Location = new Point(3, 12);
            lblAttributions.Name = "lblAttributions";
            lblAttributions.Size = new Size(543, 200);
            lblAttributions.TabIndex = 0;
            lblAttributions.Text = resources.GetString("lblAttributions.Text");
            // 
            // llSebCboLb
            // 
            llSebCboLb.AutoSize = true;
            llSebCboLb.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            llSebCboLb.Location = new Point(3, 100);
            llSebCboLb.Name = "llSebCboLb";
            llSebCboLb.Size = new Size(173, 25);
            llSebCboLb.TabIndex = 1;
            llSebCboLb.TabStop = true;
            llSebCboLb.Text = "GitHub Contributors";
            llSebCboLb.LinkClicked += llSebCboLb_LinkClicked;
            // 
            // tpDiagnostics
            // 
            tpDiagnostics.Controls.Add(cmdSaveLogs);
            tpDiagnostics.Controls.Add(btnCopy);
            tpDiagnostics.Controls.Add(lblDiagnostics);
            tpDiagnostics.Location = new Point(4, 34);
            tpDiagnostics.Name = "tpDiagnostics";
            tpDiagnostics.Padding = new Padding(3);
            tpDiagnostics.Size = new Size(592, 262);
            tpDiagnostics.TabIndex = 3;
            tpDiagnostics.Text = "Diagnostics";
            tpDiagnostics.UseVisualStyleBackColor = true;
            // 
            // cmdSaveLogs
            // 
            cmdSaveLogs.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdSaveLogs.Location = new Point(216, 194);
            cmdSaveLogs.Name = "cmdSaveLogs";
            cmdSaveLogs.Size = new Size(140, 32);
            cmdSaveLogs.TabIndex = 12;
            cmdSaveLogs.Text = "Enable Logging";
            cmdSaveLogs.UseVisualStyleBackColor = true;
            cmdSaveLogs.Click += cmdSaveLogs_Click;
            // 
            // lblDiagnostics
            // 
            lblDiagnostics.BackColor = Color.White;
            lblDiagnostics.Font = new Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDiagnostics.ForeColor = Color.Navy;
            lblDiagnostics.Location = new Point(5, 5);
            lblDiagnostics.Name = "lblDiagnostics";
            lblDiagnostics.Size = new Size(511, 224);
            lblDiagnostics.TabIndex = 13;
            lblDiagnostics.Text = "Diagnostic information will be displayed here...";
            // 
            // OKButton
            // 
            OKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OKButton.DialogResult = DialogResult.OK;
            OKButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OKButton.Location = new Point(587, 603);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(80, 28);
            OKButton.TabIndex = 9;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // AboutForm
            // 
            AcceptButton = OKButton;
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = OKButton;
            ClientSize = new Size(678, 644);
            Controls.Add(tabControl1);
            Controls.Add(OKButton);
            Controls.Add(pictureBox1);
            Font = new Font("Segoe UI", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            Padding = new Padding(9);
            RightToLeft = RightToLeft.No;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About BrowserChooser3";
            TopMost = true;
            Load += AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabControl1.ResumeLayout(false);
            tpInfo.ResumeLayout(false);
            tpInfo.PerformLayout();
            tpContrib.ResumeLayout(false);
            tpContrib.PerformLayout();
            tpAttributions.ResumeLayout(false);
            tpAttributions.PerformLayout();
            tpDiagnostics.ResumeLayout(false);
            ResumeLayout(false);
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
                // pictureBox1のアイコン読み込み（BrowserChooser3アイコン）
                pictureBox1.Image = Properties.Resources.BrowserChooserIcon;

                // pictureBox2のアイコン読み込み（BCLogoアイコン）
                pictureBox2.Image = Properties.Resources.BCLogoIcon.ToBitmap();

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
                // テスト環境ではブラウザを開かない
                if (IsTestEnvironment())
                {
                    return;
                }

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
                // テスト環境ではブラウザを開かない
                if (IsTestEnvironment())
                {
                    return;
                }

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
                // テスト環境ではブラウザを開かない
                if (IsTestEnvironment())
                {
                    return;
                }

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
                // テスト環境ではブラウザを開かない
                if (IsTestEnvironment())
                {
                    return;
                }

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

        /// <summary>
        /// テスト環境かどうかを判定
        /// </summary>
        private bool IsTestEnvironment()
        {
            return Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") == "true" ||
                   Environment.GetEnvironmentVariable("DISABLE_HELP") == "true" ||
                   System.Diagnostics.Process.GetCurrentProcess().ProcessName.Contains("test", StringComparison.OrdinalIgnoreCase);
        }


    }
}
