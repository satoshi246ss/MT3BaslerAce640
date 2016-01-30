using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using TIS.Imaging;

namespace MT3
{
    partial class Form1 // ImagingSouce
    {
        public void ImagingSouce_cam_start()
        {
            if (cam_maker == Camera_Maker.ImagingSouce)
            {
                // 有効な画像取り込みデバイスが選択されているかをチェック。
                if (!icImagingControl1.DeviceValid)
                {
                    //icImagingControl1.ShowDeviceSettingsDialog();

                    if (!icImagingControl1.DeviceValid)
                    {
                        MessageBox.Show("Start Live Video failed. No device was selected.");
                        return;
                    }
                }
                sw_fr.Start(); // framerate用
                icImagingControl1.LiveStart();
                icImagingControl1.LiveCaptureContinuous = true;
                icImagingControl1.LiveDisplay = false;
            }
        }

        public void ImagingSouce_cam_end()
        {
            //ImaginSouse
            if (cam_maker == Camera_Maker.ImagingSouce)
            {
                if (icImagingControl1.LiveVideoRunning == true)
                    icImagingControl1.LiveStop();
            }
        }
        
        
        // ImagingSouce frame Capture event.
        private void icImagingControl1_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e)
        {
            elapsed_fr0 = elapsed_fr1;
            elapsed_fr1 = sw_fr.ElapsedTicks; // 0.1ms
            CurrentBuffer = icImagingControl1.ImageBuffers[e.bufferIndex];
            CurrentBuffer.Lock();
            CopyMemory(imgdata.img.ImageDataOrigin, CurrentBuffer.GetImageDataPtr(), imgdata.img.ImageSize);
            CurrentBuffer.Unlock();

            //検知処理
            detect();
            imgdata_push_FIFO();

            //framerate
            dFramerate = cal_framerate(elapsed_fr1 - elapsed_fr0, dFramerate);
        }
        public double cal_framerate(long elapsed, double framerate_pre, double alfa_fr = 0.999)
        {
            return alfa_fr * framerate_pre + (1 - alfa_fr) * (Stopwatch.Frequency / (double)elapsed);
        }
        public int get_CountOfFramesDropped()
        {
            return icImagingControl1.DeviceCountOfFramesDropped;
        }
        public int get_CountOfFramesNotDropped()
        {
            return icImagingControl1.DeviceCountOfFramesNotDropped;
        }
        public int get_ImagingSouceGain()
        {
            TIS.Imaging.VCDPropertyItem Gain = null;
            Gain = icImagingControl1.VCDPropertyItems.FindItem(TIS.Imaging.VCDIDs.VCDID_Gain);

            TIS.Imaging.VCDRangeProperty GainRange;
            TIS.Imaging.VCDSwitchProperty GainSwitch;
            // Acquire interfaces to the range and switch interface for value and auto
            GainRange = (TIS.Imaging.VCDRangeProperty)Gain.Elements.FindInterface(
                                                                TIS.Imaging.VCDIDs.VCDElement_Value + ":" +
                                                                TIS.Imaging.VCDIDs.VCDInterface_Range);
            GainSwitch = (TIS.Imaging.VCDSwitchProperty)Gain.Elements.FindInterface(
                                                                TIS.Imaging.VCDIDs.VCDElement_Auto + ":" +
                                                                TIS.Imaging.VCDIDs.VCDInterface_Switch);
            if (GainSwitch == null)
            {
                MessageBox.Show("Automation of Gain is not supported by the current device!");
            }
            return GainRange.Value;
        }
        public int get_ImagingSouceExpo()
        {
            TIS.Imaging.VCDPropertyItem Expo = null;
            Expo = icImagingControl1.VCDPropertyItems.FindItem(TIS.Imaging.VCDIDs.VCDID_Exposure);

            TIS.Imaging.VCDRangeProperty ExpoRange;
            TIS.Imaging.VCDSwitchProperty ExpoSwitch;
            // Acquire interfaces to the range and switch interface for value and auto
            ExpoRange = (TIS.Imaging.VCDRangeProperty)Expo.Elements.FindInterface(
                                                                TIS.Imaging.VCDIDs.VCDElement_Value + ":" +
                                                                TIS.Imaging.VCDIDs.VCDInterface_Range);
            ExpoSwitch = (TIS.Imaging.VCDSwitchProperty)Expo.Elements.FindInterface(
                                                                TIS.Imaging.VCDIDs.VCDElement_Auto + ":" +
                                                                TIS.Imaging.VCDIDs.VCDInterface_Switch);
            if (ExpoSwitch == null)
            {
                MessageBox.Show("Automation of exposure is not supported by the current device!");
            }
            return ExpoRange.Value ;
        }

//        TIS.Imaging.VCDPropertyItem Brightness = null;
//Brightness = icImagingControl1.VCDPropertyItems.FindItem(TIS.Imaging.VCDIDs.VCDID_Brightness);
    }
}
