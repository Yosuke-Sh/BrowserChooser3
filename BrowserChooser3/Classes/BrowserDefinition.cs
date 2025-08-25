using System.Collections.Generic;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// オンライン定義のブラウザ情報を表すクラス
    /// </summary>
    public class BrowserDefinition
    {
        /// <summary>
        /// ブラウザ名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ブラウザの実行ファイルパス（複数指定可能）
        /// </summary>
        public List<string> Paths { get; set; } = new List<string>();

        /// <summary>
        /// ブラウザのバージョン
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// ブラウザの説明
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// ブラウザのWebサイトURL
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// ブラウザのカテゴリ
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// ブラウザがアクティブかどうか
        /// </summary>
        public bool Active { get; set; } = true;
    }
}

