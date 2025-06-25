using Hutzper.Library.Common;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common
{
    internal class DeviceStatusLabelController
    {
        public DeviceKind Device { get; init; }

        public DeviceStatusKind Status => CurrentStatus;


        public Label Label { get; }

        protected FormsHelper FormsHelper;
        protected DeviceStatusKind CurrentStatus;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DeviceStatusLabelController(Form form, IServiceCollectionSharing? serviceCollectionSharing, Label label, DeviceKind deviceKind, DeviceStatusKind currentStatus = DeviceStatusKind.Unknown)
        {
            FormsHelper = new FormsHelper(form, serviceCollectionSharing);
            Label = label;
            CurrentStatus = currentStatus;
            Device = deviceKind;
        }

        public void Initialize()
        {
            CurrentStatus = DeviceStatusKind.Unknown;
            FormsHelper.ChangeDeviceStatus(Label, CurrentStatus);
        }

        public DeviceStatusKind ExchangeStatus(DeviceStatusKind removalStatusKind, DeviceStatusKind aditionalStatusKind)
        {
            FormsHelper.ChangeDeviceStatusExchange(Label, ref CurrentStatus, removalStatusKind, aditionalStatusKind);

            return CurrentStatus;
        }

        public DeviceStatusKind AddStatus(params DeviceStatusKind[] aditionalStatusKind)
        {
            FormsHelper.ChangeDeviceStatusAdd(Label, ref CurrentStatus, aditionalStatusKind);

            return CurrentStatus;
        }

        public DeviceStatusKind RemoveStatus(params DeviceStatusKind[] removalStatusKind)
        {
            FormsHelper.ChangeDeviceStatusAdd(Label, ref CurrentStatus, removalStatusKind);

            return CurrentStatus;
        }
    }
}
