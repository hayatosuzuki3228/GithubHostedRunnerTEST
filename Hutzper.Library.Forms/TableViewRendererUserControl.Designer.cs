﻿namespace Hutzper.Library.Forms
{
    partial class TableViewRendererUserControl
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
            this.SuspendLayout();
            // 
            // TableViewRendererUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Name = "TableViewRendererUserControl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RecordTableViewUserControl_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TableViewRendererUserControl_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TableViewRendererUserControl_MouseDoubleClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TableViewRendererUserControl_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
