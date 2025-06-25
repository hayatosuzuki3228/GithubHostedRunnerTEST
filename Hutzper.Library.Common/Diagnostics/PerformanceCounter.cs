using System.Diagnostics;

namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// パフォーマンスカウンタ
    /// </summary>
    /// <remarks>メモリ使用量の監視などに使用します。</remarks>
    [Serializable]
    public class PerformanceCounter : SafelyDisposable, ILoggable
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
        /// 有効かどうか
        /// </summary>
        public bool Enabled { get; protected set; } = false;

        /// <summary>
        /// パフォーマンス カウンタと、それに関連付けられているカテゴリが存在するコンピュータ
        /// </summary>
        public string MachineName { get; protected set; } = ".";

        /// <summary>
        /// このパフォーマンス カウンタが関連付けられているパフォーマンス カウンタ カテゴリ (パフォーマンス オブジェクト) の名前
        /// </summary>
        public string CategoryName { get; protected set; } = string.Empty;

        /// <summary>
        /// パフォーマンス カウンタの名前
        /// </summary>
        public string CounterName { get; protected set; } = string.Empty;

        /// <summary>
        /// パフォーマンス カウンタ カテゴリ インスタンスの名前
        /// </summary>
        public string InstanceName { get; protected set; } = string.Empty;

        /// <summary>
        /// プロセス名
        /// </summary>
        public string ProcessName { get; protected set; } = string.Empty;

        /// <summary>
        /// インスタンスを特定するためのPID
        /// </summary>
        public int ProcessId { get; protected set; } = -1;

        #endregion

        #region フィールド

        /// <summary>
        /// パフォーマンスカウンター
        /// </summary>
        protected System.Diagnostics.PerformanceCounter? currentCounter;

        /// <summary>
        /// 同種のプロセスIDリスト
        /// </summary>
        protected List<int> listSimilarProcessId = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PerformanceCounter()
        {
            this.Nickname = "PerformanceCounter";
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
                this.currentCounter?.Close();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Enabled = false;
                this.currentCounter = null;
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
        /// 初期化
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public bool Initialize(
                                    string categoryName
                                  , string counterName
                                  )
        {
            return this.Initialize(
                                     categoryName
                                   , counterName
                                   , System.Diagnostics.Process.GetCurrentProcess().Id
                                   );
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public bool Initialize(
                                    string categoryName
                                  , string counterName
                                  , int specifiedpId
                                  )
        {
            #region 既存インスタンス破棄
            try
            {
                this.currentCounter?.Close();
            }
            catch
            {
            }
            finally
            {
                this.currentCounter = null;
            }
            #endregion

            // プロパティの設定
            this.Enabled = false;
            this.MachineName = ".";
            this.CategoryName = categoryName;
            this.CounterName = counterName;
            this.InstanceName = string.Empty;
            this.ProcessName = string.Empty;
            this.ProcessId = specifiedpId;

            try
            {
                // 現在のプロセス情報を取得する
                this.listSimilarProcessId.Clear();
                this.ProcessName = System.Diagnostics.Process.GetProcessById(this.ProcessId).ProcessName;
                foreach (var selectedProcess in System.Diagnostics.Process.GetProcessesByName(this.ProcessName))
                {
                    this.listSimilarProcessId.Add(selectedProcess.Id);
                }

                // カテゴリが存在するか確かめる
                if (true == System.Diagnostics.PerformanceCounterCategory.Exists(this.CategoryName, this.MachineName))
                {
                    // カウンタが存在するか確かめる
                    if (true == System.Diagnostics.PerformanceCounterCategory.CounterExists(this.CounterName, this.CategoryName, this.MachineName))
                    {
                        // 現在のインスタンス名を取得する
                        this.InstanceName = PerformanceCounter.LookupInstanceNamefromProcessId(this.ProcessId, this.MachineName, this.ProcessName);

                        // パフォーマンスカウンタのインスタンスを生成する
                        this.currentCounter = new System.Diagnostics.PerformanceCounter(
                                                                      this.CategoryName
                                                                    , this.CounterName
                                                                    , this.InstanceName
                                                                    , this.MachineName
                                                                    );

                        // 使用可能とする
                        this.Enabled = true;

                        Serilog.Log.Verbose($"create instance (ProcessID:{this.ProcessId}, {this.GetFullName()})");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                this.listSimilarProcessId.Clear();
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// カウンタ値を取得する
        /// </summary>
        /// <returns></returns>
        public double GetValue()
        {
            var value = 0d;

            try
            {
                if (true == this.Enabled)
                {
                    var isProcessChanged = (this.currentCounter == null);

                    // 現在のプロセス情報を取得する
                    var listCurrentSimilarProcessId = new List<int>();
                    foreach (var selectedProcess in System.Diagnostics.Process.GetProcessesByName(this.ProcessName))
                    {
                        listCurrentSimilarProcessId.Add(selectedProcess.Id);
                    }

                    if (false == isProcessChanged)
                    {
                        // プロセスに変更があるか調べる
                        foreach (var pId in this.listSimilarProcessId)
                        {
                            // PIDに変更がある場合
                            if (false == listCurrentSimilarProcessId.Contains(pId))
                            {
                                isProcessChanged = true;
                                break;
                            }
                        }
                    }

                    // プロセスに変更がある場合
                    if (true == isProcessChanged)
                    {
                        // 現在のインスタンス名を取得する
                        var currentInstanceName = PerformanceCounter.LookupInstanceNamefromProcessId(this.ProcessId, this.MachineName, this.ProcessName);

                        // インスタンス名が変更されている場合
                        if (this.InstanceName != currentInstanceName)
                        {
                            #region 既存インスタンス破棄
                            try
                            {
                                this.currentCounter?.Close();
                            }
                            catch
                            {
                            }
                            finally
                            {
                                this.currentCounter = null;
                            }
                            #endregion

                            // 現在のインスタンス名を設定する
                            this.InstanceName = currentInstanceName;

                            // パフォーマンスカウンタのインスタンスを生成する                        
                            this.currentCounter = new System.Diagnostics.PerformanceCounter(
                                                                          this.CategoryName
                                                                        , this.CounterName
                                                                        , this.InstanceName
                                                                        , this.MachineName
                                                                        );
                        }
                        // プロセス情報を更新する
                        this.listSimilarProcessId = listCurrentSimilarProcessId;

                        Serilog.Log.Verbose($"change instance (ProcessID:{this.ProcessId}, {this.GetFullName()})");
                    }

                    // カウンタ値を取得する
                    value = this.currentCounter?.NextValue() ?? 0d;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);

                #region 既存インスタンス破棄
                try
                {
                    this.currentCounter?.Close();
                }
                catch
                {
                }
                finally
                {
                    this.currentCounter = null;
                }
                #endregion
            }

            return value;
        }

        /// <summary>
        /// カウンタ名を取得する
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            string? name;

            try
            {
                name = string.Format("\\\\{0}\\{1}({2})\\{3}", this.MachineName, this.CategoryName, this.InstanceName, this.CounterName);
            }
            catch (Exception ex)
            {
                name = string.Empty;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return name;
        }

        #endregion

        #region スタティックメソッド

        /// <summary>
        /// PIDからインスタンス名を取得する
        /// </summary>
        /// <param name="specifiedpId"></param>
        /// <param name="machineName"></param>
        /// <returns>インスタンス名</returns>
        public static string LookupInstanceNamefromProcessId(long specifiedpId, string machineName, string? processName = null)
        {
            var selectedName = string.Empty;

            try
            {
                var categoly = new PerformanceCounterCategory("Process", machineName);

                if (true == categoly.CounterExists("ID Process"))
                {
                    var instanceNames = categoly.GetInstanceNames();
                    if (false == string.IsNullOrEmpty(processName))
                    {
                        // すべてのプロセスでPerformanceCounterを作成すると処理時間が長いため、事前に絞り込む
                        // ※プロセス名は"プロセス名[#N]"または"プロセス名_PID"(レジストリ設定による)となっている
                        instanceNames = instanceNames.Where(s => s.StartsWith(processName)).ToArray();
                    }

                    foreach (string name in instanceNames)
                    {
                        try
                        {
                            if (categoly.InstanceExists(name))
                            {
                                using (var counter = new System.Diagnostics.PerformanceCounter("Process", "ID Process", name, machineName))
                                {
                                    if (specifiedpId == counter.RawValue)
                                    {
                                        selectedName = name;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return selectedName;
        }

        #endregion
    }
}