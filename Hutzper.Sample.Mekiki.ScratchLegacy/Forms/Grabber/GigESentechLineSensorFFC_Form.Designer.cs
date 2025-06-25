namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    partial class GigESentechLineSensorFFC_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            UcButtonCorrectDark = new Library.Common.Forms.NoFocusedRoundedButton(components);
            UcButtonCorrectBright = new Library.Common.Forms.NoFocusedRoundedButton(components);
            UcButtonExit = new Library.Common.Forms.RoundedButton();
            label1 = new Label();
            label2 = new Label();
            UcButtonSave = new Library.Common.Forms.RoundedButton();
            UcFFC_OffsetTarget = new Library.Forms.Setting.NumericUpDownUserControl();
            UcFFC_GainTarget = new Library.Forms.Setting.NumericUpDownUserControl();
            SuspendLayout();
            // 
            // UcButtonCorrectDark
            // 
            UcButtonCorrectDark.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UcButtonCorrectDark.BackColor = Color.DodgerBlue;
            UcButtonCorrectDark.BackgroundColor = Color.DodgerBlue;
            UcButtonCorrectDark.BorderColor = Color.PaleVioletRed;
            UcButtonCorrectDark.BorderRadius = 25;
            UcButtonCorrectDark.BorderSize = 0;
            UcButtonCorrectDark.FlatAppearance.BorderSize = 0;
            UcButtonCorrectDark.FlatStyle = FlatStyle.Flat;
            UcButtonCorrectDark.ForeColor = Color.White;
            UcButtonCorrectDark.Location = new Point(8, 8);
            UcButtonCorrectDark.Name = "UcButtonCorrectDark";
            UcButtonCorrectDark.Size = new Size(600, 60);
            UcButtonCorrectDark.TabIndex = 30;
            UcButtonCorrectDark.Text = "①キャップをし、遮光時の補正";
            UcButtonCorrectDark.TextAlign = ContentAlignment.MiddleLeft;
            UcButtonCorrectDark.TextColor = Color.White;
            UcButtonCorrectDark.UseVisualStyleBackColor = false;
            UcButtonCorrectDark.Click += UcButtonCorrectDark_Click;
            // 
            // UcButtonCorrectBright
            // 
            UcButtonCorrectBright.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UcButtonCorrectBright.BackColor = Color.DodgerBlue;
            UcButtonCorrectBright.BackgroundColor = Color.DodgerBlue;
            UcButtonCorrectBright.BorderColor = Color.PaleVioletRed;
            UcButtonCorrectBright.BorderRadius = 25;
            UcButtonCorrectBright.BorderSize = 0;
            UcButtonCorrectBright.FlatAppearance.BorderSize = 0;
            UcButtonCorrectBright.FlatStyle = FlatStyle.Flat;
            UcButtonCorrectBright.ForeColor = Color.White;
            UcButtonCorrectBright.Location = new Point(8, 80);
            UcButtonCorrectBright.Name = "UcButtonCorrectBright";
            UcButtonCorrectBright.Size = new Size(600, 60);
            UcButtonCorrectBright.TabIndex = 31;
            UcButtonCorrectBright.Text = "②飽和しない範囲で明るくし、受光時の補正";
            UcButtonCorrectBright.TextAlign = ContentAlignment.MiddleLeft;
            UcButtonCorrectBright.TextColor = Color.White;
            UcButtonCorrectBright.UseVisualStyleBackColor = false;
            UcButtonCorrectBright.Click += UcButtonCorrectBright_Click;
            // 
            // UcButtonExit
            // 
            UcButtonExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            UcButtonExit.BackColor = Color.DodgerBlue;
            UcButtonExit.BackgroundColor = Color.DodgerBlue;
            UcButtonExit.BorderColor = Color.PaleVioletRed;
            UcButtonExit.BorderRadius = 25;
            UcButtonExit.BorderSize = 0;
            UcButtonExit.FlatAppearance.BorderSize = 0;
            UcButtonExit.FlatStyle = FlatStyle.Flat;
            UcButtonExit.ForeColor = Color.White;
            UcButtonExit.Location = new Point(392, 352);
            UcButtonExit.Name = "UcButtonExit";
            UcButtonExit.Size = new Size(221, 60);
            UcButtonExit.TabIndex = 32;
            UcButtonExit.Text = "閉じる";
            UcButtonExit.TextColor = Color.White;
            UcButtonExit.UseVisualStyleBackColor = false;
            UcButtonExit.Click += UcButtonExit_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 256);
            label1.Name = "label1";
            label1.Size = new Size(465, 38);
            label1.TabIndex = 33;
            label1.Text = "レンズ、光源、カメラ設定を変更した場合";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 304);
            label2.Name = "label2";
            label2.Size = new Size(316, 38);
            label2.TabIndex = 34;
            label2.Text = "再度補正を行ってください。";
            // 
            // UcButtonSave
            // 
            UcButtonSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UcButtonSave.BackColor = Color.DodgerBlue;
            UcButtonSave.BackgroundColor = Color.DodgerBlue;
            UcButtonSave.BorderColor = Color.PaleVioletRed;
            UcButtonSave.BorderRadius = 25;
            UcButtonSave.BorderSize = 0;
            UcButtonSave.FlatAppearance.BorderSize = 0;
            UcButtonSave.FlatStyle = FlatStyle.Flat;
            UcButtonSave.ForeColor = Color.White;
            UcButtonSave.Location = new Point(8, 352);
            UcButtonSave.Name = "UcButtonSave";
            UcButtonSave.Size = new Size(221, 60);
            UcButtonSave.TabIndex = 35;
            UcButtonSave.Text = "カメラに保存";
            UcButtonSave.TextColor = Color.White;
            UcButtonSave.UseVisualStyleBackColor = false;
            UcButtonSave.Click += UcButtonSave_Click;
            // 
            // UcFFC_OffsetTarget
            // 
            UcFFC_OffsetTarget.BackColor = SystemColors.Control;
            UcFFC_OffsetTarget.BorderColor = Color.Black;
            UcFFC_OffsetTarget.BorderRadius = 25D;
            UcFFC_OffsetTarget.BorderSize = 1D;
            UcFFC_OffsetTarget.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcFFC_OffsetTarget.Index = -1;
            UcFFC_OffsetTarget.IsTextShrinkToFit = true;
            UcFFC_OffsetTarget.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcFFC_OffsetTarget.LabelForeColor = SystemColors.ControlText;
            UcFFC_OffsetTarget.LabelText = "遮光時 +目標レベル（通常は初期値を使用）";
            UcFFC_OffsetTarget.Location = new Point(8, 144);
            UcFFC_OffsetTarget.Name = "UcFFC_OffsetTarget";
            UcFFC_OffsetTarget.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcFFC_OffsetTarget.NumericUpDownDecimalPlaces = 0;
            UcFFC_OffsetTarget.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcFFC_OffsetTarget.NumericUpDownMaximum = new decimal(new int[] { 255, 0, 0, 0 });
            UcFFC_OffsetTarget.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcFFC_OffsetTarget.NumericUpDownThousandsSeparator = false;
            UcFFC_OffsetTarget.NumericUpDownValue = new decimal(new int[] { 3, 0, 0, 0 });
            UcFFC_OffsetTarget.NumericUpDownWidth = 120;
            UcFFC_OffsetTarget.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcFFC_OffsetTarget.Size = new Size(600, 48);
            UcFFC_OffsetTarget.TabIndex = 36;
            UcFFC_OffsetTarget.ValueDecimal = new decimal(new int[] { 3, 0, 0, 0 });
            UcFFC_OffsetTarget.ValueDouble = 3D;
            UcFFC_OffsetTarget.ValueInt = 3;
            // 
            // UcFFC_GainTarget
            // 
            UcFFC_GainTarget.BackColor = SystemColors.Control;
            UcFFC_GainTarget.BorderColor = Color.Black;
            UcFFC_GainTarget.BorderRadius = 25D;
            UcFFC_GainTarget.BorderSize = 1D;
            UcFFC_GainTarget.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcFFC_GainTarget.Index = -1;
            UcFFC_GainTarget.IsTextShrinkToFit = true;
            UcFFC_GainTarget.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcFFC_GainTarget.LabelForeColor = SystemColors.ControlText;
            UcFFC_GainTarget.LabelText = "受光時 +目標レベル（通常は初期値を使用）";
            UcFFC_GainTarget.Location = new Point(8, 200);
            UcFFC_GainTarget.Name = "UcFFC_GainTarget";
            UcFFC_GainTarget.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcFFC_GainTarget.NumericUpDownDecimalPlaces = 0;
            UcFFC_GainTarget.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcFFC_GainTarget.NumericUpDownMaximum = new decimal(new int[] { 255, 0, 0, 0 });
            UcFFC_GainTarget.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcFFC_GainTarget.NumericUpDownThousandsSeparator = false;
            UcFFC_GainTarget.NumericUpDownValue = new decimal(new int[] { 10, 0, 0, 0 });
            UcFFC_GainTarget.NumericUpDownWidth = 120;
            UcFFC_GainTarget.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcFFC_GainTarget.Size = new Size(600, 48);
            UcFFC_GainTarget.TabIndex = 37;
            UcFFC_GainTarget.ValueDecimal = new decimal(new int[] { 10, 0, 0, 0 });
            UcFFC_GainTarget.ValueDouble = 10D;
            UcFFC_GainTarget.ValueInt = 10;
            // 
            // GigESentechLineSensorFFC_Form
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(618, 424);
            ControlBox = false;
            Controls.Add(UcFFC_GainTarget);
            Controls.Add(UcFFC_OffsetTarget);
            Controls.Add(UcButtonSave);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(UcButtonExit);
            Controls.Add(UcButtonCorrectBright);
            Controls.Add(UcButtonCorrectDark);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "GigESentechLineSensorFFC_Form";
            Nickname = "SentechLineSensorFFC_Form [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: SentechLineSensorFFC_Form";
            Text = "輝度補正";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Library.Common.Forms.NoFocusedRoundedButton UcButtonCorrectDark;
        private Library.Common.Forms.NoFocusedRoundedButton UcButtonCorrectBright;
        private Library.Common.Forms.RoundedButton UcButtonExit;
        private Label label1;
        private Label label2;
        private Library.Common.Forms.RoundedButton UcButtonSave;
        private Library.Forms.Setting.NumericUpDownUserControl UcFFC_OffsetTarget;
        private Library.Forms.Setting.NumericUpDownUserControl UcFFC_GainTarget;
    }
}