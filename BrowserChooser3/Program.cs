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
            // アプリケーション起動時のログ初期化
            Logger.CurrentLogLevel = Logger.LogLevel.Trace;
            Logger.LogDebug("Program.Main", "アプリケーション開始");

            try
            {
                // パス管理の初期化
                PathManager.Initialize();
                Logger.LogDebug("Program.Main", "PathManager初期化完了");

                // ログレベルを初期化
                Logger.InitializeLogLevel();
                Logger.LogDebug("Program.Main", "ログレベル初期化完了");

                // コマンドライン引数の処理
                var args = Environment.GetCommandLineArgs();
                Logger.LogTrace("Program.Main", "起動パラメータ", $"引数数={args.Length - 1}");

                // 起動時初期化処理
                var startupArgs = args.Skip(1).ToArray();
                var startupResult = StartupLauncher.Initialize(startupArgs);
                
                if (!startupResult)
                {
                    Logger.LogWarning("Program.Main", "起動時初期化に失敗しましたが、アプリケーションを続行します");
                }

                // Windows Forms アプリケーションの設定
                ApplicationConfiguration.Initialize();
                Logger.LogDebug("Program.Main", "ApplicationConfiguration初期化完了");

                // メインフォームの作成と実行
                Logger.LogDebug("Program.Main", "メインフォーム作成開始");
                var mainForm = new MainForm();
                Logger.LogDebug("Program.Main", "メインフォーム作成完了");

                // コマンドライン引数からURLを取得（従来の処理）
                string url = string.Empty;
                if (args.Length > 1)
                {
                    Logger.LogDebug("Program.Main", "従来のURL処理開始", $"引数数: {startupArgs.Length}");
                    
                    // 最初の非オプション引数をURLとして扱う
                    var firstNonOptionArg = startupArgs.FirstOrDefault(arg => 
                        !arg.StartsWith("-") && !arg.StartsWith("/") && 
                        !arg.StartsWith("--") && !arg.StartsWith("--"));
                    
                    if (!string.IsNullOrEmpty(firstNonOptionArg))
                    {
                        url = firstNonOptionArg;
                        Logger.LogDebug("Program.Main", "従来のURL処理でURLを検出", $"URL: {url}, 長さ: {url.Length}");
                        
                        // フォームのLoadイベントでURLを設定するように設定
                        mainForm.SetInitialURL(url);
                        Logger.LogInfo("Program.Main", "初期URL設定", url);
                    }
                    else
                    {
                        Logger.LogDebug("Program.Main", "従来のURL処理でURLが見つかりませんでした");
                    }
                }

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
        }

        /// <summary>
        /// コマンドライン引数を処理（従来の処理 - 後方互換性のため保持）
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        private static void ProcessCommandLineArgs(string[] args)
        {
            Logger.LogDebug("Program.ProcessCommandLineArgs", "Start", $"引数数={args.Length}");

            foreach (var arg in args)
            {
                Logger.LogTrace("Program.ProcessCommandLineArgs", "引数処理", arg);

                if (arg.StartsWith("--"))
                {
                    switch (arg.ToLower())
                    {
                        case "--logging-enabled":
                            Logger.LogInfo("Program.ProcessCommandLineArgs", "ログ出力を有効化");
                            Settings.LogDebugs = Settings.TriState.True;
                            break;
                        case "--extract-dlls":
                            Logger.LogInfo("Program.ProcessCommandLineArgs", "DLL抽出を有効化");
                            Settings.DoExtractDLLs = true;
                            break;
                        default:
                            Logger.LogWarning("Program.ProcessCommandLineArgs", "未知のコマンドライン引数", arg);
                            break;
                    }
                }
            }

            Logger.LogDebug("Program.ProcessCommandLineArgs", "End");
        }
    }
}