using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Diagnostics;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using System.Linq;

namespace BrowserChooser3.Classes.Services.BrowserServices
{
    /// <summary>
    /// ブラウザ検出クラス
    /// システムにインストールされているブラウザを検出し、オンライン更新に対応します
    /// </summary>
    public static class DetectedBrowsers
    {
        /// <summary>
        /// オンライン定義のURL
        /// </summary>
        private const string ONLINE_DEFINITIONS_URL = "https://raw.githubusercontent.com/browserchooser/browser-definitions/main/browsers.json";

        /// <summary>
        /// ローカル定義ファイルのパス
        /// </summary>
        private static string LocalDefinitionsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BrowserChooser3",
            "browser-definitions.json");

        /// <summary>
        /// ブラウザ検出を実行
        /// </summary>
        /// <param name="webBrowsersOnly">Webブラウザのみを検出するかどうか</param>
        /// <returns>検出されたブラウザのリスト</returns>
        public static List<Browser> DoBrowserDetection(bool webBrowsersOnly = false)
        {
            Logger.LogInfo("DetectedBrowsers.DoBrowserDetection", "ブラウザ検出開始", webBrowsersOnly);

            try
            {
                var detectedBrowsers = new List<Browser>();

                // ローカル定義から検出
                var localBrowsers = DetectFromLocalDefinitions(webBrowsersOnly);
                detectedBrowsers.AddRange(localBrowsers);

                // オンライン定義から検出（オプション）
                if (ShouldCheckOnlineDefinitions())
                {
                    var onlineBrowsers = DetectFromOnlineDefinitions(webBrowsersOnly);
                    detectedBrowsers.AddRange(onlineBrowsers);
                }

                // 重複を除去
                var uniqueBrowsers = RemoveDuplicates(detectedBrowsers);

                Logger.LogInfo("DetectedBrowsers.DoBrowserDetection", "ブラウザ検出完了", uniqueBrowsers.Count);
                return uniqueBrowsers;
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.DoBrowserDetection", "ブラウザ検出エラー", ex.Message);
                return new List<Browser>();
            }
        }

