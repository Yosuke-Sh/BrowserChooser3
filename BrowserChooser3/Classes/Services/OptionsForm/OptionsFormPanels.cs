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
                Size = new Size(630, 420),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = !IsTestEnvironment(), // テスト環境ではDragDropを無効化
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
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 52),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var cloneButton = new Button
            {
                Name = "btnClone",
                Text = "Clone",
                Location = new Point(6, 98),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var detectButton = new Button
            {
                Name = "btnDetect",
                Text = "Detect",
                Location = new Point(6, 144),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 190),
                Size = new Size(85, 40),
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
                Size = new Size(630, 420),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = !IsTestEnvironment(), // テスト環境ではDragDropを無効化
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
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 52),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 98),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveUpButton = new Button
            {
                Text = "Move Up",
                Location = new Point(6, 144),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var moveDownButton = new Button
            {
                Text = "Move Down",
                Location = new Point(6, 188),
                Size = new Size(85, 40),
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
                Size = new Size(630, 420),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = !IsTestEnvironment(), // テスト環境ではDragDropを無効化
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
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 52),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 98),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 144),
                Size = new Size(85, 40),
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
                Size = new Size(630, 420),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowDrop = !IsTestEnvironment(), // テスト環境ではDragDropを無効化
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
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            var editButton = new Button
            {
                Name = "btnEdit",
                Text = "Edit",
                Location = new Point(6, 52),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var deleteButton = new Button
            {
                Name = "btnDelete",
                Text = "Delete",
                Location = new Point(6, 98),
                Size = new Size(85, 40),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                Enabled = false
            };

            var selectDefaultButton = new Button
            {
                Text = "Select Default App",
                Location = new Point(6, 144),
                Size = new Size(85, 40),
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
                Padding = new Padding(10),
                AutoScroll = true
            };

            int currentY = 6;

            // === Visual Settings ===
            var lblVisualTitle = new Label
            {
                Text = "Visual Settings",
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;

            // アクセシビリティボタン
            var accessibilityButton = new Button
            {
                Name = "btnAccessibility",
                Text = "Accessibility Settings",
                Location = new Point(6, currentY),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var lblAccessibilityDesc = new Label
            {
                Text = "フォーカス表示やキーボードナビゲーションの設定を行います",
                Location = new Point(220, currentY + 10),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 50;

            // 背景色ボタン
            var backgroundColorButton = new Button
            {
                Name = "btnBackgroundColor",
                Text = "Change Background Color",
                Location = new Point(6, currentY),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            // 背景色表示用PictureBox（ChangeBackgroundボタンの横に配置）
            var pbBackgroundColor = new PictureBox
            {
                Name = "pbBackgroundColor",
                Location = new Point(220, currentY),
                Size = new Size(40, 40),
                BackColor = settings.BackgroundColorValue,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblBackgroundColorDesc = new Label
            {
                Text = "メイン画面の背景色を変更します",
                Location = new Point(270, currentY + 10),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 50;

            // 透明化設定チェックボックス
            var chkEnableTransparency = new CheckBox
            {
                Name = "chkEnableTransparency",
                Text = "Enable Transparency",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Checked = settings.EnableTransparency,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkEnableTransparency.CheckedChanged += (s, e) => setModified(true);

            var lblEnableTransparencyDesc = new Label
            {
                Text = "メイン画面を透明化します",
                Location = new Point(220, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 35;

            // 透明化色設定
            var lblTransparencyColor = new Label
            {
                Text = "Transparency Color:",
                Location = new Point(6, currentY - 5),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var pbTransparencyColor = new PictureBox
            {
                Name = "pbTransparencyColor",
                Location = new Point(130, currentY),
                Size = new Size(30, 23),
                BackColor = Color.FromArgb(settings.TransparencyColor),
                BorderStyle = BorderStyle.FixedSingle
            };
            pbTransparencyColor.Click += (s, e) =>
            {
                // テスト環境ではダイアログを表示しない
                if (IsTestEnvironment())
                {
                    return;
                }

                using var colorDialog = new ColorDialog
                {
                    Color = Color.FromArgb(settings.TransparencyColor)
                };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    pbTransparencyColor.BackColor = colorDialog.Color;
                    settings.TransparencyColor = colorDialog.Color.ToArgb();
                    setModified(true);
                }
            };

            var lblTransparencyColorDesc = new Label
            {
                Text = "透明化に使用する色を設定します",
                Location = new Point(170, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 35;

            // 透明度設定
            var lblOpacity = new Label
            {
                Text = "Opacity:",
                Location = new Point(6, currentY),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudOpacity = new NumericUpDown
            {
                Name = "nudOpacity",
                Location = new Point(90, currentY - 3),
                Size = new Size(60, 23),
                DecimalPlaces = 2,
                Increment = 0.01m,
                Minimum = 0.01m,
                Maximum = 1.00m,
                Value = (decimal)settings.Opacity,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudOpacity.ValueChanged += (s, e) => setModified(true);

            var lblOpacityDesc = new Label
            {
                Text = "透明度を設定します（0.01-1.00）",
                Location = new Point(160, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 35;

            // タイトルバー非表示設定
            var chkHideTitleBar = new CheckBox
            {
                Name = "chkHideTitleBar",
                Text = "Hide Title Bar",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Checked = settings.HideTitleBar,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkHideTitleBar.CheckedChanged += (s, e) => setModified(true);

            var lblHideTitleBarDesc = new Label
            {
                Text = "メイン画面のタイトルバーを非表示にします",
                Location = new Point(220, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 35;

            // 角を丸くする設定
            var lblRoundedCorners = new Label
            {
                Text = "Rounded Corners Radius:",
                Location = new Point(6, currentY + 3),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudRoundedCorners = new NumericUpDown
            {
                Name = "nudRoundedCorners",
                Location = new Point(160, currentY),
                Size = new Size(60, 25),
                Minimum = 0,
                Maximum = 50,
                Value = settings.RoundedCornersRadius,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudRoundedCorners.ValueChanged += (s, e) => setModified(true);

            var lblRoundedCornersDesc = new Label
            {
                Text = "メイン画面の角を丸くする半径を設定します（0で無効、1-50で有効）",
                Location = new Point(230, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 40;

            // === Display Effects ===
            // レイアウト変数の定義
            const int displayBaseX = 6;           // 基本X位置
            int displayBaseY = currentY;          // 基本Y位置
            const int displayItemSpacing = 35;    // アイテム間の間隔
            const int displayDescOffset = 250;    // 説明文のXオフセット

            var lblEffectsTitle = new Label
            {
                Text = "Display Effects",
                Location = new Point(displayBaseX, displayBaseY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;



            // アクセシブルレンダリング使用設定
            var chkUseAccessibleRendering = new CheckBox
            {
                Name = "chkUseAccessibleRendering",
                Text = "Use Accessible Rendering",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(displayBaseX, currentY),
                Size = new Size(240, 25),
                Checked = settings.UseAccessibleRendering,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkUseAccessibleRendering.CheckedChanged += (s, e) => setModified(true);

            var lblUseAccessibleRenderingDesc = new Label
            {
                Text = "アクセシビリティ対応のレンダリングを使用します",
                Location = new Point(displayDescOffset, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += displayItemSpacing;

            // === Focus Settings ===
            // レイアウト変数の定義
            const int focusBaseX = 6;           // 基本X位置
            int focusBaseY = currentY;          // 基本Y位置
            const int focusItemSpacing = 35;    // アイテム間の間隔
            const int focusDescOffset = 250;    // 説明文のXオフセット

            var lblFocusTitle = new Label
            {
                Text = "Focus Settings",
                Location = new Point(focusBaseX, focusBaseY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;

            // フォーカス表示設定
            var chkShowFocus = new CheckBox
            {
                Name = "chkShowFocus",
                Text = "Show Focus",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(focusBaseX, currentY),
                Size = new Size(200, 25),
                Checked = settings.ShowFocus,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkShowFocus.CheckedChanged += (s, e) => setModified(true);

            var lblShowFocusDesc = new Label
            {
                Text = "キーボードフォーカスを視覚的に表示します",
                Location = new Point(focusDescOffset, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += focusItemSpacing;

            // 視覚的フォーカス表示設定
            var chkShowVisualFocus = new CheckBox
            {
                Name = "chkShowVisualFocus",
                Text = "Show Visual Focus",
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(focusBaseX, currentY),
                Size = new Size(200, 25),
                Checked = settings.ShowVisualFocus,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkShowVisualFocus.CheckedChanged += (s, e) => setModified(true);

            var lblShowVisualFocusDesc = new Label
            {
                Text = "視覚的なフォーカスボックスを表示します",
                Location = new Point(focusDescOffset, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += focusItemSpacing;

            // フォーカスボックス線幅設定
            var lblFocusBoxLineWidth = new Label
            {
                Text = "Focus Box Line Width:",
                Location = new Point(focusBaseX, currentY),
                Size = new Size(190, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudFocusBoxLineWidth = new NumericUpDown
            {
                Name = "nudFocusBoxLineWidth",
                Location = new Point(focusBaseX + 190, currentY - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 10,
                Value = settings.FocusBoxLineWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudFocusBoxLineWidth.ValueChanged += (s, e) => setModified(true);

            var lblFocusBoxLineWidthDesc = new Label
            {
                Text = "フォーカスボックスの線の太さを設定します",
                Location = new Point(focusDescOffset + 10, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += focusItemSpacing;

            // フォーカスボックス幅設定
            var lblFocusBoxWidth = new Label
            {
                Text = "Focus Box Width:",
                Location = new Point(focusBaseX, currentY),
                Size = new Size(190, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudFocusBoxWidth = new NumericUpDown
            {
                Name = "nudFocusBoxWidth",
                Location = new Point(focusBaseX + 190, currentY - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 20,
                Value = settings.FocusBoxWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudFocusBoxWidth.ValueChanged += (s, e) => setModified(true);

            var lblFocusBoxWidthDesc = new Label
            {
                Text = "フォーカスボックスの幅を設定します",
                Location = new Point(focusDescOffset + 10, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += focusItemSpacing;

            // フォーカスボックス色設定
            var lblFocusBoxColor = new Label
            {
                Text = "Focus Box Color:",
                Location = new Point(focusBaseX, currentY - 5),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var pbFocusBoxColor = new PictureBox
            {
                Name = "pbFocusBoxColor",
                Location = new Point(focusBaseX + 100, currentY),
                Size = new Size(30, 23),
                BackColor = Color.FromArgb(settings.FocusBoxColor),
                BorderStyle = BorderStyle.FixedSingle
            };
            pbFocusBoxColor.Click += (s, e) =>
            {
                // テスト環境ではダイアログを表示しない
                if (IsTestEnvironment())
                {
                    return;
                }

                using var colorDialog = new ColorDialog
                {
                    Color = Color.FromArgb(settings.FocusBoxColor)
                };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    pbFocusBoxColor.BackColor = colorDialog.Color;
                    settings.FocusBoxColor = colorDialog.Color.ToArgb();
                    setModified(true);
                }
            };

            var lblFocusBoxColorDesc = new Label
            {
                Text = "フォーカスボックスの色を設定します",
                Location = new Point(focusDescOffset, currentY + 3),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // コントロールの追加
            panel.Controls.Add(lblVisualTitle);
            panel.Controls.Add(accessibilityButton);
            panel.Controls.Add(lblAccessibilityDesc);
            panel.Controls.Add(backgroundColorButton);
            panel.Controls.Add(lblBackgroundColorDesc);
            panel.Controls.Add(pbBackgroundColor);
            panel.Controls.Add(chkEnableTransparency);
            panel.Controls.Add(lblEnableTransparencyDesc);
            panel.Controls.Add(lblTransparencyColor);
            panel.Controls.Add(pbTransparencyColor);
            panel.Controls.Add(lblTransparencyColorDesc);
            panel.Controls.Add(lblOpacity);
            panel.Controls.Add(nudOpacity);
            panel.Controls.Add(lblOpacityDesc);
            panel.Controls.Add(chkHideTitleBar);
            panel.Controls.Add(lblHideTitleBarDesc);
            panel.Controls.Add(lblRoundedCorners);
            panel.Controls.Add(nudRoundedCorners);
            panel.Controls.Add(lblRoundedCornersDesc);
            
            panel.Controls.Add(lblEffectsTitle);
            panel.Controls.Add(chkUseAccessibleRendering);
            panel.Controls.Add(lblUseAccessibleRenderingDesc);
            
            panel.Controls.Add(lblFocusTitle);
            panel.Controls.Add(chkShowFocus);
            panel.Controls.Add(lblShowFocusDesc);
            panel.Controls.Add(chkShowVisualFocus);
            panel.Controls.Add(lblShowVisualFocusDesc);
            panel.Controls.Add(lblFocusBoxLineWidth);
            panel.Controls.Add(nudFocusBoxLineWidth);
            panel.Controls.Add(lblFocusBoxLineWidthDesc);
            panel.Controls.Add(lblFocusBoxWidth);
            panel.Controls.Add(nudFocusBoxWidth);
            panel.Controls.Add(lblFocusBoxWidthDesc);
            panel.Controls.Add(lblFocusBoxColor);
            panel.Controls.Add(pbFocusBoxColor);
            panel.Controls.Add(lblFocusBoxColorDesc);

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

            // レイアウト変数の定義
            const int baseX = 6;           // 基本X位置
            const int baseY = 6;           // 基本Y位置
            const int groupSpacing = 105;   // GroupBox間の間隔
            const int contentOffset = 28;  // GroupBox内のコンテンツオフセット
            const int labelWidth = 100;    // Widthラベルの幅
            const int controlSpacing = 10; // コントロール間の間隔

            // グリッドサイズ設定
            var groupBox1 = new GroupBox
            {
                Text = "Grid Size",
                Location = new Point(baseX, baseY),
                Size = new Size(400, 70),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var lblGridWidth = new Label
            {
                Text = "Width:",
                Location = new Point(controlSpacing, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridWidth = new NumericUpDown
            {
                Name = "nudGridWidth",
                Location = new Point(controlSpacing + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 20,
                Value = settings.GridWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridWidth.ValueChanged += (s, e) => setModified(true);

            var lblGridHeight = new Label
            {
                Text = "Height:",
                Location = new Point(controlSpacing + labelWidth + 80, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridHeight = new NumericUpDown
            {
                Name = "nudGridHeight",
                Location = new Point(controlSpacing + labelWidth + 80 + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 20,
                Value = settings.GridHeight,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridHeight.ValueChanged += (s, e) => setModified(true);

            // Grid Size説明文
            var lblGridSizeDesc = new Label
            {
                Text = "ブラウザボタンのグリッドレイアウトの列数と行数を設定します",
                Location = new Point(baseX, baseY + 75),
                Size = new Size(550, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // アイコンサイズ設定
            var groupBox2 = new GroupBox
            {
                Text = "Icon Size",
                Location = new Point(baseX, baseY + groupSpacing),
                Size = new Size(550, 70),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var lblIconWidth = new Label
            {
                Text = "Width:",
                Location = new Point(controlSpacing, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudIconWidth = new NumericUpDown
            {
                Name = "nudIconSizeWidth",
                Location = new Point(controlSpacing + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 1000,
                Value = settings.IconWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudIconWidth.ValueChanged += (s, e) => setModified(true);

            var lblIconHeight = new Label
            {
                Text = "Height:",
                Location = new Point(controlSpacing + labelWidth + 80, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudIconHeight = new NumericUpDown
            {
                Name = "nudIconSizeHeight",
                Location = new Point(controlSpacing + labelWidth + 80 + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 1000,
                Value = settings.IconHeight,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudIconHeight.ValueChanged += (s, e) => setModified(true);

            var lblIconScale = new Label
            {
                Text = "Scale:",
                Location = new Point(controlSpacing + labelWidth + 80 + labelWidth + 80, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudIconScale = new NumericUpDown
            {
                Name = "nudIconScale",
                Location = new Point(controlSpacing + labelWidth + 80 + labelWidth + 80 + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                DecimalPlaces = 1,
                Increment = 0.1m,
                Minimum = 0.1m,
                Maximum = 5.0m,
                Value = (decimal)settings.IconScale,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudIconScale.ValueChanged += (s, e) => setModified(true);

            // Icon Size説明文
            var lblIconSizeDesc = new Label
            {
                Text = "ブラウザボタンのアイコンサイズとスケールを設定します",
                Location = new Point(baseX, baseY + groupSpacing + 75),
                Size = new Size(550, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // アイコン間隔設定
            var groupBox3 = new GroupBox
            {
                Text = "Icon Gap (can be negative)",
                Location = new Point(baseX, baseY + groupSpacing * 2),
                Size = new Size(400, 70),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var lblGapWidth = new Label
            {
                Text = "Width:",
                Location = new Point(controlSpacing, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGapWidth = new NumericUpDown
            {
                Name = "nudIconGapWidth",
                Location = new Point(controlSpacing + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = -100,
                Maximum = 100,
                Value = settings.IconGapWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGapWidth.ValueChanged += (s, e) => setModified(true);

            var lblGapHeight = new Label
            {
                Text = "Height:",
                Location = new Point(controlSpacing + labelWidth + 80, contentOffset),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGapHeight = new NumericUpDown
            {
                Name = "nudIconGapHeight",
                Location = new Point(controlSpacing + labelWidth + 80 + labelWidth, contentOffset - 3),
                Size = new Size(60, 23),
                Minimum = -100,
                Maximum = 100,
                Value = settings.IconGapHeight,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGapHeight.ValueChanged += (s, e) => setModified(true);

            // Icon Gap説明文
            var lblIconGapDesc = new Label
            {
                Text = "ブラウザボタン間の間隔を設定します（負の値で重ねることができます）",
                Location = new Point(baseX, baseY + groupSpacing * 2 + 75),
                Size = new Size(600, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // グリッド表示設定
            var chkShowGrid = new CheckBox
            {
                Name = "chkShowGrid",
                Text = "Show Grid",
                Location = new Point(baseX, baseY + groupSpacing * 3),
                Size = new Size(180, 25),
                Checked = settings.ShowGrid,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkShowGrid.CheckedChanged += (s, e) => setModified(true);

            // Show Grid説明文
            var lblShowGridDesc = new Label
            {
                Text = "ブラウザボタンの背景にグリッド線を表示します",
                Location = new Point(baseX + labelWidth + 90, baseY + groupSpacing * 3 + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // グリッド線の色設定
            var lblGridColor = new Label
            {
                Text = "Grid Color:",
                Location = new Point(baseX, baseY + groupSpacing * 3 + 35),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var pbGridColor = new Panel
            {
                Name = "pbGridColor",
                Location = new Point(baseX + labelWidth, baseY + groupSpacing * 3 + 35),
                Size = new Size(80, 23),
                BackColor = Color.FromArgb(settings.GridColor),
                BorderStyle = BorderStyle.FixedSingle
            };
            pbGridColor.Click += (s, e) =>
            {
                // テスト環境ではダイアログを表示しない
                if (IsTestEnvironment())
                {
                    return;
                }

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

            // Grid Color説明文
            var lblGridColorDesc = new Label
            {
                Text = "グリッド線の色を設定します（クリックで色選択ダイアログを開きます）",
                Location = new Point(baseX + labelWidth + 90, baseY + groupSpacing * 3 + 35 + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // グリッド線の太さ設定
            var lblGridLineWidth = new Label
            {
                Text = "Line Width:",
                Location = new Point(baseX, baseY + groupSpacing * 3 + 70),
                Size = new Size(labelWidth, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudGridLineWidth = new NumericUpDown
            {
                Name = "nudGridLineWidth",
                Location = new Point(baseX + labelWidth, baseY + groupSpacing * 3 + 70 - 3),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 10,
                Value = settings.GridLineWidth,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudGridLineWidth.ValueChanged += (s, e) => setModified(true);

            // Line Width説明文
            var lblGridLineWidthDesc = new Label
            {
                Text = "グリッド線の太さを設定します（1-10ピクセル）",
                Location = new Point(baseX + labelWidth + 90, baseY + groupSpacing * 3 + 70 + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // コントロールの追加
            groupBox1.Controls.Add(lblGridWidth);
            groupBox1.Controls.Add(nudGridWidth);
            groupBox1.Controls.Add(lblGridHeight);
            groupBox1.Controls.Add(nudGridHeight);

            groupBox2.Controls.Add(lblIconWidth);
            groupBox2.Controls.Add(nudIconWidth);
            groupBox2.Controls.Add(lblIconHeight);
            groupBox2.Controls.Add(nudIconHeight);
            groupBox2.Controls.Add(lblIconScale);
            groupBox2.Controls.Add(nudIconScale);

            groupBox3.Controls.Add(lblGapWidth);
            groupBox3.Controls.Add(nudGapWidth);
            groupBox3.Controls.Add(lblGapHeight);
            groupBox3.Controls.Add(nudGapHeight);

            panel.Controls.Add(groupBox1);
            panel.Controls.Add(groupBox2);
            panel.Controls.Add(groupBox3);
            panel.Controls.Add(chkShowGrid);
            panel.Controls.Add(lblGridColor);
            panel.Controls.Add(pbGridColor);
            panel.Controls.Add(lblGridLineWidth);
            panel.Controls.Add(nudGridLineWidth);
            panel.Controls.Add(lblGridSizeDesc);
            panel.Controls.Add(lblIconSizeDesc);
            panel.Controls.Add(lblIconGapDesc);
            panel.Controls.Add(lblShowGridDesc);
            panel.Controls.Add(lblGridColorDesc);
            panel.Controls.Add(lblGridLineWidthDesc);

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
                Size = new Size(220, 25),
                Checked = settings.EnableLogging,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkEnableLogging.CheckedChanged += (s, e) => setModified(true);

            // Enable Logging説明文
            var lblEnableLoggingDesc = new Label
            {
                Text = "アプリケーションの動作ログを記録します",
                Location = new Point(270, 9),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // ログレベル設定
            var lblLogLevel = new Label
            {
                Text = "Log Level:",
                Location = new Point(6, 39),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var cmbLogLevel = new ComboBox
            {
                Name = "cmbLogLevel",
                Location = new Point(130, 36),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            cmbLogLevel.Items.AddRange(new object[] { "Trace", "Debug", "Info", "Warning", "Error" });
            cmbLogLevel.SelectedIndex = Math.Min(settings.LogLevel, cmbLogLevel.Items.Count - 1);
            cmbLogLevel.SelectedIndexChanged += (s, e) => setModified(true);

            // Log Level説明文
            var lblLogLevelDesc = new Label
            {
                Text = "ログの詳細レベルを設定します（Traceが最も詳細）",
                Location = new Point(270, 39),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 履歴保持設定
            var chkKeepHistory = new CheckBox
            {
                Name = "chkKeepHistory",
                Text = "Keep Browser History",
                Location = new Point(6, 69),
                Size = new Size(250, 25),
                Checked = settings.KeepHistory,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkKeepHistory.CheckedChanged += (s, e) => setModified(true);

            // Keep History説明文
            var lblKeepHistoryDesc = new Label
            {
                Text = "ブラウザの使用履歴を保持します",
                Location = new Point(270, 72),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 履歴保持日数
            var lblHistoryDays = new Label
            {
                Text = "History Days:",
                Location = new Point(6, 102),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudHistoryDays = new NumericUpDown
            {
                Name = "nudHistoryDays",
                Location = new Point(160, 99),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 365,
                Value = settings.HistoryDays,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudHistoryDays.ValueChanged += (s, e) => setModified(true);

            // History Days説明文
            var lblHistoryDaysDesc = new Label
            {
                Text = "履歴を保持する日数を設定します（1-365日）",
                Location = new Point(270, 102),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // プライバシーモード設定
            var chkPrivacyMode = new CheckBox
            {
                Name = "chkPrivacyMode",
                Text = "Privacy Mode (Clear on Exit)",
                Location = new Point(6, 132),
                Size = new Size(260, 25),
                Checked = settings.PrivacyMode,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkPrivacyMode.CheckedChanged += (s, e) => setModified(true);

            // Privacy Mode説明文
            var lblPrivacyModeDesc = new Label
            {
                Text = "アプリケーション終了時に履歴とログをクリアします",
                Location = new Point(270, 135),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // データ収集設定
            var chkAllowDataCollection = new CheckBox
            {
                Name = "chkAllowDataCollection",
                Text = "Allow Data Collection",
                Location = new Point(6, 165),
                Size = new Size(260, 25),
                Checked = settings.AllowDataCollection,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAllowDataCollection.CheckedChanged += (s, e) => setModified(true);

            // Allow Data Collection説明文
            var lblAllowDataCollectionDesc = new Label
            {
                Text = "アプリケーションの改善のための匿名データ収集を許可します",
                Location = new Point(270, 168),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // コントロールの追加
            panel.Controls.Add(chkEnableLogging);
            panel.Controls.Add(lblLogLevel);
            panel.Controls.Add(cmbLogLevel);
            panel.Controls.Add(chkKeepHistory);
            panel.Controls.Add(lblHistoryDays);
            panel.Controls.Add(nudHistoryDays);
            panel.Controls.Add(chkPrivacyMode);
            panel.Controls.Add(chkAllowDataCollection);
            panel.Controls.Add(lblEnableLoggingDesc);
            panel.Controls.Add(lblLogLevelDesc);
            panel.Controls.Add(lblKeepHistoryDesc);
            panel.Controls.Add(lblHistoryDaysDesc);
            panel.Controls.Add(lblPrivacyModeDesc);
            panel.Controls.Add(lblAllowDataCollectionDesc);

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
                Size = new Size(200, 25),
                Checked = settings.AutoStart,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAutoStart.CheckedChanged += (s, e) => setModified(true);

            var lblAutoStartDesc = new Label
            {
                Text = "Windows起動時に自動的にBrowser Chooserを起動します",
                Location = new Point(210, 9),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 最小化で起動設定
            var chkStartMinimized = new CheckBox
            {
                Name = "chkStartMinimized",
                Text = "Start Minimized",
                Location = new Point(6, 39),
                Size = new Size(200, 25),
                Checked = settings.StartMinimized,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkStartMinimized.CheckedChanged += (s, e) => setModified(true);

            var lblStartMinimizedDesc = new Label
            {
                Text = "起動時にメイン画面を最小化状態で表示します",
                Location = new Point(210, 42),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // システムトレイ設定
            var chkStartInTray = new CheckBox
            {
                Name = "chkStartInTray",
                Text = "Start in System Tray",
                Location = new Point(6, 72),
                Size = new Size(200, 25),
                Checked = settings.StartInTray,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkStartInTray.CheckedChanged += (s, e) => setModified(true);

            var lblStartInTrayDesc = new Label
            {
                Text = "起動時にシステムトレイに最小化して表示します",
                Location = new Point(210, 75),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 起動時のチェック設定
            var chkCheckDefaultOnStartup = new CheckBox
            {
                Name = "chkCheckDefaultOnStartup",
                Text = "Check Default Browser on Startup",
                Location = new Point(6, 105),
                Size = new Size(300, 25),
                Checked = settings.CheckDefaultOnLaunch,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkCheckDefaultOnStartup.CheckedChanged += (s, e) => setModified(true);

            var lblCheckDefaultDesc = new Label
            {
                Text = "起動時にデフォルトブラウザの変更をチェックします",
                Location = new Point(310, 108),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 起動遅延設定
            var lblStartupDelay = new Label
            {
                Text = "Startup Delay (ms):",
                Location = new Point(6, 138),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudStartupDelay = new NumericUpDown
            {
                Name = "nudStartupDelay",
                Location = new Point(160, 135),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 10000,
                Value = settings.StartupDelay,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudStartupDelay.ValueChanged += (s, e) => setModified(true);

            var lblStartupDelayDesc = new Label
            {
                Text = "起動時の遅延時間をミリ秒で設定します（0-10000ms）",
                Location = new Point(250, 138),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // 起動時のメッセージ設定
            var lblStartupMessage = new Label
            {
                Text = "Startup Message:",
                Location = new Point(6, 171),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtStartupMessage = new TextBox
            {
                Name = "txtStartupMessage",
                Location = new Point(160, 168),
                Size = new Size(200, 23),
                Text = settings.StartupMessage,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtStartupMessage.TextChanged += (s, e) => setModified(true);

            var lblStartupMessageDesc = new Label
            {
                Text = "起動時に表示するメッセージを設定します",
                Location = new Point(370, 171),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // コントロールの追加
            panel.Controls.Add(chkAutoStart);
            panel.Controls.Add(lblAutoStartDesc);
            panel.Controls.Add(chkStartMinimized);
            panel.Controls.Add(lblStartMinimizedDesc);
            panel.Controls.Add(chkStartInTray);
            panel.Controls.Add(lblStartInTrayDesc);
            panel.Controls.Add(chkCheckDefaultOnStartup);
            panel.Controls.Add(lblCheckDefaultDesc);
            panel.Controls.Add(lblStartupDelay);
            panel.Controls.Add(nudStartupDelay);
            panel.Controls.Add(lblStartupDelayDesc);
            panel.Controls.Add(lblStartupMessage);
            panel.Controls.Add(txtStartupMessage);
            panel.Controls.Add(lblStartupMessageDesc);

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
                Padding = new Padding(10),
                AutoScroll = true
            };

            int currentY = 6;

            // === General Settings ===
            var lblGeneralTitle = new Label
            {
                Text = "General Settings",
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;

            // ポータブルモード設定
            var chkPortableMode = new CheckBox
            {
                Name = "chkPortableMode",
                Text = "Portable Mode",
                Location = new Point(6, currentY),
                Size = new Size(150, 25),
                Checked = settings.PortableMode,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkPortableMode.CheckedChanged += (s, e) => setModified(true);

            var lblPortableModeDesc = new Label
            {
                Text = "設定ファイルをアプリケーションフォルダに保存します（USBメモリなどで持ち運び可能）",
                Location = new Point(160, currentY + 3),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 自動更新設定
            var chkAutoCheckUpdate = new CheckBox
            {
                Name = "chkAutoCheckUpdate",
                Text = "Check for Updates Automatically",
                Location = new Point(6, currentY),
                Size = new Size(300, 25),
                Checked = settings.AutomaticUpdates,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAutoCheckUpdate.CheckedChanged += (s, e) => setModified(true);

            var lblAutoCheckUpdateDesc = new Label
            {
                Text = "起動時に自動的にアップデートをチェックします",
                Location = new Point(310, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 詳細画面設定
            var chkAdvanced = new CheckBox
            {
                Name = "chkAdvanced",
                Text = "Show Advanced Options",
                Location = new Point(6, currentY),
                Size = new Size(250, 25),
                Checked = settings.AdvancedScreens,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAdvanced.CheckedChanged += (s, e) => setModified(true);

            var lblAdvancedDesc = new Label
            {
                Text = "上級者向けの詳細設定オプションを表示します",
                Location = new Point(260, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 開いたまま許可設定
            var chkAllowStayOpen = new CheckBox
            {
                Name = "chkAllowStayOpen",
                Text = "Allow Window to Stay Open",
                Location = new Point(6, currentY),
                Size = new Size(300, 25),
                Checked = settings.AllowStayOpen,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAllowStayOpen.CheckedChanged += (s, e) => setModified(true);

            var lblAllowStayOpenDesc = new Label
            {
                Text = "ブラウザ選択後もウィンドウを開いたままにします",
                Location = new Point(310, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 40;

            // === Advanced Settings ===
            var lblAdvancedTitle = new Label
            {
                Text = "Advanced Settings",
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;

            // デフォルト遅延設定
            var lblDefaultDelay = new Label
            {
                Text = "Default Delay (ms):",
                Location = new Point(6, currentY),
                Size = new Size(180, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var nudDefaultDelay = new NumericUpDown
            {
                Name = "nudDefaultDelay",
                Location = new Point(190, currentY - 3),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 10000,
                Value = settings.DefaultDelay,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            nudDefaultDelay.ValueChanged += (s, e) => setModified(true);

            var lblDefaultDelayDesc = new Label
            {
                Text = "ブラウザ起動時のデフォルト遅延時間をミリ秒で設定します",
                Location = new Point(280, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // セパレーター設定
            var lblSeparator = new Label
            {
                Text = "Separator:",
                Location = new Point(6, currentY),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtSeparator = new TextBox
            {
                Name = "txtSeparator",
                Location = new Point(110, currentY - 3),
                Size = new Size(50, 23),
                Text = settings.Separator,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtSeparator.TextChanged += (s, e) => setModified(true);

            var lblSeparatorDesc = new Label
            {
                Text = "URLとブラウザ引数の間のセパレーター文字を設定します",
                Location = new Point(170, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // ユーザーエージェント設定
            var lblUserAgent = new Label
            {
                Text = "User Agent:",
                Location = new Point(6, currentY),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtUserAgent = new TextBox
            {
                Name = "txtUserAgent",
                Location = new Point(130, currentY - 3),
                Size = new Size(200, 23),
                Text = settings.UserAgent,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtUserAgent.TextChanged += (s, e) => setModified(true);

            var lblUserAgentDesc = new Label
            {
                Text = "ブラウザで使用するユーザーエージェント文字列を設定します",
                Location = new Point(340, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 正規化設定
            var chkCanonicalize = new CheckBox
            {
                Name = "chkCanonicalize",
                Text = "Canonicalize URLs",
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Checked = settings.Canonicalize,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkCanonicalize.CheckedChanged += (s, e) => setModified(true);

            var lblCanonicalizeDesc = new Label
            {
                Text = "URLを正規化して標準形式に変換します",
                Location = new Point(210, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 正規化追加テキスト設定
            var lblCanonicalizeText = new Label
            {
                Text = "Canonicalize Text:",
                Location = new Point(6, currentY),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtCanonicalizeText = new TextBox
            {
                Name = "txtCanonicalizeText",
                Location = new Point(160, currentY - 3),
                Size = new Size(200, 23),
                Text = settings.CanonicalizeAppendedText,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtCanonicalizeText.TextChanged += (s, e) => setModified(true);

            var lblCanonicalizeTextDesc = new Label
            {
                Text = "正規化時に追加するテキストを設定します",
                Location = new Point(370, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 40;

            // === System Settings ===
            var lblSystemTitle = new Label
            {
                Text = "System Settings",
                Location = new Point(6, currentY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue
            };
            currentY += 34;

            // ダウンロード検出ファイル設定
            var chkDownloadDetectionFile = new CheckBox
            {
                Name = "chkDownloadDetectionFile",
                Text = "Download Detection File",
                Location = new Point(6, currentY),
                Size = new Size(250, 25),
                Checked = settings.DownloadDetectionFile,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkDownloadDetectionFile.CheckedChanged += (s, e) => setModified(true);

            var lblDownloadDetectionFileDesc = new Label
            {
                Text = "ダウンロード検出用のファイルを生成します",
                Location = new Point(260, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // ログ有効化設定
            var chkEnableLogging = new CheckBox
            {
                Name = "chkEnableLogging",
                Text = "Enable Logging",
                Location = new Point(6, currentY),
                Size = new Size(180, 25),
                Checked = settings.EnableLogging,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkEnableLogging.CheckedChanged += (s, e) => setModified(true);

            var lblEnableLoggingDesc = new Label
            {
                Text = "アプリケーションの動作ログを記録します",
                Location = new Point(190, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // DLL抽出設定
            var chkExtractDLLs = new CheckBox
            {
                Name = "chkExtractDLLs",
                Text = "Extract DLLs",
                Location = new Point(6, currentY),
                Size = new Size(150, 25),
                Checked = settings.ExtractDLLs,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkExtractDLLs.CheckedChanged += (s, e) => setModified(true);

            var lblExtractDLLsDesc = new Label
            {
                Text = "必要なDLLファイルを自動的に抽出します",
                Location = new Point(160, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // オプションショートカット設定
            var lblOptionsShortcut = new Label
            {
                Text = "Options Shortcut:",
                Location = new Point(6, currentY),
                Size = new Size(180, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtOptionsShortcut = new TextBox
            {
                Name = "txtOptionsShortcut",
                Location = new Point(190, currentY - 3),
                Size = new Size(30, 23),
                Text = settings.OptionsShortcut.ToString(),
                MaxLength = 1,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtOptionsShortcut.TextChanged += (s, e) => setModified(true);

            var lblOptionsShortcutDesc = new Label
            {
                Text = "オプション画面を開くショートカットキーを設定します",
                Location = new Point(230, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // デフォルトメッセージ設定
            var lblDefaultMessage = new Label
            {
                Text = "Default Message:",
                Location = new Point(6, currentY),
                Size = new Size(160, 23),
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            var txtDefaultMessage = new TextBox
            {
                Name = "txtDefaultMessage",
                Location = new Point(170, currentY - 3),
                Size = new Size(200, 23),
                Text = settings.DefaultMessage,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            txtDefaultMessage.TextChanged += (s, e) => setModified(true);

            var lblDefaultMessageDesc = new Label
            {
                Text = "デフォルトで表示するメッセージを設定します",
                Location = new Point(380, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };
            currentY += 34;

            // 自動起動設定
            var chkAutoStart = new CheckBox
            {
                Name = "chkAutoStart",
                Text = "Auto Start",
                Location = new Point(6, currentY),
                Size = new Size(120, 25),
                Checked = settings.AutoStart,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            chkAutoStart.CheckedChanged += (s, e) => setModified(true);

            var lblAutoStartDesc = new Label
            {
                Text = "Windows起動時に自動的にアプリケーションを起動します",
                Location = new Point(130, currentY + 3),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.Gray
            };

            // コントロールの追加
            panel.Controls.Add(lblGeneralTitle);
            panel.Controls.Add(chkPortableMode);
            panel.Controls.Add(lblPortableModeDesc);
            panel.Controls.Add(chkAutoCheckUpdate);
            panel.Controls.Add(lblAutoCheckUpdateDesc);
            panel.Controls.Add(chkAdvanced);
            panel.Controls.Add(lblAdvancedDesc);
            panel.Controls.Add(chkAllowStayOpen);
            panel.Controls.Add(lblAllowStayOpenDesc);
            
            panel.Controls.Add(lblAdvancedTitle);
            panel.Controls.Add(lblDefaultDelay);
            panel.Controls.Add(nudDefaultDelay);
            panel.Controls.Add(lblDefaultDelayDesc);
            panel.Controls.Add(lblSeparator);
            panel.Controls.Add(txtSeparator);
            panel.Controls.Add(lblSeparatorDesc);
            panel.Controls.Add(lblUserAgent);
            panel.Controls.Add(txtUserAgent);
            panel.Controls.Add(lblUserAgentDesc);
            panel.Controls.Add(chkCanonicalize);
            panel.Controls.Add(lblCanonicalizeDesc);
            panel.Controls.Add(lblCanonicalizeText);
            panel.Controls.Add(txtCanonicalizeText);
            panel.Controls.Add(lblCanonicalizeTextDesc);
            
            panel.Controls.Add(lblSystemTitle);
            panel.Controls.Add(chkDownloadDetectionFile);
            panel.Controls.Add(lblDownloadDetectionFileDesc);
            panel.Controls.Add(chkEnableLogging);
            panel.Controls.Add(lblEnableLoggingDesc);
            panel.Controls.Add(chkExtractDLLs);
            panel.Controls.Add(lblExtractDLLsDesc);
            panel.Controls.Add(lblOptionsShortcut);
            panel.Controls.Add(txtOptionsShortcut);
            panel.Controls.Add(lblOptionsShortcutDesc);
            panel.Controls.Add(lblDefaultMessage);
            panel.Controls.Add(txtDefaultMessage);
            panel.Controls.Add(lblDefaultMessageDesc);
            panel.Controls.Add(chkAutoStart);
            panel.Controls.Add(lblAutoStartDesc);

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
