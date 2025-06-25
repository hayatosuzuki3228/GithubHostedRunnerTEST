using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得パラメータ基底
    /// </summary>
    [Serializable]
    public record GrabberParameterBase : ControllerParameterBaseRecord, IGrabberParameter
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        [IniKey(true, "")]
        public virtual string DeviceID { get; set; } = string.Empty;
        public bool UseSetParameter { get; set; } = true;
        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }
        /// <summary>
        /// 使用するUserSetのIndex
        /// </summary>
        [IniKey(true, 0)]
        public int UseUserSetIndex { get; set; }

        /// <summary>
        /// トリガーモード
        /// </summary>
        [IniKey(true, TriggerMode.InternalTrigger)]
        public TriggerMode TriggerMode { get; set; }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        [IniKey(true, 1000d)]
        public virtual double ExposureTimeMicroseconds { get; set; } = 1000d;

        /// <summary>
        /// アナログゲイン
        /// </summary>
        [IniKey(true, 0d)]
        public virtual double AnalogGain { get; set; }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        [IniKey(true, 0d)]
        public virtual double DigitalGain { get; set; }

        /// <summary>
        /// 画像幅
        /// </summary>
        [IniKey(true, -1)]
        public int Width { get; set; } = -1;

        /// <summary>
        /// 画像高さ
        /// </summary>
        [IniKey(true, -1)]
        public int Height { get; set; } = -1;

        /// <summary>
        /// Xオフセット
        /// </summary>
        [IniKey(true, 0)]
        public int OffsetX { get; set; } = 0;

        /// <summary>
        /// Xオフセット
        /// </summary>
        [IniKey(true, 0)]
        public int OffsetY { get; set; } = 0;

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public GrabberParameterBase(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"Grabber_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "GrabberParameter", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}