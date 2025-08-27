using System.Xml.Serialization;

namespace BrowserChooser3.Classes.Models
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
        
        /// <summary>URL（Browser Chooser 2互換）</summary>
        public string URLValue { get; set; } = string.Empty;
        
        /// <summary>関連するブラウザのGUID</summary>
        public Guid BrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>遅延時間</summary>
        public int Delay { get; set; } = 0;
        
        /// <summary>遅延時間（Browser Chooser 2互換）</summary>
        public int DelayTime { get; set; } = 0;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>自動クローズ</summary>
        public bool AutoClose { get; set; } = false;
        
        /// <summary>URLパターン（Browser Chooser 2互換）</summary>
        public string Pattern { get; set; } = string.Empty;
        
        /// <summary>対応ブラウザのGUIDリスト（Browser Chooser 2互換）</summary>
        public List<Guid> SupportingBrowsers { get; set; } = new();
        
        /// <summary>カテゴリ（Browser Chooser 2互換）</summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>アクティブ状態（Browser Chooser 2互換）</summary>
        public bool Active { get; set; } = true;
        
        /// <summary>
        /// URLのクローンを作成（Browser Chooser 2互換）
        /// </summary>
        public URL Clone()
        {
            return new URL
            {
                Guid = this.Guid,
                Name = this.Name,
                URLPattern = this.URLPattern,
                URLValue = this.URLValue,
                BrowserGuid = this.BrowserGuid,
                Delay = this.Delay,
                DelayTime = this.DelayTime,
                IsActive = this.IsActive,
                AutoClose = this.AutoClose,
                Pattern = this.Pattern,
                SupportingBrowsers = new List<Guid>(this.SupportingBrowsers),
                Category = this.Category,
                Active = this.Active
            };
        }
    }
}
