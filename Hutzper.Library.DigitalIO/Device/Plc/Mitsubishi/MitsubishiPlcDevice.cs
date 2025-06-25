using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using System.Text;

namespace Hutzper.Library.DigitalIO.Device.Plc.Mitsubishi
{
    [Serializable]
    public class MitsubishiPlcDevice : ControllerBase, IDigitalIOOutputDevice, IDigitalIOInputDevice, IDigitalIODevice
    {
        #region フィールド

        private ITcpClientWrapper client;

        protected IDigitalIODeviceParameter? Parameter;

        protected Common.Drawing.Point location;

        #endregion

        #region コンストラクタ

        public MitsubishiPlcDevice() : this(typeof(MitsubishiPlcDevice).Name, Common.Drawing.Point.New()) => this.client = null!;

        public MitsubishiPlcDevice(Common.Drawing.Point location) : this(typeof(MitsubishiPlcDevice).Name, location)
        {
        }

        public MitsubishiPlcDevice(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
            this.client = null!;
        }

        #endregion

        #region IController

        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        public override void SetConfig(IApplicationConfig? config) => base.SetConfig(config);

        public override void SetParameter(IControllerParameter? parameter)
        {
            base.SetParameter(parameter);
            if (parameter is MitsubishiPlcDeviceParameter p)
            {
                this.Parameter = p;
                this.DeviceID = p.DeviceID;
                if (p.UseUseDetailedSetting && p.ReadDeviceNumberPairs is not null)
                {
                    this.UseDetailedSetting = true;
                    this.IsBinary = p.IsBinary;
                    this.ReadCommand = p.IsBinary ? this.CreateBinaryRandomReadCommand(p.ReadDeviceNumberPairs.Count, p.ReadDeviceNumberPairs.ToArray()) : this.CreateRandomReadCommand(p.ReadDeviceNumberPairs.Count, p.ReadDeviceNumberPairs.ToArray());
                    this.WriteDeviceNumberPairs = p.WriteDeviceNumberPairs;
                }
            }
        }

        public override void Update() => base.Update();

        #endregion

        #region IDigitalIODevice

        public string DeviceID { get; protected set; } = string.Empty;

        public Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        public bool Enabled => this.IsConnection;

        public event Action<object>? Disabled;

        private bool IsConnection { get; set; } = false;

