using Hutzper.Library.Common.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Controller
{
    /// <summary>
    /// 検査制御パラメータ
    /// </summary>
    public interface IInspectionControllerParameter : IControllerParameter
    {
        /// <summary>
        /// 結果出力タイミング
        /// </summary>
        public ResultOutputTiming ResultOutputTiming { get; set; }

        /// <summary>
        /// 照明点灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOnTiming { get; set; }

        /// <summary>
        /// 照明消灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOffTiming { get; set; }

        /// <summary>
        /// 結果出力遅延ミリ秒        
        /// </summary>
        public int ResultOutputDelayMs { get; set; }

        /// <summary>
        /// 照明点灯タイミング遅延ミリ秒        
        /// </summary>
        public int[] LightTurnOnDelayMs { get; set; }

        /// <summary>
        /// 照明消灯タイミング遅延ミリ秒        
        /// </summary>
        public int[] LightTurnOffDelayMs { get; set; }

        /// <summary>
        /// 撮影遅延時間ミリ秒
        /// </summary>
        /// <remarks>カメラ台数分ミリ秒単位で設定します</remarks>
        public int[] AcquisitionTriggerDelayMs { get; set; }

        /// <summary>
        /// 推論処理の最大同時実行数
        /// </summary>
        /// <remarks>0以下指定で並列処理を無効化</remarks>
        public int MaxDegreeOfInferenceParallelism { get; set; }

        /// <summary>
        /// 推論インデックス
        /// </summary>
        /// <remarks>ONNXファイル設定とカメラの紐づけ</remarks>
        public int[] InferenceIndecies { get; set; }

        /// <summary>
        /// 撮影トリガタイムアウト
        /// </summary>
        /// <remarks>ONしっぱなし状態になったときの挙動を決定する</remarks>
        public int AcquisitionTriggerHoldingTimeoutMs { get; set; }

        /// <summary>
        /// 撮影トリガ有効ON時間
        /// </summary>
        public int AcquisitionTriggerValidHoldingTimeMs { get; set; }

        /// <summary>
        /// 撮影のみ
        /// </summary>
        /// <remarks>データ収集時を想定</remarks>
        public bool ImageAcquisitionOnly { get; set; }

        /// <summary>
        /// 基準搬送速度
        /// </summary>
        /// <remarks>遅延時間を設定したときの基準速度</remarks>
        public double ReferenceConveyingSpeedMillPerSecond { get; set; }

        /// <summary>
        /// 指定搬送速度
        /// </summary>
        /// <remarks>現在の搬送速度</remarks>
        public double SpecifiedConveyingSpeedMillPerSecond { get; set; }

        /// <summary>
        /// 撮影画像保存フォーマット
        /// </summary>
        public AvailableImageFormat ImageSaveFormat { get; set; }

        /// <summary>
        /// 画像保存キュー上限
        /// </summary>
        public int LocalImageSaveTaskQueueUpperLimit { get; set; }

        /// <summary>
        /// 画像保存キュー復帰下限
        /// </summary>
        public int LocalImageSaveTaskQueueResumeLimit { get; set; }
    }
}