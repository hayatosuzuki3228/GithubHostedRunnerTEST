using Hutzper.Library.Common;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.Setting;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    public partial class ResultJudgmentParameterForm : ServiceCollectionSharingForm, IResultJudgmentParameterForm
    {
        #region IResultJudgmentParameterForm

        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object>? ParameterChanged;

        /// <summary>
        /// パラメータの表示
        /// </summary>
        /// <param name="parameter">画面に表示したいパラメータ</param>
        public void ShowParameter(IInferenceResultJudgmentParameter parameter)
        {
            if (parameter is SampleJudgementParameter jp)
            {
                this.ParameterControlls.ForEach(c => c.Visible = true);

                #region 対角線長さのしきい値(mm)
                {
                    var control = this.UcDiagonalLengthThresholdMm;

                    control.Visible = true;
                    control.ValueDouble = Math.Clamp(jp.DiagonalLengthThresholdMm, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                }
                #endregion
            }
            else
            {
                this.ParameterControlls.ForEach(c => c.Visible = false);
            }
        }

        /// <summary>
        /// パラメータの取得
        /// </summary>
        /// <param name="parameter">画面の表示値を反映させるパラメータ</param>
        public void UpdateParameter(IInferenceResultJudgmentParameter parameter)
        {
            if (parameter is SampleJudgementParameter jp)
            {
                #region 対角線長さのしきい値(mm)
                {
                    var control = this.UcDiagonalLengthThresholdMm;
                    jp.DiagonalLengthThresholdMm = control.ValueDouble;
                }
                #endregion
            }
        }

        /// <summary>
        /// 画面の取得
        /// </summary>
        public Form? GetForm() => this;

        #endregion

        /// <summary>
        /// パラメータ設定用コントロールリスト
        /// </summary>
        private List<Control> ParameterControlls = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResultJudgmentParameterForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResultJudgmentParameterForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();

            // コントロールのリスト化
            this.ParameterControlls.Add(this.UcDiagonalLengthThresholdMm);

            // 対角線長さのしきい値(mm)
            this.UcDiagonalLengthThresholdMm.NumericUpDownMaximum = 9999;
            this.UcDiagonalLengthThresholdMm.NumericUpDownMinimum = 0;
            this.UcDiagonalLengthThresholdMm.NumericUpDownDecimalPlaces = 2;

            // イベントの設定
            foreach (var control in this.ParameterControlls)
            {
                if (control is NumericUpDownUserControl nuc)
                {
                    nuc.ValueChanged += this.UcParam_ValueChanged;
                }
            }
        }

        private void UcParam_ValueChanged(object sender, decimal value)
        {
            this.ParameterChanged?.Invoke(this);
        }

        private void UcButtonExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
