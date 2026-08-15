using System.Windows.Forms;

namespace BrowserChooser3.Classes.Interfaces
{
    /// <summary>
    /// OptionsFormのハンドラー群が親フォームに対して実際に必要としている操作だけを表すインターフェース。
    /// </summary>
    /// <remarks>
    /// <para>
    /// ハンドラー群は従来、具象クラスである <c>OptionsForm</c> を直接受け取っていた。
    /// Moqは具象のWinFormsクラスをプロキシできないため、
    /// <c>new Mock&lt;OptionsForm&gt;()</c> を使うテストは全てスキップされていた（約218件）。
    /// </para>
    /// <para>
    /// 実際にハンドラーが触っているのは Controls / tabSettings / UseWaitCursor /
    /// DialogResult / Close の5つだけなので、それだけをこのインターフェースに切り出し、
    /// ハンドラーはこちらに依存させる。既存の <c>IMessageBoxService</c> /
    /// <c>IFileDialogService</c> と同じ方針。
    /// </para>
    /// </remarks>
    public interface IOptionsFormContext
    {
        /// <summary>
        /// フォーム上のコントロールコレクション。
        /// ハンドラーは <c>Controls.Find(name, true)</c> でボタンやラベルを探す。
        /// </summary>
        Control.ControlCollection Controls { get; }

        /// <summary>
        /// 設定タブのTabControl。ハンドラーは各タブページ配下のリストを更新する。
        /// </summary>
        TabControl TabSettings { get; }

        /// <summary>
        /// 待機カーソルを表示するかどうか。時間のかかる処理中にtrueにする。
        /// </summary>
        bool UseWaitCursor { get; set; }

        /// <summary>
        /// ダイアログの結果。
        /// </summary>
        DialogResult DialogResult { get; set; }

        /// <summary>
        /// フォームを閉じる。
        /// </summary>
        void Close();
    }
}
