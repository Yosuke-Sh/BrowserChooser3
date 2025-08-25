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
        /// 透明背景設定
        /// </summary>
        public void SetTransparentBackground()
        {
            var pbBackgroundColor = _form.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
            if (pbBackgroundColor != null)
            {
                pbBackgroundColor.BackColor = Color.Transparent;
            }
            _setModified(true);
        }

        /// <summary>
        /// 背景色変更
        /// </summary>
        public void ChangeBackgroundColor()
        {
            try
            {
                var colorDialog = new ColorDialog();
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
    }
}
