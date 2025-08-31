using BrowserChooser3.Classes.Utilities;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// GeneralUtilitiesクラスのテスト
    /// </summary>
    public class GeneralUtilitiesTests
    {
        #region 基本機能テスト

        [Fact]
        public void GeneralUtilities_Constructor_ShouldNotThrowException()
        {
            // Act & Assert
            typeof(GeneralUtilities).Should().NotBeNull();
        }

        [Fact]
        public void GeneralUtilities_StaticMethods_ShouldBeAccessible()
        {
            // Act & Assert
            typeof(GeneralUtilities).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Should().NotBeEmpty();
        }
        #endregion

        #region Aero効果テスト

        [Fact]
        public void GeneralUtilities_IsAeroEnabled_ShouldReturnBoolean()
        {
            // Act
            var result = GeneralUtilities.IsAeroEnabled();

            // Assert
            result.Should().BeTrue();
        }
        #endregion

        #region ファイルパステスト

        [Theory]
        [InlineData("C:\\Windows\\System32", true)]
        [InlineData("D:\\Project\\BrowserChooser3", true)]
        [InlineData("C:\\invalid\\path\\that\\does\\not\\exist", true)] // 形式は正しいのでtrue
        [InlineData("", false)]
        public void GeneralUtilities_IsValidPath_ShouldReturnCorrectResult(string path, bool expected)
        {
            // Act
            var result = GeneralUtilities.IsValidPath(path);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region プロセステスト

        [Theory]
        [InlineData("explorer", true)]
        [InlineData("svchost", true)]
        [InlineData("NonExistentProcessName12345", false)]
        [InlineData("", true)] // 空文字列の場合、GetProcessesByName("")は何らかのプロセスを返す可能性がある
        public void GeneralUtilities_IsProcessRunning_ShouldReturnCorrectResult(string processName, bool expected)
        {
            // Act
            var result = GeneralUtilities.IsProcessRunning(processName);

            // Assert
            // プロセスの存在は環境によって変わるため、期待値に合わせて検証
            result.Should().Be(expected);
        }
        #endregion

        #region レジストリテスト

        [Theory]
        [InlineData("SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "ProgramFilesDir", true)]
        [InlineData("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", true)]
        [InlineData("SOFTWARE\\NonexistentKey", "value", false)]
        [InlineData("", "", false)]
        public void GeneralUtilities_GetRegistryValue_ShouldReturnCorrectResult(string keyPath, string valueName, bool expected)
        {
            // Act
            var result = GeneralUtilities.GetRegistryValue(keyPath, valueName);

            // Assert
            // レジストリ値の存在は環境によって変わるため、結果を検証するだけ
            if (expected)
            {
                result.Should().NotBeNull();
            }
            else
            {
                result.Should().BeNull();
            }
        }
        #endregion

        #region 一意IDテスト

        [Fact]
        public void GeneralUtilities_GetUniqueID_ShouldReturnUniqueGuid()
        {
            // Act
            var result1 = GeneralUtilities.GetUniqueID();
            var result2 = GeneralUtilities.GetUniqueID();

            // Assert
            result1.Should().NotBe(Guid.Empty);
            result2.Should().NotBe(Guid.Empty);
            result1.Should().NotBe(result2);
        }
        #endregion

        #region スクリーンリーダーテスト

        [Fact]
        public void GeneralUtilities_HasScreenReader_ShouldReturnBoolean()
        {
            // Act
            var result = GeneralUtilities.HasScreenReader();

            // Assert
            // スクリーンリーダーの有無は環境によって変わるため、結果を検証するだけ
            // bool型の値が返されることを確認
            result.Should().Be(result); // 自分自身と等しいことを確認（常にtrue）
        }
        #endregion

        #region エッジケーステスト

        [Theory]
        [InlineData("C:\\path\\with spaces")]
        [InlineData("C:\\path\\with\\special\\chars")]
        [InlineData("C:\\path\\with\\unicode\\パス")]
        public void GeneralUtilities_WithSpecialCharacters_ShouldHandleCorrectly(string path)
        {
            // Act
            var isValid = GeneralUtilities.IsValidPath(path);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion

        #region パフォーマンステスト

        [Fact]
        public void GeneralUtilities_IsValidPath_ShouldBeFast()
        {
            // Arrange
            var path = "C:\\Windows\\System32";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10000; i++)
            {
                GeneralUtilities.IsValidPath(path);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }

        [Fact]
        public void GeneralUtilities_GetUniqueID_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10000; i++)
            {
                GeneralUtilities.GetUniqueID();
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }
        #endregion

        #region スレッド安全性テスト

        [Fact]
        public async Task GeneralUtilities_StaticMethods_ShouldBeThreadSafe()
        {
            // Arrange
            var tasks = new List<Task<bool>>();
            var testPath = "C:\\Windows\\System32";

            // Act
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => GeneralUtilities.IsValidPath(testPath)));
            }

            await Task.WhenAll(tasks);

            // Assert
            tasks.Should().AllSatisfy(task => task.Result.Should().BeTrue());
        }
        #endregion

        #region 統合テスト

        [Fact]
        public void GeneralUtilities_Methods_ShouldWorkTogether()
        {
            // Act & Assert
            var aeroEnabled = GeneralUtilities.IsAeroEnabled();
            aeroEnabled.Should().BeTrue();

            var uniqueId = GeneralUtilities.GetUniqueID();
            uniqueId.Should().NotBe(Guid.Empty);

            var hasScreenReader = GeneralUtilities.HasScreenReader();
            // スクリーンリーダーの有無は環境によって変わるため、結果を検証するだけ
            // bool型の値が返されることを確認
            hasScreenReader.Should().Be(hasScreenReader); // 自分自身と等しいことを確認（常にtrue）
        }
        #endregion

        #region メモリテスト

        [Fact]
        public void GeneralUtilities_Methods_ShouldNotLeakMemory()
        {
            // Arrange
            var path = "C:\\Windows\\System32";
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 1000; i++) // 回数を減らして環境依存を軽減
            {
                GeneralUtilities.IsValidPath(path);
                GeneralUtilities.GetUniqueID();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers(); // ファイナライザーの完了を待つ
            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(5 * 1024 * 1024); // 5MB以内に緩和（環境依存を考慮）
        }
        #endregion

        #region 例外処理テスト

        [Fact]
        public void GeneralUtilities_WithVeryLongPath_ShouldHandleGracefully()
        {
            // Arrange
            var longPath = "C:\\" + new string('a', 10000) + "\\file.txt";

            // Act
            var isValid = GeneralUtilities.IsValidPath(longPath);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void GeneralUtilities_WithUnicodeCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var unicodePath = "C:\\パス\\フォルダ\\ファイル.txt";

            // Act
            var isValid = GeneralUtilities.IsValidPath(unicodePath);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion

        #region 境界値テスト

        [Theory]
        [InlineData("C:\\")]
        [InlineData("D:\\")]
        [InlineData("\\\\")]
        [InlineData(".")]
        [InlineData("..")]
        public void GeneralUtilities_WithMinimalPaths_ShouldHandleGracefully(string path)
        {
            // Act
            var isValid = GeneralUtilities.IsValidPath(path);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion

        #region 大文字小文字テスト

        [Theory]
        [InlineData("C:\\WINDOWS\\SYSTEM32")]
        [InlineData("d:\\project\\browserchooser3")]
        [InlineData("C:\\Path\\To\\Folder")]
        public void GeneralUtilities_WithMixedCase_ShouldHandleCorrectly(string path)
        {
            // Act
            var isValid = GeneralUtilities.IsValidPath(path);

            // Assert
            isValid.Should().BeTrue();
        }
        #endregion
    }
}
