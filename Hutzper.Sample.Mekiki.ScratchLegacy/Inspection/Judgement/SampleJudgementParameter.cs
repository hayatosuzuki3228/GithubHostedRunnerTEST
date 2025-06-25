using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Imaging;
using Hutzper.Library.Common.IO;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// 判定ロジック用パラメータサンプル
    /// </summary>
    /// <remarks>IInferenceResultJudgmentParameterを使ったサンプルコードです。</remarks>
    [Serializable]
    public record SampleJudgementParameter : IniFileCompatible<SampleJudgementParameter>, IInferenceResultJudgmentParameter
    {
        /// <summary>
        /// 対角線長さのしきい値(mm)
        /// </summary>
        /// <remarks>この例ではクラス別にしきい値を設けていません</remarks>
        [IniKey(true, 1d)]
        public double DiagonalLengthThresholdMm { get; set; } = 1d;

        /// <summary>
        /// 結合距離しきい値(mm)
        /// </summary>
        /// <remarks>外接矩形同士の距離が近い場合に統合して扱うかを決定するしきい値</remarks>
        [IniKey(true, 0)]
        public double MergeDistanceThresholdMm { get; set; } = 0;

        #region IInferenceResultJudgmentParameter

        /// <summary>
        /// 全体のクラス名
        /// </summary>
        public string[] AllClassNames { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// クラス定義
        /// </summary>
        public IInferenceResultJudgementClass? JudgementClass { get; private set; }

        /// <summary>
        /// 画像情報
        /// </summary>
        /// <remarks>カメラ台数分の撮影分解能を格納します</remarks>
        public IImageProperties[] ImageProperties { get; set; } = Array.Empty<IImageProperties>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(IInferenceResultJudgementClass judgementClass)
        {
            this.JudgementClass = judgementClass;

            this.ImageProperties = new IImageProperties[judgementClass.ClassNamesPerGrabber.Length];

            var tempClassNames = new List<string>();
            foreach (var names in judgementClass.ClassNamesPerGrabber)
            {
                // okは先頭としてソート
                names.Sort((x, y) =>
                {
                    if (x == "ok" && y != "ok")
                        return -1;
                    if (x != "ok" && y == "ok")
                        return 1;

                    return string.Compare(x, y, StringComparison.Ordinal);
                });

                // カメラ毎のクラスを統合
                foreach (var name in names)
                {
                    var formatedName = name.Trim().ToLower();

                    if (true == string.IsNullOrEmpty(formatedName))
                    {
                        continue;
                    }

                    if (false == tempClassNames.Contains(formatedName))
                    {
                        tempClassNames.Add(formatedName);
                    }
                }
            }

            // okは先頭としてソート
            tempClassNames.Sort((x, y) =>
            {
                if (x == "ok" && y != "ok")
                    return -1;
                if (x != "ok" && y == "ok")
                    return 1;

                return string.Compare(x, y, StringComparison.Ordinal);
            });

            // 統合されたクラス名
            this.AllClassNames = tempClassNames.ToArray();
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SampleJudgementParameter() : base("Judgement_Parameter".ToUpper())
        {
        }
    }
}
