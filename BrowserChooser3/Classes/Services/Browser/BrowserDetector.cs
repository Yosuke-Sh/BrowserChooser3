using Microsoft.Win32;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.BrowserServices
{
    /// <summary>
    /// システムにインストールされているブラウザを自動検出するクラス
    /// </summary>
    public static class BrowserDetector
    {
        /// <summary>
        /// 検出されたブラウザのリスト
        /// </summary>
        public static List<Browser> DetectedBrowsers { get; private set; } = new();

        /// <summary>
        /// ブラウザを検出します
        /// </summary>
        /// <returns>検出されたブラウザのリスト</returns>
        public static List<Browser> DetectBrowsers()
        {
            Logger.LogInfo("BrowserDetector.DetectBrowsers", "Start");
            DetectedBrowsers.Clear();

            try
            {
                // Chrome
                DetectChrome();
                
                // Firefox
                DetectFirefox();
                
                // Edge
                DetectEdge();
                
                // Opera
                DetectOpera();
                
                // Safari
                DetectSafari();
                
                // Brave
                DetectBrave();
                
                // Vivaldi
                DetectVivaldi();
                
                Logger.LogInfo("BrowserDetector.DetectBrowsers", "End", DetectedBrowsers.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserDetector.DetectBrowsers", "ブラウザ検出エラー", ex.Message, ex.StackTrace ?? "");
            }

            return DetectedBrowsers;
        }

        /// <summary>
        /// Chromeを検出
        /// </summary>
        private static void DetectChrome()
        {
            var paths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe"
            };

            foreach (var path in paths)
            {
                var chromePath = GeneralUtilities.GetRegistryValue(path, "");
                if (!string.IsNullOrEmpty(chromePath) && System.IO.File.Exists(chromePath))
                {
                    var browser = new Browser
                    {
                        Name = "Google Chrome",
                        Target = chromePath,
                        Arguments = "--new-window",
                        Category = "Web Browsers",
                        IsActive = true,
                        Visible = true
                    };
                    DetectedBrowsers.Add(browser);
                    Logger.LogInfo("BrowserDetector.DetectChrome", "Chrome検出", chromePath);
                    break;
                }
            }
        }

        /// <summary>
        /// Firefoxを検出
        /// </summary>
        private static void DetectFirefox()
        {
            var paths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe"
            };

            foreach (var path in paths)
            {
                var firefoxPath = GeneralUtilities.GetRegistryValue(path, "");
                if (!string.IsNullOrEmpty(firefoxPath) && System.IO.File.Exists(firefoxPath))
                {
                    var browser = new Browser
                    {
                        Name = "Mozilla Firefox",
                        Target = firefoxPath,
                        Arguments = "-new-window",
                        Category = "Web Browsers",
                        IsActive = true,
                        Visible = true
                    };
                    DetectedBrowsers.Add(browser);
                    Logger.LogInfo("BrowserDetector.DetectFirefox", "Firefox検出", firefoxPath);
                    break;
                }
            }
        }

        /// <summary>
        /// Edgeを検出
        /// </summary>
        private static void DetectEdge()
        {
            var edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (System.IO.File.Exists(edgePath))
            {
                var browser = new Browser
                {
                    Name = "Microsoft Edge",
                    Target = edgePath,
                    Arguments = "--new-window",
                    Category = "Web Browsers",
                    IsActive = true,
                    Visible = true,
                    IsEdge = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.DetectEdge", "Edge検出", edgePath);
            }
        }



        /// <summary>
        /// Operaを検出
        /// </summary>
        private static void DetectOpera()
        {
            var operaPath = @"C:\Program Files\Opera\launcher.exe";
            if (System.IO.File.Exists(operaPath))
            {
                var browser = new Browser
                {
                    Name = "Opera",
                    Target = operaPath,
                    Arguments = "--new-window",
                    Category = "Web Browsers",
                    IsActive = true,
                    Visible = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.DetectOpera", "Opera検出", operaPath);
            }
        }

        /// <summary>
        /// Safariを検出
        /// </summary>
        private static void DetectSafari()
        {
            var safariPath = @"C:\Program Files\Safari\Safari.exe";
            if (System.IO.File.Exists(safariPath))
            {
                var browser = new Browser
                {
                    Name = "Safari",
                    Target = safariPath,
                    Arguments = "",
                    Category = "Web Browsers",
                    IsActive = true,
                    Visible = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.DetectSafari", "Safari検出", safariPath);
            }
        }

        /// <summary>
        /// Braveを検出
        /// </summary>
        private static void DetectBrave()
        {
            var bravePath = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
            if (System.IO.File.Exists(bravePath))
            {
                var browser = new Browser
                {
                    Name = "Brave Browser",
                    Target = bravePath,
                    Arguments = "--new-window",
                    Category = "Web Browsers",
                    IsActive = true,
                    Visible = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.DetectBrave", "Brave検出", bravePath);
            }
        }

        /// <summary>
        /// Vivaldiを検出
        /// </summary>
        private static void DetectVivaldi()
        {
            var vivaldiPath = @"C:\Users\%USERNAME%\AppData\Local\Vivaldi\Application\vivaldi.exe";
            var expandedPath = Environment.ExpandEnvironmentVariables(vivaldiPath);
            if (System.IO.File.Exists(expandedPath))
            {
                var browser = new Browser
                {
                    Name = "Vivaldi",
                    Target = expandedPath,
                    Arguments = "--new-window",
                    Category = "Web Browsers",
                    IsActive = true,
                    Visible = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.DetectVivaldi", "Vivaldi検出", expandedPath);
            }
        }

        /// <summary>
        /// カスタムブラウザを追加
        /// </summary>
        /// <param name="name">ブラウザ名</param>
        /// <param name="path">実行ファイルパス</param>
        /// <param name="arguments">起動引数</param>
        public static void AddCustomBrowser(string name, string path, string arguments = "")
        {
            if (System.IO.File.Exists(path))
            {
                var browser = new Browser
                {
                    Name = name,
                    Target = path,
                    Arguments = arguments,
                    Category = "Custom Browsers",
                    IsActive = true,
                    Visible = true
                };
                DetectedBrowsers.Add(browser);
                Logger.LogInfo("BrowserDetector.AddCustomBrowser", "カスタムブラウザ追加", name, path);
            }
        }
    }
}
