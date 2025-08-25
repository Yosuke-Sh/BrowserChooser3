using System.Net;
using System.Threading;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Services.BrowserServices;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// アプリケーション起動時の初期化とブラウザ起動を管理するクラス
    /// URL処理、ブラウザ検出、遅延起動などを担当します
    /// </summary>
    public class StartupLauncher
    {
        /// <summary>
        /// URL更新のデリゲート
        /// </summary>
        public delegate void UpdateURL(string url);
        
        private static string _url = string.Empty;
        private static bool _is64Bit = false;
        private static int _delay = 0;
        private static Browser? _browser = null;
        private static UpdateURL? _delegate = null;
        private static List<Guid> _supportingBrowsers = new();
        private static System.Threading.Thread? _worker = null;

        /// <summary>
        /// コンストラクタ
        /// 基本的な初期化処理を実行します
        /// </summary>
        public StartupLauncher()
        {
            Logger.LogInfo("StartupLauncher.New", "開始");
            
            // 64ビット環境の検出
            if (IntPtr.Size == 8 || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
            {
                Logger.LogInfo("StartupLauncher.New", "64ビット環境");
                _is64Bit = true;
            }

            Logger.LogInfo("StartupLauncher.New", "終了");
        }

        #region Read-only Properties
        /// <summary>64ビット環境かどうか</summary>
        public static bool Is64Bit => _is64Bit;
        
        /// <summary>現在のURL</summary>
        public static string URL => _url;
        
        /// <summary>選択されたブラウザ</summary>
        public static Browser? Browser => _browser;
        
        /// <summary>遅延時間</summary>
        public static int Delay => _delay;
        
        /// <summary>対応ブラウザのGUIDリスト</summary>
        public static List<Guid> SupportingBrowsers => _supportingBrowsers;
        #endregion

        /// <summary>
        /// URLを設定し、対応ブラウザを検出します
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <param name="unShorten">短縮URLを展開するかどうか</param>
        /// <param name="updateDelegate">URL更新デリゲート</param>
        public static void SetURL(string url, bool unShorten, UpdateURL updateDelegate)
        {
            _delegate = updateDelegate;
            var parts = URLUtilities.DetermineParts(url);
            
            if (parts.IsProtocol == Settings.TriState.True)
            {
                _url = parts.ToString();
            }
            else
            {
                _url = url; // ファイル名
            }
            
            ProcessParts(parts);

            if (unShorten && !string.IsNullOrEmpty(_url) && parts.IsProtocol == Settings.TriState.True)
            {
                if (parts.Protocol == "http" || parts.Protocol == "https")
                {
                    _worker = new System.Threading.Thread(Worker_DoWork_HTTP);
                    _worker.IsBackground = true;
                    _worker.Start();
                }
            }
        }

        /// <summary>
        /// URLを設定し、遅延起動とブラウザを指定します
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <param name="unShorten">短縮URLを展開するかどうか</param>
        /// <param name="delay">遅延時間（秒）</param>
        /// <param name="browser">選択されたブラウザ</param>
        /// <param name="updateDelegate">URL更新デリゲート</param>
        public static void SetURL(string url, bool unShorten, int delay, Browser browser, UpdateURL updateDelegate)
        {
            _delay = delay;
            _browser = browser;
            _delegate = updateDelegate;
            var parts = URLUtilities.DetermineParts(url);
            
            if (parts.IsProtocol == Settings.TriState.True)
            {
                _url = parts.ToString();
            }
            else
            {
                _url = url; // ファイル名
            }
            
            ProcessParts(parts);

            if (unShorten && !string.IsNullOrEmpty(_url) && parts.IsProtocol == Settings.TriState.True)
            {
                if (parts.Protocol == "http" || parts.Protocol == "https")
                {
                    _worker = new System.Threading.Thread(Worker_DoWork_HTTP);
                    _worker.IsBackground = true;
                    _worker.Start();
                }
            }
        }

        /// <summary>
        /// コマンドライン引数から起動処理を実行します
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <param name="updateDelegate">URL更新デリゲート</param>
        /// <returns>処理結果</returns>
        public static bool ProcessCommandLineArgs(CommandLineProcessor.CommandLineArgs args, UpdateURL? updateDelegate = null)
        {
            Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "コマンドライン引数処理開始");

            try
            {
                // ヘルプ表示
                if (args.ShowHelp)
                {
                    Console.WriteLine(CommandLineProcessor.GetHelpMessage());
                    return true;
                }

                // バージョン表示
                if (args.ShowVersion)
                {
                    Console.WriteLine(CommandLineProcessor.GetVersionInfo());
                    return true;
                }

                // デバッグログの設定
                if (args.DebugLog)
                {
                    Settings.LogDebugs = Settings.TriState.True;
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "デバッグログを有効化");
                }

                // DLL抽出の設定
                if (args.ExtractDLLs)
                {
                    Settings.DoExtractDLLs = true;
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "DLL抽出を有効化");
                }

                // ポータブルモードの設定
                if (args.PortableMode)
                {
                    // 設定ファイルの読み込み時に適用
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "ポータブルモードを有効化");
                }

                // 設定ファイル無視の設定
                if (args.IgnoreSettings)
                {
                    Policy.IgnoreSettingsFile = true;
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "設定ファイル無視を有効化");
                }

                // URLが指定されている場合の処理
                if (!string.IsNullOrEmpty(args.URL))
                {
                    // 指定されたブラウザの検索
                    Browser? selectedBrowser = null;
                    if (args.BrowserGuid.HasValue)
                    {
                        selectedBrowser = Settings.Current.Browsers.FirstOrDefault(b => b.Guid == args.BrowserGuid.Value);
                        if (selectedBrowser == null)
                        {
                            Logger.LogWarning("StartupLauncher.ProcessCommandLineArgs", "指定されたブラウザが見つかりません", args.BrowserGuid.Value);
                        }
                    }

                    // URL設定
                    if (selectedBrowser != null)
                    {
                        SetURL(args.URL, args.UnshortenURL, args.Delay, selectedBrowser, updateDelegate ?? DefaultUpdateDelegate);
                    }
                    else
                    {
                        SetURL(args.URL, args.UnshortenURL, updateDelegate ?? DefaultUpdateDelegate);
                    }

                    // 自動起動モードの場合
                    if (args.AutoLaunch)
                    {
                        Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "自動起動モードで実行");
                        // 自動起動の処理をここに追加
                    }

                    return true;
                }

                // サイレントモードの場合
                if (args.SilentMode)
                {
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "サイレントモードで実行");
                    // サイレントモードの処理をここに追加
                    return true;
                }

                Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "コマンドライン引数処理完了");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("StartupLauncher.ProcessCommandLineArgs", "コマンドライン引数処理エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// デフォルトのURL更新デリゲート
        /// </summary>
        /// <param name="url">更新されたURL</param>
        private static void DefaultUpdateDelegate(string url)
        {
            Logger.LogInfo("StartupLauncher.DefaultUpdateDelegate", "URL更新", url);
        }

        /// <summary>
        /// デフォルトブラウザチェックを実行します
        /// </summary>
        /// <returns>デフォルトブラウザが設定されている場合はtrue</returns>
        public static bool CheckDefaultBrowser()
        {
            Logger.LogInfo("StartupLauncher.CheckDefaultBrowser", "デフォルトブラウザチェック開始");

            try
            {
                var hasDefaultBrowser = DefaultBrowserChecker.HasDefaultBrowser();
                
                if (hasDefaultBrowser)
                {
                    var defaultBrowserInfo = DefaultBrowserChecker.GetDefaultBrowser();
                    Logger.LogInfo("StartupLauncher.CheckDefaultBrowser", "デフォルトブラウザを検出", defaultBrowserInfo.Name);
                }
                else
                {
                    Logger.LogWarning("StartupLauncher.CheckDefaultBrowser", "デフォルトブラウザが設定されていません");
                }

                return hasDefaultBrowser;
            }
            catch (Exception ex)
            {
                Logger.LogError("StartupLauncher.CheckDefaultBrowser", "デフォルトブラウザチェックエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 起動時の初期化処理を実行します
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>初期化が成功した場合はtrue</returns>
        public static bool Initialize(string[] args)
        {
            Logger.LogInfo("StartupLauncher.Initialize", "起動時初期化開始");

            try
            {
                // コマンドライン引数の解析
                var commandLineArgs = CommandLineProcessor.ParseArguments(args);
                
                // 環境変数からのオプション読み込み
                commandLineArgs = CommandLineProcessor.LoadFromEnvironment(commandLineArgs);
                
                // 引数の検証
                if (!CommandLineProcessor.ValidateArguments(commandLineArgs))
                {
                    Logger.LogError("StartupLauncher.Initialize", "無効なコマンドライン引数");
                    return false;
                }

                // ポリシーの初期化
                Policy.Initialize();

                // デフォルトブラウザチェック
                if (Settings.Current.CheckDefaultOnLaunch)
                {
                    CheckDefaultBrowser();
                }

                // コマンドライン引数の処理
                var result = ProcessCommandLineArgs(commandLineArgs);
                
                Logger.LogInfo("StartupLauncher.Initialize", "起動時初期化完了");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("StartupLauncher.Initialize", "起動時初期化エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// URLパーツを処理し、対応ブラウザを検出します
        /// </summary>
        /// <param name="parts">URLパーツ</param>
        private static void ProcessParts(URLUtilities.BC2URLParts parts)
        {
            _supportingBrowsers = new List<Guid>();

            if (parts.IsProtocol == Settings.TriState.True)
            {
                // プロトコルベースの処理
                foreach (var protocol in Settings.Current.Protocols)
                {
                    if (protocol.Header == parts.Protocol)
                    {
                        foreach (var browser in Settings.Current.Browsers)
                        {
                            if (protocol.SupportingBrowsers.Contains(browser.Guid))
                            {
                                _supportingBrowsers.Add(browser.Guid);
                            }
                        }
                        break; // short circuit
                    }
                }
            }
            else if (parts.IsProtocol == Settings.TriState.False)
            {
                // ファイル拡張子ベースの処理
                foreach (var fileType in Settings.Current.FileTypes)
                {
                    if (fileType.Extension.ToLower() == parts.Extension.ToLower())
                    {
                        foreach (var browser in Settings.Current.Browsers)
                        {
                            if (fileType.SupportingBrowsers.Contains(browser.Guid))
                            {
                                _supportingBrowsers.Add(browser.Guid);
                            }
                        }
                        break; // short circuit
                    }
                }
            }
            else
            {
                // デフォルト処理 - すべてのブラウザを表示
                foreach (var protocol in Settings.Current.Protocols)
                {
                    foreach (var browser in Settings.Current.Browsers)
                    {
                        if (protocol.SupportingBrowsers.Contains(browser.Guid))
                        {
                            _supportingBrowsers.Add(browser.Guid);
                        }
                    }
                }
            }
        }

        #region ShortURL deshortening
        /// <summary>
        /// HTTP/HTTPS短縮URLの展開処理
        /// </summary>
        private static async void Worker_DoWork_HTTP()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", Settings.Current.UserAgent);

            try
            {
                // HEADリクエストを試行
                using var headRequest = new HttpRequestMessage(HttpMethod.Head, _url);
                using var headResponse = await httpClient.SendAsync(headRequest);
                
                if (headResponse.RequestMessage?.RequestUri != null)
                {
                    _url = headResponse.RequestMessage.RequestUri.ToString();
                }
            }
            catch (HttpRequestException)
            {
                try
                {
                    // GETリクエストを試行
                    using var getResponse = await httpClient.GetAsync(_url);
                    
                    if (getResponse.RequestMessage?.RequestUri != null)
                    {
                        _url = getResponse.RequestMessage.RequestUri.ToString();
                    }
                }
                catch
                {
                    // 変換できない場合
                }
            }

            // クリーンアップとデリゲート呼び出し
            if (_delegate != null)
            {
                string finalUrl;
                var parts = URLUtilities.DetermineParts(_url);
                if (parts.IsProtocol == Settings.TriState.True)
                {
                    finalUrl = parts.ToString();
                }
                else
                {
                    finalUrl = _url;
                }

                _delegate.Invoke(finalUrl);
            }

            _worker = null;
        }
        #endregion
    }
}
