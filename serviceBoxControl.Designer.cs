namespace CyanSystemManager
{
    partial class serviceBoxControl
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceBox = new System.Windows.Forms.CheckBox();
            this.check = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serviceBox
            // 
            this.serviceBox.AutoSize = true;
            this.serviceBox.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceBox.ForeColor = System.Drawing.Color.White;
            this.serviceBox.Location = new System.Drawing.Point(38, 0);
            this.serviceBox.Name = "serviceBox";
            this.serviceBox.Size = new System.Drawing.Size(87, 26);
            this.serviceBox.TabIndex = 6;
            this.serviceBox.Text = "Service";
            this.serviceBox.UseVisualStyleBackColor = true;
            this.serviceBox.CheckedChanged += new System.EventHandler(this.serviceBox_CheckedChanged);
            // 
            // check
            // 
            this.check.BackColor = System.Drawing.Color.Transparent;
            this.check.Location = new System.Drawing.Point(3, 0);
            this.check.Name = "check";
            this.check.Size = new System.Drawing.Size(26, 26);
            this.check.TabIndex = 7;
            // 
            // serviceBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.check);
            this.Controls.Add(this.serviceBox);
            this.Name = "serviceBoxControl";
            this.Size = new System.Drawing.Size(151, 26);
            this.Load += new System.EventHandler(this.serviceBoxControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox serviceBox;
        public System.Windows.Forms.Label check;
    }
}
