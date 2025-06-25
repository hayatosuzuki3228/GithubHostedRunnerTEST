using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Common.Imaging
{
    /// <summary>
    /// 画像に関する設定
    /// </summary>
    [Serializable]
    public record ImagingSetting : IniFileCompatible<ImagingSetting>, IImageProperties
    {
        #region プロパティ

        /// <summary>
        /// 識別
        /// </summary>
        public Library.Common.Drawing.Point Location => this.location.Clone();

        /// <summary>
        /// 撮影分解能 mm/pixel
        /// </summary>
        [IniKey(true, 1d)]
        public double ResolutionMmPerPixel { get; set; } = 1d;

        #endregion

        #region フィールド

        protected Library.Common.Drawing.Point location = new();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImagingSetting(Library.Common.Drawing.Point location) : base($"Imaging_Y{location.Y + 1:D2}_X{location.X + 1:D2}")
        {
            this.location = location.Clone();
        }
    }
}
