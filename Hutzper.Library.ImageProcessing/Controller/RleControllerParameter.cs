using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageProcessing.Controller
{
    /// <summary>
    /// Run Length Encoding制御パラメータ
    /// </summary>
    [Serializable]
    public record RleControllerParameter : ControllerParameterBaseRecord, IRleControllerParameter
    {
        #region IRleControllerParameter

        /// <summary>
        /// 最大ライン数
        /// </summary>
        [IniKey(true, 5000)]
        public int MaximumLineNumber { get; set; } = 5000;

        /// <summary>
        /// 最大ラベル数
        /// </summary>
        [IniKey(true, 50000)]
        public int MaximumLabelNumber { get; set; } = 50000;

        /// <summary>
        /// 最大Rle数
        /// </summary>
        [IniKey(true, 3000000)]
        public int MaximumRleNumber { get; set; } = 3000000;

        /// <summary>
        /// ラベリングウェイト時間ミリ秒
        /// </summary>
        [IniKey(true, 10)]
        public int LabelingThreadRestPeriodsMs { get; set; } = 10;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RleControllerParameter() : this(typeof(RleControllerParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileNameWithoutExtension"></param>
        public RleControllerParameter(string fileNameWithoutExtension) : base("Rle_Controller".ToUpper(), "RleControllerParameter", $"{fileNameWithoutExtension}.ini")
        {
            this.IsHierarchy = false;
        }

        #endregion
    }
}