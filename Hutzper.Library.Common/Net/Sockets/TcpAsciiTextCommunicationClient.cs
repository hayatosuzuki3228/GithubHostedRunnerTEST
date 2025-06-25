using System.Text;

namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// TCP ASCII文字列通信クライアント
    /// </summary>
    [Serializable]
    public class TcpAsciiTextCommunicationClient : TcpTextCommunicationClient
    {
        public TcpAsciiTextCommunicationClient() : base(Encoding.ASCII)
        {

        }
    }
}