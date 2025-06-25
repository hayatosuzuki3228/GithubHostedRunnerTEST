using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.Common
{
    [Serializable]
    public enum ConfirmationType
    {
        [AliasName("情報")]
        Info,

        [AliasName("エラー")]
        Error,

        [AliasName("警告")]
        Warning,

        [AliasName("質問")]
        Question,
    }
}