using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic; // Added for List
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services
{
    /// <summary>
    /// レガシー設定インポート機能を提供するクラス
    /// Browser Chooser 2の設定ファイルをBrowserChooser3に変換します
    /// </summary>
    public static class Importer
    {
        /// <summary>
        /// Browser Chooser 2の設定ファイル名
        /// </summary>
        private const string BC2_CONFIG_FILE = "BrowserChooser2Config.xml";

        /// <summary>
        /// レガシー設定をインポートします
        /// </summary>
        /// <param name="sourcePath">ソースパス</param>
        /// <param name="targetSettings">ターゲット設定</param>
        /// <returns>インポートが成功した場合はtrue</returns>
        public static bool ImportLegacySettings(string sourcePath, Settings targetSettings)
        {
            Logger.LogInfo("Importer.ImportLegacySettings", "レガシー設定インポート開始", sourcePath);

            try
            {
                var configPath = Path.Combine(sourcePath, BC2_CONFIG_FILE);
                if (!File.Exists(configPath))
                {
                    Logger.LogInfo("Importer.ImportLegacySettings", "レガシー設定ファイルが見つかりません", configPath);
                    return false;
                }

                // Browser Chooser 2の設定を読み込み
                var legacySettings = LoadBC2Settings(configPath);
                if (legacySettings == null)
                {
                    Logger.LogError("Importer.ImportLegacySettings", "レガシー設定の読み込みに失敗", configPath);
                    return false;
                }

                // 設定を変換
                ConvertSettings(legacySettings, targetSettings);

                Logger.LogInfo("Importer.ImportLegacySettings", "レガシー設定インポート完了", configPath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Importer.ImportLegacySettings", "レガシー設定インポートエラー", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Browser Chooser 2の設定ファイルを読み込みます
        /// </summary>
        /// <param name="configPath">設定ファイルパス</param>
        /// <returns>BC2Settingsオブジェクト</returns>
        private static BC2Settings? LoadBC2Settings(string configPath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(BC2Settings));
                using var reader = new StreamReader(configPath);
                var settings = (BC2Settings?)serializer.Deserialize(reader);
                return settings;
            }
            catch (Exception ex)
            {
                Logger.LogError("Importer.LoadBC2Settings", "BC2設定読み込みエラー", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 設定を変換します
        /// </summary>
        /// <param name="source">ソース設定</param>
        /// <param name="target">ターゲット設定</param>
        private static void ConvertSettings(BC2Settings source, Settings target)
        {
            Logger.LogInfo("Importer.ConvertSettings", "設定変換開始");

            try
            {
                // 基本設定の変換
                target.FileVersion = source.FileVersion;
                target.IconWidth = source.IconWidth;
                target.IconHeight = source.IconHeight;
                target.IconGapWidth = source.IconGapWidth;
                target.IconGapHeight = source.IconGapHeight;
                target.IconScale = (double)source.IconScale;
                target.OptionsShortcut = source.OptionsShortcut;
                target.DefaultMessage = source.DefaultMessage;
                target.DefaultDelay = source.DefaultDelay;
                target.DefaultBrowserGuid = source.DefaultBrowserGuid;
                target.ShowFocus = source.ShowFocus;
                target.UseAero = source.UseAero;
                target.FocusBoxLineWidth = source.FocusBoxLineWidth;
                target.FocusBoxColor = source.FocusBoxColor;
                target.UserAgent = source.UserAgent;
                target.BackgroundColor = source.BackgroundColor;
                target.StartingPosition = source.StartingPosition;
                target.OffsetX = source.OffsetX;
                target.OffsetY = source.OffsetY;
                target.AllowStayOpen = source.AllowStayOpen;
                target.Canonicalize = source.Canonicalize;
                target.CanonicalizeAppendedText = source.CanonicalizeAppendedText;
                target.EnableLogging = source.EnableLogging;
                target.LogLevel = source.LogLevel;

                // ブラウザの変換
                ConvertBrowsers(source.Browsers, target.Browsers);

                // プロトコルの変換
                ConvertProtocols(source.Protocols, target.Protocols);

                // ファイルタイプの変換
                ConvertFileTypes(source.FileTypes, target.FileTypes);

                // URLの変換
                ConvertURLs(source.URLs, target.URLs);

                Logger.LogInfo("Importer.ConvertSettings", "設定変換完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("Importer.ConvertSettings", "設定変換エラー", ex.Message);
            }
        }

        /// <summary>
        /// ブラウザ設定を変換します
        /// </summary>
        /// <param name="source">ソースブラウザリスト</param>
        /// <param name="target">ターゲットブラウザリスト</param>
        private static void ConvertBrowsers(BC2Browser[] source, List<Browser> target)
        {
            target.Clear();
            foreach (var bc2Browser in source)
            {
                var browser = new Browser
                {
                    Name = bc2Browser.Name,
                    Target = bc2Browser.Target,
                    Arguments = bc2Browser.Arguments,
                    Guid = bc2Browser.Guid,
                    Hotkey = bc2Browser.Hotkey,
                    PosX = bc2Browser.PosX,
                    PosY = bc2Browser.PosY,
                    Scale = (double)bc2Browser.Scale,
                    IconIndex = bc2Browser.IconIndex,
                    Category = bc2Browser.Category,
                    Visible = bc2Browser.Visible,
                    IsDefault = bc2Browser.IsDefault,
                    CustomImagePath = bc2Browser.CustomImagePath
                };
                target.Add(browser);
            }
        }

        /// <summary>
        /// プロトコル設定を変換します
        /// </summary>
        /// <param name="source">ソースプロトコルリスト</param>
        /// <param name="target">ターゲットプロトコルリスト</param>
        private static void ConvertProtocols(BC2Protocol[] source, List<Protocol> target)
        {
            target.Clear();
            foreach (var bc2Protocol in source)
            {
                var protocol = new Protocol
                {
                    Name = bc2Protocol.Name,
                    Header = bc2Protocol.Header,
                    Guid = bc2Protocol.Guid,
                    SupportingBrowsers = new List<Guid>(bc2Protocol.SupportingBrowsers),
                    Category = bc2Protocol.Category,
                    Active = bc2Protocol.Active
                };
                target.Add(protocol);
            }
        }

        /// <summary>
        /// ファイルタイプ設定を変換します
        /// </summary>
        /// <param name="source">ソースファイルタイプリスト</param>
        /// <param name="target">ターゲットファイルタイプリスト</param>
        private static void ConvertFileTypes(BC2FileType[] source, List<FileType> target)
        {
            target.Clear();
            foreach (var bc2FileType in source)
            {
                var fileType = new FileType
                {
                    Name = bc2FileType.Name,
                    Extension = bc2FileType.Extension,
                    Guid = bc2FileType.Guid,
                    SupportingBrowsers = new List<Guid>(bc2FileType.SupportingBrowsers),
                    Category = bc2FileType.Category,
                    Active = bc2FileType.Active
                };
                target.Add(fileType);
            }
        }

        /// <summary>
        /// URL設定を変換します
        /// </summary>
        /// <param name="source">ソースURLリスト</param>
        /// <param name="target">ターゲットURLリスト</param>
        private static void ConvertURLs(BC2URL[] source, List<URL> target)
        {
            target.Clear();
            foreach (var bc2URL in source)
            {
                var url = new URL
                {
                    Name = bc2URL.Name,
                    Pattern = bc2URL.Pattern,
                    Guid = bc2URL.Guid,
                    SupportingBrowsers = new List<Guid>(bc2URL.SupportingBrowsers),
                    Category = bc2URL.Category,
                    Active = bc2URL.Active
                };
                target.Add(url);
            }
        }

        /// <summary>
        /// レガシー設定ファイルの存在をチェックします
        /// </summary>
        /// <param name="path">チェックするパス</param>
        /// <returns>存在する場合はtrue</returns>
        public static bool HasLegacySettings(string path)
        {
            var configPath = Path.Combine(path, BC2_CONFIG_FILE);
            return File.Exists(configPath);
        }

        /// <summary>
        /// レガシー設定ファイルのバックアップを作成します
        /// </summary>
        /// <param name="sourcePath">ソースパス</param>
        /// <param name="backupPath">バックアップパス</param>
        /// <returns>バックアップが成功した場合はtrue</returns>
        public static bool CreateBackup(string sourcePath, string backupPath)
        {
            try
            {
                var sourceFile = Path.Combine(sourcePath, BC2_CONFIG_FILE);
                var backupFile = Path.Combine(backupPath, $"{BC2_CONFIG_FILE}.backup.{DateTime.Now:yyyyMMdd_HHmmss}");

                if (File.Exists(sourceFile))
                {
                    Directory.CreateDirectory(backupPath);
                    File.Copy(sourceFile, backupFile);
                    Logger.LogInfo("Importer.CreateBackup", "バックアップ作成完了", backupFile);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError("Importer.CreateBackup", "バックアップ作成エラー", ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// Browser Chooser 2の設定クラス（XMLデシリアライゼーション用）
    /// </summary>
    [XmlRoot("Settings")]
    public class BC2Settings
    {
        /// <summary>ファイルバージョン</summary>
        [XmlElement("FileVersion")] public int FileVersion { get; set; }
        /// <summary>アイコン幅</summary>
        [XmlElement("IconWidth")] public int IconWidth { get; set; }
        /// <summary>アイコン高さ</summary>
        [XmlElement("IconHeight")] public int IconHeight { get; set; }
        /// <summary>アイコン間隔（幅）</summary>
        [XmlElement("IconGapWidth")] public int IconGapWidth { get; set; }
        /// <summary>アイコン間隔（高さ）</summary>
        [XmlElement("IconGapHeight")] public int IconGapHeight { get; set; }
        /// <summary>アイコンスケール</summary>
        [XmlElement("IconScale")] public decimal IconScale { get; set; }
        /// <summary>オプションショートカットキー</summary>
        [XmlElement("OptionsShortcut")] public char OptionsShortcut { get; set; }
        /// <summary>デフォルトメッセージ</summary>
        [XmlElement("DefaultMessage")] public string DefaultMessage { get; set; } = string.Empty;
        /// <summary>デフォルト遅延時間</summary>
        [XmlElement("DefaultDelay")] public int DefaultDelay { get; set; }
        /// <summary>デフォルトブラウザのGUID</summary>
        [XmlElement("DefaultBrowserGuid")] public Guid DefaultBrowserGuid { get; set; }
        /// <summary>フォーカス表示の有効/無効</summary>
        [XmlElement("ShowFocus")] public bool ShowFocus { get; set; }
        /// <summary>Aero効果の有効/無効</summary>
        [XmlElement("UseAreo")] public bool UseAero { get; set; }
        /// <summary>フォーカスボックスの線幅</summary>
        [XmlElement("FocusBoxLineWidth")] public int FocusBoxLineWidth { get; set; }
        /// <summary>フォーカスボックスの色</summary>
        [XmlElement("FocusBoxColor")] public int FocusBoxColor { get; set; }
        /// <summary>ユーザーエージェント</summary>
        [XmlElement("UserAgent")] public string UserAgent { get; set; } = string.Empty;
        /// <summary>背景色</summary>
        [XmlElement("BackgroundColor")] public int BackgroundColor { get; set; }
        /// <summary>開始位置</summary>
        [XmlElement("StartingPosition")] public int StartingPosition { get; set; }
        /// <summary>X座標オフセット</summary>
        [XmlElement("OffsetX")] public int OffsetX { get; set; }
        /// <summary>Y座標オフセット</summary>
        [XmlElement("OffsetY")] public int OffsetY { get; set; }
        /// <summary>開いたままにするかどうか</summary>
        [XmlElement("AllowStayOpen")] public bool AllowStayOpen { get; set; }
        /// <summary>正規化の有効/無効</summary>
        [XmlElement("Canonicalize")] public bool Canonicalize { get; set; }
        /// <summary>正規化時に追加するテキスト</summary>
        [XmlElement("CanonicalizeAppendedText")] public string CanonicalizeAppendedText { get; set; } = string.Empty;
        /// <summary>ログの有効/無効</summary>
        [XmlElement("EnableLogging")] public bool EnableLogging { get; set; }
        /// <summary>ログレベル</summary>
        [XmlElement("LogLevel")] public int LogLevel { get; set; }

        /// <summary>ブラウザリスト</summary>
        [XmlArray("Browsers")] public BC2Browser[] Browsers { get; set; } = Array.Empty<BC2Browser>();
        /// <summary>プロトコルリスト</summary>
        [XmlArray("Protocols")] public BC2Protocol[] Protocols { get; set; } = Array.Empty<BC2Protocol>();
        /// <summary>ファイルタイプリスト</summary>
        [XmlArray("FileTypes")] public BC2FileType[] FileTypes { get; set; } = Array.Empty<BC2FileType>();
        /// <summary>URLリスト</summary>
        [XmlArray("URLs")] public BC2URL[] URLs { get; set; } = Array.Empty<BC2URL>();
    }

    /// <summary>
    /// Browser Chooser 2のブラウザクラス
    /// </summary>
    public class BC2Browser
    {
        /// <summary>ブラウザ名</summary>
        [XmlElement("Name")] public string Name { get; set; } = string.Empty;
        /// <summary>実行ファイルパス</summary>
        [XmlElement("Target")] public string Target { get; set; } = string.Empty;
        /// <summary>起動引数</summary>
        [XmlElement("Arguments")] public string Arguments { get; set; } = string.Empty;
        /// <summary>ブラウザのGUID</summary>
        [XmlElement("Guid")] public Guid Guid { get; set; }
        /// <summary>ホットキー</summary>
        [XmlElement("Hotkey")] public char Hotkey { get; set; }
        /// <summary>X座標</summary>
        [XmlElement("PosX")] public int PosX { get; set; }
        /// <summary>Y座標</summary>
        [XmlElement("PosY")] public int PosY { get; set; }
        /// <summary>スケール</summary>
        [XmlElement("Scale")] public decimal Scale { get; set; }
        /// <summary>アイコンインデックス</summary>
        [XmlElement("IconIndex")] public int IconIndex { get; set; }
        /// <summary>カテゴリ</summary>
        [XmlElement("Category")] public string Category { get; set; } = string.Empty;
        /// <summary>表示フラグ</summary>
        [XmlElement("Visible")] public bool Visible { get; set; }
        /// <summary>デフォルトブラウザフラグ</summary>
        [XmlElement("IsDefault")] public bool IsDefault { get; set; }
        /// <summary>カスタム画像パス</summary>
        [XmlElement("CustomImagePath")] public string CustomImagePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Browser Chooser 2のプロトコルクラス
    /// </summary>
    public class BC2Protocol
    {
        /// <summary>プロトコル名</summary>
        [XmlElement("Name")] public string Name { get; set; } = string.Empty;
        /// <summary>プロトコルヘッダー</summary>
        [XmlElement("Header")] public string Header { get; set; } = string.Empty;
        /// <summary>プロトコルのGUID</summary>
        [XmlElement("Guid")] public Guid Guid { get; set; }
        /// <summary>対応ブラウザのGUIDリスト</summary>
        [XmlArray("SupportingBrowsers")] public Guid[] SupportingBrowsers { get; set; } = Array.Empty<Guid>();
        /// <summary>カテゴリ</summary>
        [XmlElement("Category")] public string Category { get; set; } = string.Empty;
        /// <summary>アクティブフラグ</summary>
        [XmlElement("Active")] public bool Active { get; set; }
    }

    /// <summary>
    /// Browser Chooser 2のファイルタイプクラス
    /// </summary>
    public class BC2FileType
    {
        /// <summary>ファイルタイプ名</summary>
        [XmlElement("Name")] public string Name { get; set; } = string.Empty;
        /// <summary>ファイル拡張子</summary>
        [XmlElement("Extension")] public string Extension { get; set; } = string.Empty;
        /// <summary>ファイルタイプのGUID</summary>
        [XmlElement("Guid")] public Guid Guid { get; set; }
        /// <summary>対応ブラウザのGUIDリスト</summary>
        [XmlArray("SupportingBrowsers")] public Guid[] SupportingBrowsers { get; set; } = Array.Empty<Guid>();
        /// <summary>カテゴリ</summary>
        [XmlElement("Category")] public string Category { get; set; } = string.Empty;
        /// <summary>アクティブフラグ</summary>
        [XmlElement("Active")] public bool Active { get; set; }
    }

    /// <summary>
    /// Browser Chooser 2のURLクラス
    /// </summary>
    public class BC2URL
    {
        /// <summary>URL名</summary>
        [XmlElement("Name")] public string Name { get; set; } = string.Empty;
        /// <summary>URLパターン</summary>
        [XmlElement("Pattern")] public string Pattern { get; set; } = string.Empty;
        /// <summary>URLのGUID</summary>
        [XmlElement("Guid")] public Guid Guid { get; set; }
        /// <summary>対応ブラウザのGUIDリスト</summary>
        [XmlArray("SupportingBrowsers")] public Guid[] SupportingBrowsers { get; set; } = Array.Empty<Guid>();
        /// <summary>カテゴリ</summary>
        [XmlElement("Category")] public string Category { get; set; } = string.Empty;
        /// <summary>アクティブフラグ</summary>
        [XmlElement("Active")] public bool Active { get; set; }
    }
}
