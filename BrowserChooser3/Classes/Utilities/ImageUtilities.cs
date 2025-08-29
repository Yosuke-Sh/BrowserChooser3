using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using BrowserChooser3.Classes.Models;

namespace BrowserChooser3.Classes.Utilities
{
    /// <summary>
    /// 画像処理ユーティリティクラス
    /// アイコン抽出、スケーリング、画像変換などの機能を提供します
    /// </summary>
    public static class ImageUtilities
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// ブラウザからアイコンを取得
        /// </summary>
        /// <param name="browser">ブラウザオブジェクト</param>
        /// <param name="useCustomPath">カスタムパスを使用するかどうか</param>
        /// <returns>アイコン画像</returns>
        public static Image? GetImage(Browser browser, bool useCustomPath)
        {
            try
            {
                            string filePath = useCustomPath && !string.IsNullOrEmpty(browser.ImagePath)
                ? browser.ImagePath 
                    : browser.Target;

                Logger.LogInfo("ImageUtilities.GetImage", "アイコン取得開始", browser.Name, filePath, useCustomPath);

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Logger.LogWarning("ImageUtilities.GetImage", "ファイルが存在しません", filePath, browser.Name);
                    return null;
                }

                // アイコンを抽出
                var icon = ExtractIconFromFile(filePath, browser.IconIndex);
                if (icon != null)
                {
                    Logger.LogInfo("ImageUtilities.GetImage", "アイコン抽出成功", browser.Name, filePath);
                    return icon.ToBitmap();
                }

                // フォールバック: 関連付けられたアイコンを取得
                try
                {
                    var associatedIcon = Icon.ExtractAssociatedIcon(filePath);
                    if (associatedIcon != null)
                    {
                        return associatedIcon.ToBitmap();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("ImageUtilities.GetImage", "関連付けられたアイコンの取得に失敗", ex.Message);
                }

                // 最終フォールバック: システムアイコン
                return SystemIcons.Application.ToBitmap();
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.GetImage", "アイコン取得エラー", ex.Message);
                return SystemIcons.Application.ToBitmap();
            }
        }

        /// <summary>
        /// ファイルからアイコンを抽出
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="iconIndex">アイコンインデックス</param>
        /// <returns>アイコン</returns>
        public static Icon? ExtractIconFromFile(string filePath, int iconIndex = 0)
        {
            try
            {
                IntPtr hIcon = ExtractIcon(IntPtr.Zero, filePath, iconIndex);
                if (hIcon != IntPtr.Zero)
                {
                    var icon = Icon.FromHandle(hIcon);
                    // ハンドルをコピーして元のハンドルを解放
                    var clonedIcon = (Icon)icon.Clone();
                    DestroyIcon(hIcon);
                    return clonedIcon;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ExtractIconFromFile", "アイコン抽出エラー", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 画像を指定サイズにスケール
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="targetSize">目標サイズ</param>
        /// <returns>スケールされた画像</returns>
        public static Image ScaleImageTo(Image image, Size targetSize)
        {
            if (image == null)
            {
                return new Bitmap(targetSize.Width, targetSize.Height);
            }

            try
            {
                var scaledImage = new Bitmap(targetSize.Width, targetSize.Height);
                using (var graphics = Graphics.FromImage(scaledImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, targetSize.Width, targetSize.Height);
                }
                return scaledImage;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ScaleImageTo", "画像スケーリングエラー", ex.Message);
                return new Bitmap(targetSize.Width, targetSize.Height);
            }
        }

        /// <summary>
        /// 画像をリサイズ
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="width">新しい幅</param>
        /// <param name="height">新しい高さ</param>
        /// <returns>リサイズされた画像</returns>
        public static Image ResizeImage(Image image, int width, int height)
        {
            return ScaleImageTo(image, new Size(width, height));
        }

        /// <summary>
        /// 画像をリサイズ（アスペクト比を保持）
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="maxWidth">最大幅</param>
        /// <param name="maxHeight">最大高さ</param>
        /// <returns>リサイズされた画像</returns>
        public static Image ResizeImageKeepAspectRatio(Image image, int maxWidth, int maxHeight)
        {
            if (image == null)
            {
                return new Bitmap(maxWidth, maxHeight);
            }

            try
            {
                double ratioX = (double)maxWidth / image.Width;
                double ratioY = (double)maxHeight / image.Height;
                double ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(image.Width * ratio);
                int newHeight = (int)(image.Height * ratio);

                return ScaleImageTo(image, new Size(newWidth, newHeight));
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ResizeImageKeepAspectRatio", "画像リサイズエラー", ex.Message);
                return new Bitmap(maxWidth, maxHeight);
            }
        }

        /// <summary>
        /// 画像をグレースケールに変換
        /// </summary>
        /// <param name="image">元画像</param>
        /// <returns>グレースケール画像</returns>
        public static Image ConvertToGrayscale(Image image)
        {
            if (image == null)
            {
                return new Bitmap(1, 1);
            }

            try
            {
                var grayImage = new Bitmap(image.Width, image.Height);
                using (var graphics = Graphics.FromImage(grayImage))
                {
                    var colorMatrix = new ColorMatrix(
                        new float[][]
                        {
                            new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                            new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                            new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                        });

                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(colorMatrix);

                    graphics.DrawImage(image, 
                        new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height,
                        GraphicsUnit.Pixel, imageAttributes);
                }
                return grayImage;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.ConvertToGrayscale", "グレースケール変換エラー", ex.Message);
                return image;
            }
        }

        /// <summary>
        /// 画像の透明度を調整
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="opacity">透明度（0.0-1.0）</param>
        /// <returns>透明度調整された画像</returns>
        public static Image AdjustOpacity(Image image, float opacity)
        {
            if (image == null)
            {
                return new Bitmap(1, 1);
            }

            try
            {
                var adjustedImage = new Bitmap(image.Width, image.Height);
                using (var graphics = Graphics.FromImage(adjustedImage))
                {
                    var colorMatrix = new ColorMatrix(
                        new float[][]
                        {
                            new float[] { 1, 0, 0, 0, 0 },
                            new float[] { 0, 1, 0, 0, 0 },
                            new float[] { 0, 0, 1, 0, 0 },
                            new float[] { 0, 0, 0, opacity, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                        });

                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(colorMatrix);

                    graphics.DrawImage(image, 
                        new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height,
                        GraphicsUnit.Pixel, imageAttributes);
                }
                return adjustedImage;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.AdjustOpacity", "透明度調整エラー", ex.Message);
                return image;
            }
        }

        /// <summary>
        /// 画像をファイルに保存
        /// </summary>
        /// <param name="image">保存する画像</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="format">画像フォーマット</param>
        public static void SaveImage(Image image, string filePath, ImageFormat format)
        {
            try
            {
                if (image != null)
                {
                    image.Save(filePath, format);
                    Logger.LogInfo("ImageUtilities.SaveImage", "画像保存完了", filePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.SaveImage", "画像保存エラー", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 画像をメモリストリームに保存
        /// </summary>
        /// <param name="image">保存する画像</param>
        /// <param name="format">画像フォーマット</param>
        /// <returns>メモリストリーム</returns>
        public static MemoryStream SaveImageToStream(Image image, ImageFormat format)
        {
            try
            {
                var stream = new MemoryStream();
                if (image != null)
                {
                    image.Save(stream, format);
                    stream.Position = 0;
                }
                return stream;
            }
            catch (Exception ex)
            {
                Logger.LogError("ImageUtilities.SaveImageToStream", "ストリーム保存エラー", ex.Message);
                return new MemoryStream();
            }
        }
    }
}


