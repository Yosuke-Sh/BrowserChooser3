using Xunit;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// 実際の設定ファイル（%APPDATA%\BrowserChooser3\BrowserChooser3Config.xml）や
    /// <see cref="BrowserChooser3.Classes.Settings.Current"/> に触れるテストクラスをまとめるコレクション。
    /// </summary>
    /// <remarks>
    /// xUnitは既定でテストクラス間を並列実行するため、同じ実ファイル・同じstaticを
    /// 触るクラス同士が競合していた（Phase 1 から記録されていたフレーキネスの原因）。
    /// このコレクションに属するクラスは互いに直列化される。
    /// </remarks>
    [CollectionDefinition(Name)]
    public sealed class SettingsStateCollection
    {
        /// <summary>コレクション名</summary>
        public const string Name = "SettingsState";
    }
}
