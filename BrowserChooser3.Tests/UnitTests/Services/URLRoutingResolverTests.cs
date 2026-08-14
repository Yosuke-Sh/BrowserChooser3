using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.BrowserServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Phase 3-8（URLルーティングのプレビュー）の振る舞いテスト。
    ///
    /// URLRoutingResolver は Options の URL テスト欄と MainForm の実際の起動処理の
    /// 両方が使うため、ここでの判定が実挙動そのものになる。
    /// 「プレビューでは選択画面が出ると表示されたのに実際は自動起動した」という
    /// 食い違いが起きないよう、実装を1つに保っていることが前提。
    /// </summary>
    public class URLRoutingResolverTests
    {
        private static readonly Guid ChromeGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid FirefoxGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");

        private static Settings BuildSettings(
            IEnumerable<URL>? urls = null,
            IEnumerable<Protocol>? protocols = null,
            int defaultDelay = 5)
        {
            var settings = new Settings
            {
                DefaultDelay = defaultDelay,
                Browsers = new List<Browser>
                {
                    new() { Guid = ChromeGuid, Name = "Chrome", Target = @"C:\chrome.exe" },
                    new() { Guid = FirefoxGuid, Name = "Firefox", Target = @"C:\firefox.exe" }
                },
                URLs = urls?.ToList() ?? new List<URL>(),
                Protocols = protocols?.ToList() ?? new List<Protocol>()
            };
            return settings;
        }

        #region 誤爆の回帰（最重要）

        [Fact]
        public void Resolve_WithPatternAppearingOnlyInQueryString_ShouldNotAutoLaunch()
        {
            // Phase 1-1 で修正した誤爆の回帰テスト。
            // 双方向 Contains だった頃は、このURLが github.com のルールにマッチして
            // ユーザーの意図しないブラウザが自動起動していた。
            var settings = BuildSettings(new[]
            {
                new URL { Name = "GitHub", URLPattern = "github.com", BrowserGuid = FirefoxGuid }
            });

            var result = URLRoutingResolver.Resolve(settings, "https://evil.com/?q=github.com");

            result.Kind.Should().Be(URLRoutingKind.NoMatch);
            result.Browser.Should().BeNull();
            URLRoutingResolver.DescribeResult(result).Should().Contain("ブラウザ選択画面");
        }

        [Fact]
        public void Resolve_WithGenuineSubdomain_ShouldStillAutoLaunch()
        {
            // 誤爆を止めても、意図通りに動いていたケースは維持されなければならない。
            var settings = BuildSettings(new[]
            {
                new URL { Name = "GitHub", URLPattern = "github.com", BrowserGuid = FirefoxGuid }
            });

            var result = URLRoutingResolver.Resolve(settings, "https://gist.github.com/x");

            result.Kind.Should().Be(URLRoutingKind.AutoUrl);
            result.Browser!.Name.Should().Be("Firefox");
        }

        #endregion

        #region AutoURL

        [Fact]
        public void Resolve_WithNoRules_ShouldReturnNoMatch()
        {
            URLRoutingResolver.Resolve(BuildSettings(), "https://example.com/")
                .Kind.Should().Be(URLRoutingKind.NoMatch);
        }

        [Fact]
        public void Resolve_ShouldUseFirstMatchingRuleInOrder()
        {
            // ルールは並び順に評価される（Options の Move Up/Down が意味を持つ）
            var settings = BuildSettings(new[]
            {
                new URL { Name = "First", URLPattern = "example.com", BrowserGuid = ChromeGuid },
                new URL { Name = "Second", URLPattern = "example.com", BrowserGuid = FirefoxGuid }
            });

            var result = URLRoutingResolver.Resolve(settings, "https://example.com/");

            result.RuleName.Should().Be("First");
            result.Browser!.Name.Should().Be("Chrome");
        }

        [Fact]
        public void Resolve_ShouldSkipInactiveRules()
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "Disabled", URLPattern = "example.com", BrowserGuid = ChromeGuid, IsActive = false },
                new URL { Name = "Enabled", URLPattern = "example.com", BrowserGuid = FirefoxGuid }
            });

            URLRoutingResolver.Resolve(settings, "https://example.com/")
                .RuleName.Should().Be("Enabled");
        }

        [Fact]
        public void Resolve_ShouldSkipRulesWithEmptyPattern()
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "Empty", URLPattern = "", BrowserGuid = ChromeGuid },
                new URL { Name = "Real", URLPattern = "example.com", BrowserGuid = FirefoxGuid }
            });

            URLRoutingResolver.Resolve(settings, "https://example.com/")
                .RuleName.Should().Be("Real");
        }

        [Fact]
        public void Resolve_WithNegativeDelay_ShouldUseDefaultDelay()
        {
            var settings = BuildSettings(
                new[] { new URL { Name = "R", URLPattern = "example.com", BrowserGuid = ChromeGuid, Delay = -1 } },
                defaultDelay: 7);

            URLRoutingResolver.Resolve(settings, "https://example.com/")
                .DelaySeconds.Should().Be(7);
        }

        [Fact]
        public void Resolve_WithExplicitDelay_ShouldUseIt()
        {
            var settings = BuildSettings(
                new[] { new URL { Name = "R", URLPattern = "example.com", BrowserGuid = ChromeGuid, Delay = 3 } },
                defaultDelay: 7);

            URLRoutingResolver.Resolve(settings, "https://example.com/")
                .DelaySeconds.Should().Be(3);
        }

        [Fact]
        public void Resolve_WithMissingBrowser_ShouldReportMatchedButBrowserMissing()
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "Orphan", URLPattern = "example.com", BrowserGuid = Guid.NewGuid() }
            });

            var result = URLRoutingResolver.Resolve(settings, "https://example.com/");

            result.Kind.Should().Be(URLRoutingKind.MatchedButBrowserMissing);
            result.Browser.Should().BeNull();
            URLRoutingResolver.DescribeResult(result).Should().Contain("ブラウザが設定に存在しません");
        }

        #endregion

        #region プロトコル

        [Fact]
        public void Resolve_WithProtocolRule_ShouldMatchWhenNoAutoUrlMatches()
        {
            var settings = BuildSettings(
                urls: Array.Empty<URL>(),
                protocols: new[] { new Protocol { Name = "FTP", Header = "ftp", BrowserGuid = FirefoxGuid } });

            var result = URLRoutingResolver.Resolve(settings, "ftp://files.example.com/x");

            result.Kind.Should().Be(URLRoutingKind.Protocol);
            result.Browser!.Name.Should().Be("Firefox");
            // プロトコル経路は遅延なしで即座に起動する
            result.DelaySeconds.Should().Be(0);
        }

        [Fact]
        public void Resolve_ShouldPreferAutoUrlOverProtocol()
        {
            // 優先順位は AutoURLs > Protocol（MainForm の実処理と同じ）
            var settings = BuildSettings(
                urls: new[] { new URL { Name = "Auto", URLPattern = "example.com", BrowserGuid = ChromeGuid } },
                protocols: new[] { new Protocol { Name = "HTTPS", Header = "https", BrowserGuid = FirefoxGuid } });

            var result = URLRoutingResolver.Resolve(settings, "https://example.com/");

            result.Kind.Should().Be(URLRoutingKind.AutoUrl);
            result.Browser!.Name.Should().Be("Chrome");
        }

        [Fact]
        public void Resolve_ShouldSkipInactiveProtocols()
        {
            var settings = BuildSettings(
                urls: Array.Empty<URL>(),
                protocols: new[]
                {
                    new Protocol { Name = "FTP", Header = "ftp", BrowserGuid = ChromeGuid, IsActive = false }
                });

            URLRoutingResolver.Resolve(settings, "ftp://files.example.com/x")
                .Kind.Should().Be(URLRoutingKind.NoMatch);
        }

        [Fact]
        public void Resolve_ProtocolMatching_ShouldBeCaseInsensitive()
        {
            var settings = BuildSettings(
                urls: Array.Empty<URL>(),
                protocols: new[] { new Protocol { Name = "HTTPS", Header = "https", BrowserGuid = ChromeGuid } });

            URLRoutingResolver.Resolve(settings, "HTTPS://example.com/")
                .Kind.Should().Be(URLRoutingKind.Protocol);
        }

        #endregion

        #region プロトコル抽出

        [Theory]
        [InlineData("https://example.com/", "https")]
        [InlineData("ftp://files.example.com/", "ftp")]
        [InlineData("mailto:someone@example.com", "mailto")]
        [InlineData("example.com", "")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void ExtractProtocol_ShouldHandleCommonForms(string? url, string expected)
        {
            URLRoutingResolver.ExtractProtocol(url).Should().Be(expected);
        }

        #endregion

        #region 異常系

        [Fact]
        public void Resolve_WithNullSettings_ShouldReturnNoMatch()
        {
            URLRoutingResolver.Resolve(null, "https://example.com/")
                .Kind.Should().Be(URLRoutingKind.NoMatch);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Resolve_WithBlankUrl_ShouldReturnNoMatch(string? url)
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "R", URLPattern = "example.com", BrowserGuid = ChromeGuid }
            });

            URLRoutingResolver.Resolve(settings, url).Kind.Should().Be(URLRoutingKind.NoMatch);
        }

        #endregion

        #region 説明文

        [Fact]
        public void DescribeResult_WithDelay_ShouldMentionSecondsAndBrowser()
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "GitHub", URLPattern = "github.com", BrowserGuid = FirefoxGuid, Delay = 4 }
            });

            var description = URLRoutingResolver.DescribeResult(
                URLRoutingResolver.Resolve(settings, "https://github.com/x"));

            description.Should().Contain("4秒後");
            description.Should().Contain("Firefox");
            description.Should().Contain("github.com");
        }

        [Fact]
        public void DescribeResult_WithUnnamedRule_ShouldNotShowEmptyName()
        {
            var settings = BuildSettings(new[]
            {
                new URL { Name = "", URLPattern = "example.com", BrowserGuid = ChromeGuid }
            });

            URLRoutingResolver.DescribeResult(URLRoutingResolver.Resolve(settings, "https://example.com/"))
                .Should().Contain("(名前なし)");
        }

        #endregion
    }
}
