using BrowserChooser3.Classes.Interfaces;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.UI
{
    /// <summary>
    /// ファイルダイアログサービスの実装クラス
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        /// <summary>
        /// ファイルを開くダイアログを表示
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="filter">ファイルフィルター</param>
        /// <returns>選択されたファイルパス</returns>
        public string? ShowOpenFileDialog(string title, string filter)
        {
            try
            {
                Logger.LogDebug("FileDialogService.ShowOpenFileDialog", $"Start: {title}");
                
                // テスト環境では実際のダイアログを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FileDialogService.ShowOpenFileDialog", "テスト環境のためファイルダイアログ表示をスキップ");
                    return "test_file.txt";
                }
                
                using var openFileDialog = new OpenFileDialog
                {
                    Title = title,
                    Filter = filter,
                    CheckFileExists = true,
                    CheckPathExists = true
                };
                
                var result = openFileDialog.ShowDialog();
                string? selectedPath = null;
                
                if (result == DialogResult.OK)
                {
                    selectedPath = openFileDialog.FileName;
                    Logger.LogDebug("FileDialogService.ShowOpenFileDialog", $"End: {selectedPath}");
                }
                else
                {
                    Logger.LogDebug("FileDialogService.ShowOpenFileDialog", "End: Cancelled");
                }
                
                return selectedPath;
            }
            catch (Exception ex)
            {
                Logger.LogError("FileDialogService.ShowOpenFileDialog", "ファイルを開くダイアログ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// ファイルを保存するダイアログを表示
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="filter">ファイルフィルター</param>
        /// <param name="defaultFileName">デフォルトファイル名</param>
        /// <returns>保存先ファイルパス</returns>
        public string? ShowSaveFileDialog(string title, string filter, string defaultFileName = "")
        {
            try
            {
                Logger.LogDebug("FileDialogService.ShowSaveFileDialog", $"Start: {title}");
                
                // テスト環境では実際のダイアログを表示しない
                if (TestEnvironmentDetector.IsTestEnvironment())
                {
                    Logger.LogDebug("FileDialogService.ShowSaveFileDialog", "テスト環境のためファイルダイアログ表示をスキップ");
                    return string.IsNullOrEmpty(defaultFileName) ? "test_save.txt" : defaultFileName;
                }
                
                using var saveFileDialog = new SaveFileDialog
                {
                    Title = title,
                    Filter = filter,
                    OverwritePrompt = true,
                    ValidateNames = true
                };
                
                if (!string.IsNullOrEmpty(defaultFileName))
                {
                    saveFileDialog.FileName = defaultFileName;
                }
                
                var result = saveFileDialog.ShowDialog();
                string? selectedPath = null;
                
                if (result == DialogResult.OK)
                {
                    selectedPath = saveFileDialog.FileName;
                    Logger.LogDebug("FileDialogService.ShowSaveFileDialog", $"End: {selectedPath}");
                }
                else
                {
                    Logger.LogDebug("FileDialogService.ShowSaveFileDialog", "End: Cancelled");
                }
                
                return selectedPath;
            }
            catch (Exception ex)
            {
                Logger.LogError("FileDialogService.ShowSaveFileDialog", "ファイルを保存するダイアログ表示エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }
    }
}
