using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace MT3
{
    class position_mesure
    {
        CvPoint2D64f pos_pre = new CvPoint2D64f(-1, -1);
        double distance_pre = 1000;
        double dist_alpha = 0.5;
        double distance0;　//距離定数　評価値が1/eになる距離[pixel]


        public position_mesure()
        {
        }
        /// <summary>
        /// 最適blobの選定
        /// </summary>
        /// <remarks>
        /// 最適blobの選定（areaの大きさと前回からの距離）
        /// </remarks>
        public int mesure(CvBlobs blobs)
        {
            if (blobs.Count == 0)
            {
                return 0;
            }
            CvPoint2D64f pos_ans = new CvPoint2D64f(-1, -1);
            CvBlob maxBlob = blobs.LargestBlob();
            int max_label = blobs.GreaterBlob().Label;
            if (blobs.Count == 0) return 0;
            pos_ans = maxBlob.Centroid;
            distance0 = Cal_distance_const(distance_pre);
            if (blobs.Count > 1)
            {
                // 最適blobの選定
                double eval, eval_max = 0;
                foreach (var item in blobs)
                {
                    eval = position_mesure.Cal_Evaluate(item.Value.Centroid, item.Value.Area, pos_pre, distance0);
                    if (eval > eval_max)
                    {
                        eval_max = eval;
                        max_label = item.Key;
                        pos_ans = item.Value.Centroid;

                        ///Console.WriteLine("{0} | Centroid:{1} Area:{2} eval:{3}", item.Key, item.Value.Centroid, item.Value.Area, eval);
                        //w.WriteLine("{0} {1} {2} {3} {4}", dis, dv, i, item.Key, item.Value.Area);
                    }
                    //sw.Stop(); t5 = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency; sw.Reset(); sw.Start();  
                    ///Console.WriteLine(" pos_ans:{0}", pos_ans);
                }
            }
            double dis = Cal_distance(pos_ans, pos_pre);
            if (distance_pre > dis)
            {
                distance_pre = (1 - dist_alpha) * distance_pre + dist_alpha * dis;
            }
            else
            {
                distance_pre = dis;
            }
            pos_pre = pos_ans;
            return max_label;
        }

        /// <summary>
        /// 評価関数
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="maxval">ブロブの面積</param>
        /// <param name="pos_pre">前回の目標位置</param>
        /// <param name="distance0">距離定数</param>
        public static double Cal_Evaluate(CvPoint2D64f pos, double maxval, CvPoint2D64f pos_pre, double distance0)
        {
            double distance = Cal_distance(pos, pos_pre);
            double eval = maxval * Math.Exp(-distance / distance0);
            return eval;
        }
        public static double Cal_distance(CvPoint2D64f pos1, CvPoint2D64f pos2)
        {
            return Math.Sqrt(((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y)));
        }
        /// <summary>
        /// 評価関数用距離定数算出
        /// </summary>
        public static double Cal_distance_const(double distance_pre)
        {
            double ans = Math.Abs(distance_pre * 20);
            if (ans > 10000) ans = 10000;
            if (ans < 10) ans = 10;
            return ans;
        }
        /// <summary>
        /// 内部変数初期化
        /// </summary>
        public void init(double dis = 1000)
        {
            distance_pre = dis;
        }
        /// <summary>
        /// 内部変数取り出し
        /// </summary>
        public double get_distance_pre()
        {
            return distance_pre;
        }
        public double get_distance0()
        {
            return distance0;
        }
    }
}
