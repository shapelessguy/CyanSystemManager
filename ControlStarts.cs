using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class ControlStarts : UserControl
    {
        public bool filled = false;
        public bool toBeRemoved = false;
        public infos info;
        public ControlStarts()
        {
            InitializeComponent();
            new Timer() { Enabled = true, Interval = 100 }.Tick += (o, e) => { Check(); };
        }
        private void Check()
        {
            filled = pathBox.Text != "" || processBox.Text != "" || commandBox.Text != "";
        }

        private void escBtn_Click(object sender, EventArgs e) { toBeRemoved = true; }
        public infos getInfo()
        {
            info = new infos(pathBox.Text, processBox.Text, commandBox.Text);


            return info;
        }
    }
    public class infos
    {
        public string path, process, command;
        public infos(string path, string process, string command)
        {
            this.path = path;
            this.process = process;
            this.command = command;
        }
    }
}
