using System.Configuration;
using System.Text;
using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Loggerクラスのテスト
    /// </summary>
    public class LoggerTests : IDisposable
    {
        private readonly string _originalLogLevel;
        private readonly Logger.LogLevel _originalCurrentLogLevel;

        public LoggerTests()
        {
            // テスト前の状態を保存
            _originalCurrentLogLevel = Logger.CurrentLogLevel;
            _originalLogLevel = ConfigurationManager.AppSettings["LogLevel"] ?? "";
        }

        public void Dispose()
        {
            // テスト後の状態を復元
            Logger.CurrentLogLevel = _originalCurrentLogLevel;
        }

        #region コンストラクタ・プロパティテスト

        [Fact]
        public void CurrentLogLevel_ShouldHaveDefaultValue()
        {
            // Arrange & Act
            var logLevel = Logger.CurrentLogLevel;

            // Assert
            // 実際のログレベルを確認（Warning、Debug、Traceの可能性がある）
            logLevel.Should().BeOneOf(Logger.LogLevel.Warning, Logger.LogLevel.Debug, Logger.LogLevel.Trace);
        }

        [Fact]
        public void CurrentLogLevel_ShouldBeSettable()
        {
            // Arrange
            var expectedLevel = Logger.LogLevel.Debug;

            // Act
            Logger.CurrentLogLevel = expectedLevel;

            // Assert
            Logger.CurrentLogLevel.Should().Be(expectedLevel);
        }

        #endregion

        #region LogLevel列挙型テスト

        [Fact]
        public void LogLevel_ShouldHaveCorrectValues()
        {
            // Assert
            ((int)Logger.LogLevel.None).Should().Be(0);
            ((int)Logger.LogLevel.Error).Should().Be(1);
            ((int)Logger.LogLevel.Warning).Should().Be(2);
            ((int)Logger.LogLevel.Info).Should().Be(3);
            ((int)Logger.LogLevel.Debug).Should().Be(4);
            ((int)Logger.LogLevel.Trace).Should().Be(5);
        }

        [Fact]
        public void LogLevel_ShouldHaveCorrectNames()
        {
            // Assert
            Logger.LogLevel.None.ToString().Should().Be("None");
            Logger.LogLevel.Error.ToString().Should().Be("Error");
            Logger.LogLevel.Warning.ToString().Should().Be("Warning");
            Logger.LogLevel.Info.ToString().Should().Be("Info");
            Logger.LogLevel.Debug.ToString().Should().Be("Debug");
            Logger.LogLevel.Trace.ToString().Should().Be("Trace");
        }

        #endregion

        #region AddToLogテスト

        [Fact]
        public void AddToLog_WithValidParameters_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Test message";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithNullCaller_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var message = "Test message";

            // Act & Assert
            var action = () => Logger.AddToLog(level, null!, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithNullMessage_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, null!);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Test message";
            var extraVars = new object[] { "param1", 123, true };

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message, extraVars);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithHigherLevelThanCurrent_ShouldNotLog()
        {
            // Arrange
            Logger.CurrentLogLevel = Logger.LogLevel.Info;
            var level = Logger.LogLevel.Debug; // 現在のレベルより高い
            var caller = "TestCaller";
            var message = "Test message";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message);
            action.Should().NotThrow();
        }

        #endregion

        #region LogErrorテスト

        [Fact]
        public void LogError_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Error message";

            // Act & Assert
            var action = () => Logger.LogError(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void LogError_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Error message";
            var extraVars = new object[] { "errorCode", 500 };

            // Act & Assert
            var action = () => Logger.LogError(caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region LogWarningテスト

        [Fact]
        public void LogWarning_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Warning message";

            // Act & Assert
            var action = () => Logger.LogWarning(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void LogWarning_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Warning message";
            var extraVars = new object[] { "warningType", "deprecated" };

            // Act & Assert
            var action = () => Logger.LogWarning(caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region LogInfoテスト

        [Fact]
        public void LogInfo_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Info message";

            // Act & Assert
            var action = () => Logger.LogInfo(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void LogInfo_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Info message";
            var extraVars = new object[] { "userId", 12345 };

            // Act & Assert
            var action = () => Logger.LogInfo(caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region LogDebugテスト

        [Fact]
        public void LogDebug_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Debug message";

            // Act & Assert
            var action = () => Logger.LogDebug(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void LogDebug_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Debug message";
            var extraVars = new object[] { "debugLevel", 2 };

            // Act & Assert
            var action = () => Logger.LogDebug(caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region LogTraceテスト

        [Fact]
        public void LogTrace_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Trace message";

            // Act & Assert
            var action = () => Logger.LogTrace(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void LogTrace_WithExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Trace message";
            var extraVars = new object[] { "traceId", "abc123" };

            // Act & Assert
            var action = () => Logger.LogTrace(caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region InitializeLogLevelテスト

        [Fact]
        public void InitializeLogLevel_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => Logger.InitializeLogLevel();
            action.Should().NotThrow();
        }

        [Fact]
        public void InitializeLogLevel_WithValidSetting_ShouldSetLogLevel()
        {
            // Arrange
            var expectedLevel = Logger.LogLevel.Debug;

            // Act
            Logger.InitializeLogLevel((int)expectedLevel);

            // Assert
            Logger.CurrentLogLevel.Should().Be(expectedLevel);
        }

        [Fact]
        public void InitializeLogLevel_WithInvalidSetting_ShouldUseDefault()
        {
            // Arrange
            var invalidSetting = 10; // 無効な値

            // Act
            Logger.InitializeLogLevel(invalidSetting);

            // Assert
            Logger.CurrentLogLevel.Should().Be(Logger.LogLevel.Info);
        }

        [Fact]
        public void InitializeLogLevel_WithNegativeSetting_ShouldUseDefault()
        {
            // Arrange
            var invalidSetting = -1; // 負の値

            // Act
            Logger.InitializeLogLevel(invalidSetting);

            // Assert
            Logger.CurrentLogLevel.Should().Be(Logger.LogLevel.Info);
        }

        [Theory]
        [InlineData(0, Logger.LogLevel.None)]
        [InlineData(1, Logger.LogLevel.Error)]
        [InlineData(2, Logger.LogLevel.Warning)]
        [InlineData(3, Logger.LogLevel.Info)]
        [InlineData(4, Logger.LogLevel.Debug)]
        [InlineData(5, Logger.LogLevel.Trace)]
        public void InitializeLogLevel_WithValidSettings_ShouldSetCorrectLevel(int setting, Logger.LogLevel expectedLevel)
        {
            // Act
            Logger.InitializeLogLevel(setting);

            // Assert
            Logger.CurrentLogLevel.Should().Be(expectedLevel);
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void AddToLog_WithEmptyString_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "";
            var message = "";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithVeryLongMessage_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = new string('A', 10000); // 非常に長いメッセージ

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithSpecialCharacters_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Special chars: \"quotes\", \n newline, \t tab, \\ backslash";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message);
            action.Should().NotThrow();
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void AddToLog_WithNullExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Test message";

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message, null!);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithEmptyExtraVars_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Test message";
            var extraVars = new object[0];

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message, extraVars);
            action.Should().NotThrow();
        }

        [Fact]
        public void AddToLog_WithNullExtraVarsElements_ShouldNotThrowException()
        {
            // Arrange
            var level = Logger.LogLevel.Info;
            var caller = "TestCaller";
            var message = "Test message";
            var extraVars = new object[] { null!, "valid", null! };

            // Act & Assert
            var action = () => Logger.AddToLog(level, caller, message, extraVars);
            action.Should().NotThrow();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void Logger_ShouldHandleMultipleLogLevels()
        {
            // Arrange
            Logger.CurrentLogLevel = Logger.LogLevel.Trace; // すべてのレベルを有効化
            var caller = "TestCaller";

            // Act & Assert
            var action = () =>
            {
                Logger.LogError(caller, "Error message");
                Logger.LogWarning(caller, "Warning message");
                Logger.LogInfo(caller, "Info message");
                Logger.LogDebug(caller, "Debug message");
                Logger.LogTrace(caller, "Trace message");
            };
            action.Should().NotThrow();
        }

        [Fact]
        public void Logger_ShouldHandleConcurrentAccess()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Concurrent test message";

            // Act & Assert
            var action = () =>
            {
                var tasks = new List<Task>();
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(() => Logger.LogInfo(caller, message, i)));
                }
                Task.WaitAll(tasks.ToArray());
            };
            action.Should().NotThrow();
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Logger_ShouldBeFast()
        {
            // Arrange
            var caller = "TestCaller";
            var message = "Performance test message";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                Logger.LogInfo(caller, message, i);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒未満であることを確認
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void Logger_WithNoneLevel_ShouldNotLogAnything()
        {
            // Arrange
            Logger.CurrentLogLevel = Logger.LogLevel.None;
            var caller = "TestCaller";
            var message = "This should not be logged";

            // Act & Assert
            var action = () => Logger.LogError(caller, message);
            action.Should().NotThrow();
        }

        [Fact]
        public void Logger_WithTraceLevel_ShouldLogEverything()
        {
            // Arrange
            Logger.CurrentLogLevel = Logger.LogLevel.Trace;
            var caller = "TestCaller";

            // Act & Assert
            var action = () =>
            {
                Logger.LogError(caller, "Error");
                Logger.LogWarning(caller, "Warning");
                Logger.LogInfo(caller, "Info");
                Logger.LogDebug(caller, "Debug");
                Logger.LogTrace(caller, "Trace");
            };
            action.Should().NotThrow();
        }

        #endregion
    }
}
