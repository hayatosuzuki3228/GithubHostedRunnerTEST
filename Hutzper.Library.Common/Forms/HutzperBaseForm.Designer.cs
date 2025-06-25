namespace Hutzper.Library.Common.Forms
{
    partial class HutzperBaseForm : Form
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
            this.components = new System.ComponentModel.Container();
            this.BasePanelContainer = new Hutzper.Library.Common.Forms.DoubleBufferedPanel();
            this.BasePanelTitleBar = new Hutzper.Library.Common.Forms.DoubleBufferedPanel();
            this.BaseButtonMinimize = new Hutzper.Library.Common.Forms.NoFocusedRoundedButton(this.components);
            this.BaseButtonClose = new Hutzper.Library.Common.Forms.NoFocusedRoundedButton(this.components);
            this.BaseButtonMaximize = new Hutzper.Library.Common.Forms.NoFocusedRoundedButton(this.components);
            this.BaseLabelTitle = new System.Windows.Forms.Label();
            this.BasePanelContainer.SuspendLayout();
            this.BasePanelTitleBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // BasePanelContainer
            // 
            this.BasePanelContainer.BackColor = System.Drawing.Color.Lavender;
            this.BasePanelContainer.Controls.Add(this.BasePanelTitleBar);
            this.BasePanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BasePanelContainer.Location = new System.Drawing.Point(0, 0);
            this.BasePanelContainer.Name = "BasePanelContainer";
            this.BasePanelContainer.Size = new System.Drawing.Size(800, 450);
            this.BasePanelContainer.TabIndex = 1;
            this.BasePanelContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.panelContainer_Paint);
            this.BasePanelContainer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDoubleClick);
            this.BasePanelContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDown);
            this.BasePanelContainer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseMove);
            this.BasePanelContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseUp);
            // 
            // BasePanelTitleBar
            // 
            this.BasePanelTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.BasePanelTitleBar.Controls.Add(this.BaseButtonMinimize);
            this.BasePanelTitleBar.Controls.Add(this.BaseButtonMaximize);
            this.BasePanelTitleBar.Controls.Add(this.BaseButtonClose);
            this.BasePanelTitleBar.Controls.Add(this.BaseLabelTitle);
            this.BasePanelTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.BasePanelTitleBar.Location = new System.Drawing.Point(0, 0);
            this.BasePanelTitleBar.Name = "BasePanelTitleBar";
            this.BasePanelTitleBar.Size = new System.Drawing.Size(800, 50);
            this.BasePanelTitleBar.TabIndex = 1;
            this.BasePanelTitleBar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDoubleClick);
            this.BasePanelTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDown);
            this.BasePanelTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseMove);
            this.BasePanelTitleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseUp);
            // 
            // BaseButtonMinimize
            // 
            this.BaseButtonMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.BaseButtonMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.BaseButtonMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BaseButtonMinimize.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BaseButtonMinimize.ForeColor = System.Drawing.Color.White;
            this.BaseButtonMinimize.Location = new System.Drawing.Point(650, 0);
            this.BaseButtonMinimize.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.BaseButtonMinimize.Name = "BaseButtonMinimize";
            this.BaseButtonMinimize.Size = new System.Drawing.Size(50, 50);
            this.BaseButtonMinimize.TabIndex = 6;
            this.BaseButtonMinimize.Text = "-";
            this.BaseButtonMinimize.UseVisualStyleBackColor = false;
            this.BaseButtonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // BaseButtonClose
            // 
            this.BaseButtonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.BaseButtonClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.BaseButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BaseButtonClose.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BaseButtonClose.ForeColor = System.Drawing.Color.White;
            this.BaseButtonClose.Location = new System.Drawing.Point(750, 0);
            this.BaseButtonClose.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.BaseButtonClose.Name = "BaseButtonClose";
            this.BaseButtonClose.Size = new System.Drawing.Size(50, 50);
            this.BaseButtonClose.TabIndex = 3;
            this.BaseButtonClose.Text = "x";
            this.BaseButtonClose.UseVisualStyleBackColor = false;
            this.BaseButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // BaseButtonMaximize
            // 
            this.BaseButtonMaximize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.BaseButtonMaximize.Dock = System.Windows.Forms.DockStyle.Right;
            this.BaseButtonMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BaseButtonMaximize.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BaseButtonMaximize.ForeColor = System.Drawing.Color.White;
            this.BaseButtonMaximize.Location = new System.Drawing.Point(700, 0);
            this.BaseButtonMaximize.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.BaseButtonMaximize.Name = "BaseButtonMaximize";
            this.BaseButtonMaximize.Size = new System.Drawing.Size(50, 50);
            this.BaseButtonMaximize.TabIndex = 7;
            this.BaseButtonMaximize.Text = "□";
            this.BaseButtonMaximize.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BaseButtonMaximize.UseVisualStyleBackColor = false;
            this.BaseButtonMaximize.Click += new System.EventHandler(this.BaseButtonMaximize_Click);
            // 
            // BaseLabelTitle
            // 
            this.BaseLabelTitle.AutoSize = true;
            this.BaseLabelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.BaseLabelTitle.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BaseLabelTitle.ForeColor = System.Drawing.Color.White;
            this.BaseLabelTitle.Location = new System.Drawing.Point(8, 8);
            this.BaseLabelTitle.Name = "BaseLabelTitle";
            this.BaseLabelTitle.Size = new System.Drawing.Size(188, 32);
            this.BaseLabelTitle.TabIndex = 5;
            this.BaseLabelTitle.Text = "Hutzper window";
            this.BaseLabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BaseLabelTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDoubleClick);
            this.BaseLabelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDown);
            this.BaseLabelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseMove);
            this.BaseLabelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseUp);
            // 
            // HutzperBaseForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BasePanelContainer);
            this.DoubleBuffered = true;
            this.Name = "HutzperBaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HutzperForm";
            this.Activated += new System.EventHandler(this.HutzperBaseForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HutzperBaseForm_FormClosed);
            this.Shown += new System.EventHandler(this.HutzperBaseForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.HutzperBaseForm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.HutzperBaseForm_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.HutzperBaseForm_Paint);
            this.BasePanelContainer.ResumeLayout(false);
            this.BasePanelTitleBar.ResumeLayout(false);
            this.BasePanelTitleBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        protected Label BaseLabelTitle;
        protected internal DoubleBufferedPanel BasePanelTitleBar;
        protected internal DoubleBufferedPanel BasePanelContainer;
        protected NoFocusedRoundedButton BaseButtonMinimize;
        protected NoFocusedRoundedButton BaseButtonClose;
        protected NoFocusedRoundedButton BaseButtonMaximize;
    }
}