using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのフォームイベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormFormHandlers
    {
        private readonly OptionsForm _form;
        private readonly Action _loadSettingsToControls;
        private readonly Action _saveSettings;
        private readonly Func<bool> _getIsModified;

        /// <summary>
        /// OptionsFormFormHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="loadSettingsToControls">設定をコントロールに読み込むアクション</param>
        /// <param name="saveSettings">設定を保存するアクション</param>
        /// <param name="getIsModified">変更フラグを取得する関数</param>
        public OptionsFormFormHandlers(
            OptionsForm form,
            Action loadSettingsToControls,
            Action saveSettings,
            Func<bool> getIsModified)
        {
            _form = form;
            _loadSettingsToControls = loadSettingsToControls;
            _saveSettings = saveSettings;
            _getIsModified = getIsModified;
        }

        /// <summary>
        /// 保存ボタンのクリックイベント
        /// </summary>
        public void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _saveSettings();
                
                // 透明化設定の変更を検出して情報メッセージを表示
                CheckTransparencySettingChange();
                
                _form.DialogResult = DialogResult.OK;
                _form.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.SaveButton_Click", "保存エラー", ex.Message);
                MessageBox.Show($"設定の保存に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 透明化設定の変更を検出し、必要に応じて情報メッセージを表示
        /// </summary>
        private void CheckTransparencySettingChange()
        {
            try
            {
                // 現在の透明化設定を取得
                var currentTransparencyEnabled = GetCurrentTransparencyEnabled();
                
                // 透明化が有効から無効に変更された場合
                if (currentTransparencyEnabled == false)
                {
                    Logger.LogInfo("OptionsFormFormHandlers.CheckTransparencySettingChange", 
                        "透明化設定が無効に変更されました。情報メッセージを表示します。");
                    
                    // 情報メッセージを表示
                    MessageBox.Show(
                        "透明化の解除を正常に反映するためには再起動が必要です",
                        "情報",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.CheckTransparencySettingChange", 
                    "透明化設定変更検出エラー", ex.Message);
            }
        }
        
        /// <summary>
        /// 現在の透明化設定を取得
        /// </summary>
        /// <returns>透明化が有効かどうか</returns>
        private bool GetCurrentTransparencyEnabled()
        {
            try
            {
                // Displayパネルから透明化設定を取得
                var displayPanel = _form.tabSettings.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tab => tab.Name == "tabDisplay");
                
                if (displayPanel != null)
                {
                    var transparencyCheckBox = displayPanel.Controls.Find("chkEnableTransparency", true)
                        .FirstOrDefault() as CheckBox;
                    
                    if (transparencyCheckBox != null)
                    {
                        return transparencyCheckBox.Checked;
                    }
                }
                
                // フォールバック：設定オブジェクトから取得
                return _form.GetSettings().EnableTransparency;
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.GetCurrentTransparencyEnabled", "透明化設定取得エラー", ex.Message);
                return _form.GetSettings().EnableTransparency;
            }
        }

        /// <summary>
        /// ヘルプボタンのクリックイベント
        /// </summary>
        public void HelpButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // テスト環境ではヘルプを開かない
                if (IsTestEnvironment())
                {
                    return;
                }

                // ヘルプファイルまたはオンラインヘルプを開く
                var helpUrl = "https://github.com/your-repo/browser-chooser/wiki";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = helpUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.HelpButton_Click", "ヘルプ表示エラー", ex.Message);
                MessageBox.Show($"ヘルプの表示に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// テスト環境かどうかを判定
        /// </summary>
        private bool IsTestEnvironment()
        {
            return Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") == "true" ||
                   Environment.GetEnvironmentVariable("DISABLE_HELP") == "true" ||
                   System.Diagnostics.Process.GetCurrentProcess().ProcessName.Contains("test", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// フォーム閉じる時の確認ダイアログ
        /// </summary>
        public void OptionsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (_getIsModified())
                {
                    var result = MessageBox.Show(
                        "設定が変更されています。保存しますか？",
                        "確認",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            _saveSettings();
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                        case DialogResult.No:
                            // 保存せずに閉じる
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.OptionsForm_FormClosing", "フォーム閉じるエラー", ex.Message);
            }
        }

        /// <summary>
        /// フォーム表示時の設定読み込み
        /// </summary>
        public void OptionsForm_Shown(object? sender, EventArgs e)
        {
            try
            {
                _loadSettingsToControls();
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormFormHandlers.OptionsForm_Shown", "フォーム表示エラー", ex.Message);
            }
        }
    }
}
