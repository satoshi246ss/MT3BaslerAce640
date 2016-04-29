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
        static uint imageCnt = 0;
        ManagedBusManager busMgr = new ManagedBusManager();
        ManagedCamera pgr_cam = new ManagedCamera();

        //        private FlyCapture2Managed.Gui.CameraControlDialog m_camCtlDlg;
        private ManagedCameraBase m_camera = null;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;

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
            Console.WriteLine("Grabbed image {0} - {1}.{2}", imageCnt++,
                pgr_image.timeStamp.cycleSeconds,
                pgr_image.timeStamp.cycleCount);

            id++;
            //
            unsafe
            {
                CopyMemory(imgdata.img.ImageDataOrigin, (IntPtr)pgr_image.data, imgdata.img.ImageSize);
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

        void RunSingleCamera(ManagedPGRGuid guid)
        {
            // Connect to a camera
            pgr_cam.Connect(guid);
            // Get the camera information
            CameraInfo camInfo = pgr_cam.GetCameraInfo();
            PrintCameraInfo(camInfo);

            // Get embedded image info from camera
            EmbeddedImageInfo embeddedInfo = pgr_cam.GetEmbeddedImageInfo();

            // Enable timestamp collection	
            if (embeddedInfo.timestamp.available == true)
            {
                embeddedInfo.timestamp.onOff = true;
            }

            // Set embedded image info to camera
            pgr_cam.SetEmbeddedImageInfo(embeddedInfo);

            // Start capturing images
            pgr_cam.StartCapture(OnImageGrabbed);

            CameraProperty frameRateProp = pgr_cam.GetProperty(PropertyType.FrameRate);

            while (imageCnt < 1000)
            {
                int millisecondsToSleep = (int)(1000 / frameRateProp.absValue);
                Thread.Sleep(millisecondsToSleep);
            }

            // Reset counter for next iteration
            imageCnt = 0;
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

