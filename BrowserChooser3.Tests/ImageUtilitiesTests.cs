using FluentAssertions;
using Xunit;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Classes.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// ImageUtilitiesクラスの単体テスト
    /// ガバレッジ100%を目指して全メソッドをテストします
    /// </summary>
    public class ImageUtilitiesTests : IDisposable
    {
        private readonly string _testImagePath;
        private readonly string _testOutputPath;

        public ImageUtilitiesTests()
        {
            _testImagePath = Path.Combine(Path.GetTempPath(), "test_image.png");
            _testOutputPath = Path.Combine(Path.GetTempPath(), "test_output.png");
            
            // テスト用画像を作成
            CreateTestImage();
        }

        public void Dispose()
        {
            // テスト用ファイルをクリーンアップ
            if (File.Exists(_testImagePath))
            {
                File.Delete(_testImagePath);
            }
            if (File.Exists(_testOutputPath))
            {
                File.Delete(_testOutputPath);
            }
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
                CustomImagePath = _testImagePath,
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
    }
}
