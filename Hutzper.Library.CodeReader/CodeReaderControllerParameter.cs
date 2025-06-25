using Hutzper.Library.CodeReader.Device;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.CodeReader
{
    /// <summary>
    /// コードリーダー制御パラメータ
    /// </summary>
    [Serializable]
    public record CodeReaderControllerParameter : ControllerParameterBaseRecord, ICodeReaderControllerParameter
    {
        #region IControllerParameter

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        /// <returns></returns>
        public override List<IIniFileCompatible> GetItems() => this.DeviceParameters.ConvertAll(p => (IIniFileCompatible)p);

        #endregion

        #region ICodeReaderControllerParameter

        /// <summary>
        /// デバイスパラメータ
        /// </summary>
        public List<ICodeReaderParameter> DeviceParameters { get; init; } = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public CodeReaderControllerParameter() : this(typeof(CodeReaderControllerParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public CodeReaderControllerParameter(string fileNameWithoutExtension) : base("CodeReader_Control".ToUpper(), "CodeReaderControl", $"{fileNameWithoutExtension}.ini")
        {
            this.IsHierarchy = false;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Read(iniFile);

            foreach (var d in this.DeviceParameters)
            {
                if (d is IIniFileCompatible i)
                {
                    isSuccess &= i.Read(iniFile);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Write(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Write(iniFile);

            foreach (var d in this.DeviceParameters)
            {
                if (d is IIniFileCompatible i)
                {
                    isSuccess &= i.Write(iniFile);
                }
            }

            return isSuccess;
        }

        #endregion
    }
}