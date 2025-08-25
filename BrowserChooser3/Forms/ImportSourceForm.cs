using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// 設定インポートフォーム
    /// 外部設定ファイルのインポート機能を提供します
    /// </summary>
    public partial class ImportSourceForm : Form
    {
        private string _importPath = string.Empty;
        private bool _importSuccessful = false;

        /// <summary>
        /// インポートパス
        /// </summary>
        public string ImportPath => _importPath;

        /// <summary>
        /// インポートが成功したかどうか
        /// </summary>
        public bool ImportSuccessful => _importSuccessful;

        /// <summary>
        /// 設定インポートフォームクラスの新しいインスタンスを初期化します
        /// </summary>
        public ImportSourceForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Import Settings";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;

            // 説明ラベル
            var lblDescription = new Label
            {
                Name = "lblDescription",
                Location = new Point(10, 10),
                Size = new Size(460, 40),
                Text = "Select a settings file to import. This will merge the settings with your current configuration."
            };

            // ファイルパスラベル
            var lblFilePath = new Label
            {
                Name = "lblFilePath",
                Location = new Point(10, 60),
                Size = new Size(80, 20),
                Text = "File Path:"
            };

            // ファイルパステキストボックス
            var txtFilePath = new TextBox
            {
                Name = "txtFilePath",
                Location = new Point(10, 80),
                Size = new Size(350, 20),
                ReadOnly = true
            };

            // 参照ボタン
            var btnBrowse = new Button
            {
                Name = "btnBrowse",
                Text = "Browse...",
                Location = new Point(370, 78),
                Size = new Size(80, 25)
            };
            btnBrowse.Click += BtnBrowse_Click;

            // インポートオプショングループ
            var grpOptions = new GroupBox
            {
                Name = "grpOptions",
                Text = "Import Options",
                Location = new Point(10, 110),
                Size = new Size(460, 100)
            };

            // ブラウザ設定チェックボックス
            var chkImportBrowsers = new CheckBox
            {
                Name = "chkImportBrowsers",
                Text = "Import Browser Settings",
                Location = new Point(10, 20),
                Size = new Size(150, 20),
                Checked = true
            };

            // URL設定チェックボックス
            var chkImportURLs = new CheckBox
            {
                Name = "chkImportURLs",
                Text = "Import URL Settings",
                Location = new Point(10, 40),
                Size = new Size(150, 20),
                Checked = true
            };

            // プロトコル設定チェックボックス
            var chkImportProtocols = new CheckBox
            {
                Name = "chkImportProtocols",
                Text = "Import Protocol Settings",
                Location = new Point(10, 60),
                Size = new Size(150, 20),
                Checked = true
            };

            // ファイルタイプ設定チェックボックス
            var chkImportFileTypes = new CheckBox
            {
                Name = "chkImportFileTypes",
                Text = "Import File Type Settings",
                Location = new Point(200, 20),
                Size = new Size(150, 20),
                Checked = true
            };

            // 一般設定チェックボックス
            var chkImportGeneral = new CheckBox
            {
                Name = "chkImportGeneral",
                Text = "Import General Settings",
                Location = new Point(200, 40),
                Size = new Size(150, 20),
                Checked = true
            };

            // 上書きオプション
            var chkOverwrite = new CheckBox
            {
                Name = "chkOverwrite",
                Text = "Overwrite existing settings",
                Location = new Point(200, 60),
                Size = new Size(150, 20),
                Checked = false
            };

            // オプションをグループに追加
            grpOptions.Controls.Add(chkImportBrowsers);
            grpOptions.Controls.Add(chkImportURLs);
            grpOptions.Controls.Add(chkImportProtocols);
            grpOptions.Controls.Add(chkImportFileTypes);
            grpOptions.Controls.Add(chkImportGeneral);
            grpOptions.Controls.Add(chkOverwrite);

            // ステータスラベル
            var lblStatus = new Label
            {
                Name = "lblStatus",
                Location = new Point(10, 220),
                Size = new Size(460, 20),
                Text = "Ready to import settings."
            };

            // OKボタン
            var btnOK = new Button
            {
                Name = "btnOK",
                Text = "Import",
                Location = new Point(320, 220),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK,
                Enabled = false
            };
            btnOK.Click += BtnOK_Click;

            // キャンセルボタン
            var btnCancel = new Button
            {
                Name = "btnCancel",
                Text = "Cancel",
                Location = new Point(405, 220),
                Size = new Size(75, 25),
                DialogResult = DialogResult.Cancel
            };

            // コントロールを追加
            Controls.Add(lblDescription);
            Controls.Add(lblFilePath);
            Controls.Add(txtFilePath);
            Controls.Add(btnBrowse);
            Controls.Add(grpOptions);
            Controls.Add(lblStatus);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            // フォームのAcceptButtonとCancelButtonを設定
            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        /// <summary>
        /// 参照ボタンのクリックイベント
        /// </summary>
        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            try
            {
                using var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Settings File",
                    Filter = "Settings Files (*.xml)|*.xml|All Files (*.*)|*.*",
                    DefaultExt = "xml",
                    CheckFileExists = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var txtFilePath = Controls.Find("txtFilePath", true).FirstOrDefault() as TextBox;
                    var btnOK = Controls.Find("btnOK", true).FirstOrDefault() as Button;
                    var lblStatus = Controls.Find("lblStatus", true).FirstOrDefault() as Label;

                    if (txtFilePath != null)
                    {
                        _importPath = openFileDialog.FileName;
                        txtFilePath.Text = _importPath;
                    }

                    if (btnOK != null)
                    {
                        btnOK.Enabled = true;
                    }

                    if (lblStatus != null)
                    {
                        lblStatus.Text = "File selected. Ready to import.";
                    }

                    Logger.LogInfo("ImportSourceForm.BtnBrowse_Click", "ファイル選択", _importPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.BtnBrowse_Click", "ファイル選択エラー", ex.Message);
                MessageBox.Show($"ファイル選択に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void BtnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_importPath))
                {
                    MessageBox.Show("ファイルを選択してください。", "警告", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(_importPath))
                {
                    MessageBox.Show("選択されたファイルが存在しません。", "エラー", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // インポートオプションを取得
                var importOptions = GetImportOptions();

                // 設定をインポート
                var result = ImportSettings(_importPath, importOptions);

                if (result)
                {
                    _importSuccessful = true;
                    Logger.LogInfo("ImportSourceForm.BtnOK_Click", "インポート成功", _importPath);
                    MessageBox.Show("設定のインポートが完了しました。", "完了", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _importSuccessful = false;
                    Logger.LogError("ImportSourceForm.BtnOK_Click", "インポート失敗", _importPath);
                    MessageBox.Show("設定のインポートに失敗しました。", "エラー", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.BtnOK_Click", "インポート処理エラー", ex.Message);
                MessageBox.Show($"インポート処理に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// インポートオプションを取得します
        /// </summary>
        /// <returns>インポートオプション</returns>
        private ImportOptions GetImportOptions()
        {
            var options = new ImportOptions();

            try
            {
                var chkImportBrowsers = Controls.Find("chkImportBrowsers", true).FirstOrDefault() as CheckBox;
                var chkImportURLs = Controls.Find("chkImportURLs", true).FirstOrDefault() as CheckBox;
                var chkImportProtocols = Controls.Find("chkImportProtocols", true).FirstOrDefault() as CheckBox;
                var chkImportFileTypes = Controls.Find("chkImportFileTypes", true).FirstOrDefault() as CheckBox;
                var chkImportGeneral = Controls.Find("chkImportGeneral", true).FirstOrDefault() as CheckBox;
                var chkOverwrite = Controls.Find("chkOverwrite", true).FirstOrDefault() as CheckBox;

                options.ImportBrowsers = chkImportBrowsers?.Checked ?? true;
                options.ImportURLs = chkImportURLs?.Checked ?? true;
                options.ImportProtocols = chkImportProtocols?.Checked ?? true;
                options.ImportFileTypes = chkImportFileTypes?.Checked ?? true;
                options.ImportGeneral = chkImportGeneral?.Checked ?? true;
                options.OverwriteExisting = chkOverwrite?.Checked ?? false;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.GetImportOptions", "オプション取得エラー", ex.Message);
            }

            return options;
        }

        /// <summary>
        /// 設定をインポートします
        /// </summary>
        /// <param name="filePath">インポートファイルパス</param>
        /// <param name="options">インポートオプション</param>
        /// <returns>成功した場合はtrue</returns>
        private bool ImportSettings(string filePath, ImportOptions options)
        {
            try
            {
                Logger.LogInfo("ImportSourceForm.ImportSettings", "インポート開始", filePath);

                // ファイルの種類を判定
                if (Path.GetFileName(filePath).Equals("BrowserChooser2Config.xml", StringComparison.OrdinalIgnoreCase))
                {
                    // Browser Chooser 2の設定ファイル
                    return ImportBC2Settings(filePath, options);
                }
                else
                {
                    // Browser Chooser 3の設定ファイル
                    return ImportBC3Settings(filePath, options);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.ImportSettings", "インポートエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Browser Chooser 2の設定をインポートします
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="options">インポートオプション</param>
        /// <returns>成功した場合はtrue</returns>
        private bool ImportBC2Settings(string filePath, ImportOptions options)
        {
            try
            {
                // Importerクラスを使用してBC2設定をインポート
                var targetSettings = Settings.Current;
                var sourcePath = Path.GetDirectoryName(filePath) ?? string.Empty;

                var result = Importer.ImportLegacySettings(sourcePath, targetSettings);
                
                if (result)
                {
                                    // 設定を保存
                targetSettings.DoSave();
                    Logger.LogInfo("ImportSourceForm.ImportBC2Settings", "BC2設定インポート成功");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.ImportBC2Settings", "BC2設定インポートエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Browser Chooser 3の設定をインポートします
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="options">インポートオプション</param>
        /// <returns>成功した場合はtrue</returns>
        private bool ImportBC3Settings(string filePath, ImportOptions options)
        {
            try
            {
                // BC3設定ファイルを読み込み
                var importedSettings = Settings.Load(filePath);
                if (importedSettings == null)
                {
                    Logger.LogError("ImportSourceForm.ImportBC3Settings", "設定ファイル読み込み失敗", filePath);
                    return false;
                }

                // 現在の設定とマージ
                var currentSettings = Settings.Current;
                MergeSettings(currentSettings, importedSettings, options);

                // 設定を保存
                currentSettings.DoSave();
                
                Logger.LogInfo("ImportSourceForm.ImportBC3Settings", "BC3設定インポート成功");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.ImportBC3Settings", "BC3設定インポートエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 設定をマージします
        /// </summary>
        /// <param name="target">ターゲット設定</param>
        /// <param name="source">ソース設定</param>
        /// <param name="options">マージオプション</param>
        private void MergeSettings(Settings target, Settings source, ImportOptions options)
        {
            try
            {
                if (options.ImportBrowsers)
                {
                    if (options.OverwriteExisting)
                    {
                        target.Browsers = new List<Browser>(source.Browsers);
                    }
                    else
                    {
                        foreach (var browser in source.Browsers)
                        {
                            if (!target.Browsers.Any(b => b.Guid == browser.Guid))
                            {
                                target.Browsers.Add(browser);
                            }
                        }
                    }
                }

                if (options.ImportURLs)
                {
                    if (options.OverwriteExisting)
                    {
                        target.URLs = new List<URL>(source.URLs);
                    }
                    else
                    {
                        foreach (var url in source.URLs)
                        {
                            if (!target.URLs.Any(u => u.Guid == url.Guid))
                            {
                                target.URLs.Add(url);
                            }
                        }
                    }
                }

                if (options.ImportProtocols)
                {
                    if (options.OverwriteExisting)
                    {
                        target.Protocols = new List<Protocol>(source.Protocols);
                    }
                    else
                    {
                        foreach (var protocol in source.Protocols)
                        {
                            if (!target.Protocols.Any(p => p.Guid == protocol.Guid))
                            {
                                target.Protocols.Add(protocol);
                            }
                        }
                    }
                }

                if (options.ImportFileTypes)
                {
                    if (options.OverwriteExisting)
                    {
                        target.FileTypes = new List<FileType>(source.FileTypes);
                    }
                    else
                    {
                        foreach (var fileType in source.FileTypes)
                        {
                            if (!target.FileTypes.Any(f => f.Guid == fileType.Guid))
                            {
                                target.FileTypes.Add(fileType);
                            }
                        }
                    }
                }

                if (options.ImportGeneral)
                {
                    // 一般設定のマージ（必要に応じて実装）
                }

                Logger.LogInfo("ImportSourceForm.MergeSettings", "設定マージ完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("ImportSourceForm.MergeSettings", "設定マージエラー", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// インポートオプションクラス
        /// </summary>
        public class ImportOptions
        {
            /// <summary>ブラウザ設定のインポート</summary>
            public bool ImportBrowsers { get; set; } = true;
            /// <summary>URL設定のインポート</summary>
            public bool ImportURLs { get; set; } = true;
            /// <summary>プロトコル設定のインポート</summary>
            public bool ImportProtocols { get; set; } = true;
            /// <summary>ファイルタイプ設定のインポート</summary>
            public bool ImportFileTypes { get; set; } = true;
            /// <summary>一般設定のインポート</summary>
            public bool ImportGeneral { get; set; } = true;
            /// <summary>既存設定の上書き</summary>
            public bool OverwriteExisting { get; set; } = false;
        }
    }
}
