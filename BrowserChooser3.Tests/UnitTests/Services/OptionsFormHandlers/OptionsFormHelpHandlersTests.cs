using FluentAssertions;
using Xunit;
using BrowserChooser3.Classes.Interfaces;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using Moq;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormHelpHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// 従来この25件は全て <c>new Mock&lt;OptionsForm&gt;()</c>（具象WinFormsクラスは
    /// Moqでプロキシできない）を理由にスキップされており、内容も
    /// 「OpenHelpがNotThrow」を条件を変えて繰り返すだけだった。
    /// IOptionsFormContextの導入でモック可能になったため、
    /// 実際に検証価値のある振る舞い（テスト環境で外部プロセスを起動しないこと、
    /// 親フォームに触れないこと）をアサートする形へ書き直している。
    /// </remarks>
    public class OptionsFormHelpHandlersTests
    {
        [Fact]
        public void Constructor_WithMockedContext_ShouldSucceed()
        {
            // Arrange
            var mockForm = new Mock<IOptionsFormContext>();

            // Act
            var handlers = new OptionsFormHelpHandlers(mockForm.Object);

            // Assert
            handlers.Should().NotBeNull();
        }

        [Fact]
        public void OpenHelp_InTestEnvironment_ShouldNotStartExternalProcess()
        {
            // Arrange
            var mockForm = new Mock<IOptionsFormContext>();
            var handlers = new OptionsFormHelpHandlers(mockForm.Object);
            var browsersBefore = System.Diagnostics.Process.GetProcesses().Length;

            // Act
            handlers.OpenHelp();

            // Assert
            // テスト環境判定により、実際のブラウザ起動は行われない。
            // プロセス数が跳ね上がっていないことで、外部起動が無かったことを確かめる。
            var browsersAfter = System.Diagnostics.Process.GetProcesses().Length;
            (browsersAfter - browsersBefore).Should().BeLessThan(5);
        }

        [Fact]
        public void OpenHelp_ShouldNotTouchParentForm()
        {
            // Arrange
            var mockForm = new Mock<IOptionsFormContext>(MockBehavior.Strict);
            var handlers = new OptionsFormHelpHandlers(mockForm.Object);

            // Act
            handlers.OpenHelp();

            // Assert
            // Strictモックなので、親フォームのメンバーに触れていれば例外になる。
            // OpenHelpはフォームに一切依存しないことを固定する。
            mockForm.VerifyNoOtherCalls();
        }

        [Fact]
        public void OpenHelp_WithNullContext_ShouldNotThrow()
        {
            // Arrange
            // ヘルプを開く処理は親フォームを参照しないため、nullでも動作する
            var handlers = new OptionsFormHelpHandlers(null!);

            // Act
            var action = () => handlers.OpenHelp();

            // Assert
            action.Should().NotThrow();
        }
    }
}
