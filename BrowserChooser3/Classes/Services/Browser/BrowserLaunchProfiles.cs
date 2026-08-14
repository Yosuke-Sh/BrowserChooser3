using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.BrowserServices
{
    /// <summary>
    /// ブラウザの種別（ファミリ）
    /// </summary>
    public enum BrowserFamily
    {
        /// <summary>判別できないブラウザ。プロファイル・シークレット指定は無視されます。</summary>
        Unknown,

        /// <summary>Chromium系（Chrome / Edge / Brave / Vivaldi / Opera など）</summary>
        Chromium,

        /// <summary>Microsoft Edge（Chromium系だがシークレットの引数名が異なる）</summary>
        Edge,

        /// <summary>Firefox系</summary>
        Firefox
    }

    /// <summary>
    /// ブラウザのプロファイル指定・シークレット起動に必要なコマンドライン引数を、
    /// ブラウザの種別ごとに解決します。
    ///
    /// 引数は必ず「1要素=1引数」のリストとして組み立て、呼び出し側が
    /// <see cref="System.Diagnostics.ProcessStartInfo.ArgumentList"/> へ渡すことを前提とします。
    /// 文字列連結でコマンドラインを組まないことで、URLやプロファイル名に含まれる
    /// 引用符・空白による引数注入を防ぎます。
    /// </summary>
    public static class BrowserLaunchProfiles
    {
        /// <summary>
        /// ブラウザ名・実行ファイルパスから種別を判定します。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <returns>判定された種別</returns>
        public static BrowserFamily DetectFamily(Browser? browser)
        {
            if (browser == null) return BrowserFamily.Unknown;

            // 名前・パス・検出時に設定されるCategoryのいずれかで判定する
            var haystack = $"{browser.Name} {browser.Target} {browser.Category}".ToLowerInvariant();

            if (browser.IsEdge || haystack.Contains("msedge") || haystack.Contains("edge"))
            {
                return BrowserFamily.Edge;
            }

            if (haystack.Contains("firefox") || haystack.Contains("librewolf") || haystack.Contains("waterfox"))
            {
                return BrowserFamily.Firefox;
            }

            if (haystack.Contains("chrome") || haystack.Contains("chromium") ||
                haystack.Contains("brave") || haystack.Contains("vivaldi") || haystack.Contains("opera"))
            {
                return BrowserFamily.Chromium;
            }

            return BrowserFamily.Unknown;
        }

        /// <summary>
        /// 指定されたプロファイルで起動するための引数を返します。
        /// 種別が判別できない場合やプロファイル名が空の場合は空のリストを返します。
        /// </summary>
        /// <param name="family">ブラウザの種別</param>
        /// <param name="profileName">プロファイル名</param>
        /// <returns>追加する引数（1要素=1引数）</returns>
        public static IReadOnlyList<string> GetProfileArguments(BrowserFamily family, string? profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName)) return Array.Empty<string>();

            return family switch
            {
                // Chromium系は --profile-directory=<名前> を1引数として渡す。
                // ArgumentListに入れるため、値を引用符で囲む必要はない。
                BrowserFamily.Chromium or BrowserFamily.Edge
                    => new[] { $"--profile-directory={profileName}" },

                // Firefoxは -P <名前>。プロファイルマネージャを出さないよう
                // -no-remote は付けない（既存ウィンドウでのURL処理を壊すため）。
                BrowserFamily.Firefox
                    => new[] { "-P", profileName },

                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// シークレット/プライベートウィンドウで起動するための引数を返します。
        /// </summary>
        /// <param name="family">ブラウザの種別</param>
        /// <returns>追加する引数（1要素=1引数）</returns>
        public static IReadOnlyList<string> GetPrivateModeArguments(BrowserFamily family)
        {
            return family switch
            {
                BrowserFamily.Chromium => new[] { "--incognito" },
                BrowserFamily.Edge => new[] { "--inprivate" },
                BrowserFamily.Firefox => new[] { "-private-window" },
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// このブラウザがプロファイル指定・シークレット起動に対応しているかどうか。
        /// 対応していない場合、MainFormはコンテキストメニューの該当項目を無効化します。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <returns>対応している場合はtrue</returns>
        public static bool SupportsProfilesOrPrivateMode(Browser? browser)
            => DetectFamily(browser) != BrowserFamily.Unknown;

        /// <summary>
        /// ブラウザ起動用の引数リストを組み立てます。
        ///
        /// 順序は「ユーザー定義の引数 → プロファイル指定 → シークレット指定 → URL」。
        /// URLは必ず最後の独立した1引数として追加されるため、URLに空白や引用符が
        /// 含まれていても他の引数として解釈されることはありません。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <param name="url">開くURL（空の場合はURLを付けない）</param>
        /// <param name="forcePrivateMode">
        /// ブラウザ設定に関わらずシークレット起動する場合はtrue
        /// （MainFormの右クリックメニューからの一時的な指定）
        /// </param>
        /// <param name="profileOverride">
        /// ブラウザ設定のプロファイルではなく指定のプロファイルで起動する場合に設定します。
        /// nullの場合は <see cref="Browser.ProfileName"/> を使用します。
        /// </param>
        /// <returns>ProcessStartInfo.ArgumentListへそのまま渡せる引数リスト</returns>
        public static List<string> BuildArgumentList(
            Browser browser,
            string? url,
            bool forcePrivateMode = false,
            string? profileOverride = null)
        {
            ArgumentNullException.ThrowIfNull(browser);

            var arguments = new List<string>();
            var family = DetectFamily(browser);

            // ユーザーが自由記述した引数（従来の Browser.Arguments）。
            // 単一の文字列なので、ここでだけコマンドライン風の分割を行う。
            arguments.AddRange(SplitUserArguments(browser.Arguments));

            var profileName = profileOverride ?? browser.ProfileName;
            arguments.AddRange(GetProfileArguments(family, profileName));

            if (forcePrivateMode || browser.UsePrivateMode)
            {
                arguments.AddRange(GetPrivateModeArguments(family));
            }

            // URLは必ず独立した最後の引数として渡す（引数注入の防止）
            if (!string.IsNullOrWhiteSpace(url))
            {
                arguments.Add(url);
            }

            return arguments;
        }

        /// <summary>
        /// ユーザーが1つの文字列として入力した引数を、個々の引数へ分割します。
        /// 引用符で囲まれた部分は空白を含んでいても1つの引数として扱います。
        /// </summary>
        /// <param name="userArguments">ユーザー定義の引数文字列</param>
        /// <returns>分割された引数</returns>
        public static IReadOnlyList<string> SplitUserArguments(string? userArguments)
        {
            if (string.IsNullOrWhiteSpace(userArguments)) return Array.Empty<string>();

            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            var inQuotes = false;

            foreach (var c in userArguments)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result;
        }

        /// <summary>
        /// ブラウザのユーザーデータディレクトリを走査し、選択可能なプロファイル名を列挙します。
        /// 検出に失敗した場合や対応していないブラウザの場合は空のリストを返します。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <returns>プロファイル名の一覧（表示順）</returns>
        public static IReadOnlyList<string> DiscoverProfiles(Browser? browser)
        {
            if (browser == null) return Array.Empty<string>();

            try
            {
                var family = DetectFamily(browser);
                return family switch
                {
                    BrowserFamily.Chromium or BrowserFamily.Edge => DiscoverChromiumProfiles(browser),
                    BrowserFamily.Firefox => DiscoverFirefoxProfiles(),
                    _ => Array.Empty<string>()
                };
            }
            catch (Exception ex)
            {
                Logger.LogWarning("BrowserLaunchProfiles.DiscoverProfiles", "プロファイル検出に失敗", browser.Name, ex.Message);
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Chromium系ブラウザのユーザーデータディレクトリからプロファイルディレクトリを列挙します。
        /// Chromiumは "Default" と "Profile N" という名前でプロファイルを保持します。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <returns>プロファイルディレクトリ名の一覧</returns>
        private static IReadOnlyList<string> DiscoverChromiumProfiles(Browser browser)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var haystack = $"{browser.Name} {browser.Target}".ToLowerInvariant();

            // 実行ファイル名からユーザーデータディレクトリの位置を推定する
            string? userDataPath = null;
            if (haystack.Contains("msedge") || haystack.Contains("edge"))
            {
                userDataPath = Path.Combine(localAppData, "Microsoft", "Edge", "User Data");
            }
            else if (haystack.Contains("chrome"))
            {
                userDataPath = Path.Combine(localAppData, "Google", "Chrome", "User Data");
            }
            else if (haystack.Contains("brave"))
            {
                userDataPath = Path.Combine(localAppData, "BraveSoftware", "Brave-Browser", "User Data");
            }
            else if (haystack.Contains("vivaldi"))
            {
                userDataPath = Path.Combine(localAppData, "Vivaldi", "User Data");
            }

            if (userDataPath == null || !Directory.Exists(userDataPath)) return Array.Empty<string>();

            var profiles = new List<string>();
            foreach (var directory in Directory.GetDirectories(userDataPath))
            {
                var name = Path.GetFileName(directory);
                // プロファイルは "Default" または "Profile N"。他は共有データ用のディレクトリ。
                if (name.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("Profile ", StringComparison.OrdinalIgnoreCase))
                {
                    profiles.Add(name);
                }
            }

            profiles.Sort(StringComparer.OrdinalIgnoreCase);
            return profiles;
        }

        /// <summary>
        /// Firefoxの profiles.ini からプロファイル名を列挙します。
        /// </summary>
        /// <returns>プロファイル名の一覧</returns>
        private static IReadOnlyList<string> DiscoverFirefoxProfiles()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var profilesIni = Path.Combine(appData, "Mozilla", "Firefox", "profiles.ini");
            if (!File.Exists(profilesIni)) return Array.Empty<string>();

            var profiles = new List<string>();
            foreach (var line in File.ReadAllLines(profilesIni))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("Name=", StringComparison.OrdinalIgnoreCase))
                {
                    var name = trimmed.Substring("Name=".Length).Trim();
                    if (!string.IsNullOrEmpty(name) && !profiles.Contains(name))
                    {
                        profiles.Add(name);
                    }
                }
            }

            return profiles;
        }

        /// <summary>
        /// 引数リストをログ出力用の1行文字列へ整形します（実際の起動には使用しません）。
        /// </summary>
        /// <param name="arguments">引数リスト</param>
        /// <returns>ログ用の文字列</returns>
        public static string FormatForLog(IEnumerable<string> arguments)
            => string.Join(" ", arguments.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
    }
}
