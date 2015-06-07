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
            if (args.Length == 0 || args[0].StartsWith("/BA") || args[0].StartsWith("/ba") || args[0].StartsWith("/Ba"))
            {
#if DEBUG
            /* This is a special debug setting needed only for GigE cameras.
                See 'Building Applications with pylon' in the Programmer's Guide. */
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "300000" /*ms*/);
#endif

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
