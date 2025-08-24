using BrowserChooser3.Classes;

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
                Guid = Guid.NewGuid(),
                Name = "",
                ProtocolName = "",
                Header = "",
                SupportingBrowsers = new List<Guid>(),
                DefaultCategories = new List<string>()
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
            
            if (txtName != null) txtName.Text = _protocol.ProtocolName;
            if (txtHeader != null) txtHeader.Text = _protocol.Header;
        }

        /// <summary>
        /// プロトコルデータを取得
        /// </summary>
        public Protocol GetData()
        {
            var txtName = Controls.Find("txtName", true).FirstOrDefault() as TextBox;
            var txtHeader = Controls.Find("txtHeader", true).FirstOrDefault() as TextBox;
            
            if (txtName != null) _protocol.ProtocolName = txtName.Text;
            if (txtHeader != null) _protocol.Header = txtHeader.Text;
            
            return _protocol;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Add/Edit Protocol";
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 基本設定
            var lblName = new Label { Text = "Protocol Name:", Location = new Point(10, 20), AutoSize = true };
            var txtName = new TextBox { Name = "txtName", Location = new Point(120, 17), Size = new Size(300, 23) };

            var lblHeader = new Label { Text = "Header:", Location = new Point(10, 50), AutoSize = true };
            var txtHeader = new TextBox { Name = "txtHeader", Location = new Point(120, 47), Size = new Size(300, 23) };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(300, 220), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(385, 220), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                lblName, txtName,
                lblHeader, txtHeader,
                btnOK, btnCancel
            });
        }
    }
}
