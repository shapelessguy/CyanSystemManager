namespace CyanSystemManager
{
    partial class NotebookColPal
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ColPal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Name = "ColPal";
            this.Size = new System.Drawing.Size(58, 55);
            this.Click += new System.EventHandler(this.ColPal_Click);
            this.MouseEnter += new System.EventHandler(this.ColPal_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ColPal_MouseLeave);
            this.ResumeLayout(false);

        }
    }
}
