using System.Drawing;
using BrowserChooser3.Classes;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// 画像処理ユーティリティクラス
    /// </summary>
    public static class ImageUtilities
    {
        /// <summary>
        /// ブラウザのアイコンを取得します
        /// </summary>
        /// <param name="browser">ブラウザオブジェクト</param>
        /// <param name="useLargeIcon">大きなアイコンを使用するかどうか</param>
        /// <returns>ブラウザのアイコン画像</returns>
        public static Image GetImage(Browser browser, bool useLargeIcon)
        {
            try
            {
                if (string.IsNullOrEmpty(browser.Target))
                    return GetDefaultIcon();

                // ファイルパスからアイコンを取得
                if (System.IO.File.Exists(browser.Target))
                {
                    var icon = Icon.ExtractAssociatedIcon(browser.Target);
                    if (icon != null)
                    {
                        return icon.ToBitmap();
                    }
                }

                // デフォルトアイコンを返す
                return GetDefaultIcon();
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.GetImage", $"ブラウザアイコン取得エラー: {browser.Name}", ex.Message);
                return GetDefaultIcon();
            }
        }

        /// <summary>
        /// 画像を指定サイズにスケールします
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="size">目標サイズ</param>
        /// <returns>スケールされた画像</returns>
        public static Image ScaleImageTo(Image image, Size size)
        {
            try
            {
                if (image == null)
                    return GetDefaultIcon();

                var scaledImage = new Bitmap(size.Width, size.Height);
                using (var graphics = Graphics.FromImage(scaledImage))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, 0, 0, size.Width, size.Height);
                }

                return scaledImage;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ScaleImageTo", "画像スケールエラー", ex.Message);
                return GetDefaultIcon();
            }
        }

        /// <summary>
        /// デフォルトアイコンを取得します
        /// </summary>
        /// <returns>デフォルトアイコン画像</returns>
        private static Image GetDefaultIcon()
        {
            try
            {
                // システムのデフォルトアイコンを使用
                var icon = SystemIcons.Application;
                return icon.ToBitmap();
            }
            catch
            {
                // 最後の手段として、1x1の透明画像を返す
                return new Bitmap(1, 1);
            }
        }

        /// <summary>
        /// 画像をリサイズします
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="width">新しい幅</param>
        /// <param name="height">新しい高さ</param>
        /// <returns>リサイズされた画像</returns>
        public static Image ResizeImage(Image image, int width, int height)
        {
            try
            {
                if (image == null)
                    return GetDefaultIcon();

                var resized = new Bitmap(width, height);
                using var graphics = Graphics.FromImage(resized);
                
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                
                graphics.DrawImage(image, 0, 0, width, height);
                
                return resized;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ResizeImage", "リサイズエラー", ex.Message);
                return GetDefaultIcon();
            }
        }
    }
}

