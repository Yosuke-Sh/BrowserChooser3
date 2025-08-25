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
        private OptionsFormCategoryHandlers _categoryHandlers;
        private OptionsFormBrowserHandlers _browserHandlers;
        private OptionsFormProtocolHandlers _protocolHandlers;
        private OptionsFormFileTypeHandlers _fileTypeHandlers;
        private OptionsFormListHandlers _listHandlers;
        private OptionsFormDragDropHandlers _dragDropHandlers;
        private OptionsFormCheckBoxHandlers _checkBoxHandlers;
        private OptionsFormBackgroundHandlers _backgroundHandlers;
        private OptionsFormHelpHandlers _helpHandlers;
        private OptionsFormAccessibilityHandlers _accessibilityHandlers;

        // UIパネル作成クラス
        private OptionsFormPanels _panels;

        // 内部データ管理（Browser Chooser 2互換）
        private Dictionary<int, Browser> _mBrowser = new();
        private SortedDictionary<int, URL> _mURLs = new();
        private Dictionary<int, Protocol> _mProtocols = new();
        private Dictionary<int, FileType> _mFileTypes = new();
        private bool _mProtocolsAreDirty = false;
        private bool _mFileTypesAreDirty = false;
        private int _mLastBrowserID = 0;
        private int _mLastURLID = 0;
        private int _mLastProtocolID = 0;
        private int _mLastFileTypeID = 0;

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
            _categoryHandlers = new OptionsFormCategoryHandlers(this, (modified) => _isModified = modified, LoadCategories);
            _browserHandlers = new OptionsFormBrowserHandlers(this, _settings, _mBrowser, _mProtocols, _mFileTypes, _imBrowserIcons, SetModified);
            _protocolHandlers = new OptionsFormProtocolHandlers(this, _mProtocols, _mBrowser, SetModified);
            _fileTypeHandlers = new OptionsFormFileTypeHandlers(this, _mFileTypes, _mBrowser, SetModified);
            _listHandlers = new OptionsFormListHandlers(this);
            _dragDropHandlers = new OptionsFormDragDropHandlers(this, _settings, _mBrowser, _mProtocols, _mFileTypes, SetModified, RebuildAutoURLs);
            _checkBoxHandlers = new OptionsFormCheckBoxHandlers(this, _settings, SetModified);
            _backgroundHandlers = new OptionsFormBackgroundHandlers(this, _settings, SetModified);
            _helpHandlers = new OptionsFormHelpHandlers(this);
            _accessibilityHandlers = new OptionsFormAccessibilityHandlers(this, _mFocusSettings, SetModified);

            // UIパネル作成クラスの初期化
            _panels = new OptionsFormPanels();

            InitializeForm();

            // フォームイベントの設定
            FormClosing += _formHandlers.OptionsForm_FormClosing;
            Shown += _formHandlers.OptionsForm_Shown;
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

                // TreeViewノードの作成
                var commonNode = new TreeNode("Common");
                commonNode.Nodes.Add(new TreeNode("Browsers & applications") { Tag = "tabBrowsers" });
                commonNode.Nodes.Add(new TreeNode("Auto URLs") { Tag = "tabAutoURLs" });

                var associationsNode = new TreeNode("Associations");
                associationsNode.Nodes.Add(new TreeNode("Protocols") { Tag = "tabProtocols" });
                associationsNode.Nodes.Add(new TreeNode("File Types") { Tag = "tabFileTypes" });
                associationsNode.Nodes.Add(new TreeNode("Categories") { Tag = "tabCategories" });

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
                var browsersTab = _panels.CreateBrowsersPanel(_settings, _mBrowser, _mProtocols, _mFileTypes, _mLastBrowserID, _imBrowserIcons, SetModified, RebuildAutoURLs);
                var autoUrlsTab = _panels.CreateAutoURLsPanel(_settings, _mURLs, _mBrowser, SetModified, RebuildAutoURLs);
                var protocolsTab = _panels.CreateProtocolsPanel(_settings, _mProtocols, _mBrowser, SetModified);
                var fileTypesTab = _panels.CreateFileTypesPanel(_settings, _mFileTypes, _mBrowser, SetModified);
                var categoriesTab = _panels.CreateCategoriesPanel();
                var displayTab = _panels.CreateDisplayPanel(_settings, SetModified);
                var gridTab = _panels.CreateGridPanel(_settings, SetModified);
                var privacyTab = _panels.CreatePrivacyPanel(_settings, SetModified);
                var startupTab = _panels.CreateStartupPanel(_settings, SetModified);
                var othersTab = _panels.CreateOthersPanel(_settings, SetModified);

                tabSettings.TabPages.Add(browsersTab);
                tabSettings.TabPages.Add(autoUrlsTab);
                tabSettings.TabPages.Add(protocolsTab);
                tabSettings.TabPages.Add(fileTypesTab);
                tabSettings.TabPages.Add(categoriesTab);
                tabSettings.TabPages.Add(displayTab);
                tabSettings.TabPages.Add(gridTab);
                tabSettings.TabPages.Add(privacyTab);
                tabSettings.TabPages.Add(startupTab);
                tabSettings.TabPages.Add(othersTab);

                // TreeViewを展開
                treeSettings.ExpandAll();

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

                Logger.LogInfo("OptionsForm.InitializeForm", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.InitializeForm", "初期化エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"オプション画面の初期化に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                SetupFileTypesPanelButtons();
                
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
                
                // カテゴリ管理ボタンのイベントハンドラー設定
                btnAddCategory.Click += _categoryHandlers.BtnAddCategory_Click;
                btnEditCategory.Click += _categoryHandlers.BtnEditCategory_Click;
                btnDeleteCategory.Click += _categoryHandlers.BtnDeleteCategory_Click;
                
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

                    if (addButton != null) addButton.Click += AddAutoURL_Click;
                    if (editButton != null) editButton.Click += EditAutoURL_Click;
                    if (deleteButton != null) deleteButton.Click += DeleteAutoURL_Click;
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

                    if (addButton != null) addButton.Click += _protocolHandlers.AddProtocol_Click;
                    if (editButton != null) editButton.Click += _protocolHandlers.EditProtocol_Click;
                    if (deleteButton != null) deleteButton.Click += _protocolHandlers.DeleteProtocol_Click;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupProtocolsPanelButtons", "プロトコルパネルボタン設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// ファイルタイプパネルのボタンイベントハンドラー設定
        /// </summary>
        private void SetupFileTypesPanelButtons()
        {
            try
            {
                var fileTypesTab = tabSettings.TabPages["tabFileTypes"];
                if (fileTypesTab != null)
                {
                    var addButton = fileTypesTab.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
                    var editButton = fileTypesTab.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
                    var deleteButton = fileTypesTab.Controls.Find("btnDelete", true).FirstOrDefault() as Button;

                    if (addButton != null) addButton.Click += _fileTypeHandlers.AddFileType_Click;
                    if (editButton != null) editButton.Click += _fileTypeHandlers.EditFileType_Click;
                    if (deleteButton != null) deleteButton.Click += _fileTypeHandlers.DeleteFileType_Click;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupFileTypesPanelButtons", "ファイルタイプパネルボタン設定エラー", ex.Message);
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
                    var accessibilityButton = displayTab.Controls.Find("btnAccessibility", true).FirstOrDefault() as Button;
                    var backgroundColorButton = displayTab.Controls.Find("btnBackgroundColor", true).FirstOrDefault() as Button;
                    var transparentButton = displayTab.Controls.Find("btnTransparent", true).FirstOrDefault() as Button;

                    if (accessibilityButton != null) accessibilityButton.Click += _accessibilityHandlers.AccessibilityButton_Click;
                    if (backgroundColorButton != null) backgroundColorButton.Click += _backgroundHandlers.BackgroundColorButton_Click;
                    if (transparentButton != null) transparentButton.Click += _backgroundHandlers.TransparentButton_Click;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupDisplayPanelButtons", "Displayパネルボタン設定エラー", ex.Message);
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
        /// レイアウトの調整
        /// </summary>
        private void AdjustLayout()
        {
            var treeSettings = Controls.OfType<TreeView>().FirstOrDefault();
            var tabSettings = Controls.OfType<TabControl>().FirstOrDefault();
            var saveButton = Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Save");
            var cancelButton = Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Cancel");
            var helpButton = Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Help");

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

            if (saveButton != null)
            {
                saveButton.Location = new Point(ClientSize.Width - 280, ClientSize.Height - 50);
            }

            if (cancelButton != null)
            {
                cancelButton.Location = new Point(ClientSize.Width - 180, ClientSize.Height - 50);
            }

            if (helpButton != null)
            {
                helpButton.Location = new Point(ClientSize.Width - 80, ClientSize.Height - 50);
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
                // プロトコル設定の読み込み
                _mProtocols.Clear();
                foreach (var protocol in _settings.Protocols)
                {
                    _mProtocols.Add(_mProtocols.Count, protocol.Clone());
                }
                _mLastProtocolID = _mProtocols.Count - 1;

                // ファイルタイプ設定の読み込み
                _mFileTypes.Clear();
                foreach (var fileType in _settings.FileTypes)
                {
                    _mFileTypes.Add(_mFileTypes.Count, fileType.Clone());
                }
                _mLastFileTypeID = _mFileTypes.Count - 1;

                // ブラウザ設定の読み込み
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
                MessageBox.Show($"設定の読み込みに失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // 基本設定
            var chkShowURLs = Controls.Find("chkShowURLs", true).FirstOrDefault() as CheckBox;
            if (chkShowURLs != null) chkShowURLs.Checked = _settings.ShowURL;

            var chkRevealShortURLs = Controls.Find("chkRevealShortURLs", true).FirstOrDefault() as CheckBox;
            if (chkRevealShortURLs != null) chkRevealShortURLs.Checked = _settings.RevealShortURL;

            var chkPortableMode = Controls.Find("chkPortableMode", true).FirstOrDefault() as CheckBox;
            if (chkPortableMode != null) chkPortableMode.Checked = _settings.PortableMode;

            var chkAutoCheckUpdate = Controls.Find("chkAutoCheckUpdate", true).FirstOrDefault() as CheckBox;
            if (chkAutoCheckUpdate != null) chkAutoCheckUpdate.Checked = _settings.AutomaticUpdates;

            var nudHeight = Controls.Find("nudHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudHeight != null) nudHeight.Value = _settings.Height;

            var nudWidth = Controls.Find("nudWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudWidth != null) nudWidth.Value = _settings.Width;

            var chkCheckDefaultOnLaunch = Controls.Find("chkCheckDefaultOnLaunch", true).FirstOrDefault() as CheckBox;
            if (chkCheckDefaultOnLaunch != null) chkCheckDefaultOnLaunch.Checked = _settings.CheckDefaultOnLaunch;

            var chkAdvanced = Controls.Find("chkAdvanced", true).FirstOrDefault() as CheckBox;
            if (chkAdvanced != null) chkAdvanced.Checked = _settings.AdvancedScreens;

            var nudDelayBeforeAutoload = Controls.Find("nudDelayBeforeAutoload", true).FirstOrDefault() as NumericUpDown;
            if (nudDelayBeforeAutoload != null) nudDelayBeforeAutoload.Value = _settings.DefaultDelay;

            var txtSeparator = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
            if (txtSeparator != null) txtSeparator.Text = _settings.Separator;

            var chkAllowStayOpen = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
            if (chkAllowStayOpen != null) chkAllowStayOpen.Checked = _settings.AllowStayOpen;

            // 詳細設定
            var txtUserAgent = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
            if (txtUserAgent != null) txtUserAgent.Text = _settings.UserAgent;

            var chkDownloadDetectionfile = Controls.Find("chkDownloadDetectionfile", true).FirstOrDefault() as CheckBox;
            if (chkDownloadDetectionfile != null) chkDownloadDetectionfile.Checked = _settings.DownloadDetectionFile;

            var nudIconSizeWidth = Controls.Find("nudIconSizeWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudIconSizeWidth != null) nudIconSizeWidth.Value = _settings.IconWidth;

            var nudIconSizeHeight = Controls.Find("nudIconSizeHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudIconSizeHeight != null) nudIconSizeHeight.Value = _settings.IconHeight;

            var nudIconGapWidth = Controls.Find("nudIconGapWidth", true).FirstOrDefault() as NumericUpDown;
            if (nudIconGapWidth != null) nudIconGapWidth.Value = _settings.IconGapWidth;

            var nudIconGapHeight = Controls.Find("nudIconGapHeight", true).FirstOrDefault() as NumericUpDown;
            if (nudIconGapHeight != null) nudIconGapHeight.Value = _settings.IconGapHeight;

            var pbBackgroundColor = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as Panel;
            if (pbBackgroundColor != null) pbBackgroundColor.BackColor = Color.FromArgb(_settings.BackgroundColor);

            var nudIconScale = Controls.Find("nudIconScale", true).FirstOrDefault() as NumericUpDown;
            if (nudIconScale != null) nudIconScale.Value = (decimal)_settings.IconScale;

            var chkCanonicalize = Controls.Find("chkCanonicalize", true).FirstOrDefault() as CheckBox;
            if (chkCanonicalize != null) chkCanonicalize.Checked = _settings.Canonicalize;

            var txtCanonicalizeAppend = Controls.Find("txtCanonicalizeAppend", true).FirstOrDefault() as TextBox;
            if (txtCanonicalizeAppend != null) txtCanonicalizeAppend.Text = _settings.CanonicalizeAppendedText;

            var chkLog = Controls.Find("chkLog", true).FirstOrDefault() as CheckBox;
            if (chkLog != null) chkLog.Checked = _settings.EnableLogging;

            var chkExtract = Controls.Find("chkExtract", true).FirstOrDefault() as CheckBox;
            if (chkExtract != null) chkExtract.Checked = _settings.ExtractDLLs;

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
            var chkEnableLogging = Controls.Find("chkEnableLogging", true).FirstOrDefault() as CheckBox;
            if (chkEnableLogging != null) chkEnableLogging.Checked = _settings.EnableLogging;

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
            var chkAutoStart = Controls.Find("chkAutoStart", true).FirstOrDefault() as CheckBox;
            if (chkAutoStart != null) chkAutoStart.Checked = _settings.AutoStart;

            var chkStartMinimized = Controls.Find("chkStartMinimized", true).FirstOrDefault() as CheckBox;
            if (chkStartMinimized != null) chkStartMinimized.Checked = _settings.StartMinimized;

            var chkStartInTray = Controls.Find("chkStartInTray", true).FirstOrDefault() as CheckBox;
            if (chkStartInTray != null) chkStartInTray.Checked = _settings.StartInTray;

            var chkCheckDefaultOnStartup = Controls.Find("chkCheckDefaultOnStartup", true).FirstOrDefault() as CheckBox;
            if (chkCheckDefaultOnStartup != null) chkCheckDefaultOnStartup.Checked = _settings.CheckDefaultOnLaunch;

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

            var chkUseAero = Controls.Find("chkUseAero", true).FirstOrDefault() as CheckBox;
            if (chkUseAero != null) chkUseAero.Checked = _settings.UseAero;

            // ショートカット設定
            var txtOptionsShortcut = Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
            if (txtOptionsShortcut != null && _settings.OptionsShortcut != char.MinValue)
            {
                txtOptionsShortcut.Text = _settings.OptionsShortcut.ToString();
            }

            var txtMessage = Controls.Find("txtMessage", true).FirstOrDefault() as TextBox;
            if (txtMessage != null) txtMessage.Text = _settings.DefaultMessage;

            // フォーカス設定
            _mFocusSettings.ShowFocus = _settings.ShowFocus;
            _mFocusSettings.BoxColor = Color.FromArgb(_settings.FocusBoxColor);
            _mFocusSettings.BoxWidth = _settings.FocusBoxLineWidth;

            // デフォルトブラウザGUIDの設定
            var hiddenLabel = lblHiddenBrowserGuid;
            if (hiddenLabel != null && _settings.DefaultBrowserGuid != Guid.Empty)
            {
                hiddenLabel.Tag = _settings.DefaultBrowserGuid.ToString();
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

                // ファイルタイプ設定の保存
                _settings.FileTypes = new List<FileType>();
                foreach (var fileType in _mFileTypes)
                {
                    _settings.FileTypes.Add(fileType.Value.Clone());
                }

                // プロトコル・ファイルタイプが変更された場合の確認
                if (_mFileTypesAreDirty || _mProtocolsAreDirty)
                {
                    var result = MessageBox.Show("You have changed the accepted Protocols or Filetypes.",
                        "Protocols/Filetypes Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 各コントロールから設定値を取得して保存
                // 基本設定
                var chkShowURLs = Controls.Find("chkShowURLs", true).FirstOrDefault() as CheckBox;
                if (chkShowURLs != null) _settings.ShowURL = chkShowURLs.Checked;

                var chkRevealShortURLs = Controls.Find("chkRevealShortURLs", true).FirstOrDefault() as CheckBox;
                if (chkRevealShortURLs != null) _settings.RevealShortURL = chkRevealShortURLs.Checked;

                var chkPortableMode = Controls.Find("chkPortableMode", true).FirstOrDefault() as CheckBox;
                if (chkPortableMode != null) _settings.PortableMode = chkPortableMode.Checked;

                var chkAutoCheckUpdate = Controls.Find("chkAutoCheckUpdate", true).FirstOrDefault() as CheckBox;
                if (chkAutoCheckUpdate != null) _settings.AutomaticUpdates = chkAutoCheckUpdate.Checked;

                var nudHeight = Controls.Find("nudHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudHeight != null) _settings.Height = (int)nudHeight.Value;

                var nudWidth = Controls.Find("nudWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudWidth != null) _settings.Width = (int)nudWidth.Value;

                var chkCheckDefaultOnLaunch = Controls.Find("chkCheckDefaultOnLaunch", true).FirstOrDefault() as CheckBox;
                if (chkCheckDefaultOnLaunch != null) _settings.CheckDefaultOnLaunch = chkCheckDefaultOnLaunch.Checked;

                var chkAdvanced = Controls.Find("chkAdvanced", true).FirstOrDefault() as CheckBox;
                if (chkAdvanced != null) _settings.AdvancedScreens = chkAdvanced.Checked;

                var nudDelayBeforeAutoload = Controls.Find("nudDelayBeforeAutoload", true).FirstOrDefault() as NumericUpDown;
                if (nudDelayBeforeAutoload != null) _settings.DefaultDelay = (int)nudDelayBeforeAutoload.Value;

                var txtSeparator = Controls.Find("txtSeparator", true).FirstOrDefault() as TextBox;
                if (txtSeparator != null) _settings.Separator = txtSeparator.Text;

                var chkAllowStayOpen = Controls.Find("chkAllowStayOpen", true).FirstOrDefault() as CheckBox;
                if (chkAllowStayOpen != null) _settings.AllowStayOpen = chkAllowStayOpen.Checked;

                // 詳細設定
                var txtUserAgent = Controls.Find("txtUserAgent", true).FirstOrDefault() as TextBox;
                if (txtUserAgent != null) _settings.UserAgent = txtUserAgent.Text;

                var chkDownloadDetectionfile = Controls.Find("chkDownloadDetectionfile", true).FirstOrDefault() as CheckBox;
                if (chkDownloadDetectionfile != null) _settings.DownloadDetectionFile = chkDownloadDetectionfile.Checked;

                var nudIconSizeWidth = Controls.Find("nudIconSizeWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeWidth != null) _settings.IconWidth = (int)nudIconSizeWidth.Value;

                var nudIconSizeHeight = Controls.Find("nudIconSizeHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconSizeHeight != null) _settings.IconHeight = (int)nudIconSizeHeight.Value;

                var nudIconGapWidth = Controls.Find("nudIconGapWidth", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapWidth != null) _settings.IconGapWidth = (int)nudIconGapWidth.Value;

                var nudIconGapHeight = Controls.Find("nudIconGapHeight", true).FirstOrDefault() as NumericUpDown;
                if (nudIconGapHeight != null) _settings.IconGapHeight = (int)nudIconGapHeight.Value;

                var pbBackgroundColor = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as Panel;
                if (pbBackgroundColor != null) _settings.BackgroundColor = pbBackgroundColor.BackColor.ToArgb();

                var nudIconScale = Controls.Find("nudIconScale", true).FirstOrDefault() as NumericUpDown;
                if (nudIconScale != null) _settings.IconScale = (double)nudIconScale.Value;

                var chkCanonicalize = Controls.Find("chkCanonicalize", true).FirstOrDefault() as CheckBox;
                if (chkCanonicalize != null) _settings.Canonicalize = chkCanonicalize.Checked;

                var txtCanonicalizeAppend = Controls.Find("txtCanonicalizeAppend", true).FirstOrDefault() as TextBox;
                if (txtCanonicalizeAppend != null) _settings.CanonicalizeAppendedText = txtCanonicalizeAppend.Text;

                var chkLog = Controls.Find("chkLog", true).FirstOrDefault() as CheckBox;
                if (chkLog != null) _settings.EnableLogging = chkLog.Checked;

                var chkExtract = Controls.Find("chkExtract", true).FirstOrDefault() as CheckBox;
                if (chkExtract != null) _settings.ExtractDLLs = chkExtract.Checked;

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
                var chkEnableLogging = Controls.Find("chkEnableLogging", true).FirstOrDefault() as CheckBox;
                if (chkEnableLogging != null) _settings.EnableLogging = chkEnableLogging.Checked;

                var cmbLogLevel = Controls.Find("cmbLogLevel", true).FirstOrDefault() as ComboBox;
                if (cmbLogLevel != null) _settings.LogLevel = cmbLogLevel.SelectedIndex;

                var chkKeepHistory = Controls.Find("chkKeepHistory", true).FirstOrDefault() as CheckBox;
                if (chkKeepHistory != null) _settings.KeepHistory = chkKeepHistory.Checked;

                var nudHistoryDays = Controls.Find("nudHistoryDays", true).FirstOrDefault() as NumericUpDown;
                if (nudHistoryDays != null) _settings.HistoryDays = (int)nudHistoryDays.Value;

                var chkPrivacyMode = Controls.Find("chkPrivacyMode", true).FirstOrDefault() as CheckBox;
                if (chkPrivacyMode != null) _settings.PrivacyMode = chkPrivacyMode.Checked;

                var chkAllowDataCollection = Controls.Find("chkAllowDataCollection", true).FirstOrDefault() as CheckBox;
                if (chkAllowDataCollection != null) _settings.AllowDataCollection = chkAllowDataCollection.Checked;

                // スタートアップ設定
                var chkAutoStart = Controls.Find("chkAutoStart", true).FirstOrDefault() as CheckBox;
                if (chkAutoStart != null) _settings.AutoStart = chkAutoStart.Checked;

                var chkStartMinimized = Controls.Find("chkStartMinimized", true).FirstOrDefault() as CheckBox;
                if (chkStartMinimized != null) _settings.StartMinimized = chkStartMinimized.Checked;

                var chkStartInTray = Controls.Find("chkStartInTray", true).FirstOrDefault() as CheckBox;
                if (chkStartInTray != null) _settings.StartInTray = chkStartInTray.Checked;

                var chkCheckDefaultOnStartup = Controls.Find("chkCheckDefaultOnStartup", true).FirstOrDefault() as CheckBox;
                if (chkCheckDefaultOnStartup != null) _settings.CheckDefaultOnLaunch = chkCheckDefaultOnStartup.Checked;

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

                var chkUseAero = Controls.Find("chkUseAero", true).FirstOrDefault() as CheckBox;
                if (chkUseAero != null) _settings.UseAero = chkUseAero.Checked;

                // フォーカス設定
                _settings.ShowFocus = _mFocusSettings.ShowFocus;
                _settings.FocusBoxColor = _mFocusSettings.BoxColor.ToArgb();
                _settings.FocusBoxLineWidth = _mFocusSettings.BoxWidth;

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

                var txtMessage = Controls.Find("txtMessage", true).FirstOrDefault() as TextBox;
                if (txtMessage != null) _settings.DefaultMessage = txtMessage.Text;

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

                                if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens,
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
                    listViewURLs.AllowDrop = true;
                    listViewURLs.DragEnter += _dragDropHandlers.ListViewURLs_DragEnter;
                    listViewURLs.DragDrop += _dragDropHandlers.ListViewURLs_DragDrop;
                    listViewURLs.DragOver += _dragDropHandlers.ListViewURLs_DragOver;
                    listViewURLs.ItemDrag += _dragDropHandlers.ListViewURLs_ItemDrag;
                    listViewURLs.DragLeave += _dragDropHandlers.ListViewURLs_DragLeave;
                }

                Logger.LogInfo("OptionsForm.SetupURLDragDrop", "URLドラッグ&ドロップ機能設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupURLDragDrop", "URLドラッグ&ドロップ機能設定エラー", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリ管理機能の設定
        /// </summary>
        private void SetupCategoryManagement()
        {
            Logger.LogInfo("OptionsForm.SetupCategoryManagement", "カテゴリ管理機能設定開始");

            try
            {
                // カテゴリ管理用のコントロールは Designer で定義済み
                // イベントハンドラーの設定
                btnAddCategory.Click += _categoryHandlers.BtnAddCategory_Click;
                btnEditCategory.Click += _categoryHandlers.BtnEditCategory_Click;
                btnDeleteCategory.Click += _categoryHandlers.BtnDeleteCategory_Click;

                // カテゴリデータの読み込み
                LoadCategories();

                Logger.LogInfo("OptionsForm.SetupCategoryManagement", "カテゴリ管理機能設定完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.SetupCategoryManagement", "カテゴリ管理機能設定エラー", ex.Message);
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

                // ファイルタイプからカテゴリを収集
                categories.AddRange(_mFileTypes.Values
                    .Select(f => f.Category)
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
            count += _mFileTypes.Values.Count(f => f.Category == category);
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
                    listViewBrowsers.AllowDrop = true;
                    listViewBrowsers.DragEnter += _dragDropHandlers.ListViewBrowsers_DragEnter;
                    listViewBrowsers.DragDrop += _dragDropHandlers.ListViewBrowsers_DragDrop;
                    listViewBrowsers.DragLeave += _dragDropHandlers.ListViewBrowsers_DragLeave;
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
