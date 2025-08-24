using BrowserChooser3.Classes;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// アイコン選択フォーム
    /// ブラウザアイコンの選択を管理します
    /// </summary>
    public partial class IconSelectionForm : Form
    {
        private Settings _settings;
        private string _selectedIconPath = string.Empty;

        /// <summary>
        /// IconSelectionFormの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="settings">設定オブジェクト</param>
        public IconSelectionForm(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadIcons();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // メインコントロール
            this.lstIcons = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            
            this.SuspendLayout();
            
            // lstIcons
            this.lstIcons.AccessibleName = "Icons List";
            this.lstIcons.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstIcons.GridLines = true;
            this.lstIcons.Location = new System.Drawing.Point(0, 0);
            this.lstIcons.Name = "lstIcons";
            this.lstIcons.Size = new System.Drawing.Size(581, 276);
            this.lstIcons.TabIndex = 0;
            this.lstIcons.TileSize = new System.Drawing.Size(96, 96);
            this.lstIcons.UseCompatibleStateImageBehavior = false;
            this.lstIcons.View = System.Windows.Forms.View.Tile;
            this.lstIcons.SelectedIndexChanged += new System.EventHandler(this.lstIcons_SelectedIndexChanged);
            this.lstIcons.DoubleClick += new System.EventHandler(this.lstIcons_DoubleClick);
            
            // btnOK
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.btnOK.Location = new System.Drawing.Point(413, 282);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            
            // btnCancel
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.btnCancel.Location = new System.Drawing.Point(503, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            
            // IconSelectionForm
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(581, 317);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstIcons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IconSelectionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose your icon";
            this.TopMost = true;
            
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ListView lstIcons = null!;
        private System.Windows.Forms.Button btnOK = null!;
        private System.Windows.Forms.Button btnCancel = null!;
        private System.ComponentModel.IContainer components = null!;

        /// <summary>
        /// アイコンを読み込み
        /// </summary>
        private void LoadIcons()
        {
            Logger.LogInfo("IconSelectionForm.LoadIcons", "Start");
            
            try
            {
                // ImageListの作成
                var imageList = new ImageList
                {
                    ImageSize = new Size(48, 48),
                    ColorDepth = ColorDepth.Depth32Bit
                };
                
                // リソースからアイコンを読み込み
                var iconResources = new Dictionary<string, Image>
                {
                    { "Icon122", Properties.Resources.Icon122 },
                    { "Icon128", Properties.Resources.Icon128 },
                    { "BrowserChooser", Properties.Resources.BrowserChooserIcon },
                    { "BrowserChooser2", Properties.Resources.BrowserChooser2Icon.ToBitmap() },
                    { "BrowserChooser3", Properties.Resources.BrowserChooser3Icon.ToBitmap() },
                    { "BCLogo", Properties.Resources.BCLogoIcon.ToBitmap() },
                    { "Paste", Properties.Resources.PasteIcon },
                    { "PasteAndClose", Properties.Resources.PasteAndCloseIcon },
                    { "Settings", Properties.Resources.SettingsIcon },
                    { "WorldGo", Properties.Resources.WorldGoIcon }
                };
                
                foreach (var kvp in iconResources)
                {
                    try
                    {
                        imageList.Images.Add(kvp.Key, kvp.Value);
                        
                        var item = new ListViewItem
                        {
                            Text = kvp.Key,
                            ImageKey = kvp.Key,
                            Tag = kvp.Key
                        };
                        
                        lstIcons.Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("IconSelectionForm.LoadIcons", $"アイコン読み込みエラー: {kvp.Key}", ex.Message);
                    }
                }
                
                // ImageListをListViewに設定
                lstIcons.LargeImageList = imageList;
                lstIcons.SmallImageList = imageList;
                
                // デフォルトアイコンを選択
                if (lstIcons.Items.Count > 0)
                {
                    lstIcons.Items[0].Selected = true;
                    _selectedIconPath = lstIcons.Items[0].Tag?.ToString() ?? string.Empty;
                }
                
                Logger.LogInfo("IconSelectionForm.LoadIcons", "End", $"アイコン数: {lstIcons.Items.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogError("IconSelectionForm.LoadIcons", "アイコン読み込みエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// 選択されたアイコンのパスを取得
        /// </summary>
        public string SelectedIconPath => _selectedIconPath;

        /// <summary>
        /// アイコンリストの選択変更イベント
        /// </summary>
        private void lstIcons_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstIcons.SelectedItems.Count > 0)
            {
                _selectedIconPath = lstIcons.SelectedItems[0].Tag?.ToString() ?? string.Empty;
                Logger.LogInfo("IconSelectionForm.lstIcons_SelectedIndexChanged", "アイコン選択", _selectedIconPath);
            }
        }

        /// <summary>
        /// アイコンリストのダブルクリックイベント
        /// </summary>
        private void lstIcons_DoubleClick(object? sender, EventArgs e)
        {
            if (lstIcons.SelectedItems.Count > 0)
            {
                btnOK_Click(sender, e);
            }
        }

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void btnOK_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedIconPath))
            {
                Logger.LogInfo("IconSelectionForm.btnOK_Click", "アイコン選択完了", _selectedIconPath);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("アイコンを選択してください。", "警告", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

