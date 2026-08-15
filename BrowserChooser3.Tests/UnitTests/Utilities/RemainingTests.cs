using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Tests;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// 残りのクラスのテスト
    /// </summary>
    /// <remarks>
    /// Policy.Initialize()/Reset()を並行アクセスの検証で激しく書き換えるテストを
    /// 含むため、同じPolicy静的状態を検証するPolicyTestsとPolicyStateCollectionで
    /// 直列化する。
    /// </remarks>
    [Collection(PolicyStateCollection.Name)]
    public class RemainingTests : IDisposable
    {
        public void Dispose()
        {
            // テスト後のクリーンアップ
        }

        #region TestConfigテスト

        [Fact]
        public void TestConfig_IsTestEnvironment_ShouldReturnBoolean()
        {
            // Act
            var result = TestConfig.IsTestEnvironment();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestConfig_IsTestEnvironment_ShouldBeConsistent()
        {
            // Act
            var result1 = TestConfig.IsTestEnvironment();
            var result2 = TestConfig.IsTestEnvironment();

            // Assert
            result1.Should().Be(result2);
        }

        #endregion

        #region Loggerテスト

        [Fact]
        public void Logger_LogMethods_WithVariousMessages_ShouldNotThrowException()
        {
            // Arrange - null/空/特殊文字/Unicode/長大メッセージをまとめて検証する。
            // テスト環境ではAddToLogが早期リターンするため、観測可能な副作用がなく、
            // 個別テストに分ける意味がないためひとつにまとめている。
            var messages = new[]
            {
                "Test message",
                "",
                null!,
                "Test\n\r\t\0message",
                "テストメッセージ",
                new string('x', 10000)
            };

            // Act & Assert
            var action = () =>
            {
                foreach (var message in messages)
                {
                    Logger.LogInfo("TestClass", message);
                    Logger.LogError("TestClass", message);
                    Logger.LogTrace("TestClass", message);
                }
            };
            action.Should().NotThrow();
        }

        [Fact]
        public void Logger_CurrentLogLevel_ShouldBeAccessible()
        {
            // Act
            var result = Logger.CurrentLogLevel;

            // Assert
            result.Should().BeOneOf(Logger.LogLevel.None, Logger.LogLevel.Error, Logger.LogLevel.Warning, Logger.LogLevel.Info, Logger.LogLevel.Debug, Logger.LogLevel.Trace);
        }

        [Fact]
        public void Logger_CurrentLogLevel_ShouldBeConsistent()
        {
            // Act
            var result1 = Logger.CurrentLogLevel;
            var result2 = Logger.CurrentLogLevel;

            // Assert
            result1.Should().Be(result2);
        }

        #endregion

        #region スレッドセーフテスト

        [Fact]
        public async Task RemainingClasses_ShouldBeThreadSafe()
        {
            // Act & Assert - TestConfig/Logger/Policyへの並行アクセスが例外を起こさないことを確認する
            var tasks = new List<Task>
            {
                Task.Run(() => {
                    for (int i = 0; i < 100; i++)
                    {
                        TestConfig.IsTestEnvironment();
                        var _ = Policy.IconScale;
                    }
                }),
                Task.Run(() => {
                    for (int i = 0; i < 100; i++)
                    {
                        Logger.LogInfo("TestClass", $"Thread message {i}");
                        Logger.LogError("TestClass", $"Thread error {i}");
                    }
                }),
                Task.Run(() => {
                    for (int i = 0; i < 10; i++)
                    {
                        Policy.Initialize();
                        Policy.Reset();
                    }
                })
            };

            await Task.WhenAll(tasks);
        }

        #endregion

        #region 一貫性テスト

        [Fact]
        public void RemainingClasses_ShouldBeConsistent()
        {
            // Act
            var testEnv1 = TestConfig.IsTestEnvironment();
            var testEnv2 = TestConfig.IsTestEnvironment();
            var iconScale1 = Policy.IconScale;
            var iconScale2 = Policy.IconScale;

            // Assert
            testEnv1.Should().Be(testEnv2);
            iconScale1.Should().Be(iconScale2);
        }

        [Fact]
        public void RemainingClasses_ShouldMaintainState()
        {
            // Arrange
            var initialIconScale = Policy.IconScale;

            // Act
            Policy.Initialize();
            var afterInitIconScale = Policy.IconScale;
            Policy.Reset();
            var afterResetIconScale = Policy.IconScale;

            // Assert
            // Policy.Initialize()でIconScaleが変更される可能性があるため、初期値との比較は行わない
            afterResetIconScale.Should().Be(initialIconScale, "Reset後は初期値に戻るため");
        }

        #endregion
    }
}
