using BrowserChooser3.Classes.Interfaces;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.UI
{
    /// <summary>
    /// メッセージボックスサービスの実装クラス
    /// </summary>
    public class MessageBoxService : IMessageBoxService
    {
        /// <summary>
        /// 情報メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowInfo(string text, string caption = "情報")
        {
            try
            {
                Logger.LogDebug("MessageBoxService.ShowInfo", $"Start: {caption}");
                
                // テスト環境では実際のメッセージボックスを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("MessageBoxService.ShowInfo", "テスト環境のためメッセージボックス表示をスキップ");
                    return DialogResult.OK;
                }
                
                var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                Logger.LogDebug("MessageBoxService.ShowInfo", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("MessageBoxService.ShowInfo", "情報メッセージ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowWarning(string text, string caption = "警告")
        {
            try
            {
                Logger.LogDebug("MessageBoxService.ShowWarning", $"Start: {caption}");
                
                // テスト環境では実際のメッセージボックスを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("MessageBoxService.ShowWarning", "テスト環境のためメッセージボックス表示をスキップ");
                    return DialogResult.OK;
                }
                
                var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                Logger.LogDebug("MessageBoxService.ShowWarning", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("MessageBoxService.ShowWarning", "警告メッセージ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowError(string text, string caption = "エラー")
        {
            try
            {
                Logger.LogDebug("MessageBoxService.ShowError", $"Start: {caption}");
                
                // テスト環境では実際のメッセージボックスを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("MessageBoxService.ShowError", "テスト環境のためメッセージボックス表示をスキップ");
                    return DialogResult.OK;
                }
                
                var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                Logger.LogDebug("MessageBoxService.ShowError", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("MessageBoxService.ShowError", "エラーメッセージ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// 確認メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowQuestion(string text, string caption = "確認")
        {
            try
            {
                Logger.LogDebug("MessageBoxService.ShowQuestion", $"Start: {caption}");
                
                // テスト環境では実際のメッセージボックスを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("MessageBoxService.ShowQuestion", "テスト環境のためメッセージボックス表示をスキップ");
                    return DialogResult.Yes;
                }
                
                var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                Logger.LogDebug("MessageBoxService.ShowQuestion", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("MessageBoxService.ShowQuestion", "確認メッセージ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }
    }
}
