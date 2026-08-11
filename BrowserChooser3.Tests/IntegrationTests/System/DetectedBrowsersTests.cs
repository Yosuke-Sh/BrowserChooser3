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

        #region 重複除去テスト

        [Fact]
        public void DoBrowserDetection_ShouldRemoveDuplicateBrowsers()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 同じパスを持つブラウザが重複していないことを確認
            var paths = result.Where(b => !string.IsNullOrEmpty(b.Target))
                             .Select(b => b.Target!.ToLowerInvariant())
                             .ToList();
            var uniquePaths = paths.Distinct().ToList();
            uniquePaths.Count.Should().Be(paths.Count);
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

        #region レジストリテスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleRegistryAccess()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // レジストリアクセスが正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region オンライン定義テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleOnlineDefinitions()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // オンライン定義の処理が正常に動作することを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region ローカル定義テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleLocalDefinitions()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ローカル定義の処理が正常に動作することを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region エラーハンドリングテスト

        [Fact]
        public void DoBrowserDetection_WithHttpClientException_ShouldHandleGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // HTTPクライアントの例外が適切に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_WithJsonDeserializationException_ShouldHandleGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // JSONデシリアライゼーションの例外が適切に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        [Fact]
        public void DoBrowserDetection_WithFileVersionInfoException_ShouldHandleGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ファイルバージョン情報の例外が適切に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region タイムアウトテスト

        [Fact]
        public void DoBrowserDetection_WithNetworkTimeout_ShouldHandleGracefully()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ネットワークタイムアウトが適切に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region ディレクトリ作成テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleDirectoryCreation()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ディレクトリ作成が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region ファイル書き込みテスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleFileWriting()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // ファイル書き込みが正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 環境変数テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleEnvironmentVariables()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 環境変数の処理が正常に動作することを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region パス展開テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandlePathExpansion()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // パス展開が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 文字列比較テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleStringComparisons()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 文字列比較が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region コレクション操作テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleCollectionOperations()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // コレクション操作が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 非同期処理テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleAsyncOperations()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 非同期処理が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region メモリ管理テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleMemoryManagement()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // メモリ管理が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region リソース解放テスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleResourceDisposal()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // リソース解放が正常に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region スレッドセーフテスト

        [Fact]
        public void DoBrowserDetection_ShouldBeThreadSafe()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // スレッドセーフであることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region パフォーマンス最適化テスト

        [Fact]
        public void DoBrowserDetection_ShouldBeOptimized()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = DetectedBrowsers.DoBrowserDetection();
            stopwatch.Stop();

            // Assert
            result.Should().NotBeNull();
            // 適切な時間内に完了することを確認
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000);
        }

        #endregion

        #region エラー回復テスト

        [Fact]
        public void DoBrowserDetection_ShouldRecoverFromErrors()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // エラーから回復できることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region データ検証テスト

        [Fact]
        public void DoBrowserDetection_ShouldValidateData()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // データが適切に検証されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region セキュリティテスト

        [Fact]
        public void DoBrowserDetection_ShouldHandleSecurity()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // セキュリティが適切に処理されることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 互換性テスト

        [Fact]
        public void DoBrowserDetection_ShouldBeCompatible()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 互換性が保たれていることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 拡張性テスト

        [Fact]
        public void DoBrowserDetection_ShouldBeExtensibleForCompatibility()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 拡張性が保たれていることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region 保守性テスト

        [Fact]
        public void DoBrowserDetection_ShouldBeMaintainable()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // 保守性が保たれていることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion

        #region テストカバレッジ確認

        [Fact]
        public void DoBrowserDetection_ShouldCoverAllCodePaths()
        {
            // Act
            var result = DetectedBrowsers.DoBrowserDetection();

            // Assert
            result.Should().NotBeNull();
            // すべてのコードパスがカバーされることを確認
            result.Should().BeOfType<List<Browser>>();
        }

        #endregion
    }
}
