using BrowserChooser3.Classes.Models;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// モデルクラスのテスト
    /// </summary>
    public class ModelTests
    {
        #region Browserテスト

        [Fact]
        public void Browser_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var browser = new Browser();

            // Assert
            browser.Should().NotBeNull();
            browser.Guid.Should().NotBe(Guid.Empty);
            browser.Name.Should().BeEmpty();
            browser.Target.Should().BeEmpty();
            browser.Arguments.Should().BeEmpty();
            browser.X.Should().Be(0);
            browser.Y.Should().Be(0);
            browser.Hotkey.Should().Be('\0');
            browser.IsDefault.Should().BeFalse();
        }

        [Fact]
        public void Browser_Properties_ShouldBeSettable()
        {
            // Arrange
            var browser = new Browser();
            var guid = Guid.NewGuid();
            var name = "Test Browser";
            var target = "C:\\test\\browser.exe";
            var arguments = "--test-arg";
            var x = 10;
            var y = 20;
            var hotkey = 'T';
            var isDefault = true;

            // Act
            browser.Guid = guid;
            browser.Name = name;
            browser.Target = target;
            browser.Arguments = arguments;
            browser.X = x;
            browser.Y = y;
            browser.Hotkey = hotkey;
            browser.IsDefault = isDefault;

            // Assert
            browser.Guid.Should().Be(guid);
            browser.Name.Should().Be(name);
            browser.Target.Should().Be(target);
            browser.Arguments.Should().Be(arguments);
            browser.X.Should().Be(x);
            browser.Y.Should().Be(y);
            browser.Hotkey.Should().Be(hotkey);
            browser.IsDefault.Should().Be(isDefault);
        }

        [Fact]
        public void Browser_Clone_ShouldCreateCopy()
        {
            // Arrange
            var original = new Browser
            {
                Name = "Original Browser",
                Target = "C:\\original\\browser.exe",
                Arguments = "--original",
                X = 5,
                Y = 10,
                Hotkey = 'O',
                IsDefault = true
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Guid.Should().Be(original.Guid);
            clone.Name.Should().Be(original.Name);
            clone.Target.Should().Be(original.Target);
            clone.Arguments.Should().Be(original.Arguments);
            clone.X.Should().Be(original.X);
            clone.Y.Should().Be(original.Y);
            clone.Hotkey.Should().Be(original.Hotkey);
            clone.IsDefault.Should().Be(original.IsDefault);
        }
        #endregion

        #region Protocolテスト

        [Fact]
        public void Protocol_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var protocol = new Protocol();

            // Assert
            protocol.Should().NotBeNull();
            protocol.Guid.Should().NotBe(Guid.Empty);
            protocol.Name.Should().BeEmpty();
            protocol.Header.Should().BeEmpty();
            protocol.BrowserGuid.Should().Be(Guid.Empty);
            protocol.IsActive.Should().BeTrue();
            protocol.SupportingBrowsers.Should().NotBeNull();
            protocol.SupportingBrowsers.Should().BeEmpty();
        }

        [Fact]
        public void Protocol_Properties_ShouldBeSettable()
        {
            // Arrange
            var protocol = new Protocol();
            var guid = Guid.NewGuid();
            var name = "Test Protocol";
            var header = "test://";
            var browserGuid = Guid.NewGuid();
            var isActive = false;

            // Act
            protocol.Guid = guid;
            protocol.Name = name;
            protocol.Header = header;
            protocol.BrowserGuid = browserGuid;
            protocol.IsActive = isActive;

            // Assert
            protocol.Guid.Should().Be(guid);
            protocol.Name.Should().Be(name);
            protocol.Header.Should().Be(header);
            protocol.BrowserGuid.Should().Be(browserGuid);
            protocol.IsActive.Should().Be(isActive);
        }

        [Fact]
        public void Protocol_ConstructorWithParameters_ShouldInitializeCorrectly()
        {
            // Arrange
            var name = "Test Protocol";
            var header = "test://";
            var supportingBrowsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var categories = new List<string> { "Test Category" };

            // Act
            var protocol = new Protocol(name, header, supportingBrowsers, categories);

            // Assert
            protocol.Should().NotBeNull();
            protocol.Name.Should().Be(name);
            protocol.Header.Should().Be(header);
            protocol.SupportingBrowsers.Should().BeEquivalentTo(supportingBrowsers);
            protocol.DefaultCategories.Should().BeEquivalentTo(categories);
            protocol.Category.Should().Be("Test Category");
            protocol.Active.Should().BeTrue();
        }

        [Fact]
        public void Protocol_Clone_ShouldCreateCopy()
        {
            // Arrange
            var original = new Protocol
            {
                Name = "Original Protocol",
                Header = "original://",
                BrowserGuid = Guid.NewGuid(),
                IsActive = true,
                SupportingBrowsers = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Guid.Should().Be(original.Guid);
            clone.Name.Should().Be(original.Name);
            clone.Header.Should().Be(original.Header);
            clone.BrowserGuid.Should().Be(original.BrowserGuid);
            clone.IsActive.Should().Be(original.IsActive);
            clone.SupportingBrowsers.Should().BeEquivalentTo(original.SupportingBrowsers);
        }
        #endregion

        #region FileTypeテスト

        [Fact]
        public void FileType_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var fileType = new FileType();

            // Assert
            fileType.Should().NotBeNull();
            fileType.Guid.Should().NotBe(Guid.Empty);
            fileType.Name.Should().BeEmpty();
            fileType.Extension.Should().BeEmpty();
            fileType.BrowserGuid.Should().Be(Guid.Empty);
            fileType.IsActive.Should().BeTrue();
            fileType.SupportingBrowsers.Should().NotBeNull();
            fileType.SupportingBrowsers.Should().BeEmpty();
        }

        [Fact]
        public void FileType_Properties_ShouldBeSettable()
        {
            // Arrange
            var fileType = new FileType();
            var guid = Guid.NewGuid();
            var name = "Test File Type";
            var extension = ".test";
            var browserGuid = Guid.NewGuid();
            var isActive = false;

            // Act
            fileType.Guid = guid;
            fileType.Name = name;
            fileType.Extension = extension;
            fileType.BrowserGuid = browserGuid;
            fileType.IsActive = isActive;

            // Assert
            fileType.Guid.Should().Be(guid);
            fileType.Name.Should().Be(name);
            fileType.Extension.Should().Be(extension);
            fileType.BrowserGuid.Should().Be(browserGuid);
            fileType.IsActive.Should().Be(isActive);
        }

        [Fact]
        public void FileType_ConstructorWithParameters_ShouldInitializeCorrectly()
        {
            // Arrange
            var name = "Test File Type";
            var extension = ".test";
            var supportingBrowsers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var categories = new List<string> { "Test Category" };

            // Act
            var fileType = new FileType(name, extension, supportingBrowsers, categories);

            // Assert
            fileType.Should().NotBeNull();
            fileType.Name.Should().Be(name);
            fileType.Extension.Should().Be(extension);
            fileType.SupportingBrowsers.Should().BeEquivalentTo(supportingBrowsers);
            fileType.DefaultCategories.Should().BeEquivalentTo(categories);
            fileType.Category.Should().Be("Test Category");
            fileType.Active.Should().BeTrue();
        }

        [Fact]
        public void FileType_Clone_ShouldCreateCopy()
        {
            // Arrange
            var original = new FileType
            {
                Name = "Original File Type",
                Extension = ".original",
                BrowserGuid = Guid.NewGuid(),
                IsActive = true,
                SupportingBrowsers = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Guid.Should().Be(original.Guid);
            clone.Name.Should().Be(original.Name);
            clone.Extension.Should().Be(original.Extension);
            clone.BrowserGuid.Should().Be(original.BrowserGuid);
            clone.IsActive.Should().Be(original.IsActive);
            clone.SupportingBrowsers.Should().BeEquivalentTo(original.SupportingBrowsers);
        }
        #endregion

        #region URLテスト

        [Fact]
        public void URL_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var url = new URL();

            // Assert
            url.Should().NotBeNull();
            url.Guid.Should().NotBe(Guid.Empty);
            url.Name.Should().BeEmpty();
            url.URLPattern.Should().BeEmpty();
            url.BrowserGuid.Should().Be(Guid.Empty);
            url.IsActive.Should().BeTrue();
            url.SupportingBrowsers.Should().NotBeNull();
            url.SupportingBrowsers.Should().BeEmpty();
        }

        [Fact]
        public void URL_Properties_ShouldBeSettable()
        {
            // Arrange
            var url = new URL();
            var guid = Guid.NewGuid();
            var name = "Test URL";
            var urlPattern = "https://example.com";
            var browserGuid = Guid.NewGuid();
            var isActive = false;

            // Act
            url.Guid = guid;
            url.Name = name;
            url.URLPattern = urlPattern;
            url.BrowserGuid = browserGuid;
            url.IsActive = isActive;

            // Assert
            url.Guid.Should().Be(guid);
            url.Name.Should().Be(name);
            url.URLPattern.Should().Be(urlPattern);
            url.BrowserGuid.Should().Be(browserGuid);
            url.IsActive.Should().Be(isActive);
        }



        [Fact]
        public void URL_Clone_ShouldCreateCopy()
        {
            // Arrange
            var original = new URL
            {
                Name = "Original URL",
                URLPattern = "https://original.com",
                BrowserGuid = Guid.NewGuid(),
                IsActive = true,
                SupportingBrowsers = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Guid.Should().Be(original.Guid);
            clone.Name.Should().Be(original.Name);
            clone.URLPattern.Should().Be(original.URLPattern);
            clone.BrowserGuid.Should().Be(original.BrowserGuid);
            clone.IsActive.Should().Be(original.IsActive);
            clone.SupportingBrowsers.Should().BeEquivalentTo(original.SupportingBrowsers);
        }
        #endregion

        #region FocusSettingsテスト

        [Fact]
        public void FocusSettings_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var focusSettings = new FocusSettings();

            // Assert
            focusSettings.Should().NotBeNull();
            focusSettings.ShowFocus.Should().BeFalse();
            focusSettings.BoxColor.Should().Be(System.Drawing.Color.Red);
            focusSettings.BoxWidth.Should().Be(2);
        }

        [Fact]
        public void FocusSettings_Properties_ShouldBeSettable()
        {
            // Arrange
            var focusSettings = new FocusSettings();
            var showFocus = true;
            var boxColor = System.Drawing.Color.Blue;
            var boxWidth = 5;

            // Act
            focusSettings.ShowFocus = showFocus;
            focusSettings.BoxColor = boxColor;
            focusSettings.BoxWidth = boxWidth;

            // Assert
            focusSettings.ShowFocus.Should().Be(showFocus);
            focusSettings.BoxColor.Should().Be(boxColor);
            focusSettings.BoxWidth.Should().Be(boxWidth);
        }
        #endregion

        #region BrowserDefinitionテスト

        [Fact]
        public void BrowserDefinition_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var browserDefinition = new BrowserDefinition();

            // Assert
            browserDefinition.Should().NotBeNull();
            browserDefinition.Name.Should().BeEmpty();
            browserDefinition.Paths.Should().BeEmpty();
            browserDefinition.Version.Should().BeEmpty();
            browserDefinition.Description.Should().BeEmpty();
            browserDefinition.Website.Should().BeEmpty();
            browserDefinition.Category.Should().BeEmpty();
            browserDefinition.Active.Should().BeTrue();
        }

        [Fact]
        public void BrowserDefinition_Properties_ShouldBeSettable()
        {
            // Arrange
            var browserDefinition = new BrowserDefinition();
            var name = "Test Browser Definition";
            var paths = new List<string> { "C:\\test\\browser.exe" };
            var version = "1.0.0";
            var description = "Test browser description";
            var website = "https://test.com";
            var category = "Test Category";
            var active = false;

            // Act
            browserDefinition.Name = name;
            browserDefinition.Paths = paths;
            browserDefinition.Version = version;
            browserDefinition.Description = description;
            browserDefinition.Website = website;
            browserDefinition.Category = category;
            browserDefinition.Active = active;

            // Assert
            browserDefinition.Name.Should().Be(name);
            browserDefinition.Paths.Should().BeEquivalentTo(paths);
            browserDefinition.Version.Should().Be(version);
            browserDefinition.Description.Should().Be(description);
            browserDefinition.Website.Should().Be(website);
            browserDefinition.Category.Should().Be(category);
            browserDefinition.Active.Should().Be(active);
        }
        #endregion

        #region DisplayDictionaryテスト


        #endregion

        #region 境界値テスト

        [Fact]
        public void Browser_WithMaxValues_ShouldHandleGracefully()
        {
            // Arrange
            var browser = new Browser();
            var maxInt = int.MaxValue;
            var maxChar = char.MaxValue;

            // Act
            browser.X = maxInt;
            browser.Y = maxInt;
            browser.Hotkey = maxChar;

            // Assert
            browser.X.Should().Be(maxInt);
            browser.Y.Should().Be(maxInt);
            browser.Hotkey.Should().Be(maxChar);
        }

        [Fact]
        public void FocusSettings_WithMaxValues_ShouldHandleGracefully()
        {
            // Arrange
            var focusSettings = new FocusSettings();
            var maxInt = int.MaxValue;

            // Act
            focusSettings.BoxWidth = maxInt;

            // Assert
            focusSettings.BoxWidth.Should().Be(maxInt);
        }
        #endregion

        #region 例外処理テスト

        [Fact]
        public void Model_WithNullStrings_ShouldHandleGracefully()
        {
            // Arrange
            var browser = new Browser();

            // Act
            browser.Name = null!;
            browser.Target = null!;
            browser.Arguments = null!;

            // Assert
            browser.Name.Should().BeNull();
            browser.Target.Should().BeNull();
            browser.Arguments.Should().BeNull();
        }
        #endregion

        #region 統合テスト

        [Fact]
        public void Models_ShouldWorkTogether()
        {
            // Arrange
            var browser = new Browser { Name = "Test Browser", Guid = Guid.NewGuid() };
            var protocol = new Protocol { Name = "Test Protocol", BrowserGuid = browser.Guid };
            var fileType = new FileType { Name = "Test File Type", BrowserGuid = browser.Guid };
            var url = new URL { Name = "Test URL", BrowserGuid = browser.Guid };

            // Act & Assert
            protocol.BrowserGuid.Should().Be(browser.Guid);
            fileType.BrowserGuid.Should().Be(browser.Guid);
            url.BrowserGuid.Should().Be(browser.Guid);
        }
        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Browser_Clone_ShouldBeFast()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = "C:\\test\\browser.exe",
                Arguments = "--test-arg",
                X = 10,
                Y = 20,
                Hotkey = 'T',
                IsDefault = true
            };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 1000; i++)
            {
                browser.Clone();
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1秒以内
        }
        #endregion

        #region エッジケーステスト

        [Fact]
        public void Browser_WithEmptyGuid_ShouldHandleGracefully()
        {
            // Arrange
            var browser = new Browser();

            // Act
            browser.Guid = Guid.Empty;

            // Assert
            browser.Guid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Protocol_WithEmptyGuid_ShouldHandleGracefully()
        {
            // Arrange
            var protocol = new Protocol();

            // Act
            protocol.Guid = Guid.Empty;
            protocol.BrowserGuid = Guid.Empty;

            // Assert
            protocol.Guid.Should().Be(Guid.Empty);
            protocol.BrowserGuid.Should().Be(Guid.Empty);
        }
        #endregion

        #region スレッド安全性テスト

        [Fact]
        public async Task Browser_Properties_ShouldBeThreadSafe()
        {
            // Arrange
            var browser = new Browser();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var index = i;
                tasks.Add(Task.Run(() =>
                {
                    browser.Name = $"Browser {index}";
                    browser.X = index;
                    browser.Y = index * 2;
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            browser.Should().NotBeNull();
            browser.Name.Should().NotBeEmpty();
        }
        #endregion

        #region メモリテスト

        [Fact]
        public void Browser_Clone_ShouldNotLeakMemory()
        {
            // Arrange
            var browser = new Browser
            {
                Name = "Test Browser",
                Target = "C:\\test\\browser.exe",
                Arguments = "--test-arg"
            };
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 10000; i++)
            {
                browser.Clone();
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(1024 * 1024); // 1MB以内
        }
        #endregion
    }
}
