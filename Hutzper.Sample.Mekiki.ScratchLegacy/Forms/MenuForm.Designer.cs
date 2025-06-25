namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    partial class MenuForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            flowLayoutPanel1 = new FlowLayoutPanel();
            buttonMenuOperation = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonMenuMaintenance = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonMenuPlcDiagnostics = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonMenuConfiguration = new Library.Common.Forms.NoFocusedRoundedButton(components);
            buttonMenuExit = new Library.Common.Forms.NoFocusedRoundedButton(components);
            panelContainerOfLog = new Panel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(buttonMenuOperation);
            flowLayoutPanel1.Controls.Add(buttonMenuMaintenance);
            flowLayoutPanel1.Controls.Add(buttonMenuPlcDiagnostics);
            flowLayoutPanel1.Controls.Add(buttonMenuConfiguration);
            flowLayoutPanel1.Controls.Add(buttonMenuExit);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(8, 8);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(232, 336);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonMenuOperation
            // 
            buttonMenuOperation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMenuOperation.BackColor = Color.DodgerBlue;
            buttonMenuOperation.BackgroundColor = Color.DodgerBlue;
            buttonMenuOperation.BorderColor = Color.PaleVioletRed;
            buttonMenuOperation.BorderRadius = 25;
            buttonMenuOperation.BorderSize = 0;
            buttonMenuOperation.FlatAppearance.BorderSize = 0;
            buttonMenuOperation.FlatStyle = FlatStyle.Flat;
            buttonMenuOperation.ForeColor = Color.White;
            buttonMenuOperation.Location = new Point(3, 3);
            buttonMenuOperation.Name = "buttonMenuOperation";
            buttonMenuOperation.Size = new Size(225, 60);
            buttonMenuOperation.TabIndex = 10;
            buttonMenuOperation.Text = "検査開始";
            buttonMenuOperation.TextColor = Color.White;
            buttonMenuOperation.UseVisualStyleBackColor = false;
            buttonMenuOperation.Click += buttonMenuOperation_Click;
            // 
            // buttonMenuMaintenance
            // 
            buttonMenuMaintenance.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMenuMaintenance.BackColor = Color.DodgerBlue;
            buttonMenuMaintenance.BackgroundColor = Color.DodgerBlue;
            buttonMenuMaintenance.BorderColor = Color.PaleVioletRed;
            buttonMenuMaintenance.BorderRadius = 25;
            buttonMenuMaintenance.BorderSize = 0;
            buttonMenuMaintenance.FlatAppearance.BorderSize = 0;
            buttonMenuMaintenance.FlatStyle = FlatStyle.Flat;
            buttonMenuMaintenance.ForeColor = Color.White;
            buttonMenuMaintenance.Location = new Point(3, 69);
            buttonMenuMaintenance.Name = "buttonMenuMaintenance";
            buttonMenuMaintenance.Size = new Size(225, 60);
            buttonMenuMaintenance.TabIndex = 11;
            buttonMenuMaintenance.Text = "撮影調整";
            buttonMenuMaintenance.TextColor = Color.White;
            buttonMenuMaintenance.UseVisualStyleBackColor = false;
            buttonMenuMaintenance.Click += buttonMenuMaintenance_Click;
            // 
            // buttonMenuPlcDiagnostics
            // 
            buttonMenuPlcDiagnostics.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMenuPlcDiagnostics.BackColor = Color.DodgerBlue;
            buttonMenuPlcDiagnostics.BackgroundColor = Color.DodgerBlue;
            buttonMenuPlcDiagnostics.BorderColor = Color.PaleVioletRed;
            buttonMenuPlcDiagnostics.BorderRadius = 25;
            buttonMenuPlcDiagnostics.BorderSize = 0;
            buttonMenuPlcDiagnostics.FlatAppearance.BorderSize = 0;
            buttonMenuPlcDiagnostics.FlatStyle = FlatStyle.Flat;
            buttonMenuPlcDiagnostics.ForeColor = Color.White;
            buttonMenuPlcDiagnostics.Location = new Point(3, 135);
            buttonMenuPlcDiagnostics.Name = "buttonMenuPlcDiagnostics";
            buttonMenuPlcDiagnostics.Size = new Size(225, 60);
            buttonMenuPlcDiagnostics.TabIndex = 13;
            buttonMenuPlcDiagnostics.Text = "PLC通信確認";
            buttonMenuPlcDiagnostics.TextColor = Color.White;
            buttonMenuPlcDiagnostics.UseVisualStyleBackColor = false;
            buttonMenuPlcDiagnostics.Click += buttonMenuPlcDiagnostics_Click;
            // 
            // buttonMenuConfiguration
            // 
            buttonMenuConfiguration.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMenuConfiguration.BackColor = Color.DodgerBlue;
            buttonMenuConfiguration.BackgroundColor = Color.DodgerBlue;
            buttonMenuConfiguration.BorderColor = Color.PaleVioletRed;
            buttonMenuConfiguration.BorderRadius = 25;
            buttonMenuConfiguration.BorderSize = 0;
            buttonMenuConfiguration.FlatAppearance.BorderSize = 0;
            buttonMenuConfiguration.FlatStyle = FlatStyle.Flat;
            buttonMenuConfiguration.ForeColor = Color.White;
            buttonMenuConfiguration.Location = new Point(3, 201);
            buttonMenuConfiguration.Name = "buttonMenuConfiguration";
            buttonMenuConfiguration.Size = new Size(225, 60);
            buttonMenuConfiguration.TabIndex = 12;
            buttonMenuConfiguration.Text = "システム設定";
            buttonMenuConfiguration.TextColor = Color.White;
            buttonMenuConfiguration.UseVisualStyleBackColor = false;
            buttonMenuConfiguration.Click += buttonMenuConfiguration_Click;
            // 
            // buttonMenuExit
            // 
            buttonMenuExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonMenuExit.BackColor = Color.DodgerBlue;
            buttonMenuExit.BackgroundColor = Color.DodgerBlue;
            buttonMenuExit.BorderColor = Color.PaleVioletRed;
            buttonMenuExit.BorderRadius = 25;
            buttonMenuExit.BorderSize = 0;
            buttonMenuExit.FlatAppearance.BorderSize = 0;
            buttonMenuExit.FlatStyle = FlatStyle.Flat;
            buttonMenuExit.ForeColor = Color.White;
            buttonMenuExit.Location = new Point(3, 267);
            buttonMenuExit.Name = "buttonMenuExit";
            buttonMenuExit.Size = new Size(225, 60);
            buttonMenuExit.TabIndex = 9;
            buttonMenuExit.Text = "終了";
            buttonMenuExit.TextColor = Color.White;
            buttonMenuExit.UseVisualStyleBackColor = false;
            buttonMenuExit.Click += buttonMenuExit_Click;
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.BackgroundImage = (Image)resources.GetObject("panelContainerOfLog.BackgroundImage");
            panelContainerOfLog.BackgroundImageLayout = ImageLayout.Zoom;
            panelContainerOfLog.Location = new Point(240, 8);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(752, 328);
            panelContainerOfLog.TabIndex = 8;
            // 
            // MenuForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1002, 344);
            ControlBox = false;
            Controls.Add(panelContainerOfLog);
            Controls.Add(flowLayoutPanel1);
            Font = new Font("Yu Gothic UI", 16F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MenuForm";
            Nickname = "MenuForm [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "メニュー画面";
            FormClosing += MenuForm_FormClosing;
            Shown += MenuForm_Shown;
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Library.Common.Forms.NoFocusedRoundedButton buttonMenuOperation;
        private Library.Common.Forms.NoFocusedRoundedButton buttonMenuMaintenance;
        private Library.Common.Forms.NoFocusedRoundedButton buttonMenuConfiguration;
        private Library.Common.Forms.NoFocusedRoundedButton buttonMenuExit;
        private Panel panelContainerOfLog;
        private Library.Common.Forms.NoFocusedRoundedButton buttonMenuPlcDiagnostics;
    }
}