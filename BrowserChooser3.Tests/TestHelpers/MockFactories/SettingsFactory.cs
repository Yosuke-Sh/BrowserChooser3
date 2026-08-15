using System;
using System.Collections.Generic;
using System.Linq;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// テスト用の <see cref="Settings"/> を組み立てるファクトリ。
    /// </summary>
    public static class SettingsFactory
    {
        /// <summary>
        /// ブラウザもルールも持たない、空の設定を作成します。
        /// </summary>
        /// <returns>作成した設定</returns>
        public static Settings CreateEmpty() => new();

        /// <summary>
        /// 指定のブラウザ群を持つ設定を作成します。
        /// </summary>
        /// <param name="browsers">登録するブラウザ</param>
        /// <returns>作成した設定</returns>
        public static Settings WithBrowsers(params Browser[] browsers)
        {
            var settings = new Settings();
            settings.Browsers.AddRange(browsers);
            return settings;
        }

        /// <summary>
        /// AutoURLルールと、その振り分け先ブラウザを持つ設定を作成します。
        /// </summary>
        /// <param name="browsers">登録するブラウザ</param>
        /// <param name="urls">登録するAutoURLルール</param>
        /// <returns>作成した設定</returns>
        public static Settings WithRouting(IEnumerable<Browser> browsers, IEnumerable<URL> urls)
        {
            var settings = new Settings();
            settings.Browsers.AddRange(browsers);
            settings.URLs.AddRange(urls);
            return settings;
        }

        /// <summary>
        /// 「パターン → ブラウザ」1件だけのルーティング設定を作成します。
        /// URLマッチングの回帰テストで最も多用する形。
        /// </summary>
        /// <param name="pattern">URLパターン</param>
        /// <param name="browserName">振り分け先ブラウザ名</param>
        /// <returns>作成した設定</returns>
        public static Settings WithSingleRoute(string pattern, string browserName = "Routed Browser")
        {
            var browser = BrowserFactory.Create(browserName);
            var url = UrlFactory.Create(pattern, browser.Guid);
            return WithRouting(new[] { browser }, new[] { url });
        }

        /// <summary>
        /// 既定ブラウザを指定した設定を作成します。
        /// </summary>
        /// <param name="browsers">登録するブラウザ</param>
        /// <param name="defaultBrowserName">既定として扱うブラウザ名</param>
        /// <returns>作成した設定</returns>
        public static Settings WithDefaultBrowser(IEnumerable<Browser> browsers, string defaultBrowserName)
        {
            var settings = new Settings();
            settings.Browsers.AddRange(browsers);

            var target = settings.Browsers.FirstOrDefault(b =>
                string.Equals(b.Name, defaultBrowserName, StringComparison.OrdinalIgnoreCase));

            if (target != null)
            {
                target.IsDefault = true;
                settings.DefaultBrowserGuid = target.Guid;
            }

            return settings;
        }
    }
}
