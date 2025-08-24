using System.Xml.Serialization;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// URL情報を管理するクラス
    /// </summary>
    [Serializable]
    public class URL
    {
        /// <summary>URLの一意識別子</summary>
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        /// <summary>URL名</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>URLパターン</summary>
        public string URLPattern { get; set; } = string.Empty;
        
        /// <summary>関連するブラウザのGUID</summary>
        public Guid BrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>遅延時間</summary>
        public int Delay { get; set; } = 0;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>自動クローズ</summary>
        public bool AutoClose { get; set; } = false;
    }
}
