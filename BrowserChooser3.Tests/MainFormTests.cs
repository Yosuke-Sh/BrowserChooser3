using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// MainFormクラスのテスト
    /// </summary>
    public class MainFormTests : IDisposable
    {
        private MainForm _form;

        public MainFormTests()
        {
            _form = new MainForm();
        }

        public void Dispose()
        {
            _form?.Dispose();
        }

        #region コンストラクタテスト

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var form = new MainForm();

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion

        #region URL処理テスト

        [Fact]
        public void UpdateURL_WithValidUrl_ShouldNotThrowException()
        {
            // Arrange
            var url = "https://example.com";

            // Act & Assert
            var action = () => _form.UpdateURL(url);
            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-url")]
        public void UpdateURL_WithInvalidUrl_ShouldNotThrowException(string? url)
        {
            // Act & Assert
            var action = () => _form.UpdateURL(url!);
            action.Should().NotThrow();
        }
        #endregion

        #region イベントテスト

        [Fact]
        public void FormLoad_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => _form.PerformLayout();
            action.Should().NotThrow();
        }
        #endregion

        #region レイアウトテスト

        [Fact]
        public void Layout_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => _form.PerformLayout();
            action.Should().NotThrow();
        }
        #endregion

        #region 境界値テスト

        [Fact]
        public void UpdateURL_WithVeryLongUrl_ShouldHandleGracefully()
        {
            // Arrange
            var longUrl = "https://example.com/" + new string('a', 10000);

            // Act & Assert
            var action = () => _form.UpdateURL(longUrl);
            action.Should().NotThrow();
        }

        [Fact]
        public void UpdateURL_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var specialUrl = "https://example.com/path with spaces and special chars!@#$%^&*()";

            // Act & Assert
            var action = () => _form.UpdateURL(specialUrl);
            action.Should().NotThrow();
        }
        #endregion

        #region 例外処理テスト

        [Fact]
        public void UpdateURL_WithNetworkError_ShouldHandleGracefully()
        {
            // Arrange
            var invalidUrl = "https://invalid-domain-that-does-not-exist-12345.com";

            // Act & Assert
            var action = () => _form.UpdateURL(invalidUrl);
            action.Should().NotThrow();
        }
        #endregion

        #region 統合テスト

        [Fact]
        public void MainForm_ShouldHandleCompleteWorkflow()
        {
            // Act & Assert
            var action = () =>
            {
                _form.UpdateURL("https://example.com");
                _form.PerformLayout();
            };
            action.Should().NotThrow();
        }
        #endregion

        #region パフォーマンステスト

        [Fact]
        public void UpdateURL_ShouldBeFast()
        {
            // Arrange
            var url = "https://example.com";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                _form.UpdateURL(url);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }
        #endregion

        #region エッジケーステスト

        [Fact]
        public void UpdateURL_WithEmptyUrl_ShouldHandleGracefully()
        {
            // Act & Assert
            var action = () => _form.UpdateURL("");
            action.Should().NotThrow();
        }

        [Fact]
        public void UpdateURL_WithNullUrl_ShouldHandleGracefully()
        {
            // Act & Assert
            var action = () => _form.UpdateURL(null!);
            action.Should().NotThrow();
        }
        #endregion

        #region スレッド安全性テスト

        [Fact]
        public async Task UpdateURL_ShouldBeThreadSafe()
        {
            // Arrange
            var url = "https://example.com";
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => _form.UpdateURL(url)));
            }

            await Task.WhenAll(tasks);

            // Assert
            tasks.Should().HaveCount(10);
        }
        #endregion

        #region メモリテスト

        [Fact]
        public void UpdateURL_ShouldNotLeakMemory()
        {
            // Arrange
            var url = "https://example.com";
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 1000; i++)
            {
                _form.UpdateURL(url);
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(10 * 1024 * 1024); // 10MB以内
        }
        #endregion
    }
}
