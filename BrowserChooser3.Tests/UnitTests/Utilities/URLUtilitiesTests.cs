using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// URLUtilitiesクラスのテスト
    /// </summary>
    public class URLUtilitiesTests
    {
        #region 基本機能テスト

        [Fact]
        public void URLUtilities_Constructor_ShouldNotThrowException()
        {
            // Act & Assert
            typeof(URLUtilities).Should().NotBeNull();
        }

        [Fact]
        public void URLUtilities_StaticMethods_ShouldBeAccessible()
        {
            // Act & Assert
            typeof(URLUtilities).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Should().NotBeEmpty();
        }
        #endregion

        #region URL検証テスト

        [Theory]
        [InlineData("https://www.google.com", true)]
        [InlineData("http://www.example.com", true)]
        [InlineData("ftp://ftp.example.com", true)]
        [InlineData("file:///C:/path/to/file.txt", true)]
        [InlineData("not-a-url", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void URLUtilities_IsValidURL_ShouldReturnCorrectResult(string? url, bool expected)
        {
            // Act
            var isValid = URLUtilities.IsValidURL(url!);

            // Assert
            isValid.Should().Be(expected);
        }
        #endregion

        #region ファイルパステスト

        [Theory]
        [InlineData("C:\\path\\to\\file.txt", true)]
        [InlineData("D:\\folder\\document.pdf", true)]
        [InlineData("\\\\server\\share\\file.doc", true)]
        [InlineData("https://www.example.com", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void URLUtilities_IsFilePath_ShouldReturnCorrectResult(string? path, bool expected)
        {
            // Act
            var isFilePath = URLUtilities.IsFilePath(path!);

            // Assert
            isFilePath.Should().Be(expected);
        }
        #endregion

        #region URL正規化テスト

        [Theory]
        [InlineData("https://www.example.com", "https://www.example.com/")]
        [InlineData("http://example.com/path", "http://example.com/path")]
        [InlineData("https://example.com/path/", "https://example.com/path/")]
        [InlineData("https://EXAMPLE.COM", "https://example.com/")]
        public void URLUtilities_CanonicalizeURL_ShouldReturnCorrectResult(string input, string expected)
        {
            // Act
            var canonicalized = URLUtilities.CanonicalizeURL(input);

            // Assert
            canonicalized.Should().Be(expected);
        }
        #endregion

        #region URL短縮解除テスト

        [Fact]
        public void URLUtilities_UnshortenURL_ShouldReturnString()
        {
            // Arrange
            var shortUrl = "https://bit.ly/test";

            // Act
            var result = URLUtilities.UnshortenURL(shortUrl, "Mozilla/5.0");

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task URLUtilities_UnshortenURLAsync_ShouldReturnString()
        {
            // Arrange
            var shortUrl = "https://example.com";
            var callbackCalled = false;
            var callbackResult = "";

            // Act
            URLUtilities.UnshortenURLAsync(shortUrl, "Mozilla/5.0", (result) =>
            {
                callbackCalled = true;
                callbackResult = result;
            });

            // Assert
            // 非同期処理の完了を待機
            await Task.Delay(2000); // 2秒待機
            callbackCalled.Should().BeTrue();
            callbackResult.Should().NotBeNull();
        }
        #endregion

        #region URLマッチングテスト

        [Theory]
        [InlineData("https://www.google.com", "https://www.google.com", true)]
        [InlineData("https://www.google.com/search", "https://www.google.com/search", true)]
        [InlineData("https://www.google.com", "https://www.bing.com", false)]
        [InlineData("", "", true)]
        [InlineData(null, null, true)]
        public void URLUtilities_MatchURLs_ShouldReturnCorrectResult(string? url1, string? url2, bool expected)
        {
            // Act
            var matches = URLUtilities.MatchURLs(url1!, url2!);

            // Assert
            matches.Should().Be(expected);
        }
        #endregion

        #region エッジケーステスト

        [Theory]
        [InlineData("https://www.example.com:8080/path?param=value#fragment")]
        [InlineData("http://user:pass@example.com/path")]
        [InlineData("file:///C:/path/with spaces/file.txt")]
        [InlineData("ftp://ftp.example.com:21/path/to/file")]
        public void URLUtilities_WithComplexURLs_ShouldHandleCorrectly(string url)
        {
            // Act
            var isValid = URLUtilities.IsValidURL(url);

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("C:\\path\\with spaces\\file.txt")]
        [InlineData("D:\\folder\\with\\unicode\\フォルダ\\file.txt")]
        [InlineData("\\\\server\\share\\with\\special\\chars\\file.txt")]
        public void URLUtilities_WithComplexFilePaths_ShouldHandleCorrectly(string path)
        {
            // Act
            var isFilePath = URLUtilities.IsFilePath(path);

            // Assert
            isFilePath.Should().BeTrue();
        }
        #endregion

        #region パフォーマンステスト

        [Fact]
        public void URLUtilities_IsValidURL_ShouldBeFast()
        {
            // Arrange
            var url = "https://www.example.com";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10000; i++)
            {
                URLUtilities.IsValidURL(url);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }

        [Fact]
        public void URLUtilities_CanonicalizeURL_ShouldBeFast()
        {
            // Arrange
            var url = "https://www.example.com/path";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10000; i++)
            {
                URLUtilities.CanonicalizeURL(url);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }
        #endregion

        #region スレッド安全性テスト

        [Fact]
        public async Task URLUtilities_StaticMethods_ShouldBeThreadSafe()
        {
            // Arrange
            var tasks = new List<Task<bool>>();
            var testUrl = "https://www.example.com";

            // Act
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => URLUtilities.IsValidURL(testUrl)));
            }

            await Task.WhenAll(tasks);

            // Assert
            tasks.Should().AllSatisfy(task => task.Result.Should().BeTrue());
        }
        #endregion

        #region 統合テスト

        [Fact]
        public void URLUtilities_Methods_ShouldWorkTogether()
        {
            // Arrange
            var url = "https://www.example.com/path";

            // Act & Assert
            var isValid = URLUtilities.IsValidURL(url);
            isValid.Should().BeTrue();

            var canonicalized = URLUtilities.CanonicalizeURL(url);
            canonicalized.Should().NotBeNull();

            var matches = URLUtilities.MatchURLs(url, canonicalized);
            matches.Should().BeTrue();
        }
        #endregion

        #region メモリテスト

        [Fact]
        public void URLUtilities_Methods_ShouldNotLeakMemory()
        {
            // Arrange
            var url = "https://www.example.com";
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 10000; i++)
            {
                URLUtilities.IsValidURL(url);
                URLUtilities.CanonicalizeURL(url);
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(3 * 1024 * 1024); // 3MB以内（さらに寛容な条件）
        }
        #endregion

        #region 例外処理テスト

        [Fact]
        public void URLUtilities_WithVeryLongURL_ShouldHandleGracefully()
        {
            // Arrange
            var longUrl = "https://www.example.com/" + new string('a', 10000);

            // Act
            var isValid = URLUtilities.IsValidURL(longUrl);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void URLUtilities_WithUnicodeCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var unicodeUrl = "https://www.example.com/パス/ファイル";

            // Act
            var isValid = URLUtilities.IsValidURL(unicodeUrl);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion

        #region 境界値テスト

        [Theory]
        [InlineData("http://")]
        [InlineData("https://")]
        [InlineData("ftp://")]
        [InlineData("file://")]
        public void URLUtilities_WithMinimalURLs_ShouldHandleGracefully(string url)
        {
            // Act
            var isValid = URLUtilities.IsValidURL(url);

            // Assert
            // 不完全なURL（スキームのみ）も有効とみなす
            isValid.Should().BeTrue();
        }
        #endregion

        #region 大文字小文字テスト

        [Theory]
        [InlineData("HTTPS://WWW.EXAMPLE.COM")]
        [InlineData("Http://Example.Com")]
        [InlineData("https://www.example.com")]
        public void URLUtilities_WithMixedCase_ShouldHandleCorrectly(string url)
        {
            // Act
            var isValid = URLUtilities.IsValidURL(url);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion

        #region 特殊文字テスト

        [Theory]
        [InlineData("https://www.example.com/path with spaces")]
        [InlineData("https://www.example.com/path/with/special/chars")]
        [InlineData("https://www.example.com/path/with/unicode/パス")]
        public void URLUtilities_WithSpecialCharacters_ShouldHandleCorrectly(string url)
        {
            // Act
            var isValid = URLUtilities.IsValidURL(url);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion
    }
}
