using System.Net.Sockets;

namespace Hutzper.Library.DigitalIO.Device
{
    public interface ITcpClientWrapper
    {
        // プロパティ
        bool Connected { get; }
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }
        NetworkStream GetStream();

        // メソッド
        void Connect(string hostname, int port);
        void Close();
    }
}