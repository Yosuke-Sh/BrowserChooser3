using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのチェックボックスイベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormCheckBoxHandlers
    {
        private readonly OptionsForm _form;
        private readonly Settings _settings;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormCheckBoxHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        public OptionsFormCheckBoxHandlers(OptionsForm form, Settings settings, Action<bool> setModified)
        {
            _form = form;
            _settings = settings;
            _setModified = setModified;
        }

        /// <summary>
        /// 設定変更を検知してダーティフラグを設定
        /// </summary>
        public void DetectDirty(object sender, EventArgs e)
        {
            _setModified(true);
        }

        /// <summary>
        /// 正規化設定の変更時の処理
        /// </summary>
        public void ChkCanonicalize_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var txtCanonicalizeAppend = _form.Controls.Find("txtCanonicalizeAppend", true).FirstOrDefault() as TextBox;
                if (txtCanonicalizeAppend != null)
                {
                    txtCanonicalizeAppend.Enabled = checkBox.Checked;
                }
            }
            _setModified(true);
        }

        /// <summary>
        /// ログ設定の変更時の処理
        /// </summary>
        public void ChkLog_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                // ログ設定を更新
                _settings.EnableLogging = checkBox.Checked;
            }
            _setModified(true);
        }
    }
}
