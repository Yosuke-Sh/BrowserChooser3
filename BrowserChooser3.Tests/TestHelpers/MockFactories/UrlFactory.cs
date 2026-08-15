using System;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// テスト用の <see cref="URL"/>（AutoURLルール）を組み立てるファクトリ。
    /// </summary>
    public static class UrlFactory
    {
        /// <summary>
        /// AutoURLルールを作成します。
        /// </summary>
        /// <param name="pattern">URLパターン</param>
        /// <param name="browserGuid">振り分け先ブラウザのGUID</param>
        /// <param name="name">ルール名（省略時はパターンをそのまま使用）</param>
        /// <returns>作成したルール</returns>
        public static URL Create(string pattern, Guid browserGuid, string? name = null)
        {
            return new URL
            {
                Guid = Guid.NewGuid(),
                Name = name ?? pattern,
                URLPattern = pattern,
                BrowserGuid = browserGuid,
                IsActive = true
            };
        }

        /// <summary>
        /// 起動後にアプリを自動終了するAutoURLルールを作成します。
        /// </summary>
        /// <param name="pattern">URLパターン</param>
        /// <param name="browserGuid">振り分け先ブラウザのGUID</param>
        /// <returns>作成したルール</returns>
        public static URL CreateAutoClosing(string pattern, Guid browserGuid)
        {
            var url = Create(pattern, browserGuid);
            url.AutoClose = true;
            return url;
        }

        /// <summary>
        /// 無効化されたAutoURLルールを作成します。
        /// マッチしても振り分けが行われないことの検証に使用します。
        /// </summary>
        /// <param name="pattern">URLパターン</param>
        /// <param name="browserGuid">振り分け先ブラウザのGUID</param>
        /// <returns>作成したルール</returns>
        public static URL CreateInactive(string pattern, Guid browserGuid)
        {
            var url = Create(pattern, browserGuid);
            url.IsActive = false;
            return url;
        }
    }
}
