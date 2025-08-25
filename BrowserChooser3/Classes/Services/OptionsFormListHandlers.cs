using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services
{
    /// <summary>
    /// OptionsFormのリスト選択変更イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormListHandlers
    {
        private readonly OptionsForm _form;

        /// <summary>
        /// OptionsFormListHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        public OptionsFormListHandlers(OptionsForm form)
        {
            _form = form;
        }

        /// <summary>
        /// ブラウザリストの選択変更イベント
        /// </summary>
        public void LstBrowsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = _form.Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
                var cloneButton = _form.Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
                var defaultButton = _form.Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (cloneButton != null) cloneButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;
                if (defaultButton != null) defaultButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = _form.Controls.Find("lblDoubleClickBrowsersNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = _form.Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
                var cloneButton = _form.Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
                var defaultButton = _form.Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (cloneButton != null) cloneButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;
                if (defaultButton != null) defaultButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = _form.Controls.Find("lblDoubleClickBrowsersNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// Auto URLsリストの選択変更イベント
        /// </summary>
        public void LstURLs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = _form.Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
                var moveUpButton = _form.Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
                var moveDownButton = _form.Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;
                if (moveUpButton != null) moveUpButton.Enabled = true;
                if (moveDownButton != null) moveDownButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = _form.Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = _form.Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
                var moveUpButton = _form.Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
                var moveDownButton = _form.Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;
                if (moveUpButton != null) moveUpButton.Enabled = false;
                if (moveDownButton != null) moveDownButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = _form.Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// プロトコルリストの選択変更イベント
        /// </summary>
        public void LstProtocols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = _form.Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = _form.Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = _form.Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = _form.Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// ファイルタイプリストの選択変更イベント
        /// </summary>
        public void LstFileTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = _form.Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = _form.Controls.Find("lblDoubleClickFileTypesNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = _form.Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = _form.Controls.Find("lblDoubleClickFileTypesNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }
    }
}
