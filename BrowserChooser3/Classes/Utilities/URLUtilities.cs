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
            
            // 従来の処理（後方互換性のため）
            // http(s)://とwwwを除去
            var lsSource = source.Replace("http://", "").Replace("https://", "").Replace("www.", "");
            var lsTarget = target.Replace("http://", "").Replace("https://", "").Replace("www.", "");

            // 基本的なワイルドカードマッチング（後で正規表現に変更予定）
            if (lsTarget.Contains(lsSource) || lsSource.Contains(lsTarget))
            {
                Logger.LogInfo("URLUtilities.MatchURLs", "End (Legacy)", source, target, true);
                return true;
            }
            else
            {
                Logger.LogInfo("URLUtilities.MatchURLs", "End (Legacy)", source, target, false);
                return false;
            }
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
                    // ワイルドカードパターンを正規表現に変換
                    var regexPattern = pattern
                        .Replace(".", "\\.")  // ドットをエスケープ
                        .Replace("*", ".*");  // ワイルドカードを正規表現に変換

                    var regex = new System.Text.RegularExpressions.Regex(regexPattern, 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                    var result = regex.IsMatch(source);
                    Logger.LogDebug("URLUtilities.MatchURLPattern", "Wildcard pattern result", pattern, source, result);
                    return result;
                }
                else
                {
                    // ワイルドカードがない場合は完全一致または部分一致
                    var result = source.Equals(pattern, StringComparison.OrdinalIgnoreCase) || 
                                source.Contains(pattern, StringComparison.OrdinalIgnoreCase) ||
                                pattern.Contains(source, StringComparison.OrdinalIgnoreCase);
                    
                    Logger.LogDebug("URLUtilities.MatchURLPattern", "Exact/partial match result", pattern, source, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("URLUtilities.MatchURLPattern", "Pattern matching error", ex.Message);
                return false;
            }
        }
    }
}
