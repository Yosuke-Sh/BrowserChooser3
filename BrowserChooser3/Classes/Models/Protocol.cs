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
        


        /// <summary>
        /// Protocolクラスの新しいインスタンスを初期化します
        /// </summary>
        public Protocol()
        {
        }


        
        /// <summary>
        /// プロトコルのクローンを作成
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
                SupportingBrowsers = new List<Guid>(this.SupportingBrowsers)
            };
        }
    }
}
