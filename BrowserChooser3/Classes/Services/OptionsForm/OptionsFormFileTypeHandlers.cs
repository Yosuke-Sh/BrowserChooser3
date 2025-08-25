using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// ファイルタイプパネルのイベントハンドラー
    /// </summary>
    public class OptionsFormFileTypeHandlers
    {
        private readonly OptionsForm _form;
        private readonly Dictionary<int, FileType> _mFileTypes;
        private readonly Dictionary<int, Browser> _mBrowser;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormFileTypeHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">オプションフォームのインスタンス</param>
        /// <param name="mFileTypes">ファイルタイプの辞書</param>
        /// <param name="mBrowser">ブラウザの辞書</param>
        /// <param name="setModified">変更フラグを設定するアクション</param>
        public OptionsFormFileTypeHandlers(
            OptionsForm form,
            Dictionary<int, FileType> mFileTypes,
            Dictionary<int, Browser> mBrowser,
            Action<bool> setModified)
        {
            _form = form;
            _mFileTypes = mFileTypes;
            _mBrowser = mBrowser;
            _setModified = setModified;
        }

        /// <summary>
        /// ファイルタイプ追加ボタンのクリックイベント
        /// </summary>
        public void AddFileType_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormFileTypeHandlers.AddFileType_Click", "ファイルタイプ追加開始");

                // 新しいファイルタイプIDを生成
                var newId = _mFileTypes.Count > 0 ? _mFileTypes.Keys.Max() + 1 : 1;

                // 新しいファイルタイプを作成
                var newFileType = new FileType
                {
                    Name = $"New File Type {newId}",
                    Extension = $".new{newId}",
                    BrowserGuid = _mBrowser.Count > 0 ? _mBrowser.Values.First().Guid : Guid.Empty,
                    IsActive = true
                };

                _mFileTypes.Add(newId, newFileType);
                _setModified(true);

                // ListViewを更新
                RefreshFileTypesListView();

                Logger.LogInfo("OptionsFormFileTypeHandlers.AddFileType_Click", "ファイルタイプ追加完了", newId);
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFileTypeHandlers.AddFileType_Click", "ファイルタイプ追加エラー", ex.Message);
                MessageBox.Show($"ファイルタイプの追加に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ファイルタイプ編集ボタンのクリックイベント
        /// </summary>
        public void EditFileType_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormFileTypeHandlers.EditFileType_Click", "ファイルタイプ編集開始");

                var fileTypesTab = _form.tabSettings.TabPages["tabFileTypes"];
                if (fileTypesTab != null)
                {
                    var listView = fileTypesTab.Controls.Find("lstFileTypes", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedItem = listView.SelectedItems[0];
                        if (selectedItem.Tag is FileType fileType)
                        {
                            // ファイルタイプ編集ダイアログを表示
                            // TODO: ファイルタイプ編集フォームを実装
                            MessageBox.Show($"ファイルタイプ '{fileType.Name}' の編集機能は未実装です。", "情報",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("編集するファイルタイプを選択してください。", "情報",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFileTypeHandlers.EditFileType_Click", "ファイルタイプ編集エラー", ex.Message);
                MessageBox.Show($"ファイルタイプの編集に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ファイルタイプ削除ボタンのクリックイベント
        /// </summary>
        public void DeleteFileType_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormFileTypeHandlers.DeleteFileType_Click", "ファイルタイプ削除開始");

                var fileTypesTab = _form.tabSettings.TabPages["tabFileTypes"];
                if (fileTypesTab != null)
                {
                    var listView = fileTypesTab.Controls.Find("lstFileTypes", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedItem = listView.SelectedItems[0];
                        if (selectedItem.Tag is FileType fileType)
                        {
                            var result = MessageBox.Show($"ファイルタイプ '{fileType.Name}' を削除しますか？", "確認",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                var keyToRemove = _mFileTypes.FirstOrDefault(kvp => kvp.Value.Guid == fileType.Guid).Key;
                                if (keyToRemove != 0)
                                {
                                    _mFileTypes.Remove(keyToRemove);
                                    _setModified(true);
                                    RefreshFileTypesListView();
                                    Logger.LogInfo("OptionsFormFileTypeHandlers.DeleteFileType_Click", "ファイルタイプ削除完了", fileType.Guid);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("削除するファイルタイプを選択してください。", "情報",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFileTypeHandlers.DeleteFileType_Click", "ファイルタイプ削除エラー", ex.Message);
                MessageBox.Show($"ファイルタイプの削除に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ファイルタイプListViewの更新
        /// </summary>
        private void RefreshFileTypesListView()
        {
            try
            {
                var fileTypesTab = _form.tabSettings.TabPages["tabFileTypes"];
                if (fileTypesTab != null)
                {
                    var listView = fileTypesTab.Controls.Find("lstFileTypes", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        listView.Items.Clear();
                        foreach (var fileType in _mFileTypes.Values)
                        {
                            var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == fileType.BrowserGuid);
                            var item = new ListViewItem(fileType.Name)
                            {
                                Tag = fileType
                            };
                            item.SubItems.Add(browser?.Name ?? "Unknown");
                            item.SubItems.Add("Default App"); // TODO: デフォルトアプリの取得
                            listView.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFileTypeHandlers.RefreshFileTypesListView", "ListView更新エラー", ex.Message);
            }
        }
    }
}
