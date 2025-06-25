using Hutzper.Library.Common.Laungage;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Library.Common
{
    /// <summary>
    /// 共有サービスインタフェース
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServiceCollectionSharing
    {
        public IServiceCollection ServiceCollection { get; }

        public IServiceProvider? ServiceProvider { get; }

        // 翻訳参照の取得
        public ITranslator? GetTranslator();
    }
}