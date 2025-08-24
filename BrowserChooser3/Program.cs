using BrowserChooser3.Classes;
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
            Logger.LogInfo("Program.Main", "アプリケーション開始");

            try
            {
                // ログレベルを初期化
                Logger.InitializeLogLevel();
                Logger.LogInfo("Program.Main", "ログレベル初期化完了");

                // コマンドライン引数の処理
                var args = Environment.GetCommandLineArgs();
                Logger.LogTrace("Program.Main", "起動パラメータ", $"引数数={args.Length - 1}");

                if (args.Length > 1)
                {
                    ProcessCommandLineArgs(args.Skip(1).ToArray());
                }

                // Windows Forms アプリケーションの設定
                ApplicationConfiguration.Initialize();
                Logger.LogInfo("Program.Main", "ApplicationConfiguration初期化完了");

                // メインフォームの作成と実行
                Logger.LogInfo("Program.Main", "メインフォーム作成開始");
                var mainForm = new MainForm();
                Logger.LogInfo("Program.Main", "メインフォーム作成完了");

                // コマンドライン引数からURLを取得
                string url = string.Empty;
                if (args.Length > 1)
                {
                    url = args[1];
                    mainForm.UpdateURL(url);
                    Logger.LogInfo("Program.Main", "URL設定", url);
                }

                Logger.LogInfo("Program.Main", "Application.Run開始");
                Application.Run(mainForm);
                Logger.LogInfo("Program.Main", "Application.Run終了");
            }
            catch (Exception ex)
            {
                Logger.LogError("Program.Main", "アプリケーション起動エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"アプリケーションの起動に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Logger.LogInfo("Program.Main", "アプリケーション終了");
        }

        /// <summary>
        /// コマンドライン引数を処理
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        private static void ProcessCommandLineArgs(string[] args)
        {
            Logger.LogInfo("Program.ProcessCommandLineArgs", "Start", $"引数数={args.Length}");

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

            Logger.LogInfo("Program.ProcessCommandLineArgs", "End");
        }
    }
}