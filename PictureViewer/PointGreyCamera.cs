using System;
using System.Collections.Generic;
using System.ComponentModel;//BackgroundWorker
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenCvSharp;
using FlyCapture2Managed;
//using FlyCapture2Managed.Gui;

namespace MT3
{
    partial class Form1 //PointGreyCamera
    {
        double pgr_time_pre = 0;
        double pgr_time_now = 0;
        double pgr_frame_rate = 0;
        double pgr_frame_rate_pre = 0;
        double alpha_pgr_frame_rate = 0.99;
        uint pgr_image_expo;
        uint pgr_image_gain;
        uint pgr_image_frame_count;
        bool pgr_post_save = false;

        ManagedBusManager busMgr ;
        ManagedCamera pgr_cam ;

        //        private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private AutoResetEvent m_grabThreadExited;

        static void PgrPrintBuildInfo()
        {
            FC2Version version = ManagedUtilities.libraryVersion;

            StringBuilder newStr = new StringBuilder();
            newStr.AppendFormat(
                "FlyCapture2 library version: {0}.{1}.{2}.{3}\n",
                version.major, version.minor, version.type, version.build);

            Console.WriteLine(newStr);
        }

        static void PrintCameraInfo(CameraInfo camInfo)
        {
            StringBuilder newStr = new StringBuilder();
            newStr.Append("\n*** CAMERA INFORMATION ***\n");
            newStr.AppendFormat("Serial number - {0}\n", camInfo.serialNumber);
            newStr.AppendFormat("Camera model - {0}\n", camInfo.modelName);
            newStr.AppendFormat("Camera vendor - {0}\n", camInfo.vendorName);
            newStr.AppendFormat("Sensor - {0}\n", camInfo.sensorInfo);
            newStr.AppendFormat("Resolution - {0}\n", camInfo.sensorResolution);

            Console.WriteLine(newStr);
        }

        /// <summary>
        /// コールバック関数
        /// </summary>
        /// <param name="capacity">PGRコールバック</param>
        void OnImageGrabbed(ManagedImage pgr_image)
        {
            //Console.WriteLine("Grabbed image {0} - {1}.{2}", imageCnt++, pgr_image.timeStamp.cycleSeconds, pgr_image.timeStamp.cycleCount);

            //画像データをバッファにコピー
            lock (this)
            {
                unsafe
                {
                    CopyMemory(imgdata.img.ImageDataOrigin, (IntPtr)pgr_image.data, imgdata.img.ImageSize);
                }
            }
            #region 他のコピー方法
            /*            // unsafe version　上手くいかない(2016.04.29)
            unsafe
            {
                CopyMemory(imgdata.img.ImageDataPtr, pgr_image.data, imgdata.img.ImageSize);
            }

            unsafe
            {
                int size = sizeof(Hoge);
                IntPtr ptr = Marshal.AllocHGlobal(size);
                *(Hoge*)ptr = obj;
            }

             unsafe
            {
                int size = imgdata.img.ImageSize;// sizeof(Hoge);
                byte[] bytes = new byte[size];
                fixed (byte* pbytes = bytes)
                {
                    *(imgdata.img.ImageDataPtr)pbytes = *(pgr_image.data);
                }
            }
             */
            #endregion

            //++frame_id; 
            detect(); // これをはずすと160fps
            imgdata_push_FIFO();

            TimeStamp timestamp;
            lock (this)
            {
                pgr_image_expo = (uint)((pgr_image.imageMetadata.embeddedShutter & 65535) * 4.883); // 1 count = 4.88268 ms
                pgr_image_gain = pgr_image.imageMetadata.embeddedGain & 65535;
                pgr_image_frame_count = pgr_image.imageMetadata.embeddedFrameCounter;
                timestamp = pgr_image.timeStamp;
            }
            frame_id = (int)pgr_image_frame_count ;
            pgr_time_now = timestamp.cycleSeconds + timestamp.cycleCount/8000.0 ; // count 8kHz, 1394 cycle timer
            pgr_frame_rate = alpha_pgr_frame_rate * pgr_frame_rate_pre + (1 - alpha_pgr_frame_rate) * 1/(pgr_time_now - pgr_time_pre);
            pgr_frame_rate_pre = pgr_frame_rate;
            pgr_time_pre = pgr_time_now;
        }

