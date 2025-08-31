using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Services.UI;
using BrowserChooser3.Classes.Utilities;
using System.Drawing.Imaging;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// アイコン選択フォーム
    /// 実行ファイルからのアイコン抽出と直接イメージファイル選択を提供します
    /// </summary>
    public partial class IconSelectionForm : Form
    {
        private List<Icon> _icons = new();
        private Icon? _selectedIcon = null;
        private string _filePath = string.Empty;
        private int _selectedIconIndex = -1;
        private bool _isDirectImageFile = false;

        /// <summary>
        /// 選択されたアイコン
        /// </summary>
        public Icon? SelectedIcon => _selectedIcon;

        /// <summary>
        /// 選択されたアイコンのインデックス
        /// </summary>
        public int SelectedIconIndex => _selectedIconIndex;

        /// <summary>
        /// 選択されたアイコンファイルのパス
        /// </summary>
        public string SelectedIconPath => _filePath;

        /// <summary>
        /// アイコン選択フォームクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="filePath">アイコンを抽出するファイルパス</param>
        public IconSelectionForm(string filePath)
        {
            _filePath = filePath;
            InitializeComponent();
            LoadIcons();
            
            // フォームを最前面に表示
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Icon Selection";
            Size = new Size(700, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;

            // アイコンリストビュー
            var iconListView = new ListView
            {
                Name = "iconListView",
                Location = new Point(10, 10),
                Size = new Size(400, 300),
                View = View.LargeIcon,
                MultiSelect = false,
                FullRowSelect = true
            };
            iconListView.SelectedIndexChanged += IconListView_SelectedIndexChanged;
            iconListView.DoubleClick += IconListView_DoubleClick;

            // アイコンImageList
            var iconImageList = new ImageList
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };
            iconListView.LargeImageList = iconImageList;

            // プレビューピクチャボックス
            var previewPictureBox = new PictureBox
            {
                Name = "previewPictureBox",
                Location = new Point(420, 10),
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            // ファイルパスラベル
            var filePathLabel = new Label
            {
                Name = "filePathLabel",
                Location = new Point(10, 320),
                Size = new Size(560, 20),
                Text = $"File: {_filePath}"
            };

            // アイコンパス変更ボタン
            var btnChangePath = new Button
            {
                Name = "btnChangePath",
                Text = "Change Icon Path",
                Location = new Point(10, 350),
                Size = new Size(120, 30)
            };
            btnChangePath.Click += BtnChangePath_Click;

            // アイコンインデックスラベル
            var iconIndexLabel = new Label
            {
                Name = "iconIndexLabel",
                Location = new Point(140, 355),
                Size = new Size(200, 20),
                Text = "Icon Index: -"
            };

            // ファイルタイプラベル
            var fileTypeLabel = new Label
            {
                Name = "fileTypeLabel",
                Location = new Point(350, 355),
                Size = new Size(200, 20),
                Text = "Type: -"
            };

            // OKボタン
            var btnOK = new Button
            {
                Name = "btnOK",
                Text = "OK",
                Location = new Point(420, 450),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK,
                Enabled = false
            };
            btnOK.Click += BtnOK_Click;

            // キャンセルボタン
            var btnCancel = new Button
            {
                Name = "btnCancel",
                Text = "Cancel",
                Location = new Point(505, 450),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            // コントロールを追加
            Controls.Add(iconListView);
            Controls.Add(previewPictureBox);
            Controls.Add(filePathLabel);
            Controls.Add(btnChangePath);
            Controls.Add(iconIndexLabel);
            Controls.Add(fileTypeLabel);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            // フォームのAcceptButtonとCancelButtonを設定
            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        /// <summary>
        /// アイコンパス変更ボタンのクリックイベント
        /// </summary>
        private void BtnChangePath_Click(object? sender, EventArgs e)
        {
            try
            {
                using var openFileDialog = new OpenFileDialog
                {
                    Filter = "実行ファイル (*.exe)|*.exe|アイコンファイル (*.ico)|*.ico|画像ファイル (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|すべてのファイル (*.*)|*.*",
                    Title = "アイコンファイルを選択"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = openFileDialog.FileName;
                    
                    // ファイルパスラベルを更新
                    var filePathLabel = Controls.Find("filePathLabel", true).FirstOrDefault() as Label;
                    if (filePathLabel != null)
                    {
                        filePathLabel.Text = $"File: {_filePath}";
                    }

                    // アイコンを再読み込み
                    ClearIcons();
                    LoadIcons();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.BtnChangePath_Click", "アイコンパス変更エラー", ex.Message);
                                MessageBoxService.ShowErrorStatic($"アイコンパスの変更に失敗しました: {ex.Message}", "エラー");
            }
        }

        /// <summary>
        /// アイコンをクリアします
        /// </summary>
        private void ClearIcons()
        {
            try
            {
                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList != null)
                {
                    // ImageListをクリア
                    iconListView.LargeImageList.Images.Clear();
                    iconListView.Items.Clear();
                }

                // アイコンリストをクリア
                _icons.Clear();
                _selectedIcon = null;
                _selectedIconIndex = -1;
                _isDirectImageFile = false;

                // プレビューをクリア
                var previewPictureBox = Controls.Find("previewPictureBox", true).FirstOrDefault() as PictureBox;
                if (previewPictureBox != null)
                {
                    previewPictureBox.Image = null;
                }

                // ラベルをクリア
                var iconIndexLabel = Controls.Find("iconIndexLabel", true).FirstOrDefault() as Label;
                if (iconIndexLabel != null)
                {
                    iconIndexLabel.Text = "Icon Index: -";
                }

                var fileTypeLabel = Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;
                if (fileTypeLabel != null)
                {
                    fileTypeLabel.Text = "Type: -";
                }

                // OKボタンを無効化
                var btnOK = Controls.Find("btnOK", true).FirstOrDefault() as Button;
                if (btnOK != null)
                {
                    btnOK.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.ClearIcons", "アイコンクリアエラー", ex.Message);
            }
        }

        /// <summary>
        /// アイコンを読み込みます
        /// </summary>
        private void LoadIcons()
        {
            try
            {
                Logger.LogInfo("IconSelectionForm.LoadIcons", "アイコン読み込み開始", _filePath);

                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList == null) return;

                // ファイル拡張子をチェック
                var extension = Path.GetExtension(_filePath).ToLowerInvariant();
                
                if (extension == ".exe")
                {
                    // 実行ファイルからアイコンを抽出
                    LoadIconsFromExecutable();
                }
                else if (extension == ".ico")
                {
                    // ICOファイルからアイコンを読み込み
                    LoadIconFromIcoFile();
                }
                else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp")
                {
                    // 画像ファイルからアイコンを作成
                    LoadIconFromImageFile();
                }
                else
                {
                    // その他のファイルは関連付けられたアイコンを取得
                    LoadAssociatedIcon();
                }

                Logger.LogInfo("IconSelectionForm.LoadIcons", "アイコン読み込み完了", $"Count: {_icons.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIcons", "アイコン読み込みエラー", ex.Message);
                MessageBoxService.ShowErrorStatic($"アイコンの読み込みに失敗しました: {ex.Message}", "エラー");
            }
        }

        /// <summary>
        /// 実行ファイルからアイコンを読み込みます
        /// </summary>
        private void LoadIconsFromExecutable()
        {
            try
            {
                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList == null) return;

                var icons = ExtractIconsFromFile(_filePath);
                
                foreach (var icon in icons)
                {
                    _icons.Add(icon);
                    
                    // ImageListにアイコンを追加
                    var bitmap = icon.ToBitmap();
                    iconListView.LargeImageList.Images.Add(bitmap);
                    
                    // ListViewItemを追加
                    var item = iconListView.Items.Add($"Icon {iconListView.Items.Count + 1}");
                    item.ImageIndex = iconListView.LargeImageList.Images.Count - 1;
                    item.Tag = icon;
                }

                // ファイルタイプラベルを更新
                var fileTypeLabel = Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;
                if (fileTypeLabel != null)
                {
                    fileTypeLabel.Text = $"Type: Executable ({_icons.Count} icons)";
                }

                _isDirectImageFile = false;
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIconsFromExecutable", "実行ファイルアイコン読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// ICOファイルからアイコンを読み込みます
        /// </summary>
        private void LoadIconFromIcoFile()
        {
            try
            {
                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList == null) return;

                // ICOファイルからアイコンを読み込み
                var icon = new Icon(_filePath);
                _icons.Add(icon);
                
                // ImageListにアイコンを追加
                var bitmap = icon.ToBitmap();
                iconListView.LargeImageList.Images.Add(bitmap);
                
                // ListViewItemを追加
                var item = iconListView.Items.Add("Icon");
                item.ImageIndex = iconListView.LargeImageList.Images.Count - 1;
                item.Tag = icon;

                // ファイルタイプラベルを更新
                var fileTypeLabel = Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;
                if (fileTypeLabel != null)
                {
                    fileTypeLabel.Text = "Type: ICO File";
                }

                _isDirectImageFile = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIconFromIcoFile", "ICOファイル読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// 画像ファイルからアイコンを作成します
        /// </summary>
        private void LoadIconFromImageFile()
        {
            try
            {
                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList == null) return;

                // 画像ファイルからBitmapを読み込み
                using var originalBitmap = new Bitmap(_filePath);
                
                // 32x32のアイコンサイズにリサイズ
                var resizedBitmap = new Bitmap(originalBitmap, new Size(32, 32));
                
                // Bitmapからアイコンを作成
                var icon = Icon.FromHandle(resizedBitmap.GetHicon());
                _icons.Add(icon);
                
                // ImageListにアイコンを追加
                iconListView.LargeImageList.Images.Add(resizedBitmap);
                
                // ListViewItemを追加
                var item = iconListView.Items.Add("Image");
                item.ImageIndex = iconListView.LargeImageList.Images.Count - 1;
                item.Tag = icon;

                // ファイルタイプラベルを更新
                var fileTypeLabel = Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;
                if (fileTypeLabel != null)
                {
                    fileTypeLabel.Text = "Type: Image File";
                }

                _isDirectImageFile = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIconFromImageFile", "画像ファイル読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// 関連付けられたアイコンを読み込みます
        /// </summary>
        private void LoadAssociatedIcon()
        {
            try
            {
                var iconListView = Controls.Find("iconListView", true).FirstOrDefault() as ListView;
                if (iconListView?.LargeImageList == null) return;

                // 関連付けられたアイコンを取得
                var icon = Icon.ExtractAssociatedIcon(_filePath);
                if (icon != null)
                {
                    _icons.Add(icon);
                    
                    // ImageListにアイコンを追加
                    var bitmap = icon.ToBitmap();
                    iconListView.LargeImageList.Images.Add(bitmap);
                    
                    // ListViewItemを追加
                    var item = iconListView.Items.Add("Associated Icon");
                    item.ImageIndex = iconListView.LargeImageList.Images.Count - 1;
                    item.Tag = icon;
                }

                // ファイルタイプラベルを更新
                var fileTypeLabel = Controls.Find("fileTypeLabel", true).FirstOrDefault() as Label;
                if (fileTypeLabel != null)
                {
                    fileTypeLabel.Text = "Type: Associated Icon";
                }

                _isDirectImageFile = false;
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadAssociatedIcon", "関連アイコン読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// ファイルからアイコンを抽出します
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>抽出されたアイコンのリスト</returns>
        private List<Icon> ExtractIconsFromFile(string filePath)
        {
            var icons = new List<Icon>();

            try
            {
                // 実行ファイルからアイコンを抽出
                var iconCount = ExtractIconEx(filePath, -1, (IntPtr[]?)null!, (IntPtr[]?)null!, 0);
                
                if (iconCount > 0)
                {
                    var largeIcons = new IntPtr[iconCount];
                    var smallIcons = new IntPtr[iconCount];
                    
                    ExtractIconEx(filePath, 0, largeIcons, smallIcons, iconCount);
                    
                    for (int i = 0; i < iconCount; i++)
                    {
                        if (largeIcons[i] != IntPtr.Zero)
                        {
                            var icon = Icon.FromHandle(largeIcons[i]);
                            icons.Add(icon);
                        }
                    }
                }
                else
                {
                    // フォールバック: 関連付けられたアイコンを取得
                    var icon = Icon.ExtractAssociatedIcon(filePath);
                    if (icon != null)
                    {
                        icons.Add(icon);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.ExtractIconsFromFile", "アイコン抽出エラー", ex.Message);
                
                // フォールバック: 関連付けられたアイコンを取得
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(filePath);
                    if (icon != null)
                    {
                        icons.Add(icon);
                    }
                }
                catch (Exception fallbackEx)
                {
                    Logger.LogError("IconSelectionForm.ExtractIconsFromFile", "フォールバックアイコン抽出エラー", fallbackEx.Message);
                }
            }

            return icons;
        }

        /// <summary>
        /// アイコンリストビューの選択変更イベント
        /// </summary>
        private void IconListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                var iconListView = sender as ListView;
                var previewPictureBox = Controls.Find("previewPictureBox", true).FirstOrDefault() as PictureBox;
                var btnOK = Controls.Find("btnOK", true).FirstOrDefault() as Button;
                var iconIndexLabel = Controls.Find("iconIndexLabel", true).FirstOrDefault() as Label;

                if (iconListView?.SelectedItems.Count > 0)
                {
                    var selectedItem = iconListView.SelectedItems[0];
                    if (selectedItem.Tag is Icon icon)
                    {
                        _selectedIcon = icon;
                        _selectedIconIndex = selectedItem.Index;
                        
                        // プレビューを更新
                        if (previewPictureBox != null)
                        {
                            previewPictureBox.Image = icon.ToBitmap();
                        }
                        
                        // アイコンインデックスラベルを更新
                        if (iconIndexLabel != null)
                        {
                            if (_isDirectImageFile)
                            {
                                iconIndexLabel.Text = "Icon Index: Direct File";
                            }
                            else
                            {
                                iconIndexLabel.Text = $"Icon Index: {_selectedIconIndex}";
                            }
                        }
                        
                        // OKボタンを有効化
                        if (btnOK != null)
                        {
                            btnOK.Enabled = true;
                        }
                    }
                }
                else
                {
                    _selectedIcon = null;
                    _selectedIconIndex = -1;
                    
                    // プレビューをクリア
                    if (previewPictureBox != null)
                    {
                        previewPictureBox.Image = null;
                    }
                    
                    // アイコンインデックスラベルをクリア
                    if (iconIndexLabel != null)
                    {
                        iconIndexLabel.Text = "Icon Index: -";
                    }
                    
                    // OKボタンを無効化
                    if (btnOK != null)
                    {
                        btnOK.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.IconListView_SelectedIndexChanged", "選択変更処理エラー", ex.Message);
            }
        }

        /// <summary>
        /// アイコンリストビューのダブルクリックイベント
        /// </summary>
        private void IconListView_DoubleClick(object? sender, EventArgs e)
        {
            if (_selectedIcon != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void BtnOK_Click(object? sender, EventArgs e)
        {
            if (_selectedIcon != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #region Win32 API
        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int ExtractIconEx(string szFileName, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
        #endregion
    }
}

