using System;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormCheckBoxHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// 従来26件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由にスキップされていた。
    /// IOptionsFormContextの抽出により、チェック状態が実際に設定オブジェクトや
    /// 関連コントロールへ反映されることを検証できるようになった。
    /// </remarks>
    public class OptionsFormCheckBoxHandlersTests
    {
        [Fact]
        public void DetectDirty_ShouldSetModifiedFlag()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            bool? modified = null;
            var handlers = new OptionsFormCheckBoxHandlers(context, new Settings(), v => modified = v);

            // Act
            handlers.DetectDirty(new object(), EventArgs.Empty);

            // Assert
            modified.Should().BeTrue();
        }

        [Fact]
        public void ChkCanonicalize_CheckedChanged_WhenChecked_ShouldEnableAppendTextBox()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var textBox = new TextBox { Name = "txtCanonicalizeAppend", Enabled = false };
            context.Controls.Add(textBox);

            var handlers = new OptionsFormCheckBoxHandlers(context, new Settings(), _ => { });
            using var checkBox = new CheckBox { Checked = true };

            // Act
            handlers.ChkCanonicalize_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            textBox.Enabled.Should().BeTrue("正規化が有効なら追記テキスト欄も入力可能になる");
        }

        [Fact]
        public void ChkCanonicalize_CheckedChanged_WhenUnchecked_ShouldDisableAppendTextBox()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var textBox = new TextBox { Name = "txtCanonicalizeAppend", Enabled = true };
            context.Controls.Add(textBox);

            var handlers = new OptionsFormCheckBoxHandlers(context, new Settings(), _ => { });
            using var checkBox = new CheckBox { Checked = false };

            // Act
            handlers.ChkCanonicalize_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            textBox.Enabled.Should().BeFalse();
        }

        [Fact]
        public void ChkCanonicalize_CheckedChanged_ShouldAlwaysSetModifiedFlag()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            bool? modified = null;
            var handlers = new OptionsFormCheckBoxHandlers(context, new Settings(), v => modified = v);
            using var checkBox = new CheckBox { Checked = true };

            // Act
            handlers.ChkCanonicalize_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            // 対象のテキストボックスが無くても変更フラグは立つ
            modified.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ChkLog_CheckedChanged_ShouldWriteEnableLoggingToSettings(bool isChecked)
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var settings = new Settings { EnableLogging = !isChecked };
            var handlers = new OptionsFormCheckBoxHandlers(context, settings, _ => { });
            using var checkBox = new CheckBox { Checked = isChecked };

            // Act
            handlers.ChkLog_CheckedChanged(checkBox, EventArgs.Empty);

            // Assert
            settings.EnableLogging.Should().Be(isChecked);
        }

        [Fact]
        public void ChkLog_CheckedChanged_WithNonCheckBoxSender_ShouldNotChangeSettings()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var settings = new Settings { EnableLogging = true };
            var handlers = new OptionsFormCheckBoxHandlers(context, settings, _ => { });

            // Act
            handlers.ChkLog_CheckedChanged(new object(), EventArgs.Empty);

            // Assert
            settings.EnableLogging.Should().BeTrue("senderがCheckBoxでない場合は設定を変更しない");
        }
    }
}
