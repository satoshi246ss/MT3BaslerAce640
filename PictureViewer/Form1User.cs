using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Blob;
using PylonC.NETSupportLibrary;
using uEye;
using MtLibrary;

namespace MT3
{
    #region 定義
    public enum Camera_Maker
    {
        analog,
        ImagingSouce,
        IDS,
        AVT,
        Basler,
        PointGreyCamera,
        TEST     // avi fileによるテスト
    }
    public enum Camera_Color
    {
        mono,
        color,
        mono12packed,
        mono16
    }
    public enum Camera_Interface
    {
        USB2,
        USB3,
        GIGE,
        IEEE1394,
        NTSC,
        HDMI
    }
    public enum Platform
    {
        Fish1,
        Fish2,
        Wide,
        MT1,
        MT2,
        MT3,
    }
    public enum uEye_Shutter_Mode
    {
        Global,
        Rolling
    }

    public class StarAzAlt
    {
        // propaty
        public int ID { get; set; }
        public double Az { get; set; }
        public double Alt { get; set; }
        public double Vmag { get; set; }
        public string Name { get; set; }
    } 

    #endregion

    public partial class Form1 : Form
    {
        #region 定数
        
        Camera_Maker cam_maker = Camera_Maker.Basler ;
        Camera_Color cam_color = Camera_Color.mono;

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
        TimeSpan starttime, endtime;

        // メイン装置光軸座標
        int xoa_mes = 640 / 2; //320  2013/11/23 MT3QHYの中心に変更
        int yoa_mes = 480 / 2; //256;  //240
        double test_start_id = 0;
        double xoa_test_start = 70;
        double yoa_test_start = 70;
        double xoa_test_step = 1;
        double yoa_test_step = 1;

        #endregion

        #region グローバル変数
        //コマンドライン引数
        string[] cmds;
        Settings appSettings = new Settings();
        //設定保存先のファイル名
        string SettingsfileName ;//= "settings.config"; //@"C:\test\settings.config";
        string appTitle = "MT3";

        // メイン装置光軸座標
        int xoa;  //320
        int yoa;  //240
        int roa = 10;
        double xoad, yoad;

        public double theta_c = 0 ;
        public double dx, dy ;
        public double az0, alt0, vaz0, valt0; // 流星位置、速度（前フレームの値）
        public double az,  alt,  vaz,  valt; // 流星位置、速度
        public double az1, alt1, vaz1, valt1; // 流星位置、速度（次フレームの値）
        public double daz, dalt, dvaz, dvalt; // 流星位置差、速度差（前フレームからの）
        position_mesure pos_mes = new position_mesure();

        // 観測開始からのフレーム番号
        int frame_id = 0;
        int id_mon = 0;
        DateTime LiveStartTime;
        //long timestamp; // [us]
        long frame_timestamp; //[us]
        double dFramerate = 0; // Frame rate[fr/s]
        double reqFramerate = 0; // 要求Frame rate[fr/s]
        double dExpo = 0; // Exposure[us]
        long igain = 0; //Gain

        ImageData imgdata = new ImageData(640,480); //struck 初期化ダミー
        CircularBuffer fifo = new CircularBuffer();

        private ImageProvider m_imageProvider = new ImageProvider(); /* Create one image provider. */
        TIS.Imaging.ImageBuffer CurrentBuffer = null;

        // IDS
        private Int32 u32DisplayID = 0;
        uEye.Camera cam;
        uEye.Defines.Status statusRet = 0;
        uEye.Types.ImageInfo imageInfo;
        ulong ueye_frame_number = 0;
        Int32 s32MemID;

        double set_exposure  = 3;   // [ms]            F1.8:F4  exp 8ms:3ms  gain 1024: 100  約106倍
        double set_exposure1 = 0.2; // [ms]
 
        IplImage img_dmk3, img_dmk, img2, imgLabel , imgAvg;
        CvBlobs blobs = new CvBlobs();
        CvFont font = new CvFont(FontFace.HersheyComplex, 0.50, 0.50);

        double gx, gy, max_val, kgx, kgy, kvx, kvy, sgx, sgy;
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

        // 位置補正データ
        CvMat grid_az = new CvMat(360, 90, MatrixType.F64C1);
        CvMat grid_alt = new CvMat(360, 90, MatrixType.F64C1);
        //frame rate 用
        Stopwatch sw_fr = new Stopwatch();
        long elapsed_fr0 = 0, elapsed_fr1 = 0;

        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        long elapsed0 = 0, elapsed1 = 0, elapsed2 = 0;
        long elapsed20 = 0, elapsed21 = 0, elapsed22 = 0;
        double lap21=0, lap22, lap0 = 0, lap1 = 0, lap2 = 0, alpha = 0.001;
        string fr_str;
        private BackgroundWorker worker;
        private BackgroundWorker worker_udp;
        Udp_kv udpkv = new Udp_kv();
        long udp_packet_id = 0;

        FSI_PID_DATA pid_data = new FSI_PID_DATA();
        KV_PID_DATA kv_pid_data = new KV_PID_DATA();
        MT_MONITOR_DATA mtmon_data = new MT_MONITOR_DATA();
        int mmUdpPortBroadCast     = 24410;            // （受信）
        int mmUdpPortBroadCastSent = 24411;            // （送信）
        int mmFsiUdpPortMTmonitor  = 24415;
        string mmFsiCore_i5 = "192.168.1.211"; // for MTmon
        string mmFsiKV1000  = "192.168.1.10" ; // KV1000 UDP data 送信アドレス
        string mmLocalIP = "";
        string mmLocalHost = "";
        System.Net.Sockets.UdpClient udpc3 = null;
        DriveInfo cDrive = new DriveInfo("D");
        long diskspace;
        System.IO.StreamWriter log_writer; //= new System.IO.StreamWriter(@"D:\img_data\log.txt", true);
        
