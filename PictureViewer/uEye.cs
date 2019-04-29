﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenCvSharp;

namespace MT3
{
    partial class Form1 //uEye
    {
        /// <summary>
        /// 画像表示ルーチン
        /// </summary>
        /// <param name="capacity">画像表示用タイマールーチン</param>
        private void ids_timerDisplay_Tick(object sender, EventArgs e)
        {
            //uEye.Camera Camera = sender as uEye.Camera;
            //      if (this.States == STOP || cam == null) return;

            // IDS　表示ルーチン（多分高速）
            //Int32 s32MemID;
            //if (cam.Memory.GetActive(out s32MemID) == uEye.Defines.Status.SUCCESS && cam.IsOpened)
            //{
            //    cam.Display.DisplayImage.Set(s32MemID, u32DisplayID, uEye.Defines.DisplayRenderMode.Normal);//FitToWindow);
            //}
        }

        public void OpenIDScamera()//(object sender, EventArgs e)
        {
            cam = new uEye.Camera();
            // IDS camera check
            int NumberOfCameras;
            uEye.Info.Camera.GetNumberOfDevices(out NumberOfCameras);
            if (NumberOfCameras <= 0) Environment.Exit(0);

            uEye.Types.CameraInformation[] cameraList;
            uEye.Info.Camera.GetCameraList(out cameraList);

            statusRet = uEye.Defines.Status.NO_SUCCESS;
            foreach (uEye.Types.CameraInformation info in cameraList)
            {
                if (info.CameraID == appSettings.CameraID)
                {
                    statusRet = uEye.Defines.Status.SUCCESS;
                }
                /*ListViewItem item = new ListViewItem();
                item.Text = info.InUse ? "No" : "Yes";
                item.ImageIndex = info.InUse ? 1 : 0;

                item.SubItems.Add(info.CameraID.ToString());
                item.SubItems.Add(info.DeviceID.ToString());
                item.SubItems.Add(info.Model);
                item.SubItems.Add(info.SerialNumber);
            */
            }

            // Open Camera
            if (statusRet == uEye.Defines.Status.SUCCESS)
            {
                statusRet = cam.Init(appSettings.CameraID);
                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    MessageBox.Show("IDS Camera initializing failed");
                    Environment.Exit(0);
                }

                if (appSettings.CameraColor == Camera_Color.mono)
                {
                    // PixelFormat MONO 8
                    set_PixelFormat_mono8();
                }
                else if (appSettings.CameraColor == Camera_Color.mono16)
                {
                    // PixelFormat MONO 16
                    set_PixelFormat_mono16();
                }

                // Allocate Memory
                statusRet = cam.Memory.Allocate(out s32MemID, true);
                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    MessageBox.Show("IDS Allocate Memory failed");
                }

                // Connect Event
                cam.EventFrame += onFrameEvent;

                // Set PC,Fr,Exp
                set_uEye_PixcelClock(appSettings.PixelClock);
                set_uEye_Exposure(appSettings.Exposure);
                set_uEye_Framerate(appSettings.Framerate);

                cam.Gain.Hardware.Scaled.SetMaster((int)appSettings.Gain);

                if (appSettings.uEyeShutterMode == uEye_Shutter_Mode.Rolling)
                {
                    set_uEye_Rolling_shutter_mode();
                }

