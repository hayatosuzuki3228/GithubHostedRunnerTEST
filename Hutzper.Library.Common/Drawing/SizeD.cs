using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 順序を付与した実数の対 (通常、四角形の幅と高さ) を格納します。
    /// </summary>
    [Serializable]
    public class SizeD
    {
        #region プロパティ

        /// <summary>
        /// 幅
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public double Height { get; set; }

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
        public SizeD() : this(0, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public SizeD(SizeD source) : this(source.Width, source.Height)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public SizeD(Size source) : this(source.Width, source.Height)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public SizeD(System.Drawing.Size source) : this(source.Width, source.Height)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public SizeD(System.Drawing.SizeF source) : this(source.Width, source.Height)
        {
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SizeD(double width, double height)
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
        public void Assign(SizeD size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

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
        public void Assign(System.Drawing.SizeF size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public SizeD Clone()
        {
            return SizeD.New(this);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public Size ToSizeInt()
        {
            return new Size((int)this.Width, (int)this.Height);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Size ToSize()
        {
            return new System.Drawing.Size((int)this.Width, (int)this.Height);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.SizeF ToSizeF()
        {
            return new System.Drawing.SizeF((float)this.Width, (float)this.Height);
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
                var size = compare as SizeD;

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

        public static SizeD New()
        {
            return new SizeD();
        }

        public static SizeD New(double width, double height)
        {
            return new SizeD(width, height);
        }

        public static SizeD New(SizeD source)
        {
            return new SizeD(source);
        }

        public static SizeD Convert(System.Drawing.SizeF size)
        {
            return new SizeD(size);
        }

        public static System.Drawing.SizeF Convert(SizeD source)
        {
            return source.ToSizeF();
        }

        #endregion
    }
}