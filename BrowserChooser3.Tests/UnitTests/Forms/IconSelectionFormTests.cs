using BrowserChooser3.Forms;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests.UnitTests.Forms
{
    /// <summary>
    /// IconSelectionFormのテスト
    /// </summary>
    public class IconSelectionFormTests
    {
        [Fact]
        public void Constructor_WithValidPath_ShouldInitializeCorrectly()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);

            // Assert
            form.Should().NotBeNull();
            form.Text.Should().Be("Icon Selection");
            form.Size.Should().Be(new Size(700, 550));
        }

        [Fact]
        public void SelectedIconPath_ShouldReturnCurrentFilePath()
        {
            // Arrange
            var testPath = "test.exe";
            var form = new IconSelectionForm(testPath);

            // Act
            var selectedPath = form.SelectedIconPath;

            // Assert
            selectedPath.Should().Be(testPath);
        }

        [Fact]
        public void SelectedIconIndex_Initially_ShouldBeNegativeOne()
        {
            // Arrange
            var testPath = "test.exe";
            var form = new IconSelectionForm(testPath);

            // Act
            var selectedIndex = form.SelectedIconIndex;

            // Assert
            selectedIndex.Should().Be(-1);
        }

        [Fact]
        public void SelectedIcon_Initially_ShouldBeNull()
        {
            // Arrange
            var testPath = "test.exe";
            var form = new IconSelectionForm(testPath);

            // Act
            var selectedIcon = form.SelectedIcon;

            // Assert
            selectedIcon.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithNullPath_ShouldHandleGracefully()
        {
            // Arrange & Act
            var action = () => new IconSelectionForm(null!);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithEmptyPath_ShouldHandleGracefully()
        {
            // Arrange & Act
            var action = () => new IconSelectionForm("");

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithNonExistentPath_ShouldHandleGracefully()
        {
            // Arrange
            var nonExistentPath = "non_existent_file.exe";

            // Act
            var action = () => new IconSelectionForm(nonExistentPath);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithInvalidPath_ShouldHandleGracefully()
        {
            // Arrange
            var invalidPath = "invalid:path\\file.exe";

            // Act
            var action = () => new IconSelectionForm(invalidPath);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithDifferentFileExtensions_ShouldHandleGracefully()
        {
            // Arrange
            var extensions = new[] { ".exe", ".ico", ".png", ".jpg", ".jpeg", ".bmp", ".txt", ".doc" };

            // Act & Assert
            foreach (var extension in extensions)
            {
                var path = "test" + extension;
                var action = () => new IconSelectionForm(path);
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Constructor_WithMultipleInstances_ShouldNotInterfere()
        {
            // Arrange
            var path1 = "test1.exe";
            var path2 = "test2.exe";

            // Act
            var form1 = new IconSelectionForm(path1);
            var form2 = new IconSelectionForm(path2);

            // Assert
            form1.SelectedIconPath.Should().Be(path1);
            form2.SelectedIconPath.Should().Be(path2);
        }

        [Fact]
        public void Constructor_ShouldSetFormPropertiesCorrectly()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);

            // Assert
            form.FormBorderStyle.Should().Be(FormBorderStyle.FixedDialog);
            form.MaximizeBox.Should().BeFalse();
            form.MinimizeBox.Should().BeTrue();
            form.StartPosition.Should().Be(FormStartPosition.CenterParent);
            form.TopMost.Should().BeTrue();
        }

        [Fact]
        public void Constructor_ShouldAddRequiredControls()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);

            // Assert
            form.Controls.Count.Should().BeGreaterThan(0);
            form.Controls.Find("iconListView", true).Should().HaveCount(1);
            form.Controls.Find("previewPictureBox", true).Should().HaveCount(1);
            form.Controls.Find("filePathLabel", true).Should().HaveCount(1);
            form.Controls.Find("btnChangePath", true).Should().HaveCount(1);
            form.Controls.Find("iconIndexLabel", true).Should().HaveCount(1);
            form.Controls.Find("fileTypeLabel", true).Should().HaveCount(1);
            form.Controls.Find("btnOK", true).Should().HaveCount(1);
            form.Controls.Find("btnCancel", true).Should().HaveCount(1);
        }

        [Fact]
        public void Constructor_ShouldSetAcceptAndCancelButtons()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);

            // Assert
            form.AcceptButton.Should().NotBeNull();
            form.CancelButton.Should().NotBeNull();
            form.AcceptButton.Should().Be(form.Controls.Find("btnOK", true).FirstOrDefault());
            form.CancelButton.Should().Be(form.Controls.Find("btnCancel", true).FirstOrDefault());
        }

        [Fact]
        public void Constructor_ShouldSetListViewProperties()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);
            var listView = form.Controls.Find("iconListView", true).FirstOrDefault() as ListView;

            // Assert
            listView.Should().NotBeNull();
            listView!.View.Should().Be(View.LargeIcon);
            listView.MultiSelect.Should().BeFalse();
            listView.FullRowSelect.Should().BeTrue();
            listView.LargeImageList.Should().NotBeNull();
            listView.LargeImageList!.ImageSize.Should().Be(new Size(32, 32));
            listView.LargeImageList.ColorDepth.Should().Be(ColorDepth.Depth32Bit);
        }

        [Fact]
        public void Constructor_ShouldSetPictureBoxProperties()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);
            var pictureBox = form.Controls.Find("previewPictureBox", true).FirstOrDefault() as PictureBox;

            // Assert
            pictureBox.Should().NotBeNull();
            pictureBox!.SizeMode.Should().Be(PictureBoxSizeMode.Zoom);
            pictureBox.BorderStyle.Should().Be(BorderStyle.FixedSingle);
        }

        [Fact]
        public void Constructor_ShouldSetLabelTexts()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);
            var filePathLabel = form.Controls.Find("filePathLabel", true).FirstOrDefault() as Label;
            var iconIndexLabel = form.Controls.Find("iconIndexLabel", true).FirstOrDefault() as Label;
            var fileTypeLabel = form.Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;

            // Assert
            filePathLabel.Should().NotBeNull();
            filePathLabel!.Text.Should().Be($"File: {testPath}");
            iconIndexLabel.Should().NotBeNull();
            iconIndexLabel!.Text.Should().Be("Icon Index: -");
            fileTypeLabel.Should().NotBeNull();
            // ファイルタイプは自動的に設定されるため、空でないことを確認
            fileTypeLabel!.Text.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Constructor_ShouldSetButtonTexts()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);
            var btnChangePath = form.Controls.Find("btnChangePath", true).FirstOrDefault() as Button;
            var btnOK = form.Controls.Find("btnOK", true).FirstOrDefault() as Button;
            var btnCancel = form.Controls.Find("btnCancel", true).FirstOrDefault() as Button;

            // Assert
            btnChangePath.Should().NotBeNull();
            btnChangePath!.Text.Should().Be("Change Icon Path");
            btnOK.Should().NotBeNull();
            btnOK!.Text.Should().Be("OK");
            btnCancel.Should().NotBeNull();
            btnCancel!.Text.Should().Be("Cancel");
        }

        [Fact]
        public void Constructor_ShouldSetButtonProperties()
        {
            // Arrange
            var testPath = "test.exe";

            // Act
            var form = new IconSelectionForm(testPath);
            var btnOK = form.Controls.Find("btnOK", true).FirstOrDefault() as Button;
            var btnCancel = form.Controls.Find("btnCancel", true).FirstOrDefault() as Button;

            // Assert
            btnOK.Should().NotBeNull();
            btnOK!.DialogResult.Should().Be(DialogResult.OK);
            btnOK.Enabled.Should().BeFalse(); // 初期状態では無効
            btnCancel.Should().NotBeNull();
            btnCancel!.DialogResult.Should().Be(DialogResult.Cancel);
        }

        [Fact]
        public void Constructor_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var invalidPath = "invalid:path\\file.exe";

            // Act
            var action = () => new IconSelectionForm(invalidPath);

            // Assert
            action.Should().NotThrow();
        }
    }
}