                if (appSettings.uEye_AOI_use)
                {
                    //set_uEye_AOI(appSettings.uEye_AOI_x, appSettings.uEye_AOI_y, appSettings.uEye_AOI_w, appSettings.uEye_AOI_h);
                    set_uEye_AOI(0, appSettings.uEye_AOI_y, appSettings.Width, appSettings.uEye_AOI_h);
                    set_uEye_GainBoost(true);   // fish2 default
                    set_uEye_AutoGain(true);    //fish2
                    set_uEye_AutoShutter(true); //fish2
                    set_uEye_Framerate(appSettings.Framerate);

                    using (IplImage org_mask = new IplImage("20190211-mask.bmp", LoadMode.GrayScale))
                    {
                        org_mask.SetROI(new CvRect(appSettings.uEye_AOI_x, appSettings.uEye_AOI_y, appSettings.uEye_AOI_w, appSettings.uEye_AOI_h));
                        Cv.Copy(org_mask, img_mask);
                        org_mask.ResetROI();
                    }
                }
            }
        }
        public void set_uEye_PixcelClock(int pc)
        {
            statusRet = cam.Timing.PixelClock.Set(pc);
            if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("IDS Camera Set PixelClock failed");
        }
        public void set_uEye_Exposure(double exp_ms)
        {
            statusRet = cam.Timing.Exposure.Set(exp_ms);
            if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("IDS Camera Set Exposure failed");
        }
        public void set_uEye_Framerate(double fr)
        {
            statusRet = cam.Timing.Framerate.Set(fr);
            if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("IDS Camera Set FrameRate failed");
        }
        public void set_uEye_AutoGain(bool flag)
        {
            bool bs;
            //statusRet = cam.AutoFeatures.Software.GetEnableAutoGain(out bs);
            //if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("IDS AutoGain support failed");
            //if (bs)
            {
                statusRet = cam.AutoFeatures.Software.SetAutoReference(32); // average level (0-255) def 128
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoRef failed");
                statusRet = cam.AutoFeatures.Software.SetAutoSpeed(1); // correction speed (0-100)
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoSpeed failed");
                statusRet = cam.AutoFeatures.Software.SetAutoGainMax(100);
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoGainMax failed");
                statusRet = cam.AutoFeatures.Software.SetEnableAutoGain(flag);
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoGainEnable failed");
            }
        }
        public void set_uEye_AutoShutter(bool flag)
        {
            bool bs;
            //statusRet = cam.AutoFeatures.Software.GetEnableAutoShutter(out bs);
            //if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("IDS AutoShutter support failed");
            //if (bs)
            {
                statusRet = cam.AutoFeatures.Software.SetAutoReference(32); // average level (0-255) def 128
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoRef failed");
                statusRet = cam.AutoFeatures.Software.SetAutoSpeed(1); // correction speed (0-100)
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoSpeed failed");
                statusRet = cam.AutoFeatures.Software.SetEnableAutoShutter(flag);
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AutoShutter failed");
            }
        }

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
        }
        public void set_uEye_GainBoost(bool bgb)
        {
            bool bs;
            statusRet = cam.Gain.Hardware.Boost.GetSupported(out bs);
            if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye GainBoost failed");

            if (bs)
            {
                statusRet = cam.Gain.Hardware.Boost.SetEnable(bgb);
                if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye GainBoost failed");
            }
        }
        public void set_uEye_AOI(int px, int py, int w, int h)
        {
            statusRet = cam.Size.AOI.Set(px, py, w, h);
            if (statusRet != uEye.Defines.Status.SUCCESS) MessageBox.Show("set uEye AOI failed");
        }
        public bool check_uEye_normal_mode()
        {
            bool ans = true;
            if (appSettings.NoCapDev == 1 && appSettings.CameraType == "IDS")
            {
                double fr;
                cam.Timing.Framerate.GetCurrentFps(out fr); //IDS
                if (fr < 3.0)
                {
                    ans = false;
                }
            }
            return ans;
        }
        // 通常撮像
        // fish2 fps=60
        public void uEye_Normal_settings()
        {
            if (appSettings.NoCapDev == 1 && appSettings.CameraType == "IDS")
            {
                set_uEye_AutoGain(true);    //fish2
                set_uEye_AutoShutter(true); //fish2
                set_uEye_Framerate(appSettings.Framerate);
                //double expomin, expomax, expoinc;
                //cam.Timing.Exposure.GetRange(out expomin, out expomax, out expoinc);
                //set_uEye_Exposure(expomax);
                //pgr_SetMemory1(pgr_cam);
                //pgr_setFrameRate((float)appSettings.Framerate);
                //pgr_setEV((float)appSettings.ExposureValue);
                //pgr_setBrightness((float)0.0);
                //pgr_setShutter((float)appSettings.Exposure);
            }
        }
        
        // 保存後撮像
        // for fish2  fps=2
        public void uEye_PostSave_settings()
        {
            double post_framerate = 2.0;
            if (appSettings.NoCapDev == 1 && appSettings.CameraType == "IDS")
            {
                set_uEye_Framerate(post_framerate);//fps 2
                double expomin, expomax, expoinc;
                cam.Timing.Exposure.GetRange(out expomin, out expomax, out expoinc);
                set_uEye_Exposure(expomax);
                //pgr_setGainAuto();
                //pgr_setShutterAuto();
            }
        }


        /// <summary>
        /// 毎フレーム呼び出し use:2015/9 
        /// IDS event追加
        /// </summary>
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
            //CopyMemory(imgdata.img.ImageDataOrigin, ptr, imgdata.img.ImageSize);
            // /*
            if (appSettings.uEye_AOI_use)
            {
                // AOI x axis cut
                CopyMemory(img_ueye_aoi.ImageDataOrigin, ptr, img_ueye_aoi.ImageSize);
                img_ueye_aoi.SetROI(new CvRect(appSettings.uEye_AOI_x, 0, appSettings.uEye_AOI_w, appSettings.uEye_AOI_h));
                Cv.Copy(img_ueye_aoi, imgdata.img);
                img_ueye_aoi.ResetROI();
            }
            else
            {
                CopyMemory(imgdata.img.ImageDataOrigin, ptr, imgdata.img.ImageSize);
            }
            // */
            ///CopyMemory(img_dmk.ImageDataOrigin, ptr, img_dmk.ImageSize);
            ///Cv.Copy(img_dmk, imgdata.img);
            if (ueye_frame_number == 0) ueye_frame_number = imageInfo.FrameNumber; //frame number初期値
            elapsed0 = sw.ElapsedTicks; // 0.1ms


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
            ++frame_id;
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

                ++frame_id;
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
            fr_str = String.Format("ID:{0,5:D1} L0:{1,4:F2} L1:{2,4:F2} L2:{3,4:F2} fr:{4,5:F1}", frame_id, elapsed0 / sf, elapsed1 / sf, elapsed2 / sf, framerate0);
        }

    }
}
