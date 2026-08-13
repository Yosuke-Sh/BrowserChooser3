using System.Net;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// URLの解析と処理を行うユーティリティクラス
    /// </summary>
    public static class URLUtilities
    {


        /// <summary>
        /// URLが有効かどうかをチェックします
        /// </summary>
        /// <param name="url">チェック対象のURL</param>
        /// <returns>有効な場合はtrue</returns>
        public static bool IsValidURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // 基本的なURLパターンチェック（大文字小文字を区別しない）
            var lowerUrl = url.ToLowerInvariant();
            if (lowerUrl.StartsWith("http://") || lowerUrl.StartsWith("https://") || 
                lowerUrl.StartsWith("ftp://") || lowerUrl.StartsWith("file://"))
            {
                // 不完全なURL（スキームのみ）の場合は有効とみなす
                if (url.EndsWith("://"))
                    return true;

                try
                {
                    var uri = new Uri(url);
                    return uri.Scheme == Uri.UriSchemeHttp || 
                           uri.Scheme == Uri.UriSchemeHttps ||
                           uri.Scheme == Uri.UriSchemeFtp ||
                           uri.Scheme == Uri.UriSchemeFile;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// ファイルパスかどうかをチェックします
        /// </summary>
        /// <param name="path">チェック対象のパス</param>
        /// <returns>ファイルパスの場合はtrue</returns>
        public static bool IsFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // URLの場合はファイルパスではない
            if (path.StartsWith("http://") || path.StartsWith("https://") || 
                path.StartsWith("ftp://") || path.StartsWith("file://"))
                return false;

            return System.IO.Path.IsPathRooted(path) || 
                   path.Contains('\\') || 
                   path.Contains('/');
        }

        /// <summary>
        /// URLを正規化します
        /// </summary>
        /// <param name="url">正規化対象のURL</param>
        /// <returns>正規化されたURL</returns>
        public static string CanonicalizeURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            try
            {
                var uri = new Uri(url);
                return uri.ToString();
            }
            catch
            {
                return url;
            }
        }

        /// <summary>
        /// URL短縮解除を実行します（バックグラウンド処理）
        /// </summary>
        /// <param name="url">短縮URL</param>
        /// <param name="userAgent">User-Agent文字列</param>
        /// <param name="callback">完了時のコールバック</param>
        public static void UnshortenURLAsync(string url, string userAgent, Action<string> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                callback?.Invoke(url);
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    var expandedUrl = UnshortenURL(url, userAgent);
                    callback?.Invoke(expandedUrl);
                }
                catch (Exception ex)
                {
                    Logger.LogError("URLUtilities.UnshortenURLAsync", "URL短縮解除エラー", ex.Message, ex.StackTrace ?? "");
                    callback?.Invoke(url); // エラーの場合は元のURLを返す
                }
            });
        }

        /// <summary>
        /// URL短縮解除を実行します（同期処理）
        /// </summary>
        /// <param name="url">短縮URL</param>
        /// <param name="userAgent">User-Agent文字列</param>
        /// <returns>展開されたURL</returns>
        public static string UnshortenURL(string url, string userAgent)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

                // HEADリクエストを試行
                var headRequest = new HttpRequestMessage(HttpMethod.Head, url);
                var headResponse = httpClient.Send(headRequest);
                
                // リダイレクト先のURLを取得
                if (headResponse.StatusCode == HttpStatusCode.Redirect || 
                    headResponse.StatusCode == HttpStatusCode.Moved || 
                    headResponse.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    var location = headResponse.Headers.Location?.ToString();
                    if (!string.IsNullOrEmpty(location))
                    {
                        Logger.LogInfo("URLUtilities.UnshortenURL", "URL短縮解除成功", url, location);
                        return location;
                    }
                }

                // HEADメソッドが失敗した場合、GETメソッドを試行
                var getResponse = httpClient.GetAsync(url).Result;
                var finalUrl = getResponse.RequestMessage?.RequestUri?.ToString() ?? url;
                
                Logger.LogInfo("URLUtilities.UnshortenURL", "URL短縮解除成功（GET）", url, finalUrl);
                return finalUrl;
            }
            catch (WebException ex)
            {
                Logger.LogError("URLUtilities.UnshortenURL", "WebException", ex.Message, ex.StackTrace ?? "");
                return url; // エラーの場合は元のURLを返す
            }
            catch (Exception ex)
            {
                Logger.LogError("URLUtilities.UnshortenURL", "URL短縮解除エラー", ex.Message, ex.StackTrace ?? "");
                return url; // エラーの場合は元のURLを返す
            }
        }





        /// <summary>
        /// URLマッチング（Browser Chooser 2互換）
        /// </summary>
        /// <param name="source">ソースURL</param>
        /// <param name="target">ターゲットURL</param>
        /// <returns>マッチする場合はtrue</returns>
        public static bool MatchURLs(string source, string target)
        {
            // nullチェック
            if (source == null && target == null)
                return true;
            if (source == null || target == null)
                return false;

            Logger.LogInfo("URLUtilities.MatchURLs", "Start", source, target);

            // パターンがre:で始まる場合は、残りの部分をユーザー指定の正規表現としてそのまま扱う
            if (target.StartsWith("re:", StringComparison.OrdinalIgnoreCase))
            {
                var pattern = target.Substring(3);
                var regexResult = MatchRegexPattern(source, pattern);
                Logger.LogInfo("URLUtilities.MatchURLs", "End (Regex)", source, target, regexResult);
                return regexResult;
            }

            // パターンが@で始まる場合は特別処理
            if (target.StartsWith("@"))
            {
                var pattern = target.Substring(1); // @を除去
                var result = MatchURLPattern(source, pattern);
                Logger.LogInfo("URLUtilities.MatchURLs", "End (Pattern)", source, target, result);
                return result;
            }
            
            // ワイルドカードパターンが含まれている場合は特別処理
            if (target.Contains("*"))
            {
                var result = MatchURLPattern(source, target);
                Logger.LogInfo("URLUtilities.MatchURLs", "End (Wildcard)", source, target, result);
                return result;
            }
            
            // ホスト（＋パス）ベースのマッチング
            var hostResult = MatchHostPattern(source, target);
            Logger.LogInfo("URLUtilities.MatchURLs", "End (Host)", source, target, hostResult);
            return hostResult;
        }

        /// <summary>
        /// ホスト名（必要に応じてパス）ベースでURLとパターンを照合します。
        /// パターンがホスト名のみの場合は完全一致またはサブドメイン一致、
        /// パスを含む場合は「ホスト＋パス」の前方一致で判定します。
        /// </summary>
        /// <remarks>
        /// 以前は双方向の部分一致（source.Contains(target) || target.Contains(source)）で
        /// 判定していたため、"github.com" というパターンが
        /// "https://evil.com/?q=github.com" にマッチしてしまう誤爆があった。
        /// </remarks>
        /// <param name="source">ソースURL</param>
        /// <param name="pattern">マッチングパターン</param>
        /// <returns>マッチする場合はtrue</returns>
        private static bool MatchHostPattern(string source, string pattern)
        {
            var normalizedPattern = NormalizeForMatching(pattern);
            var normalizedSourceForEmptyCheck = NormalizeForMatching(source);
            if (normalizedPattern.Length == 0)
                return normalizedSourceForEmptyCheck.Length == 0;

            // パターンにパス部分が含まれるかどうかで判定方法を変える
            var slashIndex = normalizedPattern.IndexOf('/');
            var patternHost = slashIndex >= 0 ? normalizedPattern[..slashIndex] : normalizedPattern;

            // ソースURLのホストを取得する。スキームが無い場合は補って解析を試みる。
            string sourceHost;
            string sourceHostAndPath;
            if (TryGetHostAndPath(source, out var parsedHost, out var parsedHostAndPath))
            {
                sourceHost = parsedHost;
                sourceHostAndPath = parsedHostAndPath;
            }
            else
            {
                // URIとして解析できない場合は正規化した文字列で代用する
                var normalizedSource = NormalizeForMatching(source);
                var sourceSlashIndex = normalizedSource.IndexOf('/');
                sourceHost = sourceSlashIndex >= 0 ? normalizedSource[..sourceSlashIndex] : normalizedSource;
                sourceHostAndPath = normalizedSource;
            }

            // ホストは完全一致またはサブドメイン一致のみ許可する
            var hostMatches = sourceHost.Equals(patternHost, StringComparison.OrdinalIgnoreCase) ||
                              sourceHost.EndsWith("." + patternHost, StringComparison.OrdinalIgnoreCase);

            if (!hostMatches)
                return false;

            // パターンがホストのみならここでマッチ確定
            if (slashIndex < 0)
                return true;

            // パス付きパターンは「ホスト＋パス」の前方一致で判定する
            return sourceHostAndPath.StartsWith(normalizedPattern, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// URLからホストと「ホスト＋パス」を取得します。
        /// スキームが無いURLにはhttp://を補って解析します。
        /// </summary>
        /// <param name="url">対象URL</param>
        /// <param name="host">取得したホスト（www.は除去済み）</param>
        /// <param name="hostAndPath">取得したホストとパスの連結</param>
        /// <returns>解析できた場合はtrue</returns>
        private static bool TryGetHostAndPath(string url, out string host, out string hostAndPath)
        {
            host = string.Empty;
            hostAndPath = string.Empty;

            var candidate = url.Trim();
            if (candidate.Length == 0)
                return false;

            // スキームが無い場合は補完して解析する
            if (!candidate.Contains("://", StringComparison.Ordinal))
                candidate = "http://" + candidate;

            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
                return false;

            if (string.IsNullOrEmpty(uri.Host))
                return false;

            host = StripWwwPrefix(uri.Host);
            hostAndPath = host + uri.AbsolutePath.TrimEnd('/');
            return true;
        }

        /// <summary>
        /// マッチング用にスキームとwwwプレフィックス、末尾のスラッシュを除去します
        /// </summary>
        /// <param name="value">正規化対象の文字列</param>
        /// <returns>正規化された文字列</returns>
        private static string NormalizeForMatching(string value)
        {
            var result = value.Trim();

            // スキームを除去する（先頭のみ）
            var schemeIndex = result.IndexOf("://", StringComparison.Ordinal);
            if (schemeIndex >= 0)
                result = result[(schemeIndex + 3)..];

            result = StripWwwPrefix(result);
            return result.TrimEnd('/');
        }

        /// <summary>
        /// 先頭のwww.のみを除去します（文字列中のwww.は除去しません）
        /// </summary>
        /// <param name="value">対象文字列</param>
        /// <returns>www.を除去した文字列</returns>
        private static string StripWwwPrefix(string value)
        {
            return value.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
                ? value[4..]
                : value;
        }

        /// <summary>
        /// URLパターンマッチング（@パターン用）
        /// </summary>
        /// <param name="source">ソースURL</param>
        /// <param name="pattern">マッチングパターン</param>
        /// <returns>マッチする場合はtrue</returns>
        private static bool MatchURLPattern(string source, string pattern)
        {
            try
            {
                Logger.LogDebug("URLUtilities.MatchURLPattern", "Pattern matching", source, pattern);

                // パターンにワイルドカードが含まれている場合
                if (pattern.Contains("*"))
                {
                    // ワイルドカードパターンを正規表現に変換。
                    // Regex.Escapeで全メタ文字を無害化してから*だけを.*に戻すことで、
                    // ?や+など.以外のメタ文字がパターンに含まれていても誤動作しないようにする。
                    // さらに前後を^$でアンカーし、部分一致による誤爆を防ぐ。
                    var escaped = System.Text.RegularExpressions.Regex.Escape(pattern);
                    var regexPattern = "^" + escaped.Replace("\\*", ".*") + "$";

                    var regex = new System.Text.RegularExpressions.Regex(regexPattern,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase,
                        TimeSpan.FromMilliseconds(200));

                    // パターンにパス区切り（/）が含まれない場合はホスト向けパターンとみなし、
                    // ソースURLのホスト部分のみに対してマッチさせる（"*.example.com" 等）。
                    // パスを含む場合は「ホスト＋パス」に対してマッチさせる。
                    // 生のURL文字列（スキーム込み）に対してアンカーマッチすると、
                    // ホスト向けパターンが常に不一致になってしまうため生文字列は使わない。
                    string candidate;
                    if (pattern.Contains('/'))
                    {
                        candidate = NormalizeForMatching(source);
                    }
                    else if (TryGetHostAndPath(source, out var host, out _))
                    {
                        candidate = host;
                    }
                    else
                    {
                        var normalized = NormalizeForMatching(source);
                        var slashIndex = normalized.IndexOf('/');
                        candidate = slashIndex >= 0 ? normalized[..slashIndex] : normalized;
                    }

                    var result = regex.IsMatch(candidate);
                    Logger.LogDebug("URLUtilities.MatchURLPattern", "Wildcard pattern result", pattern, source, result);
                    return result;
                }
                else
                {
                    // ワイルドカードがない場合はホストベースの一致判定を使う
                    var result = source.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                                MatchHostPattern(source, pattern);

                    Logger.LogDebug("URLUtilities.MatchURLPattern", "Exact/host match result", pattern, source, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("URLUtilities.MatchURLPattern", "Pattern matching error", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ユーザーが指定した正規表現でURLをマッチングします（"re:"プレフィックス用）。
        /// 不正な正規表現の場合はマッチ失敗として扱い、アプリを継続させます。
        /// </summary>
        /// <param name="source">ソースURL</param>
        /// <param name="pattern">ユーザー指定の正規表現パターン</param>
        /// <returns>マッチする場合はtrue</returns>
        private static bool MatchRegexPattern(string source, string pattern)
        {
            try
            {
                // ユーザー指定の正規表現は破局的バックトラッキング（ReDoS）を起こしうるため、
                // タイムアウトを設けてUIスレッドが固まらないようにする
                var regex = new System.Text.RegularExpressions.Regex(pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(200));
                var result = regex.IsMatch(source);
                Logger.LogDebug("URLUtilities.MatchRegexPattern", "正規表現マッチング結果", pattern, source, result);
                return result;
            }
            catch (System.Text.RegularExpressions.RegexParseException ex)
            {
                Logger.LogWarning("URLUtilities.MatchRegexPattern", "不正な正規表現のためマッチ失敗として扱います", pattern, ex.Message);
                return false;
            }
            catch (System.Text.RegularExpressions.RegexMatchTimeoutException ex)
            {
                Logger.LogWarning("URLUtilities.MatchRegexPattern", "正規表現マッチングがタイムアウトしたためマッチ失敗として扱います", pattern, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("URLUtilities.MatchRegexPattern", "正規表現マッチングエラー", ex.Message);
                return false;
            }
        }
    }
}
