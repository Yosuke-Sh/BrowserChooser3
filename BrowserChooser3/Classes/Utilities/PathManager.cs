

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// アプリケーションのパスを管理するクラス
    /// iniファイルの設定に基づいて、設定ファイルやログファイルの出力先を決定します
    /// </summary>
    public static class PathManager
    {
        private static string? _iniFilePath;
        private static bool _useExeDirectory;
        private static string? _logDirectory;
        private static string? _configDirectory;

        /// <summary>
        /// パス管理を初期化します
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // iniファイルのパスを決定
                _iniFilePath = Path.Combine(Application.StartupPath, "BrowserChooser3.ini");
                
                Logger.LogDebug("PathManager.Initialize", "INIファイルパス", _iniFilePath);

                // iniファイルから設定を読み込み
                LoadSettings();

                Logger.LogDebug("PathManager.Initialize", "パス管理初期化完了", 
                    $"UseExeDirectory: {_useExeDirectory}, " +
                    $"LogDirectory: {_logDirectory}, " +
                    $"ConfigDirectory: {_configDirectory}");
            }
            catch (Exception ex)
            {
                Logger.LogError("PathManager.Initialize", "パス管理初期化エラー", ex.Message);
                // エラーが発生した場合はデフォルト設定を使用
                SetDefaultSettings();
            }
        }

        /// <summary>
        /// iniファイルから設定を読み込みます
        /// </summary>
        private static void LoadSettings()
        {
            if (string.IsNullOrEmpty(_iniFilePath) || !File.Exists(_iniFilePath))
            {
                Logger.LogDebug("PathManager.LoadSettings", "INIファイルが存在しないためデフォルト設定を使用");
                SetDefaultSettings();
                return;
            }

            // 出力フォルダの選択
            _useExeDirectory = IniFileReader.ReadBoolValue(_iniFilePath, "Paths", "UseExeDirectory", false);

            // ログディレクトリ
            _logDirectory = IniFileReader.ReadPathValue(_iniFilePath, "Paths", "LogDirectory", "");

            // 設定ファイルディレクトリ
            _configDirectory = IniFileReader.ReadPathValue(_iniFilePath, "Paths", "ConfigDirectory", "");
        }

        /// <summary>
        /// デフォルト設定を設定します
        /// </summary>
        private static void SetDefaultSettings()
        {
            _useExeDirectory = false;
            _logDirectory = "";
            _configDirectory = "";
        }

        /// <summary>
        /// 設定ファイルの出力先ディレクトリを取得します
        /// </summary>
        /// <returns>設定ファイルの出力先ディレクトリ</returns>
        public static string GetConfigDirectory()
        {
            if (!string.IsNullOrEmpty(_configDirectory))
            {
                return _configDirectory;
            }

            if (_useExeDirectory)
            {
                return Application.StartupPath;
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BrowserChooser3");
        }

        /// <summary>
        /// ログファイルの出力先ディレクトリを取得します
        /// </summary>
        /// <returns>ログファイルの出力先ディレクトリ</returns>
        public static string GetLogDirectory()
        {
            if (!string.IsNullOrEmpty(_logDirectory))
            {
                return _logDirectory;
            }

            if (_useExeDirectory)
            {
                return Path.Combine(Application.StartupPath, "Logs");
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrowserChooser3", "Logs");
        }

        /// <summary>
        /// 設定ファイルの完全パスを取得します
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>設定ファイルの完全パス</returns>
        public static string GetConfigFilePath(string fileName)
        {
            var configDir = GetConfigDirectory();
            return Path.Combine(configDir, fileName);
        }

        /// <summary>
        /// ログファイルの完全パスを取得します
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ログファイルの完全パス</returns>
        public static string GetLogFilePath(string fileName)
        {
            var logDir = GetLogDirectory();
            return Path.Combine(logDir, fileName);
        }

        /// <summary>
        /// 必要なディレクトリを作成します
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            try
            {
                // 設定ファイルディレクトリ
                var configDir = GetConfigDirectory();
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                    Logger.LogDebug("PathManager.EnsureDirectoriesExist", "設定ファイルディレクトリを作成", configDir);
                }

                // ログディレクトリ
                var logDir = GetLogDirectory();
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                    Logger.LogDebug("PathManager.EnsureDirectoriesExist", "ログディレクトリを作成", logDir);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("PathManager.EnsureDirectoriesExist", "ディレクトリ作成エラー", ex.Message);
            }
        }
    }
}
