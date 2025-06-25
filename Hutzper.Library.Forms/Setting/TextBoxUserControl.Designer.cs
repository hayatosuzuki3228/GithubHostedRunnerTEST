namespace Hutzper.Library.Forms.Setting
{
    partial class TextBoxUserControl
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
            watermarkTextbox1 = new Common.Forms.WatermarkTextbox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label1 = new Label();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // watermarkTextbox1
            // 
            watermarkTextbox1.BorderStyle = BorderStyle.FixedSingle;
            watermarkTextbox1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            watermarkTextbox1.Location = new Point(0, 0);
            watermarkTextbox1.Margin = new Padding(0);
            watermarkTextbox1.Name = "watermarkTextbox1";
            watermarkTextbox1.Size = new Size(200, 45);
            watermarkTextbox1.TabIndex = 0;
            watermarkTextbox1.WatermarkText = "";
            watermarkTextbox1.TextChanged += watermarkTextbox1_TextChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(watermarkTextbox1);
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(320, 46);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(203, 3);
            label1.Margin = new Padding(3, 3, 3, 0);
            label1.MinimumSize = new Size(72, 32);
            label1.Name = "label1";
            label1.Size = new Size(91, 38);
            label1.TabIndex = 2;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            label1.TextChanged += label1_TextChanged;
            // 
            // TextBoxUserControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            Controls.Add(flowLayoutPanel1);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4, 5, 4, 5);
            Name = "TextBoxUserControl";
            Size = new Size(320, 46);
            Resize += TextBoxUserControl_Resize;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Common.Forms.WatermarkTextbox watermarkTextbox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label1;
    }
}
