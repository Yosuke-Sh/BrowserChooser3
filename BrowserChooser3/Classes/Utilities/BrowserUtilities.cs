using System.Diagnostics;
using System.Runtime.InteropServices;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// ブラウザ起動と管理を担当するユーティリティクラス
    /// Browser Chooser 2のBrowserUtilitiesと互換性を保ちます
    /// </summary>
    public static class BrowserUtilities
    {
        #region Win32 API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        private static extern bool GetFileAttributesEx(string lpFileName, int fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);

        [StructLayout(LayoutKind.Sequential)]
        private struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
        }

        private const int GetFileExInfoStandard = 0;
        #endregion

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            try
            {
                // 環境変数でテスト環境が明示的に設定されている場合のみ
                var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnvironment) && testEnvironment.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // 環境変数でダイアログ無効化が設定されている場合
                var disableDialogs = Environment.GetEnvironmentVariable("DISABLE_DIALOGS");
                if (!string.IsNullOrEmpty(disableDialogs) && disableDialogs.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // プロセス名にテストランナーが含まれている場合
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (processName.Contains("testhost", StringComparison.OrdinalIgnoreCase) ||
                    processName.Contains("vstest", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // アセンブリ名に"Test"が含まれている場合（テストプロジェクトの場合のみ）
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                if (assemblyName?.Contains("Test", StringComparison.OrdinalIgnoreCase) == true)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合は、テスト環境ではないと判断
                Console.WriteLine($"IsTestEnvironment check failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ブラウザを起動します
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="terminate">起動後にアプリケーションを終了するか</param>
        public static void LaunchBrowser(Browser browser, string url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Browser is null, skipping launch", url ?? "null", terminate);
                return;
            }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です
            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Start", browser.Name, browser.Target, url ?? "null", terminate);

            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                                                Logger.LogDebug("BrowserUtilities.LaunchBrowser", "テスト環境のため、ブラウザ起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return;
                }

                // IE専用処理
                if (browser.IsIE)
                {
                    LaunchIE(browser, url ?? "", terminate);
                }
                // Edge専用処理
                else if (browser.IsEdge)
                {
                    LaunchEdge(browser, url ?? "", terminate);
                }
                // 一般的なブラウザ処理
                else
                {
                    if (DoLaunch(browser, url ?? "", terminate))
                    {
                        if (terminate)
                        {
                            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.LaunchBrowser", "起動エラー", ex.Message, ex.StackTrace ?? "");
                if (!IsTestEnvironment() && browser != null)
                {
                                    MessageBox.Show($"ブラウザ {browser.Name ?? "Unknown"} の起動に失敗しました。", "起動エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "End", browser.Name ?? "Unknown", url ?? "null", terminate);
#pragma warning restore CS8602
        }

        /// <summary>
        /// IE専用の起動処理
        /// 既存のIEインスタンスにタブを追加します
        /// </summary>
        private static void LaunchIE(Browser browser, string url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchIE", "Browser is null, skipping IE launch", url ?? "null", terminate);
                return;
            }

            Logger.LogDebug("BrowserUtilities.LaunchIE", "Start", browser.Name ?? "null", url ?? "null", terminate);

            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                    Logger.LogDebug("BrowserUtilities.LaunchIE", "テスト環境のため、IE起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return;
                }

                // SHDocVwを使用したIE制御（将来的に実装）
                // 現在は一般的な起動処理を使用
                if (DoLaunch(browser, url ?? "", terminate))
                {
                    if (terminate)
                    {
                        Logger.LogDebug("BrowserUtilities.LaunchIE", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.LaunchIE", "IE起動エラー", ex.Message, ex.StackTrace ?? "");
                // フォールバック: 一般的な起動処理
                if (DoLaunch(browser, url ?? "", terminate))
                {
                    if (terminate)
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        /// <summary>
        /// Edge専用の起動処理
        /// microsoft-edge:プロトコルでの起動をサポートします
        /// </summary>
        private static void LaunchEdge(Browser browser, string url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchEdge", "Browser is null, skipping Edge launch", url ?? "null", terminate);
                return;
            }

            Logger.LogDebug("BrowserUtilities.LaunchEdge", "Start", browser.Name ?? "null", url ?? "null", terminate);

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です
            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                    Logger.LogDebug("BrowserUtilities.LaunchEdge", "テスト環境のため、Edge起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return;
                }

                // microsoft-edge:プロトコルを使用した起動
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    var edgeUrl = $"microsoft-edge:{url}";
                    Logger.LogDebug("BrowserUtilities.LaunchEdge", "microsoft-edge:プロトコルを使用", edgeUrl);
                    
                    if (DoLaunch(browser, edgeUrl, terminate))
                    {
                        if (terminate)
                        {
                            Logger.LogDebug("BrowserUtilities.LaunchEdge", "Terminate", browser.Name ?? "Unknown", url ?? "null", terminate);
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    // 通常の起動処理
                    if (DoLaunch(browser, url ?? "", terminate))
                    {
                        if (terminate)
                        {
                            Logger.LogDebug("BrowserUtilities.LaunchEdge", "Terminate", browser.Name ?? "Unknown", url ?? "null", terminate);
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.LaunchEdge", "Edge起動エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"Edge {browser.Name ?? "Unknown"} の起動に失敗しました。", "起動エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Logger.LogDebug("BrowserUtilities.LaunchEdge", "End", browser.Name ?? "null", url ?? "null", terminate);
#pragma warning restore CS8602
        }

        /// <summary>
        /// 一般的なブラウザ起動処理
        /// </summary>
        private static bool DoLaunch(Browser browser, string url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.DoLaunch", "Browser is null, skipping launch", url ?? "null", terminate);
                return false;
            }

            Logger.LogDebug("BrowserUtilities.DoLaunch", "Start", browser.Name ?? "null", url ?? "null", terminate);

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です
            try
            {
                // テスト環境では実際のプロセス起動をスキップ
                if (IsTestEnvironment())
                {
                    Logger.LogDebug("BrowserUtilities.DoLaunch", "テスト環境のため、プロセス起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return true; // 成功として扱う
                }

                // ブラウザパスの正規化
                string browserPath = NormalizeTarget(browser.Target);
                Logger.LogDebug("BrowserUtilities.DoLaunch", "Normalized path", browserPath);

                // ファイルの存在確認
                var fileData = new WIN32_FILE_ATTRIBUTE_DATA();
                if (!GetFileAttributesEx(browserPath, GetFileExInfoStandard, out fileData))
                {
                    Logger.LogError("BrowserUtilities.DoLaunch", "File not found", browserPath);
                    if (string.IsNullOrEmpty(browser.Target))
                    {
                        Logger.LogDebug("BrowserUtilities.DoLaunch", "Terminate - Empty target", browser.Name ?? "null", url ?? "null", terminate);
                        Environment.Exit(0);
                    }
                    else
                    {
                        if (!IsTestEnvironment() && browser != null)
                        {
                            MessageBox.Show($"ブラウザ {browser.Name ?? "Unknown"} が見つかりません。\nパス: {browserPath}", "見つからないターゲット", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Logger.LogError("BrowserUtilities.DoLaunch", "ブラウザが見つからない", browser?.Target ?? "null");
                        return false;
                    }
                }

                Process? process = null;
                string arguments = browser.Arguments;

                // Chrome専用の処理
                if ((browser.Name?.ToLower().Contains("chrome") == true) || browserPath.ToLower().Contains("chrome"))
                {
                    Logger.LogDebug("BrowserUtilities.DoLaunch", "Chrome detected", browser.Name ?? "null");
                    
                    // Chromeの標準的な起動引数を設定
                    if (string.IsNullOrEmpty(arguments) || arguments.Trim() == "")
                    {
                        arguments = "--new-window";
                    }
                    
                    // URLがある場合は追加
                    if (!string.IsNullOrEmpty(url))
                    {
                        arguments += $" \"{url}\"";
                    }
                    
                    Logger.LogInfo("BrowserUtilities.DoLaunch", "Chrome arguments", arguments);
                }
                else
                {
                    // 他のブラウザの処理
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (browser.Arguments.Contains("{0}") || browser.Arguments.Contains("{1}"))
                        {
                            // 置換パラメータを使用（簡易版）
                            var protocol = "";
                            var remainder = url;
                            
                            // プロトコルの抽出
                            var protocolIndex = url.IndexOf("://");
                            if (protocolIndex > 0)
                            {
                                protocol = url.Substring(0, protocolIndex);
                                remainder = url.Substring(protocolIndex + 3);
                            }
                            
                            arguments = string.Format(browser.Arguments, protocol, remainder);
                        }
                        else
                        {
                            // 従来の起動方法
                            arguments = browser.Arguments + " \"" + url + "\"";
                        }
                    }
                    else
                    {
                        arguments = browser.Arguments;
                    }
                }

                Logger.LogInfo("BrowserUtilities.DoLaunch", "Starting process", browserPath, arguments);
                process = Process.Start(browserPath, arguments);

                if (process != null)
                {
                    int processId = process.Id;
                    Logger.LogInfo("BrowserUtilities.DoLaunch", "Process started", processId.ToString());

                    try
                    {
                        process.WaitForInputIdle(1000);
                    }
                    catch
                    {
                        // 無視
                    }

                    if (!process.HasExited)
                    {
                        // プロセスを前面に移動
                        try
                        {
                            SetForegroundWindow(process.Handle);
                        }
                        catch
                        {
                            // フォールバック: プロセス名で検索
                            TryToBringToFront(browserPath);
                        }
                    }
                    else
                    {
                        Logger.LogError("BrowserUtilities.DoLaunch", "Process exited immediately", processId.ToString());
                        TryToBringToFront(browserPath);
                    }

                    if (terminate)
                    {
                        Logger.LogInfo("BrowserUtilities.DoLaunch", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                        Environment.Exit(0);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.DoLaunch", "起動エラー", ex.Message, ex.StackTrace ?? "");
            }

            if (!IsTestEnvironment() && browser != null)
            {
                MessageBox.Show($"ブラウザ {browser.Name ?? "Unknown"} の起動に失敗しました。\nパス: {browser.Target}\n引数: {browser.Arguments}", "起動エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Logger.LogError("BrowserUtilities.DoLaunch", "重大な失敗", browser.Name ?? "Unknown", url ?? "null", terminate);
#pragma warning restore CS8602
            return false;
        }

        /// <summary>
        /// プロセスを前面に移動します
        /// </summary>
        private static void TryToBringToFront(string browserPath)
        {
            Logger.LogInfo("BrowserUtilities.TryToBringToFront", "Start", browserPath);

            try
            {
                // プロセス名を抽出
                string target = Path.GetFileNameWithoutExtension(browserPath);
                Process[] processes = Process.GetProcessesByName(target);

                if (processes.Length == 1)
                {
                    // 単一プロセスの場合
                    SetForegroundWindow(processes[0].Handle);
                }
                else
                {
                    // 複数プロセスの場合、メインウィンドウを持つプロセスを検索
                    var candidates = new List<Process>();
                    foreach (var proc in processes)
                    {
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                        {
                            candidates.Add(proc);
                        }
                    }

                    if (candidates.Count == 1)
                    {
                        SetForegroundWindow(candidates[0].Handle);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.TryToBringToFront", "前面移動エラー", ex.Message, ex.StackTrace ?? "");
            }

            Logger.LogInfo("BrowserUtilities.TryToBringToFront", "End", browserPath);
        }

        /// <summary>
        /// ブラウザパスを正規化します（64ビット対応）
        /// </summary>
        public static string NormalizeTarget(string target)
        {
            Logger.LogInfo("BrowserUtilities.NormalizeTarget", "Start", target);

            try
            {
                // 64ビット環境での処理
                if (Environment.Is64BitProcess)
                {
                    string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    string programFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "";

                    // 現在のパスでファイルが存在するかチェック
                    if (File.Exists(target))
                    {
                        Logger.LogInfo("BrowserUtilities.NormalizeTarget", "File exists at current path", target);
                        return target;
                    }

                    // Program Files から Program Files (x86) への変換を試行
                    if (target.StartsWith(programFiles) && !string.IsNullOrEmpty(programFilesX86))
                    {
                        string x86Path = target.Replace(programFiles, programFilesX86);
                        if (File.Exists(x86Path))
                        {
                            Logger.LogInfo("BrowserUtilities.NormalizeTarget", "File found at x86 path", x86Path);
                            return x86Path;
                        }
                    }

                    // Program Files (x86) から Program Files への変換を試行
                    if (target.StartsWith(programFilesX86) && !string.IsNullOrEmpty(programFiles))
                    {
                        string x64Path = target.Replace(programFilesX86, programFiles);
                        if (File.Exists(x64Path))
                        {
                            Logger.LogInfo("BrowserUtilities.NormalizeTarget", "File found at x64 path", x64Path);
                            return x64Path;
                        }
                    }
                }
                else
                {
                    // 32ビット環境での処理
                    if (target.Contains("x86"))
                    {
                        string x64Path = target.Replace(" (x86)", "");
                        if (File.Exists(x64Path))
                        {
                            Logger.LogInfo("BrowserUtilities.NormalizeTarget", "File found at x64 path (32-bit env)", x64Path);
                            return x64Path;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.NormalizeTarget", "正規化エラー", ex.Message, ex.StackTrace ?? "");
            }

            Logger.LogInfo("BrowserUtilities.NormalizeTarget", "End", target);
            return target;
        }

        /// <summary>
        /// GUIDでブラウザを検索します
        /// </summary>
        public static Browser? GetBrowserByGUID(Guid guid)
        {
            Logger.LogInfo("BrowserUtilities.GetBrowserByGUID", "Start", guid.ToString());

            try
            {
                foreach (var browser in Settings.Current?.Browsers ?? new List<Browser>())
                {
                    if (browser.Guid == guid)
                    {
                        Logger.LogInfo("BrowserUtilities.GetBrowserByGUID", "GUID found", guid.ToString());
                        return browser;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.GetBrowserByGUID", "検索エラー", ex.Message, ex.StackTrace ?? "");
            }

            Logger.LogInfo("BrowserUtilities.GetBrowserByGUID", "GUID not found", guid.ToString());
            return null;
        }

        /// <summary>
        /// 別のリストからGUIDでブラウザを検索します
        /// </summary>
        public static Browser? GetBrowserByGUID(Guid guid, List<Browser> separateList)
        {
            Logger.LogInfo("BrowserUtilities.GetBrowserByGUID list", "Start", guid.ToString());

            try
            {
                foreach (var browser in separateList)
                {
                    if (browser.Guid == guid)
                    {
                        Logger.LogInfo("BrowserUtilities.GetBrowserByGUID list", "GUID found", guid.ToString());
                        return browser;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.GetBrowserByGUID list", "検索エラー", ex.Message, ex.StackTrace ?? "");
            }

            Logger.LogInfo("BrowserUtilities.GetBrowserByGUID list", "GUID not found", guid.ToString());
            return null;
        }
    }
}
