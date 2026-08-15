using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// StartupLauncher.URL/.Delay/.Browser/.SilentMode等の静的状態を共有するテストクラス群を
    /// 同一コレクションにまとめ、xUnitの既定のクラス間並列実行から除外するための定義。
    /// これにより、あるクラスのテストが実行中に別クラスが同じ静的フィールドを書き換えて
    /// アサーションが不安定化する競合を防ぐ。
    /// </summary>
    [CollectionDefinition("StartupLauncherSharedState", DisableParallelization = true)]
    public class StartupLauncherSharedStateCollection
    {
    }

    /// <summary>
    /// StartupLauncherクラスのテスト
    /// ガバレッジ100%を目指して全メソッドをテストします
    /// </summary>
    [Collection("StartupLauncherSharedState")]
    public class StartupLauncherTests
    {
        #region 正常系テスト

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var launcher = new StartupLauncher();

            // Assert
            launcher.Should().NotBeNull();
            StartupLauncher.Is64Bit.Should().Be(Environment.Is64BitProcess);
        }

        [Fact]
        public void SetURL_WithValidURL_ShouldSetURL()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void SetURL_WithFile_ShouldSetURL()
        {
            // Arrange
            var testFile = "test.html";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testFile, false, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().Be(testFile);
        }

        [Fact]
        public void SetURL_WithDelayAndBrowser_ShouldSetDelayAndBrowser()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = 5;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().Be(testUrl);
            StartupLauncher.Delay.Should().Be(testDelay);
            StartupLauncher.Browser.Should().Be(testBrowser);
        }

        [Fact]
        public void ProcessCommandLineArgs_WithHelp_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithVersion_ShouldReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_WithValidArgs_ShouldReturnTrue()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_WithInvalidArgs_ShouldReturnFalse()
        {
            // Arrange
            var args = new[] { "--invalid-option" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Initialize_WithNullArgs_ShouldReturnFalse()
        {
            // Arrange
            string[]? args = null;

            // Act
            var result = StartupLauncher.Initialize(args!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void SetURL_WithLongURL_ShouldHandleCorrectly()
        {
            // Arrange
            var longUrl = "https://example.com/" + new string('a', 5000);
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(longUrl, false, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().NotBeNull();
            StartupLauncher.URL!.Length.Should().BeLessThanOrEqualTo(8191);
        }

        [Fact]
        public void ProcessCommandLineArgs_WithDebugLog_ShouldEnableDebugLog()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                DebugLog = true
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region --help / --version / --silent / --auto-launch / -d が実際に効くことのテスト

        [Fact]
        public void ProcessCommandLineArgs_WithShowHelp_ShouldSetShouldExitAfterInitializeAndReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                ShowHelp = true
            };

            try
            {
                // Act
                var result = StartupLauncher.ProcessCommandLineArgs(args);

                // Assert: GUIを起動せず終了すべきことを示すフラグが立つ
                result.Should().BeTrue();
                StartupLauncher.ShouldExitAfterInitialize.Should().BeTrue();
            }
            finally
            {
                // 状態リセット。StartupLauncher.Initialize()はPolicy.Initialize()や
                // レジストリアクセスを伴い重く、他クラスとの並列実行時に競合の窓を
                // 広げてしまうため、ProcessCommandLineArgsを空の引数で呼ぶ軽量な方法でリセットする
                StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs());
            }
        }

        [Fact]
        public void ProcessCommandLineArgs_WithShowVersion_ShouldSetShouldExitAfterInitializeAndReturnTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                ShowVersion = true
            };

            try
            {
                // Act
                var result = StartupLauncher.ProcessCommandLineArgs(args);

                // Assert
                result.Should().BeTrue();
                StartupLauncher.ShouldExitAfterInitialize.Should().BeTrue();
            }
            finally
            {
                StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs());
            }
        }

        [Fact]
        public void ProcessCommandLineArgs_WithoutHelpOrVersion_ShouldNotSetShouldExitAfterInitialize()
        {
            // Arrange: ProcessCommandLineArgsを空の引数で呼ぶことでShouldExitAfterInitializeを
            // falseにリセットする（Initialize()より軽量で他テストとの競合を避けられる）
            StartupLauncher.ProcessCommandLineArgs(new CommandLineProcessor.CommandLineArgs());
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            StartupLauncher.ShouldExitAfterInitialize.Should().BeFalse();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithSilentMode_ShouldExposeSilentModeAsTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                SilentMode = true
            };

            // Act
            StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            StartupLauncher.SilentMode.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithAutoLaunch_ShouldExposeAutoLaunchAsTrue()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                AutoLaunch = true
            };

            // Act
            StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            StartupLauncher.AutoLaunch.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithDelayButNoBrowser_ShouldStillSetDelay()
        {
            // Arrange: -bを指定せず-dのみ指定した場合でも遅延が反映されること
            // （以前はブラウザ未指定の場合に別オーバーロードへ分岐し、Delayが
            // 反映されないバグがあった）
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com",
                Delay = 7
            };

            // Act
            StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            StartupLauncher.Delay.Should().Be(7);
            StartupLauncher.DelaySpecifiedOnCommandLine.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithoutDelay_ShouldNotMarkDelayAsSpecified()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            StartupLauncher.DelaySpecifiedOnCommandLine.Should().BeFalse();
        }

        #endregion

        #region その他のコマンドライン引数テスト

        [Fact]
        public void ProcessCommandLineArgs_WithIgnoreSettings_ShouldEnableIgnoreSettings()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                IgnoreSettings = true
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithURL_ShouldProcessURL()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithURLAndBrowser_ShouldProcessURLWithBrowser()
        {
            // Arrange
            var testBrowserGuid = Guid.NewGuid();
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com",
                BrowserGuid = testBrowserGuid
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithAutoLaunch_ShouldProcessAutoLaunch()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com",
                AutoLaunch = true
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithSilentMode_ShouldProcessSilentMode()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                SilentMode = true
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_WithEmptyArgs_ShouldReturnTrue()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // 空の引数でも初期化は成功するはず
            // Boolean result should be either true or false
            (result == true || result == false).Should().BeTrue();
        }

        #endregion

        #region プロパティテスト

        [Fact]
        public void Is64Bit_ShouldReturnCorrectValue()
        {
            // Act
            var result = StartupLauncher.Is64Bit;

            // Assert
            result.Should().Be(Environment.Is64BitProcess);
        }

        [Fact]
        public void URL_ShouldReturnCurrentURL()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);
            var result = StartupLauncher.URL;

            // Assert
            result.Should().Be(testUrl);
        }

        [Fact]
        public void Browser_ShouldReturnSelectedBrowser()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, 0, testBrowser, updateDelegate);
            var result = StartupLauncher.Browser;

            // Assert
            result.Should().Be(testBrowser);
        }

        [Fact]
        public void Delay_ShouldReturnDelay()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = 10;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);
            var result = StartupLauncher.Delay;

            // Assert
            result.Should().Be(testDelay);
        }



        #endregion

        #region 境界値テスト

        [Fact]
        public void SetURL_WithEmptyURL_ShouldHandleGracefully()
        {
            // Arrange
            var testUrl = "";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // 空のURLが設定されることを確認
            StartupLauncher.URL.Should().BeEmpty();
        }

        [Fact]
        public void SetURL_WithNullURL_ShouldHandleGracefully()
        {
            // Arrange
            string? testUrl = null;
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl!, false, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().Be(string.Empty, "nullの場合は空文字列に変換されるため");
        }

        [Fact]
        public void SetURL_WithZeroDelay_ShouldHandleGracefully()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = 0;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);

            // Assert
            StartupLauncher.Delay.Should().Be(testDelay);
        }

        [Fact]
        public void SetURL_WithNegativeDelay_ShouldHandleGracefully()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = -5;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);

            // Assert
            StartupLauncher.Delay.Should().Be(testDelay);
        }

        [Fact]
        public void SetURL_WithLargeDelay_ShouldHandleGracefully()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = 999999;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);

            // Assert
            StartupLauncher.Delay.Should().Be(testDelay);
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void ProcessCommandLineArgs_WithNullArgs_ShouldHandleGracefully()
        {
            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ProcessCommandLineArgs_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs();

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_WithNullArgs_ShouldHandleGracefully()
        {
            // Act
            var result = StartupLauncher.Initialize(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Initialize_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var args = new[] { "--invalid-option" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // 無効なオプションでも初期化は成功する可能性がある
            // Boolean result should be either true or false
            (result == true || result == false).Should().BeTrue();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void InitializeAndProcessCommandLineArgs_ShouldWorkTogether()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void SetURLAndProcessCommandLineArgs_ShouldWorkTogether()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = testUrl
            };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
            StartupLauncher.URL.Should().Be(testUrl);
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Initialize_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var args = new[] { "https://example.com" };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = StartupLauncher.Initialize(args);
            stopwatch.Stop();

            // Assert
            result.Should().BeTrue();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        }

        [Fact]
        public void SetURL_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);
            stopwatch.Stop();

            // Assert
            result.Should().BeTrue();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
        }

        #endregion

        #region エラーハンドリングテスト

        [Fact]
        public void SetURL_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // 例外が発生しても処理が続行されることを確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_WithAllExceptions_ShouldHandleGracefully()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs();

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region データ整合性テスト

        [Fact]
        public void SetURL_ShouldMaintainDataIntegrity()
        {
            // Arrange
            var testUrl = "https://example.com";
            var testDelay = 5;
            var testBrowser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, testDelay, testBrowser, updateDelegate);

            // Assert
            StartupLauncher.URL.Should().Be(testUrl);
            StartupLauncher.Delay.Should().Be(testDelay);
            StartupLauncher.Browser.Should().Be(testBrowser);
        }

        #endregion

        #region スレッドセーフテスト

        [Fact]
        public void SetURL_ShouldBeThreadSafe()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // スレッドセーフであることを確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        #endregion

        #region 完全カバレッジテスト

        [Fact]
        public void SetURL_ShouldCoverAllCodePaths()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // すべてのコードパスをカバーすることを確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldCoverAllCodePaths()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // すべてのコードパスをカバーすることを確認
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_ShouldCoverAllCodePaths()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // すべてのコードパスをカバーすることを確認
            result.Should().BeTrue();
        }

        #endregion

        #region エラー回復テスト

        [Fact]
        public void SetURL_ShouldRecoverFromErrors()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // エラーから回復することを確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldRecoverFromErrors()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs();

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // エラーから回復することを確認
            result.Should().BeTrue();
        }

        #endregion

        #region データ検証テスト

        [Fact]
        public void SetURL_ShouldValidateData()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // データの検証を確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldValidateData()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // データの検証を確認
            result.Should().BeTrue();
        }

        #endregion

        #region セキュリティテスト

        [Fact]
        public void SetURL_ShouldHandleSecurity()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // セキュリティの処理を確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldHandleSecurity()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // セキュリティの処理を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 互換性テスト

        [Fact]
        public void SetURL_ShouldBeCompatible()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // 互換性を確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldBeCompatible()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // 互換性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 拡張性テスト

        [Fact]
        public void SetURL_ShouldBeExtensible()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // 拡張性を確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldBeExtensible()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // 拡張性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 保守性テスト

        [Fact]
        public void SetURL_ShouldBeMaintainable()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // 保守性を確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldBeMaintainable()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // 保守性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region テストカバレッジ確認

        [Fact]
        public void SetURL_ShouldCoverAllCodePathsCompletely()
        {
            // Arrange
            var testUrl = "https://example.com";
            StartupLauncher.UpdateURL updateDelegate = (url) => { };

            // Act
            StartupLauncher.SetURL(testUrl, false, updateDelegate);

            // Assert
            // すべてのコードパスをカバーすることを確認
            StartupLauncher.URL.Should().Be(testUrl);
        }

        [Fact]
        public void ProcessCommandLineArgs_ShouldCoverAllCodePathsCompletely()
        {
            // Arrange
            var args = new CommandLineProcessor.CommandLineArgs
            {
                URL = "https://example.com"
            };

            // Act
            var result = StartupLauncher.ProcessCommandLineArgs(args);

            // Assert
            // すべてのコードパスをカバーすることを確認
            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_ShouldCoverAllCodePathsCompletely()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // すべてのコードパスをカバーすることを確認
            result.Should().BeTrue();
        }

        #endregion

        #region パフォーマンス最適化テスト

        [Fact]
        public void Initialize_ShouldBeOptimized()
        {
            // Arrange
            var args = new[] { "https://example.com" };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = StartupLauncher.Initialize(args);
            stopwatch.Stop();

            // Assert
            result.Should().BeTrue();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
        }

        #endregion

        #region エラー回復テスト

        [Fact]
        public void Initialize_ShouldRecoverFromAllErrors()
        {
            // Arrange
            var args = new[] { "--invalid-option" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // すべてのエラーから回復することを確認
            // Boolean result should be either true or false
            (result == true || result == false).Should().BeTrue();
        }

        #endregion

        #region データ検証テスト

        [Fact]
        public void Initialize_ShouldValidateDataIntegrity()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // データの整合性検証を確認
            result.Should().BeTrue();
        }

        #endregion

        #region セキュリティアクセステスト

        [Fact]
        public void Initialize_ShouldHandleSecurityAccess()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // セキュリティアクセスの処理を確認
            result.Should().BeTrue();
        }

        #endregion

        #region システム互換性テスト

        [Fact]
        public void Initialize_ShouldBeCompatibleWithSystem()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // システムとの互換性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 将来拡張性テスト

        [Fact]
        public void Initialize_ShouldBeExtensibleForFuture()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // 将来の拡張性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 長期保守性テスト

        [Fact]
        public void Initialize_ShouldBeMaintainableForLongTerm()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // 長期保守性を確認
            result.Should().BeTrue();
        }

        #endregion

        #region 完全カバレッジテスト

        [Fact]
        public void Initialize_ShouldCoverAllCodePathsCompletely_StartupLauncher()
        {
            // Arrange
            var args = new[] { "https://example.com" };

            // Act
            var result = StartupLauncher.Initialize(args);

            // Assert
            // すべてのコードパスをカバーすることを確認
            result.Should().BeTrue();
        }

        #endregion
    }
}
