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
        /// - Version 6: 透明化設定を追加（EnableTransparency, TransparencyColor, Opacity, HideTitleBar, RoundedCorners）
        /// </summary>
        // バージョン管理は廃止
        public const int CURRENT_FILE_VERSION = 1;

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
            
            /// <summary>セパレータ</summary>
            Separator,
            
            /// <summary>フォーカス表示</summary>
            ShowFocus,
            
            /// <summary>URL表示</summary>
            ShowURL,
            
            /// <summary>短縮URL展開</summary>
            RevealShortURL,
            
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
            
            /// <summary>ログ有効化</summary>
            EnableLogging,
            
            /// <summary>DLL抽出</summary>
            ExtractDLLs,
            
            /// <summary>ログレベル</summary>
            LogLevel,
            
            /// <summary>透明化有効</summary>
            EnableTransparency,
            
            /// <summary>透明化色</summary>
            TransparencyColor,
            
            /// <summary>透明度</summary>
            Opacity,
            
            /// <summary>タイトルバー非表示</summary>
            HideTitleBar,
            
            /// <summary>角を丸くする半径</summary>
            RoundedCornersRadius
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
            { DefaultField.IconWidth, 100 },
            { DefaultField.IconHeight, 110 },
            { DefaultField.IconGapWidth, 20 },
            { DefaultField.IconGapHeight, 20 },
            { DefaultField.IconScale, 1.0 },
            { DefaultField.OptionsShortcut, 'O' },
            { DefaultField.DefaultMessage, "Choose a Browser" },
            { DefaultField.DefaultDelay, 5 },
            { DefaultField.DefaultBrowserGuid, Guid.Empty },
            { DefaultField.Separator, " - " },
            { DefaultField.ShowFocus, false },
            { DefaultField.ShowURL, true },
            { DefaultField.RevealShortURL, false },

            { DefaultField.FocusBoxLineWidth, 1 },
            { DefaultField.FocusBoxColor, Color.Transparent.ToArgb() },
            { DefaultField.UserAgent, "Mozilla/5.0" },
            { DefaultField.DownloadDetectionFile, true },
            { DefaultField.BackgroundColor, Color.FromArgb(128, 255, 255, 255).ToArgb() },
            { DefaultField.StartingPosition, AvailableStartingPositions.CenterScreen },
            { DefaultField.OffsetX, 0 },
            { DefaultField.OffsetY, 0 },
            { DefaultField.AllowStayOpen, false },
            { DefaultField.EnableLogging, true },
            { DefaultField.ExtractDLLs, false },
            { DefaultField.LogLevel, 3 },
            { DefaultField.EnableTransparency, false },
            { DefaultField.TransparencyColor, Color.Magenta.ToArgb() },
            { DefaultField.Opacity, 0.8 },
            { DefaultField.HideTitleBar, false },
            { DefaultField.RoundedCornersRadius, 20 }
        };

        /// <summary>設定ファイル名</summary>
        public const string BrowserChooserConfigFileName = "BrowserChooser3Config.xml";

        /// <summary>ブラウザリスト</summary>
        public List<Browser> Browsers { get; set; } = new();
        
        /// <summary>ポータブルモード</summary>
        public bool PortableMode { get; set; } = true;
        
        /// <summary>URL表示</summary>
        public bool ShowURL { get; set; } = true;
        
        /// <summary>短縮URL展開</summary>
        public bool RevealShortURL { get; set; } = false;
        
        /// <summary>ファイルバージョン</summary>
        // バージョン管理は廃止
        public int FileVersion { get; set; } = CURRENT_FILE_VERSION;
        
        /// <summary>プロトコルリスト</summary>
        public List<Protocol> Protocols { get; set; } = new();
        
        

        
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
        public int IconWidth { get; set; } = 100;
        
        /// <summary>アイコン高さ</summary>
        public int IconHeight { get; set; } = 110;
        
        /// <summary>アイコン間隔幅</summary>
        public int IconGapWidth { get; set; } = 20;
        
        /// <summary>アイコン間隔高さ</summary>
        public int IconGapHeight { get; set; } = 20;
        
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
        
        /// <summary>セパレータ</summary>
        public string Separator { get; set; } = " - ";
        
        /// <summary>フォーカス表示</summary>
        public bool ShowFocus { get; set; } = false;
        

        
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
        public int BackgroundColor { get; set; } = Color.FromArgb(128, 255, 255, 255).ToArgb();
        
        /// <summary>背景色（Color型、Browser Chooser 2互換）</summary>
        public Color BackgroundColorValue 
        { 
            get
            {
                Logger.LogInfo("Settings.BackgroundColorValue.get", $"BackgroundColor: {BackgroundColor}");
                
                // BackgroundColorが-1（Color.White）の場合は白を返す
                if (BackgroundColor == -1)
                {
                    Logger.LogInfo("Settings.BackgroundColorValue.get", "BackgroundColorが-1なので白を返します");
                    return Color.White;
                }
                
                // BackgroundColorが有効な値の場合は、その値を正しく使用
                var c = Color.FromArgb(BackgroundColor);
                Logger.LogInfo("Settings.BackgroundColorValue.get", $"BackgroundColorから色を取得: {c}");
                // 常に不透明（A=255）で返す
                return c.A == 255 ? c : Color.FromArgb(255, c.R, c.G, c.B);
            }
            set
            {
                // Color.Emptyが設定された場合は処理をスキップ（XMLデシリアライゼーションの副作用回避）
                if (value == Color.Empty)
                {
                    Logger.LogInfo("Settings.BackgroundColorValue.set", "Color.Emptyが設定されたため処理をスキップ");
                    return;
                }
                
                // 常に不透明（A=255）で保存
                var c = value.A == 255 ? value : Color.FromArgb(255, value.R, value.G, value.B);
                BackgroundColor = c.ToArgb();
                Logger.LogInfo("Settings.BackgroundColorValue.set", $"背景色を設定しました: {value} -> BackgroundColor: {BackgroundColor}");
            }
        }
        
        /// <summary>開始位置</summary>
        public int StartingPosition { get; set; } = (int)AvailableStartingPositions.CenterScreen;
        
        /// <summary>Xオフセット</summary>
        public int OffsetX { get; set; } = 0;
        
        /// <summary>Yオフセット</summary>
        public int OffsetY { get; set; } = 0;
        
        /// <summary>開いたまま許可</summary>
        public bool AllowStayOpen { get; set; } = false;
        
        /// <summary>ログ有効化</summary>
        public bool EnableLogging { get; set; } = true;
        
        /// <summary>DLL抽出</summary>
        public bool ExtractDLLs { get; set; } = false;
        
        /// <summary>ログレベル</summary>
        public int LogLevel { get; set; } = 3;

        /// <summary>グリッド表示</summary>
        public bool ShowGrid { get; set; } = false;

        /// <summary>グリッド色</summary>
        public int GridColor { get; set; } = Color.Gray.ToArgb();

        /// <summary>グリッド線幅</summary>
        public int GridLineWidth { get; set; } = 1;

        /// <summary>履歴保持</summary>
        public bool KeepHistory { get; set; } = true;

        /// <summary>履歴保持日数</summary>
        public int HistoryDays { get; set; } = 30;

        /// <summary>プライバシーモード</summary>
        public bool PrivacyMode { get; set; } = false;

        /// <summary>データ収集許可</summary>
        public bool AllowDataCollection { get; set; } = false;



        /// <summary>最小化で起動</summary>
        public bool StartMinimized { get; set; } = false;

        /// <summary>システムトレイで起動</summary>
        public bool StartInTray { get; set; } = false;

        /// <summary>起動遅延</summary>
        public int StartupDelay { get; set; } = 0;

        /// <summary>起動メッセージ</summary>
        public string StartupMessage { get; set; } = "BrowserChooser3 Started";

        /// <summary>透明化有効</summary>
        public bool EnableTransparency { get; set; } = false;

        /// <summary>透明化色</summary>
        public int TransparencyColor { get; set; } = Color.Magenta.ToArgb();

        /// <summary>透明度（0.01-1.0）</summary>
        public double Opacity { get; set; } = 0.8;

        /// <summary>タイトルバー非表示</summary>
        public bool HideTitleBar { get; set; } = false;

        /// <summary>角を丸くする半径（0で無効、1-50で有効）</summary>
        public int RoundedCornersRadius { get; set; } = 20;



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
            
            // 設定ファイルが存在する場合は自動検出をスキップ
            var configPath = Path.Combine(Application.StartupPath, BrowserChooserConfigFileName);
            if (File.Exists(configPath))
            {
                Logger.LogInfo("Settings.SharedNew", "設定ファイルが存在するため自動検出をスキップ", configPath);
            }
            else
            {
                // ブラウザの自動検出（初回のみ）
                DetectBrowsers();
            }
            
            Logger.LogInfo("Settings.SharedNew", "End");
        }

        /// <summary>
        /// ブラウザを自動検出します
        /// </summary>
        private void DetectBrowsers()
        {
            Logger.LogInfo("Settings.DetectBrowsers", "Start");
            
            // 既存のブラウザがある場合は自動検出をスキップ
            if (Browsers.Count > 0)
            {
                Logger.LogInfo("Settings.DetectBrowsers", "既存のブラウザが存在するため自動検出をスキップ", Browsers.Count);
                return;
            }
            
            // 初回のみブラウザ検出を実行
            if (BrowserDetector.DetectedBrowsers.Count == 0)
            {
                BrowserDetector.DetectBrowsers();
            }
            
            var detectedBrowsers = BrowserDetector.DetectedBrowsers.ToList(); // コピーを作成
            
            // 既存のブラウザとマージ
            int rowIndex = 0;
            foreach (var detectedBrowser in detectedBrowsers)
            {
                var existingBrowser = Browsers.FirstOrDefault(b => b.Name == detectedBrowser.Name);
                if (existingBrowser == null)
                {
                    // rowとcolを自動設定
                    detectedBrowser.PosY = rowIndex;
                    detectedBrowser.PosX = 0;
                    rowIndex++;
                    
                    Browsers.Add(detectedBrowser);
                    Logger.LogInfo("Settings.DetectBrowsers", "ブラウザ追加", detectedBrowser.Name, detectedBrowser.PosY, detectedBrowser.PosX);
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

                    // BackgroundColorValueの初期化（XMLデシリアライゼーション後の処理）
                    if (output != null)
                    {
                        // BackgroundColorValueが空の場合、BackgroundColorの値を使用して初期化
                        var currentBackgroundColor = output.BackgroundColor;
                        Logger.LogInfo("Settings.Load", $"BackgroundColorValue初期化: BackgroundColor={currentBackgroundColor}");
                        
                        // BackgroundColorValueプロパティを正しく設定（XMLデシリアライゼーションの副作用を回避）
                        if (currentBackgroundColor == -1)
                        {
                            // BackgroundColorが-1（白）の場合は、BackgroundColorValueを白に設定
                            Logger.LogInfo("Settings.Load", "BackgroundColorが-1なので、BackgroundColorValueを白に設定");
                        }
                        else
                        {
                            // その他の値の場合は、BackgroundColorの値を使用
                            var color = Color.FromArgb(currentBackgroundColor);
                            Logger.LogInfo("Settings.Load", $"BackgroundColorValueを設定: {color}");
                        }
                    }

                    // 設定ファイルのマイグレーション処理は廃止（既存値をそのまま使用）

                    Logger.LogInfo("Settings.Load", "設定ファイル読み込み成功", path, output?.Browsers?.Count ?? 0);
                    return output!;
                }
                catch (Exception ex)
                {
                    Logger.LogError("Settings.Load", "Exception: Failed to load settings file. Default settings used.", path, ex.Message, ex.StackTrace ?? "");
                }
            }
            else
            {
                // レガシー設定ファイルのインポート機能は削除されました
            }

            Logger.LogInfo("Settings.Load", "設定ファイルが存在しないためデフォルト設定を使用", path);
            var defaultSettings = new Settings(false);
            Logger.LogInfo("Settings.Load", "デフォルト設定作成完了", defaultSettings.Browsers?.Count ?? 0);
            return defaultSettings;
        }

        // 旧マイグレーション関連コードは削除（不要化）

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


    }
}
