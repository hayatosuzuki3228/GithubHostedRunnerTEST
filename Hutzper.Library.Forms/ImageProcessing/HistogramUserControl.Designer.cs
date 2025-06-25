namespace Hutzper.Library.Forms.ImageProcessing
{
    partial class HistogramUserControl
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfGray = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfRed = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfGreen = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfAllOn = new System.Windows.Forms.ToolStripMenuItem();
            this.channelOfAllOff = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.graphColorNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.graphColorPseudocolor = new System.Windows.Forms.ToolStripMenuItem();
            this.graphColorPseudocolorInverted = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionCursorOverlayEnalbed = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.optionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(241, 133);
            this.contextMenuStrip1.Text = "メニュー";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.channelOfGray,
            this.channelOfRed,
            this.channelOfGreen,
            this.channelOfBlue,
            this.channelOfAllOn,
            this.channelOfAllOff});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(240, 32);
            this.toolStripMenuItem1.Text = "Channel";
            // 
            // channelOfGray
            // 
            this.channelOfGray.Checked = true;
            this.channelOfGray.CheckOnClick = true;
            this.channelOfGray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.channelOfGray.Name = "channelOfGray";
            this.channelOfGray.Size = new System.Drawing.Size(164, 34);
            this.channelOfGray.Text = "Gray";
            this.channelOfGray.CheckStateChanged += new System.EventHandler(this.channel_CheckStateChanged);
            // 
            // channelOfRed
            // 
            this.channelOfRed.Checked = true;
            this.channelOfRed.CheckOnClick = true;
            this.channelOfRed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.channelOfRed.Name = "channelOfRed";
            this.channelOfRed.Size = new System.Drawing.Size(164, 34);
            this.channelOfRed.Text = "Red";
            this.channelOfRed.CheckStateChanged += new System.EventHandler(this.channel_CheckStateChanged);
            // 
            // channelOfGreen
            // 
            this.channelOfGreen.Checked = true;
            this.channelOfGreen.CheckOnClick = true;
            this.channelOfGreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.channelOfGreen.Name = "channelOfGreen";
            this.channelOfGreen.Size = new System.Drawing.Size(164, 34);
            this.channelOfGreen.Text = "Green";
            this.channelOfGreen.CheckStateChanged += new System.EventHandler(this.channel_CheckStateChanged);
            // 
            // channelOfBlue
            // 
            this.channelOfBlue.Checked = true;
            this.channelOfBlue.CheckOnClick = true;
            this.channelOfBlue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.channelOfBlue.Name = "channelOfBlue";
            this.channelOfBlue.Size = new System.Drawing.Size(164, 34);
            this.channelOfBlue.Text = "Blue";
            this.channelOfBlue.CheckStateChanged += new System.EventHandler(this.channel_CheckStateChanged);
            // 
            // channelOfAllOn
            // 
            this.channelOfAllOn.Name = "channelOfAllOn";
            this.channelOfAllOn.Size = new System.Drawing.Size(164, 34);
            this.channelOfAllOn.Text = "All On";
            this.channelOfAllOn.Click += new System.EventHandler(this.channelOfAllOnOff_Click);
            // 
            // channelOfAllOff
            // 
            this.channelOfAllOff.Name = "channelOfAllOff";
            this.channelOfAllOff.Size = new System.Drawing.Size(164, 34);
            this.channelOfAllOff.Text = "All Off";
            this.channelOfAllOff.Click += new System.EventHandler(this.channelOfAllOnOff_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphColorNormal,
            this.graphColorPseudocolor,
            this.graphColorPseudocolorInverted});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(240, 32);
            this.toolStripMenuItem2.Text = "Color";
            // 
            // graphColorNormal
            // 
            this.graphColorNormal.Checked = true;
            this.graphColorNormal.CheckOnClick = true;
            this.graphColorNormal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.graphColorNormal.Name = "graphColorNormal";
            this.graphColorNormal.Size = new System.Drawing.Size(283, 34);
            this.graphColorNormal.Text = "Normal";
            this.graphColorNormal.CheckStateChanged += new System.EventHandler(this.color_CheckStateChanged);
            // 
            // graphColorPseudocolor
            // 
            this.graphColorPseudocolor.CheckOnClick = true;
            this.graphColorPseudocolor.Name = "graphColorPseudocolor";
            this.graphColorPseudocolor.Size = new System.Drawing.Size(283, 34);
            this.graphColorPseudocolor.Text = "Pseudocolor";
            this.graphColorPseudocolor.CheckStateChanged += new System.EventHandler(this.color_CheckStateChanged);
            // 
            // graphColorPseudocolorInverted
            // 
            this.graphColorPseudocolorInverted.CheckOnClick = true;
            this.graphColorPseudocolorInverted.Name = "graphColorPseudocolorInverted";
            this.graphColorPseudocolorInverted.Size = new System.Drawing.Size(283, 34);
            this.graphColorPseudocolorInverted.Text = "Pseudocolor Inverted";
            this.graphColorPseudocolorInverted.CheckStateChanged += new System.EventHandler(this.color_CheckStateChanged);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionCursorOverlayEnalbed});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(240, 32);
            this.optionToolStripMenuItem.Text = "Option";
            // 
            // optionCursorOverlayEnalbed
            // 
            this.optionCursorOverlayEnalbed.Checked = true;
            this.optionCursorOverlayEnalbed.CheckOnClick = true;
            this.optionCursorOverlayEnalbed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionCursorOverlayEnalbed.Name = "optionCursorOverlayEnalbed";
            this.optionCursorOverlayEnalbed.Size = new System.Drawing.Size(297, 34);
            this.optionCursorOverlayEnalbed.Text = "Cursor position display";
            this.optionCursorOverlayEnalbed.CheckStateChanged += new System.EventHandler(this.option_CheckStateChanged);
            // 
            // HistogramUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.MinimumSize = new System.Drawing.Size(320, 200);
            this.Name = "HistogramUserControl";
            this.Size = new System.Drawing.Size(320, 200);
            this.Load += new System.EventHandler(this.HistogramUserControl_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem graphColorNormal;
        private ToolStripMenuItem graphColorPseudocolor;
        private ToolStripMenuItem graphColorPseudocolorInverted;
        private ToolStripMenuItem channelOfGray;
        private ToolStripMenuItem channelOfRed;
        private ToolStripMenuItem channelOfGreen;
        private ToolStripMenuItem channelOfBlue;
        private ToolStripMenuItem channelOfAllOn;
        private ToolStripMenuItem channelOfAllOff;
        private ToolStripMenuItem optionToolStripMenuItem;
        private ToolStripMenuItem optionCursorOverlayEnalbed;
    }
}
