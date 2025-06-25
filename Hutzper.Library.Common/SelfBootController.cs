namespace Hutzper.Library.Common
{
    // 自己起動制御
    [Serializable]
    public class SelfBootController
    {
        #region プロパティ

        /// <summary>
        /// 多重起動をチェックし、アプリケーションの起動の許可・不許可を取得します。
        /// </summary>
        /// <returns>true:起動可 false:起動不可</returns>
        public bool CanBootApplication
        {
            #region 値の取得
            get
            {
                /// 戻り値を起動不許可で初期化する
                bool canBoot = false;

                /// 多重起動でない場合
                if (true == this.mutex.WaitOne(0, false))
                {
                    //// 戻り値を起動許可とする
                    canBoot = true;
                }
                /// 再起動要求の場合
                else if (true == this.bootByRebootQueue)
                {
                    //// タイムアウト時間内に再起動要求元プロセスが終了した場合
                    if (true == this.mutex.WaitOne(this.rebootTimeout, false))
                    {
                        ///// 戻り値を起動許可とする
                        canBoot = true;
                    }
                }

                /// 戻り値を返す
			    return canBoot;
            }
            #endregion
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 再起動要求のコマンドラインパラメータ
        /// </summary>
        private static readonly string rebootParameter = "/wait_prev_end";

        /// <summary>
        /// 再起動のデフォルトタイムアウト時間(ミリ秒)
        /// </summary>
        private static readonly int defaultRebootTimeout = 60 * 1000;

        /// <summary>
        /// 再起動のタイムアウト時間(ミリ秒)
        /// </summary>
        private readonly int rebootTimeout;

        /// <summary>
        /// 再起動要求有無フラグ
        /// </summary>
        private readonly bool bootByRebootQueue;

        /// <summary>
        /// 多重起動抑止用Mutexオブジェクト
        /// </summary>
        private readonly System.Threading.Mutex mutex;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="applicationUniqueString">アプリケーションを識別する文字列</param>
        /// <remarks>再起動のタイムアウト時間はデフォルト値に設定されます</remarks>
        public SelfBootController(string applicationUniqueString)
        : this(applicationUniqueString, SelfBootController.defaultRebootTimeout) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="applicationUniqueString">アプリケーションを識別する文字列</param>
        /// <param name="rebootTimeout">再起動のタイムアウト時間</param>
        public SelfBootController(string applicationUniqueString, int rebootTimeout)
        {
            #region フィールドの初期化
            {
                /// フィールドの初期化を行う
                this.bootByRebootQueue = false;
                this.rebootTimeout = rebootTimeout;
                this.mutex = new System.Threading.Mutex(false, applicationUniqueString);
            }
            #endregion

            #region コマンドラインパラメータ解析
            {
                /// コマンドラインパラメータの解析を行う
                foreach (string command in System.Environment.GetCommandLineArgs())
                {
                    //// 再起動要求の場合
                    if (0 == string.Compare(command, SelfBootController.rebootParameter, true))
                    {
                        ///// 再起動要求フラグを立てる
                        this.bootByRebootQueue = true;
                    }
                }
            }
            #endregion
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アプリケーション終了時の処理です。
        /// </summary>
        public void NotifyExitApplication()
        {
            /// 多重起動抑止用Mutexオブジェクトを解放
            this.mutex.ReleaseMutex();
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// アプリケーションの再起動を実行します。
        /// </summary>
        public static void RebootApplication()
        {
            /// 再起動パラメータを渡してアプリケーションを起動する
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath, SelfBootController.rebootParameter);

            /// このアプリケーションを終了する
            System.Windows.Forms.Application.Exit();
        }

        #endregion
    }
}