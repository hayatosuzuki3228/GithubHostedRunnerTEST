namespace Hutzper.Library.Forms
{
    partial class ConfirmationUserControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelContainerOfButton = new Hutzper.Library.Common.Forms.DoubleBufferedPanel();
            this.buttonOfOk = new Hutzper.Library.Common.Forms.RoundedButton();
            this.buttonOfCancel = new Hutzper.Library.Common.Forms.RoundedButton();
            this.textBoxOfMessages = new System.Windows.Forms.RichTextBox();
            this.pictureBoxOfIcon = new System.Windows.Forms.PictureBox();
            this.BaseLabelTitle = new System.Windows.Forms.Label();
            this.panelContainerOfButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOfIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContainerOfButton
            // 
            this.panelContainerOfButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContainerOfButton.BackColor = System.Drawing.Color.Transparent;
            this.panelContainerOfButton.Controls.Add(this.buttonOfOk);
            this.panelContainerOfButton.Controls.Add(this.buttonOfCancel);
            this.panelContainerOfButton.Location = new System.Drawing.Point(16, 232);
            this.panelContainerOfButton.Name = "panelContainerOfButton";
            this.panelContainerOfButton.Size = new System.Drawing.Size(616, 80);
            this.panelContainerOfButton.TabIndex = 5;
            // 
            // buttonOfOk
            // 
            this.buttonOfOk.BackColor = System.Drawing.Color.DodgerBlue;
            this.buttonOfOk.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.buttonOfOk.BorderColor = System.Drawing.Color.DimGray;
            this.buttonOfOk.BorderRadius = 40;
            this.buttonOfOk.BorderSize = 0;
            this.buttonOfOk.FlatAppearance.BorderSize = 0;
            this.buttonOfOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOfOk.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonOfOk.Location = new System.Drawing.Point(212, 8);
            this.buttonOfOk.Name = "buttonOfOk";
            this.buttonOfOk.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonOfOk.Size = new System.Drawing.Size(192, 60);
            this.buttonOfOk.TabIndex = 1;
            this.buttonOfOk.Text = "OK";
            this.buttonOfOk.TextColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonOfOk.UseVisualStyleBackColor = false;
            this.buttonOfOk.Visible = false;
            // 
            // buttonOfCancel
            // 
            this.buttonOfCancel.BackColor = System.Drawing.SystemColors.Window;
            this.buttonOfCancel.BackgroundColor = System.Drawing.SystemColors.Window;
            this.buttonOfCancel.BorderColor = System.Drawing.Color.DimGray;
            this.buttonOfCancel.BorderRadius = 40;
            this.buttonOfCancel.BorderSize = 1;
            this.buttonOfCancel.FlatAppearance.BorderSize = 0;
            this.buttonOfCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOfCancel.ForeColor = System.Drawing.Color.DimGray;
            this.buttonOfCancel.Location = new System.Drawing.Point(8, 8);
            this.buttonOfCancel.Name = "buttonOfCancel";
            this.buttonOfCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonOfCancel.Size = new System.Drawing.Size(192, 60);
            this.buttonOfCancel.TabIndex = 0;
            this.buttonOfCancel.Text = "キャンセル";
            this.buttonOfCancel.TextColor = System.Drawing.Color.DimGray;
            this.buttonOfCancel.UseVisualStyleBackColor = false;
            this.buttonOfCancel.Visible = false;
            // 
            // textBoxOfMessages
            // 
            this.textBoxOfMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOfMessages.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOfMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOfMessages.Font = new System.Drawing.Font("Yu Gothic UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxOfMessages.ForeColor = System.Drawing.SystemColors.InfoText;
            this.textBoxOfMessages.Location = new System.Drawing.Point(16, 56);
            this.textBoxOfMessages.Name = "textBoxOfMessages";
            this.textBoxOfMessages.ReadOnly = true;
            this.textBoxOfMessages.Size = new System.Drawing.Size(616, 168);
            this.textBoxOfMessages.TabIndex = 4;
            this.textBoxOfMessages.Text = "message";
            this.textBoxOfMessages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseDown);
            this.textBoxOfMessages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseMove);
            this.textBoxOfMessages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseUp);
            // 
            // pictureBoxOfIcon
            // 
            this.pictureBoxOfIcon.Location = new System.Drawing.Point(8, 8);
            this.pictureBoxOfIcon.Name = "pictureBoxOfIcon";
            this.pictureBoxOfIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxOfIcon.TabIndex = 7;
            this.pictureBoxOfIcon.TabStop = false;
            this.pictureBoxOfIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseDown);
            this.pictureBoxOfIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseMove);
            this.pictureBoxOfIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseUp);
            // 
            // BaseLabelTitle
            // 
            this.BaseLabelTitle.AutoSize = true;
            this.BaseLabelTitle.BackColor = System.Drawing.Color.Transparent;
            this.BaseLabelTitle.Font = new System.Drawing.Font("Yu Gothic UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BaseLabelTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BaseLabelTitle.Location = new System.Drawing.Point(48, 8);
            this.BaseLabelTitle.Name = "BaseLabelTitle";
            this.BaseLabelTitle.Size = new System.Drawing.Size(219, 38);
            this.BaseLabelTitle.TabIndex = 8;
            this.BaseLabelTitle.Text = "Hutzper window";
            this.BaseLabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BaseLabelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseDown);
            this.BaseLabelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseMove);
            this.BaseLabelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseUp);
            // 
            // ConfirmationUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.BaseLabelTitle);
            this.Controls.Add(this.pictureBoxOfIcon);
            this.Controls.Add(this.panelContainerOfButton);
            this.Controls.Add(this.textBoxOfMessages);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ConfirmationUserControl";
            this.Size = new System.Drawing.Size(640, 320);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dragableControl_MouseUp);
            this.panelContainerOfButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOfIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Common.Forms.DoubleBufferedPanel panelContainerOfButton;
        private Common.Forms.RoundedButton buttonOfOk;
        private Common.Forms.RoundedButton buttonOfCancel;
        private RichTextBox textBoxOfMessages;
        private PictureBox pictureBoxOfIcon;
        protected Label BaseLabelTitle;
    }
}
