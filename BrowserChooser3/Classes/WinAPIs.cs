using System.Runtime.InteropServices;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// Windows API呼び出しクラス
    /// </summary>
    public static class WinAPIs
    {
        /// <summary>
        /// BCM_SETSHIELD定数
        /// </summary>
        public const int BCM_SETSHIELD = 0x160C;

        /// <summary>
        /// SendMessage API
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="Msg">メッセージ</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
    }
}
