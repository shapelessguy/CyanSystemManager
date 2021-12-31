namespace CyanSystemManager
{
    partial class InactivityForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InactivityForm));
            this.clickPls = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // clickPls
            // 
            this.clickPls.BackColor = System.Drawing.Color.Transparent;
            this.clickPls.Font = new System.Drawing.Font("Cambria", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clickPls.Location = new System.Drawing.Point(49, 48);
            this.clickPls.Name = "clickPls";
            this.clickPls.Size = new System.Drawing.Size(387, 111);
            this.clickPls.TabIndex = 0;
            this.clickPls.Text = "System will shut down in few minutes.\r\nMove the mouse!";
            this.clickPls.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InactivityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.BackgroundImage = global::CyanSystemManager.Properties.Resources.BlueTag;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(494, 208);
            this.Controls.Add(this.clickPls);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "InactivityForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InactivityForm";
            this.TransparencyKey = System.Drawing.Color.Blue;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label clickPls;
    }
}