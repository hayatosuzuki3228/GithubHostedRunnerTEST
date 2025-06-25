namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// 出力デバイス
    /// </summary>
    public interface IDigitalIOOutputDevice : IDigitalIODevice
    {
        /// <summary>
        /// 出力点数
        /// </summary>
        public int NumberOfOutputs { get; }

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool WriteOutput(bool[] values);

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool WriteOutput(int[] values);

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteOutput(int index, bool value);

        /// <summary>
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteOutput(int index, int value);

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadOutput(out bool[] values);

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadOutput(out int[] values);

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadOutput(int index, out bool value);

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadOutput(int index, out int value);
    }
}