        public void InitPGR()
        {
            m_rawImage = new ManagedImage();
            m_processedImage = new ManagedImage();
            //m_camCtlDlg = new CameraControlDialog();
            m_grabThreadExited = new AutoResetEvent(false);
        }

        public void OpenPGRcamera()//(object sender, EventArgs e)
        {
            busMgr = new ManagedBusManager();
            pgr_cam = new ManagedCamera();
            //busMgr.RescanBus();

            uint numCameras = busMgr.GetNumOfCameras();
            if (numCameras > 0)
            {
                uint pgr_cam_num = 0;
                ManagedPGRGuid guid = busMgr.GetCameraFromIndex(pgr_cam_num);
                RunSingleCamera(guid);
            }
            else
            {
                MessageBox.Show("PGR Camera initializing failed");
                Environment.Exit(0);
            }
        }

        public void ClosePGRcamera()//(object sender, EventArgs e)
        {
            // Stop capturing images
            pgr_cam.StopCapture();

            // Disconnect the camera
            pgr_cam.Disconnect();           
        }

        // リクエストフレームレート
        public float pgr_getFrameRate()
        {
            CameraProperty frameRateProp = pgr_cam.GetProperty(PropertyType.FrameRate);
            return(frameRateProp.absValue);
        }
        public void pgr_setFrameRate(float fr)
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.FrameRate;
            prop.autoManualMode = false;
            prop.absControl = true;
            prop.absValue = fr;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        // Gain Auto
        public void pgr_setGainAuto()
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.Gain;
            prop.autoManualMode = true;
            prop.absControl = true;
            //prop.absValue = expo_ms;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        // Exposure Auto
        public void pgr_setShutterAuto()
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.Shutter;
            prop.autoManualMode = true;
            prop.absControl = true;
            //prop.absValue = expo_ms;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        public void pgr_setShutter(float expo_ms)
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.Shutter;
            prop.autoManualMode = false;
            prop.absControl = true;
            prop.absValue = expo_ms;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }

        // set Brightness 
        public void pgr_setBrightness(float br)
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.Brightness;
            prop.autoManualMode = false;
            prop.absControl = true;
            prop.absValue = br;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        // set ganma 
        public void pgr_setGamma(float gamma)
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.Gamma;
            prop.autoManualMode = false;
            prop.absControl = true;
            prop.absValue = gamma;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        // set EV 
        public void pgr_setEV(float ev)
        {
            //Declare a Property struct. 
            CameraProperty prop = new CameraProperty();
            prop.type = PropertyType.AutoExposure;
            prop.autoManualMode = false;
            prop.absControl = true;
            prop.absValue = ev;
            prop.onOff = true;

            pgr_cam.SetProperty(prop);
        }
        // get EV 
        public float pgr_getEV()
        { 
            return pgr_get_property(PropertyType.AutoExposure);
        }
        // get camera property
        public float pgr_get_property(PropertyType pt )
        {
            //Declare a Property struct. 
            CameraProperty prop = pgr_cam.GetProperty( pt );
            return (prop.absValue);
        }

//Enumerator: 
//S100  100Mbits/sec. 
//S200  200Mbits/sec. 
        //S400  400Mbits/sec. 
        //S480  480Mbits/sec.   //Only for USB2 cameras.
 
//S800  800Mbits/sec. 
//S1600  1600Mbits/sec. 
//S3200  3200Mbits/sec. 
//S5000  5000Mbits/sec. 
//
//Only for USB3 cameras. 
// 
//GigE_10Base_T   
//GigE_100Base_T   
//GigE_1000Base_T   
//GigE_10000Base_T   
//Fastest  The fastest speed available. 
//Any  Any speed that is available.  
//Unknown  Unknown interface. 

        public int pgr_BusSpeed()
        {
            CameraInfo camInfo = pgr_cam.GetCameraInfo();
            return ( (int)camInfo.maximumBusSpeed );
        }

        public void ReScanBus(ManagedBusManager _busMgr)
        {
            // Connect to a camera
            _busMgr.RescanBus();
        }

