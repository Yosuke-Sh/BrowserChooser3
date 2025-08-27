using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormの背景色関連イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormBackgroundHandlers
    {
        private readonly OptionsForm _form;
        private readonly Settings _settings;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormBackgroundHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        public OptionsFormBackgroundHandlers(OptionsForm form, Settings settings, Action<bool> setModified)
        {
            _form = form;
            _settings = settings;
            _setModified = setModified;
        }

        /// <summary>
        /// 透明化設定を適用
        /// </summary>
        public void ApplyTransparencySettings()
        {
            try
            {
                if (_settings.EnableTransparency)
                {
                    // 透明化が有効な場合の設定
                    _settings.BackgroundColorValue = Color.FromArgb(128, 255, 255, 255);
                    
                    var pbBackgroundColor = _form.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                    if (pbBackgroundColor != null)
                    {
                        pbBackgroundColor.BackColor = _settings.BackgroundColorValue;
                    }
                }
                else
                {
                    // 透明化が無効な場合の設定
                    _settings.BackgroundColorValue = Color.FromArgb(255, 255, 255, 255);
                    
                    var pbBackgroundColor = _form.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                    if (pbBackgroundColor != null)
                    {
                        pbBackgroundColor.BackColor = _settings.BackgroundColorValue;
                    }
                }
                
                _setModified(true);
                
                Logger.LogInfo("OptionsFormBackgroundHandlers.ApplyTransparencySettings", 
                    $"透明化設定を適用しました: Enable={_settings.EnableTransparency}, Opacity={_settings.Opacity}");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBackgroundHandlers.ApplyTransparencySettings", "透明化設定エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"透明化の設定に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 背景色変更
        /// </summary>
        public void ChangeBackgroundColor()
        {
            try
            {
                // テスト環境ではダイアログを表示しない
                if (IsTestEnvironment())
                {
                    Logger.LogInfo("OptionsFormBackgroundHandlers.ChangeBackgroundColor", "テスト環境のため、カラーダイアログをスキップしました");
                    return;
                }

                using var colorDialog = new ColorDialog();
                colorDialog.AnyColor = true;
                colorDialog.Color = _settings.BackgroundColorValue;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _settings.BackgroundColorValue = colorDialog.Color;
                    
                    var pbBackgroundColor = _form.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                    if (pbBackgroundColor != null)
                    {
                        pbBackgroundColor.BackColor = colorDialog.Color;
                    }
                    _setModified(true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBackgroundHandlers.ChangeBackgroundColor", "背景色変更エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"背景色の変更に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 背景色ボタンのクリックイベント
        /// </summary>
        public void BackgroundColorButton_Click(object? sender, EventArgs e)
        {
            ChangeBackgroundColor();
        }

        /// <summary>
        /// 透明化設定の適用
        /// </summary>
        public void TransparentButton_Click(object? sender, EventArgs e)
        {
            ApplyTransparencySettings();
        }

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            try
            {
                // 環境変数でダイアログ無効化が設定されている場合のみ
                var disableDialogs = Environment.GetEnvironmentVariable("DISABLE_DIALOGS");
                if (!string.IsNullOrEmpty(disableDialogs) && disableDialogs.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // アセンブリ名に"Test"が含まれている場合のみ（テストプロジェクトの場合）
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
