using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Phase 3-9（トラッキングパラメータ除去とURL正規化）の振る舞いテスト。
    ///
    /// この機能はブラウザへ渡すURLを書き換えるため、
    /// 「消しすぎない」「URLを壊さない」ことが正しく消せることと同じくらい重要。
    /// </summary>
    public class URLSanitizerTests
    {
        private static readonly string[] DefaultParameters = Settings.DefaultTrackingParameters;

        #region トラッキングパラメータの除去

        [Fact]
        public void RemoveTrackingParameters_ShouldRemoveUtmParametersByPrefix()
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/page?utm_source=news&utm_medium=email&id=42",
                DefaultParameters);

            result.Should().Be("https://example.com/page?id=42");
        }

        [Theory]
        [InlineData("fbclid")]
        [InlineData("gclid")]
        [InlineData("msclkid")]
        public void RemoveTrackingParameters_ShouldRemoveKnownExactNames(string parameter)
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                $"https://example.com/?{parameter}=abc123&keep=1",
                DefaultParameters);

            result.Should().Be("https://example.com/?keep=1");
        }

        [Fact]
        public void RemoveTrackingParameters_WhenAllParametersRemoved_ShouldDropQuestionMark()
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/page?utm_source=news",
                DefaultParameters);

            result.Should().Be("https://example.com/page");
        }

        [Fact]
        public void RemoveTrackingParameters_ShouldPreserveFragment()
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/doc?utm_source=x&section=2#heading",
                DefaultParameters);

            result.Should().Be("https://example.com/doc?section=2#heading");
        }

        [Fact]
        public void RemoveTrackingParameters_WithFragmentContainingQuestionMark_ShouldNotTouchFragment()
        {
            // フラグメントはクエリではないので中身を触らない
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/app#/route?utm_source=x",
                DefaultParameters);

            result.Should().Be("https://example.com/app#/route?utm_source=x");
        }

        [Fact]
        public void RemoveTrackingParameters_WithNoQueryString_ShouldReturnUrlUnchanged()
        {
            const string url = "https://example.com/page";

            URLSanitizer.RemoveTrackingParameters(url, DefaultParameters).Should().Be(url);
        }

        [Fact]
        public void RemoveTrackingParameters_ShouldNotRemoveSimilarlyNamedParameters()
        {
            // 消しすぎないこと。"gclid" の完全一致指定が "mygclid" を消してはいけない。
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/?mygclid=1&fbclid_extra=2",
                DefaultParameters);

            result.Should().Be("https://example.com/?mygclid=1&fbclid_extra=2");
        }

        [Fact]
        public void RemoveTrackingParameters_ShouldBeCaseInsensitive()
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/?UTM_Source=x&FBCLID=y&keep=1",
                DefaultParameters);

            result.Should().Be("https://example.com/?keep=1");
        }

        [Fact]
        public void RemoveTrackingParameters_WithBareAsteriskPattern_ShouldNotRemoveEverything()
        {
            // "*" 単体で全パラメータが消えるのは事故なので、前方一致には接頭辞を必須とする
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/?a=1&b=2",
                new[] { "*" });

            result.Should().Be("https://example.com/?a=1&b=2");
        }

        [Fact]
        public void RemoveTrackingParameters_WithEmptyPatternList_ShouldReturnUrlUnchanged()
        {
            const string url = "https://example.com/?utm_source=x";

            URLSanitizer.RemoveTrackingParameters(url, Array.Empty<string>()).Should().Be(url);
            URLSanitizer.RemoveTrackingParameters(url, null).Should().Be(url);
        }

        [Fact]
        public void RemoveTrackingParameters_ShouldKeepValuelessParameters()
        {
            var result = URLSanitizer.RemoveTrackingParameters(
                "https://example.com/?debug&utm_source=x",
                DefaultParameters);

            result.Should().Be("https://example.com/?debug");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void RemoveTrackingParameters_WithBlankUrl_ShouldReturnItUnchanged(string url)
        {
            URLSanitizer.RemoveTrackingParameters(url, DefaultParameters).Should().Be(url);
        }

        #endregion

        #region Sanitize（設定との組み合わせ）

        [Fact]
        public void Sanitize_WithFeatureDisabled_ShouldReturnUrlUnchanged()
        {
            // 既定はOFF。既存ユーザーのURLが勝手に書き換わらないこと。
            var settings = new Settings { RemoveTrackingParameters = false };
            const string url = "https://example.com/?utm_source=x";

            URLSanitizer.Sanitize(url, settings).Should().Be(url);
        }

        [Fact]
        public void Sanitize_WithFeatureEnabled_ShouldRemoveTrackingParameters()
        {
            var settings = new Settings { RemoveTrackingParameters = true };

            URLSanitizer.Sanitize("https://example.com/?utm_source=x&id=1", settings)
                .Should().Be("https://example.com/?id=1");
        }

        [Fact]
        public void Sanitize_WithNullSettings_ShouldReturnUrlUnchanged()
        {
            const string url = "https://example.com/?utm_source=x";

            URLSanitizer.Sanitize(url, null).Should().Be(url);
        }

        [Fact]
        public void Sanitize_WithUserEditedParameterList_ShouldHonourIt()
        {
            var settings = new Settings
            {
                RemoveTrackingParameters = true,
                TrackingParameters = new List<string> { "ref" }
            };

            // ユーザーがリストを絞り込んだ場合、既定の utm_ は消えない
            URLSanitizer.Sanitize("https://example.com/?ref=a&utm_source=b", settings)
                .Should().Be("https://example.com/?utm_source=b");
        }

        #endregion

        #region 既定リスト

        [Fact]
        public void DefaultTrackingParameters_ShouldIncludeCommonTrackers()
        {
            Settings.DefaultTrackingParameters.Should().Contain(new[] { "utm_*", "fbclid", "gclid", "msclkid" });
        }

        [Fact]
        public void NewSettings_ShouldDefaultToDisabledWithPopulatedList()
        {
            var settings = new Settings();

            settings.RemoveTrackingParameters.Should().BeFalse();
            settings.TrackingParameters.Should().NotBeEmpty();
        }

        [Fact]
        public void Settings_TrackingParameterListIsIndependentPerInstance()
        {
            // 既定リストの配列を共有していると、片方の編集が他方へ波及する
            var first = new Settings();
            var second = new Settings();

            first.TrackingParameters.Add("custom_marker");

            second.TrackingParameters.Should().NotContain("custom_marker");
        }

        #endregion

        #region 正規化

        [Fact]
        public void Canonicalize_WithAppendedText_ShouldAppendIt()
        {
            URLSanitizer.Canonicalize("https://example.com/", "#top")
                .Should().EndWith("#top");
        }

        [Fact]
        public void Canonicalize_WithMalformedUrl_ShouldReturnItUnchanged()
        {
            // 解析に失敗してもURLを壊さない
            const string url = "not a url at all";

            URLSanitizer.Canonicalize(url, null).Should().Be(url);
        }

        #endregion
    }
}
