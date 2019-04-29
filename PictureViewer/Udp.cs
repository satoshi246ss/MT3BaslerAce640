using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace MT3
{
    // C++ 構造体のマーシャリング
    [StructLayout(LayoutKind.Sequential)]
    public struct FSI_DATA
    {
        public UInt16 id;    // unsigned short
        public Byte   cam_id; //unsigned char
        public Byte   fsi_pos;
        public Byte   cmd;
        public Byte   wdt;
        public Int16  mag;
        public Double t;
        public Single az;
        public Single alt;
        public Single vaz;
        public Single valt;
        public Single az_c;
        public Single alt_c;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOTOR_DATA_KV_SP
    {
        public Int32 cmd;      // コマンド  １：検出  16: Meteor Lost 17:move end
        public Double t;       // 送信時刻
        public Single az;      // 目標方位、南が０度、西回り
        public Single alt;     // 目標高度、天頂が90度、地平が0度
        public Single vaz;
        public Single valt;
        public Single theta;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FSI_PID_DATA
    {
        public UInt16 swid;         // ソフトウェアID
        public UInt16 id;           // パケットID
        public UInt16 vmax;         // [count]カウント値
        public Double t;            // 送信時刻
        public Single dx;           // [deg] 中心からの誤差(方位方向)
        public Single dy;           // [deg] 中心からの誤差(高度方向)

        public Single az;           // [deg] wide位置(方位方向)　重心位置
        public Single alt;          // [deg] wide位置(高度方向)　
        public Single vaz;          // [deg/s] 中心からの誤差(方位方向)　重心位置
        public Single valt;         // [deg/s] 中心からの誤差(高度方向)　

        public Byte kalman_state; // 0:無効　1:有効　
        public Byte dum1;         // 予備　
        public Byte dum1a;        // 予備　
        public Byte dum1b;        // 予備　
        public Single dum2;        // 予備　
        public Single dum3;        // 予備　
        public Single dum4;        // 予備　
        public Single dum5;        // 予備　
        public Single dum6;        // 予備　
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KV_PID_DATA
    {
        public Int16 wide_time;     // 12[time code] wide Udp time code [ms]
        public Int16 wide_id;       // 34[+-ID]  wide ID
        public Int16 wide_az;       // 56[mmdeg] wide位置(方位方向)　重心位置
        public Int16 wide_alt;      // 78[mmdeg] wide位置(高度方向)　重心位置
        public Int16 wide_vk;       // 90[pix/fr] wide velosity(kalman filter sqrt(vx2+vy2)
        public Int16 fine_time;     // 90[time code] fine Udp time code [ms]
        public Int16 fine_id;       // 12[+-ID]  fine ID
        public Int16 fine_az;       // 34[mmdeg] wide位置(方位方向)　重心位置
        public Int16 fine_alt;      // 56[mmdeg] wide位置(高度方向)　重心位置
        public Int16 fine_vk;       // 90[pix/fr] fine velosity(kalman filter sqrt(vx2+vy2)
        public Int16 sf_time;       // 78[time code] sf Udp time code [ms]
        public Int16 sf_id;         // 90[+-ID]  sf ID
        public Int16 sf_az;         // 12[mmdeg] wide位置(方位方向)　重心位置
        public Int16 sf_alt;        // 34[mmdeg] wide位置(高度方向)　重心位置
        public Int16 sf_vk;         // 90[pix/fr] sf velosity(kalman filter sqrt(vx2+vy2)
        public Int16 mt2_wide_time; // 12[time code] MT2 wide Udp time code [ms]
        public Int16 mt2_wide_id;   // 34[+-ID]  wide ID
        public Int16 mt2_wide_az;   // 56[mmdeg] wide位E置u(方u位E方u向u)　@重d心S位E置u
        public Int16 mt2_wide_alt;  // 78[mmdeg] wide位E置u(高?度x方u向u)　@重d心S位E置u
        public Int16 mt2_wide_vk;   // 90[pix/fr] wide velosity(kalman filter sqrt(vx2+vy2)
        public Int16 fish_id;       // 12[+-ID]  fish ID
        public Int16 fish_vaz;      // 34[mmdeg/s] 速度推定値(方位方向)　カルマンフィルタ
        public Int16 fish_valt;     // 56[mmdeg/s] 速度推定値(高度方向)　カルマンフィルタ
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MT_MONITOR_DATA
    {
        public Byte id;        //unsigned char  Soft ID
        public Byte obs;       //unsigned char  観測中:1/0
        public Byte save;      //unsigned char  保存中:1/0
        public Int32 diskspace; // HDD残容量(MB)
    }

    /// <summary>
    /// UDP通信
    /// </summary>
    /// <typeparam name="T">要素の型</typeparam>
    public class Udp
    {
        public Udp() : this(7777) { }

        /// <summary>
        /// 初期最大容量を指定して初期化。
        /// </summary>
        /// <param name="capacity">初期載大容量</param>
        public Udp(int port)
        {
            // UDP/IPで非同期データ受信するサンプル(C#.NET/VS2005)
            // UDP/IPソケット生成
            UdpClient objSck = new UdpClient(port);

            // UDP/IP受信コールバック設定(System.AsyncCallback)
            objSck.BeginReceive(ReceiveCallback, objSck);
        }

        // UDP/IP受信コールバック関数
        public void ReceiveCallback(IAsyncResult AR)
        {
            // UDP/IP受信
            System.Net.IPEndPoint ipAny = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
            Byte[] rdat = ((System.Net.Sockets.UdpClient)AR.AsyncState).EndReceive(AR, ref ipAny);
            String rstr =  System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(rdat);
            //WriteLine(rstr);

            // 連続で(複数回)データ受信する為の再設定
            ((System.Net.Sockets.UdpClient)AR.AsyncState).BeginReceive(ReceiveCallback, AR.AsyncState);
        }

    }
}
