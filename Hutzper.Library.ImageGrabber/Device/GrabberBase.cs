using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.ImageGrabber.Data;
using System.Diagnostics;

namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得基底クラス
    /// </summary>
    public abstract class GrabberBase : ControllerBase, IGrabber
    {
        #region IGrabber

        /// <summary>
        /// 画像取得タイプ
        /// </summary>
        public virtual GrabberType GrabberType { get; protected set; }

        /// <summary>
        /// デバイスID
        /// </summary>
        public virtual string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public virtual bool Enabled { get; }

        /// <summary>
        /// デバイス情報
        /// </summary>
        public virtual IDeviceInfo? DeviceInfo { get; }

        /// <summary>
        /// トリガーモード
        /// </summary>
        public virtual TriggerMode TriggerMode
        {
            get => TriggerMode.InternalTrigger;
            set { }
        }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public virtual double ExposureTimeMicroseconds { get; set; }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public virtual double AnalogGain { get; set; }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public virtual double DigitalGain { get; set; }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        public virtual bool IsRgbColor { get; }

        /// <summary>
        /// 設定項目が反映するかどうか
        /// </summary>
        public virtual bool UseSetParameter { get; set; } = true;

        /// <summary>
        /// 画像幅
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public virtual int OffsetX { get; set; }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public virtual int OffsetY { get; set; }

        /// <summary>
        /// カメラ温度
        /// </summary>
        public virtual Dictionary<string, double> DeviceTemperature => new Dictionary<string, double>();

        /// <summary>
        /// 画像取得モード
        /// </summary>
        public virtual AcquisitionMode AcquisitionMode
        {
            get => AcquisitionMode.Continuous;
            set => Serilog.Log.Warning($"{this}, AcquisitionMode is not supported.");
        }

        /// <summary>
        /// 画像取得フレーム数
        /// </summary>
        public virtual int AcquisitionFrameCount
        {
            get => 0;
            set => Serilog.Log.Warning($"{this}, AcquisitionFrameCount is not supported.");
        }

        /// <summary>
        /// トリガータイプ
        /// </summary>
        public virtual TriggerSelector TriggerSelector
        {
            get => TriggerSelector.Unsupported;
            set => Serilog.Log.Warning($"{this}, TriggerSelector is not supported.");
        }

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, IGrabberError>? ErrorOccurred;

        /// <summary>
        /// データ取得イベント
        /// </summary>
        public event Action<object, IGrabberData>? DataGrabbed;

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public virtual bool Grab()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public virtual bool GrabContinuously(int number = -1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public virtual bool StopGrabbing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public virtual bool DoSoftTrigger()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// AW実行
        /// </summary>
        /// <returns></returns>
        public virtual bool DoAutoWhiteBalancing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Balance Ratio（RGBゲイン値）取得
        /// </summary>
        /// <returns></returns>
        public virtual bool GetBalanceRatio(out double gainRedValue, out double gainGreenValue, out double gainBlueValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadUserSetDefault()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadUserSet(int index = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ユーザーセットトセーブ
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveUserSet(int index = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// double型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual void GetValues(out Dictionary<ParametersKey, IClampedValue<double>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<double>>();
        }

        /// <summary>
        /// int型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual void GetValues(out Dictionary<ParametersKey, IClampedValue<int>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<int>>();
        }

        /// <summary>
        /// 露光時間セット
        /// </summary>
        /// <returns></returns>
        public virtual bool SetExposureTimeValue(double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// アナログゲインセット
        /// </summary>
        /// <returns></returns>
        public virtual bool SetAnalogGainValue(double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// デジタルゲインセット
        /// </summary>
        /// <returns></returns>
        public virtual bool SetDigitalGainValue(double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ラインレートセット
        /// </summary>
        /// <returns></returns>
        public virtual bool SetLineRateValue(double value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// フレームレートセット
        /// </summary>
        /// <returns></returns>
        public virtual bool SetFrameRateValue(double value)
        {
            throw new NotImplementedException();
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
                if (parameter is IGrabberParameter gp)
                {
                    this.Parameter = gp;
                    this.Location = gp.Location;
                    this.DeviceID = gp.DeviceID;
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        protected IControllerParameter? Parameter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public GrabberBase(string nickname, int locationX, int locationY) : this(nickname, new Common.Drawing.Point(locationX, locationY))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public GrabberBase(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnErrorOccurred(IGrabberError grabberError)
        {
            try
            {
                this.ErrorOccurred?.Invoke(this, grabberError);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberData"></param>
        protected virtual void OnDataGrabbed(IGrabberData grabberData)
        {
            try
            {
                this.DataGrabbed?.Invoke(this, grabberData);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        protected virtual void OnDisabled()
        {
            try
            {
                this.Disabled?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}