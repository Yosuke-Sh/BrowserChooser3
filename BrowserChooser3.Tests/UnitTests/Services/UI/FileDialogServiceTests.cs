using BrowserChooser3.Classes.Services.UI;
using FluentAssertions;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// FileDialogServiceクラスのテスト
    /// </summary>
    public class FileDialogServiceTests
    {
        private readonly FileDialogService _fileDialogService;

        public FileDialogServiceTests()
        {
            _fileDialogService = new FileDialogService();
        }

        [Fact]
        public void ShowOpenFileDialog_WithValidParameters_ShouldReturnFilePath()
        {
            // Arrange
            var title = "Test Open File";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Act
            var result = _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("test_file.txt", "テスト環境では固定のファイルパスを返すため");
        }

        [Fact]
        public void ShowOpenFileDialog_WithNullTitle_ShouldHandleGracefully()
        {
            // Arrange
            string? nullTitle = null;
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(nullTitle!, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowOpenFileDialog_WithNullFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Open File";
            string? nullFilter = null;

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, nullFilter!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowOpenFileDialog_WithEmptyTitle_ShouldHandleGracefully()
        {
            // Arrange
            var title = "";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowOpenFileDialog_WithEmptyFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Open File";
            var filter = "";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithValidParameters_ShouldReturnFilePath()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var defaultFileName = "test.txt";

            // Act
            var result = _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(defaultFileName, "テスト環境ではデフォルトファイル名を返すため");
        }

        [Fact]
        public void ShowSaveFileDialog_WithNullTitle_ShouldHandleGracefully()
        {
            // Arrange
            string? nullTitle = null;
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var defaultFileName = "test.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(nullTitle!, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithNullFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Save File";
            string? nullFilter = null;
            var defaultFileName = "test.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, nullFilter!, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithNullDefaultFileName_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            string? nullDefaultFileName = null;

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, nullDefaultFileName!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithEmptyTitle_ShouldHandleGracefully()
        {
            // Arrange
            var title = "";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var defaultFileName = "test.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithEmptyFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "";
            var defaultFileName = "test.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithEmptyDefaultFileName_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var defaultFileName = "";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithDefaultFileNameOnly_ShouldReturnFilePath()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Act
            var result = _fileDialogService.ShowSaveFileDialog(title, filter);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("test_save.txt", "テスト環境ではデフォルトのファイル名を返すため");
        }

        [Fact]
        public void ShowOpenFileDialog_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test & Open <File> \"Dialog\"";
            var filter = "Test files (*.test)|*.test|All files (*.*)|*.*";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test & Save <File> \"Dialog\"";
            var filter = "Test files (*.test)|*.test|All files (*.*)|*.*";
            var defaultFileName = "test & file <name>.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowOpenFileDialog_WithLongTitle_ShouldHandleGracefully()
        {
            // Arrange
            var title = new string('A', 1000); // 長いタイトル
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithLongTitle_ShouldHandleGracefully()
        {
            // Arrange
            var title = new string('A', 1000); // 長いタイトル
            var filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var defaultFileName = new string('B', 1000); // 長いファイル名

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowOpenFileDialog_WithComplexFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Open File";
            var filter = "Text files (*.txt)|*.txt|Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";

            // Act
            var action = () => _fileDialogService.ShowOpenFileDialog(title, filter);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowSaveFileDialog_WithComplexFilter_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test Save File";
            var filter = "Text files (*.txt)|*.txt|Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";
            var defaultFileName = "test.txt";

            // Act
            var action = () => _fileDialogService.ShowSaveFileDialog(title, filter, defaultFileName);

            // Assert
            action.Should().NotThrow();
        }
    }
}
