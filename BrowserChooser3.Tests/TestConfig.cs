using System;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// テスト環境の設定を管理するクラス
    /// </summary>
    public static class TestConfig
    {
        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        public static bool IsTestEnvironment()
        {
            try
            {
                // デバッガーがアタッチされている場合
                if (System.Diagnostics.Debugger.IsAttached)
                    return true;

                // アセンブリ名に"Test"が含まれている場合
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                if (assemblyName?.Contains("Test", StringComparison.OrdinalIgnoreCase) == true)
                    return true;

                // 環境変数でテスト環境を判定
                var testEnv = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
                if (!string.IsNullOrEmpty(testEnv) && testEnv.Equals("true", StringComparison.OrdinalIgnoreCase))
                    return true;

                // プロセス名に"test"が含まれている場合
                var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (processName.Contains("test", StringComparison.OrdinalIgnoreCase))
                    return true;

                // スタックトレースにテスト関連のメソッドが含まれている場合
                var stackTrace = Environment.StackTrace;
                if (stackTrace.Contains("xunit", StringComparison.OrdinalIgnoreCase) ||
                    stackTrace.Contains("test", StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch
            {
                // エラーが発生した場合は安全のためテスト環境とみなす
                return true;
            }
        }

        /// <summary>
        /// テスト環境でのダイアログ表示を無効化する
        /// </summary>
        public static void DisableDialogsInTestEnvironment()
        {
            if (IsTestEnvironment())
            {
                // テスト環境ではダイアログを表示しない設定を有効化
                Environment.SetEnvironmentVariable("DISABLE_DIALOGS", "true");
            }
        }

        /// <summary>
        /// テスト環境でのDrag&Drop処理を無効化する
        /// </summary>
        public static void DisableDragDropInTestEnvironment()
        {
            if (IsTestEnvironment())
            {
                // テスト環境ではDrag&Drop処理を無効化
                Environment.SetEnvironmentVariable("DISABLE_DRAGDROP", "true");
            }
        }

        /// <summary>
        /// テスト環境でのコンポーネントエラーを抑制する
        /// </summary>
        public static void SuppressComponentErrorsInTestEnvironment()
        {
            if (IsTestEnvironment())
            {
                // テスト環境ではコンポーネントエラーを抑制
                Environment.SetEnvironmentVariable("SUPPRESS_COMPONENT_ERRORS", "true");
            }
        }

        /// <summary>
        /// テスト環境でSTAスレッドを強制する
        /// </summary>
        public static void ForceSTAThreadInTestEnvironment()
        {
            if (IsTestEnvironment())
            {
                // テスト環境ではSTAスレッドを強制
                Environment.SetEnvironmentVariable("FORCE_STA_THREAD", "true");
                
                // 現在のスレッドをSTAに設定
                if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                {
                    // 注意: 実行中のスレッドのApartmentStateは変更できないため、
                    // 新しいSTAスレッドでテストを実行する必要があります
                }
            }
        }

        /// <summary>
        /// テスト環境でのヘルプ機能を無効化する
        /// </summary>
        public static void DisableHelpInTestEnvironment()
        {
            if (IsTestEnvironment())
            {
                // テスト環境ではヘルプ機能を無効化
                Environment.SetEnvironmentVariable("DISABLE_HELP", "true");
            }
        }
    }
}
