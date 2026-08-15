using System;
using System.IO;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// テストごとに一意な一時ディレクトリを用意し、破棄時に削除するフィクスチャ。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 従来 SettingsTests は <c>%TEMP%\BrowserChooser3Tests</c>、ImageUtilitiesTests は
    /// <c>test_image.png</c>、BrowserDetectorTests は <c>test_browser1..7.exe</c> という
    /// 固定パスを使っており、クラスを並列実行すると互いのファイルを削除し合っていた。
    /// GUID ベースの一意ディレクトリにすることで、この競合が原理的に起きなくなる。
    /// </para>
    /// <para>
    /// <see cref="RedirectConfigDirectory"/> を使うと、本番コードの設定ファイル出力先
    /// （既定では <c>%APPDATA%\BrowserChooser3</c>）をこの一時ディレクトリへ向けられる。
    /// 実ユーザープロファイルに触れずに保存・読み込みを検証したい場合に使う。
    /// </para>
    /// </remarks>
    public sealed class TempDirectoryFixture : IDisposable
    {
        private string? _originalConfigOverride;
        private bool _configRedirected;
        private bool _disposed;

        /// <summary>
        /// 一意な一時ディレクトリを作成します。
        /// </summary>
        public TempDirectoryFixture()
        {
            // 親ディレクトリ名を "BrowserChooser3Tests" と分けているのは意図的。
            // SettingsTests は %TEMP%\BrowserChooser3Tests を Dispose で丸ごと
            // Directory.Delete(recursive: true) するため、そこに相乗りすると
            // 並行実行中の他テストの一時ディレクトリまで巻き添えで消える。
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "BrowserChooser3TestFixtures",
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// このフィクスチャが所有する一時ディレクトリの絶対パス。
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 一時ディレクトリ配下のファイルパスを組み立てます（ファイル自体は作成しません）。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>一時ディレクトリ配下の絶対パス</returns>
        public string GetFilePath(string fileName) => System.IO.Path.Combine(Path, fileName);

        /// <summary>
        /// 一時ディレクトリ配下にファイルを作成します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="contents">書き込む内容</param>
        /// <returns>作成したファイルの絶対パス</returns>
        public string WriteFile(string fileName, string contents)
        {
            var path = GetFilePath(fileName);
            File.WriteAllText(path, contents);
            return path;
        }

        /// <summary>
        /// 本番コードの設定ディレクトリをこの一時ディレクトリへ向けます。
        /// <see cref="Dispose"/> で元の設定へ戻ります。
        /// </summary>
        public void RedirectConfigDirectory()
        {
            if (_configRedirected) return;

            _originalConfigOverride = PathManager.ConfigDirectoryOverrideForTests;
            PathManager.ConfigDirectoryOverrideForTests = Path;
            _configRedirected = true;
        }

        /// <summary>
        /// 設定ディレクトリのリダイレクトを解除し、一時ディレクトリを削除します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_configRedirected)
            {
                PathManager.ConfigDirectoryOverrideForTests = _originalConfigOverride;
                _configRedirected = false;
            }

            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
            catch (IOException)
            {
                // 他プロセス/スレッドがファイルを掴んでいる場合は放置する。
                // 一意なディレクトリなので残っても後続テストには影響しない。
            }
            catch (UnauthorizedAccessException)
            {
                // 同上
            }
        }
    }
}
