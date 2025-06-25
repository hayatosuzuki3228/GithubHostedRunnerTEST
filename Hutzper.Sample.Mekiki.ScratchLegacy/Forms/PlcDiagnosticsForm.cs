using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Forms;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    public partial class PlcDiagnosticsForm : ServiceCollectionSharingForm
    {
        #region フィールド

        /// <summary>
        /// アプリ設定
        /// </summary>
        private readonly ProjectInspectionSetting AppConfig;

        /// <summary>
        /// PLC通信
        /// </summary>
        private IPlcTcpCommunicator? PlcTcpCommunicator;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlcDiagnosticsForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlcDiagnosticsForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();

            // 設定ファイルロード
            var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
            this.AppConfig = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
            this.AppConfig.Load();

            // PLC通信
            if (false == string.IsNullOrEmpty(this.AppConfig.PlcTcpCommunicatorParameter?.IpAddress) && 0 < this.AppConfig.PlcTcpCommunicatorParameter.PortNumber)
            {
                this.PlcTcpCommunicator = this.Services?.ServiceProvider?.GetRequiredService<IPlcTcpCommunicator>();
            }
            if (this.PlcTcpCommunicator is not null && this.AppConfig.PlcTcpCommunicatorParameter is not null)
            {
                this.PlcTcpCommunicator.Initialize(this.Services);
                this.PlcTcpCommunicator.SetConfig(this.AppConfig);
                this.PlcTcpCommunicator.SetParameter(this.AppConfig.PlcTcpCommunicatorParameter);

                this.UcPlcDiagnostics.Initialize(this.AppConfig.PlcTcpCommunicatorParameter);
            }
        }

        #endregion

        private void PlcDiagnosticsForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // PLC通信
                if (true == this.PlcTcpCommunicator?.Open())
                {
                    this.UcPlcDiagnostics.StartDiagnostics(this.PlcTcpCommunicator);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.UcPlcDiagnostics.EndDiagnostics();
                this.PlcTcpCommunicator?.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}
