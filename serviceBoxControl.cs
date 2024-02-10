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
using System.Threading;
using System.Xml.Serialization;

namespace CyanSystemManager
{
    public partial class serviceBoxControl : UserControl
    {
        public Service runService;
        bool started;
        State previousState;
        System.Windows.Forms.Timer timeCheck;
        public serviceBoxControl()
        {
            runService = null;
            started = false;
            previousState = State.NEUTRAL;
            InitializeComponent();
            serviceBox.Checked = false;
            check.BackgroundImageLayout = ImageLayout.Stretch;
            check.BackgroundImage = Properties.Resources.checkNeutral;
        }

        public void SetTitle(string title)
        {
            serviceBox.Text = title;
        }

        private void serviceBoxControl_Load(object sender, EventArgs e)
        {
            timeCheck = new System.Windows.Forms.Timer() { Enabled = true, Interval = 500 };
            timeCheck.Tick += (o, ea) =>
            {
                if (runService.generation < ServiceManager.generation)
                {
                    timeCheck.Dispose();
                }
                if (runService != null && !started)
                {
                    //Log(runService.statusFromBox);
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
                    ToolStripMenuItem start = null, network = null;
                    foreach (ToolStripMenuItem v in Home.notifyIcon.ContextMenuStrip.Items)
                    {
                        if (v.ToString() == "Start now!") start = v;
                        else if (v.ToString() == "Net Stats") network = v;
                    }

                    if (actualState == State.ON) { 
                        check.BackgroundImage = Properties.Resources.checkTrue;
                        if (runService.friendlyName == "Start Service") start.Enabled = true;
                        if (runService.friendlyName == "Network Service") network.Enabled = true;
                    }
                    else if (actualState == State.OFF) { 
                        check.BackgroundImage = Properties.Resources.checkFalse;
                        if (runService.friendlyName == "Start Service") start.Enabled = false;
                        if (runService.friendlyName == "Network Service") network.Enabled = false;
                    }
                    else { check.BackgroundImage = Properties.Resources.checkNeutral; }
                }
                //Log(runService.status);
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
