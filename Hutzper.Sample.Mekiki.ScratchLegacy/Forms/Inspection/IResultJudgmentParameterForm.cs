using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    /// <summary>
    /// 推論結果判定パラメータ画面インタフェース
    /// </summary>
    public interface IResultJudgmentParameterForm
    {
        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object>? ParameterChanged;

        /// <summary>
        /// パラメータの表示
        /// </summary>
        /// <param name="parameter">画面に表示したいパラメータ</param>
        public void ShowParameter(IInferenceResultJudgmentParameter parameter);

        /// <summary>
        /// パラメータの取得
        /// </summary>
        /// <param name="parameter">画面の表示値を反映させるパラメータ</param>
        public void UpdateParameter(IInferenceResultJudgmentParameter parameter);

        /// <summary>
        /// 画面の取得
        /// </summary>
        public Form? GetForm();
    }
}
