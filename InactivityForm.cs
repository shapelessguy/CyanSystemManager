﻿using System.Drawing;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class InactivityForm : Form
    {
        static public bool active;
        static private int countdown;
        Timer timerClose, timerCheck;
        public InactivityForm()
        {
            active = true;
            countdown = 2 * 60;
            InitializeComponent();
            Home.setTopAndTransparent(Handle);

            Point mousePosition = Cursor.Position;
            timerCheck = new Timer() { Enabled = true, Interval = 20 };
            timerCheck.Tick += (o, e) => { if (Cursor.Position != mousePosition) { timerCheck.Dispose(); closeForm(); }};

            timerClose = new Timer() { Enabled = true, Interval = countdown * 1000 };
            timerClose.Tick += (o, e) => { Program.cmdAsync("cmd", "/C shutdown -f -s"); 
                                            clickPls.Text = "System will restart soon"; };

            Show();
            FormClosed += (o, e) => { active = false;};
        }
        private void closeForm()
        {
            if (timerCheck != null) timerCheck.Dispose();
            if (timerClose != null) timerClose.Dispose();
            Close();
        }
        public void locate(Screen screen)
        {
            int x = screen.Bounds.X + (screen.Bounds.Width - Width) / 2;
            int y = screen.Bounds.Y + (screen.Bounds.Height - Height) / 2;
            Location = new Point(x, y);
        }
    }
}
