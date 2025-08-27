using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのアクセシビリティ設定関連イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormAccessibilityHandlers
    {
        private readonly OptionsForm _form;
        private readonly Settings _settings;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormAccessibilityHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        public OptionsFormAccessibilityHandlers(OptionsForm form, Settings settings, Action<bool> setModified)
        {
            _form = form;
            _settings = settings;
            _setModified = setModified;
        }

        /// <summary>
        /// アクセシビリティ設定を開く
        /// </summary>
        public void OpenAccessibilitySettings()
        {
            try
            {
                // テスト環境ではダイアログを表示しない
                if (IsTestEnvironment())
                {
                    Logger.LogInfo("OptionsFormAccessibilityHandlers.OpenAccessibilitySettings", "テスト環境のため、アクセシビリティ設定ダイアログをスキップしました");
                    return;
                }

                // 現在の設定をフォームに反映
                var accessibilityForm = new AccessibilitySettingsForm();
                accessibilityForm.ShowFocus = _settings.ShowFocus;
                accessibilityForm.FocusBoxColor = Color.FromArgb(_settings.FocusBoxColor);
                accessibilityForm.FocusBoxWidth = _settings.FocusBoxWidth;

                if (accessibilityForm.ShowDialog() == DialogResult.OK)
                {
                    _settings.ShowFocus = accessibilityForm.ShowFocus;
                    _settings.FocusBoxColor = accessibilityForm.FocusBoxColor.ToArgb();
                    _settings.FocusBoxWidth = accessibilityForm.FocusBoxWidth;
                    _setModified(true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormAccessibilityHandlers.OpenAccessibilitySettings", "アクセシビリティ設定エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"アクセシビリティ設定の表示に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// アクセシビリティボタンのクリックイベント
        /// </summary>
        public void AccessibilityButton_Click(object? sender, EventArgs e)
        {
            OpenAccessibilitySettings();
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
    }
}
