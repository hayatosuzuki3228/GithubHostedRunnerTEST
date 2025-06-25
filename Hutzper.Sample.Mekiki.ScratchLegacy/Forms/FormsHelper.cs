using Hutzper.Library.Common;
using Hutzper.Library.Common.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    internal class FormsHelper
    {
        protected Form TargetForm;

        // IServiceCollection
        protected IServiceCollectionSharing? Services;

        public FormsHelper(Form form, IServiceCollectionSharing? serviceCollectionSharing)
        {
            this.TargetForm = form;
            this.Services = serviceCollectionSharing;
        }

        /// <summary>
        /// 確認画面
        /// </summary>
        /// <returns></returns>
        public ConfirmationForm NewConfirmationForm() => this.Services?.ServiceProvider?.GetRequiredService<ConfirmationForm>() ?? new ConfirmationForm();

        /// <summary>
        /// ステータスの追加
        /// </summary>
        public void ChangeDeviceStatusAdd(Label statusLabel, ref DeviceStatusKind statusKind, params DeviceStatusKind[] aditionalStatusKind)
        {
            if (aditionalStatusKind is not null)
            {
                foreach (var status in aditionalStatusKind)
                {
                    statusKind |= status;
                }
            }

            this.ChangeDeviceStatus(statusLabel, statusKind);
        }

        /// <summary>
        /// ステータスの除去
        /// </summary>
        public void ChangeDeviceStatusRemove(Label statusLabel, ref DeviceStatusKind statusKind, params DeviceStatusKind[] removalStatusKind)
        {
            if (removalStatusKind is not null)
            {
                foreach (var status in removalStatusKind)
                {
                    statusKind &= ~status;
                }
            }

            this.ChangeDeviceStatus(statusLabel, statusKind);
        }

        /// <summary>
        /// ステータスの変更
        /// </summary>
        public void ChangeDeviceStatusExchange(Label statusLabel, ref DeviceStatusKind statusKind, DeviceStatusKind removalStatusKind, DeviceStatusKind aditionalStatusKind)
        {
            statusKind &= ~removalStatusKind;
            statusKind |= aditionalStatusKind;

            this.ChangeDeviceStatus(statusLabel, statusKind);
        }

        /// <summary>
        /// 機器ステータスラベル表示変更
        /// </summary>
        public void ChangeDeviceStatus(Label statusLabel, DeviceStatusKind statusKind)
        {
            this.TargetForm.InvokeSafely(() =>
            {
                // 優先順位順でチェックする

                if (true == statusKind.Contains(DeviceStatusKind.Nonuse))
                {
                    statusLabel.BackColor = SystemColors.ControlDark;
                    statusLabel.ForeColor = SystemColors.ControlLight;
                }
                else if (true == statusKind.Contains(DeviceStatusKind.Error))
                {
                    statusLabel.BackColor = Color.Red;
                    statusLabel.ForeColor = Color.White;
                }
                else if (true == statusKind.Contains(DeviceStatusKind.Disabled))
                {
                    statusLabel.BackColor = Color.Red;
                    statusLabel.ForeColor = Color.White;
                }
                else if (true == statusKind.Contains(DeviceStatusKind.On))
                {
                    statusLabel.BackColor = Color.LimeGreen;
                    statusLabel.ForeColor = Color.Black;
                }
                else if (true == statusKind.Contains(DeviceStatusKind.Off))
                {
                    statusLabel.BackColor = SystemColors.ControlLightLight;
                    statusLabel.ForeColor = SystemColors.ControlDarkDark;
                }
                else if (true == statusKind.Contains(DeviceStatusKind.Enabled))
                {
                    statusLabel.BackColor = Color.Aqua;
                    statusLabel.ForeColor = Color.Black;
                }
            });
        }
    }
}
