namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// Inputシミュレーション可能なデバイスのインターフェース
    /// </summary>
    public interface IDigitalIODeviceInputSimulatable
    {
        /// <summary>
        /// Inputをシミュレート
        /// </summary>
        /// <param name="index">対象のChannel</param>
        /// <param name="value">信号値</param>
        public void SimulateInput(int index, bool value);

        /// <summary>
        /// Inputをシミュレート
        /// </summary>
        /// <param name="index">対象のChannel</param>
        /// <param name="value">信号値</param>
        public void SimulateInput(int index, int value);

        /// <summary>
        /// Inputをシミュレート(連続ch)
        /// </summary>
        /// <param name="values">連続する信号値</param>
        public void SimulateInput(params bool[] values);

        /// <summary>
        /// Inputをシミュレート(連続ch)
        /// </summary>
        /// <param name="values">連続する信号値</param>
        public void SimulateInput(params int[] values);
    }
}