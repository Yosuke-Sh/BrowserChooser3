using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.CustomControls;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのUIパネル作成を担当するクラス
    /// </summary>
    public class OptionsFormPanels
    {

        private ImageList? _imBrowserIcons;

        /// <summary>
        /// ブラウザアイコンリストを取得します
        /// </summary>
        /// <returns>ブラウザアイコンリスト</returns>
        public ImageList? GetBrowserIcons()
        {
            return _imBrowserIcons;
        }

        /// <summary>
        /// OptionsFormPanelsクラスの新しいインスタンスを初期化します
        /// </summary>
        public OptionsFormPanels()
        {
        }

        /// <summary>
        /// ブラウザパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="mProtocols">プロトコル辞書</param>
        /// <param name="mFileTypes">ファイルタイプ辞書</param>
        /// <param name="mLastBrowserID">最後のブラウザID</param>
        /// <param name="imBrowserIcons">ブラウザアイコンリスト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <param name="rebuildAutoURLs">Auto URLs再構築アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateBrowsersPanel(
            Settings settings,
            Dictionary<int, Browser> mBrowser,
            Dictionary<int, Protocol> mProtocols,
            Dictionary<int, FileType> mFileTypes,
            int mLastBrowserID,
            ImageList? imBrowserIcons,
            Action<bool> setModified,
            Action rebuildAutoURLs)
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
                Name = "lstBrowsers",
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(97, 6),
                Size = new Size(741, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = true,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false,
                SmallImageList = _imBrowserIcons,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            
            // 選択状態変更イベントを追加
            listView.SelectedIndexChanged += (sender, e) =>
            {
                var buttons = panel.Controls.OfType<Button>().ToList();
                var editButton = buttons.FirstOrDefault(b => b.Name == "btnEdit");
                var cloneButton = buttons.FirstOrDefault(b => b.Name == "btnClone");
                var deleteButton = buttons.FirstOrDefault(b => b.Name == "btnDelete");
                
                if (listView.SelectedItems.Count > 0)
                {
                    if (editButton != null) editButton.Enabled = true;
                    if (cloneButton != null) cloneButton.Enabled = true;
                    if (deleteButton != null) deleteButton.Enabled = true;
                }
                else
                {
                    if (editButton != null) editButton.Enabled = false;
                    if (cloneButton != null) cloneButton.Enabled = false;
                    if (deleteButton != null) deleteButton.Enabled = false;
                }
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
                Name = "btnAdd",
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 40),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var cloneButton = new Button
            {
                Name = "btnClone",
                Text = "Clone",
                Location = new Point(6, 74),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var detectButton = new Button
            {
                Name = "btnDetect",
                Text = "Detect",
                Location = new Point(6, 108),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 142),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            // コントロールの追加
            panel.Controls.Add(listView);
            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(cloneButton);
            panel.Controls.Add(detectButton);
            panel.Controls.Add(deleteButton);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// Auto URLsパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mURLs">URL辞書</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <param name="rebuildAutoURLs">Auto URLs再構築アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateAutoURLsPanel(
            Settings settings,
            SortedDictionary<int, URL> mURLs,
            Dictionary<int, Browser> mBrowser,
            Action<bool> setModified,
            Action rebuildAutoURLs)
        {
            var tabPage = new TabPage("Auto URLs");
            tabPage.Name = "tabAutoURLs";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // URLリストビュー
            var listView = new ListView
            {
                Name = "lstURLs",
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(97, 6),
                Size = new Size(741, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = true,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            listView.Columns.Add("URL", 300);
            listView.Columns.Add("Browser", 200);
            listView.Columns.Add("Delay", 100);

            // ボタン群
            var addButton = new Button
            {
                Name = "btnAdd",
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 40),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 74),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveUpButton = new Button
            {
                Text = "Move Up",
                Location = new Point(6, 108),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveDownButton = new Button
            {
                Text = "Move Down",
                Location = new Point(6, 142),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };



            // コントロールの追加
            panel.Controls.Add(listView);
            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(moveUpButton);
            panel.Controls.Add(moveDownButton);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プロトコルパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mProtocols">プロトコル辞書</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateProtocolsPanel(
            Settings settings,
            Dictionary<int, Protocol> mProtocols,
            Dictionary<int, Browser> mBrowser,
            Action<bool> setModified)
        {
            var tabPage = new TabPage("Protocols");
            tabPage.Name = "tabProtocols";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // プロトコルリストビュー
            var listView = new ListView
            {
                Name = "lstProtocols",
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(97, 6),
                Size = new Size(741, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            listView.Columns.Add("Protocol", 200);
            listView.Columns.Add("Browser", 300);
            listView.Columns.Add("Default App", 200);

            // ボタン群
            var addButton = new Button
            {
                Name = "btnAdd",
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 64),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 93),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };



            // コントロールの追加
            panel.Controls.Add(listView);
            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(selectDefaultButton);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// ファイルタイプパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mFileTypes">ファイルタイプ辞書</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateFileTypesPanel(
            Settings settings,
            Dictionary<int, FileType> mFileTypes,
            Dictionary<int, Browser> mBrowser,
            Action<bool> setModified)
        {
            var tabPage = new TabPage("File Types");
            tabPage.Name = "tabFileTypes";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // ファイルタイプリストビュー
            var listView = new ListView
            {
                Name = "lstFileTypes",
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(97, 6),
                Size = new Size(741, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            listView.Columns.Add("File Type", 200);
            listView.Columns.Add("Browser", 300);
            listView.Columns.Add("Default App", 200);

            // ボタン群
            var addButton = new Button
            {
                Name = "btnAdd",
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 64),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 93),
                Size = new Size(85, 28),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };



            // コントロールの追加
            panel.Controls.Add(listView);
            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(selectDefaultButton);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// カテゴリパネルの作成
        /// </summary>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateCategoriesPanel()
        {
            var tabPage = new TabPage("Categories");
            tabPage.Name = "tabCategories";
            
            // デザイナーで定義されたcategoryPanelを使用
            // このパネルはOptionsForm.Designer.csで定義されており、
            // カテゴリ管理に必要なすべてのコントロールが含まれている
            // このパネルは後でOptionsForm.csで表示/非表示を制御する
            
            return tabPage;
        }

        /// <summary>
        /// 表示パネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateDisplayPanel(Settings settings, Action<bool> setModified)
        {
            var tabPage = new TabPage("Display");
            tabPage.Name = "tabDisplay";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // アクセシビリティボタン
            var accessibilityButton = new Button
            {
                Name = "btnAccessibility",
                Text = "Accessibility Settings",
                Location = new Point(6, 6),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 背景色ボタン
            var backgroundColorButton = new Button
            {
                Name = "btnBackgroundColor",
                Text = "Change Background Color",
                Location = new Point(6, 40),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 透明背景ボタン
            var transparentButton = new Button
            {
                Name = "btnTransparent",
                Text = "Set Transparent Background",
                Location = new Point(6, 74),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // コントロールの追加
            panel.Controls.Add(accessibilityButton);
            panel.Controls.Add(backgroundColorButton);
            panel.Controls.Add(transparentButton);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// グリッドパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateGridPanel(Settings settings, Action<bool> setModified)
        {
            var tabPage = new TabPage("Grid");
            tabPage.Name = "tabGrid";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // グリッド幅設定
            var lblGridWidth = new Label
            {
                Text = "Grid Width:",
                Location = new Point(6, 9),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridWidth = new NumericUpDown
            {
                Name = "nudGridWidth",
                Location = new Point(90, 6),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 20,
                Value = settings.GridWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridWidth.ValueChanged += (s, e) => setModified(true);

            // グリッド高さ設定
            var lblGridHeight = new Label
            {
                Text = "Grid Height:",
                Location = new Point(6, 39),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridHeight = new NumericUpDown
            {
                Name = "nudGridHeight",
                Location = new Point(90, 36),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 20,
                Value = settings.GridHeight,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridHeight.ValueChanged += (s, e) => setModified(true);

            // グリッド表示設定
            var chkShowGrid = new CheckBox
            {
                Name = "chkShowGrid",
                Text = "Show Grid",
                Location = new Point(6, 69),
                Size = new Size(100, 23),
                Checked = settings.ShowGrid,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkShowGrid.CheckedChanged += (s, e) => setModified(true);

            // グリッド線の色設定
            var lblGridColor = new Label
            {
                Text = "Grid Color:",
                Location = new Point(6, 99),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var pbGridColor = new Panel
            {
                Name = "pbGridColor",
                Location = new Point(90, 96),
                Size = new Size(80, 23),
                BackColor = Color.FromArgb(settings.GridColor),
                BorderStyle = BorderStyle.FixedSingle
            };
            pbGridColor.Click += (s, e) =>
            {
                using var colorDialog = new ColorDialog
                {
                    Color = pbGridColor.BackColor
                };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    pbGridColor.BackColor = colorDialog.Color;
                    setModified(true);
                }
            };

            // グリッド線の太さ設定
            var lblGridLineWidth = new Label
            {
                Text = "Line Width:",
                Location = new Point(6, 129),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridLineWidth = new NumericUpDown
            {
                Name = "nudGridLineWidth",
                Location = new Point(90, 126),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 10,
                Value = settings.GridLineWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridLineWidth.ValueChanged += (s, e) => setModified(true);

            // コントロールの追加
            panel.Controls.Add(lblGridWidth);
            panel.Controls.Add(nudGridWidth);
            panel.Controls.Add(lblGridHeight);
            panel.Controls.Add(nudGridHeight);
            panel.Controls.Add(chkShowGrid);
            panel.Controls.Add(lblGridColor);
            panel.Controls.Add(pbGridColor);
            panel.Controls.Add(lblGridLineWidth);
            panel.Controls.Add(nudGridLineWidth);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プライバシーパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreatePrivacyPanel(Settings settings, Action<bool> setModified)
        {
            var tabPage = new TabPage("Privacy");
            tabPage.Name = "tabPrivacy";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // ログ設定
            var chkEnableLogging = new CheckBox
            {
                Name = "chkEnableLogging",
                Text = "Enable Logging",
                Location = new Point(6, 6),
                Size = new Size(150, 23),
                Checked = settings.EnableLogging,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkEnableLogging.CheckedChanged += (s, e) => setModified(true);

            // ログレベル設定
            var lblLogLevel = new Label
            {
                Text = "Log Level:",
                Location = new Point(6, 35),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var cmbLogLevel = new ComboBox
            {
                Name = "cmbLogLevel",
                Location = new Point(90, 32),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cmbLogLevel.Items.AddRange(new object[] { "Trace", "Debug", "Info", "Warning", "Error" });
            cmbLogLevel.SelectedIndex = Math.Min(settings.LogLevel, cmbLogLevel.Items.Count - 1);
            cmbLogLevel.SelectedIndexChanged += (s, e) => setModified(true);

            // 履歴保持設定
            var chkKeepHistory = new CheckBox
            {
                Name = "chkKeepHistory",
                Text = "Keep Browser History",
                Location = new Point(6, 65),
                Size = new Size(150, 23),
                Checked = settings.KeepHistory,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkKeepHistory.CheckedChanged += (s, e) => setModified(true);

            // 履歴保持日数
            var lblHistoryDays = new Label
            {
                Text = "History Days:",
                Location = new Point(6, 94),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudHistoryDays = new NumericUpDown
            {
                Name = "nudHistoryDays",
                Location = new Point(90, 91),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 365,
                Value = settings.HistoryDays,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudHistoryDays.ValueChanged += (s, e) => setModified(true);

            // プライバシーモード設定
            var chkPrivacyMode = new CheckBox
            {
                Name = "chkPrivacyMode",
                Text = "Privacy Mode (Clear on Exit)",
                Location = new Point(6, 123),
                Size = new Size(200, 23),
                Checked = settings.PrivacyMode,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkPrivacyMode.CheckedChanged += (s, e) => setModified(true);

            // データ収集設定
            var chkAllowDataCollection = new CheckBox
            {
                Name = "chkAllowDataCollection",
                Text = "Allow Data Collection",
                Location = new Point(6, 152),
                Size = new Size(150, 23),
                Checked = settings.AllowDataCollection,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAllowDataCollection.CheckedChanged += (s, e) => setModified(true);

            // コントロールの追加
            panel.Controls.Add(chkEnableLogging);
            panel.Controls.Add(lblLogLevel);
            panel.Controls.Add(cmbLogLevel);
            panel.Controls.Add(chkKeepHistory);
            panel.Controls.Add(lblHistoryDays);
            panel.Controls.Add(nudHistoryDays);
            panel.Controls.Add(chkPrivacyMode);
            panel.Controls.Add(chkAllowDataCollection);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// スタートアップパネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateStartupPanel(Settings settings, Action<bool> setModified)
        {
            var tabPage = new TabPage("Startup");
            tabPage.Name = "tabStartup";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // 自動起動設定
            var chkAutoStart = new CheckBox
            {
                Name = "chkAutoStart",
                Text = "Start with Windows",
                Location = new Point(6, 6),
                Size = new Size(150, 23),
                Checked = settings.AutoStart,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAutoStart.CheckedChanged += (s, e) => setModified(true);

            // 最小化で起動設定
            var chkStartMinimized = new CheckBox
            {
                Name = "chkStartMinimized",
                Text = "Start Minimized",
                Location = new Point(6, 35),
                Size = new Size(150, 23),
                Checked = settings.StartMinimized,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkStartMinimized.CheckedChanged += (s, e) => setModified(true);

            // システムトレイ設定
            var chkStartInTray = new CheckBox
            {
                Name = "chkStartInTray",
                Text = "Start in System Tray",
                Location = new Point(6, 64),
                Size = new Size(150, 23),
                Checked = settings.StartInTray,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkStartInTray.CheckedChanged += (s, e) => setModified(true);

            // 起動時のチェック設定
            var chkCheckDefaultOnStartup = new CheckBox
            {
                Name = "chkCheckDefaultOnStartup",
                Text = "Check Default Browser on Startup",
                Location = new Point(6, 93),
                Size = new Size(200, 23),
                Checked = settings.CheckDefaultOnLaunch,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkCheckDefaultOnStartup.CheckedChanged += (s, e) => setModified(true);

            // 起動遅延設定
            var lblStartupDelay = new Label
            {
                Text = "Startup Delay (ms):",
                Location = new Point(6, 122),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudStartupDelay = new NumericUpDown
            {
                Name = "nudStartupDelay",
                Location = new Point(130, 119),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 10000,
                Value = settings.StartupDelay,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudStartupDelay.ValueChanged += (s, e) => setModified(true);

            // 起動時のメッセージ設定
            var lblStartupMessage = new Label
            {
                Text = "Startup Message:",
                Location = new Point(6, 151),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtStartupMessage = new TextBox
            {
                Name = "txtStartupMessage",
                Location = new Point(110, 148),
                Size = new Size(200, 23),
                Text = settings.StartupMessage,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtStartupMessage.TextChanged += (s, e) => setModified(true);

            // コントロールの追加
            panel.Controls.Add(chkAutoStart);
            panel.Controls.Add(chkStartMinimized);
            panel.Controls.Add(chkStartInTray);
            panel.Controls.Add(chkCheckDefaultOnStartup);
            panel.Controls.Add(lblStartupDelay);
            panel.Controls.Add(nudStartupDelay);
            panel.Controls.Add(lblStartupMessage);
            panel.Controls.Add(txtStartupMessage);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 一般設定パネルの作成
        /// </summary>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateGeneralPanel()
        {
            var tabPage = new TabPage("General");
            tabPage.Name = "tabGeneral";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // 一般設定のコントロールを追加
            var label = new Label
            {
                Text = "General settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// その他パネルの作成
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateOthersPanel(Settings settings, Action<bool> setModified)
        {
            var tabPage = new TabPage("Others");
            tabPage.Name = "tabOthers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // ポータブルモード設定
            var chkPortableMode = new CheckBox
            {
                Name = "chkPortableMode",
                Text = "Portable Mode",
                Location = new Point(6, 6),
                Size = new Size(150, 23),
                Checked = settings.PortableMode,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkPortableMode.CheckedChanged += (s, e) => setModified(true);

            // 自動更新設定
            var chkAutoCheckUpdate = new CheckBox
            {
                Name = "chkAutoCheckUpdate",
                Text = "Check for Updates Automatically",
                Location = new Point(6, 35),
                Size = new Size(200, 23),
                Checked = settings.AutomaticUpdates,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAutoCheckUpdate.CheckedChanged += (s, e) => setModified(true);

            // 詳細画面設定
            var chkAdvanced = new CheckBox
            {
                Name = "chkAdvanced",
                Text = "Show Advanced Options",
                Location = new Point(6, 64),
                Size = new Size(150, 23),
                Checked = settings.AdvancedScreens,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAdvanced.CheckedChanged += (s, e) => setModified(true);

            // デフォルト遅延設定
            var lblDefaultDelay = new Label
            {
                Text = "Default Delay (ms):",
                Location = new Point(6, 93),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudDefaultDelay = new NumericUpDown
            {
                Name = "nudDefaultDelay",
                Location = new Point(130, 90),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 10000,
                Value = settings.DefaultDelay,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudDefaultDelay.ValueChanged += (s, e) => setModified(true);

            // セパレーター設定
            var lblSeparator = new Label
            {
                Text = "Separator:",
                Location = new Point(6, 122),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtSeparator = new TextBox
            {
                Name = "txtSeparator",
                Location = new Point(90, 119),
                Size = new Size(50, 23),
                Text = settings.Separator,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtSeparator.TextChanged += (s, e) => setModified(true);

            // 開いたまま許可設定
            var chkAllowStayOpen = new CheckBox
            {
                Name = "chkAllowStayOpen",
                Text = "Allow Window to Stay Open",
                Location = new Point(6, 151),
                Size = new Size(180, 23),
                Checked = settings.AllowStayOpen,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAllowStayOpen.CheckedChanged += (s, e) => setModified(true);

            // ユーザーエージェント設定
            var lblUserAgent = new Label
            {
                Text = "User Agent:",
                Location = new Point(6, 180),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtUserAgent = new TextBox
            {
                Name = "txtUserAgent",
                Location = new Point(90, 177),
                Size = new Size(200, 23),
                Text = settings.UserAgent,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtUserAgent.TextChanged += (s, e) => setModified(true);

            // コントロールの追加
            panel.Controls.Add(chkPortableMode);
            panel.Controls.Add(chkAutoCheckUpdate);
            panel.Controls.Add(chkAdvanced);
            panel.Controls.Add(lblDefaultDelay);
            panel.Controls.Add(nudDefaultDelay);
            panel.Controls.Add(lblSeparator);
            panel.Controls.Add(txtSeparator);
            panel.Controls.Add(chkAllowStayOpen);
            panel.Controls.Add(lblUserAgent);
            panel.Controls.Add(txtUserAgent);

            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// アクセシビリティパネルの作成
        /// </summary>
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateAccessibilityPanel()
        {
            var tabPage = new TabPage("Accessibility");
            tabPage.Name = "tabAccessibility";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // アクセシビリティ設定のコントロールを追加
            var label = new Label
            {
                Text = "Accessibility settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

            tabPage.Controls.Add(panel);
            return tabPage;
        }


    }
}
