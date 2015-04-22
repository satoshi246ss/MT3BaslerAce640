using System;
//using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace MT3
{
    // C++ 構造体のマーシャリング
    [StructLayout(LayoutKind.Sequential)]
    public struct VIDEO_DATA
    {
        public ushort id;
        public ushort detect_mode; // 0:off  1:on
        public ushort msec;
        public ushort sec;

        public ushort min;
        public ushort hour;
        public ushort day;
        public ushort month;

        public ushort year;
        public UInt16 kv_status, data_request;
        public short mt3mode;

        public Int32 x2pos, y2pos;
        public Int32 x2v, y2v;

        public double gx, gy, vmax;
        public double kgx, kgy, kvx, kvy;
        public double vaz2_kv, valt2_kv, az2_c, alt2_c, kvaz, kvalt;
        public double az, alt, vaz, valt;

        public CvRect max_blob_rect;
        public bool ImgSaveFlag;
    }

    public struct ImageData
    {
        public int id;
        public int detect_mode; // 0:off  1:on
        public DateTime t;
        public IplImage img;
        public bool ImgSaveFlag;
        public double gx, gy, vmax;
        public CvBlobs blobs;
        public double kgx, kgy, kvx, kvy;
        public double az, alt, vaz, valt;
        public Udp_kv udpkv1;

        // デフォルトコンストラクタ
 /*       public ImageData()
        {
            id = 0;
            detect_mode = 0;
            t = DateTime.Now;
            img = null;// new IplImage(w, h, BitDepth.U8, 1);
            ImgSaveFlag = false;
            gx = gy = vmax = 0.0;
            kgx = kgy = kvx = kvy = 0.0;
            az = alt = vaz = valt = 0.0;
            blobs = new CvBlobs();
            udpkv1 = new Udp_kv();
        }
        */
        // デフォルトコンストラクタ
        public ImageData(Int32 w, Int32 h)
        {
            id = 0;
            detect_mode = 0;
            t = DateTime.Now;
            img = null;
            //img = new IplImage(w, h, BitDepth.U8, 1);
            ImgSaveFlag = false;
            gx = gy = vmax = 0.0;
            kgx = kgy = kvx = kvy = 0.0;
            az = alt = vaz = valt = 0.0;
            blobs = new CvBlobs();
            udpkv1 = new Udp_kv();
        }
        public void init(Int32 w, Int32 h)
        {
            img = new IplImage(w, h, BitDepth.U8, 1);
        }
    }
    /// <summary>
    /// 循環バッファ。
    /// </summary>
    /// <typeparam name="T">要素の型</typeparam>
    public class CircularBuffer : IEnumerable //(ImageData img) : IEnumerable(ImageData img)
    {
        #region フィールド

        ImageData[] data;
        IplImage[] img;
        int top, bottom;
        int mask;

        CvVideoWriter vw;

        public int MtMode { get; set; } // MtMode  MT3:3   MT2:2
 
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        
        // file ID
        private int _no_cap_dev;
        public int NoCapDev
        {
            get { return _no_cap_dev; }
            set { _no_cap_dev = value; }
        }

        // Save dir
        private string _save_dir;
        public string SaveDir
        {
            get { return _save_dir; }
            set { _save_dir = value; }
        }


        IplImage imgR;
        //IplImage imgBGR = new IplImage(640, 480, BitDepth.U8, 3);
        //IplImage imgR = new IplImage(640, 480, BitDepth.U8, 1);
        CvFont font = new CvFont(FontFace.HersheyComplex, 0.5, 0.5);
        VIDEO_DATA vd = new VIDEO_DATA();
        StreamWriter writer;//= new StreamWriter(@"Test.txt", true, System.Text.Encoding.GetEncoding("shift_jis"));

        #endregion
        #region 初期化

        //public CircularBuffer() : this(256, 640, 480) { }
        public CircularBuffer() { }

        /// <summary>
        /// 初期最大容量を指定して初期化。
        /// </summary>
        /// <param name="capacity">初期最大容量</param>
        public CircularBuffer(int capacity, int width, int height)
        {
            _no_cap_dev = 9;
            _save_dir = @"C:\Users\Public\img_data\";
            _width = width;
            _height = height;

            capacity = Pow2((uint)capacity);
            this.data = new ImageData[capacity];
            this.img = new IplImage[capacity];
            for (int i = 0; i < capacity; i++)
            {
                //this.data[i] = new ImageData(width,height) ;
                this.img[i]  = new IplImage(width, height, BitDepth.U8, 1);
            }
            this.top = this.bottom = 0;
            this.mask = capacity - 1;
            this.imgR = new IplImage(width, height, BitDepth.U8, 1);
        }

        /// <summary>
        /// 初期最大容量を指定して初期化。
        /// </summary>
        /// <param name="capacity">初期最大容量</param>
        public void init(int capacity, int width, int height, int no_cap_dev, string save_dir, int mtmode=3)
        {
            MtMode = mtmode; //3:MT3    2:MT2
            _no_cap_dev = no_cap_dev;
            _save_dir   = save_dir;
            _width      = width;
            _height     = height;
            SaveDir = save_dir;

            capacity = Pow2((uint)capacity);
            this.data = new ImageData[capacity];
            this.img = new IplImage[capacity];
            for (int i = 0; i < capacity; i++)
                this.img[i] = new IplImage(width, height, BitDepth.U8, 1); //1
            this.top = this.bottom = 0;
            this.mask = capacity - 1;
            this.imgR = new IplImage(width, height, BitDepth.U8, 1);
        }


        static int Pow2(uint n)
        {
            --n;
            int p = 0;
            for (; n != 0; n >>= 1) p = (p << 1) + 1;
            return p + 1;
        }

        #endregion
        #region プロパティ

        /// <summary>
        /// 格納されている要素数。
        /// </summary>
        public int Count
        {
            get
            {
                int count = this.bottom - this.top;
                if (count < 0) count += this.data.Length;
                return count;
            }
        }

        /// <summary>
        /// i 番目の要素を読み書き。
        /// </summary>
        /// <param name="i">読み書き位置</param>
        /// <returns>読み出した要素</returns>
        public ImageData this[int i]
        {
            get { return this.data[(i + this.top) & this.mask]; }
            set { this.data[(i + this.top) & this.mask] = value; }
        }

        /// <summary>
        /// 末尾の要素を読み出し。
        /// </summary>
        /// <param name="elem">読み出した要素</param>
        public ImageData Last()
        {
            return this.data[(this.bottom) & this.mask];
        }

        /// <summary>
        /// 先頭の要素を読み出し。
        /// </summary>
        /// <param name="elem">読み出した要素</param>
        public ImageData First()
        {
            return this.data[(this.top) & this.mask];
        }

        /// <summary>
        /// i 番目の画像を読み書き。
        /// </summary>
        /// <param name="i">読み書き位置</param>
        /// <returns>読み出した要素</returns>
        public IplImage Image(int i)
        {
            return this.img[(i + this.top) & this.mask];
        }

        /// <summary>
        /// 末尾の画像を読み出し。
        /// </summary>
        /// <param name="elem">読み出した要素</param>
        public IplImage LastImage()
        {
            return this.img[(this.bottom) & this.mask];
        }

        /// <summary>
        /// 先頭の画像を読み出し。
        /// </summary>
        /// <param name="elem">読み出した要素</param>
        public IplImage FirstImage()
        {
            return this.img[(this.top) & this.mask];
        }
        #endregion
        #region 挿入・削除

        /// <summary>
        /// 配列を確保しなおす。
        /// </summary>
        /// <remarks>
        /// 配列長は2倍ずつ拡張していきます。
        /// </remarks>
        void Extend()
        {
            ImageData[] data = new ImageData[this.data.Length * 2];
            int i = 0;
            foreach (ImageData elem in this)
            {
                data[i] = elem;
                ++i;
            }
            IplImage[] img = new IplImage[this.data.Length * 2];
            for (i = 0; i < this.data.Length * 2; i++)
                this.img[i] = new IplImage(Width, Height, BitDepth.U8, 1);
            i = 0;
            foreach (IplImage elem in this)
            {
                img[i] = elem;
                ++i;
            }
            this.top = 0;
            this.bottom = this.Count;
            this.data = data;
            this.img = img;
            this.mask = data.Length - 1;
        }

        /// <summary>
        /// i 番目の位置に新しい要素を追加。
        /// </summary>
        /// <param name="i">追加位置</param>
        /// <param name="elem">追加する要素</param>
        public void Insert(int i, ImageData elem)
        {
            if (this.Count >= this.data.Length - 1)
                this.Extend();

            if (i < this.Count / 2)
            {
                for (int n = 0; n <= i; ++n)
                {
                    this[n - 1] = this[n];
                }
                this.top = (this.top - 1) & this.mask;
                this[i] = elem;
            }
            else
            {
                for (int n = this.Count; n > i; --n)
                {
                    this[n] = this[n - 1];
                }
                this[i] = elem;
                this.bottom = (this.bottom + 1) & this.mask;
            }
        }

        /// <summary>
        /// 先頭に新しい要素を追加。
        /// </summary>
        /// <param name="elem">追加する要素</param>
        public void InsertFirst(ImageData elem)
        {
            if (this.Count >= this.data.Length - 1)
                this.Extend();

            this.top = (this.top - 1) & this.mask;
            this.data[this.top] = elem;
            Cv.Copy(elem.img, this.img[this.top]);
        }
        /// <summary>
        /// 先頭に新しい要素を追加。
        /// </summary>
        /// <param name="elem">追加する要素</param>
        public void InsertFirst(ImageData elem, byte [] buf)
        {
            if (this.Count >= this.data.Length - 1)
                this.Extend();

            this.top = (this.top - 1) & this.mask;
            this.data[this.top] = elem;
            System.Runtime.InteropServices.Marshal.Copy(buf, 0, this.img[this.top].ImageDataOrigin, buf.Length);
        }

        /// <summary>
        /// 末尾に新しい要素を追加。
        /// </summary>
        /// <param name="elem">追加する要素</param>
        public void InsertLast(ImageData elem)
        {
            if (this.Count >= this.data.Length - 1)
                this.Extend();

            this.data[this.bottom] = elem;
            this.bottom = (this.bottom + 1) & this.mask;
            Cv.Copy(elem.img, this.img[this.bottom]);
        }

        /// <summary>
        /// i 番目の要素を削除。
        /// </summary>
        /// <param name="i">削除位置</param>
        public void Erase(int i)
        {
            for (int n = i; n < this.Count - 1; ++n)
            {
                this[n] = this[n + 1];
                Cv.Copy(this.img[n + 1], this.img[n]);
            }
            this.bottom = (this.bottom - 1) & this.mask;
        }

        /// <summary>
        /// 先頭の要素を削除。
        /// </summary>
        public void EraseFirst()
        {
            this.top = (this.top + 1) & this.mask;
        }

        /// <summary>
        /// 末尾の要素を削除。必要に応じて画像書き込み
        /// </summary>
        public void EraseLast()
        {
            // 初期化チェック
            if (this.data[this.bottom].ImgSaveFlag == false && this.data[(this.bottom - 1) & this.mask].ImgSaveFlag == true)
            {
                string fn = SaveDir + this.data[(this.bottom - 1) & this.mask].t.ToString("yyyyMMdd") + @"\";
                // フォルダ (ディレクトリ) が存在しているかどうか確認する
                if (!System.IO.Directory.Exists(fn))
                {
                    System.IO.Directory.CreateDirectory(fn);
                }
                fn += this.data[(this.bottom - 1) & this.mask].t.ToString("yyyyMMdd_HHmmss_fff") + string.Format("_{00}", NoCapDev) + ".avi";
                VideoWriterInit(fn);
            }
            // 書き込みチェック
            if (this.data[this.bottom].ImgSaveFlag)
            {
                VideoWriterFrame();
            }
            // 開放チェック
            if (this.data[this.bottom].ImgSaveFlag == true && this.data[(this.bottom - 1) & this.mask].ImgSaveFlag == false)
            {
                VideoWriterRelease();
            }
            // 更新
            this.bottom = (this.bottom - 1) & this.mask;
        }

        /// <summary>
        /// 末尾の要素からｎ個分のsaveflagをtrueとする。
        /// </summary>
        public void Saveflag_true_Last(int n)
        {
            if (n > bottom) n = bottom;
            for (int i = 0; i < n; ++i)
            {
                this.data[this.bottom-i].ImgSaveFlag = true;
            }
        }


        #endregion
        #region IEnumerable<T> メンバ

        public IEnumerator GetEnumerator()
        {
            if (this.top <= this.bottom)
            {
                for (int i = this.top; i < this.bottom; ++i)
                    yield return this.data[i];
            }
            else
            {
                for (int i = this.top; i < this.data.Length; ++i)
                    yield return this.data[i];
                for (int i = 0; i < this.bottom; ++i)
                    yield return this.data[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
        #region VideoWriter

        /// <summary>
        /// 画像を保存します。
        /// </summary>
        /// <remarks>
        /// ビデオ書き込み初期化
        /// </remarks>
        public void VideoWriterInit(string fn)
        {
            int codec = Cv.FOURCC('D', 'I', 'B', ' ');  // 0; //非圧縮avi
            //this.vw = new CvVideoWriter(fn, codec, 29.97, new CvSize(this.width, this.height), true); //color
            this.vw = new CvVideoWriter(fn, codec, 29.97, new CvSize(Width, Height), false); //mono
            fn += this.data[(this.bottom - 1) & this.mask].t.ToString("yyyyMMdd_HHmmss_fff") + string.Format("_{00}", NoCapDev) + ".avi";
            this.writer = new StreamWriter( this.data[(this.bottom - 1) & this.mask].t.ToString("yyyyMMdd_HHmmss_fff") + string.Format("_{00}", NoCapDev) + ".txt"
, true, System.Text.Encoding.GetEncoding("shift_jis"));
        }

        /// <summary>
        /// 画像を保存します。
        /// </summary>
        /// <remarks>
        /// ビデオ書き込み
        /// </remarks>
        public void VideoWriterFrame()
        {
            if (vw == null || writer == null) return;

            // 画像内にデータ埋め込み
            vd.id = (ushort)this.data[this.bottom].id;
            vd.gx = this.data[this.bottom].gx;
            vd.gy = this.data[this.bottom].gy;
            vd.vmax = this.data[this.bottom].vmax;
            vd.msec = (ushort)this.data[this.bottom].t.Millisecond;
            vd.sec = (ushort)this.data[this.bottom].t.Second;
            vd.min = (ushort)this.data[this.bottom].t.Minute;
            vd.hour = (ushort)this.data[this.bottom].t.Hour;
            vd.day = (ushort)this.data[this.bottom].t.Day;
            vd.month = (ushort)this.data[this.bottom].t.Month;
            vd.year = (ushort)this.data[this.bottom].t.Year;
            vd.kgx = this.data[this.bottom].kgx;
            vd.kgy = this.data[this.bottom].kgy;
            vd.kvx = this.data[this.bottom].kvx;
            vd.kvy = this.data[this.bottom].kvy;

            vd.x2pos = this.data[this.bottom].udpkv1.x2pos;
            vd.y2pos = this.data[this.bottom].udpkv1.y2pos;
            vd.x2v = this.data[this.bottom].udpkv1.x2v;
            vd.y2v = this.data[this.bottom].udpkv1.y2v;
            vd.az2_c = this.data[this.bottom].udpkv1.az2_c;
            vd.alt2_c = this.data[this.bottom].udpkv1.alt2_c;
            if (MtMode == 2) {
                vd.x2pos = this.data[this.bottom].udpkv1.xpos;
                vd.y2pos = this.data[this.bottom].udpkv1.ypos;
                vd.x2v = this.data[this.bottom].udpkv1.x1v;
                vd.y2v = this.data[this.bottom].udpkv1.y1v;
                vd.az2_c = this.data[this.bottom].udpkv1.az1_c;
                vd.alt2_c = this.data[this.bottom].udpkv1.alt1_c;
            }

            vd.kvaz = this.data[this.bottom].udpkv1.kvaz;
            vd.kvalt = this.data[this.bottom].udpkv1.kvalt;
            vd.vaz2_kv = this.data[this.bottom].udpkv1.vaz2_kv;
            vd.valt2_kv = this.data[this.bottom].udpkv1.valt2_kv;
            vd.kv_status = this.data[this.bottom].udpkv1.kv_status;
            vd.data_request = this.data[this.bottom].udpkv1.data_request;
            vd.mt3mode = this.data[this.bottom].udpkv1.mt3mode;
            vd.az = this.data[this.bottom].az;
            vd.alt = this.data[this.bottom].alt;
            vd.vaz = this.data[this.bottom].vaz;
            vd.valt = this.data[this.bottom].valt;

            Cv.Copy(this.img[this.bottom], imgR);

            //String str = String.Format("ID:{0,6:D1} ", this.data[this.bottom].id) + this.data[this.bottom].t.ToString("yyyyMMdd_HHmmss_fff") ;
            //imgR.PutText(str, new CvPoint(6, 14), font, new CvColor(0, 100, 120));

            Marshal.StructureToPtr(vd, imgR.ImageData, false);

            //String str = String.Format("ID:{0,10:D1} ", this.data[this.bottom].id) + this.data[this.bottom].t.ToString("yyyyMMdd_HHmmss_fff") + String.Format(" ({0,6:F1},{1,6:F1})({2,6:F1})", gx, gy, vmax);
            //if (this.data[this.bottom].ImgSaveFlag) str += " True";
            //Cv.CvtColor(this.img[this.bottom], imgGBR, ColorConversion.GrayToBgr);
            //3    Cv.Copy(this.img[this.bottom], imgGBR); //3
            //imgGBR.PutText(str, new CvPoint(6, 14) , font, new CvColor(0, 100, 100));
            //imgGBR.Circle(new CvPoint((int)(gx+0.5),(int)(gy+0.5)), 15, new CvColor(0, 100, 255));

            vw.WriteFrame(imgR);
            writer.WriteLine("{0} {1} {2}  ", vd.id, vd.kgx, vd.kgy);
        }

        /// <summary>
        /// 画像を保存します。
        /// </summary>
        /// <remarks>
        /// ビデオ書き込み開放処理
        /// </remarks>
        public void VideoWriterRelease()
        {
            if (vw != null)
            {
                vw.Dispose();
            }
            if (writer != null)
            {
                writer.Close();
            }
        }

        /// <summary>
        /// 画像を保存します。
        /// </summary>
        /// <remarks>
        /// Set save dir name
        /// </remarks>
        public void VideoWriterSetSaveDir(string s)
        {

        }
        #endregion
    }
}
