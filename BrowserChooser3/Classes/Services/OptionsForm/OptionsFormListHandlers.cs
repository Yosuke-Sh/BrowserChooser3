using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
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
        public void LstURLs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "Auto URLs選択変更イベントが呼び出されました");
            
            if (sender is not ListView listView) 
            {
                Logger.LogWarning("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "senderがListViewではありません");
                return;
            }

            Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", $"選択されたアイテム数: {listView.SelectedIndices.Count}");

            if (listView.SelectedIndices.Count > 0)
            {
                // Auto URLsタブを取得
                var autoUrlsTab = _form.tabSettings.TabPages["tabAutoURLs"];
                if (autoUrlsTab != null)
                {
                    // ボタンを有効化
                    var editButton = autoUrlsTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var deleteButton = autoUrlsTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;
                    var moveUpButton = autoUrlsTab.Controls.Find("btnMoveUp", true).FirstOrDefault() as Button;
                    var moveDownButton = autoUrlsTab.Controls.Find("btnMoveDown", true).FirstOrDefault() as Button;

                if (editButton != null) 
                {
                    editButton.Enabled = true;
                    Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "Editボタンを有効化しました");
                }
                if (deleteButton != null) 
                {
                    deleteButton.Enabled = true;
                    Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "Deleteボタンを有効化しました");
                }
                if (moveUpButton != null) 
                {
                    moveUpButton.Enabled = true;
                    Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "MoveUpボタンを有効化しました");
                }
                if (moveDownButton != null) 
                {
                    moveDownButton.Enabled = true;
                    Logger.LogInfo("OptionsFormListHandlers.LstURLs_SelectedIndexChanged", "MoveDownボタンを有効化しました");
                }

                // ダブルクリック注釈を表示
                var noteLabel = autoUrlsTab.Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
                }
            }
            else
            {
                // Auto URLsタブを取得
                var autoUrlsTab = _form.tabSettings.TabPages["tabAutoURLs"];
                if (autoUrlsTab != null)
                {
                    // ボタンを無効化
                    var editButton = autoUrlsTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var deleteButton = autoUrlsTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;
                    var moveUpButton = autoUrlsTab.Controls.Find("btnMoveUp", true).FirstOrDefault() as Button;
                    var moveDownButton = autoUrlsTab.Controls.Find("btnMoveDown", true).FirstOrDefault() as Button;

                    if (editButton != null) editButton.Enabled = false;
                    if (deleteButton != null) deleteButton.Enabled = false;
                    if (moveUpButton != null) moveUpButton.Enabled = false;
                    if (moveDownButton != null) moveDownButton.Enabled = false;

                    // ダブルクリック注釈を非表示
                    var noteLabel = autoUrlsTab.Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                    if (noteLabel != null) noteLabel.Visible = false;
                }
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
                var editButton = _form.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("btnDelete", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = _form.Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = _form.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                var deleteButton = _form.Controls.Find("btnDelete", true).FirstOrDefault() as Button;

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
