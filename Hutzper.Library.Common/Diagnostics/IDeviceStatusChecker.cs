namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// 機器状態チェックインタフェース
    /// </summary>
    /// <remarks>接続機器のチェックを行うクラスはこのインタフェースを継承します</remarks>
    public interface IDeviceStatusChecker
    {
        /// <summary>
        /// 最後に確認したステータス
        /// </summary>
        public List<IDeviceStatusCheckResult> LatestStatus { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="parameter"></param>
        public void Initialize(IDeviceStatusCheckParameter parameter);

        /// <summary>
        /// チェック
        /// </summary>
        /// <returns></returns>
        public List<IDeviceStatusCheckResult> Check();
    }

    /// <summary>
    /// 機器状態インタフェース
    /// </summary>
    public interface IDeviceStatusCheckResult
    {
        /// <summary>
        /// チェック済みかどうか
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// 正常かどうか
        /// </summary>
        public bool IsNormal { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// ディープコピー
        /// </summary>
        /// <param name="dest"></param>
        public void DeepCopyTo(IDeviceStatusCheckResult dest);
    }

    /// <summary>
    /// 機器状態チェックパラメータ
    /// </summary>
    public interface IDeviceStatusCheckParameter
    {

    }

    /// <summary>
    /// IDeviceStatusCheckResult実装
    /// </summary>
    [Serializable]
    public record DeviceStatusCheckResult : IDeviceStatusCheckResult
    {
        #region IDeviceStatusCheckResult

        /// <summary>
        /// チェック済みかどうか
        /// </summary>
        public bool IsChecked { get; set; } = false;

        /// <summary>
        /// 正常かどうか
        /// </summary>
        public bool IsNormal { get; set; } = false;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        #endregion

        /// <summary>
        /// ディープコピー
        /// </summary>
        /// <param name="dest"></param>
        public virtual void DeepCopyTo(IDeviceStatusCheckResult dest)
        {
            dest.IsChecked = this.IsChecked;
            dest.IsNormal = this.IsNormal;
            dest.LastUpdateTime = this.LastUpdateTime;
        }
    }
}