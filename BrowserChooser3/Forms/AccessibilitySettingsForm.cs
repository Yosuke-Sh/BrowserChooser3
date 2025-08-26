using System.Drawing;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;

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
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// フォーカスボックス色選択イベント
        /// </summary>
        private void pbFocusColor_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog
            {
                Color = pbFocusColor.BackColor
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pbFocusColor.BackColor = colorDialog.Color;
            }
        }

        /// <summary>
        /// OKボタンクリックイベント
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            _showFocus = chkShowFocus.Checked;
            _focusBoxColor = pbFocusColor.BackColor;
            _focusBoxWidth = (int)nudFocusWidth.Value;
        }
    }
}

