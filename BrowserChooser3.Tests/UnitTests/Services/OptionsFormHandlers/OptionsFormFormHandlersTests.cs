using System;
using System.Windows.Forms;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormFormHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// 従来28件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由に全てスキップされていた。
    /// IOptionsFormContextの導入により、保存時にDialogResultとCloseが実際に
    /// 呼ばれること、変更が無ければ確認ダイアログ経路に入らないことなどを検証できる。
    ///
    /// 注意: OptionsForm_FormClosing は変更ありの場合に実際のMessageBoxを表示するため、
    /// 「変更なし」経路のみをテストする（テスト実行が無応答のモーダルで停止するのを避ける。
    /// Phase 3で実際に発生した事象）。
    /// </remarks>
    public class OptionsFormFormHandlersTests
    {
        [Fact]
        public void SaveButton_Click_ShouldSaveSettingsAndCloseWithOk()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var saveCalled = 0;
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => { },
                saveSettings: () => saveCalled++,
                getIsModified: () => false);

            // Act
            handlers.SaveButton_Click(null, EventArgs.Empty);

            // Assert
            saveCalled.Should().Be(1, "保存処理が1回だけ呼ばれる");
            context.DialogResult.Should().Be(DialogResult.OK);
            context.CloseCallCount.Should().Be(1, "保存後にフォームが閉じられる");
        }

        [Fact]
        public void SaveButton_Click_WhenSaveThrows_ShouldNotCloseForm()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => { },
                saveSettings: () => throw new InvalidOperationException("保存失敗"),
                getIsModified: () => false);

            // Act
            // 例外はハンドラー内で捕捉される（ユーザーにはダイアログで通知される）
            handlers.SaveButton_Click(null, EventArgs.Empty);

            // Assert
            // 保存に失敗した以上、OKとして閉じてはならない
            context.DialogResult.Should().NotBe(DialogResult.OK);
            context.CloseCallCount.Should().Be(0);
        }

        [Fact]
        public void OptionsForm_Shown_ShouldLoadSettingsToControls()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var loadCalled = 0;
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => loadCalled++,
                saveSettings: () => { },
                getIsModified: () => false);

            // Act
            handlers.OptionsForm_Shown(null, EventArgs.Empty);

            // Assert
            loadCalled.Should().Be(1);
        }

        [Fact]
        public void OptionsForm_Shown_WhenLoadThrows_ShouldSwallowException()
        {
            // Arrange
            // 表示時の読み込み失敗でフォーム表示自体が落ちないことを保証する
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => throw new InvalidOperationException("読み込み失敗"),
                saveSettings: () => { },
                getIsModified: () => false);

            // Act
            var action = () => handlers.OptionsForm_Shown(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void OptionsForm_FormClosing_WhenNotModified_ShouldNotSaveOrCancel()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var saveCalled = 0;
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => { },
                saveSettings: () => saveCalled++,
                getIsModified: () => false);

            var args = new FormClosingEventArgs(CloseReason.UserClosing, false);

            // Act
            handlers.OptionsForm_FormClosing(null, args);

            // Assert
            // 変更が無ければ確認ダイアログを出さず、保存もキャンセルもしない
            saveCalled.Should().Be(0);
            args.Cancel.Should().BeFalse();
        }

        [Fact]
        public void OptionsForm_FormClosing_WhenIsModifiedThrows_ShouldNotCancelClose()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormFormHandlers(
                context,
                loadSettingsToControls: () => { },
                saveSettings: () => { },
                getIsModified: () => throw new InvalidOperationException("判定失敗"));

            var args = new FormClosingEventArgs(CloseReason.UserClosing, false);

            // Act
            handlers.OptionsForm_FormClosing(null, args);

            // Assert
            // 例外は捕捉され、閉じる操作を妨げない（閉じられなくなる方が有害）
            args.Cancel.Should().BeFalse();
        }
    }
}
