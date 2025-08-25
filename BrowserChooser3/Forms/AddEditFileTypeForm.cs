using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// ファイルタイプ追加・編集ダイアログ
    /// </summary>
    public partial class AddEditFileTypeForm : Form
    {
        private FileType _fileType = null!;
        private Dictionary<int, Browser> _browsers = null!;
        private bool _isEditMode;

        /// <summary>
        /// ファイルタイプ追加・編集ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AddEditFileTypeForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// ファイルタイプ追加モードでダイアログを表示
        /// </summary>
        public bool AddFileType(Dictionary<int, Browser> browsers)
        {
            _browsers = browsers;
            _isEditMode = false;

            _fileType = new FileType
            {
                Name = "",
                Extension = "",
                BrowserGuid = Guid.Empty,
                IsActive = true
            };

            LoadFileTypeData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ファイルタイプ編集モードでダイアログを表示
        /// </summary>
        public bool EditFileType(FileType fileType, Dictionary<int, Browser> browsers)
        {
            _fileType = fileType;
            _browsers = browsers;
            _isEditMode = true;

            LoadFileTypeData();
            return ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// ファイルタイプデータを読み込み
        /// </summary>
        private void LoadFileTypeData()
        {
            Text = _isEditMode ? "Edit File Type" : "Add File Type";
            
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtExtension = Controls.Find("txtExtension", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) txtName.Text = _fileType.Name;
            if (txtExtension != null) txtExtension.Text = _fileType.Extension;
            if (chkActive != null) chkActive.Checked = _fileType.IsActive;

            // ブラウザコンボボックスの設定
            if (cmbBrowser != null)
            {
                cmbBrowser.Items.Clear();
                foreach (var browser in _browsers.Values)
                {
                    cmbBrowser.Items.Add(browser.Name);
                }

                if (_fileType.BrowserGuid != Guid.Empty)
                {
                    var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Guid == _fileType.BrowserGuid);
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
        /// ファイルタイプデータを取得
        /// </summary>
        public FileType GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtExtension = Controls.Find("txtExtension", true).FirstOrDefault() as TextBox;
            var cmbBrowser = Controls.Find("cmbBrowser", true).FirstOrDefault() as ComboBox;
            var chkActive = Controls.Find("chkActive", true).FirstOrDefault() as CheckBox;

            if (txtName != null) _fileType.Name = txtName.Text;
            if (txtExtension != null) _fileType.Extension = txtExtension.Text;
            if (chkActive != null) _fileType.IsActive = chkActive.Checked;
            
            if (cmbBrowser != null && cmbBrowser.SelectedItem != null)
            {
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _fileType.BrowserGuid = selectedBrowser.Guid;
                }
            }

            return _fileType;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit File Type";
            Size = new Size(400, 230);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 基本設定
            var lblName = new Label { Text = "Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(250, 23) };

            var lblExtension = new Label { Text = "Extension:", Location = new Point(10, 50), AutoSize = true };
            var txtExtension = new TextBox { Name = "txtExtension", Location = new Point(120, 47), Size = new Size(250, 23) };

            var lblBrowser = new Label { Text = "Browser:", Location = new Point(10, 80), AutoSize = true };
            var cmbBrowser = new ComboBox { Name = "cmbBrowser", Location = new Point(120, 77), Size = new Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var chkActive = new CheckBox { Name = "chkActive", Text = "Active", Location = new Point(120, 110), AutoSize = true, Checked = true };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(200, 150), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(285, 150), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblExtension, txtExtension,
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
                    MessageBox.Show("ファイルタイプ名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtExtension.Text))
                {
                    MessageBox.Show("拡張子を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbBrowser.SelectedItem == null)
                {
                    MessageBox.Show("ブラウザを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データの保存
                _fileType.Name = txtName.Text;
                _fileType.Extension = txtExtension.Text;
                _fileType.IsActive = chkActive.Checked;
                
                var selectedBrowserName = cmbBrowser.SelectedItem.ToString();
                var selectedBrowser = _browsers.Values.FirstOrDefault(b => b.Name == selectedBrowserName);
                if (selectedBrowser != null)
                {
                    _fileType.BrowserGuid = selectedBrowser.Guid;
                }
            };
        }
    }
}
