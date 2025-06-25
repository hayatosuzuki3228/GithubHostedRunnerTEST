namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common.Diagnostics
{
    partial class MultiDeviceStatusUserControl
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
            flpContainer = new FlowLayoutPanel();
            label1 = new Label();
            SuspendLayout();
            // 
            // flpContainer
            // 
            flpContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpContainer.Location = new Point(176, 0);
            flpContainer.Name = "flpContainer";
            flpContainer.Size = new Size(624, 64);
            flpContainer.TabIndex = 0;
            flpContainer.SizeChanged += FlpContainer_SizeChanged;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Left;
            label1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(3, 3, 3, 0);
            label1.MinimumSize = new Size(72, 32);
            label1.Name = "label1";
            label1.Size = new Size(176, 64);
            label1.TabIndex = 2;
            label1.Text = "機器ステータス";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MultiDeviceStatusUserControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(label1);
            Controls.Add(flpContainer);
            Name = "MultiDeviceStatusUserControl";
            Size = new Size(800, 64);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flpContainer;
        private Label label1;
    }
}
