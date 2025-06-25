using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーデバイスパラメータ
    /// </summary>
    [Serializable]
    public record CodeReaderParameter : ControllerParameterBaseRecord, ICodeReaderParameter
    {
        #region ICodeReaderParameter

        /// <summary>
        /// デバイスID
        /// </summary>
        [IniKey(true, "")]
        public virtual string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }

        /// <summary>
        /// 読み込みタイムアウトミリ秒
        /// </summary>
        [IniKey(true, 1000)]
        public virtual int ReadTimeoutMs { get; set; } = 1000;

        /// <summary>
        /// 連続読み取り間隔ミリ秒
        /// </summary>
        [IniKey(true, 1000)]
        public int ContinuousReadingIntervalMs { get; set; } = 1000;

        #endregion

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CodeReaderParameter() : this(new Common.Drawing.Point(), typeof(CodeReaderParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public CodeReaderParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"CodeReader_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "CodeReaderParameter", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }

        #endregion
    }
}