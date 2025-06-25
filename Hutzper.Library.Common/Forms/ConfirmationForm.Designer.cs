namespace Hutzper.Library.Common.Forms
{
    partial class ConfirmationForm : HutzperForm
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
            textBoxOfMessages = new RichTextBox();
            panelContainerOfButton = new DoubleBufferedPanel();
            buttonOfOk = new RoundedButton();
            buttonOfCancel = new RoundedButton();
            pictureBoxOfIcon = new PictureBox();
            BasePanelTitleBar.SuspendLayout();
            BasePanelContainer.SuspendLayout();
            panelContainerOfButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfIcon).BeginInit();
            SuspendLayout();
            // 
            // BaseLabelTitle
            // 
            BaseLabelTitle.BackColor = Color.FromArgb(224, 224, 224);
            BaseLabelTitle.Font = new Font("Yu Gothic UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            BaseLabelTitle.Location = new Point(8, 10);
            BaseLabelTitle.Size = new Size(139, 30);
            BaseLabelTitle.Text = "Confirmation";
            // 
            // BasePanelTitleBar
            // 
            BasePanelTitleBar.BackColor = Color.FromArgb(224, 224, 224);
            BasePanelTitleBar.Size = new Size(476, 50);
            // 
            // BasePanelContainer
            // 
            BasePanelContainer.BackColor = SystemColors.Window;
            BasePanelContainer.Controls.Add(pictureBoxOfIcon);
            BasePanelContainer.Controls.Add(panelContainerOfButton);
            BasePanelContainer.Controls.Add(textBoxOfMessages);
            BasePanelContainer.Size = new Size(476, 346);
            BasePanelContainer.Controls.SetChildIndex(BasePanelTitleBar, 0);
            BasePanelContainer.Controls.SetChildIndex(textBoxOfMessages, 0);
            BasePanelContainer.Controls.SetChildIndex(panelContainerOfButton, 0);
            BasePanelContainer.Controls.SetChildIndex(pictureBoxOfIcon, 0);
            // 
            // BaseButtonMinimize
            // 
            BaseButtonMinimize.BackColor = Color.FromArgb(224, 224, 224);
            BaseButtonMinimize.BackgroundColor = Color.FromArgb(224, 224, 224);
            BaseButtonMinimize.FlatAppearance.BorderSize = 0;
            BaseButtonMinimize.Location = new Point(326, 0);
            // 
            // BaseButtonClose
            // 
            BaseButtonClose.BackColor = Color.FromArgb(224, 224, 224);
            BaseButtonClose.BackgroundColor = Color.FromArgb(224, 224, 224);
            BaseButtonClose.FlatAppearance.BorderSize = 0;
            BaseButtonClose.Location = new Point(426, 0);
            // 
            // BaseButtonMaximize
            // 
            BaseButtonMaximize.BackColor = Color.FromArgb(224, 224, 224);
            BaseButtonMaximize.BackgroundColor = Color.FromArgb(224, 224, 224);
            BaseButtonMaximize.FlatAppearance.BorderSize = 0;
            BaseButtonMaximize.Location = new Point(376, 0);
            // 
            // textBoxOfMessages
            // 
            textBoxOfMessages.BackColor = SystemColors.Window;
            textBoxOfMessages.BorderStyle = BorderStyle.None;
            textBoxOfMessages.ForeColor = SystemColors.InfoText;
            textBoxOfMessages.Location = new Point(100, 58);
            textBoxOfMessages.Name = "textBoxOfMessages";
            textBoxOfMessages.ReadOnly = true;
            textBoxOfMessages.Size = new Size(350, 198);
            textBoxOfMessages.TabIndex = 2;
            textBoxOfMessages.Text = "message";
            // 
            // panelContainerOfButton
            // 
            panelContainerOfButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfButton.BackColor = Color.Transparent;
            panelContainerOfButton.Controls.Add(buttonOfOk);
            panelContainerOfButton.Controls.Add(buttonOfCancel);
            panelContainerOfButton.Location = new Point(8, 262);
            panelContainerOfButton.Name = "panelContainerOfButton";
            panelContainerOfButton.Size = new Size(456, 80);
            panelContainerOfButton.TabIndex = 3;
            // 
            // buttonOfOk
            // 
            buttonOfOk.BackColor = Color.White;
            buttonOfOk.BackgroundColor = Color.White;
            buttonOfOk.BorderColor = Color.DimGray;
            buttonOfOk.BorderRadius = 6;
            buttonOfOk.BorderSize = 1;
            buttonOfOk.FlatAppearance.BorderSize = 0;
            buttonOfOk.FlatStyle = FlatStyle.Flat;
            buttonOfOk.ForeColor = Color.DimGray;
            buttonOfOk.Location = new Point(206, 8);
            buttonOfOk.Name = "buttonOfOk";
            buttonOfOk.RightToLeft = RightToLeft.No;
            buttonOfOk.Size = new Size(192, 60);
            buttonOfOk.TabIndex = 1;
            buttonOfOk.Text = "OK";
            buttonOfOk.TextColor = Color.DimGray;
            buttonOfOk.UseVisualStyleBackColor = false;
            buttonOfOk.Visible = false;
            // 
            // buttonOfCancel
            // 
            buttonOfCancel.BackColor = SystemColors.Window;
            buttonOfCancel.BackgroundColor = SystemColors.Window;
            buttonOfCancel.BorderColor = Color.DodgerBlue;
            buttonOfCancel.BorderRadius = 6;
            buttonOfCancel.BorderSize = 1;
            buttonOfCancel.FlatAppearance.BorderSize = 0;
            buttonOfCancel.FlatStyle = FlatStyle.Flat;
            buttonOfCancel.ForeColor = Color.DimGray;
            buttonOfCancel.Location = new Point(8, 8);
            buttonOfCancel.Name = "buttonOfCancel";
            buttonOfCancel.RightToLeft = RightToLeft.No;
            buttonOfCancel.Size = new Size(192, 60);
            buttonOfCancel.TabIndex = 0;
            buttonOfCancel.Text = "キャンセル";
            buttonOfCancel.TextColor = Color.DimGray;
            buttonOfCancel.UseVisualStyleBackColor = false;
            buttonOfCancel.Visible = false;
            // 
            // pictureBoxOfIcon
            // 
            pictureBoxOfIcon.Location = new Point(16, 102);
            pictureBoxOfIcon.Name = "pictureBoxOfIcon";
            pictureBoxOfIcon.Size = new Size(60, 60);
            pictureBoxOfIcon.TabIndex = 6;
            pictureBoxOfIcon.TabStop = false;
            // 
            // ConfirmationForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            BorderColor = Color.FromArgb(224, 224, 224);
            BorderRadius = 6;
            CancelButton = buttonOfCancel;
            ClientSize = new Size(480, 350);
            Name = "ConfirmationForm";
            Nickname = "ConfirmationForm [Hutzper.Library.Common.Forms.HutzperForm], Text: Confirmation";
            Text = "Confirmation";
            TitleFont = new Font("Yu Gothic UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            Shown += ConfirmationForm_Shown;
            BasePanelTitleBar.ResumeLayout(false);
            BasePanelTitleBar.PerformLayout();
            BasePanelContainer.ResumeLayout(false);
            panelContainerOfButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOfIcon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox textBoxOfMessages;
        private DoubleBufferedPanel panelContainerOfButton;
        private RoundedButton buttonOfOk;
        private RoundedButton buttonOfCancel;
        private PictureBox pictureBoxOfIcon;
    }
}