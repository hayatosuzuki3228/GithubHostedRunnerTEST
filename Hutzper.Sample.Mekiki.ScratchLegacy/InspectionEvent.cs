using Hutzper.Library.Common.Attribute;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    /// <summary>
    /// 検査イベント
    /// </summary>
    [Serializable]
    public enum InspectionEvent
    {
        [AliasName("Activating")]
        Activating,

        [AliasName("Deactivating")]
        Deactivating,

        [AliasName("InputSignalOn")]
        InputSignalOn,

        [AliasName("InputSignalOff")]
        InputSignalOff,

        [AliasName("ItemGrabbed")]
        ItemGrabbed,

        [AliasName("AllGrabCompleted")]
        AllGrabCompleted,

        [AliasName("JudgmentRequest")]
        JudgmentRequest,

        [AliasName("JudgmentCompleted")]
        JudgmentCompleted,

        [AliasName("ImageSaving")]
        ImageSaving,

        [AliasName("OutputTiming")]
        OutputTiming,

        [AliasName("TaskCompleted")]
        TaskCompleted,

        [AliasName("ItemsPostProcessRequest")]
        ItemsPostProcessRequest,
    }
}