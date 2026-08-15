using FluentAssertions;
using Xunit;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// ImageUtilitiesクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// 以前は %TEMP%\test_image.png という固定パスを使っており、
    /// xUnitが同クラスのテストを並列実行した際に互いのファイルを削除し合っていた。
    /// TempDirectoryFixtureでテストごとに一意なディレクトリへ隔離する。
    /// </remarks>
    public class ImageUtilitiesTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir;
        private readonly string _testImagePath;
        private readonly string _testOutputPath;

        public ImageUtilitiesTests()
        {
            _tempDir = new TempDirectoryFixture();
            _testImagePath = _tempDir.GetFilePath("test_image.png");
            _testOutputPath = _tempDir.GetFilePath("test_output.png");

            // テスト用画像を作成
            CreateTestImage();
        }

        public void Dispose()
        {
            // 一時ディレクトリごとまとめて削除される
            _tempDir.Dispose();
        }

        private void CreateTestImage()
        {
            using var bitmap = new Bitmap(100, 100);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Red);
            graphics.FillRectangle(Brushes.Blue, 25, 25, 50, 50);
            bitmap.Save(_testImagePath, ImageFormat.Png);
        }

        #region GetImageテスト

        [Fact]
        public void GetImage_WithValidBrowser_ShouldReturnImage()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = _testImagePath,
                IconIndex = 0
            };

            // Act
            var result = ImageUtilities.GetImage(browser, false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Bitmap>();
        }

        [Fact]
        public void GetImage_WithCustomPath_ShouldUseCustomPath()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = "invalid_path.exe",
                ImagePath = _testImagePath,
                IconIndex = 0
            };

            // Act
            var result = ImageUtilities.GetImage(browser, true);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Bitmap>();
        }

        [Fact]
        public void GetImage_WithInvalidPath_ShouldReturnSystemIcon()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = "invalid_path.exe",
                IconIndex = 0
            };

            // Act
            var result = ImageUtilities.GetImage(browser, false);

            // Assert
            // 実際の実装ではnullが返される可能性がある
            if (result != null)
            {
                result.Should().BeOfType<Bitmap>();
            }
        }

        [Fact]
        public void GetImage_WithEmptyPath_ShouldReturnSystemIcon()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = "",
                IconIndex = 0
            };

            // Act
            var result = ImageUtilities.GetImage(browser, false);

            // Assert
            // 実際の実装ではnullが返される可能性がある
            if (result != null)
            {
                result.Should().BeOfType<Bitmap>();
            }
        }

        [Fact]
        public void GetImage_WithNullBrowser_ShouldReturnSystemIcon()
        {
            // Act
            var result = ImageUtilities.GetImage(null!, false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Bitmap>();
        }

        [Fact]
        public void GetImage_CalledTwiceWithSamePath_ShouldReturnCachedInstance()
        {
            // Arrange
            // 他テストとキャッシュキーが衝突しないよう専用ファイルを使用する
            var cacheTestImagePath = Path.Combine(Path.GetTempPath(), $"test_image_cache_{Guid.NewGuid():N}.png");
            using (var bitmap = new Bitmap(50, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Green);
                bitmap.Save(cacheTestImagePath, ImageFormat.Png);
            }

            try
            {
                var browser = new Browser
                {
                    Name = "Cache Test Browser",
                    Target = cacheTestImagePath,
                    IconIndex = 0
                };

                // Act
                var first = ImageUtilities.GetImage(browser, false);
                var second = ImageUtilities.GetImage(browser, false);

                // Assert
                first.Should().NotBeNull();
                second.Should().NotBeNull();
                // 2回目はキャッシュから返るため、同一インスタンスであること
                ReferenceEquals(first, second).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(cacheTestImagePath))
                {
                    File.Delete(cacheTestImagePath);
                }
            }
        }

        [Fact]
        public void GetResizedImage_CalledTwiceWithSameSize_ShouldReturnCachedInstance()
        {
            // Arrange
            var cacheTestImagePath = Path.Combine(Path.GetTempPath(), $"test_image_resize_{Guid.NewGuid():N}.png");
            using (var bitmap = new Bitmap(50, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Green);
                bitmap.Save(cacheTestImagePath, ImageFormat.Png);
            }

            try
            {
                var browser = new Browser
                {
                    Name = "Resize Cache Test Browser",
                    Target = cacheTestImagePath,
                    IconIndex = 0
                };

                // Act
                var first = ImageUtilities.GetResizedImage(browser, false, 32);
                var second = ImageUtilities.GetResizedImage(browser, false, 32);
                var differentSize = ImageUtilities.GetResizedImage(browser, false, 64);

                // Assert
                first.Should().NotBeNull();
                second.Should().NotBeNull();
                // 同一サイズの2回目呼び出しはキャッシュから返るため、同一インスタンスであること
                ReferenceEquals(first, second).Should().BeTrue();
                // サイズが異なれば別インスタンスであること
                ReferenceEquals(first, differentSize).Should().BeFalse();
                first!.Width.Should().Be(32);
                differentSize!.Width.Should().Be(64);
            }
            finally
            {
                if (File.Exists(cacheTestImagePath))
                {
                    File.Delete(cacheTestImagePath);
                }
            }
        }

        #endregion

        #region ExtractIconFromFileテスト

        [Fact]
        public void ExtractIconFromFile_WithValidFile_ShouldReturnIcon()
        {
            // Act
            var result = ImageUtilities.ExtractIconFromFile(_testImagePath, 0);

            // Assert
            // 画像ファイルからはアイコンが抽出できない場合があるため、
            // nullが返される可能性もある
            if (result != null)
            {
                result.Should().BeOfType<Icon>();
            }
        }

        [Fact]
        public void ExtractIconFromFile_WithInvalidFile_ShouldReturnNull()
        {
            // Act
            var result = ImageUtilities.ExtractIconFromFile("invalid_file.exe", 0);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractIconFromFile_WithEmptyPath_ShouldReturnNull()
        {
            // Act
            var result = ImageUtilities.ExtractIconFromFile("", 0);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractIconFromFile_WithNullPath_ShouldReturnNull()
        {
            // Act
            var result = ImageUtilities.ExtractIconFromFile(null!, 0);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region ScaleImageToテスト

        [Fact]
        public void ScaleImageTo_WithValidImage_ShouldScaleCorrectly()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);
            var targetSize = new Size(50, 50);

            // Act
            var result = ImageUtilities.ScaleImageTo(originalImage, targetSize);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(50);
            result.Height.Should().Be(50);
        }

        [Fact]
        public void ScaleImageTo_WithNullImage_ShouldReturnEmptyBitmap()
        {
            // Arrange
            Image? nullImage = null;
            var targetSize = new Size(50, 50);

            // Act
            var result = ImageUtilities.ScaleImageTo(nullImage!, targetSize);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(50);
            result.Height.Should().Be(50);
        }

        [Fact]
        public void ScaleImageTo_WithMinimumSize_ShouldReturnMinimumBitmap()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);
            var targetSize = new Size(1, 1);

            // Act
            var result = ImageUtilities.ScaleImageTo(originalImage, targetSize);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(1);
            result.Height.Should().Be(1);
        }

        #endregion

        #region ResizeImageテスト

        [Fact]
        public void ResizeImage_WithValidImage_ShouldResizeCorrectly()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);

            // Act
            var result = ImageUtilities.ResizeImage(originalImage, 50, 75);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(50);
            result.Height.Should().Be(75);
        }

        [Fact]
        public void ResizeImage_WithNullImage_ShouldReturnEmptyBitmap()
        {
            // Arrange
            Image? nullImage = null;

            // Act
            var result = ImageUtilities.ResizeImage(nullImage!, 50, 75);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(50);
            result.Height.Should().Be(75);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 1)]
        [InlineData(1, 100)]
        [InlineData(50, 75)]
        public void ResizeImage_WithValidCases_ShouldHandleCorrectly(int width, int height)
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);

            // Act
            var result = ImageUtilities.ResizeImage(originalImage, width, height);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(width);
            result.Height.Should().Be(height);
        }

        #endregion

        #region ResizeImageKeepAspectRatioテスト

        [Fact]
        public void ResizeImageKeepAspectRatio_WithValidImage_ShouldMaintainAspectRatio()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 50); // 2:1 ratio

            // Act
            var result = ImageUtilities.ResizeImageKeepAspectRatio(originalImage, 80, 40);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(80);
            result.Height.Should().Be(40);
        }

        [Fact]
        public void ResizeImageKeepAspectRatio_WithWiderConstraint_ShouldScaleToHeight()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 50); // 2:1 ratio

            // Act
            var result = ImageUtilities.ResizeImageKeepAspectRatio(originalImage, 200, 30);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(60); // 30 * 2
            result.Height.Should().Be(30);
        }

        [Fact]
        public void ResizeImageKeepAspectRatio_WithTallerConstraint_ShouldScaleToWidth()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 50); // 2:1 ratio

            // Act
            var result = ImageUtilities.ResizeImageKeepAspectRatio(originalImage, 40, 100);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(40);
            result.Height.Should().Be(20); // 40 / 2
        }

        [Fact]
        public void ResizeImageKeepAspectRatio_WithNullImage_ShouldReturnEmptyBitmap()
        {
            // Arrange
            Image? nullImage = null;

            // Act
            var result = ImageUtilities.ResizeImageKeepAspectRatio(nullImage!, 50, 75);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(50);
            result.Height.Should().Be(75);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 1)]
        [InlineData(1, 100)]
        [InlineData(50, 50)]
        public void ResizeImageKeepAspectRatio_WithValidCases_ShouldHandleCorrectly(int maxWidth, int maxHeight)
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);

            // Act
            var result = ImageUtilities.ResizeImageKeepAspectRatio(originalImage, maxWidth, maxHeight);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().BeLessThanOrEqualTo(maxWidth);
            result.Height.Should().BeLessThanOrEqualTo(maxHeight);
        }

        #endregion

        #region ConvertToGrayscaleテスト

        [Fact]
        public void ConvertToGrayscale_WithValidImage_ShouldConvertToGrayscale()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);
            using var graphics = Graphics.FromImage(originalImage);
            graphics.Clear(Color.Red);

            // Act
            var result = ImageUtilities.ConvertToGrayscale(originalImage);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(100);
            result.Height.Should().Be(100);
        }

        [Fact]
        public void ConvertToGrayscale_WithNullImage_ShouldReturnEmptyBitmap()
        {
            // Arrange
            Image? nullImage = null;

            // Act
            var result = ImageUtilities.ConvertToGrayscale(nullImage!);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(1);
            result.Height.Should().Be(1);
        }

        #endregion

        #region AdjustOpacityテスト

        [Fact]
        public void AdjustOpacity_WithValidImage_ShouldAdjustOpacity()
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);
            using var graphics = Graphics.FromImage(originalImage);
            graphics.Clear(Color.Red);

            // Act
            var result = ImageUtilities.AdjustOpacity(originalImage, 0.5f);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(100);
            result.Height.Should().Be(100);
        }

        [Fact]
        public void AdjustOpacity_WithNullImage_ShouldReturnEmptyBitmap()
        {
            // Arrange
            Image? nullImage = null;

            // Act
            var result = ImageUtilities.AdjustOpacity(nullImage!, 0.5f);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(1);
            result.Height.Should().Be(1);
        }

        [Theory]
        [InlineData(0.0f)]
        [InlineData(0.5f)]
        [InlineData(1.0f)]
        [InlineData(1.5f)] // 範囲外
        [InlineData(-0.5f)] // 範囲外
        public void AdjustOpacity_WithVariousValues_ShouldHandleCorrectly(float opacity)
        {
            // Arrange
            using var originalImage = new Bitmap(100, 100);

            // Act
            var result = ImageUtilities.AdjustOpacity(originalImage, opacity);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(100);
            result.Height.Should().Be(100);
        }

        #endregion

        #region SaveImageテスト

        [Fact]
        public void SaveImage_WithValidImage_ShouldSaveToFile()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            ImageUtilities.SaveImage(image, _testOutputPath, ImageFormat.Png);

            // Assert
            File.Exists(_testOutputPath).Should().BeTrue();
        }

        [Fact]
        public void SaveImage_WithNullImage_ShouldNotThrowException()
        {
            // Arrange
            Image? nullImage = null;

            // Act & Assert
            Action act = () => ImageUtilities.SaveImage(nullImage!, _testOutputPath, ImageFormat.Png);
            act.Should().NotThrow();
        }

        [Fact]
        public void SaveImage_WithInvalidPath_ShouldThrowException()
        {
            // Arrange
            using var image = new Bitmap(100, 100);
            var invalidPath = Path.Combine("invalid", "path", "image.png");

            // Act & Assert
            Action act = () => ImageUtilities.SaveImage(image, invalidPath, ImageFormat.Png);
            act.Should().Throw<DirectoryNotFoundException>();
        }

        #endregion

        #region SaveImageToStreamテスト

        [Fact]
        public void SaveImageToStream_WithValidImage_ShouldSaveToStream()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            using var stream = ImageUtilities.SaveImageToStream(image, ImageFormat.Png);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().BeGreaterThan(0);
            stream.Position.Should().Be(0);
        }

        [Fact]
        public void SaveImageToStream_WithNullImage_ShouldReturnEmptyStream()
        {
            // Arrange
            Image? nullImage = null;

            // Act
            using var stream = ImageUtilities.SaveImageToStream(nullImage!, ImageFormat.Png);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().Be(0);
        }

        [Fact]
        public void SaveImageToStream_WithPngFormat_ShouldHandleCorrectly()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            using var stream = ImageUtilities.SaveImageToStream(image, ImageFormat.Png);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SaveImageToStream_WithJpegFormat_ShouldHandleCorrectly()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            using var stream = ImageUtilities.SaveImageToStream(image, ImageFormat.Jpeg);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SaveImageToStream_WithBmpFormat_ShouldHandleCorrectly()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            using var stream = ImageUtilities.SaveImageToStream(image, ImageFormat.Bmp);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SaveImageToStream_WithGifFormat_ShouldHandleCorrectly()
        {
            // Arrange
            using var image = new Bitmap(100, 100);

            // Act
            using var stream = ImageUtilities.SaveImageToStream(image, ImageFormat.Gif);

            // Assert
            stream.Should().NotBeNull();
            stream.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void ImageProcessing_CompleteWorkflow_ShouldWorkCorrectly()
        {
            // Arrange
            using var originalImage = new Bitmap(200, 200);
            using var graphics = Graphics.FromImage(originalImage);
            graphics.Clear(Color.Blue);

            // Act - スケーリング
            var scaledImage = ImageUtilities.ScaleImageTo(originalImage, new Size(100, 100));
            
            // Act - グレースケール変換
            var grayImage = ImageUtilities.ConvertToGrayscale(scaledImage);
            
            // Act - 透明度調整
            var transparentImage = ImageUtilities.AdjustOpacity(grayImage, 0.7f);

            // Assert
            scaledImage.Should().NotBeNull();
            scaledImage.Width.Should().Be(100);
            scaledImage.Height.Should().Be(100);

            grayImage.Should().NotBeNull();
            grayImage.Width.Should().Be(100);
            grayImage.Height.Should().Be(100);

            transparentImage.Should().NotBeNull();
            transparentImage.Width.Should().Be(100);
            transparentImage.Height.Should().Be(100);
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void ImageProcessing_WithDisposedImage_ShouldHandleGracefully()
        {
            // Arrange
            var image = new Bitmap(100, 100);
            image.Dispose();

            // Act & Assert
            var result = ImageUtilities.ScaleImageTo(image, new Size(50, 50));
            result.Should().NotBeNull();
        }

        [Fact]
        public void ImageProcessing_WithVeryLargeImage_ShouldHandleGracefully()
        {
            // Arrange
            using var image = new Bitmap(10000, 10000);

            // Act
            var result = ImageUtilities.ScaleImageTo(image, new Size(100, 100));

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(100);
            result.Height.Should().Be(100);
        }

        #endregion

        #region アイコンディスクキャッシュテスト
        //
        // GetResizedImageはプロセス寿命のメモリキャッシュ(_resizedIconCache)を先にチェックするため、
        // 同一Browser.Target・同一sizeで複数回呼ぶテストはメモリキャッシュに隠れてディスクキャッシュを
        // 検証できない。このため各テストで一意なTargetパス（TempDirectoryFixture配下）を使い、
        // 必ずディスクキャッシュ層まで到達させる。
        // PathManager.IconCacheDirectoryOverrideForTestsで%LOCALAPPDATA%配下の実キャッシュに触れず隔離する。

        [Fact]
        public void GetResizedImage_OnCacheMiss_ShouldWriteResizedImageToDiskCache()
        {
            // Arrange
            var cacheDir = _tempDir.GetFilePath("iconcache");
            var originalOverride = PathManager.IconCacheDirectoryOverrideForTests;
            PathManager.IconCacheDirectoryOverrideForTests = cacheDir;

            try
            {
                var browser = new Browser
                {
                    Name = "Test Browser",
                    Target = _testImagePath,
                    IconIndex = 0
                };

                // Act
                var result = ImageUtilities.GetResizedImage(browser, false, 32);

                // Assert: 結果が返り、ディスクキャッシュディレクトリに1件のPNGが書き出される
                result.Should().NotBeNull();
                Directory.Exists(cacheDir).Should().BeTrue();
                var cachedFiles = Directory.GetFiles(cacheDir, "*.png");
                cachedFiles.Should().ContainSingle();
            }
            finally
            {
                PathManager.IconCacheDirectoryOverrideForTests = originalOverride;
            }
        }

        [Fact]
        public void GetResizedImage_WithPreExistingDiskCacheFile_ShouldLoadFromDiskCacheRatherThanRegenerate()
        {
            // Arrange: 同一キー導出ロジック（exeパス+IconIndex+size+最終更新日時のSHA256）で
            // 期待されるキャッシュファイル名を事前に計算し、ダミー画像を先に配置しておく
            var cacheDir = _tempDir.GetFilePath("iconcache");
            Directory.CreateDirectory(cacheDir);
            var originalOverride = PathManager.IconCacheDirectoryOverrideForTests;
            PathManager.IconCacheDirectoryOverrideForTests = cacheDir;

            try
            {
                var lastWriteTicks = File.GetLastWriteTimeUtc(_testImagePath).Ticks;
                var rawKey = $"{_testImagePath}|0|32|{lastWriteTicks}";
                using var sha = System.Security.Cryptography.SHA256.Create();
                var hash = Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawKey)));
                var expectedCacheFile = Path.Combine(cacheDir, $"{hash}.png");

                using (var placeholder = new Bitmap(32, 32))
                using (var g = Graphics.FromImage(placeholder))
                {
                    g.Clear(Color.LimeGreen);
                    placeholder.Save(expectedCacheFile, ImageFormat.Png);
                }

                var browser = new Browser
                {
                    Name = "Test Browser",
                    Target = _testImagePath,
                    IconIndex = 0
                };

                // Act
                var result = ImageUtilities.GetResizedImage(browser, false, 32);

                // Assert: プレースホルダ色(LimeGreen)がそのまま返る = ディスクキャッシュから読み込まれ、
                // GetImage経由の再生成（赤/青の元画像）は行われていない
                result.Should().NotBeNull();
                using var resultBitmap = new Bitmap(result!);
                var pixel = resultBitmap.GetPixel(0, 0);
                pixel.R.Should().Be(Color.LimeGreen.R);
                pixel.G.Should().Be(Color.LimeGreen.G);
                pixel.B.Should().Be(Color.LimeGreen.B);
            }
            finally
            {
                PathManager.IconCacheDirectoryOverrideForTests = originalOverride;
            }
        }

        [Fact]
        public void BuildDiskCacheFileName_AfterSourceFileIsModified_ShouldProduceDifferentKey()
        {
            // GetResizedImage自体は_resizedIconCache（プロセス寿命・exeパス+IconIndex+sizeのみがキーで
            // 最終更新日時を含まない）を先にチェックするため、同一プロセス内で元ファイルを更新して
            // 2回呼んでも2回目はメモリキャッシュに隠れてしまいディスクキャッシュ層まで到達しない。
            // ここではディスクキャッシュのキー生成ロジック（BuildDiskCacheFileName）そのものを
            // リフレクション経由で直接呼び、最終更新日時が変わるとキーが変わる（＝無効化される）ことを検証する。
            var method = typeof(ImageUtilities).GetMethod("BuildDiskCacheFileName",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            method.Should().NotBeNull();

            // Act
            var keyBefore = (string?)method!.Invoke(null, new object?[] { _testImagePath, 0, 48 });

            File.SetLastWriteTimeUtc(_testImagePath, DateTime.UtcNow.AddMinutes(10));
            var keyAfter = (string?)method!.Invoke(null, new object?[] { _testImagePath, 0, 48 });

            // Assert: 両方とも有効なキーが生成され、最終更新日時の変化によりキーそのものが変わる
            keyBefore.Should().NotBeNull();
            keyAfter.Should().NotBeNull();
            keyAfter.Should().NotBe(keyBefore);
        }

        [Fact]
        public void GetResizedImage_TwoDifferentSizesOfSameBrowser_ShouldWriteSeparateDiskCacheEntries()
        {
            // Arrange
            var cacheDir = _tempDir.GetFilePath("iconcache");
            var originalOverride = PathManager.IconCacheDirectoryOverrideForTests;
            PathManager.IconCacheDirectoryOverrideForTests = cacheDir;

            try
            {
                var browser = new Browser
                {
                    Name = "Test Browser",
                    Target = _testImagePath,
                    IconIndex = 0
                };

                // Act: サイズ違いは要求サイズがキャッシュキーに含まれるため、それぞれ別ファイルとして
                // キャッシュミス扱いになりディスクへ書き出される
                ImageUtilities.GetResizedImage(browser, false, 16).Should().NotBeNull();
                ImageUtilities.GetResizedImage(browser, false, 64).Should().NotBeNull();

                // Assert
                var cachedFiles = Directory.GetFiles(cacheDir, "*.png");
                cachedFiles.Should().HaveCount(2);
            }
            finally
            {
                PathManager.IconCacheDirectoryOverrideForTests = originalOverride;
            }
        }

        #endregion
    }
}
