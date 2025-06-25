using Hutzper.Library.Common.Controller;
using Hutzper.Library.InsightLinkage.Connection;
using Hutzper.Library.InsightLinkage.Data;

namespace Hutzper.Library.Insight.Controller
{
    /// <summary>
    /// Insight連携制御
    /// </summary>
    public interface IInsightLinkageController : IController
    {
        /// <summary>
        /// 管理するコネクションリスト
        /// </summary>
        public List<IConnection> Connections { get; set; }

        /// <summary>
        /// コネクション割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params IConnection[] connections);

        /// <summary>
        /// Insightへの送信リクエスト
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Send(params IInsightLingageRequest[] request);

        /// <summary>
        /// 処理結果イベント
        /// </summary>
        public event Action<object, IInsightLingageResult>? Processed;
    }
}