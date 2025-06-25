using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageGrabber.Data
{
    /// <summary>
    /// Byte型一次元配列データ
    /// </summary>
    /// <remarks>Bitmapデータ保持タイプ</remarks>
    [Serializable]
    public class ByteArrayGrabberDataFastBitmapGeneration : ByteArrayGrabberData, IByteArrayGrabberData
    {
        #region IGrabberData

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <returns></returns>
        public override Bitmap? ToBitmap() => (Bitmap?)this.Bitmap?.Clone();

        /// <summary>
        /// コピー(DeepCopy)
        /// </summary>
        /// <returns></returns>
        public override IGrabberData? Clone()
        {
            var copyInstance = default(IGrabberData);

            try
            {
                var instance = new ByteArrayGrabberDataFastBitmapGeneration
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

                instance.Bitmap = (Bitmap?)this.Bitmap?.Clone();

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
                base.DisposeExplicit();
                this.DisposeSafely(this.Bitmap);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// Bitmapデータ
        /// </summary>
        public Bitmap? Bitmap { get; set; } = null;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteArrayGrabberDataFastBitmapGeneration() : this(0, 0)
        {

        }

        public ByteArrayGrabberDataFastBitmapGeneration(int x, int y) : this(new Common.Drawing.Point(x, y))
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ByteArrayGrabberDataFastBitmapGeneration(Common.Drawing.Point location) : base(location)
        {
        }

        #endregion
    }
}