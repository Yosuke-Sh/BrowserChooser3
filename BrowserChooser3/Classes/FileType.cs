using System.Xml.Serialization;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// ファイルタイプ情報を管理するクラス
    /// </summary>
    [Serializable]
    public class FileType
    {
        /// <summary>ファイルタイプの一意識別子</summary>
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        /// <summary>ファイルタイプ名</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>ファイル拡張子</summary>
        public string Extension { get; set; } = string.Empty;
        
        /// <summary>関連するブラウザのGUID</summary>
        public Guid BrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>対応ブラウザのGUIDリスト</summary>
        public List<Guid> SupportingBrowsers { get; set; } = new();
    }
}
