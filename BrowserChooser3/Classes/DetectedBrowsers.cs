using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// ブラウザ検出クラス
    /// </summary>
    public static class DetectedBrowsers
    {
        /// <summary>
        /// ブラウザ検出を実行します
        /// </summary>
        /// <returns>検出されたブラウザのリスト</returns>
        public static List<Browser> DoBrowserDetection()
        {
            var detectedBrowsers = new List<Browser>();

            try
            {
                Logger.LogInfo("DetectedBrowsers.DoBrowserDetection", "ブラウザ検出開始");

                // 一般的なブラウザのパスを検索
                var commonPaths = GetCommonBrowserPaths();
                foreach (var path in commonPaths)
                {
                    if (File.Exists(path))
                    {
                        var browser = CreateBrowserFromPath(path);
                        if (browser != null)
                        {
                            detectedBrowsers.Add(browser);
                        }
                    }
                }

                // レジストリからブラウザを検索
                var registryBrowsers = GetBrowsersFromRegistry();
                foreach (var browser in registryBrowsers)
                {
                    if (!detectedBrowsers.Exists(b => b.Target == browser.Target))
                    {
                        detectedBrowsers.Add(browser);
                    }
                }

                Logger.LogInfo("DetectedBrowsers.DoBrowserDetection", $"検出完了: {detectedBrowsers.Count}個のブラウザを検出");
                return detectedBrowsers;
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.DoBrowserDetection", "ブラウザ検出エラー", ex.Message);
                return detectedBrowsers;
            }
        }

        /// <summary>
        /// 一般的なブラウザのパスを取得します
        /// </summary>
        /// <returns>ブラウザパスのリスト</returns>
        private static List<string> GetCommonBrowserPaths()
        {
            var paths = new List<string>();

            // Program Files (x86) のパス
            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            // Chrome
            paths.Add(Path.Combine(programFilesX86, "Google\\Chrome\\Application\\chrome.exe"));
            paths.Add(Path.Combine(programFiles, "Google\\Chrome\\Application\\chrome.exe"));

            // Firefox
            paths.Add(Path.Combine(programFilesX86, "Mozilla Firefox\\firefox.exe"));
            paths.Add(Path.Combine(programFiles, "Mozilla Firefox\\firefox.exe"));

            // Edge
            paths.Add(Path.Combine(programFilesX86, "Microsoft\\Edge\\Application\\msedge.exe"));
            paths.Add(Path.Combine(programFiles, "Microsoft\\Edge\\Application\\msedge.exe"));

            // Internet Explorer
            paths.Add(Path.Combine(programFiles, "Internet Explorer\\iexplore.exe"));

            // Opera
            paths.Add(Path.Combine(programFilesX86, "Opera\\launcher.exe"));
            paths.Add(Path.Combine(programFiles, "Opera\\launcher.exe"));

            // Safari
            paths.Add(Path.Combine(programFiles, "Safari\\Safari.exe"));

            return paths;
        }

        /// <summary>
        /// レジストリからブラウザを取得します
        /// </summary>
        /// <returns>ブラウザのリスト</returns>
        private static List<Browser> GetBrowsersFromRegistry()
        {
            var browsers = new List<Browser>();

            try
            {
                // HTTP プロトコルハンドラーからブラウザを検索
                using (var key = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("") as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            var path = ExtractPathFromCommand(value);
                            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                            {
                                var browser = CreateBrowserFromPath(path);
                                if (browser != null)
                                {
                                    browsers.Add(browser);
                                }
                            }
                        }
                    }
                }

                // HTTPS プロトコルハンドラーからブラウザを検索
                using (var key = Registry.ClassesRoot.OpenSubKey("https\\shell\\open\\command"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("") as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            var path = ExtractPathFromCommand(value);
                            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                            {
                                var browser = CreateBrowserFromPath(path);
                                if (browser != null && !browsers.Exists(b => b.Target == browser.Target))
                                {
                                    browsers.Add(browser);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.GetBrowsersFromRegistry", "レジストリ検索エラー", ex.Message);
            }

            return browsers;
        }

        /// <summary>
        /// コマンド文字列からパスを抽出します
        /// </summary>
        /// <param name="command">コマンド文字列</param>
        /// <returns>抽出されたパス</returns>
        private static string ExtractPathFromCommand(string command)
        {
            try
            {
                // ダブルクォートで囲まれたパスを抽出
                if (command.StartsWith("\"") && command.Contains("\""))
                {
                    var endQuote = command.IndexOf("\"", 1);
                    if (endQuote > 1)
                    {
                        return command.Substring(1, endQuote - 1);
                    }
                }

                // スペースで区切られた最初の部分を取得
                var parts = command.Split(' ');
                if (parts.Length > 0)
                {
                    return parts[0];
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.ExtractPathFromCommand", "パス抽出エラー", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// パスからブラウザオブジェクトを作成します
        /// </summary>
        /// <param name="path">ブラウザのパス</param>
        /// <returns>ブラウザオブジェクト</returns>
        private static Browser? CreateBrowserFromPath(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;

                var fileName = Path.GetFileNameWithoutExtension(path);
                var browser = new Browser
                {
                    Name = GetBrowserDisplayName(fileName),
                    Target = path,
                    Guid = Guid.NewGuid(),
                    Hotkey = '\0',
                    PosX = 1,
                    PosY = 1
                };

                return browser;
            }
            catch (Exception ex)
            {
                Logger.LogError("DetectedBrowsers.CreateBrowserFromPath", $"ブラウザ作成エラー: {path}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// ブラウザの表示名を取得します
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>表示名</returns>
        private static string GetBrowserDisplayName(string fileName)
        {
            return fileName.ToLower() switch
            {
                "chrome" => "Google Chrome",
                "firefox" => "Mozilla Firefox",
                "msedge" => "Microsoft Edge",
                "iexplore" => "Internet Explorer",
                "opera" => "Opera",
                "safari" => "Safari",
                _ => fileName
            };
        }
    }
}
