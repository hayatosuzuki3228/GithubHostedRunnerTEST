namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    /// <summary>
    /// 挙動設定インタフェース
    /// </summary>
    public interface IBehaviorOptions
    {
        /// <summary>
        /// 撮影遅延時間ミリ秒
        /// </summary>
        public int AcquisitionTriggerDelayMs { get; set; }

        /// <summary>
        /// ライブ撮影調整用の画像高さ
        /// </summary>
        public int HeightForLiveAdjustment { get; set; }
    }

    /// <summary>
    /// IBehaviorOptions実装
    /// </summary>
    /// <remarks>呼び出し側でIBehaviorOptionsを敬称したクラスを作成したくない場合に手っ取り早くコンテナとして利用することを想定</remarks>
    public record BehaviorOptions : IBehaviorOptions
    {
        /// <summary>
        /// 撮影遅延時間ミリ秒
        /// </summary>
        public int AcquisitionTriggerDelayMs { get; set; }

        /// <summary>
        /// ライブ撮影調整用の画像高さ
        /// </summary>
        public int HeightForLiveAdjustment { get; set; } = 256;
    }
}
