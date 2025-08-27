using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Forms;
using FluentAssertions;
using Moq;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormAccessibilityHandlersクラスのテスト
    /// </summary>
    public class OptionsFormAccessibilityHandlersTests : IDisposable
    {
        private OptionsFormAccessibilityHandlers _handlers;
        private Settings _settings;
        private Mock<Action<bool>> _setModifiedMock;
        private OptionsForm _form;

        public OptionsFormAccessibilityHandlersTests()
        {
            _settings = new Settings();
            _setModifiedMock = new Mock<Action<bool>>();
            _form = new OptionsForm(_settings);
            _handlers = new OptionsFormAccessibilityHandlers(_form, _settings, _setModifiedMock.Object);
        }

        public void Dispose()
        {
            _handlers = null;
            _settings = null;
            _setModifiedMock = null;
            _form?.Dispose();
        }

        #region コンストラクタテスト

        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var handlers = new OptionsFormAccessibilityHandlers(_form, _settings, _setModifiedMock.Object);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullSettings_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => new OptionsFormAccessibilityHandlers(_form, null, _setModifiedMock.Object);
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithNullSetModified_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => new OptionsFormAccessibilityHandlers(_form, _settings, null);
            action.Should().NotThrow();
        }

        #endregion

        #region OpenAccessibilitySettingsテスト

        [Fact]
        public void OpenAccessibilitySettings_ShouldOpenForm()
        {
            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            // AccessibilitySettingsFormが開かれることを確認
            _setModifiedMock.Verify(x => x(true), Times.Once);
        }

        [Fact]
        public void OpenAccessibilitySettings_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => _handlers.OpenAccessibilitySettings();
            action.Should().NotThrow();
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void OpenAccessibilitySettings_WithCustomSettings_ShouldUpdateSettings()
        {
            // Arrange
            _settings.ShowFocus = true;
            _settings.FocusBoxColor = Color.Red.ToArgb();

            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.ShowFocus.Should().BeTrue();
            _settings.FocusBoxColor.Should().Be(Color.Red.ToArgb());
        }

        [Fact]
        public void OpenAccessibilitySettings_ShouldCallSetModified()
        {
            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _setModifiedMock.Verify(x => x(true), Times.Once);
        }

        #endregion

        #region 異常系テスト

        [Fact]
        public void OpenAccessibilitySettings_WithNullSettings_ShouldNotThrowException()
        {
            // Arrange
            var handlers = new OptionsFormAccessibilityHandlers(_form, null, _setModifiedMock.Object);

            // Act & Assert
            var action = () => handlers.OpenAccessibilitySettings();
            action.Should().NotThrow();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void AccessibilityHandlers_ShouldWorkWithSettings()
        {
            // Arrange
            _settings.ShowFocus = true;
            _settings.FocusBoxColor = Color.Blue.ToArgb();

            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.ShowFocus.Should().BeTrue();
            _settings.FocusBoxColor.Should().Be(Color.Blue.ToArgb());
            _setModifiedMock.Verify(x => x(true), Times.Once);
        }

        [Fact]
        public void AccessibilityHandlers_ShouldHandleMultipleOperations()
        {
            // Act
            _handlers.OpenAccessibilitySettings();
            _handlers.OpenAccessibilitySettings();

            // Assert
            _setModifiedMock.Verify(x => x(true), Times.Exactly(2));
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void OpenAccessibilitySettings_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            _handlers.OpenAccessibilitySettings();
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒未満であることを確認
        }

        #endregion

        #region 設定値テスト

        [Fact]
        public void OpenAccessibilitySettings_WithShowFocusTrue_ShouldPreserveSetting()
        {
            // Arrange
            _settings.ShowFocus = true;

            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.ShowFocus.Should().BeTrue();
        }

        [Fact]
        public void OpenAccessibilitySettings_WithShowFocusFalse_ShouldPreserveSetting()
        {
            // Arrange
            _settings.ShowFocus = false;

            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.ShowFocus.Should().BeFalse();
        }

        [Fact]
        public void OpenAccessibilitySettings_WithCustomFocusBoxColor_ShouldPreserveSetting()
        {
            // Arrange
            var customColor = Color.Green;
            _settings.FocusBoxColor = customColor.ToArgb();

            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.FocusBoxColor.Should().Be(customColor.ToArgb());
        }

        #endregion

        #region エッジケーステスト

        [Fact]
        public void OpenAccessibilitySettings_WithDefaultSettings_ShouldWork()
        {
            // Act
            _handlers.OpenAccessibilitySettings();

            // Assert
            _settings.ShowFocus.Should().BeFalse(); // デフォルト値
            _setModifiedMock.Verify(x => x(true), Times.Once);
        }

        [Fact]
        public void OpenAccessibilitySettings_WithNullSetModified_ShouldNotThrowException()
        {
            // Arrange
            var handlers = new OptionsFormAccessibilityHandlers(_form, _settings, null);

            // Act & Assert
            var action = () => handlers.OpenAccessibilitySettings();
            action.Should().NotThrow();
        }

        #endregion
    }
}
