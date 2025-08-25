using System.Xml.Serialization;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Services.BrowserServices;
using BrowserChooser3.Classes.Services.SystemServices;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// アプリケーション設定を管理するクラス
    /// XMLファイルとの相互変換、デフォルト値の管理、設定の保存・読み込みを行います
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// 現在の設定ファイルバージョン
        /// 将来のアップグレード検出に使用されます
        /// 
        /// バージョン履歴:
        /// - Version 2: プロトコルとファイルタイプを追加（自動追加）
        /// - Version 3: プロトコルとファイルタイプを再構築、アクセシビリティ設定を追加
        /// - Version 4: アクセシビリティ設定を再構築、v3を模倣
        /// - Version 5: レジストリチェック用（キャンセル時はバージョンを更新しない）
        /// </summary>
        public const int CURRENT_FILE_VERSION = 5;

        /// <summary>
        /// ウィンドウの開始位置を定義する列挙型
        /// </summary>
        /// <summary>利用可能な開始位置を定義する列挙型</summary>
        public enum AvailableStartingPositions
        {
            /// <summary>画面中央</summary>
            CenterScreen,
            
            /// <summary>中央からオフセット</summary>
            OffsetCenter,
            
            /// <summary>指定座標</summary>
            XY,
            
            /// <summary>左上</summary>
            TopLeft,
            
            /// <summary>右上</summary>
            TopRight,
            
            /// <summary>左下</summary>
            BottomLeft,
            
            /// <summary>右下</summary>
            BottomRight,
            
            /// <summary>左上からオフセット</summary>
            OffsetTopLeft,
            
            /// <summary>右上からオフセット</summary>
            OffsetTopRight,
            
            /// <summary>左下からオフセット</summary>
            OffsetBottomLeft,
            
            /// <summary>右下からオフセット</summary>
            OffsetBottomRight,
            
            /// <summary>セパレータ1</summary>
            Separator1 = -1,
            
            /// <summary>セパレータ2</summary>
            Separator2 = -2,
            
            /// <summary>セパレータ3</summary>
            Separator3 = -3
        }

        /// <summary>
        /// デフォルト値フィールドを定義する列挙型
        /// </summary>
        /// <summary>デフォルト値フィールドを定義する列挙型</summary>
        public enum DefaultField
        {
            /// <summary>ファイルバージョン</summary>
            FileVersion,
            
            /// <summary>アイコン幅</summary>
            IconWidth,
            
            /// <summary>アイコン高さ</summary>
            IconHeight,
            
            /// <summary>アイコン間隔幅</summary>
            IconGapWidth,
            
            /// <summary>アイコン間隔高さ</summary>
            IconGapHeight,
            
            /// <summary>アイコンスケール</summary>
            IconScale,
            
            /// <summary>オプションショートカット</summary>
            OptionsShortcut,
            
            /// <summary>デフォルトメッセージ</summary>
            DefaultMessage,
            
            /// <summary>デフォルト遅延時間</summary>
            DefaultDelay,
            
            /// <summary>デフォルトブラウザGUID</summary>
            DefaultBrowserGuid,
            
            /// <summary>自動更新</summary>
            AutomaticUpdates,
            
            /// <summary>起動時にデフォルトをチェック</summary>
            CheckDefaultOnLaunch,
            
            /// <summary>高度な画面</summary>
            AdvancedScreens,
            
            /// <summary>セパレータ</summary>
            Separator,
            
            /// <summary>フォーカス表示</summary>
            ShowFocus,
            
            /// <summary>Aero効果使用</summary>
            UseAero,
            
            /// <summary>フォーカスボックス線幅</summary>
            FocusBoxLineWidth,
            
            /// <summary>フォーカスボックス色</summary>
            FocusBoxColor,
            
            /// <summary>ユーザーエージェント</summary>
            UserAgent,
            
            /// <summary>ダウンロード検出ファイル</summary>
            DownloadDetectionFile,
            
            /// <summary>背景色</summary>
            BackgroundColor,
            
            /// <summary>開始位置</summary>
            StartingPosition,
            
            /// <summary>Xオフセット</summary>
            OffsetX,
            
            /// <summary>Yオフセット</summary>
            OffsetY,
            
            /// <summary>開いたまま許可</summary>
            AllowStayOpen,
            
            /// <summary>正規化</summary>
            Canonicalize,
            
            /// <summary>正規化追加テキスト</summary>
            CanonicalizeAppendedText,
            
            /// <summary>ログ有効化</summary>
            EnableLogging,
            
            /// <summary>DLL抽出</summary>
            ExtractDLLs,
            
            /// <summary>ログレベル</summary>
            LogLevel
        }

        /// <summary>
        /// デフォルト値の辞書
        /// この方法でデフォルト値を管理することで、後で参照できます
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        public Dictionary<DefaultField, object> Defaults = new()
        {
            { DefaultField.FileVersion, CURRENT_FILE_VERSION },
            { DefaultField.IconWidth, 75 },
            { DefaultField.IconHeight, 80 },
            { DefaultField.IconGapWidth, 0 },
            { DefaultField.IconGapHeight, 0 },
            { DefaultField.IconScale, 1.0 },
            { DefaultField.OptionsShortcut, 'O' },
            { DefaultField.DefaultMessage, "Choose a Browser" },
            { DefaultField.DefaultDelay, 5 },
            { DefaultField.DefaultBrowserGuid, Guid.Empty },
            { DefaultField.AutomaticUpdates, true },
            { DefaultField.CheckDefaultOnLaunch, false },
            { DefaultField.AdvancedScreens, false },
            { DefaultField.Separator, " - " },
            { DefaultField.ShowFocus, true },
            { DefaultField.UseAero, false },
            { DefaultField.FocusBoxLineWidth, 1 },
            { DefaultField.FocusBoxColor, Color.Transparent.ToArgb() },
            { DefaultField.UserAgent, "Mozilla/5.0" },
            { DefaultField.DownloadDetectionFile, true },
            { DefaultField.BackgroundColor, Color.Transparent.ToArgb() },
            { DefaultField.StartingPosition, AvailableStartingPositions.CenterScreen },
            { DefaultField.OffsetX, 0 },
            { DefaultField.OffsetY, 0 },
            { DefaultField.AllowStayOpen, false },
            { DefaultField.Canonicalize, false },
            { DefaultField.CanonicalizeAppendedText, string.Empty },
            { DefaultField.EnableLogging, false },
            { DefaultField.ExtractDLLs, false },
            { DefaultField.LogLevel, 3 }
        };

        /// <summary>設定ファイル名</summary>
        public const string BrowserChooserConfigFileName = "BrowserChooser3Config.xml";

        /// <summary>ブラウザリスト</summary>
        public List<Browser> Browsers { get; set; } = new();
        
        /// <summary>ポータブルモード</summary>
        public bool PortableMode { get; set; } = true;
        
        /// <summary>URL表示</summary>
        public bool ShowURL { get; set; } = true;
        
        /// <summary>URL表示（Browser Chooser 2互換）</summary>
        public bool ShowURLs { get; set; } = true;
        
        /// <summary>短縮URL展開</summary>
        public bool RevealShortURL { get; set; } = false;
        
        /// <summary>短縮URL展開（Browser Chooser 2互換）</summary>
        public bool RevealShortURLs { get; set; } = false;
        
        /// <summary>ファイルバージョン</summary>
        public int FileVersion { get; set; } = CURRENT_FILE_VERSION;
        
        /// <summary>プロトコルリスト</summary>
        public List<Protocol> Protocols { get; set; } = new();
        
        /// <summary>ファイルタイプリスト</summary>
        public List<FileType> FileTypes { get; set; } = new();
        
        /// <summary>URLリスト</summary>
        public List<URL> URLs { get; set; } = new();
        
        /// <summary>幅</summary>
        public int Width { get; set; } = 8;
        
        /// <summary>高さ</summary>
        public int Height { get; set; } = 1;
        
        /// <summary>グリッド幅（Browser Chooser 2互換）</summary>
        public int GridWidth { get; set; } = 5;
        
        /// <summary>グリッド高さ（Browser Chooser 2互換）</summary>
        public int GridHeight { get; set; } = 1;
        
        /// <summary>アイコン幅</summary>
        public int IconWidth { get; set; } = 90;
        
        /// <summary>アイコン高さ</summary>
        public int IconHeight { get; set; } = 100;
        
        /// <summary>アイコン間隔幅</summary>
        public int IconGapWidth { get; set; } = 0;
        
        /// <summary>アイコン間隔高さ</summary>
        public int IconGapHeight { get; set; } = 0;
        
        /// <summary>アイコンスケール</summary>
        public double IconScale { get; set; } = 1.0;
        
        /// <summary>オプションショートカット</summary>
        public char OptionsShortcut { get; set; } = 'O';
        
        /// <summary>デフォルトメッセージ</summary>
        public string DefaultMessage { get; set; } = "Choose a Browser";
        
        /// <summary>デフォルト遅延時間</summary>
        public int DefaultDelay { get; set; } = 5;
        
        /// <summary>デフォルトブラウザGUID</summary>
        public Guid DefaultBrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>自動更新</summary>
        public bool AutomaticUpdates { get; set; } = true;
        
        /// <summary>起動時にデフォルトをチェック</summary>
        public bool CheckDefaultOnLaunch { get; set; } = false;
        
        /// <summary>高度な画面</summary>
        public bool AdvancedScreens { get; set; } = false;
        
        /// <summary>セパレータ</summary>
        public string Separator { get; set; } = " - ";
        
        /// <summary>フォーカス表示</summary>
        public bool ShowFocus { get; set; } = true;
        
        /// <summary>Aero効果使用</summary>
        public bool UseAero { get; set; } = false;
        
        /// <summary>アクセシブルレンダリング使用</summary>
        public bool UseAccessibleRendering { get; set; } = false;
        
        /// <summary>フォーカスボックス線幅</summary>
        public int FocusBoxLineWidth { get; set; } = 1;
        
        /// <summary>フォーカスボックス色</summary>
        public int FocusBoxColor { get; set; } = Color.Transparent.ToArgb();

        /// <summary>
        /// 視覚的フォーカス表示の有効/無効
        /// </summary>
        public bool ShowVisualFocus { get; set; } = false;

        /// <summary>
        /// フォーカスボックスの幅
        /// </summary>
        public int FocusBoxWidth { get; set; } = 2;
        
        /// <summary>ユーザーエージェント</summary>
        public string UserAgent { get; set; } = "Mozilla/5.0";
        
        /// <summary>ダウンロード検出ファイル</summary>
        public bool DownloadDetectionFile { get; set; } = true;
        
        /// <summary>背景色</summary>
        public int BackgroundColor { get; set; } = Color.Transparent.ToArgb();
        
        /// <summary>背景色（Color型、Browser Chooser 2互換）</summary>
        public Color BackgroundColorValue 
        { 
            get => Color.FromArgb(BackgroundColor);
            set => BackgroundColor = value.ToArgb();
        }
        
        /// <summary>開始位置</summary>
        public int StartingPosition { get; set; } = (int)AvailableStartingPositions.CenterScreen;
        
        /// <summary>Xオフセット</summary>
        public int OffsetX { get; set; } = 0;
        
        /// <summary>Yオフセット</summary>
        public int OffsetY { get; set; } = 0;
        
        /// <summary>開いたまま許可</summary>
        public bool AllowStayOpen { get; set; } = false;
        
        /// <summary>正規化</summary>
        public bool Canonicalize { get; set; } = false;
        
        /// <summary>正規化追加テキスト</summary>
        public string CanonicalizeAppendedText { get; set; } = string.Empty;
        
        /// <summary>ログ有効化</summary>
        public bool EnableLogging { get; set; } = false;
        
        /// <summary>DLL抽出</summary>
        public bool ExtractDLLs { get; set; } = false;
        
        /// <summary>ログレベル</summary>
        public int LogLevel { get; set; } = 3;

        /// <summary>セーフモード（ファイルが読み込めない場合のみtrue - 保存を防止）</summary>
        [field: NonSerialized] public bool SafeMode { get; set; } = false;
        /// <summary>ログデバッグ（コマンドラインで指定された場合のみtrue）</summary>
        [field: NonSerialized] public static TriState LogDebugs { get; set; } = TriState.UseDefault;
        
        /// <summary>DLL抽出（コマンドラインで指定された場合のみtrue）</summary>
        [field: NonSerialized] public static bool DoExtractDLLs { get; set; } = false;

        /// <summary>
        /// 現在の設定インスタンス
        /// </summary>
        [field: NonSerialized] public static Settings Current { get; set; } = new();

        /// <summary>
        /// TriState列挙型
        /// </summary>
        /// <summary>3状態を定義する列挙型</summary>
        public enum TriState
        {
            /// <summary>デフォルト使用</summary>
            UseDefault,
            
            /// <summary>真</summary>
            True,
            
            /// <summary>偽</summary>
            False
        }

        /// <summary>
        /// Settingsクラスの新しいインスタンスを初期化します
        /// </summary>
        public Settings()
        {
            Logger.LogInfo("Settings.New (No args)", "Start");
            SharedNew();
            Logger.LogInfo("Settings.New (No args)", "End");
        }

        /// <summary>
        /// Settingsクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="error">エラー状態</param>
        public Settings(bool error)
        {
            Logger.LogInfo("Settings.New (Error)", "Start", error);
            SharedNew();
            Logger.LogInfo("Settings.New (Error)", "End", error);
        }

        private void SharedNew()
        {
            Logger.LogInfo("Settings.SharedNew", "Start");
            Browsers = new List<Browser>();
            URLs = new List<URL>();
            PortableMode = true; // default
            RevealShortURL = false; // default
            ShowURL = true; // default
            Width = 8; // default
            Height = 1; // default
            
            // ブラウザの自動検出
            DetectBrowsers();
            
            Logger.LogInfo("Settings.SharedNew", "End");
        }

        /// <summary>
        /// ブラウザを自動検出します
        /// </summary>
        private void DetectBrowsers()
        {
            Logger.LogInfo("Settings.DetectBrowsers", "Start");
            
            // 初回のみブラウザ検出を実行
            if (BrowserDetector.DetectedBrowsers.Count == 0)
            {
                BrowserDetector.DetectBrowsers();
            }
            
            var detectedBrowsers = BrowserDetector.DetectedBrowsers.ToList(); // コピーを作成
            
            // 既存のブラウザとマージ
            foreach (var detectedBrowser in detectedBrowsers)
            {
                var existingBrowser = Browsers.FirstOrDefault(b => b.Name == detectedBrowser.Name);
                if (existingBrowser == null)
                {
                    Browsers.Add(detectedBrowser);
                    Logger.LogInfo("Settings.DetectBrowsers", "ブラウザ追加", detectedBrowser.Name);
                }
            }
            
            Logger.LogInfo("Settings.DetectBrowsers", "End", Browsers.Count);
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="overrideSafeMode">SafeModeを無視するかどうか</param>
        public void DoSave(bool overrideSafeMode = false)
        {
            Logger.LogInfo("Settings.DoSave", "Start", overrideSafeMode);
            if (SafeMode && !overrideSafeMode) return; // do not save

            if (PortableMode)
            {
                IntSave(Application.StartupPath);
            }
            else
            {
                IntSave(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BrowserChooser3"));
            }
            Logger.LogInfo("Settings.DoSave", "End", overrideSafeMode);
        }

        private void IntSave(string path)
        {
            Logger.LogInfo("Settings.IntSave", "Start", path);
            if (SafeMode)
            {
                Logger.LogInfo("Settings.IntSave", "Safe Mode", path);
                return; // do not save
            }

            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                Logger.LogInfo("Settings.IntSave", "Creating Directory", path);
                Directory.CreateDirectory(path);
            }

            var xmlSerializer = new XmlSerializer(typeof(Settings));
            var filePath = Path.Combine(path, BrowserChooserConfigFileName);

            using var writer = new StreamWriter(filePath);
            Logger.LogInfo("Settings.IntSave", "Writing", path);
            xmlSerializer.Serialize(writer, this);
            Logger.LogInfo("Settings.IntSave", "End", path);
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        /// <param name="path">設定ファイルのパス</param>
        /// <returns>設定オブジェクト</returns>
        public static Settings Load(string path)
        {
            Logger.LogInfo("Settings.Load", "Start", path);
            
            // ポリシーを初期化
            Policy.Initialize();
            
            // ポリシーで設定ファイルが無視される場合
            if (Policy.IgnoreSettingsFile)
            {
                Logger.LogInfo("Settings.Load", "ポリシーにより設定ファイルが無視されます");
                return new Settings(false);
            }

            var serializer = new XmlSerializer(typeof(Settings));
            Settings output;

            var configPath = Path.Combine(path, BrowserChooserConfigFileName);
            if (File.Exists(configPath))
            {
                try
                {
                    Logger.LogInfo("Settings.Load", "Reading Settings", path);
                    using var reader = new StreamReader(configPath);
                    output = (Settings)serializer.Deserialize(reader)!;

                    if (output.EnableLogging)
                    {
                        LogDebugs = TriState.True;
                    }

                    if (output.ExtractDLLs)
                    {
                        DoExtractDLLs = true;
                    }

                    // ログレベルを初期化（ログが有効な場合のみ）
                    if (output?.EnableLogging == true)
                    {
                        Logger.InitializeLogLevel(output.LogLevel);
                    }

                    // lock width and height to 10 max, 1 min - acts as overflow protection
                    if (output?.Width > 10)
                        output.Width = 10;
                    else if (output?.Width < 1)
                        output.Width = 1;

                    if (output?.Height > 10)
                        output.Height = 10;
                    else if (output?.Height < 1)
                        output.Height = 1;

                    // 設定ファイルのバージョン管理と自動マイグレーション
                    if (output != null)
                    {
                        output = MigrateSettings(output, path);
                    }

                    Logger.LogInfo("Settings.Load", "End", path);
                    return output ?? new Settings(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Settings.Load", "Exception: Failed to load settings file. Default settings used.", path, ex.Message, ex.StackTrace ?? "");
                }
            }
            else
            {
                // レガシー設定ファイルのインポートを試行
                if (Importer.HasLegacySettings(path))
                {
                    Logger.LogInfo("Settings.Load", "レガシー設定ファイルを検出", path);
                    var newSettings = new Settings(false);
                    if (Importer.ImportLegacySettings(path, newSettings))
                    {
                        Logger.LogInfo("Settings.Load", "レガシー設定のインポートが完了しました");
                        return newSettings;
                    }
                }
            }

            Logger.LogInfo("Settings.Load", "Exception: Failed to load settings file. Default settings used.", path);
            return new Settings(false);
        }

        /// <summary>
        /// 設定ファイルのバージョン管理と自動マイグレーション
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="path">設定ファイルパス</param>
        /// <returns>マイグレーション後の設定オブジェクト</returns>
        private static Settings MigrateSettings(Settings settings, string path)
        {
            Logger.LogInfo("Settings.MigrateSettings", "マイグレーション開始", settings.FileVersion);

            try
            {
                // バージョン1から2へのマイグレーション
                if (settings.FileVersion == 1)
                {
                    Logger.LogInfo("Settings.MigrateSettings", "バージョン1から2へのマイグレーション");
                    MigrateFromVersion1(settings);
                    settings.FileVersion = 2;
                }

                // バージョン2から3へのマイグレーション
                if (settings.FileVersion == 2)
                {
                    Logger.LogInfo("Settings.MigrateSettings", "バージョン2から3へのマイグレーション");
                    MigrateFromVersion2(settings);
                    settings.FileVersion = 3;
                }

                // バージョン3から4へのマイグレーション
                if (settings.FileVersion == 3)
                {
                    Logger.LogInfo("Settings.MigrateSettings", "バージョン3から4へのマイグレーション");
                    MigrateFromVersion3(settings);
                    settings.FileVersion = 4;
                }

                // バージョン4から5へのマイグレーション
                if (settings.FileVersion == 4)
                {
                    Logger.LogInfo("Settings.MigrateSettings", "バージョン4から5へのマイグレーション");
                    MigrateFromVersion4(settings);
                    settings.FileVersion = 5;
                }

                // 最新バージョンに更新
                if (settings.FileVersion < CURRENT_FILE_VERSION)
                {
                    Logger.LogInfo("Settings.MigrateSettings", $"バージョン{settings.FileVersion}から{CURRENT_FILE_VERSION}へのマイグレーション");
                    settings.FileVersion = CURRENT_FILE_VERSION;
                }

                // マイグレーション後に保存
                if (settings.FileVersion != CURRENT_FILE_VERSION)
                {
                    settings.DoSave(true);
                }

                Logger.LogInfo("Settings.MigrateSettings", "マイグレーション完了", settings.FileVersion);
                return settings;
            }
            catch (Exception ex)
            {
                Logger.LogError("Settings.MigrateSettings", "マイグレーションエラー", ex.Message);
                return settings;
            }
        }

        /// <summary>
        /// バージョン1から2へのマイグレーション
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void MigrateFromVersion1(Settings settings)
        {
            // プロトコルとファイルタイプの追加
            if (settings.Protocols == null || settings.Protocols.Count == 0)
            {
                settings.Protocols = new List<Protocol>();
                CreateDefaultProtocols(settings);
            }

            if (settings.FileTypes == null || settings.FileTypes.Count == 0)
            {
                settings.FileTypes = new List<FileType>();
                CreateDefaultFileTypes(settings);
            }

            // ブラウザのGUID生成と位置調整
            foreach (var browser in settings.Browsers)
            {
                if (browser.Guid == Guid.Empty)
                {
                    browser.Guid = Guid.NewGuid();
                }

                // 位置の調整
                if (browser.PosX > 5)
                {
                    browser.PosY = (int)Math.Ceiling((double)browser.PosX / 5);
                    browser.PosX = browser.PosX % 5;
                    if (browser.PosX == 0) browser.PosX = 5;
                }
            }
        }

        /// <summary>
        /// バージョン2から3へのマイグレーション
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void MigrateFromVersion2(Settings settings)
        {
            // アクセシビリティ設定の追加
            if (string.IsNullOrEmpty(settings.Separator))
            {
                settings.Separator = " - ";
            }

            // スクリーンリーダーの検出
            var hasScreenReader = GeneralUtilities.HasScreenReader();
            settings.UseAccessibleRendering = hasScreenReader;
            if (hasScreenReader)
            {
                settings.ShowFocus = true;
            }
        }

        /// <summary>
        /// バージョン3から4へのマイグレーション
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void MigrateFromVersion3(Settings settings)
        {
            // アクセシビリティ設定の再構築
            // 既存の設定を保持
        }

        /// <summary>
        /// バージョン4から5へのマイグレーション
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void MigrateFromVersion4(Settings settings)
        {
            // レジストリチェック機能の追加
            // 既存の設定を保持
        }

        /// <summary>
        /// デフォルトプロトコルを作成します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void CreateDefaultProtocols(Settings settings)
        {
            var browserGuids = settings.Browsers.Select(b => b.Guid).ToList();
            var defaultCategories = new List<string> { "Default" };

            settings.Protocols.AddRange(new[]
            {
                new Protocol("HTTP", "http", browserGuids, defaultCategories),
                new Protocol("Secure HTTP", "https", browserGuids, defaultCategories),
                new Protocol("FTP", "ftp", browserGuids, defaultCategories),
                new Protocol("Secure FTP", "ftps", browserGuids, defaultCategories),
                new Protocol("URL Shortcut", "url", browserGuids, defaultCategories)
            });
        }

        /// <summary>
        /// デフォルトファイルタイプを作成します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        private static void CreateDefaultFileTypes(Settings settings)
        {
            var browserGuids = settings.Browsers.Select(b => b.Guid).ToList();
            var defaultCategories = new List<string> { "Default" };

            settings.FileTypes.AddRange(new[]
            {
                new FileType("XHTML", "xhtml", browserGuids, defaultCategories),
                new FileType("XHT", "xht", browserGuids, defaultCategories),
                new FileType("SHTML", "shtml", browserGuids, defaultCategories),
                new FileType("HTML", "html", browserGuids, defaultCategories),
                new FileType("HTM", "htm", browserGuids, defaultCategories)
            });
        }
    }
}
