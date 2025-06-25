using Hutzper.Library.Common;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.Setting;
using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.LightController;
using Hutzper.Library.LightController.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting
{
    /// <summary>
    /// ILightControllerのパラメータ設定画面
    /// </summary>
    public partial class LightParameterForm : ServiceCollectionSharingForm, ILightParameterForm
    {
        #region イベント

        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object, LightParameterSet>? ParameterChanged;

        #endregion

        #region フィールド

        /// <summary>
        /// 管理対象のILightControllerとILightControllerParameterのセットリスト
        /// </summary>
        protected List<LightParameterSet> Items = new();

        /// <summary>
        /// パラメータ設定用コントロールリスト
        /// </summary>
        protected List<Control> ParameterControls = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LightParameterForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LightParameterForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
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
        public virtual void Initialize(params LightParameterSet[] items)
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
        /// <param name="grabber"></param>
        public virtual void ChangeView(IGrabber Device) => this.ChangeView(this.Items.FindIndex((Predicate<LightParameterSet>)(d => d.Device == Device)));

        /// <summary>
        /// 指定したインデックスに対応するビューに切り替える
        /// </summary>
        /// <param name="index"></param>
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
        /// 全てのLightParameterSetを取得する
        /// </summary>
        /// <returns></returns>
        public virtual List<LightParameterSet> GetParameterSet()
        {
            var parameterSets = new List<LightParameterSet>();

            this.Items.ForEach(item => parameterSets.Add(item));

            return parameterSets;
        }

        /// <summary>
        /// 指定したILightControllerに対応するLightParameterSetを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public virtual LightParameterSet? GetParameterSet(ILightController device) => this.Items.FirstOrDefault((Func<LightParameterSet, bool>)(d => d.Device == device));

        /// <summary>
        /// 指定したILightControllerに対応するILightControllerParameterを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public virtual ILightControllerParameter? GetLightParameter(ILightController device) => this.GetParameterSet(device)?.LightParameter;

        /// <summary>
        /// 指定したILightControllerに対応するIBehaviorOptionsを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public virtual IBehaviorOptions? GetBehaviorOptions(ILightController device) => this.GetParameterSet(device)?.BehaviorOptions;

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
                var lp = selectedSet.LightParameter;

                // 光量
                if (this.UcLightModulaion.Equals(sender))
                {
                    lp.Modulation = this.UcLightModulaion.ValueInt;
                    if (true == device.Enabled)
                    {
                        device.Modulate(lp.Modulation);
                    }
                }

                var op = selectedSet.BehaviorOptions;

                // ON遅延時間ミリ秒
                if (this.UcLightTurnOnDelayMs.Equals(sender))
                {
                    op.LightTurnOnDelayMs = this.UcLightTurnOnDelayMs.ValueInt;
                }

                // OFF遅延時間ミリ秒
                if (this.UcLightTurnOffDelayMs.Equals(sender))
                {
                    op.LightTurnOffDelayMs = this.UcLightTurnOffDelayMs.ValueInt;
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
        /// コンボボックス変更イベント:パラメータ変更
        /// </summary>
        protected virtual void UcLightCombo_SelectedIndexChanged(object sender, int index, string text)
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
                var lp = selectedSet.LightParameter;

                // ON/OFF 制御方式
                if (0 <= this.UcLightingOnOffControlType.SelectedIndex)
                {
                    var ucCombo = this.UcLightingOnOffControlType;
                    if (Enum.GetValues(typeof(LightingOnOffControlType)).GetValue(ucCombo.SelectedIndex) is LightingOnOffControlType value)
                    {
                        lp.OnOffControlType = value;
                    }
                }

                // ONタイミング
                if (0 <= this.UcLightTurnOnTiming.SelectedIndex)
                {
                    var ucCombo = this.UcLightTurnOnTiming;
                    if (Enum.GetValues(typeof(LightControlTiming)).GetValue(ucCombo.SelectedIndex) is LightControlTiming value)
                    {
                        // 現状、共通設定にしているため、全てのデバイスに反映する
                        foreach (var op in this.Items.Select(i => i.BehaviorOptions))
                        {
                            op.LightTurnOnTiming = value;
                        }
                    }
                }

                // OFFタイミング
                if (0 <= this.UcLightTurnOffTiming.SelectedIndex)
                {
                    var ucCombo = this.UcLightTurnOffTiming;
                    if (Enum.GetValues(typeof(LightControlTiming)).GetValue(ucCombo.SelectedIndex) is LightControlTiming value)
                    {
                        // 現状、共通設定にしているため、全てのデバイスに反映する
                        foreach (var op in this.Items.Select(i => i.BehaviorOptions))
                        {
                            op.LightTurnOffTiming = value;
                        }
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
            this.ParameterControls.Add(this.UcLightModulaion);
            this.ParameterControls.Add(this.UcLightTurnOnDelayMs);
            this.ParameterControls.Add(this.UcLightTurnOffDelayMs);
            this.ParameterControls.Add(this.UcLightingOnOffControlType);
            this.ParameterControls.Add(this.UcLightTurnOnTiming);
            this.ParameterControls.Add(this.UcLightTurnOffTiming);
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

            // 点灯制御タイプ
            foreach (LightingOnOffControlType type in Enum.GetValues(typeof(LightingOnOffControlType)))
            {
                var description = type switch
                {
                    LightingOnOffControlType.NoControl => "手動",
                    LightingOnOffControlType.Continuous => "常時点灯",
                    LightingOnOffControlType.OneByOne => "ワーク単位",
                    LightingOnOffControlType.ExposureSync => "カメラ同期",
                    _ => string.Empty,
                };

                if (false == string.IsNullOrEmpty(description))
                {
                    this.UcLightingOnOffControlType.Items.Add(description);
                }
            }
            this.UcLightingOnOffControlType.SelectedIndexChanged += this.UcLightCombo_SelectedIndexChanged;

            // 照明制御タイミング
            foreach (var uc in new[] { this.UcLightTurnOnTiming, this.UcLightTurnOffTiming })
            {
                foreach (LightControlTiming type in Enum.GetValues(typeof(LightControlTiming)))
                {
                    var description = type switch
                    {
                        LightControlTiming.NoControl => "手動",
                        LightControlTiming.Fastest => (uc == this.UcLightTurnOnTiming) ? "信号立上り 即時" : "撮像後 即時",
                        LightControlTiming.FixedDelayFromSignalOn => "信号立上り 遅延",
                        LightControlTiming.FixedDelayFromSignalOff => "信号立下り 遅延",
                        _ => string.Empty,
                    };

                    if (false == string.IsNullOrEmpty(description))
                    {
                        uc.Items.Add(description);
                    }
                }

                uc.SelectedIndexChanged += this.UcLightCombo_SelectedIndexChanged;
            }
        }

        /// <summary>
        /// 指定されたセットに対応するコントロールを配置する
        /// </summary>
        /// <param name="selectedSet">対象のセット</param>
        protected virtual void ArrangeControls(LightParameterSet selectedSet)
        {
            try
            {
                // コンテナのクリア、コントロールを非表示にする
                this.ControlContainer.Controls.Clear();
                this.ParameterControls.ForEach(c => c.Visible = false);

                var lp = selectedSet.LightParameter;
                var op = selectedSet.BehaviorOptions;

                #region 光量
                {
                    var control = this.AddAndShowControl(this.UcLightModulaion);
                    control.ValueInt = (int)Math.Clamp(lp.Modulation, (int)control.NumericUpDownMinimum, (int)control.NumericUpDownMaximum);
                }
                #endregion

                #region ON遅延時間ミリ秒
                {
                    var control = this.AddAndShowControl(this.UcLightTurnOnDelayMs);
                    control.ValueInt = (int)Math.Clamp(op.LightTurnOnDelayMs, (int)control.NumericUpDownMinimum, (int)control.NumericUpDownMaximum);
                }
                #endregion

                #region OFF遅延時間ミリ秒
                {
                    var control = this.AddAndShowControl(this.UcLightTurnOffDelayMs);
                    control.ValueInt = (int)Math.Clamp(op.LightTurnOffDelayMs, (int)control.NumericUpDownMinimum, (int)control.NumericUpDownMaximum);
                }
                #endregion

                #region ON/OFF 制御方式
                {
                    var control = this.AddAndShowControl(this.UcLightingOnOffControlType);
                    control.SelectedIndex = Array.IndexOf(Enum.GetValues(lp.OnOffControlType.GetType()), lp.OnOffControlType);
                }
                #endregion

                #region ONタイミング
                {
                    var control = this.AddAndShowControl(this.UcLightTurnOnTiming);
                    control.SelectedIndex = Array.IndexOf(Enum.GetValues(op.LightTurnOnTiming.GetType()), op.LightTurnOnTiming);
                }
                #endregion

                #region OFFタイミング
                {
                    var control = this.AddAndShowControl(this.UcLightTurnOffTiming);
                    control.SelectedIndex = Array.IndexOf(Enum.GetValues(op.LightTurnOffTiming.GetType()), op.LightTurnOffTiming);
                }
                #endregion
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
