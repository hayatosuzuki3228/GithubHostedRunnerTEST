using Hutzper.Library.Common.Controller.Plc;

namespace Hutzper.Library.Forms.Diagnostics
{
    /// <summary>
    /// PLC診断コントロールのインタフェース
    /// </summary>
    public interface IPlcDiagnosticsControl
    {
        /// <summary>
        /// 状態更新間隔ミリ秒
        /// </summary>
        public int StatusRefreshIntervalMs { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="parameter">デバイスマップを含むパラメータ</param>
        public void Initialize(IPlcTcpCommunicatorParameter parameter);

        /// <summary>
        /// 状態更新を開始する
        /// </summary>
        /// <param name="plcTcpCommunicator"></param>
        public void StartDiagnostics(IPlcTcpCommunicator plcTcpCommunicator);

        /// <summary>
        /// 状態更新を終了する
        /// </summary>
        public void EndDiagnostics();
    }
}
