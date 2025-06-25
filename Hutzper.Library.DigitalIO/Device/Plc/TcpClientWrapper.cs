using System.Net.Sockets;

namespace Hutzper.Library.DigitalIO.Device
{
    public class TcpClientWrapper : ITcpClientWrapper
    {
        private TcpClient tcpClient = new();
        private NetworkStream? networkStream;

        public void Connect(string hostname, int port)
        {
            this.tcpClient.Connect(hostname, port);
            this.networkStream = this.tcpClient.GetStream();
            this.networkStream.ReadTimeout = 5000;
            this.networkStream.WriteTimeout = 5000;
        }

        public bool Connected => this.tcpClient.Connected;

        public void Close() => this.tcpClient.Close();

        public NetworkStream GetStream()
        {
            if (this.tcpClient.Connected)
            {
                return this.networkStream ?? this.tcpClient.GetStream();
            }
            else
            {
                throw new InvalidOperationException("TCPクライアントは接続されていません。");
            }
        }

        public int ReceiveTimeout
        {
            get
            {
                if (this.networkStream != null)
                {
                    return this.networkStream.ReadTimeout;
                }
                throw new InvalidOperationException("Network stream is null.");
            }
            set
            {
                if (this.networkStream != null)
                {
                    this.networkStream.ReadTimeout = value;
                }
                else
                {
                    throw new InvalidOperationException("Network stream is null.");
                }
            }
        }

        public int SendTimeout
        {
            get
            {
                if (this.networkStream != null)
                {
                    return this.networkStream.WriteTimeout;
                }
                throw new InvalidOperationException("Network stream is null.");
            }
            set
            {
                if (this.networkStream != null)
                {
                    this.networkStream.WriteTimeout = value;
                }
                else
                {
                    throw new InvalidOperationException("Network stream is null.");
                }
            }
        }
    }
}