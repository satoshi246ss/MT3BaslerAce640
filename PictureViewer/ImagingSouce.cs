using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        
        
        // ImagingSouce
        private void icImagingControl1_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e)
        {
            CurrentBuffer = icImagingControl1.ImageBuffers[e.bufferIndex];
            CurrentBuffer.Lock();
            CopyMemory(imgdata.img.ImageDataOrigin, CurrentBuffer.GetImageDataPtr(), imgdata.img.ImageSize);
            CurrentBuffer.Unlock();
            //img_dmk.ImageData = CurrentBuffer.GetImageDataPtr();
            //Cv.Copy(img_dmk, imgdata.img); //画像コピー
            detect();
            imgdata_push_FIFO();
        }
    }
}
