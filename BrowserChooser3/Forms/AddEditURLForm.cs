using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// URL追加・編集ダイアログ
    /// </summary>
    public partial class AddEditURLForm : Form
    {
        private URL _url = null!;
        private Dictionary<int, Browser> _browsers = null!;
        private bool _isEditMode;

        /// <summary>
        /// URL追加・編集ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AddEditURLForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// URL追加モードでダイアログを表示
        /// </summary>
        public bool AddURL(Dictionary<int, Browser> browsers)
        {
            _browsers = browsers;
            _isEditMode = false;

            _url = new URL
            {
                Guid = Guid.NewGuid(),
                Name = "",
                URLValue = "",
                BrowserGuid = Guid.Empty,
                DelayTime = -1,
                IsActive = true
            };

            LoadURLData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// URL編集モードでダイアログを表示
        /// </summary>
        public bool EditURL(URL url, Dictionary<int, Browser> browsers)
        {
            _url = url;
            _browsers = browsers;
            _isEditMode = true;

            LoadURLData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// URLデータを読み込み
        /// </summary>
        private void LoadURLData()
        {
            // TODO: コントロールにURLデータを設定
            Text = _isEditMode ? "Edit URL" : "Add URL";
        }

        /// <summary>
        /// URLデータを取得
        /// </summary>
        public URL GetData()
        {
            // TODO: コントロールからURLデータを取得
            return _url;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit URL";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 基本設定
            var lblURL = new Label { Text = "URL:", Location = new Point(10, 20), AutoSize = true };
            var txtURL = new TextBox { Name = "txtURL", Location = new Point(120, 17), Size = new Size(350, 23) };

            var lblBrowser = new Label { Text = "Browser:", Location = new Point(10, 50), AutoSize = true };
            var cmbBrowser = new ComboBox { Name = "cmbBrowser", Location = new Point(120, 47), Size = new Size(350, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDelay = new Label { Text = "Delay (seconds):", Location = new Point(10, 80), AutoSize = true };
            var nudDelay = new NumericUpDown { Name = "nudDelay", Location = new Point(120, 77), Size = new Size(100, 23), Minimum = -1, Maximum = 3600, Value = -1 };

            var chkActive = new CheckBox { Name = "chkActive", Text = "Active", Location = new Point(120, 110), AutoSize = true, Checked = true };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 220), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(385, 220), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblURL, txtURL,
                lblBrowser, cmbBrowser,
                lblDelay, nudDelay,
                chkActive,
                btnOK, btnCancel
            });

            // ブラウザリストの設定
            cmbBrowser.Items.Add("Default");
            foreach (var browser in _browsers.Values)
            {
                cmbBrowser.Items.Add(browser.Name);
            }

            // イベントハンドラー
            btnOK.Click += (s, e) =>
            {
                // TODO: データの検証と保存
                _url.URLValue = txtURL.Text;
                _url.IsActive = chkActive.Checked;
                _url.DelayTime = (int)nudDelay.Value;

                if (cmbBrowser.SelectedIndex > 0)
                {
                    var selectedBrowser = _browsers.Values.ElementAt(cmbBrowser.SelectedIndex - 1);
                    _url.BrowserGuid = selectedBrowser.Guid;
                }
            };
        }
    }
}
