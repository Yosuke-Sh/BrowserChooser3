using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;
using System.Linq;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのヘルプ関連イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormHelpHandlers
    {
        private readonly OptionsForm _form;

        /// <summary>
        /// OptionsFormHelpHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        public OptionsFormHelpHandlers(OptionsForm form)
        {
            _form = form;
        }

        /// <summary>
        /// ヘルプページを開く
        /// </summary>
        public void OpenHelp()
        {
            try
            {
                // テスト環境では実際のプロセスを起動しない
                if (IsTestEnvironment())
                {
                    return;
                }

                // Browser Chooser 3のヘルプページを開く
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/Yosuke-Sh/BrowserChooser3",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Help page cannot be reached!\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// テスト環境かどうかを判定する
        /// </summary>
        /// <returns>テスト環境の場合はtrue</returns>
        private static bool IsTestEnvironment()
        {
            // テストアセンブリが読み込まれているかチェック
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Any(assembly => 
                assembly.FullName?.Contains("xunit") == true || 
                assembly.FullName?.Contains("BrowserChooser3.Tests") == true ||
                assembly.FullName?.Contains("Microsoft.VisualStudio.TestPlatform") == true);
        }
    }
}
