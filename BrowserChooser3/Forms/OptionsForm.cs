using BrowserChooser3.Classes;

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
        private bool _isCanceled = false;
        
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
        private ImageList? _imBrowserIcons;
        
        // フォーカス設定（Browser Chooser 2互換）
        private FocusSettings _mFocusSettings = new();

        /// <summary>
        /// OptionsFormクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public OptionsForm(Settings settings)
        {
            _settings = settings;
            InitializeForm();
            
            // フォーム閉じる時の確認ダイアログ
            FormClosing += OptionsForm_FormClosing;
            
            // フォーム表示時の設定読み込み
            Shown += OptionsForm_Shown;
        }

        /// <summary>
        /// フォームの初期化（Browser Chooser 2互換）
        /// </summary>
        private void InitializeForm()
        {
            Logger.LogInfo("OptionsForm.InitializeForm", "Start");
            
            try
            {
                // フォームの基本設定（Browser Chooser 2互換）
                Text = "Options";
                Size = new Size(1034, 327);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = false;
                MinimizeBox = true;
                TopMost = true;
                
                // フォントの設定（現代的で日本語・英語両対応）
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                
                // 最小サイズの設定（Browser Chooser 2互換）
                MinimumSize = new Size(595, 350);
                
                // Windows 8/10対応の警告表示（Browser Chooser 2互換）
                SetupWindowsCompatibilityWarnings();
                
                // PictureBox1（Browser Chooser 2互換）
                var pictureBox1 = new PictureBox();
                pictureBox1.Image = Properties.Resources.BrowserChooser3Icon.ToBitmap();
                pictureBox1.Location = new Point(12, 79);
                pictureBox1.Size = new Size(161, 161);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.TabIndex = 0;
                pictureBox1.TabStop = false;

                // TreeViewの作成（Browser Chooser 2互換）
                var treeSettings = new TreeView();
                treeSettings.Location = new Point(12, 12);
                treeSettings.Size = new Size(173, 270);
                treeSettings.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                treeSettings.AfterSelect += TreeSettings_AfterSelect;
                
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
                
                // TabControlの作成（Browser Chooser 2互換）
                var tabSettings = new TabControl();
                tabSettings.Location = new Point(179, 12);
                tabSettings.Size = new Size(852, 274);
                tabSettings.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                tabSettings.Appearance = TabAppearance.FlatButtons;
                tabSettings.ItemSize = new Size(0, 1);
                tabSettings.SizeMode = TabSizeMode.Fixed;
                
                // タブページの作成
                var browsersTab = CreateBrowsersPanel();
                var autoUrlsTab = CreateAutoURLsPanel();
                var protocolsTab = CreateProtocolsPanel();
                var fileTypesTab = CreateFileTypesPanel();
                var categoriesTab = CreateCategoriesPanel();
                var displayTab = CreateDisplayPanel();
                var gridTab = CreateGridPanel();
                var privacyTab = CreatePrivacyPanel();
                var startupTab = CreateStartupPanel();
                var othersTab = CreateOthersPanel();

                var defaultBrowserTab = CreateDefaultBrowserPanel();
                
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

                tabSettings.TabPages.Add(defaultBrowserTab);
                
                // ボタンの作成（Browser Chooser 2互換）
                var saveButton = new Button();
                saveButton.Text = "Save";
                saveButton.Location = new Point(790, 292);
                saveButton.Size = new Size(75, 23);
                saveButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                saveButton.Click += (s, e) => SaveSettings();
                
                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.Location = new Point(952, 292);
                cancelButton.Size = new Size(75, 23);
                cancelButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                
                var helpButton = new Button();
                helpButton.Text = "Help";
                helpButton.Location = new Point(871, 292);
                helpButton.Size = new Size(75, 23);
                helpButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                helpButton.Click += (s, e) => OpenHelp();
                
                // コントロールの追加（Browser Chooser 2互換）
                Controls.Add(pictureBox1);
                Controls.Add(treeSettings);
                Controls.Add(tabSettings);
                Controls.Add(saveButton);
                Controls.Add(cancelButton);
                Controls.Add(helpButton);
                
                // サイズ変更イベントの設定
                Resize += OptionsForm_Resize;
                
                // TreeViewを展開
                treeSettings.ExpandAll();
                
                // 設定の読み込み
                LoadSettings();
                
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
            }

            if (tabSettings != null)
            {
                tabSettings.Location = new Point(220, 10);
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
                var tabControl = Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControl != null)
                {
                    var tabPage = tabControl.TabPages.Cast<TabPage>()
                        .FirstOrDefault(tp => tp.Name == tabName);
                    if (tabPage != null)
                    {
                        tabControl.SelectedTab = tabPage;
                    }
                }
            }
        }

        /// <summary>
        /// コントロールの作成
        /// </summary>
        private void CreateControls()
        {
            // タブコントロールの作成
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // ブラウザタブ
            var browsersTab = new TabPage("ブラウザ");
            browsersTab.Controls.Add(CreateBrowsersPanel());
            tabControl.TabPages.Add(browsersTab);

            // URLタブ
            var urlsTab = new TabPage("URL設定");
            urlsTab.Controls.Add(CreateAutoURLsPanel());
            tabControl.TabPages.Add(urlsTab);

            // 一般設定タブ
            var generalTab = new TabPage("一般設定");
            generalTab.Controls.Add(CreateGeneralPanel());
            tabControl.TabPages.Add(generalTab);

            // ボタンパネル
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            var saveButton = new Button
            {
                Text = "保存",
                DialogResult = DialogResult.OK,
                Location = new Point(buttonPanel.Width - 100, 10),
                Size = new Size(80, 30)
            };
            saveButton.Click += SaveButton_Click;

            var cancelButton = new Button
            {
                Text = "キャンセル",
                DialogResult = DialogResult.Cancel,
                Location = new Point(buttonPanel.Width - 190, 10),
                Size = new Size(80, 30)
            };

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            Controls.Add(tabControl);
            Controls.Add(buttonPanel);
        }

        /// <summary>
        /// ブラウザ設定パネルの作成
        /// </summary>
        private TabPage CreateBrowsersPanel()
        {
            var tabPage = new TabPage("Browsers & applications");
            tabPage.Name = "tabBrowsers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // アイコンリスト（Browser Chooser 2互換）
            _imBrowserIcons = new ImageList
            {
                ColorDepth = ColorDepth.Depth8Bit,
                ImageSize = new Size(16, 16),
                TransparentColor = Color.Transparent
            };

            // ブラウザリストビュー（Browser Chooser 2互換）
            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = true,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false,
                SmallImageList = _imBrowserIcons
            };

            listView.Columns.Add("Name", 109);
            listView.Columns.Add("Default", 60);
            listView.Columns.Add("Row", 50);
            listView.Columns.Add("Column", 60);
            listView.Columns.Add("Hotkey", 60);
            listView.Columns.Add("File Types and Protocols", 333);

            // ボタン群（Browser Chooser 2互換）
            var addButton = new Button
            {
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            addButton.Click += AddBrowser_Click;

            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };
            editButton.Click += EditBrowser_Click;

            var cloneButton = new Button
            {
                Text = "Clone",
                Location = new Point(6, 64),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };
            cloneButton.Click += CloneBrowser_Click;

            var defaultButton = new Button
            {
                Text = "Set default",
                Location = new Point(6, 93),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };
            defaultButton.Click += SetDefaultBrowser_Click;

            var detectButton = new Button
            {
                Text = "Detect",
                Location = new Point(6, 122),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            detectButton.Click += DetectBrowsers_Click;

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(6, 201),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };
            deleteButton.Click += DeleteBrowser_Click;

            // ダブルクリック注釈（Browser Chooser 2互換）
            var lblDoubleClickNote = new Label
            {
                Text = "(Double click\r\nrow to edit)",
                Location = new Point(6, 159),
                Size = new Size(75, 26),
                TextAlign = ContentAlignment.TopCenter,
                Visible = false
            };

            // ダブルクリック編集機能
            listView.DoubleClick += (s, e) => EditBrowser_Click(s, e);

            // 選択変更時のボタン有効/無効切り替え（Browser Chooser 2互換）
            listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSelection = listView.SelectedItems.Count > 0;
                editButton.Enabled = hasSelection;
                cloneButton.Enabled = hasSelection;
                defaultButton.Enabled = hasSelection;
                deleteButton.Enabled = hasSelection;
                lblDoubleClickNote.Visible = hasSelection;
                
                // 変更フラグを設定
                if (hasSelection)
                {
                    _isModified = true;
                }
            };

            // 隠しブラウザGUID（Browser Chooser 2互換）
            var lblHiddenBrowserGuid = new Label
            {
                Text = "Hidden Browser default Guid",
                Location = new Point(18, 227),
                Size = new Size(142, 13),
                Tag = " ",
                Visible = false
            };

            panel.Controls.Add(listView);
            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(cloneButton);
            panel.Controls.Add(defaultButton);
            panel.Controls.Add(detectButton);
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(lblDoubleClickNote);
            panel.Controls.Add(lblHiddenBrowserGuid);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// Auto URLsパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateAutoURLsPanel()
        {
            var tabPage = new TabPage("Auto URLs");
            tabPage.Name = "tabAutoURLs";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Auto URLs Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // ListView for URLs（Browser Chooser 2互換）
            var lstURLs = new ListView
            {
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            lstURLs.Columns.Add("URL", 226);
            lstURLs.Columns.Add("Browser", 105);
            lstURLs.Columns.Add("Timeout", 100);

            // Buttons（Browser Chooser 2互換）
            var btnAdd = new Button { Text = "Add", Location = new Point(6, 6), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnEdit = new Button { Text = "Edit", Location = new Point(6, 35), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnDelete = new Button { Text = "Delete", Location = new Point(6, 64), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnMoveUp = new Button { Text = "Move Up", Location = new Point(6, 93), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnMoveDown = new Button { Text = "Move Down", Location = new Point(6, 122), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };

            var noteLabel = new Label
            {
                Text = "Double-click to edit an entry",
                Location = new Point(6, 153),
                Size = new Size(75, 26),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.Gray
            };

            panel.Controls.Add(label);
            panel.Controls.Add(lstURLs);
            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnMoveUp);
            panel.Controls.Add(btnMoveDown);
            panel.Controls.Add(noteLabel);

            // 説明ラベル（Browser Chooser 2互換）
            var label7 = new Label
            {
                Text = "Auto-URLs are processed in order that they are displayed, top to bottom. Case sensitive.",
                Location = new Point(250, 206),
                Size = new Size(421, 13),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            panel.Controls.Add(label7);

            // Auto URLsの選択変更時の処理
            lstURLs.SelectedIndexChanged += (s, e) =>
            {
                bool hasSelection = lstURLs.SelectedItems.Count > 0;
                btnEdit.Enabled = hasSelection;
                btnDelete.Enabled = hasSelection;
                btnMoveUp.Enabled = hasSelection;
                btnMoveDown.Enabled = hasSelection;
                noteLabel.Visible = hasSelection;
                
                // 変更フラグを設定
                if (hasSelection)
                {
                    _isModified = true;
                }
            };
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プロトコルパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateProtocolsPanel()
        {
            var tabPage = new TabPage("Protocols");
            tabPage.Name = "tabProtocols";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Protocol Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // ListView for Protocols（Browser Chooser 2互換）
            var lstProtocols = new ListView
            {
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            lstProtocols.Columns.Add("Name", 109);
            lstProtocols.Columns.Add("Header", 124);

            // Buttons（Browser Chooser 2互換）
            var btnAdd = new Button { Text = "Add", Location = new Point(6, 6), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnEdit = new Button { Text = "Edit", Location = new Point(6, 35), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnDelete = new Button { Text = "Delete", Location = new Point(6, 64), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnOpenDefault = new Button { Text = "Select Default App", Location = new Point(6, 93), Size = new Size(75, 36), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };

            var noteLabel = new Label
            {
                Text = "Double-click to edit an entry",
                Location = new Point(6, 153),
                Size = new Size(75, 26),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.Gray
            };

            panel.Controls.Add(label);
            panel.Controls.Add(lstProtocols);
            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnOpenDefault);
            panel.Controls.Add(noteLabel);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// カテゴリパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateCategoriesPanel()
        {
            var tabPage = new TabPage("Categories");
            tabPage.Name = "tabCategories";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // ListView for Categories（Browser Chooser 2互換）
            var lstCategories = new ListView
            {
                Location = new Point(3, 6),
                Size = new Size(836, 218),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            lstCategories.Columns.Add("Name", 425);

            // カテゴリタブの選択時の処理
            tabPage.Enter += TabCategories_Enter;

            panel.Controls.Add(lstCategories);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// カテゴリタブ選択時の処理（Browser Chooser 2互換）
        /// </summary>
        private void TabCategories_Enter(object? sender, EventArgs e)
        {
            if (sender is TabPage tabPage)
            {
                var lstCategories = tabPage.Controls.OfType<ListView>().FirstOrDefault();
                if (lstCategories != null)
                {
                    // カテゴリリストを更新
                    lstCategories.Items.Clear();

                    var categories = new List<string>();
                    foreach (var browser in _mBrowser.Values)
                    {
                        if (!categories.Contains(browser.Category))
                        {
                            categories.Add(browser.Category);
                            lstCategories.Items.Add(browser.Category);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ファイルタイプパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateFileTypesPanel()
        {
            var tabPage = new TabPage("File Types");
            tabPage.Name = "tabFileTypes";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "File Type Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // ListView for File Types（Browser Chooser 2互換）
            var lstFileTypes = new ListView
            {
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            lstFileTypes.Columns.Add("Name", 109);
            lstFileTypes.Columns.Add("Extention", 74);

            // Buttons（Browser Chooser 2互換）
            var btnAdd = new Button { Text = "Add", Location = new Point(6, 6), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnEdit = new Button { Text = "Edit", Location = new Point(6, 35), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnDelete = new Button { Text = "Delete", Location = new Point(6, 64), Size = new Size(75, 23), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnOpenDefault = new Button { Text = "Select Default App", Location = new Point(6, 93), Size = new Size(75, 36), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };

            var noteLabel = new Label
            {
                Text = "Double-click to edit an entry",
                Location = new Point(6, 153),
                Size = new Size(75, 26),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.Gray
            };

            panel.Controls.Add(label);
            panel.Controls.Add(lstFileTypes);
            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnOpenDefault);
            panel.Controls.Add(noteLabel);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 表示パネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateDisplayPanel()
        {
            var tabPage = new TabPage("Display");
            tabPage.Name = "tabDisplay";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // チェックボックス群（Browser Chooser 2互換）
            var chkShowURLs = new CheckBox
            {
                Text = "Show URLs in User Interface",
                Location = new Point(6, 3),
                AutoSize = true,
                Checked = _settings.ShowURLs
            };

            var chkRevealShortURLs = new CheckBox
            {
                Text = "Reveal Shortened URLs",
                Location = new Point(6, 26),
                AutoSize = true,
                Checked = _settings.RevealShortURLs
            };

            var chkAllowStayOpen = new CheckBox
            {
                Text = "Allow interface to stay open",
                Location = new Point(6, 49),
                AutoSize = true,
                Checked = _settings.AllowStayOpen
            };

            var chkUseAreo = new CheckBox
            {
                Text = "Use AERO (when available)",
                Location = new Point(276, 26),
                AutoSize = true,
                Checked = _settings.UseAero
            };

            var chkUseAccessibleRendering = new CheckBox
            {
                Text = "Use Accessible Rendering",
                Location = new Point(276, 3),
                AutoSize = true,
                Checked = _settings.UseAccessibleRendering
            };

            // ボタン群（Browser Chooser 2互換）
            var cmdAccessiblitySettings = new Button
            {
                Text = "Focus Settings",
                Location = new Point(286, 45),
                Size = new Size(134, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cmdAccessiblitySettings.Click += (s, e) => OpenAccessibilitySettings();

            var cmdChangeBackgroundColor = new Button
            {
                Text = "Change Background Color",
                Location = new Point(131, 126),
                Size = new Size(149, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cmdChangeBackgroundColor.Click += (s, e) => ChangeBackgroundColor();

            var cmdTransparentBackground = new Button
            {
                Text = "Transparent Background",
                Location = new Point(286, 126),
                Size = new Size(149, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Visible = false
            };
            cmdTransparentBackground.Click += (s, e) => SetTransparentBackground();

            // 背景色プレビュー（Browser Chooser 2互換）
            var pbBackgroundColor = new PictureBox
            {
                Location = new Point(100, 126),
                Size = new Size(25, 23),
                BackColor = _settings.BackgroundColorValue
            };

            // ラベル群（Browser Chooser 2互換）
            var label13 = new Label
            {
                Text = "Background Color",
                Location = new Point(2, 131),
                AutoSize = true
            };

            var label5 = new Label
            {
                Text = "Message on main screen :",
                Location = new Point(2, 74),
                AutoSize = true
            };

            var label6 = new Label
            {
                Text = "Separator (Text between message and URL) :",
                Location = new Point(2, 99),
                AutoSize = true
            };

            // テキストボックス群（Browser Chooser 2互換）
            var txtMessage = new TextBox
            {
                Text = _settings.DefaultMessage,
                Location = new Point(158, 74),
                Size = new Size(537, 20),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var txtSeperator = new TextBox
            {
                Text = _settings.Separator,
                Location = new Point(232, 99),
                Size = new Size(463, 20),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            panel.Controls.Add(chkShowURLs);
            panel.Controls.Add(chkRevealShortURLs);
            panel.Controls.Add(chkAllowStayOpen);
            panel.Controls.Add(chkUseAreo);
            panel.Controls.Add(chkUseAccessibleRendering);
            panel.Controls.Add(cmdAccessiblitySettings);
            panel.Controls.Add(cmdChangeBackgroundColor);
            panel.Controls.Add(cmdTransparentBackground);
            panel.Controls.Add(pbBackgroundColor);
            panel.Controls.Add(label13);
            panel.Controls.Add(label5);
            panel.Controls.Add(label6);
            panel.Controls.Add(txtMessage);
            panel.Controls.Add(txtSeperator);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// グリッドパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateGridPanel()
        {
            var tabPage = new TabPage("Grid");
            tabPage.Name = "tabGrid";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // GroupBox1: グリッドのアイコン数設定
            var groupBox1 = new GroupBox
            {
                Text = "# of icons in grid",
                Location = new Point(6, 6),
                Size = new Size(263, 45)
            };

            var label2 = new Label
            {
                Text = "Columns:",
                Location = new Point(6, 23),
                AutoSize = true
            };

            var nudWidth = new NumericUpDown
            {
                Location = new Point(62, 20),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 10,
                Value = _settings.GridWidth
            };

            var label3 = new Label
            {
                Text = "Rows:",
                Location = new Point(149, 23),
                AutoSize = true
            };

            var nudHeight = new NumericUpDown
            {
                Location = new Point(196, 21),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 10,
                Value = _settings.GridHeight
            };

            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(nudWidth);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(nudHeight);

            // GroupBox2: アイコンサイズ設定
            var groupBox2 = new GroupBox
            {
                Text = "Size of icons in grid",
                Location = new Point(6, 57),
                Size = new Size(430, 47)
            };

            var label9 = new Label
            {
                Text = "Width :",
                Location = new Point(6, 20),
                AutoSize = true
            };

            var nudIconSizeWidth = new NumericUpDown
            {
                Location = new Point(62, 18),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 1000,
                Value = _settings.IconWidth
            };

            var label10 = new Label
            {
                Text = "Height :",
                Location = new Point(149, 20),
                AutoSize = true
            };

            var nudIconSizeHeight = new NumericUpDown
            {
                Location = new Point(196, 18),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 1000,
                Value = _settings.IconHeight
            };

            var label14 = new Label
            {
                Text = "Icon Scale :",
                Location = new Point(292, 20),
                AutoSize = true
            };

            var nudIconScale = new NumericUpDown
            {
                Location = new Point(358, 18),
                Size = new Size(60, 20),
                DecimalPlaces = 1,
                Increment = 0.1m,
                Minimum = 0.1m,
                Maximum = 5.0m,
                Value = (decimal)_settings.IconScale
            };

            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(nudIconSizeWidth);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(nudIconSizeHeight);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(nudIconScale);

            // GroupBox3: アイコン間隔設定
            var groupBox3 = new GroupBox
            {
                Text = "Gap Size between icons in grid (can be negative)",
                Location = new Point(6, 110),
                Size = new Size(263, 47)
            };

            var label12 = new Label
            {
                Text = "Width :",
                Location = new Point(6, 20),
                AutoSize = true
            };

            var nudIconGapWidth = new NumericUpDown
            {
                Location = new Point(62, 18),
                Size = new Size(60, 20),
                Minimum = -100,
                Maximum = 100,
                Value = _settings.IconGapWidth
            };

            var label11 = new Label
            {
                Text = "Height :",
                Location = new Point(150, 20),
                AutoSize = true
            };

            var nudIconGapHeight = new NumericUpDown
            {
                Location = new Point(196, 18),
                Size = new Size(60, 20),
                Minimum = -100,
                Maximum = 100,
                Value = _settings.IconGapHeight
            };

            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(nudIconGapWidth);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(nudIconGapHeight);

            panel.Controls.Add(groupBox1);
            panel.Controls.Add(groupBox2);
            panel.Controls.Add(groupBox3);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プライバシーパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreatePrivacyPanel()
        {
            var tabPage = new TabPage("Privacy");
            tabPage.Name = "tabPrivacy";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // User Agent設定（Browser Chooser 2互換）
            var label8 = new Label
            {
                Text = "User Agent To Send :",
                Location = new Point(3, 6),
                AutoSize = true
            };

            var txtUserAgent = new TextBox
            {
                Text = _settings.UserAgent,
                Location = new Point(159, 3),
                Size = new Size(277, 20),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // ダウンロード検出ファイル設定（Browser Chooser 2互換）
            var chkDownloadDetectionfile = new CheckBox
            {
                Text = "Download Detection file from BrowserChooser2.com",
                Location = new Point(6, 29),
                AutoSize = true,
                Checked = _settings.DownloadDetectionFile
            };

            panel.Controls.Add(label8);
            panel.Controls.Add(txtUserAgent);
            panel.Controls.Add(chkDownloadDetectionfile);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// スタートアップパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateStartupPanel()
        {
            var tabPage = new TabPage("Startup");
            tabPage.Name = "tabStartup";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // ポータブルモード設定（Browser Chooser 2互換）
            var chkPortableMode = new CheckBox
            {
                Text = "Portable Mode",
                Location = new Point(6, 6),
                AutoSize = true,
                Checked = _settings.PortableMode
            };

            // 自動更新チェック設定（Browser Chooser 2互換）
            var chkAutoCheckUpdate = new CheckBox
            {
                Text = "Automatically Check for Updates",
                Location = new Point(6, 28),
                AutoSize = true,
                Checked = _settings.AutomaticUpdates
            };

            // 更新チェックボタン（Browser Chooser 2互換）
            var cmdCheckForUpdate = new Button
            {
                Text = "Check Now",
                Location = new Point(185, 24),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 自動読み込み遅延設定（Browser Chooser 2互換）
            var label1 = new Label
            {
                Text = "Default Delay before Auto-Load :",
                Location = new Point(6, 54),
                AutoSize = true
            };

            var nudDelayBeforeAutoload = new NumericUpDown
            {
                Location = new Point(200, 52),
                Size = new Size(60, 20),
                Minimum = 0,
                Maximum = 60,
                Value = _settings.DefaultDelay
            };

            // 高度な画面設定（Browser Chooser 2互換）
            var chkAdvanced = new CheckBox
            {
                Text = "Use Advanced Screens",
                Location = new Point(3, 228),
                AutoSize = true,
                Checked = _settings.AdvancedScreens,
                Visible = false
            };

            // GroupBox4: メイン画面開始位置設定（Browser Chooser 2互換）
            var groupBox4 = new GroupBox
            {
                Text = "Main Screen Starting Position",
                Location = new Point(6, 78),
                Size = new Size(430, 47)
            };

            var label17 = new Label
            {
                Text = "Starting Position",
                Location = new Point(6, 20),
                AutoSize = true
            };

            var cmbStartingPosition = new ComboBox
            {
                Location = new Point(95, 17),
                Size = new Size(142, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cmbStartingPosition.Items.AddRange(new object[] {
                "Center Screen", "Offset Center", "XY", "Top Left", "Top Right", 
                "Bottom Left", "Bottom Right", "Offset Top Left", "Offset Top Right",
                "Offset Bottom Left", "Offset Bottom Right"
            });
            cmbStartingPosition.SelectedIndex = _settings.StartingPosition;

            var label16 = new Label
            {
                Text = "X :",
                Location = new Point(243, 20),
                AutoSize = true
            };

            var nudXOffset = new NumericUpDown
            {
                Location = new Point(269, 18),
                Size = new Size(60, 20),
                Increment = 10,
                Minimum = -10000,
                Maximum = 10000,
                Value = _settings.OffsetX
            };

            var label15 = new Label
            {
                Text = "Y:",
                Location = new Point(335, 20),
                AutoSize = true
            };

            var nudYOffset = new NumericUpDown
            {
                Location = new Point(358, 18),
                Size = new Size(60, 20),
                Increment = 10,
                Minimum = -10000,
                Maximum = 10000,
                Value = _settings.OffsetY
            };

            groupBox4.Controls.Add(label17);
            groupBox4.Controls.Add(cmbStartingPosition);
            groupBox4.Controls.Add(label15);
            groupBox4.Controls.Add(label16);
            groupBox4.Controls.Add(nudYOffset);
            groupBox4.Controls.Add(nudXOffset);

            panel.Controls.Add(chkPortableMode);
            panel.Controls.Add(chkAutoCheckUpdate);
            panel.Controls.Add(cmdCheckForUpdate);
            panel.Controls.Add(label1);
            panel.Controls.Add(nudDelayBeforeAutoload);
            panel.Controls.Add(chkAdvanced);
            panel.Controls.Add(groupBox4);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 一般設定パネルの作成
        /// </summary>
        private TabPage CreateGeneralPanel()
        {
            var tabPage = new TabPage("General");
            tabPage.Name = "tabGeneral";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "General Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// その他パネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateOthersPanel()
        {
            var tabPage = new TabPage("Others");
            tabPage.Name = "tabOthers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // オプションショートカット設定（Browser Chooser 2互換）
            var label4 = new Label
            {
                Text = "Hotkey to open Options dialog :",
                Location = new Point(2, 6),
                AutoSize = true
            };

            var txtOptionsShortcut = new TextBox
            {
                Text = _settings.OptionsShortcut.ToString(),
                Location = new Point(158, 3),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                MaxLength = 1
            };

            // GroupBox5: 正規化設定（Browser Chooser 2互換）
            var groupBox5 = new GroupBox
            {
                Text = "Append To / Canonicalize Domain",
                Location = new Point(3, 29),
                Size = new Size(430, 48)
            };

            var chkCanonicalize = new CheckBox
            {
                Text = "Append To / Canonicalize Domain",
                Location = new Point(9, 0),
                AutoSize = true,
                Checked = _settings.Canonicalize,
                BackColor = SystemColors.Control
            };

            var label19 = new Label
            {
                Text = "URI Part to Add (omit first dot)",
                Location = new Point(6, 22),
                AutoSize = true
            };

            var txtCanonicalizeAppend = new TextBox
            {
                Text = _settings.CanonicalizeAppendedText,
                Location = new Point(159, 19),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            groupBox5.Controls.Add(chkCanonicalize);
            groupBox5.Controls.Add(label19);
            groupBox5.Controls.Add(txtCanonicalizeAppend);

            // ログ設定（Browser Chooser 2互換）
            var chkLog = new CheckBox
            {
                Text = "Enable Logging",
                Location = new Point(5, 83),
                AutoSize = true,
                Checked = _settings.EnableLogging,
                BackColor = Color.Transparent
            };

            // DLL抽出設定（Browser Chooser 2互換）
            var chkExtract = new CheckBox
            {
                Text = "Extract embded DLLs",
                Location = new Point(5, 106),
                AutoSize = true,
                Checked = _settings.ExtractDLLs,
                BackColor = Color.Transparent
            };

            panel.Controls.Add(label4);
            panel.Controls.Add(txtOptionsShortcut);
            panel.Controls.Add(groupBox5);
            panel.Controls.Add(chkLog);
            panel.Controls.Add(chkExtract);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// アクセシビリティパネルの作成
        /// </summary>
        private TabPage CreateAccessibilityPanel()
        {
            var tabPage = new TabPage("Accessibility");
            tabPage.Name = "tabAccessibility";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Accessibility Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var accessibilityButton = new Button
            {
                Text = "Open Accessibility Settings",
                Location = new Point(10, 40),
                Size = new Size(200, 30)
            };
            accessibilityButton.Click += (s, e) =>
            {
                OpenAccessibilitySettings();
            };

            panel.Controls.Add(label);
            panel.Controls.Add(accessibilityButton);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// デフォルトブラウザパネルの作成（Browser Chooser 2互換）
        /// </summary>
        private TabPage CreateDefaultBrowserPanel()
        {
            var tabPage = new TabPage("Windows Default");
            tabPage.Name = "tabDefaultBrowser";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(3)
            };

            // スコープ設定（Browser Chooser 2互換）
            var grpScope = new GroupBox
            {
                Text = "Scope",
                Location = new Point(3, 3),
                Size = new Size(274, 45)
            };

            var rbScopeUser = new RadioButton
            {
                Text = "User",
                Location = new Point(6, 19),
                AutoSize = true,
                Checked = true
            };

            var rbScopeSystem = new RadioButton
            {
                Text = "System (Administrator access required)",
                Location = new Point(59, 19),
                AutoSize = true
            };

            grpScope.Controls.Add(rbScopeUser);
            grpScope.Controls.Add(rbScopeSystem);

            // デフォルトブラウザ設定ボタン（Browser Chooser 2互換）
            var cmdAddToDefault = new Button
            {
                Text = "Add to Default Programs",
                Location = new Point(9, 54),
                Size = new Size(202, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var cmdMakeDefault = new Button
            {
                Text = "Make Default",
                Location = new Point(9, 83),
                Size = new Size(202, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 起動時にデフォルトをチェック設定（Browser Chooser 2互換）
            var chkCheckDefaultOnLaunch = new CheckBox
            {
                Text = "Check on launch (Vista/7 Only)",
                Location = new Point(9, 112),
                AutoSize = true,
                Checked = _settings.CheckDefaultOnLaunch
            };

            var cmdRemoveFromDefaultSettings = new Button
            {
                Text = "Remove from Default Programs",
                Location = new Point(9, 135),
                Size = new Size(202, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 警告メッセージ（Browser Chooser 2互換）
            var lblWarnWin10 = new Label
            {
                Text = "Note for users of Windows 10+:",
                Location = new Point(218, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0),
                Visible = false
            };

            var txtWarnWin10 = new TextBox
            {
                Text = "Microsoft no longer allows a program to automatically gain default status of either a a protocol (Browser) or Filetypes.  You must manully assign them via the Defaults App Applet.",
                Location = new Point(224, 81),
                Size = new Size(178, 70),
                Multiline = true,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Visible = false
            };

            panel.Controls.Add(grpScope);
            panel.Controls.Add(cmdAddToDefault);
            panel.Controls.Add(cmdMakeDefault);
            panel.Controls.Add(chkCheckDefaultOnLaunch);
            panel.Controls.Add(cmdRemoveFromDefaultSettings);
            panel.Controls.Add(lblWarnWin10);
            panel.Controls.Add(txtWarnWin10);
            tabPage.Controls.Add(panel);
            return tabPage;
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
                        item.SubItems.Add(clonedBrowser.Target);
                        item.SubItems.Add(clonedBrowser.Arguments);
                        item.SubItems.Add(clonedBrowser.PosY.ToString());
                        item.SubItems.Add(clonedBrowser.PosX.ToString());
                        item.SubItems.Add(clonedBrowser.Hotkey.ToString());
                        item.SubItems.Add(GetBrowserProtocolsAndFileTypes(clonedBrowser));
                    }
                    
                    // ImageListにアイコンを追加
                    if (_imBrowserIcons != null)
                    {
                        _imBrowserIcons.Images.Add(ImageUtilities.GetImage(clonedBrowser, false));
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
            var hiddenLabel = Controls.Find("lblHiddenBrowserGuid", true).FirstOrDefault() as Label;
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
                var hiddenGuidLabel = Controls.Find("lblHiddenBrowserGuid", true).FirstOrDefault() as Label;
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
                    var result = MessageBox.Show("You have changed the accepted Protocols or Filetypes. Do you want to become default for these as well?", 
                        "Become default", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        AddToDefault();
                        MakeDefault();
                    }
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

        #region Event Handlers
        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (_isModified)
            {
                SaveSettings();
            }
        }

        private void AddBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var addEditForm = new AddEditBrowserForm();
                if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens, 
                    new Point(_settings.GridWidth, _settings.GridHeight)))
                {
                    var newBrowser = addEditForm.GetData();
                    _mBrowser.Add(_mLastBrowserID + 1, newBrowser);
                    
                    // ListViewにアイテムを追加
                    var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                    if (listView != null)
                    {
                        var item = listView.Items.Add(newBrowser.Name);
                        item.Tag = _mLastBrowserID + 1;
                        item.SubItems.Add(newBrowser.Target);
                        item.SubItems.Add(newBrowser.Arguments);
                        item.SubItems.Add(newBrowser.PosY.ToString());
                        item.SubItems.Add(newBrowser.PosX.ToString());
                        item.SubItems.Add(newBrowser.Hotkey.ToString());
                        item.SubItems.Add(GetBrowserProtocolsAndFileTypes(newBrowser));
                    }
                    
                    // ImageListにアイコンを追加
                    if (_imBrowserIcons != null)
                    {
                        _imBrowserIcons.Images.Add(ImageUtilities.GetImage(newBrowser, false));
                    }
                    
                    _mLastBrowserID++;
                    _isModified = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.AddBrowser_Click", "ブラウザ追加エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ追加に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
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
                        selectedItem.SubItems[1].Text = updatedBrowser.Target;
                        selectedItem.SubItems[2].Text = updatedBrowser.Arguments;
                        selectedItem.SubItems[3].Text = updatedBrowser.PosY.ToString();
                        selectedItem.SubItems[4].Text = updatedBrowser.PosX.ToString();
                        selectedItem.SubItems[5].Text = updatedBrowser.Hotkey.ToString();
                        selectedItem.SubItems[6].Text = GetBrowserProtocolsAndFileTypes(updatedBrowser);
                        
                        // ImageListアイコンの更新
                        if (_imBrowserIcons != null && selectedItem.Index < _imBrowserIcons.Images.Count)
                        {
                            _imBrowserIcons.Images[selectedItem.Index] = ImageUtilities.GetImage(updatedBrowser, false);
                        }
                        
                        _isModified = true;
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
                Logger.LogError("OptionsForm.EditBrowser_Click", "ブラウザ編集エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ編集に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
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
                            _isModified = true;
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
                Logger.LogError("OptionsForm.DeleteBrowser_Click", "ブラウザ削除エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ削除に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloneBrowser_Click(object? sender, EventArgs e)
        {
            try
            {
                var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
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
                        
                        _mBrowser.Add(_mLastBrowserID + 1, clonedBrowser);
                        
                        // ListViewにアイテムを追加
                        if (listView != null)
                        {
                            var item = listView.Items.Add(clonedBrowser.Name);
                            item.Tag = _mLastBrowserID + 1;
                            item.SubItems.Add(clonedBrowser.Target);
                            item.SubItems.Add(clonedBrowser.Arguments);
                            item.SubItems.Add(clonedBrowser.PosY.ToString());
                            item.SubItems.Add(clonedBrowser.PosX.ToString());
                            item.SubItems.Add(clonedBrowser.Hotkey.ToString());
                            item.SubItems.Add(GetBrowserProtocolsAndFileTypes(clonedBrowser));
                        }
                        
                        // ImageListにアイコンを追加
                        if (_imBrowserIcons != null)
                        {
                            _imBrowserIcons.Images.Add(ImageUtilities.GetImage(clonedBrowser, false));
                        }
                        
                        _mLastBrowserID++;
                        _isModified = true;
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
                Logger.LogError("OptionsForm.CloneBrowser_Click", "ブラウザ複製エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ複製に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DetectBrowsers_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("OptionsForm.DetectBrowsers_Click", "ブラウザ検出を開始");
            try
            {
                // ブラウザ検出を実行
                var detectedBrowsers = DetectedBrowsers.DoBrowserDetection();
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

                if (missingBrowsers.Count == 0)
                {
                    MessageBox.Show("Did not detect any new browsers.", "Detect Browsers", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Detected {missingBrowsers.Count} new browsers.", "Detect Browsers", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 使用済み位置のリストを作成
                    var usedPositions = new SortedList<string, object?>();
                    foreach (var browser in _mBrowser.Values)
                    {
                        usedPositions.Add($"{browser.PosX}{browser.PosY}", null);
                    }

                    // 各ブラウザに位置を割り当て
                    foreach (var browser in missingBrowsers)
                    {
                        bool found = false;
                        
                        // 空き位置を探す
                        for (int y = 1; y <= _settings.GridHeight; y++)
                        {
                            for (int x = 1; x <= _settings.GridWidth; x++)
                            {
                                if (!usedPositions.ContainsKey($"{x}{y}"))
                                {
                                    browser.PosX = x;
                                    browser.PosY = y;
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }

                        if (!found)
                        {
                            // 新しい行を追加
                            _settings.GridHeight++;
                            browser.PosX = 1;
                            browser.PosY = _settings.GridHeight;
                        }

                        // 位置を使用済みとしてマーク
                        usedPositions.Add($"{browser.PosX}{browser.PosY}", null);

                        // 新しいGUIDを生成
                        browser.Guid = Guid.NewGuid();

                        // デフォルトカテゴリのプロトコル・ファイルタイプに追加
                        foreach (var protocol in _mProtocols.Values)
                        {
                            if (protocol.DefaultCategories.Contains("Default"))
                            {
                                protocol.SupportingBrowsers.Add(browser.Guid);
                            }
                        }

                        foreach (var fileType in _mFileTypes.Values)
                        {
                            if (fileType.DefaultCategories.Contains("Default"))
                            {
                                fileType.SupportingBrowsers.Add(browser.Guid);
                            }
                        }

                        // ブラウザリストに追加
                        _mBrowser.Add(_mLastBrowserID + 1, browser);
                        _mLastBrowserID++;

                        // ListViewにアイテムを追加
                        var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                        if (listView != null)
                        {
                            var item = listView.Items.Add(browser.Name);
                            item.Tag = _mLastBrowserID;
                            
                            // デフォルトブラウザのチェックマーク
                            if (_settings.DefaultBrowserGuid == browser.Guid)
                            {
                                item.SubItems.Add("✔");
                            }
                            else
                            {
                                item.SubItems.Add("");
                            }
                            
                            // 位置情報
                            item.SubItems.Add(browser.PosY.ToString());
                            item.SubItems.Add(browser.PosX.ToString());
                            item.SubItems.Add(browser.Hotkey.ToString());
                            
                            // プロトコル・ファイルタイプ情報
                            item.SubItems.Add(GetBrowserProtocolsAndFileTypes(browser));
                        }

                        // ImageListにアイコンを追加
                        if (_imBrowserIcons != null)
                        {
                            _imBrowserIcons.Images.Add(ImageUtilities.GetImage(browser, false));
                        }
                    }

                    _isModified = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.DetectBrowsers_Click", "ブラウザ検出エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ検出に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// プロトコル・ファイルタイプ管理機能（Browser Chooser 2互換）
        /// </summary>
        private void SetupProtocolFileTypeManagement()
        {
            // プロトコル追加ボタン
            var addProtocolButton = Controls.Find("cmdAddProtocol", true).FirstOrDefault() as Button;
            if (addProtocolButton != null)
            {
                addProtocolButton.Click += (s, e) =>
                {
                    var addEditForm = new AddEditProtocolForm();
                    if (addEditForm.AddProtocol(_mBrowser))
                    {
                        var newProtocol = addEditForm.GetData();
                        _mProtocols.Add(_mLastProtocolID + 1, newProtocol);
                        
                        // ListViewにアイテムを追加
                        var listView = Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                        if (listView != null)
                        {
                            var item = listView.Items.Add(newProtocol.ProtocolName);
                            item.Tag = _mLastProtocolID + 1;
                            item.SubItems.Add(newProtocol.Header);
                        }
                        
                        _mLastProtocolID++;
                        _mProtocolsAreDirty = true;
                        _isModified = true;
                    }
                };
            }

            // プロトコル編集ボタン
            var editProtocolButton = Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
            if (editProtocolButton != null)
            {
                editProtocolButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex == -1 || !_mProtocols.ContainsKey(selectedIndex)) return;
                        
                        var addEditForm = new AddEditProtocolForm();
                        if (addEditForm.EditProtocol(_mProtocols[selectedIndex], _mBrowser))
                        {
                            var updatedProtocol = addEditForm.GetData();
                            _mProtocols[selectedIndex] = updatedProtocol;
                            
                            // ListViewアイテムの更新
                            var selectedItem = listView.SelectedItems[0];
                            selectedItem.Text = updatedProtocol.ProtocolName;
                            selectedItem.SubItems[1].Text = updatedProtocol.Header;
                            
                            _mProtocolsAreDirty = true;
                            _isModified = true;
                        }
                    }
                };
            }

            // プロトコル削除ボタン
            var deleteProtocolButton = Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;
            if (deleteProtocolButton != null)
            {
                deleteProtocolButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var protocolName = listView.SelectedItems[0].Text;
                        var result = MessageBox.Show($"Are you sure you want delete the {protocolName} entry?", 
                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                            if (selectedIndex != -1)
                            {
                                _mProtocols.Remove(selectedIndex);
                                listView.Items.Remove(listView.SelectedItems[0]);
                                _mProtocolsAreDirty = true;
                                _isModified = true;
                            }
                        }
                    }
                };
            }

            // ファイルタイプ追加ボタン
            var addFileTypeButton = Controls.Find("cmdAddFileType", true).FirstOrDefault() as Button;
            if (addFileTypeButton != null)
            {
                addFileTypeButton.Click += (s, e) =>
                {
                    var addEditForm = new AddEditFileTypeForm();
                    if (addEditForm.AddFileType(_mBrowser))
                    {
                        var newFileType = addEditForm.GetData();
                        _mFileTypes.Add(_mLastFileTypeID + 1, newFileType);
                        
                        // ListViewにアイテムを追加
                        var listView = Controls.Find("lstFiletypes", true).FirstOrDefault() as ListView;
                        if (listView != null)
                        {
                            var item = listView.Items.Add(newFileType.FiletypeName);
                            item.Tag = _mLastFileTypeID + 1;
                            item.SubItems.Add(newFileType.Extention);
                        }
                        
                        _mLastFileTypeID++;
                        _mFileTypesAreDirty = true;
                        _isModified = true;
                    }
                };
            }

            // ファイルタイプ編集ボタン
            var editFileTypeButton = Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
            if (editFileTypeButton != null)
            {
                editFileTypeButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstFiletypes", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex == -1 || !_mFileTypes.ContainsKey(selectedIndex)) return;
                        
                        var addEditForm = new AddEditFileTypeForm();
                        if (addEditForm.EditFileType(_mFileTypes[selectedIndex], _mBrowser))
                        {
                            var updatedFileType = addEditForm.GetData();
                            _mFileTypes[selectedIndex] = updatedFileType;
                            
                            // ListViewアイテムの更新
                            var selectedItem = listView.SelectedItems[0];
                            selectedItem.Text = updatedFileType.FiletypeName;
                            selectedItem.SubItems[1].Text = updatedFileType.Extention;
                            
                            _mFileTypesAreDirty = true;
                            _isModified = true;
                        }
                    }
                };
            }

            // ファイルタイプ削除ボタン
            var deleteFileTypeButton = Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;
            if (deleteFileTypeButton != null)
            {
                deleteFileTypeButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstFiletypes", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var fileTypeName = listView.SelectedItems[0].Text;
                        var result = MessageBox.Show($"Are you sure you want delete the {fileTypeName} entry?", 
                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                            if (selectedIndex != -1)
                            {
                                _mFileTypes.Remove(selectedIndex);
                                listView.Items.Remove(listView.SelectedItems[0]);
                                _mFileTypesAreDirty = true;
                                _isModified = true;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// URL管理機能（Browser Chooser 2互換）
        /// </summary>
        private void SetupURLManagement()
        {
            // URL追加ボタン
            var addURLButton = Controls.Find("cmdAddAutoURL", true).FirstOrDefault() as Button;
            if (addURLButton != null)
            {
                addURLButton.Click += (s, e) =>
                {
                    var addEditForm = new AddEditURLForm();
                    if (addEditForm.AddURL(_mBrowser))
                    {
                        var newURL = addEditForm.GetData();
                        _mURLs.Add(_mLastURLID + 1, newURL);
                        
                        // ListViewにアイテムを追加
                        var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                        if (listView != null)
                        {
                            var item = listView.Items.Add(newURL.URLValue);
                            item.Tag = _mLastURLID + 1;
                            
                            // ブラウザ名
                            if (newURL.BrowserGuid != Guid.Empty)
                            {
                                var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == newURL.BrowserGuid);
                                item.SubItems.Add(browser?.Name ?? "Unknown");
                            }
                            else
                            {
                                item.SubItems.Add("Default");
                            }
                            
                            // 遅延時間
                            if (newURL.DelayTime < 0)
                            {
                                item.SubItems.Add("Default");
                            }
                            else
                            {
                                item.SubItems.Add(newURL.DelayTime.ToString());
                            }
                        }
                        
                        _mLastURLID++;
                        _isModified = true;
                    }
                };
            }

            // URL編集ボタン
            var editURLButton = Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
            if (editURLButton != null)
            {
                editURLButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex == -1 || !_mURLs.ContainsKey(selectedIndex)) return;
                        
                        var addEditForm = new AddEditURLForm();
                        if (addEditForm.EditURL(_mURLs[selectedIndex], _mBrowser))
                        {
                            var updatedURL = addEditForm.GetData();
                            _mURLs[selectedIndex] = updatedURL;
                            
                            // ListViewアイテムの更新
                            var selectedItem = listView.SelectedItems[0];
                            selectedItem.Text = updatedURL.URLValue;
                            
                            // ブラウザ名
                            if (updatedURL.BrowserGuid != Guid.Empty)
                            {
                                var browser = _mBrowser.Values.FirstOrDefault(b => b.Guid == updatedURL.BrowserGuid);
                                selectedItem.SubItems[1].Text = browser?.Name ?? "Unknown";
                            }
                            else
                            {
                                selectedItem.SubItems[1].Text = "Default";
                            }
                            
                            // 遅延時間
                            if (updatedURL.DelayTime < 0)
                            {
                                selectedItem.SubItems[2].Text = "Default";
                            }
                            else
                            {
                                selectedItem.SubItems[2].Text = updatedURL.DelayTime.ToString();
                            }
                            
                            _isModified = true;
                        }
                    }
                };
            }

            // URL削除ボタン
            var deleteURLButton = Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
            if (deleteURLButton != null)
            {
                deleteURLButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var urlName = listView.SelectedItems[0].Text;
                        var result = MessageBox.Show($"Are you sure you want delete the {urlName} entry?", 
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
                };
            }

            // URL上移動ボタン
            var moveUpURLButton = Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
            if (moveUpURLButton != null)
            {
                moveUpURLButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Index;
                        if (selectedIndex > 0)
                        {
                            var item = listView.SelectedItems[0];
                            listView.Items.RemoveAt(selectedIndex);
                            listView.Items.Insert(selectedIndex - 1, item);
                            listView.Items[selectedIndex - 1].Selected = true;
                            
                            // 内部リストを再構築
                            RebuildAutoURLs();
                        }
                    }
                };
            }

            // URL下移動ボタン
            var moveDownURLButton = Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;
            if (moveDownURLButton != null)
            {
                moveDownURLButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Index;
                        if (selectedIndex < listView.Items.Count - 1)
                        {
                            var item = listView.SelectedItems[0];
                            listView.Items.RemoveAt(selectedIndex);
                            listView.Items.Insert(selectedIndex + 1, item);
                            listView.Items[selectedIndex + 1].Selected = true;
                            
                            // 内部リストを再構築
                            RebuildAutoURLs();
                        }
                    }
                };
            }
        }

        #region 詳細なイベントハンドラー（Browser Chooser 2互換）

        /// <summary>
        /// ブラウザリストの選択変更イベント
        /// </summary>
        private void LstBrowsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
                var cloneButton = Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
                var defaultButton = Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (cloneButton != null) cloneButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;
                if (defaultButton != null) defaultButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = Controls.Find("lblDoubleClickBrowsersNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
                var cloneButton = Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
                var defaultButton = Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (cloneButton != null) cloneButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;
                if (defaultButton != null) defaultButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = Controls.Find("lblDoubleClickBrowsersNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// Auto URLsリストの選択変更イベント
        /// </summary>
        private void LstURLs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
                var moveUpButton = Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
                var moveDownButton = Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;
                if (moveUpButton != null) moveUpButton.Enabled = true;
                if (moveDownButton != null) moveDownButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
                var moveUpButton = Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
                var moveDownButton = Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;
                if (moveUpButton != null) moveUpButton.Enabled = false;
                if (moveDownButton != null) moveDownButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// プロトコルリストの選択変更イベント
        /// </summary>
        private void LstProtocols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        /// <summary>
        /// ファイルタイプリストの選択変更イベント
        /// </summary>
        private void LstFileTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedIndices.Count > 0)
            {
                // ボタンを有効化
                var editButton = Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = true;
                if (deleteButton != null) deleteButton.Enabled = true;

                // ダブルクリック注釈を表示
                var noteLabel = Controls.Find("lblDoubleClickFileTypesNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = true;
            }
            else
            {
                // ボタンを無効化
                var editButton = Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
                var deleteButton = Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;

                if (editButton != null) editButton.Enabled = false;
                if (deleteButton != null) deleteButton.Enabled = false;

                // ダブルクリック注釈を非表示
                var noteLabel = Controls.Find("lblDoubleClickFileTypesNote", true).FirstOrDefault() as Label;
                if (noteLabel != null) noteLabel.Visible = false;
            }
        }

        #endregion

        private void SetDefaultBrowser_Click(object? sender, EventArgs e)
        {
            // TODO: デフォルトブラウザ設定機能を実装
            MessageBox.Show("デフォルトブラウザ設定機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// デフォルトプログラムに追加（Browser Chooser 2互換）
        /// </summary>
        private void AddToDefault()
        {
            // TODO: デフォルトプログラムへの追加機能を実装
            MessageBox.Show("デフォルトプログラムへの追加機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// デフォルトブラウザに設定（Browser Chooser 2互換）
        /// </summary>
        private void MakeDefault()
        {
            // TODO: デフォルトブラウザ設定機能を実装
            MessageBox.Show("デフォルトブラウザ設定機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 個別のデフォルト設定（Browser Chooser 2互換）
        /// </summary>
        private void MakeDefaultSingle(string item, bool isProtocol)
        {
            // TODO: 個別のデフォルト設定機能を実装
            MessageBox.Show($"個別のデフォルト設定機能は未実装です: {item} ({isProtocol})", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region スコープ変更処理（Browser Chooser 2互換）

        /// <summary>
        /// スコープ変更時の処理
        /// </summary>
        private void RbScope_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is not RadioButton radioButton) return;

            var cmdAddToDefault = Controls.Find("cmdAddToDefault", true).FirstOrDefault() as Button;
            var cmdMakeDefault = Controls.Find("cmdMakeDefault", true).FirstOrDefault() as Button;

            if (radioButton.Name == "rbScopeUser" && radioButton.Checked)
            {
                // ユーザースコープの場合、盾アイコンを削除
                if (cmdAddToDefault != null)
                {
                    cmdAddToDefault.FlatStyle = FlatStyle.System;
                    WinAPIs.SendMessage(cmdAddToDefault.Handle, WinAPIs.BCM_SETSHIELD, 0, IntPtr.Zero);
                }
                if (cmdMakeDefault != null)
                {
                    cmdMakeDefault.FlatStyle = FlatStyle.System;
                    WinAPIs.SendMessage(cmdMakeDefault.Handle, WinAPIs.BCM_SETSHIELD, 0, IntPtr.Zero);
                }
            }
            else if (radioButton.Name == "rbScopeSystem" && radioButton.Checked)
            {
                // システムスコープの場合、盾アイコンを追加
                if (cmdAddToDefault != null)
                {
                    cmdAddToDefault.FlatStyle = FlatStyle.System;
                    WinAPIs.SendMessage(cmdAddToDefault.Handle, WinAPIs.BCM_SETSHIELD, 0, new IntPtr(1));
                }
                if (cmdMakeDefault != null)
                {
                    cmdMakeDefault.FlatStyle = FlatStyle.System;
                    WinAPIs.SendMessage(cmdMakeDefault.Handle, WinAPIs.BCM_SETSHIELD, 0, new IntPtr(1));
                }
            }
        }

        #endregion

        private void OpenAccessibilitySettings()
        {
            try
            {
                // アクセシビリティ設定ダイアログを表示
                var accessibilityForm = new AccessibilitySettingsForm();
                if (accessibilityForm.ShowDialog() == DialogResult.OK)
                {
                    // 設定を更新
                    _mFocusSettings.ShowFocus = accessibilityForm.ShowFocus;
                    _mFocusSettings.BoxColor = accessibilityForm.FocusBoxColor;
                    _mFocusSettings.BoxWidth = accessibilityForm.FocusBoxWidth;
                    _isModified = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.OpenAccessibilitySettings", "アクセシビリティ設定エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"アクセシビリティ設定に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 設定変更検知機能（Browser Chooser 2互換）

        /// <summary>
        /// 設定変更を検知してダーティフラグを設定
        /// </summary>
        private void DetectDirty(object sender, EventArgs e)
        {
            _isModified = true;
        }

        /// <summary>
        /// 正規化設定の変更時の処理
        /// </summary>
        private void ChkCanonicalize_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var txtCanonicalizeAppend = Controls.Find("txtCanonicalizeAppend", true).FirstOrDefault() as TextBox;
                if (txtCanonicalizeAppend != null)
                {
                    txtCanonicalizeAppend.Enabled = checkBox.Checked;
                }
            }
            _isModified = true;
        }

        /// <summary>
        /// ログ設定の変更時の処理
        /// </summary>
        private void ChkLog_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                // ログ設定を更新
                _settings.EnableLogging = checkBox.Checked;
            }
            _isModified = true;
        }



        /// <summary>
        /// 透明背景設定
        /// </summary>
        private void SetTransparentBackground()
        {
            var pbBackgroundColor = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
            if (pbBackgroundColor != null)
            {
                pbBackgroundColor.BackColor = Color.Transparent;
            }
            _isModified = true;
        }

        #endregion

        private void ChangeBackgroundColor()
        {
            try
            {
                var colorDialog = new ColorDialog();
                colorDialog.AnyColor = true;
                colorDialog.Color = _settings.BackgroundColorValue;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _settings.BackgroundColorValue = colorDialog.Color;
                    
                    var pbBackgroundColor = Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
                    if (pbBackgroundColor != null)
                    {
                        pbBackgroundColor.BackColor = colorDialog.Color;
                    }
                    _isModified = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.ChangeBackgroundColor", "背景色変更エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"背景色の変更に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void OpenHelp()
        {
            try
            {
                // Browser Chooser 2のヘルプページを開く
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://bitbucket.org/gmyx/browserchooser2/wiki/Home",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Help page cannot be reached!\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OptionsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !_isCanceled)
            {
                if (_isModified)
                {
                    var result = MessageBox.Show("Do you want to save your settings?", "Save?", 
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        SaveSettings();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        /// <summary>
        /// フォーム表示時の設定読み込み（Browser Chooser 2互換）
        /// </summary>
        private void OptionsForm_Shown(object? sender, EventArgs e)
        {
            try
            {
                // 設定の読み込み
                LoadSettings();
                
                // TreeViewの展開
                var treeSettings = Controls.OfType<TreeView>().FirstOrDefault();
                if (treeSettings != null)
                {
                    treeSettings.ExpandAll();
                }

                // 変更フラグのリセット
                _isModified = false;
                
                // ブラウザ管理機能の設定
                SetupBrowserManagement();
                
                // プロトコル・ファイルタイプ管理機能の設定
                SetupProtocolFileTypeManagement();
                
                // URL管理機能の設定
                SetupURLManagement();
                

                
                Logger.LogInfo("OptionsForm.OptionsForm_Shown", "設定読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.OptionsForm_Shown", "設定読み込みエラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"設定の読み込みに失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ブラウザのプロトコル・ファイルタイプを取得（Browser Chooser 2互換）
        /// </summary>
        private string GetBrowserProtocolsAndFileTypes(Browser browser)
        {
            var items = new List<string>();

            // プロトコルを追加
            foreach (var protocol in _mProtocols.Values)
            {
                if (protocol.SupportingBrowsers.Contains(browser.Guid))
                {
                    items.Add(protocol.ProtocolName);
                }
            }

            // ファイルタイプを追加
            foreach (var fileType in _mFileTypes.Values)
            {
                if (fileType.SupportingBrowsers.Contains(browser.Guid))
                {
                    items.Add(fileType.FiletypeName);
                }
            }

            return string.Join(", ", items);
        }

        /// <summary>
        /// ブラウザ管理機能（Browser Chooser 2互換）
        /// </summary>
        private void SetupBrowserManagement()
        {
            // ブラウザ追加ボタン
            var addButton = Controls.Find("cmdBrowserAdd", true).FirstOrDefault() as Button;
            if (addButton != null)
            {
                addButton.Click += (s, e) =>
                {
                    Logger.LogInfo("OptionsForm.cmdBrowserAdd_Click", "ブラウザ追加ダイアログを表示");
                    try
                    {
                        var addEditForm = new AddEditBrowserForm();
                        var gridSize = new Point(_settings.GridWidth, _settings.GridHeight);
                        
                        if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens, gridSize))
                        {
                            var newBrowser = addEditForm.GetData();
                            _mBrowser.Add(_mLastBrowserID + 1, newBrowser);
                            
                            // ImageListにアイコンを追加
                            if (_imBrowserIcons != null)
                            {
                                _imBrowserIcons.Images.Add(ImageUtilities.GetImage(newBrowser, false));
                            }

                            // ListViewにアイテムを追加
                            var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                            if (listView != null)
                            {
                                var item = listView.Items.Add(newBrowser.Name);
                                item.Tag = _mLastBrowserID + 1;
                                
                                // デフォルトブラウザのチェックマーク
                                if (_settings.DefaultBrowserGuid == newBrowser.Guid)
                                {
                                    item.SubItems.Add("✔");
                                }
                                else
                                {
                                    item.SubItems.Add("");
                                }
                                
                                // 位置情報
                                item.SubItems.Add(newBrowser.PosY.ToString());
                                item.SubItems.Add(newBrowser.PosX.ToString());
                                item.SubItems.Add(newBrowser.Hotkey.ToString());
                                
                                // プロトコル・ファイルタイプ情報
                                item.SubItems.Add(GetBrowserProtocolsAndFileTypes(newBrowser));
                            }
                            
                            _mLastBrowserID++;
                            
                            // プロトコル・ファイルタイプの更新
                            _mProtocols = addEditForm.GetProtocols();
                            _mFileTypes = addEditForm.GetFileTypes();
                            
                            _isModified = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("OptionsForm.cmdBrowserAdd_Click", "ブラウザ追加エラー", ex.Message, ex.StackTrace ?? "");
                        MessageBox.Show($"ブラウザ追加に失敗しました: {ex.Message}", "エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
            }

            // ブラウザ編集ボタン
            var editButton = Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
            if (editButton != null)
            {
                editButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
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
                            
                            // デフォルトブラウザのチェックマーク
                            if (_settings.DefaultBrowserGuid == updatedBrowser.Guid)
                            {
                                selectedItem.SubItems[1].Text = "✔";
                            }
                            else
                            {
                                selectedItem.SubItems[1].Text = "";
                            }
                            
                            // 位置情報
                            selectedItem.SubItems[2].Text = updatedBrowser.PosY.ToString();
                            selectedItem.SubItems[3].Text = updatedBrowser.PosX.ToString();
                            selectedItem.SubItems[4].Text = updatedBrowser.Hotkey.ToString();
                            
                            // プロトコル・ファイルタイプ情報
                            selectedItem.SubItems[5].Text = GetBrowserProtocolsAndFileTypes(updatedBrowser);
                            
                            // ImageListアイコンの更新
                            if (_imBrowserIcons != null)
                            {
                                _imBrowserIcons.Images[selectedIndex] = ImageUtilities.ScaleImageTo(ImageUtilities.GetImage(updatedBrowser, false), new Size(16, 16));
                            }
                            
                            // プロトコル・ファイルタイプの更新
                            _mProtocols = addEditForm.GetProtocols();
                            _mFileTypes = addEditForm.GetFileTypes();
                            
                            _isModified = true;
                        }
                    }
                };
            }

            // ブラウザ削除ボタン
            var deleteButton = Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
            if (deleteButton != null)
            {
                deleteButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var browserName = listView.SelectedItems[0].Text;
                        var result = MessageBox.Show($"Are you sure you want delete the {browserName} entry?", 
                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                            if (selectedIndex != -1)
                            {
                                _mBrowser.Remove(selectedIndex);
                                listView.Items.Remove(listView.SelectedItems[0]);
                                _isModified = true;
                            }
                        }
                    }
                };
            }

            // ブラウザ複製ボタン
            var cloneButton = Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
            if (cloneButton != null)
            {
                cloneButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex == -1 || !_mBrowser.ContainsKey(selectedIndex)) return;

                        var templateBrowser = _mBrowser[selectedIndex];
                        var addEditForm = new AddEditBrowserForm();
                        var gridSize = new Point(_settings.GridWidth, _settings.GridHeight);

                        if (addEditForm.AddBrowser(_mBrowser, _mProtocols, _mFileTypes, _settings.AdvancedScreens, gridSize, templateBrowser))
                        {
                            var newBrowser = addEditForm.GetData();
                            _mBrowser.Add(_mLastBrowserID + 1, newBrowser);

                            // ImageListにアイコンを追加
                            if (_imBrowserIcons != null)
                            {
                                _imBrowserIcons.Images.Add(ImageUtilities.GetImage(newBrowser, false));
                            }

                            // ListViewにアイテムを追加
                            var browserListView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                            if (browserListView != null)
                            {
                                var item = browserListView.Items.Add(newBrowser.Name);
                                item.Tag = _mLastBrowserID + 1;
                                
                                // デフォルトブラウザのチェックマーク
                                if (_settings.DefaultBrowserGuid == newBrowser.Guid)
                                {
                                    item.SubItems.Add("✔");
                                }
                                else
                                {
                                    item.SubItems.Add("");
                                }
                                
                                // 位置情報
                                item.SubItems.Add(newBrowser.PosY.ToString());
                                item.SubItems.Add(newBrowser.PosX.ToString());
                                item.SubItems.Add(newBrowser.Hotkey.ToString());
                                
                                // プロトコル・ファイルタイプ情報
                                item.SubItems.Add(GetBrowserProtocolsAndFileTypes(newBrowser));
                            }

                            _mLastBrowserID++;

                            // プロトコル・ファイルタイプの更新
                            _mProtocols = addEditForm.GetProtocols();
                            _mFileTypes = addEditForm.GetFileTypes();

                            _isModified = true;
                        }
                    }
                };
            }

            // デフォルトブラウザ設定ボタン
            var defaultButton = Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;
            if (defaultButton != null)
            {
                defaultButton.Click += (s, e) =>
                {
                    var listView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
                    if (listView?.SelectedItems.Count > 0)
                    {
                        var selectedIndex = listView.SelectedItems[0].Tag is int tag ? tag : -1;
                        if (selectedIndex == -1 || !_mBrowser.ContainsKey(selectedIndex)) return;
                        
                        var selectedBrowser = _mBrowser[selectedIndex];
                        
                        // 隠しラベルにGUIDを設定
                        var hiddenLabel = Controls.Find("lblHiddenBrowserGuid", true).FirstOrDefault() as Label;
                        if (hiddenLabel != null)
                        {
                            hiddenLabel.Tag = selectedBrowser.Guid.ToString();
                        }
                        
                        // 既存のデフォルトマークを削除し、新しいものを設定
                        foreach (ListViewItem item in listView.Items)
                        {
                            if (item.Tag is int itemTag && _mBrowser.ContainsKey(itemTag))
                            {
                                if (_mBrowser[itemTag].Guid == selectedBrowser.Guid)
                                {
                                    item.SubItems[1].Text = "✔";
                                }
                                else
                                {
                                    item.SubItems[1].Text = "";
                                }
                            }
                        }
                        
                        _isModified = true;
                    }
                };
            }
        }

        /// <summary>
        /// 詳細な選択変更イベントハンドラー（Browser Chooser 2互換）
        /// </summary>
        private void SetupDetailedSelectionHandlers()
        {
            // ブラウザリストの選択変更
            var browsersListView = Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
            if (browsersListView != null)
            {
                browsersListView.SelectedIndexChanged += (s, e) =>
                {
                    var hasSelection = browsersListView.SelectedItems.Count > 0;
                    
                    // ボタンの有効/無効切り替え
                    var editButton = Controls.Find("cmdBrowserEdit", true).FirstOrDefault() as Button;
                    var cloneButton = Controls.Find("cmdBrowserClone", true).FirstOrDefault() as Button;
                    var deleteButton = Controls.Find("cmdBrowserDelete", true).FirstOrDefault() as Button;
                    var defaultButton = Controls.Find("cmdBrowserDefault", true).FirstOrDefault() as Button;
                    
                    if (editButton != null) editButton.Enabled = hasSelection;
                    if (cloneButton != null) cloneButton.Enabled = hasSelection;
                    if (deleteButton != null) deleteButton.Enabled = hasSelection;
                    if (defaultButton != null) defaultButton.Enabled = hasSelection;

                    // ダブルクリック注釈の表示/非表示
                    var noteLabel = Controls.Find("lblDoubleClickBrowsersNote", true).FirstOrDefault() as Label;
                    if (noteLabel != null) noteLabel.Visible = hasSelection;
                };
            }

            // Auto URLsリストの選択変更
            var urlsListView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
            if (urlsListView != null)
            {
                urlsListView.SelectedIndexChanged += (s, e) =>
                {
                    var hasSelection = urlsListView.SelectedItems.Count > 0;
                    
                    // ボタンの有効/無効切り替え
                    var editButton = Controls.Find("cmdAutoURLEdit", true).FirstOrDefault() as Button;
                    var deleteButton = Controls.Find("cmdAutoURLDelete", true).FirstOrDefault() as Button;
                    var moveUpButton = Controls.Find("cmdMoveUpAutoURL", true).FirstOrDefault() as Button;
                    var moveDownButton = Controls.Find("cmdMoveDownAutoURL", true).FirstOrDefault() as Button;
                    
                    if (editButton != null) editButton.Enabled = hasSelection;
                    if (deleteButton != null) deleteButton.Enabled = hasSelection;
                    if (moveUpButton != null) moveUpButton.Enabled = hasSelection;
                    if (moveDownButton != null) moveDownButton.Enabled = hasSelection;

                    // ダブルクリック注釈の表示/非表示
                    var noteLabel = Controls.Find("lblDoubleClickURLsNote", true).FirstOrDefault() as Label;
                    if (noteLabel != null) noteLabel.Visible = hasSelection;
                };
            }

            // プロトコルリストの選択変更
            var protocolsListView = Controls.Find("lstProtocols", true).FirstOrDefault() as ListView;
            if (protocolsListView != null)
            {
                protocolsListView.SelectedIndexChanged += (s, e) =>
                {
                    var hasSelection = protocolsListView.SelectedItems.Count > 0;
                    
                    // ボタンの有効/無効切り替え
                    var editButton = Controls.Find("cmdEditProtocol", true).FirstOrDefault() as Button;
                    var deleteButton = Controls.Find("cmdDeleteProtocol", true).FirstOrDefault() as Button;
                    
                    if (editButton != null) editButton.Enabled = hasSelection;
                    if (deleteButton != null) deleteButton.Enabled = hasSelection;

                    // ダブルクリック注釈の表示/非表示
                    var noteLabel = Controls.Find("lblDoubleClickProtocolsNote", true).FirstOrDefault() as Label;
                    if (noteLabel != null) noteLabel.Visible = hasSelection;
                };
            }

            // ファイルタイプリストの選択変更
            var fileTypesListView = Controls.Find("lstFiletypes", true).FirstOrDefault() as ListView;
            if (fileTypesListView != null)
            {
                fileTypesListView.SelectedIndexChanged += (s, e) =>
                {
                    var hasSelection = fileTypesListView.SelectedItems.Count > 0;
                    
                    // ボタンの有効/無効切り替え
                    var editButton = Controls.Find("cmdEditFileType", true).FirstOrDefault() as Button;
                    var deleteButton = Controls.Find("cmdDeleteFileType", true).FirstOrDefault() as Button;
                    
                    if (editButton != null) editButton.Enabled = hasSelection;
                    if (deleteButton != null) deleteButton.Enabled = hasSelection;

                    // ダブルクリック注釈の表示/非表示
                    var noteLabel = Controls.Find("lblDoubleClickFileTypesNote", true).FirstOrDefault() as Label;
                    if (noteLabel != null) noteLabel.Visible = hasSelection;
                };
            }
        }

        /// <summary>
        /// Auto URLsのドラッグ&amp;ドロップ機能（Browser Chooser 2互換）
        /// </summary>
        private void RebuildAutoURLs()
        {
            var newURLs = new SortedDictionary<int, URL>();

            // ListViewのアイテム順序に基づいてURLリストを再構築
            var listView = Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
            if (listView != null)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    if (item.Tag is int tag && _mURLs.ContainsKey(tag))
                    {
                        var oldURL = _mURLs[tag];
                        newURLs.Add(item.Index, oldURL);
                    }
                }
            }

            _mURLs = newURLs;
            _isModified = true;
        }



        /// <summary>
        /// Auto URLsのドラッグ&amp;ドロップ機能を設定（Browser Chooser 2互換）
        /// </summary>
        private void SetupAutoURLsDragDrop(ListView listView)
        {
            listView.AllowDrop = true;
            listView.DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent("System.Windows.Forms.ListViewItem") == true)
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
                _mDragHighlight = null;
            };

            listView.DragOver += (s, e) =>
            {
                var point = listView.PointToClient(new Point(e.X, e.Y));
                var hitTest = listView.HitTest(point.X, point.Y);

                if (hitTest.Item != null)
                {
                    if (_mDragHighlight == null)
                    {
                        hitTest.Item.BackColor = SystemColors.Highlight;
                        _mDragHighlight = hitTest.Item;
                    }
                    else if (_mDragHighlight.Text != hitTest.Item.Text)
                    {
                        _mDragHighlight.BackColor = SystemColors.Window;
                        hitTest.Item.BackColor = SystemColors.Highlight;
                        _mDragHighlight = hitTest.Item;
                    }
                }
                else
                {
                    if (_mDragHighlight != null)
                    {
                        _mDragHighlight.BackColor = SystemColors.Window;
                    }
                }
            };

            listView.DragDrop += (s, e) =>
            {
                if (e.Data?.GetData("System.Windows.Forms.ListViewItem") is ListViewItem draggedItem)
                {
                    var point = listView.PointToClient(new Point(e.X, e.Y));
                    var hitTest = listView.HitTest(point.X, point.Y);

                    if (hitTest.Item == null)
                    {
                        // リストの先頭または末尾に移動
                        listView.Items.Remove(draggedItem);
                        if (point.Y < 0)
                        {
                            listView.Items.Insert(0, draggedItem);
                        }
                        else
                        {
                            listView.Items.Insert(listView.Items.Count, draggedItem);
                        }
                    }
                    else if (hitTest.Item.Text != draggedItem.Text)
                    {
                        // 特定の位置に移動
                        var oldIndex = hitTest.Item.Index;
                        listView.Items.Remove(draggedItem);
                        listView.Items.Insert(hitTest.Item.Index, draggedItem);
                    }

                    // 内部リストを再構築
                    RebuildAutoURLs();

                    // ハイライトを削除
                    if (_mDragHighlight != null)
                    {
                        _mDragHighlight.BackColor = SystemColors.Window;
                    }
                }
            };

            listView.MouseDown += (s, e) =>
            {
                if (listView.SelectedItems.Count > 0)
                {
                    _mbURLMouseDown = true;
                    _mStartPoint = e.Location;
                }
            };

            listView.MouseMove += (s, e) =>
            {
                if (_mbURLMouseDown)
                {
                    if (_mStartPoint.X != e.Location.X || _mStartPoint.Y != e.Location.Y)
                    {
                        if (listView.SelectedItems.Count > 0)
                        {
                            var dataObject = new DataObject("System.Windows.Forms.ListViewItem", listView.SelectedItems[0]);
                            listView.DoDragDrop(dataObject, DragDropEffects.Move);
                        }
                    }
                }
            };

            listView.MouseUp += (s, e) =>
            {
                _mbURLMouseDown = false;
            };
        }

        /// <summary>
        /// ブラウザリストへのファイルドロップ機能を設定（Browser Chooser 2互換）
        /// </summary>
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
                                    item.SubItems.Add(GetBrowserProtocolsAndFileTypes(browser));
                                    
                                    // ImageListにアイコンを追加
                                    if (_imBrowserIcons != null)
                                    {
                                        _imBrowserIcons.Images.Add(ImageUtilities.GetImage(browser, false));
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

        #region ドラッグ&amp;ドロップ機能（Browser Chooser 2互換）
        
        /// <summary>
        /// Auto URLsのドラッグ&amp;ドロップ機能
        /// </summary>
        private bool _mbURLMouseDown = false;
        private ListViewItem? _mDragHighlight = null;
        private Point _mStartPoint = Point.Empty;



        private void LstURLs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent("System.Windows.Forms.ListViewItem") == true)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
            _mDragHighlight = null;
        }

        private void LstURLs_DragOver(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var hitTest = listView.HitTest(point.X, point.Y);

            if (hitTest.Item != null)
            {
                if (_mDragHighlight == null)
                {
                    hitTest.Item.BackColor = SystemColors.Highlight;
                    _mDragHighlight = hitTest.Item;
                }
                else if (_mDragHighlight.Text != hitTest.Item.Text)
                {
                    _mDragHighlight.BackColor = SystemColors.Window;
                    hitTest.Item.BackColor = SystemColors.Highlight;
                    _mDragHighlight = hitTest.Item;
                }
            }
            else
            {
                if (_mDragHighlight != null)
                {
                    _mDragHighlight.BackColor = SystemColors.Window;
                }
            }
        }

        private void LstURLs_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var myItem = e.Data?.GetData("System.Windows.Forms.ListViewItem") as ListViewItem;
            if (myItem == null) return;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var hitTest = listView.HitTest(point.X, point.Y);

            if (hitTest.Item == null)
            {
                // リストの先頭または末尾に移動
                listView.Items.Remove(myItem);
                if (point.X < 0)
                {
                    listView.Items.Insert(0, myItem);
                }
                else
                {
                    listView.Items.Insert(listView.Items.Count, myItem);
                }
            }
            else if (hitTest.Item.Text != myItem.Text)
            {
                // 特定の位置に移動
                var oldIndex = hitTest.Item.Index;
                listView.Items.Remove(myItem);
                listView.Items.Insert(hitTest.Item.Index, myItem);
            }

            // 内部リストを再構築
            RebuildAutoURLs();

            // ハイライトを削除
            if (_mDragHighlight != null)
            {
                _mDragHighlight.BackColor = SystemColors.Window;
            }
        }

        private void LstURLs_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedItems.Count > 0)
            {
                _mbURLMouseDown = true;
                _mStartPoint = e.Location;
            }
        }

        private void LstURLs_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not ListView listView) return;

            if (_mbURLMouseDown)
            {
                if (_mStartPoint.X != e.Location.X || _mStartPoint.Y != e.Location.Y)
                {
                    if (listView.SelectedItems.Count > 0)
                    {
                        var dataObject = new DataObject("System.Windows.Forms.ListViewItem", listView.SelectedItems[0]);
                        listView.DoDragDrop(dataObject, DragDropEffects.Move);
                    }
                }
            }
        }

        private void LstURLs_MouseUp(object sender, MouseEventArgs e)
        {
            _mbURLMouseDown = false;
        }



        private void LstBrowsers_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
                if (sender is ListView listView)
                {
                    listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void LstBrowsers_DragLeave(object sender, EventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
        }

        private void LstBrowsers_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;

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
                            item.SubItems.Add(GetBrowserProtocolsAndFileTypes(browser));
                            
                            // ImageListにアイコンを追加
                            if (_imBrowserIcons != null)
                            {
                                _imBrowserIcons.Images.Add(ImageUtilities.GetImage(browser, false));
                            }
                            
                            _mLastBrowserID++;
                            _isModified = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("OptionsForm.LstBrowsers_DragDrop", $"ブラウザ追加エラー: {filePath}", ex.Message);
                    }
                }
            }

            listView.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        #endregion
        #endregion

        #region Windows 8/10対応機能（Browser Chooser 2互換）

        /// <summary>
        /// Windows 8/10対応の警告表示を設定
        /// </summary>
        private void SetupWindowsCompatibilityWarnings()
        {
            // Windows 8以降の判定
            if (IsRunningPost8())
            {
                // ユーザースコープに強制設定
                var rbScopeUser = Controls.Find("rbScopeUser", true).FirstOrDefault() as RadioButton;
                var grpScope = Controls.Find("grpScope", true).FirstOrDefault() as GroupBox;
                
                if (rbScopeUser != null) rbScopeUser.Checked = true;
                if (grpScope != null) grpScope.Enabled = false;

                // デフォルトブラウザ設定ボタンのテキスト変更
                var cmdMakeDefault = Controls.Find("cmdMakeDefault", true).FirstOrDefault() as Button;
                if (cmdMakeDefault != null) cmdMakeDefault.Text = "Show Defaults Dialog*";

                // Windows 8警告を表示
                var lblWarnWin8 = Controls.Find("lblWarnWin8", true).FirstOrDefault() as Label;
                var txtWarnWin8 = Controls.Find("txtWarnWin8", true).FirstOrDefault() as TextBox;
                
                if (lblWarnWin8 != null) lblWarnWin8.Visible = true;
                if (txtWarnWin8 != null) txtWarnWin8.Visible = true;

                // 起動時チェックを無効化
                var chkCheckDefaultOnLaunch = Controls.Find("chkCheckDefaultOnLaunch", true).FirstOrDefault() as CheckBox;
                if (chkCheckDefaultOnLaunch != null)
                {
                    chkCheckDefaultOnLaunch.Enabled = false;
                    chkCheckDefaultOnLaunch.Checked = false;
                }
            }

            // Windows 10以降の判定
            if (IsRunningPost10())
            {
                // ユーザースコープに強制設定
                var rbScopeUser = Controls.Find("rbScopeUser", true).FirstOrDefault() as RadioButton;
                var grpScope = Controls.Find("grpScope", true).FirstOrDefault() as GroupBox;
                
                if (rbScopeUser != null) rbScopeUser.Checked = true;
                if (grpScope != null) grpScope.Enabled = false;

                // デフォルトブラウザ設定ボタンのテキスト変更
                var cmdMakeDefault = Controls.Find("cmdMakeDefault", true).FirstOrDefault() as Button;
                if (cmdMakeDefault != null) cmdMakeDefault.Text = "Show Defaults Dialog*";

                // Windows 10警告を表示
                var lblWarnWin10 = Controls.Find("lblWarnWin10", true).FirstOrDefault() as Label;
                var txtWarnWin10 = Controls.Find("txtWarnWin10", true).FirstOrDefault() as TextBox;
                
                if (lblWarnWin10 != null) lblWarnWin10.Visible = true;
                if (txtWarnWin10 != null) txtWarnWin10.Visible = true;

                // 起動時チェックを無効化
                var chkCheckDefaultOnLaunch = Controls.Find("chkCheckDefaultOnLaunch", true).FirstOrDefault() as CheckBox;
                if (chkCheckDefaultOnLaunch != null)
                {
                    chkCheckDefaultOnLaunch.Enabled = false;
                    chkCheckDefaultOnLaunch.Checked = false;
                }

                // 個別ファイル関連付けボタンを非表示
                var cmdOpenDefaultForFile = Controls.Find("cmdOpenDefaultForFile", true).FirstOrDefault() as Button;
                var cmdOpenDefaultForProtocol = Controls.Find("cmdOpenDefaultForProtocol", true).FirstOrDefault() as Button;
                
                if (cmdOpenDefaultForFile != null) cmdOpenDefaultForFile.Visible = false;
                if (cmdOpenDefaultForProtocol != null) cmdOpenDefaultForProtocol.Visible = false;
            }
        }

        /// <summary>
        /// Windows 8以降かどうかを判定
        /// </summary>
        private bool IsRunningPost8()
        {
            try
            {
                var version = Environment.OSVersion.Version;
                return version.Major > 6 || (version.Major == 6 && version.Minor >= 2);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Windows 10以降かどうかを判定
        /// </summary>
        private bool IsRunningPost10()
        {
            try
            {
                var version = Environment.OSVersion.Version;
                return version.Major > 6 || (version.Major == 6 && version.Minor >= 3);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
