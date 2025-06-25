namespace Hutzper.Library.Forms.ImageView
{
    partial class MultiImageViewControl
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
            ImageDrawingTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // ImageDrawingTimer
            // 
            ImageDrawingTimer.Enabled = true;
            ImageDrawingTimer.Interval = 50;
            ImageDrawingTimer.Tick += ImageDrawingTimer_Tick;
            // 
            // MultiImageViewControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "MultiImageViewControl";
            RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            Size = new Size(1024, 768);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer ImageDrawingTimer;
    }
}
