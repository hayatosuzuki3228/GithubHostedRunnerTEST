using Hutzper.Library.Common.Drawing;

namespace Hutzper.Library.ImageProcessing.Data
{
    [Serializable]
    public class RleLabel : IRleLabel
    {
        #region プロパティ

        /// <summary>
        /// ラベルインデックス
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 面積
        /// </summary>
        public int PixelNumber { get; set; }

        /// <summary>
        /// 矩形始点
        /// </summary>
        public Point RectBegin { get; protected set; }

        /// <summary>
        /// 矩形終点
        /// </summary>
        public Point RectEnd { get; protected set; }

        /// <summary>
        /// 矩形サイズ
        /// </summary>
        public Size RectSize { get; protected set; }

        /// <summary>
        /// Rleデータ要素
        /// </summary>
        public ushort RleDataElem { get; set; }

        /// <summary>
        /// Rleデータ数
        /// </summary>
        public int RleDataNumber { get; set; }

        /// <summary>
        /// Rleデータ始点位置
        /// </summary>
        public int RleDataBegin { get; set; }

        /// <summary>
        /// Rleデータ終点位置
        /// </summary>
        public int RleDataEnd { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RleLabel(int index)
        {
            this.Index = index;
            this.RectBegin = new Point();
            this.RectEnd = new Point();
            this.RectSize = new Size();
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="source"></param>
        public RleLabel(RleLabel source)
        {
            this.Index = source.Index;
            this.Enabled = source.Enabled;
            this.PixelNumber = source.PixelNumber;
            this.RectBegin = new Point(source.RectBegin.X, source.RectBegin.Y);
            this.RectEnd = new Point(source.RectEnd.X, source.RectEnd.Y);
            this.RectSize = new Size(source.RectSize.Width, source.RectSize.Height);
            this.RleDataElem = source.RleDataElem;
            this.RleDataNumber = source.RleDataNumber;
            this.RleDataBegin = source.RleDataBegin;
            this.RleDataEnd = source.RleDataEnd;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public IRleLabel Clone()
        {
            return new RleLabel(this);
        }

        #endregion
    }
}