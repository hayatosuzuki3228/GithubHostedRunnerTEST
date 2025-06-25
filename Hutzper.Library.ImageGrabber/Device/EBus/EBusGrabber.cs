using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageProcessing;
using PvDotNet;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.EBus
{
    /// <summary>
    /// eBus
    /// </summary>
    [Serializable]
    public abstract class EBusGrabber : GrabberBase, IEBusGrabber
    {
        #region IGrabber

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.Handles?.Enabled ?? false;

        /// <summary>
        /// トリガーモード
        /// </summary>
        public override TriggerMode TriggerMode
        {
            get
            {
                var value = TriggerMode.InternalTrigger;

                try
                {
                    var valueStr = this.Handles.Device?.Parameters.GetEnumValueAsString("TriggerMode") ?? string.Empty;

                    if ("On" == valueStr)
                    {
                        valueStr = this.Handles.Device?.Parameters.GetEnumValueAsString("TriggerSource") ?? string.Empty;

                        if ("Software" == valueStr)
                        {
                            value = TriggerMode.SoftTrigger;
                        }
                        else
                        {
                            value = TriggerMode.ExternalTrigger;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    if (TriggerMode.InternalTrigger == value)
                    {
                        this.Handles.Device?.Parameters.SetEnumValue("TriggerMode", "Off");
                    }
                    else
                    {
                        this.Handles.Device?.Parameters.SetEnumValue("TriggerSelector", "FrameStart");

                        if (TriggerMode.SoftTrigger == value)
                        {
                            this.Handles.Device?.Parameters.SetEnumValue("TriggerSource", "Software");
                        }
                        else
                        {
                            this.Handles.Device?.Parameters.SetEnumValue("TriggerSource", "Line0");
                        }

                        this.Handles.Device?.Parameters.SetEnumValue("TriggerMode", "On");
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public override double ExposureTimeMicroseconds
        {
            get
            {
                var value = 0d;

                try
                {
                    value = this.Handles.Device?.Parameters.GetFloatValue("ExposureTime") ?? value;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    var min = 0d;
                    var max = 0d;
                    this.Handles.Device?.Parameters.GetFloatRange("ExposureTime", ref min, ref max);

                    if (0 < max)
                    {
                        var clampedValue = System.Math.Clamp(value, min, max);
                        this.Handles.Device?.Parameters.SetFloatValue("ExposureTime", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public override double AnalogGain
        {
            get
            {
                var value = 0d;

                try
                {
                    this.Handles.Device?.Parameters.SetEnumValue("GainSelector", "AnalogAll");
                    value = this.Handles.Device?.Parameters.GetFloatValue("Gain") ?? value;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    this.Handles.Device?.Parameters.SetEnumValue("GainSelector", "AnalogAll");

                    var min = 0d;
                    var max = 0d;
                    this.Handles.Device?.Parameters.GetFloatRange("Gain", ref min, ref max);

                    if (0 < max)
                    {
                        var clampedValue = System.Math.Clamp(value, min, max);
                        this.Handles.Device?.Parameters.SetFloatValue("Gain", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public override double DigitalGain
        {
            get
            {
                var value = 0d;

                try
                {
                    this.Handles.Device?.Parameters.SetEnumValue("BlackLevelSelector", "DigitalAll");
                    value = this.Handles.Device?.Parameters.GetFloatValue("BlackLevel") ?? value;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    this.Handles.Device?.Parameters.SetEnumValue("BlackLevelSelector", "DigitalAll");

                    var min = 0d;
                    var max = 0d;
                    this.Handles.Device?.Parameters.GetFloatRange("BlackLevel", ref min, ref max);

                    if (0 < max)
                    {
                        var clampedValue = System.Math.Clamp(value, min, max);
                        this.Handles.Device?.Parameters.SetFloatValue("BlackLevel", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        public override bool IsRgbColor
        {
            get
            {
                var value = false;

                try
                {
                    var strValue = this.Handles.Device?.Parameters.GetEnumValueAsString("PixelFormat") ?? string.Empty;

                    if (false == string.IsNullOrEmpty(strValue))
                    {
                        if (false == strValue.ToLower().Contains("mono"))
                        {
                            value = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }
        }

        /// <summary>
        /// 画像幅
        /// </summary>
        public override int Width
        {
            get
            {
                var value = 0;

                try
                {
                    value = (int)(this.Handles.Device?.Parameters.GetIntegerValue("Width") ?? value);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    var min = 0L;
                    var max = 0L;
                    this.Handles.Device?.Parameters.GetIntegerRange("Width", ref min, ref max);

                    if (0 >= value)
                    {
                        if (0 < max)
                        {
                            this.Handles.Device?.Parameters.SetIntegerValue("Width", max);
                        }
                    }
                    else if (0 < max)
                    {
                        var unitValue = (value / 16) * 16;
                        if (unitValue != value)
                        {
                            Serilog.Log.Warning($"Width is replaced {value} -> {unitValue}");
                        }

                        var clampedValue = System.Math.Clamp(unitValue, min, max);
                        this.Handles.Device?.Parameters.SetIntegerValue("Width", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public override int Height
        {
            get
            {
                var value = 0;

                try
                {
                    value = (int)(this.Handles.Device?.Parameters.GetIntegerValue("Height") ?? value);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    var min = 0L;
                    var max = 0L;
                    this.Handles.Device?.Parameters.GetIntegerRange("Height", ref min, ref max);

                    if (0 >= value)
                    {
                        if (0 < max)
                        {
                            this.Handles.Device?.Parameters.SetIntegerValue("Height", max);
                        }
                    }
                    else if (0 < max)
                    {
                        var unitValue = (value / 4) * 4;
                        if (unitValue != value)
                        {
                            Serilog.Log.Warning($"Height is replaced {value} -> {unitValue}");
                        }

                        var clampedValue = System.Math.Clamp(unitValue, min, max);
                        this.Handles.Device?.Parameters.SetIntegerValue("Height", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public override int OffsetX
        {
            get
            {
                var value = 0;

                try
                {
                    value = (int)(this.Handles.Device?.Parameters.GetIntegerValue("OffsetX") ?? value);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    var min = 0L;
                    var max = 0L;
                    this.Handles.Device?.Parameters.GetIntegerRange("OffsetX", ref min, ref max);

                    if (0 >= value)
                    {
                        if (0 < max)
                        {
                            this.Handles.Device?.Parameters.SetIntegerValue("OffsetX", max);
                        }
                    }
                    else if (0 < max)
                    {
                        var unitValue = (value / 4) * 4;
                        if (unitValue != value)
                        {
                            Serilog.Log.Warning($"OffsetX is replaced {value} -> {unitValue}");
                        }

                        var clampedValue = System.Math.Clamp(unitValue, min, max);
                        this.Handles.Device?.Parameters.SetIntegerValue("OffsetX", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get
            {
                var value = 0;

                try
                {
                    value = (int)(this.Handles.Device?.Parameters.GetIntegerValue("OffsetY") ?? value);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    var min = 0L;
                    var max = 0L;
                    this.Handles.Device?.Parameters.GetIntegerRange("OffsetY", ref min, ref max);

                    if (0 >= value)
                    {
                        if (0 < max)
                        {
                            this.Handles.Device?.Parameters.SetIntegerValue("OffsetY", max);
                        }
                    }
                    else if (0 < max)
                    {
                        var unitValue = (value / 4) * 4;
                        if (unitValue != value)
                        {
                            Serilog.Log.Warning($"OffsetY is replaced {value} -> {unitValue}");
                        }

                        var clampedValue = System.Math.Clamp(unitValue, min, max);
                        this.Handles.Device?.Parameters.SetIntegerValue("OffsetY", clampedValue);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (
                    null == this.Handles.Device
                && this.Parameter is IGrabberParameter gp
                )
                {
                    if (false == int.TryParse(gp.DeviceID, out int intID))
                    {
                        intID = -1;
                    }

                    using var system = new PvSystem();
                    system.Find();

                    var selectedCameraInfo = (PvDeviceInfo?)null;
                    foreach (var pvIf in system)
                    {
                        foreach (var cameraInfo in pvIf)
                        {
                            if (0 > intID)
                            {
                                selectedCameraInfo = cameraInfo;
                                break;
                            }
                            else if (true == gp.DeviceID.Equals(cameraInfo.UniqueID))
                            {
                                selectedCameraInfo = cameraInfo;
                                break;
                            }
                        }

                        if (null != selectedCameraInfo)
                        {
                            break;
                        }
                    }

                    this.Handles.Device = PvDevice.CreateAndConnect(selectedCameraInfo);

                    if (null != this.Handles.Device)
                    {
                        this.Handles.Stream = PvStream.CreateAndOpen(selectedCameraInfo);

                        this.LoadUserSet();

                        if (this.Handles.Device is PvDeviceGEV dgev)
                        {
                            // Negotiate packet size
                            dgev.NegotiatePacketSize();

                            // Set stream destination.
                            if (this.Handles.Stream is PvStreamGEV sgev)
                            {
                                dgev.SetStreamDestination(sgev.LocalIPAddress, sgev.LocalPort);
                            }
                        }

                        // Read payload size, set buffer size the pipeline will use to allocate buffers
                        this.Handles.Pipeline = new PvPipeline(this.Handles.Stream);
                        this.Handles.Pipeline.BufferSize = this.Handles.Device.PayloadSize;

                        // Set buffer count. Use more buffers (at expense of using more memory) to eliminate missing block IDs.
                        this.Handles.Pipeline.BufferCount = 10;

                        #region パラメータ反映
                        {
                            this.Handles.Device.Parameters.SetEnumValue("ExposureMode", "Timed");
                            this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                            this.AnalogGain = gp.AnalogGain;
                            this.DigitalGain = gp.DigitalGain;

                            this.TriggerMode = gp.TriggerMode;

                            this.OffsetX = 0;
                            this.OffsetY = 0;

                            this.Width = gp.Width;
                            this.Height = gp.Height;

                            this.OffsetX = gp.OffsetX;
                            this.OffsetY = gp.OffsetY;

                            this.Handles.Device.OnLinkDisconnected += this.Device_OnLinkDisconnected1;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                this.Close();
                Serilog.Log.Warning(ex, ex.Message);
                this.Handles.Device = null;
            }

            return this.Enabled;
        }

        private void Device_OnLinkDisconnected1(PvDevice aDevice)
        {
            try
            {
                Serilog.Log.Error($"device lost (id = {this.DeviceID})");
                this.Close();
                this.OnDisabled();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            try
            {
                if (this.Handles.Device is not null)
                {
                    this.Handles.Device.OnLinkDisconnected -= this.Device_OnLinkDisconnected1;

                    this.StopGrabbing();

                    this.DisposeSafely(this.Handles.Pipeline);
                    this.DisposeSafely(this.Handles.Stream);
                    this.DisposeSafely(this.Handles.Device);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Handles.Device = null;
            }

            return !this.Enabled;
        }

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                    }

                    this.ControlInfo = new AcquisitionControlInfo(1);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
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
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public override bool GrabContinuously(int number = -1)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        this.ControlInfo = null;
                    }

                    this.ControlInfo = new AcquisitionControlInfo(number);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
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
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabbing()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        var temp = this.ControlInfo;
                        this.ControlInfo = null;

                        temp.WaitAcquisitionEnd();
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
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public override bool DoSoftTrigger()
        {
            var isSuccess = false;

            try
            {
                var commandNode = this.Handles.Device?.Parameters?.GetCommand("TriggerSoftware");

                if (null != commandNode)
                {
                    commandNode.Execute();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// AW実行
        /// </summary>
        /// <returns></returns>
        public override bool DoAutoWhiteBalancing()
        {
            var isSuccess = false;

            try
            {
                this.Handles.Device?.Parameters.SetEnumValue("BalanceWhiteAuto", "Once");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSetDefault()
        {
            var isSuccess = this.Enabled && (this.ControlInfo?.IsContinuable ?? true);

            try
            {
                if (true == isSuccess)
                {
                    this.Handles.Device?.Parameters.SetEnumValue("UserSetSelector", "Default");

                    var commandNode = this.Handles.Device?.Parameters.GetCommand("UserSetLoad");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
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
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSet(int index = 0)
        {
            var isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 3 > index)
                {
                    this.Handles.Device?.Parameters.SetEnumValue("UserSetSelector", $"UserSet{index + 1}");

                    var commandNode = this.Handles.Device?.Parameters.GetCommand("UserSetLoad");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
                    }
                }
                else
                {
                    isSuccess = false;
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
        /// ユーザーセットトセーブ
        /// </summary>
        /// <returns></returns>
        public override bool SaveUserSet(int index = 0)
        {
            var isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 3 > index)
                {
                    this.Handles.Device?.Parameters.SetEnumValue("UserSetSelector", $"UserSet{index + 1}");

                    var commandNode = this.Handles.Device?.Parameters.GetCommand("UserSetSave");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
                    }
                }
                else
                {
                    isSuccess = false;
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
        /// double型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override void GetValues(out Dictionary<ParametersKey, IClampedValue<double>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<double>>();

            try
            {
                base.GetValues(out values, keys);

                if (this.Handles.Device is not null)
                {
                    foreach (var key in keys)
                    {
                        if (false == values.ContainsKey(key))
                        {
                            var value = (ClampedValue<double>?)null;

                            switch (key)
                            {
                                case ParametersKey.ExposureTime:
                                    {
                                        var min = 0d;
                                        var max = 0d;
                                        var now = this.Handles.Device.Parameters.GetFloatValue("ExposureTime");
                                        this.Handles.Device.Parameters.GetFloatRange("ExposureTime", ref min, ref max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.AnalogGain:
                                    {
                                        var min = 0d;
                                        var max = 0d;
                                        this.Handles.Device.Parameters.SetEnumValue("GainSelector", "AnalogAll");
                                        this.Handles.Device.Parameters.GetFloatRange("Gain", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetFloatValue("Gain");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.DigitalGain:
                                    {
                                        var min = 0d;
                                        var max = 0d;
                                        this.Handles.Device.Parameters.SetEnumValue("BlackLevelSelector", "DigitalAll");
                                        this.Handles.Device.Parameters.GetFloatRange("Gain", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetFloatValue("Gain");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.FrameRate:
                                    {
                                        var min = 0d;
                                        var max = 0d;
                                        var now = this.Handles.Device.Parameters.GetFloatValue("AcquisitionFrameRate");
                                        this.Handles.Device.Parameters.GetFloatRange("AcquisitionFrameRate", ref min, ref max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.OffsetX:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("OffsetX", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("OffsetX");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("OffsetY", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("OffsetY");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("Width", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("Width");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("Height", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("Height");

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                            }

                            if (value is not null)
                            {
                                values.Add(key, value);
                            }
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
        /// int型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override void GetValues(out Dictionary<ParametersKey, IClampedValue<int>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<int>>();

            try
            {
                base.GetValues(out values, keys);

                if (this.Handles.Device is not null)
                {
                    foreach (var key in keys)
                    {
                        if (false == values.ContainsKey(key))
                        {
                            var value = (ClampedValue<int>?)null;

                            switch (key)
                            {
                                case ParametersKey.ExposureTime:
                                case ParametersKey.AnalogGain:
                                case ParametersKey.DigitalGain:
                                case ParametersKey.FrameRate:
                                    break;

                                case ParametersKey.OffsetX:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("OffsetX", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("OffsetX");

                                        value = new ClampedValue<int>((int)now, (int)min, (int)max);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("OffsetY", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("OffsetY");

                                        value = new ClampedValue<int>((int)now, (int)min, (int)max);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("Width", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("Width");

                                        value = new ClampedValue<int>((int)now, (int)min, (int)max);
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        var min = 0L;
                                        var max = 0L;
                                        this.Handles.Device.Parameters.GetIntegerRange("Height", ref min, ref max);
                                        var now = this.Handles.Device.Parameters.GetIntegerValue("Height");

                                        value = new ClampedValue<int>((int)now, (int)min, (int)max);
                                    }
                                    break;
                            }

                            if (value is not null)
                            {
                                values.Add(key, value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

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
                if (true == this.Enabled && this.Handles.Device != null)
                {
                    this.LoadUserSet();

                    if (this.Parameter is IGrabberParameter gp)
                    {
                        this.Handles.Device.Parameters.SetEnumValue("ExposureMode", "Timed");
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;

                        this.Handles.Device.Parameters.SetEnumValue("GainSelector", "AnalogAll");
                        this.AnalogGain = gp.AnalogGain;

                        this.Handles.Device.Parameters.SetEnumValue("BlackLevelSelector", "DigitalAll");
                        this.DigitalGain = gp.DigitalGain;

                        this.TriggerMode = gp.TriggerMode;

                        this.OffsetX = 0;
                        this.OffsetY = 0;

                        this.Width = gp.Width;
                        this.Height = gp.Height;

                        this.OffsetX = gp.OffsetX;
                        this.OffsetY = gp.OffsetY;
                    }
                }
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

                if (true == this.Enabled && this.Handles.Device != null)
                {
                    if (this.Parameter is IGrabberParameter gp)
                    {
                        this.Handles.Device.Parameters.SetEnumValue("ExposureMode", "Timed");
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;

                        this.Handles.Device.Parameters.SetEnumValue("GainSelector", "AnalogAll");
                        this.AnalogGain = gp.AnalogGain;

                        this.Handles.Device.Parameters.SetEnumValue("BlackLevelSelector", "DigitalAll");
                        this.DigitalGain = gp.DigitalGain;

                        this.TriggerMode = gp.TriggerMode;

                        this.OffsetX = 0;
                        this.OffsetY = 0;

                        this.Width = gp.Width;
                        this.Height = gp.Height;

                        this.OffsetX = gp.OffsetX;
                        this.OffsetY = gp.OffsetY;
                    }
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
                this.DisposeSafely(this.AcquisitionThread);
                this.DisposeSafely(this.Handles);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region API

        protected class EBusHandles : SafelyDisposable
        {
            public bool Enabled => this.Device?.IsConnected ?? false;

            public PvDevice? Device { get; set; }
            public PvStream? Stream { get; set; }
            public PvPipeline? Pipeline { get; set; }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.DisposeSafely(this.Device);
                    this.DisposeSafely(this.Stream);
                    this.DisposeSafely(this.Pipeline);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.Stream = null;
                    this.Pipeline = null;
                    this.Device = null;
                }
            }

            public EBusHandles()
            {


            }

            #endregion
        }

        #endregion

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected class AcquisitionControlInfo : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 画像取得必要枚数
            /// </summary>
            public int NumberOfImage { get; init; }

            /// <summary>
            /// 画像取得を続行可能かどうか
            /// </summary>
            public bool IsContinuable
            {
                get => isContinuable;
                set
                {
                    isContinuable = value;
                    if (false == isContinuable)
                    {
                        if (0 < this.WaitHandles.Length && this.WaitHandles[0] is ManualResetEvent e)
                        {
                            e.Set();
                        }
                    }
                }
            }

            public void NotifyAcquisitionEnd()
            {
                if (this.WaitHandles[1] is ManualResetEvent e)
                {
                    e.Set();
                }
            }

            public void WaitAcquisitionEnd()
            {
                if (this.WaitHandles[1] is ManualResetEvent e)
                {
                    e.WaitOne();
                }
            }

            #endregion

            #region フィールド

            public WaitHandle[] WaitHandles = new WaitHandle[3];
            protected bool isContinuable = true;

            protected List<System.Drawing.Bitmap> bufferImages = new();

            #endregion

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="numberOfImage"></param>
            public AcquisitionControlInfo(int numberOfImage)
            {
                this.NumberOfImage = numberOfImage;
                this.WaitHandles[0] = new ManualResetEvent(false);
                this.WaitHandles[1] = new ManualResetEvent(false);
                this.WaitHandles[2] = new AutoResetEvent(true);
            }

            #region SafelyDisposable

            protected override void DisposeExplicit()
            {
                try
                {
                    foreach (var e in this.WaitHandles)
                    {
                        e.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.WaitHandles = Array.Empty<WaitHandle>();
                }
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 画像取得を通委t
            /// </summary>
            /// <param name="imageData"></param>
            /// <param name="imageWidth"></param>
            /// <param name="imageHeight"></param>
            public void OnIDataStreamNewBuffer(PvBuffer buffer)
            {
                try
                {
                    if (this.WaitHandles[2] is AutoResetEvent e)
                    {
                        Bitmap bitmap;

                        if (8 == buffer.Image.BitsPerPixel)
                        {
                            bitmap = BitmapProcessor.CreateBitmap((int)buffer.Image.Width, (int)buffer.Image.Height);
                        }
                        else if (24 == buffer.Image.BitsPerPixel)
                        {
                            bitmap = new Bitmap((int)buffer.Image.Width, (int)buffer.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        }
                        else
                        {
                            bitmap = new Bitmap((int)buffer.Image.Width, (int)buffer.Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        }

                        buffer.Image.CopyToBitmap(bitmap);

                        lock (this.WaitHandles)
                        {
                            bufferImages.Add(bitmap);
                        }

                        e.Set();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            /// <summary>
            ///  画像取得を待機
            /// </summary>
            /// <returns></returns>
            public List<System.Drawing.Bitmap> GetDataStreamNewBuffer()
            {
                var images = new List<System.Drawing.Bitmap>();

                try
                {
                    WaitHandle.WaitAny(WaitHandles);

                    lock (this.WaitHandles)
                    {
                        if (0 < bufferImages.Count)
                        {
                            images.AddRange(bufferImages);
                            bufferImages.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return images;
            }

            #endregion
        }


        #region フィールド

        /// <summary>
        /// 
        /// </summary>
        protected EBusGrabber.EBusHandles Handles = new();

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        protected QueueThread<AcquisitionControlInfo> AcquisitionThread;

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected AcquisitionControlInfo? ControlInfo;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBusGrabber() : this(typeof(EBusGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBusGrabber(Common.Drawing.Point location) : this(typeof(EBusGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public EBusGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.AcquisitionThread = new()
            {
                Priority = ThreadPriority.Highest
            };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        private void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (null != this.Handles.Device && null != this.Handles.Pipeline)
                {

                    if (0 < info.NumberOfImage)
                    {
                        this.Handles.Device.Parameters.SetEnumValue("AcquisitionMode", "MultiFrame");
                        this.Handles.Device.Parameters.SetIntegerValue("AcquisitionFrameCount", info.NumberOfImage);
                    }
                    else
                    {
                        this.Handles.Device.Parameters.SetEnumValue("AcquisitionMode", "Continuous");
                    }

                    this.Handles.Pipeline.Start();
                    this.Handles.Device.StreamEnable();
                    this.Handles.Device.Parameters.ExecuteCommand("AcquisitionStart");

                    // 画像取得スレッド
                    var retrieveThread = new Thread(() =>
                    {
                        try
                        {
                            var grabbedCount = 0;
                            while (info.IsContinuable)
                            {
                                var buffer = (PvBuffer?)null;

                                var result = this.Handles.Pipeline.RetrieveNextBuffer(ref buffer);
                                if (result.IsOK)
                                {
                                    try
                                    {
                                        this.ControlInfo?.OnIDataStreamNewBuffer(buffer);
                                        grabbedCount++;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex);
                                    }
                                    finally
                                    {
                                        // We got a buffer (good or not) we must release it back.
                                        this.Handles.Pipeline.ReleaseBuffer(buffer);
                                    }
                                }

                                if (0 < info.NumberOfImage && info.NumberOfImage <= grabbedCount)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    })
                    { IsBackground = true };
                    retrieveThread.Start();

                    var imageCounter = 0UL;

                    var stopwatch = new Stopwatch();

                    while (info.IsContinuable)
                    {
                        // 画像取得を待機する
                        var bufferImages = info.GetDataStreamNewBuffer();

                        // 画像バッファからデータを列挙
                        foreach (var bitmap in bufferImages)
                        {
                            imageCounter++;

                            stopwatch.Restart();

                            // 撮影データを生成
                            var result = new ByteArrayGrabberDataFastBitmapGeneration(location);

                            try
                            {
                                // データロック
                                var lockedData = bitmap.LockBits(
                                                                  new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                                                , ImageLockMode.ReadOnly
                                                                , bitmap.PixelFormat
                                                                );

                                // byte配列化
                                var stride = lockedData.Stride;
                                var bytes = new byte[stride * lockedData.Height];
                                Marshal.Copy(lockedData.Scan0, bytes, 0, bytes.Length);

                                // データアンロック
                                bitmap.UnlockBits(lockedData);

                                // データ格納
                                result.Counter = imageCounter;
                                result.Image = bytes;
                                result.Stride = stride;
                                result.Bitmap = bitmap;
                                result.Size = new Common.Drawing.Size(bitmap.Size);
                                result.PixelFormat = bitmap.PixelFormat;
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }

                            var copyTimeMs = stopwatch.ElapsedMilliseconds;

                            // 画像取得イベント通知
                            this.OnDataGrabbed(result);

                            var callTimeMs = stopwatch.ElapsedMilliseconds - copyTimeMs;

                            stopwatch.Stop();
                            var totalTimeMs = stopwatch.ElapsedMilliseconds;
                        }
                    }

                    this.Handles.Device.Parameters.ExecuteCommand("AcquisitionStop");
                    this.Handles.Device.StreamDisable();
                    this.Handles.Pipeline.Stop();

                    retrieveThread.Join(100);

                    try
                    {
                        var buffer = (PvBuffer?)null;
                        while (this.Handles.Pipeline.RetrieveNextBuffer(ref buffer).IsOK) ;
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                info.IsContinuable = false;

                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
            finally
            {
                info.NotifyAcquisitionEnd();
                this.DisposeSafely(info);
            }
        }
    }
}