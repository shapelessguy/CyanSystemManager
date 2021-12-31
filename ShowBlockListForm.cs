using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class ShowBlockListForm : Form
    {
        static public bool active = false;
        static private Timer timerUpdate;
        //static List<Button> blocked = new List<Button>();
        public ShowBlockListForm()
        {
            active = true;
            FormClosing += (o, e) => { timerUpdate.Dispose(); active = false; };
            InitializeComponent();
            timerUpdate = new Timer() { Enabled = true, Interval = 50 };
            timerUpdate.Tick += update;
        }
        public void update(object sender, EventArgs e)
        {
            bool updated = false;
            for(int i= panel1.Controls.Count-1; i>=0; i--) if (!Charts.blockList.Contains(panel1.Controls[i].Name)) 
                { panel1.Controls.Remove(panel1.Controls[i]); updated = true; }
            foreach (string blocked_ in Charts.blockList)
            {
                bool found = false;
                foreach (Button btn in panel1.Controls.OfType<Button>()) if (btn.Name == blocked_) found = true;
                if (!found)
                {
                    Button newBtn = getNewButton(blocked_);
                    panel1.Controls.Add(newBtn);
                    updated = true;
                }
            }
            if (updated) locate();
        }
        public void locate()
        {
            if (panel1.Controls.Count == 0) return;
            int marginx = 10;
            for(int i=0; i< panel1.Controls.Count; i++)
            {
                panel1.Controls[i].Location = new Point(marginx, i * (panel1.Controls[0].Height + 3));
                panel1.Controls[i].Size = new Size(panel1.Width - 2 * marginx, 30);
            }
        }

        private Button getNewButton(string name)
        {
            Button button = new Button();
            button.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button.FlatAppearance.BorderColor = Color.FromArgb(64,64,64);
            button.FlatAppearance.MouseDownBackColor = Color.Silver;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(64,64,64);
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button.ForeColor = Color.White;
            button.Name = name;
            button.TabIndex = 0;
            button.Text = name;
            button.UseVisualStyleBackColor = true;
            button.Click += (o, e) =>
            {
                string name_ = ((Button)o).Name;
                if (Charts.blockList.Contains(name_)) Charts.blockList.Remove(name_);
                if (!Charts.allowList.Contains(name_)) Charts.allowList.Add(name_);
                Charts.reallyToSave = true;
                new Timer() { Enabled = true, Interval = 200 }.Tick += (os, ex) => { locate(); };
            };

            return button;
        }
    }
}
