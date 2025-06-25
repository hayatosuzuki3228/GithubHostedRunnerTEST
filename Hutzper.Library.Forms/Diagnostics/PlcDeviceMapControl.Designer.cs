namespace Hutzper.Library.Forms.Diagnostics
{
    partial class PlcDeviceMapControl
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
            vScrollBar1 = new VScrollBar();
            UcDeviceMap = new TableViewRendererUserControl();
            SuspendLayout();
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(336, 8);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(32, 448);
            vScrollBar1.TabIndex = 3;
            // 
            // UcDeviceMap
            // 
            UcDeviceMap.BackColor = Color.Lavender;
            UcDeviceMap.BorderColor = Color.Black;
            UcDeviceMap.BorderLineColor = SystemColors.ControlDarkDark;
            UcDeviceMap.BorderRadius = 11D;
            UcDeviceMap.BorderSize = 0D;
            UcDeviceMap.ColumnWidthStrings = "160\n160";
            UcDeviceMap.Index = -1;
            UcDeviceMap.IsTextShrinkToFit = false;
            UcDeviceMap.Location = new Point(8, 8);
            UcDeviceMap.MouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
            UcDeviceMap.Name = "UcDeviceMap";
            UcDeviceMap.Nickname = "tableViewRendererUserControl1 [Hutzper.Library.Forms.TableViewRendererUserControl]";
            UcDeviceMap.RoundedCorner = Common.Drawing.RoundedCorner.LeftTop | Common.Drawing.RoundedCorner.RightTop | Common.Drawing.RoundedCorner.RightBottom | Common.Drawing.RoundedCorner.LeftBottom;
            UcDeviceMap.RowHeight = 40;
            UcDeviceMap.RowNumber = 11;
            UcDeviceMap.Size = new Size(328, 448);
            UcDeviceMap.TabIndex = 2;
            // 
            // PlcDeviceMapControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            Controls.Add(vScrollBar1);
            Controls.Add(UcDeviceMap);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "PlcDeviceMapControl";
            RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            Size = new Size(372, 480);
            ResumeLayout(false);
        }

        #endregion

        private VScrollBar vScrollBar1;
        private TableViewRendererUserControl UcDeviceMap;
    }
}
