using System;
using System.Threading;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Windows Formsテスト用のSTAスレッド属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class STAThreadAttribute : Attribute
    {
        public STAThreadAttribute()
        {
            // STAスレッドでテストを実行するための設定
        }
    }

    /// <summary>
    /// STAスレッドでテストを実行するためのヘルパークラス
    /// </summary>
    public static class STAThreadHelper
    {
        /// <summary>
        /// STAスレッドでアクションを実行
        /// </summary>
        public static void RunInSTAThread(Action action)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                // 既にSTAスレッドの場合は直接実行
                action();
            }
            else
            {
                // STAスレッドで実行
                var thread = new Thread(() => action());
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
        }

        /// <summary>
        /// STAスレッドで関数を実行
        /// </summary>
        public static T RunInSTAThread<T>(Func<T> func)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                // 既にSTAスレッドの場合は直接実行
                return func();
            }
            else
            {
                // STAスレッドで実行
                T result = default(T)!;
                var thread = new Thread(() => result = func());
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                return result;
            }
        }
    }
}
