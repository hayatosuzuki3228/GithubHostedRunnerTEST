using Hutzper.Library.Common.Controller;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Controller
{
    /// <summary>
    /// 検査イベントハンドラ
    /// </summary>
    public delegate void InspectionEventHandler(object sender, InspectionEvent @event, IInspectionTask? currentTask, IInspectionTaskItem? currentTaskItem);

    /// <summary>
    /// 機器状態変化イベント
    /// </summary>
    public delegate void DeviceStatusChangedEventHandler(object sender, IController? device, DeviceKind kind, int index, DeviceStatusKind status);

    /// <summary>
    /// 検査制御インタフェース
    /// </summary>
    public interface IInspectionController : IController
    {
        /// <summary>
        /// 検査が有効かどうか
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// 検査ステータス変化
        /// </summary>
        public event InspectionEventHandler? InspectionStatusChanged;

        /// <summary>
        /// 機器状態変化イベント
        /// </summary>
        public event DeviceStatusChangedEventHandler? DeviceStatusChanged;
    }
}