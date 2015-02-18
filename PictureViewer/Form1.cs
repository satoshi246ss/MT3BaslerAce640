using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;
using VideoInputSharp;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using PylonC.NETSupportLibrary;

namespace MT3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timeBeginPeriod(time_period);

            //コマンドライン引数を配列で取得する
            cmds = System.Environment.GetCommandLineArgs();
            //コマンドライン引数をcheck
            if (cmds.Length != 3)
            {
                //アプリケーションを終了する
                Application.Exit();
            }
            if (cmds[1].StartsWith("/vi"))  // analog camera VideoInputを使用
            {
                cam_maker = Camera_Maker.analog;
               // cam_color = Camera_Color.mono;
            }
            if (cmds[1].StartsWith("/b")) // Basler
            {
                cam_maker = Camera_Maker.Basler;
                // cam_color = Camera_Color.mono;
            }

            worker_udp = new BackgroundWorker();
            worker_udp.WorkerReportsProgress = true;
            worker_udp.WorkerSupportsCancellation = true;
            worker_udp.DoWork += new DoWorkEventHandler(worker_udp_DoWork);
            worker_udp.ProgressChanged += new ProgressChangedEventHandler(worker_udp_ProgressChanged);

            xoa = xoa_mes;
            yoa = yoa_mes;

            // VideoInput
            if (cam_maker == Camera_Maker.analog)
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            }

            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                u32DisplayID = pictureBox1.Handle.ToInt32();
                cam = new uEye.Camera();
            }

            //Basler
            if (cam_maker == Camera_Maker.Basler)
            {
                Text = "MT3BaslerAce";
                /* Register for the events of the image provider needed for proper operation. */
                m_imageProvider.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
                m_imageProvider.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
                m_imageProvider.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
                m_imageProvider.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
                m_imageProvider.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
                m_imageProvider.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
                m_imageProvider.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);

                /* Provide the controls in the lower left area with the image provider object. */
                //     sliderExposureTime.MyImageProvider = m_imageProvider;

                /*    sliderGain.MyImageProvider = m_imageProvider;
                    sliderExposureTime.MyImageProvider = m_imageProvider;
                    sliderHeight.MyImageProvider = m_imageProvider;
                    sliderWidth.MyImageProvider = m_imageProvider;
                    comboBoxTestImage.MyImageProvider = m_imageProvider;
                    comboBoxPixelFormat.MyImageProvider = m_imageProvider;
                */

                /* Update the list of available devices in the upper left area. */
                UpdateDeviceList();
            }

            Pid_Data_Send_Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.worker_udp.RunWorkerAsync();

            // 有効な画像取り込みデバイスが選択されているかをチェック。
            /*  if (!icImagingControl1.DeviceValid)
              {
                  icImagingControl1.ShowDeviceSettingsDialog();

                  if (!icImagingControl1.DeviceValid)
                  {
                      MessageBox.Show("No device was selected.", "Display Buffer",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                      this.Close();
                      return;
                  }
              } */

        }
        //Form起動後１回だけ発生
        private void Form1_Shown(object sender, EventArgs e)
        {
            checkBoxObsAuto_CheckedChanged(sender, e);
            diskspace = cDrive.TotalFreeSpace;
            timerMTmonSend.Start();

            // IDS open
            //ShowButton.PerformClick();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //UDP停止
            worker_udp.CancelAsync();
            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                cam.Exit();
            }
            //AVT
            if (cam_maker == Camera_Maker.AVT)
            {
                avt_cam_end();
            }
            //Basler
            if (cam_maker == Camera_Maker.Basler)
            {
                BaslerEnd();
            }

            timeEndPeriod(16);
        }

        #region UDP
        // 別スレッド処理（UDP） //IP 192.168.1.214
        private void worker_udp_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            //バインドするローカルポート番号
            int localPort = mmFsiUdpPortSpCam;// 24410 broadcast
            //int localPort = mmFsiUdpPortMT3Basler; // broadcast mmFsiUdpPortMT3Basler;
            System.Net.Sockets.UdpClient udpc = null; ;
            try
            {
                udpc = new System.Net.Sockets.UdpClient(localPort);
            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }

            //文字コードを指定する
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            string str;
            MOTOR_DATA_KV_SP kmd3 = new MOTOR_DATA_KV_SP();
            int size = Marshal.SizeOf(kmd3);
            KV_DATA kd = new KV_DATA();
            int sizekd = Marshal.SizeOf(kd);


            //データを受信する
            System.Net.IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, localPort);
            while (bw.CancellationPending == false)
            {
                byte[] rcvBytes = udpc.Receive(ref remoteEP);
                if (rcvBytes.Length == sizekd)
                {
                    kd = ToStruct1(rcvBytes);
                    bw.ReportProgress(0, kd);
                }
                else if (rcvBytes.Length == size)
                {
                    kmd3 = ToStruct(rcvBytes);
                    if (kmd3.cmd == 1) //mmMove:1
                    {
                        Mode = DETECT;
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, RUN });
                        this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveTimeOver, RUN });
                        //保存処理開始
                        if (this.States == RUN)
                        {
                            ImgSaveFlag = TRUE;
                            this.States = SAVE;
                            kalman_init();
                            pos_mes.init();
                        }
                    }
                    else if (kmd3.cmd == 90) //mmPidTest:90
                    {
                        Mode = PID_TEST;
                        test_start_id = pid_data.id;
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, RUN });
                        this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveTimeOver, RUN });
                        //保存処理開始
                        if (this.States == RUN)
                        {
                            ImgSaveFlag = TRUE;
                            this.States = SAVE;
                        }
                    }
                    else if (kmd3.cmd == 16) //mmLost:16
                    {
                        //Mode = LOST;
                        //ButtonSaveEnd_Click(sender, e);
                        //匿名デリゲートで表示する
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, STOP });
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSavePostTime, RUN });
                    }
                    else if (kmd3.cmd == 17) // mmMoveEnd             17  // 位置決め完了
                    {
                        Mode = DETECT_IN;
                    }
                    else if (kmd3.cmd == 18) // mmTruckEnd            18  // 追尾完了
                    {
                        //保存処理終了
                        timerSaveTimeOver.Stop();
                        Mode = LOST;
                        ButtonSaveEnd_Click(sender, e);
                    }
                    else if (kmd3.cmd == 20) //mmData  20  // send fish pos data
                    {
                        //匿名デリゲートで表示する
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, STOP });
                        //this.Invoke(new dlgSetColor(SetTimer), new object[] { timerSaveMainTime, RUN }); // main timer 延長
                    }

                    str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UDP " + kmd3.cmd.ToString("CMD:00") + " Az:" + kmd3.az + " Alt:" + kmd3.alt + " VAz:" + kmd3.vaz + " VAlt:" + kmd3.valt + "\n";
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
                    //bw.ReportProgress(0, kmd3);
                }
                else
                {
                    string rcvMsg = enc.GetString(rcvBytes);
                    str = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "受信したデータ:[" + rcvMsg + "]\n";
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
                }

                //str = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "送信元アドレス:{0}/ポート番号:{1}/Size:{2}\n" + remoteEP.Address + "/" + remoteEP.Port + "/" + rcvBytes.Length;
                //this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, str });
            }

            //UDP接続を終了
            udpc.Close();
        }
        //メインスレッドでの処理
        private void worker_udp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 画面表示
            if ((id % 1) == 0)
            {
                //MOTOR_DATA_KV_SP kmd3 = (MOTOR_DATA_KV_SP)e.UserState;
                //string s = string.Format("worker_udp_ProgressChanged:[{0} {1} az:{2} alt:{3}]\n", kmd3.cmd, kmd3.t, kmd3.az, kmd3.alt);
                //  richTextBox1.AppendText(s);
                udpkv.kd = (KV_DATA)e.UserState;
                udpkv.cal_mt3();

                string s = string.Format("KV:[x2:{0:D6} y2:{1:D6} x2v:{2:D5} y2v:{3:D5} {4} {5}]\n", udpkv.x2pos, udpkv.y2pos, udpkv.x2v, udpkv.y2v, udpkv.binStr_status, udpkv.binStr_request);
                label_X2Y2.Text = s;
            }
        }

        static byte[] ToBytes(MOTOR_DATA_KV_SP obj)
        {
            int size = Marshal.SizeOf(typeof(MOTOR_DATA_KV_SP));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }
        static byte[] ToBytes(FSI_PID_DATA obj)
        {
            int size = Marshal.SizeOf(typeof(FSI_PID_DATA));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static MOTOR_DATA_KV_SP ToStruct(byte[] bytes)
        {
            GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            MOTOR_DATA_KV_SP result = (MOTOR_DATA_KV_SP)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(MOTOR_DATA_KV_SP));
            gch.Free();
            return result;
        }

        public static KV_DATA ToStruct1(byte[] bytes)
        {
            GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            KV_DATA result = (KV_DATA)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(KV_DATA));
            gch.Free();
            return result;
        }


        #endregion

        #region キャプチャー
        // 別スレッド処理（キャプチャー）
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            Stopwatch sw = new Stopwatch();
            string str;
            id = 0;

            //PID送信用UDP
            //バインドするローカルポート番号
            FSI_PID_DATA pid_data = new FSI_PID_DATA();
            int localPort = 24406;
            System.Net.Sockets.UdpClient udpc2 = null; ;
            try
            {
                udpc2 = new System.Net.Sockets.UdpClient(localPort);

            }
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            }

            //videoInputオブジェクト
            const int DeviceID = 0;// 0;      // 3 (pro), 4(piccolo)  7(DMK)
            const int CaptureFps = 30;  // 30
            int interval = (int)(1000 / CaptureFps / 10);
            const int CaptureWidth = 640;
            const int CaptureHeight = 480;
            // 画像保存枚数
            int mmFsiPostRec = 60;
            int save_counter = mmFsiPostRec;

            using (VideoInput vi = new VideoInput())
            {
                vi.SetIdealFramerate(DeviceID, CaptureFps);
                vi.SetupDevice(DeviceID, CaptureWidth, CaptureHeight);

                int width = vi.GetWidth(DeviceID);
                int height = vi.GetHeight(DeviceID);

                using (IplImage img = new IplImage(width, height, BitDepth.U8, 3))
                using (IplImage img_dark8 = Cv.LoadImage(@"C:\piccolo\MT3V_dark.bmp", LoadMode.GrayScale))
                //using (IplImage img_dark = new IplImage(width, height, BitDepth.U8, 3))
                using (IplImage img_mono = new IplImage(width, height, BitDepth.U8, 1))
                using (IplImage img2 = new IplImage(width, height, BitDepth.U8, 1))
                //                    using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                using (CvFont font = new CvFont(FontFace.HersheyComplex, 0.5, 0.5))
                using (CvWindow window0 = new CvWindow("FIFO0", WindowMode.AutoSize))
                {
                    this.Size = new Size(width + 12, height + 148);
                    double min_val, max_val;
                    CvPoint min_loc, max_loc;
                    int size = 15;
                    int size2x = size / 2;
                    int size2y = size / 2;
                    //int num = 0;
                    double sigma = 3;
                    long elapsed0 = 0, elapsed1 = 0;
                    double framerate0 = 0, framerate1 = 0;
                    double alfa_fr = 0.999;
                    sw.Start();
                    while (bw.CancellationPending == false)
                    {
                        if (vi.IsFrameNew(DeviceID))
                        {
                            DateTime dn = DateTime.Now; //取得時刻
                            vi.GetPixels(DeviceID, img.ImageData, false, true);
                            //str = String.Format("ID:{0}", id);
                            //img.PutText(str, new CvPoint(10, 450), font, new CvColor(0, 255, 100));
                            Cv.CvtColor(img, img_mono, ColorConversion.BgrToGray);
                            Cv.Sub(img_mono, img_dark8, imgdata.img); // dark減算
                            imgdata.id = ++id;
                            imgdata.t = dn;
                            imgdata.ImgSaveFlag = !(ImgSaveFlag != 0); //int->bool変換
                            if (fifo.Count == MaxFrame - 1) fifo.EraseLast();
                            fifo.InsertFirst(imgdata);
                            // 位置検出
                            Cv.Smooth(imgdata.img, img2, SmoothType.Gaussian, size, 0, sigma, 0);
                            CvRect rect = new CvRect(1, 1, width - 2, height - 2);
                            Cv.SetImageROI(img2, rect);
                            Cv.MinMaxLoc(img2, out  min_val, out  max_val, out  min_loc, out  max_loc, null);
                            Cv.ResetImageROI(img2);
                            max_loc.X += 1; // 基準点が(1,1)のため＋１
                            max_loc.Y += 1;
                            window0.ShowImage(img2);

                            double m00, m10, m01, gx, gy;
                            size2x = size2y = size / 2;
                            if (max_loc.X - size2x < 0) size2x = max_loc.X;
                            if (max_loc.Y - size2y < 0) size2y = max_loc.Y;
                            if (max_loc.X + size2x >= width) size2x = width - max_loc.X - 1;
                            if (max_loc.Y + size2y >= height) size2y = height - max_loc.Y - 1;
                            rect = new CvRect(max_loc.X - size2x, max_loc.Y - size2y, size2x, size2y);
                            CvMoments moments;
                            Cv.SetImageROI(img2, rect);
                            Cv.Moments(img2, out moments, false);
                            Cv.ResetImageROI(img2);
                            m00 = Cv.GetSpatialMoment(moments, 0, 0);
                            m10 = Cv.GetSpatialMoment(moments, 1, 0);
                            m01 = Cv.GetSpatialMoment(moments, 0, 1);
                            gx = max_loc.X - size2x + m10 / m00;
                            gy = max_loc.Y - size2y + m01 / m00;

                            // 画面表示
                            str = String.Format("ID:{0:D2} ", id) + dn.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,000:F2},{1,000:F2}) ({2,000:0},{3,000:0})({4,0:F1})", gx, gy, max_loc.X, max_loc.Y, max_val);
                            if (imgdata.ImgSaveFlag) str += " True";
                            img.PutText(str, new CvPoint(10, 20), font, new CvColor(0, 255, 100));
                            img.Circle(max_loc, 10, new CvColor(255, 255, 100));
                            bw.ReportProgress(0, img);

                            // PID data send for UDP
                            if (ImgSaveFlag == TRUE)
                            {
                                //データを送信するリモートホストとポート番号
                                string remoteHost = mmFsiSC440;
                                int remotePort = mmFsiUdpPortSpCam;
                                //送信するデータを読み込む
                                ++(pid_data.id);
                                pid_data.swid = 24402;          // 仮　mmFsiUdpPortFSI2
                                pid_data.t = TDateTimeDouble(DateTime.Now);
                                pid_data.dx = (float)(gx - xoa);
                                pid_data.dy = (float)(gy - yoa);
                                byte[] sendBytes = ToBytes(pid_data);
                                //リモートホストを指定してデータを送信する
                                udpc2.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
                            }

                            // 処理速度
                            elapsed0 = sw.ElapsedTicks - elapsed1; // 1frameのticks
                            elapsed1 = sw.ElapsedTicks;
                            framerate0 = alfa_fr * framerate1 + (1 - alfa_fr) * (Stopwatch.Frequency / (double)elapsed0);
                            framerate1 = framerate0;

                            str = String.Format("fr time = {0}({1}){2:F1}", sw.Elapsed, id, framerate0); //," ", sw.ElapsedMilliseconds);
                            //匿名デリゲートで現在の時間をラベルに表示する
                            this.Invoke(new dlgSetString(ShowText), new object[] { textBox1, str });
                            //img.ToBitmap(bitmap);
                            //pictureBox1.Refresh();

                        }
                        Application.DoEvents();
                        Thread.Sleep(interval);
                    }
                    this.States = STOP;
                    this.Invoke(new dlgSetColor(SetColor), new object[] { ObsStart, this.States });
                    this.Invoke(new dlgSetColor(SetColor), new object[] { ObsEndButton, this.States });
                    vi.StopDevice(DeviceID);
                    udpc2.Close();
                }
            }
        }
        //BCB互換TDatetime値に変換
        private double TDateTimeDouble(DateTime t)
        {
            TimeSpan ts = t - TBASE;   // BCB 1899/12/30 0:0:0 からの経過日数
            return (ts.TotalDays);
        }

        //現在の時刻の表示と、タイマーの表示に使用されるデリゲート
        delegate void dlgSetString(object lbl, string text);
        //ボタンのカラー変更に使用されるデリゲート
        delegate void dlgSetColor(object lbl, int state);

        //デリゲートで別スレッドから呼ばれてラベルに現在の時間又は
        //ストップウオッチの時間を表示する
        private void ShowRText(object sender, string str)
        {
            RichTextBox rtb = (RichTextBox)sender;　//objectをキャストする
            rtb.AppendText(str);
        }
        private void ShowText(object sender, string str)
        {
            TextBox rtb = (TextBox)sender;　//objectをキャストする
            rtb.Text = str;
        }
        private void SetColor(object sender, int sta)
        {
            Button rtb = (Button)sender;　//objectをキャストする
            if (sta == RUN)
            {
                rtb.BackColor = Color.Red;
            }
            else if (sta == STOP)
            {
                rtb.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }
        private void SetTimer(object sender, int sta)
        {
            System.Windows.Forms.Timer tim = (System.Windows.Forms.Timer)sender;　//objectをキャストする
            if (sta == RUN)
            {
                tim.Start();
            }
            else if (sta == STOP)
            {
                tim.Stop();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 画面表示
            if ((id % 1) == 0)
            {
                IplImage image = (IplImage)e.UserState;

                Cv.Circle(image, new CvPoint((int)xoa, (int)yoa), roa, new CvColor(0, 255, 0));
                Cv.Line(image, new CvPoint((int)xoa + roa, (int)yoa + roa), new CvPoint((int)xoa - roa, (int)yoa - roa), new CvColor(0, 255, 0));
                Cv.Line(image, new CvPoint((int)xoa - roa, (int)yoa + roa), new CvPoint((int)xoa + roa, (int)yoa - roa), new CvColor(0, 255, 0));

                pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                this.ObsStart.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.ObsEndButton.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            this.States = STOP;
        }
        #endregion


        private void ShowButton_Click(object sender, EventArgs e)
        {
            //OpenIDScamera();
            //AVT
            if (cam_maker == Camera_Maker.AVT)
            {
                avt_cam_start();
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Pid_Data_Send();

            // Basler
            if (cam_maker == Camera_Maker.Basler)
            {
                if (this.checkBox_WideDR.Checked)
                {
                    m_imageProvider.SetupGain(1024);
                }
                //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                else
                {
                    m_imageProvider.SetupGain(100);
                    //pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                }
            }

            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                /*  Detect()に実装
                statusRet = cam.Timing.Exposure.Get(out gx);
                if (gx > set_exposure - 1)
                    statusRet = cam.Timing.Exposure.Set(set_exposure1);
                else
                    statusRet = cam.Timing.Exposure.Set(set_exposure);
                */
            }
        }

        private void ObsEndButton_Click(object sender, EventArgs e)
        {
            this.States = STOP;
            timerDisplay.Enabled = false;
            this.ObsEndButton.Enabled = false;
            this.ObsEndButton.BackColor = Color.Red;
            this.ObsStart.Enabled = true;
            this.ObsStart.BackColor = Color.FromKnownColor(KnownColor.Control);

            //AVT
            if (cam_maker == Camera_Maker.AVT)
            {
                avt_cam_end();
            }
            //Basler
            if (cam_maker == Camera_Maker.Basler)
            {
                Stop(); /* Stops the grabbing of images. */
                BaslerEnd();
            }
            //IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                if (cam.Acquisition.Stop() == uEye.Defines.Status.SUCCESS)
                {
                }
            }
            //ImaginSouse
            if (cam_maker == Camera_Maker.ImagingSouce)
            {
                //icImagingControl1.LiveStop();
            }
            //analog
            if (cam_maker == Camera_Maker.analog)
            {
                // BackgroundWorkerを停止.
                if (worker.IsBusy)
                {
                    this.worker.CancelAsync();
                }
            }
        }

        private void ObsStart_Click(object sender, EventArgs e)
        {
            //AVT
            if (cam_maker == Camera_Maker.AVT)
            {
                avt_cam_start();
            }
            // Basler
            if (cam_maker == Camera_Maker.Basler)
            {
                BaslerStart(0);
                ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
            }

            //IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                statusRet = cam.Acquisition.Capture();
                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    MessageBox.Show("Start Live Video failed");
                    return;
                }
            }
            //analog
            if (cam_maker == Camera_Maker.analog)
            {
                // BackgroundWorkerを開始
                if (!worker.IsBusy)
                {
                    this.worker.RunWorkerAsync();
                }
            }

            LiveStartTime = DateTime.Now;
            this.States = RUN;
            timerDisplay.Enabled = true;
            this.ObsStart.Enabled = false;
            this.ObsStart.BackColor = Color.Red;
            this.ObsEndButton.Enabled = true;
            this.ObsEndButton.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.States == RUN)
            {
                ImgSaveFlag = TRUE;
                this.States = SAVE;
                this.timerSave.Enabled = true;
                // 過去データ保存
                if (cam_maker == Camera_Maker.analog)
                {
                    fifo.Saveflag_true_Last(30);  // 1fr=0.2s  -> 5fr=1s 
                }
            }
        }

        private void ButtonSaveEnd_Click(object sender, EventArgs e)
        {
            ImgSaveFlag = FALSE;
            this.States = RUN;
            this.timerSave.Enabled = false;
        }

        private void timerSavePostTime_Tick(object sender, EventArgs e)
        {
            timerSave.Stop();
            timerSaveMainTime.Stop();
            Mode = LOST;
            ButtonSaveEnd_Click(sender, e);
        }
        private void timerSave_Tick(object sender, EventArgs e)
        {
            timerSave.Stop();
            timerSaveMainTime.Stop();
            Mode = LOST;
            ButtonSaveEnd_Click(sender, e);
        }

        private void timerSaveMainTime_Tick(object sender, EventArgs e)
        {
            timerSaveMainTime.Stop();
        }

        private void buttonMakeDark_Click(object sender, EventArgs e)
        {
            Double dFramerate=0;
            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                cam.Timing.Framerate.GetCurrentFps(out dFramerate);
            }
            toolStripStatusLabelFramerate.Text = "Fps: " + dFramerate.ToString("00.00");
            //            label_frame_rate.Text = String.Format("FrDrop:{0} / {1}", icImagingControl1.DeviceCountOfFramesDropped, icImagingControl1.DeviceCountOfFramesNotDropped);
            Pid_Data_Send();

            if (checkBox_WideDR.Checked)
            {
                // checkBox1.Checked = false;
                //DarkMode = TRUE;
                // timerDisplay.Enabled = true;
            }
        }

        private void timerMakeDark_Tick(object sender, EventArgs e)
        {
            timerDisplay.Enabled = false;
            //DarkMode = FALSE;
        }

        private void timerObsOnOff_Tick(object sender, EventArgs e)
        {
            TimeSpan nowtime = DateTime.Now - DateTime.Today;
            TimeSpan endtime = new TimeSpan(7, 0, 0);
            TimeSpan starttime = new TimeSpan(16,30, 0);

            if (nowtime.CompareTo(endtime) >= 0 && nowtime.CompareTo(starttime) <= 0)
            {
                // DayTime
                if (this.States == RUN && checkBoxObsAuto.Checked)
                {
                    ObsEndButton_Click(sender, e);
                    timerWaitShutdown.Start();
                }
            }
            else
            {
                //NightTime
                if (this.States == STOP && checkBoxObsAuto.Checked)
                {
                    ObsStart_Click(sender, e);
                }
            }
        }

        private void checkBoxObsAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxObsAuto.Checked)
            {
                this.ObsStart.Enabled = false;
                this.ObsEndButton.Enabled = false;
            }
            else
            {
                if (States == RUN)
                {
                    this.ObsStart.Enabled = false;
                    this.ObsEndButton.Enabled = true;
                }
                if (States == SAVE)
                {
                    this.ObsStart.Enabled = false;
                    this.ObsEndButton.Enabled = true;
                }
                if (States == STOP)
                {
                    this.ObsStart.Enabled = true;
                    this.ObsEndButton.Enabled = false;
                }
            }
        }
 
        private void timerWaitShutdown_Tick(object sender, EventArgs e)
        {
            shutdown(sender, e);
        }

        /// <summary>
        /// システムシャットダウン
        /// </summary>
        /// <param name="capacity">シャットダウン</param>
        private void shutdown(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "shutdown.exe";
            //コマンドラインを指定
            psi.Arguments = "-s -f";
            //ウィンドウを表示しないようにする（こうしても表示される）
            psi.CreateNoWindow = true;
            //起動
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 画像表示ルーチン
        /// </summary>
        /// <param name="capacity">画像表示用タイマールーチン</param>
        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            if (this.States == STOP) return;

            //OpenCV　表示ルーチン
            if (imgdata.img != null)
            {
                if (cam_color == Camera_Color.mono)
                {
                    Cv.CvtColor(imgdata.img, img_dmk3, ColorConversion.GrayToBgr);
                }
                else
                {
                    Cv.CvtColor(imgdata.img, img_dmk3, ColorConversion.BayerGbToBgr);
                }
                Cv.Circle(img_dmk3, new CvPoint((int)xoa, (int)yoa), roa, new CvColor(0, 255, 0));
                Cv.Line(img_dmk3, new CvPoint(xoa + roa, yoa + roa), new CvPoint(xoa - roa, yoa - roa), new CvColor(0, 255, 0));
                Cv.Line(img_dmk3, new CvPoint(xoa - roa, yoa + roa), new CvPoint(xoa + roa, yoa - roa), new CvColor(0, 255, 0));
                Cv.Rectangle(img_dmk3, new CvRect(xoa - 70, yoa - 55, 70 + 70, 55 + 55), new CvColor(0, 255, 80));　// SF
                Cv.Circle(   img_dmk3, new CvPoint((int)xoa, (int)yoa), 9, new CvColor(0, 200,100)); // 200um Fiber

                String str = String.Format("ID:{4,7:D1} ({0,6:F1},{1,6:F1})({2,6:F0})({3,0:00})", gx, gy, max_val, max_label, id);
                img_dmk3.PutText(str, new CvPoint(6, 12), font, new CvColor(0, 150, 250));
                img_dmk3.Circle(new CvPoint((int)(gx + 0.5), (int)(gy + 0.5)), 15, new CvColor(0, 100, 255));

                try
                {
                    pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img_dmk3);
                }
                catch (System.ArgumentException)
                {
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, id.ToString() });
                    //System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Console.WriteLine(ex.Message);
                    return;
                }
                catch (System.Exception ex)
                {
                    //すべての例外をキャッチする
                    //例外の説明を表示する
                    //匿名デリゲートで表示する
                    this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
                    //System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Console.WriteLine(ex.Message);
                    return;
                }
            }

            label_ID.Text = max_label.ToString("0");

            // Status表示


            double dFramerate = 0; // Frame rate[fr/s]
            double dExpo = 0; // Exposure[us]
            long igain = 0; //Gain
            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                cam.Timing.Framerate.GetCurrentFps(out dFramerate); //IDS
                statusRet = cam.Timing.Exposure.Get(out dExpo);
            }
            if (cam_maker == Camera_Maker.Basler)
            {
                dFramerate = m_imageProvider.GetFrameRate(); // Basler
                dExpo = GetExposureTime();
                igain = GetGain();
                //igain = m_imageProvider.GetTimestamp();
            }
            if (cam_maker == Camera_Maker.AVT)
            {
                dFramerate = StatFrameRate(); //AVT
                dExpo = ExposureTimeAbs();
                igain = GainRaw();
            }
            toolStripStatusLabelFramerate.Text = "Fps: " + dFramerate.ToString("000.0");
            label_frame_rate.Text = (1000 * lap21).ToString("0000") + "[us] " + (1000 * lap22).ToString("0000");
            toolStripStatusLabelExposure.Text = "Exposure: " + dExpo.ToString("00.00")+"[ms]";
            toolStripStatusLabelGain.Text = "Gain: " + igain.ToString("00");

            // Error rate
            long frame_total=0, frame_error=0, frame_underrun = 0 ;

            /*            uEye.Types.CaptureStatus captureStatus;
                        cam.Information.GetCaptureStatus(out captureStatus); //IDS ueye
                        frame_total= captureStatus.Total();
  
             frame_underrun = StatFrameUnderrun();// AVT
             frame_total = StatFrameDelivered() ;
            */
    //        frame_total = m_imageProvider.Get_Statistic_Total_Buffer_Count();
    //        frame_error = Get_Statistic_Total_Buffer_Count();
           // toolStripStatusLabelFailed.Text = "Failed U:" + StatFrameUnderrun().ToString("0000") + " S:" + StatFrameShoved().ToString("0000") + " D:" + StatFrameDropped().ToString("0000");

    //        double err_rate = 100.0 * (frame_total / (double)id);
    //        toolStripStatusLabelID.Text = "Frames: " + frame_total + " " + frame_error + " " + err_rate.ToString("00.00");

            //Int32 s32Value;
            //statusRet = cam.Timing.PixelClock.Get(out s32Value);
            toolStripStatusLabelPixelClock.Text = "fr time[0.1ms]: " + 10000*(elapsed21-elapsed20)/(double)(Stopwatch.Frequency) +" "+ 10000*(elapsed22-elapsed21)/(double)(Stopwatch.Frequency);
 //           toolStripStatusLabelPixelClock.Text = "Gain: " + GainRaw().ToString("00");

            //Double dValue;
  //           toolStripStatusLabelExposure.Text = "Exposure: " + ExposureTimeAbs().ToString("00.00");

            if (this.States == SAVE)
            {
                this.buttonSave.BackColor = Color.Red;
                this.buttonSave.Enabled = false;
                this.ButtonSaveEnd.Enabled = true;
            }
            if (this.States == RUN)
            {
                this.buttonSave.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.buttonSave.Enabled = true;
                this.ButtonSaveEnd.Enabled = false;
                this.ObsStart.BackColor = Color.Red;
                if (!checkBoxObsAuto.Checked)
                {
                    this.ObsStart.Enabled = false;
                    this.ObsEndButton.Enabled = true;
                }
            }
            if (this.States == STOP)
            {
                this.buttonSave.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.buttonSave.Enabled = false;
                this.ButtonSaveEnd.Enabled = false;
                this.ObsStart.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.ObsStart.Enabled = true;
                this.ObsEndButton.Enabled = false;
            }
        }

        /// <summary>
        /// FIFO pushルーチン
        /// imgdata.img　は　すでにセット済み
        /// </summary>
        private void imgdata_push_FIFO()
        {
            // 文字入れ
            //String str = String.Format("ID:{0,6:D1} ", imgdata.id) + imgdata.t.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,6:F1},{1,6:F1})({2,6:F1})", gx, gy, max_val);
            //img_dmk.PutText(str, new CvPoint(10, 460), font, new CvColor(255, 100, 100));

            //try
            //{
            //Cv.Sub(img_dmk, img_dark8, imgdata.img); // dark減算
            //Cv.Copy(img_dmk, imgdata.img);
           // cam.Information.GetImageInfo(s32MemID, out imageInfo);
            imgdata.id = (int)id;     // (int)imageInfo.FrameNumber;
            imgdata.t = DateTime.Now; //imageInfo.TimestampSystem;   //  LiveStartTime.AddSeconds(CurrentBuffer.SampleEndTime);
            imgdata.ImgSaveFlag = !(ImgSaveFlag != 0); //int->bool変換
            //statusRet = cam.Timing.Exposure.Get(out exp);
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
            fifo.InsertFirst(imgdata);
            /*}
            catch (Exception ex)
            {
                //匿名デリゲートで表示する
                this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }*/
        }

        /// <summary>
        /// PID data送信ルーチン
        /// </summary>
        private void Pid_Data_Send_Init()
        {
            //PID送信用UDP
            //バインドするローカルポート番号
            //FSI_PID_DATA pid_data = new FSI_PID_DATA();
            int localPort = mmFsiUdpPortMT3BaslerS;
            //System.Net.Sockets.UdpClient udpc3 = null ;
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
            string remoteHost = mmFsiSC440;
            int remotePort = mmFsiUdpPortSpCam; // KV1000SpCam
            //送信するデータを読み込む
            ++(pid_data.id);
            //pid_data.swid = (ushort)mmFsiUdpPortMT3IDS2s;// 24417;  //mmFsiUdpPortMT3WideS
            pid_data.swid = (ushort)id;// mmFsiUdpPortMT3IDS2s;// 24417;  //mmFsiUdpPortMT3WideS
            pid_data.t = TDateTimeDouble(imageInfo.TimestampSystem);  //LiveStartTime.AddSeconds(CurrentBuffer.SampleEndTime));//(DateTime.Now);
            if (Mode == PID_TEST)
            {
                xoad = xoa_test_start + xoa_test_step * (pid_data.id - test_start_id);
                yoad = yoa_test_start + yoa_test_step * (pid_data.id - test_start_id);
            }
            else
            {
                xoad = xoa_mes;
                yoad = yoa_mes;
            }
            pid_data.dx = (float)(gx - xoad);
            pid_data.dy = (float)(gy - yoad);
            pid_data.vmax = (ushort)(max_val);
            pid_data.az = (float)(az);
            pid_data.alt = (float)(alt);
            pid_data.vaz = (float)(vaz);
            pid_data.valt = (float)(valt);

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
        /// HDDの空き領域を求めます
        /// </summary>
        /// <remarks>
        /// HDD free space
        /// </remarks>
        private long GetTotalFreeSpace(string driveName)
        {
            foreach (System.IO.DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
        /// <summary>
        /// MTmon status 送信ルーチン
        /// </summary>
        /// <remarks>
        /// MTmon status send
        /// </remarks>
        private void MTmon_Data_Send(object sender)
        {
            // MTmon status for UDP
            //データを送信するリモートホストとポート番号
            string remoteHost = mmFsiCore_i5;
            int remotePort = mmFsiUdpPortMTmonitor;
            //送信するデータを読み込む
            mtmon_data.id = 7; //MT3Wide
            mtmon_data.diskspace = (int)(diskspace / (1024 * 1024 * 1024));
            mtmon_data.obs = (byte)this.States;

            //mtmon_data.obs = this.States ; 
            byte[] sendBytes = ToBytes(mtmon_data);

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
        static byte[] ToBytes(MT_MONITOR_DATA obj)
        {
            int size = Marshal.SizeOf(typeof(MT_MONITOR_DATA));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        private void timerMTmonSend_Tick(object sender, EventArgs e)
        {
            MTmon_Data_Send(sender);
        }

        private void timer1min_Tick(object sender, EventArgs e)
        {
            diskspace = cDrive.TotalFreeSpace;
        }

        private void checkBoxGainBoost_CheckedChanged(object sender, EventArgs e)
        {
            // IDS
            if (cam_maker == Camera_Maker.IDS)
            {
                cam.Gain.Hardware.Boost.SetEnable(checkBoxGainBoost.Checked);
            }
        }

        private void timerSaveTimeOver_Tick(object sender, EventArgs e)
        {
            timerSaveTimeOver.Stop();
            Mode = LOST;
            ButtonSaveEnd_Click(sender, e);
        }
    }
}
