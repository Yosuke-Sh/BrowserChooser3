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
        private readonly FocusSettings _focusSettings;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormAccessibilityHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="focusSettings">フォーカス設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        public OptionsFormAccessibilityHandlers(OptionsForm form, FocusSettings focusSettings, Action<bool> setModified)
        {
            _form = form;
            _focusSettings = focusSettings;
            _setModified = setModified;
        }

        /// <summary>
        /// アクセシビリティ設定を開く
        /// </summary>
        public void OpenAccessibilitySettings()
        {
            try
            {
                // アクセシビリティ設定ダイアログを表示
                var accessibilityForm = new AccessibilitySettingsForm();
                if (accessibilityForm.ShowDialog() == DialogResult.OK)
                {
                    // 設定を更新
                    _focusSettings.ShowFocus = accessibilityForm.ShowFocus;
                    _focusSettings.BoxColor = accessibilityForm.FocusBoxColor;
                    _focusSettings.BoxWidth = accessibilityForm.FocusBoxWidth;
                    _setModified(true);
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
