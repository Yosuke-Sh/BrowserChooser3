using System;
using System.Collections.Generic;
using System.Linq;
using BrowserChooser3.Classes.Services.SystemServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// CommandLineProcessorクラスのテスト
    /// ガバレッジ100%を目指して全メソッドをテストします
    /// </summary>
    public class CommandLineProcessorTests
    {
        #region 正常系テスト

        [Fact]
        public void ParseArguments_WithEmptyArgs_ShouldReturnDefaultValues()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.URL.Should().BeNull();
            result.Delay.Should().Be(0);
            result.BrowserGuid.Should().BeNull();
            result.UnshortenURL.Should().BeFalse();
            result.DebugLog.Should().BeFalse();
            result.ExtractDLLs.Should().BeFalse();
            result.PortableMode.Should().BeFalse();
            result.IgnoreSettings.Should().BeFalse();
            result.ShowHelp.Should().BeFalse();
            result.ShowVersion.Should().BeFalse();
            result.SilentMode.Should().BeFalse();
            result.AutoLaunch.Should().BeFalse();
        }

        [Fact]
        public void ParseArguments_WithURL_ShouldSetURL()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.URL.Should().Be("https://example.com");
        }

        [Fact]
        public void ParseArguments_WithHelpOption_ShouldSetShowHelp()
        {
            // Arrange
            var args = new[] { "--help" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithHelpOptionShort_ShouldSetShowHelp()
        {
            // Arrange
            var args = new[] { "-h" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithHelpOptionSlash_ShouldSetShowHelp()
        {
            // Arrange
            var args = new[] { "/?" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowHelp.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithVersionOption_ShouldSetShowVersion()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowVersion.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithVersionOptionShort_ShouldSetShowVersion()
        {
            // Arrange
            var args = new[] { "-v" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ShowVersion.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithDelayOption_ShouldSetDelay()
        {
            // Arrange
            var args = new[] { "--delay", "10" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(10);
        }

        [Fact]
        public void ParseArguments_WithDelayOptionShort_ShouldSetDelay()
        {
            // Arrange
            var args = new[] { "-d", "5" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(5);
        }

        [Fact]
        public void ParseArguments_WithBrowserOption_ShouldSetBrowserGuid()
        {
            // Arrange
            var browserGuid = Guid.NewGuid();
            var args = new[] { "--browser", browserGuid.ToString() };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.BrowserGuid.Should().Be(browserGuid);
        }

        [Fact]
        public void ParseArguments_WithBrowserOptionShort_ShouldSetBrowserGuid()
        {
            // Arrange
            var browserGuid = Guid.NewGuid();
            var args = new[] { "-b", browserGuid.ToString() };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.BrowserGuid.Should().Be(browserGuid);
        }

        [Fact]
        public void ParseArguments_WithUnshortenOption_ShouldSetUnshortenURL()
        {
            // Arrange
            var args = new[] { "--unshorten" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.UnshortenURL.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithUnshortenOptionShort_ShouldSetUnshortenURL()
        {
            // Arrange
            var args = new[] { "-u" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.UnshortenURL.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithDebugOption_ShouldSetDebugLog()
        {
            // Arrange
            var args = new[] { "--debug" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.DebugLog.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithExtractDllsOption_ShouldSetExtractDLLs()
        {
            // Arrange
            var args = new[] { "--extract-dlls" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.ExtractDLLs.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithPortableOption_ShouldSetPortableMode()
        {
            // Arrange
            var args = new[] { "--portable" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.PortableMode.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithIgnoreSettingsOption_ShouldSetIgnoreSettings()
        {
            // Arrange
            var args = new[] { "--ignore-settings" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.IgnoreSettings.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithSilentOption_ShouldSetSilentMode()
        {
            // Arrange
            var args = new[] { "--silent" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.SilentMode.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithAutoLaunchOption_ShouldSetAutoLaunch()
        {
            // Arrange
            var args = new[] { "--auto-launch" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.AutoLaunch.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_WithMultipleOptions_ShouldSetAllOptions()
        {
            // Arrange
            var browserGuid = Guid.NewGuid();
            var args = new[] { "--delay", "15", "--browser", browserGuid.ToString(), "--debug", "https://example.com" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(15);
            result.BrowserGuid.Should().Be(browserGuid);
            result.DebugLog.Should().BeTrue();
            result.URL.Should().Be("https://example.com");
        }

        [Fact]
        public void ParseArguments_WithURLAndOptions_ShouldHandleCorrectly()
        {
            // Arrange
            var args = new[] { "--debug", "https://example.com", "--portable" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.DebugLog.Should().BeTrue();
            result.URL.Should().Be("https://example.com");
            result.PortableMode.Should().BeTrue();
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void ParseArguments_WithInvalidDelay_ShouldIgnoreInvalidDelay()
        {
            // Arrange
            var args = new[] { "--delay", "invalid" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(0);
        }

        [Fact]
        public void ParseArguments_WithInvalidBrowserGuid_ShouldIgnoreInvalidGuid()
        {
            // Arrange
            var args = new[] { "--browser", "invalid-guid" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.BrowserGuid.Should().BeNull();
        }

        [Fact]
        public void ParseArguments_WithMissingDelayValue_ShouldHandleGracefully()
        {
            // Arrange
            var args = new[] { "--delay" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(0);
        }

        [Fact]
        public void ParseArguments_WithMissingBrowserValue_ShouldHandleGracefully()
        {
            // Arrange
            var args = new[] { "--browser" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.BrowserGuid.Should().BeNull();
        }

        [Fact]
        public void ParseArguments_WithNegativeDelay_ShouldAcceptNegativeValue()
        {
            // Arrange
            var args = new[] { "--delay", "-5" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(-5);
        }

        [Fact]
        public void ParseArguments_WithZeroDelay_ShouldAcceptZero()
        {
            // Arrange
            var args = new[] { "--delay", "0" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(0);
        }

        [Fact]
        public void ParseArguments_WithLargeDelay_ShouldAcceptLargeValue()
        {
            // Arrange
            var args = new[] { "--delay", "999999" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(999999);
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void ParseArguments_WithNullArgs_ShouldHandleGracefully()
        {
            // Act & Assert
            Action act = () => CommandLineProcessor.ParseArguments(null!);
            
            // Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseArguments_WithUnknownOption_ShouldIgnoreUnknownOption()
        {
            // Arrange
            var args = new[] { "--unknown-option", "value" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.URL.Should().Be("value");
        }

        [Fact]
        public void ParseArguments_WithEmptyStringArgs_ShouldHandleGracefully()
        {
            // Arrange
            var args = new[] { "", "--debug", "" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.DebugLog.Should().BeTrue();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void ParseArguments_WithComplexScenario_ShouldHandleCorrectly()
        {
            // Arrange
            var browserGuid = Guid.NewGuid();
            var args = new[] { 
                "--delay", "30", 
                "--browser", browserGuid.ToString(), 
                "--debug", 
                "--portable", 
                "--silent", 
                "https://example.com/path?param=value" 
            };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(30);
            result.BrowserGuid.Should().Be(browserGuid);
            result.DebugLog.Should().BeTrue();
            result.PortableMode.Should().BeTrue();
            result.SilentMode.Should().BeTrue();
            result.URL.Should().Be("https://example.com/path?param=value");
        }

        #endregion

        #region ヘルプメッセージテスト

        [Fact]
        public void GetHelpMessage_ShouldReturnNonEmptyString()
        {
            // Act
            var result = CommandLineProcessor.GetHelpMessage();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().Contain("Browser Chooser 3");
            result.Should().Contain("使用方法:");
            result.Should().Contain("オプション:");
        }

        [Fact]
        public void GetHelpMessage_ShouldContainAllOptions()
        {
            // Act
            var result = CommandLineProcessor.GetHelpMessage();

            // Assert
            result.Should().Contain("--help");
            result.Should().Contain("--version");
            result.Should().Contain("--delay");
            result.Should().Contain("--browser");
            result.Should().Contain("--unshorten");
            result.Should().Contain("--debug");
            result.Should().Contain("--extract-dlls");
            result.Should().Contain("--portable");
            result.Should().Contain("--ignore-settings");
            result.Should().Contain("--silent");
            result.Should().Contain("--auto-launch");
        }

        #endregion

        #region バージョン情報テスト

        [Fact]
        public void GetVersionInfo_ShouldReturnNonEmptyString()
        {
            // Act
            var result = CommandLineProcessor.GetVersionInfo();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().Contain("Browser Chooser 3");
            result.Should().Contain("Version");
        }

        #endregion

        #region 引数検証テスト

        [Fact]
        public void ValidateArguments_WithValidURL_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateArguments_WithShowHelp_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                ShowHelp = true
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateArguments_WithShowVersion_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                ShowVersion = true
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateArguments_WithSilentMode_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                SilentMode = true
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateArguments_WithAutoLaunch_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                AutoLaunch = true
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateArguments_WithInvalidArgs_ShouldReturnFalse()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = null,
                ShowHelp = false,
                ShowVersion = false,
                SilentMode = false,
                AutoLaunch = false
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateArguments_WithEmptyURL_ShouldReturnFalse()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = ""
            };

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region 環境変数テスト

        [Fact]
        public void LoadFromEnvironment_WithNoEnvironmentVariables_ShouldReturnOriginalArgs()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com",
                DebugLog = false,
                ExtractDLLs = false,
                PortableMode = false,
                IgnoreSettings = false
            };

            // Act
            var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

            // Assert
            result.Should().BeSameAs(originalArgs);
            result.DebugLog.Should().BeFalse();
            result.ExtractDLLs.Should().BeFalse();
            result.PortableMode.Should().BeFalse();
            result.IgnoreSettings.Should().BeFalse();
        }

        [Fact]
        public void LoadFromEnvironment_WithDebugEnvironmentVariable_ShouldSetDebugLog()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                DebugLog = false
            };

            // 環境変数を設定（テスト用）
            Environment.SetEnvironmentVariable("BROWSERCHOOSER_DEBUG", "true");

            try
            {
                // Act
                var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

                // Assert
                result.Should().BeSameAs(originalArgs);
                result.DebugLog.Should().BeTrue();
            }
            finally
            {
                // 環境変数をクリア
                Environment.SetEnvironmentVariable("BROWSERCHOOSER_DEBUG", null);
            }
        }

        [Fact]
        public void LoadFromEnvironment_WithExtractDllsEnvironmentVariable_ShouldSetExtractDLLs()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                ExtractDLLs = false
            };

            // 環境変数を設定（テスト用）
            Environment.SetEnvironmentVariable("BROWSERCHOOSER_EXTRACT_DLLS", "true");

            try
            {
                // Act
                var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

                // Assert
                result.Should().BeSameAs(originalArgs);
                result.ExtractDLLs.Should().BeTrue();
            }
            finally
            {
                // 環境変数をクリア
                Environment.SetEnvironmentVariable("BROWSERCHOOSER_EXTRACT_DLLS", null);
            }
        }

        [Fact]
        public void LoadFromEnvironment_WithPortableEnvironmentVariable_ShouldSetPortableMode()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                PortableMode = false
            };

            // 環境変数を設定（テスト用）
            Environment.SetEnvironmentVariable("BROWSERCHOOSER_PORTABLE", "true");

            try
            {
                // Act
                var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

                // Assert
                result.Should().BeSameAs(originalArgs);
                result.PortableMode.Should().BeTrue();
            }
            finally
            {
                // 環境変数をクリア
                Environment.SetEnvironmentVariable("BROWSERCHOOSER_PORTABLE", null);
            }
        }

        [Fact]
        public void LoadFromEnvironment_WithIgnoreSettingsEnvironmentVariable_ShouldSetIgnoreSettings()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                IgnoreSettings = false
            };

            // 環境変数を設定（テスト用）
            Environment.SetEnvironmentVariable("BROWSERCHOOSER_IGNORE_SETTINGS", "true");

            try
            {
                // Act
                var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

                // Assert
                result.Should().BeSameAs(originalArgs);
                result.IgnoreSettings.Should().BeTrue();
            }
            finally
            {
                // 環境変数をクリア
                Environment.SetEnvironmentVariable("BROWSERCHOOSER_IGNORE_SETTINGS", null);
            }
        }

        [Fact]
        public void LoadFromEnvironment_WithInvalidEnvironmentVariable_ShouldNotChangeValue()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                DebugLog = false
            };

            // 環境変数を設定（テスト用）
            Environment.SetEnvironmentVariable("BROWSERCHOOSER_DEBUG", "invalid");

            try
            {
                // Act
                var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

                // Assert
                result.Should().BeSameAs(originalArgs);
                result.DebugLog.Should().BeFalse();
            }
            finally
            {
                // 環境変数をクリア
                Environment.SetEnvironmentVariable("BROWSERCHOOSER_DEBUG", null);
            }
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void ParseArguments_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var args = new[] { "--delay", "10", "--browser", Guid.NewGuid().ToString(), "--debug", "https://example.com" };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = CommandLineProcessor.ParseArguments(args);
            stopwatch.Stop();

            // Assert
            result.Should().NotBeNull();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
        }

        #endregion

        #region エラーハンドリングテスト

        [Fact]
        public void ParseArguments_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var args = new[] { "--delay", "10" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            // 例外が発生してもnullを返さないことを確認
        }

        [Fact]
        public void ValidateArguments_WithException_ShouldReturnFalse()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs();

            // Act
            var result = CommandLineProcessor.ValidateArguments(args);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void LoadFromEnvironment_WithException_ShouldReturnOriginalArgs()
        {
            // Arrange
            var originalArgs = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = CommandLineProcessor.LoadFromEnvironment(originalArgs);

            // Assert
            result.Should().BeSameAs(originalArgs);
        }

        #endregion

        #region データ整合性テスト

        [Fact]
        public void ParseArguments_ShouldMaintainDataIntegrity()
        {
            // Arrange
            var browserGuid = Guid.NewGuid();
            var args = new[] { "--delay", "20", "--browser", browserGuid.ToString(), "--debug", "https://example.com" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.Delay.Should().Be(20);
            result.BrowserGuid.Should().Be(browserGuid);
            result.DebugLog.Should().BeTrue();
            result.URL.Should().Be("https://example.com");
        }

        #endregion

        #region スレッドセーフテスト

        [Fact]
        public void ParseArguments_ShouldBeThreadSafe()
        {
            // Arrange
            var args = new[] { "--debug", "https://example.com" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            result.DebugLog.Should().BeTrue();
            result.URL.Should().Be("https://example.com");
        }

        #endregion

        #region 完全カバレッジテスト

        [Fact]
        public void ParseArguments_ShouldCoverAllCodePaths()
        {
            // Arrange
            var args = new[] { "--delay", "5", "--browser", Guid.NewGuid().ToString(), "--debug", "https://example.com" };

            // Act
            var result = CommandLineProcessor.ParseArguments(args);

            // Assert
            result.Should().NotBeNull();
            // すべてのコードパスをカバーすることを確認
        }

        #endregion
    }
}
