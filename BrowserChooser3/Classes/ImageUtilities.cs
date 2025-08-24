using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// 画像処理ユーティリティクラス
    /// Browser Chooser 2のImageUtilitiesと互換性を保ちます
    /// </summary>
    public static class ImageUtilities
    {
        /// <summary>
        /// ブラウザのアイコン画像を取得します
        /// </summary>
        /// <param name="browser">ブラウザ情報</param>
        /// <param name="useCache">キャッシュを使用するかどうか</param>
        /// <returns>アイコン画像</returns>
        public static Image? GetImage(Browser browser, bool useCache = true)
        {
            Logger.LogInfo("ImageUtilities.GetImage", "Start", browser.Name, useCache);

            try
            {
                // アイコンパスが指定されている場合
                if (!string.IsNullOrEmpty(browser.ImagePath) && File.Exists(browser.ImagePath))
                {
                    return Image.FromFile(browser.ImagePath);
                }

                // 実行ファイルからアイコンを抽出
                if (!string.IsNullOrEmpty(browser.Target) && File.Exists(browser.Target))
                {
                    return ExtractIconFromFile(browser.Target, browser.IconIndex);
                }

                // デフォルトアイコンを返す
                return GetDefaultBrowserIcon();
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.GetImage", "画像取得エラー", ex.Message, ex.StackTrace ?? "");
                return GetDefaultBrowserIcon();
            }
        }

        /// <summary>
        /// ファイルからアイコンを抽出します
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="iconIndex">アイコンインデックス</param>
        /// <returns>抽出されたアイコン画像</returns>
        public static Image? ExtractIconFromFile(string filePath, int iconIndex = 0)
        {
            Logger.LogInfo("ImageUtilities.ExtractIconFromFile", "Start", filePath, iconIndex);

            try
            {
                // System.Drawing.Iconを使用してアイコンを抽出
                var icon = Icon.ExtractAssociatedIcon(filePath);
                if (icon != null)
                {
                    return icon.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ExtractIconFromFile", "アイコン抽出エラー", ex.Message, ex.StackTrace ?? "");
            }

            return null;
        }

        /// <summary>
        /// デフォルトブラウザアイコンを取得します
        /// </summary>
        /// <returns>デフォルトアイコン画像</returns>
        public static Image GetDefaultBrowserIcon()
        {
            try
            {
                // リソースからデフォルトアイコンを読み込み
                return Properties.Resources.BrowserChooserIcon;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.GetDefaultBrowserIcon", "デフォルトアイコン取得エラー", ex.Message, ex.StackTrace ?? "");
                
                // フォールバック: 32x32の透明画像を作成
                var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bitmap);
                g.Clear(Color.Transparent);
                return bitmap;
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
            Logger.LogInfo("ImageUtilities.ResizeImage", "Start", width, height);

            try
            {
                var resized = new Bitmap(width, height);
                using var graphics = Graphics.FromImage(resized);
                
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                
                graphics.DrawImage(image, 0, 0, width, height);
                
                return resized;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ResizeImage", "リサイズエラー", ex.Message, ex.StackTrace ?? "");
                return image;
            }
        }

        /// <summary>
        /// 画像をスケールします
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="scale">スケール倍率</param>
        /// <returns>スケールされた画像</returns>
        public static Image ScaleImage(Image image, double scale)
        {
            if (Math.Abs(scale - 1.0) < 0.001)
            {
                return image; // スケールが1.0の場合は元画像をそのまま返す
            }

            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);
            
            return ResizeImage(image, newWidth, newHeight);
        }

        /// <summary>
        /// 2つの画像をマージします
        /// </summary>
        /// <param name="baseImage">ベース画像</param>
        /// <param name="overlayImage">オーバーレイ画像</param>
        /// <returns>マージされた画像</returns>
        public static Image MergeImages(Image baseImage, Image overlayImage)
        {
            Logger.LogInfo("ImageUtilities.MergeImages", "Start");

            try
            {
                var merged = new Bitmap(baseImage.Width, baseImage.Height);
                using var graphics = Graphics.FromImage(merged);
                
                // ベース画像を描画
                graphics.DrawImage(baseImage, 0, 0);
                
                // オーバーレイ画像を右下に描画
                int overlayX = baseImage.Width - overlayImage.Width;
                int overlayY = baseImage.Height - overlayImage.Height;
                graphics.DrawImage(overlayImage, overlayX, overlayY);
                
                return merged;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.MergeImages", "画像マージエラー", ex.Message, ex.StackTrace ?? "");
                return baseImage;
            }
        }

        /// <summary>
        /// 画像を円形にクリップします
        /// </summary>
        /// <param name="image">元画像</param>
        /// <returns>円形にクリップされた画像</returns>
        public static Image CreateCircularImage(Image image)
        {
            Logger.LogInfo("ImageUtilities.CreateCircularImage", "Start");

            try
            {
                int size = Math.Min(image.Width, image.Height);
                var circular = new Bitmap(size, size);
                
                using var graphics = Graphics.FromImage(circular);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // 円形のクリッピングパスを作成
                using var path = new GraphicsPath();
                path.AddEllipse(0, 0, size, size);
                graphics.SetClip(path);
                
                // 画像を描画
                graphics.DrawImage(image, 0, 0, size, size);
                
                return circular;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.CreateCircularImage", "円形画像作成エラー", ex.Message, ex.StackTrace ?? "");
                return image;
            }
        }

        /// <summary>
        /// 画像にドロップシャドウ効果を追加します
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="shadowOffset">影のオフセット</param>
        /// <param name="shadowColor">影の色</param>
        /// <returns>ドロップシャドウ付きの画像</returns>
        public static Image AddDropShadow(Image image, Point shadowOffset, Color shadowColor)
        {
            Logger.LogInfo("ImageUtilities.AddDropShadow", "Start");

            try
            {
                int newWidth = image.Width + Math.Abs(shadowOffset.X);
                int newHeight = image.Height + Math.Abs(shadowOffset.Y);
                
                var shadowed = new Bitmap(newWidth, newHeight);
                using var graphics = Graphics.FromImage(shadowed);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // 影を描画
                using var shadowBrush = new SolidBrush(shadowColor);
                graphics.FillRectangle(shadowBrush, shadowOffset.X, shadowOffset.Y, image.Width, image.Height);
                
                // 元画像を描画
                graphics.DrawImage(image, 0, 0);
                
                return shadowed;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.AddDropShadow", "ドロップシャドウ追加エラー", ex.Message, ex.StackTrace ?? "");
                return image;
            }
        }
    }
}

