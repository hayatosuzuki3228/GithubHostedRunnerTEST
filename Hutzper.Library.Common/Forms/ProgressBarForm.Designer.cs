namespace Hutzper.Library.Common.Forms
{
    partial class ProgressBarForm : HutzperForm
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
            textBoxInfoMessage = new TextBox();
            progressBar = new ProgressBar();
            buttonCancel = new Button();
            buttonGeneral = new Button();
            timer = new System.Windows.Forms.Timer(components);
            labelTimeInfo = new Label();
            BasePanelTitleBar.SuspendLayout();
            BasePanelContainer.SuspendLayout();
            SuspendLayout();
            // 
            // BaseLabelTitle
            // 
            BaseLabelTitle.Location = new Point(17, 17);
            BaseLabelTitle.Size = new Size(94, 21);
            BaseLabelTitle.Text = "Processing...";
            // 
            // BasePanelTitleBar
            // 
            BasePanelTitleBar.Size = new Size(614, 50);
            // 
            // BasePanelContainer
            // 
            BasePanelContainer.BackColor = SystemColors.Window;
            BasePanelContainer.Controls.Add(buttonGeneral);
            BasePanelContainer.Controls.Add(buttonCancel);
            BasePanelContainer.Controls.Add(progressBar);
            BasePanelContainer.Controls.Add(labelTimeInfo);
            BasePanelContainer.Controls.Add(textBoxInfoMessage);
            BasePanelContainer.Size = new Size(614, 316);
            BasePanelContainer.Controls.SetChildIndex(textBoxInfoMessage, 0);
            BasePanelContainer.Controls.SetChildIndex(BasePanelTitleBar, 0);
            BasePanelContainer.Controls.SetChildIndex(labelTimeInfo, 0);
            BasePanelContainer.Controls.SetChildIndex(progressBar, 0);
            BasePanelContainer.Controls.SetChildIndex(buttonCancel, 0);
            BasePanelContainer.Controls.SetChildIndex(buttonGeneral, 0);
            // 
            // BaseButtonMinimize
            // 
            BaseButtonMinimize.FlatAppearance.BorderSize = 0;
            // 
            // BaseButtonClose
            // 
            BaseButtonClose.FlatAppearance.BorderSize = 0;
            // 
            // BaseButtonMaximize
            // 
            BaseButtonMaximize.FlatAppearance.BorderSize = 0;
            // 
            // textBoxInfoMessage
            // 
            textBoxInfoMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxInfoMessage.BackColor = SystemColors.Window;
            textBoxInfoMessage.BorderStyle = BorderStyle.None;
            textBoxInfoMessage.ForeColor = SystemColors.ControlText;
            textBoxInfoMessage.Location = new Point(8, 56);
            textBoxInfoMessage.Margin = new Padding(0);
            textBoxInfoMessage.Multiline = true;
            textBoxInfoMessage.Name = "textBoxInfoMessage";
            textBoxInfoMessage.ReadOnly = true;
            textBoxInfoMessage.ScrollBars = ScrollBars.Both;
            textBoxInfoMessage.Size = new Size(600, 152);
            textBoxInfoMessage.TabIndex = 4;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(8, 216);
            progressBar.Margin = new Padding(0);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(600, 40);
            progressBar.TabIndex = 3;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Location = new Point(400, 256);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(192, 60);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "キャンセル";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonGeneral
            // 
            buttonGeneral.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonGeneral.Location = new Point(208, 256);
            buttonGeneral.Name = "buttonGeneral";
            buttonGeneral.Size = new Size(192, 60);
            buttonGeneral.TabIndex = 6;
            buttonGeneral.Text = "-";
            buttonGeneral.UseVisualStyleBackColor = true;
            buttonGeneral.Visible = false;
            buttonGeneral.Click += buttonGeneral_Click;
            // 
            // timer
            // 
            timer.Tick += timer_Tick;
            // 
            // labelTimeInfo
            // 
            labelTimeInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelTimeInfo.AutoSize = true;
            labelTimeInfo.Location = new Point(16, 264);
            labelTimeInfo.Name = "labelTimeInfo";
            labelTimeInfo.Size = new Size(95, 30);
            labelTimeInfo.TabIndex = 2;
            labelTimeInfo.Text = "00:00:00";
            labelTimeInfo.Visible = false;
            // 
            // ProgressBarForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(618, 320);
            CloseButtonVisible = false;
            ControlBox = false;
            MaximizeButtonVisible = false;
            MinimizeButtonVisible = false;
            Name = "ProgressBarForm";
            Nickname = "ProgressBarForm [Hutzper.Library.Common.Forms.HutzperForm], Text: Processing...";
            Text = "Processing...";
            Shown += ProgressBarForm_Shown;
            BasePanelTitleBar.ResumeLayout(false);
            BasePanelTitleBar.PerformLayout();
            BasePanelContainer.ResumeLayout(false);
            BasePanelContainer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBoxInfoMessage;
        private ProgressBar progressBar;
        private Button buttonCancel;
        private Button buttonGeneral;
        private System.Windows.Forms.Timer timer;
        private Label labelTimeInfo;
    }
}