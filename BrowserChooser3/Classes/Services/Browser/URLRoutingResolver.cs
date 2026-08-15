using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.BrowserServices
{
    /// <summary>
    /// URLルーティングの判定結果の種類
    /// </summary>
    public enum URLRoutingKind
    {
        /// <summary>どのルールにもマッチせず、ブラウザ選択画面が表示される</summary>
        NoMatch,

        /// <summary>AutoURLのルールにマッチした</summary>
        AutoUrl,

        /// <summary>プロトコルのルールにマッチした</summary>
        Protocol,

        /// <summary>
        /// ルールにはマッチしたが、指定されたブラウザが設定に存在しない。
        /// 実行時はこのルールが読み飛ばされ、以降のルール評価が続く。
        /// </summary>
        MatchedButBrowserMissing
    }

    /// <summary>
    /// URLルーティングの判定結果
    /// </summary>
    /// <param name="Kind">判定の種類</param>
    /// <param name="MatchedPattern">マッチしたパターン（AutoURLのパターンまたはプロトコルヘッダー）</param>
    /// <param name="RuleName">マッチしたルールの名前</param>
    /// <param name="Browser">起動されるブラウザ（該当しない場合はnull）</param>
    /// <param name="DelaySeconds">起動までの遅延秒数（AutoURL以外は0）</param>
    /// <param name="ForceAutoClose">
    /// ルール側で自動終了が指定されている場合はtrue。
    /// <see cref="Models.URL.AutoClose"/> は保存されるだけで参照されていなかったため、
    /// 有効なときだけメイン画面のチェックボックスより優先させる。
    /// </param>
    public record URLRoutingResult(
        URLRoutingKind Kind,
        string MatchedPattern,
        string RuleName,
        Browser? Browser,
        int DelaySeconds,
        bool ForceAutoClose = false)
    {
        /// <summary>どのルールにもマッチしなかったことを表す結果を生成します。</summary>
        public static URLRoutingResult NoMatch()
            => new(URLRoutingKind.NoMatch, string.Empty, string.Empty, null, 0);
    }

    /// <summary>
    /// 入力されたURLがどのルール（AutoURL / プロトコル）にマッチし、
    /// どのブラウザで開かれるかを判定します。
    ///
    /// MainForm の実際の起動処理（ProcessAutoURLsInternal / ProcessProtocols）と
    /// Options の URL テスト欄（3-8）がこの1つの実装を共有することで、
    /// 「プレビューでは選択画面が出ると表示されたのに実際は自動起動した」という
    /// 食い違いが起きないようにしている。誤爆事故の再発防止が目的のため、
    /// プレビューが実挙動と一致していることが最も重要。
    /// </summary>
    public static class URLRoutingResolver
    {
        /// <summary>
        /// URLに対して適用されるルーティングを判定します。
        /// 優先順位は AutoURLs → プロトコル で、MainForm の実処理と同じ順序です。
        /// </summary>
        /// <param name="settings">判定に使用する設定</param>
        /// <param name="url">判定対象のURL</param>
        /// <returns>判定結果</returns>
        public static URLRoutingResult Resolve(Settings? settings, string? url)
        {
            if (settings == null || string.IsNullOrWhiteSpace(url))
            {
                return URLRoutingResult.NoMatch();
            }

            var autoUrlResult = ResolveAutoUrl(settings, url);
            if (autoUrlResult != null) return autoUrlResult;

            var protocolResult = ResolveProtocol(settings, url);
            if (protocolResult != null) return protocolResult;

            return URLRoutingResult.NoMatch();
        }

        /// <summary>
        /// AutoURLのルールを評価します。
        /// </summary>
        /// <param name="settings">判定に使用する設定</param>
        /// <param name="url">判定対象のURL</param>
        /// <returns>マッチした場合は結果、しなかった場合はnull</returns>
        private static URLRoutingResult? ResolveAutoUrl(Settings settings, string url)
        {
            if (settings.URLs == null) return null;

            foreach (var autoUrl in settings.URLs)
            {
                if (!autoUrl.IsActive) continue;
                if (string.IsNullOrEmpty(autoUrl.URLPattern)) continue;
                if (!URLUtilities.MatchURLs(url, autoUrl.URLPattern)) continue;

                var browser = settings.Browsers?.FirstOrDefault(b => b.Guid == autoUrl.BrowserGuid);
                if (browser == null)
                {
                    // 実処理では読み飛ばして次のルールへ進むが、設定の不備として
                    // プレビューには「ブラウザが見つからない」と出す価値があるため、
                    // 最初に見つかったこのケースを結果として返す。
                    return new URLRoutingResult(
                        URLRoutingKind.MatchedButBrowserMissing,
                        autoUrl.URLPattern,
                        autoUrl.Name,
                        null,
                        0);
                }

                var delay = autoUrl.Delay < 0 ? settings.DefaultDelay : autoUrl.Delay;
                return new URLRoutingResult(
                    URLRoutingKind.AutoUrl,
                    autoUrl.URLPattern,
                    autoUrl.Name,
                    browser,
                    delay,
                    autoUrl.AutoClose);
            }

            return null;
        }

        /// <summary>
        /// プロトコルのルールを評価します。
        /// </summary>
        /// <param name="settings">判定に使用する設定</param>
        /// <param name="url">判定対象のURL</param>
        /// <returns>マッチした場合は結果、しなかった場合はnull</returns>
        private static URLRoutingResult? ResolveProtocol(Settings settings, string url)
        {
            if (settings.Protocols == null) return null;

            var protocol = ExtractProtocol(url);
            if (string.IsNullOrEmpty(protocol)) return null;

            foreach (var protocolSetting in settings.Protocols)
            {
                if (!protocolSetting.IsActive) continue;
                if (string.IsNullOrEmpty(protocolSetting.Header)) continue;
                if (!protocol.Equals(protocolSetting.Header, StringComparison.OrdinalIgnoreCase)) continue;

                var browser = settings.Browsers?.FirstOrDefault(b => b.Guid == protocolSetting.BrowserGuid);
                if (browser == null)
                {
                    return new URLRoutingResult(
                        URLRoutingKind.MatchedButBrowserMissing,
                        protocolSetting.Header,
                        protocolSetting.Name,
                        null,
                        0);
                }

                // プロトコル経路は遅延なしで即座に起動する
                return new URLRoutingResult(
                    URLRoutingKind.Protocol,
                    protocolSetting.Header,
                    protocolSetting.Name,
                    browser,
                    0);
            }

            return null;
        }

        /// <summary>
        /// URLからプロトコル（スキーム）部分を抽出します。
        ///
        /// "://" ではなく最初の ':' までを見るため、mailto: のように
        /// スラッシュを伴わないスキームも抽出できます（従来の
        /// MainForm.ExtractProtocolFromUrl と同じ挙動）。
        /// </summary>
        /// <param name="url">対象のURL</param>
        /// <returns>抽出されたプロトコル。抽出できない場合は空文字</returns>
        public static string ExtractProtocol(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            var index = url.IndexOf(':');
            return index > 0 ? url.Substring(0, index) : string.Empty;
        }

        /// <summary>
        /// 判定結果を、Options の URL テスト欄に表示する説明文へ整形します。
        /// </summary>
        /// <param name="result">判定結果</param>
        /// <returns>ユーザー向けの説明文</returns>
        public static string DescribeResult(URLRoutingResult result)
        {
            return result.Kind switch
            {
                URLRoutingKind.AutoUrl when result.DelaySeconds > 0
                    => $"AutoURL「{DescribeRuleName(result)}」（パターン: {result.MatchedPattern}）にマッチし、" +
                       $"{result.DelaySeconds}秒後に {result.Browser?.Name} で自動的に開きます。",

                URLRoutingKind.AutoUrl
                    => $"AutoURL「{DescribeRuleName(result)}」（パターン: {result.MatchedPattern}）にマッチし、" +
                       $"{result.Browser?.Name} で即座に自動的に開きます。",

                URLRoutingKind.Protocol
                    => $"プロトコル「{result.MatchedPattern}」のルールにマッチし、" +
                       $"{result.Browser?.Name} で即座に自動的に開きます。",

                URLRoutingKind.MatchedButBrowserMissing
                    => $"ルール「{DescribeRuleName(result)}」（パターン: {result.MatchedPattern}）にマッチしましたが、" +
                       "指定されたブラウザが設定に存在しません。このルールは無視されます。",

                _ => "どのルールにもマッチしません。ブラウザ選択画面が表示されます。"
            };
        }

        /// <summary>
        /// 名前が未設定のルールでも表示が空にならないようにします。
        /// </summary>
        /// <param name="result">判定結果</param>
        /// <returns>表示用のルール名</returns>
        private static string DescribeRuleName(URLRoutingResult result)
            => string.IsNullOrWhiteSpace(result.RuleName) ? "(名前なし)" : result.RuleName;
    }
}
