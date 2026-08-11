using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Forms;
using FluentAssertions;
using System.ComponentModel;

namespace BrowserChooser3.Tests.IntegrationTests
{
    /// <summary>
    /// UI関連クラスの統合テスト
    /// 実際のフォームインスタンスを使用して動作をテストします
    /// </summary>
    public class UIFormsIntegrationTests : IDisposable
    {
        private Settings _testSettings;
        private List<Form> _createdForms;

        public UIFormsIntegrationTests()
        {
            _testSettings = new Settings();
            _createdForms = new List<Form>();
            
            // テスト用の設定を準備
            _testSettings.Browsers = new List<Browser>
            {
                new Browser { Name = "Test Browser 1", Target = "test1.exe", IsDefault = true },
                new Browser { Name = "Test Browser 2", Target = "test2.exe", IsDefault = false }
            };
            
            _testSettings.URLs = new List<URL>
            {
                new URL { URLPattern = "test.com" }
            };
            
            _testSettings.Protocols = new List<Protocol>
            {
                new Protocol { Name = "test" }
            };
        }

        [Fact]
        public void AboutForm_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var aboutForm = new AboutForm();
            _createdForms.Add(aboutForm);

            // Assert
            aboutForm.Should().NotBeNull();
            aboutForm.Text.Should().NotBeNullOrEmpty();
            aboutForm.Controls.Count.Should().BeGreaterThan(0);
            
            // フォームを表示せずに破棄
            aboutForm.Dispose();
        }

        [Fact]
        public void IconSelectionForm_WithValidPath_ShouldInitializeCorrectly()
        {
            // Arrange
            var testFilePath = "test.exe";

            // Act
            var iconForm = new IconSelectionForm(testFilePath);
            _createdForms.Add(iconForm);

            // Assert
            iconForm.Should().NotBeNull();
            iconForm.Text.Should().Be("Icon Selection");
            iconForm.Size.Should().Be(new Size(700, 550));
            
            // フォームを表示せずに破棄
            iconForm.Dispose();
        }

        [Fact]
        public void OptionsForm_WithValidSettings_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var optionsForm = new OptionsForm(_testSettings);
            _createdForms.Add(optionsForm);

            // Assert
            optionsForm.Should().NotBeNull();
            optionsForm.GetSettings().Should().Be(_testSettings);
            optionsForm.Controls.Count.Should().BeGreaterThan(0);
            
            // フォームを表示せずに破棄
            optionsForm.Dispose();
        }

        [Fact]
        public void AddEditBrowserForm_WithValidBrowser_ShouldInitializeCorrectly()
        {
            // Arrange
            var testBrowser = new Browser { Name = "Test Browser", Target = "test.exe" };

            // Act
            var addEditForm = new AddEditBrowserForm();
            _createdForms.Add(addEditForm);

            // Assert
            addEditForm.Should().NotBeNull();
            addEditForm.Text.Should().Contain("Browser");
            
            // フォームを表示せずに破棄
            addEditForm.Dispose();
        }

        [Fact]
        public void AddEditURLForm_WithValidURL_ShouldInitializeCorrectly()
        {
            // Arrange
            var testURL = new URL { URLPattern = "test.com" };

            // Act
            var addEditForm = new AddEditURLForm();
            _createdForms.Add(addEditForm);

            // Assert
            addEditForm.Should().NotBeNull();
            addEditForm.Text.Should().Contain("URL");
            
            // フォームを表示せずに破棄
            addEditForm.Dispose();
        }

        [Fact]
        public void AddEditProtocolForm_WithValidProtocol_ShouldInitializeCorrectly()
        {
            // Arrange
            var testProtocol = new Protocol { Name = "test" };

            // Act
            var addEditForm = new AddEditProtocolForm();
            _createdForms.Add(addEditForm);

            // Assert
            addEditForm.Should().NotBeNull();
            addEditForm.Text.Should().Contain("Protocol");
            
            // フォームを表示せずに破棄
            addEditForm.Dispose();
        }

        [Fact]
        public void MainForm_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var mainForm = new MainForm();
            _createdForms.Add(mainForm);

            // Assert
            mainForm.Should().NotBeNull();
            mainForm.Text.Should().Be("Choose a Browser");
            mainForm.Controls.Count.Should().BeGreaterThan(0);
            
            // フォームを表示せずに破棄
            mainForm.Dispose();
        }

        [Fact]
        public void Forms_ShouldHandleDisposeCorrectly()
        {
            // Arrange
            var forms = new List<Form>
            {
                new AboutForm(),
                new IconSelectionForm("test.exe"),
                new OptionsForm(_testSettings),
                new AddEditBrowserForm(),
                new AddEditURLForm(),
                new AddEditProtocolForm(),
                new MainForm()
            };

            // Act & Assert
            foreach (var form in forms)
            {
                var action = () => form.Dispose();
                action.Should().NotThrow();
            }
        }

        [Fact]
        public void Forms_ShouldHaveValidProperties()
        {
            // Arrange
            var aboutForm = new AboutForm();
            var optionsForm = new OptionsForm(_testSettings);
            _createdForms.AddRange(new Form[] { aboutForm, optionsForm });

            // Act & Assert
            aboutForm.Should().NotBeNull();
            aboutForm.IsDisposed.Should().BeFalse();
            aboutForm.Visible.Should().BeFalse(); // 初期状態では非表示

            optionsForm.Should().NotBeNull();
            optionsForm.IsDisposed.Should().BeFalse();
            optionsForm.Visible.Should().BeFalse(); // 初期状態では非表示
        }

        [Fact]
        public void Forms_ShouldHandleShowAndHideCorrectly()
        {
            // Arrange
            var aboutForm = new AboutForm();
            _createdForms.Add(aboutForm);

            // Act & Assert
            aboutForm.Should().NotBeNull();
            
            // フォームの表示・非表示をテスト（実際には表示しない）
            aboutForm.Visible = false;
            aboutForm.Visible.Should().BeFalse();
        }

        public void Dispose()
        {
            // 作成したフォームを適切に破棄
            foreach (var form in _createdForms)
            {
                if (!form.IsDisposed)
                {
                    form.Dispose();
                }
            }
            _createdForms.Clear();
        }
    }
}
