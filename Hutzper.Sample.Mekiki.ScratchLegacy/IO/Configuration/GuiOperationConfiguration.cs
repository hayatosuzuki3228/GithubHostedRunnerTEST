using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Forms.ImageView;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration
{
    /// <summary>
    /// 運用画面の設定
    /// </summary>
    /// <remarks>エリアとライン別に使用するカメラを指定します</remarks>
    [Serializable]
    public record GuiOperationConfiguration : IniFileCompatible<GuiOperationConfiguration>
    {
        /// <summary>
        /// 変化点ログ出力ボタンを表示するかどうか
        /// </summary>
        [IniKey(true, false)]
        public bool UseButtonChangePointLogOutput { get; set; }

        /// <summary>
        /// ヒストグラムの表示を非同期で行うかどうか
        /// </summary>
        [IniKey(true, false)]
        public bool IsHistogramDisplayAsync { get; set; }

        /// <summary>
        /// マルチ画像レイアウトの種類
        /// </summary>
        [IniKey(true, MultiImageLayoutType.Spotlight)]
        public MultiImageLayoutType MultiImageLayoutType { get; set; }

        /// <summary>
        /// 検査結果統計情報のリセットタイミング
        /// </summary>
        [IniKey(true, StatisticsResetTiming.Daily)]
        public StatisticsResetTiming StatisticsResetTiming { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GuiOperationConfiguration() : base("GUI_Operation".ToUpper())
        {
        }
    }
}