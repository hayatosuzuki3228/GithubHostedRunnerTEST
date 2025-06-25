using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// ラインセンサ制御
    /// </summary>
    [Serializable]
    public class LineSensorController : GrabberControllerBase, ILineSensorController
    {
        #region IGrabberController

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <returns></returns>
        public override bool GrabContinuously(int number = -1)
        {
            var isSuccess = false;

            try
            {
                if (true == this.AquisitionSemaphore.WaitOne(0))
                {
                    // 連結画像OFF
                    this.Concatenation.Enabled = false;

                    // 撮影開始
                    isSuccess = base.GrabContinuously(number);

                    if (false == isSuccess)
                    {
                        this.AquisitionSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabbing()
        {
            var isSuccess = true;

            try
            {
                if (false == this.AquisitionSemaphore.WaitOne(0))
                {
                    // 撮影停止
                    base.StopGrabbing();

                    // 不定長連結画像作成の場合
                    if (true == this.Concatenation.Enabled && 0 > this.Concatenation.ImageHeight)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                Parallel.ForEach(Enumerable.Range(0, this.NumberOfGrabber), g =>
                                {
                                    var data = (IGrabberData?)null;
                                    lock (this.Concatenation.SyncRoot[g])
                                    {
                                        data = this.Concatenation.GrabberData[g];
                                        this.Concatenation.GrabberData[g] = new ByteJaggedArrayGrabberData(this.Grabbers[g].Location);
                                    }

                                    // 連結画像が存在する場合
                                    if (null != data && 0 < data.Size.Height)
                                    {
                                        // イベント通知
                                        this.OnDataGrabbed(data);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.AquisitionSemaphore.Release();
            }

            return isSuccess;
        }

        #endregion

        #region ILineSensorController

        /// <summary>
        /// 連結画像取得
        /// </summary>
        /// <returns></returns>
        /// <remarks>指定長の</remarks>
        public virtual bool ConcatGrab(int totalHeight)
        {
            if (0 > totalHeight)
            {
                return false;
            }

            return this.ConcatGrabContinuously(1, totalHeight);
        }

        /// <summary>
        /// 連結画像取得
        /// </summary>
        /// <returns></returns>
        public virtual bool ConcatGrabContinuously(int number = -1, int totalHeight = -1)
        {
            var isSuccess = false;

            try
            {
                if (true == this.AquisitionSemaphore.WaitOne(0))
                {
                    // 連結制御の初期化
                    this.Concatenation.Inialiize(this.Grabbers, number, totalHeight);

                    // 撮影開始
                    isSuccess = base.GrabContinuously();

                    if (false == isSuccess)
                    {
                        this.AquisitionSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        /// <summary>
        /// 画像連結情報
        /// </summary>
        protected class ConcatenationControl
        {
            /// <summary>
            /// 連結画像の有効/無効
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 連結画像の高さ
            /// </summary>
            /// <remarks>-1を指定するとStopするまで</remarks>
            public int ImageHeight { get; set; }

            /// <summary>
            /// 連結画像の必要枚数
            /// </summary>
            public int ImageRequiredNumber { get; set; }

            /// <summary>
            /// 連結画像のカウント
            /// </summary>
            /// <remarks>デバイス毎、連結画像1枚作成するごとにカウントアップ</remarks>
            public int[] ImageProcessedCounter { get; set; } = Array.Empty<int>();

            /// <summary>
            /// 連結画像作成同期オブジェクト
            /// </summary>
            /// <remarks>デバイス数分の配列</remarks>
            public object[] SyncRoot { get; set; } = Array.Empty<object>();

            /// <summary>
            /// 連結画像データ
            /// </summary>
            /// <remarks>デバイス数分の配列</remarks>
            public IByteJaggedArrayGrabberData[] GrabberData { get; set; } = Array.Empty<IByteJaggedArrayGrabberData>();

            /// <summary>
            /// 連結画像作成の終了確認用カウンタ
            /// </summary>
            protected int FinishCounter;

            /// <summary>
            /// 初期化
            /// </summary>
            /// <param name="numOfGrabber"></param>
            public void Inialiize(List<IGrabber> grabber, int requiredNumber, int requiredHeight)
            {
                var numOfGrabber = grabber.Count;

                // 画像連結のためのデータ設定
                this.Enabled = true;
                this.ImageHeight = requiredHeight;
                this.ImageRequiredNumber = requiredNumber;
                this.ImageProcessedCounter = new int[numOfGrabber];
                this.SyncRoot = new object[grabber.Count];
                this.GrabberData = new ByteJaggedArrayGrabberData[numOfGrabber];
                Interlocked.Exchange(ref this.FinishCounter, numOfGrabber);

                for (int i = 0; i < this.SyncRoot.Length; i++)
                {
                    this.SyncRoot[i] = new object();
                }
            }

            /// <summary>
            /// 終了通知
            /// </summary>
            /// <param name="grabberIndex"></param>
            /// <returns>全デバイス完了かどうか</returns>
            public bool OnFinished()
            {
                return 0 == Interlocked.Decrement(ref this.FinishCounter);
            }
        }

        protected ConcatenationControl Concatenation = new();

        /// <summary>
        /// 画像取得制御用セマフォ
        /// </summary>
        protected Semaphore AquisitionSemaphore { get; init; } = new(1, 1);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public LineSensorController(int index = -1) : base(typeof(AreaSensorController).Name, index)
        {
            this.GrabberType = GrabberType.LineSensor;
        }

        #region IGrabberイベント

        /// <summary>
        /// IGrabberイベント 画像取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected override void Grabber_DataGrabbed(object sender, IGrabberData data)
        {
            try
            {
                // 連結処理の場合
                if (true == this.Concatenation.Enabled && data is IByteJaggedArrayGrabberData jaggedArrayData)
                {
                    // デバイスインデックス
                    var index = this.Grabbers.IndexOf((IGrabber)sender);

                    // 連結完了チェック
                    var fixedData = (IByteJaggedArrayGrabberData?)null;
                    var renewData = this.Concatenation.GrabberData[index];
                    lock (this.Concatenation.SyncRoot[index])
                    {
                        if (this.Concatenation.GrabberData[index] is null)
                        {
                            this.Concatenation.GrabberData[index] = jaggedArrayData;
                            renewData = this.Concatenation.GrabberData[index];
                        }
                        else
                        {
                            // データを連結する
                            this.Concatenation.GrabberData[index].ConcatImage(jaggedArrayData);
                        }

                        // 連結画像の高さが指定されていて、その高さ以上に達した場合
                        if (0 < this.Concatenation.ImageHeight && this.Concatenation.ImageHeight <= this.Concatenation.GrabberData[index].Size.Height)
                        {
                            // 連結画像の確定データ
                            fixedData = this.Concatenation.GrabberData[index];

                            // 次の連結画像データを設定
                            this.Concatenation.GrabberData[index] = new ByteJaggedArrayGrabberData(data.Location, this.Concatenation.ImageHeight) { Counter = this.Concatenation.GrabberData[index].Counter + 1 };

                            // コピー元として参照を退避
                            renewData = this.Concatenation.GrabberData[index];
                        }
                    }

                    if (fixedData is not null)
                    {
                        // 指定高さを超える高さ(次の画像先頭になる)
                        var remainHeight = fixedData.Size.Height - this.Concatenation.ImageHeight;

                        // 指定長以上の残りがあった場合
                        if (0 < remainHeight)
                        {
                            var remainIndex = this.Concatenation.ImageHeight;

                            // 次の連結画像用データの先頭にコピーする
                            foreach (var i in Enumerable.Range(0, remainHeight))
                            {
                                renewData.Image[i] = new byte[fixedData.Size.Width];
                                Array.Copy(fixedData.Image[remainIndex++], 0, renewData.Image[i], 0, renewData.Image[i].Length);
                            }
                        }

                        // 連結画像の高さを指定値とする
                        fixedData.Size.Height = this.Concatenation.ImageHeight;

                        // 連結画像をイベント通知
                        this.OnDataGrabbed(fixedData);

                        // 枚数が指定されていて、かつその枚数に達した場合
                        if (0 < this.Concatenation.ImageRequiredNumber && (ulong)this.Concatenation.ImageRequiredNumber <= fixedData.Counter)
                        {
                            if (true == this.Concatenation.OnFinished())
                            {
                                try
                                {
                                    // 撮影停止
                                    base.StopGrabbing();
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                                finally
                                {
                                    this.AquisitionSemaphore.Release();
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.OnDataGrabbed(data);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}