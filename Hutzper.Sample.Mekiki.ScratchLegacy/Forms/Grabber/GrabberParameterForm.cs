using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Imaging;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.Setting;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    /// <summary>
    /// IGrabberのパラメータ設定画面
    /// </summary>
    public partial class GrabberParameterForm : ServiceCollectionSharingForm, IGrabberParameterForm
    {
        #region イベント

        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object, GrabberParameterSet>? ParameterChanged;

        #endregion

        #region フィールド

        /// <summary>
        /// 管理対象のIGrabberとIGrabberParameterのセットリスト
        /// </summary>
        protected List<GrabberParameterSet> Items = new();

        /// <summary>
        /// パラメータ設定用コントロールリスト
        /// </summary>
        protected List<Control> ParameterControls = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GrabberParameterForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GrabberParameterForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();

            // コントロールのリスト化
            this.InitializeControlList();
            this.ParameterControls.ForEach(c => c.Margin = new Padding(0, 0, 0, 8));

            // コンテナ位置とサイズの設定
            this.ControlContainer.Location = new(0, this.UcDeviceSelection.Top + this.UcDeviceSelection.Height + this.UcDeviceSelection.Margin.Bottom);
            this.ControlContainer.Size = new(this.ClientSize.Width, this.ParameterControls.Sum(c => c.Height + c.Margin.Bottom) + (this.ControlContainer.Padding.Top + this.ControlContainer.Padding.Bottom) * this.ParameterControls.Count);

            // フォームのサイズの設定
            this.ClientSize = new(this.ClientSize.Width, this.ControlContainer.Top + this.ControlContainer.Height + this.UcButtonExit.Height + this.UcButtonExit.Margin.Top + this.UcButtonExit.Margin.Bottom);

            // イベントの設定
            this.InitializeControlEvents();

            // コンテナのクリア、コントロールを非表示にする
            this.ControlContainer.Controls.Clear();
            this.ParameterControls.ForEach(c => c.Visible = false);
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="items">対象とするIGrabberとIGrabberParameterのセット</param>
        public virtual void Initialize(params GrabberParameterSet[] items)
        {
            try
            {
                // リストの初期化
                this.Items.Clear();
                this.UcDeviceSelection.Items.Clear();

                // リストへの追加
                foreach (var candidate in items)
                {
                    // 新規登録の場合
                    if (this.Items.FirstOrDefault(d => d.Device == candidate.Device || d.Nickname == candidate.Nickname) is null)
                    {
                        this.Items.Add(candidate);

                        this.UcDeviceSelection.Items.Add(candidate.Nickname);
                    }
                    // 既に登録されている場合
                    else
                    {
                        Serilog.Log.Warning($"{this}, duplicate device: {candidate.Nickname}");
                    }
                }

                // 先頭デバイスを選択状態にする
                if (0 < this.Items.Count)
                {
                    this.UcDeviceSelection.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 指定したIGrabberに対応するビューに切り替える
        /// </summary>
        public virtual void ChangeView(IGrabber device) => this.ChangeView(this.Items.FindIndex((Predicate<GrabberParameterSet>)(d => d.Device == device)));

        /// <summary>
        /// 指定したインデックスに対応するビューに切り替える
        /// </summary>
        /// <remarks>初期化時に登録した順番になります</remarks>
        public virtual void ChangeView(int index)
        {
            try
            {
                if (0 <= index)
                {
                    this.UcDeviceSelection.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 表示更新
        /// </summary>
        /// <remarks>外部でパラメータ変更を行った際に表示に反映させる</remarks>
        public virtual void UpdateView()
        {
            try
            {
                // 指定されたセットに対応するコントロールを配置する
                if (0 <= this.UcDeviceSelection.SelectedIndex && this.Items.Count > this.UcDeviceSelection.SelectedIndex)
                {
                    this.ArrangeControls(this.Items[this.UcDeviceSelection.SelectedIndex]);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 全てのGrabberParameterSetを取得する
        /// </summary>
        public virtual List<GrabberParameterSet> GetParameterSet()
        {
            var parameterSets = new List<GrabberParameterSet>();

            this.Items.ForEach(item => parameterSets.Add(item));

            return parameterSets;
        }

        /// <summary>
        /// 指定したIGrabberに対応するGrabberParameterSetを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public virtual GrabberParameterSet? GetParameterSet(IGrabber device) => this.Items.FirstOrDefault((Func<GrabberParameterSet, bool>)(d => d.Device == device));

        /// <summary>
        /// 指定したIGrabberに対応するIGrabberParameterを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public virtual IGrabberParameter? GetGrabberParameter(IGrabber device) => this.GetParameterSet(device)?.GrabberParameter;

        /// <summary>
        /// 指定したIGrabberに対応するIImagingParameterを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public virtual IImageProperties? GetImagingParameter(IGrabber device) => this.GetParameterSet(device)?.ImageProperties;

        /// <summary>
        /// 指定したIGrabberに対応するIBehaviorOptionsを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public virtual IBehaviorOptions? GetBehaviorOptions(IGrabber device) => this.GetParameterSet(device)?.BehaviorOptions;

        /// <summary>
        /// 画面の取得
        /// </summary>
        public virtual Form? GetForm() => this;

        #endregion

        #region GUIイベント

        /// <summary>
        /// カメラ選択変更イベント
        /// </summary>
        protected virtual void UcDeviceSelection_SelectedIndexChanged(object sender, int index, string text)
        {
            try
            {
                // 指定されたセットに対応するコントロールを配置する
                if (this.Items.Count > index)
                {
                    this.ArrangeControls(this.Items[index]);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 数値入力イベント:パラメータ変更
        /// </summary>
        protected virtual void UcParam_ValueChanged(object sender, decimal value)
        {
            try
            {
                // 対象が存在しない場合
                if (this.UcDeviceSelection.SelectedIndex < 0 || this.UcDeviceSelection.SelectedIndex >= this.Items.Count)
                {
                    return;
                }

                // 選択された対象への参照を取得
                var selectedSet = this.Items[this.UcDeviceSelection.SelectedIndex];
                var device = selectedSet.Device;
                var gp = selectedSet.GrabberParameter;
                var ip = selectedSet.ImageProperties;
                var op = selectedSet.BehaviorOptions;

                // 撮影遅延時間
                if (this.UcOptionTriggerDelayMs.Equals(sender))
                {
                    op.AcquisitionTriggerDelayMs = this.UcOptionTriggerDelayMs.ValueInt;
                }

                // 分解能
                if (this.UcImagingResolution.Equals(sender))
                {
                    ip.ResolutionMmPerPixel = this.UcImagingResolution.ValueDouble;
                }

                // 露光時間
                if (this.UcGrabberExposure.Equals(sender))
                {
                    gp.ExposureTimeMicroseconds = this.UcGrabberExposure.ValueDouble;
                    if (true == device.Enabled)
                    {
                        device.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                    }
                }

                // アナログゲイン
                if (this.UcGrabberAnalogGain.Equals(sender))
                {
                    gp.AnalogGain = this.UcGrabberAnalogGain.ValueDouble;
                    if (true == device.Enabled)
                    {
                        device.AnalogGain = gp.AnalogGain;
                    }
                }

                // エリアカメラの場合
                if (device is IAreaSensor areaGrabber && gp is IAreaSensorParameter areaParameter)
                {
                    // フレームレート
                    if (this.UcGrabberFramePerSec.Equals(sender))
                    {
                        areaParameter.FramesPerSecond = this.UcGrabberFramePerSec.ValueDouble;
                        if (true == areaGrabber.Enabled)
                        {
                            areaGrabber.FramesPerSecond = areaParameter.FramesPerSecond;
                        }
                    }
                }
                // ラインセンサの場合
                else if (device is ILineSensor lineGrabber && gp is ILineSensorParameter lineParameter)
                {
                    // ラインレート
                    if (this.UcGrabberLineLateHz.Equals(sender))
                    {
                        lineParameter.LineRateHz = this.UcGrabberLineLateHz.ValueDouble;
                        if (true == lineGrabber.Enabled)
                        {
                            lineGrabber.LineRateHz = lineParameter.LineRateHz;
                        }
                    }

                    // ライブ撮影調整用の画像高さ
                    if (this.UcGrabberHeightForLive.Equals(sender))
                    {
                        op.HeightForLiveAdjustment = this.UcGrabberHeightForLive.ValueInt;
                    }
                }

                // パラメータ変更イベントを通知する
                this.OnParameterChanged();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベント:AWB実行
        /// </summary>
        /// <remarks>カラーカメラのみボタン表示するため内部でチェックはしない</remarks>
        protected virtual void UcExecAwb_Click(object? sender, EventArgs e)
        {
            try
            {
                // イベントが無効化されている場合
                if ((sender as Control)?.Tag is not null)
                {
                    return;
                }

                // 対象が存在しない場合
                if (this.UcDeviceSelection.SelectedIndex < 0 || this.UcDeviceSelection.SelectedIndex >= this.Items.Count)
                {
                    return;
                }

                // 選択された対象への参照を取得
                var selectedSet = this.Items[this.UcDeviceSelection.SelectedIndex];

                // AWBの実行
                if (selectedSet.Device is IAreaSensor areaGrabber)
                {
                    areaGrabber.DoAutoWhiteBalancing();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// ボタンイベント:閉じるボタンクリック
        /// </summary>
        protected virtual void UcButtonExit_Click(object sender, EventArgs e) => this.Visible = false;

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 管理するコントロールをリストアップする
        /// </summary>
        protected virtual void InitializeControlList()
        {
            // コントロールのリスト化
            this.ParameterControls.Add(this.UcOptionTriggerDelayMs);
            this.ParameterControls.Add(this.UcImagingResolution);
            this.ParameterControls.Add(this.UcGrabberExposure);
            this.ParameterControls.Add(this.UcGrabberAnalogGain);
            this.ParameterControls.Add(this.UcGrabberFramePerSec);
            this.ParameterControls.Add(this.UcGrabberLineLateHz);
            this.ParameterControls.Add(this.UcButtonExecAwb);
            this.ParameterControls.Add(this.UcGrabberHeightForLive);
        }

        /// <summary>
        /// コントロールのイベント設定
        /// </summary>
        protected virtual void InitializeControlEvents()
        {
            // イベントの設定
            foreach (var control in this.ParameterControls)
            {
                if (control is NumericUpDownUserControl nuc)
                {
                    nuc.ValueChanged += this.UcParam_ValueChanged;
                }
            }
        }

        /// <summary>
        /// 指定されたセットに対応するコントロールを配置する
        /// </summary>
        /// <param name="selectedSet">対象のセット</param>
        protected virtual void ArrangeControls(GrabberParameterSet selectedSet)
        {
            try
            {
                // コンテナのクリア、コントロールを非表示にする
                this.ControlContainer.Controls.Clear();
                this.ParameterControls.ForEach(c => c.Visible = false);

                var op = selectedSet.BehaviorOptions;

                #region 撮影遅延時間
                {
                    var control = this.AddAndShowControl(this.UcOptionTriggerDelayMs);
                    control.ValueInt = Math.Clamp(op.AcquisitionTriggerDelayMs, (int)control.NumericUpDownMinimum, (int)control.NumericUpDownMaximum);
                }
                #endregion

                var ip = selectedSet.ImageProperties;

                #region 分解能
                {
                    var control = this.AddAndShowControl(this.UcImagingResolution);
                    control.ValueDouble = Math.Clamp(ip.ResolutionMmPerPixel, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                }
                #endregion

                // パラメータ値範囲を取得する
                var gp = selectedSet.GrabberParameter;
                var device = selectedSet.Device;
                device.GetValues(out Dictionary<ParametersKey, IClampedValue<double>> values, ParametersKey.ExposureTime, ParametersKey.AnalogGain, ParametersKey.FrameRate, ParametersKey.LineRate);

                #region 露光時間
                {
                    var control = this.AddAndShowControl(this.UcGrabberExposure);
                    var key = ParametersKey.ExposureTime;
                    if (true == values.ContainsKey(key))
                    {
                        control.NumericUpDownMinimum = (decimal)values[key].Minimum;
                        control.NumericUpDownMaximum = (decimal)values[key].Maximum;
                        control.ValueDouble = values[key].Value;
                    }
                    else
                    {
                        control.NumericUpDownMinimum = 1;
                        control.NumericUpDownMaximum = 100000;
                        control.ValueDouble = Math.Clamp(gp.ExposureTimeMicroseconds, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                    }
                }
                #endregion

                #region アナログゲイン
                {
                    var control = this.AddAndShowControl(this.UcGrabberAnalogGain);
                    var key = ParametersKey.AnalogGain;
                    if (true == values.ContainsKey(key))
                    {
                        control.NumericUpDownMinimum = (decimal)values[key].Minimum;
                        control.NumericUpDownMaximum = (decimal)values[key].Maximum;
                        control.ValueDouble = values[key].Value;
                    }
                    else
                    {
                        control.NumericUpDownMinimum = 0;
                        control.NumericUpDownMaximum = 255;
                        control.ValueDouble = Math.Clamp(gp.AnalogGain, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                    }
                }
                #endregion

                // エリアカメラの場合
                if (device is IAreaSensor areaGrabber && selectedSet.GrabberParameter is IAreaSensorParameter areaParam)
                {
                    #region FrameRate
                    {
                        var control = this.AddAndShowControl(this.UcGrabberFramePerSec);
                        var key = ParametersKey.FrameRate;
                        if (true == values.ContainsKey(key))
                        {
                            control.NumericUpDownMinimum = (decimal)values[key].Minimum;
                            control.NumericUpDownMaximum = (decimal)values[key].Maximum;
                            control.ValueDouble = values[key].Value;
                        }
                        else
                        {
                            control.NumericUpDownMinimum = 1;
                            control.NumericUpDownMaximum = 9999;
                            control.ValueDouble = Math.Clamp(areaParam.FramesPerSecond, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                        }
                    }
                    #endregion

                    if (areaGrabber.IsRgbColor)
                    {
                        this.AddAndShowControl(this.UcButtonExecAwb);
                    }
                }
                // ラインセンサの場合
                else if (device is ILineSensor lineGrabber && selectedSet.GrabberParameter is ILineSensorParameter lineParam)
                {
                    #region LineRateHz
                    {
                        var control = this.AddAndShowControl(this.UcGrabberLineLateHz);
                        var key = ParametersKey.LineRate;
                        if (true == values.ContainsKey(key))
                        {
                            control.NumericUpDownMinimum = (decimal)values[key].Minimum;
                            control.NumericUpDownMaximum = (decimal)values[key].Maximum;
                            control.ValueDouble = values[key].Value;
                        }
                        else
                        {
                            control.NumericUpDownMinimum = 1;
                            control.NumericUpDownMaximum = 1000 * 100;
                            control.ValueDouble = Math.Clamp(lineParam.LineRateHz, (double)control.NumericUpDownMinimum, (double)control.NumericUpDownMaximum);
                        }
                    }
                    #endregion

                    #region ライブ撮影調整用の画像高さ
                    {
                        var control = this.AddAndShowControl(this.UcGrabberHeightForLive);
                        control.ValueInt = Math.Clamp(op.HeightForLiveAdjustment, (int)control.NumericUpDownMinimum, (int)control.NumericUpDownMaximum);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// コントロールをコンテナに追加して表示する
        /// </summary>
        /// <param name="control">追加したいコントロール</param>
        /// <returns>追加されたコントロール</returns>
        protected virtual T AddAndShowControl<T>(T control) where T : Control
        {
            control.Visible = true;
            this.ControlContainer.Controls.Add(control);

            return control;
        }

        /// <summary>
        /// パラメータ変更イベントを通知する
        /// </summary>
        protected virtual void OnParameterChanged()
        {
            try
            {
                var selectedSet = this.Items[this.UcDeviceSelection.SelectedIndex];

                this.ParameterChanged?.Invoke(this, selectedSet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}