namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    partial class ResultJudgmentParameterForm
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
            UcDiagonalLengthThresholdMm = new Library.Forms.Setting.NumericUpDownUserControl();
            UcButtonExit = new Library.Common.Forms.RoundedButton();
            SuspendLayout();
            // 
            // UcDiagonalLengthThresholdMm
            // 
            UcDiagonalLengthThresholdMm.BackColor = SystemColors.Control;
            UcDiagonalLengthThresholdMm.BorderColor = Color.Black;
            UcDiagonalLengthThresholdMm.BorderRadius = 25D;
            UcDiagonalLengthThresholdMm.BorderSize = 1D;
            UcDiagonalLengthThresholdMm.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcDiagonalLengthThresholdMm.Index = -1;
            UcDiagonalLengthThresholdMm.IsTextShrinkToFit = true;
            UcDiagonalLengthThresholdMm.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcDiagonalLengthThresholdMm.LabelForeColor = SystemColors.ControlText;
            UcDiagonalLengthThresholdMm.LabelText = "不良サイズφmm";
            UcDiagonalLengthThresholdMm.Location = new Point(8, 8);
            UcDiagonalLengthThresholdMm.Name = "UcDiagonalLengthThresholdMm";
            UcDiagonalLengthThresholdMm.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcDiagonalLengthThresholdMm.NumericUpDownDecimalPlaces = 3;
            UcDiagonalLengthThresholdMm.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 65536 });
            UcDiagonalLengthThresholdMm.NumericUpDownMaximum = new decimal(new int[] { 9999, 0, 0, 0 });
            UcDiagonalLengthThresholdMm.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcDiagonalLengthThresholdMm.NumericUpDownThousandsSeparator = false;
            UcDiagonalLengthThresholdMm.NumericUpDownValue = new decimal(new int[] { 1, 0, 0, 0 });
            UcDiagonalLengthThresholdMm.NumericUpDownWidth = 240;
            UcDiagonalLengthThresholdMm.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcDiagonalLengthThresholdMm.Size = new Size(440, 48);
            UcDiagonalLengthThresholdMm.TabIndex = 29;
            UcDiagonalLengthThresholdMm.ValueDecimal = new decimal(new int[] { 1, 0, 0, 0 });
            UcDiagonalLengthThresholdMm.ValueDouble = 1D;
            UcDiagonalLengthThresholdMm.ValueInt = 1;
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
            UcButtonExit.Location = new Point(224, 112);
            UcButtonExit.Name = "UcButtonExit";
            UcButtonExit.Size = new Size(221, 60);
            UcButtonExit.TabIndex = 30;
            UcButtonExit.Text = "閉じる";
            UcButtonExit.TextColor = Color.White;
            UcButtonExit.UseVisualStyleBackColor = false;
            UcButtonExit.Click += UcButtonExit_Click;
            // 
            // ResultJudgmentParameterForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(458, 184);
            ControlBox = false;
            Controls.Add(UcDiagonalLengthThresholdMm);
            Controls.Add(UcButtonExit);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            MaximumSize = new Size(480, 240);
            MinimumSize = new Size(480, 240);
            Name = "ResultJudgmentParameterForm";
            Nickname = "ResultJudgmentParameterForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: 判定しきい値設定";
            Text = "判定しきい値設定";
            ResumeLayout(false);
        }

        #endregion

        private Library.Forms.Setting.NumericUpDownUserControl UcDiagonalLengthThresholdMm;
        private Library.Common.Forms.RoundedButton UcButtonExit;
    }
}