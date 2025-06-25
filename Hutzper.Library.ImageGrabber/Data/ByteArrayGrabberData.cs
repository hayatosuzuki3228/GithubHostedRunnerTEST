using Hutzper.Library.Common;
using Hutzper.Library.ImageProcessing;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Data
{
    /// <summary>
    /// Byte型一次元配列データ
    /// </summary>
    [Serializable]
    public class ByteArrayGrabberData : SafelyDisposable, IByteArrayGrabberData
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
        /// ピクセルフォーマット
        /// </summary>
        public virtual System.Drawing.Imaging.PixelFormat PixelFormat { get; set; } = PixelFormat.Format8bppIndexed;

        /// <summary>
        /// サイズ
        /// </summary>
        public virtual Hutzper.Library.Common.Drawing.Size Size { get; set; } = new Common.Drawing.Size();

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <returns></returns>
        public virtual Bitmap? ToBitmap()
        {
            try
            {
                var bitmap = (Bitmap?)null;

                if (this.Image is not null)
                {
                    switch (this.PixelFormat)
                    {
                        case PixelFormat.Format8bppIndexed:
                            {
                                bitmap = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format8bppIndexed);
                                BitmapProcessor.AssignColorPaletteOfDefault(bitmap);
                            }
                            break;

                        case PixelFormat.Format24bppRgb:
                            {
                                bitmap = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format24bppRgb);
                            }
                            break;

                        default:
                            {
                                bitmap = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format32bppArgb);
                            }
                            break;
                    }

                    var imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                    try
                    {
                        Marshal.Copy(this.Image, 0, imageData.Scan0, this.Image.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(imageData);
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public virtual bool Read(FileInfo fileInfo)
        {
            using var bitmap = BitmapProcessor.Read(fileInfo);

            this.Image = Array.Empty<byte>();

            if (null != bitmap)
            {
                this.Size.Width = bitmap.Width;
                this.Size.Height = bitmap.Height;
                this.PixelFormat = bitmap.PixelFormat;

                var imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                try
                {
                    this.Image = new byte[imageData.Stride * bitmap.Height];
                    this.Stride = imageData.Stride;
                    Marshal.Copy(imageData.Scan0, this.Image, 0, this.Image.Length);
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
        public virtual IGrabberData? Clone()
        {
            var copyInstance = default(IGrabberData);

            try
            {
                var instance = new ByteArrayGrabberData
                {
                    Location = this.Location.Clone()
                ,
                    PixelFormat = this.PixelFormat
                ,
                    Size = this.Size.Clone()
                ,
                    Counter = this.Counter
                ,
                    Image = new byte[this.Image.Length]
                ,
                    Stride = this.Stride
                };

                Array.Copy(this.Image, instance.Image, instance.Image.Length);

                copyInstance = instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return copyInstance;
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
                this.Image = Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region IByteArrayGrabberData

        /// <summary>
        /// 画像
        /// </summary>
        public byte[] Image { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// ストライド
        /// </summary>
        public int Stride { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteArrayGrabberData() : this(0, 0)
        {

        }

        public ByteArrayGrabberData(int x, int y) : this(new Common.Drawing.Point(x, y))
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteArrayGrabberData(Common.Drawing.Point location)
        {
            this.Location = new Common.Drawing.Point(location);
        }

        #endregion
    }
}