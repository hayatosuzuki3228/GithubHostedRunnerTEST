namespace Hutzper.Library.Common
{
    /// <summary>
    /// ガウシアン分布乱数パラメーター
    /// </summary>
    [Serializable]
    public record GaussianDistributionRnp : IRandomNumberParameter
    {
        public double Mean { get; set; }

        public double StandardDeviation { get; set; }
    }

    /// <summary>
    /// ガウシアン分布乱数生成器
    /// </summary>
    [Serializable]
    public class GaussianDistributionRng : IRandomNumberGenerator
    {
        private readonly Random random = new();

        private GaussianDistributionRnp paramter = new() { Mean = 0, StandardDeviation = 1.0 };

        public void Initialize(IRandomNumberParameter parameter)
        {
            this.paramter = (GaussianDistributionRnp)parameter;
        }

        public double NextDouble()
        {
            var x = this.random.NextDouble();
            var y = this.random.NextDouble();

            var sigma = this.paramter.StandardDeviation;
            var mean = this.paramter.Mean;

            return sigma * Math.Sqrt(-2.0 * Math.Log(x)) * Math.Cos(2.0 * Math.PI * y) + mean;
        }
    }
}