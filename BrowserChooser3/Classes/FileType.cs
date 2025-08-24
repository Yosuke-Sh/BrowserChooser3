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
        
        /// <summary>ファイル拡張子（Browser Chooser 2互換）</summary>
        public string Extention { get; set; } = string.Empty;
        
        /// <summary>ファイルタイプ名（Browser Chooser 2互換）</summary>
        public string FiletypeName { get; set; } = string.Empty;
        
        /// <summary>関連するブラウザのGUID</summary>
        public Guid BrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>対応ブラウザのGUIDリスト</summary>
        public List<Guid> SupportingBrowsers { get; set; } = new();
        
        /// <summary>デフォルトカテゴリ（Browser Chooser 2互換）</summary>
        public List<string> DefaultCategories { get; set; } = new();
        
        /// <summary>
        /// ファイルタイプのクローンを作成（Browser Chooser 2互換）
        /// </summary>
        public FileType Clone()
        {
            return new FileType
            {
                Guid = this.Guid,
                Name = this.Name,
                Extension = this.Extension,
                Extention = this.Extention,
                FiletypeName = this.FiletypeName,
                BrowserGuid = this.BrowserGuid,
                IsActive = this.IsActive,
                SupportingBrowsers = new List<Guid>(this.SupportingBrowsers),
                DefaultCategories = new List<string>(this.DefaultCategories)
            };
        }
    }
}
