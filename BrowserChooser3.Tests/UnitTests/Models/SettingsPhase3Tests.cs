using System.IO;
using System.Xml.Serialization;
using BrowserChooser3.Classes;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Phase 3 で機能化した設定（ウィンドウサイズ・起動メッセージ・
    /// アクセシブルレンダリング）の振る舞いテスト。
    /// いずれも「保存はされるが誰も読んでいなかった」設定であり、
    /// 旧設定ファイルとの後方互換が回帰しやすいためここで固定する。
    /// </summary>
    public class SettingsPhase3Tests
    {
        #region ウィンドウサイズ（3-2）

        [Fact]
        public void EffectiveWindowWidth_WithLegacyGridColumnCount_ShouldFallBackToDefault()
        {
            // 旧バージョンは Width をグリッドの列数（既定8）として書き出していた。
            // これをピクセル幅として適用すると8pxのウィンドウになってしまう。
            var settings = new Settings { Width = 8 };

            settings.EffectiveWindowWidth.Should().Be(Settings.DefaultWindowWidth);
        }

        [Fact]
        public void EffectiveWindowHeight_WithLegacyGridRowCount_ShouldFallBackToDefault()
        {
            var settings = new Settings { Height = 1 };

            settings.EffectiveWindowHeight.Should().Be(Settings.DefaultWindowHeight);
        }

        [Fact]
        public void EffectiveWindowSize_WithUserConfiguredPixelValues_ShouldUseThem()
        {
            var settings = new Settings { Width = 1024, Height = 768 };

            settings.EffectiveWindowWidth.Should().Be(1024);
            settings.EffectiveWindowHeight.Should().Be(768);
        }

        [Theory]
        [InlineData(Settings.MinimumWindowWidth - 1, Settings.DefaultWindowWidth)]
        [InlineData(Settings.MinimumWindowWidth, Settings.MinimumWindowWidth)]
        [InlineData(0, Settings.DefaultWindowWidth)]
        [InlineData(-100, Settings.DefaultWindowWidth)]
        public void EffectiveWindowWidth_AtBoundaries_ShouldNeverGoBelowMinimum(int width, int expected)
        {
            var settings = new Settings { Width = width };

            settings.EffectiveWindowWidth.Should().Be(expected);
            settings.EffectiveWindowWidth.Should().BeGreaterThanOrEqualTo(Settings.MinimumWindowWidth);
        }

        [Fact]
        public void EffectiveWindowSize_ShouldNotBeSerializedToXml()
        {
            // [XmlIgnore] が外れると設定ファイルに読み取り専用プロパティが書き出され、
            // 逆シリアライズ時に例外になる。
            var serializer = new XmlSerializer(typeof(Settings));
            using var writer = new StringWriter();
            serializer.Serialize(writer, new Settings());

            var xml = writer.ToString();
            xml.Should().NotContain("EffectiveWindowWidth");
            xml.Should().NotContain("EffectiveWindowHeight");
        }

        #endregion

        #region 起動メッセージ（3-3）

        [Fact]
        public void IsStartupMessageVisible_WithDefaultSettings_ShouldBeFalse()
        {
            new Settings().IsStartupMessageVisible.Should().BeFalse();
        }

        [Fact]
        public void IsStartupMessageVisible_WithLegacyPlaceholder_ShouldBeFalse()
        {
            // 旧既定値が残っている既存設定ファイルで、更新しただけで
            // 見覚えのないメッセージが表示されることを防ぐ。
            var settings = new Settings { StartupMessage = Settings.LegacyDefaultStartupMessage };

            settings.IsStartupMessageVisible.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void IsStartupMessageVisible_WithBlankMessage_ShouldBeFalse(string message)
        {
            new Settings { StartupMessage = message }.IsStartupMessageVisible.Should().BeFalse();
        }

        [Fact]
        public void IsStartupMessageVisible_WithUserMessage_ShouldBeTrue()
        {
            var settings = new Settings { StartupMessage = "業務用ブラウザを選んでください" };

            settings.IsStartupMessageVisible.Should().BeTrue();
        }

        #endregion

        #region アクセシブルレンダリング（3-4）

        [Fact]
        public void IsAccessibleRenderingActive_WithUserSettingEnabled_ShouldBeTrue()
        {
            new Settings { UseAccessibleRendering = true }.IsAccessibleRenderingActive.Should().BeTrue();
        }

        [Fact]
        public void IsAccessibleRenderingActive_WithUserSettingDisabledAndNoPolicy_ShouldBeFalse()
        {
            // ポリシー未設定時の既定は false。
            new Settings { UseAccessibleRendering = false }.IsAccessibleRenderingActive
                .Should().Be(BrowserChooser3.Classes.Services.SystemServices.Policy.AccessibleRendering);
        }

        #endregion

        #region グリッド（3-1）

        [Fact]
        public void ShowGrid_ShouldDefaultToOff()
        {
            // 既定 ON にすると既存ユーザーの見た目が勝手に変わる。
            new Settings().ShowGrid.Should().BeFalse();
        }

        [Fact]
        public void GridSettings_ShouldRoundTripThroughXml()
        {
            var original = new Settings
            {
                ShowGrid = true,
                GridColor = System.Drawing.Color.Red.ToArgb(),
                GridLineWidth = 3
            };

            var serializer = new XmlSerializer(typeof(Settings));
            using var writer = new StringWriter();
            serializer.Serialize(writer, original);
            using var reader = new StringReader(writer.ToString());
            var restored = (Settings)serializer.Deserialize(reader)!;

            restored.ShowGrid.Should().BeTrue();
            restored.GridColor.Should().Be(System.Drawing.Color.Red.ToArgb());
            restored.GridLineWidth.Should().Be(3);
        }

        #endregion

        #region 後方互換

        [Fact]
        public void Load_FromXmlWithoutPhase3Elements_ShouldUseDefaults()
        {
            // Phase 3 以前に保存された設定ファイル（新規プロパティを含まない）が
            // そのまま読めること。XmlSerializer は未知/欠落要素を無視する。
            const string legacyXml =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<Settings xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                "<Width>8</Width><Height>1</Height>" +
                "<StartupMessage>BrowserChooser3 Started</StartupMessage>" +
                "</Settings>";

            var serializer = new XmlSerializer(typeof(Settings));
            using var reader = new StringReader(legacyXml);
            var settings = (Settings)serializer.Deserialize(reader)!;

            settings.Width.Should().Be(8);
            settings.EffectiveWindowWidth.Should().Be(Settings.DefaultWindowWidth);
            settings.IsStartupMessageVisible.Should().BeFalse();
        }

        #endregion
    }
}
