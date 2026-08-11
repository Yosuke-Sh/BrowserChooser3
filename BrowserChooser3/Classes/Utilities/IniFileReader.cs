using System.Text.RegularExpressions;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// INIファイルを読み込むためのユーティリティクラス
    /// </summary>
    public static class IniFileReader
    {
        /// <summary>
        /// INIファイルから値を読み込む
        /// </summary>
        /// <param name="filePath">INIファイルのパス</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>読み込まれた値、見つからない場合はデフォルト値</returns>
        public static string ReadValue(string filePath, string section, string key, string defaultValue = "")
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Logger.LogDebug("IniFileReader.ReadValue", "INIファイルが存在しません", filePath);
                    return defaultValue;
                }

                var lines = File.ReadAllLines(filePath);
                bool inTargetSection = false;

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    // コメント行をスキップ
                    if (trimmedLine.StartsWith(";") || string.IsNullOrEmpty(trimmedLine))
                        continue;

                    // セクション行をチェック
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        var sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        inTargetSection = string.Equals(sectionName, section, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    // 対象セクション内でキーを検索
                    if (inTargetSection)
                    {
                        var keyValue = trimmedLine.Split('=', 2);
                        if (keyValue.Length == 2)
                        {
                            var currentKey = keyValue[0].Trim();
                            if (string.Equals(currentKey, key, StringComparison.OrdinalIgnoreCase))
                            {
                                var value = keyValue[1].Trim();
                                Logger.LogDebug("IniFileReader.ReadValue", "INIファイルから値を読み込み", filePath, section, key, value);
                                return value;
                            }
                        }
                    }
                }

                Logger.LogDebug("IniFileReader.ReadValue", "キーが見つかりません", filePath, section, key);
                return defaultValue;
            }
            catch (Exception ex)
            {
                Logger.LogError("IniFileReader.ReadValue", "INIファイル読み込みエラー", filePath, ex.Message);
                return defaultValue;
            }
        }

        /// <summary>
        /// INIファイルからブール値を読み込む
        /// </summary>
        /// <param name="filePath">INIファイルのパス</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>読み込まれたブール値、見つからない場合はデフォルト値</returns>
        public static bool ReadBoolValue(string filePath, string section, string key, bool defaultValue = false)
        {
            var value = ReadValue(filePath, section, key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// INIファイルからパスを読み込み、必要に応じて展開する
        /// </summary>
        /// <param name="filePath">INIファイルのパス</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>展開されたパス</returns>
        public static string ReadPathValue(string filePath, string section, string key, string defaultValue = "")
        {
            var value = ReadValue(filePath, section, key, defaultValue);
            
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            // 環境変数を展開
            value = Environment.ExpandEnvironmentVariables(value);
            
            // 相対パスの場合は絶対パスに変換
            if (!Path.IsPathRooted(value))
            {
                var iniFileDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(iniFileDir))
                {
                    value = Path.Combine(iniFileDir, value);
                }
            }

            return value;
        }
    }
}
