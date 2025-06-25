namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// ストレージ空き容量チェック
    /// </summary>
    [Serializable]
    public class StorageSpaceChecker
    {
        #region サブクラス

        /// <summary>
        /// 下限値の単位
        /// </summary>
        [Serializable]
        public enum LowerLimitUnit
        {
            Bytes,
            BasisPoint,
        }

        /// <summary>
        /// ストレージ空き容量取得クラス
        /// </summary>
        public class StorageSpaceItem
        {
            #region プロパティ

            /// <summary>合計サイズ</summary>
            public long TotalSize => this.driveInfo?.TotalSize ?? -1L;

            /// <summary>使用できる空き容量</summary>
            public long FreeSpace => this.driveInfo?.AvailableFreeSpace ?? 0;

            #endregion

            #region フィールド

            /// <summary>ドライブに関する情報</summary>
            private System.IO.DriveInfo? driveInfo;

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="path">ファイルまたはフォルダパス</param>
            public StorageSpaceItem(string path)
            {
                /// 指定したパスで初期化
                var root = System.IO.Path.GetPathRoot(System.IO.Path.GetFullPath(path));

                if (root is not null)
                {
                    this.driveInfo = new System.IO.DriveInfo(root);
                }
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 同じドライブであるかどうかを取得する
            /// </summary>
            /// <param name="item">比較対象</param>
            /// <returns>true:同じ false:異なる</returns>
            internal bool IsSameDrive(StorageSpaceItem item) => string.Compare(item.driveInfo?.Name ?? this.driveInfo?.Name ?? string.Empty, this.driveInfo?.Name ?? string.Empty, true) == 0;

            #endregion
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 登録されたストレージにおける最小空き容量バイト数
        /// </summary>
        public long LowerLimitBytes
        {
            get
            {
                var result = long.MaxValue;

                foreach (var item in this.items.Where(ss => ss.TotalSize > 0))
                {
                    var value = item.FreeSpace;

                    if (result > value)
                    {
                        result = value;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 登録されたストレージにおける最小空き容量bp(0.01%)
        /// </summary>
        public double LowerLimitBasisPoint
        {
            get
            {
                var result = 10000.0d;

                foreach (var item in this.items.Where(ss => ss.TotalSize > 0))
                {
                    var value = item.FreeSpace / (double)item.TotalSize * 10000.0d;

                    if (result > value)
                    {
                        result = value;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 下限値未満かどうかを取得する
        /// </summary>
        public bool IsBelowLowerLimit
        {
            get
            {
                /// 負値の場合、常に下限値以上とする
                if (this.lowerLimitValue < 0L)
                {
                    return false;
                }

                try
                {
                    switch (this.lowerLimitUnit)
                    {
                        /// バイト数の場合
                        case LowerLimitUnit.Bytes:
                            {
                                //// 登録されたストレージのうち、1つでも下限値を下回っている場合、下限値未満をreturn
                                return this.LowerLimitBytes < this.lowerLimitValue;
                            }

                        /// bp(0.01%)の場合
                        case LowerLimitUnit.BasisPoint:
                            {
                                //// 登録されたストレージのうち、1つでも下限値を下回っている場合、下限値未満をreturn

                                return this.LowerLimitBasisPoint < this.lowerLimitValue;
                            }

                        default:
                            {
                                return false;
                            }
                    }
                }
                catch
                {
                    /// 取得失敗の場合、下限値未満とする
                    return true;
                }
            }
        }

        #endregion

        #region フィールド

        /// <summary>空き容量取得クラスのリスト</summary>
        private List<StorageSpaceItem> items;

        /// <summary>下限値の単位</summary>
        private LowerLimitUnit lowerLimitUnit;

        /// <summary>下限値</summary>
        private long lowerLimitValue;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StorageSpaceChecker()
        {
            /// 初期化
            this.items = new List<StorageSpaceItem>();
            this.lowerLimitUnit = LowerLimitUnit.BasisPoint;
            this.lowerLimitValue = -1L;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// クリアする
        /// </summary>
        public void Clear()
        {
            /// クリア
            this.items.Clear();
        }

        /// <summary>
        /// 空き容量をチェックすべきパスを追加する
        /// </summary>
        /// <param name="path">ファイルまたはフォルダパス</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool AddTargetPath(string path)
        {
            /// 処理結果の初期化
            var result = false;
            if (!Directory.Exists(path))
            {
                return false;
            }
            try
            {
                var newItem = new StorageSpaceItem(path);

                /// 既に同じドライブがないかチェック
                foreach (var item in this.items)
                {
                    //// 同じドライブが追加済みの場合、追加せずに成功とする
                    if (newItem.IsSameDrive(item))
                    {
                        return true;
                    }
                }

                /// パスを追加
                this.items.Add(newItem);

                /// 処理結果の設定
                result = true;
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// 下限値を設定する
        /// </summary>
        /// <param name="borderType">下限値単位</param>
        /// <param name="borderValue">下限値(負値指定で、常にボーダー未満判定を戻す)</param>
        public void SetLowerLimit(LowerLimitUnit lowerLimitUnit, long lowerLimitValue)
        {
            /// 値を設定
            this.lowerLimitUnit = lowerLimitUnit;
            this.lowerLimitValue = lowerLimitValue;
        }

        #endregion
    }
}