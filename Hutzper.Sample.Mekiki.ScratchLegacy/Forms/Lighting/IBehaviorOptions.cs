namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting
{
    /// <summary>
    /// 挙動設定インタフェース
    /// </summary>
    public interface IBehaviorOptions
    {
        /// <summary>
        /// 照明点灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOnTiming { get; set; }

        /// <summary>
        /// 照明消灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOffTiming { get; set; }

        /// <summary>
        /// 照明点灯タイミング遅延ミリ秒        
        /// </summary>
        public int LightTurnOnDelayMs { get; set; }

        /// <summary>
        /// 照明消灯タイミング遅延ミリ秒        
        /// </summary>
        public int LightTurnOffDelayMs { get; set; }
    }

    /// <summary>
    /// IBehaviorOptions実装
    /// </summary>
    /// <remarks>呼び出し側でIBehaviorOptionsを敬称したクラスを作成したくない場合に手っ取り早くコンテナとして利用することを想定</remarks>
    public record BehaviorOptions : IBehaviorOptions
    {
        /// <summary>
        /// 照明点灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOnTiming { get; set; }

        /// <summary>
        /// 照明消灯タイミング
        /// </summary>
        public LightControlTiming LightTurnOffTiming { get; set; }

        /// <summary>
        /// 照明点灯タイミング遅延ミリ秒        
        /// </summary>
        public int LightTurnOnDelayMs { get; set; }

        /// <summary>
        /// 照明消灯タイミング遅延ミリ秒        
        /// </summary>
        public int LightTurnOffDelayMs { get; set; }
    }

}
