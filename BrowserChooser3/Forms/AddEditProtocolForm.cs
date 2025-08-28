using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// プロトコル追加・編集ダイアログ
    /// </summary>
    public partial class AddEditProtocolForm : Form
    {
        private Protocol _protocol = null!;
        private Dictionary<int, Browser> _browsers = null!;
        private bool _isEditMode;

        /// <summary>
        /// プロトコル追加・編集ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AddEditProtocolForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// プロトコル追加モードでダイアログを表示
        /// </summary>
        public bool AddProtocol(Dictionary<int, Browser> browsers)
        {
            _browsers = browsers;
            _isEditMode = false;

            _protocol = new Protocol
            {
                Name = "",
                BrowserGuid = Guid.Empty,
                IsActive = true
            };

            LoadProtocolData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// プロトコル編集モードでダイアログを表示
        /// </summary>
        public bool EditProtocol(Protocol protocol, Dictionary<int, Browser> browsers)
        {
            _protocol = protocol;
            _browsers = browsers;
            _isEditMode = true;

            LoadProtocolData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// プロトコルデータを読み込み
        /// </summary>
        private void LoadProtocolData()
        {
            Text = _isEditMode ? "Edit Protocol" : "Add Protocol";
            
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) txtName.Text = _protocol.Name;
            if (chkActive != null) chkActive.Checked = _protocol.IsActive;

            // ブラウザコンボボックスの設定
            if (cmbBrowser != null)
            {
                cmbBrowser.Items.Clear();
                foreach (var browser in _browsers.Values)
                {
                    cmbBrowser.Items.Add(browser.Name);
                }

                if (_protocol.BrowserGuid != Guid.Empty)
                {
                    var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Guid == _protocol.BrowserGuid);
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
        /// プロトコルデータを取得
        /// </summary>
        public Protocol GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) _protocol.Name = txtName.Text;
            if (chkActive != null) _protocol.IsActive = chkActive.Checked;
            
            if (cmbBrowser != null && cmbBrowser.SelectedItem != null)
            {
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _protocol.BrowserGuid = selectedBrowser.Guid;
                }
            }

            return _protocol;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit Protocol";
            Size = new Size(500, 250);
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

            var lblBrowser = new Label { Text = "Browser:", Location = new Point(10, 55), AutoSize = true };
            var cmbBrowser = new ComboBox { Name = "cmbBrowser", Location = new Point(120, 52), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var chkActive = new CheckBox { Name = "chkActive", Text = "Active", Location = new Point(120, 85), AutoSize = true, Checked = true };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(250, 170), Size = new Size(90, 30) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(350, 170), Size = new Size(90, 30) };
            btnOK.FlatStyle = FlatStyle.System;
            btnCancel.FlatStyle = FlatStyle.System;

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblBrowser, cmbBrowser,
                chkActive,
                btnOK, btnCancel
            });

            // イベントハンドラー
            btnOK.Click += (s, e) =>
            {
                // データの検証
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("プロトコル名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbBrowser.SelectedItem == null)
                {
                    MessageBox.Show("ブラウザを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの保存
                _protocol.Name = txtName.Text;
                _protocol.IsActive = chkActive.Checked;
                
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _protocol.BrowserGuid = selectedBrowser.Guid;
                }
            };
        }
    }
}
