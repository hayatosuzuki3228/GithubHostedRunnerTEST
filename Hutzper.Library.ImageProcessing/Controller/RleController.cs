using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.ImageProcessing.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Point = Hutzper.Library.Common.Drawing.Point;

namespace Hutzper.Library.ImageProcessing.Controller
{
    /// <summary>
    /// Run Length Encoding
    /// </summary>
    [Serializable]
    public class RleController : ControllerBase, IRleController
    {
        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);

                this.Close();

                this.RleDataRawArray = Array.Empty<ushort>();
                this.RleData = Array.Empty<IRleData>();
                this.RleLines = Array.Empty<IRleLineInfo>();
                this.LabelArray = Array.Empty<IRleLabel>();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config)
        {
            try
            {
                base.SetConfig(config);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is IRleControllerParameter rcp)
                {
                    this.ControllerParameter = rcp;

                    this.RleDataRawArray = new ushort[rcp.MaximumRleNumber];

                    this.RleData = new IRleData[this.RleDataRawArray.Length];
                    for (int i = 0; i < this.RleData.Length; i++)
                    {
                        this.RleData[i] = new RleData();
                    }

                    this.RleLines = new IRleLineInfo[rcp.MaximumLineNumber];
                    for (int i = 0; i < this.RleLines.Length; i++)
                    {
                        this.RleLines[i] = new RleLineInfo();
                    }

                    this.LabelArray = new IRleLabel[rcp.MaximumLabelNumber];
                    for (int i = 0; i < this.LabelArray.Length; i++)
                    {
                        this.LabelArray[i] = new RleLabel(i);
                    }
                }

                foreach (var i in Enumerable.Range(0, this.RleDataRawArray.Length))
                {
                    this.RleDataRawArray[i] = 0;
                }

                foreach (var item in this.RleData)
                {
                    item.IsLeading = 0;
                }

                foreach (var item in this.RleLines)
                {
                    item.DataNumber = 0;
                }

                foreach (var item in this.LabelArray)
                {
                    item.PixelNumber = 0;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            try
            {
                base.Update();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            var isSuccess = true;

            try
            {
                // ラベリングスレッド生成
                this.LabelingThread = new LabelingProcessing(this.labelingProcess)
                {
                    RestPeriods = this.ControllerParameter?.LabelingThreadRestPeriodsMs ?? 10
                };
                this.LabelingThread.Initialize();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                // ラベリングスレッド破棄
                this.DisposeSafely(this.LabelingThread);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.LabelingThread = null;
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

                this.DisposeSafely(this.LabelingThread);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region サブクラス

        /// <summary>
        /// ラベリング処理
        /// </summary>
        protected class LabelingProcessing : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 制御イベント
            /// </summary>
            public readonly ManualResetEvent ControlEvent;

            /// <summary>
            /// スレッドが有効であるかどうか
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 高速モードでない場合のウェイト時間
            /// </summary>
            /// <remarks>ミリ秒</remarks>
            public int RestPeriods { get; set; }

            /// <summary>
            /// 高速モードかどうか
            /// </summary>
            public bool IsFastMode { get; set; }

            /// <summary>
            /// エラーコードリスト
            /// </summary>
            public RleErrorCode[] ErrorCode
            {
                #region 取得
                get
                {
                    return this.ErrorCodeList.ToArray();
                }
                #endregion
            }

            /// <summary>
            /// エラーが発生したかどうか
            /// </summary>
            public bool HasError => this.ErrorCodeList.Count > 0;

            #endregion

            #region フィールド

            /// <summary>
            /// スレッド
            /// </summary>
            protected readonly Thread Thread;

            /// <summary>
            /// 完了イベント
            /// </summary>
            protected readonly ManualResetEvent LabelingFinishedEvent;

            /// <summary>
            /// エラーコードリスト
            /// </summary>
            protected List<RleErrorCode> ErrorCodeList;

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="threadProcessCallback"></param>
            public LabelingProcessing(ThreadStart threadProcessCallback)
            {
                // ラベリングスレッド生成
                this.Thread = new Thread(threadProcessCallback)
                {
                    IsBackground = true
                };

                // ラベリングスレッドを起動する
                this.Enabled = true;
                this.RestPeriods = 10;
                this.IsFastMode = false;
                this.ControlEvent = new ManualResetEvent(false);
                this.LabelingFinishedEvent = new ManualResetEvent(false);
                this.ErrorCodeList = new List<RleErrorCode>();
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

                    // スレッドを終了する
                    if (this.Thread.IsAlive)
                    {
                        this.Enabled = false;
                        this.IsFastMode = true;
                        this.LabelingFinishedEvent.Set();
                        this.ControlEvent.Set();
                        this.Thread.Join(5000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion

            #region パブリックメソッド

            /// <summary>
            /// 初期化
            /// </summary>
            public void Initialize()
            {
                this.Thread.Start();
            }

            /// <summary>
            /// 開始
            /// </summary>
            public void Start()
            {
                this.ErrorCodeList = new List<RleErrorCode>();
                this.LabelingFinishedEvent.Reset();
                this.ControlEvent.Set();
            }

            /// <summary>
            /// 待機
            /// </summary>
            /// <returns></returns>
            public bool Wait()
            {
                this.IsFastMode = true;
                this.LabelingFinishedEvent.WaitOne();

                return !this.HasError;
            }

            /// <summary>
            /// 中断
            /// </summary>
            public void Break()
            {
                this.LabelingFinishedEvent.Set();
            }

            /// <summary>
            /// 停止
            /// </summary>
            public void Stop()
            {
                this.ControlEvent.Reset();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="errorCode"></param>
            public void AddError(RleErrorCode errorCode)
            {
                this.ErrorCodeList.Add(errorCode);
            }

            #endregion
        }

        #endregion

        #region IRleController

        /// <summary>
        /// 最後に発生したエラー
        /// </summary>
        public RleErrorCode LastError { get; protected set; }

        /// <summary>
        /// 算出された画像の高さ
        /// </summary>
        public virtual int ImageHeight { get; protected set; }

        /// <summary>
        /// 算出された画像の幅
        /// </summary>
        public virtual int ImageWidth { get; protected set; }

        /// <summary>
        /// RLEデータ
        /// </summary>
        public IRleData[] RleData { get; protected set; } = Array.Empty<IRleData>();

        /// <summary>
        /// RLEライン情報
        /// </summary>    
        public IRleLineInfo[] RleLines { get; protected set; } = Array.Empty<IRleLineInfo>();

        #endregion

        #region プロパティ

        /// <summary>
        /// ラベリングスレッドウェイト
        /// </summary>
        /// <remarks>高速モードでない場合のウェイト時間ミリ秒</remarks>
        public int LabelingThreadRestPeriodsMs
        {
            #region 取得
            get
            {
                return this.LabelingThread?.RestPeriods ?? 0;
            }
            #endregion

            #region 更新
            set
            {
                if (this.LabelingThread is not null)
                {
                    this.LabelingThread.RestPeriods = System.Math.Max(0, value);
                }
            }
            #endregion
        }

        /// <summary>
        /// ラベリングを実行するかどうか
        /// </summary>
        public virtual bool LabelingEnabled { get; protected set; }

        #endregion

        #region フィールド

        /// <summary>
        /// 制御パラメータ
        /// </summary>
        protected IRleControllerParameter? ControllerParameter;

        /// <summary>
        /// ラベリング処理
        /// </summary>
        protected RleController.LabelingProcessing? LabelingThread;

        /// <summary>
        /// RLEデータ数
        /// </summary>
        protected int RleDataNumber;

        /// <summary>
        /// RLE次処理データインデックス
        /// </summary>
        protected int NextDataIndex;

        /// <summary>
        /// RLE終了フラグ
        /// </summary>
        protected bool RleDataFixed;

        /// <summary>
        /// RLE生データ
        /// </summary>
        protected ushort[] RleDataRawArray = Array.Empty<ushort>();

        /// <summary>
        /// ラベル数
        /// </summary>    
        protected int LabelNumber;

        /// <summary>
        /// ラベル配列
        /// </summary>    
	    protected IRleLabel[] LabelArray = Array.Empty<IRleLabel>();

        /// <summary>
        /// 次処理ラインインデックス
        /// </summary>    
        protected int NextLineIndex;

        /// <summary>
        /// 輪郭追跡マスク
        /// </summary>
        protected Point[] TrackingMaskTable = Array.Empty<Point>();

        /// <summary>
        /// 
        /// </summary>
        protected double[] TrackingDistTable = Array.Empty<double>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public RleController(int index = -1) : base(typeof(RleController).Name, index)
        {
            // 輪郭追跡マスクを初期化する
            this.TrackingMaskTable = new Point[8];

            #region 輪郭追跡マスク作成(チェインコード式)
            {
                this.TrackingMaskTable[0] = new Point(-1, 1);
                this.TrackingMaskTable[1] = new Point(0, 1);
                this.TrackingMaskTable[2] = new Point(1, 1);
                this.TrackingMaskTable[3] = new Point(1, 0);
                this.TrackingMaskTable[4] = new Point(1, -1);
                this.TrackingMaskTable[5] = new Point(0, -1);
                this.TrackingMaskTable[6] = new Point(-1, -1);
                this.TrackingMaskTable[7] = new Point(-1, 0);
            }
            #endregion

            // 追跡距離
            this.TrackingDistTable = new double[this.TrackingMaskTable.Length];
            foreach (var i in Enumerable.Range(0, this.TrackingDistTable.Length))
            {
                this.TrackingDistTable[i] = System.Math.Sqrt(System.Math.Pow(this.TrackingMaskTable[i].X, 2) + System.Math.Pow(this.TrackingMaskTable[i].Y, 2));
            }
        }

        #endregion

        #region パブリックメソッド

        #region Rleデータ作成 with ラベリング

        /// <summary>
        /// Rleデータ作成開始
        /// </summary>
        /// <param name="doAsynchronousLabeling">非同期ラベリングを実行するかどうか</param>
        /// <returns></returns>
        /// <remarks>Rleデータ作成前に呼び出します</remarks>
        public bool StartRleConcatenationWithLabeling(bool doAsynchronousLabeling)
        {
            var result = false;

            try
            {
                this.LastError = RleErrorCode.NotError;
                this.LabelingEnabled = doAsynchronousLabeling;

                result = this.BeginData(0);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// ビットマップを2値化してRleデータを追加で作成する
        /// </summary>
        /// <remarks>対応ビットマップのピクセルフォーマットはPixelFormat.Format8bppIndexed</remarks>
        public bool ConcatRleFrom(Bitmap bitmap, int threshold) => this.ConcatRleFrom(bitmap, threshold, byte.MaxValue);

        /// <summary>
        /// ビットマップを2値化してRleデータを追加で作成する
        /// </summary>
        /// <remarks>対応ビットマップのピクセルフォーマットはPixelFormat.Format8bppIndexed</remarks>
        public bool ConcatRleFrom(Bitmap bitmap, int thresholdMin, int thresholdMax)
        {
            var result = this.ConvertToRle(bitmap, thresholdMin, thresholdMax);

            result &= this.CreateRleInfo();

            this.NextDataIndex = this.RleDataNumber;

            return result;
        }

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを追加で作成する
        /// </summary>
        public bool ConcatRleFrom(byte[][] bitmap, int threshold) => this.ConcatRleFrom(bitmap, threshold, byte.MaxValue);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを追加で作成する
        /// </summary>
        public bool ConcatRleFrom(byte[][] bitmap, int thresholdMin, int thresholdMax)
        {
            var result = this.ConvertToRle(bitmap, thresholdMin, thresholdMax);

            result &= this.CreateRleInfo();

            this.NextDataIndex = this.RleDataNumber;

            return result;
        }

        /// <summary>
        /// Rleデータ作成終了
        /// </summary>
        /// <returns></returns>
        /// <remarks>全Rleデータ作成後に呼び出します</remarks>
        public bool EndRleConcatenationWithLabeling(bool doForceStopLabeling = false)
        {
            var result = false;

            try
            {
                if (true == doForceStopLabeling)
                {
                    this.LabelingThread?.Break();
                }

                result = this.EndData();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        #endregion

        #region 同期ラベリング用メソッド

        /// <summary>
        /// ビットマップを2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(Bitmap bitmap, int threshold)
        {
            return this.CreateNewRleFrom(bitmap, threshold, 255);
        }

        /// <summary>
        /// ビットマップを2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(Bitmap bitmap, int thresholdMin, int thresholdMax)
        {
            // 初期化
            this.LastError = RleErrorCode.NotError;
            this.RleDataNumber = 0;

            return this.ConvertToRle(bitmap, thresholdMin, thresholdMax);
        }

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[][] bitmap, int threshold)
        {
            return this.CreateNewRleFrom(bitmap, threshold, 255);
        }

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[][] bitmap, int thresholdMin, int thresholdMax)
        {
            // 初期化
            this.LastError = RleErrorCode.NotError;
            this.RleDataNumber = 0;

            return this.ConvertToRle(bitmap, thresholdMin, thresholdMax);
        }

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[] bitmap, int stride, int threshold)
        {
            return this.CreateNewRleFrom(bitmap, stride, threshold, 255);
        }

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[] bitmap, int stride, int thresholdMin, int thresholdMax)
        {
            // 初期化
            this.LastError = RleErrorCode.NotError;
            this.RleDataNumber = 0;

            return this.ConvertToRle(bitmap, stride, thresholdMin, thresholdMax);
        }

        /// <summary>
        /// 同期ラベリング
        /// </summary>
        /// <returns></returns>
        /// <remarks>CreateNewRleFromメソッドの後に呼び出します</remarks>
        public bool SynchronousLabeling()
        {
            var result = false;

            try
            {
                this.LabelingEnabled = false;
                if (true == this.BeginData(false))
                {
                    if (true == this.EndData(false))
                    {
                        var isFinished = false;

                        this.RleDataFixed = true;
                        while (false == isFinished)
                        {
                            this.CreateLabel(out isFinished);
                        }

                        this.NextDataIndex = this.RleDataNumber;
                        this.ImageWidth = (this.ImageHeight <= 0) ? 0 : (this.RleData[this.RleLines[this.ImageHeight - 1].LastIndex].CoordX_End) + 1;

                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }


        #endregion

        #region ラベル処理

        /// <summary>
        /// すべてのラベル収集
        /// </summary>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectAllLabels()
        {
            var labels = new List<IRleLabel>(this.LabelNumber);

            foreach (var selectedLabel in this.LabelArray.Take(this.LabelNumber))
            {
                // ラベル登録
                labels.Add(selectedLabel.Clone());
            }

            return labels;
        }

        /// <summary>
        /// 有効ラベル収集
        /// </summary>
        /// <param name="offset">座標オフセット</param>
        /// <param name="include">画像内に完全に含まれる必要があるかどうか</param>
        /// <param name="selector">有効判定</param>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectValidLabels(bool checkIncluding) => this.CollectValidLabels(checkIncluding, (o => true));

        /// <summary>
        /// 有効ラベル収集
        /// </summary>
        /// <param name="offset">座標オフセット</param>
        /// <param name="include">画像内に完全に含まれる必要があるかどうか</param>
        /// <param name="selector">有効判定</param>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectValidLabels(bool checkIncluding, Func<IRleLabel, bool> selector)
        {
            // 有効ラベル初期化
            var validLabels = new List<IRleLabel>();

            // 有効ラベル抽出
            for (int i = 0; i < this.LabelNumber; i++)
            {
                // ラベル情報
                var selectedLabel = this.LabelArray[i];

                // 無効ラベル
                if (false == selectedLabel.Enabled)
                {
                    continue;
                }

                // 包含チェック
                if (true == checkIncluding)
                {
                    // 画像の最上部
                    if (selectedLabel.RectBegin.Y == 0)
                    {
                        continue;
                    }

                    // 画像の最下部
                    if (selectedLabel.RectEnd.Y == this.ImageHeight - 1)
                    {
                        continue;
                    }

                    // 画像の最左部
                    if (selectedLabel.RectBegin.X == 0)
                    {
                        continue;
                    }

                    // 画像の最右部
                    if (selectedLabel.RectEnd.X == this.ImageWidth - 1)
                    {
                        continue;
                    }
                }

                // 条件判定
                if (false == selector(selectedLabel))
                {
                    continue;
                }

                // 有効ラベル登録
                validLabels.Add(selectedLabel.Clone());
            }

            return validLabels;
        }

        /// <summary>
        /// グレイ値情報取得
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="labels">グレイ値を取得するラベルデータリスト</param>
        /// <param name="minimum">最小グレイ値</param>
        /// <param name="maximum">最大グレイ値</param>
        /// <param name="average">平均値</param>
        public void GetLabelGrayValue(Bitmap bitmap, List<IRleLabel> labels, out double[] minimum, out double[] maximum, out double[] average)
        {
            // 戻り値を初期化する
            minimum = new double[labels.Count];
            maximum = new double[labels.Count];
            average = new double[labels.Count];

            var lockedData = (BitmapData?)null;

            try
            {
                // ビットマップロック                                                                     
                lockedData = bitmap.LockBits(
                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                             , ImageLockMode.ReadWrite
                                             , bitmap.PixelFormat
                                             );

                // 処理データ配列
                var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                // ビットマップデータから処理用配列へコピー
                Marshal.Copy(lockedData.Scan0, fullPixels, 0, fullPixels.Length);

                var histogram = new int[byte.MaxValue + 1];
                int labelIndex = 0;

                // ラベル毎にグレイ値情報を取得する                               
                foreach (var selectedLabel in labels)
                {
                    double summation = 0;
                    Array.Clear(histogram, 0, histogram.Length);

                    for (int i = selectedLabel.RleDataBegin; i <= selectedLabel.RleDataEnd; i++)
                    {
                        var selectedData = this.RleData[i];
                        if (selectedData.LabelIndex == selectedLabel.Index)
                        {
                            int index = lockedData.Stride * selectedData.CoordY + selectedData.CoordX_Begin;
                            int count = selectedData.CoordX_End - selectedData.CoordX_Begin + 1;
                            for (int j = 0; j < count; j++)
                            {
                                int value = fullPixels[index++];

                                summation += value;
                                histogram[value]++;
                            }
                        }
                    }

                    minimum[labelIndex] = Array.FindIndex(histogram, (o => o > 0));
                    maximum[labelIndex] = Array.FindLastIndex(histogram, (o => o > 0));
                    average[labelIndex] = summation / selectedLabel.PixelNumber;

                    labelIndex++;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                // ビットマップアンロック
                if (null != lockedData)
                {
                    try
                    {
                        bitmap.UnlockBits(lockedData);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// ラベルのピクセル位置を取得します。
        /// </summary>
        public void GetLabelPoints(IRleLabel selectedLabel, out List<Point> points)
        {
            points = new List<Point>();

            try
            {
                for (int i = selectedLabel.RleDataBegin; i <= selectedLabel.RleDataEnd; i++)
                {
                    var selectedData = this.RleData[i];
                    if (selectedData.LabelIndex == selectedLabel.Index)
                    {
                        for (int j = selectedData.CoordX_Begin; j <= selectedData.CoordX_End; j++)
                        {
                            points.Add(new Point(j, selectedData.CoordY));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ラベルの重心位置を取得します。
        /// </summary>
        public void GetLabelGravityCenter(IRleLabel selectedLabel, out PointD point)
        {
            point = new PointD();

            try
            {
                int count = 0;

                for (int i = selectedLabel.RleDataBegin; i <= selectedLabel.RleDataEnd; i++)
                {
                    var selectedData = this.RleData[i];
                    if (selectedData.LabelIndex == selectedLabel.Index)
                    {
                        for (int j = selectedData.CoordX_Begin; j <= selectedData.CoordX_End; j++)
                        {
                            point.Offset(j, selectedData.CoordY);
                            count++;
                        }
                    }
                }

                if (0 < count)
                {
                    point.X /= count;
                    point.Y /= count;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ラベルの開始座標(左上)を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="point"></param>
        public void GetLabelStartPoint(IRleLabel selectedLabel, out Point point)
        {
            point = new Point();

            try
            {
                for (int i = selectedLabel.RleDataBegin; i <= selectedLabel.RleDataEnd; i++)
                {
                    var selectedData = this.RleData[i];
                    if (selectedData.LabelIndex == selectedLabel.Index)
                    {
                        point.X = selectedData.CoordX_Begin;
                        point.Y = selectedData.CoordY;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ラベル番号が一致するかどうか
        /// </summary>
        /// <param name="labelNo"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool EqualsLabelIndex(int selectedLabelNo, Point point) => this.IsLabelIndex(point) == selectedLabelNo;

        /// <summary>
        /// 指定座標のラベル番号を取得する
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IsLabelIndex(Point point) => this.IsLabelIndex(point.X, point.Y);

        /// <summary>
        /// 指定座標のラベル番号を取得する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int IsLabelIndex(int x, int y)
        {
            var selectedLabelNo = -1;

            if (
                (y >= 0)
            && (y < this.ImageHeight)
            && (x >= 0)
            && (x < this.ImageWidth)
            )
            {
                var selectedLine = this.RleLines[y];

                for (int i = selectedLine.FirstIndex; i <= selectedLine.LastIndex; i++)
                {
                    var selectedData = this.RleData[i];
                    if (x <= selectedData.CoordX_End)
                    {
                        selectedLabelNo = selectedData.LabelIndex;
                        break;
                    }
                }
            }

            return selectedLabelNo;
        }

        /// <summary>
        /// ラベルの円形度を取得する(コンパクト性)
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        /// <remarks>円形度 = 4π * S/L^2 (S = 面積, L = 図形の周囲長)</remarks>
        public double GetLabelCompactness(IRleLabel selectedLabel) => this.GetLabelCompactness(selectedLabel, out _);

        /// <summary>
        /// ラベルの円形度を取得する(コンパクト性)
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contlength"></param>
        /// <returns></returns>
        /// <remarks>円形度 = 4π * S/L^2 (S = 面積, L = 図形の周囲長)</remarks>
        public double GetLabelCompactness(IRleLabel selectedLabel, out double contlength)
        {
            contlength = this.GetLabelContlength(selectedLabel);

            var value = 0d;
            if (0 < contlength)
            {
                value = System.Math.PI * 4 * selectedLabel.PixelNumber / System.Math.Pow(contlength, 2);
            }

            return value;
        }

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel)
        {
            this.GetLabelGravityCenter(selectedLabel, out PointD gravityCenter);

            return this.GetLabelCircularity(selectedLabel, gravityCenter);
        }

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel, PointD gravityCenter)
        {
            var distanceMax = double.MinValue;

            #region 重心からの輪郭への距離を求める
            try
            {
                // ラベル開始座標を取得する
                var startPoint = (Point?)null;
                this.GetLabelStartPoint(selectedLabel, out startPoint);

                // 追跡情報を初期化する
                var trackingDirections = new CircularCounter(0, 7, 0);
                var currentPoint = new Point(startPoint);
                var currentDiretion = 0;
                var searchEnabled = true;

                // 輪郭追跡マスクへの参照を取得する
                var trackingMask = this.TrackingMaskTable;

                // 輪郭追跡距離テーブルへの参照を取得する
                var trackingDist = this.TrackingDistTable;

                #region 輪郭追跡処理(チェインコード式)
                while (true == searchEnabled)
                {
                    var nextFind = false;
                    for (int i = 0; i < 7; i++)
                    {
                        var searchDirection = trackingDirections.PostIncrement();
                        var offsetPoint = trackingMask[searchDirection];
                        var searchPoint = new Point(offsetPoint);
                        searchPoint.Offset(currentPoint);

                        var searchValue = this.EqualsLabelIndex(selectedLabel.Index, searchPoint);

                        if (true == searchValue)
                        {
                            var searchDist = trackingDist[searchDirection];

                            currentPoint = searchPoint;
                            currentDiretion = searchDirection;
                            trackingDirections.Counter = (currentDiretion + 6) % 8;

                            if (true == startPoint.Equals(currentPoint))
                            {
                                searchEnabled = false;
                            }

                            var relativePoint = new PointD
                            {
                                X = currentPoint.X - gravityCenter.X,
                                Y = currentPoint.Y - gravityCenter.Y
                            };

                            var distance = System.Math.Sqrt(relativePoint.X * relativePoint.X + relativePoint.Y * relativePoint.Y);

                            if (distanceMax < distance)
                            {
                                distanceMax = distance;
                            }

                            nextFind = true;

                            break;
                        }
                    }

                    if (false == nextFind)
                    {
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion

            var value = System.Math.Min(1d, selectedLabel.PixelNumber / (System.Math.Pow(distanceMax, 2) * System.Math.PI));

            return value;
        }

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel, PointD gravityCenter, List<ContourElementData> contours)
        {
            var distanceList = new List<double>();

            #region 重心からの輪郭への距離を求める
            foreach (var selectedContour in contours)
            {
                var relativePoint = new PointD
                {
                    X = selectedContour.Point.X - gravityCenter.X,
                    Y = selectedContour.Point.Y - gravityCenter.Y
                };

                var distance = System.Math.Sqrt(relativePoint.X * relativePoint.X + relativePoint.Y * relativePoint.Y);

                distanceList.Add(distance);
            }
            #endregion

            var distanceMax = distanceList.Max();

            var value = System.Math.Min(1d, selectedLabel.PixelNumber / (System.Math.Pow(distanceMax, 2) * System.Math.PI));

            return value;
        }

        /// <summary>
        /// ラベルの周囲長を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelContlength(IRleLabel selectedLabel)
        {
            // 戻り値を初期化する
            var contlength = 0d;

            try
            {
                // ラベル開始座標を取得する
                var startPoint = (Point?)null;
                this.GetLabelStartPoint(selectedLabel, out startPoint);

                // 追跡情報を初期化する
                var trackingDirections = new CircularCounter(0, 7, 0);
                var currentPoint = new Point(startPoint);
                var currentDiretion = 0;
                var searchEnabled = true;

                // 輪郭追跡マスクへの参照を取得する
                var trackingMask = this.TrackingMaskTable;

                // 輪郭追跡距離テーブルへの参照を取得する
                var trackingDist = this.TrackingDistTable;

                #region 輪郭追跡処理(チェインコード式)
                while (true == searchEnabled)
                {
                    var nextFind = false;
                    for (int i = 0; i < 7; i++)
                    {
                        var searchDirection = trackingDirections.PostIncrement();
                        var offsetPoint = trackingMask[searchDirection];
                        var searchPoint = new Point(offsetPoint);
                        searchPoint.Offset(currentPoint);

                        var searchValue = this.EqualsLabelIndex(selectedLabel.Index, searchPoint);

                        if (true == searchValue)
                        {
                            var searchDist = trackingDist[searchDirection];

                            currentPoint = searchPoint;
                            currentDiretion = searchDirection;
                            trackingDirections.Counter = (currentDiretion + 6) % 8;

                            if (true == startPoint.Equals(currentPoint))
                            {
                                searchEnabled = false;

                                // 周囲長を更新
                                contlength += searchDist;
                            }
                            else
                            {
                                // 周囲長を更新
                                contlength += searchDist;
                            }

                            nextFind = true;

                            break;
                        }
                    }

                    if (false == nextFind)
                    {
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return contlength;
        }

        /// <summary>
        /// ラベルの輪郭を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contours"></param>
        /// <param name="contourCenter"></param>
        public void GetLabelContour(IRleLabel selectedLabel, out List<ContourElementData> contours, out PointD contourCenter) => this.GetLabelContour(selectedLabel, out contours, out contourCenter, out _);

        /// <summary>
        /// ラベルの輪郭を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contours"></param>
        /// <param name="contourCenter"></param>
        public void GetLabelContour(IRleLabel selectedLabel, out List<ContourElementData> contours, out PointD contourCenter, out double contlength)
        {
            // 戻り値を初期化する
            contours = new List<ContourElementData>();
            contourCenter = new PointD();
            contlength = 0d;

            try
            {
                // ラベル開始座標を取得する
                var startPoint = (Point?)null;
                this.GetLabelStartPoint(selectedLabel, out startPoint);

                // 輪郭追跡マスクへの参照を取得する
                var trackingMask = this.TrackingMaskTable;

                // 輪郭追跡距離テーブルへの参照を取得する
                var trackingDist = this.TrackingDistTable;

                // 追跡情報を初期化する
                var trackingDirections = new CircularCounter(0, 7, 0);
                var currentPoint = new Point(startPoint);
                var currentDiretion = 0;
                var searchEnabled = true;

                // 重心算出集計値を初期化する
                var centerSumX = 0d;
                var centerSumY = 0d;

                // 開始座標を登録する
                int contourIndex = 0;
                contours.Add(new ContourElementData(contourIndex++, currentPoint));
                centerSumX += currentPoint.X;
                centerSumY += currentPoint.Y;

                #region 輪郭追跡処理(チェインコード式)
                while (true == searchEnabled)
                {
                    var nextFind = false;
                    for (int i = 0; i < 7; i++)
                    {
                        var searchDirection = trackingDirections.PostIncrement();
                        var offsetPoint = trackingMask[searchDirection];
                        var searchPoint = new Common.Drawing.Point(offsetPoint);
                        searchPoint.Offset(currentPoint);

                        var searchValue = this.EqualsLabelIndex(selectedLabel.Index, searchPoint);

                        if (true == searchValue)
                        {
                            var searchDist = trackingDist[searchDirection];

                            currentPoint = searchPoint;
                            currentDiretion = searchDirection;
                            trackingDirections.Counter = (currentDiretion + 6) % 8;

                            if (true == startPoint.Equals(currentPoint))
                            {
                                searchEnabled = false;

                                // 周囲長を更新
                                contlength += searchDist;
                            }
                            else
                            {
                                // 輪郭・重心データを更新
                                contours.Add(new ContourElementData(contourIndex++, currentPoint));
                                centerSumX += currentPoint.X;
                                centerSumY += currentPoint.Y;

                                // 周囲長を更新
                                contlength += searchDist;
                            }

                            nextFind = true;

                            break;
                        }
                    }

                    if (false == nextFind)
                    {
                        break;
                    }
                }
                #endregion

                // 重心データを更新
                var perimeter = contours.Count;
                contourCenter.X = centerSumX / perimeter;
                contourCenter.Y = centerSumY / perimeter;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// RleデータからBitmapを作成する
        /// </summary>
        /// <returns></returns>
        public Bitmap? ToBitmap()
        {
            var bitmap = (Bitmap?)null;

            try
            {
                // ビットマップ生成
                bitmap = new Bitmap(this.ImageWidth, this.ImageHeight, PixelFormat.Format8bppIndexed);

                // カラーパレット設定
                BitmapProcessor.AssignColorPaletteOfDefault(bitmap);

                // ビットマップロック                                                                     
                var lockedData = bitmap.LockBits(
                                                   new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                                 , ImageLockMode.ReadWrite
                                                 , bitmap.PixelFormat
                                                 );

                // 処理データ配列
                var fullPixels = new byte[lockedData.Stride * lockedData.Height];
                var linePixels = new byte[lockedData.Width];
                var copyPixels = Enumerable.Repeat<byte>(byte.MaxValue, linePixels.Length).ToArray();

                int lineOffset = 0;

                for (int i = 0; i < lockedData.Height; i++)
                {
                    var selectedLine = this.RleLines[i];

                    Array.Clear(linePixels, 0, linePixels.Length);

                    int columnOffset = 0;

                    for (int j = selectedLine.FirstIndex; j <= selectedLine.LastIndex; j++)
                    {
                        var selectedData = this.RleData[j];
                        int length = selectedData.CoordX_End - selectedData.CoordX_Begin + 1;

                        if (0 != selectedData.Elem)
                        {
                            Array.Copy(copyPixels, 0, linePixels, columnOffset, length);
                        }

                        columnOffset += length;
                    }

                    Array.Copy(linePixels, 0, fullPixels, lineOffset, linePixels.Length);
                    lineOffset += lockedData.Stride;
                }

                Marshal.Copy(fullPixels, 0, lockedData.Scan0, fullPixels.Length);

                bitmap.UnlockBits(lockedData);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return bitmap;
        }

        /// <summary>
        /// RLEデータをファイルから読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Load(FileInfo fileInfo)
        {
            bool result = false;

            try
            {
                using (var stream = new System.IO.FileStream(fileInfo.FullName, System.IO.FileMode.Open))
                using (var reader = new System.IO.BinaryReader(stream))
                {
                    var tempDataNumber = reader.ReadInt32();
                    var tempDataArray = new ushort[tempDataNumber];

                    for (int i = 0; i < tempDataNumber; i++)
                    {
                        tempDataArray[i] = reader.ReadUInt16();
                    }

                    Array.Copy(tempDataArray, this.RleDataRawArray, tempDataNumber);
                    this.RleDataNumber = tempDataNumber;
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// RLEデータをファイルに保存する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Save(FileInfo fileInfo)
        {
            bool result = false;

            try
            {
                using (var stream = new System.IO.FileStream(fileInfo.FullName, System.IO.FileMode.Create))
                using (var writer = new System.IO.BinaryWriter(stream))
                {
                    writer.Write(this.RleDataNumber);
                    for (int i = 0; i < this.RleDataNumber; i++)
                    {
                        writer.Write(this.RleDataRawArray[i]);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// RLE:生データのコピー取得する
        /// </summary>
        public bool CopyTo(out int copyDataNumber, out ushort[] copyDataArray)
        {
            var result = false;

            copyDataNumber = 0;
            copyDataArray = Array.Empty<ushort>();

            try
            {
                if (0 < this.RleDataNumber)
                {
                    copyDataNumber = this.RleDataNumber;
                    copyDataArray = new ushort[copyDataNumber];
                    Array.Copy(this.RleDataRawArray, copyDataArray, copyDataArray.Length);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// RLEデータのコピーを取得する
        /// </summary>
        public bool CopyTo(out IRleData[] copyData)
        {
            var result = false;

            copyData = Array.Empty<IRleData>();

            try
            {
                if (0 < this.RleDataNumber)
                {
                    copyData = new IRleData[this.RleDataNumber];
                    for (int i = 0; i < copyData.Length; i++)
                    {
                        copyData[i] = this.RleData[i].Clone();
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// RLEライン情報のコピーを取得する
        /// </summary>
        public bool CopyTo(out IRleLineInfo[] copyLines)
        {
            var result = false;

            copyLines = Array.Empty<IRleLineInfo>();

            try
            {
                if (0 < this.ImageHeight)
                {
                    copyLines = new IRleLineInfo[this.ImageHeight];
                    for (int i = 0; i < copyLines.Length; i++)
                    {
                        copyLines[i] = this.RleLines[i].Cloe();
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// RLEデータ設定
        /// </summary>
        /// <returns></returns>
        protected bool BeginData(bool isOtherThread = true)
        {
            return this.BeginData(this.RleDataNumber, isOtherThread);
        }

        /// <summary>
        /// RLEデータ設定
        /// </summary>
        /// <returns></returns>
        protected bool BeginData(int dataNumber, bool isOtherThread = true)
        {
            // 初期化:ライン
            foreach (var selectedItem in this.RleLines)
            {
                selectedItem.Invalidate();
            }

            // 初期化:RLE
            this.NextDataIndex = 0;     //次処理位置
            this.RleDataNumber = dataNumber;    //数
            this.RleDataFixed = false; //終了フラグクリア
            this.ImageHeight = 0;     //画像高さ
            this.ImageWidth = 0;     //画像幅幅

            // 初期化:ラベル
            this.LabelNumber = 0;
            this.NextLineIndex = 0;

            // 処理データ作成
            if (false == this.CreateRleInfo())
            {
                return false;
            }

            // ラベリングが有効である場合
            if (true == this.LabelingEnabled)
            {
                if (isOtherThread)
                {
                    // ラベリング開始
                    this.LabelingThread?.Start();
                }
                else
                {
                    //別スレッド使用しない場合は直接ラベル作成を完了するまで行う。
                    var isFinished = false;

                    //終了確認のフラグは立てる
                    this.RleDataFixed = true;

                    while (false == isFinished)
                    {
                        this.CreateLabel(out isFinished);
                    }
                }
            }

            // 次処理位置
            this.NextDataIndex = this.RleDataNumber;

            return true;
        }

        /// <summary>
        /// RLEデータ設定
        /// </summary>
        /// <returns></returns>
        protected bool EndData(bool isOtherThread = true)
        {
            var result = true;

            // 処理データ作成
            if (false == this.CreateRleInfo())
            {
                return false;
            }

            // ラベリング終了待ち
            this.RleDataFixed = true;

            // ラベリングが有効である場合
            if (true == this.LabelingEnabled)
            {
                //別スレッド使用しない場合はWaitを呼ぶと無限待ちになるので、呼ばない。
                if (isOtherThread)
                {
                    result = this.LabelingThread?.Wait() ?? false;
                }

                if (false == result)
                {
                    this.LastError = this.LabelingThread?.ErrorCode.Last() ?? RleErrorCode.Undefined;
                }

                // ラベリング終了
                this.LabelingThread?.Stop();
            }

            // 幅の設定
            this.ImageWidth = (this.ImageHeight <= 0) ? 0 : (this.RleData[this.RleLines[this.ImageHeight - 1].LastIndex].CoordX_End) + 1;

            return result;
        }

        /// <summary>
        /// 処理データ作成
        /// </summary>
        /// <returns></returns>
        protected bool CreateRleInfo()
        {
            // 領域不足の場合
            if (
                (this.RleLines.Length <= this.ImageHeight)
            && (this.NextDataIndex < this.RleDataNumber)
            )
            {
                this.LastError = RleErrorCode.OverflowOfLine;

                // ラベリングが有効である場合
                if (true == this.LabelingEnabled)
                {
                    // スレッド停止
                    this.LabelingThread?.Stop();
                }
                return false;
            }

            // 高さ保持(注意:ラベリング処理から参照)
            int tempHeight = this.ImageHeight;

            // 生データから、RLEデータを作成
            for (int i = this.NextDataIndex; i < this.RleDataNumber; i++)
            {
                var selectedData = this.RleDataRawArray[i];

                this.RleData[i].IsLeading = (ushort)((selectedData & 0x8000) >> 15);

                if (0 != this.RleData[i].IsLeading)
                {
                    this.RleData[i].CoordX_Begin = 0;

                    // 領域不足の場合
                    if (this.RleLines.Length <= tempHeight)
                    {
                        this.LastError = RleErrorCode.OverflowOfLine;
                        return false;
                    }

                    tempHeight++;
                }
                else
                {
                    this.RleData[i].CoordX_Begin = (ushort)(this.RleData[i - 1].CoordX_End + 1);
                }

                this.RleData[i].CoordX_End = (ushort)(selectedData & 0x3FFF);
                this.RleData[i].CoordY = tempHeight - 1;
                this.RleData[i].LabelIndex = this.RleData[i].InvalidLabelIndex;
                this.RleData[i].Elem = (ushort)((selectedData & 0x4000) >> 8);
            }

            // RLEデータ → ライン情報の作成
            for (int i = this.NextDataIndex; i < this.RleDataNumber; i++)
            {
                var index = this.RleData[i].CoordY;

                if (0 != this.RleData[i].IsLeading)
                {
                    this.RleLines[index].FirstIndex = i;
                    this.RleLines[index].DataNumber = 0;
                }

                this.RleLines[index].LastIndex = i;
                this.RleLines[index].DataNumber++;
            }

            // 高さ更新
            this.ImageHeight = tempHeight;

            return true;
        }

        /// <summary>
        /// ラベル作成
        /// </summary>
        /// <returns></returns>
        protected bool CreateLabel(out bool isFinished)
        {
            // 取込完了のライン数
            int lineHeight = this.ImageHeight;

            //取込完了前
            if (false == this.RleDataFixed)
            {
                //取込完了前：１つ前ラインまで
                lineHeight--;
            }

            isFinished = false;

            //途中
            if (false == this.RleDataFixed)
            {
            }
            //全ライン
            else if (this.NextLineIndex >= lineHeight)
            {
                isFinished = true;
                return true;
            }

            // 取込完了ライン：なし
            if (0 >= lineHeight)
            {
                return true;
            }

            // 先頭ライン（１ラベルの先頭）は前ラインがないので、全てラベル先頭
            if (this.NextLineIndex == 0)
            {
                //ライン内のデータ分
                for (int j = 0; j < this.RleLines[0].DataNumber; j++)
                {
                    //データ位置
                    int index = this.RleLines[0].FirstIndex + j;

                    //ラベル情報：追加
                    this.AddLabel(ref this.LabelNumber, index);
                }

                this.NextLineIndex++;
            }

            var ps = new int[2];    //比較データ：位置
            var xs = new int[2];    //比較データ：始点
            var xe = new int[2];    //比較データ：終点

            // 全ラインラベリング
            for (int i = this.NextLineIndex; i < lineHeight; i++)
            {
                //ライン内のデータ分    
                for (int j = 0; j < this.RleLines[i].DataNumber; j++)
                {
                    ps[0] = this.RleLines[i].FirstIndex + j;           //現ライン：１データ位置
                    xs[0] = this.RleData[ps[0]].CoordX_Begin;            //現ライン：１データ始点
                    xe[0] = this.RleData[ps[0]].CoordX_End;            //現ライン：１データ終点

                    // 前ライン：先頭接触データ検索
                    int searchId;
                    int searchSp = this.RleLines[i - 1].FirstIndex;                              //検索範囲：始点
                    int searchEp = this.RleLines[i - 1].FirstIndex + this.RleLines[i - 1].DataNumber - 1;    //検索範囲：終点
                    int matchedId = -1;                                               //合致：初期化
                    for (; ; )
                    {
                        //検索範囲：中点
                        searchId = searchSp + (searchEp - searchSp) / 2;

                        // 接触の確認
                        xs[1] = this.RleData[searchId].CoordX_Begin;
                        xe[1] = this.RleData[searchId].CoordX_End;
                        if (((xs[0] >= xs[1]) && (xs[0] <= xe[1]))     //現：始点が、 前ライン内側
                        || ((xe[0] >= xs[1]) && (xe[0] <= xe[1]))     //現：終点が、 前ライン内側
                        || ((xs[0] <= xs[1]) && (xe[0] >= xe[1])))    //現：始終点が、前ライン外側
                        {
                            // 先頭で一致：検索終了
                            if (0 == searchId)
                            {
                                break;
                            }

                            // 前接触なし：検索終了
                            //
                            // 1****  **   [1]前ライン始点
                            //   2*******  [2]現ライン始点
                            if (xs[0] >= xs[1])
                            {
                                break;
                            }

                            // 前接触なし：検索終了
                            // 形状的に、この形が多いため。
                            //
                            // 3･････1*   [1]前ライン：現始点
                            //   2******* [2]現ライン始点
                            //            [3]前ライン：前始点
                            if (this.RleData[searchId - 1].CoordX_Begin <= xs[0])
                            {
                                searchId--;
                                break;
                            }

                            // 合致２回（自点より前なし）
                            if (matchedId == searchId)
                            {
                                break;
                            }
                            // 前に別ラベルの可能性がある
                            // 前方向に再度検索
                            else
                            {
                                matchedId = searchId;                 //合致：位置退避
                                searchSp = this.RleLines[i - 1].FirstIndex;      //検索範囲：前へ
                                searchEp = searchId;
                            }
                        }

                        // 接触なし
                        else if (this.RleData[searchId].CoordX_Begin > xe[0])
                        {
                            //検索範囲：前へ
                            searchEp = --searchId;
                        }
                        else if (this.RleData[searchId].CoordX_End < xs[0])
                        {
                            //検索範囲：後へ
                            searchSp = ++searchId;
                        }
                    }

                    // １つ上のラインから、連結するデータを検索
                    //
                    //     2***  3**  [1]処理ライン
                    //  1***********  [2]確認対象：１つめデータ
                    //                [3]確認対象：２つめデータ
                    searchEp = this.RleLines[i - 1].FirstIndex + this.RleLines[i - 1].DataNumber;

                    //ライン内のデータ分
                    for (ps[1] = searchId; ps[1] < searchEp; ps[1]++)
                    {
                        // 接触確認
                        xs[1] = this.RleData[ps[1]].CoordX_Begin;                   //前ライン：１データ始点
                        xe[1] = this.RleData[ps[1]].CoordX_End;                   //前ライン：１データ終点
                        if (
                           ((xs[0] >= xs[1]) && (xs[0] <= xe[1]))     //現：始点が、 前ライン内側
                        || ((xe[0] >= xs[1]) && (xe[0] <= xe[1]))     //現：終点が、 前ライン内側
                        || ((xs[0] <= xs[1]) && (xe[0] >= xe[1]))     //現：始終点が、前ライン外側
                        )
                        {
                            // 要素確認
                            if (this.RleData[ps[0]].Elem == this.RleData[ps[1]].Elem)
                            {
                                // 連結
                                //
                                //      2***    [1][2]を同一の
                                //  1*********        ラベルにする
                                this.ConcatLabel(i, ps[1], ps[0]);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 連結するデータなし（１ラベルの先頭）
                    //
                    //       2**  [1][2]は接触しない
                    //  1**             ラベルを追加する
                    if (this.RleData[ps[0]].InvalidLabelIndex == this.RleData[ps[0]].LabelIndex)
                    {
                        //領域なし 
                        if (this.LabelNumber >= this.LabelArray.Length)
                        {
                            this.LabelingThread?.AddError(RleErrorCode.OverflowOfLabel);
                            isFinished = true;

                            return false;
                        }

                        //ラベル情報：追加
                        this.AddLabel(ref this.LabelNumber, ps[0]);
                    }
                }
                this.NextLineIndex++;
            }

            return true;
        }

        /// <summary>
        /// ラベル情報の新規追加
        /// </summary>
        /// <param name="labelIndex"></param>
        /// <param name="infoIndex"></param>
        protected virtual void AddLabel(ref int labelIndex, int infoIndex)
        {
            var selectedLabel = this.LabelArray[labelIndex]; //ラベル情報
            var selectedInfo = this.RleData[infoIndex];  //Rleデータ情報

            // ラベル有効
            selectedLabel.Enabled = true;

            // データ数
            selectedLabel.RleDataNumber = 1;

            // 面積
            selectedLabel.PixelNumber = selectedInfo.CoordX_End - selectedInfo.CoordX_Begin + 1;

            // 要素
            selectedLabel.RleDataElem = selectedInfo.Elem;

            // 始点/終点
            selectedLabel.RleDataBegin = infoIndex;
            selectedLabel.RleDataEnd = infoIndex;

            // 矩形始点
            selectedLabel.RectBegin.X = selectedInfo.CoordX_Begin;
            selectedLabel.RectBegin.Y = selectedInfo.CoordY;

            // 矩形終点
            selectedLabel.RectEnd.X = selectedInfo.CoordX_End;
            selectedLabel.RectEnd.Y = selectedInfo.CoordY;

            // サイズ
            selectedLabel.RectSize.Width = selectedLabel.RectEnd.X - selectedLabel.RectBegin.X + 1;
            selectedLabel.RectSize.Height = selectedLabel.RectEnd.Y - selectedLabel.RectBegin.Y + 1;

            // ラベルインデックス
            selectedInfo.LabelIndex = (int)labelIndex;

            // ラベル数更新
            labelIndex++;
        }

        /// <summary>
        /// ラベル情報の連結
        /// </summary>
        /// <param name="lineY"></param>
        /// <param name="formerIndex"></param>
        /// <param name="latterIndex"></param>
        protected virtual void ConcatLabel(int lineY, int formerIndex, int latterIndex)
        {
            var formerLabelNo = this.RleData[formerIndex].LabelIndex;         //前ライン:ラベル番号
            var latterLabelNo = this.RleData[latterIndex].LabelIndex;         //後ライン:ラベル番号

            // 処理する意味なし
            if (formerLabelNo == latterLabelNo)
            {
                return;
            }

            var invalidLabelIndex = new RleData().InvalidLabelIndex;

            // ラベル番号の小さい方が有効
            if (formerLabelNo < latterLabelNo)
            {
                //          **    [1]結合する側
                //    **   1***   [2]結合される（削除されるラベル）
                //  2***********
                //
                // 無効にするラベルに、ラベル情報：あり
                if (invalidLabelIndex != latterLabelNo)
                {
                    // ラベル情報：置換
                    this.ReplaceLabel(lineY, latterLabelNo, formerLabelNo);
                }

                //          1***  [1]結合する側
                //  2***********  [2]結合される（まだラベルに登録されてない）
                //
                //ラベル情報：なし
                else
                {
                    var selectedLabel = this.LabelArray[formerLabelNo];         //ラベル情報
                    var selectedInfo = this.RleData[latterIndex];               //Rleデータ情報

                    selectedInfo.LabelIndex = (ushort)formerLabelNo;                 //ラベル番号設定

                    selectedLabel.RleDataNumber++;                                 //データ数加算
                    selectedLabel.RleDataEnd = latterIndex;                        //データ：終点

                    //面積加算
                    selectedLabel.PixelNumber += selectedInfo.CoordX_End - selectedInfo.CoordX_Begin + 1;

                    //画像枠の設定
                    if (selectedLabel.RectBegin.X > selectedInfo.CoordX_Begin) { selectedLabel.RectBegin.X = selectedInfo.CoordX_Begin; }
                    if (selectedLabel.RectBegin.Y > selectedInfo.CoordY) { selectedLabel.RectBegin.Y = selectedInfo.CoordY; }
                    if (selectedLabel.RectEnd.X < selectedInfo.CoordX_End) { selectedLabel.RectEnd.X = selectedInfo.CoordX_End; }
                    if (selectedLabel.RectEnd.Y < selectedInfo.CoordY) { selectedLabel.RectEnd.Y = selectedInfo.CoordY; }

                    //高さ・幅の設定
                    selectedLabel.RectSize.Width = selectedLabel.RectEnd.X - selectedLabel.RectBegin.X + 1;
                    selectedLabel.RectSize.Height = selectedLabel.RectEnd.Y - selectedLabel.RectBegin.Y + 1;
                }
            }

            //    **         [1]結合される（削除されるラベル）
            //    ***  1***  [2]結合する側
            //  2**********
            //
            // 後ラインが優先
            else
            {
                // ラベル情報置換
                this.ReplaceLabel(lineY, formerLabelNo, latterLabelNo);
            }
        }

        /// <summary>
        /// ラベル情報の置換
        /// </summary>
        /// <param name="lineY"></param>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected void ReplaceLabel(int lineY, int oldIndex, int newIndex)
        {
            // 現在ラインより上
            for (int i = lineY; i >= 0; i--)
            {
                //終了フラグ・クリア
                var canContinue = false;

                //ライン内のデータ分
                for (int j = 0; j < this.RleLines[i].DataNumber; j++)
                {
                    int r = this.RleLines[i].FirstIndex + j;

                    //置換ラベル：以外無視
                    if (this.RleData[r].LabelIndex != oldIndex)
                    {
                        continue;
                    }

                    //置換
                    this.RleData[r].LabelIndex = (ushort)newIndex;

                    //置換継続
                    canContinue = true;
                }

                // 置換するデータが無いとき：処理終了
                // 開始ラインは、置換が無くても処理を継続「下記:Type1」
                //
                //              Type1
                //         3**  [1]処理（開始）ライン
                //    2*    **  [2]で一度ラベリングされている
                // 1**********  [3]に置換する際、[1]も置換する
                //
                //              Type2
                // 2**          [1]処理（開始）ライン
                //  **      3*  [2]でラベリングされている
                // 1**********  [3]を置換する際、[1]は含めない
                //

                //検索開始ライン：置換なしでも終了しない
                if (i != lineY)
                {
                    //現ライン置換なし：終了
                    if (canContinue == false)
                    {
                        break;
                    }
                }
            }

            #region ラベル情報の更新
            {
                var newLabel = this.LabelArray[newIndex];      //更新するラベル
                var delLabel = this.LabelArray[oldIndex];      //削除するラベル

                // 面積/データ数の加算
                newLabel.PixelNumber += delLabel.PixelNumber;
                newLabel.RleDataNumber += delLabel.RleDataNumber;

                //ラベルの無効化
                delLabel.Enabled = false;

                //データ:始点/終点
                if (newLabel.RleDataBegin > delLabel.RleDataBegin) { newLabel.RleDataBegin = delLabel.RleDataBegin; }
                if (newLabel.RleDataEnd < delLabel.RleDataEnd) { newLabel.RleDataEnd = delLabel.RleDataEnd; }

                //画像枠の更新                                                            
                if (newLabel.RectBegin.X > delLabel.RectBegin.X) { newLabel.RectBegin.X = delLabel.RectBegin.X; }
                if (newLabel.RectBegin.Y > delLabel.RectBegin.Y) { newLabel.RectBegin.Y = delLabel.RectBegin.Y; }
                if (newLabel.RectEnd.X < delLabel.RectEnd.X) { newLabel.RectEnd.X = delLabel.RectEnd.X; }
                if (newLabel.RectEnd.Y < delLabel.RectEnd.Y) { newLabel.RectEnd.Y = delLabel.RectEnd.Y; }

                //高さ・幅の設定
                newLabel.RectSize.Width = newLabel.RectEnd.X - newLabel.RectBegin.X + 1;
                newLabel.RectSize.Height = newLabel.RectEnd.Y - newLabel.RectBegin.Y + 1;
            }
            #endregion
        }

        /// <summary>
        /// Rleデータ作成
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        /// <returns></returns>
        protected bool ConvertToRle(Bitmap bitmap, int thresholdMin, int thresholdMax)
        {
            // 初期化
            var rldNum = this.RleDataNumber;
            var result = true;
            var lockedData = (BitmapData?)null;

            try
            {
                // ビットマップロック
                lockedData = bitmap.LockBits(
                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                             , ImageLockMode.ReadWrite
                                             , bitmap.PixelFormat
                                             );

                // 処理用配列確保
                var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                // 処理用配列にコピー
                Marshal.Copy(lockedData.Scan0, fullPixels, 0, fullPixels.Length);

                int lineOffset = 0;

                // 256階調の2値化LUT作成
                var thresholdLut = this.CreateThresholdLut(thresholdMin, thresholdMax);

                // 1ライン単位でRleデータ作成
                for (int y = 0; y < lockedData.Height; y++)
                {
                    // 1ラインデータからRleデータ作成
                    if (false == this.ConvertToRle(fullPixels, thresholdLut, ref rldNum, this.RleDataRawArray, lineOffset, lineOffset + lockedData.Width - 1))
                    {
                        result = false;
                        break;
                    }

                    lineOffset += lockedData.Stride;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                rldNum = this.RleDataNumber;
                result = false;
            }
            finally
            {
                // ビットマップロック解除
                if (lockedData is not null)
                {
                    bitmap.UnlockBits(lockedData);
                }
            }

            this.RleDataNumber = rldNum;

            return result;
        }

        /// <summary>
        /// RLE作成
        /// </summary>
        /// <param name="bitmap">グレイ値配列</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        /// <returns></returns>
        protected bool ConvertToRle(byte[][] bitmap, int thresholdMin, int thresholdMax)
        {
            // 初期化
            var rldNum = this.RleDataNumber;
            var result = true;

            try
            {
                // 256階調の2値化LUT作成
                var thresholdLut = this.CreateThresholdLut(thresholdMin, thresholdMax);

                // 1ライン単位でRleデータ作成
                for (int y = 0; y < bitmap.Length; y++)
                {
                    // 1ラインデータからRleデータ作成
                    if (false == this.ConvertToRle(bitmap[y], thresholdLut, ref rldNum, this.RleDataRawArray, 0, bitmap[y].Length - 1))
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                rldNum = this.RleDataNumber;
                result = false;
            }

            this.RleDataNumber = rldNum;

            return result;
        }

        /// <summary>
        /// RLE作成
        /// </summary>
        /// <param name="bitmap">グレイ値配列</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        /// <returns></returns>
        protected bool ConvertToRle(byte[] bitmap, int stride, int thresholdMin, int thresholdMax)
        {
            // 初期化
            var rldNum = this.RleDataNumber;
            var result = true;

            try
            {
                // 256階調の2値化LUT作成
                var thresholdLut = this.CreateThresholdLut(thresholdMin, thresholdMax);

                var height = bitmap.Length / stride;

                // 1ライン単位でRle作成
                var rowOffset = 0;
                for (int y = 0; y < height; y++)
                {
                    var rowSegment = new ArraySegment<byte>(bitmap, rowOffset, stride);

                    var array = rowSegment.ToArray();

                    // 1ラインデータからRle作成
                    if (false == this.ConvertToRle(array, thresholdLut, ref rldNum, this.RleDataRawArray, 0, array.Length - 1))
                    {
                        result = false;
                        break;
                    }

                    rowOffset += stride;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                rldNum = this.RleDataNumber;
                result = false;
            }

            this.RleDataNumber = rldNum;

            return result;
        }

        /// <summary>
        /// 256階調の2値化LUT作成
        /// </summary>
        protected bool[] CreateThresholdLut(int thresholdMin, int thresholdMax)
        {
            var thresholdLut = new bool[byte.MaxValue + 1];
            var hiValues = Enumerable.Repeat<bool>(true, thresholdMax - thresholdMin + 1).ToArray();
            Array.Copy(hiValues, 0, thresholdLut, thresholdMin, hiValues.Length);

            return thresholdLut;
        }

        /// <summary>
        /// RLE：作成
        /// </summary>
        protected bool ConvertToRle(byte[] source, bool[] thresholdLut, ref int number, ushort[] data, int left, int right)
        {
            var isTop = true; //先頭フラグ
            var xoff = left;

            // Rleデータ作成
            for (int i = left; i <= right; i++)
            {
                ushort elm;    //要素

                // 要素判定 白
                if (true == thresholdLut[source[i]])
                {
                    // 変化点検索
                    elm = 0x4000;
                    for (; i < right; i++)
                    {
                        if (false == thresholdLut[source[i + 1]])
                        {
                            break;
                        }
                    }
                }
                // 要素判定 黒
                else
                {
                    // 変化点検索
                    elm = 0x0000;
                    for (; i < right; i++)
                    {
                        if (true == thresholdLut[source[i + 1]])
                        {
                            break;
                        }
                    }
                }

                // Rleデータ生成(ライン先頭)
                bool result;
                if (true == isTop)
                {
                    result = this.ConvertToLeadingRle(ref number, data, elm, i - xoff);
                    isTop = false;
                }
                // Rleデータ生成(ライン途中)
                else
                {
                    result = this.ConvertToIntermediateRle(ref number, data, elm, i - xoff);
                }

                // オーバーフロー確認
                if (false == result)
                {
                    return false;
                }
            }

            // 最終データ数の決定
            number++;

            return true;
        }

        /// <summary>
        /// Rleデータ作成:ライン先頭
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="element"></param>
        /// <param name="pointX"></param>
        /// <returns></returns>
        protected bool ConvertToLeadingRle(ref int number, ushort[] data, ushort element, int pointX)
        {
            // オーバーフロー確認
            if (data.Length <= number)
            {
                return false;
            }

            // RLE設定
            data[number] = 0x8000;                          //先頭ON
            data[number] |= element;                        //要素
            data[number] |= (ushort)(pointX & 0x3FFF);      //座標

            return true;
        }

        /// <summary>
        /// Rleデータ作成:ライン途中
        /// </summary>
        /// <param name="number"></param>
        /// <param name="data"></param>
        /// <param name="element"></param>
        /// <param name="pointX"></param>
        /// <returns></returns>
        protected bool ConvertToIntermediateRle(ref int number, ushort[] data, ushort element, int pointX)
        {
            // 同一要素の場合
            if ((data[number] & 0x4000) == element)
            {
                // 座標更新
                data[number] = (ushort)((data[number] & 0xE000) | (pointX & 0x3FFF));
            }
            // 別要素の場合	        
            else
            {
                // オーバーフロー
                if (data.Length <= ++number)
                {
                    return false;
                }

                // RLD設定
                data[number] = 0x0000;                         //先頭OFF
                data[number] = element;                        //要素
                data[number] |= (ushort)(pointX & 0x3FFF);     //座標
            }

            return true;
        }

        #endregion

        #region ラベリングスレッドイベント

        /// <summary>
        /// ラベリングスレッド処理
        /// </summary>
        private void labelingProcess()
        {
            try
            {
                if (this.LabelingThread is null)
                {
                    throw new Exception("LabelingThread is null ");
                }

                while (true == this.LabelingThread.Enabled)
                {
                    try
                    {
                        // 待機
                        this.LabelingThread.ControlEvent.WaitOne();

                        // 高速モードではない場合
                        if (false == this.LabelingThread.IsFastMode)
                        {
                            // ウェイトを行う
                            Thread.Sleep(this.LabelingThread.RestPeriods);
                        }

                        // 無効化されてい場合
                        if (false == this.LabelingThread.Enabled)
                        {
                            // 処理を中断する
                            break;
                        }

                        // ラベリングを行う
                        var result = this.CreateLabel(out bool isFinished);

                        if ((false == result) || (true == isFinished))
                        {
                            this.LabelingThread.IsFastMode = false;
                            this.LabelingThread.Break();
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
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