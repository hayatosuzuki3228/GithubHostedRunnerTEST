namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// 確認画面
    /// </summary>
    public partial class ConfirmationForm : HutzperForm
    {
        //#region Staticメソッド

        //public static   DialogResult NewShowInfo(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        //{
        //    return new ConfirmationForm().ShowInfo(owner, buttons, text, message);
        //}

        //public static DialogResult NewShowError(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        //{
        //    return new ConfirmationForm().ShowError(owner, buttons, text, message);
        //}

        //public static DialogResult NewShowWarning(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        //{
        //    return new ConfirmationForm().ShowWarning(owner, buttons, text, message);
        //}

        //public static DialogResult NewShowQuestion(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        //{
        //    return new ConfirmationForm().ShowQuestion(owner, buttons, text, message);
        //}

        //#endregion

        #region プロパティ

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

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfirmationForm() : this((IServiceCollectionSharing?)null) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cancellationCallback">キャンセル処理コールバック</param>
        public ConfirmationForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            // コンポーネントを初期化する
            this.InitializeComponent();

            this.CloseButtonVisible = false;
            this.MinimizeButtonVisible = false;

            this.buttonOfOk.Click += this.Button_Click;
            this.buttonOfCancel.Click += this.Button_Click;
        }

        #endregion

        #region フィールド

        protected Dictionary<Button, DialogResult> buttons = new();

        #endregion

        #region メソッド

        public DialogResult ShowInfo(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            return this.ShowDialog(SystemIcons.Information, owner, buttons, text, message);
        }

        public DialogResult ShowError(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            return this.ShowDialog(SystemIcons.Error, owner, buttons, text, message);
        }

        public DialogResult ShowWarning(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            return this.ShowDialog(SystemIcons.Warning, owner, buttons, text, message);
        }

        public DialogResult ShowQuestion(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            return this.ShowDialog(SystemIcons.Question, owner, buttons, text, message);
        }

        /// <summary>
        /// モーダル ダイアログ ボックスとして表示
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="owner"></param>
        /// <param name="buttons"></param>
        /// <param name="text"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual DialogResult ShowDialog(System.Drawing.Icon icon, IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            // アイコン表示
            this.pictureBoxOfIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxOfIcon.Image = icon.ToBitmap();

            // タイトル表示
            this.Text = text;

            // メッセージ表示
            this.textBoxOfMessages.Lines = message;

            // ボタン削除
            var disposeButtons = new List<Button>();
            foreach (var button in this.panelContainerOfButton.Controls.Cast<Button>())
            {
                button.Visible = false;
                button.Click -= this.Button_Click;
                if (false == this.buttonOfCancel.Equals(button) && false == this.buttonOfOk.Equals(button))
                {
                    disposeButtons.Add(button);
                }
            }
            disposeButtons.ForEach(b => b.Dispose());
            disposeButtons.Clear();
            this.panelContainerOfButton.SuspendLayout();
            this.panelContainerOfButton.Controls.Clear();
            this.panelContainerOfButton.ResumeLayout();

            this.buttonOfOk.Font = this.Font;
            this.buttonOfCancel.Font = this.Font;

            // ボタン登録
            this.buttons.Clear();
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    {
                        this.buttons.Add(this.buttonOfOk, DialogResult.OK);
                        this.buttonOfOk.BorderColor = Color.DodgerBlue;
                    }
                    break;
                case MessageBoxButtons.OKCancel:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(this.buttonOfOk, DialogResult.OK);
                    }
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    {
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfCancel, this.Translate("中止")), DialogResult.Abort);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("再試行")), DialogResult.Retry);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("無視")), DialogResult.Ignore);
                    }
                    break;
                case MessageBoxButtons.YesNo:
                    {
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("はい")), DialogResult.Yes);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("いいえ")), DialogResult.No);
                    }
                    break;
                case MessageBoxButtons.YesNoCancel:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("はい")), DialogResult.Yes);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("いいえ")), DialogResult.No);
                    }
                    break;
                case MessageBoxButtons.RetryCancel:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("再試行")), DialogResult.Retry);
                    }
                    break;
                case MessageBoxButtons.CancelTryContinue:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("再試行")), DialogResult.Retry);
                        this.buttons.Add(ConfirmationForm.CloneButtonFrom(this.buttonOfOk, this.Translate("続行")), DialogResult.Continue);
                    }
                    break;
            }

            var margin = this.panelContainerOfButton.Width - this.buttonOfOk.Width * this.buttons.Count;
            margin /= (this.buttons.Count + 1);

            // 最初のボタンを右端に配置するように変更
            var location = new Point(
                this.panelContainerOfButton.Width - this.buttonOfOk.Width - margin,
                (this.panelContainerOfButton.Height - this.buttonOfOk.Height) / 2
            );

            this.panelContainerOfButton.SuspendLayout();
            foreach (var b in this.buttons.Keys)
            {
                b.Visible = true;
                b.Click += this.Button_Click;

                b.Location = location;

                this.panelContainerOfButton.Controls.Add(b);

                // 左に向かって次のボタンの位置を更新
                location.X -= (b.Width + margin);
            }
            this.panelContainerOfButton.ResumeLayout();

            return this.ShowDialog(owner);
        }
        /// <summary>
        /// ボタンコピー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="text"></param>
        /// <returns>コピーして作成されたボタン</returns>
        protected static Button CloneButtonFrom(RoundedButton source, string text) => CloneButtonFrom(source, text, source.BackColor, source.ForeColor, new Font(source.Font.FontFamily, source.Font.Size));

        /// <summary>
        /// ボタンコピー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="text"></param>
        /// <param name="backColor"></param>
        /// <param name="foreColor"></param>
        /// <param name="font"></param>
        /// <returns>コピーして作成されたボタン</returns>
        protected static Button CloneButtonFrom(RoundedButton source, string text, Color backColor, Color foreColor, Font font)
        {
            var newButton = new RoundedButton()
            {
                Size = source.Size,
                BorderSize = source.BorderSize,
                BorderRadius = source.BorderRadius,
                BorderColor = source.BorderColor,
                BackgroundColor = source.BackgroundColor,
                BackColor = backColor,
                ForeColor = foreColor,
                Visible = true,
                Text = text,
                Font = font,
            };

            return newButton;
        }

        #endregion

        /// <summary>
        /// 画面表示イベント        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmationForm_Shown(object sender, EventArgs e)
        {
            foreach (var item in this.buttons)
            {
                item.Key.Focus();
                break;
            }
        }

        /// <summary>
        /// ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object? sender, EventArgs e)
        {
            if (sender is Button senderButton && true == this.buttons.ContainsKey(senderButton))
            {
                this.DialogResult = this.buttons[senderButton];
            }
        }
    }
}