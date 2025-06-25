namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    partial class PlcDiagnosticsForm
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
            buttonExit = new Library.Common.Forms.NoFocusedRoundedButton(components);
            UcPlcDiagnostics = new Library.Forms.Diagnostics.PlcDiagnosticsControl();
            panelContainerOfLog = new Panel();
            SuspendLayout();
            // 
            // buttonExit
            // 
            buttonExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonExit.BackColor = Color.DodgerBlue;
            buttonExit.BackgroundColor = Color.DodgerBlue;
            buttonExit.BorderColor = Color.PaleVioletRed;
            buttonExit.BorderRadius = 25;
            buttonExit.BorderSize = 0;
            buttonExit.FlatAppearance.BorderSize = 0;
            buttonExit.FlatStyle = FlatStyle.Flat;
            buttonExit.ForeColor = Color.White;
            buttonExit.Location = new Point(553, 8);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new Size(225, 60);
            buttonExit.TabIndex = 13;
            buttonExit.Text = "終了";
            buttonExit.TextColor = Color.White;
            buttonExit.UseVisualStyleBackColor = false;
            buttonExit.Click += buttonExit_Click;
            // 
            // UcPlcDiagnostics
            // 
            UcPlcDiagnostics.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UcPlcDiagnostics.BackColor = SystemColors.Window;
            UcPlcDiagnostics.BorderColor = Color.Black;
            UcPlcDiagnostics.BorderRadius = 0D;
            UcPlcDiagnostics.BorderSize = 0D;
            UcPlcDiagnostics.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            UcPlcDiagnostics.Index = -1;
            UcPlcDiagnostics.IsTextShrinkToFit = false;
            UcPlcDiagnostics.Location = new Point(8, 80);
            UcPlcDiagnostics.Name = "UcPlcDiagnostics";
            UcPlcDiagnostics.Nickname = "plcDiagnosticsControl1 [Hutzper.Library.Forms.Diagnostics.PlcDiagnosticsControl]";
            UcPlcDiagnostics.RoundedCorner = Library.Common.Drawing.RoundedCorner.Disable;
            UcPlcDiagnostics.Size = new Size(768, 512);
            UcPlcDiagnostics.StatusRefreshIntervalMs = 200;
            UcPlcDiagnostics.TabIndex = 14;
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.Location = new Point(8, 600);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(768, 174);
            panelContainerOfLog.TabIndex = 15;
            // 
            // PlcDiagnosticsForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(786, 784);
            ControlBox = false;
            Controls.Add(panelContainerOfLog);
            Controls.Add(UcPlcDiagnostics);
            Controls.Add(buttonExit);
            Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(808, 0);
            Name = "PlcDiagnosticsForm";
            Nickname = "PlcDiagnosticsForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: PLC通信確認画面";
            Text = "PLC通信確認画面";
            Shown += PlcDiagnosticsForm_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Library.Common.Forms.NoFocusedRoundedButton buttonExit;
        private Library.Forms.Diagnostics.PlcDiagnosticsControl UcPlcDiagnostics;
        private Panel panelContainerOfLog;
    }
}