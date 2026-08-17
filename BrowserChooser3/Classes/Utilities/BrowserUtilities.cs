using System.Diagnostics;
using System.Runtime.InteropServices;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.BrowserServices;

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
        /// テスト環境かどうかを判定する（判定結果はプロセス起動中一度だけ評価してキャッシュされる）
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            return TestEnvironmentDetector.IsTestEnvironment();
        }

        /// <summary>
        /// ブラウザを起動します
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="terminate">起動後にアプリケーションを終了する想定か</param>
        /// <returns>起動に成功し、かつterminateがtrueだった場合はtrue（呼び出し元での終了処理が必要なことを示す）</returns>
        public static bool LaunchBrowser(Browser browser, string url, bool terminate)
            => LaunchBrowser(browser, url, terminate, forcePrivateMode: false, profileOverride: null);

        /// <summary>
        /// プロファイル指定・シークレット起動に対応したブラウザ起動処理です。
        ///
        /// forcePrivateMode または profileOverride が指定された場合は、
        /// IE/Edge の専用プロトコル経路（これらは追加の引数を受け付けない）ではなく
        /// 実行ファイルを直接起動する経路を使用します。
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="terminate">起動後にアプリケーションを終了する想定か</param>
        /// <param name="forcePrivateMode">ブラウザ設定に関わらずシークレット起動する場合はtrue</param>
        /// <param name="profileOverride">使用するプロファイル名（nullならブラウザ設定に従う）</param>
        /// <returns>起動に成功し、かつterminateがtrueだった場合はtrue（呼び出し元での終了処理が必要なことを示す）</returns>
        public static bool LaunchBrowser(Browser browser, string url, bool terminate,
            bool forcePrivateMode, string? profileOverride)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Browser is null, skipping launch", url ?? "null", terminate);
                return false;
            }

            // プロファイル/シークレットの明示指定がある場合は、追加引数を渡せる
            // DoLaunch経路を使う（IE/Edgeの専用プロトコル経路では指定を反映できないため）
            // ブラウザ設定側でプロファイル/シークレットが指定されている場合も同様に直接起動する
            var requiresDirectLaunch =
                forcePrivateMode ||
                browser.UsePrivateMode ||
                !string.IsNullOrWhiteSpace(profileOverride) ||
                !string.IsNullOrWhiteSpace(browser.ProfileName);

            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Start", browser.Name, browser.Target, url ?? "null", terminate);

            bool shouldTerminate = false;
            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                                                Logger.LogDebug("BrowserUtilities.LaunchBrowser", "テスト環境のため、ブラウザ起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return false;
                }

                // IE専用処理
                if (browser.IsIE && !requiresDirectLaunch)
                {
                    shouldTerminate = LaunchIE(browser, url ?? "", terminate);
                }
                // Edge専用処理
                else if (browser.IsEdge && !requiresDirectLaunch)
                {
                    shouldTerminate = LaunchEdge(browser, url ?? "", terminate);
                }
                // 一般的なブラウザ処理
                else
                {
                    if (DoLaunch(browser, url ?? "", terminate, forcePrivateMode, profileOverride))
                    {
                        if (terminate)
                        {
                            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                            shouldTerminate = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.LaunchBrowser", "起動エラー", ex.Message, ex.StackTrace ?? "");
                if (!IsTestEnvironment())
                {
                    MessageBox.Show($"ブラウザ {browser.Name ?? "Unknown"} の起動に失敗しました。", "起動エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Logger.LogDebug("BrowserUtilities.LaunchBrowser", "End", browser.Name ?? "Unknown", url ?? "null", terminate);
            return shouldTerminate;
        }

        /// <summary>
        /// IE専用の起動処理
        /// 既存のIEインスタンスにタブを追加します
        /// </summary>
        /// <returns>起動に成功し、かつterminateがtrueだった場合はtrue</returns>
        private static bool LaunchIE(Browser browser, string url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchIE", "Browser is null, skipping IE launch", url ?? "null", terminate);
                return false;
            }

            Logger.LogDebug("BrowserUtilities.LaunchIE", "Start", browser.Name ?? "null", url ?? "null", terminate);

            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                    Logger.LogDebug("BrowserUtilities.LaunchIE", "テスト環境のため、IE起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return false;
                }

                // 一般的な起動処理を使用
                if (DoLaunch(browser, url ?? "", terminate))
                {
                    if (terminate)
                    {
                        Logger.LogDebug("BrowserUtilities.LaunchIE", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // DoLaunch自体が例外を投げた場合にここで再度DoLaunchを呼ぶと、
                // 既に起動済みのブラウザが二重起動する可能性があるため、再試行はしない
                Logger.LogError("BrowserUtilities.LaunchIE", "IE起動エラー", ex.Message, ex.StackTrace ?? "");
                return false;
            }
        }

        /// <summary>
        /// Edge専用の起動処理
        /// microsoft-edge:プロトコルでの起動をサポートします
        /// </summary>
        /// <returns>起動に成功し、かつterminateがtrueだった場合はtrue</returns>
        private static bool LaunchEdge(Browser browser, string? url, bool terminate)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.LaunchEdge", "Browser is null, skipping Edge launch", url ?? "null", terminate);
                return false;
            }

            url ??= string.Empty;

            Logger.LogDebug("BrowserUtilities.LaunchEdge", "Start", browser.Name ?? "null", url, terminate);

            bool shouldTerminate = false;
            try
            {
                // テスト環境では実際のブラウザ起動をスキップ
                if (IsTestEnvironment())
                {
                    Logger.LogDebug("BrowserUtilities.LaunchEdge", "テスト環境のため、Edge起動をスキップしました", browser.Name ?? "null", url ?? "null");
                    return false;
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
                            shouldTerminate = true;
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
                            shouldTerminate = true;
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
            return shouldTerminate;
        }

        /// <summary>
        /// ブラウザ起動時に渡す引数リストを組み立てます。
        ///
        /// 順序は「ユーザー定義の引数（{0}/{1}テンプレート対応）→ プロファイル指定
        /// → シークレット指定 → URL」。URLは常に独立した最後の1引数になるため、
        /// URLに空白や引用符が含まれていても他の引数として解釈されません。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <param name="browserPath">正規化済みの実行ファイルパス</param>
        /// <param name="url">開くURL</param>
        /// <param name="forcePrivateMode">ブラウザ設定に関わらずシークレット起動する場合はtrue</param>
        /// <param name="profileOverride">使用するプロファイル名（nullならブラウザ設定に従う）</param>
        /// <returns>ProcessStartInfo.ArgumentListへ渡す引数リスト</returns>
        internal static List<string> BuildLaunchArguments(
            Browser browser,
            string browserPath,
            string? url,
            bool forcePrivateMode = false,
            string? profileOverride = null)
        {
            var userArguments = browser.Arguments ?? string.Empty;

            // {0}=プロトコル / {1}=プロトコルを除いた残り のテンプレート指定
            // （既存設定との互換のため維持する）。展開後はURLを重複して付けない。
            if (!string.IsNullOrEmpty(url) &&
                (userArguments.Contains("{0}") || userArguments.Contains("{1}")))
            {
                var protocol = string.Empty;
                var remainder = url;
                var protocolIndex = url.IndexOf("://", StringComparison.Ordinal);
                if (protocolIndex > 0)
                {
                    protocol = url.Substring(0, protocolIndex);
                    remainder = url.Substring(protocolIndex + 3);
                }

                var expanded = string.Format(userArguments, protocol, remainder);
                var templateArguments = BrowserLaunchProfiles.SplitUserArguments(expanded).ToList();

                var family = BrowserLaunchProfiles.DetectFamily(browser);
                templateArguments.AddRange(
                    BrowserLaunchProfiles.GetProfileArguments(family, profileOverride ?? browser.ProfileName));
                if (forcePrivateMode || browser.UsePrivateMode)
                {
                    templateArguments.AddRange(BrowserLaunchProfiles.GetPrivateModeArguments(family));
                }

                return templateArguments;
            }

            // Chromeは引数が未指定のとき、既存ウィンドウのタブではなく新規ウィンドウで開く
            var effectiveBrowser = browser;
            var isChrome = browser.Name?.Contains("chrome", StringComparison.OrdinalIgnoreCase) == true ||
                           browserPath.Contains("chrome", StringComparison.OrdinalIgnoreCase);
            if (isChrome && string.IsNullOrWhiteSpace(userArguments))
            {
                effectiveBrowser = browser.Clone();
                effectiveBrowser.Arguments = "--new-window";
            }

            return BrowserLaunchProfiles.BuildArgumentList(effectiveBrowser, url, forcePrivateMode, profileOverride);
        }

        /// <summary>
        /// 一般的なブラウザ起動処理
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="terminate">起動後にアプリケーションを終了するかどうか</param>
        /// <param name="forcePrivateMode">ブラウザ設定に関わらずシークレット起動する場合はtrue</param>
        /// <param name="profileOverride">使用するプロファイル名（nullならブラウザ設定に従う）</param>
        private static bool DoLaunch(Browser browser, string url, bool terminate,
            bool forcePrivateMode = false, string? profileOverride = null)
        {
            // nullチェック
            if (browser == null)
            {
                Logger.LogDebug("BrowserUtilities.DoLaunch", "Browser is null, skipping launch", url ?? "null", terminate);
                return false;
            }

            Logger.LogDebug("BrowserUtilities.DoLaunch", "Start", browser.Name ?? "null", url ?? "null", terminate);

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
                        Logger.LogError("BrowserUtilities.DoLaunch", "Empty target", browser.Name ?? "null", url ?? "null", terminate);
                        return false;
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

                // 引数はProcessStartInfo.ArgumentListへ1要素=1引数で渡す。
                // 従来は文字列連結でコマンドラインを組んでいたため、URLに引用符が
                // 含まれていると後続を別の引数として解釈させられる余地があった。
                var argumentList = BuildLaunchArguments(browser, browserPath, url, forcePrivateMode, profileOverride);

                var startInfo = new ProcessStartInfo(browserPath) { UseShellExecute = false };
                foreach (var argument in argumentList)
                {
                    startInfo.ArgumentList.Add(argument);
                }

                Logger.LogInfo("BrowserUtilities.DoLaunch", "Starting process", browserPath,
                    BrowserLaunchProfiles.FormatForLog(argumentList));
                Process? process = Process.Start(startInfo);

                if (process != null)
                {
                    int processId = process.Id;
                    Logger.LogInfo("BrowserUtilities.DoLaunch", "Process started", processId.ToString());

                    // プロセス起動自体はここまでで成功しているため、以降の「前面に移動」処理で
                    // 例外が発生しても起動失敗として扱わない（起動成否の判定範囲を狭める）
                    try
                    {
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
                            // Chrome/Edge等の単一プロセスモデルでは、起動プロセスが既存インスタンスへ
                            // 委譲して即座に終了するのは正常な挙動のため、エラー扱いにはしない
                            Logger.LogInfo("BrowserUtilities.DoLaunch", "Process exited immediately", processId.ToString());
                            TryToBringToFront(browserPath);
                        }
                    }
                    catch (Exception postLaunchEx)
                    {
                        Logger.LogWarning("BrowserUtilities.DoLaunch", "起動後の前面移動処理でエラーが発生しましたが、起動自体は成功しています", postLaunchEx.Message);
                    }

                    if (terminate)
                    {
                        Logger.LogInfo("BrowserUtilities.DoLaunch", "Terminate", browser.Name ?? "null", url ?? "null", terminate);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserUtilities.DoLaunch", "起動エラー", ex.Message, ex.StackTrace ?? "");
            }

            if (!IsTestEnvironment())
            {
                MessageBox.Show($"ブラウザ {browser.Name ?? "Unknown"} の起動に失敗しました。\nパス: {browser.Target}\n引数: {browser.Arguments}", "起動エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Logger.LogError("BrowserUtilities.DoLaunch", "重大な失敗", browser.Name ?? "Unknown", url ?? "null", terminate);
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
                try
                {
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
                finally
                {
                    foreach (var proc in processes)
                    {
                        proc.Dispose();
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
            return GetBrowserByGUID(guid, Settings.Current?.Browsers ?? new List<Browser>());
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
