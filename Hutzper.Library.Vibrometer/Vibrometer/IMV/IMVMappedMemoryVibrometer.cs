using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Vibrometer.Data;
using Hutzper.Library.Vibrometer.Data.Vibrometer.IMV;
using Hutzper.Library.Vibrometer.Vibrometer.MappedMemory;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace Hutzper.Library.Vibrometer.Vibrometer.IMV
{
    /// <summary>
    /// マップドメモリ通信 振動計クラス
    /// </summary>
    public class IMVMappedMemoryVibrometer : ControllerBase, IMappedMemoryVibrometer
    {
        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Close();
                this.memoryMappedFile?.Dispose();
                this.driverProcess?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.memoryMappedFile = null;
            }
        }

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
                if (parameter is IMVMappedMemoryVibrometerParameter param)
                {
                    this.parameter = param;
                }
                else
                {
                    throw new Exception("parameter is not IMVMappedMemoryVibrometerParameter");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IVibrometer

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool Enabled { get; protected set; }

        /// <summary>
        /// 停止信号ON/OFF
        /// </summary>
        public bool StopDeviceFlag
        {
            get => this.stopDeviceFlag;

            set
            {
                try
                {
                    var previousFlag = this.stopDeviceFlag;

                    this.stopDeviceFlag = value;

                    if (true == this.Enabled && this.stopDeviceFlag != previousFlag)
                    {
                        if (true == this.stopDeviceFlag)
                        {
                            Serilog.Log.Information("StopOn");
                            this.EventOfStopSignalOn?.Set();
                        }
                        else
                        {
                            Serilog.Log.Information("StopOFF");
                            this.EventOfStopSignalOff?.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// データ受信イベント
        /// </summary>
        public event Action<object, IVibrometerResult>? DataReceived;

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, IVibrometerErrorInfo>? Error;

        /// <summary>
        /// オープン
        /// </summary>
        /// <param name="parameter"></param>
        public override bool Open()
        {
            try
            {
                var myParam = this.parameter;
                if (myParam == null)
                {
                    throw new Exception("this.parameter is null");
                }
                // 共有メモリ作成
                this.memoryMappedFile = MemoryMappedFile.CreateNew(myParam!.NameOfMappedMemory, myParam!.MappedMemoryBytes);

                // ドライバ起動
                this.driverProcess = new Process();
                this.driverProcess.StartInfo.FileName = myParam.DriverExecutablePath;
                this.driverProcess.EnableRaisingEvents = true;
                this.driverProcess.Start();
                Serilog.Log.Information($"driver process start");

                // 監視スレッド
                this.threadControlFlag?.Dispose();
                this.threadControlFlag = new ThreadControlFlag() { IsTerminate = false };
                var monitoringThread = new Thread(() =>
                {
                    monitoringProcess(this.memoryMappedFile, this.threadControlFlag);
                })
                { IsBackground = true };

                // 監視処理
                void monitoringProcess(MemoryMappedFile mappedFile, ThreadControlFlag controlFlag)
                {
                    try
                    {
                        // 監視開始遅延
                        if (0 < myParam.MonitoringStartDelayMs)
                        {
                            Thread.Sleep(myParam.MonitoringStartDelayMs);
                        }
                        Serilog.Log.Information($"monitoring start (wait {myParam.MonitoringStartDelayMs}ms)");

                        var buffer = new byte[myParam.MappedMemoryBytes];
                        var counter = new CircularCounter(0, int.MaxValue);

                        using var accessor = mappedFile.CreateViewAccessor();
                        using var dataEvent = new EventWaitHandle(false, EventResetMode.AutoReset, myParam.EventNameOfMappedMemoryUpdated);

                        var waitHandleArray = new EventWaitHandle[]
                        {
                            dataEvent
                        ,   controlFlag.EventOfTerminate
                        };

                        var sensorInputStopwatch = new Stopwatch[] { new Stopwatch(), new Stopwatch() };

                        this.EventOfStopSignalOn = new EventWaitHandle(false, EventResetMode.AutoReset, myParam.EventNameOfStopSignalOn, out bool createdNew1);
                        this.EventOfStopSignalOff = new EventWaitHandle(false, EventResetMode.AutoReset, myParam.EventNameOfStopSignalOff, out bool createdNew2);
                        this.EventOfDriverEnd = new EventWaitHandle(false, EventResetMode.AutoReset, myParam.EventNameOfDriverEnd, out bool createdNew3);

                        Serilog.Log.Debug($"EventNameOfStopSignalOn is createdNew = {createdNew1}");
                        Serilog.Log.Debug($"EventNameOfStopSignalOff is createdNew = {createdNew2}");
                        Serilog.Log.Debug($"EventNameOfDriverEnd is createdNew = {createdNew3}");

                        while (false == controlFlag.IsTerminate)
                        {
                            // 書き込みを待機する
                            WaitHandle.WaitAny(waitHandleArray);

                            if (true == controlFlag.IsTerminate)
                            {
                                break;
                            }

                            // データを読み込む
                            counter.PostIncrement();
                            accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);

                            // センサ入力
                            var input = new int[2];
                            input[0] = BitConverter.ToInt16(buffer, myParam.AllocationOffsetOfSensorInput1);
                            input[1] = BitConverter.ToInt16(buffer, myParam.AllocationOffsetOfSensorInput2);

                            // 入力変化検知
                            var inputChanged = new bool[input.Length];
                            foreach (var i in Enumerable.Range(0, input.Length))
                            {
                                var stopWatch = sensorInputStopwatch[i];

                                if (input[i] == 1)
                                {
                                    inputChanged[i] = !stopWatch.IsRunning;

                                    if (true == inputChanged[i])
                                    {
                                        stopWatch.Restart();
                                    }
                                }
                                else if (true == stopWatch.IsRunning && myParam.SensorInputFallDetectionDelaySecond < stopWatch.Elapsed.TotalSeconds)
                                {
                                    inputChanged[i] = true;
                                    stopWatch.Stop();
                                }
                            }

                            // データ作成
                            var result = new IMVMappedMemoryVibrometerResult()
                            {
                                SequenceNumber = (uint)counter.PostIncrement(),
                                DateTime = DateTime.Now,
                                Value = BitConverter.ToInt16(buffer, myParam.AllocationOffsetOfData),
                                SensorInputChanged = inputChanged,
                                SensorInputValues = Array.ConvertAll(input, s => s > 0).ToArray()
                            };

                            // イベント通知
                            this.OnDataReceived(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                }

                monitoringThread.Start();

                this.Enabled = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        public override bool Close()
        {
            this.Enabled = false;
            this.threadControlFlag?.Terminate();

            try
            {
                var exitedWait = new ManualResetEvent(false);
                if (this.driverProcess is null)
                {
                    throw new InvalidOperationException("driverProcess is null");
                }
                this.driverProcess.Exited += ((sender, e) =>
                {
                    try
                    {
                        exitedWait.Set();
                        Serilog.Log.Information($"driverProcess exited.");
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                });

                this.StopDeviceFlag = false;
                this.EventOfDriverEnd?.Set();

                try
                {
                    exitedWait.WaitOne(3000);

                    if (false == this.driverProcess?.HasExited)
                    {
                        Serilog.Log.Warning($"driverProcess killed.");
                        this.driverProcess?.Kill();
                        this.driverProcess?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    exitedWait.Close();

                    this.EventOfStopSignalOn?.Close();
                    this.EventOfStopSignalOff?.Close();
                    this.EventOfDriverEnd?.Close();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.EventOfStopSignalOn?.Dispose();
                this.EventOfStopSignalOff?.Dispose();
                this.EventOfDriverEnd?.Dispose();
            }

            return true;
        }

        public void Start()
        {
            if (true == this.Enabled)
            {
                this.IsRunning = true;
            }
        }

        public void Stop()
        {
            this.IsRunning = false;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 実行中かどうか
        /// </summary>
        public bool IsRunning { get; protected set; }

        #endregion

        #region フィールド

        /// <summary>
        /// 共有メモリ
        /// </summary>
        protected MemoryMappedFile? memoryMappedFile;

        protected Process? driverProcess;

        protected IMVMappedMemoryVibrometerParameter? parameter;

        /// <summary>
        /// スレッド制御フラグ
        /// </summary>
        protected class ThreadControlFlag : SafelyDisposable
        {
            public bool IsTerminate { get; set; }

            public EventWaitHandle EventOfTerminate { get; init; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public ThreadControlFlag()
            {
                this.EventOfTerminate = new ManualResetEvent(false);
            }

            public void Terminate()
            {
                this.IsTerminate = true;
                this.EventOfTerminate.Set();
            }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.Terminate();
                    this.EventOfTerminate?.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion
        }

        /// <summary>
        /// スレッド終了要求
        /// </summary>
        protected ThreadControlFlag threadControlFlag = new();

        /// <summary>
        /// 機械停止ON/OFF
        /// </summary>
        protected bool stopDeviceFlag;

        protected EventWaitHandle? EventOfStopSignalOn;
        protected EventWaitHandle? EventOfStopSignalOff;
        protected EventWaitHandle? EventOfDriverEnd;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IMVMappedMemoryVibrometer() : base("IMVMappedMemory")
        {
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="result"></param>
        protected virtual void OnDataReceived(IMVMappedMemoryVibrometerResult result)
        {
            try
            {
                this.DataReceived?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}