namespace Hutzper.Library.Common.Forms
{
    partial class NewProgressBarForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProgressBarForm));
            progressBar = new ProgressBar();
            panel1 = new Panel();
            textBoxTitleMessage = new TextBox();
            textBoxTextMessage = new TextBox();
            buttonGeneral = new Button();
            labelTimeInfo = new Label();
            buttonCancel = new Button();
            timer = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 93);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(600, 22);
            progressBar.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(textBoxTitleMessage);
            panel1.Controls.Add(textBoxTextMessage);
            panel1.Controls.Add(progressBar);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(624, 130);
            panel1.TabIndex = 1;
            // 
            // textBoxTitleMessage
            // 
            textBoxTitleMessage.BorderStyle = BorderStyle.None;
            textBoxTitleMessage.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            textBoxTitleMessage.ForeColor = Color.FromArgb(145, 198, 238);
            textBoxTitleMessage.Location = new Point(12, 12);
            textBoxTitleMessage.Name = "textBoxTitleMessage";
            textBoxTitleMessage.Size = new Size(600, 28);
            textBoxTitleMessage.TabIndex = 4;
            textBoxTitleMessage.Text = "TitleMessage";
            // 
            // textBoxTextMessage
            // 
            textBoxTextMessage.BorderStyle = BorderStyle.None;
            textBoxTextMessage.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxTextMessage.Location = new Point(12, 52);
            textBoxTextMessage.Name = "textBoxTextMessage";
            textBoxTextMessage.Size = new Size(600, 22);
            textBoxTextMessage.TabIndex = 3;
            textBoxTextMessage.Text = "TextMessage";
            // 
            // buttonGeneral
            // 
            buttonGeneral.Location = new Point(366, 138);
            buttonGeneral.Name = "buttonGeneral";
            buttonGeneral.Size = new Size(120, 35);
            buttonGeneral.TabIndex = 2;
            buttonGeneral.Text = "汎用ボタン";
            buttonGeneral.UseVisualStyleBackColor = true;
            // 
            // labelTimeInfo
            // 
            labelTimeInfo.AutoSize = true;
            labelTimeInfo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            labelTimeInfo.Location = new Point(12, 143);
            labelTimeInfo.Name = "labelTimeInfo";
            labelTimeInfo.Size = new Size(78, 21);
            labelTimeInfo.TabIndex = 3;
            labelTimeInfo.Text = "経過時間";
            labelTimeInfo.Visible = false;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(492, 138);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(120, 35);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "キャンセル";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // timer
            // 
            timer.Tick += timer_Tick;
            // 
            // NewProgressBarForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 181);
            ControlBox = false;
            Controls.Add(buttonCancel);
            Controls.Add(labelTimeInfo);
            Controls.Add(buttonGeneral);
            Controls.Add(panel1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "NewProgressBarForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "メキキバイト";
            TopMost = true;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar;
        private Panel panel1;
        private Button buttonGeneral;
        private TextBox textBoxTitleMessage;
        private TextBox textBoxTextMessage;
        private Label labelTimeInfo;
        private Button buttonCancel;
        private System.Windows.Forms.Timer timer;
    }
}