        [DllImport("kernel32.dll")]
        static extern unsafe void CopyMemory(void* dst, void* src, int size);        
        
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint timeEndPeriod(uint uMilliseconds);
        uint time_period = 1;

        #endregion

        public void IplImageInit()
        {
            int wi = appSettings.Width;
            if (appSettings.CameraColor == Camera_Color.mono12packed)
            {
                wi = appSettings.Width + (appSettings.Width / 2 ); 
            }
                
            img_dmk3 = new IplImage(wi, appSettings.Height, BitDepth.U8, 3);
            img_dmk  = new IplImage(wi, appSettings.Height, BitDepth.U8, 1);
            // IplImage img_dark8 = Cv.LoadImage(@"C:\Users\Public\piccolo\dark00.bmp", LoadMode.GrayScale);
            img2     = new IplImage(wi, appSettings.Height, BitDepth.U8, 1);
            imgLabel = new IplImage(wi, appSettings.Height, CvBlobLib.DepthLabel, 1);
            
            imgAvg   = new IplImage(wi, appSettings.Height, BitDepth.F32, 1);

            imgdata.init(wi, appSettings.Height);
            // FIFO init
            if (appSettings.CamPlatform == Platform.MT2)
            {
                fifo.init(appSettings.FifoMaxFrame, wi, appSettings.Height, appSettings.NoCapDev, appSettings.SaveDir,appSettings.AviMaxFrame, 2);
            }
            else
            {
                fifo.init(appSettings.FifoMaxFrame, wi, appSettings.Height, appSettings.NoCapDev, appSettings.SaveDir, appSettings.AviMaxFrame);
            }
        }

        # region Settings

