using BrowserChooser3.Classes.Utilities;
using FluentAssertions;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// TestEnvironmentDetectorクラスのテスト
    /// </summary>
    public class TestEnvironmentDetectorTests
    {
        [Fact]
        public void IsTestEnvironment_WhenRunningInTest_ShouldReturnTrue()
        {
            // Act
            var result = TestEnvironmentDetector.IsTestEnvironment();

            // Assert
            result.Should().BeTrue("テスト環境で実行されているため");
        }

        [Fact]
        public void GuardAgainstTestEnvironment_WhenInTestEnvironment_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var operationName = "テスト操作";

            // Act
            var action = () => TestEnvironmentDetector.GuardAgainstTestEnvironment(operationName);

            // Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"テスト環境では{operationName}を実行できません。");
        }

        [Fact]
        public void GuardAgainstTestEnvironment_WithNullOperationName_ShouldThrowInvalidOperationException()
        {
            // Act
            var action = () => TestEnvironmentDetector.GuardAgainstTestEnvironment(null!);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GuardAgainstTestEnvironment_WithEmptyOperationName_ShouldThrowInvalidOperationException()
        {
            // Act
            var action = () => TestEnvironmentDetector.GuardAgainstTestEnvironment("");

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }
    }
}
