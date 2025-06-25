namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    partial class ConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            buttonExit = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonSaveConfig = new Library.Common.Forms.NoFocusedRoundedButton(components);
            ucOnOffOperationActivateInspectionAtStartup = new Library.Forms.Setting.OnOffUserControl();
            ucOperationImageSavingIntervalSec = new Library.Forms.Setting.NumericUpDownUserControl();
            groupBox1 = new GroupBox();
            buttonStatisticsManualReset = new Library.Common.Forms.NoFocusedRoundedButton(components);
            ucStatisticsResetTiming = new Library.Forms.Setting.ComboBoxUserControl();
            ucMultiImageLayoutType = new Library.Forms.Setting.ComboBoxUserControl();
            groupBox3 = new GroupBox();
            ucOnOffInsightUse = new Library.Forms.Setting.OnOffUserControl();
            groupBox4 = new GroupBox();
            ucPathTemp = new Library.Forms.Setting.DirectorySelectionUserControl();
            ucPathLog = new Library.Forms.Setting.DirectorySelectionUserControl();
            ucPathData = new Library.Forms.Setting.DirectorySelectionUserControl();
            groupBox5 = new GroupBox();
            ucOnOffImageAcquisitionOnly = new Library.Forms.Setting.OnOffUserControl();
            nudSpecifiedConveyingSpeedMillPerSecond = new Library.Forms.Setting.NumericUpDownUserControl();
            nudReferenceConveyingSpeedMillPerSecond = new Library.Forms.Setting.NumericUpDownUserControl();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // buttonExit
            // 
            buttonExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonExit.BackColor = Color.DodgerBlue;
            buttonExit.BackgroundColor = Color.DodgerBlue;
            buttonExit.BorderColor = Color.PaleVioletRed;
            buttonExit.BorderRadius = 25;
            buttonExit.BorderSize = 0;
            buttonExit.FlatAppearance.BorderSize = 0;
            buttonExit.FlatStyle = FlatStyle.Flat;
            buttonExit.ForeColor = Color.White;
            buttonExit.Location = new Point(1026, 8);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new Size(225, 60);
            buttonExit.TabIndex = 13;
            buttonExit.Text = "終了";
            buttonExit.TextColor = Color.White;
            buttonExit.UseVisualStyleBackColor = false;
            buttonExit.Click += buttonExit_Click;
            // 
            // buttonSaveConfig
            // 
            buttonSaveConfig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSaveConfig.BackColor = Color.DodgerBlue;
            buttonSaveConfig.BackgroundColor = Color.DodgerBlue;
            buttonSaveConfig.BorderColor = Color.PaleVioletRed;
            buttonSaveConfig.BorderRadius = 25;
            buttonSaveConfig.BorderSize = 0;
            buttonSaveConfig.FlatAppearance.BorderSize = 0;
            buttonSaveConfig.FlatStyle = FlatStyle.Flat;
            buttonSaveConfig.ForeColor = Color.White;
            buttonSaveConfig.Location = new Point(794, 8);
            buttonSaveConfig.Name = "buttonSaveConfig";
            buttonSaveConfig.Size = new Size(225, 60);
            buttonSaveConfig.TabIndex = 17;
            buttonSaveConfig.Text = "保存";
            buttonSaveConfig.TextColor = Color.White;
            buttonSaveConfig.UseVisualStyleBackColor = false;
            buttonSaveConfig.Click += buttonSaveConfig_Click;
            // 
            // ucOnOffOperationActivateInspectionAtStartup
            // 
            ucOnOffOperationActivateInspectionAtStartup.BackColor = SystemColors.Control;
            ucOnOffOperationActivateInspectionAtStartup.BorderColor = Color.Black;
            ucOnOffOperationActivateInspectionAtStartup.BorderRadius = 25D;
            ucOnOffOperationActivateInspectionAtStartup.BorderSize = 1D;
            ucOnOffOperationActivateInspectionAtStartup.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffOperationActivateInspectionAtStartup.Index = -1;
            ucOnOffOperationActivateInspectionAtStartup.IsTextShrinkToFit = true;
            ucOnOffOperationActivateInspectionAtStartup.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffOperationActivateInspectionAtStartup.LabelForeColor = SystemColors.ControlText;
            ucOnOffOperationActivateInspectionAtStartup.LabelText = "起動時に自動で検査を開始する";
            ucOnOffOperationActivateInspectionAtStartup.Location = new Point(8, 56);
            ucOnOffOperationActivateInspectionAtStartup.Name = "ucOnOffOperationActivateInspectionAtStartup";
            ucOnOffOperationActivateInspectionAtStartup.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            ucOnOffOperationActivateInspectionAtStartup.OffBackColor = Color.Gray;
            ucOnOffOperationActivateInspectionAtStartup.OffToggleColor = Color.Gainsboro;
            ucOnOffOperationActivateInspectionAtStartup.OnBackColor = Color.DodgerBlue;
            ucOnOffOperationActivateInspectionAtStartup.OnToggleColor = Color.WhiteSmoke;
            ucOnOffOperationActivateInspectionAtStartup.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucOnOffOperationActivateInspectionAtStartup.Size = new Size(488, 48);
            ucOnOffOperationActivateInspectionAtStartup.TabIndex = 18;
            ucOnOffOperationActivateInspectionAtStartup.Value = false;
            // 
            // ucOperationImageSavingIntervalSec
            // 
            ucOperationImageSavingIntervalSec.BackColor = SystemColors.Control;
            ucOperationImageSavingIntervalSec.BorderColor = Color.Black;
            ucOperationImageSavingIntervalSec.BorderRadius = 25D;
            ucOperationImageSavingIntervalSec.BorderSize = 1D;
            ucOperationImageSavingIntervalSec.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucOperationImageSavingIntervalSec.Index = -1;
            ucOperationImageSavingIntervalSec.IsTextShrinkToFit = true;
            ucOperationImageSavingIntervalSec.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            ucOperationImageSavingIntervalSec.LabelForeColor = SystemColors.ControlText;
            ucOperationImageSavingIntervalSec.LabelText = "画像保存間隔 秒（負数は保存無効）";
            ucOperationImageSavingIntervalSec.Location = new Point(8, 112);
            ucOperationImageSavingIntervalSec.Name = "ucOperationImageSavingIntervalSec";
            ucOperationImageSavingIntervalSec.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            ucOperationImageSavingIntervalSec.NumericUpDownDecimalPlaces = 1;
            ucOperationImageSavingIntervalSec.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            ucOperationImageSavingIntervalSec.NumericUpDownMaximum = new decimal(new int[] { 9999, 0, 0, 0 });
            ucOperationImageSavingIntervalSec.NumericUpDownMinimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            ucOperationImageSavingIntervalSec.NumericUpDownThousandsSeparator = false;
            ucOperationImageSavingIntervalSec.NumericUpDownValue = new decimal(new int[] { 0, 0, 0, 0 });
            ucOperationImageSavingIntervalSec.NumericUpDownWidth = 120;
            ucOperationImageSavingIntervalSec.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucOperationImageSavingIntervalSec.Size = new Size(488, 48);
            ucOperationImageSavingIntervalSec.TabIndex = 19;
            ucOperationImageSavingIntervalSec.ValueDecimal = new decimal(new int[] { 0, 0, 0, 0 });
            ucOperationImageSavingIntervalSec.ValueDouble = 0D;
            ucOperationImageSavingIntervalSec.ValueInt = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonStatisticsManualReset);
            groupBox1.Controls.Add(ucStatisticsResetTiming);
            groupBox1.Controls.Add(ucMultiImageLayoutType);
            groupBox1.Controls.Add(ucOnOffOperationActivateInspectionAtStartup);
            groupBox1.Controls.Add(ucOperationImageSavingIntervalSec);
            groupBox1.Location = new Point(8, 80);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(504, 360);
            groupBox1.TabIndex = 20;
            groupBox1.TabStop = false;
            groupBox1.Text = "運用";
            // 
            // buttonStatisticsManualReset
            // 
            buttonStatisticsManualReset.BackColor = Color.DodgerBlue;
            buttonStatisticsManualReset.BackgroundColor = Color.DodgerBlue;
            buttonStatisticsManualReset.BorderColor = Color.PaleVioletRed;
            buttonStatisticsManualReset.BorderRadius = 25;
            buttonStatisticsManualReset.BorderSize = 0;
            buttonStatisticsManualReset.FlatAppearance.BorderSize = 0;
            buttonStatisticsManualReset.FlatStyle = FlatStyle.Flat;
            buttonStatisticsManualReset.ForeColor = Color.White;
            buttonStatisticsManualReset.Location = new Point(8, 288);
            buttonStatisticsManualReset.Name = "buttonStatisticsManualReset";
            buttonStatisticsManualReset.Size = new Size(225, 60);
            buttonStatisticsManualReset.TabIndex = 33;
            buttonStatisticsManualReset.Text = "統計情報リセット";
            buttonStatisticsManualReset.TextColor = Color.White;
            buttonStatisticsManualReset.UseVisualStyleBackColor = false;
            buttonStatisticsManualReset.Click += buttonStatisticsManualReset_Click;
            // 
            // ucStatisticsResetTiming
            // 
            ucStatisticsResetTiming.BackColor = SystemColors.Window;
            ucStatisticsResetTiming.BorderColor = Color.Black;
            ucStatisticsResetTiming.BorderRadius = 25D;
            ucStatisticsResetTiming.BorderSize = 1D;
            ucStatisticsResetTiming.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucStatisticsResetTiming.ComboBoxWidth = 296;
            ucStatisticsResetTiming.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucStatisticsResetTiming.Index = -1;
            ucStatisticsResetTiming.IsTextShrinkToFit = true;
            ucStatisticsResetTiming.LabelFont = new Font("Yu Gothic UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            ucStatisticsResetTiming.LabelForeColor = SystemColors.ControlText;
            ucStatisticsResetTiming.LabelText = "検査結果リセットタイミング";
            ucStatisticsResetTiming.Location = new Point(8, 232);
            ucStatisticsResetTiming.Name = "ucStatisticsResetTiming";
            ucStatisticsResetTiming.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            ucStatisticsResetTiming.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucStatisticsResetTiming.SelectedIndex = -1;
            ucStatisticsResetTiming.SelectedItem = null;
            ucStatisticsResetTiming.Size = new Size(488, 40);
            ucStatisticsResetTiming.TabIndex = 32;
            // 
            // ucMultiImageLayoutType
            // 
            ucMultiImageLayoutType.BackColor = SystemColors.Window;
            ucMultiImageLayoutType.BorderColor = Color.Black;
            ucMultiImageLayoutType.BorderRadius = 25D;
            ucMultiImageLayoutType.BorderSize = 1D;
            ucMultiImageLayoutType.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucMultiImageLayoutType.ComboBoxWidth = 296;
            ucMultiImageLayoutType.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucMultiImageLayoutType.Index = -1;
            ucMultiImageLayoutType.IsTextShrinkToFit = true;
            ucMultiImageLayoutType.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucMultiImageLayoutType.LabelForeColor = SystemColors.ControlText;
            ucMultiImageLayoutType.LabelText = "画像表示レイアウト";
            ucMultiImageLayoutType.Location = new Point(8, 176);
            ucMultiImageLayoutType.Name = "ucMultiImageLayoutType";
            ucMultiImageLayoutType.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            ucMultiImageLayoutType.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucMultiImageLayoutType.SelectedIndex = -1;
            ucMultiImageLayoutType.SelectedItem = null;
            ucMultiImageLayoutType.Size = new Size(488, 40);
            ucMultiImageLayoutType.TabIndex = 31;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(ucOnOffInsightUse);
            groupBox3.Location = new Point(8, 456);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(504, 128);
            groupBox3.TabIndex = 22;
            groupBox3.TabStop = false;
            groupBox3.Text = "クラウド";
            // 
            // ucOnOffInsightUse
            // 
            ucOnOffInsightUse.BackColor = SystemColors.Control;
            ucOnOffInsightUse.BorderColor = Color.Black;
            ucOnOffInsightUse.BorderRadius = 25D;
            ucOnOffInsightUse.BorderSize = 1D;
            ucOnOffInsightUse.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffInsightUse.Index = -1;
            ucOnOffInsightUse.IsTextShrinkToFit = true;
            ucOnOffInsightUse.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffInsightUse.LabelForeColor = SystemColors.ControlText;
            ucOnOffInsightUse.LabelText = "使用する";
            ucOnOffInsightUse.Location = new Point(8, 56);
            ucOnOffInsightUse.Name = "ucOnOffInsightUse";
            ucOnOffInsightUse.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            ucOnOffInsightUse.OffBackColor = Color.Gray;
            ucOnOffInsightUse.OffToggleColor = Color.Gainsboro;
            ucOnOffInsightUse.OnBackColor = Color.DodgerBlue;
            ucOnOffInsightUse.OnToggleColor = Color.WhiteSmoke;
            ucOnOffInsightUse.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucOnOffInsightUse.Size = new Size(488, 48);
            ucOnOffInsightUse.TabIndex = 20;
            ucOnOffInsightUse.Value = false;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(ucPathTemp);
            groupBox4.Controls.Add(ucPathLog);
            groupBox4.Controls.Add(ucPathData);
            groupBox4.Location = new Point(520, 336);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(728, 368);
            groupBox4.TabIndex = 23;
            groupBox4.TabStop = false;
            groupBox4.Text = "パス";
            // 
            // ucPathTemp
            // 
            ucPathTemp.BackColor = SystemColors.Control;
            ucPathTemp.BorderColor = Color.Black;
            ucPathTemp.BorderRadius = 8D;
            ucPathTemp.BorderSize = 1D;
            ucPathTemp.DirectorySelectable = false;
            ucPathTemp.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathTemp.Index = -1;
            ucPathTemp.IsTextShrinkToFit = true;
            ucPathTemp.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathTemp.LabelForeColor = SystemColors.ControlText;
            ucPathTemp.LabelText = "一時フォルダ";
            ucPathTemp.Location = new Point(8, 176);
            ucPathTemp.Name = "ucPathTemp";
            ucPathTemp.Nickname = "directorySelectionUserControl1 [Hutzper.Library.Forms.Setting.DirectorySelectionUserControl]";
            ucPathTemp.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucPathTemp.Size = new Size(712, 45);
            ucPathTemp.TabIndex = 2;
            ucPathTemp.TextBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathTemp.TextBoxImeMode = ImeMode.Disable;
            ucPathTemp.TextBoxMaxLength = 32767;
            ucPathTemp.TextBoxText = "";
            ucPathTemp.TextBoxTextAlign = HorizontalAlignment.Left;
            ucPathTemp.TextBoxWatermarkText = "";
            ucPathTemp.TextBoxWidth = 512;
            ucPathTemp.Value = null;
            // 
            // ucPathLog
            // 
            ucPathLog.BackColor = SystemColors.Control;
            ucPathLog.BorderColor = Color.Black;
            ucPathLog.BorderRadius = 8D;
            ucPathLog.BorderSize = 1D;
            ucPathLog.DirectorySelectable = false;
            ucPathLog.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathLog.Index = -1;
            ucPathLog.IsTextShrinkToFit = true;
            ucPathLog.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathLog.LabelForeColor = SystemColors.ControlText;
            ucPathLog.LabelText = "ログ";
            ucPathLog.Location = new Point(8, 120);
            ucPathLog.Name = "ucPathLog";
            ucPathLog.Nickname = "directorySelectionUserControl1 [Hutzper.Library.Forms.Setting.DirectorySelectionUserControl]";
            ucPathLog.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucPathLog.Size = new Size(712, 45);
            ucPathLog.TabIndex = 1;
            ucPathLog.TextBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathLog.TextBoxImeMode = ImeMode.Disable;
            ucPathLog.TextBoxMaxLength = 32767;
            ucPathLog.TextBoxText = "";
            ucPathLog.TextBoxTextAlign = HorizontalAlignment.Left;
            ucPathLog.TextBoxWatermarkText = "";
            ucPathLog.TextBoxWidth = 512;
            ucPathLog.Value = null;
            // 
            // ucPathData
            // 
            ucPathData.BackColor = SystemColors.Control;
            ucPathData.BorderColor = Color.Black;
            ucPathData.BorderRadius = 8D;
            ucPathData.BorderSize = 1D;
            ucPathData.DirectorySelectable = false;
            ucPathData.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathData.Index = -1;
            ucPathData.IsTextShrinkToFit = true;
            ucPathData.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathData.LabelForeColor = SystemColors.ControlText;
            ucPathData.LabelText = "データ";
            ucPathData.Location = new Point(8, 56);
            ucPathData.Name = "ucPathData";
            ucPathData.Nickname = "directorySelectionUserControl1 [Hutzper.Library.Forms.Setting.DirectorySelectionUserControl]";
            ucPathData.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucPathData.Size = new Size(712, 45);
            ucPathData.TabIndex = 0;
            ucPathData.TextBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucPathData.TextBoxImeMode = ImeMode.Disable;
            ucPathData.TextBoxMaxLength = 32767;
            ucPathData.TextBoxText = "";
            ucPathData.TextBoxTextAlign = HorizontalAlignment.Left;
            ucPathData.TextBoxWatermarkText = "";
            ucPathData.TextBoxWidth = 512;
            ucPathData.Value = null;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(ucOnOffImageAcquisitionOnly);
            groupBox5.Controls.Add(nudSpecifiedConveyingSpeedMillPerSecond);
            groupBox5.Controls.Add(nudReferenceConveyingSpeedMillPerSecond);
            groupBox5.Location = new Point(520, 80);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(728, 248);
            groupBox5.TabIndex = 24;
            groupBox5.TabStop = false;
            groupBox5.Text = "検査条件";
            // 
            // ucOnOffImageAcquisitionOnly
            // 
            ucOnOffImageAcquisitionOnly.BackColor = SystemColors.Control;
            ucOnOffImageAcquisitionOnly.BorderColor = Color.Black;
            ucOnOffImageAcquisitionOnly.BorderRadius = 25D;
            ucOnOffImageAcquisitionOnly.BorderSize = 1D;
            ucOnOffImageAcquisitionOnly.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffImageAcquisitionOnly.Index = -1;
            ucOnOffImageAcquisitionOnly.IsTextShrinkToFit = true;
            ucOnOffImageAcquisitionOnly.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            ucOnOffImageAcquisitionOnly.LabelForeColor = SystemColors.ControlText;
            ucOnOffImageAcquisitionOnly.LabelText = "画像取得のみ";
            ucOnOffImageAcquisitionOnly.Location = new Point(16, 48);
            ucOnOffImageAcquisitionOnly.Name = "ucOnOffImageAcquisitionOnly";
            ucOnOffImageAcquisitionOnly.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            ucOnOffImageAcquisitionOnly.OffBackColor = Color.Gray;
            ucOnOffImageAcquisitionOnly.OffToggleColor = Color.Gainsboro;
            ucOnOffImageAcquisitionOnly.OnBackColor = Color.DodgerBlue;
            ucOnOffImageAcquisitionOnly.OnToggleColor = Color.WhiteSmoke;
            ucOnOffImageAcquisitionOnly.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucOnOffImageAcquisitionOnly.Size = new Size(488, 48);
            ucOnOffImageAcquisitionOnly.TabIndex = 22;
            ucOnOffImageAcquisitionOnly.Value = false;
            // 
            // nudSpecifiedConveyingSpeedMillPerSecond
            // 
            nudSpecifiedConveyingSpeedMillPerSecond.BackColor = SystemColors.Control;
            nudSpecifiedConveyingSpeedMillPerSecond.BorderColor = Color.Black;
            nudSpecifiedConveyingSpeedMillPerSecond.BorderRadius = 25D;
            nudSpecifiedConveyingSpeedMillPerSecond.BorderSize = 1D;
            nudSpecifiedConveyingSpeedMillPerSecond.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            nudSpecifiedConveyingSpeedMillPerSecond.Index = -1;
            nudSpecifiedConveyingSpeedMillPerSecond.IsTextShrinkToFit = true;
            nudSpecifiedConveyingSpeedMillPerSecond.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            nudSpecifiedConveyingSpeedMillPerSecond.LabelForeColor = SystemColors.ControlText;
            nudSpecifiedConveyingSpeedMillPerSecond.LabelText = "指定の搬送速度 mm/s（運用時）";
            nudSpecifiedConveyingSpeedMillPerSecond.Location = new Point(8, 184);
            nudSpecifiedConveyingSpeedMillPerSecond.Name = "nudSpecifiedConveyingSpeedMillPerSecond";
            nudSpecifiedConveyingSpeedMillPerSecond.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownDecimalPlaces = 2;
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownMaximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownThousandsSeparator = false;
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownValue = new decimal(new int[] { 100, 0, 0, 0 });
            nudSpecifiedConveyingSpeedMillPerSecond.NumericUpDownWidth = 240;
            nudSpecifiedConveyingSpeedMillPerSecond.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            nudSpecifiedConveyingSpeedMillPerSecond.Size = new Size(624, 48);
            nudSpecifiedConveyingSpeedMillPerSecond.TabIndex = 21;
            nudSpecifiedConveyingSpeedMillPerSecond.ValueDecimal = new decimal(new int[] { 100, 0, 0, 0 });
            nudSpecifiedConveyingSpeedMillPerSecond.ValueDouble = 100D;
            nudSpecifiedConveyingSpeedMillPerSecond.ValueInt = 100;
            // 
            // nudReferenceConveyingSpeedMillPerSecond
            // 
            nudReferenceConveyingSpeedMillPerSecond.BackColor = SystemColors.Control;
            nudReferenceConveyingSpeedMillPerSecond.BorderColor = Color.Black;
            nudReferenceConveyingSpeedMillPerSecond.BorderRadius = 25D;
            nudReferenceConveyingSpeedMillPerSecond.BorderSize = 1D;
            nudReferenceConveyingSpeedMillPerSecond.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            nudReferenceConveyingSpeedMillPerSecond.Index = -1;
            nudReferenceConveyingSpeedMillPerSecond.IsTextShrinkToFit = true;
            nudReferenceConveyingSpeedMillPerSecond.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            nudReferenceConveyingSpeedMillPerSecond.LabelForeColor = SystemColors.ControlText;
            nudReferenceConveyingSpeedMillPerSecond.LabelText = "基準の搬送速度 mm/s（調整時）";
            nudReferenceConveyingSpeedMillPerSecond.Location = new Point(8, 120);
            nudReferenceConveyingSpeedMillPerSecond.Name = "nudReferenceConveyingSpeedMillPerSecond";
            nudReferenceConveyingSpeedMillPerSecond.Nickname = "numericUpDownUserControl1 [Hutzper.Library.Forms.Setting.NumericUpDownUserControl]";
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownDecimalPlaces = 2;
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownIncrement = new decimal(new int[] { 1, 0, 0, 0 });
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownMaximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownMinimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownThousandsSeparator = false;
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownValue = new decimal(new int[] { 100, 0, 0, 0 });
            nudReferenceConveyingSpeedMillPerSecond.NumericUpDownWidth = 240;
            nudReferenceConveyingSpeedMillPerSecond.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            nudReferenceConveyingSpeedMillPerSecond.Size = new Size(624, 48);
            nudReferenceConveyingSpeedMillPerSecond.TabIndex = 20;
            nudReferenceConveyingSpeedMillPerSecond.ValueDecimal = new decimal(new int[] { 100, 0, 0, 0 });
            nudReferenceConveyingSpeedMillPerSecond.ValueDouble = 100D;
            nudReferenceConveyingSpeedMillPerSecond.ValueInt = 100;
            // 
            // ConfigurationForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1258, 712);
            ControlBox = false;
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Controls.Add(buttonSaveConfig);
            Controls.Add(buttonExit);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ConfigurationForm";
            Nickname = "ConfigurationForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: ConfigForm";
            Text = "システム設定";
            Shown += ConfigurationForm_Shown;
            groupBox1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Library.Common.Forms.NoFocusedRoundedButton buttonExit;
        private Library.Common.Forms.NoFocusedRoundedButton buttonSaveConfig;
        private Library.Forms.Setting.OnOffUserControl ucOnOffOperationActivateInspectionAtStartup;
        private Library.Forms.Setting.NumericUpDownUserControl ucOperationImageSavingIntervalSec;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private Library.Forms.Setting.OnOffUserControl ucOnOffInsightUse;
        private GroupBox groupBox4;
        private Library.Forms.Setting.DirectorySelectionUserControl ucPathTemp;
        private Library.Forms.Setting.DirectorySelectionUserControl ucPathLog;
        private Library.Forms.Setting.DirectorySelectionUserControl ucPathData;
        private GroupBox groupBox5;
        private Library.Forms.Setting.NumericUpDownUserControl nudReferenceConveyingSpeedMillPerSecond;
        private Library.Forms.Setting.NumericUpDownUserControl nudSpecifiedConveyingSpeedMillPerSecond;
        private Library.Forms.Setting.OnOffUserControl ucOnOffImageAcquisitionOnly;
        private Library.Forms.Setting.ComboBoxUserControl ucMultiImageLayoutType;
        private Library.Forms.Setting.ComboBoxUserControl ucStatisticsResetTiming;
        private Library.Common.Forms.NoFocusedRoundedButton buttonStatisticsManualReset;
    }
}