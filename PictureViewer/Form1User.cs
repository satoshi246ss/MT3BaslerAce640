using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Blob;

using PylonC.NETSupportLibrary;

namespace MT3
{
    public partial class Form1 : Form
    {
        #region 定数
        //状態を表す定数
        const int TRUE = 0;
        const int FALSE = 1;
        //上の2つ状態を保持します
        int ImgSaveFlag = FALSE;

        //カメラの状態を表す定数
        const int STOP = 0;
        const int RUN = 1;
        const int SAVE = 2;
        //上の状態を保持します
        int States = 0;

        //状態を表す定数
        const int LOST = 0;
        const int DETECT = 1;
        const int DETECT_IN = 2;
        const int PID_TEST = 3;
        //上の2つ状態を保持します
        int Mode = LOST;

        // 時刻基準（BCB互換）
        DateTime TBASE = new DateTime(1899, 12, 30, 0, 0, 0);

        // メイン装置光軸座標
        int xoa_mes = 640 / 2; //320  2013/11/23 MT3QHYの中心に変更
        int yoa_mes = 480 / 2; //256;  //240
        double test_start_id = 0;
        double xoa_test_start = 70;
        double yoa_test_start = 70;
        double xoa_test_step = 1;
        double yoa_test_step = 1;
        //int roa_mes = 10;

        #endregion

        #region グローバル変数
        // メイン装置光軸座標
        int xoa;  //320
        int yoa;  //240
        int roa = 10;
        double xoad, yoad;

        const double fl = 12.5, ccdpx = 0.0074, ccdpy = 0.0074;
        public double dx, dy, theta_c = 0, dt = 1.0 / 12.0;
        public double az0, alt0, vaz0, valt0; // 流星位置、速度（前フレームの値）
        public double az, alt, vaz, valt; // 流星位置、速度
        public double az1, alt1, vaz1, valt1; // 流星位置、速度（次フレームの値）
        public double daz, dalt, dvaz, dvalt; // 流星位置差、速度差（前フレームからの）
        position_mesure pos_mes = new position_mesure();

        // 観測開始からのフレーム番号
        int id = 0;
        DateTime LiveStartTime;

        const int MaxFrame = 128;  //512
        //const int WIDTH = 2456; // 2456 max piA2400-12gm
        //const int HEIGHT = 2058; // 2058 max
        const int WIDTH = 640; // 2456 max piA2400-12gm
        const int HEIGHT = 480; // 2058 max

        ImageData imgdata = new ImageData(WIDTH, HEIGHT);
        CircularBuffer fifo = new CircularBuffer(MaxFrame, WIDTH, HEIGHT);

        private ImageProvider m_imageProvider = new ImageProvider(); /* Create one image provider. */
        //private Bitmap m_bitmap = null; /* The bitmap is used for displaying the image. */


        //TIS.Imaging.ImageBuffer CurrentBuffer = null;

        // IDS
        private Int32 u32DisplayID = 0;
        uEye.Camera cam;
        uEye.Defines.Status statusRet = 0;
        uEye.Types.ImageInfo imageInfo;
        Int32 s32MemID;
        int cameraID = 2;  // 1:UI5240   2:UI2410
        Int32 set_pixelclock = 30; // [MHz]
        double set_framerate = 12;// [fps]
        Double set_exposure = 80; // [ms]
        Double set_exposure1 = 0.2; // [ms]
        Int32 set_gain = 1000;//[0-1000]


