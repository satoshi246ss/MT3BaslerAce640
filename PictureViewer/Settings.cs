<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MT3
{
    public class Settings
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int MtMon_ID { get; set; }

        private string _camera_type;
        public string CameraType
        {
            get { return _camera_type; }
            set { _camera_type = value; }
        }

        private int _camera_id;
        public int CameraID
        {
            get { return _camera_id; }
            set { _camera_id = value; }
        }

        private Camera_Color _camera_color;
        public Camera_Color CameraColor
        {
            get { return _camera_color; }
            set { _camera_color = value; }
        }

        private Camera_Interface _camera_interface;
        public Camera_Interface CameraInterface
        {
            get { return _camera_interface; }
            set { _camera_interface = value; }
        }

        private Platform _platform;
        public Platform CamPlatform
        {
            get { return _platform; }
            set { _platform = value; }
        }

        public bool FlipOn { get; set; }

        private OpenCvSharp.FlipMode _flip_mode;
        public  OpenCvSharp.FlipMode Flipmode
        {
            get { return _flip_mode; }
            set { _flip_mode = value; }
        }

        //CCD
        public int Width { get; set; }
        public int Height { get; set; }

        private double _focal_length;
        public double FocalLength
        {
            get { return _focal_length; }
            set { _focal_length = value; }
        }

        private double _ccdpx;
        public double Ccdpx
        {
            get { return _ccdpx; }
            set { _ccdpx = value; }
        }

        private double _ccdpy;
        public double Ccdpy
        {
            get { return _ccdpy; }
            set { _ccdpy = value; }
        }

        private double _xoa;
        public  double Xoa
        {
            get { return _xoa; }
            set { _xoa = value; }
        }

        private double _yoa;
        public double Yoa
        {
            get { return _yoa; }
            set { _yoa = value; }
        }

        private double _roa;
        public double Roa
        {
            get { return _roa; }
            set { _roa = value; }
        }

        private double _theta;
        public double Theta
        {
            get { return _theta; }
            set { _theta = value; }
        }

        private int _pixel_clock;
        public int PixelClock
        {
            get { return _pixel_clock; }
            set { _pixel_clock = value; }
        }

        private double _framerate;
        public double Framerate
        {
            get { return _framerate; }
            set { _framerate = value; }
        }

        private double _exposure;
        public double Exposure
        {
            get { return _exposure; }
            set { _exposure = value; }
        }

        private double _gain;
        public double Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        private uEye_Shutter_Mode _ueye_shutter_mode;
        public uEye_Shutter_Mode uEyeShutterMode
        {
            get { return _ueye_shutter_mode; }
            set { _ueye_shutter_mode = value; }
        }

        private int _fifo_max_frame;
        public int FifoMaxFrame
        {
            get { return _fifo_max_frame; }
            set { _fifo_max_frame = value; }
        }

        private bool _use_detect;
        public bool UseDetect
        {
            get { return _use_detect; }
            set { _use_detect = value; }
        }
        
        private int _threshold_blob;
        public int ThresholdBlob
        {
            get { return _threshold_blob; }
            set { _threshold_blob = value; }
        }

        private double _threshold_min_area;
        public double ThresholdMinArea
        {
            get { return _threshold_min_area; }
            set { _threshold_min_area = value; }
        }

        private string _ip_gige_camera;
        public string IP_GIGE_Camera
        {
            get { return _ip_gige_camera; }
            set { _ip_gige_camera = value; }
        }

        private int _udp_port_recieve;
        public int UdpPortRecieve
        {
            get { return _udp_port_recieve; }
            set { _udp_port_recieve = value; }
        }

        private int _udp_port_send;
        public int UdpPortSend
        {
            get { return _udp_port_send; }
            set { _udp_port_send = value; }
        }

        private string _ip_kv1000spcam2;
        public string IP_KV1000SpCam2
        {
            get { return _ip_kv1000spcam2; }
            set { _ip_kv1000spcam2 = value; }
        }
        
        private int _udp_port_kv1000spcam2;
        public int UdpPortKV1000SpCam2
        {
            get { return _udp_port_kv1000spcam2; }
            set { _udp_port_kv1000spcam2 = value; }
        }

        private string _ip_kv1000;
        public string IP_KV1000
        {
            get { return _ip_kv1000; }
            set { _ip_kv1000 = value; }
        }

        private int _udp_port_kv1000;
        public int UdpPortKV1000
        {
            get { return _udp_port_kv1000; }
            set { _udp_port_kv1000 = value; }
        }

        private string _ip_mtmon;
        public string IP_MtMon
        {
            get { return _ip_mtmon; }
            set { _ip_mtmon = value; }
        }

        private int _udp_port_mtmon;
        public int UdpPortMtMon
        {
            get { return _udp_port_mtmon; }
            set { _udp_port_mtmon = value; }
        }

        //ringbuf
        // file ID
        private int _no_cap_dev;
        public int NoCapDev
        {
            get { return _no_cap_dev; }
            set { _no_cap_dev = value; }
        }
        // Save dir
        private string _save_dir;
        public string SaveDir
        {
            get { return _save_dir; }
            set { _save_dir = value; }
        }
        // 過去画像保存枚数　０ならなし。
        private int _pre_save_num;
        public int PreSaveNum
        {
            get { return _pre_save_num; }
            set { _pre_save_num = value; }
        }
 
        // Default value
        public Settings()
        {
            _text   = "IDS UI-2410SE-M";
            _id     = 4;          //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            _camera_type = "IDS"; //カメラタイプ： IDS Basler AVT IS analog
            _camera_id = 2;       //カメラタイプ毎のID
            _camera_color = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            _camera_interface = Camera_Interface.USB2 ;
            _platform = Platform.MT2;
            FlipOn = false;
            _flip_mode = OpenCvSharp.FlipMode.XY; // XY:負値　は回転させない。X,Yのみ有効
            _ip_gige_camera = "192.168.1.150"; //GIGE Camera only.
            Width = 640;
            Height = 480;
            _focal_length = 12.5;      //[mm]
            _ccdpx = 0.0074; //[mm]
            _ccdpy = 0.0074; //[mm]
            _xoa = Width / 2;
            _yoa = Height / 2;
            _roa = 40;
            _theta = 0;
            _pixel_clock = 43;//[MHz] int
            _framerate = 75.0; //[fps]
            _fifo_max_frame = 64;
            _exposure = 13; //[ms]
            _gain = 100;
            _use_detect = true;
            _threshold_blob = 128;     // 検出閾値（０－２５５）
            _threshold_min_area = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            _udp_port_recieve = 24410;
            _udp_port_send = 24429;
            _ip_kv1000spcam2 = "192.168.1.204";
            _udp_port_kv1000spcam2 = 24426;
            _ip_kv1000 = "192.168.1.10";
            _udp_port_kv1000 = 8503;
            _ip_mtmon = "192.168.1.211";
            _udp_port_mtmon = 24415;
            _no_cap_dev = _id;
            _save_dir = @"C:\Users\Public\img_data\";
            _pre_save_num = 0;
            _ueye_shutter_mode = uEye_Shutter_Mode.Global;
        }

     }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MT3
{
    public class Settings
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int MtMon_ID { get; set; }

        private string _camera_type;
        public string CameraType
        {
            get { return _camera_type; }
            set { _camera_type = value; }
        }

        private int _camera_id;
        public int CameraID
        {
            get { return _camera_id; }
            set { _camera_id = value; }
        }

        private Camera_Color _camera_color;
        public Camera_Color CameraColor
        {
            get { return _camera_color; }
            set { _camera_color = value; }
        }

        private Camera_Interface _camera_interface;
        public Camera_Interface CameraInterface
        {
            get { return _camera_interface; }
            set { _camera_interface = value; }
        }

        private Platform _platform;
        public Platform CamPlatform
        {
            get { return _platform; }
            set { _platform = value; }
        }

        public bool FlipOn { get; set; }

        private OpenCvSharp.FlipMode _flip_mode;
        public  OpenCvSharp.FlipMode Flipmode
        {
            get { return _flip_mode; }
            set { _flip_mode = value; }
        }

        //CCD
        public int Width { get; set; }
        public int Height { get; set; }

        private double _focal_length;
        public double FocalLength
        {
            get { return _focal_length; }
            set { _focal_length = value; }
        }

        private double _ccdpx;
        public double Ccdpx
        {
            get { return _ccdpx; }
            set { _ccdpx = value; }
        }

        private double _ccdpy;
        public double Ccdpy
        {
            get { return _ccdpy; }
            set { _ccdpy = value; }
        }

        private double _xoa;
        public  double Xoa
        {
            get { return _xoa; }
            set { _xoa = value; }
        }

        private double _yoa;
        public double Yoa
        {
            get { return _yoa; }
            set { _yoa = value; }
        }

        private double _roa;
        public double Roa
        {
            get { return _roa; }
            set { _roa = value; }
        }

        private double _theta;
        public double Theta
        {
            get { return _theta; }
            set { _theta = value; }
        }

        private int _pixel_clock;
        public int PixelClock
        {
            get { return _pixel_clock; }
            set { _pixel_clock = value; }
        }

        private double _framerate;
        public double Framerate
        {
            get { return _framerate; }
            set { _framerate = value; }
        }

        private double _exposure;
        public double Exposure
        {
            get { return _exposure; }
            set { _exposure = value; }
        }

        private double _gain;
        public double Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        private uEye_Shutter_Mode _ueye_shutter_mode;
        public uEye_Shutter_Mode uEyeShutterMode
        {
            get { return _ueye_shutter_mode; }
            set { _ueye_shutter_mode = value; }
        }

        private int _fifo_max_frame;
        public int FifoMaxFrame
        {
            get { return _fifo_max_frame; }
            set { _fifo_max_frame = value; }
        }

        private bool _use_detect;
        public bool UseDetect
        {
            get { return _use_detect; }
            set { _use_detect = value; }
        }
        
        private int _threshold_blob;
        public int ThresholdBlob
        {
            get { return _threshold_blob; }
            set { _threshold_blob = value; }
        }

        private double _threshold_min_area;
        public double ThresholdMinArea
        {
            get { return _threshold_min_area; }
            set { _threshold_min_area = value; }
        }

        private string _ip_gige_camera;
        public string IP_GIGE_Camera
        {
            get { return _ip_gige_camera; }
            set { _ip_gige_camera = value; }
        }

        private int _udp_port_recieve;
        public int UdpPortRecieve
        {
            get { return _udp_port_recieve; }
            set { _udp_port_recieve = value; }
        }

        private int _udp_port_send;
        public int UdpPortSend
        {
            get { return _udp_port_send; }
            set { _udp_port_send = value; }
        }

        private string _ip_kv1000spcam2;
        public string IP_KV1000SpCam2
        {
            get { return _ip_kv1000spcam2; }
            set { _ip_kv1000spcam2 = value; }
        }
        
        private int _udp_port_kv1000spcam2;
        public int UdpPortKV1000SpCam2
        {
            get { return _udp_port_kv1000spcam2; }
            set { _udp_port_kv1000spcam2 = value; }
        }

        private string _ip_kv1000;
        public string IP_KV1000
        {
            get { return _ip_kv1000; }
            set { _ip_kv1000 = value; }
        }

        private int _udp_port_kv1000;
        public int UdpPortKV1000
        {
            get { return _udp_port_kv1000; }
            set { _udp_port_kv1000 = value; }
        }

        private string _ip_mtmon;
        public string IP_MtMon
        {
            get { return _ip_mtmon; }
            set { _ip_mtmon = value; }
        }

        private int _udp_port_mtmon;
        public int UdpPortMtMon
        {
            get { return _udp_port_mtmon; }
            set { _udp_port_mtmon = value; }
        }

        //ringbuf
        // file ID
        private int _no_cap_dev;
        public int NoCapDev
        {
            get { return _no_cap_dev; }
            set { _no_cap_dev = value; }
        }
        // Save dir
        private string _save_dir;
        public string SaveDir
        {
            get { return _save_dir; }
            set { _save_dir = value; }
        }
        // 過去画像保存枚数　０ならなし。
        private int _pre_save_num;
        public int PreSaveNum
        {
            get { return _pre_save_num; }
            set { _pre_save_num = value; }
        }
 
        // Default value
        public Settings()
        {
            _text   = "IDS UI-2410SE-M";
            _id     = 4;          //ID 全カメラの中のID　保存ファルイの識別にも使用。FishEye:0  MT3Wide:4  MT3Fine:8  MT3SF:12 等々
            _camera_type = "IDS"; //カメラタイプ： IDS Basler AVT IS analog
            _camera_id = 2;       //カメラタイプ毎のID
            _camera_color = Camera_Color.mono;    // 0:mono(mono8)  1:color 2:mono12packed
            _camera_interface = Camera_Interface.USB2 ;
            _platform = Platform.MT2;
            FlipOn = false;
            _flip_mode = OpenCvSharp.FlipMode.XY; // XY:負値　は回転させない。X,Yのみ有効
            _ip_gige_camera = "192.168.1.150"; //GIGE Camera only.
            Width = 640;
            Height = 480;
            _focal_length = 12.5;      //[mm]
            _ccdpx = 0.0074; //[mm]
            _ccdpy = 0.0074; //[mm]
            _xoa = Width / 2;
            _yoa = Height / 2;
            _roa = 40;
            _theta = 0;
            _pixel_clock = 43;//[MHz] int
            _framerate = 75.0; //[fps]
            _fifo_max_frame = 64;
            _exposure = 13; //[ms]
            _gain = 100;
            _use_detect = true;
            _threshold_blob = 128;     // 検出閾値（０－２５５）
            _threshold_min_area = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            _udp_port_recieve = 24410;
            _udp_port_send = 24429;
            _ip_kv1000spcam2 = "192.168.1.204";
            _udp_port_kv1000spcam2 = 24426;
            _ip_kv1000 = "192.168.1.10";
            _udp_port_kv1000 = 8503;
            _ip_mtmon = "192.168.1.211";
            _udp_port_mtmon = 24415;
            _no_cap_dev = _id;
            _save_dir = @"C:\Users\Public\img_data\";
            _pre_save_num = 0;
            _ueye_shutter_mode = uEye_Shutter_Mode.Global;
        }

     }
}
>>>>>>> 6c94530982d70e3b8ebf9dc7387fe48eba22ba50
