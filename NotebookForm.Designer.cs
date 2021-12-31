using System.Windows.Forms;

namespace CyanSystemManager
{
    partial class NotebookForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.del_btn = new System.Windows.Forms.Button();
            this.new_btn = new System.Windows.Forms.Button();
            this.tab_lab = new System.Windows.Forms.Label();
            this.cmbTabs = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.cmbFont = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.wall_btn = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.colPal1 = new CyanSystemManager.NotebookColPal();
            this.Italic = new CyanSystemManager.NotebookStyleT();
            this.Regular = new CyanSystemManager.NotebookStyleT();
            this.Bold = new CyanSystemManager.NotebookStyleT();
            this.Underline = new CyanSystemManager.NotebookStyleT();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.panel1.Controls.Add(this.wall_btn);
            this.panel1.Controls.Add(this.del_btn);
            this.panel1.Controls.Add(this.new_btn);
            this.panel1.Controls.Add(this.tab_lab);
            this.panel1.Controls.Add(this.cmbTabs);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.Italic);
            this.panel1.Controls.Add(this.Regular);
            this.panel1.Controls.Add(this.Bold);
            this.panel1.Controls.Add(this.Underline);
            this.panel1.Controls.Add(this.cmbSize);
            this.panel1.Controls.Add(this.cmbFont);
            this.panel1.Location = new System.Drawing.Point(0, 386);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1145, 64);
            this.panel1.TabIndex = 0;
            // 
            // del_btn
            // 
            this.del_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.del_btn.BackColor = System.Drawing.Color.Silver;
            this.del_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.del_btn.Location = new System.Drawing.Point(1067, 31);
            this.del_btn.Name = "del_btn";
            this.del_btn.Size = new System.Drawing.Size(75, 25);
            this.del_btn.TabIndex = 22;
            this.del_btn.Text = "Delete";
            this.del_btn.UseVisualStyleBackColor = false;
            this.del_btn.Click += new System.EventHandler(this.button2_Click);
            // 
            // new_btn
            // 
            this.new_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.new_btn.BackColor = System.Drawing.Color.Silver;
            this.new_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.new_btn.Location = new System.Drawing.Point(1067, 8);
            this.new_btn.Name = "new_btn";
            this.new_btn.Size = new System.Drawing.Size(75, 25);
            this.new_btn.TabIndex = 21;
            this.new_btn.Text = "New";
            this.new_btn.UseVisualStyleBackColor = false;
            this.new_btn.Click += new System.EventHandler(this.button1_Click);
            // 
            // tab_lab
            // 
            this.tab_lab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tab_lab.AutoSize = true;
            this.tab_lab.Font = new System.Drawing.Font("Modern No. 20", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab_lab.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.tab_lab.Location = new System.Drawing.Point(788, 22);
            this.tab_lab.Name = "tab_lab";
            this.tab_lab.Size = new System.Drawing.Size(46, 24);
            this.tab_lab.TabIndex = 20;
            this.tab_lab.Text = "Tab";
            // 
            // cmbTabs
            // 
            this.cmbTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTabs.CausesValidation = false;
            this.cmbTabs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTabs.FormattingEnabled = true;
            this.cmbTabs.ItemHeight = 24;
            this.cmbTabs.Location = new System.Drawing.Point(840, 18);
            this.cmbTabs.Name = "cmbTabs";
            this.cmbTabs.Size = new System.Drawing.Size(221, 32);
            this.cmbTabs.TabIndex = 19;
            this.cmbTabs.SelectedIndexChanged += new System.EventHandler(this.cmbTabs_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.colPal1);
            this.panel2.Location = new System.Drawing.Point(488, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(46, 39);
            this.panel2.TabIndex = 18;
            // 
            // cmbSize
            // 
            this.cmbSize.CausesValidation = false;
            this.cmbSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(429, 22);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(53, 28);
            this.cmbSize.TabIndex = 9;
            this.cmbSize.TextChanged += new System.EventHandler(this.cmbSize_TextChanged);
            // 
            // cmbFont
            // 
            this.cmbFont.CausesValidation = false;
            this.cmbFont.Font = new System.Drawing.Font("Modern No. 20", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbFont.FormattingEnabled = true;
            this.cmbFont.ItemHeight = 15;
            this.cmbFont.Location = new System.Drawing.Point(8, 24);
            this.cmbFont.Name = "cmbFont";
            this.cmbFont.Size = new System.Drawing.Size(283, 23);
            this.cmbFont.TabIndex = 8;
            this.cmbFont.TextChanged += new System.EventHandler(this.cmbFont_TextChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1121, 368);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            this.richTextBox1.SelectionChanged += new System.EventHandler(this.richTextBox1_SelectionChanged);
            // 
            // wall_btn
            // 
            this.wall_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.wall_btn.BackColor = System.Drawing.Color.Snow;
            this.wall_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wall_btn.Location = new System.Drawing.Point(726, 23);
            this.wall_btn.Name = "wall_btn";
            this.wall_btn.Size = new System.Drawing.Size(46, 25);
            this.wall_btn.TabIndex = 23;
            this.wall_btn.Text = "Wall";
            this.wall_btn.UseVisualStyleBackColor = false;
            this.wall_btn.Click += new System.EventHandler(this.wall_btn_Click);
            // 
            // colPal1
            // 
            this.colPal1.BackColor = System.Drawing.Color.Black;
            this.colPal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colPal1.Location = new System.Drawing.Point(4, 4);
            this.colPal1.Name = "colPal1";
            this.colPal1.Size = new System.Drawing.Size(28, 28);
            this.colPal1.TabIndex = 0;
            // 
            // Italic
            // 
            this.Italic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Italic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Italic.Location = new System.Drawing.Point(363, 25);
            this.Italic.Name = "Italic";
            this.Italic.Size = new System.Drawing.Size(27, 21);
            this.Italic.TabIndex = 17;
            // 
            // Regular
            // 
            this.Regular.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Regular.Location = new System.Drawing.Point(297, 25);
            this.Regular.Name = "Regular";
            this.Regular.Size = new System.Drawing.Size(27, 21);
            this.Regular.TabIndex = 15;
            // 
            // Bold
            // 
            this.Bold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Bold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bold.Location = new System.Drawing.Point(330, 25);
            this.Bold.Name = "Bold";
            this.Bold.Size = new System.Drawing.Size(27, 21);
            this.Bold.TabIndex = 14;
            // 
            // Underline
            // 
            this.Underline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Underline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Underline.Location = new System.Drawing.Point(396, 25);
            this.Underline.Name = "Underline";
            this.Underline.Size = new System.Drawing.Size(27, 21);
            this.Underline.TabIndex = 13;
            // 
            // Appunti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1145, 450);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "Appunti";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Appunti";
            this.Load += new System.EventHandler(this.Appunti_Load);
            this.VisibleChanged += new System.EventHandler(this.Appunti_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private NotebookColPal colPal1;
        private System.Windows.Forms.ComboBox cmbFont;
        private System.Windows.Forms.ComboBox cmbSize;
        private NotebookStyleT Italic;
        private NotebookStyleT Regular;
        private NotebookStyleT Bold;
        private NotebookStyleT Underline;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbTabs;
        private System.Windows.Forms.Button del_btn;
        private System.Windows.Forms.Button new_btn;
        private System.Windows.Forms.Label tab_lab;
        private Button wall_btn;
        private ColorDialog colorDialog1;
    }
}