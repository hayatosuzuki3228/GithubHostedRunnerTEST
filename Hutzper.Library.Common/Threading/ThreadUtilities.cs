using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Common.Threading
{
    /// <summary>
    /// スレッドユーティリティ
    /// </summary>
    [Serializable]
    public static class ThreadUtilities
    {
        #region メソッド

        /// <summary>
        /// スレッドに割り当てたプロセッサ指定を解除する
        /// </summary>
        /// <returns>true:正常 false:異常</returns>
        public static bool SetThreadAffinityMask()
        {
            /// 処理結果を異常として初期化する
            bool isSuccess = false;

            try
            {

                //// カレントスレッドのプロセッサ指定を解除する
                UIntPtr result = ThreadUtilities.SetThreadAffinityMask(ThreadUtilities.GetCurrentThread(), new UIntPtr(0));

                //// プロセッサ指定解除処理の結果が正常の場合
                if (result.ToUInt32() != 0)
                {
                    ///// 処理結果を正常とする
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Debug.WriteLine(ex);
            }

            /// 処理結果を返す
            return isSuccess;
        }

        /// <summary>
        /// スレッドに割り当てるプロセッサを指定する
        /// </summary>
        /// <param name="targetCore">指定プロセッサ番号 0～</param>
        /// <returns>true:正常 false:異常</returns>
        public static bool SetThreadAffinityMask(uint targetProcessor)
        {
            return ThreadUtilities.SetThreadAffinityMask(new uint[] { targetProcessor });
        }

        /// <summary>
        /// スレッドに割り当てるプロセッサを指定する
        /// </summary>
        /// <param name="targetCore">指定プロセッサ番号配列 0～</param>
        /// <returns>true:正常 false:異常</returns>
        public static bool SetThreadAffinityMask(uint[] targetProcessor)
        {
            /// 処理結果を異常として初期化する
            bool isSuccess = false;

            try
            {
                uint affinityMask = 0;
                ulong validProcessorCount = 0;
                if (null != targetProcessor)
                {
                    foreach (var processor in targetProcessor)
                    {
                        /// 指定プロセッサ番号がプロセッサ数以内の場合
                        if (processor < System.Environment.ProcessorCount)
                        {
                            //// ビットマスクを更新する
                            affinityMask |= (uint)(1 << (int)processor);

                            validProcessorCount++;
                        }
                    }
                }

                /// 有効な指定プロセッサが1つ以上ある場合
                if (0 < validProcessorCount)
                {
                    //// カレントスレッドに指定プロセッサを割り当てる
                    UIntPtr result = ThreadUtilities.SetThreadAffinityMask(ThreadUtilities.GetCurrentThread(), new UIntPtr(affinityMask));

                    //// プロセッサ割り当て処理の結果が正常の場合
                    if (result.ToUInt32() != 0)
                    {
                        ///// 処理結果を正常とする
                        isSuccess = true;
                    }
                }
                /// 有効な指定プロセッサが無い場合
                else
                {
                    //// カレントスレッドのプロセッサ指定を解除する
                    isSuccess = ThreadUtilities.SetThreadAffinityMask();
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Debug.WriteLine(ex);
            }

            /// 処理結果を返す
            return isSuccess;
        }

        #endregion

        #region WinAPIラッパー

        #region SetThreadAffinityMask:指定されたスレッドのプロセッサ指定マスクを設定します。

        /// <summary>
        /// 指定されたスレッドのプロセッサ指定マスクを設定します
        /// </summary>
        [DllImport("kernel32")]
        private static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        #endregion

        #region GetCurrentThread:現在のスレッドの疑似ハンドル。

        /// <summary>
        /// 現在のスレッドの疑似ハンドル
        /// </summary>
        [DllImport("kernel32")]
        private static extern IntPtr GetCurrentThread();

        #endregion

        #endregion
    }
}