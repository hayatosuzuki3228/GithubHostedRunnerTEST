namespace Hutzper.Library.Forms.Setting
{
    partial class DirectorySelectionUserControl
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
            components = new System.ComponentModel.Container();
            flowLayoutPanel1 = new FlowLayoutPanel();
            watermarkTextbox1 = new Common.Forms.WatermarkTextbox();
            noFocusedButton1 = new Common.Forms.NoFocusedButton(components);
            label1 = new Label();
            folderBrowserDialog = new FolderBrowserDialog();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(watermarkTextbox1);
            flowLayoutPanel1.Controls.Add(noFocusedButton1);
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(460, 46);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // watermarkTextbox1
            // 
            watermarkTextbox1.BorderStyle = BorderStyle.FixedSingle;
            watermarkTextbox1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            watermarkTextbox1.Location = new Point(0, 0);
            watermarkTextbox1.Margin = new Padding(0);
            watermarkTextbox1.Name = "watermarkTextbox1";
            watermarkTextbox1.ReadOnly = true;
            watermarkTextbox1.Size = new Size(280, 45);
            watermarkTextbox1.TabIndex = 0;
            watermarkTextbox1.WatermarkText = "";
            watermarkTextbox1.TextChanged += watermarkTextbox1_TextChanged;
            watermarkTextbox1.DoubleClick += watermarkTextbox1_DoubleClick;
            // 
            // noFocusedButton1
            // 
            noFocusedButton1.Location = new Point(280, 0);
            noFocusedButton1.Margin = new Padding(0);
            noFocusedButton1.Name = "noFocusedButton1";
            noFocusedButton1.Size = new Size(64, 46);
            noFocusedButton1.TabIndex = 3;
            noFocusedButton1.Text = "...";
            noFocusedButton1.UseVisualStyleBackColor = true;
            noFocusedButton1.Click += noFocusedButton1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(347, 3);
            label1.Margin = new Padding(3, 3, 3, 0);
            label1.MinimumSize = new Size(72, 32);
            label1.Name = "label1";
            label1.Size = new Size(91, 38);
            label1.TabIndex = 2;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            label1.TextChanged += label1_TextChanged;
            // 
            // DirectorySelectionUserControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            Controls.Add(flowLayoutPanel1);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "DirectorySelectionUserControl";
            Size = new Size(460, 46);
            Resize += DirectorySelectionUserControl_Resize;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Common.Forms.WatermarkTextbox watermarkTextbox1;
        private Label label1;
        private Common.Forms.NoFocusedButton noFocusedButton1;
        private FolderBrowserDialog folderBrowserDialog;
    }
}
