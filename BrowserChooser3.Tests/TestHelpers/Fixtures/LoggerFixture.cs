using System;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// <see cref="Logger"/> の static なログレベル状態を退避・復元するフィクスチャ。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Logger.CurrentLogLevel"/> と内部の「初期化済み」フラグは static であり、
    /// <see cref="Logger.InitializeLogLevel()"/> は一度初期化したら再初期化しない設計になっている。
    /// このため、LoggerTests が先に走ったか ProgramTests が先に走ったかで
    /// 後続テストの観測するログレベルが変わるという実行順依存が発生していた。
    /// </para>
    /// <para>
    /// このフィクスチャは両方の状態を退避し、<see cref="Dispose"/> で必ず戻すことで
    /// その順序依存を断ち切る。
    /// </para>
    /// </remarks>
    public sealed class LoggerFixture : IDisposable
    {
        private readonly Logger.LogLevel _originalLevel;
        private readonly bool _originalInitialized;
        private bool _disposed;

        /// <summary>
        /// 現在のログレベルと初期化フラグを退避します。
        /// </summary>
        public LoggerFixture()
        {
            _originalLevel = Logger.CurrentLogLevel;
            _originalInitialized = Logger.IsLogLevelInitializedForTests;
        }

        /// <summary>
        /// ログレベルを指定の値に設定します（初期化済みフラグは変更しません）。
        /// </summary>
        /// <param name="level">設定するログレベル</param>
        public void SetLevel(Logger.LogLevel level)
        {
            Logger.CurrentLogLevel = level;
        }

        /// <summary>
        /// 退避しておいたログレベルと初期化フラグを復元します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Logger.RestoreLogLevelStateForTests(_originalLevel, _originalInitialized);
        }
    }
}
