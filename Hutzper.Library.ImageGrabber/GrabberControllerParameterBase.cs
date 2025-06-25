using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// 画像取得制御パラメータ
    /// </summary>
    [Serializable]
    public record GrabberControllerParameterBase : ControllerParameterBaseRecord, IGrabberControllerParameter
    {
        #region IControllerParameter

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        /// <returns></returns>
        public override List<IIniFileCompatible> GetItems() => this.GrabberParameters.ConvertAll(p => (IIniFileCompatible)p);

        #endregion

        #region IGrabberControllerParameter

        /// <summary>
        /// 画像取得パラメータ
        /// </summary>
        public List<IGrabberParameter> GrabberParameters { get; } = new();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public GrabberControllerParameterBase() : this("GrabberControl")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public GrabberControllerParameterBase(string fileNameWithoutExtension) : this("Grabber_Control", "GrabberControllerParameter", $"{fileNameWithoutExtension}.ini")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="iniFileSection"></param>
        /// <param name="directoryName"></param>
        /// <param name="fileNames"></param>
        public GrabberControllerParameterBase(string iniFileSection, string directoryName, string fileName) : base(iniFileSection, directoryName, fileName)
        {
            this.IsHierarchy = false;
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Read(iniFile);

            foreach (var g in this.GrabberParameters)
            {
                if (g is IIniFileCompatible i)
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

            foreach (var g in this.GrabberParameters)
            {
                if (g is IIniFileCompatible i)
                {
                    isSuccess &= i.Write(iniFile);
                }
            }

            return isSuccess;
        }
    }
}