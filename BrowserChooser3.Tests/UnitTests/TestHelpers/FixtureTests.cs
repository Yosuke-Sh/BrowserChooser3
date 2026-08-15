using System;
using System.IO;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests.UnitTests.TestHelpers
{
    /// <summary>
    /// テスト分離基盤（Fixture群）自体の振る舞いテスト。
    /// </summary>
    /// <remarks>
    /// これらのFixtureは「テスト間で状態が漏れないこと」を担保するための土台なので、
    /// 復元が実際に行われることを検証しておかないと、静かに壊れたときに
    /// 他の全テストが前のテストの残骸に対して走ることになる。
    /// </remarks>
    [Collection(SettingsStateCollection.Name)]
    public class FixtureTests
    {
        [Fact]
        public void SettingsFixture_Dispose_ShouldRestoreOriginalCurrent()
        {
            // Arrange
            var original = Settings.Current;

            // Act
            Settings replaced;
            using (var fixture = new SettingsFixture())
            {
                replaced = Settings.Current;
                replaced.Should().NotBeSameAs(original, "フィクスチャは新しい設定へ差し替える");
            }

            // Assert
            Settings.Current.Should().BeSameAs(original);
        }

        [Fact]
        public void SettingsFixture_Replace_ShouldSwapCurrentAndStillRestore()
        {
            // Arrange
            var original = Settings.Current;
            var custom = new Settings { ShowURL = false };

            // Act
            using (var fixture = new SettingsFixture())
            {
                fixture.Replace(custom);
                Settings.Current.Should().BeSameAs(custom);
            }

            // Assert
            Settings.Current.Should().BeSameAs(original);
        }

        // LoggerFixture自体のテストは、同じstatic状態に触れるLoggerTests
        // （StartupLauncherSharedStateコレクション）側に置いている。

        [Fact]
        public void EnvironmentFixture_Dispose_ShouldRestorePreviouslyUnsetVariable()
        {
            // Arrange
            var name = "BC3_FIXTURE_TEST_" + Guid.NewGuid().ToString("N");
            Environment.GetEnvironmentVariable(name).Should().BeNull();

            // Act
            using (var fixture = new EnvironmentFixture())
            {
                fixture.Set(name, "value");
                Environment.GetEnvironmentVariable(name).Should().Be("value");
            }

            // Assert
            Environment.GetEnvironmentVariable(name).Should().BeNull("元々未設定だった変数は未設定へ戻る");
        }

        [Fact]
        public void EnvironmentFixture_Dispose_ShouldRestorePreviousValue()
        {
            // Arrange
            var name = "BC3_FIXTURE_TEST_" + Guid.NewGuid().ToString("N");
            Environment.SetEnvironmentVariable(name, "original");

            try
            {
                // Act
                using (var fixture = new EnvironmentFixture())
                {
                    fixture.Set(name, "changed");
                    fixture.Set(name, "changed-again");
                    Environment.GetEnvironmentVariable(name).Should().Be("changed-again");
                }

                // Assert
                // 複数回変更しても、戻るのは最初に記録した値
                Environment.GetEnvironmentVariable(name).Should().Be("original");
            }
            finally
            {
                Environment.SetEnvironmentVariable(name, null);
            }
        }

        [Fact]
        public void TempDirectoryFixture_ShouldCreateUniqueDirectoryPerInstance()
        {
            // Act
            using var first = new TempDirectoryFixture();
            using var second = new TempDirectoryFixture();

            // Assert
            first.Path.Should().NotBe(second.Path, "固定パス衝突を避けるのがこのフィクスチャの目的");
            Directory.Exists(first.Path).Should().BeTrue();
            Directory.Exists(second.Path).Should().BeTrue();
        }

        [Fact]
        public void TempDirectoryFixture_Dispose_ShouldDeleteDirectoryAndContents()
        {
            // Arrange
            string path;
            string filePath;
            using (var fixture = new TempDirectoryFixture())
            {
                path = fixture.Path;
                filePath = fixture.WriteFile("sample.txt", "contents");
                File.Exists(filePath).Should().BeTrue();
            }

            // Assert
            Directory.Exists(path).Should().BeFalse();
        }

        [Fact]
        public void TempDirectoryFixture_RedirectConfigDirectory_ShouldRouteConfigPathAndRestore()
        {
            // Arrange
            var originalConfigDir = PathManager.GetConfigDirectory();

            // Act
            using (var fixture = new TempDirectoryFixture())
            {
                fixture.RedirectConfigDirectory();

                // Assert: 本番コードの設定パス解決が一時ディレクトリを指す
                PathManager.GetConfigDirectory().Should().Be(fixture.Path);
                PathManager.GetConfigFilePath("x.xml").Should().StartWith(fixture.Path);
            }

            // Assert: 解除されて元へ戻る
            PathManager.GetConfigDirectory().Should().Be(originalConfigDir);
        }
    }
}
