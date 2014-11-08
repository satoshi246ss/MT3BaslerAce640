using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        public void avt_cam_end()
        {
            //AVT
            camera.Close();
            sys.Shutdown();
        }

        public void avt_cam_start()
        {
            String str;
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
                str = ve.MapReturnCodeToString();
            }
            this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });

            try
            {
                camera.OnFrameReceived += new AVT.VmbAPINET.Camera.OnFrameReceivedHandler(OnFrameReceived);
                str = "/// +OnFrameReceived\n";
            }
            catch (AVT.VmbAPINET.VimbaException ve)
            {
                str = ve.MapReturnCodeToString();
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
        }

        private void OnFrameReceived(AVT.VmbAPINET.Frame frame)
        {
            String str=null;
            try
            {
                if (InvokeRequired) // if not from this thread invoke it in our context
                {
                    Invoke(new AVT.VmbAPINET.Camera.OnFrameReceivedHandler(OnFrameReceived), frame);
                }
            }
            catch (ObjectDisposedException e)
            {
                Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, e.Message });
            }

            if (AVT.VmbAPINET.VmbFrameStatusType.VmbFrameStatusComplete == frame.ReceiveStatus)
            {
                str = "/// Frame status complete";
              //  Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }

            try
            {
                camera.QueueFrame(frame);

                System.Object lockThis = new System.Object();
                lock (lockThis)
                {
                    //img_dmk は使わず、直接imgdata.imgにコピー
                    System.Runtime.InteropServices.Marshal.Copy(frame.Buffer, 0, imgdata.img.ImageDataOrigin, frame.Buffer.Length);
                }
            }
            catch (AVT.VmbAPINET.VimbaException ve)
            {
                str = ve.MapReturnCodeToString();
                Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }

            // detect();
            imgdata_push_FIFO();
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
        public long StatFrameDelivered()
        {
            feature = features["StatFrameDelivered"];
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
