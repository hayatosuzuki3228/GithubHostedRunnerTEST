using Hutzper.Library.Common.Controller;
using Hutzper.Library.InsightLinkage.Controller;

namespace Hutzper.Library.InsightLinkage.Connection
{
    /// <summary>
    /// Insight連携通信
    /// </summary>
    public interface IConnection : IController
    {
        /// <summary>
        /// 有効かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// Insight連携タイプ
        /// </summary>
        public InsightLinkageType LinkageType { get; }
    }
}