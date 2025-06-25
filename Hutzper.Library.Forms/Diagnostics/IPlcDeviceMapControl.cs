using Hutzper.Library.Common.Controller.Plc;

namespace Hutzper.Library.Forms.Diagnostics
{
    /// <summary>
    /// PLCデバイスマップ表示コントロールのインタフェース
    /// </summary>
    public interface IPlcDeviceMapControl
    {
        /// <summary>
        /// 一度に表示できるデータ行数
        /// </summary>
        /// <remarks>タイトル行は含まない</remarks>
        public int MaxDisplayedDataRows { get; }

        /// <summary>
        /// ワードデバイスかどうか(ビットデバイスの場合はfalse)
        /// </summary>
        public bool IsWordDevice { get; }

        /// <summary>
        /// 16進数表示かどうか
        /// </summary>
        public bool IsHexadecimal { get; set; }

        /// <summary>
        /// デバイス書き込み要求イベント
        /// </summary>
        public event Action<object, PlcDeviceUnit, int, int>? DeviceWriteRequested;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="deviceUnit">設定</param>
        /// <param name="referenceDeviceValues">表示のために参照されるデバイス値が格納されるユニット単位の配列</param>
        public void Initialize(PlcDeviceUnit[] deviceUnit, params int[][] referenceDeviceValues);

        /// <summary>
        /// デバイスマップ表示の更新要求
        /// </summary>
        /// <remarks>SetDeviceMapReferenceメソッドで渡したデバイス値を表示します</remarks>
        public void RequestUpdateMap();

        /// <summary>
        /// 入力コントロールを非表示にします
        /// </summary>
        public void HideInputControl();
    }
}
