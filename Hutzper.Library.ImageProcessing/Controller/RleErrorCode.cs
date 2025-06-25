namespace Hutzper.Library.ImageProcessing.Controller
{
    [Serializable]
    public enum RleErrorCode
    {
        NotError,
        Undefined,
        OverflowOfRle,
        OverflowOfLine,
        OverflowOfLabel,
    }
}