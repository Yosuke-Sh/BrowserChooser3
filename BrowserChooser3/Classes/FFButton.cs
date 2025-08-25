using System.Drawing;

namespace BrowserChooser3.Classes
{
    /// <summary>
    /// Browser Chooser 2互換のカスタムボタンコントロール
    /// 矢印キー対応とフォーカス表示機能を提供します
    /// </summary>
    public class FFButton : Button
    {
        /// <summary>
        /// 矢印キーイベント
        /// </summary>
        public event EventHandler<Keys>? ArrowKeyUp;

        private bool _showFocusBox = true;
        private bool _trapArrowKeys = true;

        /// <summary>
        /// FFButtonクラスの新しいインスタンスを初期化します
        /// </summary>
        public FFButton()
        {
            // イベントハンドラーの設定
            GotFocus += FFButton_GotFocus;
            LostFocus += FFButton_LostFocus;
        }

        /// <summary>
        /// フォーカスキューの表示を制御します
        /// </summary>
        protected override bool ShowFocusCues => false; // カスタムフォーカス表示を使用

        /// <summary>
        /// 入力キーの判定
        /// </summary>
        /// <param name="keyData">キーデータ</param>
        /// <returns>処理する場合はtrue</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                // 矢印キーイベントを発生
                ArrowKeyUp?.Invoke(this, keyData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// フォーカス取得時の処理
        /// </summary>
        private void FFButton_GotFocus(object? sender, EventArgs e)
        {
            if (_showFocusBox)
            {
                FlatAppearance.BorderColor = Color.Black;
                FlatAppearance.BorderSize = 1;
            }
        }

        /// <summary>
        /// フォーカス喪失時の処理
        /// </summary>
        private void FFButton_LostFocus(object? sender, EventArgs e)
        {
            FlatAppearance.BorderSize = 0;
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
    }
}

