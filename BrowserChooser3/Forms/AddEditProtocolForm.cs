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
            var txtHeader = Controls.Find("txtHeader", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) txtName.Text = _protocol.Name;
            if (txtHeader != null) txtHeader.Text = _protocol.Header;
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
            var txtHeader = Controls.Find("txtHeader", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) _protocol.Name = txtName.Text;
            if (txtHeader != null) _protocol.Header = txtHeader.Text;
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
            Size = new Size(500, 280);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // Windows 11 風スタイル
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // 基本設定
            var lblName = new Label { Text = "Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(170, 17), Size = new Size(300, 23) };

            var lblHeader = new Label { Text = "Protocol Header:", Location = new Point(10, 50), AutoSize = true };
            var txtHeader = new TextBox { Name = "txtHeader", Location = new Point(170, 47), Size = new Size(300, 23) };

            var lblBrowser = new Label { Text = "Browser:", Location = new Point(10, 80), AutoSize = true };
            var cmbBrowser = new ComboBox { Name = "cmbBrowser", Location = new Point(170, 77), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var chkActive = new CheckBox { Name = "chkActive", Text = "Active", Location = new Point(170, 115), AutoSize = true, Checked = true };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(250, 160), Size = new Size(90, 30) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(350, 160), Size = new Size(90, 30) };
            btnOK.FlatStyle = FlatStyle.System;
            btnCancel.FlatStyle = FlatStyle.System;

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblHeader, txtHeader,
                lblBrowser, cmbBrowser,
                chkActive,
                btnOK, btnCancel
            });

            // イベントハンドラー
            // フォームが閉じられる前の検証
            this.FormClosing += (s, e) =>
            {
                if (DialogResult == DialogResult.OK)
                {
                    // データの検証
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show("プロトコル名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        e.Cancel = true; // フォームを閉じない
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtHeader.Text))
                    {
                        MessageBox.Show("Protocol Headerを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtHeader.Focus();
                        e.Cancel = true; // フォームを閉じない
                        return;
                    }

                    if (cmbBrowser.SelectedItem == null)
                    {
                        MessageBox.Show("ブラウザを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbBrowser.Focus();
                        e.Cancel = true; // フォームを閉じない
                        return;
                    }

                    // データの保存
                    _protocol.Name = txtName.Text;
                    _protocol.Header = txtHeader.Text;
                    _protocol.IsActive = chkActive.Checked;
                    
                    var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                    var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                    if (selectedBrowser != null)
                    {
                        _protocol.BrowserGuid = selectedBrowser.Guid;
                    }
                }
            };
        }
    }
}
