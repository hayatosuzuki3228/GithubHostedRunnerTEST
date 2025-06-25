namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Common.Diagnostics
{
    partial class DeviceStatusUserControl
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
            StatusLabel = new Label();
            SuspendLayout();
            // 
            // StatusLabel
            // 
            StatusLabel.Dock = DockStyle.Fill;
            StatusLabel.Location = new Point(0, 0);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(160, 64);
            StatusLabel.TabIndex = 0;
            StatusLabel.Text = "label1";
            StatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            StatusLabel.SizeChanged += StatusLabel_SizeChanged;
            StatusLabel.TextChanged += StatusLabel_SizeChanged;
            // 
            // DeviceStatusUserControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(StatusLabel);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "DeviceStatusUserControl";
            Size = new Size(160, 64);
            ResumeLayout(false);
        }

        #endregion

        private Label StatusLabel;
    }
}
