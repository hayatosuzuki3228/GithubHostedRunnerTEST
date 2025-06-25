namespace Hutzper.Library.CodeReader.Device
{
    public interface ITcpCodeReaderParameter : ICodeReaderParameter
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// ポート番号
        /// </summary>
        public int PortNumber { get; set; }
    }
}