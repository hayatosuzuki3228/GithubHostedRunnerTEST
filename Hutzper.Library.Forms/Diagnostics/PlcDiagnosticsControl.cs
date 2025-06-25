using Hutzper.Library.Common.Controller.Plc;

namespace Hutzper.Library.Forms.Diagnostics
{
    /// <summary>
    /// IPlcDiagnosticsControl実装ユーザーコントロール
    /// </summary>
    /// <remarks>PLCとの通信状態や、デバイスマップを表示します</remarks>
    public partial class PlcDiagnosticsControl : HutzperUserControl, IPlcDiagnosticsControl
    {
        #region IPlcDiagnosticsControl

        /// <summary>
        /// 状態更新間隔ミリ秒
        /// </summary>
        public int StatusRefreshIntervalMs
        {
            get => this.StatusRefreshTimer.Interval;
            set => this.StatusRefreshTimer.Interval = Math.Max(10, value);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="parameter">デバイスマップを含むパラメータ</param>
        public void Initialize(IPlcTcpCommunicatorParameter parameter)
        {
            var isInitialized = false;

            try
            {
                this.WordValues.Clear();
                this.BitValues.Clear();

                foreach (var map in this.DeviceMaps)
                {
                    var unitAndValues = new Dictionary<PlcDeviceUnit, int[]>();

                    if (map.Key == "Word")
                    {
                        foreach (var unit in parameter.WordDeviceUnits)
                        {
                            unitAndValues.Add(unit, new int[unit.RangeLength]);
                        }

                        foreach (var uav in unitAndValues)
                        {
                            this.WordValues.Add(uav.Value);
                        }
                    }
                    else if (map.Key == "Bit")
                    {
                        foreach (var unit in parameter.BitDeviceUnits)
                        {
                            unitAndValues.Add(unit, new int[unit.RangeLength]);
                        }

                        foreach (var uav in unitAndValues)
                        {
                            this.BitValues.Add(uav.Value);
                        }
                    }
                    else
                    {
                        continue;
                    }

                    map.Value.Initialize(unitAndValues.Keys.ToArray(), unitAndValues.Values.ToArray());
                }

                isInitialized = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                if (true == isInitialized)
                {
                    this.Parameter = parameter;
                }
            }
        }

        /// <summary>
        /// 状態更新を開始する
        /// </summary>
        /// <param name="plcTcpCommunicator"></param>
        public void StartDiagnostics(IPlcTcpCommunicator plcTcpCommunicator)
        {
            try
            {
                if (this.Parameter is null)
                {
                    throw new InvalidOperationException("Initialize method must be called before using this functionality.");
                }

                if (this.PlcCommunicator is not null)
                {
                    this.EndDiagnostics();
                }

                this.PlcCommunicator = plcTcpCommunicator;
            }
            catch (Exception ex)
            {
                this.PlcCommunicator = null;
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                if (this.PlcCommunicator is not null)
                {
                    this.StatusRefreshTimer.Start();
                }
            }
        }

        /// <summary>
        /// 状態更新を終了する
        /// </summary>
        public void EndDiagnostics()
        {
            try
            {
                this.StatusRefreshTimer.Stop();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.PlcCommunicator = null;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// Plc通信制御
        /// </summary>
        private IPlcTcpCommunicator? PlcCommunicator;

        /// <summary>
        /// パラメータ
        /// </summary>
        private IPlcTcpCommunicatorParameter? Parameter;

        /// <summary>
        /// デバイスマップ
        /// </summary>
        private Dictionary<string, IPlcDeviceMapControl> DeviceMaps = new();

        /// <summary>
        /// ワードデバイス値
        /// </summary>
        private List<int[]> WordValues = new();

        /// <summary>
        /// ビットデバイス値
        /// </summary>
        private List<int[]> BitValues = new();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlcDiagnosticsControl()
        {
            this.InitializeComponent();

            this.nickname = "PlcDiagnostics";

            this.DeviceMaps.Add("Word", this.UcDeviceMapWord);
            this.DeviceMaps.Add("Bit", this.UcDeviceMapBit);

            foreach (var map in this.DeviceMaps.Values)
            {
                map.DeviceWriteRequested += this.DeviceMap_DeviceWriteRequested;
            }

            // 表示形式
            this.UcBaseNumber.SetItemStringArray(new[] { "10進数", "16進数" });
            this.UcBaseNumber.SelectedIndex = 0;
        }

        #region IPlcDeviceMapControl

        /// <summary>
        /// IPlcDeviceMapControl:書き込み要求イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceUnit">対象のユニット</param>
        /// <param name="relativeIndexInUnit">ユニット内の相対アドレス</param>
        /// <param name="value">書込み値</param>
        private void DeviceMap_DeviceWriteRequested(object sender, PlcDeviceUnit deviceUnit, int relativeIndexInUnit, int value)
        {
            try
            {
                if (this.PlcCommunicator is null)
                {
                    return;
                }

                var request = new PlcWriteRequest(deviceUnit.Index, relativeIndexInUnit, value);

                if (deviceUnit.IsWord)
                {
                    this.PlcCommunicator.WriteWordDevice(request);
                }
                else
                {
                    this.PlcCommunicator.WriteBitDevice(request);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// 状態更新タイマイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusRefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.PlcCommunicator is not null)
                {
                    this.LabelStatus.BackColor = this.PlcCommunicator.IsProcessingCorrectly ? Color.LimeGreen : Color.Red;
                    this.LabelStatus.ForeColor = this.PlcCommunicator.IsProcessingCorrectly ? Color.Black : Color.White;

                    #region ワードデバイスの瞬時値取得
                    if (0 < this.WordValues.Count)
                    {
                        var values = this.PlcCommunicator.GetInstantWordValues();
                        foreach (var holdingValues in this.WordValues)
                        {
                            var unitIndex = this.WordValues.IndexOf(holdingValues);
                            var currentValues = values[unitIndex].ToArray();

                            Array.Copy(currentValues, holdingValues, holdingValues.Length);
                        }
                    }
                    #endregion

                    #region ビットデバイスの瞬時値取得
                    if (0 < this.BitValues.Count)
                    {
                        var values = this.PlcCommunicator.GetInstantBitValues();
                        foreach (var holdingValues in this.BitValues)
                        {
                            var unitIndex = this.BitValues.IndexOf(holdingValues);
                            var currentValues = values[unitIndex].ToArray();

                            Array.Copy(currentValues, holdingValues, holdingValues.Length);
                        }
                    }
                    #endregion

                    // マップ更新要求
                    foreach (var map in this.DeviceMaps.Values)
                    {
                        map.RequestUpdateMap();
                    }
                }
                else
                {
                    this.LabelStatus.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void UcBaseNumber_SelectedIndexChanged(object sender, int index, string text)
        {
            try
            {
                foreach (var map in this.DeviceMaps.Values)
                {
                    map.IsHexadecimal = (this.UcBaseNumber.SelectedIndex != 0);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void PlcDiagnosticsControl_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                foreach (var map in this.DeviceMaps.Values)
                {
                    map.HideInputControl();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}
