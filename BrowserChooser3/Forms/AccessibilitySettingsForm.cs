using System.Drawing;
using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// アクセシビリティ設定ダイアログ
    /// </summary>
    public partial class AccessibilitySettingsForm : Form
    {
        private bool _showFocus;
        private Color _focusBoxColor;
        private int _focusBoxWidth;

        /// <summary>
        /// フォーカス表示フラグ
        /// </summary>
        public bool ShowFocus => _showFocus;

        /// <summary>
        /// フォーカスボックスの色
        /// </summary>
        public Color FocusBoxColor => _focusBoxColor;

        /// <summary>
        /// フォーカスボックスの線幅
        /// </summary>
        public int FocusBoxWidth => _focusBoxWidth;

        /// <summary>
        /// アクセシビリティ設定ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AccessibilitySettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Accessibility Settings";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // フォーカス表示設定
            var chkShowFocus = new CheckBox 
            { 
                Name = "chkShowFocus", 
                Text = "Show Focus Box", 
                Location = new Point(20, 20), 
                AutoSize = true 
            };

            // フォーカスボックス色設定
            var lblFocusColor = new Label { Text = "Focus Box Color:", Location = new Point(20, 60), AutoSize = true };
            var pbFocusColor = new Panel 
            { 
                Name = "pbFocusColor", 
                Location = new Point(120, 57), 
                Size = new Size(50, 23), 
                BackColor = Color.Red,
                BorderStyle = BorderStyle.FixedSingle
            };

            // フォーカスボックス線幅設定
            var lblFocusWidth = new Label { Text = "Focus Box Width:", Location = new Point(20, 100), AutoSize = true };
            var nudFocusWidth = new NumericUpDown 
            { 
                Name = "nudFocusWidth", 
                Location = new Point(120, 97), 
                Size = new Size(80, 23), 
                Minimum = 1, 
                Maximum = 10, 
                Value = 2 
            };

            // ボタン
            var btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(200, 220), Size = new Size(75, 23) };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(285, 220), Size = new Size(75, 23) };

            // コントロールの追加
            Controls.AddRange(new Control[] 
            {
                chkShowFocus,
                lblFocusColor, pbFocusColor,
                lblFocusWidth, nudFocusWidth,
                btnOK, btnCancel
            });

            // イベントハンドラー
            pbFocusColor.Click += (s, e) =>
            {
                var colorDialog = new ColorDialog
                {
                    Color = pbFocusColor.BackColor
                };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    pbFocusColor.BackColor = colorDialog.Color;
                }
            };

            btnOK.Click += (s, e) =>
            {
                _showFocus = chkShowFocus.Checked;
                _focusBoxColor = pbFocusColor.BackColor;
                _focusBoxWidth = (int)nudFocusWidth.Value;
            };
        }
    }
}

