using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Blob;


namespace MT3
{
    partial class Form1 //AviTest
    {
        private BackgroundWorker AviTestworker;
        private CvCapture capture;
        IplImage AviTest_image;
        double AviTest_fps = 4;
        //string fn = @"D:\image_data\20151018\20151018_175329_491_10.avi";
        string AviTest_fn = @"20151018_224542_615_10.avi";

        public void AviTest_init()
        {
            AviTestworker = new BackgroundWorker();
            AviTestworker.WorkerReportsProgress = true;
            AviTestworker.DoWork += new DoWorkEventHandler(AviTestworker_DoWork);
            AviTestworker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

            capture = new CvCapture(AviTest_fn);
            //Cv.NamedWindow("AviTest_window");
            //Cv.ShowImage("window", img);
            //Cv2.ImShow("dst", imgMatches); // for mat
        }

        public void AviTest_run()
        {
            AviTestworker.RunWorkerAsync(); 
        }

        private void AviTestworker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            using (CvCapture capture = new CvCapture(AviTest_fn))
            {
                int interval = (int)(1000 / AviTest_fps);
                while ((AviTest_image = capture.QueryFrame()) != null)
                {
                    bw.ReportProgress(0, AviTest_image);
                    System.Threading.Thread.Sleep(interval);
                }
            }
        }
    }
}
