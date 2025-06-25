using System.Diagnostics;

namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// FPS計算
    /// </summary>
    [Serializable]
    public class FpsCalculator
    {
        #region プロパティ

        /// <summary>
        /// 算出間隔
        /// </summary>
        /// <remarks>フレームレート(平均値)を算出する撮影数を指定します。</remarks>
        public int RequiredFrameNumber { get; set; }

        /// <summary>
        /// 算出されたFPS値
        /// </summary>
        public double Result { get; protected set; }

        /// <summary>
        /// タグ
        /// </summary>
        public object? Tag { get; set; }

        #endregion

        #region フィールド

        /// <summary>
        /// 累積カウント
        /// </summary>
        private int accumulatedCount;

        /// <summary>
        /// 累積時間
        /// </summary>
        private double accumulatedTime;

        /// <summary>
        /// ストップウォッチ
        /// </summary>
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// 開始時値
        /// </summary>
        private double startTicks;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FpsCalculator()
        {
            this.RequiredFrameNumber = 15;

            this.stopwatch = Stopwatch.StartNew();
            this.accumulatedCount = -1;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// フレーム追加
        /// </summary>
        /// <returns>FPS値が更新されたかどうか</returns>
        public bool AddFrame()
        {
            var isUpdate = false;

            // 1回目
            if (0 > this.accumulatedCount)
            {
                //スタート時間を記録
                this.startTicks = this.stopwatch.ElapsedTicks;
                this.accumulatedCount++;
            }
            else if (this.RequiredFrameNumber == this.accumulatedCount)
            {
                //FPS値の計算
                this.Result = 1000.0 / (this.accumulatedTime / this.accumulatedCount);

                //計算用変数の初期化
                this.Reset();

                isUpdate = true;
            }
            // 2回目以降
            else
            {
                //現在の時間を取得
                double stopTicks = this.stopwatch.ElapsedTicks;

                //かかった時間を足す
                this.accumulatedTime += ((stopTicks - this.startTicks) / Stopwatch.Frequency) * 1000;

                this.startTicks = stopTicks;

                this.accumulatedCount++;
            }

            return isUpdate;
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            //計算用変数の初期化
            this.accumulatedTime = 0;
            this.accumulatedCount = -1;
        }

        #endregion
    }
}