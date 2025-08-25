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

            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var cloneButton = new Button
            {
                Text = "Clone",
                Location = new Point(6, 64),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var detectButton = new Button
            {
                Text = "Detect",
                Location = new Point(6, 93),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(6, 122),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
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
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = true,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false
            };

            listView.Columns.Add("URL", 300);
            listView.Columns.Add("Browser", 200);
            listView.Columns.Add("Delay", 100);

            // ボタン群
            var addButton = new Button
            {
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(6, 64),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveUpButton = new Button
            {
                Text = "Move Up",
                Location = new Point(6, 93),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveDownButton = new Button
            {
                Text = "Move Down",
                Location = new Point(6, 122),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
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
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false
            };

            listView.Columns.Add("Protocol", 200);
            listView.Columns.Add("Browser", 300);
            listView.Columns.Add("Default App", 200);

            // ボタン群
            var addButton = new Button
            {
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(6, 64),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 93),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
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
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(87, 6),
                Size = new Size(751, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false
            };

            listView.Columns.Add("File Type", 200);
            listView.Columns.Add("Browser", 300);
            listView.Columns.Add("Default App", 200);

            // ボタン群
            var addButton = new Button
            {
                Text = "Add",
                Location = new Point(6, 6),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Text = "Edit",
                Location = new Point(6, 35),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(6, 64),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 93),
                Size = new Size(75, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
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
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // カテゴリリストビュー
            var listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(6, 6),
                Size = new Size(832, 218),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                MultiSelect = false,
                HideSelection = false,
                UseCompatibleStateImageBehavior = false
            };

            listView.Columns.Add("Category", 400);
            listView.Columns.Add("Description", 400);

            // コントロールの追加
            panel.Controls.Add(listView);

            tabPage.Controls.Add(panel);
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
                Text = "Accessibility Settings",
                Location = new Point(6, 6),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 背景色ボタン
            var backgroundColorButton = new Button
            {
                Text = "Change Background Color",
                Location = new Point(6, 35),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            // 透明背景ボタン
            var transparentButton = new Button
            {
                Text = "Set Transparent Background",
                Location = new Point(6, 64),
                Size = new Size(150, 23),
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

            // グリッド設定のコントロールを追加
            var label = new Label
            {
                Text = "Grid settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

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

            // プライバシー設定のコントロールを追加
            var label = new Label
            {
                Text = "Privacy settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

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

            // スタートアップ設定のコントロールを追加
            var label = new Label
            {
                Text = "Startup settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

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
        /// <returns>作成されたTabPage</returns>
        public TabPage CreateOthersPanel()
        {
            var tabPage = new TabPage("Others");
            tabPage.Name = "tabOthers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // その他設定のコントロールを追加
            var label = new Label
            {
                Text = "Other settings will be implemented here",
                Location = new Point(6, 6),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            panel.Controls.Add(label);

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
