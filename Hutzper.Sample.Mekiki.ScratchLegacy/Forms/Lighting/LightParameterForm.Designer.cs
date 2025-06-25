namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting
{
    partial class LightParameterForm
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
            UcButtonExit = new Library.Common.Forms.RoundedButton();
            ControlContainer = new FlowLayoutPanel();
            UcLightModulaion = new Library.Forms.Setting.NumericUpDownUserControl();
            UcDeviceSelection = new Library.Forms.Setting.ComboBoxUserControl();
            UcLightTurnOnTiming = new Library.Forms.Setting.ComboBoxUserControl();
            UcLightTurnOnDelayMs = new Library.Forms.Setting.NumericUpDownUserControl();
            UcLightingOnOffControlType = new Library.Forms.Setting.ComboBoxUserControl();
            UcLightTurnOffDelayMs = new Library.Forms.Setting.NumericUpDownUserControl();
            UcLightTurnOffTiming = new Library.Forms.Setting.ComboBoxUserControl();
            SuspendLayout();
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
            UcButtonExit.Location = new Point(233, 524);
            UcButtonExit.Name = "UcButtonExit";
            UcButtonExit.Size = new Size(221, 60);
            UcButtonExit.TabIndex = 32;
            UcButtonExit.Text = "閉じる";
            UcButtonExit.TextColor = Color.White;
            UcButtonExit.UseVisualStyleBackColor = false;
            UcButtonExit.Click += UcButtonExit_Click;
            // 
            // ControlContainer
            // 
            ControlContainer.FlowDirection = FlowDirection.TopDown;
            ControlContainer.Location = new Point(1, 52);
            ControlContainer.Name = "ControlContainer";
            ControlContainer.Size = new Size(456, 112);
            ControlContainer.TabIndex = 29;
            // 
            // UcLightModulaion
            // 
            UcLightModulaion.BackColor = SystemColors.Control;
            UcLightModulaion.BorderColor = Color.Black;
            UcLightModulaion.BorderRadius = 25D;
            UcLightModulaion.BorderSize = 1D;
            UcLightModulaion.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightModulaion.Index = -1;
            UcLightModulaion.IsTextShrinkToFit = true;
            UcLightModulaion.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightModulaion.LabelForeColor = SystemColors.ControlText;
            UcLightModulaion.LabelText = "光量";
            UcLightModulaion.Location = new Point(8, 168);
            UcLightModulaion.Name = "UcLightModulaion";
            UcLightModulaion.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcLightModulaion.NumericUpDownDecimalPlaces = 0;
            UcLightModulaion.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcLightModulaion.NumericUpDownMaximum = new decimal(new int[] { 1000, 0, 0, 0 });
            UcLightModulaion.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcLightModulaion.NumericUpDownThousandsSeparator = false;
            UcLightModulaion.NumericUpDownValue = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightModulaion.NumericUpDownWidth = 224;
            UcLightModulaion.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightModulaion.Size = new Size(440, 48);
            UcLightModulaion.TabIndex = 1;
            UcLightModulaion.ValueDecimal = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightModulaion.ValueDouble = 100D;
            UcLightModulaion.ValueInt = 100;
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
            UcDeviceSelection.Location = new Point(1, 4);
            UcDeviceSelection.Name = "UcDeviceSelection";
            UcDeviceSelection.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcDeviceSelection.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcDeviceSelection.SelectedIndex = -1;
            UcDeviceSelection.SelectedItem = null;
            UcDeviceSelection.Size = new Size(456, 40);
            UcDeviceSelection.TabIndex = 0;
            UcDeviceSelection.SelectedIndexChanged += UcDeviceSelection_SelectedIndexChanged;
            // 
            // UcLightTurnOnTiming
            // 
            UcLightTurnOnTiming.BackColor = SystemColors.Window;
            UcLightTurnOnTiming.BorderColor = Color.Black;
            UcLightTurnOnTiming.BorderRadius = 25D;
            UcLightTurnOnTiming.BorderSize = 1D;
            UcLightTurnOnTiming.ComboBoxFont = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOnTiming.ComboBoxWidth = 224;
            UcLightTurnOnTiming.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOnTiming.Index = -1;
            UcLightTurnOnTiming.IsTextShrinkToFit = true;
            UcLightTurnOnTiming.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOnTiming.LabelForeColor = SystemColors.ControlText;
            UcLightTurnOnTiming.LabelText = "ONタイミング（共通）";
            UcLightTurnOnTiming.Location = new Point(8, 408);
            UcLightTurnOnTiming.Name = "UcLightTurnOnTiming";
            UcLightTurnOnTiming.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcLightTurnOnTiming.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightTurnOnTiming.SelectedIndex = -1;
            UcLightTurnOnTiming.SelectedItem = null;
            UcLightTurnOnTiming.Size = new Size(440, 40);
            UcLightTurnOnTiming.TabIndex = 5;
            // 
            // UcLightTurnOnDelayMs
            // 
            UcLightTurnOnDelayMs.BackColor = SystemColors.Control;
            UcLightTurnOnDelayMs.BorderColor = Color.Black;
            UcLightTurnOnDelayMs.BorderRadius = 25D;
            UcLightTurnOnDelayMs.BorderSize = 1D;
            UcLightTurnOnDelayMs.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOnDelayMs.Index = -1;
            UcLightTurnOnDelayMs.IsTextShrinkToFit = true;
            UcLightTurnOnDelayMs.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOnDelayMs.LabelForeColor = SystemColors.ControlText;
            UcLightTurnOnDelayMs.LabelText = "ON遅延時間ミリ秒";
            UcLightTurnOnDelayMs.Location = new Point(8, 232);
            UcLightTurnOnDelayMs.Name = "UcLightTurnOnDelayMs";
            UcLightTurnOnDelayMs.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcLightTurnOnDelayMs.NumericUpDownDecimalPlaces = 0;
            UcLightTurnOnDelayMs.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcLightTurnOnDelayMs.NumericUpDownMaximum = new decimal(new int[] { 1000, 0, 0, 0 });
            UcLightTurnOnDelayMs.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcLightTurnOnDelayMs.NumericUpDownThousandsSeparator = false;
            UcLightTurnOnDelayMs.NumericUpDownValue = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightTurnOnDelayMs.NumericUpDownWidth = 224;
            UcLightTurnOnDelayMs.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightTurnOnDelayMs.Size = new Size(440, 48);
            UcLightTurnOnDelayMs.TabIndex = 2;
            UcLightTurnOnDelayMs.ValueDecimal = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightTurnOnDelayMs.ValueDouble = 100D;
            UcLightTurnOnDelayMs.ValueInt = 100;
            // 
            // UcLightingOnOffControlType
            // 
            UcLightingOnOffControlType.BackColor = SystemColors.Window;
            UcLightingOnOffControlType.BorderColor = Color.Black;
            UcLightingOnOffControlType.BorderRadius = 25D;
            UcLightingOnOffControlType.BorderSize = 1D;
            UcLightingOnOffControlType.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightingOnOffControlType.ComboBoxWidth = 224;
            UcLightingOnOffControlType.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightingOnOffControlType.Index = -1;
            UcLightingOnOffControlType.IsTextShrinkToFit = true;
            UcLightingOnOffControlType.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightingOnOffControlType.LabelForeColor = SystemColors.ControlText;
            UcLightingOnOffControlType.LabelText = "ON/OFF 制御方式";
            UcLightingOnOffControlType.Location = new Point(8, 352);
            UcLightingOnOffControlType.Name = "UcLightingOnOffControlType";
            UcLightingOnOffControlType.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcLightingOnOffControlType.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightingOnOffControlType.SelectedIndex = -1;
            UcLightingOnOffControlType.SelectedItem = null;
            UcLightingOnOffControlType.Size = new Size(440, 40);
            UcLightingOnOffControlType.TabIndex = 4;
            // 
            // UcLightTurnOffDelayMs
            // 
            UcLightTurnOffDelayMs.BackColor = SystemColors.Control;
            UcLightTurnOffDelayMs.BorderColor = Color.Black;
            UcLightTurnOffDelayMs.BorderRadius = 25D;
            UcLightTurnOffDelayMs.BorderSize = 1D;
            UcLightTurnOffDelayMs.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOffDelayMs.Index = -1;
            UcLightTurnOffDelayMs.IsTextShrinkToFit = true;
            UcLightTurnOffDelayMs.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOffDelayMs.LabelForeColor = SystemColors.ControlText;
            UcLightTurnOffDelayMs.LabelText = "OFF遅延時間ミリ秒";
            UcLightTurnOffDelayMs.Location = new Point(8, 296);
            UcLightTurnOffDelayMs.Name = "UcLightTurnOffDelayMs";
            UcLightTurnOffDelayMs.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            UcLightTurnOffDelayMs.NumericUpDownDecimalPlaces = 0;
            UcLightTurnOffDelayMs.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            UcLightTurnOffDelayMs.NumericUpDownMaximum = new decimal(new int[] { 1000, 0, 0, 0 });
            UcLightTurnOffDelayMs.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            UcLightTurnOffDelayMs.NumericUpDownThousandsSeparator = false;
            UcLightTurnOffDelayMs.NumericUpDownValue = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightTurnOffDelayMs.NumericUpDownWidth = 224;
            UcLightTurnOffDelayMs.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightTurnOffDelayMs.Size = new Size(440, 48);
            UcLightTurnOffDelayMs.TabIndex = 3;
            UcLightTurnOffDelayMs.ValueDecimal = new decimal(new int[] { 100, 0, 0, 0 });
            UcLightTurnOffDelayMs.ValueDouble = 100D;
            UcLightTurnOffDelayMs.ValueInt = 100;
            // 
            // UcLightTurnOffTiming
            // 
            UcLightTurnOffTiming.BackColor = SystemColors.Window;
            UcLightTurnOffTiming.BorderColor = Color.Black;
            UcLightTurnOffTiming.BorderRadius = 25D;
            UcLightTurnOffTiming.BorderSize = 1D;
            UcLightTurnOffTiming.ComboBoxFont = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOffTiming.ComboBoxWidth = 224;
            UcLightTurnOffTiming.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOffTiming.Index = -1;
            UcLightTurnOffTiming.IsTextShrinkToFit = true;
            UcLightTurnOffTiming.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            UcLightTurnOffTiming.LabelForeColor = SystemColors.ControlText;
            UcLightTurnOffTiming.LabelText = "OFFタイミング（共通）";
            UcLightTurnOffTiming.Location = new Point(8, 464);
            UcLightTurnOffTiming.Name = "UcLightTurnOffTiming";
            UcLightTurnOffTiming.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcLightTurnOffTiming.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcLightTurnOffTiming.SelectedIndex = -1;
            UcLightTurnOffTiming.SelectedItem = null;
            UcLightTurnOffTiming.Size = new Size(440, 40);
            UcLightTurnOffTiming.TabIndex = 6;
            // 
            // LightParameterForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(458, 588);
            ControlBox = false;
            Controls.Add(ControlContainer);
            Controls.Add(UcLightTurnOffDelayMs);
            Controls.Add(UcLightTurnOffTiming);
            Controls.Add(UcLightingOnOffControlType);
            Controls.Add(UcLightTurnOnDelayMs);
            Controls.Add(UcLightTurnOnTiming);
            Controls.Add(UcButtonExit);
            Controls.Add(UcLightModulaion);
            Controls.Add(UcDeviceSelection);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(480, 480);
            Name = "LightParameterForm";
            Nickname = "LightingParameterForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: 照明設定画面";
            Text = "照明設定画面";
            ResumeLayout(false);
        }

        #endregion

        private Library.Common.Forms.RoundedButton UcButtonExit;
        private FlowLayoutPanel ControlContainer;
        private Library.Forms.Setting.NumericUpDownUserControl UcLightModulaion;
        private Library.Forms.Setting.ComboBoxUserControl UcDeviceSelection;
        private Library.Forms.Setting.ComboBoxUserControl UcLightTurnOnTiming;
        private Library.Forms.Setting.NumericUpDownUserControl UcLightTurnOnDelayMs;
        private Library.Forms.Setting.ComboBoxUserControl UcLightingOnOffControlType;
        private Library.Forms.Setting.NumericUpDownUserControl UcLightTurnOffDelayMs;
        private Library.Forms.Setting.ComboBoxUserControl UcLightTurnOffTiming;
    }
}