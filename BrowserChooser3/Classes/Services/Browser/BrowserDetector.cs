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
            Logger.LogDebug("BrowserDetector.DetectBrowsers", "Start");
            DetectedBrowsers.Clear();

            // 1つのブラウザの検出で例外が発生しても、他のブラウザの検出を継続できるよう
            // 各Detectメソッド内部で個別にtry/catchする（DetectByRegistryPaths/DetectByFixedPath参照）
            DetectChrome();
            DetectFirefox();
            DetectEdge();
            DetectOpera();
            DetectSafari();
            DetectBrave();
            DetectVivaldi();

            Logger.LogDebug("BrowserDetector.DetectBrowsers", "End", DetectedBrowsers.Count);

            return DetectedBrowsers;
        }

        /// <summary>
        /// Chromeを検出
        /// </summary>
        private static void DetectChrome()
        {
            DetectByRegistryPaths("DetectChrome", "Google Chrome", "--new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe"
            });
        }

        /// <summary>
        /// Firefoxを検出
        /// </summary>
        private static void DetectFirefox()
        {
            DetectByRegistryPaths("DetectFirefox", "Mozilla Firefox", "-new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe"
            });
        }

        /// <summary>
        /// Edgeを検出
        /// </summary>
        private static void DetectEdge()
        {
            DetectByRegistryPathsThenFixedPath("DetectEdge", "Microsoft Edge", "--new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe"
            }, @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", isEdge: true);
        }

        /// <summary>
        /// Operaを検出
        /// </summary>
        private static void DetectOpera()
        {
            DetectByRegistryPathsThenFixedPath("DetectOpera", "Opera", "--new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\opera.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\opera.exe"
            }, @"C:\Program Files\Opera\launcher.exe");
        }

        /// <summary>
        /// Safariを検出
        /// </summary>
        private static void DetectSafari()
        {
            // Safariは配布終了済みで、レジストリApp Pathsに登録される慣習を持たないため固定パスのみで検出する
            DetectByFixedPath("DetectSafari", "Safari", @"C:\Program Files\Safari\Safari.exe", "");
        }

        /// <summary>
        /// Braveを検出
        /// </summary>
        private static void DetectBrave()
        {
            DetectByRegistryPathsThenFixedPath("DetectBrave", "Brave Browser", "--new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\brave.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\brave.exe"
            }, @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe");
        }

        /// <summary>
        /// Vivaldiを検出
        /// </summary>
        private static void DetectVivaldi()
        {
            var fixedPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Vivaldi\Application\vivaldi.exe");

            DetectByRegistryPathsThenFixedPath("DetectVivaldi", "Vivaldi", "--new-window", new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\vivaldi.exe",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\vivaldi.exe"
            }, fixedPath);
        }

        /// <summary>
        /// レジストリのApp Pathsキー群を順に確認し、最初に見つかった実行ファイルをブラウザとして登録します
        /// </summary>
        private static void DetectByRegistryPaths(string callerName, string browserName, string arguments, string[] registryPaths)
        {
            DetectByRegistryPaths(callerName, browserName, arguments, registryPaths, isEdge: false);
        }

        /// <summary>
        /// レジストリのApp Pathsキー群を順に確認し、最初に見つかった実行ファイルをブラウザとして登録します。
        /// 見つかった場合はtrueを返します。
        /// </summary>
        private static bool DetectByRegistryPaths(string callerName, string browserName, string arguments, string[] registryPaths, bool isEdge)
        {
            try
            {
                foreach (var registryPath in registryPaths)
                {
                    var exePath = GeneralUtilities.GetRegistryValue(registryPath, "");
                    if (!string.IsNullOrEmpty(exePath) && System.IO.File.Exists(exePath))
                    {
                        AddDetectedBrowser(callerName, browserName, exePath, arguments, isEdge);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"BrowserDetector.{callerName}", $"{browserName}の検出中にエラーが発生しました", ex.Message, ex.StackTrace ?? "");
            }

            return false;
        }

        /// <summary>
        /// まずレジストリのApp Pathsで検出を試み、見つからない場合は既知の固定インストールパスにフォールバックします
        /// </summary>
        private static void DetectByRegistryPathsThenFixedPath(string callerName, string browserName, string arguments, string[] registryPaths, string fallbackFixedPath, bool isEdge = false)
        {
            var foundInRegistry = DetectByRegistryPaths(callerName, browserName, arguments, registryPaths, isEdge);
            if (!foundInRegistry)
            {
                DetectByFixedPath(callerName, browserName, fallbackFixedPath, arguments, isEdge);
            }
        }

        /// <summary>
        /// 固定インストールパスの実行ファイル存在を確認し、見つかった場合はブラウザとして登録します
        /// </summary>
        private static void DetectByFixedPath(string callerName, string browserName, string exePath, string arguments, bool isEdge = false)
        {
            try
            {
                if (System.IO.File.Exists(exePath))
                {
                    AddDetectedBrowser(callerName, browserName, exePath, arguments, isEdge);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"BrowserDetector.{callerName}", $"{browserName}の検出中にエラーが発生しました", ex.Message, ex.StackTrace ?? "");
            }
        }

        private static void AddDetectedBrowser(string callerName, string browserName, string exePath, string arguments, bool isEdge = false)
        {
            var browser = new Browser
            {
                Name = browserName,
                Target = exePath,
                Arguments = arguments,
                Category = "Web Browsers",
                IsActive = true,
                Visible = true,
                IsEdge = isEdge
            };

            lock (DetectedBrowsers)
            {
                DetectedBrowsers.Add(browser);
            }

            Logger.LogDebug($"BrowserDetector.{callerName}", $"{browserName}検出", exePath);
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

                // スレッドセーフな追加
                lock (DetectedBrowsers)
                {
                    DetectedBrowsers.Add(browser);
                }

                Logger.LogDebug("BrowserDetector.AddCustomBrowser", "カスタムブラウザ追加", name, path);
            }
        }
    }
}