        /// <summary>
        /// ローカル定義からブラウザを検出
        /// </summary>
        /// <param name="webBrowsersOnly">Webブラウザのみを検出するかどうか</param>
        /// <returns>検出されたブラウザのリスト</returns>
        private static List<Browser> DetectFromLocalDefinitions(bool webBrowsersOnly = false)
        {
            var browsers = new List<Browser>();

            try
            {
                // 一般的なブラウザのインストールパスをチェック
                var commonPaths = GetCommonBrowserPaths();
                
                foreach (var path in commonPaths)
                {
                    // Webブラウザのみの場合は、Webブラウザカテゴリのみを処理
                    if (webBrowsersOnly && !IsWebBrowser(path.Category))
                    {
                        continue;
                    }
                    
                    if (File.Exists(path.ExecutablePath))
                    {
                        var browser = CreateBrowserFromPath(path);
                        if (browser != null)
                        {
                            browsers.Add(browser);
                        }
                    }
                }

                // レジストリから検出
                var registryBrowsers = DetectFromRegistry(webBrowsersOnly);
                browsers.AddRange(registryBrowsers);

                Logger.LogInfo("DetectedBrowsers.DetectFromLocalDefinitions", "ローカル検出完了", browsers.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.DetectFromLocalDefinitions", "ローカル検出エラー", ex.Message);
            }

            return browsers;
        }

        /// <summary>
        /// オンライン定義からブラウザを検出
        /// </summary>
        /// <param name="webBrowsersOnly">Webブラウザのみを検出するかどうか</param>
        /// <returns>検出されたブラウザのリスト</returns>
        private static List<Browser> DetectFromOnlineDefinitions(bool webBrowsersOnly = false)
        {
            var browsers = new List<Browser>();

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = httpClient.GetStringAsync(ONLINE_DEFINITIONS_URL).Result;
                var onlineDefinitions = JsonSerializer.Deserialize<List<OnlineBrowserDefinition>>(response);

                if (onlineDefinitions != null)
                {
                    foreach (var definition in onlineDefinitions)
                    {
                        // Webブラウザのみの場合は、Webブラウザカテゴリのみを処理
                        if (webBrowsersOnly && !IsWebBrowser(definition.Category))
                        {
                            continue;
                        }
                        
                        var browser = CreateBrowserFromOnlineDefinition(definition);
                        if (browser != null)
                        {
                            browsers.Add(browser);
                        }
                    }

                    // オンライン定義をローカルに保存
                    SaveOnlineDefinitions(onlineDefinitions);
                }

                Logger.LogInfo("DetectedBrowsers.DetectFromOnlineDefinitions", "オンライン検出完了", browsers.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.DetectFromOnlineDefinitions", "オンライン検出エラー", ex.Message);
            }

            return browsers;
        }

        /// <summary>
        /// オンライン定義のチェックが必要かどうかを判定
        /// </summary>
        /// <returns>チェックが必要な場合はtrue</returns>
        private static bool ShouldCheckOnlineDefinitions()
        {
            try
            {
                // 最後のチェック時刻を確認
                var lastCheckFile = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BrowserChooser3",
                    "last-online-check.txt");

                if (File.Exists(lastCheckFile))
                {
                    var lastCheckText = File.ReadAllText(lastCheckFile);
                    if (DateTime.TryParse(lastCheckText, out var lastCheck))
                    {
                        // 24時間以内にチェック済みの場合はスキップ
                        if (DateTime.Now - lastCheck < TimeSpan.FromHours(24))
                        {
                            return false;
                        }
                    }
                }

                // チェック時刻を記録
                var directory = Path.GetDirectoryName(lastCheckFile);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(lastCheckFile, DateTime.Now.ToString("O"));

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.ShouldCheckOnlineDefinitions", "オンライン定義チェック判定エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 一般的なブラウザのパスを取得
        /// </summary>
        /// <returns>ブラウザパスのリスト</returns>
        private static List<BrowserPathInfo> GetCommonBrowserPaths()
        {
            var paths = new List<BrowserPathInfo>();

            // Chrome
            paths.Add(new BrowserPathInfo
            {
                Name = "Google Chrome",
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Arguments = "",
                Category = "Popular"
            });

            paths.Add(new BrowserPathInfo
            {
                Name = "Google Chrome",
                ExecutablePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                Arguments = "",
                Category = "Popular"
            });

            // Firefox
            paths.Add(new BrowserPathInfo
            {
                Name = "Mozilla Firefox",
                ExecutablePath = @"C:\Program Files\Mozilla Firefox\firefox.exe",
                Arguments = "",
                Category = "Popular"
            });

            paths.Add(new BrowserPathInfo
            {
                Name = "Mozilla Firefox",
                ExecutablePath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe",
                Arguments = "",
                Category = "Popular"
            });

            // Edge
            paths.Add(new BrowserPathInfo
            {
                Name = "Microsoft Edge",
                ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                Arguments = "",
                Category = "Popular"
            });

            paths.Add(new BrowserPathInfo
            {
                Name = "Microsoft Edge",
                ExecutablePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
                Arguments = "",
                Category = "Popular"
            });

            // Opera
            paths.Add(new BrowserPathInfo
            {
                Name = "Opera",
                ExecutablePath = @"C:\Program Files\Opera\launcher.exe",
                Arguments = "",
                Category = "Alternative"
            });

            paths.Add(new BrowserPathInfo
            {
                Name = "Opera",
                ExecutablePath = @"C:\Program Files (x86)\Opera\launcher.exe",
                Arguments = "",
                Category = "Alternative"
            });

            // Safari
            paths.Add(new BrowserPathInfo
            {
                Name = "Safari",
                ExecutablePath = @"C:\Program Files\Safari\Safari.exe",
                Arguments = "",
                Category = "Alternative"
            });

            // Brave
            paths.Add(new BrowserPathInfo
            {
                Name = "Brave Browser",
                ExecutablePath = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
                Arguments = "",
                Category = "Alternative"
            });

            return paths;
        }

        /// <summary>
        /// レジストリからブラウザを検出
        /// </summary>
        /// <param name="webBrowsersOnly">Webブラウザのみを検出するかどうか</param>
        /// <returns>検出されたブラウザのリスト</returns>
        private static List<Browser> DetectFromRegistry(bool webBrowsersOnly = false)
        {
            var browsers = new List<Browser>();

            try
            {
                // アプリケーションのレジストリキーをチェック
                var appPaths = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths");
                if (appPaths != null)
                {
                    foreach (var subKeyName in appPaths.GetSubKeyNames())
                    {
                        if (subKeyName.EndsWith(".exe"))
                        {
                            var subKey = appPaths.OpenSubKey(subKeyName);
                            if (subKey != null)
                            {
                                var defaultValue = subKey.GetValue("") as string;
                                if (!string.IsNullOrEmpty(defaultValue) && File.Exists(defaultValue))
                                {
                                    // Webブラウザのみの場合は、Webブラウザ関連のファイル名のみを処理
                                    if (webBrowsersOnly && !IsWebBrowserExecutable(subKeyName))
                                    {
                                        continue;
                                    }
                                    
                                    var browser = CreateBrowserFromPath(new BrowserPathInfo
                                    {
                                        Name = Path.GetFileNameWithoutExtension(subKeyName),
                                        ExecutablePath = defaultValue,
                                        Arguments = "",
                                        Category = "Registry"
                                    });
                                    if (browser != null)
                                    {
                                        browsers.Add(browser);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.DetectFromRegistry", "レジストリ検出エラー", ex.Message);
            }

            return browsers;
        }

        /// <summary>
        /// パス情報からブラウザを作成
        /// </summary>
        /// <param name="pathInfo">パス情報</param>
        /// <returns>ブラウザオブジェクト</returns>
        private static Browser? CreateBrowserFromPath(BrowserPathInfo pathInfo)
        {
            try
            {
                if (!File.Exists(pathInfo.ExecutablePath))
                {
                    return null;
                }

                var fileInfo = new FileInfo(pathInfo.ExecutablePath);
                var versionInfo = FileVersionInfo.GetVersionInfo(pathInfo.ExecutablePath);

                return new Browser
                {
                    Name = !string.IsNullOrEmpty(versionInfo.ProductName) ? versionInfo.ProductName : pathInfo.Name,
                    Target = pathInfo.ExecutablePath,
                    Arguments = pathInfo.Arguments,
                    Guid = Guid.NewGuid(),
                    Hotkey = '\0',
                    PosX = 1,
                    PosY = 1,
                    Scale = 1.0,
                    IconIndex = 0,
                    Category = pathInfo.Category,
                    Visible = true,
                    IsDefault = false
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.CreateBrowserFromPath", "ブラウザ作成エラー", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// オンライン定義からブラウザを作成
        /// </summary>
        /// <param name="definition">オンライン定義</param>
        /// <returns>ブラウザオブジェクト</returns>
        private static Browser? CreateBrowserFromOnlineDefinition(OnlineBrowserDefinition definition)
        {
            try
            {
                // オンライン定義のパスが実際に存在するかチェック
                if (!File.Exists(definition.ExecutablePath))
                {
                    return null;
                }

                return new Browser
                {
                    Name = definition.Name,
                    Target = definition.ExecutablePath,
                    Arguments = definition.Arguments ?? "",
                    Guid = Guid.NewGuid(),
                    Hotkey = !string.IsNullOrEmpty(definition.Hotkey) ? definition.Hotkey[0] : '\0',
                    PosX = definition.PosX ?? 1,
                    PosY = definition.PosY ?? 1,
                    Scale = definition.Scale ?? 1.0,
                    IconIndex = definition.IconIndex ?? 0,
                    Category = definition.Category ?? "Online",
                    Visible = definition.Visible ?? true,
                    IsDefault = false
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.CreateBrowserFromOnlineDefinition", "オンライン定義からブラウザ作成エラー", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 重複するブラウザを除去
        /// </summary>
        /// <param name="browsers">ブラウザのリスト</param>
        /// <returns>重複除去されたブラウザのリスト</returns>
        private static List<Browser> RemoveDuplicates(List<Browser> browsers)
        {
            var uniqueBrowsers = new List<Browser>();
            var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var browser in browsers)
            {
                if (!string.IsNullOrEmpty(browser.Target) && !seenPaths.Contains(browser.Target))
                {
                    seenPaths.Add(browser.Target);
                    uniqueBrowsers.Add(browser);
                }
            }

            return uniqueBrowsers;
        }

        /// <summary>
        /// オンライン定義をローカルに保存
        /// </summary>
        /// <param name="definitions">オンライン定義</param>
        private static void SaveOnlineDefinitions(List<OnlineBrowserDefinition> definitions)
        {
            try
            {
                var directory = Path.GetDirectoryName(LocalDefinitionsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(definitions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(LocalDefinitionsPath, json);

                Logger.LogInfo("DetectedBrowsers.SaveOnlineDefinitions", "オンライン定義保存完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.SaveOnlineDefinitions", "オンライン定義保存エラー", ex.Message);
            }
        }

        /// <summary>
        /// 指定されたカテゴリがWebブラウザかどうかを判定
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <returns>Webブラウザの場合はtrue</returns>
        private static bool IsWebBrowser(string? category)
        {
            if (string.IsNullOrEmpty(category))
                return false;
                
            var webBrowserCategories = new[] { "Popular", "Web Browser", "Browser", "Internet" };
            return webBrowserCategories.Any(cat => 
                category.Equals(cat, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 指定された実行ファイル名がWebブラウザかどうかを判定
        /// </summary>
        /// <param name="executableName">実行ファイル名</param>
        /// <returns>Webブラウザの場合はtrue</returns>
        private static bool IsWebBrowserExecutable(string executableName)
        {
            var webBrowserExecutables = new[] 
            { 
                "chrome.exe", "firefox.exe", "msedge.exe", "iexplore.exe", 
                "opera.exe", "safari.exe", "brave.exe", "vivaldi.exe",
                "chromium.exe", "waterfox.exe", "pale-moon.exe", "seamonkey.exe"
            };
            
            return webBrowserExecutables.Any(exe => 
                executableName.Equals(exe, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// ブラウザパス情報
        /// </summary>
        private class BrowserPathInfo
        {
            public string Name { get; set; } = "";
            public string ExecutablePath { get; set; } = "";
            public string Arguments { get; set; } = "";
            public string Category { get; set; } = "";
        }

        /// <summary>
        /// オンライン定義
        /// </summary>
        private class OnlineBrowserDefinition
        {
            public string Name { get; set; } = "";
            public string ExecutablePath { get; set; } = "";
            public string? Arguments { get; set; }
            public string? Hotkey { get; set; }
            public int? PosX { get; set; }
            public int? PosY { get; set; }
            public double? Scale { get; set; }
            public int? IconIndex { get; set; }
            public string? Category { get; set; }
            public bool? Visible { get; set; }
        }
    }
}
