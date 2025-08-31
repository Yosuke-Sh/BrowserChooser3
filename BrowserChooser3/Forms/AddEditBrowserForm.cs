using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// ブラウザ追加・編集ダイアログ
    /// </summary>
    public partial class AddEditBrowserForm : Form
    {
        private Browser _browser = null!;
        private Dictionary<int, Browser> _browsers = null!;
        private Dictionary<int, Protocol> _protocols = null!;

        private bool _isAdvanced;
        private Point _gridSize;
        private bool _isEditMode;

        /// <summary>
        /// ブラウザ追加・編集ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AddEditBrowserForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// ブラウザ追加モードでダイアログを表示
        /// </summary>
        public bool AddBrowser(Dictionary<int, Browser> browsers, Dictionary<int, Protocol> protocols, 
            bool isAdvanced, Point gridSize, Browser? templateBrowser = null)
        {
            _browsers = browsers;
            _protocols = protocols;
            _isAdvanced = isAdvanced;
            _gridSize = gridSize;
            _isEditMode = false;

            if (templateBrowser != null)
            {
                _browser = templateBrowser.Clone();
            }
            else
            {
                _browser = new Browser
                {
                    Guid = Guid.NewGuid(),
                    Name = "",
                    Target = "",
                    Arguments = "",
                                    X = 1,
                Y = 1,
                    Hotkey = '\0',

                };
            }

            LoadBrowserData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ブラウザ編集モードでダイアログを表示
        /// </summary>
        public bool EditBrowser(Browser browser, Dictionary<int, Browser> browsers, 
            Dictionary<int, Protocol> protocols, bool isAdvanced)
        {
            _browser = browser;
            _browsers = browsers;
            _protocols = protocols;
            _isAdvanced = isAdvanced;
            _isEditMode = true;

            LoadBrowserData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ブラウザデータを読み込み
        /// </summary>
        private void LoadBrowserData()
        {
            Text = _isEditMode ? "Edit Browser" : "Add Browser";
            
            // _browserがnullの場合は処理をスキップ
            if (_browser == null)
            {
                return;
            }
            
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtTarget = Controls.Find("txtTarget", true).FirstOrDefault() as TextBox;
            var txtArguments = Controls.Find("txtArguments", true).FirstOrDefault() as TextBox;
            var txtHotkey = Controls.Find("txtHotkey", true).FirstOrDefault() as TextBox;
            var nudRow = Controls.Find("nudRow", true).FirstOrDefault() as NumericUpDown;
            var nudCol = Controls.Find("nudCol", true).FirstOrDefault() as NumericUpDown;

            if (txtName != null) txtName.Text = _browser.Name ?? "";
            if (txtTarget != null) txtTarget.Text = _browser.Target ?? "";
            if (txtArguments != null) txtArguments.Text = _browser.Arguments ?? "";
            if (txtHotkey != null) txtHotkey.Text = _browser.Hotkey != '\0' ? _browser.Hotkey.ToString() : "";
            if (nudRow != null) nudRow.Value = _browser.Y;
            if (nudCol != null) nudCol.Value = _browser.X;

            // アイコン表示を更新
            UpdateIconDisplay();
        }

        /// <summary>
        /// アイコン表示を更新
        /// </summary>
        private void UpdateIconDisplay()
        {
            try
            {
                var picIcon = Controls.Find("picIcon", true).FirstOrDefault() as PictureBox;
                if (picIcon == null) return;

                if (!string.IsNullOrEmpty(_browser.ImagePath) && File.Exists(_browser.ImagePath))
                {
                    // 保存されたアイコンインデックスを使用してアイコンを抽出
                    var icon = ExtractIconFromFile(_browser.ImagePath, _browser.IconIndex);
                    if (icon != null)
                    {
                        picIcon.Image = icon.ToBitmap();
                        return;
                    }
                }

                // アイコンが取得できない場合はクリア
                picIcon.Image = null;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("AddEditBrowserForm.UpdateIconDisplay", "アイコン表示更新エラー", ex.Message);
                var picIcon = Controls.Find("picIcon", true).FirstOrDefault() as PictureBox;
                if (picIcon != null)
                {
                    picIcon.Image = null;
                }
            }
        }

        /// <summary>
        /// ファイルから指定されたインデックスのアイコンを抽出します
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="iconIndex">アイコンインデックス</param>
        /// <returns>抽出されたアイコン</returns>
        private Icon? ExtractIconFromFile(string filePath, int iconIndex)
        {
            try
            {
                // 指定されたインデックスのアイコンを抽出
                var largeIcon = IntPtr.Zero;
                var smallIcon = IntPtr.Zero;
                
                if (ExtractIconEx(filePath, iconIndex, out largeIcon, out smallIcon, 1) > 0)
                {
                    if (largeIcon != IntPtr.Zero)
                    {
                        return Icon.FromHandle(largeIcon);
                    }
                }
                
                // フォールバック: 関連付けられたアイコンを取得
                return Icon.ExtractAssociatedIcon(filePath);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("AddEditBrowserForm.ExtractIconFromFile", "アイコン抽出エラー", ex.Message);
                
                // フォールバック: 関連付けられたアイコンを取得
                try
                {
                    return Icon.ExtractAssociatedIcon(filePath);
                }
                catch (Exception fallbackEx)
                {
                    Logger.LogError("AddEditBrowserForm.ExtractIconFromFile", "フォールバックアイコン抽出エラー", fallbackEx.Message);
                    return null;
                }
            }
        }

        #region Win32 API
        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int ExtractIconEx(string szFileName, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
        #endregion

        /// <summary>
        /// ブラウザデータを取得
        /// </summary>
        public Browser GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtTarget = Controls.Find("txtTarget", true).FirstOrDefault() as TextBox;
            var txtArguments = Controls.Find("txtArguments", true).FirstOrDefault() as TextBox;
            var txtHotkey = Controls.Find("txtHotkey", true).FirstOrDefault() as TextBox;
            var nudRow = Controls.Find("nudRow", true).FirstOrDefault() as NumericUpDown;
            var nudCol = Controls.Find("nudCol", true).FirstOrDefault() as NumericUpDown;

            if (txtName != null) _browser.Name = txtName.Text;
            if (txtTarget != null) _browser.Target = txtTarget.Text;
            if (txtArguments != null) _browser.Arguments = txtArguments.Text;
            if (nudRow != null) _browser.Y = (int)nudRow.Value;
            if (nudCol != null) _browser.X = (int)nudCol.Value;
            
            if (txtHotkey != null && txtHotkey.Text.Length > 0)
            {
                _browser.Hotkey = txtHotkey.Text[0];
            }
            else
            {
                _browser.Hotkey = '\0';
            }

            return _browser;
        }

        /// <summary>
        /// プロトコルデータを取得
        /// </summary>
        public Dictionary<int, Protocol> GetProtocols()
        {
            return _protocols;
        }



        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit Browser";
            Size = new Size(600, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // Windows 11 風スタイル
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // 基本設定
            var lblName = new Label { Text = "Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(300, 23) };

            // 行間 +10px（2行目以降）
            var lblTarget = new Label { Text = "Target:", Location = new Point(10, 60), AutoSize = true };
            var txtTarget = new TextBox { Name = "txtTarget", Location = new Point(120, 57), Size = new Size(250, 23) };
            var btnBrowse = new Button { Text = "Browse", Location = new Point(380, 56), Size = new Size(85, 32) };

            var lblArguments = new Label { Text = "Arguments:", Location = new Point(10, 100), AutoSize = true };
            var txtArguments = new TextBox { Name = "txtArguments", Location = new Point(120, 97), Size = new Size(300, 23) };

            var lblHotkey = new Label { Text = "Hotkey:", Location = new Point(10, 140), AutoSize = true };
            var txtHotkey = new TextBox { Name = "txtHotkey", Location = new Point(120, 137), Size = new Size(50, 23), MaxLength = 1 };

            // アイコン表示用PictureBox
            var lblIcon = new Label { Text = "Icon:", Location = new Point(10, 180), AutoSize = true };
            var picIcon = new PictureBox { Name = "picIcon", Location = new Point(120, 177), Size = new Size(64, 64), SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
            var btnEditIcon = new Button { Text = "Edit Icon", Location = new Point(200, 177), Size = new Size(85, 35) };

            var lblRow = new Label { Text = "Row:", Location = new Point(10, 250), AutoSize = true };
            var nudRow = new NumericUpDown { Name = "nudRow", Location = new Point(120, 247), Size = new Size(80, 23), Minimum = 0, Maximum = 100 };

            var lblCol = new Label { Text = "Column:", Location = new Point(220, 250), AutoSize = true };
            var nudCol = new NumericUpDown { Name = "nudCol", Location = new Point(320, 247), Size = new Size(80, 23), Minimum = 0, Maximum = 100 };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 350), Size = new Size(90, 30) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(400, 350), Size = new Size(90, 30) };
            btnOK.FlatStyle = FlatStyle.System;
            btnCancel.FlatStyle = FlatStyle.System;

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblTarget, txtTarget, btnBrowse,
                lblArguments, txtArguments,
                lblHotkey, txtHotkey,
                lblIcon, picIcon, btnEditIcon,
                lblRow, nudRow,
                lblCol, nudCol,
                btnOK, btnCancel
            });

            // イベントハンドラー
            btnBrowse.Click += (s, e) =>
            {
                using var openFileDialog = new OpenFileDialog
                {
                    Filter = "実行ファイル (*.exe)|*.exe|すべてのファイル (*.*)|*.*",
                    Title = "ブラウザ実行ファイルを選択"
                };
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtTarget.Text = openFileDialog.FileName;
                    
                    // ブラウザ名を自動設定
                    try
                    {
                        var fileInfo = new FileInfo(openFileDialog.FileName);
                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                        
                        if (!string.IsNullOrEmpty(versionInfo.ProductName))
                        {
                            txtName.Text = versionInfo.ProductName;
                        }
                        else
                        {
                            txtName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        }
                    }
                    catch
                    {
                        // バージョン情報が取得できない場合はファイル名を使用
                        txtName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    }
                    
                    // アイコン選択ダイアログを表示
                    try
                    {
                        using var iconSelectionForm = new IconSelectionForm(openFileDialog.FileName);
                        if (iconSelectionForm.ShowDialog() == DialogResult.OK && iconSelectionForm.SelectedIcon != null)
                        {
                            // 選択されたアイコンをブラウザに設定
                            _browser.ImagePath = openFileDialog.FileName;
                            _browser.IconIndex = iconSelectionForm.SelectedIconIndex; // 選択されたアイコンのインデックスを保存
                            UpdateIconDisplay();
                            
                            Logger.LogInfo("AddEditBrowserForm.btnBrowse_Click", "アイコン選択完了", 
                                $"IconIndex: {_browser.IconIndex}, ImagePath: {_browser.ImagePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // アイコン選択に失敗した場合は無視（デフォルトアイコンを使用）
                        Logger.LogWarning("AddEditBrowserForm.btnBrowse_Click", "アイコン選択に失敗しました", ex.Message);
                    }
                }
            };

            // アイコン編集ボタンのクリックイベント
            btnEditIcon.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(_browser.ImagePath) || !File.Exists(_browser.ImagePath))
                {
                    MessageBox.Show("先に実行ファイルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using var iconSelectionForm = new IconSelectionForm(_browser.ImagePath);
                    if (iconSelectionForm.ShowDialog() == DialogResult.OK && iconSelectionForm.SelectedIcon != null)
                    {
                        // 選択されたアイコンのインデックスを保存
                        _browser.IconIndex = iconSelectionForm.SelectedIconIndex;
                        
                        // アイコン表示を更新
                        UpdateIconDisplay();
                        
                        Logger.LogInfo("AddEditBrowserForm.btnEditIcon_Click", "アイコン編集完了", 
                            $"IconIndex: {_browser.IconIndex}, ImagePath: {_browser.ImagePath}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("AddEditBrowserForm.btnEditIcon_Click", "アイコン編集エラー", ex.Message);
                    MessageBox.Show($"アイコンの編集に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            
            // フォームが閉じられる前の検証
            this.FormClosing += (s, e) =>
            {
                if (DialogResult == DialogResult.OK)
                {
                    // データの検証
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show("ブラウザ名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        e.Cancel = true; // フォームを閉じない
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtTarget.Text))
                    {
                        MessageBox.Show("実行ファイルパスを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTarget.Focus();
                        e.Cancel = true; // フォームを閉じない
                        return;
                    }

                    // データの保存
                    _browser.Name = txtName.Text;
                    _browser.Target = txtTarget.Text;
                    _browser.Arguments = txtArguments.Text;

                    
                    if (txtHotkey != null && txtHotkey.Text.Length > 0)
                    {
                        _browser.Hotkey = txtHotkey.Text[0];
                    }
                    else
                    {
                        _browser.Hotkey = '\0';
                    }
                }
            };
        }
    }
}
