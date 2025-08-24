namespace BrowserChooser3.Classes
{
    /// <summary>
    /// URLの解析と処理を行うユーティリティクラス
    /// </summary>
    public static class URLUtilities
    {
        /// <summary>
        /// URLパーツを表す構造体
        /// </summary>
        public struct URLParts
        {
            /// <summary>プロトコル</summary>
            public string Protocol;
            
            /// <summary>ドメイン</summary>
            public string Domain;
            
            /// <summary>パス</summary>
            public string Path;
            
            /// <summary>拡張子</summary>
            public string Extension;
            
            /// <summary>残りの部分</summary>
            public string Remainder;
            
            /// <summary>プロトコルかどうか</summary>
            public Settings.TriState IsProtocol;

            /// <summary>
            /// URLパーツを文字列に変換します
            /// </summary>
            /// <returns>URL文字列</returns>
            public override string ToString()
            {
                if (IsProtocol == Settings.TriState.True)
                {
                    return $"{Protocol}://{Domain}{Path}";
                }
                else
                {
                    return $"{Domain}{Path}";
                }
            }
        }

        /// <summary>
        /// URLの各部分を解析します
        /// </summary>
        /// <param name="url">解析対象のURL</param>
        /// <returns>URLパーツ</returns>
        public static URLParts DetermineParts(string url)
        {
            var parts = new URLParts();
            
            if (string.IsNullOrEmpty(url))
            {
                parts.IsProtocol = Settings.TriState.UseDefault;
                return parts;
            }

            // プロトコルの検出
            var colonIndex = url.IndexOf(':');
            if (colonIndex > 0 && colonIndex < url.Length - 2)
            {
                var protocol = url.Substring(0, colonIndex).ToLower();
                if (url.Substring(colonIndex + 1, 2) == "//")
                {
                    // プロトコル付きURL
                    parts.Protocol = protocol;
                    parts.IsProtocol = Settings.TriState.True;
                    
                    var pathStart = url.IndexOf('/', colonIndex + 3);
                    if (pathStart > 0)
                    {
                        parts.Domain = url.Substring(colonIndex + 3, pathStart - colonIndex - 3);
                        parts.Path = url.Substring(pathStart);
                        parts.Remainder = parts.Domain + parts.Path;
                    }
                    else
                    {
                        parts.Domain = url.Substring(colonIndex + 3);
                        parts.Path = "/";
                        parts.Remainder = parts.Domain + parts.Path;
                    }
                }
                else
                {
                    // ファイル拡張子の可能性
                    parts.IsProtocol = Settings.TriState.False;
                    parts.Extension = protocol;
                    parts.Domain = url;
                    parts.Remainder = url;
                }
            }
            else
            {
                // プロトコルなしURL
                parts.IsProtocol = Settings.TriState.False;
                parts.Domain = url;
                parts.Remainder = url;
                
                // 拡張子の検出
                var lastDotIndex = url.LastIndexOf('.');
                if (lastDotIndex > 0 && lastDotIndex < url.Length - 1)
                {
                    var lastSlashIndex = url.LastIndexOf('/');
                    if (lastSlashIndex < lastDotIndex)
                    {
                        parts.Extension = url.Substring(lastDotIndex + 1);
                    }
                }
            }

            return parts;
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
    }
}
