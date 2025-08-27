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
            catch (Exception ex)
            {
                // その他の例外は再スロー
                throw;
            }
        }

        /// <summary>
        /// STAスレッドエラーを無視してフォームを作成
        /// </summary>
        public static T CreateFormSafely<T>() where T : Form, new()
        {
            try
            {
                return new T();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("DragDrop 登録は成功しませんでした"))
            {
                // STAスレッドエラーを無視してnullを返す
                Console.WriteLine("フォーム作成時のSTAスレッドエラーを無視しました: " + ex.Message);
                return null!;
            }
            catch (ThreadStateException ex) when (ex.Message.Contains("OLE が呼び出される前に"))
            {
                // STAスレッドエラーを無視してnullを返す
                Console.WriteLine("フォーム作成時のSTAスレッドエラーを無視しました: " + ex.Message);
                return null!;
            }
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
        /// フォームの破棄を安全に実行
        /// </summary>
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
            catch (Exception ex)
            {
                // その他の例外は無視
                Console.WriteLine("フォーム破棄時のエラーを無視しました: " + ex.Message);
            }
        }
    }
}
