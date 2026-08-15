using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.MockFactories;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormPanelsクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// <para>
    /// 従来27件すべてが「OptionsFormのモック化が困難」を理由にスキップされていたが、
    /// このクラスは実際には <c>OptionsForm</c> に一切依存していない（引数は Settings と
    /// 辞書とコールバックのみ）。スキップは他のハンドラーテストからのコピーによるもので、
    /// 内容も <c>NotBeNull()</c> / <c>NotThrow()</c> だけだった。
    /// </para>
    /// <para>
    /// ここでは各パネルの中核的な契約——(1) 設定値が実際にコントロールの初期値へ反映されること、
    /// (2) コントロールを操作すると変更フラグが立つこと、(3) 表示中のデータ件数が
    /// 一覧へ反映されること——を検証する。特に (1) は Phase 3 で「UIで編集できるのに
    /// どこからも読まれていない設定」を潰した箇所であり、退行すると同じ状態に戻る。
    /// </para>
    /// </remarks>
    public class OptionsFormPanelsTests
    {
        private readonly OptionsFormPanels _panels = new();
        private bool _modified;
        private int _rebuildCount;

        private void SetModified(bool value) => _modified = value;
        private void RebuildAutoURLs() => _rebuildCount++;

        /// <summary>
        /// タブページ配下から名前でコントロールを1つ取り出します。
        /// </summary>
        private static T FindControl<T>(TabPage page, string name) where T : Control
        {
            var found = page.Controls.Find(name, searchAllChildren: true).OfType<T>().FirstOrDefault();
            found.Should().NotBeNull($"パネルには '{name}' という {typeof(T).Name} が存在するはず");
            return found!;
        }

        #region Browsers パネル

        [Fact]
        public void CreateBrowsersPanel_ShouldCreateNamedTabWithBrowserListView()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();

            // Act
            using var page = _panels.CreateBrowsersPanel(
                settings, new Dictionary<int, Browser>(), new Dictionary<int, Protocol>(),
                0, null, SetModified, RebuildAutoURLs);

            // Assert
            page.Name.Should().Be("tabBrowsers", "OptionsFormはタブ名で対象を探すため名前が契約になる");
            var listView = FindControl<ListView>(page, "lstBrowsers");
            listView.Columns.Count.Should().Be(6, "Name/Target/Row/Column/Hotkey/Arguments の6列");
        }

        [Fact]
        public void CreateBrowsersPanel_ShouldDisableEditButtonsWhileNothingIsSelected()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();

            // Act
            using var page = _panels.CreateBrowsersPanel(
                settings, new Dictionary<int, Browser>(), new Dictionary<int, Protocol>(),
                0, null, SetModified, RebuildAutoURLs);

            // Assert
            // 未選択のまま Edit/Clone/Delete を押せると、対象不明のまま操作されてしまう
            FindControl<Button>(page, "btnEdit").Enabled.Should().BeFalse();
            FindControl<Button>(page, "btnClone").Enabled.Should().BeFalse();
            FindControl<Button>(page, "btnDelete").Enabled.Should().BeFalse();
            FindControl<Button>(page, "btnAdd").Enabled.Should().BeTrue("追加は選択に依存しない");
            FindControl<Button>(page, "btnDetect").Enabled.Should().BeTrue("検出は選択に依存しない");
        }

        [Fact]
        public void CreateBrowsersPanel_InTestEnvironment_ShouldDisableDropTarget()
        {
            // Arrange
            // テスト実行中にAllowDropが有効だと、OLEドラッグ処理が
            // ヘッドレス環境で例外や停止を招くため無効化される契約
            var settings = SettingsFactory.CreateEmpty();

            // Act
            using var page = _panels.CreateBrowsersPanel(
                settings, new Dictionary<int, Browser>(), new Dictionary<int, Protocol>(),
                0, null, SetModified, RebuildAutoURLs);

            // Assert
            FindControl<ListView>(page, "lstBrowsers").AllowDrop.Should().BeFalse();
        }

        [Fact]
        public void GetBrowserIcons_BeforeCreatingPanel_ShouldReturnNull()
        {
            // Act & Assert
            _panels.GetBrowserIcons().Should().BeNull("パネル未構築ならアイコンリストも未生成");
        }

        [Fact]
        public void GetBrowserIcons_AfterCreatingBrowsersPanel_ShouldReturnImageListUsedByTheListView()
        {
            // Arrange & Act
            using var page = _panels.CreateBrowsersPanel(
                SettingsFactory.CreateEmpty(), new Dictionary<int, Browser>(),
                new Dictionary<int, Protocol>(), 0, null, SetModified, RebuildAutoURLs);

            // Assert
            var icons = _panels.GetBrowserIcons();
            icons.Should().NotBeNull();
            FindControl<ListView>(page, "lstBrowsers").SmallImageList.Should()
                .BeSameAs(icons, "OptionsFormが取得するアイコンリストは実際に一覧が使うものと同一であること");
        }

        #endregion

        #region Auto URLs パネル

        [Fact]
        public void CreateAutoURLsPanel_ShouldCreateNamedTabWithUrlListAndTestField()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();

            // Act
            using var page = _panels.CreateAutoURLsPanel(
                settings, new SortedDictionary<int, URL>(), new Dictionary<int, Browser>(),
                SetModified, RebuildAutoURLs);

            // Assert
            page.Name.Should().Be("tabAutoURLs");
            FindControl<ListView>(page, "lstURLs").Columns.Count.Should().Be(3, "URL/Browser/Delay の3列");
            // 3-8 で追加したルーティングプレビュー欄
            FindControl<TextBox>(page, "txtTestUrl").Should().NotBeNull();
        }

        [Fact]
        public void CreateAutoURLsPanel_InTestEnvironment_ShouldDisableDropTarget()
        {
            // Act
            using var page = _panels.CreateAutoURLsPanel(
                SettingsFactory.CreateEmpty(), new SortedDictionary<int, URL>(),
                new Dictionary<int, Browser>(), SetModified, RebuildAutoURLs);

            // Assert
            FindControl<ListView>(page, "lstURLs").AllowDrop.Should().BeFalse();
        }

        #endregion

        #region Protocols パネル

        [Fact]
        public void CreateProtocolsPanel_ShouldCreateNamedTabWithProtocolList()
        {
            // Act
            using var page = _panels.CreateProtocolsPanel(
                SettingsFactory.CreateEmpty(), new Dictionary<int, Protocol>(),
                new Dictionary<int, Browser>(), SetModified);

            // Assert
            page.Name.Should().Be("tabProtocols");
            FindControl<ListView>(page, "lstProtocols").Should().NotBeNull();
        }

        #endregion

        #region Display パネル — 設定値の反映と変更検知

        [Fact]
        public void CreateDisplayPanel_ShouldSeedControlsFromSettings()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            settings.EnableTransparency = true;
            settings.HideTitleBar = true;
            settings.ShowURL = false;
            settings.RevealShortURL = true;
            settings.Opacity = 0.42;
            settings.RoundedCornersRadius = 12;

            // Act
            using var page = _panels.CreateDisplayPanel(settings, SetModified);

            // Assert
            page.Name.Should().Be("tabDisplay");
            FindControl<CheckBox>(page, "chkEnableTransparency").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkHideTitleBar").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkShowURLs").Checked.Should().BeFalse();
            FindControl<CheckBox>(page, "chkRevealShortURLs").Checked.Should().BeTrue();
            FindControl<NumericUpDown>(page, "nudOpacity").Value.Should().Be(0.42m);
            FindControl<NumericUpDown>(page, "nudRoundedCorners").Value.Should().Be(12);
        }

        [Fact]
        public void CreateDisplayPanel_WhenCheckBoxToggled_ShouldSetModified()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            using var page = _panels.CreateDisplayPanel(settings, SetModified);
            var checkBox = FindControl<CheckBox>(page, "chkHideTitleBar");
            _modified.Should().BeFalse("構築しただけでは変更扱いにならない");

            // Act
            checkBox.Checked = !checkBox.Checked;

            // Assert
            _modified.Should().BeTrue("UI操作は未保存の変更として記録される");
        }

        [Fact]
        public void CreateDisplayPanel_WhenNumericChanged_ShouldSetModified()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            using var page = _panels.CreateDisplayPanel(settings, SetModified);
            var numeric = FindControl<NumericUpDown>(page, "nudOpacity");

            // Act
            numeric.Value = 0.55m;

            // Assert
            _modified.Should().BeTrue();
        }

        #endregion

        #region Accessibility(Focus) パネル

        [Fact]
        public void CreateFocusPanel_ShouldSeedControlsFromSettings()
        {
            // Arrange
            // 3-4 でプレースホルダのAccessibilityタブをこのパネルへ統合した
            var settings = SettingsFactory.CreateEmpty();
            settings.ShowFocus = true;
            settings.ShowVisualFocus = true;
            settings.UseAccessibleRendering = true;
            settings.FocusBoxLineWidth = 3;
            settings.FocusBoxWidth = 7;

            // Act
            using var page = _panels.CreateFocusPanel(settings, SetModified);

            // Assert
            FindControl<CheckBox>(page, "chkShowFocus").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkShowVisualFocus").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkUseAccessibleRendering").Checked.Should().BeTrue(
                "Use Accessible Rendering は Display ではなく Accessibility タブに置かれている");
            FindControl<NumericUpDown>(page, "nudFocusBoxLineWidth").Value.Should().Be(3);
            FindControl<NumericUpDown>(page, "nudFocusBoxWidth").Value.Should().Be(7);
        }

        #endregion

        #region Grid パネル — グリッドとウィンドウサイズ

        [Fact]
        public void CreateGridPanel_ShouldSeedGridControlsFromSettings()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            settings.GridWidth = 5;
            settings.GridHeight = 4;
            settings.ShowGrid = true;
            settings.GridLineWidth = 3;
            settings.IconWidth = 48;
            settings.IconHeight = 48;

            // Act
            using var page = _panels.CreateGridPanel(settings, SetModified);

            // Assert
            page.Name.Should().Be("tabGrid");
            FindControl<NumericUpDown>(page, "nudGridWidth").Value.Should().Be(5);
            FindControl<NumericUpDown>(page, "nudGridHeight").Value.Should().Be(4);
            FindControl<CheckBox>(page, "chkShowGrid").Checked.Should().BeTrue();
            FindControl<NumericUpDown>(page, "nudGridLineWidth").Value.Should().Be(3);
            FindControl<NumericUpDown>(page, "nudIconSizeWidth").Value.Should().Be(48);
            FindControl<NumericUpDown>(page, "nudIconSizeHeight").Value.Should().Be(48);
        }

        [Fact]
        public void CreateGridPanel_ShouldSeedWindowSizeFromSettings()
        {
            // Arrange
            // 3-2 まで nudWidth / nudHeight は「Controls.Find で読み書きされるのに
            // 一度も生成されていない」コントロールだった。実在することが契約。
            var settings = SettingsFactory.CreateEmpty();
            settings.Width = 900;
            settings.Height = 640;

            // Act
            using var page = _panels.CreateGridPanel(settings, SetModified);

            // Assert
            FindControl<NumericUpDown>(page, "nudWidth").Value.Should().Be(900);
            FindControl<NumericUpDown>(page, "nudHeight").Value.Should().Be(640);
        }

        [Fact]
        public void CreateGridPanel_WithLegacyUndersizedWindowSize_ShouldFallBackToEffectiveDefaults()
        {
            // Arrange
            // 旧形式の設定は Width/Height に 1〜10 のような「グリッド列数」を持つ。
            // そのまま表示するとウィンドウ幅1pxとして提示されてしまうため、
            // EffectiveWindowWidth/Height 経由の既定値へ落ちる必要がある。
            var settings = SettingsFactory.CreateEmpty();
            settings.Width = 5;
            settings.Height = 4;

            // Act
            using var page = _panels.CreateGridPanel(settings, SetModified);

            // Assert
            FindControl<NumericUpDown>(page, "nudWidth").Value
                .Should().Be(settings.EffectiveWindowWidth)
                .And.BeGreaterThanOrEqualTo(Settings.MinimumWindowWidth);
            FindControl<NumericUpDown>(page, "nudHeight").Value
                .Should().Be(settings.EffectiveWindowHeight)
                .And.BeGreaterThanOrEqualTo(Settings.MinimumWindowHeight);
        }

        [Fact]
        public void CreateGridPanel_WhenWindowSizeChanged_ShouldSetModified()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            using var page = _panels.CreateGridPanel(settings, SetModified);
            var nudWidth = FindControl<NumericUpDown>(page, "nudWidth");

            // Act
            nudWidth.Value = nudWidth.Value + 10;

            // Assert
            _modified.Should().BeTrue();
        }

        #endregion

        #region Privacy パネル

        [Fact]
        public void CreatePrivacyPanel_ShouldSeedTrackingSettingsFromSettings()
        {
            // Arrange
            // 3-9 で追加。既定OFFであること・一覧が改行区切りで提示されることが契約。
            var settings = SettingsFactory.CreateEmpty();
            settings.RemoveTrackingParameters = true;
            settings.TrackingParameters = new List<string> { "utm_source", "fbclid" };
            settings.EnableLogging = true;

            // Act
            using var page = _panels.CreatePrivacyPanel(settings, SetModified);

            // Assert
            FindControl<CheckBox>(page, "chkRemoveTrackingParameters").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkEnableLogging").Checked.Should().BeTrue();

            var textBox = FindControl<TextBox>(page, "txtTrackingParameters");
            textBox.Lines.Should().BeEquivalentTo(new[] { "utm_source", "fbclid" },
                "パラメータは1行1件で編集できる");
        }

        [Fact]
        public void CreatePrivacyPanel_ResetButton_ShouldRestoreDefaultTrackingParameters()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            settings.TrackingParameters = new List<string> { "custom_only" };
            using var page = _panels.CreatePrivacyPanel(settings, SetModified);
            var textBox = FindControl<TextBox>(page, "txtTrackingParameters");
            var resetButton = FindControl<Button>(page, "btnResetTrackingParameters");

            // Act
            resetButton.PerformClick();

            // Assert
            textBox.Lines.Should().BeEquivalentTo(Settings.DefaultTrackingParameters,
                "既定値に戻すボタンは組み込みの一覧を復元する");
            _modified.Should().BeTrue();
        }

        #endregion

        #region Startup パネル

        [Fact]
        public void CreateStartupPanel_ShouldSeedControlsFromSettings()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            settings.StartInTray = true;
            settings.AlwaysResidentInTray = true;
            settings.StartupDelay = 3;
            settings.StartupMessage = "Ready";

            // Act
            using var page = _panels.CreateStartupPanel(settings, SetModified);

            // Assert
            FindControl<CheckBox>(page, "chkStartInTray").Checked.Should().BeTrue();
            FindControl<CheckBox>(page, "chkAlwaysResidentInTray").Checked.Should().BeTrue();
            FindControl<NumericUpDown>(page, "nudStartupDelay").Value.Should().Be(3);
            // 3-3 で MainForm に表示されるようになった起動メッセージ
            FindControl<TextBox>(page, "txtStartupMessage").Text.Should().Be("Ready");
        }

        #endregion

        #region Others パネル

        [Fact]
        public void CreateOthersPanel_ShouldSeedControlsFromSettings()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            settings.AllowStayOpen = true;
            settings.DefaultDelay = 7;
            settings.Separator = " | ";
            settings.UserAgent = "TestAgent/1.0";
            settings.DefaultMessage = "Choose a browser";
            settings.OptionsShortcut = 'K';

            // Act
            using var page = _panels.CreateOthersPanel(settings, SetModified);

            // Assert
            FindControl<CheckBox>(page, "chkAllowStayOpen").Checked.Should().BeTrue();
            FindControl<NumericUpDown>(page, "nudDefaultDelay").Value.Should().Be(7);
            FindControl<TextBox>(page, "txtSeparator").Text.Should().Be(" | ");
            FindControl<TextBox>(page, "txtUserAgent").Text.Should().Be("TestAgent/1.0");
            FindControl<TextBox>(page, "txtDefaultMessage").Text.Should().Be("Choose a browser");
            FindControl<TextBox>(page, "txtOptionsShortcut").Text.Should().Be("K",
                "1-6 で空文字保存によるショートカット恒久無効化を潰したため、値が往復できること");
        }

        [Fact]
        public void CreateOthersPanel_WhenTextChanged_ShouldSetModified()
        {
            // Arrange
            var settings = SettingsFactory.CreateEmpty();
            using var page = _panels.CreateOthersPanel(settings, SetModified);
            var textBox = FindControl<TextBox>(page, "txtUserAgent");

            // Act
            textBox.Text = "Changed/2.0";

            // Assert
            _modified.Should().BeTrue();
        }

        #endregion

        #region 全パネル共通の契約

        [Fact]
        public void AllSettingPanels_ShouldHaveUniqueTabNames()
        {
            // Arrange
            // OptionsForm は Controls.Find(name, true) でタブ配下を横断検索するため、
            // タブ名やコントロール名が重複すると誤ったコントロールを掴む
            var settings = SettingsFactory.CreateEmpty();

            // Act
            var pages = new List<TabPage>
            {
                _panels.CreateBrowsersPanel(settings, new Dictionary<int, Browser>(),
                    new Dictionary<int, Protocol>(), 0, null, SetModified, RebuildAutoURLs),
                _panels.CreateAutoURLsPanel(settings, new SortedDictionary<int, URL>(),
                    new Dictionary<int, Browser>(), SetModified, RebuildAutoURLs),
                _panels.CreateProtocolsPanel(settings, new Dictionary<int, Protocol>(),
                    new Dictionary<int, Browser>(), SetModified),
                _panels.CreateDisplayPanel(settings, SetModified),
                _panels.CreateFocusPanel(settings, SetModified),
                _panels.CreateGridPanel(settings, SetModified),
                _panels.CreatePrivacyPanel(settings, SetModified),
                _panels.CreateStartupPanel(settings, SetModified),
                _panels.CreateOthersPanel(settings, SetModified)
            };

            try
            {
                // Assert
                var names = pages.Select(p => p.Name).ToList();
                names.Should().OnlyHaveUniqueItems();
                names.Should().NotContain(string.Empty, "全タブが名前で特定できること");
            }
            finally
            {
                foreach (var page in pages) page.Dispose();
            }
        }

        [Fact]
        public void AllSettingPanels_WhenBuiltWithDefaults_ShouldNotReportModified()
        {
            // Arrange
            // 構築時のプロパティ代入でValueChanged/TextChangedが走ると、
            // ユーザーが何も触っていないのに「未保存の変更があります」と出てしまう
            var settings = SettingsFactory.CreateEmpty();

            // Act
            var pages = new List<TabPage>
            {
                _panels.CreateDisplayPanel(settings, SetModified),
                _panels.CreateFocusPanel(settings, SetModified),
                _panels.CreateGridPanel(settings, SetModified),
                _panels.CreatePrivacyPanel(settings, SetModified),
                _panels.CreateStartupPanel(settings, SetModified),
                _panels.CreateOthersPanel(settings, SetModified)
            };

            try
            {
                // Assert
                _modified.Should().BeFalse("パネル構築だけでは変更フラグを立てない");
            }
            finally
            {
                foreach (var page in pages) page.Dispose();
            }
        }

        [Fact]
        public void CreateBrowsersPanel_CalledRepeatedly_ShouldReturnIndependentTabPages()
        {
            // Arrange
            // OptionsForm は開くたびにパネルを作り直すため、
            // 前回のインスタンスを共有すると破棄済みコントロールを掴む
            var settings = SettingsFactory.CreateEmpty();

            // Act
            using var first = _panels.CreateBrowsersPanel(settings, new Dictionary<int, Browser>(),
                new Dictionary<int, Protocol>(), 0, null, SetModified, RebuildAutoURLs);
            using var second = _panels.CreateBrowsersPanel(settings, new Dictionary<int, Browser>(),
                new Dictionary<int, Protocol>(), 0, null, SetModified, RebuildAutoURLs);

            // Assert
            second.Should().NotBeSameAs(first);
            FindControl<ListView>(page: second, name: "lstBrowsers")
                .Should().NotBeSameAs(FindControl<ListView>(page: first, name: "lstBrowsers"));
        }

        #endregion
    }
}
