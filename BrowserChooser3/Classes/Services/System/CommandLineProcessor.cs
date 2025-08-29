using System;
using System.Collections.Generic;
using System.Linq;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// コマンドライン引数処理を管理するクラス
    /// 詳細なオプション対応と引数解析を提供します
    /// </summary>
    public static class CommandLineProcessor
    {
        /// <summary>
        /// コマンドライン引数の解析結果
        /// </summary>
        public class CommandLineArgs
        {
            /// <summary>処理対象のURL</summary>
            public string? URL { get; set; }
            
            /// <summary>遅延時間（秒）</summary>
            public int Delay { get; set; } = 0;
            
            /// <summary>指定されたブラウザのGUID</summary>
            public Guid? BrowserGuid { get; set; }
            
            /// <summary>URL短縮解除を行うかどうか</summary>
            public bool UnshortenURL { get; set; } = false;
            
            /// <summary>デバッグログを有効にするかどうか</summary>
            public bool DebugLog { get; set; } = false;
            
            /// <summary>DLLを抽出するかどうか</summary>
            public bool ExtractDLLs { get; set; } = false;
            
            /// <summary>ポータブルモードで起動するかどうか</summary>
            public bool PortableMode { get; set; } = false;
            
            /// <summary>設定ファイルを無視するかどうか</summary>
            public bool IgnoreSettings { get; set; } = false;
            
            /// <summary>ヘルプを表示するかどうか</summary>
            public bool ShowHelp { get; set; } = false;
            
            /// <summary>バージョン情報を表示するかどうか</summary>
            public bool ShowVersion { get; set; } = false;
            
            /// <summary>サイレントモードで起動するかどうか</summary>
            public bool SilentMode { get; set; } = false;
            
            /// <summary>自動起動するかどうか</summary>
            public bool AutoLaunch { get; set; } = false;
        }

        /// <summary>
        /// コマンドライン引数を解析します
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>解析結果</returns>
        public static CommandLineArgs ParseArguments(string[] args)
        {
            // nullチェック
            if (args == null)
            {
                Logger.LogDebug("CommandLineProcessor.ParseArguments", "コマンドライン引数がnull");
                return new CommandLineArgs();
            }

            Logger.LogDebug("CommandLineProcessor.ParseArguments", "コマンドライン引数解析開始", string.Join(" ", args));

            var result = new CommandLineArgs();

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    
                    switch (arg)
                    {
                        case "-h":
                        case "--help":
                        case "/?":
                            result.ShowHelp = true;
                            break;

                        case "-v":
                        case "--version":
                            result.ShowVersion = true;
                            break;

                        case "-d":
                        case "--delay":
                            if (i + 1 < args.Length && int.TryParse(args[i + 1], out var delay))
                            {
                                result.Delay = delay;
                                i++; // 次の引数をスキップ
                            }
                            break;

                        case "-b":
                        case "--browser":
                            if (i + 1 < args.Length && Guid.TryParse(args[i + 1], out var browserGuid))
                            {
                                result.BrowserGuid = browserGuid;
                                i++; // 次の引数をスキップ
                            }
                            break;

                        case "-u":
                        case "--unshorten":
                            result.UnshortenURL = true;
                            break;

                        case "--debug":
                            result.DebugLog = true;
                            break;

                        case "--extract-dlls":
                            result.ExtractDLLs = true;
                            break;

                        case "--portable":
                            result.PortableMode = true;
                            break;

                        case "--ignore-settings":
                            result.IgnoreSettings = true;
                            break;

                        case "--silent":
                            result.SilentMode = true;
                            break;

                        case "--auto-launch":
                            result.AutoLaunch = true;
                            break;

                        default:
                            // URLとして扱う（最初の非オプション引数）
                            if (!arg.StartsWith("-") && !arg.StartsWith("/") && result.URL == null)
                            {
                                // 長いURLの場合の処理
                                var url = args[i];
                                
                                // URLの長さ制限チェック（Windowsのコマンドライン制限を考慮）
                                if (url.Length > 8191) // Windowsのコマンドライン制限
                                {
                                    Logger.LogWarning("CommandLineProcessor.ParseArguments", "URLが長すぎます", url.Length);
                                    // 長すぎる場合は切り詰めるか、エラーとして扱う
                                    url = url.Substring(0, 8191);
                                }
                                
                                // URLエンコーディングの問題を修正
                                try
                                {
                                    // 必要に応じてURLデコード
                                    if (url.Contains("%"))
                                    {
                                        url = Uri.UnescapeDataString(url);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogWarning("CommandLineProcessor.ParseArguments", "URLデコードエラー", ex.Message);
                                    // デコードに失敗した場合は元のURLを使用
                                }
                                
                                result.URL = url; // 処理済みのURLを設定
                            }
                            break;
                    }
                }

                Logger.LogDebug("CommandLineProcessor.ParseArguments", "コマンドライン引数解析完了", 
                    $"URL: {result.URL}, Delay: {result.Delay}, Browser: {result.BrowserGuid}");
            }
            catch (Exception ex)
            {
                Logger.LogError("CommandLineProcessor.ParseArguments", "コマンドライン引数解析エラー", ex.Message);
            }

            return result;
        }

        /// <summary>
        /// ヘルプメッセージを取得します
        /// </summary>
        /// <returns>ヘルプメッセージ</returns>
        public static string GetHelpMessage()
        {
            return @"Browser Chooser 3 - ブラウザ選択ツール

使用方法:
  BrowserChooser3.exe [URL] [オプション]

引数:
  URL                   処理対象のURLまたはファイルパス

オプション:
  -h, --help            このヘルプメッセージを表示
  -v, --version         バージョン情報を表示
  -d, --delay <秒>      指定した秒数後にブラウザを起動
  -b, --browser <GUID>  指定したブラウザのGUIDで起動
  -u, --unshorten       URL短縮解除を実行
  --debug               デバッグログを有効化
  --extract-dlls        DLLファイルを抽出
  --portable            ポータブルモードで起動
  --ignore-settings     設定ファイルを無視
  --silent              サイレントモードで起動
  --auto-launch         自動起動モード

例:
  BrowserChooser3.exe https://example.com
  BrowserChooser3.exe -d 5 https://example.com
  BrowserChooser3.exe -b 12345678-1234-1234-1234-123456789012 file.html
  BrowserChooser3.exe --portable --debug";
        }

        /// <summary>
        /// バージョン情報を取得します
        /// </summary>
        /// <returns>バージョン情報</returns>
        public static string GetVersionInfo()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return $"Browser Chooser 3 Version {version?.ToString() ?? "Unknown"}";
        }

        /// <summary>
        /// 引数が有効かどうかを検証します
        /// </summary>
        /// <param name="args">解析された引数</param>
        /// <returns>有効な場合はtrue</returns>
        public static bool ValidateArguments(CommandLineArgs args)
        {
            try
            {
                // ヘルプまたはバージョン表示の場合は常に有効
                if (args.ShowHelp || args.ShowVersion)
                    return true;

                // URLが指定されている場合は有効
                if (!string.IsNullOrEmpty(args.URL))
                    return true;

                // サイレントモードまたは自動起動モードの場合は有効
                if (args.SilentMode || args.AutoLaunch)
                    return true;

                // その他の場合は無効
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("CommandLineProcessor.ValidateArguments", "引数検証エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 環境変数からコマンドラインオプションを読み込みます
        /// </summary>
        /// <param name="args">既存の引数</param>
        /// <returns>環境変数で更新された引数</returns>
        public static CommandLineArgs LoadFromEnvironment(CommandLineArgs args)
        {
            try
            {
                // 環境変数からオプションを読み込み
                var debugLog = Environment.GetEnvironmentVariable("BROWSERCHOOSER_DEBUG");
                if (bool.TryParse(debugLog, out var debug) && debug)
                {
                    args.DebugLog = true;
                }

                var extractDlls = Environment.GetEnvironmentVariable("BROWSERCHOOSER_EXTRACT_DLLS");
                if (bool.TryParse(extractDlls, out var extract) && extract)
                {
                    args.ExtractDLLs = true;
                }

                var portableMode = Environment.GetEnvironmentVariable("BROWSERCHOOSER_PORTABLE");
                if (bool.TryParse(portableMode, out var portable) && portable)
                {
                    args.PortableMode = true;
                }

                var ignoreSettings = Environment.GetEnvironmentVariable("BROWSERCHOOSER_IGNORE_SETTINGS");
                if (bool.TryParse(ignoreSettings, out var ignore) && ignore)
                {
                    args.IgnoreSettings = true;
                }

                Logger.LogDebug("CommandLineProcessor.LoadFromEnvironment", "環境変数からオプションを読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("CommandLineProcessor.LoadFromEnvironment", "環境変数読み込みエラー", ex.Message);
            }

            return args;
        }
    }
}

