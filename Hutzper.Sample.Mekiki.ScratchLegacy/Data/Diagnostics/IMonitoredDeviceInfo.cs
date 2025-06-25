namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data.Diagnostics
{
    /// <summary>
    /// 監視対象機器情報
    /// </summary>
    public interface IMonitoredDeviceInfo
    {
        /// <summary>
        /// 表示名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// デバイスインデックス
        /// </summary>
        int Index { get; }

        /// <summary>
        /// ステータス
        /// </summary>
        DeviceStatusKind Status { get; set; }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        object? Tag { get; set; }

        /// <summary>
        /// ステータス変化イベント
        /// </summary>
        event Action<object, DeviceStatusKind>? StatusChanged;

        /// <summary>
        /// 更新
        /// </summary>
        void Update();
    }

    /// <summary>
    /// IMonitoredDeviceInfo実装
    /// </summary>
    [Serializable]
    public class MonitoredDeviceInfo : IMonitoredDeviceInfo
    {
        #region IMonitoredDeviceInfo

        /// <summary>
        /// 表示名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// デバイスインデックス
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// ステータス
        /// </summary>
        public DeviceStatusKind Status
        {
            get => this.CurrentStatus;

            set
            {
                var hasChanged = false;
                lock (this.SyncStatus)
                {
                    if (this.Status != value)
                    {
                        this.CurrentStatus = value;
                        hasChanged = true;
                    }
                }

                if (true == hasChanged)
                {
                    this.OnStatusChanged();
                }
            }
        }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// ステータス変化イベント
        /// </summary>
        public event Action<object, DeviceStatusKind>? StatusChanged;

        /// <summary>
        /// 更新
        /// </summary>
        public void Update() => this.OnStatusChanged();

        #endregion

        #region フィールド

        /// <summary>
        /// 現在のステータス
        /// </summary>
        protected DeviceStatusKind CurrentStatus;

        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        protected object SyncStatus = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonitoredDeviceInfo(string name, int index, object? tag = null)
        {
            this.Name = name;
            this.Index = index;
            this.Tag = tag;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ステータス変更イベント通知
        /// </summary>
        protected void OnStatusChanged() => this.StatusChanged?.Invoke(this, this.Status);

        #endregion
    }

    /// <summary>
    /// IMonitoredDeviceInfoリスト管理
    /// </summary>
    [Serializable]
    public class MonitoredDeviceInfoUnit : IMonitoredDeviceInfo
    {
        #region プロパティ

        public List<IMonitoredDeviceInfo> Items { get; } = new();

        #endregion

        #region IMonitoredDeviceInfo

        /// <summary>
        /// 表示名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// デバイスインデックス
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// ステータス
        /// </summary>
        public DeviceStatusKind Status
        {
            get
            {
                var margedStatus = DeviceStatusKind.Unknown;

                foreach (var s in this.StatusArray)
                {
                    foreach (var item in this.Items)
                    {
                        if (s == item.Status)
                        {
                            margedStatus = s;
                            break;
                        }
                    }
                }

                return margedStatus;
            }

            set
            {
                // 何もしない
            }
        }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// ステータス変化イベント
        /// </summary>
        public event Action<object, DeviceStatusKind>? StatusChanged;

        /// <summary>
        /// 更新
        /// </summary>
        public void Update() => this.OnStatusChanged();

        #endregion

        #region フィールド

        /// <summary>
        /// ステータ種リスト
        /// </summary>
        protected List<DeviceStatusKind> StatusArray = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonitoredDeviceInfoUnit(string name, int index, int numOfDevice, object? tag = null)
        {
            this.Name = name;
            this.Index = index;
            this.Tag = tag;

            foreach (var i in Enumerable.Range(0, Math.Max(numOfDevice, 0)))
            {
                var item = new MonitoredDeviceInfo($"{this.Name}{i + 1:D2}", i, this.Tag);

                item.StatusChanged += this.Item_StatusChanged;

                this.Items.Add(item);
            }

            foreach (DeviceStatusKind s in Enum.GetValues(typeof(DeviceStatusKind)))
            {
                this.StatusArray.Add(s);
            }
        }

        #endregion

        #region IMonitoredDeviceInfo

        /// <summary>
        /// IMonitoredDeviceInfo
        /// </summary>
        private void Item_StatusChanged(object sender, DeviceStatusKind status) => this.OnStatusChanged();

        #endregion

        #region メソッド

        /// <summary>
        /// ステータス変化イベント通知
        /// </summary>
        protected void OnStatusChanged() => this.StatusChanged?.Invoke(this, this.Status);

        #endregion
    }
}