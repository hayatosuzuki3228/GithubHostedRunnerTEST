using Hutzper.Library.Common;
using Hutzper.Library.Common.Laungage;

namespace Hutzper.Library.Forms
{
    /// <summary>
    /// 共有サービスへの参照を提供する基本フォーム
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>直接継承しないでください。ServiceCollectionSharingFormを継承してください</remarks>
    public partial class ServiceCollectionSharingBaseForm : Form
    {
        #region プロパティ

        // ITranslator
        public ITranslator? Translator { get; protected set; }

        #endregion

        #region フィールド

        // IServiceCollection
        protected IServiceCollectionSharing? Services;

        // 翻訳ヘルパ
        protected TranslationHelper TranslationHelper;

        /// <summary>
        /// Enterイベントを登録されているコントロールのリスト
        /// </summary>
        protected List<Control> listEnterControl;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceCollectionSharingBaseForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceCollectionSharingBaseForm(IServiceCollectionSharing? serviceCollectionSharing)
        {
            this.InitializeComponent();

            // DIサービスへの参照を保持
            this.Services = serviceCollectionSharing;

            // 翻訳への参照を取得
            this.Translator = this.Services?.GetTranslator();
            this.TranslationHelper = new TranslationHelper(this.Translator);

            this.listEnterControl = new List<Control>();

            this.KeyPreview = true;
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// フォーム表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceCollectionSharingBaseForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.TranslationHelper.TranslateControl(this);

                foreach (var control in Common.Forms.ControlUtilities.GetAllControls(this))
                {
                    if (
                        (control is TextBox)
                    || (control is NumericUpDown)
                    )
                    {
                        this.listEnterControl.Add(control);

                        control.Enter += this.Control_Enter;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceCollectionSharingBaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                foreach (var control in this.listEnterControl)
                {
                    control.Enter -= this.Control_Enter;
                }

                this.listEnterControl.Clear();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// アクティブコントロール取得
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual Control GetActiveControl(Control control)
        {
            Control selectedControl = control;

            if (selectedControl is ContainerControl container && container.ActiveControl != null)
            {
                selectedControl = this.GetActiveControl(container.ActiveControl);
            }

            return selectedControl;
        }

        /// <summary>
        /// コントロールイベント:フォーカス入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_Enter(object? sender, EventArgs e)
        {
            try
            {
                var control = sender as Control;

                if (control is TextBox)
                {
                    var textBox = control as TextBox;

                    textBox?.Select(0, textBox.Text.Length);
                }
                else if (control is NumericUpDown)
                {
                    var numericUpDown = control as NumericUpDown;

                    numericUpDown?.Select(0, numericUpDown.Value.ToString().Length);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }


        #endregion

        /// <summary>
        /// 翻訳
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        protected virtual string Translate(params string[] sourceString) => this.Translator?.Translate(sourceString) ?? string.Join("", sourceString);
    }
}