namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    partial class ResultAndStatisticsDisplay
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
            panel1 = new Panel();
            LabelJudgmentResult = new Label();
            LabelJudgmentDateTime = new Label();
            UcOverallResultTable = new Library.Forms.TableViewRendererUserControl();
            UcNgDetailsTable = new Library.Forms.TableViewRendererUserControl();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(LabelJudgmentResult);
            panel1.Controls.Add(LabelJudgmentDateTime);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 216);
            panel1.TabIndex = 0;
            // 
            // LabelJudgmentResult
            // 
            LabelJudgmentResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LabelJudgmentResult.BorderStyle = BorderStyle.FixedSingle;
            LabelJudgmentResult.Font = new Font("Yu Gothic UI", 48F, FontStyle.Regular, GraphicsUnit.Point);
            LabelJudgmentResult.Location = new Point(0, 0);
            LabelJudgmentResult.Name = "LabelJudgmentResult";
            LabelJudgmentResult.Size = new Size(400, 152);
            LabelJudgmentResult.TabIndex = 7;
            LabelJudgmentResult.Text = "-";
            LabelJudgmentResult.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelJudgmentDateTime
            // 
            LabelJudgmentDateTime.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LabelJudgmentDateTime.BackColor = SystemColors.ControlLightLight;
            LabelJudgmentDateTime.BorderStyle = BorderStyle.FixedSingle;
            LabelJudgmentDateTime.Location = new Point(0, 152);
            LabelJudgmentDateTime.Margin = new Padding(0);
            LabelJudgmentDateTime.Name = "LabelJudgmentDateTime";
            LabelJudgmentDateTime.Size = new Size(400, 64);
            LabelJudgmentDateTime.TabIndex = 6;
            LabelJudgmentDateTime.Text = "検査日時";
            LabelJudgmentDateTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UcOverallResultTable
            // 
            UcOverallResultTable.BackColor = Color.Lavender;
            UcOverallResultTable.BorderColor = Color.Black;
            UcOverallResultTable.BorderLineColor = SystemColors.ControlDarkDark;
            UcOverallResultTable.BorderRadius = 8D;
            UcOverallResultTable.BorderSize = 0D;
            UcOverallResultTable.ColumnWidthStrings = "240\n160";
            UcOverallResultTable.Index = -1;
            UcOverallResultTable.IsTextShrinkToFit = false;
            UcOverallResultTable.Location = new Point(0, 224);
            UcOverallResultTable.MouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
            UcOverallResultTable.Name = "UcOverallResultTable";
            UcOverallResultTable.Nickname = "UcJudgmentStatistics [Hutzper.Library.Forms.TableViewRendererUserControl]";
            UcOverallResultTable.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcOverallResultTable.RowHeight = 48;
            UcOverallResultTable.RowNumber = 3;
            UcOverallResultTable.Size = new Size(400, 152);
            UcOverallResultTable.TabIndex = 1;
            // 
            // UcNgDetailsTable
            // 
            UcNgDetailsTable.BackColor = Color.Lavender;
            UcNgDetailsTable.BorderColor = Color.Black;
            UcNgDetailsTable.BorderLineColor = SystemColors.ControlDarkDark;
            UcNgDetailsTable.BorderRadius = 8D;
            UcNgDetailsTable.BorderSize = 0D;
            UcNgDetailsTable.ColumnWidthStrings = "240\n160";
            UcNgDetailsTable.Index = -1;
            UcNgDetailsTable.IsTextShrinkToFit = false;
            UcNgDetailsTable.Location = new Point(0, 384);
            UcNgDetailsTable.MouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
            UcNgDetailsTable.Name = "UcNgDetailsTable";
            UcNgDetailsTable.Nickname = "UcJudgmentStatistics [Hutzper.Library.Forms.TableViewRendererUserControl]";
            UcNgDetailsTable.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            UcNgDetailsTable.RowHeight = 48;
            UcNgDetailsTable.RowNumber = 6;
            UcNgDetailsTable.Size = new Size(400, 288);
            UcNgDetailsTable.TabIndex = 2;
            // 
            // ResultAndStatisticsDisplay
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            BorderSize = 0D;
            Controls.Add(UcNgDetailsTable);
            Controls.Add(UcOverallResultTable);
            Controls.Add(panel1);
            Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "ResultAndStatisticsDisplay";
            RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            Size = new Size(400, 720);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label LabelJudgmentResult;
        private Label LabelJudgmentDateTime;
        private Library.Forms.TableViewRendererUserControl UcOverallResultTable;
        private Library.Forms.TableViewRendererUserControl UcNgDetailsTable;
    }
}
