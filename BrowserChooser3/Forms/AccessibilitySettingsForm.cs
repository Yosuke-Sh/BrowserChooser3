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
        public bool ShowFocus 
        { 
            get => _showFocus;
            set => _showFocus = value;
        }

        /// <summary>
        /// フォーカスボックスの色
        /// </summary>
        public Color FocusBoxColor 
        { 
            get => _focusBoxColor;
            set => _focusBoxColor = value;
        }

        /// <summary>
        /// フォーカスボックスの線幅
        /// </summary>
        public int FocusBoxWidth 
        { 
            get => _focusBoxWidth;
            set => _focusBoxWidth = value;
        }

        /// <summary>
        /// アクセシビリティ設定ダイアログの新しいインスタンスを初期化します
        /// </summary>
        public AccessibilitySettingsForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
            
            // 初期値を設定
            _showFocus = true;
            _focusBoxColor = Color.Black;
            _focusBoxWidth = 2;
        }

        /// <summary>
        /// フォーカスボックス色選択イベント
        /// </summary>
        private void pbFocusColor_Click(object sender, EventArgs e)
        {
            // テスト環境ではダイアログを表示しない
            if (IsTestEnvironment())
            {
                return;
            }

            using var colorDialog = new ColorDialog
            {
                Color = pbFocusColor.BackColor
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pbFocusColor.BackColor = colorDialog.Color;
            }
        }

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            try
            {
                // 環境変数でダイアログ無効化が設定されている場合
                var disableDialogs = Environment.GetEnvironmentVariable("DISABLE_DIALOGS");
                if (!string.IsNullOrEmpty(disableDialogs) && disableDialogs.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // デバッガーがアタッチされている場合
                if (System.Diagnostics.Debugger.IsAttached)
                    return true;

                // アセンブリ名に"Test"が含まれている場合
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                if (assemblyName?.Contains("Test", StringComparison.OrdinalIgnoreCase) == true)
                    return true;

                // 環境変数でテスト環境を判定
                var testEnv = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnv) && testEnv.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // プロセス名に"test"が含まれている場合
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (processName.Contains("test", StringComparison.OrdinalIgnoreCase))
                    return true;

                // スタックトレースにテスト関連のメソッドが含まれている場合
                var stackTrace = Environment.StackTrace;
                if (stackTrace.Contains("xunit", StringComparison.OrdinalIgnoreCase) ||
                    stackTrace.Contains("test", StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch
            {
                // エラーが発生した場合は安全のためテスト環境とみなす
                return true;
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

