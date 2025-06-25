using System.Collections.Concurrent;
using System.Diagnostics;
namespace Hutzper.Library.Common.Threading
{
    /// <summary>
    /// スレッドキュー
    /// </summary>
    public class QueueThread<T> : SafelyDisposable, IThread where T : class
    {
        #region プロパティ

        #region IThread

        /// <summary>
        /// スレッドのスケジューリング優先順位
        /// </summary>
        public ThreadPriority Priority
        {
            #region 取得
            get
            {
                return this.thread.Priority;
            }
            #endregion

            #region 更新
            set
            {
                this.thread.Priority = value;
            }
            #endregion
        }

        /// <summary>
        /// スレッドの名前を取得または設定します。
        /// </summary>
        public string Name
        {
            #region 取得
            get
            {
                return this.thread.Name ?? string.Empty;
            }
            #endregion

            #region 更新
            set
            {
                this.thread.Name = value;
            }
            #endregion
        }

        /// <summary>
        /// 汎用オブジェクト
        /// </summary>
        public object? Tag { get; set; }

        #endregion

        /// <summary>
        /// キューに格納されている要素の数
        /// </summary>
        /// <remarks>呼び出し時の瞬間値</remarks>
        public int Count => this.queue.Count;

        #endregion

        #region イベント

        /// <summary>
        /// デキューイベント
        /// </summary>

        public event Action<object, T>? Dequeue;

        /// <summary>
        /// クリアの前処理を提供します
        /// </summary>
        public event Action<object, T[]>? ImmediatelyBeforeClear;

        #endregion

        #region フィールド

        /// <summary>
        /// スレッド
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        /// スレッド終了要求
        /// </summary>
        private bool threadTerminateFlag;

        /// <summary>
        /// キューイベント
        /// </summary>
        private readonly AutoResetEvent queuingEvent;

        /// <summary>
        /// キュー
        /// </summary>
        private readonly ConcurrentQueue<T> queue;

        /// <summary>
        /// スレッドに割り当てるプロセッサ番号配列
        /// </summary>
        private readonly uint[] targetProcessor = Array.Empty<uint>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QueueThread(params uint[] targetProcessor)
        {
            /// キューの生成
            this.queue = new();

            /// キュー操作イベントの生成
            this.queuingEvent = new AutoResetEvent(false);

            /// スレッドに割り当てるプロセッサが指定されている場合
            if (null != targetProcessor && 0 < targetProcessor.Length)
            {
                //// プロセッサ番号をコピーする
                this.targetProcessor = new uint[targetProcessor.Length];
                System.Array.Copy(targetProcessor, this.targetProcessor, this.targetProcessor.Length);
            }

            /// スレッドの生成
            this.threadTerminateFlag = false;
            this.thread = new Thread(this.threadProcess)
            {
                IsBackground = true
            };
            this.thread.Name = $"QueueThread {typeof(T).Name}";

            this.thread.Start();
        }

        #endregion

        #region リソースの解放

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                /// スレッドの解放
                this.threadTerminateFlag = true;
                this.queuingEvent.Set();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion

        #region キュー操作

        /// <summary>
        /// キューに追加します。
        /// </summary>
        /// <param name="t">キューに追加するオブジェクト</param>
        public void Enqueue(T t)
        {
            try
            {
                this.queue.Enqueue(t);
                this.queuingEvent.Set();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

        }

        /// <summary>
        /// キューの内容をクリアします。
        /// </summary>
        public void Clear()
        {
            try
            {
                this.OnImmediatelyBeforeClear(this.queue.ToArray());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

            this.queue.Clear();

        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// キューからオブジェクトを取り出します。
        /// </summary>
        /// <returns>ブロッキング処理です。</returns>
        private T? dequeue(out bool queueExists)
        {
            var t = default(T);

            queueExists = false;
            while (true)
            {
                //// キューから要求を取り出す
                try
                {
                    if (0 < this.queue.Count)
                    {
                        if (this.queue.TryDequeue(out t))
                        {
                            queueExists = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(this, ex.Message);
                }

                //// データがある場合
                if (queueExists)
                {
                    ///// 中断
                    break;
                }
                //// データが無いが終了指示がある場合
                else if (true == this.threadTerminateFlag)
                {
                    ///// 中断
                    break;
                }

                //// イベント待ち
                this.queuingEvent.WaitOne();
            }

            return t;
        }

        /// <summary>
        /// スレッド処理
        /// </summary>
        private void threadProcess()
        {
            try
            {
                /// プロセッサ指定されている場合
                if (null != this.targetProcessor)
                {
                    //// スレッドに割り当てるプロセッサを指定する
                    ThreadUtilities.SetThreadAffinityMask(this.targetProcessor);
                }

                /// 終了指示があるまで繰り返し処理
                while (false == this.threadTerminateFlag)
                {
                    //// キューからオブジェクトを取り出す
                    var t = this.dequeue(out bool queueExists);

                    //// 終了指示がある場合
                    if (true == this.threadTerminateFlag)
                    {
                        if (queueExists && t != null)
                        {
                            this.OnImmediatelyBeforeClear(t);
                        }

                        ///// 何もしない
                        break;
                    }

                    this.OnDequeue(t);
                }
            }
            catch (ThreadAbortException tae)
            {
                Debug.WriteLine(this, tae.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// イベント通知
        /// </summary>
        protected void OnDequeue(T? t)
        {
            try
            {
                if (null != t)
                {
                    this.Dequeue?.Invoke(this, t);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        protected void OnImmediatelyBeforeClear(params T[] t)
        {
            try
            {
                if (null != t)
                {
                    this.ImmediatelyBeforeClear?.Invoke(this, t);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion
    }
}