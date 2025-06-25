using System.Runtime.InteropServices;

namespace Hutzper.Library.Common.Windows
{
    /// <summary>
    /// WinAPIラッパー
    /// </summary>
    [Serializable]
    public class WinSystemController
    {
        #region サブクラス

        /// <summary>
        /// 終了オプション
        /// </summary>
        [Serializable]
        public enum ExitOption
        {
            LogOff,     //  ログオフ
            Reboot,     //  リブート
            Shutdown,   //  シャットダウン
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// シャットダウンします。
        /// </summary>
        public static void Shutdown()
        {
            /// メンバメソッドをコールします
            exit(ExitOption.Shutdown);
        }

        /// <summary>
        /// リブートします。
        /// </summary>
        public static void Reboot()
        {
            /// メンバメソッドをコールします
            exit(ExitOption.Reboot);
        }

        /// <summary>
        /// ログオフします。
        /// </summary>
        public static void LogOff()
        {
            /// メンバメソッドをコールします
            exit(ExitOption.LogOff);
        }

        /// <summary>
        /// アプリケーションまたはデバイスドライバの最小タイマ分解能を設定します。
        /// </summary>
        public static void TimeBeginPeriod()
        {
            if (0 != WinSystemController.timeGetDevCaps(out TimeCaps ptc, (uint)Marshal.SizeOf(typeof(TimeCaps))))
            {
                throw new Exception("タイマデバイスの分解能を取得できません。");
            }

            uint period = System.Math.Max(1U, ptc.PeriodMin);
            if (0 != WinSystemController.timeBeginPeriod(period))
            {
                throw new Exception("最小タイマ分解能を設定できません。");
            }

            WinSystemController.uPeriod = period;
        }

        /// <summary>
        /// 以前にセットされた最小タイマ分解能をクリアします。
        /// </summary>
        public static void TimeEndPeriod()
        {
            if (uint.MaxValue == WinSystemController.uPeriod)
            {
                throw new Exception("最小タイマ分解能を設定していません。");
            }
            if (0 != WinSystemController.timeEndPeriod(WinSystemController.uPeriod))
            {
                throw new Exception("最小タイマ分解能をクリアできません。");
            }
        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// システムを終了する
        /// </summary>
        /// <param name="option">終了オプション</param>
        /// <remarks>ログオフ, 再起動, シャットダウンを行ないます。</remarks>
        private static void exit(ExitOption option)
        {
            /// 宣言：トークンのハンドル
            var tokenHandle = IntPtr.Zero;

            /// WinAPIのラッパーをコールしてハンドルを取得
            WinSystemController.OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle);

            /// 宣言：LUID
            long luid = 0;

            /// WinAPIのラッパーをコールしてLUIDを取得
            WinSystemController.LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref luid);

            /// 宣言、初期化：特権情報構造体
            TOKEN_PRIVILEGES tokenPrivileges;
            tokenPrivileges.PrivilegeCount = 1;
            tokenPrivileges.Privilege.Luid = luid;
            tokenPrivileges.Privilege.Attributes = SE_PRIVILEGE_ENABLED;

            /// WinAPIのラッパーをコールしてトークンの特権を変更
            WinSystemController.AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);

            #region 終了処理

            /// 宣言：終了方法を決める列挙型
            ExitWindowExFlags flag;
            switch (option)
            {
                case ExitOption.LogOff:
                    flag = ExitWindowExFlags.EWX_LOGOFF;
                    break;
                case ExitOption.Reboot:
                    flag = ExitWindowExFlags.EWX_REBOOT;
                    break;
                default:
                    flag = ExitWindowExFlags.EWX_POWEROFF;
                    break;
            }

            /// WinAPIのラッパーをコールして終了
            WinSystemController.ExitWindowsEx(flag | ExitWindowExFlags.EWX_FORCE, 0);

            #endregion
        }

        #endregion

        #region スタティックフィールド

        /// <summary>
        /// 最小タイマ分解能設定値
        /// </summary>
        private static uint uPeriod = uint.MaxValue;

        #endregion

        #region WinAPIラッパー

        #region GetCurrentProcess:現在のプロセスに対する疑似ハンドルを返します。

        /// <summary>
        /// 現在のプロセスに対する疑似ハンドルを返します
        /// </summary>
        /// <returns>トークンのハンドル</returns>
        [DllImport("kernel32")]
        private static extern IntPtr GetCurrentProcess();

        #endregion

        #region OpenProcessToken:プロセスに関連付けられたアクセス トークンをオープンします。

        /// <summary>
        /// プロセスに関連付けられたアクセス トークンをオープンします
        /// </summary>
        /// <param name="processHandle">プロセスハンドル</param>
        /// <param name="desiredAccess">希望するアクセス権</param>
        /// <param name="tokenHandle">トークンのハンドル</param>
        /// <returns>0:失敗 それ以外:成功</returns>
        [DllImport("advapi32")]
        private static extern int OpenProcessToken(IntPtr processHandle, uint desiredAccess, ref IntPtr tokenHandle);

        /// <summary>
        /// アクセス権を表す文字列
        /// </summary>
        private const uint TOKEN_ADJUST_PRIVILEGES = 0x20;

        /// <summary>
        /// アクセス権を表す文字列
        /// </summary>
        private const uint TOKEN_QUERY = 0x08;

        #endregion

        #region LookupPrivilegeValue:指定されたシステムで指定された特権名を示すために使われるローカルに一意な識別子 (LUID) を取得します。

