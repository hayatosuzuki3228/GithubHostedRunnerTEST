using Hutzper.Library.Common;
using Hutzper.Library.ImageProcessing;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageGrabber.Data
{
    /// <summary>
    /// Bitmap型データ
    /// </summary>
    [Serializable]
    public class BitmapGrabberData : SafelyDisposable, IGrabberData
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
        public virtual System.Drawing.Imaging.PixelFormat PixelFormat
        {
            get => this.Image?.PixelFormat ?? System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            set { }
        }

        /// <summary>
        /// サイズ
        /// </summary>
        public Hutzper.Library.Common.Drawing.Size Size
        {
            get => new(this.Image?.Size ?? new System.Drawing.Size());
            set
            {
            }
        }

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <returns></returns>
        public virtual Bitmap? ToBitmap() => (Bitmap?)this.Image?.Clone();

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Read(FileInfo fileInfo)
        {
            try
            {
                var bitmap = new Bitmap(fileInfo.FullName);

                if (null != bitmap)
                {
                    this.DisposeSafely(this.Image);
                    this.Image = bitmap;

                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// ファイル書き込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Write(FileInfo fileInfo)
        {
            return BitmapProcessor.Write(fileInfo, this.Image);
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
                var instance = new BitmapGrabberData
                {
                    Image = BitmapProcessor.Clone(this.Image)
                ,
                    Location = this.Location.Clone()
                ,
                    Counter = this.Counter
                };
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
                this.DisposeSafely(this.Image);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 画像
        /// </summary>
        public Bitmap? Image { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BitmapGrabberData() : this(0, 0)
        {

        }

        public BitmapGrabberData(int x, int y) : this(new Common.Drawing.Point(x, y))
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BitmapGrabberData(Common.Drawing.Point location)
        {
            this.Location = new Common.Drawing.Point(location);
        }

        #endregion
    }
}