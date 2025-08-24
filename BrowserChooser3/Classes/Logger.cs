using System.Configuration;
using System.Text;

namespace BrowserChooser3.Classes
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
        /// ログメッセージのキュー
        /// </summary>
        private static readonly Queue<string> _logQueue = new Queue<string>();

        /// <summary>
        /// ログファイルのパス
        /// </summary>
        private static string LogFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
            "bc3.log");

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
                TextWriter writer;
                if (Application.StartupPath == Environment.SystemDirectory)
                {
                    writer = new StreamWriter(LogFilePath, true, Encoding.UTF8);
                }
                else
                {
                    var logPath = Path.Combine(Application.StartupPath, "bc3.log");
                    writer = new StreamWriter(logPath, true, Encoding.UTF8);
                }

                lock (_logQueue)
                {
                    while (_logQueue.Count > 0)
                    {
                        var logEntry = _logQueue.Dequeue();
                        writer.WriteLine(logEntry);
                    }
                }

                writer.Close();
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
