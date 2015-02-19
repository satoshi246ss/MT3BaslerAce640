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

        private int _camera_id;
        public int CameraID
        {
            get { return _camera_id; }
            set { _camera_id = value; }
        }

        private int _camera_color;
        public int CameraColor
        {
            get { return _camera_color; }
            set { _camera_color = value; }
        }
       
        //CCD
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private double _fl;
        public double Fl
        {
            get { return _fl; }
            set { _fl = value; }
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
 
        private int _fifo_max_frame;
        public int FifoMaxFrame
        {
            get { return _fifo_max_frame; }
            set { _fifo_max_frame = value; }
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

        public Settings()
        {
            _text   = "IDS UI-2410SE-M";
            _id     = 4;
            _camera_id = 2;
            _camera_color = 0; // 0:mono  1:color
            _width  = 640;
            _height = 480;
            _fl = 12.5;      //[mm]
            _ccdpx = 0.0074; //[mm]
            _ccdpy = 0.0074; //[mm]
            _framerate = 75.0; //[fps]
            _fifo_max_frame = 64;
            _exposure = 13; //[ms]
            _gain = 100;
            _threshold_blob = 128;     // 検出閾値（０－２５５）
            _threshold_min_area = 0.25;// 最小エリア閾値（最大値ｘ_threshold_min_area)
            _udp_port_recieve = 24410;
            _udp_port_send = 24429;
            _ip_kv1000spcam2="192.168.1.204"
            _udp_port_kv1000spcam2 = 24410;

        }

     }
}
