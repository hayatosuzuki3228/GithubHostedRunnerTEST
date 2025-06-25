using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageProcessing.Controller
{
    /// <summary>
    /// Run Length Encoding制御パラメータ
    /// </summary>
    public interface IRleControllerParameter : IControllerParameter
    {
        /// <summary>
        /// 最大ライン数
        /// </summary>
        public int MaximumLineNumber { get; set; }

        /// <summary>
        /// 最大ラベル数
        /// </summary>
        public int MaximumLabelNumber { get; set; }

        /// <summary>
        /// 最大Rle数
        /// </summary>
        public int MaximumRleNumber { get; set; }

        /// <summary>
        /// ラベリングウェイト時間ミリ秒
        /// </summary>
        public int LabelingThreadRestPeriodsMs { get; set; }
    }
}