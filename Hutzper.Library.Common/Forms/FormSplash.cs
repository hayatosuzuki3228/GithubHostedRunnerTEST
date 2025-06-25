using Hutzper.Library.Common.Drawing;
using System.Diagnostics;

namespace Hutzper.Library.Common.Forms
{
    public class FormSplash
    {
        private Form? splashForm;
        private Form? mainForm;

        private AutoResetEvent? splashShownEvent;
        private readonly Stopwatch splashTimeWatch = new();
        private int minimumSplashTimeMs;

        #region メソッド

        /// <summary>
        /// スプラッシュ表示
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="minimumTimeMs"></param>
        public void ShowSplash<T>(Form? mainForm, bool isCoverMainForm = false, int minimumTimeMs = 2000) where T : Form, new()
        {
            if (this.mainForm != null || this.splashForm != null)
            {
                return;
            }

            this.mainForm = mainForm;
            if (this.mainForm != null)
            {
                this.mainForm.Activated += this.mainForm_Activated;
            }

            this.splashShownEvent = new AutoResetEvent(false);
            this.minimumSplashTimeMs = minimumTimeMs;

            System.Drawing.Size mainFormSize = this.mainForm?.Size ?? System.Drawing.Size.Empty;

            Task.Run(() =>
            {
                try
                {
                    this.splashForm = new T
                    {
                        TopMost = true
                    };
                    this.splashForm.Activated += this.splashForm_Activated;

                    if (true == isCoverMainForm && this.mainForm != null)
                    {
                        var sizePer = new SizeD(mainFormSize);
                        sizePer.Width /= this.splashForm.Width;
                        sizePer.Height /= this.splashForm.Height;

                        var selectedPer = sizePer.Height;

                        if (this.splashForm.Width * selectedPer > Screen.PrimaryScreen?.Bounds.Width)
                            selectedPer = Screen.PrimaryScreen.Bounds.Width * 0.95d / this.splashForm.Width;

                        this.splashForm.Width = Convert.ToInt32(this.splashForm.Width * selectedPer);
                        this.splashForm.Height = Convert.ToInt32(this.splashForm.Height * selectedPer);
                    }

                    Application.Run(this.splashForm);
                }
                catch (Exception)
                {
                }
            });
        }

        #endregion

        #region Formイベント

        /// <summary>
        /// スプラッシュフォームが不要になったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>メインフォームがアクティブになった時, スプラッシュフォームがクリックされたとき</remarks>
        private void mainForm_Activated(object? sender, EventArgs e)
        {
            var splashCloseEvent = new ManualResetEvent(false);

            if ((this.mainForm != null) && this.mainForm.Equals(sender))
            {
                this.mainForm.Activated -= this.mainForm_Activated;
                this.mainForm.Visible = false;

                if (this.splashForm != null)
                {
                    this.splashShownEvent?.WaitOne();
                    this.splashShownEvent?.Close();

                    Task.Run(async () =>
                    {
                        var delayTimeMs = this.minimumSplashTimeMs - (int)this.splashTimeWatch.ElapsedMilliseconds;
                        if (0 < delayTimeMs)
                            await Task.Delay(delayTimeMs);

                        this.splashForm.InvokeSafely((MethodInvoker)delegate ()
                        {
                            if (false == this.splashForm.IsDisposed)
                            {
                                this.splashForm.Close();
                                splashCloseEvent.Set();
                            }
                        });
                    });
                }
                else
                {
                    splashCloseEvent.Set();
                }

                splashCloseEvent.WaitOne();

                this.mainForm.Activate();

                this.mainForm.Visible = true;

                this.splashForm = null;
                this.mainForm = null;
            }
        }

        //Splashフォームが表示された時
        private void splashForm_Activated(object? sender, EventArgs e)
        {
            if ((this.splashForm != null) && this.splashForm.Equals(sender))
            {
                this.splashForm.Activated -= this.splashForm_Activated;

                this.splashShownEvent?.Set();
                this.splashTimeWatch.Restart();
            }
        }

        #endregion
    }
}