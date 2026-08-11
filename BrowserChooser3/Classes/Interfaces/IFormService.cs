using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Classes.Interfaces
{
    /// <summary>
    /// フォームサービスのインターフェース
    /// テスト可能な設計のために抽象化します
    /// </summary>
    public interface IFormService
    {
        /// <summary>
        /// オプション画面を表示
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowOptionsForm(Settings settings);

        /// <summary>
        /// About画面を表示
        /// </summary>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowAboutForm();

        /// <summary>
        /// アイコン選択画面を表示
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>選択されたアイコン</returns>
        Icon? ShowIconSelectionForm(string filePath);

        /// <summary>
        /// ブラウザ追加・編集画面を表示
        /// </summary>
        /// <param name="browser">ブラウザオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowAddEditBrowserForm(Browser browser, Settings settings);

        /// <summary>
        /// URL追加・編集画面を表示
        /// </summary>
        /// <param name="url">URLオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowAddEditURLForm(URL url, Settings settings);

        /// <summary>
        /// プロトコル追加・編集画面を表示
        /// </summary>
        /// <param name="protocol">プロトコルオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowAddEditProtocolForm(Protocol protocol, Settings settings);
    }

    /// <summary>
    /// メッセージボックスサービスのインターフェース
    /// </summary>
    public interface IMessageBoxService
    {
        /// <summary>
        /// 情報メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowInfo(string text, string caption = "情報");

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowWarning(string text, string caption = "警告");

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowError(string text, string caption = "エラー");

        /// <summary>
        /// 確認メッセージを表示
        /// </summary>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="caption">キャプション</param>
        /// <returns>ダイアログ結果</returns>
        DialogResult ShowQuestion(string text, string caption = "確認");
    }

    /// <summary>
    /// ファイルダイアログサービスのインターフェース
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// ファイルを開くダイアログを表示
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="filter">ファイルフィルター</param>
        /// <returns>選択されたファイルパス</returns>
        string? ShowOpenFileDialog(string title, string filter);

        /// <summary>
        /// ファイルを保存するダイアログを表示
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="filter">ファイルフィルター</param>
        /// <param name="defaultFileName">デフォルトファイル名</param>
        /// <returns>保存先ファイルパス</returns>
        string? ShowSaveFileDialog(string title, string filter, string defaultFileName = "");
    }
}
