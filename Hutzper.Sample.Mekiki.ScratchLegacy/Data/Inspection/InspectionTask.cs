using Hutzper.Library.Common;
using Hutzper.Library.InsightLinkage;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// 検査タスク
    /// </summary>
    public class InspectionTask : SafelyDisposable, IInspectionTask
    {
        #region IInspectionTask

        /// <summary>
        /// 処理番号
        /// </summary>
        public int TaskIndex { get; protected set; }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        public object? TaskTag { get; set; }

        /// <summary>
        /// 起点の日時
        /// </summary>
        /// <remarks>撮影トリガの立ち上がり</remarks>
        public DateTime DateTimeOrigin { get; protected set; }

        /// <summary>
        /// ストップウォッチ:起点からの経過時間
        /// </summary>
        public Stopwatch StopwatchFromOrigin { get; protected set; } = new();

        /// <summary>
        /// タスクが有効かどうか
        /// </summary>
        public bool Enabled { get; protected set; }

        /// <summary>
        /// ストップウォッチ:物有りOFFからの経過時間
        /// </summary>
        public Stopwatch? StopwatchFromIoFalled { get; protected set; }

        /// <summary>
        /// ストップウォッチ:撮影完了からの経過時間        
        /// </summary>
        public Stopwatch? StopwatchFromGrabEnd { get; protected set; }

        /// <summary>
        /// 物有り信号がOFFしたかどうか
        /// </summary>
        public bool IsIoFalled => this.StopwatchFromIoFalled != null;

        /// <summary>
        /// 撮影が完了しているかどうか
        /// </summary>
        public bool IsGrabEnd => this.CounterOfGrabEnd == 0;

        /// <summary>
        /// 推論が完了しているかどうか
        /// </summary>
        public bool IsInferenceEnd => this.CounterOfInferenceEnd == 0;

        /// <summary>
        /// 紐づく情報
        /// </summary>
        public string LinkedInfomation { get; set; } = string.Empty;

        /// <summary>
        /// タスク項目
        /// </summary>
        public IInspectionTaskItem?[] Items { get; protected set; } = Array.Empty<IInspectionTaskItem>();

        /// <summary>
        /// 判定結果インデックス
        /// </summary>
        public int JudgementIndex { get; set; } = -1;

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; } = new();

        /// <summary>
        /// 画像保存を行うかどうか
        /// </summary>
        public bool ImageSavingEnabled { get; set; }

        /// <summary>
        /// 画像保存に成功したかどうか
        /// </summary>
        /// <remarks>保存が必要で実行した場合の結果(保存を実行しない場合はtrue)</remarks>
        public bool ImageSaveSucceeded
        {
            get => this.Items.All(i => i?.ImageSaveSucceeded ?? true);
        }

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; } = new();

        /// <summary>
        /// 全ての処理が完了したかどうか
        /// </summary>
        public bool IsTaskEnd { get; set; }

        /// <summary>
        /// メキキデータ
        /// </summary>
        public List<MekikiUnit> MekikiUnitList { get; init; } = new();

        /// <summary>
        /// 撮影完了フラグ
        /// </summary>
        public bool[] FlagsOfGrabEnd { get; protected set; } = Array.Empty<bool>();

        /// <summary>
        /// 推論完了フラグ
        /// </summary>
        public bool[] FlagsInferenceEnd { get; protected set; } = Array.Empty<bool>();

        #endregion

        #region フィールド

        /// <summary>
        /// 撮影完了フラグ
        /// </summary>
        protected int CounterOfGrabEnd;

        /// <summary>
        /// 処理完了フラグ
        /// </summary>
        protected int CounterOfInferenceEnd;

        /// <summary>
        /// 物有りOFF待機イベント
        /// </summary>
        protected ManualResetEvent WaitEventForIoFalling;

        /// <summary>
        /// 撮影完了待機イベント
        /// </summary>
        protected ManualResetEvent WaitEventForGrabEnd;

        /// <summary>
        /// 推論完了待機イベント
        /// </summary>
        protected ManualResetEvent WaitEventForInferenceEnd;

        /// <summary>
        /// 検査結果
        /// </summary>
        protected IInspectionResult? InspectionResult;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectionTask()
        {
            this.WaitEventForIoFalling = new ManualResetEvent(false);
            this.WaitEventForGrabEnd = new ManualResetEvent(false);
            this.WaitEventForInferenceEnd = new ManualResetEvent(false);
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
                // タスクを無効化する
                this.Invalidate();

                foreach (var w in new[] { this.WaitEventForIoFalling, this.WaitEventForGrabEnd, this.WaitEventForInferenceEnd })
                {
                    w.Close();
                }

                foreach (var item in Items)
                {
                    this.DisposeSafely(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public void Initialize(int taskIndex, DateTime origin, Stopwatch stopwatchFromOrigin, int validGrabberNumber)
        {
            this.TaskIndex = taskIndex;
            this.DateTimeOrigin = origin;
            this.StopwatchFromOrigin = stopwatchFromOrigin;
            this.Enabled = true;

            this.CounterOfGrabEnd = validGrabberNumber;
            this.CounterOfInferenceEnd = validGrabberNumber;

            this.FlagsOfGrabEnd = new bool[validGrabberNumber];
            this.FlagsInferenceEnd = new bool[validGrabberNumber];

            this.Items = new InspectionTaskItem[validGrabberNumber];
        }

        /// <summary>
        /// 物有り信号OFFを待機する
        /// </summary>
        public bool WaitIoFalled(int timeoutMs, out bool isTimeout)
        {
            var waitTimeMs = timeoutMs - Convert.ToInt32(this.StopwatchFromOrigin.ElapsedMilliseconds);

            if (0 < waitTimeMs)
            {
                isTimeout = !this.WaitEventForIoFalling.WaitOne(waitTimeMs);
            }
            else
            {
                isTimeout = !this.WaitEventForIoFalling.WaitOne();
            }

            return this.Enabled;
        }

        /// <summary>
        /// 撮影完了を待機する
        /// </summary>
        public bool WaitGrabEnd() => this.WaitEventForGrabEnd.WaitOne();

        /// <summary>
        /// 処理完了を待機する
        /// </summary>
        public bool WaitInferenceEnd() => this.WaitEventForInferenceEnd.WaitOne();

        /// <summary>
        /// タスク無効化
        /// </summary>
        public void Invalidate()
        {
            try
            {
                this.Enabled = false;

                foreach (var w in new[] { this.WaitEventForIoFalling, this.WaitEventForGrabEnd, this.WaitEventForInferenceEnd })
                {
                    w.Set();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 信号変化通知
        /// </summary>
        public void NotifyIoFalled(Stopwatch signalStopwatch)
        {
            this.StopwatchFromIoFalled = signalStopwatch;
            this.WaitEventForIoFalling.Set();
        }

        /// <summary>
        /// 撮影完了通知
        /// </summary>
        public IInspectionTaskItem? NotifyGrabEnd(int index, Bitmap? bitmap)
        {
            if (false == this.FlagsOfGrabEnd[index])
            {
                if (0 == Interlocked.Decrement(ref this.CounterOfGrabEnd))
                {
                    this.StopwatchFromGrabEnd = Stopwatch.StartNew();
                    this.WaitEventForGrabEnd.Set();
                }

                this.FlagsOfGrabEnd[index] = true;
                this.Items[index] = new InspectionTaskItem(this, index, bitmap);
            }

            return this.Items[index];
        }

        /// <summary>
        /// 処理完了通知
        /// </summary>
        public void NotifyInferenceEnd(int index)
        {
            if (false == this.FlagsInferenceEnd[index])
            {
                if (0 == Interlocked.Decrement(ref this.CounterOfInferenceEnd))
                {
                    this.WaitEventForInferenceEnd.Set();
                }

                this.FlagsInferenceEnd[index] = true;
            }
        }

        /// <summary>
        /// 検査結果を生成する
        /// </summary>
        /// <remarks>このメソッドで検査結果を生成することで、GetInspectionResultで検査結果を取得できるようになります</remarks>
        public virtual void CreateInspectionResult()
        {
            // 検査結果データを生成
            var result = new InspectionResult(this.TaskIndex, this.DateTimeOrigin, this.Items.Length)
            {
                JudgementIndex = this.JudgementIndex,
                ResultClassNames = new List<string>(this.ResultClassNames),
                LinkedInfomation = this.LinkedInfomation,
                AdditionalData = this.Items.Select(i => i?.AdditionalData).ToArray(),
            };
            result.GeneralValues.AddRange(this.GeneralValues);
            foreach (var itemIndex in Enumerable.Range(0, this.Items.Length))
            {
                if (this.Items[itemIndex] is IInspectionTaskItem item)
                {
                    result.ItemsGeneralValues[itemIndex].AddRange(item.GeneralValues);
                    result.Images[itemIndex] = (Bitmap?)item.Bitmap?.Clone();
                }
            }

            this.InspectionResult = result;
        }

        /// <summary>
        /// 検査結果を取得する
        /// </summary>
        /// <remarks>CreateInspectionResultメソッドが事前に実行されている必要があります</remarks>
        public virtual IInspectionResult? GetInspectionResult() => this.InspectionResult;

        /// <summary>
        /// シャローコピー
        /// </summary>
        public virtual IInspectionTask ShallowCopy()
        {
            var copied = (IInspectionTask)this.MemberwiseClone();

            foreach (var itemIndex in Enumerable.Range(0, this.Items.Length))
            {
                if (this.Items[itemIndex] is IInspectionTaskItem item)
                {
                    copied.Items[itemIndex] = item.ShallowCopy();
                }
            }

            return copied;
        }

        #endregion
    }
}