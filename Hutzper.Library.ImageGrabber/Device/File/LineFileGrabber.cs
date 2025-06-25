using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Data;
using System.Diagnostics;

namespace Hutzper.Library.ImageGrabber.Device.File
{
    [Serializable]
    public class LineFileGrabber : FileGrabber, ILineSensor
    {
        #region ILineSensor

        /// <summary>
        /// X反転
        /// </summary>
        public virtual bool ReverseX { get; set; }

        /// <summary>
        /// ラインレート
        /// </summary>
        public virtual double LineRateHz { get; set; }

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is ILineSensorParameter p)
                {
                    this.ReverseX = p.ReverseX;
                    this.LineRateHz = p.LineRateHz;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IGrabber

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            var isSuccess = false;

            try
            {
                this.QueueGrabberData = null;

                isSuccess = base.Grab();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public override bool GrabContinuously(int number = -1)
        {
            var isSuccess = false;

            try
            {
                this.QueueGrabberData = null;

                isSuccess = base.GrabContinuously(number);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
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
                base.DisposeExplicit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineFileGrabber() : this(typeof(LineFileGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineFileGrabber(Common.Drawing.Point location) : this(typeof(LineFileGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public LineFileGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.GrabberType = GrabberType.LineSensor;
        }

        #endregion

        #region フィールド

        protected IByteJaggedArrayGrabberData? QueueGrabberData;

        #endregion

        #region protected メソッド

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        protected override void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (this.Parameter is not FileGrabberParameter fp)
                {
                    throw new InvalidOperationException($"{nameof(this.Parameter)} is not FileGrabberParameter.");
                }

                // カレントディレクトリ情報取得
                var currentDirectory = this.CurrentDirectory;

                // 拡張子リストを取得
                var validExtensions = new List<string>();
                foreach (var ext in fp.ValidExtensions)
                {
                    if (false == string.IsNullOrEmpty(ext.Trim()))
                    {
                        var formatedExt = (ext[0] == '.') ? ext.Trim() : $".{ext.Trim()}";
                        if (false == validExtensions.Contains(formatedExt))
                        {
                            validExtensions.Add(formatedExt);
                        }
                    }
                }

                // カレントディレクトリが存在する場合
                if (currentDirectory?.Exists ?? false)
                {
                    // 画像枚数を初期化
                    var imageCounter = 0;
                    var lastAcquisitionDateTime = DateTime.Now - new TimeSpan(0, 0, 10);

                    var fps = 15d;

                    var skipUntilLastReadFile = this.LastFileInfo is not null && fp.ResumeFromLastFile;

                    // 続行可能な場合は繰り返し処理
                    while (info.IsContinuable)
                    {
                        // ファイル情報を列強
                        foreach (var fileInfo in currentDirectory.EnumerateFiles())
                        {
                            // 拡張子チェック
                            if (0 < validExtensions.Count && false == validExtensions.Contains(System.IO.Path.GetExtension(fileInfo.FullName)))
                            {
                                continue;
                            }

                            // 最後に読み込んだファイルの次から再開する
                            if (true == skipUntilLastReadFile)
                            {
                                if (this.LastFileInfo?.FullName == fileInfo.FullName)
                                {
                                    // 前回のファイルに一致するまでスキップ
                                    skipUntilLastReadFile = false;
                                }
                                continue;
                            }

                            // 画像取得終了
                            if (false == info.IsContinuable)
                            {
                                break;
                            }

                            // 画像データを生成
                            IByteJaggedArrayGrabberData sourceData = new ByteJaggedArrayGrabberData(this.location);

                            // 画像ファイル読み込み
                            sourceData.Read(fileInfo);

                            // 画像連結
                            if (this.QueueGrabberData is null)
                            {
                                this.QueueGrabberData = new ByteJaggedArrayGrabberData(this.location, System.Math.Max(this.Height, sourceData.Size.Height));
                                this.QueueGrabberData.Size.Width = this.Width;
                                this.QueueGrabberData.Size.Height = 0;
                                this.QueueGrabberData.PixelFormat = sourceData.PixelFormat;
                            }

                            // 追加するデータへの参照
                            var availableNewData = sourceData;

                            var bitCount = System.Drawing.Bitmap.GetPixelFormatSize(availableNewData.PixelFormat);
                            var channel = bitCount / 8;

                            #region 幅調整
                            if (this.Width < sourceData.Size.Width)
                            {
                                availableNewData = new ByteJaggedArrayGrabberData(this.location, sourceData.Size.Height);

                                var stride = this.Width * channel;
                                foreach (var i in Enumerable.Range(0, sourceData.Size.Height))
                                {
                                    availableNewData.Image[i] = new byte[stride];
                                    Array.Copy(sourceData.Image[i], 0, availableNewData.Image[i], 0, availableNewData.Image[i].Length);
                                }
                                availableNewData.Size.Width = this.Width;
                                availableNewData.Size.Height = sourceData.Size.Height;
                                availableNewData.PixelFormat = sourceData.PixelFormat;
                            }
                            else if (this.Width > sourceData.Size.Width)
                            {
                                availableNewData = new ByteJaggedArrayGrabberData(this.location, sourceData.Size.Height);

                                var offsetLeft = (this.Width - sourceData.Size.Width) / 2 + this.OffsetX;
                                var copyLength = sourceData.Size.Width - this.OffsetX;

                                offsetLeft *= channel;
                                copyLength *= channel;

                                var stride = this.Width * channel;
                                foreach (var i in Enumerable.Range(0, sourceData.Size.Height))
                                {
                                    availableNewData.Image[i] = new byte[stride];
                                    Array.Copy(sourceData.Image[i], 0, availableNewData.Image[i], offsetLeft, copyLength);
                                }
                                availableNewData.Size.Width = this.Width;
                                availableNewData.Size.Height = sourceData.Size.Height;
                                availableNewData.PixelFormat = sourceData.PixelFormat;
                            }
                            #endregion

                            // 画像データを連結する
                            this.QueueGrabberData.ConcatImage(availableNewData);

                            foreach (var b in Enumerable.Range(0, this.QueueGrabberData.Size.Height / this.Height))
                            {
                                Task.Run(() =>
                                {
                                    // カウンタ値をインクリメント
                                    this.QueueGrabberData.Counter++;

                                    // 現在保持している画像をコピー
                                    if (this.QueueGrabberData.Clone() is IByteJaggedArrayGrabberData grabberData)
                                    {
                                        // 画像高さを処理単位のブロック高さに変更
                                        grabberData.Size.Height = this.Height;

                                        // 残っている画像を前詰め
                                        var remainHeight = this.QueueGrabberData.Size.Height - this.Height;
                                        this.QueueGrabberData.Size.Height -= this.Height;

                                        this.QueueGrabberData.Size.Height = System.Math.Max(0, this.QueueGrabberData.Size.Height);
                                        remainHeight = System.Math.Max(0, remainHeight);

                                        foreach (var i in Enumerable.Range(0, remainHeight))
                                        {
                                            Array.Copy(this.QueueGrabberData.Image[this.Height + i], this.QueueGrabberData.Image[i], this.QueueGrabberData.Image[i].Length);
                                        }

                                        // トリガ待機
                                        info.WaitTrigger();

                                        // 完了通知
                                        info.OnDataStreamNewBuffer(grabberData);
                                    }
                                });

                                // 内部トリガーの場合
                                if (this.triggerMode == TriggerMode.InternalTrigger)
                                {
                                    // 設定FPSから待機時間を算出
                                    if (0 < fps)
                                    {
                                        var elapsedSinceLastAcquisition = DateTime.Now - lastAcquisitionDateTime;

                                        var waitTimeMs = Convert.ToInt32(1000 / fps - elapsedSinceLastAcquisition.TotalMilliseconds);

                                        // 待機が必要な場合
                                        if (0 < waitTimeMs)
                                        {
                                            // FPSに合わせて待機                                 
                                            Thread.Sleep(waitTimeMs);
                                        }
                                    }

                                    // トリガ発行
                                    info.DoSoftTrigger();
                                }

                                // 画像取得完了待ち
                                var bufferImages = info.GetDataStreamNewBuffer();

                                // 画像取得終了
                                if (false == info.IsContinuable)
                                {
                                    break;
                                }

                                // 最後に取得したファイル情報を保持
                                this.LastFileInfo = fileInfo;

                                // 画像取得間隔計測開始
                                lastAcquisitionDateTime = DateTime.Now;

                                // 画像バッファから有効データを列挙
                                foreach (var buffer in bufferImages.Where(b => b.Size.Height > 0))
                                {
                                    // カウンタ値を設定
                                    buffer.Counter = (ulong)++imageCounter;

                                    // 画像取得通知
                                    this.OnDataGrabbed(buffer);

                                    // 必要画像枚数に達している、もしくは続行不可能な場合
                                    if (
                                        (false == info.IsContinuable)
                                    || (0 < info.NumberOfImage && imageCounter >= info.NumberOfImage)
                                    )
                                    {
                                        // 画像取得終了
                                        info.Invalidate();
                                        break;
                                    }
                                }

                                // 画像取得終了
                                if (false == info.IsContinuable)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                info.Invalidate();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);

                this.OnErrorOccurred(new GrabberErrorBase(this.location));
            }
            finally
            {
                this.DisposeSafely(info);
            }
        }

        #endregion
    }
}