        /// <summary>
        /// LookupPrivilegeValue:指定されたシステムで指定された特権名を示すために使われるローカルに一意な識別子 (LUID) を取得します
        /// </summary>
        /// <param name="systemName">システムを指定する文字列のアドレス</param>
        /// <param name="name">特権を指定する文字列のアドレス</param>
        /// <param name="luid">ローカル一意識別子のアドレス</param>
        /// <returns>0:失敗 、0以外:成功</returns>
        [DllImport("advapi32", EntryPoint = "LookupPrivilegeValueA")]
        private static extern int LookupPrivilegeValue(string? systemName, string name, ref long luid);

        /// <summary>
        /// シャットダウンを表す文字列
        /// </summary>
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        #endregion

        #region AdjustTokenPrivileges:指定されたアクセス トークンの特権を調整します。

        /// <summary>
        /// 構造体：特権情報
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LUID_AND_ATTRIBUTES
        {
            public long Luid;
            public uint Attributes;
        }

        /// <summary>
        /// 構造体：特権情報
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privilege;
        }

        /// <summary>
        /// 指定されたアクセス トークンの特権を変更します。
        /// </summary>
        /// <param name="tokenHandle">特権を保持するトークンのハンドル</param>
        /// <param name="disableAllPrivilege">すべての特権を無効にするためのフラグ</param>
        /// <param name="newState">新しい特権情報へのポインタ</param>
        /// <param name="bufferLength">バッファのバイト単位のサイズ</param>
        /// <param name="previousState">変更を加えられた特権の元の状態を受け取る</param>
        /// <param name="returnLength">PreviousState バッファが必要とするサイズを受け取る</param>
        /// <returns>0：失敗 それ以外：成功</returns>
        [DllImport("advapi32")]
        private static extern int AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivilege, ref TOKEN_PRIVILEGES newState, uint bufferLength, IntPtr previousState, IntPtr returnLength);

        private const uint SE_PRIVILEGE_ENABLED = 0x2;

        #endregion

        #region ExitWindowsEx:システムをログオフまたはシャットダウンします。

        /// <summary>
        /// 終了方法を決める列挙型
        /// </summary>
        [Flags]
        internal enum ExitWindowExFlags : uint
        {
            EWX_LOGOFF = 0,
            EWX_SHUTDOWN = 1,
            EWX_REBOOT = 2,
            EWX_FORCE = 4,
            EWX_POWEROFF = 8
        }

        /// <summary>
        /// システムをログオフまたはシャットダウンします
        /// </summary>
        /// <param name="flags">シャットダウン操作</param>
        /// <param name="reserved">無視されるパラメータ</param>
        /// <returns>0：失敗 それ以外：成功</returns>
        [DllImport("user32")]
        private static extern int ExitWindowsEx(ExitWindowExFlags flags, uint reserved);

        #endregion

        #region タイマデバイスを照会して、分解能を調べます。

        /// <summary>
        /// 構造体：タイマの分解能に関する情報を格納します。
        /// </summary>        
        [StructLayout(LayoutKind.Sequential)]
        private struct TimeCaps
        {
            public uint PeriodMin;  // タイマがサポートする最小間隔 (ミリ秒単位) です。
            public uint PeriodMax;  // タイマがサポートする最大間隔 (ミリ秒単位) です。
        }

        /// <summary>
        /// タイマデバイスを照会して、分解能を調べます。
        /// </summary>
        /// <param name="ptc">TIMECAPS 構造体のアドレスを指定します。この構造体には、タイマデバイスの分解能に関する情報が入ります。</param>
        /// <param name="cbtc">TIMECAPS 構造体のサイズをバイト単位で指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。関数が失敗すると TIMERR_STRUCT が返り、タイマデバイスの性能が返ります。</returns>
		[DllImport("winmm.dll", EntryPoint = "timeGetDevCaps")]
        private static extern uint timeGetDevCaps(out TimeCaps ptc, uint cbtc);

        #endregion

        #region アプリケーションまたはデバイスドライバの最小タイマ分解能を設定します。

        /// <summary>
        /// アプリケーションまたはデバイスドライバの最小タイマ分解能を設定します。
        /// </summary>
        /// <param name="uPeriod">アプリケーションまたはデバイスドライバの最小タイマ分解能を、ミリ秒単位で指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。uPeriod パラメータで指定した分解能が範囲外の場合、TIMERR_NOCANDO が返ります。</returns>
        /// <remarks>タイマサービスの使用直前にこの関数を呼び出し、タイマサービスの使用終了後ただちに timeEndPeriod 関数を呼び出してください。両方の呼び出しで同じ最小分解能を指定し、timeBeginPeriod 関数の呼び出しと、timeEndPeriod 関数の呼び出しを一致させてください。アプリケーションは、timeEndPeriod 関数の呼び出しと一致している限り、何度でも timeBeginPeriod 関数を呼び出すことができます。</remarks>
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        #endregion

        #region 以前にセットされた最小タイマ分解能をクリアします。

        /// <summary>
        /// 以前にセットされた最小タイマ分解能をクリアします。
        /// </summary>
        /// <param name="uPeriod">timeBeginPeriod 関数への以前の呼び出しで指定された最小タイマ分解能を指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。uPeriod パラメータで指定した分解能が範囲外の場合、TIMERR_NOCANDO が返ります。</returns>
        /// <remarks>タイマサービスの使用終了後ただちにこの関数を呼び出してください。両方の呼び出しで同じ最小分解能を指定し、timeBeginPeriod 関数の呼び出しと、timeEndPeriod 関数の呼び出しを一致させてください。アプリケーションは、timeEndPeriod 関数の呼び出しと一致している限り、何度でも timeBeginPeriod 関数を呼び出すことができます。</remarks>
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint timeEndPeriod(uint uPeriod);

        #endregion

        #endregion
    }
}