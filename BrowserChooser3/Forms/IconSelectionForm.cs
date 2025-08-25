using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services;
using BrowserChooser3.Classes.Utilities;
using System.Drawing.Imaging;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// アイコン選択フォーム
    /// 実行ファイルからのアイコン抽出と選択を提供します
    /// </summary>
    public partial class IconSelectionForm : Form
    {
        private List<Icon> _icons = new();
        private Icon? _selectedIcon = null;
        private string _filePath = string.Empty;

        /// <summary>
        /// 選択されたアイコン
        /// </summary>
        public Icon? SelectedIcon => _selectedIcon;

        /// <summary>
        /// アイコン選択フォームクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="filePath">アイコンを抽出するファイルパス</param>
        public IconSelectionForm(string filePath)
        {
            _filePath = filePath;
            InitializeComponent();
            LoadIcons();
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Icon Selection";
            Size = new Size(600, 400);
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

            // OKボタン
            var btnOK = new Button
            {
                Name = "btnOK",
                Text = "OK",
                Location = new Point(420, 320),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK,
                Enabled = false
            };
            btnOK.Click += BtnOK_Click;

            // キャンセルボタン
            var btnCancel = new Button
            {
                Name = "btnCancel",
                Text = "Cancel",
                Location = new Point(505, 320),
                Size = new Size(75, 25),
                DialogResult = DialogResult.Cancel
            };

            // コントロールを追加
            Controls.Add(iconListView);
            Controls.Add(previewPictureBox);
            Controls.Add(filePathLabel);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            // フォームのAcceptButtonとCancelButtonを設定
            AcceptButton = btnOK;
            CancelButton = btnCancel;
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

                // 実行ファイルからアイコンを抽出
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

                Logger.LogInfo("IconSelectionForm.LoadIcons", "アイコン読み込み完了", $"Count: {_icons.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIcons", "アイコン読み込みエラー", ex.Message);
                MessageBox.Show($"アイコンの読み込みに失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (iconListView?.SelectedItems.Count > 0)
                {
                    var selectedItem = iconListView.SelectedItems[0];
                    if (selectedItem.Tag is Icon icon)
                    {
                        _selectedIcon = icon;
                        
                        // プレビューを更新
                        if (previewPictureBox != null)
                        {
                            previewPictureBox.Image = icon.ToBitmap();
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
                    
                    // プレビューをクリア
                    if (previewPictureBox != null)
                    {
                        previewPictureBox.Image = null;
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

