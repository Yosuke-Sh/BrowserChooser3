using System;
using System.IO;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// PathManagerクラスのテスト
    /// </summary>
    /// <remarks>
    /// <para>
    /// 計画4-4で指摘されていた「テストが1件も無い」クラス。設定ファイル・ログファイルの
    /// 出力先を決める中核であり、<see cref="Logger"/> や <see cref="Settings"/> の
    /// 挙動はすべてここに依存する。
    /// </para>
    /// <para>
    /// <see cref="PathManager.GetConfigDirectory"/> は <see cref="PathManager.ConfigDirectoryOverrideForTests"/>
    /// で上書き可能だが、<see cref="PathManager.GetLogDirectory"/> にはテスト用の上書き口が無い。
    /// 既に <see cref="Logger"/> がテスト実行時にも無条件で実ログディレクトリ
    /// （既定では %LOCALAPPDATA%\BrowserChooser3\Logs）を作成しており、この既存の
    /// 前提を踏襲して同じ実ディレクトリに対してテストする（新たな副作用は増やさない）。
    /// </para>
    /// </remarks>
    public class PathManagerTests : IDisposable
    {
        private readonly string? _originalOverride = PathManager.ConfigDirectoryOverrideForTests;

        public void Dispose()
        {
            PathManager.ConfigDirectoryOverrideForTests = _originalOverride;
        }

        #region GetConfigDirectory

        [Fact]
        public void GetConfigDirectory_WithoutOverride_ShouldReturnPathUnderApplicationData()
        {
            // Arrange
            PathManager.ConfigDirectoryOverrideForTests = null;

            // Act
            var result = PathManager.GetConfigDirectory();

            // Assert
            var expectedRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            result.Should().StartWith(expectedRoot, "既定では%APPDATA%配下に置かれる");
            result.Should().EndWith("BrowserChooser3");
        }

        [Fact]
        public void GetConfigDirectory_WithOverride_ShouldReturnOverriddenPath()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            var result = PathManager.GetConfigDirectory();

            // Assert
            result.Should().Be(tempDir.Path, "テスト用の上書きが最優先される");
        }

        [Fact]
        public void GetConfigDirectory_WithEmptyOverride_ShouldFallBackToDefault()
        {
            // Arrange
            // 空文字は「未設定」として扱われ、実ディレクトリへフォールバックする
            // （nullチェックだけだと空文字上書きがそのまま使われてしまう不具合を防ぐ回帰）
            PathManager.ConfigDirectoryOverrideForTests = "";

            // Act
            var result = PathManager.GetConfigDirectory();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().EndWith("BrowserChooser3");
        }

        [Fact]
        public void GetConfigDirectory_CalledRepeatedly_ShouldReturnSameOverriddenPath()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            var first = PathManager.GetConfigDirectory();
            var second = PathManager.GetConfigDirectory();

            // Assert
            first.Should().Be(second);
        }

        #endregion

        #region GetLogDirectory

        [Fact]
        public void GetLogDirectory_ShouldReturnPathUnderLocalApplicationDataWithLogsSuffix()
        {
            // Act
            var result = PathManager.GetLogDirectory();

            // Assert
            var expectedRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            result.Should().StartWith(expectedRoot);
            result.Should().Contain("BrowserChooser3");
            result.Should().EndWith("Logs");
        }

        [Fact]
        public void GetLogDirectory_ShouldNotBeAffectedByConfigDirectoryOverride()
        {
            // Arrange
            // ログとConfigは別の出力先を持つため、Configの上書きがログに漏れ出さないこと
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            var logDir = PathManager.GetLogDirectory();

            // Assert
            logDir.Should().NotBe(tempDir.Path);
            logDir.Should().NotStartWith(tempDir.Path);
        }

        [Fact]
        public void GetLogDirectory_CalledRepeatedly_ShouldReturnSamePath()
        {
            // Act
            var first = PathManager.GetLogDirectory();
            var second = PathManager.GetLogDirectory();

            // Assert
            first.Should().Be(second);
        }

        #endregion

        #region GetIconCacheDirectory

        [Fact]
        public void GetIconCacheDirectory_ShouldReturnPathUnderLocalApplicationDataWithIconCacheSuffix()
        {
            // Act
            var result = PathManager.GetIconCacheDirectory();

            // Assert
            var expectedRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            result.Should().StartWith(expectedRoot);
            result.Should().EndWith("iconcache");
        }

        [Fact]
        public void GetIconCacheDirectory_ShouldBeDistinctFromLogDirectory()
        {
            // Act
            var iconCacheDir = PathManager.GetIconCacheDirectory();
            var logDir = PathManager.GetLogDirectory();

            // Assert
            // 2-3で追加されたアイコンキャッシュがログファイルと混ざらないこと
            iconCacheDir.Should().NotBe(logDir);
        }

        #endregion

        #region GetConfigFilePath / GetLogFilePath

        [Fact]
        public void GetConfigFilePath_ShouldCombineConfigDirectoryWithFileName()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            var result = PathManager.GetConfigFilePath("BrowserChooser3Config.xml");

            // Assert
            result.Should().Be(Path.Combine(tempDir.Path, "BrowserChooser3Config.xml"));
        }

        [Fact]
        public void GetLogFilePath_ShouldCombineLogDirectoryWithFileName()
        {
            // Act
            var result = PathManager.GetLogFilePath("bc3_2026-08-15.log");

            // Assert
            result.Should().Be(Path.Combine(PathManager.GetLogDirectory(), "bc3_2026-08-15.log"));
        }

        #endregion

        #region EnsureDirectoriesExist

        [Fact]
        public void EnsureDirectoriesExist_ShouldCreateConfigAndLogDirectoriesIfMissing()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            // Configディレクトリ自体は上書き先として存在するが、その配下に無いサブパスを使い、
            // 「無ければ作る」分岐を確実に通す
            var configDir = Path.Combine(tempDir.Path, "config-subdir");
            PathManager.ConfigDirectoryOverrideForTests = configDir;
            Directory.Exists(configDir).Should().BeFalse("テストの前提としてまだ存在しないこと");

            // Act
            PathManager.EnsureDirectoriesExist();

            // Assert
            Directory.Exists(configDir).Should().BeTrue("存在しなければ作成される");
            // ログディレクトリは実際の%LOCALAPPDATA%配下（Loggerが既にテスト実行時にも
            // 作成している前提）のため、例外なく完了することの確認に留める
        }

        [Fact]
        public void EnsureDirectoriesExist_WhenDirectoriesAlreadyExist_ShouldNotThrow()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            // 既に存在するディレクトリに対して呼んでも冪等であること
            var action = () =>
            {
                PathManager.EnsureDirectoriesExist();
                PathManager.EnsureDirectoriesExist();
            };

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region Initialize

        [Fact]
        public void Initialize_ShouldNotThrow()
        {
            // Act
            var action = () => PathManager.Initialize();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Initialize_ShouldNotAffectConfigDirectoryOverride()
        {
            // Arrange
            using var tempDir = new TempDirectoryFixture();
            PathManager.ConfigDirectoryOverrideForTests = tempDir.Path;

            // Act
            PathManager.Initialize();

            // Assert
            // Initialize()は内部の既定値をリセットするだけで、
            // テスト用の上書きは常に最優先されるべき
            PathManager.GetConfigDirectory().Should().Be(tempDir.Path);
        }

        #endregion
    }
}
