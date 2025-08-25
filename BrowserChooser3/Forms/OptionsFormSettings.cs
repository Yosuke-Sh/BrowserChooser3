using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// OptionsFormの設定管理を担当するクラス
    /// </summary>
    public class OptionsFormSettings
    {
        private Settings _settings;
        private bool _isModified = false;
        
        // 内部データ管理（Browser Chooser 2互換）
        private Dictionary<Guid, Browser> _mBrowser = new();
        private SortedDictionary<Guid, URL> _mURLs = new();
        private Dictionary<Guid, Protocol> _mProtocols = new();
        private Dictionary<Guid, FileType> _mFileTypes = new();
        private bool _mProtocolsAreDirty = false;
        private bool _mFileTypesAreDirty = false;
        
        // フォーカス設定（Browser Chooser 2互換）
        private FocusSettings _mFocusSettings = new();

        /// <summary>
        /// 設定が変更されたかどうか
        /// </summary>
        public bool IsModified => _isModified;

        /// <summary>
        /// 設定オブジェクト
        /// </summary>
        public Settings Settings => _settings;

        /// <summary>
        /// ブラウザ辞書
        /// </summary>
        public Dictionary<Guid, Browser> Browsers => _mBrowser;

        /// <summary>
        /// URL辞書
        /// </summary>
        public SortedDictionary<Guid, URL> URLs => _mURLs;

        /// <summary>
        /// プロトコル辞書
        /// </summary>
        public Dictionary<Guid, Protocol> Protocols => _mProtocols;

        /// <summary>
        /// ファイルタイプ辞書
        /// </summary>
        public Dictionary<Guid, FileType> FileTypes => _mFileTypes;

        /// <summary>
        /// フォーカス設定
        /// </summary>
        public FocusSettings FocusSettings => _mFocusSettings;

        /// <summary>
        /// OptionsFormSettingsクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public OptionsFormSettings(Settings settings)
        {
            _settings = settings;
            LoadSettings();
        }

        /// <summary>
        /// 設定を読み込みます
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                Logger.LogInfo("OptionsFormSettings.LoadSettings", "Start");

                // ブラウザ設定の読み込み
                LoadBrowserSettings();

                // URL設定の読み込み
                LoadURLSettings();

                // プロトコル設定の読み込み
                LoadProtocolSettings();

                // ファイルタイプ設定の読み込み
                LoadFileTypeSettings();

                // フォーカス設定の読み込み
                LoadFocusSettings();

                _isModified = false;
                Logger.LogInfo("OptionsFormSettings.LoadSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormSettings.LoadSettings", "設定読み込みエラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// 設定を保存します
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                Logger.LogInfo("OptionsFormSettings.SaveSettings", "Start");

                // ブラウザ設定の保存
                SaveBrowserSettings();

                // URL設定の保存
                SaveURLSettings();

                // プロトコル設定の保存
                SaveProtocolSettings();

                // ファイルタイプ設定の保存
                SaveFileTypeSettings();

                // フォーカス設定の保存
                SaveFocusSettings();

                // 設定ファイルの保存
                _settings.DoSave();

                _isModified = false;
                Logger.LogInfo("OptionsFormSettings.SaveSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormSettings.SaveSettings", "設定保存エラー", ex.Message, ex.StackTrace ?? "");
                throw;
            }
        }

        /// <summary>
        /// ブラウザ設定を読み込みます
        /// </summary>
        private void LoadBrowserSettings()
        {
            _mBrowser.Clear();

            if (_settings.Browsers != null)
            {
                foreach (var browser in _settings.Browsers)
                {
                    _mBrowser[browser.Guid] = browser.Clone();
                }
            }
        }

        /// <summary>
        /// URL設定を読み込みます
        /// </summary>
        private void LoadURLSettings()
        {
            _mURLs.Clear();

            if (_settings.URLs != null)
            {
                foreach (var url in _settings.URLs)
                {
                    _mURLs[url.Guid] = url.Clone();
                }
            }
        }

        /// <summary>
        /// プロトコル設定を読み込みます
        /// </summary>
        private void LoadProtocolSettings()
        {
            _mProtocols.Clear();
            _mProtocolsAreDirty = false;

            if (_settings.Protocols != null)
            {
                foreach (var protocol in _settings.Protocols)
                {
                    _mProtocols[protocol.Guid] = protocol.Clone();
                }
            }
        }

        /// <summary>
        /// ファイルタイプ設定を読み込みます
        /// </summary>
        private void LoadFileTypeSettings()
        {
            _mFileTypes.Clear();
            _mFileTypesAreDirty = false;

            if (_settings.FileTypes != null)
            {
                foreach (var fileType in _settings.FileTypes)
                {
                    _mFileTypes[fileType.Guid] = fileType.Clone();
                }
            }
        }

        /// <summary>
        /// フォーカス設定を読み込みます
        /// </summary>
        private void LoadFocusSettings()
        {
            _mFocusSettings.ShowFocus = _settings.ShowFocus;
            _mFocusSettings.BoxColor = Color.FromArgb(_settings.FocusBoxColor);
            _mFocusSettings.BoxWidth = _settings.FocusBoxLineWidth;
        }

        /// <summary>
        /// ブラウザ設定を保存します
        /// </summary>
        private void SaveBrowserSettings()
        {
            _settings.Browsers = _mBrowser.Values.ToList();
        }

        /// <summary>
        /// URL設定を保存します
        /// </summary>
        private void SaveURLSettings()
        {
            _settings.URLs = _mURLs.Values.ToList();
        }

        /// <summary>
        /// プロトコル設定を保存します
        /// </summary>
        private void SaveProtocolSettings()
        {
            if (_mProtocolsAreDirty)
            {
                _settings.Protocols = _mProtocols.Values.ToList();
                _mProtocolsAreDirty = false;
            }
        }

        /// <summary>
        /// ファイルタイプ設定を保存します
        /// </summary>
        private void SaveFileTypeSettings()
        {
            if (_mFileTypesAreDirty)
            {
                _settings.FileTypes = _mFileTypes.Values.ToList();
                _mFileTypesAreDirty = false;
            }
        }

        /// <summary>
        /// フォーカス設定を保存します
        /// </summary>
        private void SaveFocusSettings()
        {
            _settings.ShowFocus = _mFocusSettings.ShowFocus;
            _settings.FocusBoxColor = _mFocusSettings.BoxColor.ToArgb();
            _settings.FocusBoxLineWidth = _mFocusSettings.BoxWidth;
        }

        /// <summary>
        /// ブラウザを追加します
        /// </summary>
        /// <param name="browser">追加するブラウザ</param>
        public void AddBrowser(Browser browser)
        {
            if (browser.Guid == Guid.Empty)
            {
                browser.Guid = Guid.NewGuid();
            }
            _mBrowser[browser.Guid] = browser;
            _isModified = true;
        }

        /// <summary>
        /// ブラウザを更新します
        /// </summary>
        /// <param name="browser">更新するブラウザ</param>
        public void UpdateBrowser(Browser browser)
        {
            if (_mBrowser.ContainsKey(browser.Guid))
            {
                _mBrowser[browser.Guid] = browser;
                _isModified = true;
            }
        }

        /// <summary>
        /// ブラウザを削除します
        /// </summary>
        /// <param name="browserGuid">削除するブラウザのGUID</param>
        public void RemoveBrowser(Guid browserGuid)
        {
            if (_mBrowser.Remove(browserGuid))
            {
                _isModified = true;
            }
        }

        /// <summary>
        /// URLを追加します
        /// </summary>
        /// <param name="url">追加するURL</param>
        public void AddURL(URL url)
        {
            if (url.Guid == Guid.Empty)
            {
                url.Guid = Guid.NewGuid();
            }
            _mURLs[url.Guid] = url;
            _isModified = true;
        }

        /// <summary>
        /// URLを更新します
        /// </summary>
        /// <param name="url">更新するURL</param>
        public void UpdateURL(URL url)
        {
            if (_mURLs.ContainsKey(url.Guid))
            {
                _mURLs[url.Guid] = url;
                _isModified = true;
            }
        }

        /// <summary>
        /// URLを削除します
        /// </summary>
        /// <param name="urlGuid">削除するURLのGUID</param>
        public void RemoveURL(Guid urlGuid)
        {
            if (_mURLs.Remove(urlGuid))
            {
                _isModified = true;
            }
        }

        /// <summary>
        /// プロトコルを追加します
        /// </summary>
        /// <param name="protocol">追加するプロトコル</param>
        public void AddProtocol(Protocol protocol)
        {
            if (protocol.Guid == Guid.Empty)
            {
                protocol.Guid = Guid.NewGuid();
            }
            _mProtocols[protocol.Guid] = protocol;
            _mProtocolsAreDirty = true;
            _isModified = true;
        }

        /// <summary>
        /// プロトコルを更新します
        /// </summary>
        /// <param name="protocol">更新するプロトコル</param>
        public void UpdateProtocol(Protocol protocol)
        {
            if (_mProtocols.ContainsKey(protocol.Guid))
            {
                _mProtocols[protocol.Guid] = protocol;
                _mProtocolsAreDirty = true;
                _isModified = true;
            }
        }

        /// <summary>
        /// プロトコルを削除します
        /// </summary>
        /// <param name="protocolGuid">削除するプロトコルのGUID</param>
        public void RemoveProtocol(Guid protocolGuid)
        {
            if (_mProtocols.Remove(protocolGuid))
            {
                _mProtocolsAreDirty = true;
                _isModified = true;
            }
        }

        /// <summary>
        /// ファイルタイプを追加します
        /// </summary>
        /// <param name="fileType">追加するファイルタイプ</param>
        public void AddFileType(FileType fileType)
        {
            if (fileType.Guid == Guid.Empty)
            {
                fileType.Guid = Guid.NewGuid();
            }
            _mFileTypes[fileType.Guid] = fileType;
            _mFileTypesAreDirty = true;
            _isModified = true;
        }

        /// <summary>
        /// ファイルタイプを更新します
        /// </summary>
        /// <param name="fileType">更新するファイルタイプ</param>
        public void UpdateFileType(FileType fileType)
        {
            if (_mFileTypes.ContainsKey(fileType.Guid))
            {
                _mFileTypes[fileType.Guid] = fileType;
                _mFileTypesAreDirty = true;
                _isModified = true;
            }
        }

        /// <summary>
        /// ファイルタイプを削除します
        /// </summary>
        /// <param name="fileTypeGuid">削除するファイルタイプのGUID</param>
        public void RemoveFileType(Guid fileTypeGuid)
        {
            if (_mFileTypes.Remove(fileTypeGuid))
            {
                _mFileTypesAreDirty = true;
                _isModified = true;
            }
        }

        /// <summary>
        /// 設定が変更されたことをマークします
        /// </summary>
        public void MarkAsModified()
        {
            _isModified = true;
        }

        /// <summary>
        /// 設定の変更をリセットします
        /// </summary>
        public void ResetModification()
        {
            _isModified = false;
        }
    }
}
