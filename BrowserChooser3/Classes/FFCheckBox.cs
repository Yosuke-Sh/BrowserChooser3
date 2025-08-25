using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// Browser Chooser 2互換のカスタムチェックボックスコントロール
    /// Aero効果対応とカスタムフォーカス表示機能を提供します
    /// </summary>
    public class FFCheckBox : CheckBox
    {
        private bool _showFocusBox = true;
        private bool _usesAero = true;
        private bool _trapArrowKeys = true;
        private int _oldWidth;
        private int _oldHeight;
        private bool _hasFocus = false;

        /// <summary>
        /// FFCheckBoxクラスの新しいインスタンスを初期化します
        /// </summary>
        public FFCheckBox()
        {
            // イベントハンドラーの設定
            GotFocus += FFCheckBox_GotFocus;
            LostFocus += FFCheckBox_LostFocus;
            HandleCreated += FFCheckBox_HandleCreated;
            Paint += FFCheckBox_Paint;
        }

        /// <summary>
        /// フォーカスキューの表示を制御します
        /// </summary>
        protected override bool ShowFocusCues
        {
            get
            {
                if (!DesignMode)
                {
                    if (_settings?.ShowFocus == true)
                    {
                        return false; // カスタムフォーカス表示を使用
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 設定への参照
        /// </summary>
        private Settings? _settings => Settings.Current;

        /// <summary>
        /// フォーカス取得時の処理
        /// </summary>
        private void FFCheckBox_GotFocus(object? sender, EventArgs e)
        {
            _hasFocus = true;
        }

        /// <summary>
        /// ハンドル作成時の処理
        /// </summary>
        private void FFCheckBox_HandleCreated(object? sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.MouseUp += Parent_MouseUp;
            }
        }

        /// <summary>
        /// フォーカス喪失時の処理
        /// </summary>
        private void FFCheckBox_LostFocus(object? sender, EventArgs e)
        {
            _hasFocus = false;
            Parent?.Refresh();
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        private void FFCheckBox_Paint(object? sender, PaintEventArgs e)
        {
            if (DesignMode) return; // デザインモードでは何もしない

            if (GeneralUtilities.IsAeroEnabled() && _usesAero && !(_settings?.UseAccessibleRendering ?? false))
            {
                // Aero効果を使用した描画（バッファードグラフィックスでちらつき防止）
                using var g = Parent?.CreateGraphics();
                if (g != null)
                {
                    var rect = new Rectangle(Location.X + 15, Location.Y - 4, _oldWidth, _oldHeight);

                    var context = BufferedGraphicsManager.Current;
                    using var buffered = context.Allocate(g, rect);

                    var path = new GraphicsPath();

                    if (Tag == null || Tag.ToString() == "")
                    {
                        Tag = Text;
                        _oldWidth = Width;
                        _oldHeight = Height;
                        Text = "";
                    }

                    // テキストの描画
                    path.AddString(Tag?.ToString() ?? "", Font.FontFamily, (int)FontStyle.Regular, 16,
                                 new Point(Location.X + 15, Location.Y - 4), StringFormat.GenericDefault);
                    
                    // スムージングモードを設定
                    buffered.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    
                    // テキストを描画
                    buffered.Graphics.FillPath(new SolidBrush(Color.Black), path);
                    buffered.Render();
                }
            }

            // フォーカスボックスの描画
            if (_showFocusBox && _hasFocus)
            {
                using var g = Parent?.CreateGraphics();
                if (g != null)
                {
                    using var pen = new Pen(Brushes.Black, 2);
                    
                    if (GeneralUtilities.IsAeroEnabled() && !(_settings?.UseAccessibleRendering ?? false))
                    {
                        g.DrawRectangle(pen, Location.X - 5, Location.Y - 5, _oldWidth + 10, _oldHeight);
                    }
                    else
                    {
                        g.DrawRectangle(pen, Location.X - 5, Location.Y - 5, Width + 10, Height + 10);
                    }

                    BringToFront();
                }
            }
        }

        /// <summary>
        /// 親フォームのマウスアップイベント
        /// </summary>
        private void Parent_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_usesAero && !(_settings?.UseAccessibleRendering ?? false))
            {
                // チェックボックス領域のヒットテスト
                if (e.Location.X > Location.X + Width && e.Location.X < Location.X + _oldWidth)
                {
                    if (e.Location.Y > Location.Y && e.Location.Y < Location.Y + _oldHeight)
                    {
                        Focus();
                        Checked = !Checked;
                    }
                }
            }
        }

        /// <summary>
        /// 矢印キーのトラップ設定
        /// </summary>
        public bool TrapArrowKeys
        {
            get => _trapArrowKeys;
            set => _trapArrowKeys = value;
        }

        /// <summary>
        /// フォーカスボックスの表示設定
        /// </summary>
        public bool ShowFocusBox
        {
            get => _showFocusBox;
            set => _showFocusBox = value;
        }

        /// <summary>
        /// Aero効果の使用設定
        /// </summary>
        public bool UsesAero
        {
            get => _usesAero;
            set => _usesAero = value;
        }
    }
}
