namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// 判定クラス定義インタフェース
    /// </summary>
    public interface IInferenceResultJudgementClass
    {
        /// <summary>
        /// カメラ毎のクラス名
        /// </summary>
        public List<string>[] ClassNamesPerGrabber { get; set; }
    }
}
