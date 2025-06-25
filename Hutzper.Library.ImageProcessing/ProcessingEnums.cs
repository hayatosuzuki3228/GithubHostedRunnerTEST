namespace Hutzper.Library.ImageProcessing
{
    /// <summary>
    /// 連結方向
    /// </summary>        
    [Serializable]
    public enum ConcatDirection
    {
        Vertical,
    }

    /// <summary>
    /// 配列変換方向
    /// </summary>
    [Serializable]
    public enum ArrayConversionDirection
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// 処理タイプ
    /// </summary>
    [Serializable]
    public enum ProcessignType
    {
        Sequential,
        Parallel,
    }
}