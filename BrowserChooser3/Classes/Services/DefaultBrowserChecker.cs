using System;
using System.IO;
using Microsoft.Win32;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services
{
    /// <summary>
    /// デフォルトブラウザチェック機能を提供するクラス
    /// システム設定との同期とデフォルトブラウザの検出を行います
    /// </summary>
    public static class DefaultBrowserChecker
    {
        /// <summary>
        /// デフォルトブラウザの情報
        /// </summary>
        public class DefaultBrowserInfo
        {
            /// <summary>ブラウザ名</summary>
            public string Name { get; set; } = string.Empty;
            
            /// <summary>実行ファイルパス</summary>
            public string Path { get; set; } = string.Empty;
            
            /// <summary>起動引数</summary>
            public string Arguments { get; set; } = string.Empty;
            
            /// <summary>プロトコルハンドラー</summary>
            public string Protocol { get; set; } = string.Empty;
            
            /// <summary>検出方法</summary>
            public string DetectionMethod { get; set; } = string.Empty;
        }

        /// <summary>
        /// システムのデフォルトブラウザを検出します
        /// </summary>
        /// <returns>デフォルトブラウザ情報</returns>
        public static DefaultBrowserInfo GetDefaultBrowser()
        {
            Logger.LogInfo("DefaultBrowserChecker.GetDefaultBrowser", "デフォルトブラウザ検出開始");

            try
            {
                // HTTPプロトコルハンドラーから検出
                var httpBrowser = GetBrowserFromProtocol("http");
                if (httpBrowser != null)
                {
                    Logger.LogInfo("DefaultBrowserChecker.GetDefaultBrowser", "HTTPプロトコルから検出", httpBrowser.Name);
                    return httpBrowser;
                }

                // HTTPSプロトコルハンドラーから検出
                var httpsBrowser = GetBrowserFromProtocol("https");
                if (httpsBrowser != null)
                {
                    Logger.LogInfo("DefaultBrowserChecker.GetDefaultBrowser", "HTTPSプロトコルから検出", httpsBrowser.Name);
                    return httpsBrowser;
                }

                // レジストリから直接検出
                var registryBrowser = GetBrowserFromRegistry();
                if (registryBrowser != null)
                {
                    Logger.LogInfo("DefaultBrowserChecker.GetDefaultBrowser", "レジストリから検出", registryBrowser.Name);
                    return registryBrowser;
                }

                Logger.LogWarning("DefaultBrowserChecker.GetDefaultBrowser", "デフォルトブラウザを検出できませんでした");
                return new DefaultBrowserInfo { Name = "Unknown", DetectionMethod = "Not Found" };
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.GetDefaultBrowser", "デフォルトブラウザ検出エラー", ex.Message);
                return new DefaultBrowserInfo { Name = "Error", DetectionMethod = "Exception" };
            }
        }

        /// <summary>
        /// プロトコルハンドラーからブラウザを検出します
        /// </summary>
        /// <param name="protocol">プロトコル名</param>
        /// <returns>ブラウザ情報</returns>
        private static DefaultBrowserInfo? GetBrowserFromProtocol(string protocol)
        {
            try
            {
                using var key = Registry.ClassesRoot.OpenSubKey($"{protocol}\\shell\\open\\command");
                if (key != null)
                {
                    var value = key.GetValue("") as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var (path, arguments) = ParseCommandString(value);
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            var browserName = GetBrowserNameFromPath(path);
                            return new DefaultBrowserInfo
                            {
                                Name = browserName,
                                Path = path,
                                Arguments = arguments,
                                Protocol = protocol,
                                DetectionMethod = $"Protocol Handler ({protocol})"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.GetBrowserFromProtocol", $"プロトコル{protocol}からの検出エラー", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// レジストリからブラウザを検出します
        /// </summary>
        /// <returns>ブラウザ情報</returns>
        private static DefaultBrowserInfo? GetBrowserFromRegistry()
        {
            try
            {
                // 一般的なブラウザのレジストリキーをチェック
                var browserKeys = new[]
                {
                    @"SOFTWARE\Clients\StartMenuInternet\IEXPLORE.EXE\shell\open\command",
                    @"SOFTWARE\Clients\StartMenuInternet\chrome.exe\shell\open\command",
                    @"SOFTWARE\Clients\StartMenuInternet\firefox.exe\shell\open\command",
                    @"SOFTWARE\Clients\StartMenuInternet\msedge.exe\shell\open\command"
                };

                foreach (var browserKey in browserKeys)
                {
                    using var key = Registry.LocalMachine.OpenSubKey(browserKey);
                    if (key != null)
                    {
                        var value = key.GetValue("") as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            var (path, arguments) = ParseCommandString(value);
                            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                            {
                                var browserName = GetBrowserNameFromPath(path);
                                return new DefaultBrowserInfo
                                {
                                    Name = browserName,
                                    Path = path,
                                    Arguments = arguments,
                                    Protocol = "registry",
                                    DetectionMethod = "Registry"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.GetBrowserFromRegistry", "レジストリからの検出エラー", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// コマンド文字列をパスと引数に分割します
        /// </summary>
        /// <param name="command">コマンド文字列</param>
        /// <returns>(パス, 引数)のタプル</returns>
        private static (string path, string arguments) ParseCommandString(string command)
        {
            try
            {
                command = command.Trim();
                
                // ダブルクォートで囲まれたパスを抽出
                if (command.StartsWith("\"") && command.Contains("\""))
                {
                    var endQuote = command.IndexOf("\"", 1);
                    if (endQuote > 1)
                    {
                        var path = command.Substring(1, endQuote - 1);
                        var arguments = command.Substring(endQuote + 1).Trim();
                        return (path, arguments);
                    }
                }

                // スペースで区切られた最初の部分をパスとして取得
                var spaceIndex = command.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    var path = command.Substring(0, spaceIndex);
                    var arguments = command.Substring(spaceIndex + 1).Trim();
                    return (path, arguments);
                }

                return (command, string.Empty);
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.ParseCommandString", "コマンド文字列解析エラー", ex.Message);
                return (string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// パスからブラウザ名を取得します
        /// </summary>
        /// <param name="path">実行ファイルパス</param>
        /// <returns>ブラウザ名</returns>
        private static string GetBrowserNameFromPath(string path)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(path).ToLower();
                return fileName switch
                {
                    "iexplore" => "Internet Explorer",
                    "chrome" => "Google Chrome",
                    "firefox" => "Mozilla Firefox",
                    "msedge" => "Microsoft Edge",
                    "opera" => "Opera",
                    "safari" => "Safari",
                    _ => fileName
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.GetBrowserNameFromPath", "ブラウザ名取得エラー", ex.Message);
                return "Unknown";
            }
        }

        /// <summary>
        /// デフォルトブラウザが設定されているかどうかをチェックします
        /// </summary>
        /// <returns>設定されている場合はtrue</returns>
        public static bool HasDefaultBrowser()
        {
            try
            {
                var defaultBrowser = GetDefaultBrowser();
                return !string.IsNullOrEmpty(defaultBrowser.Path) && 
                       File.Exists(defaultBrowser.Path) && 
                       defaultBrowser.Name != "Unknown" && 
                       defaultBrowser.Name != "Error";
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.HasDefaultBrowser", "デフォルトブラウザチェックエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// デフォルトブラウザの設定を更新します
        /// </summary>
        /// <param name="browserPath">ブラウザのパス</param>
        /// <param name="protocol">プロトコル</param>
        /// <returns>成功した場合はtrue</returns>
        public static bool SetDefaultBrowser(string browserPath, string protocol = "http")
        {
            Logger.LogInfo("DefaultBrowserChecker.SetDefaultBrowser", "デフォルトブラウザ設定開始", browserPath);

            try
            {
                if (!File.Exists(browserPath))
                {
                    Logger.LogError("DefaultBrowserChecker.SetDefaultBrowser", "ブラウザファイルが存在しません", browserPath);
                    return false;
                }

                // プロトコルハンドラーを設定
                var command = $"\"{browserPath}\" \"%1\"";
                
                using var key = Registry.ClassesRoot.CreateSubKey($"{protocol}\\shell\\open\\command");
                if (key != null)
                {
                    key.SetValue("", command);
                    Logger.LogInfo("DefaultBrowserChecker.SetDefaultBrowser", "デフォルトブラウザ設定完了", protocol);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.SetDefaultBrowser", "デフォルトブラウザ設定エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// デフォルトブラウザの設定をリセットします
        /// </summary>
        /// <param name="protocol">プロトコル</param>
        /// <returns>成功した場合はtrue</returns>
        public static bool ResetDefaultBrowser(string protocol = "http")
        {
            Logger.LogInfo("DefaultBrowserChecker.ResetDefaultBrowser", "デフォルトブラウザリセット開始", protocol);

            try
            {
                var keyPath = $"{protocol}\\shell\\open\\command";
                Registry.ClassesRoot.DeleteSubKey(keyPath, false);
                Logger.LogInfo("DefaultBrowserChecker.ResetDefaultBrowser", "デフォルトブラウザリセット完了", protocol);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.ResetDefaultBrowser", "デフォルトブラウザリセットエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// デフォルトブラウザの情報を取得します（詳細版）
        /// </summary>
        /// <returns>詳細なブラウザ情報</returns>
        public static string GetDefaultBrowserDetails()
        {
            try
            {
                var defaultBrowser = GetDefaultBrowser();
                return $"Name: {defaultBrowser.Name}\n" +
                       $"Path: {defaultBrowser.Path}\n" +
                       $"Arguments: {defaultBrowser.Arguments}\n" +
                       $"Protocol: {defaultBrowser.Protocol}\n" +
                       $"Detection Method: {defaultBrowser.DetectionMethod}";
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.GetDefaultBrowserDetails", "詳細情報取得エラー", ex.Message);
                return "Error getting default browser details";
            }
        }
    }
}

