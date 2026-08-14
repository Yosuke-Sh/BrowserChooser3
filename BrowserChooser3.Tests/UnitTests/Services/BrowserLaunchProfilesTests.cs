using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.BrowserServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Phase 3-5（プロファイル/シークレット起動）の振る舞いテスト。
    ///
    /// 特に重要なのは「URLが常に独立した最後の1引数として渡ること」で、
    /// これが崩れるとURL内の引用符でブラウザに任意の引数を渡せてしまう。
    /// </summary>
    public class BrowserLaunchProfilesTests
    {
        private static Browser Chrome() => new()
        {
            Name = "Google Chrome",
            Target = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        };

        private static Browser Edge() => new()
        {
            Name = "Microsoft Edge",
            Target = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            IsEdge = true
        };

        private static Browser Firefox() => new()
        {
            Name = "Mozilla Firefox",
            Target = @"C:\Program Files\Mozilla Firefox\firefox.exe"
        };

        #region 種別判定

        [Fact]
        public void DetectFamily_WithChrome_ShouldBeChromium()
            => BrowserLaunchProfiles.DetectFamily(Chrome()).Should().Be(BrowserFamily.Chromium);

        [Fact]
        public void DetectFamily_WithEdge_ShouldBeEdge()
            => BrowserLaunchProfiles.DetectFamily(Edge()).Should().Be(BrowserFamily.Edge);

        [Fact]
        public void DetectFamily_WithFirefox_ShouldBeFirefox()
            => BrowserLaunchProfiles.DetectFamily(Firefox()).Should().Be(BrowserFamily.Firefox);

        [Fact]
        public void DetectFamily_WithUnknownBrowser_ShouldBeUnknown()
        {
            var browser = new Browser { Name = "Some Browser", Target = @"C:\tools\sb.exe" };

            BrowserLaunchProfiles.DetectFamily(browser).Should().Be(BrowserFamily.Unknown);
        }

        [Fact]
        public void DetectFamily_WithNull_ShouldBeUnknown()
            => BrowserLaunchProfiles.DetectFamily(null).Should().Be(BrowserFamily.Unknown);

        #endregion

        #region プロファイル引数

        [Fact]
        public void GetProfileArguments_ForChromium_ShouldUseProfileDirectory()
        {
            BrowserLaunchProfiles.GetProfileArguments(BrowserFamily.Chromium, "Profile 1")
                .Should().Equal("--profile-directory=Profile 1");
        }

        [Fact]
        public void GetProfileArguments_ForFirefox_ShouldUseSeparateArguments()
        {
            // -P と名前は別々の引数でなければならない
            BrowserLaunchProfiles.GetProfileArguments(BrowserFamily.Firefox, "work")
                .Should().Equal("-P", "work");
        }

        [Fact]
        public void GetProfileArguments_WithEmptyProfile_ShouldBeEmpty()
        {
            BrowserLaunchProfiles.GetProfileArguments(BrowserFamily.Chromium, "").Should().BeEmpty();
            BrowserLaunchProfiles.GetProfileArguments(BrowserFamily.Chromium, null).Should().BeEmpty();
        }

        [Fact]
        public void GetProfileArguments_ForUnknownFamily_ShouldBeEmpty()
        {
            BrowserLaunchProfiles.GetProfileArguments(BrowserFamily.Unknown, "Profile 1").Should().BeEmpty();
        }

        #endregion

        #region シークレット引数

        [Theory]
        [InlineData(BrowserFamily.Chromium, "--incognito")]
        [InlineData(BrowserFamily.Edge, "--inprivate")]
        [InlineData(BrowserFamily.Firefox, "-private-window")]
        public void GetPrivateModeArguments_ShouldMatchBrowserFamily(BrowserFamily family, string expected)
        {
            BrowserLaunchProfiles.GetPrivateModeArguments(family).Should().Equal(expected);
        }

        [Fact]
        public void GetPrivateModeArguments_ForUnknownFamily_ShouldBeEmpty()
        {
            BrowserLaunchProfiles.GetPrivateModeArguments(BrowserFamily.Unknown).Should().BeEmpty();
        }

        #endregion

        #region 引数リストの組み立て（注入防止）

        [Fact]
        public void BuildArgumentList_WithUrlContainingQuotesAndSpaces_ShouldKeepUrlAsSingleArgument()
        {
            // 文字列連結でコマンドラインを組んでいた頃は、この形のURLで
            // 引用符を閉じて任意の引数を後続させることができた。
            const string maliciousUrl = "https://example.com/\" --no-sandbox --load-extension=C:\\evil \"";

            var arguments = BrowserLaunchProfiles.BuildArgumentList(Chrome(), maliciousUrl);

            arguments.Should().HaveCount(1);
            arguments[0].Should().Be(maliciousUrl);
            arguments.Should().NotContain("--no-sandbox");
            arguments.Should().NotContain("--load-extension=C:\\evil");
        }

        [Fact]
        public void BuildArgumentList_ShouldAlwaysPlaceUrlLast()
        {
            var browser = Chrome();
            browser.Arguments = "--new-window";
            browser.ProfileName = "Profile 2";
            browser.UsePrivateMode = true;

            var arguments = BrowserLaunchProfiles.BuildArgumentList(browser, "https://example.com/");

            arguments.Should().Equal(
                "--new-window",
                "--profile-directory=Profile 2",
                "--incognito",
                "https://example.com/");
        }

        [Fact]
        public void BuildArgumentList_WithoutUrl_ShouldNotAppendEmptyArgument()
        {
            var arguments = BrowserLaunchProfiles.BuildArgumentList(Chrome(), "");

            arguments.Should().BeEmpty();
        }

        [Fact]
        public void BuildArgumentList_WithForcePrivateMode_ShouldOverrideBrowserSetting()
        {
            var browser = Firefox();
            browser.UsePrivateMode = false;

            var arguments = BrowserLaunchProfiles.BuildArgumentList(
                browser, "https://example.com/", forcePrivateMode: true);

            arguments.Should().Equal("-private-window", "https://example.com/");
        }

        [Fact]
        public void BuildArgumentList_WithProfileOverride_ShouldIgnoreBrowserProfile()
        {
            var browser = Chrome();
            browser.ProfileName = "Default";

            var arguments = BrowserLaunchProfiles.BuildArgumentList(
                browser, "https://example.com/", profileOverride: "Profile 3");

            arguments.Should().Equal("--profile-directory=Profile 3", "https://example.com/");
        }

        [Fact]
        public void BuildArgumentList_ForUnknownBrowser_ShouldIgnoreProfileAndPrivateMode()
        {
            // 対応していないブラウザに --incognito 等を渡すと起動に失敗しうるため、
            // 指定があっても付けない。
            var browser = new Browser { Name = "Some Browser", Target = @"C:\tools\sb.exe" };
            browser.ProfileName = "work";
            browser.UsePrivateMode = true;

            var arguments = BrowserLaunchProfiles.BuildArgumentList(browser, "https://example.com/");

            arguments.Should().Equal("https://example.com/");
        }

        [Fact]
        public void BuildArgumentList_WithNullBrowser_ShouldThrow()
        {
            var act = () => BrowserLaunchProfiles.BuildArgumentList(null!, "https://example.com/");

            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region ユーザー引数の分割

        [Fact]
        public void SplitUserArguments_ShouldKeepQuotedSegmentsTogether()
        {
            BrowserLaunchProfiles.SplitUserArguments("--window-size=800,600 --user-data-dir=\"C:\\My Data\"")
                .Should().Equal("--window-size=800,600", "--user-data-dir=C:\\My Data");
        }

        [Fact]
        public void SplitUserArguments_WithBlankInput_ShouldBeEmpty()
        {
            BrowserLaunchProfiles.SplitUserArguments("").Should().BeEmpty();
            BrowserLaunchProfiles.SplitUserArguments("   ").Should().BeEmpty();
            BrowserLaunchProfiles.SplitUserArguments(null).Should().BeEmpty();
        }

        [Fact]
        public void SplitUserArguments_WithExtraWhitespace_ShouldNotProduceEmptyArguments()
        {
            BrowserLaunchProfiles.SplitUserArguments("  --a    --b  ")
                .Should().Equal("--a", "--b");
        }

        #endregion

        #region 対応判定

        [Fact]
        public void SupportsProfilesOrPrivateMode_ForKnownBrowsers_ShouldBeTrue()
        {
            BrowserLaunchProfiles.SupportsProfilesOrPrivateMode(Chrome()).Should().BeTrue();
            BrowserLaunchProfiles.SupportsProfilesOrPrivateMode(Edge()).Should().BeTrue();
            BrowserLaunchProfiles.SupportsProfilesOrPrivateMode(Firefox()).Should().BeTrue();
        }

        [Fact]
        public void SupportsProfilesOrPrivateMode_ForUnknownBrowser_ShouldBeFalse()
        {
            var browser = new Browser { Name = "Some Browser", Target = @"C:\tools\sb.exe" };

            BrowserLaunchProfiles.SupportsProfilesOrPrivateMode(browser).Should().BeFalse();
        }

        #endregion

        #region モデルの互換

        [Fact]
        public void Clone_ShouldCopyProfileAndPrivateMode()
        {
            var browser = Chrome();
            browser.ProfileName = "Profile 1";
            browser.UsePrivateMode = true;

            var clone = browser.Clone();

            clone.ProfileName.Should().Be("Profile 1");
            clone.UsePrivateMode.Should().BeTrue();
        }

        [Fact]
        public void NewBrowser_ShouldDefaultToNoProfileAndNormalWindow()
        {
            // 既存設定ファイルにはこれらの要素が無く、読み込み時は既定値になる。
            // 既定値が「プロファイル指定なし・通常ウィンドウ」でなければ既存動作が変わる。
            var browser = new Browser();

            browser.ProfileName.Should().BeEmpty();
            browser.UsePrivateMode.Should().BeFalse();
        }

        #endregion
    }
}
