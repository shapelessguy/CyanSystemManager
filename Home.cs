using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Utility;
using Timer = System.Windows.Forms.Timer;

namespace CyanSystemManager
{
    public partial class Home : Form
    {
        static readonly IContainer componentsNotify = new Container();
        static public NotifyIcon notifyIcon;
        static public BindElements binded = new BindElements();
        static public List<serviceBoxControl> serviceControls = new List<serviceBoxControl>();
        static public Thread runTimeSupport;
        public Home()
        {
            InitializeComponent();
            //FormClosing += Closing;
            FurtherInitialization();
            Settings.apply();
            buttonPressed.KeyDown += findKey;
            Load += LoadHome;
            panel1.MouseMove += form_MouseMove;
            foreach (Control ctrl in panel1.Controls) if(ctrl.Name != "escBtn") ctrl.MouseMove += form_MouseMove;


        }
        private void LoadHome(object o, EventArgs e)
        {
            createNotify();
            createServiceControls();
            ServiceManager.loadActiveServices(true);

            runTimeSupport = new Thread(RunTime);
            runTimeSupport.Start();
        }
        private void Closing(object sender, EventArgs e)
        {
            Program.timeToClose = true;
            Timer timerClose = new Timer() { Enabled = true, Interval = 2000 };
            timerClose.Tick += (o, ea) => { Close(); };

            Timer timerEmClose = new Timer() { Enabled = true, Interval = 6000 };
            timerEmClose.Tick += (o, ea) => { killMainProcess(); };
        }

        private void RunTime()
        {
            DateTime date = DateTime.UtcNow;
            double difference = 9999999;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string directory = Path.Combine(path, "StartUp_Time");
            string file = Path.Combine(directory, "time.txt");
            Thread.Sleep(1000);
            try
            {
                string text = File.ReadAllText(file);
                try { date = DateTime.FromFileTimeUtc(Convert.ToInt64(text)); } catch (Exception) { }
                difference = DateTime.Now.ToUniversalTime().Subtract(date).TotalSeconds;
                Console.WriteLine("Date: " + date.ToLocalTime());
                Console.WriteLine("Difference: " + difference);
            }
            catch (Exception) { }


            bool is_Startup = difference < 120;
            //MessageBox.Show(date + "    " + difference + "    " + is_Startup);

            if(is_Startup && Properties.Settings.Default.startOnReboot) Service_Start.SystemStart(true);
        }

        private void createServiceControls()
        {
            foreach(Service service in ServiceManager.allServices)
            {
                serviceControls.Add(new serviceBoxControl() { runService = service });
                panel3.Controls.Add(serviceControls[serviceControls.Count - 1]);
            }
            for(int i=0; i<serviceControls.Count; i++)
            {
                serviceBoxControl ctrl = serviceControls[i];
                ctrl.Location = new Point(5, servicesLabel.Height+ servicesLabel.Location.Y + 15 + (ctrl.Height+10)*i);
                ctrl.Size = new Size(panel3.Width - ctrl.Location.X, ctrl.Height);
            }
        }

        private void FurtherInitialization()
        {
            new ControlPanel(this);
            WindowState = FormWindowState.Minimized;
            FormClosing += (o, e) =>
            {
                if (Program.timeToClose) return;
                e.Cancel = true;
                Visible = false;
            };
        }

        private void createNotify()
        {
            notifyIcon = new NotifyIcon(componentsNotify)
            {
                Icon = (Icon)Properties.Resources.ResourceManager.GetObject("manager"),
                Text = "SystemManager",
                Visible = true,
            };
            TrayMenuContext();
            notifyIcon.MouseClick += (o, e) => { if (e.Button == MouseButtons.Left)
                { 
                    WindowState = FormWindowState.Normal; 
                    Visible = true;
                    SetForegroundWindow(this.Handle);
                    buttonPressed.Focus();
                } };
        }
        void TrayMenuContext()
        {
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            //notifyIcon.ContextMenuStrip.Items.Add("Start on reboot", Properties.Resources.managerPNG, Statistics_Click);
            //ToolStripMenuItem startOnReboot = (ToolStripMenuItem)notifyIcon.ContextMenuStrip.Items[0];
            //startOnReboot.DropDownItems.Add("YES", Properties.Resources.managerPNG, YeReboot_Click);
            //startOnReboot.DropDownItems.Add("NO", Properties.Resources.managerPNG, NoReboot_Click);
            //startOnReboot.Checked = Properties.Settings.Default.startOnReboot;
            notifyIcon.ContextMenuStrip.Items.Add("Start now!", Properties.Resources.managerPNG, Now_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Net Stats", Properties.Resources.managerPNG, Statistics_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", Properties.Resources.managerPNG, MenuExit_Click);
        }
        private void Now_Click(object sender, EventArgs e) { Service_Start.SystemStart(false); }
        private void YeReboot_Click(object sender, EventArgs e) {Reboot_Click(true); }
        private void NoReboot_Click(object sender, EventArgs e) {Reboot_Click(false);}
        private void Reboot_Click(bool value)
        {
            Properties.Settings.Default.startOnReboot = value;
            ToolStripMenuItem startOnReboot = (ToolStripMenuItem)notifyIcon.ContextMenuStrip.Items[0];
            startOnReboot.Checked = Properties.Settings.Default.startOnReboot;
            Properties.Settings.Default.Save();
        }

