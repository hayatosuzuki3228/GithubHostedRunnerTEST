namespace Hutzper.Library.Common.Attribute
{
    /// <summary>
    /// Enum型に別名をつける属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class AliasNameAttribute : System.Attribute
    {
        public string AliasName { get; private set; }

        public AliasNameAttribute(string aliasName)
        {
            this.AliasName = aliasName;
        }
    }
}