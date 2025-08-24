using BrowserChooser3.Classes;

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
            // TODO: コントロールにブラウザデータを設定
            Text = _isEditMode ? "Edit Browser" : "Add Browser";
        }

        /// <summary>
        /// ブラウザデータを取得
        /// </summary>
        public Browser GetData()
        {
            // TODO: コントロールからブラウザデータを取得
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
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 基本設定
            var lblName = new Label { Text = "Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(300, 23) };

            var lblTarget = new Label { Text = "Target:", Location = new Point(10, 50), AutoSize = true };
            var txtTarget = new TextBox { Name = "txtTarget", Location = new Point(120, 47), Size = new Size(300, 23) };

            var lblArguments = new Label { Text = "Arguments:", Location = new Point(10, 80), AutoSize = true };
            var txtArguments = new TextBox { Name = "txtArguments", Location = new Point(120, 77), Size = new Size(300, 23) };

            var lblHotkey = new Label { Text = "Hotkey:", Location = new Point(10, 110), AutoSize = true };
            var txtHotkey = new TextBox { Name = "txtHotkey", Location = new Point(120, 107), Size = new Size(50, 23), MaxLength = 1 };

            var lblCategory = new Label { Text = "Category:", Location = new Point(10, 140), AutoSize = true };
            var txtCategory = new TextBox { Name = "txtCategory", Location = new Point(120, 137), Size = new Size(300, 23) };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 320), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(385, 320), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblTarget, txtTarget,
                lblArguments, txtArguments,
                lblHotkey, txtHotkey,
                lblCategory, txtCategory,
                btnOK, btnCancel
            });

            // イベントハンドラー
            btnOK.Click += (s, e) =>
            {
                // TODO: データの検証と保存
                _browser.Name = txtName.Text;
                _browser.Target = txtTarget.Text;
                _browser.Arguments = txtArguments.Text;
                _browser.Category = txtCategory.Text;
                
                if (txtHotkey.Text.Length > 0)
                {
                    _browser.Hotkey = txtHotkey.Text[0];
                }
            };
        }
    }
}
