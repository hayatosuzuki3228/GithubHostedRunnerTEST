using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.LightController.Device.Net.Sockets.Revox
{
    /// <summary>
    /// Revox 照明電源CB-TB TCP通信制御パラメータ
    /// </summary>
    [Serializable]
    public record TcpRevoxCBTB_Parameter : TcpCommunicationParameter
    {
        #region ITcpCommunicationParameter

        /// <summary>
        /// PWMモード設定(SPX-TB-80 のみ有効)
        /// </summary>
        /// <remarks>0:無効 1:外部パルス 2:高速調光</remarks>
        [IniKey(true, 0)]
        public int PwmMode { get; set; }

        /// <summary>
        /// 高速調光値(SPX-TB-80 のみ有効)
        /// </summary>
        /// <remarks>最大12</remarks>
        [IniKey(true, new[] { 0, 0, 0 })]
        public int[] HighSpeedModulationValues { get; set; } = new int[] { 0, 0, 0 };

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpRevoxCBTB_Parameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public TcpRevoxCBTB_Parameter(Common.Drawing.Point location) : base(location)
        {
        }
    }
}