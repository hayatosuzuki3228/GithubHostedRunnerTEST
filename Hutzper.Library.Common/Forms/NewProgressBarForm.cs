using System.ComponentModel;
using System.Diagnostics;

namespace Hutzper.Library.Common.Forms
{
    public partial class NewProgressBarForm : Form
    {
        #region プログレスバー

        /// <summary>
        /// プロパティ:プログレス ブロックがプログレス バー内をスクロールするためにかかる時間を、ミリ秒単位で取得または設定します。
        /// </summary>
        /// <remarks>プログレス ブロックがプログレス バー内をスクロールするためにかかる時間 (ミリ秒単位)。</remarks>
        [Category("ProgressBar")]
        public int MarqueeAnimationSpeed
        {
            get => this.progressBar.MarqueeAnimationSpeed;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.MarqueeAnimationSpeed = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// プロパティ:コントロールの範囲の最大値を取得または設定します。
        /// </summary>
        /// <remarks>範囲の最大値。既定値は100です。</remarks>
        [Category("ProgressBar")]
        public int ProgressBarMaximum
        {
            get => this.progressBar.Maximum;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.Maximum = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// プロパティ:コントロールの範囲の最小値を取得または設定します。
        /// </summary>
        /// <remarks>範囲の最小値。既定値は0です。</remarks>
        [Category("ProgressBar")]
        public int ProgressBarMinimum
        {
            get => this.progressBar.Minimum;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.Minimum = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// プログレス バーで進行状況を示す方法を取得または設定します。
        /// </summary>
        /// <remarks>ProgressBarStyle値の1つ。既定値は Blocksです。</remarks>
        [Category("ProgressBar")]
        public ProgressBarStyle ProgressBarStyle
        {
            get => this.progressBar.Style;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.Style = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// プログレス バーの現在位置を取得または設定します。
        /// </summary>
        /// <remarks>プログレス バーの範囲内の位置。既定値は0です。</remarks>
        [Category("ProgressBar")]
        public int ProgressBarValue
        {
            get => this.progressBar.Value;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.Value = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// PerformStep メソッドを呼び出したときに、プログレスバーの現在の位置を進める量を取得または設定します。
        /// </summary>
        /// <remarks>PerformStepメソッドを呼び出すごとに、プログレス バーをインクリメントする量。既定値は10です。</remarks>
        [Category("ProgressBar")]
        public int ProgressBarStep
        {
            get => this.progressBar.Step;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.progressBar.Step = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        #endregion

        #region メッセージ

        /// <summary>
        /// タイトル文字列
        /// </summary>
        [Category("タイトル")]
        public string TitleText
        {
            get => this.textBoxTitleMessage.Text;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.textBoxTitleMessage.Text = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// メッセージ文字列
        /// </summary>
        [Category("メッセージ")]
        public string MessageText
        {
            get => this.textBoxTitleMessage.Text;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.textBoxTitleMessage.Text = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        #endregion

        #region 汎用ボタン

        /// <summary>
        /// 汎用ボタン有効かどうか
        /// </summary>
        [Category("General Button")]
        public bool GeneralButtonEnabled
        {
            get => this.buttonGeneral.Enabled;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.buttonGeneral.Enabled = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        [Category("General Button")]
        /// <summary>
        /// 汎用ボタン表示
        /// </summary>
        public bool GeneralButtonVisible
        {
            get => this.buttonGeneral.Visible;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.buttonGeneral.Visible = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// 汎用ボタン文字列
        /// </summary>
        [Category("General Button")]
        public string GeneralButtonText
        {
            get => this.buttonGeneral.Text;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.buttonGeneral.Text = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        #endregion

        #region キャンセルボタン

        [Category("Cancel Button")]
        /// <summary>
        /// 汎用ボタン表示
        /// </summary>
        public bool CancelButtonVisible
        {
            get => this.buttonGeneral.Visible;
            set
            {
                this.InvokeSafely((MethodInvoker)delegate ()
                {
                    try
                    {
                        this.buttonCancel.Visible = value;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                });
            }
        }

        #endregion

        #region 時間

        /// <summary>
        /// 経過時間
        /// </summary>
        public TimeSpan Elapsed => this.stopwatch.Elapsed;

        #endregion

        #region フォーム

        /// <summary>
        /// プロパティ:CreateParams
        /// </summary>
        /// <remarks>フォームが閉じられないようにします</remarks>
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                // ClassStyle に CS_NOCLOSE ビットを立てる
                var createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;

                return createParams;
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// 汎用ボタンクリック
        /// </summary>
        public event Action<object, Button, EventArgs>? GeneralButtonClick;

        #endregion

        #region フィールド

        /// <summary>
        /// キャンセル処理のコールバック
        /// </summary>
        private readonly Action<object>? cancellationCallback;

        /// <summary>
        /// ストップウォッチ
        /// </summary>
        private readonly Stopwatch stopwatch = new();

        #endregion

        #region スタティックメソッド

        /// <summary>
        /// MarqueeタイプのProgressBarForm
        /// </summary>
        /// <param name="text">タイトル文字列</param>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="formStartPosition">フォーム位置</param>
        /// <param name="cancellationCallback">キャンセルコールバック</param>
        /// <returns>MarqueeタイプのAgcProgressBarFormインスタンス</returns>
        public static ProgressBarForm NewMarqueeType(Form owner, string text, string message, FormStartPosition formStartPosition = FormStartPosition.CenterParent, Action<object>? cancellationCallback = null)
        {
            var dialog = new ProgressBarForm(cancellationCallback)
            {
                Owner = owner,
                Text = text,
                ProgressBarStyle = ProgressBarStyle.Marquee,
                StartPosition = formStartPosition,
                MessageText = message
            };

            return dialog;
        }

        /// <summary>
        /// ContinuousタイプのProgressBarForm
        /// </summary>
        /// <param name="text">タイトル文字列</param>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="formStartPosition">フォーム位置</param>
        /// <param name="cancellationCallback">キャンセルコールバック</param>
        /// <returns>MarqueeタイプのAgcProgressBarFormインスタンス</returns>
        public static ProgressBarForm NewContinuousType(Form owner, string text, string message, int min, int max, FormStartPosition formStartPosition = FormStartPosition.CenterParent, Action<object>? cancellationCallback = null)
        {
            var dialog = new ProgressBarForm(cancellationCallback)
            {
                Owner = owner,
                Text = text,
                ProgressBarStyle = ProgressBarStyle.Continuous,
                ProgressBarMinimum = min,
                ProgressBarMaximum = max,
                StartPosition = formStartPosition,
                MessageText = message
            };

            return dialog;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// タイトルを追加する
        /// </summary>
        /// <param name="message"></param>
        public void AppendTitleMessage(string message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    this.textBoxTitleMessage.Text = string.Empty;
                    this.textBoxTitleMessage.AppendText(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        /// <summary>
        /// メッセージを追加する
        /// </summary>
        /// <param name="message"></param>
        public void AppendInfoMessage(string message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    this.textBoxTextMessage.Text = string.Empty;
                    this.textBoxTextMessage.AppendText(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        /// <summary>
        /// キャンセルボタン無効化
        /// </summary>
        public void InvalidateCancelButton()
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    this.buttonGeneral.Enabled = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        /// <summary>
        /// プログレスバーの現在位置をStepプロパティの量だけ進めます。
        /// </summary>
        public void ProgressBarPerformStep(int count = 1)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        this.progressBar.PerformStep();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        /// <summary>
        /// 経過時間表示を開始する
        /// </summary>
        /// <param name="timeRequiredMilliseconds">所要時間</param>
        public void StartElapsedTimeDisplay()
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    this.labelTimeInfo.Visible = true;
                    this.stopwatch.Restart();
                    this.timer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NewProgressBarForm() : this(null, null) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cancellationCallback">キャンセル処理コールバック</param>
        public NewProgressBarForm(Action<object>? cancellationCallback = null) : this(null, cancellationCallback) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cancellationCallback">キャンセル処理コールバック</param>
        public NewProgressBarForm(IServiceCollectionSharing? serviceCollectionSharing = null, Action<object>? cancellationCallback = null)
        {
            // コンポーネントを初期化する
            InitializeComponent();

            // 最前面表示を明示的に無効化
            this.TopMost = false;

            // キャンセルコールバックを設定する
            this.cancellationCallback = cancellationCallback;

            // キャンセルボタンの表示設定
            this.buttonGeneral.Visible = this.cancellationCallback != null;

            this.progressBar.MarqueeAnimationSpeed = 10; // マーキーアニメーション時間
            this.progressBar.Maximum = 100; // プログレスバーの最大値
            this.progressBar.Value = 0; // プログレスバーの初期値
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// 経過時間表示タイマ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeSpan elapsed = this.stopwatch.Elapsed;
                this.labelTimeInfo.Text = $"{(int)(elapsed.TotalSeconds)}秒 経過..";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        /// <summary>
        /// 汎用ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGeneral_Click(object sender, EventArgs e)
        {
            try
            {
                this.GeneralButtonClick?.Invoke(this, (Button)sender, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.cancellationCallback?.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        private void ProgressBarForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if (true == this.buttonGeneral.Visible)
                {
                    this.buttonGeneral.Focus();
                }
                else
                {
                    this.progressBar.Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion
    }
}