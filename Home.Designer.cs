namespace CyanSystemManager
{
    partial class Home
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

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.escBtn = new System.Windows.Forms.Button();
            this.buttonPressed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.icon_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.plant_leds_auto_btn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.plant_leds_off_btn = new System.Windows.Forms.Button();
            this.plant_leds_autoset_btn = new System.Windows.Forms.Button();
            this.plant_leds_on_btn = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.tv_off_btn = new System.Windows.Forms.Button();
            this.tv_on_btn = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.menu_btn = new System.Windows.Forms.Button();
            this.menu_panel = new System.Windows.Forms.Panel();
            this.to_icon_menu = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.servicesLabel = new System.Windows.Forms.Label();
            this.startBtn = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.generalBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.icon_panel.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel10.SuspendLayout();
            this.menu_panel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
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
            this.escBtn.Location = new System.Drawing.Point(1232, 9);
            this.escBtn.Name = "escBtn";
            this.escBtn.Size = new System.Drawing.Size(28, 24);
            this.escBtn.TabIndex = 0;
            this.escBtn.Text = "X";
            this.escBtn.UseVisualStyleBackColor = false;
            this.escBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonPressed
            // 
            this.buttonPressed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPressed.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.buttonPressed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.buttonPressed.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPressed.Location = new System.Drawing.Point(1106, 671);
            this.buttonPressed.Name = "buttonPressed";
            this.buttonPressed.Size = new System.Drawing.Size(145, 23);
            this.buttonPressed.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Cambria", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cyan System Manager";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.escBtn);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1276, 48);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.icon_panel);
            this.panel2.Controls.Add(this.menu_panel);
            this.panel2.Location = new System.Drawing.Point(0, 51);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1276, 713);
            this.panel2.TabIndex = 4;
            // 
            // icon_panel
            // 
            this.icon_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.icon_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(39)))));
            this.icon_panel.Controls.Add(this.panel5);
            this.icon_panel.Controls.Add(this.panel6);
            this.icon_panel.Controls.Add(this.panel7);
            this.icon_panel.Controls.Add(this.panel8);
            this.icon_panel.Controls.Add(this.panel9);
            this.icon_panel.Controls.Add(this.panel10);
            this.icon_panel.Location = new System.Drawing.Point(3, 2);
            this.icon_panel.Name = "icon_panel";
            this.icon_panel.Size = new System.Drawing.Size(1257, 700);
            this.icon_panel.TabIndex = 5;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.plant_leds_auto_btn);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.dateTimePicker1);
            this.panel5.Controls.Add(this.plant_leds_off_btn);
            this.panel5.Controls.Add(this.plant_leds_autoset_btn);
            this.panel5.Controls.Add(this.plant_leds_on_btn);
            this.panel5.Location = new System.Drawing.Point(1, 1);
            this.panel5.Margin = new System.Windows.Forms.Padding(1);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(416, 347);
            this.panel5.TabIndex = 30;
            // 
            // plant_leds_auto_btn
            // 
            this.plant_leds_auto_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plant_leds_auto_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.plant_leds_auto_btn.BackgroundImage = global::CyanSystemManager.Properties.Resources.auto;
            this.plant_leds_auto_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plant_leds_auto_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.plant_leds_auto_btn.Location = new System.Drawing.Point(24, 192);
            this.plant_leds_auto_btn.Margin = new System.Windows.Forms.Padding(0);
            this.plant_leds_auto_btn.Name = "plant_leds_auto_btn";
            this.plant_leds_auto_btn.Size = new System.Drawing.Size(163, 140);
            this.plant_leds_auto_btn.TabIndex = 29;
            this.plant_leds_auto_btn.UseVisualStyleBackColor = false;
            this.plant_leds_auto_btn.Click += new System.EventHandler(this.plant_leds_auto_btn_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(283, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 24);
            this.label3.TabIndex = 28;
            this.label3.Text = "Auto time";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(287, 237);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(85, 31);
            this.dateTimePicker1.TabIndex = 27;
            this.dateTimePicker1.Value = new System.DateTime(2023, 12, 14, 22, 0, 0, 0);
            // 
            // plant_leds_off_btn
            // 
            this.plant_leds_off_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plant_leds_off_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.plant_leds_off_btn.BackgroundImage = global::CyanSystemManager.Properties.Resources.neon_off;
            this.plant_leds_off_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plant_leds_off_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.plant_leds_off_btn.Location = new System.Drawing.Point(237, 18);
            this.plant_leds_off_btn.Margin = new System.Windows.Forms.Padding(0);
            this.plant_leds_off_btn.Name = "plant_leds_off_btn";
            this.plant_leds_off_btn.Size = new System.Drawing.Size(163, 140);
            this.plant_leds_off_btn.TabIndex = 25;
            this.plant_leds_off_btn.UseVisualStyleBackColor = false;
            this.plant_leds_off_btn.Click += new System.EventHandler(this.plant_leds_off_btn_Click);
            // 
            // plant_leds_autoset_btn
            // 
            this.plant_leds_autoset_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plant_leds_autoset_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.plant_leds_autoset_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plant_leds_autoset_btn.ForeColor = System.Drawing.Color.White;
            this.plant_leds_autoset_btn.Location = new System.Drawing.Point(246, 271);
            this.plant_leds_autoset_btn.Margin = new System.Windows.Forms.Padding(0);
            this.plant_leds_autoset_btn.Name = "plant_leds_autoset_btn";
            this.plant_leds_autoset_btn.Size = new System.Drawing.Size(148, 26);
            this.plant_leds_autoset_btn.TabIndex = 26;
            this.plant_leds_autoset_btn.Text = "Set";
            this.plant_leds_autoset_btn.UseVisualStyleBackColor = false;
            this.plant_leds_autoset_btn.Click += new System.EventHandler(this.plant_leds_autoset_btn_Click);
            // 
            // plant_leds_on_btn
            // 
            this.plant_leds_on_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plant_leds_on_btn.BackColor = System.Drawing.Color.DimGray;
            this.plant_leds_on_btn.BackgroundImage = global::CyanSystemManager.Properties.Resources.neon_on;
            this.plant_leds_on_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plant_leds_on_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.plant_leds_on_btn.Location = new System.Drawing.Point(24, 18);
            this.plant_leds_on_btn.Margin = new System.Windows.Forms.Padding(0);
            this.plant_leds_on_btn.Name = "plant_leds_on_btn";
            this.plant_leds_on_btn.Size = new System.Drawing.Size(163, 140);
            this.plant_leds_on_btn.TabIndex = 24;
            this.plant_leds_on_btn.UseVisualStyleBackColor = false;
            this.plant_leds_on_btn.Click += new System.EventHandler(this.plant_leds_on_btn_Click);
            // 
            // panel6
            // 
            this.panel6.Location = new System.Drawing.Point(419, 1);
            this.panel6.Margin = new System.Windows.Forms.Padding(1);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(416, 347);
            this.panel6.TabIndex = 31;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.tv_off_btn);
            this.panel7.Controls.Add(this.tv_on_btn);
            this.panel7.Location = new System.Drawing.Point(837, 1);
            this.panel7.Margin = new System.Windows.Forms.Padding(1);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(416, 347);
            this.panel7.TabIndex = 32;
            // 
            // tv_off_btn
            // 
            this.tv_off_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tv_off_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tv_off_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tv_off_btn.BackgroundImage")));
            this.tv_off_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tv_off_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tv_off_btn.Location = new System.Drawing.Point(79, 186);
            this.tv_off_btn.Margin = new System.Windows.Forms.Padding(0);
            this.tv_off_btn.Name = "tv_off_btn";
            this.tv_off_btn.Size = new System.Drawing.Size(281, 146);
            this.tv_off_btn.TabIndex = 26;
            this.tv_off_btn.UseVisualStyleBackColor = false;
            this.tv_off_btn.Click += new System.EventHandler(this.tv_off_btn_Click);
            // 
            // tv_on_btn
            // 
            this.tv_on_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tv_on_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tv_on_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tv_on_btn.BackgroundImage")));
            this.tv_on_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tv_on_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tv_on_btn.Location = new System.Drawing.Point(79, 12);
            this.tv_on_btn.Margin = new System.Windows.Forms.Padding(0);
            this.tv_on_btn.Name = "tv_on_btn";
            this.tv_on_btn.Size = new System.Drawing.Size(281, 146);
            this.tv_on_btn.TabIndex = 25;
            this.tv_on_btn.UseVisualStyleBackColor = false;
            this.tv_on_btn.Click += new System.EventHandler(this.tv_on_btn_Click);
            // 
            // panel8
            // 
            this.panel8.Location = new System.Drawing.Point(1, 350);
            this.panel8.Margin = new System.Windows.Forms.Padding(1);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(416, 347);
            this.panel8.TabIndex = 33;
            // 
            // panel9
            // 
            this.panel9.Location = new System.Drawing.Point(419, 350);
            this.panel9.Margin = new System.Windows.Forms.Padding(1);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(416, 347);
            this.panel9.TabIndex = 33;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.menu_btn);
            this.panel10.Location = new System.Drawing.Point(837, 350);
            this.panel10.Margin = new System.Windows.Forms.Padding(1);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(416, 347);
            this.panel10.TabIndex = 33;
            // 
            // menu_btn
            // 
            this.menu_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.menu_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menu_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menu_btn.BackgroundImage")));
            this.menu_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menu_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.menu_btn.Location = new System.Drawing.Point(68, 100);
            this.menu_btn.Margin = new System.Windows.Forms.Padding(0);
            this.menu_btn.Name = "menu_btn";
            this.menu_btn.Size = new System.Drawing.Size(281, 146);
            this.menu_btn.TabIndex = 26;
            this.menu_btn.UseVisualStyleBackColor = false;
            this.menu_btn.Click += new System.EventHandler(this.menu_btn_Click);
            // 
            // menu_panel
            // 
            this.menu_panel.Controls.Add(this.to_icon_menu);
            this.menu_panel.Controls.Add(this.label2);
            this.menu_panel.Controls.Add(this.panel3);
            this.menu_panel.Controls.Add(this.startBtn);
            this.menu_panel.Controls.Add(this.panel4);
            this.menu_panel.Controls.Add(this.buttonPressed);
            this.menu_panel.Controls.Add(this.generalBtn);
            this.menu_panel.Location = new System.Drawing.Point(4, 3);
            this.menu_panel.Name = "menu_panel";
            this.menu_panel.Size = new System.Drawing.Size(1258, 699);
            this.menu_panel.TabIndex = 30;
            // 
            // to_icon_menu
            // 
            this.to_icon_menu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.to_icon_menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.to_icon_menu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.to_icon_menu.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.to_icon_menu.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.to_icon_menu.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.to_icon_menu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.to_icon_menu.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.to_icon_menu.Location = new System.Drawing.Point(826, 660);
            this.to_icon_menu.Name = "to_icon_menu";
            this.to_icon_menu.Size = new System.Drawing.Size(105, 33);
            this.to_icon_menu.TabIndex = 11;
            this.to_icon_menu.Text = "Ico Menu";
            this.to_icon_menu.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.to_icon_menu.UseVisualStyleBackColor = false;
            this.to_icon_menu.Click += new System.EventHandler(this.button7_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Cambria", 14.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(971, 670);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Key pressed:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.servicesLabel);
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(243, 693);
            this.panel3.TabIndex = 5;
            // 
            // servicesLabel
            // 
            this.servicesLabel.AutoSize = true;
            this.servicesLabel.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.servicesLabel.Location = new System.Drawing.Point(59, 8);
            this.servicesLabel.Name = "servicesLabel";
            this.servicesLabel.Size = new System.Drawing.Size(104, 28);
            this.servicesLabel.TabIndex = 4;
            this.servicesLabel.Text = "Services";
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.startBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.startBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.startBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Red;
            this.startBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.startBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startBtn.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startBtn.Location = new System.Drawing.Point(350, 660);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(105, 33);
            this.startBtn.TabIndex = 10;
            this.startBtn.Text = "Windows";
            this.startBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.startBtn.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.AutoScroll = true;
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Location = new System.Drawing.Point(252, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1000, 651);
            this.panel4.TabIndex = 8;
            // 
            // generalBtn
            // 
            this.generalBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.generalBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.generalBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.generalBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.generalBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.generalBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.generalBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generalBtn.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generalBtn.Location = new System.Drawing.Point(252, 660);
            this.generalBtn.Name = "generalBtn";
            this.generalBtn.Size = new System.Drawing.Size(92, 33);
            this.generalBtn.TabIndex = 9;
            this.generalBtn.Text = "General";
            this.generalBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.generalBtn.UseVisualStyleBackColor = false;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1276, 764);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Home";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.icon_panel.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.menu_panel.ResumeLayout(false);
            this.menu_panel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button escBtn;
        private System.Windows.Forms.TextBox buttonPressed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label servicesLabel;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button generalBtn;
        public System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.FlowLayoutPanel icon_panel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button plant_leds_auto_btn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button plant_leds_off_btn;
        private System.Windows.Forms.Button plant_leds_autoset_btn;
        private System.Windows.Forms.Button plant_leds_on_btn;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button tv_off_btn;
        private System.Windows.Forms.Button tv_on_btn;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button menu_btn;
        public System.Windows.Forms.Panel menu_panel;
        private System.Windows.Forms.Button to_icon_menu;
    }
}

