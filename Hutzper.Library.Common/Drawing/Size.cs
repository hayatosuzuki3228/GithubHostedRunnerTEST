using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 順序を付与した整数の対 (通常、四角形の幅と高さ) を格納します。
    /// </summary>
    [Serializable]
    public class Size
    {
        #region プロパティ

        /// <summary>
        /// 幅
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 対角線長
        /// </summary>
        public double DiagonalLength
        {
            #region 取得
            get
            {
                return System.Math.Sqrt(System.Math.Pow(this.Width, 2) + System.Math.Pow(this.Height, 2));
            }
            #endregion
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Size() : this(0, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public Size(Size source) : this(source.Width, source.Height)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public Size(System.Drawing.Size source) : this(source.Width, source.Height)
        {
        }

        public Size(SizeD size) : this((int)size.Width, (int)size.Height)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="size"></param>
        public void Assign(Size size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="size"></param>
        public void Assign(System.Drawing.Size size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public Size Clone()
        {
            return Size.New(this);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Size ToSize()
        {
            return new System.Drawing.Size(this.Width, this.Height);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.SizeF ToSizeF()
        {
            return new System.Drawing.SizeF(this.Width, this.Height);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public SizeD ToSizeD()
        {
            return new SizeD(this.Width, this.Height);
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public override bool Equals(object? compare)
        {
            bool isEqual = false;

            try
            {
                Size? size = compare as Size;

                if (null != size)
                {
                    if ((true == this.Width.Equals(size.Width))
                    && (true == this.Height.Equals(size.Height)))
                    {
                        isEqual = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isEqual = false;
                Debug.WriteLine(this, ex.Message);
            }

            return isEqual;
        }

        /// <summary>
        /// ハッシュコード
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region スタティックメソッド

        public static Size New()
        {
            return new Size();
        }

        public static Size New(int width, int height)
        {
            return new Size(width, height);
        }

        public static Size New(Size source)
        {
            return new Size(source);
        }

        public static Size Convert(System.Drawing.Size size)
        {
            return new Size(size);
        }

        public static System.Drawing.Size Convert(Size source)
        {
            return source.ToSize();
        }

        #endregion
    }
}