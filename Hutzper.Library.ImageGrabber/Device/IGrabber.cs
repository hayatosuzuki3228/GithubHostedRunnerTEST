using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Data;

namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得インタフェース
    /// </summary>
    public interface IGrabber : IController
    {
        /// <summary>
        /// 画像取得タイプ
        /// </summary>
        public GrabberType GrabberType { get; }

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; protected set; }

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// デバイス情報
        /// </summary>
        public IDeviceInfo? DeviceInfo { get; }

        /// <summary>
        /// トリガーモード
        /// </summary>
        public TriggerMode TriggerMode { get; set; }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public double ExposureTimeMicroseconds { get; set; }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public double AnalogGain { get; set; }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public double DigitalGain { get; set; }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        public bool IsRgbColor { get; }

        /// <summary>
        /// 画像幅
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public int OffsetY { get; set; }

        /// <summary>
        /// カメラ温度
        /// </summary>
        public Dictionary<string, double> DeviceTemperature { get; }

        /// <summary>
        /// 画像取得モード
        /// </summary>
        public AcquisitionMode AcquisitionMode { get; set; }

        /// <summary>
        /// 画像取得フレーム数
        /// </summary>
        public int AcquisitionFrameCount { get; set; }

        /// <summary>
        /// トリガータイプ
        /// </summary>
        public TriggerSelector TriggerSelector { get; set; }

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
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public bool Grab();

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public bool GrabContinuously(int number = -1);

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public bool StopGrabbing();

        /// <summary>
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public bool DoSoftTrigger();

        /// <summary>
        /// AW実行
        /// </summary>
        /// <returns></returns>
        public bool DoAutoWhiteBalancing();

        /// <summary>
        /// Balance Ratio（RGBゲイン値）取得
        /// </summary>
        /// <returns></returns>
        public bool GetBalanceRatio(out double gainRedValue, out double gainGreenValue, out double gainBlueValue);

        /// <summary>
        /// デフォルトロード
        /// </summary>
        /// <returns></returns>
        public bool LoadUserSetDefault();

        /// <summary>
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public bool LoadUserSet(int index = 0);

        /// <summary>
        /// ユーザーセットセーブ
        /// </summary>
        /// <returns></returns>
        public bool SaveUserSet(int index = 0);

        /// <summary>
        /// double型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public void GetValues(out Dictionary<ParametersKey, IClampedValue<double>> values, params ParametersKey[] keys);

        /// <summary>
        /// int型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public void GetValues(out Dictionary<ParametersKey, IClampedValue<int>> values, params ParametersKey[] keys);

        /// <summary>
        /// 露光時間セット
        /// </summary>
        /// <returns></returns>
        public bool SetExposureTimeValue(double value);

        /// <summary>
        /// アナログゲインセット
        /// </summary>
        /// <returns></returns>
        public bool SetAnalogGainValue(double value);

        /// <summary>
        /// デジタルゲインセット
        /// </summary>
        /// <returns></returns>
        public bool SetDigitalGainValue(double value);

        /// <summary>
        /// ラインレートセット
        /// </summary>
        /// <returns></returns>
        public bool SetLineRateValue(double value);

        /// <summary>
        /// フレームレートセット
        /// </summary>
        /// <returns></returns>
        public bool SetFrameRateValue(double value);
    }
}