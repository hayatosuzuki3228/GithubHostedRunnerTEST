namespace Hutzper.Simulator.PlcTcpCommunication
{
    partial class MainForm
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
            panelContainerOfLog = new Panel();
            numericUpDown1 = new NumericUpDown();
            onOffUserControl1 = new Library.Forms.Setting.OnOffUserControl();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.Location = new Point(8, 56);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(784, 384);
            panelContainerOfLog.TabIndex = 5;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(176, 8);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 49152, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(112, 39);
            numericUpDown1.TabIndex = 6;
            numericUpDown1.TextAlign = HorizontalAlignment.Right;
            numericUpDown1.Value = new decimal(new int[] { 50001, 0, 0, 0 });
            // 
            // onOffUserControl1
            // 
            onOffUserControl1.BackColor = SystemColors.Window;
            onOffUserControl1.BorderColor = Color.Black;
            onOffUserControl1.BorderRadius = 25D;
            onOffUserControl1.BorderSize = 1D;
            onOffUserControl1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl1.Index = -1;
            onOffUserControl1.IsTextShrinkToFit = true;
            onOffUserControl1.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl1.LabelForeColor = SystemColors.ControlText;
            onOffUserControl1.LabelText = "listen";
            onOffUserControl1.Location = new Point(8, 8);
            onOffUserControl1.Name = "onOffUserControl1";
            onOffUserControl1.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUserControl1.OffBackColor = Color.Gray;
            onOffUserControl1.OffToggleColor = Color.Gainsboro;
            onOffUserControl1.OnBackColor = Color.DodgerBlue;
            onOffUserControl1.OnToggleColor = Color.WhiteSmoke;
            onOffUserControl1.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUserControl1.Size = new Size(160, 40);
            onOffUserControl1.TabIndex = 7;
            onOffUserControl1.Value = false;
            onOffUserControl1.ValueChanged += onOffUserControl1_ValueChanged;
            // 
            // button1
            // 
            button1.Location = new Point(296, 8);
            button1.Name = "button1";
            button1.Size = new Size(240, 40);
            button1.TabIndex = 8;
            button1.Text = "デバイスマップ";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(numericUpDown1);
            Controls.Add(onOffUserControl1);
            Controls.Add(panelContainerOfLog);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "Form1";
            Nickname = "Form1 [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelContainerOfLog;
        private NumericUpDown numericUpDown1;
        private Library.Forms.Setting.OnOffUserControl onOffUserControl1;
        private Button button1;
    }
}
