using Hutzper.Library.Common;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.Net.Sockets;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.Setting;

namespace Hutzper.Simulator.DigitalIO
{
    public partial class Form1 : ServiceCollectionSharingForm
    {
        #region フィールド

        /// <summary>
        /// 通信リスナ
        /// </summary>
        private TcpTextCommunicationListener TcpListener;

        /// <summary>
        /// 通信クライアント
        /// </summary>
        private List<TcpTextCommunicationClient?> TcpClients = new();
        private object SyncClients = new();

        private int[] InputValues = new int[8];
        private int[] OutputValues = new int[8];

        private object ValueSync = new object();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            this.InitializeComponent();

            // エラー
            if (null == this.Services)
            {
                throw new Exception("services is null");
            }

            // 通信リスナ
            this.TcpListener = new(System.Text.Encoding.UTF8);
            this.TcpListener.AcceptTcpClient += this.TcpListener_AcceptTcpClient;
        }

        public void ParseArguments(string[] args)
        {
            if (args.Length == 4 && args[0] == "1")
            {
                this.onOffUserControl1.Value = true;
                this.onOffUserControl1_ValueChanged(this);
                this.onOffUserControl2.Value = System.Convert.ToBoolean(args[1]);              // "1" または "0" から bool に変換
                this.numericUpDown2.Value = int.Parse(args[2]);
                this.numericUpDown3.Value = int.Parse(args[3]);
            }
        }

        #endregion

        #region TcpTextCommunicationListener

