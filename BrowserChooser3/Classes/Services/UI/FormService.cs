using BrowserChooser3.Classes.Interfaces;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Forms;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.UI
{
    /// <summary>
    /// フォームサービスの実装クラス
    /// </summary>
    public class FormService : IFormService
    {
        /// <summary>
        /// オプション画面を表示
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowOptionsForm(Settings settings)
        {
            try
            {
                if (settings == null)
                    throw new ArgumentNullException(nameof(settings));

                Logger.LogDebug("FormService.ShowOptionsForm", "Start");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowOptionsForm", "テスト環境のためフォーム表示をスキップ");
                    return DialogResult.OK;
                }
                
                using var optionsForm = new OptionsForm(settings);
                var result = optionsForm.ShowDialog();
                
                Logger.LogDebug("FormService.ShowOptionsForm", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowOptionsForm", "オプション画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// About画面を表示
        /// </summary>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowAboutForm()
        {
            try
            {
                Logger.LogDebug("FormService.ShowAboutForm", "Start");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowAboutForm", "テスト環境のためフォーム表示をスキップ");
                    return DialogResult.OK;
                }
                
                using var aboutForm = new AboutForm();
                var result = aboutForm.ShowDialog();
                
                Logger.LogDebug("FormService.ShowAboutForm", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowAboutForm", "About画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// アイコン選択画面を表示
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>選択されたアイコン</returns>
        public Icon? ShowIconSelectionForm(string filePath)
        {
            try
            {
                Logger.LogDebug("FormService.ShowIconSelectionForm", $"Start: {filePath}");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowIconSelectionForm", "テスト環境のためフォーム表示をスキップ");
                    return null;
                }
                
                using var iconForm = new IconSelectionForm(filePath);
                var result = iconForm.ShowDialog();
                
                if (result == DialogResult.OK)
                {
                    var selectedIcon = iconForm.SelectedIcon;
                    Logger.LogDebug("FormService.ShowIconSelectionForm", $"End: Icon selected");
                    return selectedIcon;
                }
                
                Logger.LogDebug("FormService.ShowIconSelectionForm", "End: Cancelled");
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowIconSelectionForm", "アイコン選択画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// ブラウザ追加・編集画面を表示
        /// </summary>
        /// <param name="browser">ブラウザオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowAddEditBrowserForm(Browser browser, Settings settings)
        {
            try
            {
                if (browser == null)
                    throw new ArgumentNullException(nameof(browser));
                if (settings == null)
                    throw new ArgumentNullException(nameof(settings));

                Logger.LogDebug("FormService.ShowAddEditBrowserForm", $"Start: {browser.Name}");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowAddEditBrowserForm", "テスト環境のためフォーム表示をスキップ");
                    return DialogResult.OK;
                }
                
                using var addEditForm = new AddEditBrowserForm();
                var result = addEditForm.EditBrowser(browser, new Dictionary<int, Browser>(), new Dictionary<int, Protocol>(), false) ? DialogResult.OK : DialogResult.Cancel;
                
                Logger.LogDebug("FormService.ShowAddEditBrowserForm", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowAddEditBrowserForm", "ブラウザ追加・編集画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// URL追加・編集画面を表示
        /// </summary>
        /// <param name="url">URLオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowAddEditURLForm(URL url, Settings settings)
        {
            try
            {
                if (url == null)
                    throw new ArgumentNullException(nameof(url));
                if (settings == null)
                    throw new ArgumentNullException(nameof(settings));

                Logger.LogDebug("FormService.ShowAddEditURLForm", $"Start: {url.URLPattern}");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowAddEditURLForm", "テスト環境のためフォーム表示をスキップ");
                    return DialogResult.OK;
                }
                
                using var addEditForm = new AddEditURLForm();
                var result = addEditForm.EditURL(url, new Dictionary<int, Browser>()) ? DialogResult.OK : DialogResult.Cancel;
                
                Logger.LogDebug("FormService.ShowAddEditURLForm", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowAddEditURLForm", "URL追加・編集画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// プロトコル追加・編集画面を表示
        /// </summary>
        /// <param name="protocol">プロトコルオブジェクト</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <returns>ダイアログ結果</returns>
        public DialogResult ShowAddEditProtocolForm(Protocol protocol, Settings settings)
        {
            try
            {
                if (protocol == null)
                    throw new ArgumentNullException(nameof(protocol));
                if (settings == null)
                    throw new ArgumentNullException(nameof(protocol));

                Logger.LogDebug("FormService.ShowAddEditProtocolForm", $"Start: {protocol.Name}");
                
                // テスト環境では実際のフォームを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FormService.ShowAddEditProtocolForm", "テスト環境のためフォーム表示をスキップ");
                    return DialogResult.OK;
                }
                
                using var addEditForm = new AddEditProtocolForm();
                var result = addEditForm.EditProtocol(protocol, new Dictionary<int, Browser>()) ? DialogResult.OK : DialogResult.Cancel;
                
                Logger.LogDebug("FormService.ShowAddEditProtocolForm", $"End: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("FormService.ShowAddEditProtocolForm", "プロトコル追加・編集画面表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }
    }
}
