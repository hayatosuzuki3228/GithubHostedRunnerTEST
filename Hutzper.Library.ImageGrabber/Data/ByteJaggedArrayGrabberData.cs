using Hutzper.Library.Common;
using Hutzper.Library.ImageProcessing;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Data
{
    /// <summary>
    /// ジャグ配列画像データ
    /// </summary>
    [Serializable]
    public class ByteJaggedArrayGrabberData : SafelyDisposable, IByteJaggedArrayGrabberData
    {
        #region IGrabberData

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; init; }

        /// <summary>
        /// 汎用カウンタ値
        /// </summary>
        public ulong Counter { get; set; }

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <returns></returns>
        public virtual Bitmap? ToBitmap()
        {
            var bitmap = (Bitmap?)null;

            try
            {

                if (this.Image is not null && 0 < this.Size.Width && 0 < this.Size.Height)
                {
                    BitmapProcessor.ConvertToBitmap(this.Image, this.Size, this.PixelFormat, out bitmap);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return bitmap;
        }

        /// <summary>
        /// ピクセルフォーマット
        /// </summary>
        public virtual System.Drawing.Imaging.PixelFormat PixelFormat { get; set; } = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;

        /// <summary>
        /// サイズ
        /// </summary>
        public virtual Hutzper.Library.Common.Drawing.Size Size { get; set; } = new Common.Drawing.Size();

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public virtual bool Read(FileInfo fileInfo)
        {
            using var bitmap = BitmapProcessor.Read(fileInfo);

            this.Size = new Common.Drawing.Size();

            if (null != bitmap)
            {
                var imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                try
                {
                    var renewArray = this.Image;
                    if (renewArray.Length < bitmap.Height || renewArray[0].Length != imageData.Stride)
                    {
                        renewArray = new byte[bitmap.Height][];
                    }

                    foreach (var i in Enumerable.Range(0, bitmap.Height))
                    {
                        if (renewArray[i] is null)
                        {
                            renewArray[i] = new byte[imageData.Stride];
                        }
                        Marshal.Copy(imageData.Scan0 + imageData.Stride * i, renewArray[i], 0, imageData.Stride);
                    }

                    this.Image = renewArray;
                    this.Size.Width = bitmap.Width;
                    this.Size.Height = bitmap.Height;
                    this.PixelFormat = bitmap.PixelFormat;
                }
                finally
                {
                    bitmap.UnlockBits(imageData);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// ファイル書き込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public virtual bool Write(FileInfo fileInfo)
        {
            using var bitmap = this.ToBitmap();

            return BitmapProcessor.Write(fileInfo, bitmap);
        }

        /// <summary>
        /// コピー(DeepCopy)
        /// </summary>
        /// <returns></returns>
        public IGrabberData? Clone()
        {
            var copyInstance = default(IGrabberData);

            try
            {
                var instance = new ByteJaggedArrayGrabberData
                {
                    Location = this.Location.Clone()
                ,
                    PixelFormat = this.PixelFormat
                ,
                    Size = this.Size.Clone()
                ,
                    Counter = this.Counter
                ,
                    Image = new byte[this.Image.Length][]
                };

                foreach (var i in Enumerable.Range(0, this.Image.Length))
                {
                    instance.Image[i] = new byte[this.Image[i].Length];
                    Array.Copy(this.Image[i], instance.Image[i], instance.Image[i].Length);
                }

                copyInstance = instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return copyInstance;
        }

        #endregion

        #region IByteJaggedArrayGrabberData

        /// <summary>
        /// 画像
        /// </summary>
        public byte[][] Image { get; set; } = Array.Empty<byte[]>();

        /// <summary>
        /// 画像の連結
        /// </summary>
        /// <param name="data"></param>
        public void ConcatImage(params IByteJaggedArrayGrabberData[] data)
        {
            try
            {
                var candidateDatas = data.Where(d =>
                    this.Size.Width == d.Size.Width
                && this.PixelFormat == d.PixelFormat
                && 0 < d.Size.Height
                );

                var totalHeight = this.Size.Height;
                totalHeight += candidateDatas.Sum(d => d.Size.Height);

                var concatArray = this.Image;
                var concatIndex = this.Size.Height;
                if (concatArray.Length < totalHeight)
                {
                    concatArray = new byte[totalHeight][];
                    Array.Copy(this.Image, concatArray, this.Size.Height);
                }

                foreach (var d in candidateDatas)
                {
                    Array.Copy(d.Image, 0, concatArray, concatIndex, d.Size.Height);
                    concatIndex += d.Size.Height;
                }

                this.Image = concatArray;
                this.Size.Height = totalHeight;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// 配列化
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var bytes = Array.Empty<byte>();

            try
            {
                if (0 < this.Size.Height)
                {
                    bytes = new byte[this.Image.Length * this.Image[0].Length];

                    var offset = 0;
                    foreach (var i in Enumerable.Range(0, this.Image.Length))
                    {
                        Array.Copy(this.Image[i], 0, bytes, offset, this.Image[i].Length);
                        offset += this.Image[i].Length;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return bytes;
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Image = Array.Empty<byte[]>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteJaggedArrayGrabberData(int capacity = -1) : this(0, 0, capacity)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteJaggedArrayGrabberData(int x, int y, int capacity = -1) : this(new Common.Drawing.Point(x, y), capacity)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteJaggedArrayGrabberData(Common.Drawing.Point location, int capacityOfHeight = -1)
        {
            this.Location = new Common.Drawing.Point(location);

            if (0 < capacityOfHeight)
            {
                this.Image = new byte[capacityOfHeight][];
            }
        }

        #endregion
    }
}