using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Data;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// Sentechラインセンサ画像取得
    /// </summary>
    [Serializable]
    public class SentechLineSensorGrabber : SentechGrabber, ILineSensor
    {
        #region ILineSensor

        /// <summary>
        /// X反転
        /// </summary>
        public virtual bool ReverseX
        {
            get => base.GetValue("ReverseX", out bool result) ? result : false;

            set => base.SetValue("ReverseX", value);
        }

        /// <summary>
        /// ラインレート
        /// </summary>
        public virtual double LineRateHz
        {
            get => base.GetValue("AcquisitionLineRate", out double value) ? value : 0d;

            set
            {
                double min = 0, max = 0;
                base.GetValue("AcquisitionLineRate", out double _, out min, out max);
                if (0 > value && 0 < max) base.SetValue("AcquisitionLineRate", (float)max);
                else if (0 < max) base.SetValue("AcquisitionLineRate", System.Math.Clamp((float)value, (float)min, (float)max));
            }
        }

        #endregion

        #region IGrabber

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get => 0;
            set { }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (base.Open() && this.Handles.Device is not null)
                {
                    if (this.Parameter is ILineSensorParameter gp)
                    {
                        this.ReverseX = gp.ReverseX;
                        this.LineRateHz = gp.LineRateHz;
                    }
                    this.JaggedArrayConvertor = true == this.IsRgbColor ? this.ToJaggedArrayRgb24 : this.ToJaggedArrayMono8;
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
        /// AW実行
        /// </summary>
        /// <returns></returns>
        /// <remarks>非対応</remarks>
        public override bool DoAutoWhiteBalancing()
        {
            var isSuccess = false;

            try
            {
                Serilog.Log.Warning("AWB is not supported.");
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
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is ILineSensorParameter gp)
                {
                    this.ReverseX = gp.ReverseX;
                    this.LineRateHz = gp.LineRateHz;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region SentechGrabber

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        protected override void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (null != this.Handles.Device && null != this.Handles.DataStream)
                {
                    if (0 < info.NumberOfImage)
                    {
                        this.Handles.DataStream.StartAcquisition((ulong)info.NumberOfImage);
                    }
                    else
                    {
                        this.Handles.DataStream.StartAcquisition();
                    }

                    this.Handles.Device.AcquisitionStart();

                    var imageCounter = 0UL;

                    var stopwatch = new Stopwatch();

                    while (this.Handles.DataStream.IsGrabbing && info.IsContinuable)
                    {
                        // 画像取得(コールバック)を待機する
                        var bufferImages = info.GetDataStreamNewBuffer();

                        // 画像バッファからデータを列挙
                        foreach (var buffer in bufferImages)
                        {
                            imageCounter++;

                            stopwatch.Restart();

                            // 画像データ変換
                            var bytes = this.JaggedArrayConvertor(buffer, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat);

                            // 撮影データを生成
                            var result = new ByteJaggedArrayGrabberData(location)
                            {
                                Counter = imageCounter
                            ,
                                Image = bytes
                            ,
                                Size = imageSize
                            ,
                                PixelFormat = pixelFormat
                            };

                            buffer.Dispose();

                            var copyTimeMs = stopwatch.ElapsedMilliseconds;

                            // 画像取得イベント通知
                            this.OnDataGrabbed(result);

                            var callTimeMs = stopwatch.ElapsedMilliseconds - copyTimeMs;

                            stopwatch.Stop();
                            var totalTimeMs = stopwatch.ElapsedMilliseconds;
                        }
                    }

                    if (false == this.Handles.Device.IsDeviceLost)
                    {
                        // Stop the image acquisition of the camera side.
                        this.Handles.Device?.AcquisitionStop();
                    }

                    // Stop the image acquisition of the host side.
                    this.Handles.DataStream?.StopAcquisition();
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
                this.DisposeSafely(info);
            }
        }

        #endregion

        #region SentechFlatFieldCorrection

        /// <summary>
        /// FFCOffsetMode
        /// </summary>
        /// <param name="mode">読み取ったモード</param>
        /// <returns>成功した場合true</returns>
        /// <remarks>通常は、処理完了を意味するONへの変化検知に使用する</remarks>
        public bool Get_FFCOffsetMode(out FFCOffsetMode mode)
        {
            var isSuccess = false;

            mode = FFCOffsetMode.Off;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    isSuccess = this.GetValue("FFCOffsetMode", out string result);

                    switch (result)
                    {
                        case "Off":
                            mode = FFCOffsetMode.Off;
                            break;

                        case "On":
                            mode = FFCOffsetMode.On;
                            break;

                        case "Once":
                            mode = FFCOffsetMode.Once;
                            break;

                        default: throw new ArgumentOutOfRangeException(nameof(result), result, null);
                    };
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
        /// FFCOffsetMode
        /// </summary>
        /// <param name="mode">読み取ったモード</param>
        /// <returns>成功した場合true</returns>
        /// <remarks>通常は、処理完了を意味するONへの変化検知に使用する</remarks>
        public bool Get_FFCGainMode(out FFCGainMode mode)
        {
            var isSuccess = false;

            mode = FFCGainMode.Off;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    isSuccess = this.GetValue("FFCGainMode", out string result);

                    switch (result)
                    {
                        case "Off":
                            mode = FFCGainMode.Off;
                            break;

                        case "On":
                            mode = FFCGainMode.On;
                            break;

                        case "Once":
                            mode = FFCGainMode.Once;
                            break;

                        case "TargetPlusOnce":
                            mode = FFCGainMode.TargetPlusOnce;
                            break;

                        default: throw new ArgumentOutOfRangeException(nameof(result), result, null);
                    };
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
        /// FFCOffsetMode
        /// </summary>
        /// <param name="mode">指定するモード：Offで待機、それ以外で実行k</param>
        /// <returns>成功した場合true</returns>
        /// <remarks>遮光時の補正</remarks>
        public bool Set_FFCOffsetMode(FFCOffsetMode mode)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    switch (mode)
                    {
                        case FFCOffsetMode.Off:
                            this.SetValue("FFCOffsetMode", "Off", true);
                            break;

                        case FFCOffsetMode.Once:
                            this.SetValue("FFCOffsetMode", "Once", true);
                            break;

                        default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    };

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
        /// FFCGainMode
        /// </summary>
        /// <param name="mode">指定するモード：Offで待機、それ以外で実行k</param>
        /// <returns>成功した場合true</returns>
        /// <remarks>受光時の補正</remarks>
        public bool Set_FFCGainMode(FFCGainMode mode)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    switch (mode)
                    {
                        case FFCGainMode.Off:
                            this.SetValue("FFCGainMode", "Off", true);
                            break;

                        case FFCGainMode.Once:
                            this.SetValue("FFCGainMode", "Once", true);
                            break;

                        case FFCGainMode.TargetPlusOnce:
                            this.SetValue("FFCGainMode", "TargetPlusOnce", true);
                            break;

                        default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    };

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
        /// FFCOffsetTarget
        /// </summary>
        /// <param name="value">目標値0～255</param>
        /// <returns>成功した場合true</returns>
        public bool Set_FFCOffsetTarget(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("FFCOffsetTarget", value);

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
        /// FFCOffsetTarget
        /// </summary>
        /// <param name="value">目標値0～255</param>
        /// <returns>成功した場合true</returns>
        public bool Set_FFCGainTarget(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("FFCGainTarget", value);

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
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public bool Load_FFCSet(int index = 0)
        {
            bool isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 7 > index)
                {
                    this.SetValue("FFCSetSelector", $"FFCSet{index}", true);

                    var commandNode = this.Handles.NodeAccessor?.GetCommand("FFCSetLoad");

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
        public bool Save_FFCSet(int index = 0)
        {
            bool isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 7 > index)
                {
                    this.SetValue("FFCSetDefault", $"FFCSet{index}");
                    this.SetValue("FFCSetSelector", $"FFCSet{index}");

                    var commandNode = this.Handles.NodeAccessor?.GetCommand("FFCSetSave");

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

        #endregion

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected delegate byte[][] ToJaggedArrayDelegate(ImageContainer data, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat);
        protected ToJaggedArrayDelegate JaggedArrayConvertor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechLineSensorGrabber()
        {
            this.GrabberType = GrabberType.LineSensor;
            this.JaggedArrayConvertor = this.ToJaggedArrayMono8;
        }

        /// <summary>
        /// Bitmap変換:Mono8
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual byte[][] ToJaggedArrayMono8(ImageContainer data, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat)
        {
            var jaggedArray = Array.Empty<byte[]>();

            imageSize = new Common.Drawing.Size(data.Size.Width, data.Size.Height);
            pixelFormat = PixelFormat.Format8bppIndexed;

            try
            {
                var oneDimBytes = data.ConvertedBuffer.GetIStImage().GetByteArray();

                jaggedArray = new byte[imageSize.Height][];

                var sourceIndex = 0;
                foreach (var i in Enumerable.Range(0, imageSize.Height))
                {
                    jaggedArray[i] = new byte[imageSize.Width];
                    Array.Copy(oneDimBytes, sourceIndex, jaggedArray[i], 0, jaggedArray[i].Length);

                    sourceIndex += imageSize.Width;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return jaggedArray;
        }

        /// <summary>
        /// Bitmap変換:RGB24
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual byte[][] ToJaggedArrayRgb24(ImageContainer data, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat)
        {
            var jaggedArray = Array.Empty<byte[]>();

            imageSize = new Common.Drawing.Size(data.Size.Width, data.Size.Height);
            pixelFormat = PixelFormat.Format24bppRgb;

            try
            {
                var oneDimBytes = data.ConvertedBuffer.GetIStImage().GetByteArray();

                jaggedArray = new byte[imageSize.Height][];

                var sourceIndex = 0;
                var packedWidth = imageSize.Width * 3;
                foreach (var i in Enumerable.Range(0, imageSize.Height))
                {
                    jaggedArray[i] = new byte[packedWidth];
                    Array.Copy(oneDimBytes, sourceIndex, jaggedArray[i], 0, jaggedArray[i].Length);

                    sourceIndex += packedWidth;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return jaggedArray;
        }
    }
}