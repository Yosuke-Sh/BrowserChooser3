using System;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// テスト用の <see cref="Browser"/> を組み立てるファクトリ。
    /// </summary>
    /// <remarks>
    /// 各テストファイルが arrange を自前で書き直していたため、
    /// 「テストが何を検証したいのか」がセットアップに埋もれていた。
    /// ここに集約し、テスト側では検証対象のプロパティだけを上書きする。
    /// </remarks>
    public static class BrowserFactory
    {
        /// <summary>
        /// 起動可能な標準的なブラウザを作成します。
        /// </summary>
        /// <param name="name">ブラウザ名</param>
        /// <param name="target">実行ファイルのパス</param>
        /// <param name="guid">GUID（省略時は新規採番）</param>
        /// <returns>作成したブラウザ</returns>
        public static Browser Create(
            string name = "Test Browser",
            string target = @"C:\Program Files\TestBrowser\browser.exe",
            Guid? guid = null)
        {
            return new Browser
            {
                Guid = guid ?? Guid.NewGuid(),
                Name = name,
                Target = target,
                Visible = true,
                IsActive = true
            };
        }

        /// <summary>
        /// Chromium系（Chrome/Edge）のブラウザを作成します。
        /// プロファイル指定・シークレット起動の引数解決テストで使用します。
        /// </summary>
        /// <param name="name">ブラウザ名</param>
        /// <param name="profileName">プロファイル名</param>
        /// <param name="usePrivateMode">シークレットモードで起動するか</param>
        /// <returns>作成したブラウザ</returns>
        public static Browser CreateChromium(
            string name = "Google Chrome",
            string profileName = "",
            bool usePrivateMode = false)
        {
            return new Browser
            {
                Guid = Guid.NewGuid(),
                Name = name,
                Target = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Category = "Chrome",
                ProfileName = profileName,
                UsePrivateMode = usePrivateMode,
                Visible = true,
                IsActive = true
            };
        }

        /// <summary>
        /// Firefoxを作成します。
        /// </summary>
        /// <param name="profileName">プロファイル名</param>
        /// <param name="usePrivateMode">プライベートウィンドウで起動するか</param>
        /// <returns>作成したブラウザ</returns>
        public static Browser CreateFirefox(string profileName = "", bool usePrivateMode = false)
        {
            return new Browser
            {
                Guid = Guid.NewGuid(),
                Name = "Mozilla Firefox",
                Target = @"C:\Program Files\Mozilla Firefox\firefox.exe",
                Category = "Firefox",
                ProfileName = profileName,
                UsePrivateMode = usePrivateMode,
                Visible = true,
                IsActive = true
            };
        }

        /// <summary>
        /// ホットキー付きのブラウザを作成します。
        /// </summary>
        /// <param name="hotkey">割り当てるホットキー文字</param>
        /// <param name="name">ブラウザ名</param>
        /// <returns>作成したブラウザ</returns>
        public static Browser CreateWithHotkey(char hotkey, string name = "Hotkey Browser")
        {
            var browser = Create(name);
            browser.Hotkey = hotkey;
            return browser;
        }
    }
}
