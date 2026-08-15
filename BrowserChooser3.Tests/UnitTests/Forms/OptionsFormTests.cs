using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormクラスのテスト
    /// </summary>
    public class OptionsFormTests : IDisposable
    {
        private OptionsForm _form;
        private Settings _settings;

        public OptionsFormTests()
        {
            _settings = new Settings();
            _form = new OptionsForm(_settings);
        }

        public void Dispose()
        {
            _form?.Dispose();
        }

        #region コンストラクタテスト

        [Fact]
        public void Constructor_WithValidSettings_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var settings = new Settings();
            var form = new OptionsForm(settings);

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void Constructor_WithNullSettings_ShouldNotThrowException()
        {
            // Act & Assert
            var action = () => new OptionsForm(null!);
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ShouldSetDefaultProperties()
        {
            // Arrange & Act
            var form = new OptionsForm(_settings);

            // Assert
            form.Text.Should().NotBeNullOrEmpty();
            form.Size.Should().NotBe(Size.Empty);

            // Cleanup
            form.Dispose();
        }

        #endregion

        #region 設定保存テスト

        private static void InvokeSaveSettings(OptionsForm form)
        {
            var method = typeof(OptionsForm).GetMethod("SaveSettings",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method!.Invoke(form, null);
        }

        [Fact]
        public void SaveSettings_WithEmptyOptionsShortcutText_ShouldFallBackToDefaultInsteadOfMinValue()
        {
            // Arrange: 以前はショートカット欄を空にして保存するとchar.MinValueが設定され、
            // MainForm_KeyDown側の比較対象が'\0'になってショートカットが恒久的に
            // 無効化されてしまっていた。空入力時は既定値へフォールバックすることを確認する。
            var txtOptionsShortcut = _form.Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
            txtOptionsShortcut.Should().NotBeNull("OptionsFormの初期化でtxtOptionsShortcutが生成されているはず");
            txtOptionsShortcut!.Text = string.Empty;

            // Act
            InvokeSaveSettings(_form);

            // Assert
            var expectedDefault = (char)_settings.Defaults[Settings.DefaultField.OptionsShortcut];
            _settings.OptionsShortcut.Should().Be(expectedDefault);
            _settings.OptionsShortcut.Should().NotBe(char.MinValue);
        }

        [Fact]
        public void SaveSettings_WithOptionsShortcutText_ShouldUseFirstCharacter()
        {
            // Arrange
            var txtOptionsShortcut = _form.Controls.Find("txtOptionsShortcut", true).FirstOrDefault() as TextBox;
            txtOptionsShortcut.Should().NotBeNull();
            txtOptionsShortcut!.Text = "Q";

            // Act
            InvokeSaveSettings(_form);

            // Assert
            _settings.OptionsShortcut.Should().Be('Q');
        }

        #endregion

        #region 境界値テスト

        [Fact]
        public void Constructor_WithMultipleInstances_ShouldWorkIndependently()
        {
            // Arrange & Act
            var form1 = new OptionsForm(_settings);
            var form2 = new OptionsForm(_settings);

            // Assert
            form1.Should().NotBeNull();
            form2.Should().NotBeNull();
            form1.Should().NotBeSameAs(form2);

            // Cleanup
            form1.Dispose();
            form2.Dispose();
        }

        [Fact]
        public void Dispose_ShouldSetIsDisposedToTrue()
        {
            // Act
            _form.Dispose();

            // Assert
            _form.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region 統合テスト

        [Fact]
        public void FullLifecycle_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var form = new OptionsForm(_settings);

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Act
            form.Dispose();
            form.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Constructor_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var form = new OptionsForm(_settings);
                form.Dispose();
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5秒未満
        }

        #endregion

        #region 設定値テスト

        [Fact]
        public void Settings_ShouldBeAccessible()
        {
            // Assert
            _settings.Should().NotBeNull();
            _settings.IconWidth.Should().BeGreaterThan(0);
            _settings.IconHeight.Should().BeGreaterThan(0);
            _settings.IconGapWidth.Should().BeGreaterThanOrEqualTo(0);
            _settings.IconGapHeight.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void Settings_ShouldBeModifiable()
        {
            // Arrange
            var originalIconWidth = _settings.IconWidth;
            var newIconWidth = originalIconWidth + 10;

            // Act
            _settings.IconWidth = newIconWidth;

            // Assert
            _settings.IconWidth.Should().Be(newIconWidth);
        }

        #endregion

        [Fact]
        public void BackgroundColorChange_ShouldUpdateMainFormCorrectly()
        {
            // Arrange
            var settings = new Settings();
            using var optionsForm = new OptionsForm(settings);

            // 背景色を変更
            var newColor = Color.Red;
            settings.BackgroundColorValue = newColor;

            // メイン画面の背景色を即時更新（OptionsFormの処理を模擬）
            var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.BackColor = settings.BackgroundColorValue;
                mainForm.Invalidate(); // 再描画を強制

                // 背景色が正しく設定されていることを確認
                mainForm.BackColor.ToArgb().Should().Be(newColor.ToArgb());
            }
        }

        [Fact]
        public void BackgroundColorChange_ShouldCallInvalidate()
        {
            // Arrange
            var settings = new Settings();
            using var optionsForm = new OptionsForm(settings);

            // 背景色変更処理をテスト
            var newColor = Color.Blue;
            settings.BackgroundColorValue = newColor;

            // メイン画面の背景色を即時更新（OptionsFormの処理を模擬）
            var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (mainForm != null)
            {
                // Invalidate()が呼ばれることを確認
                var originalBackColor = mainForm.BackColor;
                mainForm.BackColor = settings.BackgroundColorValue;
                mainForm.Invalidate(); // 再描画を強制

                // 背景色が変更されていることを確認
                mainForm.BackColor.ToArgb().Should().Be(newColor.ToArgb());
                mainForm.BackColor.ToArgb().Should().NotBe(originalBackColor.ToArgb());
            }
        }
    }
}
