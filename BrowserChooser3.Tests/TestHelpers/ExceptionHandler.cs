using System;
using System.Threading;
using System.Windows.Forms;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// テスト環境での例外ハンドリングを管理するクラス
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// STAスレッドエラーを無視してアクションを実行
        /// </summary>
        public static void ExecuteIgnoringSTAErrors(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("DragDrop 登録は成功しませんでした"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("STAスレッドエラーを無視しました: " + ex.Message);
            }
            catch (ThreadStateException ex) when (ex.Message.Contains("OLE が呼び出される前に"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("STAスレッドエラーを無視しました: " + ex.Message);
            }
            catch (Exception)
            {
                // その他の例外は再スロー
                throw;
            }
        }

        /// <summary>
        /// フォームを作成します。
        /// </summary>
        /// <remarks>
        /// 以前はSTAスレッドエラー時に <c>null!</c> を返しており、呼び出し側の
        /// <c>if (form != null)</c> ガードによって「フォームの構築に失敗したテスト」が
        /// そのまま成功として通っていた。構築できないことは検証すべき失敗なので、
        /// 例外はそのまま伝播させる。
        /// </remarks>
        /// <typeparam name="T">作成するフォームの型</typeparam>
        /// <returns>作成したフォーム</returns>
        public static T CreateFormSafely<T>() where T : Form, new()
        {
            return new T();
        }

        /// <summary>
        /// フォームのメソッドを安全に実行
        /// </summary>
        public static void ExecuteFormMethodSafely(Form form, Action<Form> method)
        {
            if (form == null) return;

            try
            {
                method(form);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("DragDrop 登録は成功しませんでした"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("フォームメソッド実行時のSTAスレッドエラーを無視しました: " + ex.Message);
            }
            catch (ThreadStateException ex) when (ex.Message.Contains("OLE が呼び出される前に"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("フォームメソッド実行時のSTAスレッドエラーを無視しました: " + ex.Message);
            }
        }

        /// <summary>
        /// フォームを破棄します。
        /// </summary>
        /// <remarks>
        /// 以前は最後に <c>catch (Exception)</c> があり、破棄処理中のあらゆる不具合を
        /// 握り潰していた。STAスレッド由来の既知エラーのみ無視し、それ以外は伝播させる。
        /// </remarks>
        /// <param name="form">破棄するフォーム</param>
        public static void DisposeFormSafely(Form form)
        {
            if (form == null) return;

            try
            {
                if (!form.IsDisposed)
                {
                    form.Dispose();
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("DragDrop 登録は成功しませんでした"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("フォーム破棄時のSTAスレッドエラーを無視しました: " + ex.Message);
            }
            catch (ThreadStateException ex) when (ex.Message.Contains("OLE が呼び出される前に"))
            {
                // STAスレッドエラーを無視
                Console.WriteLine("フォーム破棄時のSTAスレッドエラーを無視しました: " + ex.Message);
            }
        }
    }
}
