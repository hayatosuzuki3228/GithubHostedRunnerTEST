namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// IInferenceResultJudgementClass実装サンプル
    /// </summary>
    internal class SampleJudgementClass : IInferenceResultJudgementClass
    {
        #region IInferenceResultJudgementClass

        /// <summary>
        /// カメラ別クラス名
        /// </summary>
        public List<string>[] ClassNamesPerGrabber { get; set; } = Array.Empty<List<string>>();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SampleJudgementClass()
        {

        }
    }
}
