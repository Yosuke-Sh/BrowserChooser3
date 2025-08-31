using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Classes.Services.BrowserServices;
using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// サービスクラスのテスト
    /// </summary>
    public class ServiceTests : IDisposable
    {
        public void Dispose()
        {
            // テスト後のクリーンアップ
        }

        #region StartupLauncherテスト

        [Fact]
        public void StartupLauncher_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var launcher = new StartupLauncher();

            // Assert
            launcher.Should().NotBeNull();
        }

        [Fact]
        public void StartupLauncher_ProcessCommandLineArgs_ShouldNotThrowException()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs { URL = "https://example.com" };

            // Act & Assert
            var action = () => StartupLauncher.ProcessCommandLineArgs(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void StartupLauncher_ProcessCommandLineArgs_WithEmptyArgs_ShouldNotThrowException()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs();

            // Act & Assert
            var action = () => StartupLauncher.ProcessCommandLineArgs(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void StartupLauncher_ProcessCommandLineArgs_WithNullArgs_ShouldNotThrowException()
        {
            // Arrange

            // Act & Assert
            var action = () => StartupLauncher.ProcessCommandLineArgs(null!);
            action.Should().NotThrow();
        }

        [Fact]
        public void StartupLauncher_ProcessCommandLineArgs_WithMultipleArgs_ShouldNotThrowException()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs 
            { 
                URL = "https://example.com"
            };

            // Act & Assert
            var action = () => StartupLauncher.ProcessCommandLineArgs(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void StartupLauncher_ProcessCommandLineArgs_WithInvalidUrls_ShouldNotThrowException()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs 
            { 
                URL = "invalid-url"
            };

            // Act & Assert
            var action = () => StartupLauncher.ProcessCommandLineArgs(args);
            action.Should().NotThrow();
        }

        #endregion

        #region CommandLineProcessorテスト



        [Fact]
        public void CommandLineProcessor_ParseArguments_ShouldNotThrowException()
        {
            // Arrange
            var args = new string[] { "https://example.com" };

            // Act & Assert
            var action = () => CommandLineProcessor.ParseArguments(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void CommandLineProcessor_ParseArguments_WithEmptyArgs_ShouldNotThrowException()
        {
            // Arrange
            var args = new string[] { };

            // Act & Assert
            var action = () => CommandLineProcessor.ParseArguments(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void CommandLineProcessor_ParseArguments_WithNullArgs_ShouldNotThrowException()
        {
            // Arrange

            // Act & Assert
            var action = () => CommandLineProcessor.ParseArguments(null!);
            action.Should().NotThrow();
        }

        [Fact]
        public void CommandLineProcessor_ParseArguments_WithFilePaths_ShouldNotThrowException()
        {
            // Arrange
            var args = new string[] { "C:\\test.html", "D:\\document.pdf" };

            // Act & Assert
            var action = () => CommandLineProcessor.ParseArguments(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void CommandLineProcessor_ParseArguments_WithProtocols_ShouldNotThrowException()
        {
            // Arrange
            var args = new string[] { "mailto:test@example.com", "ftp://example.com" };

            // Act & Assert
            var action = () => CommandLineProcessor.ParseArguments(args);
            action.Should().NotThrow();
        }

        #endregion

        #region DefaultBrowserCheckerテスト

        [Fact]
        public void DefaultBrowserChecker_GetDefaultBrowser_ShouldNotThrowException()
        {
            // Arrange

            // Act & Assert
            var action = () => DefaultBrowserChecker.GetDefaultBrowser();
            action.Should().NotThrow();
        }









        #endregion

        #region BrowserDetectorテスト



        [Fact]
        public void BrowserDetector_DetectBrowsers_ShouldNotThrowException()
        {
            // Arrange

            // Act & Assert
            var action = () => BrowserDetector.DetectBrowsers();
            action.Should().NotThrow();
        }

        [Fact]
        public void BrowserDetector_DetectBrowsers_ShouldReturnList()
        {
            // Arrange

            // Act
            var browsers = BrowserDetector.DetectBrowsers();

            // Assert
            browsers.Should().NotBeNull();
            browsers.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void BrowserDetector_DetectBrowsers_ShouldHandleExceptions()
        {
            // Arrange

            // Act & Assert
            var action = () => BrowserDetector.DetectBrowsers();
            action.Should().NotThrow();
        }

        #endregion







        #region 境界値テスト

        [Fact]
        public void Services_WithMinimalData_ShouldInitializeCorrectly()
        {
            // Act & Assert
            var actions = new List<Action>
            {
                () => new StartupLauncher()
                // CommandLineProcessor, DefaultBrowserChecker, BrowserDetectorは静的クラスなのでインスタンス化不要
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Services_WithEmptyData_ShouldHandleGracefully()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs()),
                () => CommandLineProcessor.ParseArguments(new string[] { }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void Services_WithInvalidData_ShouldHandleGracefully()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "invalid-url" }),
                () => CommandLineProcessor.ParseArguments(new string[] { "invalid-arg", "malformed://arg" }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Services_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://example.com\n\r\t\0" }),
                () => CommandLineProcessor.ParseArguments(new string[] { "C:\\test\n\r\t\0.html" }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void Services_ShouldWorkTogether()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://example.com" }),
                () => CommandLineProcessor.ParseArguments(new string[] { "https://example.com" }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Services_WithRealData_ShouldWorkTogether()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => {
                    var browsers = BrowserDetector.DetectBrowsers();
                    if (browsers.Count > 0)
                    {
                        var browser = browsers[0];
                        // ブラウザの検出が成功したことを確認
                        browser.Should().NotBeNull();
                    }
                },
                () => {
                    var defaultBrowser = DefaultBrowserChecker.GetDefaultBrowser();
                    // デフォルトブラウザの検出が成功したことを確認
                    defaultBrowser.Should().NotBeNull();
                }
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Services_Constructor_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var launcher = new StartupLauncher();
                // CommandLineProcessor, DefaultBrowserChecker, BrowserDetectorは静的クラスなのでインスタンス化不要
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒未満
        }

        [Fact]
        public void Services_Methods_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10; i++)
            {
                StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://example.com" });
                CommandLineProcessor.ParseArguments(new string[] { "https://example.com" });
                DefaultBrowserChecker.GetDefaultBrowser();
                BrowserDetector.DetectBrowsers();
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5秒未満
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void Services_WithUnicodeData_ShouldHandleGracefully()
        {
            // Arrange

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://テスト.com" }),
                () => CommandLineProcessor.ParseArguments(new string[] { "C:\\テスト.html" }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Services_WithVeryLongData_ShouldHandleGracefully()
        {
            // Arrange
            var longString = new string('x', 10000);

            // Act & Assert
            var actions = new List<Action>
            {
                () => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = longString }),
                () => CommandLineProcessor.ParseArguments(new string[] { longString }),
                () => DefaultBrowserChecker.GetDefaultBrowser(),
                () => BrowserDetector.DetectBrowsers()
            };

            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        }

        #endregion

        #region スレッドセーフテスト

        [Fact]
        public async Task Services_ShouldBeThreadSafe()
        {
            // Arrange

            // Act & Assert
            var tasks = new List<Task>
            {
                Task.Run(() => StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://example.com" })),
                Task.Run(() => CommandLineProcessor.ParseArguments(new string[] { "https://example.com" })),
                Task.Run(() => DefaultBrowserChecker.GetDefaultBrowser()),
                Task.Run(() => BrowserDetector.DetectBrowsers())
            };

            await Task.WhenAll(tasks);
        }

        #endregion

        #region メモリテスト

        [Fact]
        public void Services_ShouldNotLeakMemory()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 100; i++)
            {
                StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs { URL = "https://example.com" });
                CommandLineProcessor.ParseArguments(new string[] { "https://example.com" });
                DefaultBrowserChecker.GetDefaultBrowser();
                BrowserDetector.DetectBrowsers();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(10 * 1024 * 1024); // 10MB未満
        }

        #endregion
    }
}
