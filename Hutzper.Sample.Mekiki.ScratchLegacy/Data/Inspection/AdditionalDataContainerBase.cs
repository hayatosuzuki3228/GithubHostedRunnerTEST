using Hutzper.Library.Common;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// IAdditionalDataContainer実装基本クラス
    /// </summary>
    [Serializable]
    public class AdditionalDataContainerBase : SafelyDisposable, IAdditionalDataContainer
    {
        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion
    }
}
