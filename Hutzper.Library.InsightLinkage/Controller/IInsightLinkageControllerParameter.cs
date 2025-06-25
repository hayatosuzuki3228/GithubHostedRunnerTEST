using Hutzper.Library.Common.Data;
using Hutzper.Library.InsightLinkage.Connection;

namespace Hutzper.Library.InsightLinkage.Controller
{
    /// <summary>
    /// Insight連携パラメータ
    /// </summary>
    public interface IInsightLinkageControllerParameter : IControllerParameter
    {
        /// <summary>
        /// 使用するかどうか
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// uuid
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 管理するコネクション毎のパラメータリスト
        /// </summary>
        public List<IConnectionParameter> ConnectionParameters { get; set; }

        public List<IFileUploaderParameter> FileUploaderParameters { get; }

        public List<ITextMessengerParameter> TextMessengerParameter { get; }
    }
}