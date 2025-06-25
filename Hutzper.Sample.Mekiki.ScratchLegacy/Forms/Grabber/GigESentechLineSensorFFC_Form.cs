using Hutzper.Library.Common;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.Forms;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.ImageGrabber.Device.GigE.Sentech;
using Hutzper.Library.ImageGrabber.Device.Sentech;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    /// <summary>
    /// Sentech GigE Line Sensor FFC Form
    /// </summary>
    public partial class GigESentechLineSensorFFC_Form : ServiceCollectionSharingForm, ILineSensorFFC_Form
    {
        #region ILineSensorFFC_Form

        /// <summary>
        /// カメラ状態を問い合わせるイベント
        /// </summary>
        public event EventHandler<GrabberStatusEventArgs>? GrabberStatusRequested;

        /// <summary>
        /// 指定したデバイスがFFCをサポートしているかどうかを取得します
        /// </summary>
        /// <param name="device">チェック対象のデバイス</param>
        /// <returns>サポートしている場合 true</returns>
        public bool IsFFCSupported(ILineSensor device) => device is GigESentechLineSensorGrabber;

        /// <summary>
        /// FFCを実行するデバイスを登録して初期化します。
        /// </summary>
        /// <param name="devices">対象のデバイス</param>
        public void Initialize(params ILineSensor[] devices)
        {
            try
            {
                if (devices is null || devices.Length == 0)
                {
                    return;
                }

                foreach (var d in devices)
                {
                    if (false == this.IsFFCSupported(d))
                    {
                        continue;
                    }

                    if (true == this.Grabbers.Contains(d))
                    {
                        continue;
                    }

                    if (d is GigESentechLineSensorGrabber g)
                    {
                        this.Grabbers.Add(g);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UcButtonCorrectDark.Tag = null;
                this.UcButtonCorrectBright.Tag = null;
            }
        }

        /// <summary>
        /// 画面の表示
        /// </summary>
        /// <param name="owner"></param>
        public Form? GetForm() => this;

        #endregion

        #region フィールド

        private List<GigESentechLineSensorGrabber> Grabbers = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechLineSensorFFC_Form() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GigESentechLineSensorFFC_Form(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// ボタンイベント:終了
        /// </summary>
        private void UcButtonExit_Click(object sender, EventArgs e) => this.Visible = false;

        /// <summary>
        /// ボタンイベント：遮光時の補正
        /// </summary>
        private void UcButtonCorrectDark_Click(object sender, EventArgs e)
        {
            var correctionText = "遮光時の補正";

            using var confirmDialog = new ConfirmationForm();

            if (0 >= this.Grabbers.Count)
            {
                confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "利用可能なカメラがありません。");
                return;
            }

            // ライブ撮影状態かどうか
            var status = this.OnGrabberStatusRequested();
            if (status is null || false == status.IsLive)
            {
                confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "ライブ撮影状態にしてください。");
                return;
            }

            // 補正処理の実行確認
            if (DialogResult.OK != confirmDialog.ShowQuestion(this, MessageBoxButtons.OKCancel, correctionText, $"キャップ等で遮光されている必要があります。{Environment.NewLine}また、この処理には時間がかかります。{Environment.NewLine}実行しますか?"))
            {
                return;
            }

            try
            {
                this.UcButtonCorrectDark.Tag = null;
                this.UcButtonCorrectBright.Tag = null;

                // キャンセルフラグ
                var cancellationToken = false;

                // キャンセルコールバック処理
                void cancellationCallback(object sender)
                {
                    cancellationToken = true;
                };

                using var marqueue = ProgressBarForm.NewMarqueeType(this, correctionText, string.Empty, FormStartPosition.CenterParent, cancellationCallback);

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                var targetLevel = this.UcFFC_OffsetTarget.ValueInt;
                var timeoutSpan = TimeSpan.FromSeconds(180);

                marqueue.StartElapsedTimeDisplay(timeoutSpan);
                marqueue.AppendMessageText($"補正を実行しています。{Environment.NewLine}しばらくお待ちください。");

                #region 補正処理
                Task.Run(async () =>
                {
                    var isSuccess = false;

                    try
                    {
                        var tasks = this.Grabbers.Select(async g =>
                        {
                            g.Set_FFCOffsetMode(FFCOffsetMode.Off);
                            g.Set_FFCGainMode(FFCGainMode.Off);

                            g.Set_FFCOffsetTarget(targetLevel);

                            g.Set_FFCOffsetMode(FFCOffsetMode.Once);

                            var stopwatch = Stopwatch.StartNew();
                            while (true == g.Get_FFCOffsetMode(out FFCOffsetMode mode))
                            {
                                if (mode == FFCOffsetMode.On)
                                {
                                    return true;
                                }
                                else if (stopwatch.Elapsed > timeoutSpan)
                                {
                                    break;
                                }
                                else if (true == cancellationToken)
                                {
                                    break;
                                }

                                await Task.Delay(500);
                            }

                            return false;
                        });

                        var results = await Task.WhenAll(tasks);

                        isSuccess = results.All(r => r);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        if (true == isSuccess)
                        {
                            formBlockingAccessor.Set(DialogResult.OK);
                        }
                        else if (true == cancellationToken)
                        {
                            formBlockingAccessor.Set(DialogResult.Cancel);
                        }
                        else
                        {
                            formBlockingAccessor.Set(DialogResult.Abort);
                        }
                    }
                });
                #endregion

                var dialogResult = formBlockingAccessor.ShowBlocking();
                if (DialogResult.OK == dialogResult)
                {
                    Serilog.Log.Information($"{this}, dark correction succeed");
                    this.UcButtonCorrectDark.Tag = new object();

                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, correctionText, "補正処理が完了しました。");
                }
                else if (DialogResult.Cancel == dialogResult)
                {
                    Serilog.Log.Information($"{this}, dark correction canceled");

                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, correctionText, "補正処理がキャンセルされました。");
                }
                else
                {
                    Serilog.Log.Warning($"{this}, dark correction failed");

                    confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "補正処理に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"dark correction failed");
            }
        }

        /// <summary>
        /// ボタンイベント：受光時の補正
        /// </summary>
        /// <remarks>一番明るい画素の明るさレベルを基準とした FFC を行うタイプです(FFCGainMode: TargetPlusOnce)</remarks>
        private void UcButtonCorrectBright_Click(object sender, EventArgs e)
        {
            var correctionText = "受光時の補正";

            using var confirmDialog = new ConfirmationForm();

            // 遮光時の補正の実施確認
            if (this.UcButtonCorrectDark.Tag is null)
            {
                confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "①の補正を行ってください。");
                return;
            }

            // ライブ撮影状態かどうか
            var status = this.OnGrabberStatusRequested();
            if (status is null || false == status.IsLive)
            {
                confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "ライブ撮影状態にしてください。");
                return;
            }

            // 補正処理の実行確認
            if (DialogResult.OK != confirmDialog.ShowQuestion(this, MessageBoxButtons.OKCancel, correctionText, $"最大明るさレベルが180程度となるように調整してありますか?{Environment.NewLine}また、この処理には時間がかかります。{Environment.NewLine}実行しますか?"))
            {
                return;
            }

            try
            {
                this.UcButtonCorrectBright.Tag = null;

                // キャンセルフラグ
                var cancellationToken = false;

                // キャンセルコールバック処理
                void cancellationCallback(object sender)
                {
                    cancellationToken = true;
                };

                using var marqueue = ProgressBarForm.NewMarqueeType(this, correctionText, string.Empty, FormStartPosition.CenterParent, cancellationCallback);

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                var targetLevel = this.UcFFC_GainTarget.ValueInt;
                var timeoutSpan = TimeSpan.FromSeconds(180);

                marqueue.StartElapsedTimeDisplay(timeoutSpan);
                marqueue.AppendMessageText($"補正を実行しています。{Environment.NewLine}しばらくお待ちください。");

                #region 補正処理
                Task.Run(async () =>
                {
                    var isSuccess = false;

                    try
                    {
                        var tasks = this.Grabbers.Select(async g =>
                        {
                            g.Set_FFCGainTarget(targetLevel);

                            g.Set_FFCGainMode(FFCGainMode.TargetPlusOnce);

                            var stopwatch = Stopwatch.StartNew();
                            while (true == g.Get_FFCGainMode(out FFCGainMode mode))
                            {
                                if (mode == FFCGainMode.On)
                                {
                                    return true;
                                }
                                else if (stopwatch.Elapsed > timeoutSpan)
                                {
                                    break;
                                }
                                else if (true == cancellationToken)
                                {
                                    break;
                                }

                                await Task.Delay(500);
                            }

                            return false;
                        });

                        var results = await Task.WhenAll(tasks);

                        isSuccess = results.All(r => r);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        if (true == isSuccess)
                        {
                            formBlockingAccessor.Set(DialogResult.OK);
                        }
                        else if (true == cancellationToken)
                        {
                            formBlockingAccessor.Set(DialogResult.Cancel);
                        }
                        else
                        {
                            formBlockingAccessor.Set(DialogResult.Abort);
                        }
                    }
                });
                #endregion

                var dialogResult = formBlockingAccessor.ShowBlocking();
                if (DialogResult.OK == dialogResult)
                {
                    Serilog.Log.Information($"{this}, bright correction succeed");
                    this.UcButtonCorrectBright.Tag = new object();

                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, correctionText, "補正処理が完了しました。");
                }
                else if (DialogResult.Cancel == dialogResult)
                {
                    Serilog.Log.Information($"{this}, bright correction canceled");

                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, correctionText, "補正処理がキャンセルされました。");
                }
                else
                {
                    Serilog.Log.Warning($"{this}, bright correction failed");

                    confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "補正処理に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"bright correction failed");
            }
        }

        /// <summary>
        /// ボタンイベント：保存
        /// </summary>
        private void UcButtonSave_Click(object sender, EventArgs e)
        {
            var correctionText = "補正データの保存";

            using var confirmDialog = new ConfirmationForm();

            // ライブ撮影状態かどうか
            var status = this.OnGrabberStatusRequested();
            if (status is null || true == status.IsLive)
            {
                confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "保存を行う場合はライブ撮影を停止してください。");
                return;
            }

            try
            {
                using var marqueue = ProgressBarForm.NewMarqueeType(this, correctionText, $"カメラに保存しています。{Environment.NewLine}しばらくお待ちください。");

                var formBlockingAccessor = new FormBlockingAccessor(marqueue);

                #region 保存処理
                Task.Run(async () =>
                {
                    var isSuccess = false;

                    try
                    {
                        var tasks = this.Grabbers.Select(async g =>
                        {
                            return await Task.Run(() => g.Save_FFCSet());
                        });

                        var results = await Task.WhenAll(tasks);

                        isSuccess = results.All(r => r);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        if (true == isSuccess)
                        {
                            formBlockingAccessor.Set(DialogResult.OK);
                        }
                        else
                        {
                            formBlockingAccessor.Set(DialogResult.Abort);
                        }
                    }
                });
                #endregion

                if (DialogResult.OK == formBlockingAccessor.ShowBlocking())
                {
                    Serilog.Log.Information($"{this}, correction saving succeed");

                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, correctionText, "補正データの保存に成功しました。");
                }
                else
                {
                    Serilog.Log.Warning($"{this}, correction saving failed");

                    confirmDialog.ShowError(this, MessageBoxButtons.OK, correctionText, "補正データの保存に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"correction saving failed");
            }
        }

        #endregion

        /// <summary>
        /// 状態問い合わせ
        /// </summary>
        private GrabberStatusEventArgs? OnGrabberStatusRequested()
        {
            var args = (GrabberStatusEventArgs?)null;

            try
            {
                var currentEvent = this.GrabberStatusRequested;
                if (currentEvent is not null)
                {
                    args = new GrabberStatusEventArgs();
                    currentEvent(this, args);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return args;
        }
    }
}
