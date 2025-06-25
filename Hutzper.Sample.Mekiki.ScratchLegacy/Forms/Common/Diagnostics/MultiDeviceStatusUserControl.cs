using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common.Diagnostics
{
    public partial class MultiDeviceStatusUserControl : UserControl
    {
        #region プロパティ

        /// <summary>
        /// デバイスリスト
        /// </summary>
        public Dictionary<DeviceKind, MonitoredDeviceInfoUnit> Deviecs { get; } = new();

        public int ItemWidth
        {
            get => this.itemWidth;

            set
            {
                try
                {
                    this.itemWidth = Math.Max(value, 32);

                    foreach (DeviceStatusUserControl control in this.Items.Values)
                    {
                        control.Width = this.itemWidth;
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// コントロールリスト
        /// </summary>
        protected Dictionary<DeviceKind, IDeviceStatusControl> Items = new();

        protected int itemWidth = 160;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiDeviceStatusUserControl()
        {
            InitializeComponent();

            // デザイン時の参考として初期値
            this.AddDevice(DeviceKind.DigitalIO, new MonitoredDeviceInfoUnit("トリガー", 0, 1));
            this.AddDevice(DeviceKind.Camera, new MonitoredDeviceInfoUnit("カメラ", 0, 1));
            this.AddDevice(DeviceKind.Light, new MonitoredDeviceInfoUnit("照明", 0, 1));
            this.AddDevice(DeviceKind.Plc, new MonitoredDeviceInfoUnit("PLC", 0, 1));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            try
            {
                #region 既存のコントロールを破棄
                {
                    foreach (var item in this.Items)
                    {
                        if (item.Value is Control control)
                        {
                            if (this.flpContainer.Contains(control))
                            {
                                this.flpContainer.Controls.Remove(control);
                            }

                            control.Dispose();
                        }
                        this.Items.Clear();
                        this.Deviecs.Clear();
                        this.flpContainer.Controls.Clear();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// デバイス追加
        /// </summary>
        public void AddDevice(DeviceKind deviceKind, MonitoredDeviceInfoUnit monitoredDeviceInfoUnit)
        {
            try
            {
                if (this.Items.ContainsKey(deviceKind))
                {
                    throw new Exception("device is already exits");
                }
                else
                {
                    var control = new DeviceStatusUserControl();
                    control.Initialize(monitoredDeviceInfoUnit);
                    control.Width = this.itemWidth;
                    control.Height = this.flpContainer.ClientSize.Height - control.Margin.Size.Height;

                    this.Items.Add(deviceKind, control);
                    this.flpContainer.Controls.Add(control);
                    this.Deviecs.Add(deviceKind, control.DeviceInfoUnit);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Invalidate();
            }
        }

        private void FlpContainer_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DeviceStatusUserControl control in this.Items.Values)
                {
                    control.Height = this.flpContainer.ClientSize.Height - control.Margin.Size.Height;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Invalidate();
            }
        }
    }
}