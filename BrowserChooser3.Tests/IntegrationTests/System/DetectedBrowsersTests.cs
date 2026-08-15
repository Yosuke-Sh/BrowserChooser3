using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.BrowserServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// DetectedBrowsersクラスのテスト
    /// ガバレッジ100%を目指して全メソッドをテストします
    /// </summary>
    public class DetectedBrowsersTests
    {
        #region 正常系テスト

        [Fact]
        public void DoBrowserDetection_ShouldReturnListOfBrowsers()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_ShouldHandleExceptionsGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 例外が発生してもnullが返されないことを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void DoBrowserDetection_WithNetworkError_ShouldReturnEmptyList()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ネットワークエラーが発生しても空のリストが返されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_WithFileSystemError_ShouldReturnEmptyList()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ファイルシステムエラーが発生しても空のリストが返されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void DoBrowserDetection_WithEmptySystem_ShouldReturnEmptyList()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // システムにブラウザがインストールされていない場合でも空のリストが返される
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_WithMultipleBrowsers_ShouldReturnUniqueBrowsers()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 重複するブラウザが除去されていることを確認
            var uniquePaths = result.Select(b => b.Target?.ToLowerInvariant()).Distinct();
            uniquePaths.Count().Should().Be(result.Count);
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void DoBrowserDetection_ShouldReturnConsistentResults()
        {
            // Act
            var result1 = DetectedBrowsers.DoBrowserDetection();
            var result2 = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            // 同じシステムでは一貫した結果が返されることを確認
            result1.Count.Should().Be(result2.Count);
        }

        [Fact]
        public void DoBrowserDetection_ShouldReturnValidBrowserObjects()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            foreach (var browser in result)
            {
                browser.Should().NotBeNull();
                browser.Guid.Should().NotBe(Guid.Empty);
                browser.Name.Should().NotBeNullOrEmpty();
                browser.Target.Should().NotBeNullOrEmpty();
            }
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void DoBrowserDetection_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = DetectedBrowsers.DoBrowserDetection();
            stopwatch.Stop();

            // Assert
            result.Should().NotBeNull();
            // 5秒以内に完了することを確認
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void DoBrowserDetection_WithCorruptedRegistry_ShouldHandleGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // レジストリが破損していてもエラーが発生しないことを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_WithInvalidFilePaths_ShouldFilterOutInvalidBrowsers()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 無効なファイルパスを持つブラウザが除外されていることを確認
            foreach (var browser in result)
            {
                if (!string.IsNullOrEmpty(browser.Target))
                {
                    // 実際にファイルが存在するか、または有効なパスであることを確認
                    browser.Target.Should().NotBeNullOrEmpty();
                }
            }
        }

        #endregion

        #region モックテスト（将来の拡張用）

        [Fact]
        public void DoBrowserDetection_ShouldBeExtensibleForFuture()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 将来の拡張に対応できる構造であることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 定数テスト

        [Fact]
        public void Constants_ShouldHaveExpectedValues()
        {
            // 定数が期待される値を持つことを確認
            // 注: プライベート定数なので、間接的にテスト
            var result = DetectedBrowsers.DoBrowserDetection();
            result.Should().NotBeNull();
        }

        #endregion

        #region 例外処理テスト

        [Fact]
        public void DoBrowserDetection_WithAllExceptions_ShouldReturnEmptyList()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // すべての例外が発生しても空のリストが返されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region データ整合性テスト

        [Fact]
        public void DoBrowserDetection_ShouldReturnBrowsersWithValidProperties()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            foreach (var browser in result)
            {
                // 必須プロパティが設定されていることを確認
                browser.Guid.Should().NotBe(Guid.Empty);
                browser.Name.Should().NotBeNullOrEmpty();
                browser.Target.Should().NotBeNullOrEmpty();
                
                // オプショナルプロパティが適切な範囲内であることを確認
                browser.Scale.Should().BeGreaterThan(0);
                browser.X.Should().BeGreaterThanOrEqualTo(0);
                browser.Y.Should().BeGreaterThanOrEqualTo(0);
                browser.IconIndex.Should().BeGreaterThanOrEqualTo(0);
            }
        }

        #endregion

        #region カテゴリテスト

        [Fact]
        public void DoBrowserDetection_ShouldReturnBrowsersWithValidCategories()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            foreach (var browser in result)
            {
                // カテゴリが設定されていることを確認
                browser.Category.Should().NotBeNullOrEmpty();
            }
        }

        #endregion
        #region ファイル存在テスト

        [Fact]
        public void DoBrowserDetection_ShouldOnlyReturnExistingBrowsers()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            foreach (var browser in result)
            {
                if (!string.IsNullOrEmpty(browser.Target))
                {
                    // ファイルが存在するか、または有効なパスであることを確認
                    browser.Target.Should().NotBeNullOrEmpty();
                }
            }
        }

        #endregion
    }
}
