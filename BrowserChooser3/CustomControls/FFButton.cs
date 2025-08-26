using System.Drawing;
using System.Drawing.Drawing2D;

namespace BrowserChooser3.CustomControls
{
    /// <summary>
    /// カスタムボタンコントロール（Browser Chooser 2互換）
    /// 矢印キー対応、フォーカス表示、Aero効果などの機能を提供します
    /// </summary>
    public class FFButton : Button
    {
        /// <summary>
        /// 矢印キーイベント
        /// </summary>
        public event EventHandler<Keys>? ArrowKeyUp;

        /// <summary>
        /// フォーカス表示の有効/無効
        /// </summary>
        public bool ShowFocus { get; set; } = true;

        /// <summary>
        /// フォーカスボックスの表示設定
        /// </summary>
        public bool ShowFocusBox { get; set; } = true;

        /// <summary>
        /// 視覚的フォーカス表示の有効/無効
        /// </summary>
        public bool ShowVisualFocus { get; set; } = false;

        /// <summary>
        /// 矢印キーのトラップ設定
        /// </summary>
        public bool TrapArrowKeys { get; set; } = true;

        /// <summary>
        /// フォーカスボックスの色
        /// </summary>
        public Color FocusBoxColor { get; set; } = Color.Blue;

        /// <summary>
        /// フォーカスボックスの線幅
        /// </summary>
        public int FocusBoxLineWidth { get; set; } = 2;

        /// <summary>
        /// Aero効果の有効/無効
        /// </summary>
        public bool UseAero { get; set; } = true;

        /// <summary>
        /// カスタム背景色
        /// </summary>
        public Color CustomBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// グラデーション開始色
        /// </summary>
        public Color GradientStartColor { get; set; } = Color.LightBlue;

        /// <summary>
        /// グラデーション終了色
        /// </summary>
        public Color GradientEndColor { get; set; } = Color.White;

        /// <summary>
        /// ホバー時の色
        /// </summary>
        public Color HoverColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 押下時の色
        /// </summary>
        public Color PressedColor { get; set; } = Color.Gray;

        /// <summary>
        /// マウスがホバーしているかどうか
        /// </summary>
        private bool _isHovered = false;

        /// <summary>
        /// マウスが押下されているかどうか
        /// </summary>
        private bool _isPressed = false;

        /// <summary>
        /// フォーカスが当たっているかどうか
        /// </summary>
        private bool _isFocused = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FFButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            // デフォルト設定
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            
            // イベントハンドラーの設定
            MouseEnter += FFButton_MouseEnter;
            MouseLeave += FFButton_MouseLeave;
            MouseDown += FFButton_MouseDown;
            MouseUp += FFButton_MouseUp;
            GotFocus += FFButton_GotFocus;
            LostFocus += FFButton_LostFocus;
            KeyDown += FFButton_KeyDown;
        }

        /// <summary>
        /// マウス進入イベント
        /// </summary>
        private void FFButton_MouseEnter(object? sender, EventArgs e)
        {
            _isHovered = true;
            Invalidate();
        }

        /// <summary>
        /// マウス離脱イベント
        /// </summary>
        private void FFButton_MouseLeave(object? sender, EventArgs e)
        {
            _isHovered = false;
            Invalidate();
        }

