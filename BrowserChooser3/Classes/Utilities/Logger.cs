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
        /// 現在のログレベル（デフォルトはWarning）
        /// </summary>
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Warning;

        /// <summary>
        /// ログレベルが初期化済みかどうか
        /// </summary>
        private static bool _isLogLevelInitialized = false;

        /// <summary>
        /// テスト環境かどうか（プロセス起動中に一度だけ評価してキャッシュする）
        /// </summary>
        public static bool IsTestEnvironment { get; } = TestEnvironmentDetector.IsTestEnvironment();



        /// <summary>
        /// ログメッセージのキュー
        /// </summary>
        private static readonly Queue<string> _logQueue = new Queue<string>();

        /// <summary>
        /// この件数以上キューに溜まったら即座にファイルへフラッシュする
        /// </summary>
        private const int FlushBatchSize = 20;

        /// <summary>
        /// 前回フラッシュしてからこの時間が経過したら次回のログ追加時にフラッシュする
        /// </summary>
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(2);

        private static DateTime _lastFlushTime = DateTime.MinValue;

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
        /// 解決済みログディレクトリのキャッシュ（初回解決後は再チェックしない）
        /// </summary>
        private static string? _cachedLogDirectory;

        /// <summary>
        /// ログディレクトリのパス
        /// </summary>
        private static string LogDirectory
        {
            get
            {
                if (_cachedLogDirectory != null)
                {
                    return _cachedLogDirectory;
                }

                // PathManagerを使用してログディレクトリを取得
                var logDir = PathManager.GetLogDirectory();

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
                        // 作成に失敗した場合はTempPathにフォールバック（キャッシュはしない。次回リトライを許す）
                        return Path.GetTempPath();
                    }
                }

                _cachedLogDirectory = logDir;
                return logDir;
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
            // ログレベルが初期化されていない場合は、ErrorとWarningのみ出力
            if (!_isLogLevelInitialized && level < LogLevel.Error)
            {
                // デバッグ用：初期化前のDEBUGログを抑制
                return;
            }

            if (level > CurrentLogLevel) return;

            // テスト環境ではログ出力をスキップ
            if (IsTestEnvironment)
            {
                return;
            }

            // ログディレクトリの存在を確認（初回ログ出力時に確実に作成）
            try
            {
                var logDir = LogDirectory; // この呼び出しでディレクトリが作成される
            }
            catch (Exception)
            {
                // ログディレクトリの作成に失敗した場合は何もしない
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

            bool shouldFlush;
            lock (_logQueue)
            {
                _logQueue.Enqueue(logEntry.ToString());

                // Error/Warningは即座に書き込む（クラッシュ時の情報欠落を防ぐ）。
                // それ以外は一定件数・一定時間たまるまでファイルI/Oをまとめる。
                shouldFlush = level <= LogLevel.Warning
                    || _logQueue.Count >= FlushBatchSize
                    || (DateTime.Now - _lastFlushTime) >= FlushInterval;
            }

            if (shouldFlush)
            {
                WriteLogsToFile();
            }
        }

        /// <summary>
        /// キューに溜まっているログを強制的にファイルへ書き込みます（アプリ終了時などに呼び出します）
        /// </summary>
        public static void Flush()
        {
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
            List<string>? pendingEntries = null;
            try
            {
                lock (_logQueue)
                {
                    if (_logQueue.Count == 0)
                    {
                        return;
                    }

                    pendingEntries = new List<string>(_logQueue);
                    _logQueue.Clear();
                    _lastFlushTime = DateTime.Now;
                }

                var logPath = LogFilePath;
                using (var writer = new StreamWriter(logPath, true, Encoding.UTF8))
                {
                    foreach (var logEntry in pendingEntries)
                    {
                        writer.WriteLine(logEntry);
                    }
                }

                pendingEntries = null;

                // 定期的に古いログファイルをクリーンアップ（1日1回程度、マーカーファイルで永続化）
                RunCleanupIfDue();
            }
            catch (Exception)
            {
                // 書き込みに失敗した場合は、デキューしたログをキューへ戻し次回のフラッシュで再試行する
                if (pendingEntries != null)
                {
                    lock (_logQueue)
                    {
                        foreach (var logEntry in pendingEntries)
                        {
                            _logQueue.Enqueue(logEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 前回クリーンアップ日を記録するマーカーファイル名
        /// </summary>
        private const string CleanupMarkerFileName = ".last_cleanup";

        /// <summary>
        /// 1日1回を目安に古いログファイルをクリーンアップする。
        /// 実行日はプロセス寿命を超えて永続化するため、環境変数ではなくマーカーファイルを使う。
        /// </summary>
        private static void RunCleanupIfDue()
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var markerPath = Path.Combine(LogDirectory, CleanupMarkerFileName);

                string? lastCleanupDate = null;
                if (File.Exists(markerPath))
                {
                    lastCleanupDate = File.ReadAllText(markerPath).Trim();
                }

                if (lastCleanupDate == today)
                {
                    return;
                }

                CleanupOldLogFiles();
                File.WriteAllText(markerPath, today);
            }
            catch (Exception)
            {
                // クリーンアップ判定に失敗してもログ出力自体は継続する
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
                    }
                    else
                    {
                        CurrentLogLevel = LogLevel.Info;
                    }
                }
                else
                {
                    // app.configから読み取れない場合はデフォルト値を使用
                    CurrentLogLevel = LogLevel.Warning;
                }

                // ログレベル初期化完了をマーク
                _isLogLevelInitialized = true;
            }
            catch (Exception)
            {
                // 無効な値の場合はWarningレベルにフォールバック
                CurrentLogLevel = LogLevel.Warning;
                _isLogLevelInitialized = true;
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
            }
            else
            {
                CurrentLogLevel = LogLevel.Warning;
            }

            // ログレベル初期化完了をマーク
            _isLogLevelInitialized = true;

            // 初期化完了後にログ出力
            LogInfo("Logger.InitializeLogLevel", "ログレベルを設定しました", CurrentLogLevel.ToString());
        }

        /// <summary>
        /// テスト専用：ログレベルの初期化状態を取得します。
        /// LoggerFixtureがテスト間で状態を退避・復元するために使用します。
        /// </summary>
        internal static bool IsLogLevelInitializedForTests => _isLogLevelInitialized;

        /// <summary>
        /// テスト専用：ログレベルと初期化フラグを指定の状態へ復元します。
        /// InitializeLogLevel()は「一度初期化したら再初期化しない」ことを前提とした
        /// staticフラグを持つため、これを戻さないとテストの実行順で結果が変わる。
        /// </summary>
        /// <param name="level">復元するログレベル</param>
        /// <param name="isInitialized">復元する初期化済みフラグ</param>
        internal static void RestoreLogLevelStateForTests(LogLevel level, bool isInitialized)
        {
            CurrentLogLevel = level;
            _isLogLevelInitialized = isInitialized;
        }
    }
}
