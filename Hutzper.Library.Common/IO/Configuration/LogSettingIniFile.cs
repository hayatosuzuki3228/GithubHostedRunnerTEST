using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.Common.IO.Configuration
{
    /// <summary>
    /// ログ出力設定
    /// </summary>
    [Serializable]
    public record LogSettingIniFile : IniFileCompatible<LogSettingIniFile>
    {
        #region プロパティ

        /// <summary>
        /// 最大保存日数
        /// </summary>
        [IniKey(true, 30)]
        public int MaximumStorageDays { get; set; }

        // /// <summary>
        // /// ログ出力フィルタ
        // /// </summary>
        // /// <remarks>ファイル用</remarks>
        // [IniKey(true, LogCategory.Info | LogCategory.Error | LogCategory.Warning)]
        // public LogCategory PassFilteringForFile { get; set; } = LogCategory.Info | LogCategory.Error | LogCategory.Warning;

        // /// <summary>
        // /// ログ出力フィルタ
        // /// </summary>
        // /// <remarks>モニター用</remarks>
        // [IniKey(true, LogCategory.Info | LogCategory.Error | LogCategory.Warning)]
        // public LogCategory PassFilteringForMonitor { get; set; } = LogCategory.Info | LogCategory.Error | LogCategory.Warning;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogSettingIniFile() : base("LOG")
        {
        }
    }
}