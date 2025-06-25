using System.Text;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCP ASCII文字列通信リスナー
    /// </summary>
    [Serializable]
    public class TcpAsciiTextCommunicationListener : TcpTextCommunicationListener
    {
        public TcpAsciiTextCommunicationListener() : base(Encoding.ASCII)
        {
        }
    }
}