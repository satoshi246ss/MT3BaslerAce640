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
        public Int32 cmd;      // コマンド  １：検出  16: Meteor Lost
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

        public Single dum1;        // 予備　
        public Single dum2;        // 予備　
        public Single dum3;        // 予備　
        public Single dum4;        // 予備　
        public Single dum5;        // 予備　
        public Single dum6;        // 予備　
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
