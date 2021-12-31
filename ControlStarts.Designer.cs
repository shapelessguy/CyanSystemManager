namespace CyanSystemManager
{
    partial class ControlStarts
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
            this.commandBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.processBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.escBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // commandBox
            // 
            this.commandBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.commandBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commandBox.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commandBox.ForeColor = System.Drawing.Color.White;
            this.commandBox.Location = new System.Drawing.Point(478, 35);
            this.commandBox.Name = "commandBox";
            this.commandBox.Size = new System.Drawing.Size(225, 26);
            this.commandBox.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(369, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 22);
            this.label5.TabIndex = 18;
            this.label5.Text = "Command:";
            // 
            // processBox
            // 
            this.processBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.processBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.processBox.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processBox.ForeColor = System.Drawing.Color.White;
            this.processBox.Location = new System.Drawing.Point(146, 35);
            this.processBox.Name = "processBox";
            this.processBox.Size = new System.Drawing.Size(217, 26);
            this.processBox.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(4, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 22);
            this.label4.TabIndex = 16;
            this.label4.Text = "Process name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(5, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 22);
            this.label3.TabIndex = 15;
            this.label3.Text = "Path:";
            // 
            // pathBox
            // 
            this.pathBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.pathBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pathBox.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathBox.ForeColor = System.Drawing.Color.White;
            this.pathBox.Location = new System.Drawing.Point(66, 3);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(608, 26);
            this.pathBox.TabIndex = 14;
            // 
            // escBtn
            // 
            this.escBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.escBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.escBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.escBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.escBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.escBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.escBtn.ForeColor = System.Drawing.Color.Black;
            this.escBtn.Location = new System.Drawing.Point(681, 5);
            this.escBtn.Name = "escBtn";
            this.escBtn.Size = new System.Drawing.Size(22, 22);
            this.escBtn.TabIndex = 20;
            this.escBtn.Text = "X";
            this.escBtn.UseVisualStyleBackColor = false;
            this.escBtn.Click += new System.EventHandler(this.escBtn_Click);
            // 
            // ControlStarts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.escBtn);
            this.Controls.Add(this.commandBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.processBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pathBox);
            this.Name = "ControlStarts";
            this.Size = new System.Drawing.Size(709, 65);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox commandBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox processBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.Button escBtn;
    }
}
