using FluentAssertions;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Forms;
using BrowserChooser3.Classes;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsForm関連のハンドラークラスのテスト
    /// </summary>
    public class OptionsFormHandlersTests : IDisposable
    {
        private Settings _testSettings;
        private bool _isDisposed = false;

        public OptionsFormHandlersTests()
        {
            _testSettings = new Settings(false);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _testSettings = null!;
                _isDisposed = true;
            }
        }

        [Fact]
        public void OptionsFormCheckBoxHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var setModifiedCalled = false;
            Action<bool> setModified = (modified) => setModifiedCalled = modified;

            var handlers = new OptionsFormCheckBoxHandlers(form, _testSettings, setModified);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormCheckBoxHandlers_DetectDirty_ShouldCallSetModified()
        {
            // Arrange
            using var form = new OptionsForm();
            var setModifiedCalled = false;
            Action<bool> setModified = (modified) => setModifiedCalled = modified;

            var handlers = new OptionsFormCheckBoxHandlers(form, _testSettings, setModified);

            // Act
            handlers.DetectDirty(this, EventArgs.Empty);

            // Assert
            setModifiedCalled.Should().BeTrue();
        }

        [Fact]
        public void OptionsFormCheckBoxHandlers_ChkCanonicalize_CheckedChanged_ShouldEnableTextBox()
        {
            // Arrange
            using var form = new OptionsForm();
            var setModifiedCalled = false;
            Action<bool> setModified = (modified) => setModifiedCalled = modified;

            var handlers = new OptionsFormCheckBoxHandlers(form, _testSettings, setModified);
            var checkBox = new CheckBox { Checked = true };

            // Act
            handlers.ChkCanonicalize_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            setModifiedCalled.Should().BeTrue();
        }

        [Fact]
        public void OptionsFormCheckBoxHandlers_ChkLog_CheckedChanged_ShouldUpdateSettings()
        {
            // Arrange
            using var form = new OptionsForm();
            var setModifiedCalled = false;
            Action<bool> setModified = (modified) => setModifiedCalled = modified;

            var handlers = new OptionsFormCheckBoxHandlers(form, _testSettings, setModified);
            var checkBox = new CheckBox { Checked = true };

            // Act
            handlers.ChkLog_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            _testSettings.EnableLogging.Should().BeTrue();
            setModifiedCalled.Should().BeTrue();
        }

        [Fact]
        public void OptionsFormHelpHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var handlers = new OptionsFormHelpHandlers(form);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormHelpHandlers_OpenHelp_ShouldNotThrowException()
        {
            // Arrange
            using var form = new OptionsForm();
            var handlers = new OptionsFormHelpHandlers(form);

            // Act & Assert
            var action = () => handlers.OpenHelp();
            action.Should().NotThrow();
        }

        [Fact]
        public void OptionsFormFormHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            Action loadSettings = () => { };
            Action saveSettings = () => { };
            Func<bool> getIsModified = () => false;
            var handlers = new OptionsFormFormHandlers(form, loadSettings, saveSettings, getIsModified);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormFormHandlers_HelpButton_Click_ShouldNotThrowException()
        {
            // Arrange
            using var form = new OptionsForm();
            Action loadSettings = () => { };
            Action saveSettings = () => { };
            Func<bool> getIsModified = () => false;
            var handlers = new OptionsFormFormHandlers(form, loadSettings, saveSettings, getIsModified);

            // Act & Assert
            var action = () => handlers.HelpButton_Click(this, EventArgs.Empty);
            action.Should().NotThrow();
        }

        [Fact]
        public void OptionsFormFormHandlers_OptionsForm_FormClosing_ShouldNotThrowException()
        {
            // Arrange
            using var form = new OptionsForm();
            Action loadSettings = () => { };
            Action saveSettings = () => { };
            Func<bool> getIsModified = () => false;
            var handlers = new OptionsFormFormHandlers(form, loadSettings, saveSettings, getIsModified);
            var closingArgs = new FormClosingEventArgs(CloseReason.UserClosing, false);

            // Act & Assert
            var action = () => handlers.OptionsForm_FormClosing(this, closingArgs);
            action.Should().NotThrow();
        }



        [Fact]
        public void OptionsFormBrowserHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var handlers = new OptionsFormBrowserHandlers(form);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormProtocolHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var handlers = new OptionsFormProtocolHandlers(form);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormListHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var handlers = new OptionsFormListHandlers(form);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OptionsFormDragDropHandlers_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var form = new OptionsForm();
            var handlers = new OptionsFormDragDropHandlers(form);

            // Assert
            handlers.Should().NotBeNull();
        }
    }
}
