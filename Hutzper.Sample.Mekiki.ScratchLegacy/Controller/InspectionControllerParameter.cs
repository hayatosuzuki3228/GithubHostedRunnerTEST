using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Controller
{
    /// <summary>
    /// 検査制御パラメータ
    /// </summary>
    [Serializable]
    public record InspectionControllerParameter : ControllerParameterBaseRecord, IInspectionControllerParameter
    {
        #region IInspectionControllerParameter

        /// <summary>
        /// 結果出力タイミング
        /// </summary>
        [IniKey(true, ResultOutputTiming.Fastest)]
        public ResultOutputTiming ResultOutputTiming { get; set; } = ResultOutputTiming.Fastest;

        /// <summary>
        /// 照明点灯タイミング
        /// </summary>
        [IniKey(true, LightControlTiming.Fastest)]
        public LightControlTiming LightTurnOnTiming { get; set; } = LightControlTiming.Fastest;

        /// <summary>
        /// 照明消灯タイミング
        /// </summary>
        [IniKey(true, LightControlTiming.Fastest)]
        public LightControlTiming LightTurnOffTiming { get; set; } = LightControlTiming.Fastest;

        /// <summary>
        /// 結果出力遅延ミリ秒        
        /// </summary>
        [IniKey(true, 0)]
        public int ResultOutputDelayMs { get; set; } = 0;

        /// <summary>
        /// 照明点灯タイミング遅延ミリ秒        
        /// </summary>
        [IniKey(true, new int[] { 0 })]
        public int[] LightTurnOnDelayMs { get; set; } = new int[1];

        /// <summary>
        /// 照明消灯タイミング遅延ミリ秒        
        /// </summary>
        [IniKey(true, new int[] { 0 })]
        public int[] LightTurnOffDelayMs { get; set; } = new int[1];

        /// <summary>
        /// 撮影遅延時間ミリ秒
        /// </summary>
        /// <remarks>カメラ台数分ミリ秒単位で設定します</remarks>
        [IniKey(true, new int[] { 100 })]
        public int[] AcquisitionTriggerDelayMs { get; set; }

        /// <summary>
        /// 推論処理の最大同時実行数
        /// </summary>
        /// <remarks>0以下指定で並列処理を無効化</remarks>
        [IniKey(true, 1)]
        public int MaxDegreeOfInferenceParallelism { get; set; } = 1;

        /// <summary>
        /// 推論インデックス
        /// </summary>
        /// <remarks>ONNXファイル設定とカメラの紐づけ</remarks>
        [IniKey(true, new int[] { 0 })]
        public int[] InferenceIndecies { get; set; }

        /// <summary>
        /// 撮影トリガタイムアウト
        /// </summary>
        /// <remarks>ONしっぱなし状態になったときの挙動を決定する</remarks>
        [IniKey(true, 3000)]
        public int AcquisitionTriggerHoldingTimeoutMs { get; set; } = 3000;

        /// <summary>
        /// 撮影トリガ有効ON時間
        /// </summary>
        [IniKey(true, 100)]
        public int AcquisitionTriggerValidHoldingTimeMs { get; set; } = 100;

        /// <summary>
        /// 撮影のみ
        /// </summary>
        /// <remarks>データ収集時を想定</remarks>
        [IniKey(true, false)]
        public bool ImageAcquisitionOnly { get; set; }

        /// <summary>
        /// 基準搬送速度
        /// </summary>
        /// <remarks>遅延時間を設定したときの基準速度</remarks>
        [IniKey(true, 100d)]
        public double ReferenceConveyingSpeedMillPerSecond { get; set; }

        /// <summary>
        /// 指定搬送速度
        /// </summary>
        /// <remarks>現在の搬送速度</remarks>
        [IniKey(true, 100d)]
        public double SpecifiedConveyingSpeedMillPerSecond { get; set; }

        /// <summary>
        /// 撮影画像保存フォーマット
        /// </summary>
        [IniKey(true, AvailableImageFormat.Png)]
        public AvailableImageFormat ImageSaveFormat { get; set; } = AvailableImageFormat.Png;

        /// <summary>
        /// 画像保存キュー上限
        /// </summary>
        [IniKey(true, 10)]
        public int LocalImageSaveTaskQueueUpperLimit { get; set; } = 10;

        /// <summary>
        /// 画像保存キュー復帰下限
        /// </summary>
        [IniKey(true, 5)]
        public int LocalImageSaveTaskQueueResumeLimit { get; set; } = 5;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public InspectionControllerParameter() : this(typeof(InspectionControllerParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectionControllerParameter(string fileNameWithoutExtension) : base("Inspection_Control".ToUpper(), "InspectionControl", $"{fileNameWithoutExtension}.ini")
        {
            this.IsHierarchy = false;

            this.AcquisitionTriggerDelayMs = new int[] { 100 };
            this.InferenceIndecies = new int[] { 0 };
        }

        #endregion
    }
}