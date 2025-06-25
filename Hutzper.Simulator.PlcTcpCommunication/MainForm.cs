using Hutzper.Library.Common;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.Net.Sockets;
using Hutzper.Library.Forms;

namespace Hutzper.Simulator.PlcTcpCommunication
{
    /// <summary>
    /// PLC通信シミュレータ
    /// </summary>
    /// <remarks>バイナリ一括送受信方式に対応します(PlcDeviceReaderWriterMcp)</remarks>
    public partial class MainForm : ServiceCollectionSharingForm
    {
        #region フィールド

        private TcpBinaryCommunicationListener<TcpBinaryCommunicationLinkageBase> TcpListener;  // 通信リスナ

        private List<TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase>?> TcpClients = new();  // 通信クライアント
        private object SyncClients = new();

        private MemoryStream ReceiveBuffer = new(); // 受信バッファ

        private int[] BitDevice = new int[short.MaxValue]; // 仮想ビットデバイス
        private int[] WordDevice = new int[short.MaxValue];    // 仮想ワードデバイス

        private FpsCalculator performanceCalculator = new() { RequiredFrameNumber = 1000 }; // パフォーマンス計算

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            this.InitializeComponent();

            // エラー
            if (null == this.Services)
            {
                throw new Exception("services is null");
            }

            // 通信リスナ
            this.TcpListener = new();
        }

        #endregion

        #region TcpBinaryCommunicationListener

