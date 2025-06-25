using Hutzper.Library.Common;
using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Hutzper.Library.Forms
{
    /// <summary>
    /// 確認ユーザーコントロール
    /// </summary>
    public partial class ConfirmationUserControl : HutzperUserControl
    {
        #region プロパティ

        /// <summary>
        /// 確認タイプ
        /// </summary>
        public ConfirmationType ConfirmationType { get; protected set; }

        /// <summary>
        /// 戻り値
        /// </summary>
        public DialogResult DialogResult { get; protected set; } = DialogResult.None;

        /// <summary>
        /// タイトル文字列
        /// </summary>
        [Category("Custom Appearance")]

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                this.BaseLabelTitle.Text = value;
            }
        }

        /// <summary>
        /// マウスドラッグが有効かどうか
        /// </summary>
        [Browsable(true)]
        [Category("Custom Appearance")]
        [Description("マウスドラッグが有効かどうか")]
        public virtual bool MouseDragEnabled { get; set; } = false;

        /// <summary>
        /// メッセージのフォント
        /// </summary>
        [Browsable(true)]
        [Category("Custom Appearance")]
        [Description("メッセージのフォント")]
        public virtual Font MessageFont
        {
            get => this.textBoxOfMessages.Font;
            set => this.textBoxOfMessages.Font = value;
        }

        #endregion

        /// <summary>
        /// イベント:ボタンクリック
        /// </summary>
        public delegate void ButtonClickEventHandler(object sender, DialogResult result, out bool canClose);
        public event ButtonClickEventHandler? ButtonClick;

        #region フィールド

        protected Dictionary<Button, DialogResult> buttons = new();

        /// <summary>
        /// ドラッグ位置
        /// </summary>
        protected Common.Drawing.Point? dragLocation;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfirmationUserControl()
        {
            this.InitializeComponent();

            this.buttonOfOk.Click += this.Button_Click;
            this.buttonOfCancel.Click += this.Button_Click;

            this.Visible = false;
        }

        #region メソッド

        public void ShowInfo(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    this.ConfirmationType = ConfirmationType.Info;
                    this.BackColor = Color.Blue;
                    this.Show(owner, SystemIcons.Information, buttons, text, message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        public void ShowError(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    System.Media.SystemSounds.Hand.Play();
                    this.ConfirmationType = ConfirmationType.Error;
                    this.BackColor = Color.Red;
                    this.Show(owner, SystemIcons.Error, buttons, text, message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        public void ShowWarning(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    System.Media.SystemSounds.Beep.Play();
                    this.ConfirmationType = ConfirmationType.Warning;
                    this.BackColor = Color.Yellow;
                    this.Show(owner, SystemIcons.Warning, buttons, text, message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
        }

        public void ShowQuestion(IWin32Window owner, MessageBoxButtons buttons, string text, params string[] message)
        {
            this.InvokeSafely((MethodInvoker)delegate ()
            {
                try
                {
                    System.Media.SystemSounds.Question.Play();
                    this.ConfirmationType = ConfirmationType.Question;
                    this.BackColor = Color.Green;
                    this.Show(owner, SystemIcons.Question, buttons, text, message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
            });
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
        protected virtual void Show(IWin32Window owner, System.Drawing.Icon icon, MessageBoxButtons buttons, string text, params string[] message)
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

            this.DialogResult = DialogResult.None;

            // ボタン登録
            this.buttons.Clear();
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    {
                        this.buttons.Add(this.buttonOfOk, DialogResult.OK);
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
                        this.buttons.Add(CloneButtonFrom(this.buttonOfCancel, "中止"), DialogResult.Abort);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "再試行"), DialogResult.Retry);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "無視"), DialogResult.Ignore);
                    }
                    break;
                case MessageBoxButtons.YesNo:
                    {
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "はい"), DialogResult.Yes);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "いいえ"), DialogResult.No);
                    }
                    break;
                case MessageBoxButtons.YesNoCancel:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "はい"), DialogResult.Yes);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "いいえ"), DialogResult.No);
                    }
                    break;
                case MessageBoxButtons.RetryCancel:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "再試行"), DialogResult.Retry);
                    }
                    break;
                case MessageBoxButtons.CancelTryContinue:
                    {
                        this.buttons.Add(this.buttonOfCancel, DialogResult.Cancel);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "再試行"), DialogResult.Retry);
                        this.buttons.Add(CloneButtonFrom(this.buttonOfOk, "続行"), DialogResult.Continue);
                    }
                    break;
            }

            var margin = this.panelContainerOfButton.Width - this.buttonOfOk.Width * this.buttons.Count;
            margin /= (this.buttons.Count + 1);

            var location = new Point(margin, (this.panelContainerOfButton.Height - this.buttonOfOk.Height) / 2);
            this.panelContainerOfButton.SuspendLayout();
            foreach (var b in this.buttons.Keys)
            {
                b.Visible = true;
                b.Click += this.Button_Click;

                b.Location = location;

                this.panelContainerOfButton.Controls.Add(b);

                location.X += b.Width + margin;
            }
            this.panelContainerOfButton.ResumeLayout();

            // モードレス表示
            this.Visible = true;
            this.BringToFront();
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

        private void Button_Click(object? sender, EventArgs e)
        {
            if (sender is Button senderButton && true == this.buttons.ContainsKey(senderButton))
            {
                this.DialogResult = this.buttons[senderButton];

                var canClose = true;
                this.ButtonClick?.Invoke(this, this.DialogResult, out canClose);

                if (true == canClose)
                {
                    this.Hide();
                }
            }
        }

        /// <summary>
        /// マウスイベント:マウスダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragableControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (true == this.MouseDragEnabled)
            {
                Cursor.Current = Cursors.Hand;
                this.dragLocation = new Common.Drawing.Point(e.Location);
                this.BringToFront();
            }
        }

        /// <summary>
        /// マウスイベント:マウスアップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragableControl_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            this.dragLocation = null;
        }

        /// <summary>
        /// マウスイベント:マウスムーブ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragableControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != this.dragLocation)
            {
                this.Left += e.X - this.dragLocation.X;
                this.Top += e.Y - this.dragLocation.Y;
            }
        }
    }
}