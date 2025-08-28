using System;
using System.IO;
using Microsoft.Win32;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.BrowserServices
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

                // Windows 11対応のデフォルトブラウザ設定
                if (IsWindows11())
                {
                    return SetDefaultBrowserWindows11(browserPath, protocol);
                }
                else
                {
                    return SetDefaultBrowserLegacy(browserPath, protocol);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.SetDefaultBrowser", "デフォルトブラウザ設定エラー", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Windows 11かどうかを判定
        /// </summary>
        /// <returns>Windows 11の場合はtrue</returns>
        private static bool IsWindows11()
        {
            try
            {
                var osVersion = Environment.OSVersion;
                return osVersion.Version.Major == 10 && osVersion.Version.Build >= 22000;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Windows 11用のデフォルトブラウザ設定
        /// </summary>
        /// <param name="browserPath">ブラウザのパス</param>
        /// <param name="protocol">プロトコル</param>
        /// <returns>成功した場合はtrue</returns>
        private static bool SetDefaultBrowserWindows11(string browserPath, string protocol)
        {
            try
            {
                // Windows 11では、アプリケーションの関連付けを使用
                var browserName = GetBrowserNameFromPath(browserPath);
                var appId = GetAppIdFromPath(browserPath);
                
                if (string.IsNullOrEmpty(appId))
                {
                    Logger.LogWarning("DefaultBrowserChecker.SetDefaultBrowserWindows11", "AppIdを取得できませんでした", browserPath);
                    return SetDefaultBrowserLegacy(browserPath, protocol);
                }
                
                // プロトコルハンドラーを設定
                var command = $"\"{browserPath}\" \"%1\"";
                
                // レジストリに設定
                using var key = Registry.ClassesRoot.CreateSubKey($"{protocol}\\shell\\open\\command");
                if (key != null)
                {
                    key.SetValue("", command);
                    
                    // Windows 11用の追加設定
                    using var userChoiceKey = Registry.CurrentUser.CreateSubKey(
                        $"Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\{protocol}\\UserChoice");
                    if (userChoiceKey != null)
                    {
                        userChoiceKey.SetValue("ProgId", appId);
                    }
                    
                    Logger.LogInfo("DefaultBrowserChecker.SetDefaultBrowserWindows11", "Windows 11デフォルトブラウザ設定完了", protocol);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.SetDefaultBrowserWindows11", "Windows 11デフォルトブラウザ設定エラー", ex.Message);
                return SetDefaultBrowserLegacy(browserPath, protocol);
            }
        }
        
        /// <summary>
        /// 従来のデフォルトブラウザ設定
        /// </summary>
        /// <param name="browserPath">ブラウザのパス</param>
        /// <param name="protocol">プロトコル</param>
        /// <returns>成功した場合はtrue</returns>
        private static bool SetDefaultBrowserLegacy(string browserPath, string protocol)
        {
            try
            {
                // プロトコルハンドラーを設定
                var command = $"\"{browserPath}\" \"%1\"";
                
                using var key = Registry.ClassesRoot.CreateSubKey($"{protocol}\\shell\\open\\command");
                if (key != null)
                {
                    key.SetValue("", command);
                    Logger.LogInfo("DefaultBrowserChecker.SetDefaultBrowserLegacy", "従来のデフォルトブラウザ設定完了", protocol);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("DefaultBrowserChecker.SetDefaultBrowserLegacy", "従来のデフォルトブラウザ設定エラー", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// パスからAppIdを取得
        /// </summary>
        /// <param name="browserPath">ブラウザのパス</param>
        /// <returns>AppId</returns>
        private static string GetAppIdFromPath(string browserPath)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(browserPath);
                
                // 主要ブラウザのAppId
                switch (fileName.ToLower())
                {
                    case "chrome":
                        return "ChromeHTML";
                    case "firefox":
                        return "FirefoxURL";
                    case "msedge":
                        return "MSEdgeHTM";
                    case "iexplore":
                        return "IE.HTTP";
                    case "opera":
                        return "OperaStable";
                    case "brave":
                        return "BraveHTML";
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
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

