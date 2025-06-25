using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageProcessing;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using TIS.Imaging;

namespace Hutzper.Library.ImageGrabber.Device.Tis
{
    /// <summary>
    /// TIS ★DFK 22BUC03 のみ対応★
    /// </summary>
    [Serializable]
    public abstract class TisGrabber : GrabberBase, ITisGrabber
    {
        #region IGrabber


        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => !string.IsNullOrEmpty(this.IcImagingControl.Device);

        /// <summary>
        /// トリガーモード
        /// </summary>
        /// <remarks>DFK 22BUC03</remarks>
        public override TriggerMode TriggerMode
        {
            get
            {
                var value = TriggerMode.InternalTrigger;

                try
                {
                    if (true == this.Enabled)
                    {
                        if (true == this.IcImagingControl.DeviceTrigger)
                        {
                            if (this.Parameter is IGrabberParameter gp)
                            {
                                if (TriggerMode.ExternalTrigger == gp.TriggerMode)
                                {
                                    value = TriggerMode.ExternalTrigger;
                                }
                                else
                                {
                                    value = TriggerMode.SoftTrigger;
                                }
                            }
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
                    if (this.Enabled)
                    {
                        this.IcImagingControl.DeviceTrigger = (TriggerMode.InternalTrigger != value);
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDAbsoluteValueProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value) is VCDAbsoluteValueProperty item)
                        {
                            var sec2micro = Math.Pow(10, 6);

                            value = item.Value * sec2micro;
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDAbsoluteValueProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value) is VCDAbsoluteValueProperty item)
                        {
                            var sec2micro = Math.Pow(10, 6);

                            item.Value = Math.Clamp(value / sec2micro, item.RangeMin, item.RangeMax);
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
        /// アナログゲイン
        /// </summary>
        public override double AnalogGain
        {
            get
            {
                var value = 0d;

                try
                {
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value) is VCDRangeProperty item)
                        {
                            value = item.Value;
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value) is VCDRangeProperty item)
                        {
                            item.Value = Math.Clamp((int)value, item.RangeMin, item.RangeMax);
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
        /// ゲインR
        /// </summary>
        public double GainR
        {
            get
            {
                var value = 0d;

                try
                {
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceRed) is VCDRangeProperty item)
                        {
                            value = item.Value;
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceRed) is VCDRangeProperty item)
                        {
                            item.Value = Math.Clamp((int)value, item.RangeMin, item.RangeMax);
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
        /// ゲインG
        /// </summary>
        public double GainG
        {
            get
            {
                var value = 0d;

                try
                {
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceGreen) is VCDRangeProperty item)
                        {
                            value = item.Value;
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceGreen) is VCDRangeProperty item)
                        {
                            item.Value = Math.Clamp((int)value, item.RangeMin, item.RangeMax);
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
        /// ゲインB
        /// </summary>
        public double GainB
        {
            get
            {
                var value = 0d;

                try
                {
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceBlue) is VCDRangeProperty item)
                        {
                            value = item.Value;
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
                    if (this.Enabled)
                    {
                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceBlue) is VCDRangeProperty item)
                        {
                            item.Value = Math.Clamp((int)value, item.RangeMin, item.RangeMax);
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
        /// デジタルゲイン
        /// </summary>
        /// <remarks>DFK 22BUC03</remarks>
        public override double DigitalGain
        {
            get => 0;
            set => Serilog.Log.Warning($"{this.Nickname}, DigitalGain, unsupported property");
        }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        /// <remarks>DFK 22BUC03</remarks>
        public override bool IsRgbColor => (this.DefaultFrameType?.PixelFormat ?? PixelFormat.Format24bppRgb) == PixelFormat.Format24bppRgb;

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
                    if (this.Parameter is IGrabberParameter gp)
                    {
                        if (this.DefaultFrameType is not null)
                        {
                            value = this.DefaultFrameType.Width;
                        }
                        else
                        {
                            value = gp.Width;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set => Serilog.Log.Warning($"{this.Nickname}, Width, unsupported property");
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
                    if (this.Parameter is IGrabberParameter gp)
                    {
                        if (this.DefaultFrameType is not null)
                        {
                            value = this.DefaultFrameType.Height;
                        }
                        else
                        {
                            value = gp.Height;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set => Serilog.Log.Warning($"{this.Nickname}, Height, unsupported property");

        }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public override int OffsetX
        {
            get => 0;
            set => Serilog.Log.Warning($"{this.Nickname}, OffsetX, unsupported property");
        }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get => 0;
            set => Serilog.Log.Warning($"{this.Nickname}, OffsetY, unsupported property");
        }

        private ManualResetEventSlim RunningStopLiveEvent = new(true);
        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (false == this.Enabled && this.Parameter is TisAreaSensorGrabberParameter gp)
                {
                    if (false == int.TryParse(gp.DeviceID, out int intID))
                    {
                        intID = -1;
                    }

                    var selectedCameraInfo = (TIS.Imaging.Device?)null;
                    foreach (var cameraInfo in this.IcImagingControl.Devices)
                    {
                        if (0 > intID)
                        {
                            selectedCameraInfo = cameraInfo;
                            break;
                        }

                        if (cameraInfo.GetSerialNumber(out string serialNo))
                        {
                            if (true == gp.DeviceID.Equals(serialNo))
                            {
                                selectedCameraInfo = cameraInfo;
                                break;
                            }
                        }
                        else
                        {
                            Serilog.Log.Warning($"{cameraInfo.Name}:unknown");
                        }
                    }
                    this.IcImagingControl.Device = string.Empty;

                    if (selectedCameraInfo is not null)
                    {
                        this.IcImagingControl.Device = selectedCameraInfo;
                        this.DefaultFrameType = null;

                        #region 1フレーム取得して画像情報を得る
                        try
                        {
                            this.AcquisitionThread.Dequeue -= this.AcquisitionThread_Dequeue;
                            this.AcquisitionThread.Dequeue += this.AcquisitionThread_DequeueForInit;

                            this.WaitEventForInit?.Close();
                            this.WaitEventForInit = new AutoResetEvent(false);
                            this.AcquisitionThread.Enqueue(new AcquisitionControlInfo(1));
                            {
                                this.WaitEventForInit?.WaitOne(3000);

                                if (this.DefaultFrameType is null)
                                {
                                    throw new Exception("Failed to get frame type");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                        finally
                        {
                            this.IcImagingControl.Sink = this.FrameQueueSink;

                            this.AcquisitionThread.Dequeue -= this.AcquisitionThread_DequeueForInit;
                            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
                        }
                        #endregion

                        this.LoadUserSet();

                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoWB)
                        {
                            autoWB.Switch = false;
                        }

                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoExp)
                        {
                            autoExp.Switch = false;
                        }

                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoGain)
                        {
                            autoGain.Switch = false;
                        }

                        #region パラメータ反映
                        {
                            this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                            this.AnalogGain = gp.AnalogGain;
                            this.TriggerMode = gp.TriggerMode;
                            this.GainR = gp.GainR;
                            this.GainG = gp.GainG;
                            this.GainB = gp.GainB;
                            //this.DigitalGain = gp.DigitalGain;
                            //this.OffsetX = 0;
                            //this.OffsetY = 0;
                            //this.Width = gp.Width;
                            //this.Height = gp.Height;
                            //this.OffsetX = gp.OffsetX;
                            //this.OffsetY = gp.OffsetY;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                this.Close();
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            try
            {
                if (null != this.ControlInfo)
                {
                    this.RunningStopLiveEvent.Reset();
                    this.ControlInfo.IsContinueable = false;
                    this.ControlInfo = null;
                    if (!this.RunningStopLiveEvent.Wait(100))
                    {
                        Serilog.Log.Warning($"TisGrabber RunningStopLiveEvent.Wait(100)");
                    }
                }
                this.IcImagingControl.Sink = null;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
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
                    if (this.ControlInfo is AcquisitionControlInfo info)
                    {
                        info.IsContinueable = false;
                        info.Dispose();
                    }

                    this.ControlInfo = new AcquisitionControlInfo(1);
                    this.AcquisitionThread.Enqueue(ControlInfo);
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
                        this.ControlInfo.IsContinueable = false;
                        this.ControlInfo = null;
                    }

                    this.ControlInfo = new AcquisitionControlInfo(number);
                    this.AcquisitionThread.Enqueue(ControlInfo);
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
                        this.ControlInfo.IsContinueable = false;
                        this.ControlInfo = null;
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
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (this.IcImagingControl.VCDPropertyItems.Find<VCDButtonProperty>(VCDGUIDs.VCDID_TriggerMode, VCDGUIDs.VCDElement_SoftwareTrigger) is VCDButtonProperty softwareTrigger)
                    {
                        softwareTrigger.Push();
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

        protected object lockAW = new();

        /// <summary>
        /// AW実行
        /// </summary>
        /// <returns></returns>
        public override bool DoAutoWhiteBalancing()
        {
            if (false == this.Enabled)
            {
                return false;
            }

            if (false == Monitor.TryEnter(this.lockAW))
            {
                return false;
            }

            var isSuccess = false;

            try
            {
                if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoWB)
                {
                    autoWB.Switch = true;

                    try
                    {
                        var fps = this.IcImagingControl.DeviceFrameRate;
                        var waitMs = (int)(10 * (1000 / fps));  // 10フレーム分待機
                        waitMs = Math.Max(waitMs, 1000);    // 最低1秒は待機
                        Thread.Sleep(waitMs);
                        autoWB.Switch = false;
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                Monitor.Exit(this.lockAW);
            }

            return isSuccess;
        }

        public override bool GetBalanceRatio(out double gainRedValue, out double gainGreenValue, out double gainBlueValue)
        {
            var isSuccess = false;
            gainRedValue = this.GainR;
            gainGreenValue = this.GainG;
            gainBlueValue = this.GainB;
            return isSuccess;
        }
        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSetDefault() => true;

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSet(int index = 0) => true;

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool SaveUserSet(int index = 0) => true;

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

                if (true == this.Enabled)
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
                                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDAbsoluteValueProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value) is VCDAbsoluteValueProperty item)
                                        {
                                            var sec2micro = Math.Pow(10, 6);

                                            value = new ClampedValue<double>(item.Value * sec2micro, item.RangeMin * sec2micro, item.RangeMax * sec2micro);
                                        }
                                    }
                                    break;

                                case ParametersKey.AnalogGain:
                                    {
                                        if (this.IcImagingControl.VCDPropertyItems.Find<VCDRangeProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value) is VCDRangeProperty item)
                                        {
                                            value = new ClampedValue<double>(item.Value, item.RangeMin, item.RangeMax);
                                        }
                                    }
                                    break;

                                case ParametersKey.DigitalGain:
                                    {
                                        value = new ClampedValue<double>(0, 0, 0);
                                    }
                                    break;

                                case ParametersKey.FrameRate:
                                    {
                                        var range = this.IcImagingControl.DeviceFrameRates;

                                        value = new ClampedValue<double>(this.IcImagingControl.DeviceFrameRate, range.Min(), range.Max());
                                    }
                                    break;

                                case ParametersKey.OffsetX:
                                    {
                                        value = new ClampedValue<double>(0, 0, 0);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        value = new ClampedValue<double>(0, 0, 0);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        if (this.DefaultFrameType is not null)
                                        {
                                            value = new ClampedValue<double>(0, this.DefaultFrameType.Width, this.DefaultFrameType.Width);
                                        }
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        if (this.DefaultFrameType is not null)
                                        {
                                            value = new ClampedValue<double>(0, this.DefaultFrameType.Height, this.DefaultFrameType.Height);
                                        }
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

                if (true == this.Enabled)
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
                                        value = new ClampedValue<int>(0, 0, 0);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        value = new ClampedValue<int>(0, 0, 0);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        if (this.DefaultFrameType is not null)
                                        {
                                            value = new ClampedValue<int>(0, this.DefaultFrameType.Width, this.DefaultFrameType.Width);
                                        }
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        if (this.DefaultFrameType is not null)
                                        {
                                            value = new ClampedValue<int>(0, this.DefaultFrameType.Height, this.DefaultFrameType.Height);
                                        }
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
        /// 露光時間をセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetExposureTimeValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty ExposureTime)
                {
                    ExposureTime.Switch = true;

                    Task.Run(async () =>
                    {
                        try
                        {
                            var fps = this.IcImagingControl.DeviceFrameRate;
                            var waitMs = (int)(10 * (1000 / fps));  // 10フレーム分待機
                            waitMs = Math.Max(waitMs, 1000);    // 最低1秒は待機

                            await Task.Delay(waitMs);
                            ExposureTime.Switch = false;
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    });

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
        /// アナログゲインをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetAnalogGainValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty analogGain)
                {
                    analogGain.Switch = true;

                    Task.Run(async () =>
                    {
                        try
                        {
                            var fps = this.IcImagingControl.DeviceFrameRate;
                            var waitMs = (int)(10 * (1000 / fps));  // 10フレーム分待機
                            waitMs = Math.Max(waitMs, 1000);    // 最低1秒は待機

                            await Task.Delay(waitMs);
                            analogGain.Switch = false;
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    });

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
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
                if (true == this.Enabled)
                {
                    this.LoadUserSet();

                    this.IcImagingControl.LiveDisplay = false;

                    if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoWB)
                    {
                        autoWB.Switch = false;
                    }

                    if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoExp)
                    {
                        autoExp.Switch = false;
                    }

                    if (this.IcImagingControl.VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Auto) is TIS.Imaging.VCDSwitchProperty autoGain)
                    {
                        autoGain.Switch = false;
                    }

                    if (this.Parameter is IGrabberParameter gp)
                    {
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                        this.AnalogGain = gp.AnalogGain;
                        this.TriggerMode = gp.TriggerMode;
                        //this.DigitalGain = gp.DigitalGain;
                        //this.OffsetX = 0;
                        //this.OffsetY = 0;
                        //this.Width = gp.Width;
                        //this.Height = gp.Height;
                        //this.OffsetX = gp.OffsetX;
                        //this.OffsetY = gp.OffsetY;
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

                if (true == this.Enabled && this.Parameter is IGrabberParameter gp)
                {
                    this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                    this.AnalogGain = gp.AnalogGain;
                    this.TriggerMode = gp.TriggerMode;
                    //this.DigitalGain = gp.DigitalGain;
                    //this.OffsetX = 0;
                    //this.OffsetY = 0;
                    //this.Width = gp.Width;
                    //this.Height = gp.Height;
                    //this.OffsetX = gp.OffsetX;
                    //this.OffsetY = gp.OffsetY;
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
                this.WaitEventForInit?.Close();
                this.DisposeSafely(this.AcquisitionThread);

                this.IcImagingControl.Sink = null;
                this.IcImagingControl.Invoke((MethodInvoker)delegate { this.DisposeSafely(this.IcImagingControl); });
                this.DisposeSafely(this.FrameQueueSink);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
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
            public uint NumberOfImage { get; init; }

            /// <summary>
            /// 画像取得を続行可能かどうか
            /// </summary>
            public bool IsContinueable
            {
                get => isContinueable;
                set
                {
                    isContinueable = value;
                    if (false == isContinueable)
                    {
                        if (0 < this.WaitHandles.Length && this.WaitHandles[0] is ManualResetEvent e)
                        {
                            e.Set();
                        }
                    }
                }
            }

            #endregion

            #region フィールド

            public WaitHandle[] WaitHandles = new WaitHandle[2];

            protected bool isContinueable = true;

            protected List<ImageContainer> bufferImages = new();

            #endregion

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="numberOfImage"></param>
            public AcquisitionControlInfo(int numberOfImage)
            {
                this.NumberOfImage = (uint)numberOfImage;
                this.WaitHandles[0] = new ManualResetEvent(false);
                this.WaitHandles[1] = new AutoResetEvent(true);
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
            public void OnDataStreamNewBuffer(ImageContainer grabResult)
            {
                try
                {
                    if (this.WaitHandles[1] is AutoResetEvent e)
                    {
                        lock (this.WaitHandles)
                        {
                            bufferImages.Add(grabResult);
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
            public List<ImageContainer> GetDataStreamNewBuffer()
            {
                var images = new List<ImageContainer>();

                try
                {
                    WaitHandle.WaitAny(this.WaitHandles);

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

        /// <summary>
        /// データコンテナ
        /// </summary>
        protected class ImageContainer
        {
            public readonly FrameType FrameType;
            public readonly byte[] Bytes;

            public ImageContainer(FrameType type, byte[] bytes)
            {
                this.FrameType = type;
                this.Bytes = bytes;
            }
        }

        #region フィールド

        protected ICImagingControl IcImagingControl;
        protected FrameQueueSink FrameQueueSink;
        protected FrameType? DefaultFrameType;
        protected float[] AvailableFrameFrameRates = Array.Empty<float>();
        protected AutoResetEvent? WaitEventForInit;

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
        public TisGrabber() : this(typeof(TisGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TisGrabber(Common.Drawing.Point location) : this(typeof(TisGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        [SupportedOSPlatform("windows7.0")]
        public TisGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.IcImagingControl = new();

            // RGB24に固定 → IsColorは常にtrue
            this.FrameQueueSink = new FrameQueueSink(this.FrameQueued, new TIS.Imaging.FrameType(TIS.Imaging.MediaSubtypes.RGB24), 10);
            this.IcImagingControl.Sink = this.FrameQueueSink;

            this.AcquisitionThread = new()
            {
                Priority = ThreadPriority.Highest
            };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// キューイベント
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows7.0")]
        private FrameQueuedResult FrameQueued(IFrameQueueBuffer buffer)
        {
            try
            {
                var bytes = new byte[buffer.FrameType.BufferSize];
                Marshal.Copy(buffer.GetIntPtr(), bytes, 0, bytes.Length);

                var grabResult = new ImageContainer(buffer.FrameType, bytes);

                this.ControlInfo?.OnDataStreamNewBuffer(grabResult);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return FrameQueuedResult.ReQueue;
        }

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        private void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (this.Enabled)
                {
                    this.IcImagingControl.LiveStart();
                    var imageCounter = 0UL;

                    while (info.IsContinueable)
                    {
                        // 画像取得(コールバック)を待機する
                        var bufferImages = info.GetDataStreamNewBuffer();

                        // 画像バッファからデータを列挙
                        foreach (var grabResult in bufferImages)
                        {
                            try
                            {
                                // Bitmapを生成
                                var bitmap = new Bitmap(grabResult.FrameType.Width, grabResult.FrameType.Height, this.IsRgbColor ? PixelFormat.Format24bppRgb : PixelFormat.Format8bppIndexed);

                                if (PixelFormat.Format8bppIndexed == bitmap.PixelFormat)
                                {
                                    BitmapProcessor.AssignColorPaletteOfDefault(bitmap);
                                }

                                var lockedData = bitmap.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , bitmap.PixelFormat
                                                             );
                                Marshal.Copy(grabResult.Bytes, 0, lockedData.Scan0, grabResult.Bytes.Length);

                                bitmap.UnlockBits(lockedData);
                                if (this.Parameter is IGrabberParameter gp && gp.Width > 0 && gp.Height > 0) bitmap = BitmapProcessor.CropAndResizeImage(bitmap, gp.Width, gp.Height, gp.OffsetX, gp.OffsetY);
                                // 撮影データを生成
                                var result = new ByteArrayGrabberDataFastBitmapGeneration(location)
                                {
                                    Counter = ++imageCounter
                                ,
                                    Image = grabResult.Bytes
                                ,
                                    Stride = grabResult.FrameType.BytesPerLine
                                ,
                                    Bitmap = bitmap
                                ,
                                    Size = new Common.Drawing.Size(bitmap?.Size ?? new Size())
                                ,
                                    PixelFormat = bitmap?.PixelFormat ?? PixelFormat.Format8bppIndexed
                                };

                                // 画像取得イベント通知
                                this.OnDataGrabbed(result);

                                // 必要画像枚数に達している、もしくは続行不可能な場合
                                if (
                                    (false == info.IsContinueable)
                                || (0 < info.NumberOfImage && imageCounter >= info.NumberOfImage)
                                )
                                {
                                    // 画像取得終了
                                    info.IsContinueable = false;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                info.IsContinueable = false;
                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
            finally
            {
                this.DisposeSafely(info);
                if (this.Enabled)
                {
                    this.IcImagingControl.LiveStop();
                    this.RunningStopLiveEvent.Set();
                }
            }
        }

        /// <summary>
        /// 画像取得スレッド(open時 フレーム情報取得用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        private void AcquisitionThread_DequeueForInit(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (this.Enabled)
                {
                    // 画像取得終了
                    info.IsContinueable = false;

                    // 撮影時間を短く設定
                    this.ExposureTimeMicroseconds = 1000;

                    // Snap取得設定
                    using var snapSink = new FrameSnapSink(new TIS.Imaging.FrameType(TIS.Imaging.MediaSubtypes.RGB24));
                    this.IcImagingControl.Sink = snapSink;

                    // 撮影開始
                    this.IcImagingControl.DeviceTrigger = false;
                    this.IcImagingControl.LiveStart();

                    // 1フレーム取得
                    using var snapFrame = snapSink.SnapSingle(TimeSpan.FromSeconds(1));

                    // フレーム情報を保持
                    this.DefaultFrameType = snapFrame.FrameType;

                    // 利用可能なフレームレートを取得
                    this.AvailableFrameFrameRates = new float[this.IcImagingControl.DeviceFrameRates.Length];
                    Array.Copy(this.IcImagingControl.DeviceFrameRates, this.AvailableFrameFrameRates, this.AvailableFrameFrameRates.Length);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.DisposeSafely(info);
                if (this.Enabled)
                {
                    this.IcImagingControl.LiveStop();
                }

                this.WaitEventForInit?.Set();
            }
        }
    }

}