using System.Runtime.InteropServices;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// 一般的なユーティリティ機能を提供するクラス
    /// </summary>
    public static class GeneralUtilities
    {
        #region Win32 API
        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint attr, ref int attrValue, uint attrSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const uint DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const uint DWMWA_MICA_EFFECT = 1029;
        #endregion

        /// <summary>
        /// Aero効果が有効かどうかをチェックします
        /// </summary>
        /// <returns>Aero効果が有効な場合はtrue</returns>
        public static bool IsAeroEnabled()
        {
            try
            {
                bool enabled;
                DwmIsCompositionEnabled(out enabled);
                return enabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// フォームにAero効果を適用します
        /// </summary>
        /// <param name="form">対象のフォーム</param>
        public static void MakeFormGlassy(Form form)
        {
            try
            {
                var margins = new MARGINS
                {
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 0,
                    bottomHeight = 0
                };

                DwmExtendFrameIntoClientArea(form.Handle, ref margins);
                
                // フォームの背景を透明にする
                form.BackColor = Color.Transparent;
                form.TransparencyKey = Color.Transparent;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("GeneralUtilities.MakeFormGlassy", "Aero効果の適用に失敗", ex.Message);
            }
        }

        /// <summary>
        /// フォームにMica効果を適用します（Windows 11）
        /// </summary>
        /// <param name="form">対象のフォーム</param>
        public static void ApplyMicaEffect(Form form)
        {
            try
            {
                int value = 1;
                DwmSetWindowAttribute(form.Handle, DWMWA_MICA_EFFECT, ref value, sizeof(int));
            }
            catch (Exception ex)
            {
                Logger.LogWarning("GeneralUtilities.ApplyMicaEffect", "Mica効果の適用に失敗", ex.Message);
            }
        }

        /// <summary>
        /// ダークモードを適用します
        /// </summary>
        /// <param name="form">対象のフォーム</param>
        public static void ApplyDarkMode(Form form)
        {
            try
            {
                int value = 1;
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
            }
            catch (Exception ex)
            {
                Logger.LogWarning("GeneralUtilities.ApplyDarkMode", "ダークモードの適用に失敗", ex.Message);
            }
        }

        /// <summary>
        /// 一意のIDを生成します
        /// </summary>
        /// <returns>一意のGUID</returns>
        public static Guid GetUniqueID()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// ファイルパスが有効かどうかをチェックします
        /// </summary>
        /// <param name="path">チェック対象のパス</param>
        /// <returns>有効な場合はtrue</returns>
        public static bool IsValidPath(string path)
        {
            try
            {
                return !string.IsNullOrEmpty(path) && 
                       System.IO.Path.IsPathRooted(path) && 
                       System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// プロセスが実行中かどうかをチェックします
        /// </summary>
        /// <param name="processName">プロセス名</param>
        /// <returns>実行中の場合はtrue</returns>
        public static bool IsProcessRunning(string processName)
        {
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName(processName);
                return processes.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// レジストリから値を読み取ります
        /// </summary>
        /// <param name="keyPath">レジストリキーパス</param>
        /// <param name="valueName">値の名前</param>
        /// <returns>値（見つからない場合はnull）</returns>
        public static string? GetRegistryValue(string keyPath, string valueName)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath);
                return key?.GetValue(valueName) as string;
            }
            catch
            {
                return null;
            }
        }
    }
}