        public void SettingsSave(Settings sett)
        {
            string fileName = string.Format("settings{00}.config", sett.ID);  //@"C:\test\settings.config";

            //＜XMLファイルに書き込む＞
            //XmlSerializerオブジェクトを作成
            //書き込むオブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer1 = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            //ファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false, new System.Text.UTF8Encoding(false));
            //シリアル化し、XMLファイルに保存する
            serializer1.Serialize(sw, sett);
            //閉じる
            sw.Close();
        }
        public Settings SettingsLoad(int ID)
        {
            //loadする設定を作成する
            Settings appSettings = new Settings();
            string fileName = string.Format("settings{00}.config", ID);  //@"C:\test\settings.config";

            // ファイルが存在しているかどうか確認する
            if (!System.IO.File.Exists(fileName))
            {
                SettingsMake();
            }

            //＜XMLファイルから読み込む＞
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            //ファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(
                fileName, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            appSettings =
                (Settings)serializer2.Deserialize(sr);
            //閉じる
            sr.Close();
            return appSettings;
        }
        public void SettingsRingBuf()
        {

        }
        public void SettingsMake()
        {
            //保存する設定を作成する
            Settings sett = new Settings();
            sett.Text = "IDS UI-2410SE-M";
            sett.ID = 4;             //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 4;
            sett.CameraType = "IDS"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 2;       //カメラタイプ毎のID
            sett.CameraColor = 0;    // 0:mono  1:color
            sett.Width  = 640;
            sett.Height = 480;
            sett.FocalLength = 12.5;      //[mm]
            sett.Ccdpx = 0.0074; //[mm]
            sett.Ccdpy = 0.0074; //[mm]
            sett.Framerate = 75.0; //[fps]
            sett.FifoMaxFrame = 64;
            sett.Exposure = 13; //[ms]
            sett.Gain = 100;
            sett.UseDetect = true;
            sett.ThresholdBlob = 128;     // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410;
            sett.UdpPortSend    = 24429;
            sett.SaveDir = @"C:\Users\Public\img_data\";
            sett.SaveDrive = "C:";
            SettingsSave(sett);

            // MT2 Basler Guide
            sett.Text = "Basler acA640-120gm";
            sett.ID = 10;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 10;
            sett.CameraType = "Basler"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 2;          //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.GIGE;
            sett.CamPlatform = Platform.MT2;
            //sett.FlipOn = true;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width = 656;// 652; // Max 659    4の倍数でメモリ確保される。
            sett.Height = 494; // Max 494
            sett.FocalLength = 35;      //[mm] fuji 35mm
            sett.Ccdpx = 0.0056; //[mm] CCD:ICX618
            sett.Ccdpy = 0.0056; //[mm]
            sett.Xoa = 320;
            sett.Yoa = 240;
            sett.Roa = 1.0 / (Math.Atan(sett.Ccdpx / sett.FocalLength) * 180 / Math.PI); //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.Framerate = 120.0; //[fps]
            sett.FifoMaxFrame = 16;
            sett.Exposure = 8.3; //[ms]
            sett.Gain = 1023; // 100-1023  要検討
            sett.UseDetect = true;
            sett.ThresholdBlob = 64;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24431;
            sett.SaveDir = @"D:\img_data\";
            sett.SaveDrive = "D:";
            SettingsSave(sett);

            // MT3 7 IDS
            sett.Text = "NUV (IDS UI-2410SE-M)";
            sett.ID = 7;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 7;
            sett.CameraType = "IDS"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 3;       //カメラタイプ毎のID  NUV:3
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.USB2;
            sett.CamPlatform = Platform.MT3;
            sett.FlipOn = false;
            //sett.Flipmode = OpenCvSharp.FlipMode.X;
            //sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width  = 640;// 652; // Max 659    4の倍数でメモリ確保される。
            sett.Height = 480; // Max 494
            sett.FocalLength = 80;    //[mm] EL Nikkor 80mm f5.6
            sett.Ccdpx = 0.0074; //[mm] CCD:ICX414
            sett.Ccdpy = 0.0074; //[mm]
            sett.Xoa = 320;
            sett.Yoa = 240;
            sett.Roa = 1.0 / (Math.Atan(sett.Ccdpx / sett.FocalLength) * 180 / Math.PI); //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.PixelClock = 30;//[MHz]
            sett.Framerate = 75.0; //[fps]
            sett.FifoMaxFrame = 64;
            sett.PreSaveNum = 0;
            sett.Exposure = 13.3; //[ms]
            sett.Gain = 100; // 100-1023  要検討
            sett.UseDetect = false;
            sett.ThresholdBlob = 128;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24441; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24423;
            sett.IP_KV1000SpCam2 = "192.168.1.204";
            sett.UdpPortKV1000SpCam2 = 24426;
            sett.SaveDir = @"F:\img_data\";
            sett.SaveDrive = "F:";
            SettingsSave(sett);

            // FishEye2 PointGreyCamera
            sett.Text = "FishEye2 PGC GS3-U3-23S6M";
            sett.ID = 1;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 1;
            sett.CameraType = "PG";    //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 3;         //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.USB3;
            sett.CamPlatform = Platform.Fish1;
            sett.FlipOn = false;
            //sett.Flipmode = OpenCvSharp.FlipMode.X;
            //sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width = 1920; // 652; //Max 659    4の倍数でメモリ確保される。
            sett.Height = 1200; // 949; //Max 494    約2.2MB／fr
            sett.FocalLength = 1.8;      //[mm] Fuji 1.8mm f1.8
            sett.Ccdpx = 0.00586; //[mm] CCD:IMX174
            sett.Ccdpy = 0.00586; //[mm]
            sett.Xoa = 960;// 320;
            sett.Yoa = 600;// 240;            
            sett.Roa = 10.0 / (Math.Atan(sett.Ccdpx / sett.FocalLength) * 180 / Math.PI); //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.Framerate = 50;// 100.0; //[fps]
            sett.FifoMaxFrame = 256;
            sett.ExposureValue = -0.5;
            sett.Exposure = 19.9; //[ms]
            sett.Gain = 1023; // 100-1023  要検討
            sett.UseDetect = true;
            sett.PreSaveNum = 100 ;
            sett.PostSaveProcess = true;
            sett.AviMaxFrame = 500 ;
            sett.ThresholdBlob = 64;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24431;
            sett.SaveDir = @"E:\img_data\";
            sett.SaveDrive = "E:";
            SettingsSave(sett);

            // MT3Wide2 PointGreyCamera
            sett.Text = "Wide2 PGC GS3-U3-23S6M";
            sett.ID = 5;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 5;
            sett.CameraType = "PG";    //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 1;         //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.USB3;
            sett.CamPlatform = Platform.MT3;
            sett.FlipOn = false;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            //sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width = 1920; // 652; //Max 659    4の倍数でメモリ確保される。
            sett.Height =1200; // 949; //Max 494    約2.2MB／fr
            sett.FocalLength = 50;      //[mm] CBC 50mm f1.8
            sett.Ccdpx = 0.00586; //[mm] CCD:IMX174
            sett.Ccdpy = 0.00586; //[mm]
            sett.Xoa = 960;// 320;
            sett.Yoa = 600;// 240;            
            sett.Roa = 1.0 / (Math.Atan(sett.Ccdpx / sett.FocalLength) * 180 / Math.PI); //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.Framerate = 50;// 100.0; //[fps]
            sett.FifoMaxFrame = 256;
            sett.Exposure = 9.946; //[ms]
            sett.Gain = 1023; // 100-1023  要検討
            sett.UseDetect = true;
            sett.ThresholdBlob = 64;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24431;
            sett.SaveDir = @"E:\img_data\";
            sett.SaveDrive = "E:";
            SettingsSave(sett);

            // MT3SuperFine2 PointGreyCamera
            sett.Text = "SuperFine2 PGC GS3-U3-23S6C";
            sett.ID = 9;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 9;
            sett.CameraType = "PG";    //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 2;         //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.color;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.USB3;
            sett.CamPlatform = Platform.MT3;
            sett.FlipOn = false;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            //sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width = 1920; // 652; //Max 659    4の倍数でメモリ確保される。
            sett.Height = 1200; // 949; //Max 494
            sett.FocalLength = 300;      //[mm] CBC 50mm f1.8
            sett.Ccdpx = 0.00586; //[mm] CCD:IMX174
            sett.Ccdpy = 0.00586; //[mm]
            sett.Xoa = 960;// 320;
            sett.Yoa = 600;// 240;            
            sett.Roa = 1.0 / (Math.Atan(sett.Ccdpx / sett.FocalLength) * 180 / Math.PI); //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.Framerate = 162.0; //[fps]
            sett.FifoMaxFrame = 16;
            sett.Exposure = 6.11; //[ms]
            sett.Gain = 1023; // 100-1023  要検討
            sett.UseDetect = false;
            sett.ThresholdBlob = 64;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24432;
            sett.SaveDir = @"E:\img_data\";
            sett.SaveDrive = "E:";
            SettingsSave(sett);

            // MT3Fine ImagingSouce
            sett.Text = "Fine IS DMK23G618";
            sett.ID = 8;               //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev   = 8;
            sett.CameraType = "IS";    //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID   = 1;       //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.GIGE;
            sett.CamPlatform = Platform.MT3;
            sett.FlipOn = false;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            sett.IP_GIGE_Camera = "192.168.1.151"; //GIGE Camera only.
            sett.Width  = 640; // 652; //Max 659    4の倍数でメモリ確保される。
            sett.Height = 480; // 949; //Max 494
            sett.FocalLength = 35;      //[mm] fuji 35mm
            sett.Ccdpx = 0.0056; //[mm] CCD:ICX618
            sett.Ccdpy = 0.0056; //[mm]
            sett.Xoa = 307;// 320;
            sett.Yoa = 184;// 240;            
            sett.Roa = 1.0/(Math.Atan(sett.Ccdpx/sett.FocalLength)*180/Math.PI) ; //半径1deg    // 255x192:ace640の縦視野
            sett.Theta = 0;
            sett.Framerate = 120.0; //[fps]
            sett.FifoMaxFrame = 16;
            sett.Exposure = 8.3; //[ms]
            sett.Gain = 1023; // 100-1023  要検討
            sett.UseDetect = true;
            sett.ThresholdBlob = 64;    // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; // Broadcast0
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortSend = 24431;
            sett.SaveDir = @"F:\img_data\";
            sett.SaveDrive = "F:";
            SettingsSave(sett);

            // MT2 Echelle
            sett.Text = "AVT GE-2040";
            sett.ID = 11;             //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 11;
            sett.CameraType = "AVT"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 2;       //カメラタイプ毎のID
            //sett.CameraColor = Camera_Color.mono12packed;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraColor = Camera_Color.mono;              // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.GIGE;
            sett.CamPlatform = Platform.MT2;
            sett.Flipmode = OpenCvSharp.FlipMode.XY;
            sett.IP_GIGE_Camera = "192.168.1.152"; //GIGE Camera only.
            sett.Width  = 2048;
            sett.Height = 2048;
            sett.FocalLength = 50;      //[mm]
            sett.Ccdpx = 0.0074; //[mm]
            sett.Ccdpy = 0.0074; //[mm]
            sett.Theta = 0.0;
            sett.Framerate = 15.0; //[fps]
            sett.FifoMaxFrame = 4;
            sett.PreSaveNum = 0;
            sett.Exposure = 66; //[ms]
            sett.Gain = 30; // 0-30  要検討
            sett.UseDetect = false;
            sett.ThresholdBlob = 128;     // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24441; //Broadcast1
            sett.UdpPortSend    = 24433;
            sett.SaveDir = @"D:\img_data\";
            sett.SaveDrive = "D:";
            SettingsSave(sett);

            // Wat100N Cam ID 20
            sett.Text = "Watec WAT-100N";
            sett.ID = 20;             //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 20;
            sett.CameraType = "analog"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 0;       //カメラタイプ毎のID
            sett.CameraColor = 0;    // 0:mono  1:color
            sett.CamPlatform = Platform.Wide;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            sett.Width = 640;
            sett.Height = 480;
            sett.FocalLength = 35;      //[mm]
            sett.Ccdpx = 0.010; //[mm]
            sett.Ccdpy = 0.010; //[mm]
            sett.Xoa = 327; //435;
            sett.Yoa = 292; //(480 - 197);// for flip
            sett.Roa = 91; //直径3deg     192/2;  // 255x192:ace640の縦視野
            sett.Theta = 180;
            sett.Framerate = 30.0; //[fps]
            sett.FifoMaxFrame = 64;
            sett.PreSaveNum = 30;
            sett.UseDetect = true;
            sett.ThresholdBlob = 32;     // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24442; //Broadcast2
            //sett.UdpPortRecieve = 24410; // Broadcast0
            sett.UdpPortSend = 24451;
            sett.SaveDir = @"F:\img_data\";
            sett.SaveDrive = "F:";
            SettingsSave(sett);

            // Wat100N Cam ID 21
            sett.Text = "Watec WAT-100N";
            sett.ID = 21;             //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 21;
            sett.CameraType = "analog"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 0;       //カメラタイプ毎のID
            sett.CameraColor = 0;    // 0:mono  1:color
            sett.CamPlatform = Platform.Wide;
            sett.Flipmode = OpenCvSharp.FlipMode.X;
            sett.Width = 640;
            sett.Height = 480;
            sett.FocalLength = 35;      //[mm]
            sett.Ccdpx = 0.010; //[mm]
            sett.Ccdpy = 0.010; //[mm]
            sett.Xoa = 435;
            sett.Yoa = (480-197);// for flip
            sett.Roa = 91; //直径3deg     192/2;  // 255x192:ace640の縦視野
            sett.Theta = 180;
            sett.Framerate = 30.0; //[fps]
            sett.FifoMaxFrame = 64;
            sett.PreSaveNum = 30;
            sett.UseDetect = true;
            sett.ThresholdBlob = 128;     // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            //sett.UdpPortRecieve = 24442; //Broadcast2
            sett.UdpPortRecieve = 24410; // Broadcast0
            sett.UdpPortSend = 24451;
            sett.SaveDir = @"D:\img_data\";
            sett.SaveDrive = "D:";
            SettingsSave(sett);

            // IDS
            sett.Text = "IDS UI-1540";
            sett.ID = 14;             //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            sett.NoCapDev = 14;
            sett.CameraType = "IDS"; //カメラタイプ： IDS Basler AVT IS analog
            sett.CameraID = 5;       //カメラタイプ毎のID
            sett.CameraColor = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            sett.CameraInterface = Camera_Interface.USB2;
            sett.IP_GIGE_Camera = "192.168.1.xxx"; //GIGE Camera only.
            sett.Width  = 1280;
            sett.Height = 1024;
            sett.FocalLength = 50;      //[mm]
            sett.Ccdpx = 0.0074; //[mm]
            sett.Ccdpy = 0.0074; //[mm]
            sett.PixelClock = 43; //[MHz] 5-43MHz  UI-1540
            sett.Framerate = 25.0; //[fps]
            sett.FifoMaxFrame = 32;
            sett.Exposure = 33; //[ms]
            sett.Gain = 30; // 0-30  要検討
            sett.UseDetect = false;
            sett.ThresholdBlob = 128;     // 検出閾値（０－２５５）
            sett.ThresholdMinArea = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            sett.UdpPortRecieve = 24410; //Broadcast0
            sett.UdpPortSend = 24429;
            sett.SaveDir = @"C:\Users\Public\img_data\";
            sett.SaveDrive = "C:";
            SettingsSave(sett);

            /*
            sett.IP_KV1000SpCam2 = "192.168.1.204";
            sett.UdpPortKV1000SpCam2 = 24410;
            sett.IP_KV1000 = "192.168.1.10";
            sett.UdpPortKV1000 = 8503;
            sett.IP_MtMon = "192.168.1.211";
            sett.UdpPortMtMon = 24415;
            */
            //const int WIDTH = 2456; // 2456 max piA2400-12gm
            //const int HEIGHT = 2058; // 2058 max
        }
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
            cameras = sys.Cameras;
            if (cameras.Count == 0)
            {
                MessageBox.Show("AVT Camera not found!");
                return;
            }
            //foreach( AVT.VmbAPINET.Camera camera in cameras){
            try
            {
                camera = sys.OpenCameraByID(appSettings.IP_GIGE_Camera, AVT.VmbAPINET.VmbAccessModeType.VmbAccessModeFull);
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
            feature = features["GVSPAdjustPacketSize"];
            feature.RunCommand();

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
            // パラメータロード
            feature = features["UserSetSelector"];
            if (appSettings.CameraColor == Camera_Color.mono12packed)
            {
                feature.EnumValue = "UserSet1";  // packed12
            }
            else if (appSettings.CameraColor == Camera_Color.mono)
            {
                feature.EnumValue = "UserSet2";  // mono8
            }
            feature = features["UserSetLoad"];
            feature.RunCommand();

            //露出設定
            feature = features["ExposureTimeAbs"];
            feature.FloatValue = 1000*appSettings.Exposure;// 50000;//us
            //Gain設定
            feature = features["GainRaw"];
            feature.IntValue = (int)appSettings.Gain; // range:0-30

            //撮像開始
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

            ++frame_id; 
            detect();            
            imgdata_push_FIFO();
            elapsed22 = sw2.ElapsedTicks; // 0.1ms
            // 処理速度
            //double sf = (double)Stopwatch.Frequency / 1000; //msec
            lap22 = (1 - alpha) * lap22 + alpha * (elapsed22 - elapsed20) / sf;
        }

