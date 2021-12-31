using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class ScreenSaverForm : Form
    {
        public static bool active = false;
        public Screen follow;
        public Timer timerClose = new Timer() { Enabled = true, Interval = 50 };
        public ScreenSaverForm()
        {
            active = true;
            InitializeComponent();
            Timer afterWhile = new Timer() { Enabled = true, Interval = 100 };
            afterWhile.Tick += (o, e) => { afterWhile.Dispose(); LocationChanged += (ob, ex) => { active = false; }; };
            timerClose.Tick += CheckClose;
            Home.setTopAndTransparent(Handle);
        }
        private void CheckClose(object o, EventArgs e)
        {
            if (follow != null && Bounds != follow.Bounds) Bounds = follow.Bounds;
            if (!active) { Close(); timerClose.Dispose(); }
        }
    }
}