        /// <summary>
        /// 接続受け入れ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="client"></param>
        /// <param name="notUseClient"></param>
        private void TcpListener_AcceptTcpClient(object sender, TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase> client, out bool notUseClient)
        {
            notUseClient = false;

            try
            {
                lock (this.SyncClients)
                {
                    if (0 >= this.TcpClients.Count)
                    {
                        this.TcpClients.Add(client);

                        this.ReceiveBuffer?.Dispose();
                        this.ReceiveBuffer = new MemoryStream();

                        client.Disconnected += this.TcpClient_Disconnected;
                        client.TransferDataRead += this.TcpClient_TransferDataRead;

                        Serilog.Log.Information($"client connected. connection = {this.TcpClients.Count}");
                    }
                    else
                    {
                        notUseClient = true;
                    }
                }

                if (false == notUseClient)
                {
                    this.InvokeSafely(() =>
                    {
                        this.BackColor = Color.Aqua;
                    });
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region TcpBinaryCommunicationClient

        /// <summary>
        /// 通信クライアントデータ受信イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataBytes"></param>
        private void TcpClient_TransferDataRead(object sender, ArraySegment<byte> dataBytes)
        {
            try
            {
                if (sender is TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase> client)
                {
                    // データをバッファに追加
                    var bytesArray = dataBytes.ToArray();
                    this.ReceiveBuffer.Seek(0, SeekOrigin.End);
                    this.ReceiveBuffer.Write(bytesArray, 0, bytesArray.Length);

                    // ヘッダ
                    var searchList = this.CreateHeader();
                    searchList[0] = 0x50;

                    // バッファの先頭から読み出す
                    this.ReceiveBuffer.Seek(0, SeekOrigin.Begin);
                    if (searchList.Count > this.BytesRemaining(this.ReceiveBuffer))
                    {
                        return;
                    }

                    // ヘッダ検索
                    foreach (var target in searchList)
                    {
                        if (false == this.Search(this.ReceiveBuffer, target))
                        {
                            throw new Exception("header not found");
                        }
                    }

                    // 最低受信長のチェック
                    if ((2 + 12) > this.BytesRemaining(this.ReceiveBuffer))
                    {
                        return;
                    }

                    var readBytes = new byte[this.BytesRemaining(this.ReceiveBuffer)];

                    // データ長
                    var read = this.ReceiveBuffer.Read(readBytes, 0, 2);
                    var length = (int)BitConverter.ToUInt16(new ArraySegment<byte>(readBytes, 0, read));

                    // 監視タイマ(読み捨て)
                    length -= this.ReceiveBuffer.Read(readBytes, 0, 2);
                    var timL = readBytes[0];
                    var timH = readBytes[1];

                    // コマンド
                    length -= this.ReceiveBuffer.Read(readBytes, 0, 2);
                    var cmdL = readBytes[0];
                    var cmdH = readBytes[1];

                    var isWrite = false;
                    if (cmdH == 0x14)
                    {
                        isWrite = true;
                    }

                    // サブコマンド(読み捨て)
                    length -= this.ReceiveBuffer.Read(readBytes, 0, 2);
                    var subL = readBytes[0];
                    var subH = readBytes[1];

                    // 先頭アドレス
                    read = this.ReceiveBuffer.Read(readBytes, 0, 4);
                    length -= read;
                    var device = readBytes[read - 1];
                    readBytes[read - 1] = 0x00;
                    var address = BitConverter.ToInt32(new ArraySegment<byte>(readBytes, 0, read));
                    if (0 > address || short.MaxValue < address)
                    {
                        throw new Exception($"address is invalid. address = {address}");
                    }

                    var isWord = true;
                    switch (device)
                    {
                        case 0xB4:  //  W
                            {
                            }
                            break;

                        case 0xA8:  //  D
                            {
                            }
                            break;

                        case 0xA0:  //  B
                            {
                                isWord = false;
                            }
                            break;

                        case 0x90:  //  M
                            {
                                isWord = false;
                            }
                            break;

                        default:
                            {
                                throw new Exception($"device is invalid. device = 0x{device:X2}");
                            }
                    }

                    // デバイス点数
                    read = this.ReceiveBuffer.Read(readBytes, 0, 2);
                    length -= read;
                    var number = (int)BitConverter.ToUInt16(new ArraySegment<byte>(readBytes, 0, read));
                    if (0 >= number)
                    {
                        throw new Exception($"number is invalid. number = {number}");
                    }
                    else if (true == isWord && 480 < number)
                    {
                        throw new Exception($"number is invalid. number = {number}/ {480}");
                    }
                    else if (false == isWord && (480 * 16) < number)
                    {
                        throw new Exception($"number is invalid. number = {number}/ {480 * 16}");
                    }

                    // 読み出し
                    if (false == isWrite)
                    {
                        // 応答ヘッダ作成
                        var response = this.CreateHeader();

                        // WORD
                        if (true == isWord)
                        {
                            var segmentValues = new ArraySegment<int>(this.WordDevice, address, number).ToArray();

                            var devLength = segmentValues.Length * 2;
                            devLength += 2;
                            response.AddRange(BitConverter.GetBytes((ushort)devLength));
                            response.Add(0x00); // 終了コードH
                            response.Add(0x00); // 終了コードL
                            foreach (var d in segmentValues)
                            {
                                response.AddRange(BitConverter.GetBytes((ushort)d));
                            }
                        }
                        // BIT
                        else
                        {
                            var segmentValues = new ArraySegment<int>(this.BitDevice, address, number).ToList();

                            if (segmentValues.Count % 2 > 0)
                            {
                                segmentValues.Add(0);
                            }

                            var devLength = segmentValues.Count / 2;
                            devLength += 2;
                            response.AddRange(BitConverter.GetBytes((ushort)devLength));
                            response.Add(0x00); // 終了コードH
                            response.Add(0x00); // 終了コードL

                            for (int i = 0; i < segmentValues.Count; i += 2)
                            {
                                if (segmentValues[i] == 1)
                                {
                                    if (segmentValues[i + 1] == 1)
                                    {
                                        response.Add(0x11);
                                    }
                                    else
                                    {
                                        response.Add(0x10);
                                    }
                                }
                                else
                                {
                                    if (segmentValues[i + 1] == 1)
                                    {
                                        response.Add(0x01);
                                    }
                                    else
                                    {
                                        response.Add(0x00);
                                    }
                                }
                            }
                        }

                        // 応答送信
                        client.AysncWrite(response.ToArray());
                    }
                    // 書き込み
                    else
                    {
                        // WORD
                        if (true == isWord)
                        {
                            // 最低受信長のチェック 2byte/デバイス
                            if (number * 2 > this.BytesRemaining(this.ReceiveBuffer))
                            {
                                return;
                            }

                            var devValues = new int[number];
                            for (var i = 0; i < number; i++)
                            {
                                read = this.ReceiveBuffer.Read(readBytes, 0, 2);
                                devValues[i] = BitConverter.ToUInt16(new ArraySegment<byte>(readBytes, 0, read));
                            }

                            var writeIndex = address;
                            foreach (var d in devValues)
                            {
                                this.WordDevice[writeIndex++] = d;
                            }

                            Serilog.Log.Information($"word device write. address = {address}, number = {number}");
                        }
                        // BIT
                        else
                        {

                            // 最低受信長のチェック 2デバイス/byte(偶数)
                            var number2 = (number % 2 == 0) ? number : number + 1;
                            if (number2 / 2 > this.BytesRemaining(this.ReceiveBuffer))
                            {
                                return;
                            }

                            var devValues = new int[number2];
                            for (var i = 0; i < number2; i += 2)
                            {
                                var bitValue = this.ReceiveBuffer.ReadByte();

                                switch (bitValue)
                                {
                                    case 0x11:
                                        devValues[i + 0] = 1;
                                        devValues[i + 1] = 1;
                                        break;
                                    case 0x10:
                                        devValues[i + 0] = 1;
                                        devValues[i + 1] = 0;
                                        break;
                                    case 0x01:
                                        devValues[i + 0] = 0;
                                        devValues[i + 1] = 1;
                                        break;
                                    case 0x00:
                                        devValues[i + 0] = 0;
                                        devValues[i + 1] = 0;
                                        break;
                                }
                            }

                            var writeIndex = address;
                            for (var i = 0; i < number; i++)
                            {
                                this.BitDevice[writeIndex++] = devValues[i];
                            }

                            Serilog.Log.Information($"bit device write. address = {address}, number = {number}");
                        }

                        // 応答送信
                        var response = this.CreateHeader();
                        response.Add(0x02);
                        response.Add(0x00);
                        response.Add(0x00); // 終了コードH
                        response.Add(0x00); // 終了コードL
                        client.AysncWrite(response.ToArray());
                    }

                    // バッファの残りを切り詰め
                    this.ReceiveBuffer.SetLength(this.ReceiveBuffer.Length - this.ReceiveBuffer.Position);

                    if (this.performanceCalculator.AddFrame())
                    {
                        this.InvokeSafely(() =>
                        {
                            this.Text = $"performance = {this.performanceCalculator.Result:F1} Hz";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReceiveBuffer.SetLength(this.ReceiveBuffer.Length - this.ReceiveBuffer.Position);
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 通信クライアント切断イベント
        /// </summary>
        /// <param name="sender"></param>
        private void TcpClient_Disconnected(object sender)
        {
            var isAllDisconnected = false;

            if (sender is TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase> client)
            {
                client.Disconnected -= this.TcpClient_Disconnected;
                client.TransferDataRead -= this.TcpClient_TransferDataRead;
                client.Disconnect();
                client.Dispose();

                lock (this.SyncClients)
                {
                    if (true == this.TcpClients.Contains(client))
                    {
                        this.TcpClients.Remove(client);
                    }

                    if (0 >= this.TcpClients.Count)
                    {
                        isAllDisconnected = true;
                    }
                }
            }

            Serilog.Log.Information($"client disconnected. connection = {this.TcpClients.Count}");

            if (true == isAllDisconnected)
            {
                this.InvokeSafely(() =>
                {
                    this.BackColor = Color.White;
                });
            }
        }

        #endregion

        #region GUIイベント

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.TcpListener.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void onOffUserControl1_ValueChanged(object obj)
        {
            try
            {
                if (true == this.onOffUserControl1.Value)
                {
                    this.TcpListener.AcceptTcpClient += this.TcpListener_AcceptTcpClient;
                    this.TcpListener.BeginListening((int)this.numericUpDown1.Value);
                    Serilog.Log.Information("begin listening");
                }
                else
                {
                    this.TcpListener.AcceptTcpClient -= this.TcpListener_AcceptTcpClient;
                    this.TcpListener.EndListening();
                    Serilog.Log.Information("end listening");

                    var clients = new List<TcpBinaryCommunicationClient<TcpBinaryCommunicationLinkageBase>?>();
                    lock (this.SyncClients)
                    {
                        clients.AddRange(this.TcpClients);
                        this.TcpClients.Clear();
                    }

                    foreach (var client in clients)
                    {
                        try
                        {
                            client?.Disconnect();
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // エラーログ出力
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// ヘッダ作成
        /// </summary>
        /// <returns></returns>
        private List<byte> CreateHeader()
        {
            var header = new List<byte>()
            {
                0xD0,   // サブヘッダL
                0x00,   // サブヘッダH
                0x00,   // ネットワーク番号
                0xFF,   // PC番号
                0xFF,   // 固有値1;L
                0x03,   // 固有値1:H
                0x00,   // 固有値2
            };

            return header;
        }

        /// <summary>
        /// 指定バイト値を検索
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool Search(MemoryStream stream, byte target)
        {
            var isFound = false;

            for (var i = 0; i < stream.Length; i++)
            {
                if (target == stream.ReadByte())
                {
                    isFound = true;
                    break;
                }
            }

            return isFound;
        }

        /// <summary>
        /// 読み込み位置からの残りバイト数を算出
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private long BytesRemaining(MemoryStream stream) => stream.Length - stream.Position;

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog1 = new DeviceMapForm();
            dialog1.SetDeviceValues(true, this.WordDevice);
            dialog1.Show(this);

            var dialog2 = new DeviceMapForm();
            dialog2.SetDeviceValues(false, this.BitDevice);
            dialog2.Show(this);
        }
    }
}