        private bool UseDetailedSetting { get; set; } = false;
        private object ReadCommand { get; set; } = new();
        private bool IsBinary { get; set; } = false;
        private List<DeviceNumberPair>? WriteDeviceNumberPairs { get; set; }
        private Stream? Stream;
        private static object StreamLock = new();
        public override bool Open()
        {
            try
            {
                if (this.Parameter is MitsubishiPlcDeviceParameter mp)
                {
                    MitsubishiPlcDeviceParameter param = mp;
                    // IPアドレスとポート番号で接続
                    this.client = new TcpClientWrapper();
                    this.client.Connect(param.IpAddress, param.PortNumber);
                    // 接続に失敗した場合は例外をスロー
                    if (false == this.client.Connected) throw new Exception("PLC connection failed.");
                    this.IsConnection = true;
                    this.Stream = this.client.GetStream();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            return this.Enabled;
        }

        public override bool Close()
        {
            try
            {
                // TcpClientがnullでなく、接続されている場合
                if (this.client != null && true == this.client.Connected) this.client.Close(); // クローズする
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
                return false;
            }
            return true;
        }

        #endregion

        #region IDigitalIOInputDevice

        public int NumberOfInputs => this.Parameter?.InputChannels.Length ?? 0;

        // 全読み出し（bool）
        public virtual bool ReadInput(out bool[] values)
        {
            bool result = this.ReadInput(out int[] intValues);
            values = Array.ConvertAll(intValues, v => v != 0);

            return result;
        }

        // 全読み出し（int）
        public virtual bool ReadInput(out int[] values)  //変更
        {
            values = Array.Empty<int>();
            if (this.Stream is not null && this.UseDetailedSetting)
            {
                if (this.IsBinary && this.ReadCommand is not null)
                {
                    if (false == SendCommand((byte[])this.ReadCommand, this.Stream)) return false;
                }
                else if (this.ReadCommand is byte[] bytesReadCommand)
                {
                    bool sendResult = SendCommand((string)this.ReadCommand, this.Stream);
                    if (false == sendResult) return false;
                }
                else return false;
                try
                {
                    bool receiveResult = ReceiveResponse(this.Stream, this.NumberOfInputs, out int[] intValues, this.IsBinary); // 応答の読み取り
                    if (true == receiveResult) values = intValues;
                    else return false;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                    return false;
                }
            }
            else
            {
                int[]? inputDeviceNumbers = this.Parameter?.InputChannels;

                // 読み出しコマンドを作成
                string commandHexString = string.Empty;
                if (inputDeviceNumbers != null)
                {
                    commandHexString = this.CreateReadCommand(this.NumberOfInputs, "X", inputDeviceNumbers);
                    if (string.IsNullOrEmpty(commandHexString)) return false;
                }

                // コマンドを送信
                bool sendResult = SendCommand(commandHexString, this.Stream!);
                if (false == sendResult) return false;
                try
                {
                    bool receiveResult = ReceiveResponse(this.Stream!, this.NumberOfInputs, out int[] intValues); // 応答の読み取り
                    if (true == receiveResult) values = intValues;
                    else return false;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                    return false;
                }
            }
            return true;
        }

        // 指定読み出し（bool）
        public bool ReadInput(int inputDeviceNumber, out bool value)
        {
            bool result = this.ReadInput(inputDeviceNumber, out int intValue);
            value = intValue != 0;

            return result;
        }

        // 指定読み出し（int）
        public bool ReadInput(int inputDeviceNumber, out int value)
        {
            value = 0;
            string commandHexString = this.CreateReadCommand(1, "X", [inputDeviceNumber]); // 読み出しコマンドを作成
            if (string.IsNullOrEmpty(commandHexString)) return false;

            bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
            if (false == sendResult) return false;
            try
            {
                bool receiveResult = ReceiveResponse(this.Stream!, 1, out int[] intValues); // 応答の読み取り
                if (true == receiveResult) value = intValues[0];
                else return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region IPlcOutputDevice

        public int NumberOfOutputs => this.Parameter?.OutputChannels.Length ?? 0;
        public record DeviceNumberPair(string DeviceCode, int NumberPair, int? Value = null, bool IsBit = true, bool IsWork = true);

        // 全書き込み（bool）
        public bool WriteOutput(bool[] values)
        {
            int[] intValues = Array.ConvertAll(values, b => b ? 1 : 0);
            return this.WriteOutput(intValues);
        }

        // 全書き込み（int）
        public virtual bool WriteOutput(int[] values)
        {
            int[]? outputDeviceNumbers = this.Parameter?.OutputChannels;
            string commandHexString = string.Empty;
            if (outputDeviceNumbers != null)
            {
                commandHexString = this.CreateWriteCommand(this.NumberOfOutputs, "Y", outputDeviceNumbers, values); // 書込みコマンドを作成
                if (string.IsNullOrEmpty(commandHexString)) return false;
            }
            bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
            if (false == sendResult) return false;
            return true;
        }

        // 指定書き込み（bool）
        public bool WriteOutput(int outputDeviceNumber, bool value) => this.WriteOutput(outputDeviceNumber, value ? 1 : 0);

        // 指定書き込み（int）
        public bool WriteOutput(int outputDeviceNumber, int value)
        {
            int[] outputDeviceNumbers = { outputDeviceNumber }; // 書込みコマンドを作成
            int[] values = { value };
            if (this.UseDetailedSetting && this.WriteDeviceNumberPairs is not null)
            {
                if (outputDeviceNumber < this.WriteDeviceNumberPairs.Count)
                {
                    DeviceNumberPair targetPair = this.WriteDeviceNumberPairs[outputDeviceNumber];
                    DeviceNumberPair deviceNumberPair = new(targetPair.DeviceCode, targetPair.NumberPair, value, IsBit: true);
                    if (deviceNumberPair is not null) this.WriteRandomValue(deviceNumberPair, this.IsBinary);
                }
            }
            else
            {
                string commandHexString = this.CreateWriteCommand(1, "Y", outputDeviceNumbers, values);
                if (string.IsNullOrEmpty(commandHexString)) return false;
                bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
                if (false == sendResult) return false;
            }
            return true;
        }
        public virtual bool WriteRandomValue(DeviceNumberPair deviceNumberPair, bool isBinary = false)
        {
            if (isBinary)
            {
                byte[]? binaryCommand;
                if (deviceNumberPair.IsBit) binaryCommand = this.CreateBinaryRandomBitWriteCommand(1, new DeviceNumberPair[1] { deviceNumberPair });
                else binaryCommand = this.CreateBinaryRandomWordWriteCommand(deviceNumberPair);
                if (binaryCommand is null || binaryCommand.Length == 0) return false;
                bool sendResult = SendCommand(binaryCommand, this.Stream!);
                if (false == sendResult) return false;
                byte[] response = new byte[1024];
                int bytesRead = 0;
                if (this.Stream is not null && this.Stream.CanRead) bytesRead = this.Stream.Read(response, 0, response.Length);
                if (bytesRead < 11 || response[9] != 0 || response[10] != 0) return false;
            }
            else
            {
                string commandHexString;
                if (deviceNumberPair.IsBit) commandHexString = this.CreateRandomBitWriteCommand(1, new DeviceNumberPair[1] { deviceNumberPair }); // 書込みコマンドを作成
                else commandHexString = this.CreateRandomWordWriteCommand(deviceNumberPair);
                if (string.IsNullOrEmpty(commandHexString)) return false;
                bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
                if (false == sendResult) return false;
                try
                {
                    byte[] response = new byte[1024];
                    string responseString = "";
                    if (this.Stream is not null && this.Stream.CanRead) responseString = Encoding.ASCII.GetString(response, 0, this.Stream!.Read(response, 0, response.Length));
                    if (responseString == "" || responseString.Substring(18, 4) != "0000") return false; // 異常終了の場合
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                    return false;
                }
            }
            return true;
        }
        public virtual bool WriteRandomValue(DeviceNumberPair[] deviceNumberPairs, bool isBinary = false)
        {
            if (isBinary)
            {
                byte[] binaryCommand;
                binaryCommand = this.CreateBinaryRandomBitWriteCommand(deviceNumberPairs.Length, deviceNumberPairs);
                if (binaryCommand.Length == 0) return false;
                bool sendResult = SendCommand(binaryCommand, this.Stream!);
                if (false == sendResult) return false;
                byte[] response = new byte[1024];
                int bytesRead = 0;
                if (this.Stream is not null && this.Stream.CanRead) bytesRead = this.Stream!.Read(response, 0, response.Length);
                if (bytesRead < 11 || response[9] != 0 || response[10] != 0) return false;
            }
            else
            {
                string commandHexString = this.CreateRandomBitWriteCommand(deviceNumberPairs.Length, deviceNumberPairs); // 書込みコマンドを作成
                if (string.IsNullOrEmpty(commandHexString)) return false;
                bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
                byte[] response = new byte[1024];
                try
                {
                    string responseString = Encoding.ASCII.GetString(response, 0, this.Stream!.Read(response, 0, response.Length));
                    if (responseString.Substring(18, 4) != "0000") return false; // 異常終了の場合
                    if (false == sendResult) return false;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                    return false;
                }
            }

            return true;
        }
        // 全読み出し（bool）
        public virtual bool ReadOutput(out bool[] values)
        {
            values = null!;
            if (this.UseDetailedSetting) return true;
            bool result = this.ReadOutput(out int[] intValues);
            if (result) values = Array.ConvertAll(intValues, v => v != 0);
            return result;
        }

        // 全読み出し（int）
        public virtual bool ReadOutput(out int[] values)
        {
            values = Array.Empty<int>();
            if (this.UseDetailedSetting) return true;
            int[]? outputDeviceNumbers = this.Parameter?.OutputChannels;
            string commandHexString = string.Empty; // 読み出しコマンドを作成
            if (outputDeviceNumbers != null)
            {
                commandHexString = this.CreateReadCommand(this.NumberOfOutputs, "Y", outputDeviceNumbers);
                if (string.IsNullOrEmpty(commandHexString)) return false;
            }
            var sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
            if (false == sendResult) return false;
            Thread.Sleep(10); // 10ms待機（PLCの性能限界対策）
            try
            {
                // 応答の読み取り
                bool receiveResult = ReceiveResponse(this.Stream!, this.NumberOfOutputs, out int[] intValues);
                if (true == receiveResult) values = intValues;
                else return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }

            return true;
        }

        // 指定読み出し（bool）
        public bool ReadOutput(int outputDeviceNumber, out bool value)
        {
            int intValue;
            bool result = this.ReadOutput(outputDeviceNumber, out intValue);
            value = intValue != 0;

            return result;
        }
        public bool GetRandomValue(DeviceNumberPair[] deviceNumberPairs, out int[] values, bool isBinary = false)
        {
            values = Array.Empty<int>();
            if (isBinary)
            {
                byte[] binaryCommand = this.CreateBinaryRandomReadCommand(deviceNumberPairs.Length, deviceNumberPairs);
                if (binaryCommand.Length == 0) return false;
                if (false == SendCommand(binaryCommand, this.Stream!)) return false; // コマンドを送信
            }
            else
            {
                string commandHexString = this.CreateRandomReadCommand(deviceNumberPairs.Length, deviceNumberPairs);
                if (string.IsNullOrEmpty(commandHexString)) return false;
                bool sendResult = SendCommand(commandHexString, this.Stream!); // コマンドを送信
                if (false == sendResult) return false;
            }
            Thread.Sleep(10); // 10ms待機（PLCの性能限界対策）
            try
            {
                bool receiveResult = ReceiveResponse(this.Stream!, deviceNumberPairs.Length, out int[] intValues, isBinary);  // 応答の読み取り
                if (true == receiveResult) values = intValues;
                else return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }

            return true;
        }
        public bool GetValue(DeviceNumberPair deviceNumberPair, out int values, bool isBinary = false)
        {
            values = new int();
            if (isBinary)
            {
                byte[] binaryCommand = this.CreateBinaryReadCommand(deviceNumberPair.IsWork ? 1 : 2, deviceNumberPair);
                if (binaryCommand.Length == 0) return false;

                // コマンドを送信
                if (false == SendCommand(binaryCommand, this.Stream!)) return false;
            }
            else
            {
                string commandHexString = this.CreateReadCommand(deviceNumberPair.IsWork ? 1 : 2, deviceNumberPair);
                if (string.IsNullOrEmpty(commandHexString)) return false;

                // コマンドを送信
                if (false == SendCommand(commandHexString, this.Stream!)) return false;
            }
            Thread.Sleep(10); // 10ms待機（PLCの性能限界対策）

            try
            {
                // 応答の読み取り
                if (true == ReceiveWordResponse(this.Stream!, deviceNumberPair.IsWork, isBinary, out int intValues)) values = intValues;
                else return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }

            return true;
        }
        // 指定読み出し（int）
        public bool ReadOutput(int outputDeviceNumber, out int value)
        {
            value = 0;

            // 読み出しコマンドを作成
            int[] outputDeviceNumbers = { outputDeviceNumber };
            string commandHexString = this.CreateReadCommand(1, "Y", outputDeviceNumbers);
            if (string.IsNullOrEmpty(commandHexString))
            {
                return false;
            }

            // コマンドを送信
            var sendResult = SendCommand(commandHexString, this.Stream!);
            if (false == sendResult)
            {
                return false;
            }
            Thread.Sleep(10); // 10ms待機（PLCの性能限界対策）
            try
            {
                // 応答の読み取り
                var receiveResult = ReceiveResponse(this.Stream!, 1, out int[] intValues);
                if (true == receiveResult)
                {
                    value = intValues[0];
                }
                else return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region メソッド

        private string CreateWriteCommand(int accessPointCountInt, string deviceCode, int[] deviceNumbers, int[] values)
        {
            // バリデーション
            if (deviceCode != "Y")
            {
                return string.Empty;
            }
            foreach (int deviceNumber in deviceNumbers)
            {
                if (deviceNumber.ToString().Length > 6)
                {
                    return string.Empty;
                }
            }
            foreach (int value in values)
            {
                if (value != 0 && value != 1)
                {
                    return string.Empty;
                }
            }

            // コマンド部分の文字列を生成
            string accessPointCountString = accessPointCountInt.ToString("X2").PadLeft(2, '0');
            string commandData = "1402"                                     // ランダム書込み
                               + "0001"                                     // ビット単位指定・デバイスメモリ拡張指定なし
                               + accessPointCountString;                    // ワードアクセス点数

            for (int i = 0; i < deviceNumbers.Length; i++)
            {
                int deviceNumberInt = deviceNumbers[i];

                // デバイスコードとデバイス番号を取得
                string deviceNumberString = string.Empty;
                if (deviceNumberInt.ToString().Length <= 6)
                {
                    // デバイス番号を取得
                    deviceNumberString = deviceNumberInt.ToString().PadLeft(6, '0');
                }
                commandData += $"{deviceCode}*" + deviceNumberString + $"0{values[i]}";
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                                // サブヘッダ
                                    + "00"                                  // ネットワーク番号
                                    + "FF"                                  // PC番号
                                    + "03FF"                                // 要求先ユニットI/O番号
                                    + "00"                                  // 要求先ユニット局番号
                                    + commandDataByteLengthString           // 要求データ長
                                    + "0000"                                // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private string CreateRandomBitWriteCommand(int accessPointCountInt, DeviceNumberPair[] deviceNumberPairs)
        {
            // コマンド部分の文字列を生成
            string accessPointCountString = accessPointCountInt.ToString("X2").PadLeft(2, '0');
            string commandData = "1402"                                     // ランダム書込み
                               + "0001"                                     // ビット単位指定・デバイスメモリ拡張指定なし
                               + accessPointCountString;                    // ワードアクセス点数

            for (int i = 0; i < deviceNumberPairs.Length; i++)
            {
                DeviceNumberPair deviceNumberPair = deviceNumberPairs[i];

                // デバイスコードとデバイス番号を取得
                string deviceNumberString = string.Empty;
                if (deviceNumberPair.NumberPair.ToString().Length <= 6)
                {
                    // デバイス番号を取得
                    deviceNumberString = deviceNumberPair.NumberPair.ToString().PadLeft(6, '0');
                }
                commandData += $"{deviceNumberPair.DeviceCode}*" + deviceNumberString + deviceNumberPair.Value?.ToString("X").PadLeft(2, '0');
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                                // サブヘッダ
                                    + "00"                                  // ネットワーク番号
                                    + "FF"                                  // PC番号
                                    + "03FF"                                // 要求先ユニットI/O番号
                                    + "00"                                  // 要求先ユニット局番号
                                    + commandDataByteLengthString           // 要求データ長
                                    + "0000"                                // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private byte[] CreateBinaryRandomBitWriteCommand(int accessPointCountInt, DeviceNumberPair[] deviceNumberPairs)
        {
            List<byte> commandData = new()
            {
                Convert.ToByte("02", 16), // コマンド
                Convert.ToByte("14", 16),

                Convert.ToByte("01", 16), // サブコマンド
                Convert.ToByte("00", 16),

                (byte)(accessPointCountInt & 0xFF),
            };

            foreach (DeviceNumberPair deviceNumberPair in deviceNumberPairs)
            {
                commandData.Add((byte)(deviceNumberPair.NumberPair & 0xFF));
                commandData.Add((byte)((deviceNumberPair.NumberPair >> 8) & 0xFF));
                commandData.Add((byte)((deviceNumberPair.NumberPair >> 16) & 0xFF));

                commandData.Add(Convert.ToByte(this.DeviceIDBinaryTransfer(deviceNumberPair.DeviceCode), 16));

                if (deviceNumberPair.Value is null) continue;
                commandData.Add((byte)(deviceNumberPair.Value! & 0xFF));
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Count + 2; // 監視タイマの4文字を加算する

            List<byte> result = new()
            {
                Convert.ToByte("50", 16),// サブヘッダ
                Convert.ToByte("00", 16),

                Convert.ToByte("00", 16),// ネットワーク番号

                Convert.ToByte("FF", 16),// PC番号

                Convert.ToByte("FF", 16),// 要求先ユニットI/O番号
                Convert.ToByte("03", 16),

                Convert.ToByte("00", 16),// 要求先ユニット局番号

                (byte)(commandDataByteLengthInt & 0xFF),// 要求データ長
                (byte)((commandDataByteLengthInt >> 8) & 0xFF),

                Convert.ToByte("00", 16),// 監視タイマ
                Convert.ToByte("00", 16),
            };
            result.AddRange(commandData);
            return result.ToArray();
        }
        private string CreateRandomWordWriteCommand(DeviceNumberPair deviceNumberPair)
        {
            // コマンド部分の文字列を生成
            bool isWork = deviceNumberPair.IsWork;
            string commandData = "1402"                                     // ランダム書込み
                               + "0000"                                     // ビット単位指定・デバイスメモリ拡張指定なし
                               + (isWork ? "0100" : "0001");                // ワードアクセス点数

            // デバイスコードとデバイス番号を取得
            string deviceNumberString = string.Empty;
            if (deviceNumberPair.NumberPair.ToString().Length <= 6)
            {
                // デバイス番号を取得
                deviceNumberString = deviceNumberPair.NumberPair.ToString().PadLeft(6, '0');
            }
            if (isWork) commandData += $"{deviceNumberPair.DeviceCode}*" + deviceNumberString + ((short)(deviceNumberPair.Value ?? 0)).ToString("X").PadLeft(4, '0');
            else commandData += $"{deviceNumberPair.DeviceCode}*" + deviceNumberString + deviceNumberPair.Value?.ToString("X").PadLeft(8, '0');

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                                // サブヘッダ
                                    + "00"                                  // ネットワーク番号
                                    + "FF"                                  // PC番号
                                    + "03FF"                                // 要求先ユニットI/O番号
                                    + "00"                                  // 要求先ユニット局番号
                                    + commandDataByteLengthString           // 要求データ長
                                    + "0000"                                // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private byte[]? CreateBinaryRandomWordWriteCommand(DeviceNumberPair deviceNumberPair)
        {
            bool isWork = deviceNumberPair.IsWork;

            List<byte> commandData = new()
            {
                Convert.ToByte("02", 16), // コマンド
                Convert.ToByte("14", 16),

                Convert.ToByte("00", 16), // サブコマンド
                Convert.ToByte("00", 16),
            };

            if (isWork)
            {
                commandData.Add(Convert.ToByte("01", 16));
                commandData.Add(Convert.ToByte("00", 16));
            }
            else
            {
                commandData.Add(Convert.ToByte("00", 16));
                commandData.Add(Convert.ToByte("01", 16));
            }

            commandData.Add((byte)(deviceNumberPair.NumberPair & 0xFF));
            commandData.Add((byte)((deviceNumberPair.NumberPair >> 8) & 0xFF));
            commandData.Add((byte)((deviceNumberPair.NumberPair >> 16) & 0xFF));
            try { commandData.Add(Convert.ToByte(this.DeviceIDBinaryTransfer(deviceNumberPair.DeviceCode), 16)); } catch { return null; }


            if (isWork)
            {
                commandData.Add((byte)(deviceNumberPair.Value! & 0xFF));
                commandData.Add((byte)((deviceNumberPair.Value! >> 8) & 0xFF));
            }
            else
            {
                commandData.Add((byte)(deviceNumberPair.Value! & 0xFF));
                commandData.Add((byte)((deviceNumberPair.Value! >> 8) & 0xFF));
                commandData.Add((byte)((deviceNumberPair.Value! >> 16) & 0xFF));
                commandData.Add((byte)((deviceNumberPair.Value! >> 24) & 0xFF));
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Count + 2; // 監視タイマの4文字を加算する

            List<byte> result = new()
            {
                Convert.ToByte("50", 16),// サブヘッダ
                Convert.ToByte("00", 16),

                Convert.ToByte("00", 16),// ネットワーク番号

                Convert.ToByte("FF", 16),// PC番号

                Convert.ToByte("FF", 16),// 要求先ユニットI/O番号
                Convert.ToByte("03", 16),

                Convert.ToByte("00", 16),// 要求先ユニット局番号

                (byte)(commandDataByteLengthInt & 0xFF),// 要求データ長
                (byte)((commandDataByteLengthInt >> 8) & 0xFF),

                Convert.ToByte("00", 16),// 監視タイマ
                Convert.ToByte("00", 16),
            };
            result.AddRange(commandData);
            return result.ToArray();
        }
        private string CreateReadCommand(int accessPointCountInt, string deviceCode, int[] deviceNumbers)
        {
            // バリデーション
            if (deviceCode != "X" && deviceCode != "Y")
            {
                return string.Empty;
            }
            foreach (int deviceNumber in deviceNumbers)
            {
                if (deviceNumber.ToString().Length > 6)
                {
                    return string.Empty;
                }
            }

            // コマンド部分の文字列を生成
            string accessPointCountString = accessPointCountInt.ToString("X2").PadLeft(2, '0');
            string commandData = "0403"                                   // ランダム読出し
                               + "0000"                                   // デバイスメモリ拡張指定なし
                               + accessPointCountString                   // ワードアクセス点数
                               + "00";                                    // ダブルワードアクセス点数

            for (int i = 0; i < deviceNumbers.Length; i++)
            {
                int deviceNumberInt = deviceNumbers[i];

                // デバイスコードとデバイス番号を取得
                string deviceNumberString = string.Empty;
                if (deviceNumberInt.ToString().Length <= 6)
                {
                    // デバイス番号を取得
                    deviceNumberString = deviceNumberInt.ToString().PadLeft(6, '0');
                }
                commandData += $"{deviceCode}*" + deviceNumberString;
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                             // サブヘッダ
                                    + "00"                               // ネットワーク番号
                                    + "FF"                               // PC番号
                                    + "03FF"                             // 要求先ユニットI/O番号
                                    + "00"                               // 要求先ユニット局番号
                                    + commandDataByteLengthString        // 要求データ長
                                    + "0000"                             // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private string CreateRandomReadCommand(int accessPointCountInt, DeviceNumberPair[] deviceNumberPairs)
        {
            // コマンド部分の文字列を生成
            string accessPointCountString = accessPointCountInt.ToString("X2").PadLeft(2, '0');
            string commandData = "0403"                                   // ランダム読出し
                               + "0000"                                   // デバイスメモリ拡張指定なし
                               + accessPointCountString                   // ワードアクセス点数
                               + "00";                                    // ダブルワードアクセス点数

            for (int i = 0; i < deviceNumberPairs.Length; i++)
            {
                DeviceNumberPair deviceNumberPair = deviceNumberPairs[i];

                // デバイスコードとデバイス番号を取得
                string deviceNumberString = string.Empty;
                if (deviceNumberPair.NumberPair.ToString().Length <= 6)
                {
                    // デバイス番号を取得
                    deviceNumberString = deviceNumberPair.NumberPair.ToString().PadLeft(6, '0');
                }
                commandData += $"{deviceNumberPair.DeviceCode}*" + deviceNumberString;
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                             // サブヘッダ
                                    + "00"                               // ネットワーク番号
                                    + "FF"                               // PC番号
                                    + "03FF"                             // 要求先ユニットI/O番号
                                    + "00"                               // 要求先ユニット局番号
                                    + commandDataByteLengthString        // 要求データ長
                                    + "0000"                             // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private byte[] CreateBinaryRandomReadCommand(int accessPointCountInt, DeviceNumberPair[] deviceNumberPairs)
        {
            List<byte> commandData = new()
            {
                Convert.ToByte("03", 16), // コマンド
                Convert.ToByte("04", 16),

                Convert.ToByte("00", 16), // サブコマンド
                Convert.ToByte("00", 16),

                (byte)(accessPointCountInt & 0xFF),

                Convert.ToByte("00", 16),
            };

            foreach (DeviceNumberPair deviceNumberPair in deviceNumberPairs)
            {
                commandData.Add((byte)(deviceNumberPair.NumberPair & 0xFF));
                commandData.Add((byte)((deviceNumberPair.NumberPair >> 8) & 0xFF));
                commandData.Add((byte)((deviceNumberPair.NumberPair >> 16) & 0xFF));

                commandData.Add(Convert.ToByte(this.DeviceIDBinaryTransfer(deviceNumberPair.DeviceCode), 16));
            }

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Count + 2; // 監視タイマの4文字を加算する

            List<byte> result = new()
            {
                Convert.ToByte("50", 16),// サブヘッダ
                Convert.ToByte("00", 16),

                Convert.ToByte("00", 16),// ネットワーク番号

                Convert.ToByte("FF", 16),// PC番号

                Convert.ToByte("FF", 16),// 要求先ユニットI/O番号
                Convert.ToByte("03", 16),

                Convert.ToByte("00", 16),// 要求先ユニット局番号

                (byte)(commandDataByteLengthInt & 0xFF),// 要求データ長
                (byte)((commandDataByteLengthInt >> 8) & 0xFF),

                Convert.ToByte("00", 16),// 監視タイマ
                Convert.ToByte("00", 16),
            };
            result.AddRange(commandData);
            return result.ToArray();
        }
        private string CreateReadCommand(int accessPointCountInt, DeviceNumberPair deviceNumberPair)
        {
            // コマンド部分の文字列を生成
            string commandData = "0401"                                   // コマンド
                               + "0000";                                  // サブコマンド

            // デバイスコードとデバイス番号を取得
            string deviceNumberString = string.Empty;
            string accessPointCountString = accessPointCountInt.ToString("X2").PadLeft(4, '0');
            if (deviceNumberPair.NumberPair.ToString().Length <= 6)
            {
                // デバイス番号を取得
                deviceNumberString = deviceNumberPair.NumberPair.ToString().PadLeft(6, '0');
            }
            commandData += $"{deviceNumberPair.DeviceCode}*" + deviceNumberString + accessPointCountString;

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Length + 4; // 監視タイマの4文字を加算する
            string commandDataByteLengthString = commandDataByteLengthInt.ToString("X4");

            string commandHexString = "5000"                             // サブヘッダ
                                    + "00"                               // ネットワーク番号
                                    + "FF"                               // PC番号
                                    + "03FF"                             // 要求先ユニットI/O番号
                                    + "00"                               // 要求先ユニット局番号
                                    + commandDataByteLengthString        // 要求データ長
                                    + "0000"                             // 監視タイマ
                                    + commandData;

            return commandHexString;
        }
        private byte[] CreateBinaryReadCommand(int accessPointCountInt, DeviceNumberPair deviceNumberPair)
        {
            List<byte> commandData = new()
            {
                Convert.ToByte("01", 16), // コマンド
                Convert.ToByte("04", 16),

                Convert.ToByte("00", 16), // サブコマンド
                Convert.ToByte("00", 16),

                (byte)(deviceNumberPair.NumberPair & 0xFF), //デバイス番号
                (byte)((deviceNumberPair.NumberPair >> 8) & 0xFF),
                (byte)((deviceNumberPair.NumberPair >> 16) & 0xFF),

                Convert.ToByte(this.DeviceIDBinaryTransfer(deviceNumberPair.DeviceCode), 16), //デバイスコード

                (byte)(accessPointCountInt & 0xFF),
                (byte)((accessPointCountInt >> 8) & 0xFF),
            };

            // 要求データ長の計算（コマンドとデータ部分＋監視タイマの文字数を16進数で表現）
            int commandDataByteLengthInt = commandData.Count + 2; // 監視タイマの4文字を加算する

            List<byte> result = new()
            {
                Convert.ToByte("50", 16),// サブヘッダ
                Convert.ToByte("00", 16),

                Convert.ToByte("00", 16),// ネットワーク番号

                Convert.ToByte("FF", 16),// PC番号

                Convert.ToByte("FF", 16),// 要求先ユニットI/O番号
                Convert.ToByte("03", 16),

                Convert.ToByte("00", 16),// 要求先ユニット局番号

                (byte)(commandDataByteLengthInt & 0xFF),// 要求データ長
                (byte)((commandDataByteLengthInt >> 8) & 0xFF),

                Convert.ToByte("00", 16),// 監視タイマ
                Convert.ToByte("00", 16),
            };
            result.AddRange(commandData);
            return result.ToArray();
        }

        private string DeviceIDBinaryTransfer(string DeviceID) => DeviceID switch
        {
            "X" => "9C",
            "Y" => "9D",
            "M" => "90",
            "L" => "92",
            "F" => "93",
            "V" => "94",
            "B" => "A0",
            "S" => "98",
            "D" => "A8",
            "W" => "B4",
            "TS" => "C1",
            "TC" => "C0",
            "TN" => "C2",
            "SS" => "C7",
            "SC" => "C6",
            "SN" => "C8",
            "CS" => "C4",
            "CC" => "C3",
            "CN" => "C5",
            "SB" => "A1",
            "SW" => "B5",
            "SM" => "91",
            "SD" => "A9",
            "Z" => "CC",
            "LZ" => "62",
            "R" => "AF",
            "ZR" => "B0",
            "G" => "AB",
            "BL" => "DC",
            _ => throw new ArgumentException("Unknown DeviceID")
        };
        private static bool SendCommand(string commandHexString, Stream stream) => SendCommand(Encoding.ASCII.GetBytes(commandHexString), stream);
        private static bool SendCommand(byte[] commandBytes, Stream stream)
        {
            // TcpClientを使ってPLCにコマンドを送信
            try
            {
                stream.Write(commandBytes, 0, commandBytes.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private static bool ReceiveResponse(Stream stream, int deviceCount, out int[] values, bool isBinary = false)
        {
            values = Array.Empty<int>();
            byte[] response = new byte[1024]; // 応答の読み取り
            int bytesRead = 0;
            try
            {
                lock (StreamLock)
                {
                    bytesRead = stream.Read(response, 0, response.Length);
                }
            }
            catch { }
            if (bytesRead == 0) return false; // 応答データが存在しない場合
            if (isBinary)
            {
                byte[] result = new byte[bytesRead];
                System.Array.Copy(response, result, bytesRead);
                if (result[9] != 0 || result[10] != 0) return false;
                values = new int[deviceCount];
                for (int i = 0; i < deviceCount; ++i)
                {
                    int startIndex = 11 + i * 2; ;
                    if (startIndex + 2 <= result.Length) values[i] = (int)((short)((result[startIndex + 1] << 8) | (result[startIndex] & 0xFF))) & 1;
                    else return false;
                }
            }
            else
            {

                string responseString = Encoding.ASCII.GetString(response, 0, bytesRead); // 応答データから16進数の文字列を取得
                string endCode = responseString.Substring(18, 4);  // 終了コードを確認（通常は応答データの直前4バイト）
                if (endCode != "0000") return false; // 異常終了の場合
                values = new int[deviceCount]; // 正常終了の場合
                for (int i = 0; i < deviceCount; i++)
                {
                    int startIndex = 22 + i * 4; // 各デバイスデータの開始位置を計算
                    if (startIndex + 4 <= responseString.Length)
                    {
                        string deviceData = responseString.Substring(startIndex, 4);
                        if (int.TryParse(deviceData, System.Globalization.NumberStyles.HexNumber, null, out int value)) values[i] = value & 1;
                        else return false; // データの解析に失敗した場合
                    }
                    else return false; // 応答データが不足している場合
                }
            }

            return true;
        }
        private static bool ReceiveWordResponse(Stream stream, bool isWord, bool isBinary, out int value)
        {
            value = new int();
            byte[] response = new byte[1024]; // 応答の読み取り
            int bytesRead = stream.Read(response, 0, response.Length);
            if (bytesRead == 0) return false; // 応答データが存在しない場合

            if (isBinary)
            {
                byte[] result = new byte[bytesRead];
                System.Array.Copy(response, result, bytesRead);
                if (result[9] != 0 || result[10] != 0) return false;
                int startIndex = 11;
                if (result.Length - startIndex == 2) value = (int)((short)((result[12] << 8) | (result[11] & 0xFF)));
                else if (result.Length - startIndex == 4) value = (int)((result[14] << 24) | (result[13] & 16 | result[12] << 8) | (result[11] & 0xFF));
                else return false;
            }
            else
            {
                string responseString = Encoding.ASCII.GetString(response, 0, bytesRead); // 応答データから16進数の文字列を取得
                if (responseString.Substring(18, 4) != "0000") return false; // 終了コードを確認（通常は応答データの直前4バイト）
                int startIndex = 22; // 各デバイスデータの開始位置を計算
                if (responseString.Length - startIndex == 4)
                {
                    string deviceData = responseString.Substring(responseString.Length - 4, 4);
                    if (!short.TryParse(deviceData, System.Globalization.NumberStyles.HexNumber, null, out short shortValue)) return false;
                    value = shortValue;
                }
                else if (responseString.Length - startIndex == 8)
                {
                    if (!int.TryParse(responseString.Substring(responseString.Length - 4, 4) + responseString.Substring(responseString.Length - 8, 4), System.Globalization.NumberStyles.HexNumber, null, out value)) return false;
                }
                else return false;
            }
            return true;
        }
        #endregion
    }
}