using BrowserChooser3.Classes;

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
                Guid = Guid.NewGuid(),
                Name = "",
                FiletypeName = "",
                Extention = "",
                SupportingBrowsers = new List<Guid>(),
                DefaultCategories = new List<string>()
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
            
            if (txtName != null) txtName.Text = _fileType.FiletypeName;
            if (txtExtension != null) txtExtension.Text = _fileType.Extention;
        }

        /// <summary>
        /// ファイルタイプデータを取得
        /// </summary>
        public FileType GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtExtension = Controls.Find("txtExtension", true).FirstOrDefault() as TextBox;
            
            if (txtName != null) _fileType.FiletypeName = txtName.Text;
            if (txtExtension != null) _fileType.Extention = txtExtension.Text;
            
            return _fileType;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit File Type";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 基本設定
            var lblName = new Label { Text = "File Type Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(300, 23) };

            var lblExtension = new Label { Text = "Extension:", Location = new Point(10, 50), AutoSize = true };
            var txtExtension = new TextBox { Name = "txtExtension", Location = new Point(120, 47), Size = new Size(300, 23) };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 220), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(385, 220), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblExtension, txtExtension,
                btnOK, btnCancel
            });
        }
    }
}
