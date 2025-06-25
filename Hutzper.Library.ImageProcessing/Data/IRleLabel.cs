using Hutzper.Library.Common.Drawing;

namespace Hutzper.Library.ImageProcessing.Data
{
    /// <summary>
    /// Rleラベルインタフェース
    /// </summary>
    public interface IRleLabel
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
        public Point RectBegin { get; }

        /// <summary>
        /// 矩形終点
        /// </summary>
        public Point RectEnd { get; }

        /// <summary>
        /// 矩形サイズ
        /// </summary>
        public Size RectSize { get; }

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

        #region パブリックメソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public IRleLabel Clone();

        #endregion
    }
}