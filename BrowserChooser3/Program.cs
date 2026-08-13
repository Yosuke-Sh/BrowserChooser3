using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Classes.Services.UI;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 最初にログレベルを初期化（設定ファイルから読み取り）
            Logger.InitializeLogLevel();

            // コマンドライン引数からURLを事前抽出（単一インスタンス判定に使用）
            var rawArgs = Environment.GetCommandLineArgs();
            var startupArgsForInstanceCheck = rawArgs.Skip(1).ToArray();
            var earlyUrl = startupArgsForInstanceCheck.FirstOrDefault(arg =>
                !arg.StartsWith("-") && !arg.StartsWith("/") && !arg.StartsWith("--")) ?? string.Empty;

            using var singleInstanceManager = new SingleInstanceManager();
            if (!singleInstanceManager.TryAcquire())
            {
                // 既に起動中のインスタンスがあるため、URLを引き渡して即座に終了する
                Logger.LogInfo("Program.Main", "既存インスタンスへURLを引き渡して終了", earlyUrl);
                SingleInstanceManager.TrySendUrlToExistingInstance(earlyUrl);
                Logger.Flush();
                return;
            }

            try
            {
                // パス管理の初期化
                PathManager.Initialize();
                Logger.LogDebug("Program.Main", "PathManager初期化完了");

                Logger.LogDebug("Program.Main", "アプリケーション開始");
                Logger.LogDebug("Program.Main", "ログレベル初期化完了");

                // コマンドライン引数の処理
                var args = rawArgs;
                Logger.LogTrace("Program.Main", "起動パラメータ", $"引数数={args.Length - 1}");

                // 起動時初期化処理（コマンドライン引数の解析・検証もここで行われ、
                // StartupLauncherの静的プロパティ(URL/Browser/Delay/SilentMode/AutoLaunch)に反映される）
                var startupArgs = startupArgsForInstanceCheck;
                var startupResult = StartupLauncher.Initialize(startupArgs);

                // --help / --version が指定された場合はGUIを起動せずここで終了する
                if (StartupLauncher.ShouldExitAfterInitialize)
                {
                    Logger.LogInfo("Program.Main", "--help/--versionが指定されたためGUIを起動せず終了します");
                    Logger.Flush();
                    return;
                }

                if (!startupResult)
                {
                    Logger.LogWarning("Program.Main", "起動時初期化に失敗しましたが、アプリケーションを続行します");
                }

                // 既定のアプリ設定はインストーラーのオプションで開くように変更

                // Windows Forms アプリケーションの設定
                ApplicationConfiguration.Initialize();
                Logger.LogDebug("Program.Main", "ApplicationConfiguration初期化完了");

                // メインフォームの作成と実行
                Logger.LogDebug("Program.Main", "メインフォーム作成開始");
                var mainForm = new MainForm();
                Logger.LogDebug("Program.Main", "メインフォーム作成完了");

                // 他プロセスからURLを受信したら、既存ウィンドウに反映する
                singleInstanceManager.UrlReceived += url => mainForm.ReceiveExternalURL(url);

                // StartupLauncherが解析した結果を単一の情報源として使う
                // （以前はここで引数を再パースしていたが、CommandLineProcessorの結果と
                // 二重管理になり整合性が崩れる原因だったため、StartupLauncher経由に一本化した）
                if (!string.IsNullOrEmpty(StartupLauncher.URL))
                {
                    var url = StartupLauncher.URL;
                    Logger.LogInfo("Program.Main", "初期URL設定", url);
                    mainForm.SetInitialURL(url);
                }

                // -d/--delay、-b/--browser、--silent、--auto-launchをMainFormへ伝達する。
                // --auto-launchは遅延ゼロでの即時起動として扱う。
                int? delayOverride = StartupLauncher.DelaySpecifiedOnCommandLine ? StartupLauncher.Delay : null;
                if (StartupLauncher.AutoLaunch)
                {
                    delayOverride = 0;
                }
                mainForm.SetStartupOptions(delayOverride, StartupLauncher.Browser?.Guid, StartupLauncher.SilentMode);

                Logger.LogDebug("Program.Main", "Application.Run開始");
                Application.Run(mainForm);
                Logger.LogDebug("Program.Main", "Application.Run終了");
            }
            catch (Exception ex)
            {
                Logger.LogError("Program.Main", "アプリケーション起動エラー", ex.Message, ex.StackTrace ?? "");
                                MessageBoxService.ShowErrorStatic($"アプリケーションの起動に失敗しました: {ex.Message}", "エラー");
            }

            Logger.LogDebug("Program.Main", "アプリケーション終了");
            Logger.Flush();
        }
    }
}