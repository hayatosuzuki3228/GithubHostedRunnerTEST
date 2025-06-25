namespace Hutzper.Library.Forms.Diagnostics
{
    partial class PlcDiagnosticsControl
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
            StatusRefreshTimer = new System.Windows.Forms.Timer(components);
            LabelStatus = new Label();
            UcDeviceMapWord = new PlcDeviceMapControl();
            UcDeviceMapBit = new PlcDeviceMapControl();
            UcBaseNumber = new Setting.ComboBoxUserControl();
            SuspendLayout();
            // 
            // StatusRefreshTimer
            // 
            StatusRefreshTimer.Interval = 200;
            StatusRefreshTimer.Tick += StatusRefreshTimer_Tick;
            // 
            // LabelStatus
            // 
            LabelStatus.BorderStyle = BorderStyle.FixedSingle;
            LabelStatus.Location = new Point(8, 8);
            LabelStatus.Name = "LabelStatus";
            LabelStatus.Size = new Size(160, 40);
            LabelStatus.TabIndex = 8;
            LabelStatus.Text = "通信状況";
            LabelStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UcDeviceMapWord
            // 
            UcDeviceMapWord.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            UcDeviceMapWord.BackColor = SystemColors.Window;
            UcDeviceMapWord.BorderColor = Color.Black;
            UcDeviceMapWord.BorderRadius = 25D;
            UcDeviceMapWord.BorderSize = 0D;
            UcDeviceMapWord.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcDeviceMapWord.Index = -1;
            UcDeviceMapWord.IsHexadecimal = false;
            UcDeviceMapWord.IsTextShrinkToFit = false;
            UcDeviceMapWord.Location = new Point(0, 56);
            UcDeviceMapWord.Name = "UcDeviceMapWord";
            UcDeviceMapWord.Nickname = "UcDeviceMapWord [Hutzper.Library.Forms.Diagnostics.PlcDeviceMapControl]";
            UcDeviceMapWord.RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            UcDeviceMapWord.Size = new Size(376, 456);
            UcDeviceMapWord.TabIndex = 9;
            UcDeviceMapWord.Visible = false;
            // 
            // UcDeviceMapBit
            // 
            UcDeviceMapBit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            UcDeviceMapBit.BackColor = SystemColors.Window;
            UcDeviceMapBit.BorderColor = Color.Black;
            UcDeviceMapBit.BorderRadius = 25D;
            UcDeviceMapBit.BorderSize = 0D;
            UcDeviceMapBit.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcDeviceMapBit.Index = -1;
            UcDeviceMapBit.IsHexadecimal = false;
            UcDeviceMapBit.IsTextShrinkToFit = false;
            UcDeviceMapBit.Location = new Point(384, 56);
            UcDeviceMapBit.Name = "UcDeviceMapBit";
            UcDeviceMapBit.Nickname = "plcDeviceMapControl1 [Hutzper.Library.Forms.Diagnostics.PlcDeviceMapControl]";
            UcDeviceMapBit.RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            UcDeviceMapBit.Size = new Size(376, 456);
            UcDeviceMapBit.TabIndex = 10;
            UcDeviceMapBit.Visible = false;
            // 
            // UcBaseNumber
            // 
            UcBaseNumber.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            UcBaseNumber.Location = new Point(440, 8);
            UcBaseNumber.Name = "UcBaseNumber";
            UcBaseNumber.Nickname = "comboBoxUserControl1 [Hutzper.Library.Forms.Setting.ComboBoxUserControl]";
            UcBaseNumber.RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            UcBaseNumber.SelectedIndex = -1;
            UcBaseNumber.SelectedItem = null;
            UcBaseNumber.Size = new Size(320, 40);
            UcBaseNumber.TabIndex = 11;
            UcBaseNumber.SelectedIndexChanged += UcBaseNumber_SelectedIndexChanged;
            // 
            // PlcDiagnosticsControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            Controls.Add(UcBaseNumber);
            Controls.Add(UcDeviceMapBit);
            Controls.Add(UcDeviceMapWord);
            Controls.Add(LabelStatus);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "PlcDiagnosticsControl";
            RoundedCorner = Common.Drawing.RoundedCorner.Disable;
            Size = new Size(768, 512);
            MouseDown += PlcDiagnosticsControl_MouseDown;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer StatusRefreshTimer;
        private Label LabelStatus;
        private PlcDeviceMapControl UcDeviceMapWord;
        private PlcDeviceMapControl UcDeviceMapBit;
        private Setting.ComboBoxUserControl UcBaseNumber;
    }
}
