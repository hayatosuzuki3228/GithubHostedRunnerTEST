using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration
{
    /// <summary>
    /// 運用設定
    /// </summary>
    [Serializable]
    public record OperationSetting : IniFileCompatible<OperationSetting>
    {
        /// <summary>
        /// 起動時に自動で検査を有効化するかどうか
        /// </summary>
        [IniKey(true, false)]
        public bool ActivateInspectionAtStartup { get; set; } = false;

        /// <summary>
        /// ストレージ空き容量が少なくなった時に自動で古い画像から削除するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool IsAutoDeleteOldestImage { get; set; } = true;

        /// <summary>
        /// ストレージ空き容量が少なくなった時に自動で古い画像から削除する場合の下限値%
        /// </summary>
        [IniKey(true, 5.0)]
        public double LowerLimitPercentageOfAutoDeleteImage { get; set; } = 5.0;

        /// <summary>
        /// 画像保存条件
        /// </summary>
        [IniKey(true, ImageSavingCondition.Always)]
        public ImageSavingCondition ImageSavingCondition { get; set; } = ImageSavingCondition.Always;

        /// <summary>
        /// 画像保存間隔秒(件数より時間の方が汎用的に保存負荷を制御できる)
        /// </summary>
        /// <remarks>最後に保存された時刻から設定された時間間隔が経過している場合に保存する</remarks>
        [IniKey(true, 5d)]
        public double ImageSavingIntervalSeconds { get; set; } = 5d;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OperationSetting() : base("Operation".ToUpper())
        {
        }
    }
}