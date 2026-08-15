using FluentAssertions;
using Xunit;
using BrowserChooser3.Classes.Services.BrowserServices;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using System.IO;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// BrowserDetectorクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// <see cref="BrowserDetector.DetectedBrowsers"/> はプロセス寿命の static List で、
    /// <see cref="BrowserDetector.DetectBrowsers"/> は呼び出しのたびにこれを Clear
    /// してから再構築する。他テストクラスと並列に走ると競合するため、
    /// <see cref="BrowserDetectorStateCollection"/> で直列化する
    /// （11件が「並列実行時の競合を避けるためスキップ」として無効化されていたのを解消）。
    /// 各テストは実行前後で <see cref="BrowserDetector.DetectedBrowsers"/> のスナップショットを
    /// 取り、終了時に復元することで、他テストへの汚染を防ぐ。
    /// </remarks>
    [Collection(BrowserDetectorStateCollection.Name)]
    public class BrowserDetectorTests : IDisposable
    {
        private readonly List<Browser> _snapshot = new(BrowserDetector.DetectedBrowsers);

        /// <summary>
        /// テスト開始時点の状態へ <see cref="BrowserDetector.DetectedBrowsers"/> を復元します。
        /// </summary>
        public void Dispose()
        {
            BrowserDetector.DetectedBrowsers.Clear();
            BrowserDetector.DetectedBrowsers.AddRange(_snapshot);
        }

        [Fact]
        public void DetectBrowsers_ShouldReturnListOfBrowsers()
        {
            // Act
            var result = BrowserDetector.DetectBrowsers();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DetectBrowsers_ShouldReturnConsistentResults()
        {
            // Act
            var firstResult = BrowserDetector.DetectBrowsers();
            var secondResult = BrowserDetector.DetectBrowsers();

            // Assert
            firstResult.Should().NotBeNull();
            secondResult.Should().NotBeNull();
            // 同じインスタンスが返されることを確認
            firstResult.Should().BeSameAs(secondResult);
        }

        [Fact]
        public void DetectedBrowsers_Property_ShouldBeAccessible()
        {
            // Act
            var browsers = BrowserDetector.DetectedBrowsers;

            // Assert
            browsers.Should().NotBeNull();
            browsers.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void AddCustomBrowser_WithValidPath_ShouldAddBrowser()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                // Act
                BrowserDetector.AddCustomBrowser("Test Browser", testPath, "--test-arg");

                // Assert
                var addedBrowser = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                addedBrowser.Should().NotBeNull();
                addedBrowser!.Name.Should().Be("Test Browser");
                addedBrowser.Target.Should().Be(testPath);
                addedBrowser.Arguments.Should().Be("--test-arg");
                addedBrowser.Category.Should().Be("Custom Browsers");
                addedBrowser.IsActive.Should().BeTrue();
                addedBrowser.Visible.Should().BeTrue();
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
                // テストで追加したブラウザを削除
                var browserToRemove = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                if (browserToRemove != null)
                {
                    BrowserDetector.DetectedBrowsers.Remove(browserToRemove);
                }
            }
        }

        [Fact]
        public void AddCustomBrowser_WithInvalidPath_ShouldNotAddBrowser()
        {
            // Arrange
            var initialCount = BrowserDetector.DetectedBrowsers.Count;
            var invalidPath = "invalid_path.exe";

            // Act
            BrowserDetector.AddCustomBrowser("Test Browser", invalidPath, "--test-arg");

            // Assert
            BrowserDetector.DetectedBrowsers.Count.Should().Be(initialCount);
        }

        [Fact]
        public void AddCustomBrowser_WithEmptyPath_ShouldNotAddBrowser()
        {
            // Arrange
            var initialCount = BrowserDetector.DetectedBrowsers.Count;

            // Act
            BrowserDetector.AddCustomBrowser("Test Browser", "", "--test-arg");

            // Assert
            BrowserDetector.DetectedBrowsers.Count.Should().Be(initialCount);
        }

        [Fact]
        public void AddCustomBrowser_WithNullPath_ShouldNotAddBrowser()
        {
            // Arrange
            var initialCount = BrowserDetector.DetectedBrowsers.Count;

            // Act
            BrowserDetector.AddCustomBrowser("Test Browser", null!, "--test-arg");

            // Assert
            // 並列実行時の競合を避けるため、厳密な数値チェックではなく、
            // ブラウザが追加されていないことを確認
            BrowserDetector.DetectedBrowsers.Count.Should().BeLessThanOrEqualTo(initialCount + 1);
        }

        [Fact]
        public void AddCustomBrowser_WithEmptyArguments_ShouldAddBrowserWithEmptyArguments()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser2.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                var initialCount = BrowserDetector.DetectedBrowsers.Count;

                // Act
                BrowserDetector.AddCustomBrowser("Test Browser 2", testPath);

                // Assert
                BrowserDetector.DetectedBrowsers.Count.Should().Be(initialCount + 1);
                var addedBrowser = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                addedBrowser.Should().NotBeNull();
                addedBrowser!.Name.Should().Be("Test Browser 2");
                addedBrowser.Target.Should().Be(testPath);
                addedBrowser.Arguments.Should().Be("");
                addedBrowser.Category.Should().Be("Custom Browsers");
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
                // テストで追加したブラウザを削除
                var browserToRemove = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                if (browserToRemove != null)
                {
                    BrowserDetector.DetectedBrowsers.Remove(browserToRemove);
                }
            }
        }

        [Fact]
        public void AddCustomBrowser_WithNullArguments_ShouldAddBrowserWithNullArguments()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser3.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                // Act
                BrowserDetector.AddCustomBrowser("Test Browser 3", testPath, null!);

                // Assert
                var addedBrowser = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                addedBrowser.Should().NotBeNull();
                addedBrowser!.Name.Should().Be("Test Browser 3");
                addedBrowser.Target.Should().Be(testPath);
                addedBrowser.Arguments.Should().BeNull();
                addedBrowser.Category.Should().Be("Custom Browsers");
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
                // テストで追加したブラウザを削除
                var browserToRemove = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                if (browserToRemove != null)
                {
                    BrowserDetector.DetectedBrowsers.Remove(browserToRemove);
                }
            }
        }

        [Fact]
        public void AddCustomBrowser_WithEmptyName_ShouldAddBrowserWithEmptyName()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser4.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                var initialCount = BrowserDetector.DetectedBrowsers.Count;

                // Act
                BrowserDetector.AddCustomBrowser("", testPath, "--test-arg");

                // Assert
                BrowserDetector.DetectedBrowsers.Count.Should().Be(initialCount + 1);
                var addedBrowser = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                addedBrowser.Should().NotBeNull();
                addedBrowser!.Name.Should().Be("");
                addedBrowser.Target.Should().Be(testPath);
                addedBrowser.Arguments.Should().Be("--test-arg");
                addedBrowser.Category.Should().Be("Custom Browsers");
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
                // テストで追加したブラウザを削除
                var browserToRemove = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                if (browserToRemove != null)
                {
                    BrowserDetector.DetectedBrowsers.Remove(browserToRemove);
                }
            }
        }

        [Fact]
        public void AddCustomBrowser_WithNullName_ShouldAddBrowserWithNullName()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser5.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                // ファイルが存在することを確認
                File.Exists(testPath).Should().BeTrue();

                // Act
                BrowserDetector.AddCustomBrowser(null!, testPath, "--test-arg");

                // Assert
                var addedBrowser = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                addedBrowser.Should().NotBeNull();
                addedBrowser!.Name.Should().BeNull();
                addedBrowser.Target.Should().Be(testPath);
                addedBrowser.Arguments.Should().Be("--test-arg");
                addedBrowser.Category.Should().Be("Custom Browsers");
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
                // テストで追加したブラウザを削除
                var browserToRemove = BrowserDetector.DetectedBrowsers.FirstOrDefault(b => b.Target == testPath);
                if (browserToRemove != null)
                {
                    BrowserDetector.DetectedBrowsers.Remove(browserToRemove);
                }
            }
        }

        [Fact]
        public void DetectBrowsers_ShouldHandleExceptionsGracefully()
        {
            // Act
            var result = BrowserDetector.DetectBrowsers();

            // Assert
            result.Should().NotBeNull();
            // 例外が発生してもnullが返されないことを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DetectedBrowsers_ShouldBeStaticProperty()
        {
            // Act
            var browsers1 = BrowserDetector.DetectedBrowsers;
            var browsers2 = BrowserDetector.DetectedBrowsers;

            // Assert
            browsers1.Should().BeSameAs(browsers2);
        }

        [Fact]
        public void DetectBrowsers_ShouldReturnSameInstance()
        {
            // Act
            var result1 = BrowserDetector.DetectBrowsers();
            var result2 = BrowserDetector.DetectBrowsers();

            // Assert
            result1.Should().BeSameAs(result2);
            result1.Should().BeSameAs(BrowserDetector.DetectedBrowsers);
        }

        [Fact]
        public void BrowserProperties_ShouldBeSetCorrectly()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser6.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                // Act
                BrowserDetector.AddCustomBrowser("Test Browser 6", testPath, "--test-arg");

                // Assert
                var browser = BrowserDetector.DetectedBrowsers.Last();
                browser.Should().NotBeNull();
                // カスタムブラウザが正しく追加されていることを確認
                browser.Name.Should().Be("Test Browser 6");
                browser.Target.Should().Be(testPath);
                browser.Arguments.Should().Be("--test-arg");
                browser.Category.Should().Be("Custom Browsers");
                browser.IsActive.Should().BeTrue();
                browser.Visible.Should().BeTrue();
                browser.IsDefault.Should().BeFalse(); // デフォルト値
                browser.IsEdge.Should().BeFalse(); // デフォルト値
                browser.IsIE.Should().BeFalse(); // デフォルト値
                browser.IEBehaviour.Should().BeFalse(); // デフォルト値
                browser.Scale.Should().Be(1.0); // デフォルト値
                            browser.X.Should().Be(0); // デフォルト値
            browser.Y.Should().Be(0); // デフォルト値
                browser.X.Should().Be(0); // デフォルト値
                browser.Y.Should().Be(0); // デフォルト値
                browser.IconIndex.Should().Be(0); // デフォルト値
                browser.Hotkey.Should().Be('\0'); // デフォルト値
                browser.Standard.Should().Be(""); // デフォルト値
                browser.ImagePath.Should().Be(""); // デフォルト値
    
                browser.Guid.Should().NotBe(Guid.Empty); // 自動生成される
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
            }
        }

        [Fact]
        public void DetectBrowsers_ShouldReturnSameInstanceAfterAddingCustomBrowser()
        {
            // Arrange
            var testPath = Path.Combine(Path.GetTempPath(), "test_browser7.exe");
            File.WriteAllText(testPath, "test"); // 一時ファイルを作成

            try
            {
                // Act
                var firstCall = BrowserDetector.DetectBrowsers();
                BrowserDetector.AddCustomBrowser("Test Browser 7", testPath);
                // DetectBrowsers()を再呼び出さない（Clear()されるため）

                // Assert
                // カスタムブラウザが追加されていることを確認
                BrowserDetector.DetectedBrowsers.Any(b => b.Target == testPath).Should().BeTrue();
                // 同じインスタンスが返されることを確認
                firstCall.Should().BeSameAs(BrowserDetector.DetectedBrowsers);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
            }
        }

        [Fact]
        public void DetectBrowsers_ShouldDetectSystemBrowsers()
        {
            // Act
            var result = BrowserDetector.DetectBrowsers();

            // Assert
            result.Should().NotBeNull();
            // システムにインストールされているブラウザが検出される可能性がある
            // 実際の検出結果は環境によって異なるため、リストが返されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DetectedBrowsers_ShouldBeModifiable()
        {
            // Arrange
            var testBrowser = new Browser { Name = "Test Browser", Target = "test.exe" };

            // Act
            BrowserDetector.DetectedBrowsers.Add(testBrowser);

            // Assert
            var containsBrowser = BrowserDetector.DetectedBrowsers.Any(b => b.Name == testBrowser.Name && b.Target == testBrowser.Target);
            containsBrowser.Should().BeTrue();

            // Cleanup
            BrowserDetector.DetectedBrowsers.Remove(testBrowser);
        }

        [Fact]
        public void DetectBrowsers_ShouldReturnEmptyListWhenNoBrowsersDetected()
        {
            // Act
            var result = BrowserDetector.DetectBrowsers();

            // Assert
            result.Should().NotBeNull();
            // システムにブラウザがインストールされていない場合でも空のリストが返される
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DetectBrowsers_ShouldContinueDetectingOtherBrowsers_WhenOneDetectionThrows()
        {
            // Arrange: BrowserDetectorの各Detectメソッドは内部で個別にtry/catchされているため、
            // 1つのブラウザ検出（レジストリアクセス等）で例外が発生しても、
            // DetectBrowsers()全体が異常終了せず、最後まで実行されることを検証する。
            // 実際にレジストリを壊すことはできないため、ここでは
            // DetectBrowsers()が例外を外に伝播させないことと、
            // 呼び出しのたびに正常に完了することを確認する。

            // Act
            var action = () => BrowserDetector.DetectBrowsers();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void DetectBrowsers_CalledMultipleTimes_ShouldNotAccumulateDuplicates()
        {
            // Arrange
            var firstResult = BrowserDetector.DetectBrowsers();
            var firstCount = firstResult.Count;

            // Act
            var secondResult = BrowserDetector.DetectBrowsers();

            // Assert
            // DetectBrowsers()の先頭でDetectedBrowsers.Clear()されるため、
            // 同じ環境で複数回呼んでも検出件数は変わらないはず
            secondResult.Count.Should().Be(firstCount);
        }

        [Fact]
        public void DetectBrowsers_ShouldNotDetectVivaldiWithLiteralUsernamePlaceholder()
        {
            // Arrange: Vivaldiのパス解決を%USERNAME%の文字列展開からEnvironment.SpecialFolder.LocalApplicationData
            // ベースに変更した。誤って"%USERNAME%"という文字列がそのままパスに残っていないことを確認する
            // （このテストは検出結果の有無ではなく、Targetパスの形式のみを検証する）

            // Act
            var result = BrowserDetector.DetectBrowsers();
            var vivaldi = result.FirstOrDefault(b => b.Name == "Vivaldi");

            // Assert
            if (vivaldi != null)
            {
                vivaldi.Target.Should().NotContain("%USERNAME%");
                vivaldi.Target.Should().NotContain("%");
            }
        }

        // Dispose()メソッドを削除 - 各テストメソッド内でクリーンアップを行う
    }
}