        /// <summary>
        /// マウス押下イベント
        /// </summary>
        private void FFButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }
        }

        /// <summary>
        /// マウス解放イベント
        /// </summary>
        private void FFButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = false;
                Invalidate();
            }
        }

        /// <summary>
        /// フォーカス取得イベント
        /// </summary>
        private void FFButton_GotFocus(object? sender, EventArgs e)
        {
            _isFocused = true;
            Invalidate();
        }

        /// <summary>
        /// フォーカス喪失イベント
        /// </summary>
        private void FFButton_LostFocus(object? sender, EventArgs e)
        {
            _isFocused = false;
            Invalidate();
        }

        /// <summary>
        /// キー押下イベント
        /// </summary>
        private void FFButton_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    PerformClick();
                    e.Handled = true;
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    if (TrapArrowKeys)
                    {
                        // 矢印キーイベントを発生
                        ArrowKeyUp?.Invoke(this, e.KeyCode);
                        e.Handled = true;
                    }
                    else
                    {
                        // 矢印キーでフォーカス移動
                        MoveFocus(e.KeyCode);
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// フォーカス移動
        /// </summary>
        /// <param name="keyCode">押されたキー</param>
        private void MoveFocus(Keys keyCode)
        {
            if (Parent == null) return;

            var controls = Parent.Controls.Cast<Control>().Where(c => c != this && c.CanFocus).ToList();
            if (controls.Count == 0) return;

            var currentIndex = controls.IndexOf(this);
            if (currentIndex == -1) return;

            Control? nextControl = null;

            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Up:
                    // 前のコントロールに移動
                    nextControl = currentIndex > 0 ? controls[currentIndex - 1] : controls[controls.Count - 1];
                    break;
                case Keys.Right:
                case Keys.Down:
                    // 次のコントロールに移動
                    nextControl = currentIndex < controls.Count - 1 ? controls[currentIndex + 1] : controls[0];
                    break;
            }

            nextControl?.Focus();
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="e">描画イベント引数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;

            // 背景の描画
            DrawBackground(graphics, rect);

            // テキストの描画
            DrawText(graphics, rect);

            // フォーカスボックスの描画
            if ((ShowFocus || ShowVisualFocus) && _isFocused)
            {
                DrawFocusBox(graphics, rect);
            }
        }

        /// <summary>
        /// 背景の描画
        /// </summary>
        /// <param name="graphics">グラフィックスオブジェクト</param>
        /// <param name="rect">描画領域</param>
        private void DrawBackground(Graphics graphics, Rectangle rect)
        {
            Color backgroundColor;

            if (_isPressed)
            {
                backgroundColor = PressedColor;
            }
            else if (_isHovered)
            {
                backgroundColor = HoverColor;
            }
            else if (CustomBackColor != Color.Empty)
            {
                backgroundColor = CustomBackColor;
            }
            else
            {
                backgroundColor = BackColor;
            }

            if (UseAero && !_isPressed && !_isHovered)
            {
                // Aero効果のグラデーション背景
                using var brush = new LinearGradientBrush(rect, GradientStartColor, GradientEndColor, LinearGradientMode.Vertical);
                graphics.FillRectangle(brush, rect);
            }
            else
            {
                // 通常の背景
                using var brush = new SolidBrush(backgroundColor);
                graphics.FillRectangle(brush, rect);
            }

            // 境界線の描画
            if (FlatAppearance.BorderSize > 0)
            {
                using var pen = new Pen(FlatAppearance.BorderColor, FlatAppearance.BorderSize);
                graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }

        /// <summary>
        /// テキストの描画
        /// </summary>
        /// <param name="graphics">グラフィックスオブジェクト</param>
        /// <param name="rect">描画領域</param>
        private void DrawText(Graphics graphics, Rectangle rect)
        {
            if (string.IsNullOrEmpty(Text)) return;

            var textColor = _isPressed ? Color.White : ForeColor;
            using var brush = new SolidBrush(textColor);
            using var font = new Font(Font, _isPressed ? FontStyle.Bold : Font.Style);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // アイコンがある場合の処理
            if (Image != null)
            {
                var imageRect = new Rectangle(
                    rect.X + 5,
                    rect.Y + (rect.Height - Image.Height) / 2,
                    Image.Width,
                    Image.Height);

                var textRect = new Rectangle(
                    rect.X + Image.Width + 10,
                    rect.Y,
                    rect.Width - Image.Width - 15,
                    rect.Height);

                graphics.DrawImage(Image, imageRect);
                graphics.DrawString(Text, font, brush, textRect, format);
            }
            else
            {
                graphics.DrawString(Text, font, brush, rect, format);
            }
        }

        /// <summary>
        /// フォーカスボックスの描画
        /// </summary>
        /// <param name="graphics">グラフィックスオブジェクト</param>
        /// <param name="rect">描画領域</param>
        private void DrawFocusBox(Graphics graphics, Rectangle rect)
        {
            using var pen = new Pen(FocusBoxColor, FocusBoxLineWidth)
            {
                DashStyle = DashStyle.Dash
            };

            var focusRect = new Rectangle(
                rect.X + 2,
                rect.Y + 2,
                rect.Width - 4,
                rect.Height - 4);

            graphics.DrawRectangle(pen, focusRect);
        }

        /// <summary>
        /// アクセシビリティ対応
        /// </summary>
        /// <returns>アクセシビリティオブジェクト</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new FFButtonAccessibleObject(this);
        }

        /// <summary>
        /// カスタムアクセシビリティオブジェクト
        /// </summary>
        private class FFButtonAccessibleObject : ControlAccessibleObject
        {
            public FFButtonAccessibleObject(FFButton owner) : base(owner)
            {
            }

            public override string? Name
            {
                get => base.Name ?? Owner?.Text ?? "";
                set => base.Name = value;
            }

            public override string Description => Owner?.Text ?? "";

            public override AccessibleRole Role => AccessibleRole.PushButton;

            public override string DefaultAction => "Press";
        }
    }
}