        public UserSetSelectorEnum UserSetSelector
        {
            get { return (UserSetSelectorEnum)UserSetSelectorFeature.EnumIntValue; }
            set { UserSetSelectorFeature.EnumIntValue = (int)value; }
        }
        public AVT.VmbAPINET.Feature UserSetSelectorFeature
        {
            get
            {
                if (m_UserSetSelectorFeature == null)
                    m_UserSetSelectorFeature = camera.Features["UserSetSelector"];
                return m_UserSetSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetSelectorFeature = null;

        public double StatFrameRate()
        {
            feature = features["StatFrameRate"];
            return feature.FloatValue;
        }
        public double ExposureTimeAbs()
        {
            feature = features["ExposureTimeAbs"];
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
            feature = features["GainRaw"];
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

        #region Public methods.

        #region Category /AcquisitionControl

        public void AcquisitionAbort()
        {
            AcquisitionAbortFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature AcquisitionAbortFeature
        {
            get
            {
                if (m_AcquisitionAbortFeature == null)
                    m_AcquisitionAbortFeature = camera.Features["AcquisitionAbort"];
                return m_AcquisitionAbortFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionAbortFeature = null;

        public void AcquisitionStart()
        {
            AcquisitionStartFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature AcquisitionStartFeature
        {
            get
            {
                if (m_AcquisitionStartFeature == null)
                    m_AcquisitionStartFeature = camera.Features["AcquisitionStart"];
                return m_AcquisitionStartFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionStartFeature = null;

        public void AcquisitionStop()
        {
            AcquisitionStopFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature AcquisitionStopFeature
        {
            get
            {
                if (m_AcquisitionStopFeature == null)
                    m_AcquisitionStopFeature = camera.Features["AcquisitionStop"];
                return m_AcquisitionStopFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionStopFeature = null;

        public void TriggerSoftware()
        {
            TriggerSoftwareFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature TriggerSoftwareFeature
        {
            get
            {
                if (m_TriggerSoftwareFeature == null)
                    m_TriggerSoftwareFeature = camera.Features["TriggerSoftware"];
                return m_TriggerSoftwareFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerSoftwareFeature = null;

        #endregion

        #region Category /SavedUserSets

        public void UserSetLoad()
        {
            UserSetLoadFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature UserSetLoadFeature
        {
            get
            {
                if (m_UserSetLoadFeature == null)
                    m_UserSetLoadFeature = camera.Features["UserSetLoad"];
                return m_UserSetLoadFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetLoadFeature = null;

        public void UserSetSave()
        {
            UserSetSaveFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature UserSetSaveFeature
        {
            get
            {
                if (m_UserSetSaveFeature == null)
                    m_UserSetSaveFeature = camera.Features["UserSetSave"];
                return m_UserSetSaveFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetSaveFeature = null;

        #endregion

        #region Category /Stream/Settings

        public void GVSPAdjustPacketSize()
        {
            GVSPAdjustPacketSizeFeature.RunCommand();
        }
        public AVT.VmbAPINET.Feature GVSPAdjustPacketSizeFeature
        {
            get
            {
                if (m_GVSPAdjustPacketSizeFeature == null)
                    m_GVSPAdjustPacketSizeFeature = camera.Features["GVSPAdjustPacketSize"];
                return m_GVSPAdjustPacketSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPAdjustPacketSizeFeature = null;

        #endregion

        #endregion

        #region Enum declarations.

        public enum AcquisitionModeEnum
        {
            Continuous = 1,
            SingleFrame = 2,
            MultiFrame = 3,
            Recorder = 4
        }

        public enum PixelFormatEnum
        {
            Mono8 = 17301505,
            BayerGR8 = 17301512,
            BayerRG8 = 17301513,
            BayerBG8 = 17301515,
            Mono10 = 17825795,
            Mono12 = 17825797,
            BayerBG10 = 17825807,
            BayerGR12 = 17825808,
            BayerRG12 = 17825809,
            YUV411Packed = 34340894,
            YUV422Packed = 34603039,
            RGB8Packed = 35127316,
            BGR8Packed = 35127317,
            YUV444Packed = 35127328,
            RGBA8Packed = 35651606,
            BGRA8Packed = 35651607,
            RGB10Packed = 36700184,
            RGB12Packed = 36700186
        }

        public enum UserSetSelectorEnum
        {
            Default = 0,
            UserSet1 = 1,
            UserSet2 = 2,
            UserSet3 = 3,
            UserSet4 = 4,
            UserSet5 = 5
        }

        #endregion

        #region common FIFO UDP
        /// <summary>
        /// FIFO pushルーチン
        /// </summary>
        private void imgdata_push_FIFO(byte[] buf)
        {
            // 文字入れ
            //String str = String.Format("ID:{0,6:D1} ", imgdata.id) + imgdata.t.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,6:F1},{1,6:F1})({2,6:F1})", gx, gy, max_val);
            //img_dmk.PutText(str, new CvPoint(10, 460), font, new CvColor(255, 100, 100));

            //try
            //{
            imgdata.id = (int)frame_id;     // (int)imageInfo.FrameNumber;
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
            if (fifo.Count == appSettings.FifoMaxFrame - 1) fifo.EraseLast();
            fifo.InsertFirst(imgdata, buf);
            /*}
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }*/
        }


        // PID data送信ルーチン
        private void Pid_Data_Send_Init()
        {
            //PID送信用UDP
            //バインドするローカルポート番号
            int localPort = appSettings.UdpPortSend;// 24412;  //mmFsiUdpPortMT3FineS
            try
            {
                udpc3 = new System.Net.Sockets.UdpClient(localPort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }
        // PID data送信ルーチン
        private void Pid_Data_Send()
        {
            // PID data send for UDP
            //データを送信するリモートホストとポート番号
            string remoteHost = "192.168.1.206";
            int remotePort = 24410; // KV1000SpCam
            //送信するデータを読み込む
            ++(pid_data.id);
            pid_data.swid = (ushort)appSettings.ID;
            //pid_data.t = TDateTimeDouble(LiveStartTime.AddSeconds(CurrentBuffer.SampleEndTime));//(DateTime.Now);
            pid_data.t =  TDateTimeDouble(DateTime.Now);
            pid_data.vmax = (ushort)(max_val);
            pid_data.dx = (float)(sgx - xoa);
            pid_data.dy = (float)(sgy - yoa);
            pid_data.az = (float)(az);
            pid_data.alt = (float)(alt);
            pid_data.vaz = (float)(vaz);
            pid_data.valt = (float)(valt);
            if (udpkv.mt3state_move == 0 && udpkv.mt3state_center == 0 && udpkv.mt3state_truck != 0) pid_data.kalman_state = 1;
            else pid_data.kalman_state = 0;

            byte[] sendBytes = ToBytes(pid_data);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }
        /// <summary>
        /// PID data送信ルーチン(KV1000) DM937
        /// </summary>
        private void Pid_Data_Send_KV1000(short id, double daz, double dalt, double vk)
        {
            // PID data send for UDP
            //データを送信するリモートホストとポート番号
            //string remoteHost = "192.168.1.10";
            //int remotePort = 8503; //KV1000 UDP   8501(KV1000 cmd); // KV1000SpCam
            string remoteHost = appSettings.IP_KV1000SpCam2 ;
            int remotePort = appSettings.UdpPortKV1000SpCam2;// 24426; //KV1000SpCam2 UDP

            //送信するデータを読み込む
            //string s1 = string.Format("WRS DM11393 3 {0} {1} {2}\r", (ushort)id, udpkv.PIDPV_makedata(daz), udpkv.PIDPV_makedata(dalt));
            //byte[] sendBytes = Encoding.ASCII.GetBytes(s1);
            kv_pid_data.fine_time = udpkv.EndianChange((short)udpkv.udp_time_code);
            kv_pid_data.fine_id = udpkv.EndianChange(id);
            kv_pid_data.fine_az = udpkv.EndianChange((short)udpkv.PIDPV_makedata(daz));
            kv_pid_data.fine_alt = udpkv.EndianChange((short)udpkv.PIDPV_makedata(dalt));
            kv_pid_data.fine_vk = udpkv.EndianChange((short)udpkv.PIDPV_makedata(vk));

            byte[] sendBytes = udpkv.ToBytes(kv_pid_data);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }

        /// <summary>
        /// PID data送信ルーチン(KV1000 UDP2)
        /// </summary>
        private void Pid_Data_Send_KV1000_SpCam2(short id, double daz, double dalt, double vk)
        {
            // PID data send for UDP
            if (appSettings.ID == 10) // MT2 WideCam 設定
            {
                //送信するデータを読み込む
                /*
                string s1 = string.Format("WRS DM937 3 {0} {1} {2}\r", (ushort)id, udpkv.PIDPV_makedata(daz), udpkv.PIDPV_makedata(dalt));
                kv_pid_data.mt2_wide_time = (short)udpkv.udp_time_code;
                kv_pid_data.mt2_wide_id   = id;
                kv_pid_data.mt2_wide_az   = udpkv.PIDPV_makedata(daz);
                kv_pid_data.mt2_wide_alt  = udpkv.PIDPV_makedata(dalt);
                kv_pid_data.mt2_wide_vk   = udpkv.PIDPV_makedata(vk);
                */
                //送信するデータを読み込む ( for KV1000 )
                //string s1 = string.Format("WRS DM937 3 {0} {1} {2}\r", (ushort)id, udpkv.PIDPV_makedata(daz), udpkv.PIDPV_makedata(dalt));
                kv_pid_data.mt2_wide_time = udpkv.EndianChange((short)udpkv.udp_time_code);
                kv_pid_data.mt2_wide_id = udpkv.EndianChange(id);
                kv_pid_data.mt2_wide_az = udpkv.EndianChange(udpkv.PIDPV_makedata(daz));
                kv_pid_data.mt2_wide_alt = udpkv.EndianChange(udpkv.PIDPV_makedata(dalt));
                kv_pid_data.mt2_wide_vk = udpkv.EndianChange(udpkv.PIDPV_makedata(vk));
            } else if (appSettings.ID == 8) // MT3 Fine 設定
            {
                //送信するデータを読み込む
                /*
                string s1 = string.Format("WRS DM937 3 {0} {1} {2}\r", (ushort)id, udpkv.PIDPV_makedata(daz), udpkv.PIDPV_makedata(dalt));
                kv_pid_data.mt2_wide_time = (short)udpkv.udp_time_code;
                kv_pid_data.mt2_wide_id   = id;
                kv_pid_data.mt2_wide_az   = udpkv.PIDPV_makedata(daz);
                kv_pid_data.mt2_wide_alt  = udpkv.PIDPV_makedata(dalt);
                kv_pid_data.mt2_wide_vk   = udpkv.PIDPV_makedata(vk);
                */
                //送信するデータを読み込む ( for KV1000 )
                //string s1 = string.Format("WRS DM937 3 {0} {1} {2}\r", (ushort)id, udpkv.PIDPV_makedata(daz), udpkv.PIDPV_makedata(dalt));
                kv_pid_data.fine_time = udpkv.EndianChange((short)udpkv.udp_time_code);
                kv_pid_data.fine_id = udpkv.EndianChange(id);
                kv_pid_data.fine_az = udpkv.EndianChange(udpkv.PIDPV_makedata(daz));
                kv_pid_data.fine_alt= udpkv.EndianChange(udpkv.PIDPV_makedata(dalt));
                kv_pid_data.fine_vk = udpkv.EndianChange(udpkv.PIDPV_makedata(vk));
            }
            else return;

            //データを送信するリモートホストとポート番号
            string remoteHost = "192.168.1.204";                     // KV1000SpCam2
            int remotePort = 24426; //KV1000 UDP   8501(KV1000 cmd); // KV1000SpCam2
            byte[] sendBytes = udpkv.ToBytes(kv_pid_data);

            try
            {
                //リモートホストを指定してデータを送信する
                udpc3.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }
        }

        private async void star_auto_check()
        {
            if (appSettings.ID != 10 && appSettings.ID != 8) return;

            string s="", name = "";
            int star_id_min = (int)numericUpDownStarMin.Value; // 0-5 月、惑星  6:シリウス　7:ベガ
            int star_id_max = star_id_min + (int)numericUpDownStarCount.Value; // 0-5 月、惑星  6:シリウス　7:ベガ
            if (star_id_max > 98) star_id_max = 98; 
            double az0 = 90, alt0 = 90;
            double az_t = 90, alt_t = 90, vmag = 0;
            double min_alt = 5.0; //最低高度

            Star.init();
            //Star.ID = (int)numericUpDownStarNum.Value; // 0-5 月、惑星  6:シリウス　7:ベガ
            //Star.cal_azalt();
            //s = string.Format("Star count:{0} {1} {2} Az:{3} {4}\n", Star.Count, Star.ID, Star.Name, Star.Az, Star.Alt);
            //richTextBox1.AppendText(s);
            //if (Star.Alt <= 0) return;

            // for star mes
            List<StarAzAlt> star_azalt = new List<StarAzAlt>();
            for (int i = star_id_min; i < star_id_max; i++)
            {
                Star.ID = i;
                Star.cal_azalt();
                if (Star.Alt > min_alt)
                {
                    StarAzAlt sta = new StarAzAlt();
                    sta.Az = Star.Az;
                    sta.Alt = Star.Alt;
                    sta.Vmag = Star.Mag;
                    sta.Name = Star.Name;
                    star_azalt.Add(sta);
                    s = string.Format("Star count:{0} {1} {2} Az:{3} {4}\n", i, Star.ID, Star.Name, Star.Az, Star.Alt);
                    richTextBox1.Focus(); richTextBox1.AppendText(s);
                }
            }

            while (star_azalt.Count > 0)
            {
                // 次のターゲットを選択（距離）
                int ii = 0;
                double len_max = 1000000;
                for (int i = 0; i < star_azalt.Count; i++)
                {
                    double len = Common.Cal_Distance(az0, alt0, star_azalt[i].Az, star_azalt[i].Alt);
                    if (len < len_max)
                    {
                        ii = i;
                        len_max = len;
                        az_t = star_azalt[i].Az;
                        alt_t = star_azalt[i].Alt;
                        vmag = star_azalt[i].Vmag;
                        name = star_azalt[i].Name;
                    }
                }
                star_azalt.RemoveAt(ii);

                double daz_grid = (double)numericUpDown_daz.Value / 10.0 - grid_az.Get2D((int)az_t, (int)alt_t);
                double dalt_grid = (double)numericUpDown_dalt.Value / 10.0 - grid_alt.Get2D((int)az_t, (int)alt_t);

                // KV1000通信  MT2,3 move
                if (appSettings.ID == 10) // MT2 WideCam 設定
                {
                    s = send_mt2_cmd(az_t + daz_grid, alt_t + dalt_grid);
                }
                if (appSettings.ID == 8) // MT3 Fine 設定
                {
                    s = send_mt3_cmd(az_t + daz_grid, alt_t + dalt_grid,0,0);
                }                   
                    
                richTextBox1.Focus(); richTextBox1.AppendText(s);
                //await Task.Run(() => System.Threading.Thread.Sleep(3000)); 
                await Task.Delay(2000);
                s = "2s wait\n"; richTextBox1.Focus(); richTextBox1.AppendText(s);

                if (appSettings.ID == 10) // MT2 WideCam 設定
                {
                    theta_c = -udpkv.cal_mt2_theta(appSettings.Flipmode, appSettings.FlipOn, az_t, alt_t) - appSettings.Theta;
                    //udpkv.cxcy2azalt_mt2(-dx, -dy, udpkv.az1_c, udpkv.alt1_c, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az, ref alt);
                    //udpkv.cxcy2azalt_mt2(-(dx + kvx), -(dy + kvy), udpkv.az1_c, udpkv.alt1_c, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az1, ref alt1);
                    udpkv.cxcy2azalt_mt2(-dx, -dy, az_t, alt_t, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az, ref alt);
                    //udpkv.cxcy2azalt_mt2(-(dx + kvx), -(dy + kvy), udpkv.az1_c, udpkv.alt1_c, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az1, ref alt1);
                }
                if (appSettings.ID == 8) // MT3 Fine 設定
                {
                    theta_c = appSettings.Theta;
                    udpkv.cxcy2azalt(-dx, -dy, az_t, alt_t, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az, ref alt);
                }
                daz = az - az_t; dalt = alt - alt_t;             //位置誤差
                s = string.Format("daz_grid[ {0}, {3} ] = az_t[{1}] -KV_az_c[{2}]\r", daz_grid, az_t, udpkv.az2_c, dalt_grid);
                richTextBox1.Focus(); richTextBox1.AppendText(s);
                //データ保存
                if (max_val > 0)
                {
                    write_star_position_error(name, az_t, alt_t, daz - daz_grid, dalt - dalt_grid, vmag, max_val, gx, gy, xoa, yoa);
                }

            }
        }
        // 補正データ読み込み(360x90)
        public void read_grid_data()
        {
            if (appSettings.CamPlatform == Platform.MT2)
            {
                // ファイルからテキストを読み出し。
                string fnaz  = appSettings.SaveDir + "lib\\grid_z1_az.txt";
                string fnalt = appSettings.SaveDir + "lib\\grid_z1_alt.txt";
                using (StreamReader r = new StreamReader(fnaz))
                {
                    int j = 0;
                    string line;
                    while ((line = r.ReadLine()) != null) // 1行ずつ読み出し。
                    {
                        // 区切りで分割して配列に格納する
                        string[] stArrayData = line.Split(' ');
                        int i = 0;
                        foreach (string s in stArrayData)
                        {
                            double data = double.Parse(s);
                            grid_az.Set2D(j, i, data);
                            i++;
                        }
                        j++;
                    }
                }
                // ファイルからテキストを読み出し。
                using (StreamReader r = new StreamReader(fnalt))  //(@"D:\img_data\lib\grid_z1_alt.txt"))
                {
                    int j = 0;
                    string line;
                    while ((line = r.ReadLine()) != null) // 1行ずつ読み出し。
                    {
                        // 区切りで分割して配列に格納する
                        string[] stArrayData = line.Split(' ');
                        int i = 0;
                        foreach (string s in stArrayData)
                        {
                            double data = double.Parse(s);
                            grid_alt.Set2D(j, i, data);
                            i++;
                        }
                        j++;
                    }
                }
            }
        }

        private async void sleepAsync(int sec)
        {
            await Task.Delay(sec * 1000);
        }

        // KV1000通信
        private string send_mt2_cmd(double az_t, double alt_t)
        {
            string s, ss;
            Common.Send_cmd_KV1000_init();
            ss = Common.Send_cmd_KV1000(Common.MT2SetPos(az_t , alt_t ));
            System.Threading.Thread.Sleep(100);
            s = string.Format("ST 01001\r");
            ss += Common.Send_cmd_KV1000(s);
            Common.Send_cmd_KV1000_close();
            return ss;
        }
        // KV1000通信
        private string send_mt3_cmd(double az_t, double alt_t, double vaz_t=0, double valt_t=0)
        {
            string s, ss;
            Common.Send_cmd_KV1000_init();
            ss = Common.Send_cmd_KV1000(Common.MT3SetPos(az_t, alt_t, vaz_t, valt_t));
            System.Threading.Thread.Sleep(100);
            s = string.Format("ST 01201\r");
            ss += Common.Send_cmd_KV1000(s);
            Common.Send_cmd_KV1000_close();
            return ss;
        }

        //誤差測定ルーチン
        private void write_star_position_error(string name, double az, double alt, double daz, double dalt, double vmag, double count, double cx, double cy, double xoa, double yoa)
        {
            // appTitle = "MT3" + appSettings.Text +" "+ appSettings.ID.ToString()+"  " + mmLocalHost +"(" + mmLocalIP+")";
            string fn = appSettings.ID.ToString() + "_star_position_error_" + DateTime.Today.ToString("yyyyMM") + ".txt";
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

            using (StreamWriter w = new StreamWriter(fn, true, sjisEnc))
            {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                //             az       alt      daz      dalt     vmag     count    cx       cy       xoa      yoa     name
                w.Write("{11} {0,7:F3} {1,7:F3} {2,7:F3} {3,7:F3} {4,5:F1} {5,7:F1} {6,7:F3} {7,7:F3} {8,7:F3} {9,7:F3} {10}\r\n", az, alt, daz, dalt, vmag, count,cx, cy, xoa, yoa, name, dt);
            }
        }
        #endregion
    }  
}
