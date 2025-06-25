using Hutzper.Library.Common.Forms;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common.Diagnostics
{
    /// <summary>
    /// IDeviceStatusControl実装:監視対象デバイスステータス表示コントロール
    /// </summary>
    public partial class DeviceStatusUserControl : UserControl, IDeviceStatusControl
    {
        #region IDeviceStatusControl

        /// <summary>
        /// 監視対象デバイス情報
        /// </summary>
        public MonitoredDeviceInfoUnit DeviceInfoUnit { get; protected set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <remarks>紐づける監視対象デバイス情報を割り当てる</remarks>
        public void Initialize(MonitoredDeviceInfoUnit monitoredDeviceInfoUnit)
        {
            try
            {
                if (this.DeviceInfoUnit is not null)
                {
                    this.DeviceInfoUnit.StatusChanged -= this.DeviceInfoUnit_StatusChanged;
                }

                this.DeviceInfoUnit = monitoredDeviceInfoUnit;
                this.DeviceInfoUnit.StatusChanged += this.DeviceInfoUnit_StatusChanged;

                this.StatusLabel.Text = this.DeviceInfoUnit.Name;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 監視対象デバイスイベント:ステータス変化
        /// </summary>
        private void DeviceInfoUnit_StatusChanged(object sender, DeviceStatusKind status)
        {
            try
            {
                this.InvokeSafely(() =>
                {
                    if (status == DeviceStatusKind.Nonuse)
                    {
                        this.StatusLabel.BackColor = SystemColors.ControlDark;
                        this.StatusLabel.ForeColor = SystemColors.ControlLight;
                    }
                    else if (status == DeviceStatusKind.Error)
                    {
                        this.StatusLabel.BackColor = Color.Red;
                        this.StatusLabel.ForeColor = Color.White;
                    }
                    else if (status == DeviceStatusKind.Warning)
                    {
                        this.StatusLabel.BackColor = Color.Orange;
                        this.StatusLabel.ForeColor = Color.Black;
                    }
                    else if (status == DeviceStatusKind.Disabled)
                    {
                        this.StatusLabel.BackColor = Color.Red;
                        this.StatusLabel.ForeColor = Color.White;
                    }
                    else if (status == DeviceStatusKind.Enabled)
                    {
                        this.StatusLabel.BackColor = Color.LimeGreen;
                        this.StatusLabel.ForeColor = Color.Black;
                    }
                    else if (status == DeviceStatusKind.On)
                    {
                        this.StatusLabel.BackColor = Color.Aqua;
                        this.StatusLabel.ForeColor = Color.Black;
                    }
                    else if (status == DeviceStatusKind.Off)
                    {
                        this.StatusLabel.BackColor = SystemColors.ControlLightLight;
                        this.StatusLabel.ForeColor = SystemColors.ControlDarkDark;
                    }
                    else
                    {
                        Serilog.Log.Warning($"{this}, {status}");
                    }
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DeviceStatusUserControl()
        {
            InitializeComponent();

            this.DeviceInfoUnit = new MonitoredDeviceInfoUnit("unknown", 0, 0);
            this.DeviceInfoUnit.StatusChanged += this.DeviceInfoUnit_StatusChanged;

            this.StatusLabel.Text = this.DeviceInfoUnit.Name;
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// ラベルイベント:テキスト変更 OR サイズ変更
        /// </summary>
        private void StatusLabel_SizeChanged(object sender, EventArgs e)
        {
            ControlUtilities.SetFontSizeForTextShrinkToFit(this.StatusLabel, this.Font.Size, this.StatusLabel.Text);
        }

        #endregion
    }
}