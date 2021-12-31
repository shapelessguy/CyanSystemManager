using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public partial class serviceBoxControl : UserControl
    {
        public Service runService = null;
        bool started = false;
        State previousState = State.NEUTRAL;
        public serviceBoxControl()
        {
            InitializeComponent();
            serviceBox.Checked = false;
            check.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void SetTitle(string title)
        {
            serviceBox.Text = title;
        }

        private void serviceBoxControl_Load(object sender, EventArgs e)
        {
            Timer timeCheck = new Timer() { Enabled = true, Interval = 50 };
            timeCheck.Tick += (o, ea) =>
            {
                if (runService != null && !started)
                {
                    //Console.WriteLine(runService.statusFromBox);
                    //if (runService.statusFromBox == State.ON) startup = true;
                    //serviceBox.Checked = startup;
                    //runService.statusFromBox = State.NEUTRAL;
                    serviceBox.Checked = runService.startup;
                    started = true;
                }
                if (serviceBox.Text != runService.friendlyName) serviceBox.Text = runService.friendlyName;
                State actualState = runService.status;
                if(actualState != previousState)
                {
                    previousState = actualState;
                    if (actualState == State.ON) { check.BackgroundImage = Properties.Resources.checkTrue; }
                    else if (actualState == State.OFF) { check.BackgroundImage = Properties.Resources.checkFalse; }
                    else if (actualState == State.NEUTRAL) { check.BackgroundImage = null; }
                }
                //Console.WriteLine(runService.status);
            };
        }

        private void serviceBox_CheckedChanged(object sender, EventArgs e)
        {
            ServiceManager.timeToSave = true;
            if(runService != null)
            {
                State newStatus = State.OFF;
                if (((CheckBox)sender).Checked) newStatus = State.ON;
                if(started) runService.statusFromBox = newStatus;
            }
        }
    }
}
