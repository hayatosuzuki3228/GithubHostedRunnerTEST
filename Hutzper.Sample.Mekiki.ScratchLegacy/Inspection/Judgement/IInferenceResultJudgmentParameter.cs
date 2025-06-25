using Hutzper.Library.Common.Imaging;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// 推論結果判定パラメータインタフェース
    /// </summary>
    public interface IInferenceResultJudgmentParameter
    {
        /// <summary>
        /// 全体のクラス名
        /// </summary>
        public string[] AllClassNames { get; }

        /// <summary>
        /// クラス定義
        /// </summary>
        public IInferenceResultJudgementClass? JudgementClass { get; }

        /// <summary>
        /// 画像情報
        /// </summary>
        /// <remarks>カメラ台数分の撮影分解能を格納します</remarks>
        public IImageProperties[] ImageProperties { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="judgementClass">クラス名の定義</param>
        void Initialize(IInferenceResultJudgementClass judgementClass);
    }
}