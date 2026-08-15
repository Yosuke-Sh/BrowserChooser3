using System;
using System.IO;
using BrowserChooser3.Classes.Services.BrowserServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// DefaultBrowserCheckerクラスのテスト
    /// </summary>
    /// <remarks>
    /// 元は41件あったが、うち約30件が「GetDefaultBrowser()を呼んでNotBeNull/BeOfTypeを
    /// 確認するだけ」の同一本体を、異なるセクション見出し（スレッドセーフ・保守性・
    /// 拡張性・セキュリティ等）の下にコピー&amp;ペーストしただけの重複だった（4-1）。
    /// これらは実質1つのテストと同じ検証しかしておらず、削除しても検出力は変わらない。
    /// ここでは実際に区別可能な振る舞い——プロパティの既定値/設定、フォーマット済み
    /// 文字列の内容、リフレクションでのメソッド存在確認——のみを残す。
    /// </remarks>
    public class DefaultBrowserCheckerTests
    {
        [Fact]
        public void GetDefaultBrowser_ShouldReturnDefaultBrowserInfoWithNonEmptyName()
        {
            // Act
            var result = DefaultBrowserChecker.GetDefaultBrowser();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DefaultBrowserChecker.DefaultBrowserInfo>();
            result.Name.Should().NotBeNullOrEmpty();
            result.DetectionMethod.Should().NotBeNullOrEmpty("どの経路で検出したかは診断表示に使われる");
        }

        [Fact]
        public void GetDefaultBrowser_ShouldReturnConsistentResultsAcrossCalls()
        {
            // Act
            var result1 = DefaultBrowserChecker.GetDefaultBrowser();
            var result2 = DefaultBrowserChecker.GetDefaultBrowser();

            // Assert
            result1.Name.Should().Be(result2.Name, "同一環境で連続呼び出しした結果は一致するはず");
            result1.DetectionMethod.Should().Be(result2.DetectionMethod);
        }

        [Fact]
        public void HasDefaultBrowser_ShouldNotThrow()
        {
            // Act
            var action = () => DefaultBrowserChecker.HasDefaultBrowser();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void GetDefaultBrowserDetails_ShouldContainAllExpectedFields()
        {
            // Act
            var result = DefaultBrowserChecker.GetDefaultBrowserDetails();

            // Assert
            result.Should().Contain("Name:");
            result.Should().Contain("Path:");
            result.Should().Contain("Arguments:");
            result.Should().Contain("Protocol:");
            result.Should().Contain("Detection Method:");
        }

        [Fact]
        public void DefaultBrowserInfo_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var info = new DefaultBrowserChecker.DefaultBrowserInfo();

            // Act & Assert
            info.Name = "Test Browser";
            info.Name.Should().Be("Test Browser");

            info.Path = "C:\\test\\browser.exe";
            info.Path.Should().Be("C:\\test\\browser.exe");

            info.Arguments = "--test-arg";
            info.Arguments.Should().Be("--test-arg");

            info.Protocol = "http";
            info.Protocol.Should().Be("http");

            info.DetectionMethod = "Test Method";
            info.DetectionMethod.Should().Be("Test Method");
        }

        [Fact]
        public void DefaultBrowserInfo_Properties_ShouldHaveEmptyStringDefaults()
        {
            // Act
            var info = new DefaultBrowserChecker.DefaultBrowserInfo();

            // Assert
            // nullではなく空文字がデフォルトであること（GetDefaultBrowserDetailsの
            // 文字列整形がnull参照例外を起こさないための前提）
            info.Name.Should().Be(string.Empty);
            info.Path.Should().Be(string.Empty);
            info.Arguments.Should().Be(string.Empty);
            info.Protocol.Should().Be(string.Empty);
            info.DetectionMethod.Should().Be(string.Empty);
        }

        [Fact]
        public void GetDefaultBrowser_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = DefaultBrowserChecker.GetDefaultBrowser();
            stopwatch.Stop();

            // Assert
            // レジストリ/HTTPプロトコルハンドラー照会が主な処理のため、
            // 秒単位で遅くなるのは異常（UIブロッキングに直結する）
            result.Should().NotBeNull();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        }

        [Fact]
        public void SetDefaultBrowser_ShouldBeImplemented()
        {
            // Act & Assert
            var methodInfo = typeof(DefaultBrowserChecker).GetMethod("SetDefaultBrowser");
            methodInfo.Should().NotBeNull();
        }

        [Fact]
        public void ResetDefaultBrowser_ShouldBeImplemented()
        {
            // Act & Assert
            var methodInfo = typeof(DefaultBrowserChecker).GetMethod("ResetDefaultBrowser");
            methodInfo.Should().NotBeNull();
        }
    }
}
