namespace Hutzper.Library.Common.Imaging
{
    /// <summary>
    /// 画像に関する設定を持つインタフェース
    /// </summary>
    public interface IImageProperties
    {
        /// <summary>
        /// 識別
        /// </summary>
        public Library.Common.Drawing.Point Location { get; }

        /// <summary>
        /// 撮影分解能 mm/pixel
        /// </summary>
        double ResolutionMmPerPixel { get; set; }
    }
}
