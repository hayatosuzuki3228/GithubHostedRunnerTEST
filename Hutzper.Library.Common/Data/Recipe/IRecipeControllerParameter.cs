namespace Hutzper.Library.Common.Data.Recipe
{
    /// <summary>
    /// レシピ管理パラメータインタフェース
    /// </summary>
    public interface IRecipeControllerParameter : IControllerParameter
    {
    }

    /// <summary>
    /// レシピ管理パラメータ
    /// </summary>
    [Serializable]
    public class RecipeControllerParameterBase : ControllerParameterBase, IRecipeControllerParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecipeControllerParameterBase() : base("recipe", "recipe.list")
        {
        }
    }

    /// <summary>
    /// レシピ管理パラメータ(record型)
    /// </summary>
    [Serializable]
    public record RecipeControllerParameterBaseRecord : ControllerParameterBaseRecord, IRecipeControllerParameter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecipeControllerParameterBaseRecord() : base("recipe", "recipe.list")
        {
        }
    }
}