using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// Fomr関連ユーティリティ
    /// </summary>
    [Serializable]
    public static class ControlUtilities
    {
        #region メソッド

        public static System.Drawing.Point GetLocation(System.Drawing.ContentAlignment texAlign, System.Drawing.Rectangle clipRectangle, System.Drawing.Size textSize)
        {
            var pointF = ControlUtilities.GetLocation(texAlign, new RectangleF(clipRectangle.Left, clipRectangle.Top, clipRectangle.Size.Width, clipRectangle.Size.Height), new SizeF(textSize.Width, textSize.Height));

            return new Point((int)pointF.X, (int)pointF.Y);
        }

        public static System.Drawing.PointF GetLocation(System.Drawing.ContentAlignment texAlign, System.Drawing.RectangleF clipRectangle, System.Drawing.SizeF textSize)
        {
            var location = new System.Drawing.PointF();

            switch (texAlign)
            {
                case System.Drawing.ContentAlignment.TopLeft:
                    {
                        location.X = clipRectangle.Left;
                        location.Y = clipRectangle.Top;
                    }
                    break;

                case System.Drawing.ContentAlignment.TopCenter:
                    {
                        location.X = clipRectangle.Left + (clipRectangle.Width - textSize.Width) / 2f;
                        location.Y = clipRectangle.Top;
                    }
                    break;

                case System.Drawing.ContentAlignment.TopRight:
                    {
                        location.X = clipRectangle.Left + clipRectangle.Width - textSize.Width;
                        location.Y = clipRectangle.Top;
                    }
                    break;

                case System.Drawing.ContentAlignment.MiddleLeft:
                    {
                        location.X = clipRectangle.Left;
                        location.Y = clipRectangle.Top + (clipRectangle.Height - textSize.Height) / 2f;
                    }
                    break;

                case System.Drawing.ContentAlignment.MiddleCenter:
                    {
                        location.X = clipRectangle.Left + (clipRectangle.Width - textSize.Width) / 2f;
                        location.Y = clipRectangle.Top + (clipRectangle.Height - textSize.Height) / 2f;
                    }
                    break;

                case System.Drawing.ContentAlignment.MiddleRight:
                    {
                        location.X = clipRectangle.Left + clipRectangle.Width - textSize.Width;
                        location.Y = clipRectangle.Top + (clipRectangle.Height - textSize.Height) / 2f;
                    }
                    break;

                case System.Drawing.ContentAlignment.BottomLeft:
                    {
                        location.X = clipRectangle.Left;
                        location.Y = clipRectangle.Top + clipRectangle.Height - textSize.Height;
                    }
                    break;

                case System.Drawing.ContentAlignment.BottomCenter:
                    {
                        location.X = clipRectangle.Left + (clipRectangle.Width - textSize.Width) / 2f;
                        location.Y = clipRectangle.Top + clipRectangle.Height - textSize.Height;
                    }
                    break;

                case System.Drawing.ContentAlignment.BottomRight:
                    {
                        location.X = clipRectangle.Left + clipRectangle.Width - textSize.Width;
                        location.Y = clipRectangle.Top + clipRectangle.Height - textSize.Height;
                    }
                    break;
            }

            return location;
        }

        /// <summary>
        /// 全てのコントロールを再帰的に取得する
        /// </summary>
        /// <param name="parent">指定のコントロール</param>
        /// <returns>全てのコントロール</returns>
        public static Control?[] GetAllControls(Control parent) => ControlUtilities.GetAllControls(parent, (Control?)null);

        /// <summary>
        /// 全てのコントロールを再帰的に取得する
        /// </summary>
        /// <param name="parent">指定のコントロール</param>        
        /// <returns>全てのコントロール</returns>
        public static T[] GetAllControls<T>(Control parent, T t) where T : Control?
        {
            var children = new ArrayList();
            if (null != parent)
            {
                foreach (Control child in parent.Controls)
                {
                    if (child is T)
                    {
                        children.Add(child);
                    }
                    children.AddRange(ControlUtilities.GetAllControls(child, t));
                }
            }

            return (T[])children.ToArray(typeof(T));
        }

        /// <summary>
        /// EndUpdate メソッドが呼ばれるまで、コントロールを再描画しないようにします。
        /// </summary>
        public static void BeginUpdate(this Control control)
        {
            ControlUtilities.SendMessage(control.Handle, WM_SETREDRAW, false, IntPtr.Zero);
        }

        /// <summary>
        /// BeginUpdate メソッドにより中断されていた描画を再開します。
        /// </summary>
        public static void EndUpdate(this Control control)
        {
            ControlUtilities.SendMessage(control.Handle, WM_SETREDRAW, true, IntPtr.Zero);
            control.Invalidate();
        }

        /// <summary>
        /// コントロールの基になるウィンドウ ハンドルを所有するスレッド上で、デリゲートを実行します。
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="method">コントロールのスレッド コンテキストで呼び出されるメソッドを格納しているデリゲート</param>
        /// <remarks>GCを効率的に動作させる目的で、非同期操作が完了するまで待機するために使用する WaitHandle を明示的に破棄してハンドル増加を防ぎます。</remarks>
        public static void InvokeSafely(this Control control, MethodInvoker method)
        {
            try
            {
                if (true == control.InvokeRequired)
                {
                    if ((true == control.IsHandleCreated)
                    && (false == control.IsDisposed))
                    {
                        var asyncResult = control.BeginInvoke(method);

                        control.EndInvoke(asyncResult);

                        asyncResult.AsyncWaitHandle.Close();
                    }
                }
                else
                {
                    method.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(control, ex.Message);
            }
        }

        #region Dock

        ///// <summary>
        /// フォームを埋め込みます。
        /// </summary>
        /// <param name="form">埋め込むフォーム</param>
        /// <param name="parent">フォームの埋め込み先</param>
        public static void Dock(Form? target, Control? parent)
        {
            // 埋め込めない場合
            if (null == target || null == parent)
            {
                // 何もしない
                return;
            }

            /// 親コントロールに配置
            target.TopLevel = false;
            target.Parent = parent;
            target.BackColor = parent.BackColor;
            target.Dock = DockStyle.Fill;
            ControlUtilities.MoveWindow(target.Handle, 0, 0, parent.Width, parent.Height, true);

            /// 前面表示
            target.Show();
            target.BringToFront();
        }

        /// <summary>
        /// ユーザーコントロールを埋め込みます。
        /// </summary>
        /// <param name="userControl">埋め込むユーザーコントロール</param>
        /// <param name="parent">ユーザーコントロールの埋め込み先</param>
        public static void Dock(UserControl? target, Control? parent)
        {
            // 埋め込めない場合
            if (null == target || null == parent)
            {
                // 何もしない
                return;
            }

            /// 親コントロールに配置
            target.Parent = parent;
            target.BackColor = parent.BackColor;
            target.Dock = DockStyle.Fill;
            ControlUtilities.MoveWindow(target.Handle, 0, 0, parent.Width, parent.Height, true);

            /// 前面表示
            target.Show();
            target.BringToFront();
        }

        /// <summary>
        /// コントロールを埋め込みます。
        /// </summary>
        /// <param name="Control">埋め込むコントロール</param>
        /// <param name="parent">ユーザーコントロールの埋め込み先</param>
        public static void Dock(Control? target, Control? parent)
        {
            // 埋め込めない場合
            if (null == target || null == parent)
            {
                // 何もしない
                return;
            }

            // 親コントロールに配置
            target.Parent = parent;
            target.BackColor = parent.BackColor;
            target.Dock = DockStyle.Fill;
            ControlUtilities.MoveWindow(target.Handle, 0, 0, parent.Width, parent.Height, true);

            // 前面表示
            target.Show();
            target.BringToFront();
        }
        #endregion

        #region Font

        /// <summary>
        /// 文字列全体を表示するフォントサイズに設定する
        /// </summary>
        /// <param name="control"></param>
        public static void SetFontSizeForTextShrinkToFit(Control control) => ControlUtilities.SetFontSizeForTextShrinkToFit(control, control.Font.Size, control.Text, control.Size);

        /// <summary>
        /// 文字列全体を表示するフォントサイズに設定する
        /// </summary>
        /// <param name="control"></param>
        public static void SetFontSizeForTextShrinkToFit(Control control, float defaultSize) => ControlUtilities.SetFontSizeForTextShrinkToFit(control, defaultSize, control.Text, control.Size);

        /// <summary>
        /// 文字列全体を表示するフォントサイズに設定する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        public static void SetFontSizeForTextShrinkToFit(Control control, float defaultSize, string text) => ControlUtilities.SetFontSizeForTextShrinkToFit(control, defaultSize, text, control.Size);

        /// <summary>
        /// 文字列全体を表示するフォントサイズに設定する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <param name="targetSize"></param>
        public static void SetFontSizeForTextShrinkToFit(Control control, float defaultSize, string text, SizeF targetSize)
        {
            try
            {
                var selectedFontSize = ControlUtilities.GetFontSizeForTextShrinkToFit(control, defaultSize, text, targetSize);

                control.Font = new Font(control.Font.FontFamily, selectedFontSize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(control, ex.Message);
            }
        }

        /// <summary>
        /// 文字列全体を表示するフォントサイズを取得する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public static float GetFontSizeForTextShrinkToFit(Control control, float defaultSize, string text, SizeF targetSize)
        {
            var selectedFontSize = defaultSize;

            try
            {
                var availableSize = targetSize;
                var isSizeFixed = false;

                do
                {
                    using var tryFont = new Font(control.Font.FontFamily, selectedFontSize);
                    var tryTextSize = TextRenderer.MeasureText(text, tryFont);

                    if (
                        (availableSize.Width > tryTextSize.Width)
                    && (availableSize.Height > tryTextSize.Height)
                    )
                    {
                        isSizeFixed = true;
                    }
                    else
                    {
                        selectedFontSize--;

                        if (selectedFontSize < 1.0f)
                        {
                            selectedFontSize = 1.0f;
                            break;
                        }
                    }
                }
                while (false == isSizeFixed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(control, ex.Message);
            }

            return selectedFontSize;
        }

        #endregion

        #endregion

        #region WinAPIラッパー

        #region SendMessage

        /// <summary>
        /// 指定された1つまたは複数のウィンドウに､ 指定されたメッセージを送ります。
        /// </summary>
        /// <param name="hWnd">送り先のウィンドウのハンドル	</param>
        /// <param name="msg">送るメッセージ	</param>
        /// <param name="wParam">第1メッセージ パラメータ	</param>
        /// <param name="lParam">第2メッセージ パラメータ	</param>
        /// <returns>戻り値は､ メッセージ処理の結果を示します。結果の値は送られたメッセージにより異なります。</returns>
        /// <remarks>この関数は指定されたウィンドウのウィンドウプロシージャを呼び出し､ プロシージャがメッセージを処理し終わるまで制御を戻しません。</remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, IntPtr lParam);

        public const int WM_SETREDRAW = 0x000B;

        #endregion

        #region MoveWindow

        /// <summary>
        /// 指定されたウィンドウの位置およびサイズを変更します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="X">X座標</param>
        /// <param name="Y">Y座標</param>
        /// <param name="nWidth">幅</param>
        /// <param name="nHeight">高さ</param>
        /// <param name="bRepaint">再描画フラグ</param>
        /// <returns>true:成功 false:失敗</returns>
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion

        #endregion
    }
}