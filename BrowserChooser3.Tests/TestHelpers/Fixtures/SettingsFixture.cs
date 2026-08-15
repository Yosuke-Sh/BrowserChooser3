using System;
using BrowserChooser3.Classes;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// <see cref="Settings.Current"/> を退避・復元するフィクスチャ。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Settings.Current"/> は static であり、テスト間で共有される。
    /// これを読む本番コード（MainForm、URLRoutingResolver、URLSanitizer など）は、
    /// 何も手当てしなければ「前のテストが残した設定」に対して実行されてしまう。
    /// </para>
    /// <para>
    /// このフィクスチャは構築時に現在値を退避し、<see cref="Dispose"/> で必ず戻す。
    /// テストクラスからは <c>IClassFixture&lt;SettingsFixture&gt;</c> で利用するか、
    /// テストメソッド内で <c>using</c> して利用する。
    /// </para>
    /// </remarks>
    public sealed class SettingsFixture : IDisposable
    {
        private readonly Settings _original;
        private bool _disposed;

        /// <summary>
        /// 現在の <see cref="Settings.Current"/> を退避し、まっさらな設定に差し替えます。
        /// </summary>
        public SettingsFixture()
        {
            _original = Settings.Current;
            Settings.Current = new Settings();
        }

        /// <summary>
        /// このフィクスチャが差し替えた設定インスタンス。
        /// </summary>
        public Settings Current => Settings.Current;

        /// <summary>
        /// <see cref="Settings.Current"/> を指定の設定へ差し替えます。
        /// <see cref="Dispose"/> 時には退避しておいた元の設定へ戻ります。
        /// </summary>
        /// <param name="settings">差し替える設定</param>
        public void Replace(Settings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            Settings.Current = settings;
        }

        /// <summary>
        /// 退避しておいた <see cref="Settings.Current"/> を復元します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Settings.Current = _original;
        }
    }
}
