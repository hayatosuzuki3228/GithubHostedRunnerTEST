using Hutzper.Library.Common.Laungage;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Library.Common
{
    /// <summary>
    /// 共用IServiceCollection
    /// </summary>
    [Serializable]
    public class ServiceCollectionSharing : IServiceCollectionSharing
    {
        /// <summary>
        /// プロパティ:IServiceCollection
        /// </summary>
        public IServiceCollection ServiceCollection { get; set; } = new ServiceCollection();

        /// <summary>
        /// プロパティ:IServiceProvider
        /// </summary>
        public IServiceProvider? ServiceProvider => this.serviceProvider;

        /// <summary>
        /// プロパティ:シングルトンインスタンス
        /// </summary>
        public static ServiceCollectionSharing Instance => ServiceCollectionSharing.singleton;

        /// <summary>
        /// サービスの作成
        /// </summary>
        public void BuildServiceProvider()
        {
            try
            {
                if (null == this.serviceProvider)
                {
                    this.serviceProvider = this.ServiceCollection.BuildServiceProvider();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static readonly ServiceCollectionSharing singleton = new();

        /// <summary>
        /// IServiceProvider
        /// </summary>
        private IServiceProvider? serviceProvider;

        /// <summary>
        /// ITranslator
        /// </summary>
        /// <returns></returns>
        public ITranslator? GetTranslator()
        {
            ITranslator? translator;

            try
            {
                translator = this.serviceProvider?.GetRequiredService<ITranslator>();
            }
            catch
            {
                translator = null;
            }

            return translator;
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ServiceCollectionSharing()
        {
        }
    }
}