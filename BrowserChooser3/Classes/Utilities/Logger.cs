using System.Configuration;
using System.Text;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// ログ記録を管理するクラス
    /// アプリケーションの動作ログをファイルに記録します
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// ログレベルの定義
        /// </summary>
            /// <summary>ログレベルを定義する列挙型</summary>
    public enum LogLevel
    {
        /// <summary>ログなし</summary>
        None = 0,
        
        /// <summary>エラーレベル</summary>
        Error = 1,
        
        /// <summary>警告レベル</summary>
        Warning = 2,
        
        /// <summary>情報レベル</summary>
        Info = 3,
        
        /// <summary>デバッグレベル</summary>
        Debug = 4,
        
        /// <summary>トレースレベル</summary>
        Trace = 5
    }

        /// <summary>
        /// 現在のログレベル（デフォルトはInfo）
        /// </summary>
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            try
            {
                // 環境変数でダイアログ無効化が設定されている場合
                var disableDialogs = Environment.GetEnvironmentVariable("DISABLE_DIALOGS");
                if (!string.IsNullOrEmpty(disableDialogs) && disableDialogs.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // 環境変数でテスト環境が設定されている場合
                var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnvironment) && testEnvironment.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合は、テスト環境ではないと判断
                Console.WriteLine($"IsTestEnvironment check failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ログメッセージのキュー
        /// </summary>
        private static readonly Queue<string> _logQueue = new Queue<string>();

        /// <summary>
        /// インストーラー経由でインストールされたかどうかを判定
        /// </summary>
        /// <returns>インストーラー経由の場合はtrue</returns>
        private static bool IsInstalledViaInstaller()
        {
            try
            {
                // 実行ファイルがProgram FilesまたはProgram Files (x86)にある場合はインストーラー経由と判定
                var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                var programFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                
                return executablePath.StartsWith(programFilesPath, StringComparison.OrdinalIgnoreCase) ||
                       executablePath.StartsWith(programFilesX86Path, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                // エラーが発生した場合は、インストーラー経由ではないと判断
                Console.WriteLine($"IsInstalledViaInstaller check failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// ログディレクトリのパス
        /// </summary>
        private static string LogDirectory
        {
            get
            {
                if (IsInstalledViaInstaller())
                {
                    // インストーラー経由でインストールされた場合はLocalApplicationData
                    var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var logDir = Path.Combine(localAppData, "BrowserChooser3", "Logs");
                    
                    // ディレクトリが存在しない場合は作成
                    if (!Directory.Exists(logDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(logDir);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to create log directory: {ex.Message}");
                            // 作成に失敗した場合はTempPathにフォールバック
                            return Path.GetTempPath();
                        }
                    }
                    
                    return logDir;
                }
                else
                {
                    // その他の場合は実行フォルダ
                    var startupPath = Application.StartupPath;
                    var logDir = Path.Combine(startupPath, "Logs");
                    
                    // ディレクトリが存在しない場合は作成
                    if (!Directory.Exists(logDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(logDir);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to create log directory: {ex.Message}");
                            // 作成に失敗した場合は実行フォルダにフォールバック
                            return startupPath;
                        }
                    }
                    
                    return logDir;
                }
            }
        }
        
        /// <summary>
        /// 現在の日付に基づくログファイル名を取得
        /// </summary>
        /// <returns>ログファイル名</returns>
        private static string GetLogFileName()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return $"bc3_{today}.log";
        }
        
        /// <summary>
        /// ログファイルのパス
        /// </summary>
        private static string LogFilePath
        {
            get
            {
                var logDir = LogDirectory;
                var fileName = GetLogFileName();
                return Path.Combine(logDir, fileName);
            }
        }
        
        /// <summary>
        /// 古いログファイルを削除（30日以上古いファイル）
        /// </summary>
        private static void CleanupOldLogFiles()
        {
            try
            {
                var logDir = LogDirectory;
                if (!Directory.Exists(logDir))
                    return;
                
                var cutoffDate = DateTime.Now.AddDays(-30); // 30日以上古いファイルを削除
                var logFiles = Directory.GetFiles(logDir, "bc3_*.log");
                
                foreach (var logFile in logFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(logFile);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            File.Delete(logFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 個別ファイルの削除に失敗しても他のファイルの処理を続行
                        Console.WriteLine($"Failed to delete old log file {logFile}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // クリーンアップに失敗してもログ出力は続行
                Console.WriteLine($"Failed to cleanup old log files: {ex.Message}");
            }
        }

        /// <summary>
        /// ログを追加する（基本メソッド）
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="caller">呼び出し元</param>
        /// <param name="message">メッセージ</param>
        /// <param name="extraVars">追加情報</param>
        public static void AddToLog(LogLevel level, string caller, string message, params object[] extraVars)
        {
            if (level > CurrentLogLevel) return;

            // テスト環境ではログ出力をスキップ
            if (IsTestEnvironment())
            {
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var levelName = level.ToString().ToUpper();
            
            var logEntry = new StringBuilder();
            logEntry.Append($"\"{timestamp}\",\"{levelName}\",\"{caller}\",\"{message}\"");
            
            if (extraVars != null && extraVars.Length > 0)
            {
                foreach (var extraVar in extraVars)
                {
                    logEntry.Append($",\"{extraVar}\"");
                }
            }

            lock (_logQueue)
            {
                _logQueue.Enqueue(logEntry.ToString());
            }

            // ログをファイルに書き込み
            WriteLogsToFile();
        }

        /// <summary>
        /// エラーログを追加
        /// </summary>
        public static void LogError(string caller, string message, params object[] extraVars)
        {
            AddToLog(LogLevel.Error, caller, message, extraVars);
        }

        /// <summary>
        /// 警告ログを追加
        /// </summary>
        public static void LogWarning(string caller, string message, params object[] extraVars)
        {
            AddToLog(LogLevel.Warning, caller, message, extraVars);
        }

        /// <summary>
        /// 情報ログを追加
        /// </summary>
        public static void LogInfo(string caller, string message, params object[] extraVars)
        {
            AddToLog(LogLevel.Info, caller, message, extraVars);
        }

        /// <summary>
        /// デバッグログを追加
        /// </summary>
        public static void LogDebug(string caller, string message, params object[] extraVars)
        {
            AddToLog(LogLevel.Debug, caller, message, extraVars);
        }

        /// <summary>
        /// トレースログを追加
        /// </summary>
        public static void LogTrace(string caller, string message, params object[] extraVars)
        {
            AddToLog(LogLevel.Trace, caller, message, extraVars);
        }

        /// <summary>
        /// ログをファイルに書き込む処理を分離
        /// </summary>
        private static void WriteLogsToFile()
        {
            try
            {
                var logPath = LogFilePath;
                var writer = new StreamWriter(logPath, true, Encoding.UTF8);

                lock (_logQueue)
                {
                    while (_logQueue.Count > 0)
                    {
                        var logEntry = _logQueue.Dequeue();
                        writer.WriteLine(logEntry);
                    }
                }

                writer.Close();
                
                // 定期的に古いログファイルをクリーンアップ（1日1回程度）
                var lastCleanupKey = "LastLogCleanupDate";
                var lastCleanupDate = Environment.GetEnvironmentVariable(lastCleanupKey);
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                
                if (lastCleanupDate != today)
                {
                    CleanupOldLogFiles();
                    Environment.SetEnvironmentVariable(lastCleanupKey, today);
                }
            }
            catch (Exception)
            {
                // エラーが発生した場合は何もしない、ログは次回にキューイングされる
            }
        }

        /// <summary>
        /// app.configと設定値からログレベルを初期化する
        /// </summary>
        public static void InitializeLogLevel()
        {
            try
            {
                // app.configから読み取りを試行
                var configValue = ConfigurationManager.AppSettings["LogLevel"];
                if (!string.IsNullOrEmpty(configValue))
                {
                    if (int.TryParse(configValue, out int logLevelValue) && 
                        logLevelValue >= 0 && logLevelValue <= 5)
                    {
                        CurrentLogLevel = (LogLevel)logLevelValue;
                        LogInfo("Logger.InitializeLogLevel", "app.configからログレベルを設定しました", CurrentLogLevel.ToString());
                        return;
                    }
                }

                // app.configから読み取れない場合はデフォルト値を使用
                CurrentLogLevel = LogLevel.Info;
                LogInfo("Logger.InitializeLogLevel", "デフォルトログレベルを設定しました", CurrentLogLevel.ToString());
            }
            catch (Exception ex)
            {
                // 無効な値の場合はInfoレベルにフォールバック
                CurrentLogLevel = LogLevel.Info;
                LogError("Logger.InitializeLogLevel", "ログレベル初期化エラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定値からログレベルを初期化する（オーバーロード）
        /// </summary>
        /// <param name="logLevelSetting">設定値（0-5）</param>
        public static void InitializeLogLevel(int logLevelSetting)
        {
            if (logLevelSetting >= 0 && logLevelSetting <= 5)
            {
                CurrentLogLevel = (LogLevel)logLevelSetting;
                LogInfo("Logger.InitializeLogLevel", "ログレベルを設定しました", CurrentLogLevel.ToString());
            }
            else
            {
                CurrentLogLevel = LogLevel.Info;
                LogWarning("Logger.InitializeLogLevel", "無効なログレベル設定値。デフォルト値を使用します", logLevelSetting);
            }
        }
    }
}
