using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace MT3
{
    partial class Form1
    {
        /// <summary>
        /// 最大値サーチ
        /// </summary>
        public void detect()
        {
            ++id;
            if (appSettings.CamPlatform == Platform.MT2)
            {
                theta_c = -udpkv.cal_mt2_theta(appSettings.Flipmode) - appSettings.Theta;
            }

            if (!appSettings.UseDetect) return;

            #region 位置検出2  //Blob
            try
            {
                //Cv.Smooth(imgdata.img, img2, SmoothType.Median, 5, 0, 0, 0);
                //Cv.Threshold(img2, img2, appSettings.ThresholdBlob, 255, ThresholdType.Binary); //2ms
                Cv.Threshold(imgdata.img, img2, appSettings.ThresholdBlob, 255, ThresholdType.Binary); //2ms
                blobs.Label(img2); //1.4ms
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("KeyNotFoundException:211");
            }
            try
            {
                maxBlob = blobs.LargestBlob();
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("KeyNotFoundException:212");
            }

            try
            {
                if (blobs.Count > 0)
                {
                    int min_area = Math.Max(2, (int)(appSettings.ThresholdMinArea * maxBlob.Area));
                    blobs.FilterByArea(min_area, int.MaxValue); //0.001ms   面積がmin_area未満のblobを削除
                }
                max_label = 0;
                if (blobs.Count > 0)
                {
                    max_label = pos_mes.mesure(blobs);
                } 
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("KeyNotFoundException:213");
            }

            if (max_label > 0 && blobs.ContainsKey(max_label))
            {
                try
                {
                    maxBlob = blobs[max_label];
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("KeyNotFoundException:2171");
                }
                try{
                    max_centroid = maxBlob.Centroid;
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("KeyNotFoundException:2172");
                }
                try{
                    gx = max_centroid.X;
                    gy = max_centroid.Y;
                    max_val = maxBlob.Area;
                    blob_rect = maxBlob.Rect;
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("KeyNotFoundException:2173");
                }

                // 観測値(kalman)
                measurement.Set2D(0, 0, (float)(gx - xoa));
                measurement.Set2D(1, 0, (float)(gy - yoa));
                if (kalman_id++ == 0)
                {
                    // 初期値設定
                    double errcov = 1.0;
                    kalman.StatePost.Set1D(0, measurement.Get1D(0));
                    kalman.StatePost.Set1D(1, measurement.Get1D(1));
                    Cv.SetIdentity(kalman.ErrorCovPost, Cv.RealScalar(errcov));
                }
                // 修正フェーズ(kalman)
                try
                {
                    correction = Cv.KalmanCorrect(kalman, measurement);
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("KeyNotFoundException:216");
                }

                // 予測フェーズ(kalman)
                try
                {
                    prediction = Cv.KalmanPredict(kalman);
                    kgx = prediction.DataArraySingle[0] + xoa;
                    kgy = prediction.DataArraySingle[1] + yoa;
                    kvx = prediction.DataArraySingle[2];
                    kvy = prediction.DataArraySingle[3];
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("KeyNotFoundException:215");
                }

                // カルマン　or　観測重心　の選択
                sgx = gx; sgy = gy;
                if ((Math.Abs(kgx - gx) + Math.Abs(kgy - gy) < 15))  // 
                {
                    sgx = kgx;
                    sgy = kgy;
                    //imgSrc.Circle(new CvPoint((int)(prediction.DataArraySingle[0] + xoa), (int)(prediction.DataArraySingle[1] + yoa)), 30, new CvColor(100, 100, 255));
                    //w2.WriteLine("{0:D3} {1:F2} {2:F2} {3:F2} {4:F2} {5} {6} {7}", i, max_centroid.X, max_centroid.Y, prediction.DataArraySingle[0] + xc, prediction.DataArraySingle[1] + yc, vm, dx, dy);
                }
                dx = sgx - appSettings.Xoa;
                dy = sgy - appSettings.Yoa;

                // 目標位置からの誤差(pix)からターゲットの位置を計算
                if (appSettings.CamPlatform == Platform.MT3)
                {
                    try
                    {
                        theta_c = -udpkv.cal_mt3_theta() - appSettings.Theta;
                        udpkv.cxcy2azalt(-dx, -dy, udpkv.az2_c, udpkv.alt2_c, udpkv.mt3mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az, ref alt);
                        udpkv.cxcy2azalt(-(dx + kvx), -(dy + kvy), udpkv.az2_c, udpkv.alt2_c, udpkv.mt3mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az1, ref alt1);
                        vaz = udpkv.vaz2_kv + (az1 - az) * appSettings.Framerate;
                        valt = udpkv.valt2_kv + (alt1 - alt) * appSettings.Framerate;

                        daz = az - udpkv.az2_c; dalt = alt - udpkv.alt2_c;             //位置誤差
                        //dvaz = (daz - daz1) / dt; dvalt = (dalt - dalt1) / dt;        //速度誤差
                        //diff_vaz = (az - az_pre1) / dt; diff_valt = (alt - alt_pre1) / dt; //速度差

                        az0 = az; alt0 = alt;
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("KeyNotFoundException:218");
                    }
                }
                else if (appSettings.CamPlatform == Platform.MT2)
                {
                    try
                    {
                        theta_c = -udpkv.cal_mt2_theta(appSettings.Flipmode) - appSettings.Theta;
                        udpkv.cxcy2azalt_mt2(+dx, +dy, udpkv.az1_c, udpkv.alt1_c, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az, ref alt);
                        udpkv.cxcy2azalt_mt2(+(dx + kvx), +(dy + kvy), udpkv.az1_c, udpkv.alt1_c, udpkv.mt2mode, theta_c, appSettings.FocalLength, appSettings.Ccdpx, appSettings.Ccdpy, ref az1, ref alt1);
                        vaz = udpkv.vaz1_kv + (az1 - az) * appSettings.Framerate;
                        valt = udpkv.valt1_kv + (alt1 - alt) * appSettings.Framerate;

                        daz = az - udpkv.az1_c; dalt = alt - udpkv.alt1_c;             //位置誤差
                        az0 = az; alt0 = alt;
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("KeyNotFoundException:218b");
                    }
                }
                // Data send
                if (ImgSaveFlag == TRUE)
                {
                    // 観測目標移動速作成
                    double vk=1000;  // [pixel/frame]
                    if (kalman_id > 3)
                    {
                        vk = Math.Sqrt(kvx * kvx + kvy * kvy);
                    }
                    // 観測データ送信
                    //Pid_Data_Send(true);
                    Pid_Data_Send_KV1000_SpCam2((short)(id & 32767), daz, dalt, vk); // 32767->7FFF

                    if (Math.Abs(udpkv.vaz2_kv) > 0.1 || Math.Abs(udpkv.valt2_kv) > 0.1)
                    {
                        // 保存時間延長
                        //timerSavePostTime.Stop();
                        timerSaveMainTime.Stop();
                        timerSaveMainTime.Start();
                    }
                }

            }
            else
            {
                if (ImgSaveFlag == TRUE)
                {
                    // 観測データ送信
                    //Pid_Data_Send(false);
                    ////Pid_Data_Send_KV1000_SpCam2((short)(-(id & 32767)), (az - udpkv.az2_c), (alt - udpkv.alt2_c), -1000);
                }
                gx = gy = 0;
                sgx = sgy = 0;
                max_val = 0;
            }

            // ｋｖデータのチェック用
            if (ImgSaveFlag == TRUE)
            {
                if (log_writer != null)
                {
                    //xpos = ((kd.x1 << 8) + kd.x0) << 4; // <<16 ->256*256  <<8 ->256
                    //ypos = ((kd.y1 << 8) + kd.y0) << 4; // <<16 ->256*256  <<8 ->256
                    string st = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") +"("+ udpkv.xpos + " " + udpkv.ypos + ")( " + udpkv.x2pos + " " + udpkv.y2pos + ") " + udpkv.kd.x1 + " " + udpkv.kd.x0 + " " + udpkv.kd.y1 + " " + udpkv.kd.y0 + "\n";
                    log_writer.WriteLine(st);
                }
            }

            #endregion

            elapsed2 = sw.ElapsedTicks; sw.Stop(); sw.Reset();
            // 処理速度
            double sf = (double)Stopwatch.Frequency / 1000; //msec
            lap0 = (1 - alpha) * lap0 + alpha * elapsed0 / sf;
            lap1 = (1 - alpha) * lap1 + alpha * elapsed1 / sf;
            lap2 = (1 - alpha) * lap2 + alpha * elapsed2 / sf;
            fr_str = String.Format("ID:{0,5:D1} L0:{1,4:F2} L1:{2,4:F2} L2:{3,4:F2}", id, lap0, lap1, lap2);

            // ワイドダイナミックレンジ用設定 Exp 100-1-100-1-
            if (checkBox_WideDR.Checked)
            {
                // IDS
                if (cam_maker == Camera_Maker.IDS)
                {

                    statusRet = cam.Timing.Exposure.Get(out gx);
                    if (gx > set_exposure - 1)
                        statusRet = cam.Timing.Exposure.Set(set_exposure1);
                    else
                        statusRet = cam.Timing.Exposure.Set(set_exposure);
                }
            }
        }

        /// <summary>
        /// kalman 初期化ルーチン
        /// </summary>
        /// <param name="elem">読み出した要素</param> 
        private void kalman_init()
        {
            // 初期化(kalman)
            kalman_id = 0;
            Cv.SetIdentity(kalman.MeasurementMatrix, Cv.RealScalar(1.0));
            Cv.SetIdentity(kalman.ProcessNoiseCov, Cv.RealScalar(1e-4));
            Cv.SetIdentity(kalman.MeasurementNoiseCov, Cv.RealScalar(0.001));
            Cv.SetIdentity(kalman.ErrorCovPost, Cv.RealScalar(1.0));
            measurement.Zero();

            // 等速直線運動モデル(kalman)
            kalman.TransitionMatrix.Set2D(0, 0, 1.0f);
            kalman.TransitionMatrix.Set2D(0, 1, 0.0f);
            kalman.TransitionMatrix.Set2D(0, 2, 1.0f);
            kalman.TransitionMatrix.Set2D(0, 3, 0.0f);

            kalman.TransitionMatrix.Set2D(1, 0, 0.0f);
            kalman.TransitionMatrix.Set2D(1, 1, 1.0f);
            kalman.TransitionMatrix.Set2D(1, 2, 0.0f);
            kalman.TransitionMatrix.Set2D(1, 3, 1.0f);

            kalman.TransitionMatrix.Set2D(2, 0, 0.0f);
            kalman.TransitionMatrix.Set2D(2, 1, 0.0f);
            kalman.TransitionMatrix.Set2D(2, 2, 1.0f);
            kalman.TransitionMatrix.Set2D(2, 3, 0.0f);

            kalman.TransitionMatrix.Set2D(3, 0, 0.0f);
            kalman.TransitionMatrix.Set2D(3, 1, 0.0f);
            kalman.TransitionMatrix.Set2D(3, 2, 0.0f);
            kalman.TransitionMatrix.Set2D(3, 3, 1.0f);
        }

    }
}
