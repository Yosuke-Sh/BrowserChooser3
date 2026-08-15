using System.Text;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// ブラウザへ渡す前のURLを整えます。
    ///
    /// - トラッキングパラメータ（utm_* / fbclid / gclid 等）の除去
    /// - ポリシー（<see cref="Services.SystemServices.Policy.Canonicalize"/>）による正規化
    ///
    /// いずれも既定では無効で、明示的に有効化された場合のみURLを書き換えます。
    /// 書き換えに失敗した場合は必ず元のURLをそのまま返し、
    /// 「URLが壊れて開けない」状態を作らないことを優先します。
    /// </summary>
    public static class URLSanitizer
    {
        /// <summary>
        /// 設定とポリシーに従ってURLを整えます。
        /// </summary>
        /// <param name="url">対象のURL</param>
        /// <param name="settings">適用する設定（nullの場合は何もしない）</param>
        /// <returns>整えられたURL。処理対象外・失敗時は元のURL</returns>
        public static string Sanitize(string url, Settings? settings)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;

            var result = url;

            if (settings?.RemoveTrackingParameters == true)
            {
                result = RemoveTrackingParameters(result, settings.TrackingParameters);
            }

            // ポリシーで正規化が指示されている場合のみ適用する。
            // Policy.Canonicalize はレジストリ・環境変数から読み込まれるが
            // 従来URL処理側で一度も参照されていなかった。
            if (Services.SystemServices.Policy.Canonicalize)
            {
                result = Canonicalize(result, Services.SystemServices.Policy.CanonicalizeAppendedText);
            }

            if (!string.Equals(result, url, StringComparison.Ordinal))
            {
                Logger.LogDebug("URLSanitizer.Sanitize", "URLを整形しました", url, result);
            }

            return result;
        }

        /// <summary>
        /// クエリ文字列からトラッキングパラメータを除去します。
        ///
        /// パラメータ名は大文字小文字を区別せず比較し、末尾が * のものは
        /// 前方一致として扱います（utm_* は utm_source / utm_medium などにマッチ）。
        /// 除去した結果クエリが空になった場合は "?" も残しません。
        /// </summary>
        /// <param name="url">対象のURL</param>
        /// <param name="parameterNames">除去するパラメータ名のリスト</param>
        /// <returns>トラッキングパラメータを除いたURL</returns>
        public static string RemoveTrackingParameters(string url, IEnumerable<string>? parameterNames)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;

            var patterns = parameterNames?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
            if (patterns == null || patterns.Count == 0) return url;

            try
            {
                // フラグメント（#以降）はクエリではないので触らない
                var fragment = string.Empty;
                var working = url;
                var fragmentIndex = working.IndexOf('#');
                if (fragmentIndex >= 0)
                {
                    fragment = working.Substring(fragmentIndex);
                    working = working.Substring(0, fragmentIndex);
                }

                var queryIndex = working.IndexOf('?');
                if (queryIndex < 0) return url; // クエリが無ければ何もしない

                var basePart = working.Substring(0, queryIndex);
                var query = working.Substring(queryIndex + 1);
                if (query.Length == 0) return url;

                var kept = new List<string>();
                foreach (var pair in query.Split('&'))
                {
                    if (pair.Length == 0) continue;

                    var separatorIndex = pair.IndexOf('=');
                    var name = separatorIndex >= 0 ? pair.Substring(0, separatorIndex) : pair;

                    if (!IsTrackingParameter(name, patterns))
                    {
                        kept.Add(pair);
                    }
                }

                var builder = new StringBuilder(basePart);
                if (kept.Count > 0)
                {
                    builder.Append('?').Append(string.Join("&", kept));
                }
                builder.Append(fragment);

                return builder.ToString();
            }
            catch (Exception ex)
            {
                // 整形に失敗してURLを壊すより、元のURLをそのまま開く方が安全
                Logger.LogWarning("URLSanitizer.RemoveTrackingParameters", "トラッキングパラメータの除去に失敗しました", url, ex.Message);
                return url;
            }
        }

        /// <summary>
        /// パラメータ名が除去対象かどうかを判定します。
        /// </summary>
        /// <param name="name">クエリパラメータ名</param>
        /// <param name="patterns">除去対象のパターン</param>
        /// <returns>除去対象の場合はtrue</returns>
        private static bool IsTrackingParameter(string name, IEnumerable<string> patterns)
        {
            foreach (var rawPattern in patterns)
            {
                var pattern = rawPattern.Trim();
                if (pattern.Length == 0) continue;

                if (pattern.EndsWith('*'))
                {
                    var prefix = pattern.Substring(0, pattern.Length - 1);
                    // "*" 単体で全パラメータが消えるのは事故なので前方一致には接頭辞を必須とする
                    if (prefix.Length == 0) continue;
                    if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return true;
                }
                else if (name.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// URLを正規化し、必要に応じて指定のテキストを付加します。
        /// </summary>
        /// <param name="url">対象のURL</param>
        /// <param name="appendedText">付加するテキスト（空なら付加しない）</param>
        /// <returns>正規化されたURL。解析に失敗した場合は元のURL</returns>
        public static string Canonicalize(string url, string? appendedText)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;

            try
            {
                var canonical = URLUtilities.CanonicalizeURL(url);
                if (!string.IsNullOrEmpty(appendedText))
                {
                    canonical += appendedText;
                }
                return canonical;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("URLSanitizer.Canonicalize", "URLの正規化に失敗しました", url, ex.Message);
                return url;
            }
        }
    }
}