        /// <summary>
        /// TcpTextCommunicationListener:クライアント接続
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="client"></param>
        /// <param name="notUseClient"></param>
        private void TcpListener_AcceptTcpClient(object sender, TcpTextCommunicationClient client, out bool notUseClient)
        {
            notUseClient = false;

            try
            {
                lock (this.SyncClients)
                {
                    this.TcpClients.Add(client);
                    client.Connected += this.TcpClient_Connected;
                    client.Disconnected += this.TcpClient_Disconnected;
                    client.TransferDataRead += this.TcpClient_TransferDataRead;

                    Serilog.Log.Information("client connected");
                }

                this.InvokeSafely(() =>
                {
                    this.BackColor = Color.Aqua;
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region TcpTextCommunicationClient

        /// <summary>
        /// TcpTextCommunicationClient:文字列受信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void TcpClient_TransferDataRead(object sender, string data)
        {
            try
            {
                var client = (TcpTextCommunicationClient)sender;

                if (6 <= data.Length)
                {
                    // 受信項目の分割
                    var splitedText = data.Split(",");

                    var cmd = splitedText[0];                   // コマンド
                    var pid = Convert.ToInt32(splitedText[1]);  // チャンネルオフセット
                    var num = Convert.ToInt32(splitedText[2]);  // チャンネル数

                    // 読み出し要求
                    if ('R' == cmd[0])
                    {
                        var targetArray = (int[]?)null;
                        if ('I' == cmd[1])
                        {
                            targetArray = this.InputValues;
                        }
                        else if ('O' == cmd[1])
                        {
                            targetArray = this.OutputValues;
                        }

                        if (targetArray is not null)
                        {
                            var valueInt = Array.Empty<int>();
                            lock (this.ValueSync)
                            {
                                valueInt = new ArraySegment<int>(targetArray, pid, num).ToArray();
                            }
                            var valueText = Array.ConvertAll(valueInt, t => t.ToString());

                            client.AysncWrite($"{cmd},{pid},{num},{string.Join(",", valueText)}");
                        }
                    }
                    // 書き込み要求
                    else if ("WO" == cmd)
                    {
                        var valueText = new ArraySegment<string>(splitedText, 3, num).ToArray();
                        var valueInt = Array.ConvertAll(valueText, t => Convert.ToBoolean(Convert.ToInt32(t)) ? 1 : 0);

                        lock (this.ValueSync)
                        {
                            Array.Copy(valueInt, 0, this.OutputValues, pid, valueInt.Length);
                        }

                        client.AysncWrite($"{cmd},{pid},{num}");
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// TcpTextCommunicationClient:切断
        /// </summary>
        /// <param name="obj"></param>
        private void TcpClient_Disconnected(object obj)
        {
            Serilog.Log.Error("client disconnected");

            var isAllDisconnected = false;

            if (obj is TcpTextCommunicationClient client)
            {
                client.Connected -= this.TcpClient_Connected;
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

            this.InvokeSafely(() =>
            {
                if (true == isAllDisconnected)
                {
                    this.BackColor = Color.White;
                }
            });
        }

        /// <summary>
        /// TcpTextCommunicationClient:接続
        /// </summary>
        /// <param name="obj"></param>
        private void TcpClient_Connected(object obj)
        {
            Serilog.Log.Information("client connected");

            this.InvokeSafely(() =>
            {
                this.BackColor = Color.Aqua;
            });
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// リスニング開始/終了
        /// </summary>
        /// <param name="obj"></param>
        private void onOffUserControl1_ValueChanged(object obj)
        {
            try
            {
                if (true == this.onOffUserControl1.Value)
                {
                    this.TcpListener.BeginListening((int)this.numericUpDown1.Value);
                    Serilog.Log.Information("begin listening");
                }
                else
                {
                    this.TcpListener.EndListening();
                    Serilog.Log.Information("end listening");

                    var clients = new List<TcpTextCommunicationClient?>();
                    lock (this.SyncClients)
                    {
                        clients.AddRange(this.TcpClients);
                        this.TcpClients.Clear();
                    }

                    foreach (var client in clients)
                    {
                        try
                        {
                            if (client is not null)
                            {
                                client.Disconnect();

                                client.Connected -= this.TcpClient_Connected;
                                client.Disconnected -= this.TcpClient_Disconnected;
                                client.TransferDataRead -= this.TcpClient_TransferDataRead;

                                client.Dispose();
                            }
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

        /// <summary>
        /// 入力信号値の変更
        /// </summary>
        /// <param name="obj"></param>
        private void onOffUcIn_ValueChanged(object obj)
        {
            if (obj is OnOffUserControl onOff)
            {
                var index = this.flowLayoutPanel1.Controls.IndexOf(onOff);

                lock (this.ValueSync)
                {
                    this.InputValues[index] = onOff.Value ? 1 : 0;
                }

                Serilog.Log.Information($"input changed {index}={(onOff.Value ? "on" : "off")}");
            }
        }

        /// <summary>
        /// 出力信号値の変更
        /// </summary>
        /// <param name="obj"></param>
        private void onOffUcOut_ValueChanged(object obj)
        {
            if (obj is OnOffUserControl onOff)
            {
                var index = this.flowLayoutPanel2.Controls.IndexOf(onOff);

                lock (this.ValueSync)
                {
                    this.OutputValues[index] = onOff.Value ? 1 : 0;
                }

                Serilog.Log.Information($"output changed {index}={(onOff.Value ? "on" : "off")}");
            }
        }

        /// <summary>
        /// 信号値の表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 現在値の取得
            var tempI = new int[this.InputValues.Length];
            var tempO = new int[this.OutputValues.Length];
            lock (this.ValueSync)
            {
                Array.Copy(this.InputValues, tempI, tempI.Length);
                Array.Copy(this.OutputValues, tempO, tempO.Length);
            }

            // 入力値の表示更新
            foreach (OnOffUserControl c in this.flowLayoutPanel1.Controls)
            {
                var index = this.flowLayoutPanel1.Controls.IndexOf(c);

                if (0 <= index)
                {
                    c.Value = Convert.ToBoolean(tempI[index]);
                }
            }

            // 出力値の表示更新
            foreach (OnOffUserControl c in this.flowLayoutPanel2.Controls)
            {
                var index = this.flowLayoutPanel2.Controls.IndexOf(c);

                if (0 <= index)
                {
                    c.Value = Convert.ToBoolean(tempO[index]);
                }
            }
        }

        /// <summary>
        /// 連続トリガー
        /// </summary>
        /// <param name="obj"></param>
        public void onOffUserControl2_ValueChanged(object obj)
        {
            this.flowLayoutPanel1.Enabled = !this.onOffUserControl2.Value;
            this.flowLayoutPanel2.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown2.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown3.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown4.Enabled = !this.onOffUserControl2.Value;

            if (true == this.onOffUserControl2.Value)
            {
                this.onOffUserControl2.Tag = new object();

                var signalHiTime = (int)this.numericUpDown2.Value;
                var signalLoTime = (int)this.numericUpDown3.Value;
                var signalNumber = (ulong)this.numericUpDown4.Value;

                Serilog.Log.Information($"begin auto trigger");

                Task.Run(async () =>
                {
                    try
                    {
                        var counter = 0ul;

                        while (this.onOffUserControl2.Tag is not null)
                        {
                            if (signalNumber <= counter++ && 0 < signalNumber)
                            {
                                break;
                            }

                            lock (this.ValueSync)
                            {
                                this.InputValues[0] = 1;
                            }
                            Serilog.Log.Information($"trigger ■, {counter}");

                            await Task.Delay(signalHiTime);

                            if (this.onOffUserControl2.Tag is null)
                            {
                                break;
                            }

                            lock (this.ValueSync)
                            {
                                this.InputValues[0] = 0;
                            }

                            Serilog.Log.Information($"trigger □, {counter}");
                            await Task.Delay(signalLoTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        Serilog.Log.Information($"end auto trigger");
                    }
                });
            }
            else
            {
                this.onOffUserControl2.Tag = null;
            }
        }
        public void onOffUserControl2_ValueLoad(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Enabled = !this.onOffUserControl2.Value;
            this.flowLayoutPanel2.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown2.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown3.Enabled = !this.onOffUserControl2.Value;
            this.numericUpDown4.Enabled = !this.onOffUserControl2.Value;

            if (true == this.onOffUserControl2.Value)
            {
                this.onOffUserControl2.Tag = new object();

                var signalHiTime = (int)this.numericUpDown2.Value;
                var signalLoTime = (int)this.numericUpDown3.Value;
                var signalNumber = (ulong)this.numericUpDown4.Value;

                Serilog.Log.Information($"begin auto trigger");

                Task.Run(async () =>
                {
                    try
                    {
                        var counter = 0ul;
                        await Task.Delay(1000);
                        while (this.onOffUserControl2.Tag is not null)
                        {
                            if (signalNumber <= counter++ && 0 < signalNumber)
                            {
                                break;
                            }

                            lock (this.ValueSync)
                            {
                                this.InputValues[0] = 1;
                            }
                            Serilog.Log.Information($"trigger ■, {counter}");

                            await Task.Delay(signalHiTime);

                            if (this.onOffUserControl2.Tag is null)
                            {
                                break;
                            }

                            lock (this.ValueSync)
                            {
                                this.InputValues[0] = 0;
                            }

                            Serilog.Log.Information($"trigger □, {counter}");
                            await Task.Delay(signalLoTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        Serilog.Log.Information($"end auto trigger");
                    }
                });
            }
            else
            {
                this.onOffUserControl2.Tag = null;
            }
        }

        #endregion
    }
}