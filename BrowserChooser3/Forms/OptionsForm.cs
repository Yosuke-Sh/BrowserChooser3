using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// オプション設定画面
    /// ブラウザ設定、URL設定、一般設定などを管理します
    /// </summary>
    public partial class OptionsForm : Form
    {
        private Settings _settings;
        private bool _isModified = false;
        


        // イベントハンドラークラス
        private OptionsFormFormHandlers _formHandlers;
        private OptionsFormBrowserHandlers _browserHandlers;
        private OptionsFormProtocolHandlers _protocolHandlers;
        private OptionsFormListHandlers _listHandlers;
        private OptionsFormDragDropHandlers _dragDropHandlers;

        // UIパネル作成クラス
        private OptionsFormPanels _panels;

        // 内部データ管理（Browser Chooser 2互換）
        private Dictionary<int, Browser> _mBrowser = new();
        private SortedDictionary<int, URL> _mURLs = new();
        private Dictionary<int, Protocol> _mProtocols = new();
        private int _mLastBrowserID = 0;
        private int _mLastURLID = 0;
        private int _mLastProtocolID = 0;

        // ImageList（Browser Chooser 2互換）
        private ImageList? _imBrowserIcons => _panels?.GetBrowserIcons();

        // フォーカス設定（Browser Chooser 2互換）
        private FocusSettings _mFocusSettings = new();

        /// <summary>
        /// OptionsFormクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public OptionsForm(Settings settings)
        {
            _settings = settings;

            // イベントハンドラークラスの初期化
            _formHandlers = new OptionsFormFormHandlers(this, LoadSettingsToControls, SaveSettings, () => _isModified);
            _browserHandlers = new OptionsFormBrowserHandlers(this, _settings, _mBrowser, _mProtocols, null!, _imBrowserIcons, SetModified);
            _protocolHandlers = new OptionsFormProtocolHandlers(this, _mProtocols, _mBrowser, SetModified);
            _listHandlers = new OptionsFormListHandlers(this);
            _dragDropHandlers = new OptionsFormDragDropHandlers(this, _settings, _mBrowser, _mProtocols, null!, SetModified, RebuildAutoURLs);

            // UIパネル作成クラスの初期化
            _panels = new OptionsFormPanels();

            InitializeForm();

            // フォームイベントの設定
            FormClosing += _formHandlers.OptionsForm_FormClosing;
            Shown += _formHandlers.OptionsForm_Shown;
        }


        
        /// <summary>
        /// 設定オブジェクトを取得
        /// </summary>
        /// <returns>設定オブジェクト</returns>
        public Settings GetSettings()
        {
            return _settings;
        }
        
        /// <summary>
        /// フォームの初期化（Browser Chooser 2互換）
        /// </summary>
        private void InitializeForm()
        {
            Logger.LogInfo("OptionsForm.InitializeForm", "Start");

            try
            {
                // Designerで定義されたコンポーネントを初期化
                InitializeComponent();

                // テスト環境かどうかを判定
                bool isTestEnvironment = IsTestEnvironment();

                // TreeViewノードの作成
                var commonNode = new TreeNode("Common");
                commonNode.Nodes.Add(new TreeNode("Browsers & app") { Tag = "tabBrowsers" });
                commonNode.Nodes.Add(new TreeNode("Auto URLs") { Tag = "tabAutoURLs" });

                var associationsNode = new TreeNode("Associations");
                associationsNode.Nodes.Add(new TreeNode("Protocols") { Tag = "tabProtocols" });

                var settingsNode = new TreeNode("Settings");
                settingsNode.Nodes.Add(new TreeNode("Display") { Tag = "tabDisplay" });
                settingsNode.Nodes.Add(new TreeNode("Grid") { Tag = "tabGrid" });
                settingsNode.Nodes.Add(new TreeNode("Privacy") { Tag = "tabPrivacy" });
                settingsNode.Nodes.Add(new TreeNode("Startup") { Tag = "tabStartup" });
                settingsNode.Nodes.Add(new TreeNode("Others") { Tag = "tabOthers" });

                var defaultBrowserNode = new TreeNode("Windows Default") { Tag = "tabDefaultBrowser" };

                treeSettings.Nodes.Add(commonNode);
                treeSettings.Nodes.Add(associationsNode);
                treeSettings.Nodes.Add(settingsNode);
                treeSettings.Nodes.Add(defaultBrowserNode);

                // タブページの作成
                var browsersTab = _panels.CreateBrowsersPanel(_settings, _mBrowser, _mProtocols, null!, _mLastBrowserID, _imBrowserIcons, SetModified, RebuildAutoURLs);
                var autoUrlsTab = _panels.CreateAutoURLsPanel(_settings, _mURLs, _mBrowser, SetModified, RebuildAutoURLs);
                var protocolsTab = _panels.CreateProtocolsPanel(_settings, _mProtocols, _mBrowser, SetModified);
                var defaultBrowserTab = _panels.CreateDefaultBrowserPanel(_settings, SetModified);
                var displayTab = _panels.CreateDisplayPanel(_settings, SetModified);
                var gridTab = _panels.CreateGridPanel(_settings, SetModified);
                var privacyTab = _panels.CreatePrivacyPanel(_settings, SetModified);
                var startupTab = _panels.CreateStartupPanel(_settings, SetModified);
                var othersTab = _panels.CreateOthersPanel(_settings, SetModified);

                tabSettings.TabPages.Add(browsersTab);
                tabSettings.TabPages.Add(autoUrlsTab);
                tabSettings.TabPages.Add(protocolsTab);
                tabSettings.TabPages.Add(defaultBrowserTab);
                tabSettings.TabPages.Add(displayTab);
                tabSettings.TabPages.Add(gridTab);
                tabSettings.TabPages.Add(privacyTab);
                tabSettings.TabPages.Add(startupTab);
                tabSettings.TabPages.Add(othersTab);

                // TreeViewを展開
                treeSettings.ExpandAll();

                // テスト環境でない場合のみ設定の読み込みとイベントハンドラーの設定を実行
                if (!isTestEnvironment)
                {
                    // 設定の読み込み
                    LoadSettings();

                    // ボタンイベントハンドラーの設定
                    SetupButtonEventHandlers();
                    SetupPanelButtonEventHandlers();
                    
                    // ドラッグ&ドロップ機能の設定
                    SetupURLDragDrop();
                    SetupBrowserDragDrop();
                    
                    // カテゴリパネルの設定
                    SetupCategoriesPanel();
                }
                else
                {
                    Logger.LogInfo("OptionsForm.InitializeForm", "テスト環境のため、設定読み込みとイベントハンドラー設定をスキップしました");
                }

                // 初期化完了後にリサイズ処理を実行してレイアウトを調整
                AdjustLayout();

                Logger.LogInfo("OptionsForm.InitializeForm", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.InitializeForm", "初期化エラー", ex.Message, ex.StackTrace ?? "");
                if (!IsTestEnvironment())
                {
                    MessageBox.Show($"オプション画面の初期化に失敗しました: {ex.Message}", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private bool IsTestEnvironment()
        {
            try
            {
                // 環境変数でテスト環境を判定
                var testEnv = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnv) && testEnv.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // プロセス名に"test"が含まれている場合
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (processName.Contains("test", StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// カテゴリパネルの設定
        /// </summary>
        private void SetupCategoriesPanel()
        {
            try
            {
                // カテゴリパネルをタブページに追加
                var categoriesTab = tabSettings.TabPages["tabCategories"];
                if (categoriesTab != null && categoryPanel != null)
                {
                    categoryPanel.Dock = DockStyle.Fill;
                    categoriesTab.Controls.Add(categoryPanel);
                    categoryPanel.Visible = true;
                    
                    // カテゴリリストの初期化
                    LoadCategories();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupCategoriesPanel", "カテゴリパネル設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// パネルボタンイベントハンドラーの設定
        /// </summary>
        private void SetupPanelButtonEventHandlers()
        {
            Logger.LogInfo("OptionsForm.SetupPanelButtonEventHandlers", "パネルボタンイベントハンドラー設定開始");

            try
            {
                // ブラウザパネルのボタンイベントハンドラー設定
                SetupBrowserPanelButtons();
                
                // Auto URLsパネルのボタンイベントハンドラー設定
                SetupAutoURLsPanelButtons();
                
                // プロトコルパネルのボタンイベントハンドラー設定
                SetupProtocolsPanelButtons();
                
                // ファイルタイプパネルのボタンイベントハンドラー設定
                // SetupFileTypesPanelButtons();
                
                // Displayパネルのボタンイベントハンドラー設定
                SetupDisplayPanelButtons();
                
                Logger.LogInfo("OptionsForm.SetupPanelButtonEventHandlers", "パネルボタンイベントハンドラー設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupPanelButtonEventHandlers", "パネルボタンイベントハンドラー設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベントハンドラーの設定
        /// </summary>
        private void SetupButtonEventHandlers()
        {
            Logger.LogInfo("OptionsForm.SetupButtonEventHandlers", "ボタンイベントハンドラー設定開始");

            try
            {
                // メインボタンのイベントハンドラー設定
                saveButton.Click += _formHandlers.SaveButton_Click;
                helpButton.Click += _formHandlers.HelpButton_Click;
                
                // TabControlのイベントハンドラー設定
                tabSettings.SelectedIndexChanged += TabSettings_SelectedIndexChanged;
                
                Logger.LogInfo("OptionsForm.SetupButtonEventHandlers", "ボタンイベントハンドラー設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupButtonEventHandlers", "ボタンイベントハンドラー設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// ブラウザパネルのボタンイベントハンドラー設定
        /// </summary>
        private void SetupBrowserPanelButtons()
        {
            try
            {
                var browsersTab = tabSettings.TabPages["tabBrowsers"];
                if (browsersTab != null)
                {
                    var addButton = browsersTab.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
                    var editButton = browsersTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var cloneButton = browsersTab.Controls.Find("btnClone", true).FirstOrDefault() as Button;
                    var detectButton = browsersTab.Controls.Find("btnDetect", true).FirstOrDefault() as Button;
                    var deleteButton = browsersTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;

                    Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", $"Addボタン: {(addButton != null ? "見つかりました" : "見つかりませんでした")}");
                    Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", $"Editボタン: {(editButton != null ? "見つかりました" : "見つかりませんでした")}");
                    Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", $"Cloneボタン: {(cloneButton != null ? "見つかりました" : "見つかりませんでした")}");
                    Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", $"Detectボタン: {(detectButton != null ? "見つかりました" : "見つかりませんでした")}");
                    Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", $"Deleteボタン: {(deleteButton != null ? "見つかりました" : "見つかりませんでした")}");

                    if (addButton != null) 
                    {
                        addButton.Click += _browserHandlers.AddBrowser_Click;
                        Logger.LogInfo("OptionsForm.SetupBrowserPanelButtons", "Addボタンのイベントハンドラーを設定しました");
                    }
                    if (editButton != null) editButton.Click += _browserHandlers.EditBrowser_Click;
                    if (cloneButton != null) cloneButton.Click += _browserHandlers.CloneBrowser_Click;
                    if (detectButton != null) detectButton.Click += _browserHandlers.DetectBrowsers_Click;
                    if (deleteButton != null) deleteButton.Click += _browserHandlers.DeleteBrowser_Click;
                }
                else
                {
                    Logger.LogWarning("OptionsForm.SetupBrowserPanelButtons", "ブラウザタブが見つかりませんでした");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupBrowserPanelButtons", "ブラウザパネルボタン設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// Auto URLsパネルのボタンイベントハンドラー設定
        /// </summary>
        private void SetupAutoURLsPanelButtons()
        {
            try
            {
                var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                if (autoUrlsTab != null)
                {
                    var addButton = autoUrlsTab.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
                    var editButton = autoUrlsTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var deleteButton = autoUrlsTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;
                    var moveUpButton = autoUrlsTab.Controls.Find("btnMoveUp", true).FirstOrDefault() as Button;
                    var moveDownButton = autoUrlsTab.Controls.Find("btnMoveDown", true).FirstOrDefault() as Button;

                    var listView = autoUrlsTab.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    
                    if (addButton != null) addButton.Click += AddAutoURL_Click;
                    if (editButton != null) editButton.Click += EditAutoURL_Click;
                    if (deleteButton != null) deleteButton.Click += DeleteAutoURL_Click;
                    if (moveUpButton != null) moveUpButton.Click += MoveUpAutoURL_Click;
                    if (moveDownButton != null) moveDownButton.Click += MoveDownAutoURL_Click;
                    
                    // ListViewの選択変更イベントを設定
                    if (listView != null)
                    {
                        listView.SelectedIndexChanged += _listHandlers.LstURLs_SelectedIndexChanged;
                        Logger.LogInfo("OptionsForm.SetupAutoURLsPanelButtons", "Auto URLs ListViewのSelectedIndexChangedイベントを設定しました");
                        
                        // デバッグ用：ListViewの名前とイベントハンドラーの確認
                        Logger.LogInfo("OptionsForm.SetupAutoURLsPanelButtons", $"ListView名: {listView.Name}, イベントハンドラー数: {listView.GetType().GetEvents().Length}");

                        // 選択変更の補助（環境によってSelectedIndexChangedが遅延する場合のフォロー）
                        listView.ItemSelectionChanged += (s, e2) =>
                        {
                            try
                            {
                                _listHandlers.LstURLs_SelectedIndexChanged(listView, EventArgs.Empty);
                            }
                            catch (Exception ex2)
                            {
                                Logger.LogError("OptionsForm.SetupAutoURLsPanelButtons", "ItemSelectionChanged処理エラー", ex2.Message);
                            }
                        };
                    }
                    else
                    {
                        Logger.LogWarning("OptionsForm.SetupAutoURLsPanelButtons", "Auto URLs ListViewが見つかりませんでした");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupAutoURLsPanelButtons", "Auto URLsパネルボタン設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// Auto URL追加ボタンのクリックイベント
        /// </summary>
        private void AddAutoURL_Click(object? sender, EventArgs e)
        {
            try
            {
                var addEditForm = new AddEditURLForm();
                if (addEditForm.AddURL(_mBrowser))
                {
                    var newURL = addEditForm.GetData();
                    var newIndex = _mURLs.Count + 1;
                    _mURLs.Add(newIndex, newURL);
                    
                    // ListViewにアイテムを追加
                    var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                    var listView = autoUrlsTab?.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        var item = listView.Items.Add(newURL.URLPattern);
                        item.Tag = newIndex;
                        item.SubItems.Add(BrowserUtilities.GetBrowserByGUID(newURL.Guid, _mBrowser.Values.ToList())?.Name ?? "");
                        item.SubItems.Add(newURL.DelayTime < 0 ? "Default" : newURL.DelayTime.ToString());

                        // 追加直後に選択してボタンを有効化
                        item.Selected = true;
                        _listHandlers.LstURLs_SelectedIndexChanged(listView, EventArgs.Empty);
                    }
                    
                    _isModified = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.AddAutoURL_Click", "Auto URL追加エラー", ex.Message);
                MessageBox.Show($"Auto URL追加に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto URL編集ボタンのクリックイベント
        /// </summary>
        private void EditAutoURL_Click(object? sender, EventArgs e)
        {
            try
            {
                var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                var listView = autoUrlsTab?.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                    if (selectedIndex != -1 && _mURLs.ContainsKey(selectedIndex))
                    {
                        var addEditForm = new AddEditURLForm();
                        if (addEditForm.EditURL(_mURLs[selectedIndex], _mBrowser))
                        {
                            var updatedURL = addEditForm.GetData();
                            _mURLs[selectedIndex] = updatedURL;
                            
                            // ListViewアイテムの更新
                            var selectedItem = listView.SelectedItems[0];
                            selectedItem.Text = updatedURL.URLPattern;
                            selectedItem.SubItems[1].Text = BrowserUtilities.GetBrowserByGUID(updatedURL.Guid, _mBrowser.Values.ToList())?.Name ?? "";
                            selectedItem.SubItems[2].Text = updatedURL.DelayTime < 0 ? "Default" : updatedURL.DelayTime.ToString();
                            
                            _isModified = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("編集するAuto URLを選択してください。", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.EditAutoURL_Click", "Auto URL編集エラー", ex.Message);
                MessageBox.Show($"Auto URL編集に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto URL削除ボタンのクリックイベント
        /// </summary>
        private void DeleteAutoURL_Click(object? sender, EventArgs e)
        {
            try
            {
                var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                var listView = autoUrlsTab?.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var urlName = listView.SelectedItems[0].Text;
                    var result = MessageBox.Show($"Are you sure you want to delete the {urlName} entry?", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex != -1)
                        {
                            _mURLs.Remove(selectedIndex);
                            listView.Items.Remove(listView.SelectedItems[0]);
                            _isModified = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("削除するAuto URLを選択してください。", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.DeleteAutoURL_Click", "Auto URL削除エラー", ex.Message);
                MessageBox.Show($"Auto URL削除に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto URL上移動ボタンのクリックイベント
        /// </summary>
        private void MoveUpAutoURL_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsForm.MoveUpAutoURL_Click", "Auto URL上移動開始");
                var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                var listView = autoUrlsTab?.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var selectedIndex = listView.SelectedIndices[0];
                    if (selectedIndex > 0)
                    {
                        // ListViewアイテムの移動
                        var item = listView.Items[selectedIndex];
                        listView.Items.RemoveAt(selectedIndex);
                        listView.Items.Insert(selectedIndex - 1, item);
                        listView.Items[selectedIndex - 1].Selected = true;
                        
                        // データの移動
                        var keys = _mURLs.Keys.ToList();
                        if (selectedIndex < keys.Count)
                        {
                            var currentKey = keys[selectedIndex];
                            var previousKey = keys[selectedIndex - 1];
                            var temp = _mURLs[currentKey];
                            _mURLs[currentKey] = _mURLs[previousKey];
                            _mURLs[previousKey] = temp;
                        }
                        
                        _isModified = true;
                        Logger.LogInfo("OptionsForm.MoveUpAutoURL_Click", "Auto URL上移動完了");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.MoveUpAutoURL_Click", "Auto URL上移動エラー", ex.Message);
                MessageBox.Show($"Auto URL上移動に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto URL下移動ボタンのクリックイベント
        /// </summary>
        private void MoveDownAutoURL_Click(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogInfo("OptionsForm.MoveDownAutoURL_Click", "Auto URL下移動開始");
                var autoUrlsTab = tabSettings.TabPages["tabAutoURLs"];
                var listView = autoUrlsTab?.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (listView?.SelectedItems.Count > 0)
                {
                    var selectedIndex = listView.SelectedIndices[0];
                    if (selectedIndex < listView.Items.Count - 1)
                    {
                        // ListViewアイテムの移動
                        var item = listView.Items[selectedIndex];
                        listView.Items.RemoveAt(selectedIndex);
                        listView.Items.Insert(selectedIndex + 1, item);
                        listView.Items[selectedIndex + 1].Selected = true;
                        
                        // データの移動
                        var keys = _mURLs.Keys.ToList();
                        if (selectedIndex < keys.Count - 1)
                        {
                            var currentKey = keys[selectedIndex];
                            var nextKey = keys[selectedIndex + 1];
                            var temp = _mURLs[currentKey];
                            _mURLs[currentKey] = _mURLs[nextKey];
                            _mURLs[nextKey] = temp;
                        }
                        
                        _isModified = true;
                        Logger.LogInfo("OptionsForm.MoveDownAutoURL_Click", "Auto URL下移動完了");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.MoveDownAutoURL_Click", "Auto URL下移動エラー", ex.Message);
                MessageBox.Show($"Auto URL下移動に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// プロトコルパネルのボタンイベントハンドラー設定
        /// </summary>
        private void SetupProtocolsPanelButtons()
        {
            try
            {
                var protocolsTab = tabSettings.TabPages["tabProtocols"];
                if (protocolsTab != null)
                {
                    var addButton = protocolsTab.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
                    var editButton = protocolsTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var deleteButton = protocolsTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;
                    var listView = protocolsTab.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;

                    if (addButton != null) 
                    {
                        addButton.Click += _protocolHandlers.AddProtocol_Click;
                        Logger.LogInfo("OptionsForm.SetupProtocolsPanelButtons", "Addボタンのイベントを設定しました");
                    }
                    if (editButton != null) 
                    {
                        editButton.Click += _protocolHandlers.EditProtocol_Click;
                        Logger.LogInfo("OptionsForm.SetupProtocolsPanelButtons", "Editボタンのイベントを設定しました");
                    }
                    if (deleteButton != null) 
                    {
                        deleteButton.Click += _protocolHandlers.DeleteProtocol_Click;
                        Logger.LogInfo("OptionsForm.SetupProtocolsPanelButtons", "Deleteボタンのイベントを設定しました");
                    }
                    
                    // ListViewの選択変更イベントを設定
                    if (listView != null)
                    {
                        listView.SelectedIndexChanged += (sender, e) =>
                        {
                            var hasSelection = listView.SelectedItems.Count > 0;
                            if (editButton != null) editButton.Enabled = hasSelection;
                            if (deleteButton != null) deleteButton.Enabled = hasSelection;
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupProtocolsPanelButtons", "プロトコルパネルボタン設定エラー", ex.Message);
            }
        }



        /// <summary>
        /// Displayパネルのボタンイベントハンドラー設定
        /// </summary>
        private void SetupDisplayPanelButtons()
        {
            try
            {
                var displayTab = tabSettings.TabPages["tabDisplay"];
                if (displayTab != null)
                {
                    Logger.LogInfo("OptionsForm.SetupDisplayPanelButtons", "Displayタブが見つかりました");
                    
                    
                }
                else
                {
                    Logger.LogError("OptionsForm.SetupDisplayPanelButtons", "Displayタブが見つかりませんでした");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupDisplayPanelButtons", "Displayパネルボタン設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// レイアウト調整メソッド（初期化時とリサイズ時に使用）
        /// </summary>
        private void AdjustLayout()
        {
            try
            {
                Logger.LogTrace("OptionsForm.AdjustLayout", "レイアウト調整開始", ClientSize.Width, ClientSize.Height);

                if (treeSettings != null)
                {
                    treeSettings.Size = new Size(200, ClientSize.Height - 80);
                    // 左側のツリーと右側のグリッドの上部位置を合わせる
                    treeSettings.Location = new Point(treeSettings.Location.X, 12);
                }

                if (tabSettings != null)
                {
                    // 左側のツリーと右側のグリッドの上部位置を合わせる
                    tabSettings.Location = new Point(220, 12);
                    tabSettings.Size = new Size(ClientSize.Width - 240, ClientSize.Height - 100);
                }

                Logger.LogTrace("OptionsForm.AdjustLayout", "レイアウト調整完了", ClientSize.Width, ClientSize.Height);
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.AdjustLayout", "レイアウト調整エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// フォームサイズ変更時の処理
        /// </summary>
        private void OptionsForm_Resize(object? sender, EventArgs e)
        {
            try
            {
                AdjustLayout();
                Logger.LogTrace("OptionsForm.OptionsForm_Resize", "フォームサイズ変更完了", ClientSize.Width, ClientSize.Height);
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.OptionsForm_Resize", "サイズ変更エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// 現在表示されているパネルの設定をデフォルト値にリセットします
        /// </summary>
        private void ResetToDefaults_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "現在表示されているパネルの設定をデフォルト値にリセットしますか？\n\nこの操作は元に戻せません。",
                    "設定リセット確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    Logger.LogInfo("OptionsForm.ResetToDefaults", "設定リセット開始");
                    
                    // 現在選択されているタブを取得
                    var currentTab = tabSettings.SelectedTab;
                    if (currentTab == null)
                    {
                        MessageBox.Show("リセット対象のパネルが見つかりません。", "エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // 現在表示されているパネルの設定をリセット
                    ResetCurrentPanelToDefaults(currentTab);
                    
                    // 変更フラグを設定
                    _isModified = true;
                    
                    Logger.LogInfo("OptionsForm.ResetToDefaults", "設定リセット完了");
                    MessageBox.Show("現在表示されているパネルの設定をデフォルト値にリセットしました。\n\n変更を保存するには「保存」ボタンをクリックしてください。", "リセット完了", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.ResetToDefaults", "リセットエラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"設定のリセット中にエラーが発生しました。\n{ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 現在表示されているパネルの設定をデフォルト値にリセットします
        /// </summary>
        private void ResetCurrentPanelToDefaults(TabPage currentTab)
        {
            switch (currentTab.Name)
            {
                case "tabDisplay":
                    ResetDisplayPanelToDefaults(currentTab);
                    break;
                case "tabGrid":
                    ResetGridPanelToDefaults(currentTab);
                    break;
                case "tabPrivacy":
                    ResetPrivacyPanelToDefaults(currentTab);
                    break;
                case "tabStartup":
                    ResetStartupPanelToDefaults(currentTab);
                    break;
                case "tabOthers":
                    ResetOthersPanelToDefaults(currentTab);
                    break;
                case "tabBrowsers":
                    ResetBrowsersPanelToDefaults(currentTab);
                    break;
                case "tabAutoURLs":
                    ResetAutoURLsPanelToDefaults(currentTab);
                    break;
                case "tabProtocols":
                    ResetProtocolsPanelToDefaults(currentTab);
                    break;

                default:
                    Logger.LogInfo("OptionsForm.ResetCurrentPanelToDefaults", $"未対応のタブ: {currentTab.Name}");
                    break;
            }
        }

        /// <summary>
        /// 表示パネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetDisplayPanelToDefaults(TabPage tabPage)
        {
            // 設定をデフォルト値に更新
            _settings.EnableTransparency = (bool)_settings.Defaults[Settings.DefaultField.EnableTransparency];
            _settings.HideTitleBar = (bool)_settings.Defaults[Settings.DefaultField.HideTitleBar];
            _settings.Opacity = (double)_settings.Defaults[Settings.DefaultField.Opacity];
            _settings.RoundedCornersRadius = (int)_settings.Defaults[Settings.DefaultField.RoundedCornersRadius];
            _settings.EnableBackgroundGradient = (bool)_settings.Defaults[Settings.DefaultField.EnableBackgroundGradient];
            _settings.ShowFocus = (bool)_settings.Defaults[Settings.DefaultField.ShowFocus];
            _settings.ShowURL = (bool)_settings.Defaults[Settings.DefaultField.ShowURL];
            _settings.RevealShortURL = (bool)_settings.Defaults[Settings.DefaultField.RevealShortURL];
            _settings.TransparencyColor = (int)_settings.Defaults[Settings.DefaultField.TransparencyColor];
            _settings.FocusBoxColor = (int)_settings.Defaults[Settings.DefaultField.FocusBoxColor];
            _settings.UseAccessibleRendering = false; // デフォルト値
            _settings.ShowVisualFocus = false; // デフォルト値
            _settings.FocusBoxLineWidth = (int)_settings.Defaults[Settings.DefaultField.FocusBoxLineWidth];
            _settings.FocusBoxWidth = 2; // デフォルト値

            // UIに反映
            var chkEnableTransparency = tabPage.Controls.Find("chkEnableTransparency", true).FirstOrDefault() as CheckBox;
            if (chkEnableTransparency != null) chkEnableTransparency.Checked = _settings.EnableTransparency;

            var chkHideTitleBar = tabPage.Controls.Find("chkHideTitleBar", true).FirstOrDefault() as CheckBox;
            if (chkHideTitleBar != null) chkHideTitleBar.Checked = _settings.HideTitleBar;

            var nudOpacity = tabPage.Controls.Find("nudOpacity", true).FirstOrDefault() as NumericUpDown;
            if (nudOpacity != null) nudOpacity.Value = (decimal)_settings.Opacity;

            var nudRoundedCorners = tabPage.Controls.Find("nudRoundedCorners", true).FirstOrDefault() as NumericUpDown;
            if (nudRoundedCorners != null) nudRoundedCorners.Value = _settings.RoundedCornersRadius;

            var chkEnableBackgroundGradient = tabPage.Controls.Find("chkEnableBackgroundGradient", true).FirstOrDefault() as CheckBox;
            if (chkEnableBackgroundGradient != null) chkEnableBackgroundGradient.Checked = _settings.EnableBackgroundGradient;

            var chkShowFocus = tabPage.Controls.Find("chkShowFocus", true).FirstOrDefault() as CheckBox;
            if (chkShowFocus != null) chkShowFocus.Checked = _settings.ShowFocus;

            var chkShowURLs = tabPage.Controls.Find("chkShowURLs", true).FirstOrDefault() as CheckBox;
            if (chkShowURLs != null) chkShowURLs.Checked = _settings.ShowURL;

            var chkRevealShortURLs = tabPage.Controls.Find("chkRevealShortURLs", true).FirstOrDefault() as CheckBox;
            if (chkRevealShortURLs != null) chkRevealShortURLs.Checked = _settings.RevealShortURL;

            // 色設定のリセット
            var pbTransparencyColor = tabPage.Controls.Find("pbTransparencyColor", true).FirstOrDefault() as PictureBox;
            if (pbTransparencyColor != null) pbTransparencyColor.BackColor = Color.FromArgb(_settings.TransparencyColor);

            var pbFocusBoxColor = tabPage.Controls.Find("pbFocusBoxColor", true).FirstOrDefault() as PictureBox;
            if (pbFocusBoxColor != null) pbFocusBoxColor.BackColor = Color.FromArgb(_settings.FocusBoxColor);

            var chkUseAccessibleRendering = tabPage.Controls.Find("chkUseAccessibleRendering", true).FirstOrDefault() as CheckBox;
            if (chkUseAccessibleRendering != null) chkUseAccessibleRendering.Checked = _settings.UseAccessibleRendering;

            var chkShowVisualFocus = tabPage.Controls.Find("chkShowVisualFocus", true).FirstOrDefault() as CheckBox;
            if (chkShowVisualFocus != null) chkShowVisualFocus.Checked = _settings.ShowVisualFocus;

            var nudFocusBoxLineWidth = tabPage.Controls.Find("nudFocusBoxLineWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudFocusBoxLineWidth != null) nudFocusBoxLineWidth.Value = _settings.FocusBoxLineWidth;

            var nudFocusBoxWidth = tabPage.Controls.Find("nudFocusBoxWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudFocusBoxWidth != null) nudFocusBoxWidth.Value = _settings.FocusBoxWidth;
        }

        /// <summary>
        /// グリッドパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetGridPanelToDefaults(TabPage tabPage)
        {
            // 設定をデフォルト値に更新
            _settings.IconWidth = (int)_settings.Defaults[Settings.DefaultField.IconWidth];
            _settings.IconHeight = (int)_settings.Defaults[Settings.DefaultField.IconHeight];
            _settings.IconGapWidth = (int)_settings.Defaults[Settings.DefaultField.IconGapWidth];
            _settings.IconGapHeight = (int)_settings.Defaults[Settings.DefaultField.IconGapHeight];
            _settings.IconScale = (double)_settings.Defaults[Settings.DefaultField.IconScale];
            _settings.ShowGrid = false; // デフォルト値
            _settings.GridColor = Color.Gray.ToArgb(); // デフォルト値
            _settings.GridLineWidth = 1; // デフォルト値
            _settings.GridWidth = 5; // デフォルト値（Browser Chooser 2互換）
            _settings.GridHeight = 1; // デフォルト値（Browser Chooser 2互換）

            // UIに反映
            var nudIconSizeWidth = tabPage.Controls.Find("nudIconSizeWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudIconSizeWidth != null) nudIconSizeWidth.Value = _settings.IconWidth;

            var nudIconSizeHeight = tabPage.Controls.Find("nudIconSizeHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudIconSizeHeight != null) nudIconSizeHeight.Value = _settings.IconHeight;

            var nudIconGapWidth = tabPage.Controls.Find("nudIconGapWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudIconGapWidth != null) nudIconGapWidth.Value = _settings.IconGapWidth;

            var nudIconGapHeight = tabPage.Controls.Find("nudIconGapHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudIconGapHeight != null) nudIconGapHeight.Value = _settings.IconGapHeight;

            var nudIconScale = tabPage.Controls.Find("nudIconScale", true).FirstOrDefault() as NumericUpDown;
            if (nudIconScale != null) nudIconScale.Value = (decimal)_settings.IconScale;

            var chkShowGrid = tabPage.Controls.Find("chkShowGrid", true).FirstOrDefault() as CheckBox;
            if (chkShowGrid != null) chkShowGrid.Checked = _settings.ShowGrid;

            var pbGridColor = tabPage.Controls.Find("pbGridColor", true).FirstOrDefault() as Panel;
            if (pbGridColor != null) pbGridColor.BackColor = Color.FromArgb(_settings.GridColor);

            var nudGridLineWidth = tabPage.Controls.Find("nudGridLineWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudGridLineWidth != null) nudGridLineWidth.Value = _settings.GridLineWidth;

            // Grid SizeのWidth、Height（Browser Chooser 2互換）
            var nudGridWidth = tabPage.Controls.Find("nudGridWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudGridWidth != null) nudGridWidth.Value = _settings.GridWidth;

            var nudGridHeight = tabPage.Controls.Find("nudGridHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudGridHeight != null) nudGridHeight.Value = _settings.GridHeight;
        }

        /// <summary>
        /// プライバシーパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetPrivacyPanelToDefaults(TabPage tabPage)
        {
            // 設定をデフォルト値に更新
            _settings.EnableLogging = (bool)_settings.Defaults[Settings.DefaultField.EnableLogging];
            _settings.LogLevel = (int)_settings.Defaults[Settings.DefaultField.LogLevel];
            _settings.HistoryDays = 30; // デフォルト値
            _settings.PrivacyMode = false; // デフォルト値
            _settings.AllowDataCollection = false; // デフォルト値

            // UIに反映
            var chkEnableLogging = tabPage.Controls.Find("chkEnableLogging", true).FirstOrDefault() as CheckBox;
            if (chkEnableLogging != null) chkEnableLogging.Checked = _settings.EnableLogging;

            var cmbLogLevel = tabPage.Controls.Find("cmbLogLevel", true).FirstOrDefault() as ComboBox;
            if (cmbLogLevel != null) cmbLogLevel.SelectedIndex = Math.Min(_settings.LogLevel, cmbLogLevel.Items.Count - 1);

            var nudHistoryDays = tabPage.Controls.Find("nudHistoryDays", true).FirstOrDefault() as NumericUpDown;
            if (nudHistoryDays != null) nudHistoryDays.Value = _settings.HistoryDays;

            var chkPrivacyMode = tabPage.Controls.Find("chkPrivacyMode", true).FirstOrDefault() as CheckBox;
            if (chkPrivacyMode != null) chkPrivacyMode.Checked = _settings.PrivacyMode;

            var chkAllowDataCollection = tabPage.Controls.Find("chkAllowDataCollection", true).FirstOrDefault() as CheckBox;
            if (chkAllowDataCollection != null) chkAllowDataCollection.Checked = _settings.AllowDataCollection;
        }

        /// <summary>
        /// スタートアップパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetStartupPanelToDefaults(TabPage tabPage)
        {
            // 設定をデフォルト値に更新
            _settings.StartMinimized = false; // デフォルト値はfalse
            _settings.StartInTray = false; // デフォルト値
            _settings.StartupDelay = 0; // デフォルト値
            _settings.StartupMessage = "BrowserChooser3 Started"; // デフォルト値

            // UIに反映
            var chkStartMinimized = tabPage.Controls.Find("chkStartMinimized", true).FirstOrDefault() as CheckBox;
            if (chkStartMinimized != null) chkStartMinimized.Checked = _settings.StartMinimized;

            var chkStartInTray = tabPage.Controls.Find("chkStartInTray", true).FirstOrDefault() as CheckBox;
            if (chkStartInTray != null) chkStartInTray.Checked = _settings.StartInTray;

            var nudStartupDelay = tabPage.Controls.Find("nudStartupDelay", true).FirstOrDefault() as NumericUpDown;
            if (nudStartupDelay != null) nudStartupDelay.Value = _settings.StartupDelay;

            var txtStartupMessage = tabPage.Controls.Find("txtStartupMessage", true).FirstOrDefault() as TextBox;
            if (txtStartupMessage != null) txtStartupMessage.Text = _settings.StartupMessage;
        }

        /// <summary>
        /// その他パネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetOthersPanelToDefaults(TabPage tabPage)
        {
            // 設定をデフォルト値に更新
            _settings.Separator = (string)_settings.Defaults[Settings.DefaultField.Separator];
            _settings.DefaultDelay = (int)_settings.Defaults[Settings.DefaultField.DefaultDelay];
            _settings.AllowStayOpen = (bool)_settings.Defaults[Settings.DefaultField.AllowStayOpen];
            _settings.ExtractDLLs = (bool)_settings.Defaults[Settings.DefaultField.ExtractDLLs];
            _settings.UserAgent = (string)_settings.Defaults[Settings.DefaultField.UserAgent];
            _settings.DownloadDetectionFile = (bool)_settings.Defaults[Settings.DefaultField.DownloadDetectionFile];
            _settings.BackgroundColor = (int)_settings.Defaults[Settings.DefaultField.BackgroundColor];
            _settings.StartingPosition = (int)_settings.Defaults[Settings.DefaultField.StartingPosition];
            _settings.OffsetX = (int)_settings.Defaults[Settings.DefaultField.OffsetX];
            _settings.OffsetY = (int)_settings.Defaults[Settings.DefaultField.OffsetY];
            _settings.OptionsShortcut = (char)_settings.Defaults[Settings.DefaultField.OptionsShortcut];
            _settings.DefaultMessage = (string)_settings.Defaults[Settings.DefaultField.DefaultMessage];
            _settings.PortableMode = true; // デフォルト値

            // UIに反映
            var nudDefaultDelay = tabPage.Controls.Find("nudDefaultDelay", true).FirstOrDefault() as NumericUpDown;
            if (nudDefaultDelay != null) nudDefaultDelay.Value = _settings.DefaultDelay;

            var txtSeparator = tabPage.Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
            if (txtSeparator != null) txtSeparator.Text = _settings.Separator;

            var chkAllowStayOpen = tabPage.Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
            if (chkAllowStayOpen != null) chkAllowStayOpen.Checked = _settings.AllowStayOpen;

            var chkExtractDLLs = tabPage.Controls.Find("chkExtractDLLs", true).FirstOrDefault() as CheckBox;
            if (chkExtractDLLs != null) chkExtractDLLs.Checked = _settings.ExtractDLLs;

            var txtUserAgent = tabPage.Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
            if (txtUserAgent != null) txtUserAgent.Text = _settings.UserAgent;

            var chkDownloadDetectionFile = tabPage.Controls.Find("chkDownloadDetectionFile", true).FirstOrDefault() as CheckBox;
            if (chkDownloadDetectionFile != null) chkDownloadDetectionFile.Checked = _settings.DownloadDetectionFile;

            var chkPortableMode = tabPage.Controls.Find("chkPortableMode", true).FirstOrDefault() as CheckBox;
            if (chkPortableMode != null) chkPortableMode.Checked = _settings.PortableMode;

            var txtOptionsShortcut = tabPage.Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
            if (txtOptionsShortcut != null) txtOptionsShortcut.Text = _settings.OptionsShortcut.ToString();

            var txtDefaultMessage = tabPage.Controls.Find("txtDefaultMessage", true).FirstOrDefault() as TextBox;
            if (txtDefaultMessage != null) txtDefaultMessage.Text = _settings.DefaultMessage;
        }

        /// <summary>
        /// ブラウザパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetBrowsersPanelToDefaults(TabPage tabPage)
        {
            // ブラウザリストをクリア
            _settings.Browsers.Clear();
            _mBrowser.Clear();

            // UIに反映
            var listView = tabPage.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
            if (listView != null)
            {
                listView.Items.Clear();
            }
        }

        /// <summary>
        /// Auto URLsパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetAutoURLsPanelToDefaults(TabPage tabPage)
        {
            // URLリストをクリア
            _settings.URLs.Clear();
            _mURLs.Clear();

            // UIに反映
            var listView = tabPage.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
            if (listView != null)
            {
                listView.Items.Clear();
            }
        }

        /// <summary>
        /// プロトコルパネルの設定をデフォルト値にリセットし、UIに反映します
        /// </summary>
        private void ResetProtocolsPanelToDefaults(TabPage tabPage)
        {
            // プロトコルリストをクリア
            _settings.Protocols.Clear();
            _mProtocols.Clear();

            // UIに反映
            var listView = tabPage.Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
            if (listView != null)
            {
                listView.Items.Clear();
            }
        }





        /// <summary>
        /// TreeViewの選択変更イベント
        /// </summary>
        private void TreeSettings_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is string tabName)
            {
                // 対応するタブを選択
                var tabPage = tabSettings.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Name == tabName);
                if (tabPage != null)
                {
                    tabSettings.SelectedTab = tabPage;
                }
            }
        }

        /// <summary>
        /// TabControlの選択変更イベント
        /// </summary>
        private void TabSettings_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // カテゴリタブが選択された場合、categoryPanelを表示
            if (tabSettings.SelectedTab?.Name == "tabCategories")
            {
                var categoryPanel = Controls.Find("categoryPanel", true).FirstOrDefault() as Panel;
                if (categoryPanel != null)
                {
                    categoryPanel.Visible = true;
                    categoryPanel.BringToFront();
                }
            }
            else
            {
                // 他のタブが選択された場合、categoryPanelを非表示
                var categoryPanel = Controls.Find("categoryPanel", true).FirstOrDefault() as Panel;
                if (categoryPanel != null)
                {
                    categoryPanel.Visible = false;
                }
            }
        }

        /// <summary>
        /// 設定値の読み込み（Browser Chooser 2互換）
        /// </summary>
        private void LoadSettings()
        {
            Logger.LogInfo("OptionsForm.LoadSettings", "Start");

            try
            {
                // ブラウザ設定の読み込み（プロトコルより先に読み込む）
                _mBrowser.Clear();
                var defaultBrowserGuid = _settings.DefaultBrowserGuid;
                var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                if (listView != null) listView.Items.Clear();

                if (_imBrowserIcons != null) _imBrowserIcons.Images.Clear();

                foreach (var browser in _settings.Browsers)
                {
                    var clonedBrowser = browser.Clone();
                    _mBrowser.Add(_mBrowser.Count, clonedBrowser);

                    // デフォルトブラウザの判定
                    bool isDefault = (defaultBrowserGuid == browser.Guid);

                    // ListViewにアイテムを追加
                    if (listView != null)
                    {
                        var item = listView.Items.Add(clonedBrowser.Name);
                        item.Tag = _mBrowser.Count - 1;
                        item.SubItems.Add(""); // Default column
                        item.SubItems.Add(clonedBrowser.PosY.ToString());
                        item.SubItems.Add(clonedBrowser.PosX.ToString());
                        item.SubItems.Add(clonedBrowser.Hotkey.ToString());
                        item.SubItems.Add(_browserHandlers.GetBrowserProtocolsAndFileTypes(clonedBrowser));
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
                }
                _mLastBrowserID = _mBrowser.Count - 1;

                // プロトコル設定の読み込み
                _mProtocols.Clear();
                var protocolListView = Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                if (protocolListView != null) protocolListView.Items.Clear();

                foreach (var protocol in _settings.Protocols)
                {
                    _mProtocols.Add(_mProtocols.Count, protocol.Clone());
                }
                _mLastProtocolID = _mProtocols.Count - 1;

                // プロトコルListViewにアイテムを追加
                if (protocolListView != null)
                {
                    foreach (var protocol in _mProtocols)
                    {
                        var item = protocolListView.Items.Add(protocol.Value.Name);
                        item.Tag = protocol.Key;
                        item.SubItems.Add(_mBrowser.Values.FirstOrDefault(b => b.Guid == protocol.Value.BrowserGuid)?.Name ?? "");
                        item.SubItems.Add("Default");
                    }
                }

                // URL設定の読み込み
                _mURLs.Clear();
                var urlListView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (urlListView != null) urlListView.Items.Clear();

                foreach (var url in _settings.URLs)
                {
                    _mURLs.Add(_mURLs.Count, url.Clone());

                    // ListViewにアイテムを追加
                    if (urlListView != null)
                    {
                        var item = urlListView.Items.Add(url.URLValue);
                        item.Tag = _mURLs.Count - 1;

                        // ブラウザ名
                        if (url.BrowserGuid != Guid.Empty)
                        {
                            var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == url.BrowserGuid);
                            item.SubItems.Add(browser?.Name ?? "Unknown");
                        }
                        else
                        {
                            item.SubItems.Add("Default");
                        }

                        // 遅延時間
                        if (url.DelayTime < 0)
                        {
                            item.SubItems.Add("Default");
                        }
                        else
                        {
                            item.SubItems.Add(url.DelayTime.ToString());
                        }
                    }
                }
                _mLastURLID = _mURLs.Count - 1;

                // コントロールに設定値を設定
                LoadSettingsToControls();

                Logger.LogInfo("OptionsForm.LoadSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.LoadSettings", "設定読み込みエラー", ex.Message, ex.StackTrace ?? "");
                if (!IsTestEnvironment())
                {
                    MessageBox.Show($"設定の読み込みに失敗しました: {ex.Message}", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 変更フラグを設定するメソッド
        /// </summary>
        /// <param name="modified">変更フラグ</param>
        private void SetModified(bool modified)
        {
            _isModified = modified;
        }

        /// <summary>
        /// Auto URLsを再構築するメソッド
        /// </summary>
        private void RebuildAutoURLs()
        {
            // Auto URLsの再構築処理
            // 必要に応じて実装
        }

        /// <summary>
        /// 設定値をコントロールに設定（Browser Chooser 2互換）
        /// </summary>
        private void LoadSettingsToControls()
        {
            try
            {
                // 基本設定
                var chkShowURLs = Controls.Find("chkShowURLs", true).FirstOrDefault() as CheckBox;
                if (chkShowURLs != null) chkShowURLs.Checked = _settings.ShowURL;

                var chkRevealShortURLs = Controls.Find("chkRevealShortURLs", true).FirstOrDefault() as CheckBox;
                if (chkRevealShortURLs != null) chkRevealShortURLs.Checked = _settings.RevealShortURL;

                var chkPortableMode = Controls.Find("chkPortableMode", true).FirstOrDefault() as CheckBox;
                if (chkPortableMode != null) chkPortableMode.Checked = _settings.PortableMode;

                var nudHeight = Controls.Find("nudHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudHeight != null) nudHeight.Value = _settings.Height;

                var nudWidth = Controls.Find("nudWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudWidth != null) nudWidth.Value = _settings.Width;

                // 背景色の表示用PictureBoxを設定値で初期化（保存時との不整合防止）
                var pbBackgroundColorLoad = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                if (pbBackgroundColorLoad != null)
                {
                    pbBackgroundColorLoad.BackColor = _settings.BackgroundColorValue;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"BackgroundColor loaded: {_settings.BackgroundColorValue} (to control: {pbBackgroundColorLoad.BackColor})");
                }

                var nudDelayBeforeAutoload = Controls.Find("nudDelayBeforeAutoload", true).FirstOrDefault() as NumericUpDown;
                if (nudDelayBeforeAutoload != null) nudDelayBeforeAutoload.Value = _settings.DefaultDelay;

                var txtSeparator = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
                if (txtSeparator != null) txtSeparator.Text = _settings.Separator;

                var chkAllowStayOpen = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
                if (chkAllowStayOpen != null) chkAllowStayOpen.Checked = _settings.AllowStayOpen;

                // 詳細設定
                var txtUserAgent = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
                if (txtUserAgent != null) txtUserAgent.Text = _settings.UserAgent;

                var chkDownloadDetectionFile = Controls.Find("chkDownloadDetectionFile", true).FirstOrDefault() as CheckBox;
                if (chkDownloadDetectionFile != null) chkDownloadDetectionFile.Checked = _settings.DownloadDetectionFile;

                var nudIconSizeWidth = Controls.Find("nudIconSizeWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeWidth != null) 
                {
                    nudIconSizeWidth.Value = _settings.IconWidth;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"IconWidth loaded: {_settings.IconWidth} (to control: {nudIconSizeWidth.Value})");
                }

                var nudIconSizeHeight = Controls.Find("nudIconSizeHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeHeight != null) 
                {
                    nudIconSizeHeight.Value = _settings.IconHeight;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"IconHeight loaded: {_settings.IconHeight} (to control: {nudIconSizeHeight.Value})");
                }

                var nudIconGapWidth = Controls.Find("nudIconGapWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapWidth != null) 
                {
                    nudIconGapWidth.Value = _settings.IconGapWidth;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"IconGapWidth loaded: {_settings.IconGapWidth} (to control: {nudIconGapWidth.Value})");
                }

                var nudIconGapHeight = Controls.Find("nudIconGapHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapHeight != null) 
                {
                    nudIconGapHeight.Value = _settings.IconGapHeight;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"IconGapHeight loaded: {_settings.IconGapHeight} (to control: {nudIconGapHeight.Value})");
                }

                // 背景色はPictureBoxから読み書きする（Panel扱いは廃止）
                var pbBackgroundColor = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                if (pbBackgroundColor != null) pbBackgroundColor.BackColor = _settings.BackgroundColorValue;

                var nudIconScale = Controls.Find("nudIconScale", true).FirstOrDefault() as NumericUpDown;
                if (nudIconScale != null) 
                {
                    nudIconScale.Value = (decimal)_settings.IconScale;
                    Logger.LogInfo("OptionsForm.LoadSettingsToControls", $"IconScale loaded: {_settings.IconScale} (to control: {nudIconScale.Value})");
                }



                var chkExtractDLLs = Controls.Find("chkExtractDLLs", true).FirstOrDefault() as CheckBox;
                if (chkExtractDLLs != null) chkExtractDLLs.Checked = _settings.ExtractDLLs;



                // グリッド設定
                var nudGridWidth = Controls.Find("nudGridWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudGridWidth != null) nudGridWidth.Value = _settings.GridWidth;

                var nudGridHeight = Controls.Find("nudGridHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudGridHeight != null) nudGridHeight.Value = _settings.GridHeight;

                var chkShowGrid = Controls.Find("chkShowGrid", true).FirstOrDefault() as CheckBox;
                if (chkShowGrid != null) chkShowGrid.Checked = _settings.ShowGrid;

                var pbGridColor = Controls.Find("pbGridColor", true).FirstOrDefault() as Panel;
                if (pbGridColor != null) pbGridColor.BackColor = Color.FromArgb(_settings.GridColor);

                var nudGridLineWidth = Controls.Find("nudGridLineWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudGridLineWidth != null) nudGridLineWidth.Value = _settings.GridLineWidth;

                // プライバシー設定

                var cmbLogLevel = Controls.Find("cmbLogLevel", true).FirstOrDefault() as ComboBox;
                if (cmbLogLevel != null) cmbLogLevel.SelectedIndex = Math.Min(_settings.LogLevel, cmbLogLevel.Items.Count - 1);

                var chkKeepHistory = Controls.Find("chkKeepHistory", true).FirstOrDefault() as CheckBox;
                if (chkKeepHistory != null) chkKeepHistory.Checked = _settings.KeepHistory;

                var nudHistoryDays = Controls.Find("nudHistoryDays", true).FirstOrDefault() as NumericUpDown;
                if (nudHistoryDays != null) nudHistoryDays.Value = _settings.HistoryDays;

                var chkPrivacyMode = Controls.Find("chkPrivacyMode", true).FirstOrDefault() as CheckBox;
                if (chkPrivacyMode != null) chkPrivacyMode.Checked = _settings.PrivacyMode;

                var chkAllowDataCollection = Controls.Find("chkAllowDataCollection", true).FirstOrDefault() as CheckBox;
                if (chkAllowDataCollection != null) chkAllowDataCollection.Checked = _settings.AllowDataCollection;

                // スタートアップ設定
                var chkStartMinimized = Controls.Find("chkStartMinimized", true).FirstOrDefault() as CheckBox;
                if (chkStartMinimized != null) chkStartMinimized.Checked = _settings.StartMinimized;

                var chkStartInTray = Controls.Find("chkStartInTray", true).FirstOrDefault() as CheckBox;
                if (chkStartInTray != null) chkStartInTray.Checked = _settings.StartInTray;

                var nudStartupDelay = Controls.Find("nudStartupDelay", true).FirstOrDefault() as NumericUpDown;
                if (nudStartupDelay != null) nudStartupDelay.Value = _settings.StartupDelay;

                var txtStartupMessage = Controls.Find("txtStartupMessage", true).FirstOrDefault() as TextBox;
                if (txtStartupMessage != null) txtStartupMessage.Text = _settings.StartupMessage;

                // 位置設定
                var nudXOffset = Controls.Find("nudXOffset", true).FirstOrDefault() as NumericUpDown;
                if (nudXOffset != null) nudXOffset.Value = _settings.OffsetX;

                var nudYOffset = Controls.Find("nudYOffset", true).FirstOrDefault() as NumericUpDown;
                if (nudYOffset != null) nudYOffset.Value = _settings.OffsetY;

                // 開始位置設定
                var cmbStartingPosition = Controls.Find("cmbStartingPosition", true).FirstOrDefault() as ComboBox;
                if (cmbStartingPosition != null)
                {
                    // 開始位置の選択
                    for (int i = 0; i < cmbStartingPosition.Items.Count; i++)
                    {
                        if (cmbStartingPosition.Items[i] is DisplayDictionary position && position.Index == _settings.StartingPosition)
                        {
                            cmbStartingPosition.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // アクセシビリティ設定
                var chkUseAccessibleRendering = Controls.Find("chkUseAccessibleRendering", true).FirstOrDefault() as CheckBox;
                if (chkUseAccessibleRendering != null) chkUseAccessibleRendering.Checked = _settings.UseAccessibleRendering;



                // フォーカス設定
                var chkShowFocus = Controls.Find("chkShowFocus", true).FirstOrDefault() as CheckBox;
                if (chkShowFocus != null) chkShowFocus.Checked = _settings.ShowFocus;

                var chkShowVisualFocus = Controls.Find("chkShowVisualFocus", true).FirstOrDefault() as CheckBox;
                if (chkShowVisualFocus != null) chkShowVisualFocus.Checked = _settings.ShowVisualFocus;

                var nudFocusBoxLineWidth = Controls.Find("nudFocusBoxLineWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudFocusBoxLineWidth != null) nudFocusBoxLineWidth.Value = _settings.FocusBoxLineWidth;

                var nudFocusBoxWidth = Controls.Find("nudFocusBoxWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudFocusBoxWidth != null) nudFocusBoxWidth.Value = _settings.FocusBoxWidth;

                var pbFocusBoxColor = Controls.Find("pbFocusBoxColor", true).FirstOrDefault() as PictureBox;
                if (pbFocusBoxColor != null) pbFocusBoxColor.BackColor = Color.FromArgb(_settings.FocusBoxColor);

                // 透明化設定
                var chkEnableTransparency = Controls.Find("chkEnableTransparency", true).FirstOrDefault() as CheckBox;
                if (chkEnableTransparency != null) chkEnableTransparency.Checked = _settings.EnableTransparency;

                var pbTransparencyColor = Controls.Find("pbTransparencyColor", true).FirstOrDefault() as PictureBox;
                if (pbTransparencyColor != null) pbTransparencyColor.BackColor = Color.FromArgb(_settings.TransparencyColor);

                var nudOpacity = Controls.Find("nudOpacity", true).FirstOrDefault() as NumericUpDown;
                if (nudOpacity != null) nudOpacity.Value = (decimal)_settings.Opacity;

                var chkHideTitleBar = Controls.Find("chkHideTitleBar", true).FirstOrDefault() as CheckBox;
                if (chkHideTitleBar != null) chkHideTitleBar.Checked = _settings.HideTitleBar;

                var nudRoundedCorners = Controls.Find("nudRoundedCorners", true).FirstOrDefault() as NumericUpDown;
                if (nudRoundedCorners != null) nudRoundedCorners.Value = _settings.RoundedCornersRadius;

                var chkEnableBackgroundGradient = Controls.Find("chkEnableBackgroundGradient", true).FirstOrDefault() as CheckBox;
                if (chkEnableBackgroundGradient != null) chkEnableBackgroundGradient.Checked = _settings.EnableBackgroundGradient;

                // ショートカット設定
                var txtOptionsShortcut = Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
                if (txtOptionsShortcut != null && _settings.OptionsShortcut != char.MinValue)
                {
                    txtOptionsShortcut.Text = _settings.OptionsShortcut.ToString();
                }

                var txtDefaultMessage = Controls.Find("txtDefaultMessage", true).FirstOrDefault() as TextBox;
                if (txtDefaultMessage != null) txtDefaultMessage.Text = _settings.DefaultMessage;

                // その他の設定項目の読み込み
                var chkDownloadDetectionFileLoad = Controls.Find("chkDownloadDetectionFile", true).FirstOrDefault() as CheckBox;
                if (chkDownloadDetectionFileLoad != null) chkDownloadDetectionFileLoad.Checked = _settings.DownloadDetectionFile;



                var chkExtractDLLsLoad = Controls.Find("chkExtractDLLs", true).FirstOrDefault() as CheckBox;
                if (chkExtractDLLsLoad != null) chkExtractDLLsLoad.Checked = _settings.ExtractDLLs;



                var txtUserAgentLoad = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
                if (txtUserAgentLoad != null) txtUserAgentLoad.Text = _settings.UserAgent;

                var nudDefaultDelayLoad = Controls.Find("nudDefaultDelay", true).FirstOrDefault() as NumericUpDown;
                if (nudDefaultDelayLoad != null) nudDefaultDelayLoad.Value = _settings.DefaultDelay;

                var txtSeparatorLoad = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
                if (txtSeparatorLoad != null) txtSeparatorLoad.Text = _settings.Separator;

                var chkAllowStayOpenLoad = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
                if (chkAllowStayOpenLoad != null) chkAllowStayOpenLoad.Checked = _settings.AllowStayOpen;
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.LoadSettingsToControls", "コントロール設定エラー", ex.Message, ex.StackTrace ?? "");
                if (!IsTestEnvironment())
                {
                    MessageBox.Show($"コントロールの設定に失敗しました: {ex.Message}", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 設定値の保存（Browser Chooser 2互換）
        /// </summary>
        private void SaveSettings()
        {
            Logger.LogInfo("OptionsForm.SaveSettings", "Start");

            try
            {
                // ブラウザ設定の保存
                _settings.Browsers = new List<Browser>();
                foreach (var browser in _mBrowser)
                {
                    _settings.Browsers.Add(browser.Value.Clone());
                }

                // デフォルトブラウザの設定
                var hiddenGuidLabel = lblHiddenBrowserGuid;
                if (hiddenGuidLabel?.Tag != null && !string.IsNullOrEmpty(hiddenGuidLabel.Tag.ToString()?.Trim()))
                {
                    try
                    {
                        var tagString = hiddenGuidLabel.Tag.ToString();
                        if (!string.IsNullOrEmpty(tagString))
                        {
                            var newGuid = new Guid(tagString);
                            _settings.DefaultBrowserGuid = newGuid;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There is a problem with the default browser (GUID error)", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // URL設定の保存
                _settings.URLs = new List<URL>();
                foreach (var url in _mURLs)
                {
                    _settings.URLs.Add(url.Value.Clone());
                }

                // プロトコル設定の保存
                _settings.Protocols = new List<Protocol>();
                foreach (var protocol in _mProtocols)
                {
                    _settings.Protocols.Add(protocol.Value.Clone());
                }

                // ファイルタイプ設定の保存（未実装のため削除）
                // _settings.FileTypes = new List<FileType>();
                // foreach (var fileType in _mFileTypes)
                // {
                //     _settings.FileTypes.Add(fileType.Value.Clone());
                // }

                // プロトコル・ファイルタイプが変更された場合の確認
                // if (_mFileTypesAreDirty || _mProtocolsAreDirty)
                // {
                //     var result = MessageBox.Show("You have changed the accepted Protocols or Filetypes.",
                //         "Protocols/Filetypes Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // }

                // 各コントロールから設定値を取得して保存
                // 基本設定
                var chkShowURLs = Controls.Find("chkShowURLs", true).FirstOrDefault() as CheckBox;
                if (chkShowURLs != null) _settings.ShowURL = chkShowURLs.Checked;

                var chkRevealShortURLs = Controls.Find("chkRevealShortURLs", true).FirstOrDefault() as CheckBox;
                if (chkRevealShortURLs != null) _settings.RevealShortURL = chkRevealShortURLs.Checked;

                var chkPortableMode = Controls.Find("chkPortableMode", true).FirstOrDefault() as CheckBox;
                if (chkPortableMode != null) _settings.PortableMode = chkPortableMode.Checked;

                var nudHeight = Controls.Find("nudHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudHeight != null) _settings.Height = (int)nudHeight.Value;

                var nudWidth = Controls.Find("nudWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudWidth != null) _settings.Width = (int)nudWidth.Value;

                var nudDelayBeforeAutoload = Controls.Find("nudDelayBeforeAutoload", true).FirstOrDefault() as NumericUpDown;
                if (nudDelayBeforeAutoload != null) _settings.DefaultDelay = (int)nudDelayBeforeAutoload.Value;

                var txtSeparator = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
                if (txtSeparator != null) _settings.Separator = txtSeparator.Text;

                var chkAllowStayOpen = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
                if (chkAllowStayOpen != null) _settings.AllowStayOpen = chkAllowStayOpen.Checked;

                // 詳細設定
                var txtUserAgent = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
                if (txtUserAgent != null) _settings.UserAgent = txtUserAgent.Text;

                            var chkDownloadDetectionFile = Controls.Find("chkDownloadDetectionFile", true).FirstOrDefault() as CheckBox;
            if (chkDownloadDetectionFile != null) _settings.DownloadDetectionFile = chkDownloadDetectionFile.Checked;

                var nudIconSizeWidth = Controls.Find("nudIconSizeWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeWidth != null) 
                {
                    _settings.IconWidth = (int)nudIconSizeWidth.Value;
                    Logger.LogInfo("OptionsForm.SaveSettings", $"IconWidth saved: {_settings.IconWidth} (from control: {nudIconSizeWidth.Value})");
                }

                var nudIconSizeHeight = Controls.Find("nudIconSizeHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeHeight != null) 
                {
                    _settings.IconHeight = (int)nudIconSizeHeight.Value;
                    Logger.LogInfo("OptionsForm.SaveSettings", $"IconHeight saved: {_settings.IconHeight} (from control: {nudIconSizeHeight.Value})");
                }

                var nudIconGapWidth = Controls.Find("nudIconGapWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapWidth != null) 
                {
                    _settings.IconGapWidth = (int)nudIconGapWidth.Value;
                    Logger.LogInfo("OptionsForm.SaveSettings", $"IconGapWidth saved: {_settings.IconGapWidth} (from control: {nudIconGapWidth.Value})");
                }

                var nudIconGapHeight = Controls.Find("nudIconGapHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapHeight != null) 
                {
                    _settings.IconGapHeight = (int)nudIconGapHeight.Value;
                    Logger.LogInfo("OptionsForm.SaveSettings", $"IconGapHeight saved: {_settings.IconGapHeight} (from control: {nudIconGapHeight.Value})");
                }

                // Panel経由の保存は廃止。DisplayタブのPictureBoxから保存する

                var nudIconScale = Controls.Find("nudIconScale", true).FirstOrDefault() as NumericUpDown;
                if (nudIconScale != null) 
                {
                    _settings.IconScale = (double)nudIconScale.Value;
                    Logger.LogInfo("OptionsForm.SaveSettings", $"IconScale saved: {_settings.IconScale} (from control: {nudIconScale.Value})");
                }



                            var chkExtractDLLs = Controls.Find("chkExtractDLLs", true).FirstOrDefault() as CheckBox;
            if (chkExtractDLLs != null) _settings.ExtractDLLs = chkExtractDLLs.Checked;

                // グリッド設定
                var nudGridWidth = Controls.Find("nudGridWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudGridWidth != null) _settings.GridWidth = (int)nudGridWidth.Value;

                var nudGridHeight = Controls.Find("nudGridHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudGridHeight != null) _settings.GridHeight = (int)nudGridHeight.Value;

                var chkShowGrid = Controls.Find("chkShowGrid", true).FirstOrDefault() as CheckBox;
                if (chkShowGrid != null) _settings.ShowGrid = chkShowGrid.Checked;

                var pbGridColor = Controls.Find("pbGridColor", true).FirstOrDefault() as Panel;
                if (pbGridColor != null) _settings.GridColor = pbGridColor.BackColor.ToArgb();

                var nudGridLineWidth = Controls.Find("nudGridLineWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudGridLineWidth != null) _settings.GridLineWidth = (int)nudGridLineWidth.Value;

                // プライバシー設定

                var cmbLogLevel = Controls.Find("cmbLogLevel", true).FirstOrDefault() as ComboBox;
                if (cmbLogLevel != null)
                {
                    // ComboBox.Index → Settings.LogLevel(int) へのマッピング
                    int MapIndexToLogLevel(int idx)
                    {
                        // ComboBox: Trace(0), Debug(1), Info(2), Warning(3), Error(4)
                        return idx switch
                        {
                            0 => 5, // Trace
                            1 => 4, // Debug
                            2 => 3, // Info
                            3 => 2, // Warning
                            4 => 1, // Error
                            _ => 3
                        };
                    }
                    _settings.LogLevel = MapIndexToLogLevel(cmbLogLevel.SelectedIndex);
                    Logger.InitializeLogLevel(_settings.LogLevel);
                }

                var chkKeepHistory = Controls.Find("chkKeepHistory", true).FirstOrDefault() as CheckBox;
                if (chkKeepHistory != null) _settings.KeepHistory = chkKeepHistory.Checked;

                var nudHistoryDays = Controls.Find("nudHistoryDays", true).FirstOrDefault() as NumericUpDown;
                if (nudHistoryDays != null) _settings.HistoryDays = (int)nudHistoryDays.Value;

                var chkPrivacyMode = Controls.Find("chkPrivacyMode", true).FirstOrDefault() as CheckBox;
                if (chkPrivacyMode != null) _settings.PrivacyMode = chkPrivacyMode.Checked;

                var chkAllowDataCollection = Controls.Find("chkAllowDataCollection", true).FirstOrDefault() as CheckBox;
                if (chkAllowDataCollection != null) _settings.AllowDataCollection = chkAllowDataCollection.Checked;

                // スタートアップ設定
                var chkStartMinimized = Controls.Find("chkStartMinimized", true).FirstOrDefault() as CheckBox;
                if (chkStartMinimized != null) _settings.StartMinimized = chkStartMinimized.Checked;

                var chkStartInTray = Controls.Find("chkStartInTray", true).FirstOrDefault() as CheckBox;
                if (chkStartInTray != null) _settings.StartInTray = chkStartInTray.Checked;



                var nudStartupDelay = Controls.Find("nudStartupDelay", true).FirstOrDefault() as NumericUpDown;
                if (nudStartupDelay != null) _settings.StartupDelay = (int)nudStartupDelay.Value;

                var txtStartupMessage = Controls.Find("txtStartupMessage", true).FirstOrDefault() as TextBox;
                if (txtStartupMessage != null) _settings.StartupMessage = txtStartupMessage.Text;

                // 位置設定
                var cmbStartingPosition = Controls.Find("cmbStartingPosition", true).FirstOrDefault() as ComboBox;
                if (cmbStartingPosition?.SelectedItem != null)
                {
                    var selectedPosition = cmbStartingPosition.SelectedItem as DisplayDictionary;
                    if (selectedPosition != null)
                    {
                        _settings.StartingPosition = selectedPosition.Index;
                    }
                }

                var nudXOffset = Controls.Find("nudXOffset", true).FirstOrDefault() as NumericUpDown;
                if (nudXOffset != null) _settings.OffsetX = (int)nudXOffset.Value;

                var nudYOffset = Controls.Find("nudYOffset", true).FirstOrDefault() as NumericUpDown;
                if (nudYOffset != null) _settings.OffsetY = (int)nudYOffset.Value;

                // アクセシビリティ設定
                var chkUseAccessibleRendering = Controls.Find("chkUseAccessibleRendering", true).FirstOrDefault() as CheckBox;
                if (chkUseAccessibleRendering != null) _settings.UseAccessibleRendering = chkUseAccessibleRendering.Checked;



                // フォーカス設定
                var chkShowFocus = Controls.Find("chkShowFocus", true).FirstOrDefault() as CheckBox;
                if (chkShowFocus != null) _settings.ShowFocus = chkShowFocus.Checked;

                var chkShowVisualFocus = Controls.Find("chkShowVisualFocus", true).FirstOrDefault() as CheckBox;
                if (chkShowVisualFocus != null) _settings.ShowVisualFocus = chkShowVisualFocus.Checked;

                var nudFocusBoxLineWidth = Controls.Find("nudFocusBoxLineWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudFocusBoxLineWidth != null) _settings.FocusBoxLineWidth = (int)nudFocusBoxLineWidth.Value;

                var nudFocusBoxWidth = Controls.Find("nudFocusBoxWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudFocusBoxWidth != null) _settings.FocusBoxWidth = (int)nudFocusBoxWidth.Value;

                var pbFocusBoxColor = Controls.Find("pbFocusBoxColor", true).FirstOrDefault() as PictureBox;
                if (pbFocusBoxColor != null) _settings.FocusBoxColor = pbFocusBoxColor.BackColor.ToArgb();

                // 背景色設定
                Logger.LogInfo("OptionsForm.SaveSettings", "背景色保存処理開始");
                var displayTab = tabSettings.TabPages["tabDisplay"];
                var pbBackgroundColorSave = displayTab?.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                Logger.LogInfo("OptionsForm.SaveSettings", $"pbBackgroundColor検索結果: {(pbBackgroundColorSave != null ? "見つかりました" : "見つかりませんでした")}");
                
                if (pbBackgroundColorSave != null) 
                {
                    Logger.LogInfo("OptionsForm.SaveSettings", $"pbBackgroundColor.BackColor: {pbBackgroundColorSave.BackColor}");
                    _settings.BackgroundColorValue = pbBackgroundColorSave.BackColor;
                    Logger.LogInfo("OptionsForm.SaveSettings", "背景色を保存しました", pbBackgroundColorSave.BackColor.ToString());
                    Logger.LogInfo("OptionsForm.SaveSettings", "PictureBoxのBackColor", pbBackgroundColorSave.BackColor.ToString());

                    // 透明化が無効の場合、保存直後にメイン画面へ即時反映
                    try
                    {
                        var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                        if (mainForm != null)
                        {
                            var transparencyEnabled = _settings.EnableTransparency;
                            Logger.LogInfo("OptionsForm.SaveSettings", $"即時反映: EnableTransparency={transparencyEnabled}");
                            if (!transparencyEnabled)
                            {
                                mainForm.BackColor = _settings.BackgroundColorValue;
                                Logger.LogInfo("OptionsForm.SaveSettings", "メイン画面の背景色を即時更新しました", _settings.BackgroundColorValue.ToString());
                            }
                            else
                            {
                                Logger.LogInfo("OptionsForm.SaveSettings", "透明化有効のため即時の背景色反映はスキップされます");
                            }
                        }
                        else
                        {
                            Logger.LogWarning("OptionsForm.SaveSettings", "MainFormが見つからないため即時反映をスキップ");
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.LogError("OptionsForm.SaveSettings", "メイン画面即時反映時エラー", ex2.Message);
                    }
                }
                else
                {
                    Logger.LogWarning("OptionsForm.SaveSettings", "pbBackgroundColorが見つかりませんでした");
                    
                    // デバッグ用：Displayタブ内のすべてのコントロールを検索
                    if (displayTab != null)
                    {
                        var allControls = displayTab.Controls.Find("pbBackgroundColor", true);
                        Logger.LogInfo("OptionsForm.SaveSettings", $"pbBackgroundColor検索結果数: {allControls.Length}");
                        foreach (var control in allControls)
                        {
                            Logger.LogInfo("OptionsForm.SaveSettings", $"見つかったコントロール: {control.Name}, 型: {control.GetType().Name}");
                        }
                    }
                }

                // 透明化設定
                var chkEnableTransparency = Controls.Find("chkEnableTransparency", true).FirstOrDefault() as CheckBox;
                if (chkEnableTransparency != null) _settings.EnableTransparency = chkEnableTransparency.Checked;

                var pbTransparencyColor = Controls.Find("pbTransparencyColor", true).FirstOrDefault() as PictureBox;
                if (pbTransparencyColor != null) _settings.TransparencyColor = pbTransparencyColor.BackColor.ToArgb();

                var nudOpacity = Controls.Find("nudOpacity", true).FirstOrDefault() as NumericUpDown;
                if (nudOpacity != null) _settings.Opacity = (double)nudOpacity.Value;

                var chkHideTitleBar = Controls.Find("chkHideTitleBar", true).FirstOrDefault() as CheckBox;
                if (chkHideTitleBar != null) _settings.HideTitleBar = chkHideTitleBar.Checked;

                var nudRoundedCorners = Controls.Find("nudRoundedCorners", true).FirstOrDefault() as NumericUpDown;
                if (nudRoundedCorners != null) _settings.RoundedCornersRadius = (int)nudRoundedCorners.Value;

                var chkEnableBackgroundGradient = Controls.Find("chkEnableBackgroundGradient", true).FirstOrDefault() as CheckBox;
                if (chkEnableBackgroundGradient != null) _settings.EnableBackgroundGradient = chkEnableBackgroundGradient.Checked;

                // ショートカット設定
                var txtOptionsShortcut = Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
                if (txtOptionsShortcut != null && !string.IsNullOrEmpty(txtOptionsShortcut.Text))
                {
                    _settings.OptionsShortcut = txtOptionsShortcut.Text[0];
                }
                else
                {
                    _settings.OptionsShortcut = char.MinValue;
                }

                var txtDefaultMessage = Controls.Find("txtDefaultMessage", true).FirstOrDefault() as TextBox;
                if (txtDefaultMessage != null) _settings.DefaultMessage = txtDefaultMessage.Text;

                // その他の設定項目の保存
                var chkDownloadDetectionFileSave = Controls.Find("chkDownloadDetectionFile", true).FirstOrDefault() as CheckBox;
                if (chkDownloadDetectionFileSave != null) _settings.DownloadDetectionFile = chkDownloadDetectionFileSave.Checked;



                var chkExtractDLLsSave = Controls.Find("chkExtractDLLs", true).FirstOrDefault() as CheckBox;
                if (chkExtractDLLsSave != null) _settings.ExtractDLLs = chkExtractDLLsSave.Checked;



                var txtUserAgentSave = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
                if (txtUserAgentSave != null) _settings.UserAgent = txtUserAgentSave.Text;

                var nudDefaultDelaySave = Controls.Find("nudDefaultDelay", true).FirstOrDefault() as NumericUpDown;
                if (nudDefaultDelaySave != null) _settings.DefaultDelay = (int)nudDefaultDelaySave.Value;

                var txtSeparatorSave = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
                if (txtSeparatorSave != null) _settings.Separator = txtSeparatorSave.Text;

                var chkAllowStayOpenSave = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
                if (chkAllowStayOpenSave != null) _settings.AllowStayOpen = chkAllowStayOpenSave.Checked;

                // Enable Logging設定
                var chkEnableLoggingSave = Controls.Find("chkEnableLogging", true).FirstOrDefault() as CheckBox;
                if (chkEnableLoggingSave != null) 
                {
                    _settings.EnableLogging = chkEnableLoggingSave.Checked;
                    Logger.LogInfo("OptionsForm.SaveSettings", "Enable Logging設定を保存しました", _settings.EnableLogging.ToString());
                }

                _settings.DoSave();
                _isModified = false;

                Logger.LogInfo("OptionsForm.SaveSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SaveSettings", "保存エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"設定の保存に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Auto URLsのドラッグ&amp;ドロップ機能を設定（Browser Chooser 2互換）
        /// 現在は未実装（Browser Chooser 2でも実装されていない）
        /// </summary>
        private void SetupAutoURLsDragDrop(ListView listView)
        {
            // Browser Chooser 2では実装されていないため、空の実装のままとする
        }



        /// <summary>
        /// ブラウザリストへのファイルドロップ機能を設定（Browser Chooser 2互換）
        /// </summary>
        /// <param name="listView">対象のListView</param>
        private void SetupBrowsersDragDrop(ListView listView)
        {
            // テスト環境ではDragDropを無効化
            if (IsTestEnvironment())
            {
                listView.AllowDrop = false;
                return;
            }

            listView.AllowDrop = true;

            listView.DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                {
                    e.Effect = DragDropEffects.Copy;
                    listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            };

            listView.DragLeave += (s, e) =>
            {
                listView.BackColor = Color.FromKnownColor(KnownColor.Window);
            };

            listView.DragDrop += (s, e) =>
            {
                if (e.Data?.GetData(DataFormats.FileDrop) is string[] files)
                {
                    foreach (var filePath in files)
                    {
                        if (Path.GetExtension(filePath.ToLower()) == ".exe")
                        {
                            // ブラウザ追加ダイアログを表示
                            try
                            {
                                var addEditForm = new AddEditBrowserForm();
                                var newBrowser = new Browser
                                {
                                    Guid = Guid.NewGuid(),
                                    Name = Path.GetFileNameWithoutExtension(filePath),
                                    Target = filePath,
                                    Arguments = "",
                                    PosX = 1,
                                    PosY = 1,
                                    Hotkey = '\0',
                                    Category = "Default"
                                };

                                if (addEditForm.AddBrowser(_mBrowser, _mProtocols, null!, false,
                                    new Point(_settings.GridWidth, _settings.GridHeight), newBrowser))
                                {
                                    var browser = addEditForm.GetData();
                                    _mBrowser.Add(_mLastBrowserID + 1, browser);

                                    // ListViewにアイテムを追加
                                    var item = listView.Items.Add(browser.Name);
                                    item.Tag = _mLastBrowserID + 1;
                                    item.SubItems.Add(browser.Target);
                                    item.SubItems.Add(browser.Arguments);
                                    item.SubItems.Add(browser.PosY.ToString());
                                    item.SubItems.Add(browser.PosX.ToString());
                                    item.SubItems.Add(browser.Hotkey.ToString());
                                    item.SubItems.Add(_browserHandlers.GetBrowserProtocolsAndFileTypes(browser));

                                    // ImageListにアイコンを追加
                                    if (_imBrowserIcons != null)
                                    {
                                        var image = ImageUtilities.GetImage(browser, false);
                                        if (image != null)
                                        {
                                            _imBrowserIcons.Images.Add(image);
                                        }
                                    }

                                    _mLastBrowserID++;
                                    _isModified = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("OptionsForm.SetupBrowsersDragDrop", $"ブラウザ追加エラー: {filePath}", ex.Message);
                            }
                        }
                    }
                }
                listView.BackColor = Color.FromKnownColor(KnownColor.Window);
            };
        }

        /// <summary>
        /// URLドラッグ&amp;ドロップ機能の設定
        /// </summary>
        private void SetupURLDragDrop()
        {
            Logger.LogInfo("OptionsForm.SetupURLDragDrop", "URLドラッグ&ドロップ機能設定開始");

            try
            {
                // URLリストビューにドラッグ&ドロップを設定
                var listViewURLs = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                if (listViewURLs != null)
                {
                    // テスト環境ではDragDropを無効化
                    if (IsTestEnvironment())
                    {
                        listViewURLs.AllowDrop = false;
                    }
                    else
                    {
                        listViewURLs.AllowDrop = true;
                        listViewURLs.DragEnter += _dragDropHandlers.ListViewURLs_DragEnter;
                        listViewURLs.DragDrop += _dragDropHandlers.ListViewURLs_DragDrop;
                        listViewURLs.DragOver += _dragDropHandlers.ListViewURLs_DragOver;
                        listViewURLs.ItemDrag += _dragDropHandlers.ListViewURLs_ItemDrag;
                        listViewURLs.DragLeave += _dragDropHandlers.ListViewURLs_DragLeave;
                    }
                }

                Logger.LogInfo("OptionsForm.SetupURLDragDrop", "URLドラッグ&ドロップ機能設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupURLDragDrop", "URLドラッグ&ドロップ機能設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリデータを読み込みます
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                categoryListView.Items.Clear();

                // ブラウザからカテゴリを収集
                var categories = _mBrowser.Values
                    .Select(b => b.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();

                // URLからカテゴリを収集
                categories.AddRange(_mURLs.Values
                    .Select(u => u.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct());

                // プロトコルからカテゴリを収集
                categories.AddRange(_mProtocols.Values
                    .Select(p => p.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct());


                // 重複を除去してソート
                categories = categories.Distinct().OrderBy(c => c).ToList();

                foreach (var category in categories)
                {
                    var item = categoryListView.Items.Add(category);
                    var count = GetCategoryItemCount(category);
                    item.SubItems.Add(count.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.LoadCategories", "カテゴリ読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリ内のアイテム数を取得します
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <returns>アイテム数</returns>
        private int GetCategoryItemCount(string category)
        {
            var count = 0;
            count += _mBrowser.Values.Count(b => b.Category == category);
            count += _mURLs.Values.Count(u => u.Category == category);
            count += _mProtocols.Values.Count(p => p.Category == category);
            // count += _mFileTypes.Values.Count(f => f.Category == category);
            return count;
        }

        /// <summary>
        /// ブラウザドラッグ&amp;ドロップ機能の設定
        /// </summary>
        private void SetupBrowserDragDrop()
        {
            Logger.LogInfo("OptionsForm.SetupBrowserDragDrop", "ブラウザドラッグ&ドロップ機能設定開始");

            try
            {
                // ブラウザリストビューにドラッグ&ドロップを設定
                var listViewBrowsers = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                if (listViewBrowsers != null)
                {
                    // テスト環境ではDragDropを無効化
                    if (IsTestEnvironment())
                    {
                        listViewBrowsers.AllowDrop = false;
                    }
                    else
                    {
                        listViewBrowsers.AllowDrop = true;
                        listViewBrowsers.DragEnter += _dragDropHandlers.ListViewBrowsers_DragEnter;
                        listViewBrowsers.DragDrop += _dragDropHandlers.ListViewBrowsers_DragDrop;
                        listViewBrowsers.DragLeave += _dragDropHandlers.ListViewBrowsers_DragLeave;
                    }
                }

                Logger.LogInfo("OptionsForm.SetupBrowserDragDrop", "ブラウザドラッグ&ドロップ機能設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupBrowserDragDrop", "ブラウザドラッグ&ドロップ機能設定エラー", ex.Message);
            }
        }
    }
}
