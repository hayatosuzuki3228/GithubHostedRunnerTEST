namespace Hutzper.Simulator.PlcTcpCommunication
{
    partial class DeviceMapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableViewRendererUserControl1 = new Library.Forms.TableViewRendererUserControl();
            vScrollBar1 = new VScrollBar();
            UcBaseNumber = new Library.Forms.Setting.ComboBoxUserControl();
            timer1 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // tableViewRendererUserControl1
            // 
            tableViewRendererUserControl1.BackColor = Color.Lavender;
            tableViewRendererUserControl1.BorderColor = Color.Black;
            tableViewRendererUserControl1.BorderLineColor = SystemColors.ControlDarkDark;
            tableViewRendererUserControl1.BorderRadius = 11D;
            tableViewRendererUserControl1.BorderSize = 0D;
            tableViewRendererUserControl1.ColumnWidthStrings = "160\n160";
            tableViewRendererUserControl1.Index = -1;
            tableViewRendererUserControl1.IsTextShrinkToFit = false;
            tableViewRendererUserControl1.Location = new Point(0, 48);
            tableViewRendererUserControl1.MouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
            tableViewRendererUserControl1.Name = "tableViewRendererUserControl1";
            tableViewRendererUserControl1.Nickname = "tableViewRendererUserControl1 [Hutzper.Library.Forms.TableViewRendererUserControl]";
            tableViewRendererUserControl1.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            tableViewRendererUserControl1.RowHeight = 40;
            tableViewRendererUserControl1.RowNumber = 11;
            tableViewRendererUserControl1.Size = new Size(328, 448);
            tableViewRendererUserControl1.TabIndex = 0;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(328, 48);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(40, 440);
            vScrollBar1.TabIndex = 1;
            // 
            // UcBaseNumber
            // 
            UcBaseNumber.BackColor = SystemColors.Window;
            UcBaseNumber.BorderColor = Color.Black;
            UcBaseNumber.BorderRadius = 25D;
            UcBaseNumber.BorderSize = 1D;
            UcBaseNumber.ComboBoxFont = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcBaseNumber.ComboBoxWidth = 160;
            UcBaseNumber.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            UcBaseNumber.Index = -1;
            UcBaseNumber.IsTextShrinkToFit = true;
            UcBaseNumber.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            UcBaseNumber.LabelForeColor = SystemColors.ControlText;
            UcBaseNumber.LabelText = "表示形式";
            UcBaseNumber.Location = new Point(0, 0);
            UcBaseNumber.Name = "UcBaseNumber";
            UcBaseNumber.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcBaseNumber.RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            UcBaseNumber.SelectedIndex = -1;
            UcBaseNumber.SelectedItem = null;
            UcBaseNumber.Size = new Size(320, 40);
            UcBaseNumber.TabIndex = 2;
            UcBaseNumber.SelectedIndexChanged += comboBoxUserControl1_SelectedIndexChanged;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // DeviceMapForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(378, 493);
            Controls.Add(UcBaseNumber);
            Controls.Add(vScrollBar1);
            Controls.Add(tableViewRendererUserControl1);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DeviceMapForm";
            Nickname = "DeviceMapForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: DeviceMapForm";
            Text = "DeviceMapForm";
            Shown += DeviceMapForm_Shown;
            LocationChanged += DeviceMapForm_LocationChanged;
            MouseDown += DeviceMapForm_MouseDown;
            ResumeLayout(false);
        }

        #endregion

        private Library.Forms.TableViewRendererUserControl tableViewRendererUserControl1;
        private VScrollBar vScrollBar1;
        private Library.Forms.Setting.ComboBoxUserControl UcBaseNumber;
        private System.Windows.Forms.Timer timer1;
    }
}