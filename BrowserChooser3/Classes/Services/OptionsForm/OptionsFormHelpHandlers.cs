using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

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
                // Browser Chooser 2のヘルプページを開く
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://bitbucket.org/gmyx/browserchooser2/wiki/Home",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Help page cannot be reached!\n\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
