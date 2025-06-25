using Hutzper.Library.CodeReader.Device;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.CodeReader
{
    /// <summary>
    /// コードリーダー制御パラメータ
    /// </summary>
    public interface ICodeReaderControllerParameter : IControllerParameter
    {
        #region プロパティ

        /// <summary>
        /// コードリーダーデバイスパラメータ
        /// </summary>
        public List<ICodeReaderParameter> DeviceParameters { get; }

        #endregion
    }
}