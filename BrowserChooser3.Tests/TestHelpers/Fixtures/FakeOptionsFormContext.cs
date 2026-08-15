using System;
using System.Windows.Forms;
using BrowserChooser3.Classes.Interfaces;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// <see cref="IOptionsFormContext"/> のテスト用実装。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Control.ControlCollection"/> はMoqでプロキシできないうえ、
    /// ハンドラー群は <c>Controls.Find(name, true)</c> の結果に対して
    /// Enabled / Visible を設定するという「実際のコントロールへの副作用」を持つ。
    /// そのため本物の <see cref="Form"/> をホストとして内部に持ち、
    /// そこへテスト対象のコントロールを登録できるようにしている。
    /// </para>
    /// <para>
    /// OptionsForm本体を構築せずに済むので、重いUI初期化を経ずにハンドラーの
    /// 振る舞いだけを検証できる。
    /// </para>
    /// </remarks>
    public sealed class FakeOptionsFormContext : IOptionsFormContext, IDisposable
    {
        private readonly Form _host = new();
        private bool _disposed;

        /// <summary>
        /// このコンテキストが公開するコントロールコレクション。
        /// </summary>
        public Control.ControlCollection Controls => _host.Controls;

        /// <summary>
        /// 設定タブのTabControl。
        /// </summary>
        public TabControl TabSettings { get; } = new();

        /// <summary>
        /// 待機カーソルの状態。
        /// </summary>
        public bool UseWaitCursor { get; set; }

        /// <summary>
        /// ダイアログの結果。
        /// </summary>
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// <see cref="Close"/> が呼ばれた回数。
        /// </summary>
        public int CloseCallCount { get; private set; }

        /// <summary>
        /// フォームを閉じる（実際には閉じず、呼び出しを記録するだけ）。
        /// </summary>
        public void Close() => CloseCallCount++;

        /// <summary>
        /// 名前付きのボタンをコントロールコレクションへ追加します。
        /// </summary>
        /// <param name="name">コントロール名</param>
        /// <param name="enabled">初期の有効状態</param>
        /// <returns>追加したボタン</returns>
        public Button AddButton(string name, bool enabled = false)
        {
            var button = new Button { Name = name, Enabled = enabled };
            _host.Controls.Add(button);
            return button;
        }

        /// <summary>
        /// 名前付きのラベルをコントロールコレクションへ追加します。
        /// </summary>
        /// <param name="name">コントロール名</param>
        /// <param name="visible">初期の可視状態</param>
        /// <returns>追加したラベル</returns>
        public Label AddLabel(string name, bool visible = false)
        {
            var label = new Label { Name = name, Visible = visible };
            _host.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// 名前付きのタブページを <see cref="TabSettings"/> へ追加します。
        /// </summary>
        /// <param name="name">タブページ名</param>
        /// <returns>追加したタブページ</returns>
        public TabPage AddTabPage(string name)
        {
            var page = new TabPage { Name = name };
            TabSettings.TabPages.Add(page);
            return page;
        }

        /// <summary>
        /// ホストフォームとタブコントロールを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            TabSettings.Dispose();
            _host.Dispose();
        }
    }
}