        IplImage img_dmk3 = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);
        IplImage img_dmk = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 1);
        // IplImage img_dark8 = Cv.LoadImage(@"C:\Users\Public\piccolo\dark00.bmp", LoadMode.GrayScale);
        IplImage img2 = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 1);
        IplImage imgLabel = new IplImage(WIDTH, HEIGHT, CvBlobLib.DepthLabel, 1);
        CvBlobs blobs = new CvBlobs();
        CvFont font = new CvFont(FontFace.HersheyComplex, 0.50, 0.50);

        //CvWindow window1 = new CvWindow("DMK2", WindowMode.AutoSize);
        //int id_fr = 0;
        double gx, gy, max_val, kgx, kgy, kvx, kvy, sgx, sgy;
        int threshold_blob = 128; // 検出閾値（０－２５５）
        double threshold_min_area = 0.25; // 最小エリア閾値（最大値ｘ0.25)
        CvPoint2D64f max_centroid;
        int max_label;
        CvBlob maxBlob;
        CvRect blob_rect;
        CvKalman kalman = Cv.CreateKalman(4, 2);
        int kalman_id = 0;
        // 観測値(kalman)
        CvMat measurement = new CvMat(2, 1, MatrixType.F32C1);
        CvMat correction;
        CvMat prediction;

        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        long elapsed0 = 0, elapsed1 = 0, elapsed2 = 0;
        long elapsed20 = 0, elapsed21 = 0, elapsed22 = 0;
        double lap21=0, lap22, lap0 = 0, lap1 = 0, lap2 = 0, alpha = 0.001;
        string fr_str;
        private BackgroundWorker worker;
        private BackgroundWorker worker_udp;
        Udp_kv udpkv = new Udp_kv();

        FSI_PID_DATA pid_data = new FSI_PID_DATA();
        MT_MONITOR_DATA mtmon_data = new MT_MONITOR_DATA();
        int mmFsiUdpPortMT3Basler = 24428;            // （受信）
        int mmFsiUdpPortMT3BaslerS = 24429;            // （送信）
        int mmFsiUdpPortMTmonitor = 24415;
        string mmFsiCore_i5 = "192.168.1.211";
        int mmFsiUdpPortSpCam = 24410;   // SpCam（受信）
        string mmFsiSC440 = "192.168.1.206";
        System.Net.Sockets.UdpClient udpc3 = null;
        DriveInfo cDrive = new DriveInfo("C");
        long diskspace;
        
        //[DllImport("kernel32.dll")]
        //static extern unsafe void CopyMemory(void* dst, void* src, int size);        
        
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint timeEndPeriod(uint uMilliseconds);
        uint time_period = 1;

        #endregion

        # region AVT
        //AVT
        //GE680CCamera cam = new GE680CCamera(1);
        AVT.VmbAPINET.Vimba sys = new AVT.VmbAPINET.Vimba();
        AVT.VmbAPINET.CameraCollection cameras = null;
        AVT.VmbAPINET.Camera camera = null;
        AVT.VmbAPINET.FeatureCollection features = null;
        AVT.VmbAPINET.Feature feature = null;
        long payloadSize;
        AVT.VmbAPINET.Frame[] frameArray = new AVT.VmbAPINET.Frame[3];

        String avtcam_ip = "192.168.1.150";

        static void CopyMemory(IntPtr dst, IntPtr src, int size)
        {
            byte[] temp = new byte[size];
            Marshal.Copy(src, temp, 0, size);
            Marshal.Copy(temp, 0, dst, size);
        }

        public void avt_cam_end()
        {
            //AVT
            camera.Close();
            sys.Shutdown();
        }

        public void avt_cam_start()
        {
            String str=null;
            //AVT
            sys.Startup();
            //cameras = sys.Cameras;
            //if (cameras.Count == 0) return ;
            //foreach( AVT.VmbAPINET.Camera camera in cameras){
            try
            {
                camera = sys.OpenCameraByID(avtcam_ip, AVT.VmbAPINET.VmbAccessModeType.VmbAccessModeFull);
                str = "/// Camera opened\n";
            }
            catch (AVT.VmbAPINET.VimbaException ve)
            {
                str += ve.MapReturnCodeToString();
            }
            this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });

            try
            {
                camera.OnFrameReceived += new AVT.VmbAPINET.Camera.OnFrameReceivedHandler(OnFrameReceived);
                str = "/// +OnFrameReceived\n";
            }
            catch (AVT.VmbAPINET.VimbaException ve)
            {
                str += ve.MapReturnCodeToString();
            }
            this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });

            features = camera.Features;
            feature = features["PayloadSize"];
            payloadSize = feature.IntValue;

            for (int index = 0; index < frameArray.Length; ++index)
            {
                frameArray[index] = new AVT.VmbAPINET.Frame(payloadSize);
                camera.AnnounceFrame(frameArray[index]);
            }

            camera.StartCapture();

            for (int index = 0; index < frameArray.Length; ++index)
            {
                camera.QueueFrame(frameArray[index]);
            }

            feature = features["AcquisitionMode"];
            feature.EnumValue = "Continuous";

            feature = features["AcquisitionStart"];
            feature.RunCommand();

            sw2.Start();
        }

        private void OnFrameReceived(AVT.VmbAPINET.Frame frame)
        {
 /*           try
            {
                if (InvokeRequired) // if not from this thread invoke it in our context
                {
                    Invoke(new AVT.VmbAPINET.Camera.OnFrameReceivedHandler(OnFrameReceived), frame);
                    return;
                }
            }
            catch (ObjectDisposedException e)
            {
                Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, e.Message });
            }
*/
            // 処理速度
            double sf = (double)Stopwatch.Frequency / 1000; //msec
            lap21 = (1 - alpha) * lap21 + alpha * (sw2.ElapsedTicks - elapsed20) / sf;
            elapsed20 = sw2.ElapsedTicks; // 0.1ms
            String str = null;

            if (AVT.VmbAPINET.VmbFrameStatusType.VmbFrameStatusComplete == frame.ReceiveStatus)
            {
                str = "/// Frame status complete";
              //  Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }

            try
            {
                camera.QueueFrame(frame);

                //System.Object lockThis = new System.Object();
                //lock (lockThis)
                lock (frame)
                {
                    //imgdata_push_FIFO(frame.Buffer);
                    
                    //img_dmk は使わず、直接imgdata.imgにコピー (0.3ms)
                    System.Runtime.InteropServices.Marshal.Copy(frame.Buffer, 0, imgdata.img.ImageDataOrigin, frame.Buffer.Length);

                    // unsafeバージョン(0.2-0.3ms)
                  //  unsafe
                  //  {
                  //      fixed (byte* pbytes = frame.Buffer)
                  //      {
                  //          CopyMemory(imgdata.img.ImageDataPtr, pbytes, frame.Buffer.Length);
                  //      }
                  //  }
                }
            }
            catch (AVT.VmbAPINET.VimbaException ve)
            {
                str = ve.MapReturnCodeToString();
                Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }
            elapsed21 = sw2.ElapsedTicks; // 0.1ms
            
            //detect();
            id++;
            imgdata_push_FIFO();
            elapsed22 = sw2.ElapsedTicks; // 0.1ms
            // 処理速度
            //double sf = (double)Stopwatch.Frequency / 1000; //msec
            lap22 = (1 - alpha) * lap22 + alpha * (elapsed22 - elapsed20) / sf;
        }

        /// <summary>
        /// FIFO pushルーチン
        /// </summary>
        private void imgdata_push_FIFO(byte [] buf)
        {
            // 文字入れ
            //String str = String.Format("ID:{0,6:D1} ", imgdata.id) + imgdata.t.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,6:F1},{1,6:F1})({2,6:F1})", gx, gy, max_val);
            //img_dmk.PutText(str, new CvPoint(10, 460), font, new CvColor(255, 100, 100));

            //try
            //{
            imgdata.id = (int)id;     // (int)imageInfo.FrameNumber;
            imgdata.t = DateTime.Now; //imageInfo.TimestampSystem;   //  LiveStartTime.AddSeconds(CurrentBuffer.SampleEndTime);
            imgdata.ImgSaveFlag = !(ImgSaveFlag != 0); //int->bool変換
            imgdata.gx = gx;
            imgdata.gy = gy;
            imgdata.kgx = kgx;
            imgdata.kgy = kgy;
            imgdata.kvx = kvx;
            imgdata.kvy = kvy;
            imgdata.vmax = max_val;
            imgdata.blobs = blobs;
            imgdata.udpkv1 = (Udp_kv)udpkv.Clone();
            imgdata.az = az;
            imgdata.alt = alt;
            imgdata.vaz = vaz;
            imgdata.valt = valt;
            if (fifo.Count == MaxFrame - 1) fifo.EraseLast();
            fifo.InsertFirst(imgdata, buf);
            /*}
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }*/
        }

        public double StatFrameRate()
        {
            feature = features["StatFrameRate"];
            return feature.FloatValue;
        }
        public double ExposureTimeAbs()
        {
            feature = features[" ExposureTimeAbs"];
            return feature.FloatValue;
        }
        //DESCRIPTION:
        //Number of frames succesfully delivered by the streaming engine.
        public long StatFrameDelivered()
        {
            feature = features["StatFrameDelivered"];
            return feature.IntValue;
        }
        //DESCRIPTION:
        //Number of frames dropped by the streaming engine due to missing packets.
        public long StatFrameDropped()
        {
            feature = features["StatFrameDropped"];
            return feature.IntValue;
        }
        //DESCRIPTION:
        //Number of frames succesfully delivered by the streaming engine after having had missing packets.
        public long StatFrameRescued()
        {
            feature = features["StatFrameRescued"];
            return feature.IntValue;
        }
        //DESCRIPTION:
        //Number of frames dropped because a following frame was completed before.
        public long StatFrameShoved()
        {
            feature = features["StatFrameShoved"];
            return feature.IntValue;
        }
        //DESCRIPTION:
        //Number of frames missed due to the non-availibity of a user supplied buffer.
        public long StatFrameUnderrun()
        {
            feature = features["StatFrameUnderrun"];
            return feature.IntValue;
        }
        public long GainRaw()
        {
            feature = features[" GainRaw"];
            return feature.IntValue;
        }
 
        public long AcquisitionFrameCount
        {
            get { return AcquisitionFrameCountFeature.IntValue; }
            set { AcquisitionFrameCountFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature AcquisitionFrameCountFeature
        {
            get
            {
                if (m_AcquisitionFrameCountFeature == null)
                    m_AcquisitionFrameCountFeature = camera.Features["AcquisitionFrameCount"];
                return m_AcquisitionFrameCountFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionFrameCountFeature = null;

        #endregion
    }  
}
