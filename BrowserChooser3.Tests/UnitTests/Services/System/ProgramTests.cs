using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// StartupLauncherの静的状態（URL/Delay/Browser等）をStartupLauncherTests等と共有するため、
    /// 同一コレクションに参加し並列実行による競合を避ける
    /// （コレクション定義自体はStartupLauncherTests.csにある）
    /// </summary>
    [Collection("StartupLauncherSharedState")]
    public class ProgramTests
    {
        [Fact]
        public void Program_StaticInitialization_ShouldSetUpLoggingCorrectly()
        {
            // Arrange & Act - Program.Main()の初期化部分をテスト
            // ログレベルが正しく設定されていることを確認
            var currentLogLevel = Logger.CurrentLogLevel;

            // Assert
            // 実際のログレベルを確認（Warning、Debug、Traceの可能性がある）
            currentLogLevel.Should().BeOneOf(Logger.LogLevel.Warning, Logger.LogLevel.Debug, Logger.LogLevel.Trace);
        }

        [Fact]
        public void Program_LoggerInitialization_ShouldWorkCorrectly()
        {
            // Arrange
            // Logger.CurrentLogLevelはstaticであり、LoggerTestsが先に走ると
            // 別の値が残ったままになる。フィクスチャで退避・復元して順序依存を断つ。
            using var loggerState = new LoggerFixture();
            loggerState.SetLevel(Logger.LogLevel.Trace);

            // Act
            Logger.InitializeLogLevel();

            // Assert
            // app.configにLogLevel指定が無いため、既定のWarningへ落ち着く
            Logger.CurrentLogLevel.Should().Be(Logger.LogLevel.Warning);
        }

        [Fact]
        public void Program_EnvironmentArgs_ShouldBeAccessible()
        {
            // Arrange & Act
            var args = Environment.GetCommandLineArgs();

            // Assert
            args.Should().NotBeNull();
            args.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Program_StartupLauncherInitialization_ShouldHandleEmptyArgs()
        {
            // Arrange
            var emptyArgs = new string[0];

            // Act
            var result = StartupLauncher.Initialize(emptyArgs);

            // Assert
            result.Should().BeFalse(); // 空の引数では初期化が失敗する可能性がある
        }

        [Fact]
        public void Program_StartupLauncherInitialization_ShouldHandleNullArgs()
        {
            // Arrange
            string[]? nullArgs = null;

            // Act
            var result = StartupLauncher.Initialize(nullArgs!);

            // Assert
            result.Should().BeFalse(); // null引数では初期化が失敗する可能性がある
        }

        [Fact]
        public void Program_StartupLauncherInitialization_ShouldHandleValidArgs()
        {
            // Arrange
            var validArgs = new string[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(validArgs);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Program_MainFormCreation_ShouldBePossible()
        {
            // Arrange & Act
            // MainFormの作成が可能であることを確認
            var mainFormType = typeof(BrowserChooser3.Forms.MainForm);

            // Assert
            mainFormType.Should().NotBeNull();
            // コンストラクタの存在を確認
            mainFormType.GetConstructor(Type.EmptyTypes).Should().NotBeNull();
        }

        [Fact]
        public void Program_CommandLineArgsProcessing_ShouldWorkCorrectly()
        {
            // Arrange
            var testArgs = new string[] { "program.exe", "https://test.com", "--option" };

            // Act
            var args = testArgs.Skip(1).ToArray();

            // Assert
            args.Should().HaveCount(2);
            args[0].Should().Be("https://test.com");
            args[1].Should().Be("--option");
        }

        [Fact]
        public void Program_StartupLauncherURL_ShouldBeAccessible()
        {
            // Arrange & Act
            var url = StartupLauncher.URL;

            // Assert
            url.Should().NotBeNull();
        }

        [Fact]
        public void Program_StartupLauncherSetURL_ShouldWorkCorrectly()
        {
            // Arrange
            var testUrl = "https://test.example.com";

            // Act
            StartupLauncher.SetURL(testUrl, false, (url) => { });

            // Assert
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void Program_StartupLauncherSetURL_WithNull_ShouldHandleGracefully()
        {
            // Arrange
            string? nullUrl = null;

            // Act
            StartupLauncher.SetURL(nullUrl!, false, (url) => { });

            // Assert
            StartupLauncher.URL.Should().Be(string.Empty);
        }

        [Fact]
        public void Program_StartupLauncherSetURL_WithEmpty_ShouldHandleGracefully()
        {
            // Arrange
            var emptyUrl = string.Empty;

            // Act
            StartupLauncher.SetURL(emptyUrl, false, (url) => { });

            // Assert
            StartupLauncher.URL.Should().Be(string.Empty);
        }

        [Fact]
        public void Program_EnvironmentVariables_ShouldBeAccessible()
        {
            // Arrange & Act
            var currentDirectory = Environment.CurrentDirectory;
            var machineName = Environment.MachineName;
            var osVersion = Environment.OSVersion;

            // Assert
            currentDirectory.Should().NotBeNullOrEmpty();
            machineName.Should().NotBeNullOrEmpty();
            osVersion.Should().NotBeNull();
        }

        [Fact]
        public void Program_ResourceCleanup_ShouldBeConsidered()
        {
            // Arrange & Act
            // リソースクリーンアップが考慮されていることを確認
            // MainFormはIDisposableを実装しているため、適切にDisposeされるべき
            var mainFormType = typeof(BrowserChooser3.Forms.MainForm);
            var isDisposable = typeof(IDisposable).IsAssignableFrom(mainFormType);

            // Assert
            isDisposable.Should().BeTrue();
        }

        [Fact]
        public void Program_LoggingLevel_ShouldBeConfigurable()
        {
            // Arrange & Act
            var originalLevel = Logger.CurrentLogLevel;
            Logger.CurrentLogLevel = Logger.LogLevel.Debug;

            // Assert
            Logger.CurrentLogLevel.Should().Be(Logger.LogLevel.Debug);

            // Cleanup
            Logger.CurrentLogLevel = originalLevel;
        }

        [Fact]
        public void Program_StartupProcess_ShouldBeDeterministic()
        {
            // Arrange & Act
            // 起動プロセスが決定論的であることを確認
            // 同じ引数で同じ結果が得られることを確認
            var args1 = new string[] { "https://example1.com" };
            var args2 = new string[] { "https://example1.com" };

            var result1 = StartupLauncher.Initialize(args1);
            var result2 = StartupLauncher.Initialize(args2);

            // Assert
            result1.Should().Be(result2);
        }
    }
}
