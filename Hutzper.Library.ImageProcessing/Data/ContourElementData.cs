namespace Hutzper.Library.ImageProcessing.Data
{
    using Hutzper.Library.Common.Drawing;

    /// <summary>
    /// 輪郭データ
    /// </summary>
    [Serializable]
    public class ContourElementData
    {
        #region プロパティ

        /// <summary>
        /// データインデックス
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 次データインデックス
        /// </summary>
        public int NextIndex { get; set; }

        /// <summary>
        /// 座標
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// 重心からの距離
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// 重心からのX方向相対座標
        /// </summary>
        public double RelativeX { get; set; }

        /// <summary>
        /// 重心からのY方向相対座標
        /// </summary>
        public double RelativeY { get; set; }

        /// <summary>
        /// 重心からの向き
        /// </summary>
        /// <remarks>ラジアン</remarks>
        public double Direction { get; set; }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        public object? Tag
        {
            #region 取得
            get
            {
                return this.tag;
            }
            #endregion

            #region 更新
            set
            {
                this.tag = value;
            }
            #endregion
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 汎用タグ
        /// </summary>
        [NonSerialized]
        protected object? tag;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourElementData() : this(-1)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourElementData(int index)
        {
            this.Index = index;
            this.NextIndex = this.Index + 1;
            this.Point = Point.New();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourElementData(int index, Point point) : this(index, point, 0, 0, 0, 0, null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourElementData(int index, Point point, double len, double lenX, double lenY, double direction)
        : this(index, point, len, lenX, lenY, direction, null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourElementData(int index, Point point, double len, double lenX, double lenY, double direction, object? tag)
        {
            this.Index = index;
            this.NextIndex = this.Index + 1;
            this.Point = new Point(point);
            this.Distance = len;
            this.RelativeX = lenX;
            this.RelativeY = lenY;
            this.Direction = direction;
            this.Tag = tag;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public ContourElementData(ContourElementData source)
        {
            this.Index = source.Index;
            this.NextIndex = source.NextIndex;
            this.Point = new Point(source.Point);
            this.Distance = source.Distance;
            this.RelativeX = source.RelativeX;
            this.RelativeY = source.RelativeY;
            this.Direction = source.Direction;
            this.Tag = source.Tag;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:D}:{1}", this.Index, this.Point.ToString());
        }

        #endregion
    }
}