namespace Hutzper.Library.CodeReader.Device
{
    public interface ITcpCodeReader : ICodeReader
    {
        #region イベント

        /// <summary>
        /// 接続された
        /// </summary>
        public event Action<object>? Connected;

        #endregion
    }
}