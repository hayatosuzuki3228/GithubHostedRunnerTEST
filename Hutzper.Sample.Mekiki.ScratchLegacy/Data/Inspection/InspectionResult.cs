using Hutzper.Library.Common;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// 検査結果
    /// </summary>
    public class InspectionResult : SafelyDisposable, IInspectionResult
    {
        #region IInspectionResult

        /// <summary>
        /// 処理番号
        /// </summary>
        public int TaskIndex { get; }

        /// <summary>
        /// 検査日時
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// 紐づく情報
        /// </summary>
        public string LinkedInfomation { get; set; } = string.Empty;

        /// <summary>
        /// 判定結果文字列
        /// </summary>
        public string JudgementText
        {
            get
            {
                if (0 > this.JudgementIndex || this.ResultClassNames.Count <= this.JudgementIndex)
                {
                    return string.Empty;
                }

                return this.ResultClassNames[this.JudgementIndex];
            }
        }

        /// <summary>
        /// 判定結果インデックス
        /// </summary>
        public int JudgementIndex { get; set; } = -1;

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; } = new();

        /// <summary>
        /// 画像データ
        /// </summary>
        public Bitmap?[] Images { get; set; }

        /// <summary>
        /// 追加データ
        /// </summary>
        public IAdditionalDataContainer?[] AdditionalData { get; set; }

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; } = new();

        /// <summary>
        /// 項目別汎用格納域
        /// </summary>
        public List<double>[] ItemsGeneralValues { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectionResult(int taskIndex, DateTime origin, int validGrabberNumber)
        {
            this.TaskIndex = taskIndex;
            this.DateTime = origin;
            this.Images = new Bitmap?[validGrabberNumber];

            this.ItemsGeneralValues = new List<double>[validGrabberNumber];
            foreach (var i in Enumerable.Range(0, this.ItemsGeneralValues.Length))
            {
                this.ItemsGeneralValues[i] = new List<double>();
            }

            this.AdditionalData = new IAdditionalDataContainer?[validGrabberNumber];
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                foreach (var bmp in this.Images)
                {
                    bmp?.Dispose();
                }

                foreach (var data in this.AdditionalData)
                {
                    this.DisposeSafely(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.AdditionalData = Array.Empty<IAdditionalDataContainer?>();
            }
        }

        #endregion
    }
}