        private void Statistics_Click(object sender, EventArgs e) {new Statistics();}
        private void MenuExit_Click(object sender, EventArgs e)
        {
            Closing(null, null);
        }

        private void killMainProcess()
        {
            void kill()
            {
                Thread.Sleep(1000);
                foreach (Process clsProcess in Process.GetProcesses())
                    if (clsProcess.ProcessName == Application.ProductName)
                    { clsProcess.Kill(); Console.WriteLine("Process killed"); }
            }
            new Thread(kill).Start();
        }

        public static void registerHotkeys(string service)
        {
            foreach (BindDef def in activityList)
                if (def.service == service)
                    binded.keyList.Add(new Key(def.service, def.id, def.name, def.key, 
                                                def.modifier, def.mode, def.trigger, def.initialTrigger));
            for (int i = 0; i < binded.keyList.Count; i++)
                RegisterHotKey(Program.home.Handle, binded.keyList[i].id, (int)binded.keyList[i].modifier, 
                                    binded.keyList[i].key.GetHashCode());
            binded.makeConsistent();
        }
        public static void unregisterHotkeys(string service)
        {
            for (int i = 0; i < binded.keyList.Count; i++)
                if (binded.keyList[i].service == service)
                    UnregisterHotKey(Program.home.Handle, binded.keyList[i].id);
            binded.makeConsistent();
        }

        public static void setTopAndTransparent(IntPtr window, bool transparent=true)
        {
            if (transparent)
            {
                int wl = GetWindowLong(window, GWL.ExStyle) | 0x20;
                SetWindowLong(window, GWL.ExStyle, wl);
            }
            SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            //SetLayeredWindowAttributes(window, 0, 128, LWA.ColorKey);
        }

        private static int WM_QUERYENDSESSION = 0x11;
        private static int WM_ENDSESSION = 0x16;
        public const uint SHUTDOWN_NORETRY = 0x00000001;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_QUERYENDSESSION) || m.Msg.Equals(WM_ENDSESSION) || m.Msg.Equals(SHUTDOWN_NORETRY))
            {
                killMainProcess();
            }
            if (m.Msg == 0x0312)
            {
                // Note that the three lines below are not needed if you only want to register one hotkey.
                // The below lines are useful in case you want to register multiple keys, which you can use 
                // a switch with the id as argument, or if you want to know which key/modifier was pressed 
                // for some particular reason.

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);          // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
                int id = m.WParam.ToInt32();                                // The id of the hotkey that was pressed.
                Binded bind = binded.findKey(key, modifier);
                if (bind.key.name == "") return;

                bind.activity.call();

            }
            base.WndProc(ref m);
        }
        private void findKey(object o, KeyEventArgs e)
        {
            buttonPressed.Text = "";
            e.SuppressKeyPress = true;
            Keys keyName = e.KeyCode;
            bool Ctrl = e.Control;
            bool Shift = e.Shift;
            bool Alt = e.Alt;
            string modifiers = "";
            if (Shift) modifiers += "Shift +";
            if (Ctrl) modifiers += "Ctrl +";
            if (Alt) modifiers += "Alt +";
            modifiers += " ";
            //if (modifiers.Length > 0) modifiers = modifiers.Substring(0, modifiers.Length - 2);

            if (keyName == Keys.Menu || keyName == Keys.Shift || keyName == Keys.Control || keyName == Keys.ControlKey) return;
            buttonPressed.Text = modifiers + keyName;
        }
        private void button1_Click(object sender, EventArgs e) {Close();}


        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
