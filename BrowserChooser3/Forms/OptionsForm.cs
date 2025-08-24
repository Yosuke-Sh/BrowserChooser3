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

        /// <summary>
        /// OptionsFormクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public OptionsForm(Settings settings)
        {
            _settings = settings;
            InitializeForm();
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeForm()
        {
            Logger.LogInfo("OptionsForm.InitializeForm", "Start");
            
            try
            {
                // フォームの基本設定
                Text = "Options";
                Size = new Size(1100, 400);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = true;
                MinimizeBox = true;
                TopMost = true;
                
                // フォントの設定（現代的で日本語・英語両対応）
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                
                // 最小サイズの設定
                MinimumSize = new Size(800, 450);
                
                // TreeViewの作成
                var treeSettings = new TreeView();
                treeSettings.Location = new Point(10, 10);
                treeSettings.Size = new Size(200, 320);
                treeSettings.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                treeSettings.AfterSelect += TreeSettings_AfterSelect;
                
                // TreeViewノードの作成
                var commonNode = new TreeNode("Common");
                commonNode.Nodes.Add(new TreeNode("Browsers & applications") { Tag = "tabBrowsers" });
                commonNode.Nodes.Add(new TreeNode("Auto URLs") { Tag = "tabAutoURLs" });
                
                var associationsNode = new TreeNode("Associations");
                associationsNode.Nodes.Add(new TreeNode("Protocols") { Tag = "tabProtocols" });
                associationsNode.Nodes.Add(new TreeNode("File Types") { Tag = "tabFileTypes" });
                
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
                
                // TabControlの作成
                var tabSettings = new TabControl();
                tabSettings.Location = new Point(220, 10);
                tabSettings.Size = new Size(850, 320);
                tabSettings.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                
                // タブページの作成
                var browsersTab = CreateBrowsersPanel();
                var autoUrlsTab = CreateAutoURLsPanel();
                var protocolsTab = CreateProtocolsPanel();
                var fileTypesTab = CreateFileTypesPanel();
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
                tabSettings.TabPages.Add(displayTab);
                tabSettings.TabPages.Add(gridTab);
                tabSettings.TabPages.Add(privacyTab);
                tabSettings.TabPages.Add(startupTab);
                tabSettings.TabPages.Add(othersTab);

                tabSettings.TabPages.Add(defaultBrowserTab);
                
                // ボタンの作成
                var saveButton = new Button();
                saveButton.Text = "Save";
                saveButton.Location = new Point(850, 340);
                saveButton.Size = new Size(90, 32);
                saveButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                saveButton.Click += (s, e) => SaveSettings();
                
                var cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.Location = new Point(950, 340);
                cancelButton.Size = new Size(90, 32);
                cancelButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                cancelButton.DialogResult = DialogResult.Cancel;
                
                var helpButton = new Button();
                helpButton.Text = "Help";
                helpButton.Location = new Point(1050, 340);
                helpButton.Size = new Size(90, 32);
                helpButton.Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
                
                // コントロールの追加
                Controls.Add(treeSettings);
                Controls.Add(tabSettings);
                Controls.Add(saveButton);
                Controls.Add(cancelButton);
                Controls.Add(helpButton);
                
                // サイズ変更イベントの設定
                Resize += OptionsForm_Resize;
                
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

            // ブラウザリストビュー
            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(10, 10),
                Size = new Size(600, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            listView.Columns.Add("Name", 150);
            listView.Columns.Add("Row", 50);
            listView.Columns.Add("Column", 50);
            listView.Columns.Add("Hotkey", 80);
            listView.Columns.Add("Protocols and File Types", 200);

            // ボタンパネル
            var buttonPanel = new Panel
            {
                Location = new Point(10, 320),
                Size = new Size(600, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var addButton = new Button
            {
                Text = "Add",
                Location = new Point(0, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            addButton.Click += AddBrowser_Click;

            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(90, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            editButton.Click += EditBrowser_Click;

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(180, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            deleteButton.Click += DeleteBrowser_Click;

            var cloneButton = new Button
            {
                Text = "Clone",
                Location = new Point(270, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cloneButton.Click += CloneBrowser_Click;

            var detectButton = new Button
            {
                Text = "Detect",
                Location = new Point(360, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            detectButton.Click += DetectBrowsers_Click;

            var defaultButton = new Button
            {
                Text = "Default",
                Location = new Point(450, 5),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            defaultButton.Click += SetDefaultBrowser_Click;

            // 説明ラベル
            var noteLabel = new Label
            {
                Text = "Double-click to edit browser settings",
                Location = new Point(10, 370),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);
            buttonPanel.Controls.Add(cloneButton);
            buttonPanel.Controls.Add(detectButton);
            buttonPanel.Controls.Add(defaultButton);

            panel.Controls.Add(listView);
            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(noteLabel);
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

            // ListView for URLs
            var lstURLs = new ListView
            {
                Location = new Point(10, 40),
                Size = new Size(600, 150),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            
            lstURLs.Columns.Add("URL", 300);
            lstURLs.Columns.Add("Browser", 200);
            lstURLs.Columns.Add("Time", 100);

            // Buttons
            var btnAdd = new Button { Text = "Add", Location = new Point(620, 40), Size = new Size(90, 32), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnEdit = new Button { Text = "Edit", Location = new Point(620, 80), Size = new Size(90, 32), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnDelete = new Button { Text = "Delete", Location = new Point(620, 120), Size = new Size(90, 32), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnMoveUp = new Button { Text = "Move Up", Location = new Point(620, 160), Size = new Size(90, 32), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };
            var btnMoveDown = new Button { Text = "Move Down", Location = new Point(620, 200), Size = new Size(90, 32), Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0) };

            var noteLabel = new Label
            {
                Text = "Double-click to edit an entry",
                Location = new Point(10, 200),
                AutoSize = true,
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
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プロトコルパネルの作成
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

            panel.Controls.Add(label);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// ファイルタイプパネルの作成
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

            panel.Controls.Add(label);
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
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Display Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // チェックボックス群（Browser Chooser 2互換）
            var chkAllowStayOpen = new CheckBox
            {
                Text = "Allow stay open",
                Location = new Point(10, 40),
                AutoSize = true
            };

            var chkRevealShortURLs = new CheckBox
            {
                Text = "Reveal short URLs",
                Location = new Point(10, 65),
                AutoSize = true
            };

            var chkUseAero = new CheckBox
            {
                Text = "Use Aero effects",
                Location = new Point(10, 90),
                AutoSize = true
            };

            var chkUseAccessibleRendering = new CheckBox
            {
                Text = "Use accessible rendering",
                Location = new Point(10, 115),
                AutoSize = true
            };

            // アクセシビリティ設定ボタン
            var btnAccessibilitySettings = new Button
            {
                Text = "Accessibility Settings...",
                Location = new Point(10, 145),
                Size = new Size(150, 25)
            };
            btnAccessibilitySettings.Click += (s, e) =>
            {
                var accessibilityForm = new AccessibilitySettingsForm(_settings);
                accessibilityForm.ShowDialog(this);
            };

            // 背景色設定
            var lblBackgroundColor = new Label
            {
                Text = "Background Color:",
                Location = new Point(10, 180),
                AutoSize = true
            };

            var pbBackgroundColor = new PictureBox
            {
                BackColor = Color.Gold,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(120, 178),
                Size = new Size(30, 20)
            };

            var btnChangeBackgroundColor = new Button
            {
                Text = "Change...",
                Location = new Point(160, 176),
                Size = new Size(75, 23)
            };

            var btnTransparentBackground = new Button
            {
                Text = "Transparent",
                Location = new Point(245, 176),
                Size = new Size(85, 23)
            };

            panel.Controls.Add(label);
            panel.Controls.Add(chkAllowStayOpen);
            panel.Controls.Add(chkRevealShortURLs);
            panel.Controls.Add(chkUseAero);
            panel.Controls.Add(chkUseAccessibleRendering);
            panel.Controls.Add(btnAccessibilitySettings);
            panel.Controls.Add(lblBackgroundColor);
            panel.Controls.Add(pbBackgroundColor);
            panel.Controls.Add(btnChangeBackgroundColor);
            panel.Controls.Add(btnTransparentBackground);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// グリッドパネルの作成
        /// </summary>
        private TabPage CreateGridPanel()
        {
            var tabPage = new TabPage("Grid");
            tabPage.Name = "tabGrid";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Grid Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プライバシーパネルの作成
        /// </summary>
        private TabPage CreatePrivacyPanel()
        {
            var tabPage = new TabPage("Privacy");
            tabPage.Name = "tabPrivacy";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Privacy Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// スタートアップパネルの作成
        /// </summary>
        private TabPage CreateStartupPanel()
        {
            var tabPage = new TabPage("Startup");
            tabPage.Name = "tabStartup";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Startup Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
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
        /// その他パネルの作成
        /// </summary>
        private TabPage CreateOthersPanel()
        {
            var tabPage = new TabPage("Others");
            tabPage.Name = "tabOthers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Other Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
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
                var accessibilityForm = new AccessibilitySettingsForm(_settings);
                accessibilityForm.ShowDialog(this);
            };

            panel.Controls.Add(label);
            panel.Controls.Add(accessibilityButton);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// デフォルトブラウザパネルの作成
        /// </summary>
        private TabPage CreateDefaultBrowserPanel()
        {
            var tabPage = new TabPage("Windows Default");
            tabPage.Name = "tabDefaultBrowser";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = "Windows Default Browser Settings",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(label);
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 設定値の読み込み
        /// </summary>
        private void LoadSettings()
        {
            Logger.LogInfo("OptionsForm.LoadSettings", "Start");
            
            // 設定値を各コントロールに設定
            // この部分は実際のコントロールに応じて実装
            
            Logger.LogInfo("OptionsForm.LoadSettings", "End");
        }

        /// <summary>
        /// 設定値の保存
        /// </summary>
        private void SaveSettings()
        {
            Logger.LogInfo("OptionsForm.SaveSettings", "Start");
            
            try
            {
                // 各コントロールから設定値を取得して保存
                // この部分は実際のコントロールに応じて実装
                
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
            // TODO: ブラウザ追加ダイアログを実装
            MessageBox.Show("ブラウザ追加機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditBrowser_Click(object? sender, EventArgs e)
        {
            // TODO: ブラウザ編集ダイアログを実装
            MessageBox.Show("ブラウザ編集機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteBrowser_Click(object? sender, EventArgs e)
        {
            // TODO: ブラウザ削除機能を実装
            MessageBox.Show("ブラウザ削除機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CloneBrowser_Click(object? sender, EventArgs e)
        {
            // TODO: ブラウザ複製機能を実装
            MessageBox.Show("ブラウザ複製機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DetectBrowsers_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("OptionsForm.DetectBrowsers_Click", "ブラウザ検出を開始");
            try
            {
                var detectedBrowsers = BrowserDetector.DetectBrowsers();
                if (detectedBrowsers.Count > 0)
                {
                    // 既存のブラウザリストに追加
                    _settings?.Browsers.AddRange(detectedBrowsers);
                    MessageBox.Show($"{detectedBrowsers.Count}個のブラウザを検出しました", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("ブラウザが見つかりませんでした", "情報", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsForm.DetectBrowsers_Click", "ブラウザ検出エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"ブラウザ検出に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultBrowser_Click(object? sender, EventArgs e)
        {
            // TODO: デフォルトブラウザ設定機能を実装
            MessageBox.Show("デフォルトブラウザ設定機能は未実装です", "情報", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
