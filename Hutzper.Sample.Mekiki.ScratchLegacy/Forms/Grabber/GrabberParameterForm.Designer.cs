namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    partial class GrabberParameterForm
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
            ControlContainer = new FlowLayoutPanel();
            UcDeviceSelection = new Library.Forms.Setting.ComboBoxUserControl();
            UcGrabberExposure = new Library.Forms.Setting.NumericUpDownUserControl();
            UcGrabberAnalogGain = new Library.Forms.Setting.NumericUpDownUserControl();
            UcButtonExecAwb = new Library.Common.Forms.RoundedButton();
            UcGrabberLineLateHz = new Library.Forms.Setting.NumericUpDownUserControl();
            UcGrabberFramePerSec = new Library.Forms.Setting.NumericUpDownUserControl();
            UcButtonExit = new Library.Common.Forms.RoundedButton();
            UcImagingResolution = new Library.Forms.Setting.NumericUpDownUserControl();
            UcOptionTriggerDelayMs = new Library.Forms.Setting.NumericUpDownUserControl();
            UcGrabberHeightForLive = new Library.Forms.Setting.NumericUpDownUserControl();
            SuspendLayout();
            // 
            // ControlContainer
            // 
            ControlContainer.FlowDirection = FlowDirection.TopDown;
            ControlContainer.Location = new Point(0, 48);
            ControlContainer.Name = "ControlContainer";
            ControlContainer.Size = new Size(456, 56);
            ControlContainer.TabIndex = 0;
            // 
            // UcDeviceSelection
            // 
            UcDeviceSelection.BackColor = SystemColors.Window;
            UcDeviceSelection.BorderColor = Color.Black;
            UcDeviceSelection.BorderRadius = 25D;
            UcDeviceSelection.BorderSize = 1D;
            UcDeviceSelection.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcDeviceSelection.ComboBoxWidth = 320;
            UcDeviceSelection.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcDeviceSelection.Index = -1;
            UcDeviceSelection.IsTextShrinkToFit = true;
            UcDeviceSelection.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcDeviceSelection.LabelForeColor = SystemColors.ControlText;
            UcDeviceSelection.LabelText = "選択";
            UcDeviceSelection.Location = new Point(0, 0);
            UcDeviceSelection.Name = "UcDeviceSelection";
            UcDeviceSelection.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcDeviceSelection.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcDeviceSelection.SelectedIndex = -1;
            UcDeviceSelection.SelectedItem = null;
            UcDeviceSelection.Size = new Size(456, 40);
            UcDeviceSelection.TabIndex = 1;
            UcDeviceSelection.SelectedIndexChanged += UcDeviceSelection_SelectedIndexChanged;
            // 
            // UcGrabberExposure
            // 
            UcGrabberExposure.BackColor = SystemColors.Control;
            UcGrabberExposure.BorderColor = Color.Black;
            UcGrabberExposure.BorderRadius = 25D;
            UcGrabberExposure.BorderSize = 1D;
            UcGrabberExposure.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberExposure.Index = -1;
            UcGrabberExposure.IsTextShrinkToFit = true;
            UcGrabberExposure.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberExposure.LabelForeColor = SystemColors.ControlText;
            UcGrabberExposure.LabelText = "露光 um";
            UcGrabberExposure.Location = new Point(8, 232);
            UcGrabberExposure.Name = "UcGrabberExposure";
            UcGrabberExposure.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcGrabberExposure.NumericUpDownDecimalPlaces = 0;
            UcGrabberExposure.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcGrabberExposure.NumericUpDownMaximum = new decimal(new int[] { 10000, 0, 0, 0 });
            UcGrabberExposure.NumericUpDownMinimum = new decimal(new int[] { 10, 0, 0, 0 });
            UcGrabberExposure.NumericUpDownThousandsSeparator = false;
            UcGrabberExposure.NumericUpDownValue = new decimal(new int[] { 150, 0, 0, 0 });
            UcGrabberExposure.NumericUpDownWidth = 240;
            UcGrabberExposure.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcGrabberExposure.Size = new Size(440, 48);
            UcGrabberExposure.TabIndex = 4;
            UcGrabberExposure.ValueDecimal = new decimal(new int[] { 150, 0, 0, 0 });
            UcGrabberExposure.ValueDouble = 150D;
            UcGrabberExposure.ValueInt = 150;
            // 
            // UcGrabberAnalogGain
            // 
            UcGrabberAnalogGain.BackColor = SystemColors.Control;
            UcGrabberAnalogGain.BorderColor = Color.Black;
            UcGrabberAnalogGain.BorderRadius = 25D;
            UcGrabberAnalogGain.BorderSize = 1D;
            UcGrabberAnalogGain.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberAnalogGain.Index = -1;
            UcGrabberAnalogGain.IsTextShrinkToFit = true;
            UcGrabberAnalogGain.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberAnalogGain.LabelForeColor = SystemColors.ControlText;
            UcGrabberAnalogGain.LabelText = "アナログゲイン";
            UcGrabberAnalogGain.Location = new Point(8, 288);
            UcGrabberAnalogGain.Name = "UcGrabberAnalogGain";
            UcGrabberAnalogGain.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcGrabberAnalogGain.NumericUpDownDecimalPlaces = 0;
            UcGrabberAnalogGain.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcGrabberAnalogGain.NumericUpDownMaximum = new decimal(new int[] { 192, 0, 0, 0 });
            UcGrabberAnalogGain.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcGrabberAnalogGain.NumericUpDownThousandsSeparator = false;
            UcGrabberAnalogGain.NumericUpDownValue = new decimal(new int[] { 0, 0, 0, 0 });
            UcGrabberAnalogGain.NumericUpDownWidth = 240;
            UcGrabberAnalogGain.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcGrabberAnalogGain.Size = new Size(440, 48);
            UcGrabberAnalogGain.TabIndex = 5;
            UcGrabberAnalogGain.ValueDecimal = new decimal(new int[] { 0, 0, 0, 0 });
            UcGrabberAnalogGain.ValueDouble = 0D;
            UcGrabberAnalogGain.ValueInt = 0;
            // 
            // UcButtonExecAwb
            // 
            UcButtonExecAwb.BackColor = Color.DodgerBlue;
            UcButtonExecAwb.BackgroundColor = Color.DodgerBlue;
            UcButtonExecAwb.BorderColor = Color.PaleVioletRed;
            UcButtonExecAwb.BorderRadius = 25;
            UcButtonExecAwb.BorderSize = 0;
            UcButtonExecAwb.FlatAppearance.BorderSize = 0;
            UcButtonExecAwb.FlatStyle = FlatStyle.Flat;
            UcButtonExecAwb.ForeColor = Color.White;
            UcButtonExecAwb.Location = new Point(0, 520);
            UcButtonExecAwb.Name = "UcButtonExecAwb";
            UcButtonExecAwb.Size = new Size(221, 60);
            UcButtonExecAwb.TabIndex = 19;
            UcButtonExecAwb.Text = "WB調整";
            UcButtonExecAwb.TextColor = Color.White;
            UcButtonExecAwb.UseVisualStyleBackColor = false;
            UcButtonExecAwb.Click += UcExecAwb_Click;
            // 
            // UcGrabberLineLateHz
            // 
            UcGrabberLineLateHz.BackColor = SystemColors.Control;
            UcGrabberLineLateHz.BorderColor = Color.Black;
            UcGrabberLineLateHz.BorderRadius = 25D;
            UcGrabberLineLateHz.BorderSize = 1D;
            UcGrabberLineLateHz.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberLineLateHz.Index = -1;
            UcGrabberLineLateHz.IsTextShrinkToFit = true;
            UcGrabberLineLateHz.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberLineLateHz.LabelForeColor = SystemColors.ControlText;
            UcGrabberLineLateHz.LabelText = "line rate Hz";
            UcGrabberLineLateHz.Location = new Point(8, 344);
            UcGrabberLineLateHz.Name = "UcGrabberLineLateHz";
            UcGrabberLineLateHz.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcGrabberLineLateHz.NumericUpDownDecimalPlaces = 2;
            UcGrabberLineLateHz.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcGrabberLineLateHz.NumericUpDownMaximum = new decimal(new int[] { 26000, 0, 0, 0 });
            UcGrabberLineLateHz.NumericUpDownMinimum = new decimal(new int[] { 1000, 0, 0, 0 });
            UcGrabberLineLateHz.NumericUpDownThousandsSeparator = false;
            UcGrabberLineLateHz.NumericUpDownValue = new decimal(new int[] { 3000, 0, 0, 0 });
            UcGrabberLineLateHz.NumericUpDownWidth = 240;
            UcGrabberLineLateHz.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcGrabberLineLateHz.Size = new Size(440, 48);
            UcGrabberLineLateHz.TabIndex = 6;
            UcGrabberLineLateHz.ValueDecimal = new decimal(new int[] { 3000, 0, 0, 0 });
            UcGrabberLineLateHz.ValueDouble = 3000D;
            UcGrabberLineLateHz.ValueInt = 3000;
            // 
            // UcGrabberFramePerSec
            // 
            UcGrabberFramePerSec.BackColor = SystemColors.Control;
            UcGrabberFramePerSec.BorderColor = Color.Black;
            UcGrabberFramePerSec.BorderRadius = 25D;
            UcGrabberFramePerSec.BorderSize = 1D;
            UcGrabberFramePerSec.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberFramePerSec.Index = -1;
            UcGrabberFramePerSec.IsTextShrinkToFit = true;
            UcGrabberFramePerSec.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberFramePerSec.LabelForeColor = SystemColors.ControlText;
            UcGrabberFramePerSec.LabelText = "frame rate Hz";
            UcGrabberFramePerSec.Location = new Point(8, 400);
            UcGrabberFramePerSec.Name = "UcGrabberFramePerSec";
            UcGrabberFramePerSec.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcGrabberFramePerSec.NumericUpDownDecimalPlaces = 2;
            UcGrabberFramePerSec.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcGrabberFramePerSec.NumericUpDownMaximum = new decimal(new int[] { 100, 0, 0, 0 });
            UcGrabberFramePerSec.NumericUpDownMinimum = new decimal(new int[] { 1, 0, 0, 0 });
            UcGrabberFramePerSec.NumericUpDownThousandsSeparator = false;
            UcGrabberFramePerSec.NumericUpDownValue = new decimal(new int[] { 15, 0, 0, 0 });
            UcGrabberFramePerSec.NumericUpDownWidth = 240;
            UcGrabberFramePerSec.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcGrabberFramePerSec.Size = new Size(440, 48);
            UcGrabberFramePerSec.TabIndex = 7;
            UcGrabberFramePerSec.ValueDecimal = new decimal(new int[] { 15, 0, 0, 0 });
            UcGrabberFramePerSec.ValueDouble = 15D;
            UcGrabberFramePerSec.ValueInt = 15;
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
            UcButtonExit.Location = new Point(232, 520);
            UcButtonExit.Name = "UcButtonExit";
            UcButtonExit.Size = new Size(221, 60);
            UcButtonExit.TabIndex = 28;
            UcButtonExit.Text = "閉じる";
            UcButtonExit.TextColor = Color.White;
            UcButtonExit.UseVisualStyleBackColor = false;
            UcButtonExit.Click += UcButtonExit_Click;
            // 
            // UcImagingResolution
            // 
            UcImagingResolution.BackColor = SystemColors.Control;
            UcImagingResolution.BorderColor = Color.Black;
            UcImagingResolution.BorderRadius = 25D;
            UcImagingResolution.BorderSize = 1D;
            UcImagingResolution.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcImagingResolution.Index = -1;
            UcImagingResolution.IsTextShrinkToFit = true;
            UcImagingResolution.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcImagingResolution.LabelForeColor = SystemColors.ControlText;
            UcImagingResolution.LabelText = "分解能 mm/pix";
            UcImagingResolution.Location = new Point(8, 176);
            UcImagingResolution.Name = "UcImagingResolution";
            UcImagingResolution.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcImagingResolution.NumericUpDownDecimalPlaces = 3;
            UcImagingResolution.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 65536 });
            UcImagingResolution.NumericUpDownMaximum = new decimal(new int[] { 100, 0, 0, 0 });
            UcImagingResolution.NumericUpDownMinimum = new decimal(new int[] { 1, 0, 0, 196608 });
            UcImagingResolution.NumericUpDownThousandsSeparator = false;
            UcImagingResolution.NumericUpDownValue = new decimal(new int[] { 1, 0, 0, 0 });
            UcImagingResolution.NumericUpDownWidth = 240;
            UcImagingResolution.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcImagingResolution.Size = new Size(440, 48);
            UcImagingResolution.TabIndex = 3;
            UcImagingResolution.ValueDecimal = new decimal(new int[] { 1, 0, 0, 0 });
            UcImagingResolution.ValueDouble = 1D;
            UcImagingResolution.ValueInt = 1;
            // 
            // UcOptionTriggerDelayMs
            // 
            UcOptionTriggerDelayMs.BackColor = SystemColors.Control;
            UcOptionTriggerDelayMs.BorderColor = Color.Black;
            UcOptionTriggerDelayMs.BorderRadius = 25D;
            UcOptionTriggerDelayMs.BorderSize = 1D;
            UcOptionTriggerDelayMs.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcOptionTriggerDelayMs.Index = -1;
            UcOptionTriggerDelayMs.IsTextShrinkToFit = true;
            UcOptionTriggerDelayMs.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcOptionTriggerDelayMs.LabelForeColor = SystemColors.ControlText;
            UcOptionTriggerDelayMs.LabelText = "撮影遅延時間ミリ秒";
            UcOptionTriggerDelayMs.Location = new Point(8, 112);
            UcOptionTriggerDelayMs.Name = "UcOptionTriggerDelayMs";
            UcOptionTriggerDelayMs.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcOptionTriggerDelayMs.NumericUpDownDecimalPlaces = 0;
            UcOptionTriggerDelayMs.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcOptionTriggerDelayMs.NumericUpDownMaximum = new decimal(new int[] { 9999, 0, 0, 0 });
            UcOptionTriggerDelayMs.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcOptionTriggerDelayMs.NumericUpDownThousandsSeparator = false;
            UcOptionTriggerDelayMs.NumericUpDownValue = new decimal(new int[] { 1, 0, 0, 0 });
            UcOptionTriggerDelayMs.NumericUpDownWidth = 240;
            UcOptionTriggerDelayMs.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcOptionTriggerDelayMs.Size = new Size(440, 48);
            UcOptionTriggerDelayMs.TabIndex = 2;
            UcOptionTriggerDelayMs.ValueDecimal = new decimal(new int[] { 1, 0, 0, 0 });
            UcOptionTriggerDelayMs.ValueDouble = 1D;
            UcOptionTriggerDelayMs.ValueInt = 1;
            // 
            // UcGrabberHeightForLive
            // 
            UcGrabberHeightForLive.BackColor = SystemColors.Control;
            UcGrabberHeightForLive.BorderColor = Color.Black;
            UcGrabberHeightForLive.BorderRadius = 25D;
            UcGrabberHeightForLive.BorderSize = 1D;
            UcGrabberHeightForLive.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberHeightForLive.Index = -1;
            UcGrabberHeightForLive.IsTextShrinkToFit = true;
            UcGrabberHeightForLive.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcGrabberHeightForLive.LabelForeColor = SystemColors.ControlText;
            UcGrabberHeightForLive.LabelText = "調整用の画像高さ";
            UcGrabberHeightForLive.Location = new Point(8, 456);
            UcGrabberHeightForLive.Name = "UcGrabberHeightForLive";
            UcGrabberHeightForLive.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcGrabberHeightForLive.NumericUpDownDecimalPlaces = 0;
            UcGrabberHeightForLive.NumericUpDownIncrement = new decimal(new int[] { 8, 0, 0, 0 });
            UcGrabberHeightForLive.NumericUpDownMaximum = new decimal(new int[] { 8192, 0, 0, 0 });
            UcGrabberHeightForLive.NumericUpDownMinimum = new decimal(new int[] { 16, 0, 0, 0 });
            UcGrabberHeightForLive.NumericUpDownThousandsSeparator = false;
            UcGrabberHeightForLive.NumericUpDownValue = new decimal(new int[] { 256, 0, 0, 0 });
            UcGrabberHeightForLive.NumericUpDownWidth = 240;
            UcGrabberHeightForLive.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcGrabberHeightForLive.Size = new Size(440, 48);
            UcGrabberHeightForLive.TabIndex = 29;
            UcGrabberHeightForLive.ValueDecimal = new decimal(new int[] { 256, 0, 0, 0 });
            UcGrabberHeightForLive.ValueDouble = 256D;
            UcGrabberHeightForLive.ValueInt = 256;
            // 
            // GrabberParameterForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(458, 588);
            ControlBox = false;
            Controls.Add(UcGrabberHeightForLive);
            Controls.Add(UcOptionTriggerDelayMs);
            Controls.Add(UcImagingResolution);
            Controls.Add(UcButtonExit);
            Controls.Add(ControlContainer);
            Controls.Add(UcGrabberFramePerSec);
            Controls.Add(UcGrabberLineLateHz);
            Controls.Add(UcButtonExecAwb);
            Controls.Add(UcGrabberExposure);
            Controls.Add(UcGrabberAnalogGain);
            Controls.Add(UcDeviceSelection);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 480);
            Name = "GrabberParameterForm";
            Nickname = "GrabberParameterForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: GrabberParameterForm";
            Text = "カメラ設定画面";
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel ControlContainer;
        private Library.Forms.Setting.ComboBoxUserControl UcDeviceSelection;
        private Library.Forms.Setting.NumericUpDownUserControl UcGrabberExposure;
        private Library.Forms.Setting.NumericUpDownUserControl UcGrabberAnalogGain;
        private Library.Common.Forms.RoundedButton UcButtonExecAwb;
        private Library.Forms.Setting.NumericUpDownUserControl UcGrabberLineLateHz;
        private Library.Forms.Setting.NumericUpDownUserControl UcGrabberFramePerSec;
        private Library.Common.Forms.RoundedButton UcButtonExit;
        private Library.Forms.Setting.NumericUpDownUserControl UcImagingResolution;
        private Library.Forms.Setting.NumericUpDownUserControl UcOptionTriggerDelayMs;
        private Library.Forms.Setting.NumericUpDownUserControl UcGrabberHeightForLive;
    }
}