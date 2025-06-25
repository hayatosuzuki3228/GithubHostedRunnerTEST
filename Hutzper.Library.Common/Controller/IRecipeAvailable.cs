using Hutzper.Library.Common.Data.Recipe;

namespace Hutzper.Library.Common.Controller
{
    /// <summary>
    /// レシピ利用インタフェース
    /// </summary>
    /// <remarks>レシピデータを利用するクラスはこのインタフェースを継承します。</remarks>
    public interface IRecipeAvailable
    {
        /// <summary>
        /// レシピを適用する
        /// </summary>
        void Apply(IRecipeData recipeData);
    }
}