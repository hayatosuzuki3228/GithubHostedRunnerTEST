using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller.Plc;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration
{
    /// <summary>
    /// システム設定
    /// </summary>
    [Serializable]
    internal class ProjectInspectionSetting : ProjectConfiguration
    {
        /// <summary>
        /// 運用画面の設定
        /// </summary>
        public GuiOperationConfiguration GuiOperation { get; init; }

        /// <summary>
        /// 画像に関する設定
        /// </summary>
        public ImagingConfiguration ImagingConfiguration { get; init; }

        /// <summary>
        /// PLC通信設定
        /// </summary>
        public IPlcTcpCommunicatorParameter? PlcTcpCommunicatorParameter { get; init; }

        /// <summary>
        /// 判定パラメータ
        /// </summary>
        public IInferenceResultJudgmentParameter? JudgmentParameter { get; init; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProjectInspectionSetting(IServiceCollectionSharing? serviceCollectionSharing, string fullFileName) : base(serviceCollectionSharing, fullFileName)
        {
            this.Items.Add(this.GuiOperation = new GuiOperationConfiguration());
            this.Items.Add(this.ImagingConfiguration = new ImagingConfiguration());

            // PLC通信設定
            this.PlcTcpCommunicatorParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IPlcTcpCommunicatorParameter>();
            if (this.PlcTcpCommunicatorParameter is IIniFileCompatible iniPlcTcpCommunicatorParameter)
            {
                this.Items.Add(iniPlcTcpCommunicatorParameter);
            }

            // 判定パラメータ
            this.JudgmentParameter = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IInferenceResultJudgmentParameter>();
            if (this.JudgmentParameter is IIniFileCompatible iniJudgmentParameter)
            {
                this.Items.Add(iniJudgmentParameter);
            }
        }
    }
}
