using System.Net;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// URLの解析と処理を行うユーティリティクラス
    /// </summary>
    public static class URLUtilities
    {
        /// <summary>
        /// Browser Chooser 2互換のURLパーツ構造体
        /// </summary>
        public struct BC2URLParts
        {
            /// <summary>プロトコルかどうか</summary>
            public Settings.TriState IsProtocol;
            
            /// <summary>プロトコル</summary>
            public string Protocol;
            
            /// <summary>拡張子</summary>
            public string Extension;
            
            /// <summary>残りの部分</summary>
            public string Remainder;

            /// <summary>
            /// URLパーツを文字列に変換します
            /// </summary>
            /// <returns>URL文字列</returns>
            public override string ToString()
            {
                if (IsProtocol == Settings.TriState.True)
                {
                    return $"{Protocol}://{Remainder}";
                }
                else
                {
                    return $"{Extension}.{Remainder}";
                }
            }
        }

        /// <summary>
        /// URLが有効かどうかをチェックします
        /// </summary>
        /// <param name="url">チェック対象のURL</param>
        /// <returns>有効な場合はtrue</returns>
        public static bool IsValidURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

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

        /// <summary>
        /// ファイルパスかどうかをチェックします
        /// </summary>
        /// <param name="path">チェック対象のパス</param>
        /// <returns>ファイルパスの場合はtrue</returns>
        public static bool IsFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
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
        /// Browser Chooser 2互換のURLパーツ解析
        /// </summary>
        /// <param name="url">解析対象のURL</param>
        /// <returns>URLパーツ</returns>
        public static BC2URLParts DetermineParts(string url)
        {
            Logger.LogInfo("URLUtilities.DetermineParts", "Start", url);
            var parts = new BC2URLParts();

            if (string.IsNullOrEmpty(url))
            {
                parts.IsProtocol = Settings.TriState.UseDefault;
                return Canonicalize(parts);
            }

            // プロトコルの検出
            var protocolIndex = url.IndexOf("://");
            if (protocolIndex > 0)
            {
                parts.IsProtocol = Settings.TriState.True;
                parts.Protocol = url.Substring(0, protocolIndex);
                parts.Remainder = url.Substring(protocolIndex + 3);
            }
            else if (url.Contains('.'))
            {
                // ファイル拡張子
                parts.IsProtocol = Settings.TriState.False;
                var lastDotIndex = url.LastIndexOf('.');
                parts.Extension = url.Substring(lastDotIndex + 1);
                parts.Remainder = url.Substring(0, lastDotIndex);
            }
            else if (url.Contains('/'))
            {
                // ドメインなしのプロトコル
                parts.IsProtocol = Settings.TriState.True;
                parts.Protocol = "https";
                parts.Remainder = url;
            }
            else
            {
                parts.IsProtocol = Settings.TriState.UseDefault;
            }

            Logger.LogInfo("URLUtilities.DetermineParts", "End", parts.IsProtocol, parts.Protocol ?? "", parts.Extension ?? "", parts.Remainder ?? "");
            return Canonicalize(parts);
        }

        /// <summary>
        /// Browser Chooser 2互換のURL正規化
        /// </summary>
        /// <param name="url">正規化対象のURLパーツ</param>
        /// <returns>正規化されたURLパーツ</returns>
        public static BC2URLParts Canonicalize(BC2URLParts url)
        {
            var settings = Settings.Current;
            
            // 正規化が有効で、プロトコルが設定されている場合
            if (url.IsProtocol == Settings.TriState.True && settings?.Canonicalize == true)
            {
                var firstSlash = url.Remainder.IndexOf('/');
                var firstQuestion = url.Remainder.IndexOf('?');
                var firstDot = url.Remainder.IndexOf('.');

                string subIn;
                
                if (firstDot == -1 && firstSlash > 0 && (firstQuestion == -1 || firstQuestion > firstSlash))
                {
                    // ドットなし、スラッシュが最初の疑問符より前
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 1", firstSlash, firstQuestion, firstDot);
                    subIn = $"{url.Remainder.Substring(0, firstSlash)}{settings.CanonicalizeAppendedText}{url.Remainder.Substring(firstSlash)}";
                }
                else if (firstDot == -1 && firstQuestion > 0 && (firstSlash == -1 || firstSlash > firstQuestion))
                {
                    // ドットなし、疑問符が最初のスラッシュより前
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 2", firstSlash, firstQuestion, firstDot);
                    subIn = $"{url.Remainder.Substring(0, firstQuestion)}{settings.CanonicalizeAppendedText}{url.Remainder.Substring(firstQuestion)}";
                }
                else if (firstDot == -1 && firstSlash == -1 && firstQuestion == -1)
                {
                    // ドット、スラッシュ、疑問符なし
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 3", firstSlash, firstQuestion, firstDot);
                    subIn = $"{url.Remainder}.{settings.CanonicalizeAppendedText}";
                }
                else if (firstSlash > 0 && firstSlash < firstDot && (firstQuestion == -1 || firstQuestion > firstSlash))
                {
                    // ドットがあるが、スラッシュがドットより前で、疑問符より前
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 4", firstSlash, firstQuestion, firstDot);
                    subIn = $"{url.Remainder.Substring(0, firstSlash)}{settings.CanonicalizeAppendedText}{url.Remainder.Substring(firstSlash)}";
                }
                else if (firstQuestion > 0 && firstQuestion < firstDot && (firstSlash == -1 || firstSlash > firstQuestion))
                {
                    // 疑問符がドットより前で、スラッシュより前
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 5", firstSlash, firstQuestion, firstDot);
                    subIn = $"{url.Remainder.Substring(0, firstQuestion)}{settings.CanonicalizeAppendedText}{url.Remainder.Substring(firstQuestion)}";
                }
                else
                {
                    // その他の場合（ドットがスラッシュや疑問符より前）
                    Logger.LogInfo("URLUtilities.Canonicalize", "Select 6", firstSlash, firstQuestion, firstDot);
                    subIn = url.Remainder;
                }

                url.Remainder = subIn;
            }

            return url;
        }

        /// <summary>
        /// URLマッチング（Browser Chooser 2互換）
        /// </summary>
        /// <param name="source">ソースURL</param>
        /// <param name="target">ターゲットURL</param>
        /// <returns>マッチする場合はtrue</returns>
        public static bool MatchURLs(string source, string target)
        {
            Logger.LogInfo("URLUtilities.MatchURLs", "Start", source, target);
            
            // http(s)://とwwwを除去
            var lsSource = source.Replace("http://", "").Replace("https://", "").Replace("www.", "");
            var lsTarget = target.Replace("http://", "").Replace("https://", "").Replace("www.", "");

            // 基本的なワイルドカードマッチング（後で正規表現に変更予定）
            if (lsTarget.Contains(lsSource) || lsSource.Contains(lsTarget))
            {
                Logger.LogInfo("URLUtilities.MatchURLs", "End", source, target, true);
                return true;
            }
            else
            {
                Logger.LogInfo("URLUtilities.MatchURLs", "End", source, target, false);
                return false;
            }
        }
    }
}