        public uint pgr_Temperature(ManagedCamera _cam)
        {
            const uint k_addr = 0x82C;
            const uint k_eVal = 0xFFF;
            uint regVal = _cam.ReadRegister(k_addr);
            return (regVal & k_eVal);
        }
        public uint pgr_PixelClockFreq(ManagedCamera _cam)        
        {
            const uint k_addr = 0x1AF0;
            uint regVal = _cam.ReadRegister(k_addr);
            return (regVal);
        }
        // Power on the camera
        public void pgr_PowerOnCamera(ManagedCamera _cam)
        {
            const uint k_cameraPower = 0x610;
            const uint k_powerVal = 0x80000000;
            _cam.WriteRegister(k_cameraPower, k_powerVal);

            const Int32 k_millisecondsToSleep = 100;
            uint regVal = 0;

            // Wait for camera to complete power-up
            do
            {
                System.Threading.Thread.Sleep(k_millisecondsToSleep);
                regVal = _cam.ReadRegister(k_cameraPower);
            } while ((regVal & k_powerVal) == 0);
        }
        // Power off the camera
        public void pgr_PowerOffCamera(ManagedCamera _cam)
        {
            const uint k_cameraPower = 0x610;
            const uint k_powerVal = 0x00000000;
            _cam.WriteRegister(k_cameraPower, k_powerVal);

            const Int32 k_millisecondsToSleep = 100;
            uint regVal = 0;

            // Wait for camera to complete power-down
            do
            {
                System.Threading.Thread.Sleep(k_millisecondsToSleep);
                regVal = _cam.ReadRegister(k_cameraPower);
            } while (regVal != 0);
        }
        // Memory Set 1
        public void pgr_SetMemory1(ManagedCamera _cam)
        {
            const uint k_cur_mem_ch = 0x624;
            const uint k_Val = 0x10000000;
            _cam.WriteRegister(k_cur_mem_ch, k_Val);
        }

        // 通常撮像
        public void pgr_Normal_settings()
        {
            pgr_SetMemory1(pgr_cam);
            pgr_setFrameRate((float)appSettings.Framerate);
            pgr_setEV((float)appSettings.ExposureValue);
            pgr_setBrightness((float)0.0);
            pgr_setShutter((float)appSettings.Exposure);
        }
        // 保存後撮像
        public void pgr_PostSave_settings()
        {
            pgr_setFrameRate((float)1.0);
            pgr_setShutter((float)1000);
            pgr_setGainAuto();
            pgr_setShutterAuto();
        }

        void RunSingleCamera(ManagedPGRGuid guid)
        {
            // Connect to a camera
            pgr_cam.Connect(guid);

            pgr_PowerOnCamera(pgr_cam);

            // Get the camera information
            CameraInfo camInfo = pgr_cam.GetCameraInfo();
            PrintCameraInfo(camInfo);

            // 通常撮像
            pgr_Normal_settings();
 
            // Get embedded image info from camera
            EmbeddedImageInfo embeddedInfo = pgr_cam.GetEmbeddedImageInfo();

            // Enable timestamp collection	
            if (embeddedInfo.timestamp.available == true)
            {
                embeddedInfo.timestamp.onOff = true;
            }
            // Enable exposure collection	
            if (embeddedInfo.exposure.available == true)
            {
                embeddedInfo.exposure.onOff = true;
            }
            // Enable shutter collection	
            if (embeddedInfo.shutter.available == true)
            {
                embeddedInfo.shutter.onOff = true;
            }
            // Enable gain collection	
            if (embeddedInfo.gain.available == true)
            {
                embeddedInfo.gain.onOff = true;
            }
            // Enable frameCounter collection	
            if (embeddedInfo.frameCounter.available == true)
            {
                embeddedInfo.frameCounter.onOff = true;
            }

            // Set embedded image info to camera
            pgr_cam.SetEmbeddedImageInfo(embeddedInfo);

            CameraProperty frameRateProp = pgr_cam.GetProperty(PropertyType.FrameRate);
            Console.WriteLine( frameRateProp.absValue );

            FC2Config fc2conf = pgr_cam.GetConfiguration();
            Console.WriteLine(fc2conf.isochBusSpeed);

            // Start capturing images
            pgr_cam.StartCapture(OnImageGrabbed);
        }


