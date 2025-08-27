using System.Diagnostics;
using System.Runtime.InteropServices;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// BrowserUtilitiesクラスのテスト
    /// </summary>
    public class BrowserUtilitiesTests : IDisposable
    {
        private Settings _settings;

        public BrowserUtilitiesTests()
        {
            _settings = new Settings();
        }

        public void Dispose()
        {
            _settings = null;
        }

        #region NormalizeTargetテスト

        [Fact]
        public void NormalizeTarget_WithValidPath_ShouldReturnSamePath()
        {
            // Arrange
            var validPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(validPath);

            // Assert
            result.Should().Be(validPath);
        }

        [Fact]
        public void NormalizeTarget_WithNullPath_ShouldReturnNull()
        {
            // Act
            var result = BrowserUtilities.NormalizeTarget(null!);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NormalizeTarget_WithEmptyPath_ShouldReturnEmpty()
        {
            // Arrange
            var emptyPath = "";

            // Act
            var result = BrowserUtilities.NormalizeTarget(emptyPath);

            // Assert
            result.Should().Be(emptyPath);
        }

        [Fact]
        public void NormalizeTarget_WithWhitespacePath_ShouldReturnWhitespace()
        {
            // Arrange
            var whitespacePath = "   ";

            // Act
            var result = BrowserUtilities.NormalizeTarget(whitespacePath);

            // Assert
            result.Should().Be(whitespacePath);
        }

        [Fact]
        public void NormalizeTarget_WithNonExistentPath_ShouldReturnOriginalPath()
        {
            // Arrange
            var nonExistentPath = @"C:\NonExistent\Browser\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(nonExistentPath);

            // Assert
            result.Should().Be(nonExistentPath);
        }

        [Fact]
        public void NormalizeTarget_WithSpecialCharacters_ShouldReturnSamePath()
        {
            // Arrange
            var pathWithSpecialChars = @"C:\Program Files\Test Browser\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(pathWithSpecialChars);

            // Assert
            result.Should().Be(pathWithSpecialChars);
        }

        #endregion

        #region GetBrowserByGUIDテスト

        [Fact]
        public void GetBrowserByGUID_WithValidGUID_ShouldReturnBrowser()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var browser = new Browser
            {
                Guid = guid,
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe"
            };
            _settings.Browsers.Add(browser);

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid);

            // Assert
            result.Should().NotBeNull();
            result.Guid.Should().Be(guid);
            result.Name.Should().Be("Test Browser");
        }

        [Fact]
        public void GetBrowserByGUID_WithInvalidGUID_ShouldReturnNull()
        {
            // Arrange
            var invalidGuid = Guid.NewGuid();
            var browser = new Browser
            {
                Guid = Guid.NewGuid(),
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe"
            };
            _settings.Browsers.Add(browser);

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(invalidGuid);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetBrowserByGUID_WithEmptyBrowsersList_ShouldReturnNull()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetBrowserByGUID_WithSeparateList_ShouldReturnBrowser()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var browser = new Browser
            {
                Guid = guid,
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe"
            };
            var separateList = new List<Browser> { browser };

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid, separateList);

            // Assert
            result.Should().NotBeNull();
            result.Guid.Should().Be(guid);
            result.Name.Should().Be("Test Browser");
        }

        [Fact]
        public void GetBrowserByGUID_WithEmptySeparateList_ShouldReturnNull()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var separateList = new List<Browser>();

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid, separateList);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetBrowserByGUID_WithNullSeparateList_ShouldReturnNull()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid, null!);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region LaunchBrowserテスト

        [Fact]
        public void LaunchBrowser_WithNullBrowser_ShouldNotThrowException()
        {
            // Arrange
            Browser browser = null!;
            var url = "https://example.com";
            var terminate = false;

            // Act & Assert
            var action = () => BrowserUtilities.LaunchBrowser(browser, url, terminate);
            action.Should().NotThrow();
        }

        [Fact]
        public void LaunchBrowser_WithNullUrl_ShouldNotThrowException()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe"
            };
            string url = null!;
            var terminate = false;

            // Act & Assert
            var action = () => BrowserUtilities.LaunchBrowser(browser, url, terminate);
            action.Should().NotThrow();
        }

        [Fact]
        public void LaunchBrowser_WithEmptyUrl_ShouldNotThrowException()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe"
            };
            var url = "";
            var terminate = false;

            // Act & Assert
            var action = () => BrowserUtilities.LaunchBrowser(browser, url, terminate);
            action.Should().NotThrow();
        }

        [Fact]
        public void LaunchBrowser_WithNonExistentBrowser_ShouldNotThrowException()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Non-existent Browser",
                Target = @"C:\NonExistent\browser.exe"
            };
            var url = "https://example.com";
            var terminate = false;

            // Act & Assert
            var action = () => BrowserUtilities.LaunchBrowser(browser, url, terminate);
            action.Should().NotThrow();
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void NormalizeTarget_WithVeryLongPath_ShouldReturnSamePath()
        {
            // Arrange
            var longPath = new string('A', 1000) + @"\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(longPath);

            // Assert
            result.Should().Be(longPath);
        }

        [Fact]
        public void NormalizeTarget_WithUnicodeCharacters_ShouldReturnSamePath()
        {
            // Arrange
            var unicodePath = @"C:\Program Files\ブラウザ\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(unicodePath);

            // Assert
            result.Should().Be(unicodePath);
        }

        [Fact]
        public void GetBrowserByGUID_WithMultipleBrowsers_ShouldReturnCorrectBrowser()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var browser1 = new Browser { Guid = guid1, Name = "Browser 1" };
            var browser2 = new Browser { Guid = guid2, Name = "Browser 2" };
            _settings.Browsers.Add(browser1);
            _settings.Browsers.Add(browser2);

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid2);

            // Assert
            result.Should().NotBeNull();
            result.Guid.Should().Be(guid2);
            result.Name.Should().Be("Browser 2");
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void NormalizeTarget_WithInvalidCharacters_ShouldReturnSamePath()
        {
            // Arrange
            var invalidPath = @"C:\Program Files\Test<>|:""*\?/Browser\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(invalidPath);

            // Assert
            result.Should().Be(invalidPath);
        }

        [Fact]
        public void GetBrowserByGUID_WithNullSettings_ShouldReturnNull()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void LaunchBrowser_WithInvalidBrowserData_ShouldNotThrowException()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "",
                Target = null,
                Arguments = null
            };
            var url = "https://example.com";
            var terminate = false;

            // Act & Assert
            var action = () => BrowserUtilities.LaunchBrowser(browser, url, terminate);
            action.Should().NotThrow();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void BrowserUtilities_ShouldHandleCompleteWorkflow()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var browser = new Browser
            {
                Guid = guid,
                Name = "Test Browser",
                Target = @"C:\Test\browser.exe",
                Arguments = "--new-window"
            };
            _settings.Browsers.Add(browser);

            // Act
            var foundBrowser = BrowserUtilities.GetBrowserByGUID(guid);

            // Assert
            foundBrowser.Should().NotBeNull();
            foundBrowser.Guid.Should().Be(guid);
            foundBrowser.Name.Should().Be("Test Browser");
        }

        [Fact]
        public void BrowserUtilities_ShouldHandleMultipleOperations()
        {
            // Arrange
            var browsers = new List<Browser>
            {
                new Browser { Guid = Guid.NewGuid(), Name = "Browser 1" },
                new Browser { Guid = Guid.NewGuid(), Name = "Browser 2" },
                new Browser { Guid = Guid.NewGuid(), Name = "Browser 3" }
            };

            // Act & Assert
            foreach (var browser in browsers)
            {
                var result = BrowserUtilities.GetBrowserByGUID(browser.Guid, browsers);
                result.Should().NotBeNull();
                result.Guid.Should().Be(browser.Guid);
            }
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void NormalizeTarget_ShouldBeFast()
        {
            // Arrange
            var path = @"C:\Program Files\Test\browser.exe";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                BrowserUtilities.NormalizeTarget(path);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒未満であることを確認
        }

        [Fact]
        public void GetBrowserByGUID_ShouldBeFast()
        {
            // Arrange
            var browsers = new List<Browser>();
            for (int i = 0; i < 100; i++)
            {
                browsers.Add(new Browser { Guid = Guid.NewGuid(), Name = $"Browser {i}" });
            }
            var targetGuid = browsers[50].Guid;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                BrowserUtilities.GetBrowserByGUID(targetGuid, browsers);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒未満であることを確認
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void NormalizeTarget_WithNetworkPath_ShouldReturnSamePath()
        {
            // Arrange
            var networkPath = @"\\server\share\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(networkPath);

            // Assert
            result.Should().Be(networkPath);
        }

        [Fact]
        public void NormalizeTarget_WithRelativePath_ShouldReturnSamePath()
        {
            // Arrange
            var relativePath = @".\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(relativePath);

            // Assert
            result.Should().Be(relativePath);
        }

        [Fact]
        public void GetBrowserByGUID_WithDuplicateGUIDs_ShouldReturnFirstMatch()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var browser1 = new Browser { Guid = guid, Name = "First Browser" };
            var browser2 = new Browser { Guid = guid, Name = "Second Browser" };
            var browsers = new List<Browser> { browser1, browser2 };

            // Act
            var result = BrowserUtilities.GetBrowserByGUID(guid, browsers);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("First Browser");
        }

        #endregion

        #region 環境依存テスト

        [Fact]
        public void NormalizeTarget_ShouldHandleEnvironmentVariables()
        {
            // Arrange
            var pathWithEnvVar = @"%ProgramFiles%\Test\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(pathWithEnvVar);

            // Assert
            result.Should().Be(pathWithEnvVar);
        }

        [Fact]
        public void NormalizeTarget_ShouldHandleDifferentDriveLetters()
        {
            // Arrange
            var pathWithDrive = @"D:\Program Files\Test\browser.exe";

            // Act
            var result = BrowserUtilities.NormalizeTarget(pathWithDrive);

            // Assert
            result.Should().Be(pathWithDrive);
        }

        #endregion
    }
}
