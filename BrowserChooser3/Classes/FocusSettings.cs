using System.Drawing;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// フォーカス設定クラス
    /// </summary>
    public class FocusSettings
    {
        /// <summary>
        /// フォーカス表示フラグ
        /// </summary>
        public bool ShowFocus { get; set; } = false;

        /// <summary>
        /// フォーカスボックスの色
        /// </summary>
        public Color BoxColor { get; set; } = Color.Red;

        /// <summary>
        /// フォーカスボックスの線幅
        /// </summary>
        public int BoxWidth { get; set; } = 2;
    }
}
