using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Services.BrowserServices;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのブラウザ管理イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormBrowserHandlers
    {
        private readonly OptionsForm _form;
        private readonly Settings _settings;
        private readonly Dictionary<int, Browser> _mBrowser;
        private readonly Dictionary<int, Protocol> _mProtocols;
        private readonly Dictionary<int, FileType> _mFileTypes;
        private readonly ImageList? _imBrowserIcons;
        private readonly Action<bool> _setModified;

        /// <summary>
        /// OptionsFormBrowserHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="mProtocols">プロトコル辞書</param>
        /// <param name="mFileTypes">ファイルタイプ辞書</param>
        /// <param name="imBrowserIcons">ブラウザアイコンリスト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        public OptionsFormBrowserHandlers(
            OptionsForm form,
            Settings settings,
            Dictionary<int, Browser> mBrowser,
            Dictionary<int, Protocol> mProtocols,
            Dictionary<int, FileType> mFileTypes,
            ImageList? imBrowserIcons,
            Action<bool> setModified)
        {
            _form = form;
            _settings = settings;
            _mBrowser = mBrowser;
            _mProtocols = mProtocols;
            _mFileTypes = mFileTypes;
            _imBrowserIcons = imBrowserIcons;
            _setModified = setModified;
        }

        /// <summary>
        /// ブラウザ追加ボタンのクリックイベント
        /// </summary>
        public void AddBrowser_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("OptionsFormBrowserHandlers.AddBrowser_Click", "ブラウザ追加ボタンがクリックされました");
            try
            {
                var addEditForm = new AddEditBrowserForm();
                if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens, 
                    new Point(_settings.GridWidth, _settings.GridHeight)))
                {
                    var newBrowser = addEditForm.GetData();
                    
                    // rowとcolを自動設定（重複を避ける）
                    var maxRow = _mBrowser.Values.Max(b => b.PosY);
                    newBrowser.PosY = maxRow + 1;
                    newBrowser.PosX = 0;
                    
                    var newIndex = _mBrowser.Count + 1;
                    _mBrowser.Add(newIndex, newBrowser);
                    
                    // ListViewにアイテムを追加（タブページ内から検索）
                    var browsersTab = _form.tabSettings.TabPages["tabBrowsers"];
                    var listView = browsersTab?.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        var item = listView.Items.Add(newBrowser.Name);
                        item.Tag = newIndex;
                        item.SubItems.Add(""); // Default column
                        item.SubItems.Add(newBrowser.PosY.ToString());
                        item.SubItems.Add(newBrowser.PosX.ToString());
                        item.SubItems.Add(newBrowser.Hotkey.ToString());
                        item.SubItems.Add(GetBrowserProtocolsAndFileTypes(newBrowser));
                        
                        // アイコンを設定
                        if (_imBrowserIcons != null)
                        {
                            var image = ImageUtilities.GetImage(newBrowser, false);
                            if (image != null)
                            {
                                _imBrowserIcons.Images.Add(image);
                                item.ImageIndex = _imBrowserIcons.Images.Count - 1;
                            }
                        }
                    }
                    
                    _setModified(true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBrowserHandlers.AddBrowser_Click", "ブラウザ追加エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ追加に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザ編集ボタンのクリックイベント
        /// </summary>
        public void EditBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var browsersTab = _form.tabSettings.TabPages["tabBrowsers"];
                var listView = browsersTab?.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                    if (selectedIndex == -1 || !_mBrowser.ContainsKey(selectedIndex)) return;
                    
                    var addEditForm = new AddEditBrowserForm();
                    if (addEditForm.EditBrowser(_mBrowser[selectedIndex], _mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens))
                    {
                        var updatedBrowser = addEditForm.GetData();
                        _mBrowser[selectedIndex] = updatedBrowser;
                        
                        // ListViewアイテムの更新
                        var selectedItem = listView.SelectedItems[0];
                        selectedItem.Text = updatedBrowser.Name;
                        selectedItem.SubItems[1].Text = ""; // Default column
                        selectedItem.SubItems[2].Text = updatedBrowser.PosY.ToString();
                        selectedItem.SubItems[3].Text = updatedBrowser.PosX.ToString();
                        selectedItem.SubItems[4].Text = updatedBrowser.Hotkey.ToString();
                        selectedItem.SubItems[5].Text = GetBrowserProtocolsAndFileTypes(updatedBrowser);
                        
                        // ImageListアイコンの更新
                        if (_imBrowserIcons != null && selectedItem.Index < _imBrowserIcons.Images.Count)
                        {
                            var image = ImageUtilities.GetImage(updatedBrowser, false);
                            if (image != null)
                            {
                                _imBrowserIcons.Images[selectedItem.Index] = image;
                            }
                        }
                        
                        _setModified(true);
                    }
                }
                else
                {
                    MessageBox.Show("編集するブラウザを選択してください。", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBrowserHandlers.EditBrowser_Click", "ブラウザ編集エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ編集に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザ削除ボタンのクリックイベント
        /// </summary>
        public void DeleteBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var browsersTab = _form.tabSettings.TabPages["tabBrowsers"];
                var listView = browsersTab?.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var browserName = listView.SelectedItems[0].Text;
                    var result = MessageBox.Show($"Are you sure you want to delete the {browserName} browser?", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex != -1)
                        {
                            _mBrowser.Remove(selectedIndex);
                            listView.Items.Remove(listView.SelectedItems[0]);
                            _setModified(true);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("削除するブラウザを選択してください。", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBrowserHandlers.DeleteBrowser_Click", "ブラウザ削除エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ削除に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザ複製ボタンのクリックイベント
        /// </summary>
        public void CloneBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var browsersTab = _form.tabSettings.TabPages["tabBrowsers"];
                var listView = browsersTab?.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                    if (selectedIndex == -1 || !_mBrowser.ContainsKey(selectedIndex)) return;
                    
                    var templateBrowser = _mBrowser[selectedIndex];
                    var addEditForm = new AddEditBrowserForm();
                    if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens, 
                        new Point(_settings.GridWidth, _settings.GridHeight), templateBrowser))
                    {
                        var clonedBrowser = addEditForm.GetData();
                        clonedBrowser.Name = $"{clonedBrowser.Name} (Copy)";
                        clonedBrowser.Guid = Guid.NewGuid();
                        
                        _mBrowser.Add(_mBrowser.Count + 1, clonedBrowser);
                        
                        // ListViewにアイテムを追加
                        if (listView != null)
                        {
                            var item = listView.Items.Add(clonedBrowser.Name);
                            item.Tag = _mBrowser.Count;
                            item.SubItems.Add(""); // Default column
                            item.SubItems.Add(clonedBrowser.PosY.ToString());
                            item.SubItems.Add(clonedBrowser.PosX.ToString());
                            item.SubItems.Add(clonedBrowser.Hotkey.ToString());
                            item.SubItems.Add(GetBrowserProtocolsAndFileTypes(clonedBrowser));
                        }
                        
                        // ImageListにアイコンを追加
                        if (_imBrowserIcons != null)
                        {
                            var image = ImageUtilities.GetImage(clonedBrowser, false);
                            if (image != null)
                            {
                                _imBrowserIcons.Images.Add(image);
                            }
                        }
                        
                        _setModified(true);
                    }
                }
                else
                {
                    MessageBox.Show("複製するブラウザを選択してください。", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBrowserHandlers.CloneBrowser_Click", "ブラウザ複製エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ複製に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザ検出ボタンのクリックイベント
        /// </summary>
        public void DetectBrowsers_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("OptionsFormBrowserHandlers.DetectBrowsers_Click", "ブラウザ検出を開始");
            try
            {
                // Webブラウザのみを検出するかどうかを確認
                var result = MessageBox.Show(
                    "Webブラウザのみを検出しますか？\n\n「はい」: Webブラウザのみ\n「いいえ」: すべてのアプリケーション",
                    "ブラウザ検出オプション",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    return;
                }

                var webBrowsersOnly = (result == DialogResult.Yes);
                
                // ブラウザ検出を実行
                var detectedBrowsers = DetectedBrowsers.DoBrowserDetection(webBrowsersOnly);
                Logger.LogInfo("OptionsFormBrowserHandlers.DetectBrowsers_Click", $"検出されたブラウザ数: {detectedBrowsers.Count}", webBrowsersOnly);
                
                var missingBrowsers = new List<Browser>();

                // 既存のブラウザと比較して不足しているものを特定
                foreach (var detectedBrowser in detectedBrowsers)
                {
                    bool found = false;
                    foreach (var existingBrowser in _mBrowser.Values)
                    {
                        if (existingBrowser.Target == detectedBrowser.Target)
                        {
                            found = true;
                            break;
                        }
                    }
                    
                    if (!found)
                    {
                        missingBrowsers.Add(detectedBrowser);
                    }
                }

                Logger.LogInfo("OptionsFormBrowserHandlers.DetectBrowsers_Click", $"不足しているブラウザ数: {missingBrowsers.Count}");

                if (missingBrowsers.Count > 0)
                {
                    var addResult = MessageBox.Show(
                        $"{missingBrowsers.Count}個の新しいブラウザが見つかりました。追加しますか？",
                        "ブラウザ検出",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // 正しいListViewを取得（タブページ内から検索）
                        var browsersTab = _form.tabSettings.TabPages["tabBrowsers"];
                        var listView = browsersTab?.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                        Logger.LogInfo("OptionsFormBrowserHandlers.DetectBrowsers_Click", $"ListView: {(listView != null ? "見つかりました" : "見つかりませんでした")}");
                        
                                                 if (listView != null)
                         {
                             int rowIndex = 0;
                             foreach (var browser in missingBrowsers)
                             {
                                 // rowとcolを自動設定（重複を避ける）
                                 browser.PosY = rowIndex;
                                 browser.PosX = 0;
                                 rowIndex++;
                                 
                                 var newIndex = _mBrowser.Count + 1;
                                 _mBrowser.Add(newIndex, browser);
                                 
                                 var item = listView.Items.Add(browser.Name);
                                 item.Tag = newIndex;
                                 item.SubItems.Add(""); // Default column
                                 item.SubItems.Add(browser.PosY.ToString());
                                 item.SubItems.Add(browser.PosX.ToString());
                                 item.SubItems.Add(browser.Hotkey.ToString());
                                 item.SubItems.Add(GetBrowserProtocolsAndFileTypes(browser));
                                 
                                 // ImageListにアイコンを追加
                                 if (_imBrowserIcons != null)
                                 {
                                     var image = ImageUtilities.GetImage(browser, false);
                                     if (image != null)
                                     {
                                         _imBrowserIcons.Images.Add(image);
                                     }
                                 }
                             }
                            
                            _setModified(true);
                            MessageBox.Show($"{missingBrowsers.Count}個のブラウザを追加しました。", "完了", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Logger.LogError("OptionsFormBrowserHandlers.DetectBrowsers_Click", "ListViewが見つかりませんでした");
                            MessageBox.Show("ブラウザリストが見つかりませんでした。", "エラー", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("新しいブラウザは見つかりませんでした。", "ブラウザ検出", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormBrowserHandlers.DetectBrowsers_Click", "ブラウザ検出エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ検出に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザのプロトコルとファイルタイプを取得
        /// </summary>
        public string GetBrowserProtocolsAndFileTypes(Browser browser)
        {
            var protocols = _mProtocols.Values.Where(p => p.BrowserGuid == browser.Guid).Select(p => p.Name);
            var fileTypes = _mFileTypes.Values.Where(f => f.BrowserGuid == browser.Guid).Select(f => f.Name);
            
            var allItems = protocols.Concat(fileTypes);
            return string.Join(", ", allItems);
        }
    }
}
