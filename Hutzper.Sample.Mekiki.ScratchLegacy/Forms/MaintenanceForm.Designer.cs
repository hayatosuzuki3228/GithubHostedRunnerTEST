namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    partial class MaintenanceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaintenanceForm));
            panelContainerOfLog = new Panel();
            onOffUcCameraLive = new Library.Forms.Setting.OnOffUserControl();
            buttonExit = new Library.Common.Forms.NoFocusedRoundedButton(components);
            onOffUcTestRunning = new Library.Forms.Setting.OnOffUserControl();
            histoUc = new Library.Forms.ImageProcessing.HistogramUserControl();
            buttonSaveConfig = new Library.Common.Forms.NoFocusedRoundedButton(components);
            btnSaveImage = new Library.Common.Forms.NoFocusedRoundedButton(components);
            onOffUc1ClickSave = new Library.Forms.Setting.OnOffUserControl();
            onOffUcViewCenter = new Library.Forms.Setting.OnOffUserControl();
            onOffUcViewProjection = new Library.Forms.Setting.OnOffUserControl();
            saveImageFileDialog = new SaveFileDialog();
            ucMultiDeviceStatus = new Common.Diagnostics.MultiDeviceStatusUserControl();
            onOffUcDistanceMeasurement = new Library.Forms.Setting.OnOffUserControl();
            UcMultiImageView = new Library.Forms.ImageView.MultiImageViewControl();
            onOffUcViewPseudo = new Library.Forms.Setting.OnOffUserControl();
            buttonGrabberParameter = new Library.Common.Forms.NoFocusedRoundedButton(components);
            UcCameraSelection = new Library.Forms.Setting.ComboBoxUserControl();
            buttonLightParameter = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonManualTrigger = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonLineSensorCorrection = new Library.Common.Forms.NoFocusedRoundedButton(components);
            SuspendLayout();
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.Location = new Point(8, 744);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(968, 168);
            panelContainerOfLog.TabIndex = 9;
            // 
            // onOffUcCameraLive
            // 
            onOffUcCameraLive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            onOffUcCameraLive.BackColor = SystemColors.Control;
            onOffUcCameraLive.BorderColor = Color.Black;
            onOffUcCameraLive.BorderRadius = 25D;
            onOffUcCameraLive.BorderSize = 1D;
            onOffUcCameraLive.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcCameraLive.Index = -1;
            onOffUcCameraLive.IsTextShrinkToFit = true;
            onOffUcCameraLive.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcCameraLive.LabelForeColor = SystemColors.ControlText;
            onOffUcCameraLive.LabelText = "ライブ撮影";
            onOffUcCameraLive.Location = new Point(1672, 560);
            onOffUcCameraLive.Name = "onOffUcCameraLive";
            onOffUcCameraLive.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcCameraLive.OffBackColor = Color.Gray;
            onOffUcCameraLive.OffToggleColor = Color.Gainsboro;
            onOffUcCameraLive.OnBackColor = Color.DodgerBlue;
            onOffUcCameraLive.OnToggleColor = Color.WhiteSmoke;
            onOffUcCameraLive.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcCameraLive.Size = new Size(221, 48);
            onOffUcCameraLive.TabIndex = 12;
            onOffUcCameraLive.Value = false;
            onOffUcCameraLive.ValueChanged += onOffUcLive_ValueChanged;
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
            buttonExit.TabIndex = 12;
            buttonExit.Text = "終了";
            buttonExit.TextColor = Color.White;
            buttonExit.UseVisualStyleBackColor = false;
            buttonExit.Click += buttonExit_Click;
            // 
            // onOffUcTestRunning
            // 
            onOffUcTestRunning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            onOffUcTestRunning.BackColor = SystemColors.Control;
            onOffUcTestRunning.BorderColor = Color.Black;
            onOffUcTestRunning.BorderRadius = 25D;
            onOffUcTestRunning.BorderSize = 1D;
            onOffUcTestRunning.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcTestRunning.Index = -1;
            onOffUcTestRunning.IsTextShrinkToFit = true;
            onOffUcTestRunning.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcTestRunning.LabelForeColor = SystemColors.ControlText;
            onOffUcTestRunning.LabelText = "テスト実行";
            onOffUcTestRunning.Location = new Point(1672, 616);
            onOffUcTestRunning.Margin = new Padding(0);
            onOffUcTestRunning.Name = "onOffUcTestRunning";
            onOffUcTestRunning.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcTestRunning.OffBackColor = Color.Gray;
            onOffUcTestRunning.OffToggleColor = Color.Gainsboro;
            onOffUcTestRunning.OnBackColor = Color.DodgerBlue;
            onOffUcTestRunning.OnToggleColor = Color.WhiteSmoke;
            onOffUcTestRunning.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcTestRunning.Size = new Size(216, 48);
            onOffUcTestRunning.TabIndex = 17;
            onOffUcTestRunning.Value = false;
            onOffUcTestRunning.ValueChanged += onOffUcTestRunning_ValueChanged;
            // 
            // histoUc
            // 
            histoUc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            histoUc.BackColor = Color.Black;
            histoUc.BorderColor = Color.Black;
            histoUc.BorderRadius = 25D;
            histoUc.BorderSize = 1D;
            histoUc.CursorColor = Color.LightGoldenrodYellow;
            histoUc.CursorOverlayEnalbed = true;
            histoUc.HorizontalRangeMaximum = 255;
            histoUc.HorizontalRangeMinimum = 0;
            histoUc.Index = -1;
            histoUc.IsTextShrinkToFit = false;
            histoUc.Location = new Point(984, 16);
            histoUc.MinimumSize = new Size(320, 200);
            histoUc.Name = "histoUc";
            histoUc.Nickname = "histogramUserControl1 [Hutzper.Library.Forms.ImageProcessing.HistogramUserControl]";
            histoUc.PseudColorEnabled = false;
            histoUc.PseudColorInverted = false;
            histoUc.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            histoUc.Size = new Size(672, 712);
            histoUc.TabIndex = 15;
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
            buttonSaveConfig.Location = new Point(1664, 80);
            buttonSaveConfig.Name = "buttonSaveConfig";
            buttonSaveConfig.Size = new Size(225, 60);
            buttonSaveConfig.TabIndex = 16;
            buttonSaveConfig.Text = "設定保存";
            buttonSaveConfig.TextColor = Color.White;
            buttonSaveConfig.UseVisualStyleBackColor = false;
            buttonSaveConfig.Click += buttonSave_Click;
            // 
            // btnSaveImage
            // 
            btnSaveImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveImage.BackColor = Color.DodgerBlue;
            btnSaveImage.BackgroundColor = Color.DodgerBlue;
            btnSaveImage.BorderColor = Color.PaleVioletRed;
            btnSaveImage.BorderRadius = 25;
            btnSaveImage.BorderSize = 0;
            btnSaveImage.FlatAppearance.BorderSize = 0;
            btnSaveImage.FlatStyle = FlatStyle.Flat;
            btnSaveImage.ForeColor = Color.White;
            btnSaveImage.Location = new Point(984, 744);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(225, 60);
            btnSaveImage.TabIndex = 17;
            btnSaveImage.Text = "画像保存";
            btnSaveImage.TextColor = Color.White;
            btnSaveImage.UseVisualStyleBackColor = false;
            btnSaveImage.Click += btnSaveImage_Click;
            // 
            // onOffUc1ClickSave
            // 
            onOffUc1ClickSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            onOffUc1ClickSave.BackColor = SystemColors.Control;
            onOffUc1ClickSave.BorderColor = Color.Black;
            onOffUc1ClickSave.BorderRadius = 25D;
            onOffUc1ClickSave.BorderSize = 1D;
            onOffUc1ClickSave.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUc1ClickSave.Index = -1;
            onOffUc1ClickSave.IsTextShrinkToFit = true;
            onOffUc1ClickSave.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUc1ClickSave.LabelForeColor = SystemColors.ControlText;
            onOffUc1ClickSave.LabelText = "1クリック保存";
            onOffUc1ClickSave.Location = new Point(1216, 752);
            onOffUc1ClickSave.Margin = new Padding(0);
            onOffUc1ClickSave.Name = "onOffUc1ClickSave";
            onOffUc1ClickSave.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUc1ClickSave.OffBackColor = Color.Gray;
            onOffUc1ClickSave.OffToggleColor = Color.Gainsboro;
            onOffUc1ClickSave.OnBackColor = Color.DodgerBlue;
            onOffUc1ClickSave.OnToggleColor = Color.WhiteSmoke;
            onOffUc1ClickSave.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUc1ClickSave.Size = new Size(216, 48);
            onOffUc1ClickSave.TabIndex = 18;
            onOffUc1ClickSave.Value = true;
            // 
            // onOffUcViewCenter
            // 
            onOffUcViewCenter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            onOffUcViewCenter.BackColor = SystemColors.Control;
            onOffUcViewCenter.BorderColor = Color.Black;
            onOffUcViewCenter.BorderRadius = 25D;
            onOffUcViewCenter.BorderSize = 1D;
            onOffUcViewCenter.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewCenter.Index = -1;
            onOffUcViewCenter.IsTextShrinkToFit = true;
            onOffUcViewCenter.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewCenter.LabelForeColor = SystemColors.ControlText;
            onOffUcViewCenter.LabelText = "中心線を表示する";
            onOffUcViewCenter.Location = new Point(984, 824);
            onOffUcViewCenter.Margin = new Padding(0);
            onOffUcViewCenter.Name = "onOffUcViewCenter";
            onOffUcViewCenter.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcViewCenter.OffBackColor = Color.Gray;
            onOffUcViewCenter.OffToggleColor = Color.Gainsboro;
            onOffUcViewCenter.OnBackColor = Color.DodgerBlue;
            onOffUcViewCenter.OnToggleColor = Color.WhiteSmoke;
            onOffUcViewCenter.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcViewCenter.Size = new Size(264, 48);
            onOffUcViewCenter.TabIndex = 19;
            onOffUcViewCenter.Value = true;
            onOffUcViewCenter.ValueChanged += onOffUcViewOption_ValueChanged;
            // 
            // onOffUcViewProjection
            // 
            onOffUcViewProjection.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            onOffUcViewProjection.BackColor = SystemColors.Control;
            onOffUcViewProjection.BorderColor = Color.Black;
            onOffUcViewProjection.BorderRadius = 25D;
            onOffUcViewProjection.BorderSize = 1D;
            onOffUcViewProjection.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewProjection.Index = -1;
            onOffUcViewProjection.IsTextShrinkToFit = true;
            onOffUcViewProjection.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewProjection.LabelForeColor = SystemColors.ControlText;
            onOffUcViewProjection.LabelText = "輝度投影を表示する";
            onOffUcViewProjection.Location = new Point(984, 872);
            onOffUcViewProjection.Margin = new Padding(0);
            onOffUcViewProjection.Name = "onOffUcViewProjection";
            onOffUcViewProjection.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcViewProjection.OffBackColor = Color.Gray;
            onOffUcViewProjection.OffToggleColor = Color.Gainsboro;
            onOffUcViewProjection.OnBackColor = Color.DodgerBlue;
            onOffUcViewProjection.OnToggleColor = Color.WhiteSmoke;
            onOffUcViewProjection.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcViewProjection.Size = new Size(264, 48);
            onOffUcViewProjection.TabIndex = 20;
            onOffUcViewProjection.Value = false;
            onOffUcViewProjection.ValueChanged += onOffUcViewOption_ValueChanged;
            // 
            // ucMultiDeviceStatus
            // 
            ucMultiDeviceStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ucMultiDeviceStatus.ItemWidth = 160;
            ucMultiDeviceStatus.Location = new Point(8, 920);
            ucMultiDeviceStatus.Name = "ucMultiDeviceStatus";
            ucMultiDeviceStatus.Size = new Size(976, 48);
            ucMultiDeviceStatus.TabIndex = 24;
            // 
            // onOffUcDistanceMeasurement
            // 
            onOffUcDistanceMeasurement.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            onOffUcDistanceMeasurement.BackColor = SystemColors.Control;
            onOffUcDistanceMeasurement.BorderColor = Color.Black;
            onOffUcDistanceMeasurement.BorderRadius = 25D;
            onOffUcDistanceMeasurement.BorderSize = 1D;
            onOffUcDistanceMeasurement.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcDistanceMeasurement.Index = -1;
            onOffUcDistanceMeasurement.IsTextShrinkToFit = true;
            onOffUcDistanceMeasurement.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcDistanceMeasurement.LabelForeColor = SystemColors.ControlText;
            onOffUcDistanceMeasurement.LabelText = "距離を計測する";
            onOffUcDistanceMeasurement.Location = new Point(1248, 824);
            onOffUcDistanceMeasurement.Margin = new Padding(0);
            onOffUcDistanceMeasurement.Name = "onOffUcDistanceMeasurement";
            onOffUcDistanceMeasurement.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcDistanceMeasurement.OffBackColor = Color.Gray;
            onOffUcDistanceMeasurement.OffToggleColor = Color.Gainsboro;
            onOffUcDistanceMeasurement.OnBackColor = Color.DodgerBlue;
            onOffUcDistanceMeasurement.OnToggleColor = Color.WhiteSmoke;
            onOffUcDistanceMeasurement.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcDistanceMeasurement.Size = new Size(264, 48);
            onOffUcDistanceMeasurement.TabIndex = 25;
            onOffUcDistanceMeasurement.Value = false;
            onOffUcDistanceMeasurement.ValueChanged += onOffUcViewOption_ValueChanged;
            // 
            // UcMultiImageView
            // 
            UcMultiImageView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UcMultiImageView.BackColor = SystemColors.Window;
            UcMultiImageView.BorderColor = Color.Black;
            UcMultiImageView.BorderRadius = 25D;
            UcMultiImageView.BorderSize = 1D;
            UcMultiImageView.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcMultiImageView.Index = -1;
            UcMultiImageView.IsTextShrinkToFit = false;
            UcMultiImageView.LayoutType = Library.Forms.ImageView.MultiImageLayoutType.Spotlight;
            UcMultiImageView.Location = new Point(8, 56);
            UcMultiImageView.Margin = new Padding(4);
            UcMultiImageView.Name = "UcMultiImageView";
            UcMultiImageView.Nickname = "UcMultiImageView [Hutzper.Library.Forms.ImageView.MultiImageViewControl]";
            UcMultiImageView.RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            UcMultiImageView.SelectedIndex = -1;
            UcMultiImageView.Size = new Size(968, 672);
            UcMultiImageView.SpotlightRatio = 0.7D;
            UcMultiImageView.TabIndex = 27;
            // 
            // onOffUcViewPseudo
            // 
            onOffUcViewPseudo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            onOffUcViewPseudo.BackColor = SystemColors.Control;
            onOffUcViewPseudo.BorderColor = Color.Black;
            onOffUcViewPseudo.BorderRadius = 25D;
            onOffUcViewPseudo.BorderSize = 1D;
            onOffUcViewPseudo.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewPseudo.Index = -1;
            onOffUcViewPseudo.IsTextShrinkToFit = true;
            onOffUcViewPseudo.LabelFont = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcViewPseudo.LabelForeColor = SystemColors.ControlText;
            onOffUcViewPseudo.LabelText = "疑似カラー表示";
            onOffUcViewPseudo.Location = new Point(1248, 872);
            onOffUcViewPseudo.Margin = new Padding(0);
            onOffUcViewPseudo.Name = "onOffUcViewPseudo";
            onOffUcViewPseudo.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcViewPseudo.OffBackColor = Color.Gray;
            onOffUcViewPseudo.OffToggleColor = Color.Gainsboro;
            onOffUcViewPseudo.OnBackColor = Color.DodgerBlue;
            onOffUcViewPseudo.OnToggleColor = Color.WhiteSmoke;
            onOffUcViewPseudo.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcViewPseudo.Size = new Size(264, 48);
            onOffUcViewPseudo.TabIndex = 28;
            onOffUcViewPseudo.Value = false;
            onOffUcViewPseudo.ValueChanged += onOffUcViewOption_ValueChanged;
            // 
            // buttonGrabberParameter
            // 
            buttonGrabberParameter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonGrabberParameter.BackColor = Color.DodgerBlue;
            buttonGrabberParameter.BackgroundColor = Color.DodgerBlue;
            buttonGrabberParameter.BorderColor = Color.PaleVioletRed;
            buttonGrabberParameter.BorderRadius = 25;
            buttonGrabberParameter.BorderSize = 0;
            buttonGrabberParameter.FlatAppearance.BorderSize = 0;
            buttonGrabberParameter.FlatStyle = FlatStyle.Flat;
            buttonGrabberParameter.ForeColor = Color.White;
            buttonGrabberParameter.Location = new Point(1664, 232);
            buttonGrabberParameter.Name = "buttonGrabberParameter";
            buttonGrabberParameter.Size = new Size(225, 60);
            buttonGrabberParameter.TabIndex = 29;
            buttonGrabberParameter.Text = "カメラ設定";
            buttonGrabberParameter.TextColor = Color.White;
            buttonGrabberParameter.UseVisualStyleBackColor = false;
            buttonGrabberParameter.Click += buttonShowParameter;
            // 
            // UcCameraSelection
            // 
            UcCameraSelection.BackColor = SystemColors.Window;
            UcCameraSelection.BorderColor = Color.Black;
            UcCameraSelection.BorderRadius = 25D;
            UcCameraSelection.BorderSize = 1D;
            UcCameraSelection.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcCameraSelection.ComboBoxWidth = 320;
            UcCameraSelection.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcCameraSelection.Index = -1;
            UcCameraSelection.IsTextShrinkToFit = true;
            UcCameraSelection.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcCameraSelection.LabelForeColor = SystemColors.ControlText;
            UcCameraSelection.LabelText = "選択";
            UcCameraSelection.Location = new Point(8, 8);
            UcCameraSelection.Name = "UcCameraSelection";
            UcCameraSelection.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcCameraSelection.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcCameraSelection.SelectedIndex = -1;
            UcCameraSelection.SelectedItem = null;
            UcCameraSelection.Size = new Size(456, 40);
            UcCameraSelection.TabIndex = 30;
            // 
            // buttonLightParameter
            // 
            buttonLightParameter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonLightParameter.BackColor = Color.DodgerBlue;
            buttonLightParameter.BackgroundColor = Color.DodgerBlue;
            buttonLightParameter.BorderColor = Color.PaleVioletRed;
            buttonLightParameter.BorderRadius = 25;
            buttonLightParameter.BorderSize = 0;
            buttonLightParameter.FlatAppearance.BorderSize = 0;
            buttonLightParameter.FlatStyle = FlatStyle.Flat;
            buttonLightParameter.ForeColor = Color.White;
            buttonLightParameter.Location = new Point(1664, 304);
            buttonLightParameter.Name = "buttonLightParameter";
            buttonLightParameter.Size = new Size(225, 60);
            buttonLightParameter.TabIndex = 31;
            buttonLightParameter.Text = "照明設定";
            buttonLightParameter.TextColor = Color.White;
            buttonLightParameter.UseVisualStyleBackColor = false;
            buttonLightParameter.Click += buttonShowParameter;
            // 
            // buttonManualTrigger
            // 
            buttonManualTrigger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonManualTrigger.BackColor = Color.DodgerBlue;
            buttonManualTrigger.BackgroundColor = Color.DodgerBlue;
            buttonManualTrigger.BorderColor = Color.PaleVioletRed;
            buttonManualTrigger.BorderRadius = 25;
            buttonManualTrigger.BorderSize = 0;
            buttonManualTrigger.FlatAppearance.BorderSize = 0;
            buttonManualTrigger.FlatStyle = FlatStyle.Flat;
            buttonManualTrigger.ForeColor = Color.White;
            buttonManualTrigger.Location = new Point(1664, 672);
            buttonManualTrigger.Name = "buttonManualTrigger";
            buttonManualTrigger.Size = new Size(225, 60);
            buttonManualTrigger.TabIndex = 32;
            buttonManualTrigger.Text = "テストトリガー";
            buttonManualTrigger.TextColor = Color.White;
            buttonManualTrigger.UseVisualStyleBackColor = false;
            buttonManualTrigger.Visible = false;
            buttonManualTrigger.Click += buttonManualTrigger_Click;
            // 
            // buttonLineSensorCorrection
            // 
            buttonLineSensorCorrection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonLineSensorCorrection.BackColor = Color.DodgerBlue;
            buttonLineSensorCorrection.BackgroundColor = Color.DodgerBlue;
            buttonLineSensorCorrection.BorderColor = Color.PaleVioletRed;
            buttonLineSensorCorrection.BorderRadius = 25;
            buttonLineSensorCorrection.BorderSize = 0;
            buttonLineSensorCorrection.FlatAppearance.BorderSize = 0;
            buttonLineSensorCorrection.FlatStyle = FlatStyle.Flat;
            buttonLineSensorCorrection.ForeColor = Color.White;
            buttonLineSensorCorrection.Location = new Point(1664, 744);
            buttonLineSensorCorrection.Name = "buttonLineSensorCorrection";
            buttonLineSensorCorrection.Size = new Size(225, 60);
            buttonLineSensorCorrection.TabIndex = 33;
            buttonLineSensorCorrection.Text = "ラインセンサ補正";
            buttonLineSensorCorrection.TextColor = Color.White;
            buttonLineSensorCorrection.UseVisualStyleBackColor = false;
            buttonLineSensorCorrection.Click += buttonLineSensorCorrection_Click;
            // 
            // MaintenanceForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1898, 968);
            ControlBox = false;
            Controls.Add(buttonLineSensorCorrection);
            Controls.Add(buttonManualTrigger);
            Controls.Add(onOffUcTestRunning);
            Controls.Add(onOffUcCameraLive);
            Controls.Add(buttonLightParameter);
            Controls.Add(UcCameraSelection);
            Controls.Add(buttonGrabberParameter);
            Controls.Add(onOffUcViewPseudo);
            Controls.Add(UcMultiImageView);
            Controls.Add(onOffUcDistanceMeasurement);
            Controls.Add(ucMultiDeviceStatus);
            Controls.Add(onOffUcViewProjection);
            Controls.Add(onOffUcViewCenter);
            Controls.Add(onOffUc1ClickSave);
            Controls.Add(btnSaveImage);
            Controls.Add(buttonSaveConfig);
            Controls.Add(histoUc);
            Controls.Add(buttonExit);
            Controls.Add(panelContainerOfLog);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MaintenanceForm";
            Nickname = "MaintenanceForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: MaintenanceForm";
            Text = "撮影調整";
            Shown += MaintenanceForm_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Panel panelContainerOfLog;
        private Library.Common.Forms.NoFocusedRoundedButton buttonExit;
        private Library.Forms.Setting.OnOffUserControl onOffUcCameraLive;
        private Library.Forms.ImageProcessing.HistogramUserControl histoUc;
        private Library.Common.Forms.NoFocusedRoundedButton buttonSaveConfig;
        private Library.Forms.Setting.OnOffUserControl onOffUcTestRunning;
        private Library.Common.Forms.NoFocusedRoundedButton btnSaveImage;
        private Library.Forms.Setting.OnOffUserControl onOffUc1ClickSave;
        private Library.Forms.Setting.OnOffUserControl onOffUcViewCenter;
        private Library.Forms.Setting.OnOffUserControl onOffUcViewProjection;
        private SaveFileDialog saveImageFileDialog;
        private Common.Diagnostics.MultiDeviceStatusUserControl ucMultiDeviceStatus;
        private Library.Forms.Setting.OnOffUserControl onOffUcDistanceMeasurement;
        private Library.Forms.ImageView.MultiImageViewControl UcMultiImageView;
        private Library.Forms.Setting.OnOffUserControl onOffUcViewPseudo;
        private Library.Common.Forms.NoFocusedRoundedButton buttonGrabberParameter;
        private Library.Forms.Setting.ComboBoxUserControl UcCameraSelection;
        private Library.Common.Forms.NoFocusedRoundedButton buttonLightParameter;
        private Library.Common.Forms.NoFocusedRoundedButton buttonManualTrigger;
        private Library.Common.Forms.NoFocusedRoundedButton buttonLineSensorCorrection;
    }
}