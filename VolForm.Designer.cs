namespace CyanSystemManager
{
    partial class VolForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.container = new System.Windows.Forms.Panel();
            this.volBar = new System.Windows.Forms.Label();
            this.terminal = new System.Windows.Forms.Label();
            this.volValue = new System.Windows.Forms.Label();
            this.textDevice = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.container.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.container.Controls.Add(this.volBar);
            this.container.Controls.Add(this.terminal);
            this.container.Location = new System.Drawing.Point(30, 9);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(42, 176);
            this.container.TabIndex = 0;
            // 
            // volBar
            // 
            this.volBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.volBar.BackColor = System.Drawing.Color.DarkRed;
            this.volBar.Location = new System.Drawing.Point(3, 61);
            this.volBar.Name = "volBar";
            this.volBar.Size = new System.Drawing.Size(32, 108);
            this.volBar.TabIndex = 0;
            // 
            // terminal
            // 
            this.terminal.BackColor = System.Drawing.Color.Black;
            this.terminal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.terminal.Location = new System.Drawing.Point(0, 0);
            this.terminal.Name = "terminal";
            this.terminal.Size = new System.Drawing.Size(38, 7);
            this.terminal.TabIndex = 1;
            // 
            // volValue
            // 
            this.volValue.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volValue.ForeColor = System.Drawing.Color.White;
            this.volValue.Location = new System.Drawing.Point(3, 192);
            this.volValue.Name = "volValue";
            this.volValue.Size = new System.Drawing.Size(95, 36);
            this.volValue.TabIndex = 1;
            this.volValue.Text = "100";
            this.volValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textDevice
            // 
            this.textDevice.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textDevice.ForeColor = System.Drawing.Color.White;
            this.textDevice.Location = new System.Drawing.Point(78, 9);
            this.textDevice.Name = "textDevice";
            this.textDevice.Size = new System.Drawing.Size(716, 36);
            this.textDevice.TabIndex = 2;
            this.textDevice.Text = "Creative Audigy High Definition Audio";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(820, 228);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // VolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.ClientSize = new System.Drawing.Size(820, 228);
            this.Controls.Add(this.textDevice);
            this.Controls.Add(this.volValue);
            this.Controls.Add(this.container);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VolForm";
            this.ShowInTaskbar = false;
            this.Text = "VolForm";
            this.TransparencyKey = System.Drawing.Color.DeepSkyBlue;
            this.container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel container;
        public System.Windows.Forms.Label volBar;
        private System.Windows.Forms.Label volValue;
        private System.Windows.Forms.Label textDevice;
        private System.Windows.Forms.Label terminal;
        private System.Windows.Forms.PictureBox pictureBox;
    }
}