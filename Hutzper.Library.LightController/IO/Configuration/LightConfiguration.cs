using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.LightController.IO.Configuration
{
    /// <summary>
    /// 照明構成
    /// </summary>
    /// <remarks>制御対象のコントローラーを指定します</remarks>
    [Serializable]
    public record LightConfiguration : IniFileCompatible<LightConfiguration>
    {
        /// <summary>
        /// 照明コントローラーリスト
        /// </summary>
        [IniKey(true, new string[] { "CcsPD4" })]
        public string[] Devices { get; set; } = Array.Empty<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LightConfiguration() : base("Light_Configuration".ToUpper())
        {
        }
    }
}