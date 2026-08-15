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

    /// <summary>
    /// <see cref="BrowserChooser3.Classes.Services.BrowserServices.BrowserDetector.DetectedBrowsers"/>
    /// （プロセス寿命の static List）に触れるテストクラスをまとめるコレクション。
    /// </summary>
    /// <remarks>
    /// <see cref="BrowserChooser3.Classes.Services.BrowserServices.BrowserDetector.DetectBrowsers"/> は
    /// 呼び出しのたびに共有リストを <c>Clear()</c> してから再構築するため、
    /// 他クラスと並列に実行されると「呼び出した瞬間に別テストの追加結果が消える／
    /// 列挙中にCollection was modifiedが飛ぶ」といった競合が起きる。
    /// このコレクションに属するクラスは互いに直列化される。
    /// </remarks>
    [CollectionDefinition(Name)]
    public sealed class BrowserDetectorStateCollection
    {
        /// <summary>コレクション名</summary>
        public const string Name = "BrowserDetectorState";
    }
}
