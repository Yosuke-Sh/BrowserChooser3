using System.Reflection;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// テスト環境の検出を行うユーティリティクラス
    /// </summary>
    public static class TestEnvironmentDetector
    {
        /// <summary>
        /// テスト環境判定結果のキャッシュ（プロセス起動中に一度だけ評価する）
        /// </summary>
        private static readonly Lazy<bool> _isTestEnvironment = new(DetectTestEnvironment);

        /// <summary>
        /// 現在の実行環境がテスト環境かどうかを判定します。
        /// 判定結果はプロセス起動中一度だけ評価してキャッシュされます。
        /// </summary>
        /// <returns>テスト環境の場合はtrue、それ以外はfalse</returns>
        public static bool IsTestEnvironment()
        {
            return _isTestEnvironment.Value;
        }

        private static bool DetectTestEnvironment()
        {
            try
            {
                // 現在のアセンブリを取得
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                // テスト関連のアセンブリが存在するかチェック
                foreach (var assembly in assemblies)
                {
                    var assemblyName = assembly.GetName().Name;
                    if (assemblyName != null &&
                        (assemblyName.Contains("xunit") ||
                         assemblyName.Contains("nunit") ||
                         assemblyName.Contains("mstest") ||
                         assemblyName.Contains("testhost") ||
                         assemblyName.Contains("test") ||
                         assemblyName.Contains("Test") ||
                         assemblyName.Contains("BrowserChooser3.Tests")))
                    {
                        return true;
                    }
                }

                // 環境変数でテスト環境かどうかをチェック
                var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnvironment) && testEnvironment.ToLower() == "true")
                {
                    return true;
                }

                // 環境変数でダイアログ無効化が設定されている場合
                var disableDialogs = Environment.GetEnvironmentVariable("DISABLE_DIALOGS");
                if (!string.IsNullOrEmpty(disableDialogs) && disableDialogs.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // プロセス名でテスト環境かどうかをチェック
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower();
                if (processName.Contains("test") || processName.Contains("dotnet") || processName.Contains("vstest"))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                // エラーが発生した場合は安全のためfalseを返す
                return false;
            }
        }

        /// <summary>
        /// テスト環境での実行を防ぐためのガードメソッド
        /// </summary>
        /// <param name="operationName">実行しようとしている操作の名前</param>
        /// <exception cref="InvalidOperationException">テスト環境で実行された場合</exception>
        public static void GuardAgainstTestEnvironment(string operationName)
        {
            if (IsTestEnvironment())
            {
                throw new InvalidOperationException($"テスト環境では{operationName}を実行できません。");
            }
        }
    }
}
