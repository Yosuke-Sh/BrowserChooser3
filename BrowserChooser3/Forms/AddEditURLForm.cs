using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// Auto URL追加・編集ダイアログ
    /// </summary>
    public partial class AddEditURLForm : Form
    {
        private URL _url = null!;
        private Dictionary<int, Browser> _browsers = null!;
        private bool _isEditMode;

        /// <summary>
        /// Auto URL追加・編集ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AddEditURLForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// Auto URL追加モードでダイアログを表示
        /// </summary>
        public bool AddURL(Dictionary<int, Browser> browsers)
        {
            _browsers = browsers;
            _isEditMode = false;

            _url = new URL
            {
                URLPattern = "",
                Guid = Guid.Empty,
                DelayTime = -1
            };

            LoadURLData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Auto URL編集モードでダイアログを表示
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
            // nullチェック
            if (_url == null)
            {
                _url = new URL();
            }
            if (_browsers == null)
            {
                _browsers = new Dictionary<int, Browser>();
            }

            Text = _isEditMode ? "Edit Auto URL" : "Add Auto URL";
            
            var txtURL = Controls.Find("txtURL", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var txtDelay = Controls.Find("txtDelay", true).FirstOrDefault() as TextBox;

            if (txtURL != null) txtURL.Text = _url.URLPattern ?? "";
            if (txtDelay != null) txtDelay.Text = _url.DelayTime < 0 ? "" : _url.DelayTime.ToString();

            // ブラウザコンボボックスの設定
            if (cmbBrowser != null)
            {
                cmbBrowser.Items.Clear();
                foreach (var browser in _browsers.Values)
                {
                    if (browser != null && !string.IsNullOrEmpty(browser.Name))
                    {
                        cmbBrowser.Items.Add(browser.Name);
                    }
                }

                if (_url.Guid != Guid.Empty)
                {
                    var selectedBrowser = _browsers.Values.FirstOrDefault(b => b != null && b.Guid == _url.Guid);
                    if (selectedBrowser != null)
                    {
                        cmbBrowser.SelectedItem = selectedBrowser.Name;
                    }
                }
                else if (cmbBrowser.Items.Count > 0)
                {
                    cmbBrowser.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// URLデータを取得
        /// </summary>
        public URL GetData()
        {
            var txtURL = Controls.Find("txtURL", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var txtDelay = Controls.Find("txtDelay", true).FirstOrDefault() as TextBox;

            if (txtURL != null) _url.URLPattern = txtURL.Text;
            
            if (cmbBrowser != null && cmbBrowser.SelectedItem != null)
            {
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _url.Guid = selectedBrowser.Guid;
                }
            }
            
            if (txtDelay != null && int.TryParse(txtDelay.Text, out var delay))
            {
                _url.DelayTime = delay;
            }
            else
            {
                _url.DelayTime = -1; // Default
            }

            return _url;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit Auto URL";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // Windows 11 風スタイル
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(250, 250, 250);

            // 基本設定
            var lblURL = new Label { Text = "URL:", Location = new Point(10, 20), AutoSize = true };
            var txtURL = new TextBox { Name = "txtURL", Location = new Point(120, 17), Size = new Size(320, 23) };

            var lblBrowser = new Label { Text = "Browser:", Location = new Point(10, 55), AutoSize = true };
            var cmbBrowser = new ComboBox { Name = "cmbBrowser", Location = new Point(120, 52), Size = new Size(320, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDelay = new Label { Text = "Delay (seconds):", Location = new Point(10, 90), AutoSize = true };
            var txtDelay = new TextBox { Name = "txtDelay", Location = new Point(120, 87), Size = new Size(120, 23) };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 220), Size = new Size(90, 30) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(395, 220), Size = new Size(90, 30) };
            btnOK.FlatStyle = FlatStyle.System;
            btnCancel.FlatStyle = FlatStyle.System;

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblURL, txtURL,
                lblBrowser, cmbBrowser,
                lblDelay, txtDelay,
                btnOK, btnCancel
            });

            // イベントハンドラー
            btnOK.Click += (s, e) =>
            {
                // データの検証
                if (string.IsNullOrWhiteSpace(txtURL.Text))
                {
                    MessageBox.Show("URLを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbBrowser.SelectedItem == null)
                {
                    MessageBox.Show("ブラウザを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの保存
                _url.URLPattern = txtURL.Text;
                
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _url.Guid = selectedBrowser.Guid;
                }
                
                if (int.TryParse(txtDelay.Text, out var delay))
                {
                    _url.DelayTime = delay;
                }
                else
                {
                    _url.DelayTime = -1; // Default
                }
            };
        }
    }
}
