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
        private Dictionary<int, FileType> _fileTypes = null!;
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
            Dictionary<int, FileType> fileTypes, bool isAdvanced, Point gridSize, Browser? templateBrowser = null)
        {
            _browsers = browsers;
            _protocols = protocols;
            _fileTypes = fileTypes;
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
                    PosX = 1,
                    PosY = 1,
                    Hotkey = '\0',
                    Category = "Default"
                };
            }

            LoadBrowserData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ブラウザ編集モードでダイアログを表示
        /// </summary>
        public bool EditBrowser(Browser browser, Dictionary<int, Browser> browsers, 
            Dictionary<int, Protocol> protocols, Dictionary<int, FileType> fileTypes, bool isAdvanced)
        {
            _browser = browser;
            _browsers = browsers;
            _protocols = protocols;
            _fileTypes = fileTypes;
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
            var txtCategory = Controls.Find("txtCategory", true).FirstOrDefault() as TextBox;
            var nudRow = Controls.Find("nudRow", true).FirstOrDefault() as NumericUpDown;
            var nudCol = Controls.Find("nudCol", true).FirstOrDefault() as NumericUpDown;

            if (txtName != null) txtName.Text = _browser.Name ?? "";
            if (txtTarget != null) txtTarget.Text = _browser.Target ?? "";
            if (txtArguments != null) txtArguments.Text = _browser.Arguments ?? "";
            if (txtHotkey != null) txtHotkey.Text = _browser.Hotkey != '\0' ? _browser.Hotkey.ToString() : "";
            if (txtCategory != null) txtCategory.Text = _browser.Category ?? "";
            if (nudRow != null) nudRow.Value = _browser.PosY;
            if (nudCol != null) nudCol.Value = _browser.PosX;
        }

        /// <summary>
        /// ブラウザデータを取得
        /// </summary>
        public Browser GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtTarget = Controls.Find("txtTarget", true).FirstOrDefault() as TextBox;
            var txtArguments = Controls.Find("txtArguments", true).FirstOrDefault() as TextBox;
            var txtHotkey = Controls.Find("txtHotkey", true).FirstOrDefault() as TextBox;
            var txtCategory = Controls.Find("txtCategory", true).FirstOrDefault() as TextBox;
            var nudRow = Controls.Find("nudRow", true).FirstOrDefault() as NumericUpDown;
            var nudCol = Controls.Find("nudCol", true).FirstOrDefault() as NumericUpDown;

            if (txtName != null) _browser.Name = txtName.Text;
            if (txtTarget != null) _browser.Target = txtTarget.Text;
            if (txtArguments != null) _browser.Arguments = txtArguments.Text;
            if (txtCategory != null) _browser.Category = txtCategory.Text;
            if (nudRow != null) _browser.PosY = (int)nudRow.Value;
            if (nudCol != null) _browser.PosX = (int)nudCol.Value;
            
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
        /// ファイルタイプデータを取得
        /// </summary>
        public Dictionary<int, FileType> GetFileTypes()
        {
            return _fileTypes;
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
            BackColor = Color.FromArgb(250, 250, 250);

            // 基本設定
            var lblName = new Label { Text = "Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(300, 23) };

            // 行間 +5px（2行目以降）
            var lblTarget = new Label { Text = "Target:", Location = new Point(10, 55), AutoSize = true };
            var txtTarget = new TextBox { Name = "txtTarget", Location = new Point(120, 52), Size = new Size(250, 23) };
            var btnBrowse = new Button { Text = "Browse", Location = new Point(380, 51), Size = new Size(85, 28) };

            var lblArguments = new Label { Text = "Arguments:", Location = new Point(10, 90), AutoSize = true };
            var txtArguments = new TextBox { Name = "txtArguments", Location = new Point(120, 87), Size = new Size(300, 23) };

            var lblHotkey = new Label { Text = "Hotkey:", Location = new Point(10, 120), AutoSize = true };
            var txtHotkey = new TextBox { Name = "txtHotkey", Location = new Point(120, 117), Size = new Size(50, 23), MaxLength = 1 };

            var lblCategory = new Label { Text = "Category:", Location = new Point(10, 150), AutoSize = true };
            var txtCategory = new TextBox { Name = "txtCategory", Location = new Point(120, 147), Size = new Size(300, 23) };

            var lblRow = new Label { Text = "Row:", Location = new Point(10, 180), AutoSize = true };
            var nudRow = new NumericUpDown { Name = "nudRow", Location = new Point(120, 177), Size = new Size(80, 23), Minimum = 0, Maximum = 100 };

            var lblCol = new Label { Text = "Column:", Location = new Point(220, 180), AutoSize = true };
            var nudCol = new NumericUpDown { Name = "nudCol", Location = new Point(300, 177), Size = new Size(80, 23), Minimum = 0, Maximum = 100 };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 400), Size = new Size(90, 30) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(400, 400), Size = new Size(90, 30) };
            btnOK.FlatStyle = FlatStyle.System;
            btnCancel.FlatStyle = FlatStyle.System;

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblTarget, txtTarget, btnBrowse,
                lblArguments, txtArguments,
                lblHotkey, txtHotkey,
                lblCategory, txtCategory,
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
                }
            };
            
            btnOK.Click += (s, e) =>
            {
                // データの検証
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("ブラウザ名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTarget.Text))
                {
                    MessageBox.Show("実行ファイルパスを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの保存
                _browser.Name = txtName.Text;
                _browser.Target = txtTarget.Text;
                _browser.Arguments = txtArguments.Text;
                _browser.Category = txtCategory.Text;
                
                if (txtHotkey.Text.Length > 0)
                {
                    _browser.Hotkey = txtHotkey.Text[0];
                }
                else
                {
                    _browser.Hotkey = '\0';
                }
            };
        }
    }
}
