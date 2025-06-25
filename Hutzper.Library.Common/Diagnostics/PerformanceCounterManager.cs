namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// パフォーマンスカウンタ管理
    /// </summary>
    [Serializable]
    public class PerformanceCounterManager : SafelyDisposable, ILoggable
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// カウンタリスト
        /// </summary>
        public List<PerformanceCounter> Items
        {
            #region 取得
            get
            {
                return new List<PerformanceCounter>(this.items);
            }
            #endregion
        }

        /// <summary>
        /// 登録されているカウンタ数
        /// </summary>
        public int Count
        {
            #region 取得
            get
            {
                return this.items.Count;
            }
            #endregion
        }

        #endregion

        #region フィールド

        /// <summary>
        /// カウンタリスト
        /// </summary>
        protected List<PerformanceCounter> items = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// パフォーマンスカウンタリスト
        /// </summary>
        public PerformanceCounterManager()
        {
            // 基本プロパティを設定する
            this.Nickname = "PerformanceCounterManager";
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Clear();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.UniqueName;
        }

        /// <summary>
        /// クリア
        /// </summary>
        /// <remarks>登録済みのカウンタリストをクリアします</remarks>
        public void Clear()
        {
            this.items.Clear();
        }

        /// <summary>
        /// 利用可能な全てのパフォーマンスカウンタを追加する
        /// </summary>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddAvailableCounters()
        {
            var infoArray = PerformanceCounterUtilities.GetAvailableCounterInfoArray();

            var isSuccess = true;
            foreach (var selectedInfo in infoArray)
            {
                isSuccess &= this.AddCounter(selectedInfo);
            }

            return isSuccess;
        }

        /// <summary>
        /// .NET CLR Memoryカテゴリのパフォーマンスカウンタを追加する
        /// </summary>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCountersOfClrMemory()
        {
            var infoArray = PerformanceCounterUtilities.GetCounterInfoArrayOfClrMemory();

            var isSuccess = true;
            foreach (var selectedInfo in infoArray)
            {
                isSuccess &= this.AddCounter(selectedInfo);
            }

            return isSuccess;
        }

        /// <summary>
        /// Processカテゴリのパフォーマンスカウンタを追加する
        /// </summary>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCountersOfProcess()
        {
            var infoArray = PerformanceCounterUtilities.GetCounterInfoArrayOfProcess();

            var isSuccess = true;
            foreach (var selectedInfo in infoArray)
            {
                isSuccess &= this.AddCounter(selectedInfo);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定した文字列を含むパフォーマンスカウンタを追加する
        /// </summary>
        /// <param name="category">文字列(PvPerformanceCounterTypeの要素名の一部)</param>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCountersOf(string category)
        {
            var infoArray = PerformanceCounterUtilities.GetCounterInfoArrayOf(category);

            var isSuccess = true;
            foreach (var selectedInfo in infoArray)
            {
                isSuccess &= this.AddCounter(selectedInfo);
            }

            return isSuccess;
        }

        /// <summary>
        /// カウンタを追加する
        /// </summary>
        /// <param name="selectedType">パフォーマンスカウンタタイプ</param>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCounter(PerformanceCounterType selectedType)
        {
            return this.AddCounter(PerformanceCounterUtilities.GetCounterInfo(selectedType));
        }

        /// <summary>
        /// カウンタを追加する
        /// </summary>
        /// <param name="selectedInfo">パフォーマンスカウンタ情報</param>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCounter(PerformanceCounterInfo selectedInfo)
        {
            return this.AddCounter(selectedInfo.CategoryName, selectedInfo.CounterName);
        }

        /// <summary>
        /// カウンタを追加する
        /// </summary>
        /// <param name="categoryName">カテゴリ名</param>
        /// <param name="counterName">カウンタ名</param>
        /// <returns>正常に追加できたかどうか</returns>
        public bool AddCounter(
                                    string categoryName
                                  , string counterName
                                  )
        {
            var isSuccess = false;

            try
            {
                var newCounter = new PerformanceCounter();
                newCounter.Initialize(categoryName, counterName);

                if (true == newCounter.Enabled)
                {
                    this.items.Add(newCounter);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// カウンタ値を取得する
        /// </summary>
        /// <returns></returns>
        public double[] GetValues()
        {
            var values = new double[0];

            try
            {
                var listValue = new List<double>();

                foreach (var selectedItem in this.items)
                {
                    listValue.Add(selectedItem.GetValue());
                }

                values = listValue.ToArray();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return values;
        }

        /// <summary>
        /// カウンタ名を取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetNames()
        {
            var names = new string[0];

            try
            {
                var listName = new List<string>();

                foreach (var selectedItem in this.items)
                {
                    listName.Add(selectedItem.CounterName);
                }

                names = listName.ToArray();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return names;
        }

        /// <summary>
        /// カウンタ名を取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetFullNames()
        {
            var names = new string[0];

            try
            {
                var listName = new List<string>();

                foreach (var selectedItem in this.items)
                {
                    listName.Add(selectedItem.GetFullName());
                }

                names = listName.ToArray();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return names;
        }

        /// <summary>
        /// カウンタ値のCSV文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string GetCsvStringOfValues()
        {
            var csvString = string.Empty;

            try
            {
                var listString = new List<string>();

                foreach (var selectedItem in this.items)
                {
                    listString.Add(selectedItem.GetValue().ToString());
                }

                csvString = string.Join(",", listString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return csvString;
        }

        /// <summary>
        /// カウンタ名のCSV文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string GetCsvStringOfNames()
        {
            var csvString = string.Empty;

            try
            {
                var listString = new List<string>();

                foreach (var selectedItem in this.items)
                {
                    listString.Add(selectedItem.CounterName.ToString());
                }

                csvString = string.Join(",", listString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return csvString;
        }

        /// <summary>
        /// カウンタ名のCSV文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string GetCsvStringOfFullNames()
        {
            var csvString = string.Empty;

            try
            {
                var listString = new List<string>();

                foreach (var selectedItem in this.items)
                {
                    listString.Add(selectedItem.GetFullName().ToString());
                }

                csvString = string.Join(",", listString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return csvString;
        }

        #endregion
    }

    /// <summary>
    /// パフォーマンスカウンタタイプ
    /// </summary>
    [Serializable]
    public enum PerformanceCounterType
    {
        Undefined,

        ClrMemory_Gen0HeapSize,
        ClrMemory_Gen1HeapSize,
        ClrMemory_Gen2HeapSize,
        ClrMemory_LargeObjectHeapSize,
        ClrMemory_GCHandles,
        ClrMemory_TimeInGC,
        ClrMemory_BytesInAllHeaps,
        ClrMemory_OfPinnedObjects,

        Process_HandleCount,
        Process_PrivateBytes,
        Process_ThreadCount,
    }

    /// <summary>
    /// パフォーマンスカウンタ情報
    /// </summary>
    [Serializable]
    public class PerformanceCounterInfo
    {
        #region プロパティ

        public PerformanceCounterType CounterType { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PerformanceCounterInfo()
        : this(PerformanceCounterType.Undefined, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PerformanceCounterInfo(PerformanceCounterType counterType)
        : this(counterType, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PerformanceCounterInfo(
                                          PerformanceCounterType counterType
                                        , string categoryName
                                        , string counterName
                                        )
        {
            this.CounterType = counterType;
            this.CategoryName = categoryName;
            this.CounterName = counterName;
        }

        #endregion
    }

    /// <summary>
    /// ユーティリティ
    /// </summary>
    public static class PerformanceCounterUtilities
    {
        #region パブリックメソッド

        /// <summary>
        /// 利用可能な全てのパフォーマンスカウンタ情報の配列を取得する
        /// </summary>
        /// <returns>パフォーマンスカウンタ情報の配列</returns>
        /// <remarks>PvPerformanceCounterTypeに定義されているタイプ全て</remarks>
        public static PerformanceCounterInfo[] GetAvailableCounterInfoArray()
        {
            var listInfo = new List<PerformanceCounterInfo>();

            foreach (PerformanceCounterType selectedType in Enum.GetValues(typeof(PerformanceCounterType)))
            {
                if (PerformanceCounterType.Undefined != selectedType)
                {
                    listInfo.Add(PerformanceCounterUtilities.GetCounterInfo(selectedType));
                }
            }

            return listInfo.ToArray();
        }

        /// <summary>
        /// .NET CLR Memoryカテゴリのパフォーマンスカウンタ情報の配列を取得する
        /// </summary>
        /// <returns>パフォーマンスカウンタ情報の配列</returns>
        /// <remarks>PvPerformanceCounterTypeに定義されているタイプに限ります(全てではないことに注意)</remarks>
        public static PerformanceCounterInfo[] GetCounterInfoArrayOfClrMemory()
        {
            return PerformanceCounterUtilities.GetCounterInfoArrayOf("ClrMemory");
        }

        /// <summary>
        /// Processカテゴリのパフォーマンスカウンタ情報の配列を取得する
        /// </summary>
        /// <returns>パフォーマンスカウンタ情報の配列</returns>
        /// <remarks>PvPerformanceCounterTypeに定義されているタイプに限ります(全てではないことに注意)</remarks>
        public static PerformanceCounterInfo[] GetCounterInfoArrayOfProcess()
        {
            return PerformanceCounterUtilities.GetCounterInfoArrayOf("Process");
        }

        /// <summary>
        /// 指定した文字列を含むパフォーマンスカウンタ情報の配列を取得する
        /// </summary>
        /// <param name="category">文字列(PvPerformanceCounterTypeの要素名の一部)</param>
        /// <returns>パフォーマンスカウンタ情報の配列</returns>
        public static PerformanceCounterInfo[] GetCounterInfoArrayOf(string category)
        {
            var listInfo = new List<PerformanceCounterInfo>();

            var typeArray = PerformanceCounterUtilities.GetCounterTypeArrayOf(category);

            foreach (var selectedType in typeArray)
            {
                listInfo.Add(PerformanceCounterUtilities.GetCounterInfo(selectedType));
            }

            return listInfo.ToArray();
        }

        /// <summary>
        /// パフォーマンスカウンタ情報を取得する
        /// </summary>
        /// <param name="selectedType">パフォーマンスカウンタタイプ</param>
        /// <returns>パフォーマンスカウンタ情報</returns>
        public static PerformanceCounterInfo GetCounterInfo(PerformanceCounterType selectedType)
        {
            var info = new PerformanceCounterInfo(selectedType);

            info.CategoryName = PerformanceCounterUtilities.GetCategoryName(selectedType);
            info.CounterName = PerformanceCounterUtilities.GetCounterName(selectedType);

            return info;
        }

        /// <summary>
        /// .NET CLR Memoryカテゴリのパフォーマンスカウンタタイプの配列を取得する
        /// </summary>
        /// <returns>パフォーマンスカウンタタイプの配列</returns>
        /// <remarks>PvPerformanceCounterTypeに定義されているタイプに限ります(全てではないことに注意)</remarks>
        public static PerformanceCounterType[] GetCounterTypeArrayOfClrMemory()
        {
            return PerformanceCounterUtilities.GetCounterTypeArrayOf("ClrMemory");
        }

        /// <summary>
        /// Processカテゴリのパフォーマンスカウンタタイプの配列を取得する
        /// </summary>
        /// <returns>パフォーマンスカウンタタイプの配列</returns>
        /// <remarks>PvPerformanceCounterTypeに定義されているタイプに限ります(全てではないことに注意)</remarks>
        public static PerformanceCounterType[] GetCounterTypeArrayOfProcess()
        {
            return PerformanceCounterUtilities.GetCounterTypeArrayOf("Process");
        }

        /// <summary>
        /// 指定した文字列を含むパフォーマンスカウンタタイプの配列を取得する
        /// </summary>
        /// <param name="category">文字列(PvPerformanceCounterTypeの要素名の一部)</param>
        /// <returns>パフォーマンスカウンタタイプの配列</returns>
        public static PerformanceCounterType[] GetCounterTypeArrayOf(string category)
        {
            var listType = new List<PerformanceCounterType>();

            if (false == string.IsNullOrEmpty(category))
            {
                foreach (PerformanceCounterType selectedType in Enum.GetValues(typeof(PerformanceCounterType)))
                {
                    var typeString = selectedType.ToString();

                    if (0 == typeString.IndexOf(category))
                    {
                        listType.Add(selectedType);
                    }
                }
            }

            return listType.ToArray();
        }

        /// <summary>
        /// カテゴリ名を取得する
        /// </summary>
        /// <param name="selectedType">パフォーマンスカウンタタイプ</param>
        /// <returns>カテゴリ名</returns>
        public static string GetCategoryName(PerformanceCounterType selectedType)
        {
            var name = string.Empty;

            switch (selectedType)
            {
                case PerformanceCounterType.ClrMemory_Gen0HeapSize: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_Gen1HeapSize: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_Gen2HeapSize: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_LargeObjectHeapSize: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_GCHandles: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_TimeInGC: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_BytesInAllHeaps: name = ".NET CLR Memory"; break;
                case PerformanceCounterType.ClrMemory_OfPinnedObjects: name = ".NET CLR Memory"; break;

                case PerformanceCounterType.Process_HandleCount: name = "Process"; break;
                case PerformanceCounterType.Process_PrivateBytes: name = "Process"; break;
                case PerformanceCounterType.Process_ThreadCount: name = "Process"; break;
            }

            return name;
        }

        /// <summary>
        /// カウンタ名を取得する
        /// </summary>
        /// <param name="selectedType">パフォーマンスカウンタタイプ</param>
        /// <returns>カウンタ名</returns>
        public static string GetCounterName(PerformanceCounterType selectedType)
        {
            var name = string.Empty;

            switch (selectedType)
            {
                case PerformanceCounterType.ClrMemory_Gen0HeapSize: name = "Gen 0 heap size"; break;
                case PerformanceCounterType.ClrMemory_Gen1HeapSize: name = "Gen 1 heap size"; break;
                case PerformanceCounterType.ClrMemory_Gen2HeapSize: name = "Gen 2 heap size"; break;
                case PerformanceCounterType.ClrMemory_LargeObjectHeapSize: name = "Large Object Heap size"; break;
                case PerformanceCounterType.ClrMemory_GCHandles: name = "# GC Handles"; break;
                case PerformanceCounterType.ClrMemory_TimeInGC: name = "% Time in GC"; break;
                case PerformanceCounterType.ClrMemory_BytesInAllHeaps: name = "# Bytes in all Heaps"; break;
                case PerformanceCounterType.ClrMemory_OfPinnedObjects: name = "# of Pinned Objects"; break;

                case PerformanceCounterType.Process_HandleCount: name = "Handle Count"; break;
                case PerformanceCounterType.Process_PrivateBytes: name = "Private Bytes"; break;
                case PerformanceCounterType.Process_ThreadCount: name = "Thread Count"; break;
            }

            return name;
        }

        #endregion
    }
}