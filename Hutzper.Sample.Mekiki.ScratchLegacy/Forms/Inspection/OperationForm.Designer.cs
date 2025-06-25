namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    partial class OperationForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperationForm));
            panelContainerOfLog = new Panel();
            buttonExit = new Library.Common.Forms.NoFocusedRoundedButton(components);
            ButtonChangePointLog = new Library.Common.Forms.NoFocusedRoundedButton(components);
            ucImageFileSuffix = new Library.Forms.Setting.TextBoxUserControl();
            label1 = new Label();
            ucMultiDeviceStatus = new Common.Diagnostics.MultiDeviceStatusUserControl();
            buttonGrab = new Library.Common.Forms.NoFocusedRoundedButton(components);
            UcMultiImageView = new Library.Forms.ImageView.MultiImageViewControl();
            UcCameraSelection = new Library.Forms.Setting.ComboBoxUserControl();
            panelContainerOfImageCollection = new Panel();
            UcResultAndStatisticsDisplay = new Inspection.ResultAndStatisticsDisplay();
            buttonJudgementSetting = new Library.Common.Forms.NoFocusedRoundedButton(components);
            panelContainerOfImageCollection.SuspendLayout();
            SuspendLayout();
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.Location = new Point(8, 816);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(1416, 96);
            panelContainerOfLog.TabIndex = 7;
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
            buttonExit.Location = new Point(1664, 8);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new Size(225, 60);
            buttonExit.TabIndex = 8;
            buttonExit.Text = "終了";
            buttonExit.TextColor = Color.White;
            buttonExit.UseVisualStyleBackColor = false;
            buttonExit.Click += buttonExit_Click;
            // 
            // ButtonChangePointLog
            // 
            ButtonChangePointLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonChangePointLog.BackColor = Color.DodgerBlue;
            ButtonChangePointLog.BackgroundColor = Color.DodgerBlue;
            ButtonChangePointLog.BorderColor = Color.PaleVioletRed;
            ButtonChangePointLog.BorderRadius = 25;
            ButtonChangePointLog.BorderSize = 0;
            ButtonChangePointLog.FlatAppearance.BorderSize = 0;
            ButtonChangePointLog.FlatStyle = FlatStyle.Flat;
            ButtonChangePointLog.ForeColor = Color.White;
            ButtonChangePointLog.Location = new Point(144, 520);
            ButtonChangePointLog.Name = "ButtonChangePointLog";
            ButtonChangePointLog.Size = new Size(256, 200);
            ButtonChangePointLog.TabIndex = 12;
            ButtonChangePointLog.Text = "変化点 ログ出力";
            ButtonChangePointLog.TextColor = Color.White;
            ButtonChangePointLog.UseVisualStyleBackColor = false;
            ButtonChangePointLog.Visible = false;
            ButtonChangePointLog.Click += noFocusedRoundedButton1_Click;
            // 
            // ucImageFileSuffix
            // 
            ucImageFileSuffix.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ucImageFileSuffix.BackColor = SystemColors.Control;
            ucImageFileSuffix.BorderColor = Color.Black;
            ucImageFileSuffix.BorderRadius = 25D;
            ucImageFileSuffix.BorderSize = 1D;
            ucImageFileSuffix.Font = new Font("Yu Gothic UI", 14F);
            ucImageFileSuffix.Index = -1;
            ucImageFileSuffix.IsTextShrinkToFit = true;
            ucImageFileSuffix.LabelFont = new Font("Yu Gothic UI", 9F);
            ucImageFileSuffix.LabelForeColor = SystemColors.ControlText;
            ucImageFileSuffix.LabelText = "付与する識別文字";
            ucImageFileSuffix.Location = new Point(0, 784);
            ucImageFileSuffix.Margin = new Padding(4, 5, 4, 5);
            ucImageFileSuffix.Name = "ucImageFileSuffix";
            ucImageFileSuffix.Nickname = "textBoxUserControl1 [Hutzper.Library.Forms.Setting.TextBoxUserControl]";
            ucImageFileSuffix.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            ucImageFileSuffix.Size = new Size(400, 48);
            ucImageFileSuffix.TabIndex = 21;
            ucImageFileSuffix.TextBoxImeMode = ImeMode.NoControl;
            ucImageFileSuffix.TextBoxMaxLength = 10;
            ucImageFileSuffix.TextBoxText = "マーク";
            ucImageFileSuffix.TextBoxTextAlign = HorizontalAlignment.Left;
            ucImageFileSuffix.TextBoxWatermarkText = "";
            ucImageFileSuffix.TextBoxWidth = 240;
            ucImageFileSuffix.Value = "マーク";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(8, 744);
            label1.Name = "label1";
            label1.Size = new Size(157, 38);
            label1.TabIndex = 22;
            label1.Text = "画像収集用";
            // 
            // ucMultiDeviceStatus
            // 
            ucMultiDeviceStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ucMultiDeviceStatus.ItemWidth = 160;
            ucMultiDeviceStatus.Location = new Point(8, 920);
            ucMultiDeviceStatus.Name = "ucMultiDeviceStatus";
            ucMultiDeviceStatus.Size = new Size(976, 48);
            ucMultiDeviceStatus.TabIndex = 23;
            // 
            // buttonGrab
            // 
            buttonGrab.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonGrab.BackColor = Color.DodgerBlue;
            buttonGrab.BackgroundColor = Color.DodgerBlue;
            buttonGrab.BorderColor = Color.PaleVioletRed;
            buttonGrab.BorderRadius = 25;
            buttonGrab.BorderSize = 0;
            buttonGrab.FlatAppearance.BorderSize = 0;
            buttonGrab.FlatStyle = FlatStyle.Flat;
            buttonGrab.ForeColor = Color.White;
            buttonGrab.Location = new Point(144, 0);
            buttonGrab.Name = "buttonGrab";
            buttonGrab.Size = new Size(256, 200);
            buttonGrab.TabIndex = 24;
            buttonGrab.Text = "撮影実行";
            buttonGrab.TextColor = Color.White;
            buttonGrab.UseVisualStyleBackColor = false;
            buttonGrab.Visible = false;
            buttonGrab.Click += buttonGrab_Click;
            // 
            // UcMultiImageView
            // 
            UcMultiImageView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UcMultiImageView.BackColor = SystemColors.Window;
            UcMultiImageView.BorderColor = Color.Black;
            UcMultiImageView.BorderRadius = 25D;
            UcMultiImageView.BorderSize = 1D;
            UcMultiImageView.Font = new Font("Yu Gothic UI", 12F);
            UcMultiImageView.Index = -1;
            UcMultiImageView.IsTextShrinkToFit = false;
            UcMultiImageView.LayoutType = Library.Forms.ImageView.MultiImageLayoutType.Spotlight;
            UcMultiImageView.Location = new Point(8, 56);
            UcMultiImageView.Margin = new Padding(4);
            UcMultiImageView.Name = "UcMultiImageView";
            UcMultiImageView.Nickname = "UcMultiImageView [Hutzper.Library.Forms.ImageView.MultiImageViewControl]";
            UcMultiImageView.RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            UcMultiImageView.Size = new Size(1416, 752);
            UcMultiImageView.SpotlightRatio = 0.7D;
            UcMultiImageView.TabIndex = 28;
            // 
            // UcCameraSelection
            // 
            UcCameraSelection.BackColor = SystemColors.Window;
            UcCameraSelection.BorderColor = Color.Black;
            UcCameraSelection.BorderRadius = 25D;
            UcCameraSelection.BorderSize = 1D;
            UcCameraSelection.ComboBoxFont = new Font("Yu Gothic UI", 14F);
            UcCameraSelection.ComboBoxWidth = 320;
            UcCameraSelection.Font = new Font("Yu Gothic UI", 14F);
            UcCameraSelection.Index = -1;
            UcCameraSelection.IsTextShrinkToFit = true;
            UcCameraSelection.LabelFont = new Font("Yu Gothic UI", 11F);
            UcCameraSelection.LabelForeColor = SystemColors.ControlText;
            UcCameraSelection.LabelText = "選択";
            UcCameraSelection.Location = new Point(8, 8);
            UcCameraSelection.Name = "UcCameraSelection";
            UcCameraSelection.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcCameraSelection.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcCameraSelection.SelectedIndex = -1;
            UcCameraSelection.SelectedItem = null;
            UcCameraSelection.Size = new Size(456, 40);
            UcCameraSelection.TabIndex = 31;
            // 
            // panelContainerOfImageCollection
            // 
            panelContainerOfImageCollection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panelContainerOfImageCollection.BackColor = Color.Bisque;
            panelContainerOfImageCollection.Controls.Add(ButtonChangePointLog);
            panelContainerOfImageCollection.Controls.Add(ucImageFileSuffix);
            panelContainerOfImageCollection.Controls.Add(label1);
            panelContainerOfImageCollection.Controls.Add(buttonGrab);
            panelContainerOfImageCollection.Location = new Point(1200, 16);
            panelContainerOfImageCollection.Name = "panelContainerOfImageCollection";
            panelContainerOfImageCollection.Size = new Size(400, 832);
            panelContainerOfImageCollection.TabIndex = 32;
            panelContainerOfImageCollection.Visible = false;
            // 
            // UcResultAndStatisticsDisplay
            // 
            UcResultAndStatisticsDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            UcResultAndStatisticsDisplay.BackColor = SystemColors.Control;
            UcResultAndStatisticsDisplay.BorderColor = Color.Black;
            UcResultAndStatisticsDisplay.BorderRadius = 8D;
            UcResultAndStatisticsDisplay.BorderSize = 0D;
            UcResultAndStatisticsDisplay.Font = new Font("ＭＳ ゴシック", 14F);
            UcResultAndStatisticsDisplay.Index = -1;
            UcResultAndStatisticsDisplay.IsTextShrinkToFit = false;
            UcResultAndStatisticsDisplay.Location = new Point(1440, 80);
            UcResultAndStatisticsDisplay.Name = "UcResultAndStatisticsDisplay";
            UcResultAndStatisticsDisplay.Nickname = "resultAndStatisticsDisplay1 [Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection.ResultAndStatisticsDisplay]";
            UcResultAndStatisticsDisplay.RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            UcResultAndStatisticsDisplay.Size = new Size(440, 832);
            UcResultAndStatisticsDisplay.TabIndex = 33;
            UcResultAndStatisticsDisplay.Visible = false;
            // 
            // buttonJudgementSetting
            // 
            buttonJudgementSetting.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonJudgementSetting.BackColor = Color.DodgerBlue;
            buttonJudgementSetting.BackgroundColor = Color.DodgerBlue;
            buttonJudgementSetting.BorderColor = Color.PaleVioletRed;
            buttonJudgementSetting.BorderRadius = 25;
            buttonJudgementSetting.BorderSize = 0;
            buttonJudgementSetting.FlatAppearance.BorderSize = 0;
            buttonJudgementSetting.FlatStyle = FlatStyle.Flat;
            buttonJudgementSetting.ForeColor = Color.White;
            buttonJudgementSetting.Location = new Point(1432, 8);
            buttonJudgementSetting.Name = "buttonJudgementSetting";
            buttonJudgementSetting.Size = new Size(225, 60);
            buttonJudgementSetting.TabIndex = 34;
            buttonJudgementSetting.Text = "検査設定";
            buttonJudgementSetting.TextColor = Color.White;
            buttonJudgementSetting.UseVisualStyleBackColor = false;
            buttonJudgementSetting.Click += buttonJudgementSetting_Click;
            // 
            // OperationForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1898, 968);
            ControlBox = false;
            Controls.Add(buttonJudgementSetting);
            Controls.Add(UcResultAndStatisticsDisplay);
            Controls.Add(panelContainerOfImageCollection);
            Controls.Add(UcCameraSelection);
            Controls.Add(UcMultiImageView);
            Controls.Add(ucMultiDeviceStatus);
            Controls.Add(buttonExit);
            Controls.Add(panelContainerOfLog);
            Font = new Font("Yu Gothic UI", 14F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "OperationForm";
            Nickname = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "メキキバイト";
            Shown += OperationForm_Shown;
            panelContainerOfImageCollection.ResumeLayout(false);
            panelContainerOfImageCollection.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panelContainerOfLog;
        private Library.Common.Forms.NoFocusedRoundedButton buttonExit;
        private Library.Common.Forms.NoFocusedRoundedButton ButtonChangePointLog;
        private Library.Forms.Setting.TextBoxUserControl ucImageFileSuffix;
        private Label label1;
        private Common.Diagnostics.MultiDeviceStatusUserControl ucMultiDeviceStatus;
        private Library.Common.Forms.NoFocusedRoundedButton buttonGrab;
        private Library.Forms.ImageView.MultiImageViewControl UcMultiImageView;
        private Library.Forms.Setting.ComboBoxUserControl UcCameraSelection;
        private Panel panelContainerOfImageCollection;
        private Inspection.ResultAndStatisticsDisplay UcResultAndStatisticsDisplay;
        private Library.Common.Forms.NoFocusedRoundedButton buttonJudgementSetting;
    }
}