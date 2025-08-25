using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// OptionsFormの各パネル作成を担当するクラス
    /// </summary>
    public static class OptionsFormPanels
    {
        /// <summary>
        /// ブラウザ設定パネルを作成
        /// </summary>
        public static TabPage CreateBrowsersPanel()
        {
            var tabPage = new TabPage("Browsers & applications");
            tabPage.Name = "tabBrowsers";
            
            // ブラウザ設定のUI要素を作成
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // ブラウザリストビュー
            var listViewBrowsers = new ListView
            {
                Location = new Point(10, 10),
                Size = new Size(400, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewBrowsers.Columns.Add("Name", 150);
            listViewBrowsers.Columns.Add("Path", 250);

            // ボタン群
            var btnAddBrowser = new Button
            {
                Text = "Add",
                Location = new Point(420, 10),
                Size = new Size(75, 23)
            };

            var btnEditBrowser = new Button
            {
                Text = "Edit",
                Location = new Point(420, 40),
                Size = new Size(75, 23)
            };

            var btnRemoveBrowser = new Button
            {
                Text = "Remove",
                Location = new Point(420, 70),
                Size = new Size(75, 23)
            };

            panel.Controls.Add(listViewBrowsers);
            panel.Controls.Add(btnAddBrowser);
            panel.Controls.Add(btnEditBrowser);
            panel.Controls.Add(btnRemoveBrowser);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 自動URL設定パネルを作成
        /// </summary>
        public static TabPage CreateAutoURLsPanel()
        {
            var tabPage = new TabPage("Auto URLs");
            tabPage.Name = "tabAutoURLs";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // URL設定のUI要素を作成
            var listViewURLs = new ListView
            {
                Location = new Point(10, 10),
                Size = new Size(400, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewURLs.Columns.Add("Pattern", 200);
            listViewURLs.Columns.Add("Browser", 200);

            var btnAddURL = new Button
            {
                Text = "Add",
                Location = new Point(420, 10),
                Size = new Size(75, 23)
            };

            var btnEditURL = new Button
            {
                Text = "Edit",
                Location = new Point(420, 40),
                Size = new Size(75, 23)
            };

            var btnRemoveURL = new Button
            {
                Text = "Remove",
                Location = new Point(420, 70),
                Size = new Size(75, 23)
            };

            panel.Controls.Add(listViewURLs);
            panel.Controls.Add(btnAddURL);
            panel.Controls.Add(btnEditURL);
            panel.Controls.Add(btnRemoveURL);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プロトコル設定パネルを作成
        /// </summary>
        public static TabPage CreateProtocolsPanel()
        {
            var tabPage = new TabPage("Protocols");
            tabPage.Name = "tabProtocols";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // プロトコル設定のUI要素を作成
            var listViewProtocols = new ListView
            {
                Location = new Point(10, 10),
                Size = new Size(400, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewProtocols.Columns.Add("Protocol", 100);
            listViewProtocols.Columns.Add("Browser", 200);
            listViewProtocols.Columns.Add("Description", 100);

            var btnAddProtocol = new Button
            {
                Text = "Add",
                Location = new Point(420, 10),
                Size = new Size(75, 23)
            };

            var btnEditProtocol = new Button
            {
                Text = "Edit",
                Location = new Point(420, 40),
                Size = new Size(75, 23)
            };

            var btnRemoveProtocol = new Button
            {
                Text = "Remove",
                Location = new Point(420, 70),
                Size = new Size(75, 23)
            };

            panel.Controls.Add(listViewProtocols);
            panel.Controls.Add(btnAddProtocol);
            panel.Controls.Add(btnEditProtocol);
            panel.Controls.Add(btnRemoveProtocol);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// ファイルタイプ設定パネルを作成
        /// </summary>
        public static TabPage CreateFileTypesPanel()
        {
            var tabPage = new TabPage("File Types");
            tabPage.Name = "tabFileTypes";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // ファイルタイプ設定のUI要素を作成
            var listViewFileTypes = new ListView
            {
                Location = new Point(10, 10),
                Size = new Size(400, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewFileTypes.Columns.Add("Extension", 100);
            listViewFileTypes.Columns.Add("Browser", 200);
            listViewFileTypes.Columns.Add("Description", 100);

            var btnAddFileType = new Button
            {
                Text = "Add",
                Location = new Point(420, 10),
                Size = new Size(75, 23)
            };

            var btnEditFileType = new Button
            {
                Text = "Edit",
                Location = new Point(420, 40),
                Size = new Size(75, 23)
            };

            var btnRemoveFileType = new Button
            {
                Text = "Remove",
                Location = new Point(420, 70),
                Size = new Size(75, 23)
            };

            panel.Controls.Add(listViewFileTypes);
            panel.Controls.Add(btnAddFileType);
            panel.Controls.Add(btnEditFileType);
            panel.Controls.Add(btnRemoveFileType);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// カテゴリ設定パネルを作成
        /// </summary>
        public static TabPage CreateCategoriesPanel()
        {
            var tabPage = new TabPage("Categories");
            tabPage.Name = "tabCategories";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // カテゴリ設定のUI要素を作成
            var listViewCategories = new ListView
            {
                Location = new Point(10, 10),
                Size = new Size(400, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewCategories.Columns.Add("Category", 150);
            listViewCategories.Columns.Add("Description", 250);

            var btnAddCategory = new Button
            {
                Text = "Add",
                Location = new Point(420, 10),
                Size = new Size(75, 23)
            };

            var btnEditCategory = new Button
            {
                Text = "Edit",
                Location = new Point(420, 40),
                Size = new Size(75, 23)
            };

            var btnRemoveCategory = new Button
            {
                Text = "Remove",
                Location = new Point(420, 70),
                Size = new Size(75, 23)
            };

            panel.Controls.Add(listViewCategories);
            panel.Controls.Add(btnAddCategory);
            panel.Controls.Add(btnEditCategory);
            panel.Controls.Add(btnRemoveCategory);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// 表示設定パネルを作成
        /// </summary>
        public static TabPage CreateDisplayPanel()
        {
            var tabPage = new TabPage("Display");
            tabPage.Name = "tabDisplay";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // 表示設定のUI要素を作成
            var chkShowIcons = new CheckBox
            {
                Text = "Show browser icons",
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                Checked = true
            };

            var chkShowNames = new CheckBox
            {
                Text = "Show browser names",
                Location = new Point(10, 35),
                Size = new Size(200, 20),
                Checked = true
            };

            var chkShowCountdown = new CheckBox
            {
                Text = "Show countdown timer",
                Location = new Point(10, 60),
                Size = new Size(200, 20),
                Checked = true
            };

            panel.Controls.Add(chkShowIcons);
            panel.Controls.Add(chkShowNames);
            panel.Controls.Add(chkShowCountdown);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// グリッド設定パネルを作成
        /// </summary>
        public static TabPage CreateGridPanel()
        {
            var tabPage = new TabPage("Grid");
            tabPage.Name = "tabGrid";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // グリッド設定のUI要素を作成
            var lblColumns = new Label
            {
                Text = "Columns:",
                Location = new Point(10, 10),
                Size = new Size(80, 20)
            };

            var numColumns = new NumericUpDown
            {
                Location = new Point(100, 10),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 10,
                Value = 3
            };

            var lblRows = new Label
            {
                Text = "Rows:",
                Location = new Point(10, 40),
                Size = new Size(80, 20)
            };

            var numRows = new NumericUpDown
            {
                Location = new Point(100, 40),
                Size = new Size(60, 20),
                Minimum = 1,
                Maximum = 10,
                Value = 3
            };

            panel.Controls.Add(lblColumns);
            panel.Controls.Add(numColumns);
            panel.Controls.Add(lblRows);
            panel.Controls.Add(numRows);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// プライバシー設定パネルを作成
        /// </summary>
        public static TabPage CreatePrivacyPanel()
        {
            var tabPage = new TabPage("Privacy");
            tabPage.Name = "tabPrivacy";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // プライバシー設定のUI要素を作成
            var chkLogURLs = new CheckBox
            {
                Text = "Log URLs",
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                Checked = false
            };

            var chkSaveHistory = new CheckBox
            {
                Text = "Save browser history",
                Location = new Point(10, 35),
                Size = new Size(200, 20),
                Checked = false
            };

            var chkAnalytics = new CheckBox
            {
                Text = "Enable analytics",
                Location = new Point(10, 60),
                Size = new Size(200, 20),
                Checked = false
            };

            panel.Controls.Add(chkLogURLs);
            panel.Controls.Add(chkSaveHistory);
            panel.Controls.Add(chkAnalytics);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// スタートアップ設定パネルを作成
        /// </summary>
        public static TabPage CreateStartupPanel()
        {
            var tabPage = new TabPage("Startup");
            tabPage.Name = "tabStartup";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // スタートアップ設定のUI要素を作成
            var chkStartWithWindows = new CheckBox
            {
                Text = "Start with Windows",
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                Checked = false
            };

            var chkMinimizeToTray = new CheckBox
            {
                Text = "Minimize to system tray",
                Location = new Point(10, 35),
                Size = new Size(200, 20),
                Checked = true
            };

            var chkCheckForUpdates = new CheckBox
            {
                Text = "Check for updates on startup",
                Location = new Point(10, 60),
                Size = new Size(200, 20),
                Checked = true
            };

            panel.Controls.Add(chkStartWithWindows);
            panel.Controls.Add(chkMinimizeToTray);
            panel.Controls.Add(chkCheckForUpdates);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }

        /// <summary>
        /// その他設定パネルを作成
        /// </summary>
        public static TabPage CreateOthersPanel()
        {
            var tabPage = new TabPage("Others");
            tabPage.Name = "tabOthers";
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // その他設定のUI要素を作成
            var chkEnableSounds = new CheckBox
            {
                Text = "Enable sounds",
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                Checked = true
            };

            var chkShowTooltips = new CheckBox
            {
                Text = "Show tooltips",
                Location = new Point(10, 35),
                Size = new Size(200, 20),
                Checked = true
            };

            var chkConfirmClose = new CheckBox
            {
                Text = "Confirm before closing",
                Location = new Point(10, 60),
                Size = new Size(200, 20),
                Checked = false
            };

            panel.Controls.Add(chkEnableSounds);
            panel.Controls.Add(chkShowTooltips);
            panel.Controls.Add(chkConfirmClose);
            
            tabPage.Controls.Add(panel);
            return tabPage;
        }
    }
}
