using System.Runtime.InteropServices;
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
        private static bool _silentMode = false;
        private static bool _autoLaunch = false;
        private static bool _delaySpecifiedOnCommandLine = false;
        private static bool _shouldExitAfterInitialize = false;

        /// <summary>
        /// 短縮URL展開処理の世代カウンター。
        /// SetURLが呼ばれるたびにインクリメントし、バックグラウンドワーカー完了時に
        /// 自分の世代が最新かどうかを確認することで、後から来たURLの展開結果が
        /// 先に来たURLの展開結果で上書きされる競合を防ぐ。
        /// </summary>
        private static int _generation = 0;

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

        /// <summary>サイレントモード（UIを表示せず既定ブラウザで開く）が指定されたかどうか</summary>
        public static bool SilentMode => _silentMode;

        /// <summary>自動起動モード（遅延なしで即座に起動）が指定されたかどうか</summary>
        public static bool AutoLaunch => _autoLaunch;

        /// <summary>コマンドラインで遅延時間(-d/--delay)が明示的に指定されたかどうか</summary>
        public static bool DelaySpecifiedOnCommandLine => _delaySpecifiedOnCommandLine;

        /// <summary>
        /// --help / --version が指定され、GUIを起動せず終了すべきかどうか。
        /// Program.Mainはこのフラグを見てApplication.Run前に処理を打ち切る。
        /// </summary>
        public static bool ShouldExitAfterInitialize => _shouldExitAfterInitialize;
        #endregion

        /// <summary>
        /// URLを設定し、対応ブラウザを検出します
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <param name="unShorten">短縮URLを展開するかどうか</param>
        /// <param name="updateDelegate">URL更新デリゲート</param>
        /// <returns>バックグラウンドでの短縮URL展開処理を開始した場合はtrue</returns>
        public static bool SetURL(string url, bool unShorten, UpdateURL updateDelegate)
        {
            Logger.LogDebug("StartupLauncher.SetURL", "SetURL開始", $"URL: {url}, 長さ: {url?.Length ?? 0}, Unshorten: {unShorten}");

            // 新しいURLを設定するたびに世代を進める。
            // 進行中の展開ワーカーは古い世代の結果をデリゲートへ渡さなくなる。
            var generation = System.Threading.Interlocked.Increment(ref _generation);

            _delegate = updateDelegate;
            _url = url ?? string.Empty;
            Logger.LogDebug("StartupLauncher.SetURL", "URL設定完了", $"設定されたURL: {_url}");

            if (unShorten && !string.IsNullOrEmpty(_url))
            {
                Logger.LogDebug("StartupLauncher.SetURL", "短縮URL展開処理開始");
                // HTTP/HTTPS URLの場合のみ短縮URL展開を実行
                if (_url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    _url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.LogDebug("StartupLauncher.SetURL", "HTTP/HTTPS URLを検出、短縮URL展開ワーカーを開始");
                    _ = Task.Run(() => Worker_DoWork_HTTP(generation, _url, _delegate));
                    return true;
                }

                Logger.LogDebug("StartupLauncher.SetURL", "HTTP/HTTPS URLではないため短縮URL展開をスキップ");
                return false;
            }

            Logger.LogDebug("StartupLauncher.SetURL", "短縮URL展開が無効またはURLが空のためスキップ");
            Logger.LogDebug("StartupLauncher.SetURL", "SetURL完了");
            return false;
        }

        /// <summary>
        /// URLを設定し、遅延起動とブラウザを指定します
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <param name="unShorten">短縮URLを展開するかどうか</param>
        /// <param name="delay">遅延時間（秒）</param>
        /// <param name="browser">選択されたブラウザ（未指定の場合はnull）</param>
        /// <param name="updateDelegate">URL更新デリゲート</param>
        public static void SetURL(string url, bool unShorten, int delay, Browser? browser, UpdateURL updateDelegate)
        {
            var generation = System.Threading.Interlocked.Increment(ref _generation);

            _delay = delay;
            _browser = browser;
            _delegate = updateDelegate;
            _url = url ?? string.Empty;

            if (unShorten && !string.IsNullOrEmpty(_url))
            {
                // HTTP/HTTPS URLの場合のみ短縮URL展開を実行
                if (_url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    _url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    _ = Task.Run(() => Worker_DoWork_HTTP(generation, _url, _delegate));
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
                // ヘルプ表示（GUIを起動せずコンソールへ出力して終了する）
                if (args.ShowHelp)
                {
                    WriteToConsole(CommandLineProcessor.GetHelpMessage());
                    _shouldExitAfterInitialize = true;
                    return true;
                }

                // バージョン表示（GUIを起動せずコンソールへ出力して終了する）
                if (args.ShowVersion)
                {
                    WriteToConsole(CommandLineProcessor.GetVersionInfo());
                    _shouldExitAfterInitialize = true;
                    return true;
                }

                // デバッグログの設定
                // Logger.InitializeLogLevel()はProgram.Mainの起点で本メソッドより先に
                // 一度呼ばれてしまっているため、Settings.LogDebugsを設定するだけでは
                // 実際のログレベルに反映されない。ここで直接CurrentLogLevelを引き上げる。
                if (args.DebugLog)
                {
                    Settings.LogDebugs = Settings.TriState.True;
                    Logger.CurrentLogLevel = Logger.LogLevel.Debug;
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "デバッグログを有効化");
                }

                // 設定ファイル無視の設定
                if (args.IgnoreSettings)
                {
                    Policy.IgnoreSettingsFile = true;
                    Logger.LogInfo("StartupLauncher.ProcessCommandLineArgs", "設定ファイル無視を有効化");
                }

                // サイレントモード・自動起動モードは常にMainForm側から参照できるよう保持する
                _silentMode = args.SilentMode;
                _autoLaunch = args.AutoLaunch;
                _delaySpecifiedOnCommandLine = args.Delay > 0;

                // URLが指定されている場合の処理
                if (!string.IsNullOrEmpty(args.URL))
                {
                    Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "URL処理開始", $"URL: {args.URL}, 長さ: {args.URL.Length}");

                    // 指定されたブラウザの検索
                    Browser? selectedBrowser = null;
                    if (args.BrowserGuid.HasValue)
                    {
                        Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "ブラウザGUID指定", args.BrowserGuid.Value.ToString());
                        selectedBrowser = Settings.Current.Browsers.FirstOrDefault(b => b.Guid == args.BrowserGuid.Value);
                        if (selectedBrowser == null)
                        {
                            Logger.LogWarning("StartupLauncher.ProcessCommandLineArgs", "指定されたブラウザが見つかりません", args.BrowserGuid.Value);
                        }
                        else
                        {
                            Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "指定されたブラウザを検出", selectedBrowser.Name);
                        }
                    }
                    else
                    {
                        Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "ブラウザGUIDが指定されていません");
                    }

                    // URL設定
                    // ブラウザが指定されていない場合も-d/--delayを反映させるため、
                    // 常に(url, unShorten, delay, browser, delegate)のオーバーロードを使う
                    Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "SetURL呼び出し前", $"URL: {args.URL}, Unshorten: {args.UnshortenURL}, Delay: {args.Delay}");
                    SetURL(args.URL, args.UnshortenURL, args.Delay, selectedBrowser, updateDelegate ?? DefaultUpdateDelegate);
                    Logger.LogDebug("StartupLauncher.ProcessCommandLineArgs", "SetURL呼び出し完了");

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

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// --help / --version の出力を親コンソール（起動元のコマンドプロンプト等）へ書き出します。
        /// WinFormsアプリはコンソールを持たないため、Console.WriteLineだけでは
        /// 呼び出し元のターミナルに何も表示されない。AttachConsoleで親プロセスの
        /// コンソールに接続してから出力する。
        /// </summary>
        /// <param name="message">出力するメッセージ</param>
        private static void WriteToConsole(string message)
        {
            try
            {
                if (AttachConsole(ATTACH_PARENT_PROCESS))
                {
                    Console.WriteLine(message);
                }
                else
                {
                    // 親コンソールが無い場合（エクスプローラー経由の起動等）はログにのみ残す
                    Logger.LogInfo("StartupLauncher.WriteToConsole", "コンソール未接続のため出力をログに記録", message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("StartupLauncher.WriteToConsole", "コンソール出力エラー", ex.Message);
            }
        }

        /// <summary>
        /// BrowserChooser3が既定のブラウザとして設定されているかチェックします。
        /// Windows 8以降、既定アプリはUserChoiceレジストリで管理されHKCRへの直接書き込みでは
        /// 変更できないため、非既定時の自動設定は行わず、初回起動時のみ設定アプリを案内します。
        /// </summary>
        private static void CheckBrowserChooserDefaultStatus()
        {
            try
            {
                var browserChooserPath = Application.ExecutablePath;
                var isDefault = DefaultBrowserChecker.IsBrowserChooserDefault(browserChooserPath);

                if (isDefault)
                {
                    Logger.LogInfo("StartupLauncher.CheckBrowserChooserDefaultStatus", "BrowserChooser3が既定のブラウザとして設定されています");
                }
                else
                {
                    Logger.LogInfo("StartupLauncher.CheckBrowserChooserDefaultStatus", "BrowserChooser3が既定のブラウザとして設定されていません");

                    // 初回起動時のみ、既定アプリ設定画面を案内する（テスト環境では実プロセスを起動しない）
                    if (IsFirstRun() && !Logger.IsTestEnvironment)
                    {
                        Logger.LogInfo("StartupLauncher.CheckBrowserChooserDefaultStatus", "初回起動時のため、既定アプリ設定画面を表示します");
                        DefaultBrowserChecker.ShowHttpProtocolSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("StartupLauncher.CheckBrowserChooserDefaultStatus", "BrowserChooser3既定ブラウザ状況チェックエラー", ex.Message);
            }
        }

        /// <summary>
        /// 初回起動かどうかを判定します
        /// </summary>
        /// <returns>初回起動の場合はtrue</returns>
        private static bool IsFirstRun()
        {
            try
            {
                var configPath = PathManager.GetConfigFilePath(Settings.BrowserChooserConfigFileName);

                // 設定ファイルが存在しない場合は初回起動とみなす
                return !File.Exists(configPath);
            }
            catch
            {
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

            // テスト等での複数回呼び出しに備えて状態をリセットする
            _shouldExitAfterInitialize = false;
            _silentMode = false;
            _autoLaunch = false;
            _delaySpecifiedOnCommandLine = false;

            try
            {
                Logger.LogDebug("StartupLauncher.Initialize", "コマンドライン引数解析開始", $"引数数: {args?.Length ?? 0}");
                if (args != null && args.Length > 0)
                {
                    Logger.LogDebug("StartupLauncher.Initialize", "コマンドライン引数内容", string.Join(" ", args));
                }
                
                // コマンドライン引数の解析
                var commandLineArgs = CommandLineProcessor.ParseArguments(args ?? Array.Empty<string>());
                Logger.LogDebug("StartupLauncher.Initialize", "CommandLineProcessor.ParseArguments完了", $"URL: {commandLineArgs.URL}, 長さ: {commandLineArgs.URL?.Length ?? 0}");
                
                // 環境変数からのオプション読み込み
                commandLineArgs = CommandLineProcessor.LoadFromEnvironment(commandLineArgs);
                
                // 引数の検証
                Logger.LogDebug("StartupLauncher.Initialize", "引数検証開始");
                if (!CommandLineProcessor.ValidateArguments(commandLineArgs))
                {
                    Logger.LogError("StartupLauncher.Initialize", "無効なコマンドライン引数");
                    return false;
                }
                Logger.LogDebug("StartupLauncher.Initialize", "引数検証完了");

                // ポリシーの初期化
                Policy.Initialize();

                // BrowserChooser3が既定のブラウザとして設定されているかのチェック（初回起動時は自動設定も行う）は
                // 起動レイテンシに直結しないため、ウィンドウ表示をブロックしないようバックグラウンドへ後退させる
                _ = Task.Run(CheckBrowserChooserDefaultStatus);

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



        #region ShortURL deshortening
        /// <summary>
        /// HTTP/HTTPS短縮URLの展開処理
        /// </summary>
        /// <param name="generation">呼び出し時点の世代番号。完了時にこれが最新でなければ結果を破棄する</param>
        /// <param name="url">展開対象のURL（呼び出し時点でキャプチャした値）</param>
        /// <param name="updateDelegate">結果を通知するデリゲート（呼び出し時点でキャプチャした値）</param>
        private static async Task Worker_DoWork_HTTP(int generation, string url, UpdateURL? updateDelegate)
        {
            // Task.Run経由で呼ばれるが、例外を外へ伝播させてもTask自体を誰も待たないため、
            // 念のためメソッド全体を確実にtry/catchで囲む
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", Settings.Current.UserAgent);

                var resultUrl = url;

                try
                {
                    // HEADリクエストを試行
                    using var headRequest = new HttpRequestMessage(HttpMethod.Head, resultUrl);
                    using var headResponse = await httpClient.SendAsync(headRequest);

                    if (headResponse.RequestMessage?.RequestUri != null)
                    {
                        resultUrl = headResponse.RequestMessage.RequestUri.ToString();
                    }
                }
                catch (HttpRequestException)
                {
                    try
                    {
                        // GETリクエストを試行
                        using var getResponse = await httpClient.GetAsync(resultUrl);

                        if (getResponse.RequestMessage?.RequestUri != null)
                        {
                            resultUrl = getResponse.RequestMessage.RequestUri.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 変換できない場合は元のURLのまま処理を続行する
                        Logger.LogWarning("StartupLauncher.Worker_DoWork_HTTP", "GETリクエストによる短縮URL展開に失敗", ex.Message);
                    }
                }

                // 自分の世代がまだ最新の場合のみ結果を反映する。
                // 完了前に新しいURLがSetURLで設定されていた場合は、古い結果で
                // 新しいURLを上書きしないよう破棄する。
                if (generation == _generation)
                {
                    _url = resultUrl;
                    updateDelegate?.Invoke(resultUrl);
                }
                else
                {
                    Logger.LogDebug("StartupLauncher.Worker_DoWork_HTTP", "新しいURLが設定されたため展開結果を破棄", url, resultUrl);
                }
            }
            catch (Exception ex)
            {
                // 短縮URL展開はベストエフォートのため、想定外の例外もここで握りつぶし、
                // 元のURLのままアプリの動作を継続させる
                Logger.LogError("StartupLauncher.Worker_DoWork_HTTP", "短縮URL展開処理で予期しないエラーが発生しました", ex.Message, ex.StackTrace ?? "");

                if (generation == _generation)
                {
                    updateDelegate?.Invoke(url);
                }
            }
        }
        #endregion
    }
}
