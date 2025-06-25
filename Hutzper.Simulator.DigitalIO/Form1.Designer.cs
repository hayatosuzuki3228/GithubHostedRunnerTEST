namespace Hutzper.Simulator.DigitalIO
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            numericUpDown1 = new NumericUpDown();
            panelContainerOfLog = new Panel();
            onOffUserControl1 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn00 = new Library.Forms.Setting.OnOffUserControl();
            flowLayoutPanel1 = new FlowLayoutPanel();
            onOffUcIn01 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn02 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn03 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn04 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn05 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn06 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcIn07 = new Library.Forms.Setting.OnOffUserControl();
            flowLayoutPanel2 = new FlowLayoutPanel();
            onOffUcOut00 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut01 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut02 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut03 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut04 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut05 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut06 = new Library.Forms.Setting.OnOffUserControl();
            onOffUcOut07 = new Library.Forms.Setting.OnOffUserControl();
            timer1 = new System.Windows.Forms.Timer(components);
            onOffUserControl2 = new Library.Forms.Setting.OnOffUserControl();
            numericUpDown2 = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            numericUpDown3 = new NumericUpDown();
            label3 = new Label();
            numericUpDown4 = new NumericUpDown();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            SuspendLayout();
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(168, 16);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 49152, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(88, 27);
            numericUpDown1.TabIndex = 1;
            numericUpDown1.TextAlign = HorizontalAlignment.Right;
            numericUpDown1.Value = new decimal(new int[] { 50000, 0, 0, 0 });
            // 
            // panelContainerOfLog
            // 
            panelContainerOfLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelContainerOfLog.BackColor = Color.Transparent;
            panelContainerOfLog.Location = new Point(0, 64);
            panelContainerOfLog.Name = "panelContainerOfLog";
            panelContainerOfLog.Size = new Size(720, 376);
            panelContainerOfLog.TabIndex = 4;
            // 
            // onOffUserControl1
            // 
            onOffUserControl1.BackColor = SystemColors.Window;
            onOffUserControl1.BorderColor = Color.Black;
            onOffUserControl1.BorderRadius = 25D;
            onOffUserControl1.BorderSize = 1D;
            onOffUserControl1.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl1.Index = -1;
            onOffUserControl1.IsTextShrinkToFit = true;
            onOffUserControl1.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl1.LabelForeColor = SystemColors.ControlText;
            onOffUserControl1.LabelText = "listen";
            onOffUserControl1.Location = new Point(8, 8);
            onOffUserControl1.Name = "onOffUserControl1";
            onOffUserControl1.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUserControl1.OffBackColor = Color.Gray;
            onOffUserControl1.OffToggleColor = Color.Gainsboro;
            onOffUserControl1.OnBackColor = Color.DodgerBlue;
            onOffUserControl1.OnToggleColor = Color.WhiteSmoke;
            onOffUserControl1.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUserControl1.Size = new Size(160, 40);
            onOffUserControl1.TabIndex = 5;
            onOffUserControl1.Value = false;
            onOffUserControl1.ValueChanged += onOffUserControl1_ValueChanged;
            // 
            // onOffUcIn00
            // 
            onOffUcIn00.BackColor = SystemColors.Window;
            onOffUcIn00.BorderColor = Color.Black;
            onOffUcIn00.BorderRadius = 24D;
            onOffUcIn00.BorderSize = 1D;
            onOffUcIn00.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn00.Index = -1;
            onOffUcIn00.IsTextShrinkToFit = true;
            onOffUcIn00.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn00.LabelForeColor = SystemColors.ControlText;
            onOffUcIn00.LabelText = "input 0";
            onOffUcIn00.Location = new Point(3, 3);
            onOffUcIn00.Name = "onOffUcIn00";
            onOffUcIn00.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn00.OffBackColor = Color.Gray;
            onOffUcIn00.OffToggleColor = Color.Gainsboro;
            onOffUcIn00.OnBackColor = Color.DodgerBlue;
            onOffUcIn00.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn00.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn00.Size = new Size(168, 40);
            onOffUcIn00.TabIndex = 6;
            onOffUcIn00.Value = false;
            onOffUcIn00.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(onOffUcIn00);
            flowLayoutPanel1.Controls.Add(onOffUcIn01);
            flowLayoutPanel1.Controls.Add(onOffUcIn02);
            flowLayoutPanel1.Controls.Add(onOffUcIn03);
            flowLayoutPanel1.Controls.Add(onOffUcIn04);
            flowLayoutPanel1.Controls.Add(onOffUcIn05);
            flowLayoutPanel1.Controls.Add(onOffUcIn06);
            flowLayoutPanel1.Controls.Add(onOffUcIn07);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(728, 64);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(176, 376);
            flowLayoutPanel1.TabIndex = 7;
            // 
            // onOffUcIn01
            // 
            onOffUcIn01.BackColor = SystemColors.Window;
            onOffUcIn01.BorderColor = Color.Black;
            onOffUcIn01.BorderRadius = 24D;
            onOffUcIn01.BorderSize = 1D;
            onOffUcIn01.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn01.Index = -1;
            onOffUcIn01.IsTextShrinkToFit = true;
            onOffUcIn01.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn01.LabelForeColor = SystemColors.ControlText;
            onOffUcIn01.LabelText = "input 1";
            onOffUcIn01.Location = new Point(3, 49);
            onOffUcIn01.Name = "onOffUcIn01";
            onOffUcIn01.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn01.OffBackColor = Color.Gray;
            onOffUcIn01.OffToggleColor = Color.Gainsboro;
            onOffUcIn01.OnBackColor = Color.DodgerBlue;
            onOffUcIn01.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn01.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn01.Size = new Size(168, 40);
            onOffUcIn01.TabIndex = 7;
            onOffUcIn01.Value = false;
            onOffUcIn01.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn02
            // 
            onOffUcIn02.BackColor = SystemColors.Window;
            onOffUcIn02.BorderColor = Color.Black;
            onOffUcIn02.BorderRadius = 24D;
            onOffUcIn02.BorderSize = 1D;
            onOffUcIn02.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn02.Index = -1;
            onOffUcIn02.IsTextShrinkToFit = true;
            onOffUcIn02.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn02.LabelForeColor = SystemColors.ControlText;
            onOffUcIn02.LabelText = "input 2";
            onOffUcIn02.Location = new Point(3, 95);
            onOffUcIn02.Name = "onOffUcIn02";
            onOffUcIn02.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn02.OffBackColor = Color.Gray;
            onOffUcIn02.OffToggleColor = Color.Gainsboro;
            onOffUcIn02.OnBackColor = Color.DodgerBlue;
            onOffUcIn02.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn02.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn02.Size = new Size(168, 40);
            onOffUcIn02.TabIndex = 8;
            onOffUcIn02.Value = false;
            onOffUcIn02.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn03
            // 
            onOffUcIn03.BackColor = SystemColors.Window;
            onOffUcIn03.BorderColor = Color.Black;
            onOffUcIn03.BorderRadius = 24D;
            onOffUcIn03.BorderSize = 1D;
            onOffUcIn03.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn03.Index = -1;
            onOffUcIn03.IsTextShrinkToFit = true;
            onOffUcIn03.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn03.LabelForeColor = SystemColors.ControlText;
            onOffUcIn03.LabelText = "input 3";
            onOffUcIn03.Location = new Point(3, 141);
            onOffUcIn03.Name = "onOffUcIn03";
            onOffUcIn03.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn03.OffBackColor = Color.Gray;
            onOffUcIn03.OffToggleColor = Color.Gainsboro;
            onOffUcIn03.OnBackColor = Color.DodgerBlue;
            onOffUcIn03.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn03.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn03.Size = new Size(168, 40);
            onOffUcIn03.TabIndex = 9;
            onOffUcIn03.Value = false;
            onOffUcIn03.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn04
            // 
            onOffUcIn04.BackColor = SystemColors.Window;
            onOffUcIn04.BorderColor = Color.Black;
            onOffUcIn04.BorderRadius = 24D;
            onOffUcIn04.BorderSize = 1D;
            onOffUcIn04.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn04.Index = -1;
            onOffUcIn04.IsTextShrinkToFit = true;
            onOffUcIn04.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn04.LabelForeColor = SystemColors.ControlText;
            onOffUcIn04.LabelText = "input 4";
            onOffUcIn04.Location = new Point(3, 187);
            onOffUcIn04.Name = "onOffUcIn04";
            onOffUcIn04.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn04.OffBackColor = Color.Gray;
            onOffUcIn04.OffToggleColor = Color.Gainsboro;
            onOffUcIn04.OnBackColor = Color.DodgerBlue;
            onOffUcIn04.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn04.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn04.Size = new Size(168, 40);
            onOffUcIn04.TabIndex = 10;
            onOffUcIn04.Value = false;
            onOffUcIn04.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn05
            // 
            onOffUcIn05.BackColor = SystemColors.Window;
            onOffUcIn05.BorderColor = Color.Black;
            onOffUcIn05.BorderRadius = 24D;
            onOffUcIn05.BorderSize = 1D;
            onOffUcIn05.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn05.Index = -1;
            onOffUcIn05.IsTextShrinkToFit = true;
            onOffUcIn05.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn05.LabelForeColor = SystemColors.ControlText;
            onOffUcIn05.LabelText = "input 5";
            onOffUcIn05.Location = new Point(3, 233);
            onOffUcIn05.Name = "onOffUcIn05";
            onOffUcIn05.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn05.OffBackColor = Color.Gray;
            onOffUcIn05.OffToggleColor = Color.Gainsboro;
            onOffUcIn05.OnBackColor = Color.DodgerBlue;
            onOffUcIn05.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn05.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn05.Size = new Size(168, 40);
            onOffUcIn05.TabIndex = 11;
            onOffUcIn05.Value = false;
            onOffUcIn05.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn06
            // 
            onOffUcIn06.BackColor = SystemColors.Window;
            onOffUcIn06.BorderColor = Color.Black;
            onOffUcIn06.BorderRadius = 24D;
            onOffUcIn06.BorderSize = 1D;
            onOffUcIn06.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn06.Index = -1;
            onOffUcIn06.IsTextShrinkToFit = true;
            onOffUcIn06.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn06.LabelForeColor = SystemColors.ControlText;
            onOffUcIn06.LabelText = "input 6";
            onOffUcIn06.Location = new Point(3, 279);
            onOffUcIn06.Name = "onOffUcIn06";
            onOffUcIn06.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn06.OffBackColor = Color.Gray;
            onOffUcIn06.OffToggleColor = Color.Gainsboro;
            onOffUcIn06.OnBackColor = Color.DodgerBlue;
            onOffUcIn06.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn06.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn06.Size = new Size(168, 40);
            onOffUcIn06.TabIndex = 12;
            onOffUcIn06.Value = false;
            onOffUcIn06.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // onOffUcIn07
            // 
            onOffUcIn07.BackColor = SystemColors.Window;
            onOffUcIn07.BorderColor = Color.Black;
            onOffUcIn07.BorderRadius = 24D;
            onOffUcIn07.BorderSize = 1D;
            onOffUcIn07.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn07.Index = -1;
            onOffUcIn07.IsTextShrinkToFit = true;
            onOffUcIn07.LabelFont = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcIn07.LabelForeColor = SystemColors.ControlText;
            onOffUcIn07.LabelText = "input 7";
            onOffUcIn07.Location = new Point(3, 325);
            onOffUcIn07.Name = "onOffUcIn07";
            onOffUcIn07.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcIn07.OffBackColor = Color.Gray;
            onOffUcIn07.OffToggleColor = Color.Gainsboro;
            onOffUcIn07.OnBackColor = Color.DodgerBlue;
            onOffUcIn07.OnToggleColor = Color.WhiteSmoke;
            onOffUcIn07.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcIn07.Size = new Size(168, 40);
            onOffUcIn07.TabIndex = 13;
            onOffUcIn07.Value = false;
            onOffUcIn07.ValueChanged += onOffUcIn_ValueChanged;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(onOffUcOut00);
            flowLayoutPanel2.Controls.Add(onOffUcOut01);
            flowLayoutPanel2.Controls.Add(onOffUcOut02);
            flowLayoutPanel2.Controls.Add(onOffUcOut03);
            flowLayoutPanel2.Controls.Add(onOffUcOut04);
            flowLayoutPanel2.Controls.Add(onOffUcOut05);
            flowLayoutPanel2.Controls.Add(onOffUcOut06);
            flowLayoutPanel2.Controls.Add(onOffUcOut07);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(912, 64);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(176, 376);
            flowLayoutPanel2.TabIndex = 8;
            // 
            // onOffUcOut00
            // 
            onOffUcOut00.BackColor = SystemColors.Window;
            onOffUcOut00.BorderColor = Color.Black;
            onOffUcOut00.BorderRadius = 24D;
            onOffUcOut00.BorderSize = 1D;
            onOffUcOut00.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut00.Index = -1;
            onOffUcOut00.IsTextShrinkToFit = true;
            onOffUcOut00.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut00.LabelForeColor = SystemColors.ControlText;
            onOffUcOut00.LabelText = "output 0";
            onOffUcOut00.Location = new Point(3, 3);
            onOffUcOut00.Name = "onOffUcOut00";
            onOffUcOut00.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut00.OffBackColor = Color.Gray;
            onOffUcOut00.OffToggleColor = Color.Gainsboro;
            onOffUcOut00.OnBackColor = Color.DodgerBlue;
            onOffUcOut00.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut00.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut00.Size = new Size(168, 40);
            onOffUcOut00.TabIndex = 6;
            onOffUcOut00.Value = false;
            onOffUcOut00.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut01
            // 
            onOffUcOut01.BackColor = SystemColors.Window;
            onOffUcOut01.BorderColor = Color.Black;
            onOffUcOut01.BorderRadius = 24D;
            onOffUcOut01.BorderSize = 1D;
            onOffUcOut01.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut01.Index = -1;
            onOffUcOut01.IsTextShrinkToFit = true;
            onOffUcOut01.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut01.LabelForeColor = SystemColors.ControlText;
            onOffUcOut01.LabelText = "output 1";
            onOffUcOut01.Location = new Point(3, 49);
            onOffUcOut01.Name = "onOffUcOut01";
            onOffUcOut01.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut01.OffBackColor = Color.Gray;
            onOffUcOut01.OffToggleColor = Color.Gainsboro;
            onOffUcOut01.OnBackColor = Color.DodgerBlue;
            onOffUcOut01.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut01.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut01.Size = new Size(168, 40);
            onOffUcOut01.TabIndex = 7;
            onOffUcOut01.Value = false;
            onOffUcOut01.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut02
            // 
            onOffUcOut02.BackColor = SystemColors.Window;
            onOffUcOut02.BorderColor = Color.Black;
            onOffUcOut02.BorderRadius = 24D;
            onOffUcOut02.BorderSize = 1D;
            onOffUcOut02.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut02.Index = -1;
            onOffUcOut02.IsTextShrinkToFit = true;
            onOffUcOut02.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut02.LabelForeColor = SystemColors.ControlText;
            onOffUcOut02.LabelText = "output 2";
            onOffUcOut02.Location = new Point(3, 95);
            onOffUcOut02.Name = "onOffUcOut02";
            onOffUcOut02.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut02.OffBackColor = Color.Gray;
            onOffUcOut02.OffToggleColor = Color.Gainsboro;
            onOffUcOut02.OnBackColor = Color.DodgerBlue;
            onOffUcOut02.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut02.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut02.Size = new Size(168, 40);
            onOffUcOut02.TabIndex = 8;
            onOffUcOut02.Value = false;
            onOffUcOut02.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut03
            // 
            onOffUcOut03.BackColor = SystemColors.Window;
            onOffUcOut03.BorderColor = Color.Black;
            onOffUcOut03.BorderRadius = 24D;
            onOffUcOut03.BorderSize = 1D;
            onOffUcOut03.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut03.Index = -1;
            onOffUcOut03.IsTextShrinkToFit = true;
            onOffUcOut03.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut03.LabelForeColor = SystemColors.ControlText;
            onOffUcOut03.LabelText = "output 3";
            onOffUcOut03.Location = new Point(3, 141);
            onOffUcOut03.Name = "onOffUcOut03";
            onOffUcOut03.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut03.OffBackColor = Color.Gray;
            onOffUcOut03.OffToggleColor = Color.Gainsboro;
            onOffUcOut03.OnBackColor = Color.DodgerBlue;
            onOffUcOut03.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut03.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut03.Size = new Size(168, 40);
            onOffUcOut03.TabIndex = 9;
            onOffUcOut03.Value = false;
            onOffUcOut03.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut04
            // 
            onOffUcOut04.BackColor = SystemColors.Window;
            onOffUcOut04.BorderColor = Color.Black;
            onOffUcOut04.BorderRadius = 24D;
            onOffUcOut04.BorderSize = 1D;
            onOffUcOut04.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut04.Index = -1;
            onOffUcOut04.IsTextShrinkToFit = true;
            onOffUcOut04.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut04.LabelForeColor = SystemColors.ControlText;
            onOffUcOut04.LabelText = "output 4";
            onOffUcOut04.Location = new Point(3, 187);
            onOffUcOut04.Name = "onOffUcOut04";
            onOffUcOut04.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut04.OffBackColor = Color.Gray;
            onOffUcOut04.OffToggleColor = Color.Gainsboro;
            onOffUcOut04.OnBackColor = Color.DodgerBlue;
            onOffUcOut04.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut04.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut04.Size = new Size(168, 40);
            onOffUcOut04.TabIndex = 10;
            onOffUcOut04.Value = false;
            onOffUcOut04.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut05
            // 
            onOffUcOut05.BackColor = SystemColors.Window;
            onOffUcOut05.BorderColor = Color.Black;
            onOffUcOut05.BorderRadius = 24D;
            onOffUcOut05.BorderSize = 1D;
            onOffUcOut05.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut05.Index = -1;
            onOffUcOut05.IsTextShrinkToFit = true;
            onOffUcOut05.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut05.LabelForeColor = SystemColors.ControlText;
            onOffUcOut05.LabelText = "output 5";
            onOffUcOut05.Location = new Point(3, 233);
            onOffUcOut05.Name = "onOffUcOut05";
            onOffUcOut05.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut05.OffBackColor = Color.Gray;
            onOffUcOut05.OffToggleColor = Color.Gainsboro;
            onOffUcOut05.OnBackColor = Color.DodgerBlue;
            onOffUcOut05.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut05.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut05.Size = new Size(168, 40);
            onOffUcOut05.TabIndex = 11;
            onOffUcOut05.Value = false;
            onOffUcOut05.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut06
            // 
            onOffUcOut06.BackColor = SystemColors.Window;
            onOffUcOut06.BorderColor = Color.Black;
            onOffUcOut06.BorderRadius = 24D;
            onOffUcOut06.BorderSize = 1D;
            onOffUcOut06.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut06.Index = -1;
            onOffUcOut06.IsTextShrinkToFit = true;
            onOffUcOut06.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut06.LabelForeColor = SystemColors.ControlText;
            onOffUcOut06.LabelText = "output 6";
            onOffUcOut06.Location = new Point(3, 279);
            onOffUcOut06.Name = "onOffUcOut06";
            onOffUcOut06.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut06.OffBackColor = Color.Gray;
            onOffUcOut06.OffToggleColor = Color.Gainsboro;
            onOffUcOut06.OnBackColor = Color.DodgerBlue;
            onOffUcOut06.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut06.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut06.Size = new Size(168, 40);
            onOffUcOut06.TabIndex = 12;
            onOffUcOut06.Value = false;
            onOffUcOut06.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // onOffUcOut07
            // 
            onOffUcOut07.BackColor = SystemColors.Window;
            onOffUcOut07.BorderColor = Color.Black;
            onOffUcOut07.BorderRadius = 24D;
            onOffUcOut07.BorderSize = 1D;
            onOffUcOut07.Font = new Font("Yu Gothic UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut07.Index = -1;
            onOffUcOut07.IsTextShrinkToFit = true;
            onOffUcOut07.LabelFont = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUcOut07.LabelForeColor = SystemColors.ControlText;
            onOffUcOut07.LabelText = "output 7";
            onOffUcOut07.Location = new Point(3, 325);
            onOffUcOut07.Name = "onOffUcOut07";
            onOffUcOut07.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUcOut07.OffBackColor = Color.Gray;
            onOffUcOut07.OffToggleColor = Color.Gainsboro;
            onOffUcOut07.OnBackColor = Color.DodgerBlue;
            onOffUcOut07.OnToggleColor = Color.WhiteSmoke;
            onOffUcOut07.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUcOut07.Size = new Size(168, 40);
            onOffUcOut07.TabIndex = 13;
            onOffUcOut07.Value = false;
            onOffUcOut07.ValueChanged += onOffUcOut_ValueChanged;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // onOffUserControl2
            // 
            onOffUserControl2.BackColor = SystemColors.Window;
            onOffUserControl2.BorderColor = Color.Black;
            onOffUserControl2.BorderRadius = 25D;
            onOffUserControl2.BorderSize = 1D;
            onOffUserControl2.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl2.Index = -1;
            onOffUserControl2.IsTextShrinkToFit = true;
            onOffUserControl2.LabelFont = new Font("Yu Gothic UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            onOffUserControl2.LabelForeColor = SystemColors.ControlText;
            onOffUserControl2.LabelText = "trigger";
            onOffUserControl2.Location = new Point(424, 8);
            onOffUserControl2.Name = "onOffUserControl2";
            onOffUserControl2.Nickname = "onOffUserControl1 [Hutzper.Library.Forms.Setting.OnOffUserControl]";
            onOffUserControl2.OffBackColor = Color.Gray;
            onOffUserControl2.OffToggleColor = Color.Gainsboro;
            onOffUserControl2.OnBackColor = Color.DodgerBlue;
            onOffUserControl2.OnToggleColor = Color.WhiteSmoke;
            onOffUserControl2.RoundedCorner = Library.Common.Drawing.RoundedCorner.LeftTop | Library.Common.Drawing.RoundedCorner.RightTop | Library.Common.Drawing.RoundedCorner.RightBottom | Library.Common.Drawing.RoundedCorner.LeftBottom;
            onOffUserControl2.Size = new Size(168, 40);
            onOffUserControl2.TabIndex = 9;
            onOffUserControl2.Value = false;
            onOffUserControl2.ValueChanged += onOffUserControl2_ValueChanged;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(600, 16);
            numericUpDown2.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(96, 27);
            numericUpDown2.TabIndex = 10;
            numericUpDown2.TextAlign = HorizontalAlignment.Right;
            numericUpDown2.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(696, 24);
            label1.Name = "label1";
            label1.Size = new Size(49, 20);
            label1.TabIndex = 11;
            label1.Text = "ms on";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(848, 24);
            label2.Name = "label2";
            label2.Size = new Size(50, 20);
            label2.TabIndex = 13;
            label2.Text = "ms off";
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(752, 16);
            numericUpDown3.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numericUpDown3.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(96, 27);
            numericUpDown3.TabIndex = 12;
            numericUpDown3.TextAlign = HorizontalAlignment.Right;
            numericUpDown3.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1008, 24);
            label3.Name = "label3";
            label3.Size = new Size(38, 20);
            label3.TabIndex = 15;
            label3.Text = "num";
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(912, 16);
            numericUpDown4.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(96, 27);
            numericUpDown4.TabIndex = 14;
            numericUpDown4.TextAlign = HorizontalAlignment.Right;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(256, 24);
            label4.Name = "label4";
            label4.Size = new Size(37, 20);
            label4.TabIndex = 16;
            label4.Text = "port";
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1109, 450);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(numericUpDown4);
            Controls.Add(label2);
            Controls.Add(numericUpDown3);
            Controls.Add(label1);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(onOffUserControl2);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(onOffUserControl1);
            Controls.Add(panelContainerOfLog);
            MaximumSize = new Size(1131, 506);
            Name = "Form1";
            Nickname = "Form1 [Hutzper.Library.Forms.ServiceCollectionSharingForm], Text: Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "疑似DigitalIO";
            Load += onOffUserControl2_ValueLoad;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private NumericUpDown numericUpDown1;
        private Panel panelContainerOfLog;
        private Library.Forms.Setting.OnOffUserControl onOffUserControl1;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn00;
        private FlowLayoutPanel flowLayoutPanel1;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn01;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn02;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn03;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn04;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn05;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn06;
        private Library.Forms.Setting.OnOffUserControl onOffUcIn07;
        private FlowLayoutPanel flowLayoutPanel2;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut00;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut01;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut02;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut03;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut04;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut05;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut06;
        private Library.Forms.Setting.OnOffUserControl onOffUcOut07;
        private System.Windows.Forms.Timer timer1;
        private Library.Forms.Setting.OnOffUserControl onOffUserControl2;
        private NumericUpDown numericUpDown2;
        private Label label1;
        private Label label2;
        private NumericUpDown numericUpDown3;
        private Label label3;
        private NumericUpDown numericUpDown4;
        private Label label4;
    }
}