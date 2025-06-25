using System.Diagnostics;

namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// ストレージチェック
    /// </summary>
    public class StorageStatusChecker : IDeviceStatusChecker
    {
        #region プロパティ

        /// <summary>
        /// 最終チェック結果
        /// </summary>
        public List<IDeviceStatusCheckResult> LatestStatus
        {
            get
            {
                var clone = new List<IDeviceStatusCheckResult>();

                lock (this.syncRoot)
                {
                    foreach (var s in this.latestStatus)
                    {
                        var copied = new StorageStatusCheckResult();
                        s.DeepCopyTo(copied);
                        clone.Add(copied);
                    }
                }

                return clone ?? new List<IDeviceStatusCheckResult>();
            }
        }

        #endregion

        #region フィールド

        private readonly object syncRoot = new();

        private List<IDeviceStatusCheckResult> latestStatus = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StorageStatusChecker()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="parameter"></param>
        public void Initialize(IDeviceStatusCheckParameter parameter)
        {
            var myParam = (StorageStatusCheckParameter)parameter;

            if (null != myParam.DirectoryPath)
            {
                var tempList = new List<IDeviceStatusCheckResult>();

                foreach (var selectedPath in myParam.DirectoryPath)
                {
                    tempList.Add(new StorageStatusCheckResult(selectedPath));
                }

                lock (this.syncRoot)
                {
                    this.latestStatus = tempList;
                }
            }
        }

        /// <summary>
        /// チェック
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<IDeviceStatusCheckResult> Check()
        {
            var checkList = this.LatestStatus;

            foreach (var target in checkList.Cast<StorageStatusCheckResult>())
            {
                try
                {
                    target.IsNormal = false;

                    if (null == target.DirectoryInfo)
                    {
                        throw new Exception("DirectoryInfo is null");
                    }

                    var canContinue = true;

                    #region ディレクトリ確認
                    if (true == canContinue)
                    {
                        canContinue = target.DirectoryInfo.Exists;
                    }
                    #endregion

                    #region ファイルアクセステスト
                    if (true == canContinue)
                    {
                        var writeText = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        var readText = string.Empty;
                        var testFileInfo = new FileInfo(System.IO.Path.Combine(target.DirectoryInfo.FullName, "TEST.txt"));

                        #region ファイル書込テスト
                        if (true == canContinue)
                        {
                            try
                            {
                                using (var writer = new StreamWriter(new FileStream(testFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Read)))
                                {
                                    writer.WriteLine(writeText);
                                }

                                testFileInfo.Refresh();
                                canContinue = testFileInfo.Exists;
                            }
                            catch (Exception ex)
                            {
                                canContinue = false;
                                Debug.WriteLine(this, ex.ToString());
                            }
                        }
                        #endregion

                        #region ファイル読込テスト
                        if (true == canContinue)
                        {
                            try
                            {
                                using (var reader = new StreamReader(new FileStream(testFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                                {
                                    readText = reader.ReadLine();
                                }

                                canContinue = readText?.Equals(writeText) ?? false;
                            }
                            catch (Exception ex)
                            {
                                canContinue = false;
                                Debug.WriteLine(this, ex.ToString());
                            }
                        }
                        #endregion

                        #region ファイル削除テスト
                        if (true == canContinue)
                        {
                            try
                            {
                                testFileInfo.Delete();
                                testFileInfo.Refresh();

                                canContinue = (false == testFileInfo.Exists);
                            }
                            catch (Exception ex)
                            {
                                canContinue = false;
                                Debug.WriteLine(this, ex.ToString());
                            }
                        }
                        #endregion
                    }
                    #endregion

                    // 最終結果
                    target.IsNormal = canContinue;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.ToString());
                }
                finally
                {
                    target.LastUpdateTime = DateTime.Now;
                    target.IsChecked = true;
                }
            }

            lock (this.syncRoot)
            {
                this.latestStatus = new List<IDeviceStatusCheckResult>();
                foreach (var s in checkList)
                {
                    var copied = new StorageStatusCheckResult();
                    s.DeepCopyTo(copied);
                    this.latestStatus.Add(copied);
                }
            }

            return checkList;
        }

        #endregion
    }

    /// <summary>
    /// ストレージステータスチェックパラメータ
    /// </summary>
    [Serializable]
    public class StorageStatusCheckParameter : IDeviceStatusCheckParameter
    {
        /// <summary>
        /// ディレクトリパスリスト
        /// </summary>
        public List<string> DirectoryPath { get; set; } = new();
    }

    /// <summary>
    /// ストレージチェックステータス
    /// </summary>
    [Serializable]
    public record StorageStatusCheckResult : DeviceStatusCheckResult
    {
        #region プロパティ

        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        public DirectoryInfo? DirectoryInfo { get; protected set; }

        /// <summary>
        /// ドライブ情報
        /// </summary>
        public DriveInfo? DriveInfo { get; protected set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StorageStatusCheckResult() : this(string.Empty)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path"></param>
        public StorageStatusCheckResult(string path)
        {
            this.DirectoryInfo = null;
            this.DriveInfo = null;

            if (false == string.IsNullOrEmpty(path))
            {
                this.DirectoryInfo = new DirectoryInfo(path);

                var drive = System.IO.Path.GetPathRoot(path);

                if (false == string.IsNullOrEmpty(drive))
                {
                    this.DriveInfo = new DriveInfo(drive);
                }
            }
        }

        #endregion

        /// <summary>
        /// ディープコピー
        /// </summary>
        /// <param name="dest"></param>
        public override void DeepCopyTo(IDeviceStatusCheckResult dest)
        {
            base.DeepCopyTo(dest);

            if (dest is StorageStatusCheckResult destS)
            {
                if (this.DirectoryInfo is not null)
                {
                    destS.DirectoryInfo = new DirectoryInfo(this.DirectoryInfo.FullName);

                    var drive = System.IO.Path.GetPathRoot(destS.DirectoryInfo.FullName);

                    if (false == string.IsNullOrEmpty(drive))
                    {
                        this.DriveInfo = new DriveInfo(drive);
                    }
                }
            }
        }
    }
}