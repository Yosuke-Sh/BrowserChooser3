using System.Xml.Serialization;

namespace BrowserChooser3.Classes.Models
{
    /// <summary>
    /// ブラウザ情報を管理するクラス
    /// </summary>
    [Serializable]
    public class Browser
    {
        /// <summary>ブラウザの一意識別子</summary>
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        /// <summary>ブラウザ名</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>実行ファイルパス</summary>
        public string Target { get; set; } = string.Empty;
        
        /// <summary>起動引数</summary>
        public string Arguments { get; set; } = string.Empty;
        
        /// <summary>アイコン画像パス</summary>
        public string ImagePath { get; set; } = string.Empty;
        
        /// <summary>アイコンインデックス</summary>
        public int IconIndex { get; set; } = 0;
        
        /// <summary>スケール</summary>
        public double Scale { get; set; } = 1.0;
        
        /// <summary>X座標</summary>
        public int X { get; set; } = 0;
        
        /// <summary>Y座標</summary>
        public int Y { get; set; } = 0;
        

        
        /// <summary>表示フラグ</summary>
        public bool Visible { get; set; } = true;
        
        /// <summary>アクティブ状態</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>Edge特殊処理フラグ</summary>
        public bool IsEdge { get; set; } = false;
        
        /// <summary>IE特殊処理フラグ</summary>
        public bool IEBehaviour { get; set; } = false;
        
        /// <summary>IE専用処理フラグ</summary>
        public bool IsIE { get; set; } = false;
        
        /// <summary>標準設定</summary>
        public string Standard { get; set; } = string.Empty;
        
        /// <summary>ホットキー</summary>
        public char Hotkey { get; set; } = '\0';
        
        /// <summary>カテゴリ</summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>デフォルトブラウザフラグ</summary>
        public bool IsDefault { get; set; } = false;
        

        
        /// <summary>
        /// ブラウザのクローンを作成
        /// </summary>
        public Browser Clone()
        {
            return new Browser
            {
                Guid = this.Guid,
                Name = this.Name,
                Target = this.Target,
                Arguments = this.Arguments,
                ImagePath = this.ImagePath,
                IconIndex = this.IconIndex,
                Scale = this.Scale,
                X = this.X,
                Y = this.Y,
                Visible = this.Visible,
                IsActive = this.IsActive,
                IsEdge = this.IsEdge,
                IEBehaviour = this.IEBehaviour,
                IsIE = this.IsIE,
                Standard = this.Standard,
                Hotkey = this.Hotkey,
                Category = this.Category,
                IsDefault = this.IsDefault
            };
        }
    }
}
