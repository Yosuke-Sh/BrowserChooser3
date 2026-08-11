using System;
using System.Threading;
using BrowserChooser3.Classes.Services.SystemServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// SingleInstanceManagerクラスのテスト
    /// 実プロセスを増殖させないよう、テストごとに一意なMutex名・パイプ名を使用する
    /// </summary>
    public class SingleInstanceManagerTests
    {
        private static (string mutexName, string pipeName) CreateUniqueNames()
        {
            var suffix = Guid.NewGuid().ToString("N");
            return ($"Local\\BC3_Test_Mutex_{suffix}", $"BC3_Test_Pipe_{suffix}");
        }

        [Fact]
        public void TryAcquire_WhenNoOtherInstance_ShouldReturnTrue()
        {
            // Arrange
            var (mutexName, pipeName) = CreateUniqueNames();
            using var manager = new SingleInstanceManager(mutexName, pipeName);

            // Act
            var acquired = manager.TryAcquire();

            // Assert
            acquired.Should().BeTrue();
        }

        [Fact]
        public void TryAcquire_WhenAnotherInstanceHoldsMutex_ShouldReturnFalse()
        {
            // Arrange
            var (mutexName, pipeName) = CreateUniqueNames();
            using var first = new SingleInstanceManager(mutexName, pipeName);
            using var second = new SingleInstanceManager(mutexName, pipeName);

            // Act
            var firstAcquired = first.TryAcquire();
            var secondAcquired = second.TryAcquire();

            // Assert
            firstAcquired.Should().BeTrue();
            secondAcquired.Should().BeFalse();
        }

        [Fact]
        public void TrySendUrlToExistingInstance_WhenOwnerIsListening_ShouldDeliverUrl()
        {
            // Arrange
            var (mutexName, pipeName) = CreateUniqueNames();
            using var owner = new SingleInstanceManager(mutexName, pipeName);
            owner.TryAcquire().Should().BeTrue();

            string? receivedUrl = null;
            using var receivedSignal = new ManualResetEventSlim(false);
            owner.UrlReceived += url =>
            {
                receivedUrl = url;
                receivedSignal.Set();
            };

            // Act
            var sent = SingleInstanceManager.TrySendUrlToExistingInstance("https://example.com", pipeName);
            var signaled = receivedSignal.Wait(TimeSpan.FromSeconds(5));

            // Assert
            sent.Should().BeTrue();
            signaled.Should().BeTrue();
            receivedUrl.Should().Be("https://example.com");
        }

        [Fact]
        public void TrySendUrlToExistingInstance_WhenNoOwnerListening_ShouldReturnFalse()
        {
            // Arrange
            var (_, pipeName) = CreateUniqueNames();

            // Act
            var sent = SingleInstanceManager.TrySendUrlToExistingInstance("https://example.com", pipeName);

            // Assert
            sent.Should().BeFalse();
        }

        [Fact]
        public void Dispose_ShouldNotThrow()
        {
            // Arrange
            var (mutexName, pipeName) = CreateUniqueNames();
            var manager = new SingleInstanceManager(mutexName, pipeName);
            manager.TryAcquire();

            // Act & Assert
            var action = () => manager.Dispose();
            action.Should().NotThrow();
        }
    }
}
