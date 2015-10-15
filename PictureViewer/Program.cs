using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using OpenCvSharp;
using PylonC.NET;

namespace MT3
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// コマンドオプション： 最初 /vi /basler /ids /avt /is
        ///                      次   カメラID { 1,2,3・・・
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string prog_name="0";
            if (args.Length == 2)
            {
                prog_name = "MT3Basler_" + args[1];
            }
            else
            {
                Application.Exit();
            }

            int basler_mode = 0;
            if (args[0].StartsWith("/BA") || args[0].StartsWith("/ba") || args[0].StartsWith("/Ba"))
            {
                basler_mode = 1;
            }

            //
            using (var mutex = new Mutex(false, prog_name))
            {
                try
                {
                    if (mutex.WaitOne(0))
                    {
                        Run(mutex, basler_mode);
                    }
                    else
                    {
                        // 起動済みのウィンドウをアクティブにすると親切かも
                        MessageBox.Show("多重起動はできません  "+prog_name);
                    }
                }
                catch (AbandonedMutexException)
                {
                    // new Mutex()～WaitOne()の間で、既に起動中のプロセスが強制終了した
                    // この場合もMutexの所有権は取得できているので、起動して問題ない
                    Run(mutex, basler_mode);
                }
            }
        }

        static void Run(Mutex mutex, int basler_mode)
        {

            if (basler_mode == 1)
            {
                /// Basler only
                Pylon.Initialize();
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                catch
                {
                    Pylon.Terminate();
                    throw;
                }
                Pylon.Terminate();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
