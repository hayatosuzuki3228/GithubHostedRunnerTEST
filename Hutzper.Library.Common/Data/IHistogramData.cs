using System.Diagnostics;

namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// ヒストグラムデータのインタフェース
    /// </summary>
    public interface IHistogramData
    {
        /// <summary>
        /// ヒストグラムデータ
        /// </summary>                    
        public int[] Histogram { get; }

        /// <summary>
        /// データ数
        /// </summary>
        public long Num { get; set; }

        /// <summary>
        /// データ総和
        /// </summary>
        public long Sum { get; set; }

        /// <summary>
        /// 最小値
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// 最大値
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// 平均値
        /// </summary>
        public double Ave { get; set; }

        /// <summary>
        /// 最頻値
        /// </summary>
        public double Mode { get; set; }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="source"></param>
        public void CopyFrom(IHistogramData source);

        /// <summary>
        /// 再計算
        /// </summary>
        /// <returns></returns>
        public bool Recalculate(bool isNeedToCountup = false);
    }

    /// <summary>
    /// ヒストグラムデータ
    /// </summary>
    /// <remarks>IHistogramDataの実装です。基本機能を提供します。</remarks>
    [Serializable]
    public class HistogramData : IHistogramData
    {
        #region プロパティ

        /// <summary>
        /// ヒストグラムデータ
        /// </summary>                    
        public int[] Histogram { get; protected set; }

        /// <summary>
        /// データ数
        /// </summary>
        public long Num { get; set; }

        /// <summary>
        /// データ総和
        /// </summary>
        public long Sum { get; set; }

        /// <summary>
        /// 最小値
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// 最大値
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// 平均値
        /// </summary>
        public double Ave { get; set; }

        /// <summary>
        /// 最頻値
        /// </summary>
        public double Mode { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HistogramData() : this(byte.MaxValue + 1)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="classNumber">階級数</param>
        public HistogramData(int classNumber)
        {
            this.Histogram = new int[classNumber];
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HistogramData(IHistogramData source)
        {
            this.Histogram = Array.Empty<int>();
            this.CopyFrom(source);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HistogramData(int[] histogram, long num, long sum, double min, double max, double ave, double mode)
        {
            this.Histogram = new int[histogram.Length];
            Array.Copy(histogram, this.Histogram, this.Histogram.Length);

            this.Num = num;
            this.Sum = sum;
            this.Min = min;
            this.Max = max;
            this.Ave = ave;
            this.Mode = mode;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Ave={this.Ave:F}, Mode={this.Mode:F}, Min={this.Min:F}, Max={this.Max:F}";
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="source"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CopyFrom(IHistogramData source)
        {
            this.Histogram = new int[source.Histogram.Length];
            Array.Copy(source.Histogram, this.Histogram, this.Histogram.Length);

            this.Num = source.Num;
            this.Sum = source.Sum;
            this.Min = source.Min;
            this.Max = source.Max;
            this.Ave = source.Ave;
            this.Mode = source.Mode;
        }

        /// <summary>
        /// 再計算
        /// </summary>
        /// <remarks>ヒストグラムデータから統計情報を算出する</remarks>
        public bool Recalculate(bool isNeedToCountup = false)
        {
            var isSuccess = false;

            try
            {
                // 統計量計算
                if (true == isNeedToCountup)
                {
                    this.Num = this.Histogram.Sum();
                    this.Sum = 0;
                    foreach (var i in Enumerable.Range(0, this.Histogram.Length))
                    {
                        this.Sum += this.Histogram[i] * i;
                    }
                }

                this.Min = 0;
                this.Max = 0;
                this.Ave = 0;
                this.Mode = 0;

                if (0 < this.Num)
                {
                    this.Min = Array.FindIndex(this.Histogram, (o => o > 0));
                    this.Max = Array.FindLastIndex(this.Histogram, (o => o > 0));
                    this.Ave = (double)this.Sum / (double)this.Num;

                    var maxNumber = this.Histogram.Max();
                    this.Mode = Array.FindIndex(this.Histogram, (o => o == maxNumber));

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

            return isSuccess;
        }

        #endregion
    }
}