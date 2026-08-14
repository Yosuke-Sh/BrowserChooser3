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

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[]? phiconLarge, IntPtr[]? phiconSmall, int nIcons);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// HICONハンドルを解放する。Icon.FromHandleが所有権を持たない
        /// GetHicon()/ExtractIcon系のハンドルを解放するために、呼び出し元から利用できるよう公開する。
        /// </summary>
        public static void DestroyIconHandle(IntPtr hIcon)
        {
            if (hIcon != IntPtr.Zero)
            {
                DestroyIcon(hIcon);
            }
        }

        /// <summary>
        /// キャッシュ1つあたりの保持上限。この数を超えたら最も長く使われていないエントリをDisposeして退避する。
        /// ブラウザ台数は現実的に数十以内のため、上限に達することは稀だが無制限な増加は避ける。
        /// </summary>
        private const int MaxCachedImages = 256;

        /// <summary>
        /// 抽出済みアイコンのキャッシュ（キー: ファイルパス+アイコンインデックス）
        /// 同じ実行ファイルへの再アクセス・Win32でのアイコン再抽出を避けるため、プロセス生存期間中保持する。
        /// 上限を超えたエントリはLRUで退避しDisposeする。
        /// </summary>
        private static readonly LruImageCache _iconCache = new(MaxCachedImages);

        /// <summary>
        /// 単純なLRU画像キャッシュ。上限を超えたら最も長く未使用のエントリをDisposeして削除する。
        /// </summary>
        private sealed class LruImageCache
        {
            private readonly int _capacity;
            private readonly Dictionary<string, LinkedListNode<(string Key, Image Image)>> _map = new();
            private readonly LinkedList<(string Key, Image Image)> _order = new();

            public LruImageCache(int capacity)
            {
                _capacity = capacity;
            }

            public bool TryGetValue(string key, out Image image)
            {
                lock (_map)
                {
                    if (_map.TryGetValue(key, out var node))
                    {
                        _order.Remove(node);
                        _order.AddFirst(node);
                        image = node.Value.Image;
                        return true;
                    }
                }

                image = null!;
                return false;
            }

            public void Set(string key, Image image)
            {
                lock (_map)
                {
                    if (_map.TryGetValue(key, out var existingNode))
                    {
                        _order.Remove(existingNode);
                        if (!ReferenceEquals(existingNode.Value.Image, image))
                        {
                            existingNode.Value.Image.Dispose();
                        }
                    }

                    var node = new LinkedListNode<(string, Image)>((key, image));
                    _map[key] = node;
                    _order.AddFirst(node);

                    while (_map.Count > _capacity && _order.Last != null)
                    {
                        var lru = _order.Last;
                        _order.RemoveLast();
                        _map.Remove(lru.Value.Key);
                        lru.Value.Image.Dispose();
                    }
                }
            }
        }

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

                var cacheKey = $"{filePath}|{browser.IconIndex}";
                if (_iconCache.TryGetValue(cacheKey, out var cachedImage))
                {
                    return cachedImage;
                }

                // アイコンを抽出
                var icon = ExtractIconFromFile(filePath, browser.IconIndex);
                if (icon != null)
                {
                    Logger.LogInfo("ImageUtilities.GetImage", "アイコン抽出成功", browser.Name, filePath);
                    var bitmap = icon.ToBitmap();
                    CacheIcon(cacheKey, bitmap);
                    return bitmap;
                }

                // フォールバック: 関連付けられたアイコンを取得
                try
                {
                    var associatedIcon = Icon.ExtractAssociatedIcon(filePath);
                    if (associatedIcon != null)
                    {
                        var bitmap = associatedIcon.ToBitmap();
                        CacheIcon(cacheKey, bitmap);
                        return bitmap;
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

        private static void CacheIcon(string cacheKey, Image bitmap)
        {
            _iconCache.Set(cacheKey, bitmap);
        }

        /// <summary>
        /// リサイズ済みアイコンのキャッシュ（キー: ファイルパス+アイコンインデックス+サイズ）
        /// ボタン表示のたびに同じサイズへのリサイズ（GDI+描画）を繰り返さないよう、プロセス生存期間中保持する。
        /// 上限を超えたエントリはLRUで退避しDisposeする。
        /// </summary>
        private static readonly LruImageCache _resizedIconCache = new(MaxCachedImages);

        /// <summary>
        /// ブラウザのアイコンを指定サイズにリサイズした画像を取得します。
        /// 元アイコン（<see cref="GetImage"/>）・リサイズ結果ともにキャッシュされ、
        /// 同一ブラウザ・同一サイズへの再要求はキャッシュから返します。
        /// </summary>
        /// <param name="browser">ブラウザオブジェクト</param>
        /// <param name="useCustomPath">カスタムパスを使用するかどうか</param>
        /// <param name="size">リサイズ後のサイズ（正方形）</param>
        /// <returns>リサイズ済み画像</returns>
        public static Image? GetResizedImage(Browser browser, bool useCustomPath, int size)
        {
            if (size <= 0)
            {
                return GetImage(browser, useCustomPath);
            }

            string filePath = useCustomPath && !string.IsNullOrEmpty(browser.ImagePath)
                ? browser.ImagePath
                : browser.Target;

            var resizedCacheKey = $"{filePath}|{browser.IconIndex}|{size}";
            if (_resizedIconCache.TryGetValue(resizedCacheKey, out var cachedResized))
            {
                return cachedResized;
            }

            // プロセスメモリ上のキャッシュにない場合は、ディスクキャッシュを確認する
            // （新規プロセス起動のたびにアイコン抽出・GDI+リサイズをやり直すコストを避けるため）
            var diskCached = TryLoadFromDiskCache(filePath, browser.IconIndex, size);
            if (diskCached != null)
            {
                _resizedIconCache.Set(resizedCacheKey, diskCached);
                return diskCached;
            }

            var sourceImage = GetImage(browser, useCustomPath);
            if (sourceImage == null)
            {
                return null;
            }

            var resized = new Bitmap(sourceImage, new Size(size, size));
            _resizedIconCache.Set(resizedCacheKey, resized);

            SaveToDiskCache(filePath, browser.IconIndex, size, resized);

            return resized;
        }

        /// <summary>
        /// ディスクアイコンキャッシュのキーを、exeパス＋最終更新日時＋要求サイズから求める。
        /// 元ファイルが更新された場合は最終更新日時が変わるため、自動的にキャッシュミスとなり再生成される。
        /// </summary>
        private static string? BuildDiskCacheFileName(string filePath, int iconIndex, int size)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return null;
                }

                var lastWriteTicks = File.GetLastWriteTimeUtc(filePath).Ticks;
                var rawKey = $"{filePath}|{iconIndex}|{size}|{lastWriteTicks}";

                using var sha = System.Security.Cryptography.SHA256.Create();
                var hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawKey));
                var hash = Convert.ToHexString(hashBytes);

                return $"{hash}.png";
            }
            catch (Exception ex)
            {
                Logger.LogWarning("ImageUtilities.BuildDiskCacheFileName", "キャッシュキー生成エラー", ex.Message);
                return null;
            }
        }

        private static Image? TryLoadFromDiskCache(string filePath, int iconIndex, int size)
        {
            try
            {
                var cacheFileName = BuildDiskCacheFileName(filePath, iconIndex, size);
                if (cacheFileName == null)
                {
                    return null;
                }

                var cachePath = Path.Combine(PathManager.GetIconCacheDirectory(), cacheFileName);
                if (!File.Exists(cachePath))
                {
                    return null;
                }

                // ファイルロックを避けるため一旦メモリへ読み込んでからBitmap化する
                var bytes = File.ReadAllBytes(cachePath);
                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("ImageUtilities.TryLoadFromDiskCache", "ディスクキャッシュ読み込みエラー", ex.Message);
                return null;
            }
        }

        private static void SaveToDiskCache(string filePath, int iconIndex, int size, Image resized)
        {
            try
            {
                var cacheFileName = BuildDiskCacheFileName(filePath, iconIndex, size);
                if (cacheFileName == null)
                {
                    return;
                }

                var cacheDir = PathManager.GetIconCacheDirectory();
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }

                var cachePath = Path.Combine(cacheDir, cacheFileName);
                resized.Save(cachePath, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                // キャッシュ書き込みに失敗しても表示自体には影響しないため続行する
                Logger.LogWarning("ImageUtilities.SaveToDiskCache", "ディスクキャッシュ書き込みエラー", ex.Message);
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
            // ExtractIconEx の「大アイコン」（環境依存だが通常32x32）を優先して取得する。
            // ExtractIcon固定サイズ版より高DPI環境での拡大ボケが少ない。
            var largeIcon = ExtractIconExLarge(filePath, iconIndex);
            if (largeIcon != null)
            {
                return largeIcon;
            }

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
        /// ExtractIconExで「大アイコン」ハンドルを取得しIconへ変換する。
        /// 取得したハンドルは確実にDestroyIconで解放する。
        /// </summary>
        private static Icon? ExtractIconExLarge(string filePath, int iconIndex)
        {
            var largeIcons = new IntPtr[1];
            try
            {
                var extracted = ExtractIconEx(filePath, iconIndex, largeIcons, null, 1);
                if (extracted <= 0 || largeIcons[0] == IntPtr.Zero)
                {
                    return null;
                }

                using var icon = Icon.FromHandle(largeIcons[0]);
                return (Icon)icon.Clone();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("ImageUtilities.ExtractIconExLarge", "ExtractIconEx抽出エラー", ex.Message);
                return null;
            }
            finally
            {
                if (largeIcons[0] != IntPtr.Zero)
                {
                    DestroyIcon(largeIcons[0]);
                }
            }
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


