using BrowserChooser3.Classes.Services.UI;
using FluentAssertions;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// MessageBoxServiceクラスのテスト
    /// </summary>
    public class MessageBoxServiceTests
    {
        private readonly MessageBoxService _messageBoxService;

        public MessageBoxServiceTests()
        {
            _messageBoxService = new MessageBoxService();
        }

        [Fact]
        public void ShowInfo_WithValidText_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test info message";
            var caption = "Test Info";

            // Act
            var result = _messageBoxService.ShowInfo(text, caption);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowInfo_WithNullText_ShouldHandleGracefully()
        {
            // Arrange
            string? nullText = null;
            var caption = "Test Info";

            // Act
            var action = () => _messageBoxService.ShowInfo(nullText!, caption);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowInfo_WithNullCaption_ShouldHandleGracefully()
        {
            // Arrange
            var text = "Test info message";
            string? nullCaption = null;

            // Act
            var action = () => _messageBoxService.ShowInfo(text, nullCaption!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowWarning_WithValidText_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test warning message";
            var caption = "Test Warning";

            // Act
            var result = _messageBoxService.ShowWarning(text, caption);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowWarning_WithNullText_ShouldHandleGracefully()
        {
            // Arrange
            string? nullText = null;
            var caption = "Test Warning";

            // Act
            var action = () => _messageBoxService.ShowWarning(nullText!, caption);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowWarning_WithNullCaption_ShouldHandleGracefully()
        {
            // Arrange
            var text = "Test warning message";
            string? nullCaption = null;

            // Act
            var action = () => _messageBoxService.ShowWarning(text, nullCaption!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowError_WithValidText_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test error message";
            var caption = "Test Error";

            // Act
            var result = _messageBoxService.ShowError(text, caption);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowError_WithNullText_ShouldHandleGracefully()
        {
            // Arrange
            string? nullText = null;
            var caption = "Test Error";

            // Act
            var action = () => _messageBoxService.ShowError(nullText!, caption);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowError_WithNullCaption_ShouldHandleGracefully()
        {
            // Arrange
            var text = "Test error message";
            string? nullCaption = null;

            // Act
            var action = () => _messageBoxService.ShowError(text, nullCaption!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowQuestion_WithValidText_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test question message";
            var caption = "Test Question";

            // Act
            var result = _messageBoxService.ShowQuestion(text, caption);

            // Assert
            result.Should().Be(DialogResult.Yes, "テスト環境ではメッセージボックス表示をスキップしてYesを返すため");
        }

        [Fact]
        public void ShowQuestion_WithNullText_ShouldHandleGracefully()
        {
            // Arrange
            string? nullText = null;
            var caption = "Test Question";

            // Act
            var action = () => _messageBoxService.ShowQuestion(nullText!, caption);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowQuestion_WithNullCaption_ShouldHandleGracefully()
        {
            // Arrange
            var text = "Test question message";
            string? nullCaption = null;

            // Act
            var action = () => _messageBoxService.ShowQuestion(text, nullCaption!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowInfo_WithDefaultCaption_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test info message";

            // Act
            var result = _messageBoxService.ShowInfo(text);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowWarning_WithDefaultCaption_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test warning message";

            // Act
            var result = _messageBoxService.ShowWarning(text);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowError_WithDefaultCaption_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test error message";

            // Act
            var result = _messageBoxService.ShowError(text);

            // Assert
            result.Should().Be(DialogResult.OK, "テスト環境ではメッセージボックス表示をスキップしてOKを返すため");
        }

        [Fact]
        public void ShowQuestion_WithDefaultCaption_ShouldReturnDialogResult()
        {
            // Arrange
            var text = "Test question message";

            // Act
            var result = _messageBoxService.ShowQuestion(text);

            // Assert
            result.Should().Be(DialogResult.Yes, "テスト環境ではメッセージボックス表示をスキップしてYesを返すため");
        }

        [Fact]
        public void ShowInfo_WithEmptyText_ShouldHandleGracefully()
        {
            // Arrange
            var text = "";

            // Act
            var action = () => _messageBoxService.ShowInfo(text);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowWarning_WithEmptyText_ShouldHandleGracefully()
        {
            // Arrange
            var text = "";

            // Act
            var action = () => _messageBoxService.ShowWarning(text);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowError_WithEmptyText_ShouldHandleGracefully()
        {
            // Arrange
            var text = "";

            // Act
            var action = () => _messageBoxService.ShowError(text);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ShowQuestion_WithEmptyText_ShouldHandleGracefully()
        {
            // Arrange
            var text = "";

            // Act
            var action = () => _messageBoxService.ShowQuestion(text);

            // Assert
            action.Should().NotThrow();
        }
    }
}
