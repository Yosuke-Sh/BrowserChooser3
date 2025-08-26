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
                // 現在の設定をフォームに反映
                var accessibilityForm = new AccessibilitySettingsForm();
                accessibilityForm.ShowFocus = _settings.ShowFocus;
                accessibilityForm.FocusBoxColor = Color.FromArgb(_settings.FocusBoxColor);
                accessibilityForm.FocusBoxWidth = _settings.FocusBoxWidth;
                
                // アクセシビリティ設定ダイアログを表示
                if (accessibilityForm.ShowDialog() == DialogResult.OK)
                {
                    // 設定を更新
                    _settings.ShowFocus = accessibilityForm.ShowFocus;
                    _settings.FocusBoxColor = accessibilityForm.FocusBoxColor.ToArgb();
                    _settings.FocusBoxWidth = accessibilityForm.FocusBoxWidth;
                    _setModified(true);
                    
                    Logger.LogInfo("OptionsFormAccessibilityHandlers.OpenAccessibilitySettings", "アクセシビリティ設定を保存しました");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormAccessibilityHandlers.OpenAccessibilitySettings", "アクセシビリティ設定エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"アクセシビリティ設定に失敗しました: {ex.Message}", "エラー", 
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
    }
}
