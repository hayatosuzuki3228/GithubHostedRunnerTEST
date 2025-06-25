namespace Hutzper.Library.Common
{
    /// <summary>
    /// 乱数パラメーター
    /// </summary>
    public interface IRandomNumberParameter
    {
    }

    /// <summary>
    /// 乱数生成器インタフェース
    /// </summary>
    public interface IRandomNumberGenerator
    {
        public void Initialize(IRandomNumberParameter parameter);

        public double NextDouble();
    }
}