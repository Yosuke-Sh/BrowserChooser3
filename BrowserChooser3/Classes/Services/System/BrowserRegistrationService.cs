using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// ブラウザアプリとしての登録を管理するサービス
    /// Windowsにアプリケーションをブラウザアプリとして認識させるための機能を提供します
    /// </summary>
    public static class BrowserRegistrationService
    {
        private const string BrowserClientKey = @"SOFTWARE\Clients\StartMenuInternet\BrowserChooser3";
        private const string HttpProtocolKey = @"http\shell\open\command";
        private const string HttpsProtocolKey = @"https\shell\open\command";
        private const string UrlFileAssociationKey = @"InternetShortcut\shell\open\command";

        /// <summary>
        /// ブラウザアプリとしての登録状況を確認します
        /// </summary>
        /// <returns>正しく登録されている場合はtrue</returns>
        public static bool IsRegisteredAsBrowser()
        {
            Logger.LogDebug("BrowserRegistrationService.IsRegisteredAsBrowser", "ブラウザ登録状況確認開始");

            try
            {
                // ブラウザクライアントキーの確認
                var isClientRegistered = CheckBrowserClientRegistration();
                Logger.LogDebug("BrowserRegistrationService.IsRegisteredAsBrowser", "ブラウザクライアント登録状況", isClientRegistered);

                // プロトコルハンドラーの確認
                var isHttpRegistered = CheckProtocolHandlerRegistration("http");
                var isHttpsRegistered = CheckProtocolHandlerRegistration("https");
                Logger.LogDebug("BrowserRegistrationService.IsRegisteredAsBrowser", "プロトコルハンドラー登録状況", $"HTTP: {isHttpRegistered}, HTTPS: {isHttpsRegistered}");

                // URLファイル関連付けの確認
                var isUrlFileRegistered = CheckUrlFileAssociation();
                Logger.LogDebug("BrowserRegistrationService.IsRegisteredAsBrowser", "URLファイル関連付け状況", isUrlFileRegistered);

                var result = isClientRegistered && isHttpRegistered && isHttpsRegistered && isUrlFileRegistered;
                Logger.LogInfo("BrowserRegistrationService.IsRegisteredAsBrowser", "ブラウザ登録状況確認完了", result);
                
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.IsRegisteredAsBrowser", "ブラウザ登録状況確認エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ブラウザアプリとして登録します
        /// </summary>
        /// <returns>登録が成功した場合はtrue</returns>
        public static bool RegisterAsBrowser()
        {
            Logger.LogInfo("BrowserRegistrationService.RegisterAsBrowser", "ブラウザ登録開始");

            try
            {
                var appPath = GetApplicationPath();
                if (string.IsNullOrEmpty(appPath))
                {
                    Logger.LogError("BrowserRegistrationService.RegisterAsBrowser", "アプリケーションパスが取得できませんでした");
                    return false;
                }

                // ブラウザクライアントの登録
                var clientResult = RegisterBrowserClient(appPath);
                Logger.LogDebug("BrowserRegistrationService.RegisterAsBrowser", "ブラウザクライアント登録結果", clientResult);

                // プロトコルハンドラーの登録
                var httpResult = RegisterProtocolHandler("http", appPath);
                var httpsResult = RegisterProtocolHandler("https", appPath);
                Logger.LogDebug("BrowserRegistrationService.RegisterAsBrowser", "プロトコルハンドラー登録結果", $"HTTP: {httpResult}, HTTPS: {httpsResult}");

                // URLファイル関連付けの登録
                var urlFileResult = RegisterUrlFileAssociation(appPath);
                Logger.LogDebug("BrowserRegistrationService.RegisterAsBrowser", "URLファイル関連付け登録結果", urlFileResult);

                var result = clientResult && httpResult && httpsResult && urlFileResult;
                Logger.LogInfo("BrowserRegistrationService.RegisterAsBrowser", "ブラウザ登録完了", result);
                
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.RegisterAsBrowser", "ブラウザ登録エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ブラウザクライアントの登録状況を確認します
        /// </summary>
        /// <returns>登録されている場合はtrue</returns>
        private static bool CheckBrowserClientRegistration()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(BrowserClientKey);
                if (key == null) return false;

                var name = key.GetValue("") as string;
                var iconPath = key.OpenSubKey("DefaultIcon")?.GetValue("") as string;
                var command = key.OpenSubKey("shell\\open\\command")?.GetValue("") as string;

                return !string.IsNullOrEmpty(name) && 
                       !string.IsNullOrEmpty(iconPath) && 
                       !string.IsNullOrEmpty(command);
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.CheckBrowserClientRegistration", "ブラウザクライアント登録確認エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// プロトコルハンドラーの登録状況を確認します
        /// </summary>
        /// <param name="protocol">プロトコル名</param>
        /// <returns>登録されている場合はtrue</returns>
        private static bool CheckProtocolHandlerRegistration(string protocol)
        {
            try
            {
                var keyPath = $"{protocol}\\shell\\open\\command";
                using var key = Registry.ClassesRoot.OpenSubKey(keyPath);
                if (key == null) return false;

                var command = key.GetValue("") as string;
                return !string.IsNullOrEmpty(command) && command.Contains("BrowserChooser3.exe");
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.CheckProtocolHandlerRegistration", $"プロトコル{protocol}登録確認エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// URLファイル関連付けの登録状況を確認します
        /// </summary>
        /// <returns>登録されている場合はtrue</returns>
        private static bool CheckUrlFileAssociation()
        {
            try
            {
                using var key = Registry.ClassesRoot.OpenSubKey(UrlFileAssociationKey);
                if (key == null) return false;

                var command = key.GetValue("") as string;
                return !string.IsNullOrEmpty(command) && command.Contains("BrowserChooser3.exe");
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.CheckUrlFileAssociation", "URLファイル関連付け確認エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ブラウザクライアントを登録します
        /// </summary>
        /// <param name="appPath">アプリケーションパス</param>
        /// <returns>登録が成功した場合はtrue</returns>
        private static bool RegisterBrowserClient(string appPath)
        {
            try
            {
                // メインキーの作成
                using var mainKey = Registry.LocalMachine.CreateSubKey(BrowserClientKey);
                if (mainKey == null) return false;

                mainKey.SetValue("", "Browser Chooser 3");

                // アイコンの設定
                using var iconKey = Registry.LocalMachine.CreateSubKey($"{BrowserClientKey}\\DefaultIcon");
                if (iconKey != null)
                {
                    iconKey.SetValue("", $"{appPath},0");
                }

                // コマンドの設定
                using var commandKey = Registry.LocalMachine.CreateSubKey($"{BrowserClientKey}\\shell\\open\\command");
                if (commandKey != null)
                {
                    commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    commandKey.SetValue("DelegateExecute", "");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.RegisterBrowserClient", "ブラウザクライアント登録エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// プロトコルハンドラーを登録します
        /// </summary>
        /// <param name="protocol">プロトコル名</param>
        /// <param name="appPath">アプリケーションパス</param>
        /// <returns>登録が成功した場合はtrue</returns>
        private static bool RegisterProtocolHandler(string protocol, string appPath)
        {
            try
            {
                var protocolKey = Registry.ClassesRoot.CreateSubKey(protocol);
                if (protocolKey == null) return false;

                protocolKey.SetValue("", $"URL:{protocol.ToUpper()} Protocol");

                var commandKey = Registry.ClassesRoot.CreateSubKey($"{protocol}\\shell\\open\\command");
                if (commandKey != null)
                {
                    commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.RegisterProtocolHandler", $"プロトコル{protocol}登録エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// URLファイル関連付けを登録します
        /// </summary>
        /// <param name="appPath">アプリケーションパス</param>
        /// <returns>登録が成功した場合はtrue</returns>
        private static bool RegisterUrlFileAssociation(string appPath)
        {
            try
            {
                // .urlファイルの関連付け
                var urlKey = Registry.ClassesRoot.CreateSubKey(".url");
                if (urlKey != null)
                {
                    urlKey.SetValue("", "InternetShortcut");
                }

                // InternetShortcutのコマンド設定
                var commandKey = Registry.ClassesRoot.CreateSubKey(UrlFileAssociationKey);
                if (commandKey != null)
                {
                    commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.RegisterUrlFileAssociation", "URLファイル関連付け登録エラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// アプリケーションのパスを取得します
        /// </summary>
        /// <returns>アプリケーションパス</returns>
        private static string GetApplicationPath()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                return process.MainModule?.FileName ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogError("BrowserRegistrationService.GetApplicationPath", "アプリケーションパス取得エラー", ex.Message);
                return string.Empty;
            }
        }
    }
}
