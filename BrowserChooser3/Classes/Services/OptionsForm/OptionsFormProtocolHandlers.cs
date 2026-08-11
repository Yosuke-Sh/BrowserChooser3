using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// プロトコルパネルのイベントハンドラー
    /// </summary>
    public class OptionsFormProtocolHandlers
    {
        private readonly OptionsForm _form;
        private readonly Dictionary<int, Protocol> _mProtocols;
        private readonly Dictionary<int, Browser> _mBrowser;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormProtocolHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">オプションフォームのインスタンス</param>
        /// <param name="mProtocols">プロトコルの辞書</param>
        /// <param name="mBrowser">ブラウザの辞書</param>
        /// <param name="setModified">変更フラグを設定するアクション</param>
        public OptionsFormProtocolHandlers(
            OptionsForm form,
            Dictionary<int, Protocol> mProtocols,
            Dictionary<int, Browser> mBrowser,
            Action<bool> setModified)
        {
            _form = form;
            _mProtocols = mProtocols;
            _mBrowser = mBrowser;
            _setModified = setModified;
        }

        /// <summary>
        /// プロトコル追加ボタンのクリックイベント
        /// </summary>
        public void AddProtocol_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormProtocolHandlers.AddProtocol_Click", "プロトコル追加開始");

                var addEditForm = new AddEditProtocolForm();
                if (addEditForm.AddProtocol(_mBrowser))
                {
                    var newProtocol = addEditForm.GetData();
                    var newId = _mProtocols.Count > 0 ? _mProtocols.Keys.Max() + 1 : 1;
                    _mProtocols.Add(newId, newProtocol);
                    
                    // ListViewにアイテムを追加
                    var protocolsTab = _form.tabSettings.TabPages["tabProtocols"];
                    var listView = protocolsTab?.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == newProtocol.BrowserGuid);
                        var item = new ListViewItem(newProtocol.Name)
                        {
                            Tag = newId
                        };
                        item.SubItems.Add(browser?.Name ?? "Unknown");
                        item.SubItems.Add(newProtocol.IsActive ? "Yes" : "No");
                        listView.Items.Add(item);

                        // 追加直後に選択してボタンを有効化
                        item.Selected = true;
                    }
                    
                    _setModified(true);
                    Logger.LogInfo("OptionsFormProtocolHandlers.AddProtocol_Click", "プロトコル追加完了", newId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormProtocolHandlers.AddProtocol_Click", "プロトコル追加エラー", ex.Message);
                MessageBox.Show($"プロトコルの追加に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// プロトコル編集ボタンのクリックイベント
        /// </summary>
        public void EditProtocol_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormProtocolHandlers.EditProtocol_Click", "プロトコル編集開始 - イベントが呼び出されました");

                var protocolsTab = _form.tabSettings.TabPages["tabProtocols"];
                if (protocolsTab != null)
                {
                    var listView = protocolsTab.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex != -1 && _mProtocols.ContainsKey(selectedIndex))
                        {
                            var addEditForm = new AddEditProtocolForm();
                            if (addEditForm.EditProtocol(_mProtocols[selectedIndex], _mBrowser))
                            {
                                var updatedProtocol = addEditForm.GetData();
                                _mProtocols[selectedIndex] = updatedProtocol;
                                
                                // ListViewアイテムの更新
                                var selectedItem = listView.SelectedItems[0];
                                selectedItem.Text = updatedProtocol.Name;
                                selectedItem.SubItems[1].Text = _mBrowser.Values.FirstOrDefault(b => b.Guid == updatedProtocol.BrowserGuid)?.Name ?? "";
                                selectedItem.SubItems[2].Text = updatedProtocol.IsActive ? "Yes" : "No";
                                
                                _setModified(true);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("編集するプロトコルを選択してください。", "情報",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormProtocolHandlers.EditProtocol_Click", "プロトコル編集エラー", ex.Message);
                MessageBox.Show($"プロトコルの編集に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// プロトコル削除ボタンのクリックイベント
        /// </summary>
        public void DeleteProtocol_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsFormProtocolHandlers.DeleteProtocol_Click", "プロトコル削除開始 - イベントが呼び出されました");

                var protocolsTab = _form.tabSettings.TabPages["tabProtocols"];
                if (protocolsTab != null)
                {
                    var listView = protocolsTab.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedItem = listView.SelectedItems[0];
                        var selectedIndex = selectedItem.Tag is int tag ? tag : -1;
                        if (selectedIndex != -1 && _mProtocols.ContainsKey(selectedIndex))
                        {
                            var protocol = _mProtocols[selectedIndex];
                            var result = MessageBox.Show($"プロトコル '{protocol.Name}' を削除しますか？", "確認",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                _mProtocols.Remove(selectedIndex);
                                listView.Items.Remove(selectedItem);
                                _setModified(true);
                                Logger.LogInfo("OptionsFormProtocolHandlers.DeleteProtocol_Click", "プロトコル削除完了", protocol.Guid);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("削除するプロトコルを選択してください。", "情報",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormProtocolHandlers.DeleteProtocol_Click", "プロトコル削除エラー", ex.Message);
                MessageBox.Show($"プロトコルの削除に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// プロトコルListViewの更新
        /// </summary>
        private void RefreshProtocolsListView()
        {
            try
            {
                var protocolsTab = _form.tabSettings.TabPages["tabProtocols"];
                if (protocolsTab != null)
                {
                    var listView = protocolsTab.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        listView.Items.Clear();
                        foreach (var protocol in _mProtocols.Values)
                        {
                            var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == protocol.BrowserGuid);
                            var item = new ListViewItem(protocol.Name)
                            {
                                Tag = protocol
                            };
                            item.SubItems.Add(browser?.Name ?? "Unknown");
                            item.SubItems.Add(protocol.IsActive ? "Yes" : "No");
                            listView.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormProtocolHandlers.RefreshProtocolsListView", "ListView更新エラー", ex.Message);
            }
        }
    }
}
