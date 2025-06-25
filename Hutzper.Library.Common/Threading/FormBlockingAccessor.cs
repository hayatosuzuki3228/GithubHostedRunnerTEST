using Hutzper.Library.Common.Forms;
using System.Diagnostics;

namespace Hutzper.Library.Common.Threading
{
    /// <summary>
    /// スレッドセーフなダイアログ呼び出し
    /// </summary>
    public class FormBlockingAccessor
    {
        #region プロパティ

        /// <summary>
        /// 汎用プロパティ
        /// </summary>
        /// <remarks>BlockingDialogControllerクラスはこのプロパティにアクセスしません</remarks>
        public object? Tag { get; set; }

        /// <summary>
        /// ShowDialogを行うフォーム
        /// </summary>
        public readonly Form Dialog;

        #endregion

        #region フィールド

        /// <summary>
        /// ダイアログ表示待機イベント
        /// ShowBlockingのタイミングによっては，ShowDialogが終了しないため，ShowBlockingを待ってからSetされるようにする
        /// </summary>
        private readonly AutoResetEvent waitEvent;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dialog">ShowDialogを行うフォーム</param>
        public FormBlockingAccessor(Form dialog)
        {
            // プロパティを引数で初期化する
            this.Dialog = dialog;

            // 待機イベントを生成する
            this.waitEvent = new AutoResetEvent(this.Dialog.IsHandleCreated);

            // イベント登録
            void eventHandler(object? s, EventArgs e)
            {
                try
                {
                    this.waitEvent.Set();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    dialog.Shown -= eventHandler;
                }
            }

            dialog.Shown += eventHandler;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ShowDialogメソッドの同期呼び出し
        /// </summary>
        /// <param name="readyCallback">ShowDialogの前に行う処理</param>
        /// <returns>ダイアログ戻り値</returns>
        public DialogResult ShowBlocking(MethodInvoker? readyCallback = null)
        {
            // 戻り値を初期化する
            var result = DialogResult.None;

            // 同期フォーム設定
            var invokeForm = this.Dialog.Owner ?? Application.OpenForms[0];

            // 同期処理
            void synchronizedProcess()
            {
                try
                {
                    // 前処理コールバックを呼び出す
                    readyCallback?.Invoke();

                    // ダイアログをモーダルで表示する
                    result = this.Dialog.ShowDialog(invokeForm);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            invokeForm?.InvokeSafely(synchronizedProcess);

            // ダイアログ戻り値を返す
            return (result);
        }

        /// <summary>
        /// ダイアログ戻り値を設定する
        /// </summary>
        /// <param name="result">設定するダイアログ戻り値</param>
        public void Set(DialogResult result)
        {
            // ダイアログが表示されるまで待機(30秒でタイムアウトとする)
            if (false == this.waitEvent.WaitOne(30000))
            {
                Console.WriteLine(this.ToString() + ":" + "Wait event timeout");
            }

            // 同期フォーム設定
            var invokeForm = this.Dialog.Owner ?? Application.OpenForms[0];

            // ダイアログ戻り値を設定する
            void synchronizedProcess()
            {
                try
                {
                    this.Dialog.DialogResult = result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            invokeForm?.InvokeSafely(synchronizedProcess);
        }

        /// <summary>
        /// 同期処理
        /// </summary>
        /// <param name="callback">コールバックデリゲート</param>
        /// <param name="state">コールバックデリゲート引数</param>
        /// <remarks>メインスレッドに同期して行う処理を呼び出します</remarks>
        public void Access(Action<object> callback, object state)
        {
            // 同期フォーム設定
            var invokeForm = this.Dialog.Owner ?? Application.OpenForms[0];

            // コールバックを実行する
            void synchronizedProcess()
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            invokeForm?.InvokeSafely(synchronizedProcess);
        }

        #endregion
    }
}