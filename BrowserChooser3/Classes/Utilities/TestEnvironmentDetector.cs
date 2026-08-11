using System.Reflection;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// テスト環境の検出を行うユーティリティクラス
    /// </summary>
    public static class TestEnvironmentDetector
    {
        /// <summary>
        /// 現在の実行環境がテスト環境かどうかを判定します
        /// </summary>
        /// <returns>テスト環境の場合はtrue、それ以外はfalse</returns>
        public static bool IsTestEnvironment()
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

                // プロセス名でテスト環境かどうかをチェック
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower();
                if (processName.Contains("test") || processName.Contains("dotnet"))
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