        /// <summary>
        /// 画像表示ルーチン
        /// </summary>
        /// <param name="capacity">画像表示用タイマールーチン</param>
     //   private void ids_timerDisplay_Tick(object sender, EventArgs e)
     //   {
            //uEye.Camera Camera = sender as uEye.Camera;
            //      if (this.States == STOP || cam == null) return;

            // IDS　表示ルーチン（多分高速）
            //Int32 s32MemID;
            //if (cam.Memory.GetActive(out s32MemID) == uEye.Defines.Status.SUCCESS && cam.IsOpened)
            //{
            //    cam.Display.DisplayImage.Set(s32MemID, u32DisplayID, uEye.Defines.DisplayRenderMode.Normal);//FitToWindow);
            //}
     //   }
        /*
              /// <summary>
                /// シャッターモードをローリングシャッターに設定
                /// </summary>
                public void set_uEye_Rolling_shutter_mode()
                {
                    if (cam.DeviceFeature.ShutterMode.IsSupported(uEye.Defines.Shuttermode.Rolling))
                    {
                        cam.DeviceFeature.ShutterMode.Set(uEye.Defines.Shuttermode.Rolling);
                    }

                }
                /// <summary>
                /// シャッターモードをグローバルシャッターに設定
                /// </summary>
                public void set_uEye_Global_shutter_mode()
                {
                    if (cam.DeviceFeature.ShutterMode.IsSupported(uEye.Defines.Shuttermode.Global))
                    {
                        cam.DeviceFeature.ShutterMode.Set(uEye.Defines.Shuttermode.Global);
                    }

                }
                public void set_PixelFormat_mono8()
                {
                    // PixelFormat MONO 8
                    uEye.Defines.ColorMode mode;
                    cam.PixelFormat.Get(out mode);
                    cam.PixelFormat.Set(uEye.Defines.ColorMode.MONO8);
                    cam.PixelFormat.Get(out mode);
                }        //
                public void set_PixelFormat_mono16()
                {
                    // PixelFormat MONO 16
                    uEye.Defines.ColorMode mode;
                    cam.PixelFormat.Get(out mode);
                    cam.PixelFormat.Set(uEye.Defines.ColorMode.MONO16);
                    cam.PixelFormat.Get(out mode);
                }        //
                // 毎フレーム呼び出し use:2015/9 
                // IDS event追加
                private void onFrameEvent(object sender, EventArgs e)
                {
                    sw.Start();
                    Int32 s32MemID;
                    cam.Memory.GetActive(out s32MemID);
                    System.IntPtr ptr;

                    //画像データをバッファにコピー
                    cam.Memory.Lock(s32MemID);
                    cam.Information.GetImageInfo(s32MemID, out imageInfo);
                    cam.Memory.ToIntPtr(s32MemID, out ptr);
                    CopyMemory(imgdata.img.ImageDataOrigin, ptr, imgdata.img.ImageSize);
                    ///CopyMemory(img_dmk.ImageDataOrigin, ptr, img_dmk.ImageSize);
                    ///Cv.Copy(img_dmk, imgdata.img);
                    if (ueye_frame_number == 0) ueye_frame_number = imageInfo.FrameNumber; //frame number初期値
                    elapsed0 = sw.ElapsedTicks; // 0.1ms

                    //3  Cv.Copy(img_dmk3, imgdata.img);
                    //Cv.Copy(img_dmk, imgdata.img);

                    //img_dmk3.ImageData = ptr;
                    //Cv.CvtColor(img_dmk3, imgdata.img, ColorConversion.BgrToGray); // 遅い er:2.6%
                    //Cv.Split(img_dmk3, imgdata.img, null,null,null); // er:1.1%
                    cam.Memory.Unlock(s32MemID);

                    ++frame_id;
                    detect();
                    imgdata_push_FIFO();
                }

                // IDS event追加
                private void onFrameEvent1(object sender, EventArgs e)
                {
                    uEye.Camera Camera = sender as uEye.Camera;

                    Int32 s32MemID;
                    Camera.Memory.GetActive(out s32MemID);

                    Camera.Display.DisplayImage.Set(s32MemID, u32DisplayID, uEye.Defines.DisplayRenderMode.FitToWindow);
                    ++id;
                }
 
                // 毎フレーム呼び出し(120fr/s)
                // IDS event追加
                private void onFrameEvent2(object sender, EventArgs e)
                {
                    sw.Start();
                    double framerate0 = 0, framerate1 = 0;//, alfa_fr = 0.99;
                    Int32 s32MemID;
                    cam.Memory.GetActive(out s32MemID);
                    try
                    {
                        System.IntPtr ptr;
                        cam.Memory.Lock(s32MemID);
                        cam.Information.GetImageInfo(s32MemID, out imageInfo);
                        cam.Memory.ToIntPtr(s32MemID, out ptr);
                        CopyMemory(img_dmk3.ImageDataOrigin, ptr, img_dmk3.ImageSize);
                        Cv.CvtColor(img_dmk3, img_dmk, ColorConversion.BgrToGray);
                        cam.Memory.Unlock(s32MemID);

                        //Cv.Copy(img_dmk, img2, null);

                        ++id;
                        elapsed0 = sw.ElapsedTicks;

                        // 保存用データをキューへ
                        if (ImgSaveFlag == TRUE)
                        {
                            double min_val, max_val;
                            CvPoint min_loc, max_loc;
                            int size = 15;
                            int size2x = size / 2;
                            int size2y = size / 2;
                            //int num    = 0;
                            double sigma = 3;

                            // 位置検出
                            Cv.Smooth(img_dmk, img2, SmoothType.Gaussian, size, 0, sigma, 0);
                            CvRect rect = new CvRect(1, 1, appSettings.Width - 2, appSettings.Height - 2);
                            Cv.SetImageROI(img2, rect);
                            Cv.MinMaxLoc(img2, out  min_val, out  max_val, out  min_loc, out  max_loc, null);
                            Cv.ResetImageROI(img2);
                            max_loc.X += 1; // 基準点が(1,1)のため＋１
                            max_loc.Y += 1;

                            double m00, m10, m01;
                            if (max_loc.X - size2x < 0) size2x = max_loc.X;
                            if (max_loc.Y - size2y < 0) size2y = max_loc.Y;
                            if (max_loc.X + size2x >= appSettings.Width ) size2x = appSettings.Width  - max_loc.X - 1;
                            if (max_loc.Y + size2y >= appSettings.Height) size2y = appSettings.Height - max_loc.Y - 1;
                            rect = new CvRect(max_loc.X - size2x, max_loc.Y - size2y, size, size);
                            CvMoments moments;
                            Cv.SetImageROI(img2, rect);
                            Cv.Moments(img2, out moments, false);
                            Cv.ResetImageROI(img2);
                            m00 = Cv.GetSpatialMoment(moments, 0, 0);
                            m10 = Cv.GetSpatialMoment(moments, 1, 0);
                            m01 = Cv.GetSpatialMoment(moments, 0, 1);
                            gx = max_loc.X - size2x + m10 / m00;
                            gy = max_loc.Y - size2y + m01 / m00;

                            //    Pid_Data_Send();
                            elapsed1 = sw.ElapsedTicks;
                            imgdata_push_FIFO();
                        }
                    }
                    catch (Exception ex)
                    {
                        //匿名デリゲートで表示する
                        this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                    finally
                    {
                    }
                    elapsed2 = sw.ElapsedTicks; sw.Stop(); sw.Reset();
                    // 処理速度
                    //  Double dFramerate;
                    //  cam.Timing.Framerate.GetCurrentFps(out dFramerate);
                    //  toolStripStatusLabelFramerate.Text = "Fps: " + dFramerate.ToString("00.00");

                    //framerate0 = alfa_fr * framerate1 + (1 - alfa_fr) * (Stopwatch.Frequency / (double)elapsed2);
                    //  framerate0 = ++id_fr / (this.icImagingControl1.ReferenceTimeCurrent - this.icImagingControl1.ReferenceTimeStart);
                    framerate1 = framerate0;

                    double sf = (double)Stopwatch.Frequency / 1000; //msec
                    fr_str = String.Format("ID:{0,5:D1} L0:{1,4:F2} L1:{2,4:F2} L2:{3,4:F2} fr:{4,5:F1}", id, elapsed0 / sf, elapsed1 / sf, elapsed2 / sf, framerate0);
                }
                */
    }
}

