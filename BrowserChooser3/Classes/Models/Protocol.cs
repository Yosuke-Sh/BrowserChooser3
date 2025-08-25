using System.Xml.Serialization;

namespace BrowserChooser3.Classes.Models
{
    /// <summary>
    /// プロトコル情報を管理するクラス
    /// </summary>
    [Serializable]
    public class Protocol
    {
        /// <summary>プロトコルの一意識別子</summary>
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        /// <summary>プロトコル名</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>プロトコル名（内部用）</summary>
        public string ProtocolName { get; set; } = string.Empty;
        
        /// <summary>プロトコルヘッダー</summary>
        public string Header { get; set; } = string.Empty;
        
        /// <summary>関連するブラウザのGUID</summary>
        public Guid BrowserGuid { get; set; } = Guid.Empty;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>対応ブラウザのGUIDリスト</summary>
        public List<Guid> SupportingBrowsers { get; set; } = new();
        
        /// <summary>デフォルトカテゴリ（Browser Chooser 2互換）</summary>
        public List<string> DefaultCategories { get; set; } = new();
        
        /// <summary>カテゴリ（Browser Chooser 2互換）</summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>アクティブ状態（Browser Chooser 2互換）</summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Protocolクラスの新しいインスタンスを初期化します
        /// </summary>
        public Protocol()
        {
        }

        /// <summary>
        /// Protocolクラスの新しいインスタンスを初期化します（Browser Chooser 2互換）
        /// </summary>
        /// <param name="name">プロトコル名</param>
        /// <param name="header">プロトコルヘッダー</param>
        /// <param name="supportingBrowsers">対応ブラウザのGUIDリスト</param>
        /// <param name="categories">カテゴリリスト</param>
        public Protocol(string name, string header, List<Guid> supportingBrowsers, List<string> categories)
        {
            Name = name;
            Header = header;
            SupportingBrowsers = new List<Guid>(supportingBrowsers);
            DefaultCategories = new List<string>(categories);
            Category = categories.FirstOrDefault() ?? "Default";
            Active = true;
        }
        
        /// <summary>
        /// プロトコルのクローンを作成（Browser Chooser 2互換）
        /// </summary>
        public Protocol Clone()
        {
            return new Protocol
            {
                Guid = this.Guid,
                Name = this.Name,
                ProtocolName = this.ProtocolName,
                Header = this.Header,
                BrowserGuid = this.BrowserGuid,
                IsActive = this.IsActive,
                SupportingBrowsers = new List<Guid>(this.SupportingBrowsers),
                DefaultCategories = new List<string>(this.DefaultCategories)
            };
        }
    }
}
