namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// 入力デバイス
    /// </summary>
    public interface IDigitalIOInputDevice : IDigitalIODevice
    {
        /// <summary>
        /// 入力点数
        /// </summary>
        public int NumberOfInputs { get; }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadInput(out bool[] values);

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadInput(out int[] values);

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadInput(int index, out bool value);

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadInput(int index, out int value);
    }
}