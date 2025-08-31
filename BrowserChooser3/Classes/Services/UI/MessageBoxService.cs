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
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogInfo("MessageBoxService.ShowInfo", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        /// <summary>
        /// 情報メッセージを表示（静的メソッド）
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public static DialogResult ShowInfoStatic(string text, string caption = "情報")
        {
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogInfo("MessageBoxService.ShowInfo", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowWarning(string text, string caption = "警告")
        {
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogWarning("MessageBoxService.ShowWarning", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return result;
        }

        /// <summary>
        /// 警告メッセージを表示（静的メソッド）
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public static DialogResult ShowWarningStatic(string text, string caption = "警告")
        {
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogWarning("MessageBoxService.ShowWarning", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return result;
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowError(string text, string caption = "エラー")
        {
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogError("MessageBoxService.ShowError", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        /// <summary>
        /// エラーメッセージを表示（静的メソッド）
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public static DialogResult ShowErrorStatic(string text, string caption = "エラー")
        {
            // テスト環境ではメッセージボックスを表示しない
            if (Logger.IsTestEnvironment)
            {
                Logger.LogError("MessageBoxService.ShowError", $"テスト環境のためメッセージボックスをスキップ: {text}");
                return DialogResult.OK;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        /// <summary>
        /// 確認メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowQuestion(string text, string caption = "確認")
        {
            // テスト環境ではメッセージボックスを表示しない（デフォルトでYesを返す）
            if (Logger.IsTestEnvironment)
            {
                Logger.LogInfo("MessageBoxService.ShowQuestion", $"テスト環境のためメッセージボックスをスキップ（Yesを返す）: {text}");
                return DialogResult.Yes;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result;
        }

        /// <summary>
        /// 確認メッセージを表示（静的メソッド）
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        public static DialogResult ShowQuestionStatic(string text, string caption = "確認")
        {
            // テスト環境ではメッセージボックスを表示しない（デフォルトでYesを返す）
            if (Logger.IsTestEnvironment)
            {
                Logger.LogInfo("MessageBoxService.ShowQuestion", $"テスト環境のためメッセージボックスをスキップ（Yesを返す）: {text}");
                return DialogResult.Yes;
            }
            
            var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result;
        }
    }
}
