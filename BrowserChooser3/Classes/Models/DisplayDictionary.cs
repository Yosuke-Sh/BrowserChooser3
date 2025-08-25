namespace BrowserChooser3.Classes.Models
{
    /// <summary>
    /// 表示位置設定クラス
    /// </summary>
    public class DisplayDictionary
    {
        /// <summary>
        /// 表示位置のインデックス
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 表示位置の名前
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 表示位置の説明
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 表示位置設定の新しいインスタンスを初期化します
        /// </summary>
        public DisplayDictionary()
        {
        }

        /// <summary>
        /// 表示位置設定の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="name">名前</param>
        /// <param name="description">説明</param>
        public DisplayDictionary(int index, string name, string description)
        {
            Index = index;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// 文字列表現を返します
        /// </summary>
        /// <returns>名前</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
