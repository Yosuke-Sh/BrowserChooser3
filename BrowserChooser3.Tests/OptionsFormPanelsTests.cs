using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormPanelsクラスのテスト
    /// </summary>
    public class OptionsFormPanelsTests : IDisposable
    {
        private OptionsFormPanels _panels;
        private Settings _settings;

        public OptionsFormPanelsTests()
        {
            _panels = new OptionsFormPanels();
            _settings = new Settings();
        }

        public void Dispose()
        {
            _panels = null!;
            _settings = null!;
        }

        #region コンストラクタテスト

        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var panels = new OptionsFormPanels();

            // Assert
            panels.Should().NotBeNull();
        }

        [Fact]
        public void GetBrowserIcons_ShouldReturnNullInitially()
        {
            // Arrange
            var panels = new OptionsFormPanels();

            // Act
            var icons = panels.GetBrowserIcons();

            // Assert
            icons.Should().BeNull();
        }

        #endregion

        #region CreateBrowsersPanelテスト

        [Fact]
        public void CreateBrowsersPanel_ShouldReturnTabPage()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var tabPage = _panels.CreateBrowsersPanel(
                _settings, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);

            // Assert
            tabPage.Should().NotBeNull();
            tabPage.Name.Should().Be("tabBrowsers");
            tabPage.Text.Should().Be("Browsers & applications");
        }

        [Fact]
        public void CreateBrowsersPanel_ShouldCreateListView()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var tabPage = _panels.CreateBrowsersPanel(
                _settings, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);

            // Assert
            var listView = tabPage.Controls.Find("lstBrowsers", true).FirstOrDefault() as ListView;
            listView.Should().NotBeNull();
            listView.Name.Should().Be("lstBrowsers");
        }

        [Fact]
        public void CreateBrowsersPanel_ShouldCreateButtons()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var tabPage = _panels.CreateBrowsersPanel(
                _settings, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);

            // Assert
            var addButton = tabPage.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
            var editButton = tabPage.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
            var cloneButton = tabPage.Controls.Find("btnClone", true).FirstOrDefault() as Button;
            var detectButton = tabPage.Controls.Find("btnDetect", true).FirstOrDefault() as Button;
            var deleteButton = tabPage.Controls.Find("btnDelete", true).FirstOrDefault() as Button;

            addButton.Should().NotBeNull();
            editButton.Should().NotBeNull();
            cloneButton.Should().NotBeNull();
            detectButton.Should().NotBeNull();
            deleteButton.Should().NotBeNull();
        }

        #endregion

        #region CreateAutoURLsPanelテスト

        [Fact]
        public void CreateAutoURLsPanel_ShouldReturnTabPage()
        {
            // Arrange
            var mURLs = new SortedDictionary<int, URL>();
            var mBrowser = new Dictionary<int, Browser>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var tabPage = _panels.CreateAutoURLsPanel(_settings, mURLs, mBrowser, setModified, rebuildAutoURLs);

            // Assert
            tabPage.Should().NotBeNull();
            tabPage.Name.Should().Be("tabAutoURLs");
        }

        [Fact]
        public void CreateAutoURLsPanel_ShouldCreateListView()
        {
            // Arrange
            var mURLs = new SortedDictionary<int, URL>();
            var mBrowser = new Dictionary<int, Browser>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var tabPage = _panels.CreateAutoURLsPanel(_settings, mURLs, mBrowser, setModified, rebuildAutoURLs);

            // Assert
            var listView = tabPage.Controls.Find("lstURLs", true).FirstOrDefault() as ListView;
            listView.Should().NotBeNull();
            listView.Name.Should().Be("lstURLs");
        }

        #endregion

        #region CreateProtocolsPanelテスト

        [Fact]
        public void CreateProtocolsPanel_ShouldReturnTabPage()
        {
            // Arrange
            var mProtocols = new Dictionary<int, Protocol>();
            var mBrowser = new Dictionary<int, Browser>();
            var setModified = new Action<bool>(modified => { });

            // Act
            var tabPage = _panels.CreateProtocolsPanel(_settings, mProtocols, mBrowser, setModified);

            // Assert
            tabPage.Should().NotBeNull();
            tabPage.Name.Should().Be("tabProtocols");
        }

        #endregion

        #region CreateDisplayPanelテスト

        [Fact]
        public void CreateDisplayPanel_ShouldReturnTabPage()
        {
            // Arrange
            var setModified = new Action<bool>(modified => { });

            // Act
            var tabPage = _panels.CreateDisplayPanel(_settings, setModified);

            // Assert
            tabPage.Should().NotBeNull();
            tabPage.Name.Should().Be("tabDisplay");
        }

        [Fact]
        public void CreateDisplayPanel_ShouldCreateBackgroundColorControl()
        {
            // Arrange
            var setModified = new Action<bool>(modified => { });

            // Act
            var tabPage = _panels.CreateDisplayPanel(_settings, setModified);

            // Assert
            var pbBackgroundColor = tabPage.Controls.Find("pbBackgroundColor", true).FirstOrDefault() as PictureBox;
            pbBackgroundColor.Should().NotBeNull();
        }

        [Fact]
        public void CreateDisplayPanel_ShouldCreateTransparencyControls()
        {
            // Arrange
            var setModified = new Action<bool>(modified => { });

            // Act
            var tabPage = _panels.CreateDisplayPanel(_settings, setModified);

            // Assert
            var chkEnableTransparency = tabPage.Controls.Find("chkEnableTransparency", true).FirstOrDefault() as CheckBox;
            chkEnableTransparency.Should().NotBeNull();
        }

        #endregion



        #region CreatePrivacyPanelテスト

        [Fact]
        public void CreatePrivacyPanel_ShouldReturnTabPage()
        {
            // Arrange
            var setModified = new Action<bool>(modified => { });

            // Act
            var tabPage = _panels.CreatePrivacyPanel(_settings, setModified);

            // Assert
            tabPage.Should().NotBeNull();
            tabPage.Name.Should().Be("tabPrivacy");
        }

        #endregion

        #region CreateAdvancedPanelテスト



        #endregion

        #region エラーハンドリングテスト

        [Fact]
        public void CreateBrowsersPanel_WithEmptyDictionaries_ShouldNotThrowException()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act & Assert
            var action = () => _panels.CreateBrowsersPanel(
                _settings, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);
            action.Should().NotThrow();
        }

        [Fact]
        public void CreateBrowsersPanel_WithNullSettings_ShouldNotThrowException()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act & Assert
            var action = () => _panels.CreateBrowsersPanel(
                null!, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);
            action.Should().NotThrow();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void CreateAllPanels_ShouldCreateAllRequiredPanels()
        {
            // Arrange
            var mBrowser = new Dictionary<int, Browser>();
            var mProtocols = new Dictionary<int, Protocol>();
            var mURLs = new SortedDictionary<int, URL>();
            var setModified = new Action<bool>(modified => { });
            var rebuildAutoURLs = new Action(() => { });

            // Act
            var browsersPanel = _panels.CreateBrowsersPanel(_settings, mBrowser, mProtocols, 0, null, setModified, rebuildAutoURLs);
            var autoUrlsPanel = _panels.CreateAutoURLsPanel(_settings, mURLs, mBrowser, setModified, rebuildAutoURLs);
            var protocolsPanel = _panels.CreateProtocolsPanel(_settings, mProtocols, mBrowser, setModified);
            var displayPanel = _panels.CreateDisplayPanel(_settings, setModified);

            var privacyPanel = _panels.CreatePrivacyPanel(_settings, setModified);


            // Assert
            browsersPanel.Should().NotBeNull();
            autoUrlsPanel.Should().NotBeNull();
            protocolsPanel.Should().NotBeNull();
            displayPanel.Should().NotBeNull();

            privacyPanel.Should().NotBeNull();

        }

        #endregion
    }
}
