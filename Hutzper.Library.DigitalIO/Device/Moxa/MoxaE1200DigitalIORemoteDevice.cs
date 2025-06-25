using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using MOXA_CSharp_MXIO;
using System.Diagnostics;

namespace Hutzper.Library.DigitalIO.Device.Moxa
{
    /// <summary>
    /// MoxaE1200パラメータ
    /// </summary>
    [Serializable]
    public class MoxaE1200DigitalIORemoteDevice : ControllerBase, IDigitalIORemoteDevice, IDigitalIOInputDevice, IDigitalIOOutputDevice
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

                var ret = MXIO_CS.MXIO_GetDllVersion();
                Serilog.Log.Debug($"MXIO_GetDllVersion:{ret >> 12}.{ret >> 8}.{ret >> 4}.{ret & 0xF}");

                ret = MXIO_CS.MXIO_GetDllBuildDate();
                Serilog.Log.Debug($"MXIO_GetDllBuildDate:{ret >> 16}/{ret >> 8}/{ret & 0xF}");

                ret = MXIO_CS.MXEIO_Init();
                Serilog.Log.Debug($"MXEIO_Init return {ret}");
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

                if (parameter is IDigitalIORemoteDeviceParameter p)
                {
                    this.Parameter = p;
                    this.DeviceID = p.DeviceID;
                    this.Location = p.Location;
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

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                MXIO_CS.MXEIO_Exit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region IDigitalIODevice

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool Enabled => this.Handles.Connection >= 0;

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (this.Parameter is MoxaE1200DigitalIORemoteDeviceParameter mp)
                {
                    ushort timeout = 5000;
                    var hConnection = -1;

                    var ret = MXIO_CS.MXEIO_E1K_Connect(
                                                           System.Text.Encoding.UTF8.GetBytes(mp.IpAddress)
                                                         , (ushort)mp.PortNumber
                                                         , timeout
                                                         , out hConnection
                                                         , System.Text.Encoding.UTF8.GetBytes(mp.Password)
                                                         );

                    if (this.IsMoxaSuccess(ret, MxFunction.MXEIO_E1K_Connect))
                    {
                        # region Check Connection

                        //var bytCheckStatus = new byte[1];
                        //ret = MXIO_CS.MXEIO_CheckConnection(hConnection, timeout, bytCheckStatus);

                        //if (this.IsMoxaSuccess(ret, MxFunction.MXEIO_CheckConnection))
                        //{
                        //    switch (bytCheckStatus[0])
                        //    {
                        //        case MXIO_CS.CHECK_CONNECTION_OK:
                        //            Serilog.Log.Debug($"MXEIO_CheckConnection: Check connection ok => {bytCheckStatus[0]}");
                        //            break;
                        //        case MXIO_CS.CHECK_CONNECTION_FAIL:
                        //            Serilog.Log.Debug($"MXEIO_CheckConnection: Check connection fail => {bytCheckStatus[0]}");
                        //            break;
                        //        case MXIO_CS.CHECK_CONNECTION_TIME_OUT:
                        //            Serilog.Log.Debug($"MXEIO_CheckConnection: Check connection time out => {bytCheckStatus[0]}");
                        //            break;
                        //        default:
                        //            Serilog.Log.Debug($"MXEIO_CheckConnection: Check connection unknown => {bytCheckStatus[0]}");
                        //            break;
                        //    }
                        //}

                        #endregion

                        this.Handles.Connection = hConnection;

                        this.SetupModeForDI(this.Handles, this.Parameter);
                        this.SetupModeForDO(this.Handles, this.Parameter);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                if (this.Enabled)
                    this.Connected?.Invoke(this);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.Connection >= 0)
                {
                    isSuccess = this.IsMoxaSuccess(MXIO_CS.MXEIO_Disconnect(this.Handles.Connection), MxFunction.MXEIO_Disconnect);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Handles.Connection = -1;

                if (isSuccess)
                    this.Disconnected?.Invoke(this);
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIOInputDevice

        /// <summary>
        /// 入力点数
        /// </summary>
        public int NumberOfInputs => this.Parameter?.InputChannels.Length ?? 0;

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out bool[] values)
        {
            var isSuccess = this.ReadInput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfInputs];

            try
            {
                var channelIndex = 0;
                foreach (var channels in GetContinuousRanges(this.Parameter?.InputChannels ?? Array.Empty<int>()))
                {
                    var dwGetDIValue = new uint[1];

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DI_Reads(this.Handles.Connection, (byte)channels.First(), (byte)channels.Count, dwGetDIValue);
                    }

                    isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DI_Reads);

                    for (int i = 0; i < channels.Count; i++)
                    {
                        values[channelIndex++] = (int)(dwGetDIValue[0] & (1 << i));
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out bool value)
        {
            var isSuccess = this.ReadInput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (this.Parameter?.InputChannels.Skip(index).First() is int ch)
                {
                    var dwGetDIValue = new uint[1];

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DI_Reads(this.Handles.Connection, (byte)ch, (byte)1, dwGetDIValue);
                    }
                    isSuccess = this.IsMoxaSuccess(ret, MxFunction.E1K_DI_Reads);

                    value = (int)dwGetDIValue[0];
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIOOutputDevice

        /// <summary>
        /// 出力点数
        /// </summary>
        public int NumberOfOutputs => this.Parameter?.OutputChannels.Length ?? 0;

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(bool[] values)
        {
            var intValues = Array.ConvertAll(values, v => v ? 1 : 0);

            var isSuccess = this.WriteOutput(intValues);

            return isSuccess;
        }

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int[] values)
        {
            var isSuccess = this.Enabled;

            try
            {
                var uintValues = Array.ConvertAll(values, v => (uint)v);

                var channelIndex = 0;
                foreach (var channels in GetContinuousRanges(this.Parameter?.OutputChannels ?? Array.Empty<int>()))
                {
                    var dwSetDOValue = (uint)0;

                    for (int i = 0; i < channels.Count; i++)
                    {
                        dwSetDOValue |= uintValues[channelIndex++] << i;
                    }

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DO_Writes(this.Handles.Connection, (byte)channels.First(), (byte)channels.Count, dwSetDOValue);
                    }

                    isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DO_Writes);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, bool value)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (this.Parameter?.OutputChannels.Skip(index).First() is int ch)
                {
                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DO_Writes(this.Handles.Connection, (byte)ch, (byte)1, value ? (uint)1 : (uint)0);
                    }
                    isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DO_Writes);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, int value)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (this.Parameter?.OutputChannels.Skip(index).First() is int ch)
                {
                    var uintValue = (value != 0) ? (ushort)1 : (ushort)0;

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DO_Writes(this.Handles.Connection, (byte)ch, (byte)1, uintValue);
                    }
                    isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DO_Writes);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out bool[] values)
        {
            var isSuccess = this.ReadOutput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfOutputs];

            try
            {
                var channelIndex = 0;
                foreach (var channels in GetContinuousRanges(this.Parameter?.OutputChannels ?? Array.Empty<int>()))
                {
                    var dwGetDOValue = new uint[1];

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DO_Reads(this.Handles.Connection, (byte)channels.First(), (byte)channels.Count, dwGetDOValue);
                    }

                    isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DO_Reads);

                    for (int i = 0; i < channels.Count; i++)
                    {
                        values[channelIndex++] = (int)(dwGetDOValue[0] & (1 << i));
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out bool value)
        {
            var isSuccess = this.ReadOutput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (this.Parameter?.OutputChannels.Skip(index).First() is int ch)
                {
                    var dwGetDOValue = new uint[1];

                    var ret = 0;
                    lock (this.commandSync)
                    {
                        ret = MXIO_CS.E1K_DO_Reads(this.Handles.Connection, (byte)ch, (byte)1, dwGetDOValue);
                    }

                    isSuccess = this.IsMoxaSuccess(ret, MxFunction.E1K_DO_Reads);

                    value = (int)dwGetDOValue[0];
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IDigitalIORemoteDevice

        /// <summary>
        /// 接続イベント
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        /// <summary>
        /// 再接続
        /// </summary>
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public int ReconnectionAttemptsIntervalSec { get; set; }

        #endregion

        protected class MoxaHandles
        {
            public int Connection { get; set; } = -1;
        }

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        protected IDigitalIORemoteDeviceParameter? Parameter;

        protected readonly MoxaHandles Handles = new();

        protected readonly object commandSync = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public MoxaE1200DigitalIORemoteDevice() : this(typeof(MoxaE1200DigitalIORemoteDevice).Name, Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public MoxaE1200DigitalIORemoteDevice(Common.Drawing.Point location) : this(typeof(MoxaE1200DigitalIORemoteDevice).Name, location)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public MoxaE1200DigitalIORemoteDevice(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }

        /// <summary>
        /// DIモード設定
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected bool SetupModeForDI(MoxaHandles handles, IDigitalIODeviceParameter parameter)
        {
            var isSuccess = true;

            try
            {
                if (null != parameter && 0 < parameter.InputChannels.Length)
                {
                    var channelIndex = 0;
                    var modeDI = 0;
                    foreach (var channels in GetContinuousRanges(parameter.InputChannels))
                    {
                        // mode
                        var modeValues = Enumerable.Repeat(modeDI, channels.Count).ToList().ConvertAll(m => (ushort)m);

                        var ret = MXIO_CS.E1K_DI_SetModes(handles.Connection, (byte)channels.First(), (byte)modeValues.Count, modeValues.ToArray());
                        isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DI_SetModes);

                        // filter
                        var filterValues = Enumerable.Range(100, channels.Count).ToList().ConvertAll(f => (ushort)f);
                        foreach (var i in Enumerable.Range(0, channels.Count))
                        {
                            if (channelIndex < parameter.FilteringTimeMsIntervals.Length)
                            {
                                filterValues[i] = (ushort)parameter.FilteringTimeMsIntervals[channelIndex];

                                channelIndex++;
                            }
                        }

                        ret = MXIO_CS.E1K_DI_SetFilters(handles.Connection, (byte)channels.First(), (byte)filterValues.Count, filterValues.ToArray());
                        isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DI_SetFilters);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// DOモード設定
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected bool SetupModeForDO(MoxaHandles handles, IDigitalIODeviceParameter parameter)
        {
            var isSuccess = true;

            try
            {
                if (null != parameter && 0 < parameter.OutputChannels.Length)
                {
                    // mode
                    var modeDO = 0;
                    foreach (var channels in GetContinuousRanges(parameter.OutputChannels))
                    {
                        var modeValues = Enumerable.Repeat(modeDO, channels.Count).ToList().ConvertAll(m => (ushort)m);

                        var ret = MXIO_CS.E1K_DO_SetModes(handles.Connection, 0, (byte)modeValues.Count, modeValues.ToArray());

                        isSuccess &= this.IsMoxaSuccess(ret, MxFunction.E1K_DO_SetModes);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        static List<List<int>> GetContinuousRanges(int[] input)
        {
            List<List<int>> result = new List<List<int>>();
            if (input == null || input.Length == 0) return result;
            List<int> currentSequence = new List<int>();
            currentSequence.Add(input[0]);
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1] + 1)
                {
                    currentSequence.Add(input[i]);
                }
                else
                {
                    result.Add(currentSequence);
                    currentSequence = new List<int>();
                    currentSequence.Add(input[i]);
                }
            }
            result.Add(currentSequence);
            return result;
        }

        #region Moxa

        [Serializable]
        protected enum MxFunction
        {
            MXEIO_E1K_Connect
        , MXEIO_Disconnect
        , MXEIO_CheckConnection
        , E1K_DI_SetModes
        , E1K_DI_SetFilters
        , E1K_DO_SetModes
        , E1K_DI_Reads
        , E1K_DO_Reads
        , E1K_DO_Writes
        }

        protected bool IsMoxaSuccess(int mxResult, MxFunction mxfunction)
        {
            var mxErrorText = "MXIO_OK";

            if (mxResult != MXIO_CS.MXIO_OK)
            {
                switch (mxResult)
                {
                    case MXIO_CS.ILLEGAL_FUNCTION:
                        mxErrorText = "ILLEGAL_FUNCTION";
                        break;
                    case MXIO_CS.ILLEGAL_DATA_ADDRESS:
                        mxErrorText = "ILLEGAL_DATA_ADDRESS";
                        break;
                    case MXIO_CS.ILLEGAL_DATA_VALUE:
                        mxErrorText = "ILLEGAL_DATA_VALUE";
                        break;
                    case MXIO_CS.SLAVE_DEVICE_FAILURE:
                        mxErrorText = "SLAVE_DEVICE_FAILURE";
                        break;
                    case MXIO_CS.SLAVE_DEVICE_BUSY:
                        mxErrorText = "SLAVE_DEVICE_BUSY";
                        break;
                    case MXIO_CS.EIO_TIME_OUT:
                        mxErrorText = "EIO_TIME_OUT";
                        break;
                    case MXIO_CS.EIO_INIT_SOCKETS_FAIL:
                        mxErrorText = "EIO_INIT_SOCKETS_FAIL";
                        break;
                    case MXIO_CS.EIO_CREATING_SOCKET_ERROR:
                        mxErrorText = "EIO_CREATING_SOCKET_ERROR";
                        break;
                    case MXIO_CS.EIO_RESPONSE_BAD:
                        mxErrorText = "EIO_RESPONSE_BAD";
                        break;
                    case MXIO_CS.EIO_SOCKET_DISCONNECT:
                        mxErrorText = "EIO_SOCKET_DISCONNECT";
                        break;
                    case MXIO_CS.PROTOCOL_TYPE_ERROR:
                        mxErrorText = "PROTOCOL_TYPE_ERROR";
                        break;
                    case MXIO_CS.SIO_OPEN_FAIL:
                        mxErrorText = "SIO_OPEN_FAIL";
                        break;
                    case MXIO_CS.SIO_TIME_OUT:
                        mxErrorText = "SIO_TIME_OUT";
                        break;
                    case MXIO_CS.SIO_CLOSE_FAIL:
                        mxErrorText = "SIO_CLOSE_FAIL";
                        break;
                    case MXIO_CS.SIO_PURGE_COMM_FAIL:
                        mxErrorText = "SIO_PURGE_COMM_FAIL";
                        break;
                    case MXIO_CS.SIO_FLUSH_FILE_BUFFERS_FAIL:
                        mxErrorText = "SIO_FLUSH_FILE_BUFFERS_FAIL";
                        break;
                    case MXIO_CS.SIO_GET_COMM_STATE_FAIL:
                        mxErrorText = "SIO_GET_COMM_STATE_FAIL";
                        break;
                    case MXIO_CS.SIO_SET_COMM_STATE_FAIL:
                        mxErrorText = "SIO_SET_COMM_STATE_FAIL";
                        break;
                    case MXIO_CS.SIO_SETUP_COMM_FAIL:
                        mxErrorText = "SIO_SETUP_COMM_FAIL";
                        break;
                    case MXIO_CS.SIO_SET_COMM_TIME_OUT_FAIL:
                        mxErrorText = "SIO_SET_COMM_TIME_OUT_FAIL";
                        break;
                    case MXIO_CS.SIO_CLEAR_COMM_FAIL:
                        mxErrorText = "SIO_CLEAR_COMM_FAIL";
                        break;
                    case MXIO_CS.SIO_RESPONSE_BAD:
                        mxErrorText = "SIO_RESPONSE_BAD";
                        break;
                    case MXIO_CS.SIO_TRANSMISSION_MODE_ERROR:
                        mxErrorText = "SIO_TRANSMISSION_MODE_ERROR";
                        break;
                    case MXIO_CS.PRODUCT_NOT_SUPPORT:
                        mxErrorText = "PRODUCT_NOT_SUPPORT";
                        break;
                    case MXIO_CS.HANDLE_ERROR:
                        mxErrorText = "HANDLE_ERROR";
                        break;
                    case MXIO_CS.SLOT_OUT_OF_RANGE:
                        mxErrorText = "SLOT_OUT_OF_RANGE";
                        break;
                    case MXIO_CS.CHANNEL_OUT_OF_RANGE:
                        mxErrorText = "CHANNEL_OUT_OF_RANGE";
                        break;
                    case MXIO_CS.COIL_TYPE_ERROR:
                        mxErrorText = "COIL_TYPE_ERROR";
                        break;
                    case MXIO_CS.REGISTER_TYPE_ERROR:
                        mxErrorText = "REGISTER_TYPE_ERROR";
                        break;
                    case MXIO_CS.FUNCTION_NOT_SUPPORT:
                        mxErrorText = "FUNCTION_NOT_SUPPORT";
                        break;
                    case MXIO_CS.OUTPUT_VALUE_OUT_OF_RANGE:
                        mxErrorText = "OUTPUT_VALUE_OUT_OF_RANGE";
                        break;
                    case MXIO_CS.INPUT_VALUE_OUT_OF_RANGE:
                        mxErrorText = "INPUT_VALUE_OUT_OF_RANGE";
                        break;
                }

                if (mxResult == MXIO_CS.EIO_TIME_OUT || mxResult == MXIO_CS.HANDLE_ERROR)
                {
                    this.Disabled?.Invoke(this);
                    this.Close();
                }

                Serilog.Log.Error($"Function {mxfunction} execution Fail. Error Message : {mxErrorText})");

                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}