using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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

            #region 位置検出2  //Blob
            Cv.Threshold(imgdata.img, img2, threshold_blob, 255, ThresholdType.Binary); //2ms
            blobs.Label(img2, imgLabel); //1.4ms
            max_label = blobs.GreaterBlob();
            elapsed1 = sw.ElapsedTicks; //1.3ms

            if (blobs.Count > 1 && gx >= 0)
            {
                uint min_area = (uint)(threshold_min_area * blobs[max_label].Area);
                blobs.FilterByArea(min_area, uint.MaxValue); //0.001ms

                // 最適blobの選定（area大　かつ　前回からの距離小）
                double x = blobs[max_label].Centroid.X;
                double y = blobs[max_label].Centroid.Y;
                uint area = blobs[max_label].Area;
                //CvRect rect;
                distance_min = ((x - gx) * (x - gx) + (y - gy) * (y - gy)); //Math.Sqrt()
                foreach (var item in blobs)
                {
                    //Console.WriteLine("{0} | Centroid:{1} Area:{2}", item.Key, item.Value.Centroid, item.Value.Area);
                    x = item.Value.Centroid.X;
                    y = item.Value.Centroid.Y;
                    //rect = item.Value.Rect;
                    distance = ((x - gx) * (x - gx) + (y - gy) * (y - gy)); //将来はマハラノビス距離
                    if (distance < distance_min)
                    {
                        d_val = (item.Value.Area) / max_val;
                        if (distance <= 25) //近距離(5pix)
                        {
                            if (d_val >= 0.4)//&& d_val <= 1.2)
                            {
                                max_label = item.Key;
                                distance_min = distance;
                            }
                        }
                        else
                        {
                            if (d_val >= 0.8 && d_val <= 1.5)
                            {
                                max_label = item.Key;
                                distance_min = distance;
                            }
                        }
                    }
                    //w.WriteLine("{0} {1} {2} {3} {4}", dis, dv, i, item.Key, item.Value.Area);
                }
                //gx = x; gy = y; max_val = area;
            }

            if (max_label > 0)
            {
                maxBlob = blobs[max_label];
                max_centroid = maxBlob.Centroid;
                gx = max_centroid.X;
                gy = max_centroid.Y;
                max_val = maxBlob.Area;
                blob_rect = maxBlob.Rect;

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
                correction = Cv.KalmanCorrect(kalman, measurement);
                // 予測フェーズ(kalman)
                prediction = Cv.KalmanPredict(kalman);
                kgx = prediction.DataArraySingle[0] + xoa;
                kgy = prediction.DataArraySingle[1] + yoa;
                kvx = prediction.DataArraySingle[2];
                kvy = prediction.DataArraySingle[3];
                // カルマン　or　観測重心　の選択
                if ((Math.Abs(kgx - gx) + Math.Abs(kgy - gy) < 15))  // 
                {
                    sgx = kgx;
                    sgy = kgy;
                    //imgSrc.Circle(new CvPoint((int)(prediction.DataArraySingle[0] + xoa), (int)(prediction.DataArraySingle[1] + yoa)), 30, new CvColor(100, 100, 255));
                    //w2.WriteLine("{0:D3} {1:F2} {2:F2} {3:F2} {4:F2} {5} {6} {7}", i, max_centroid.X, max_centroid.Y, prediction.DataArraySingle[0] + xc, prediction.DataArraySingle[1] + yc, vm, dx, dy);
                }
                // 目標位置からの誤差(pix)からターゲットの位置を計算
                dx = sgx; dy = sgy;
                udpkv.cxcy2azalt(-dx, -dy, udpkv.az2_c, udpkv.alt2_c, udpkv.mt3mode, theta_c, fl, ccdpx, ccdpx, ref az, ref alt);
                udpkv.cxcy2azalt(-(dx + kvx), -(dy + kvy), udpkv.az2_c, udpkv.alt2_c, udpkv.mt3mode, theta_c, fl, ccdpx, ccdpx, ref az1, ref alt1);
                vaz = udpkv.vaz2_kv + (az1 - az) / dt;
                valt = udpkv.valt2_kv + (alt1 - alt) / dt;

                //daz = az - udpkv.az2_c; dalt = alt - udpkv.alt2_c;             //位置誤差
                //dvaz = (daz - daz1) / dt; dvalt = (dalt - dalt1) / dt;        //速度誤差
                //diff_vaz = (az - az_pre1) / dt; diff_valt = (alt - alt_pre1) / dt; //速度差

                az0 = az; alt0 = alt;

                if (ImgSaveFlag == TRUE)
                {
                    // 観測データ送信
                    //Pid_Data_Send();

                    // 保存時間延長
                    timerSavePostTime.Stop();
                    timerSaveMainTime.Stop();
                    timerSaveMainTime.Start();
                }
            }
            else
            {
                gx = gy = 0;
                sgx = sgy = 0;
                max_val = 0;
            }
             #endregion

            // 保存用データをキューへ
            if (ImgSaveFlag == TRUE)
            {
                imgdata_push_FIFO();
            }

            elapsed2 = sw.ElapsedTicks; sw.Stop(); sw.Reset();
            // 処理速度
            double sf = (double)Stopwatch.Frequency / 1000; //msec
            lap0 = (1 - alpha) * lap0 + alpha * elapsed0 / sf;
            lap1 = (1 - alpha) * lap1 + alpha * elapsed1 / sf;
            lap2 = (1 - alpha) * lap2 + alpha * elapsed2 / sf;
            fr_str = String.Format("ID:{0,5:D1} L0:{1,4:F2} L1:{2,4:F2} L2:{3,4:F2}", id, lap0, lap1, lap2);

            //catch (Exception ex)
            //{
            //匿名デリゲートで表示する
            //  this.Invoke(new dlgSetString(ShowRText), new object[] { richTextBox1, ex.ToString() });
            //  System.Diagnostics.Trace.WriteLine(ex.Message);
            //}

            // ワイドダイナミックレンジ用設定 Exp 100-1-100-1-
            if (checkBox_WideDR.Checked)
            {
                statusRet = cam.Timing.Exposure.Get(out gx);
                if (gx > set_exposure - 1)
                    statusRet = cam.Timing.Exposure.Set(set_exposure1);
                else
                    statusRet = cam.Timing.Exposure.Set(set_exposure);
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
