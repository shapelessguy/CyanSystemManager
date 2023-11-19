using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        static public BindElements binded;
        static public List<serviceBoxControl> serviceControls;
        static public Thread runTimeSupport;
        static bool startup = false;

        public Home(bool start)
        {
            MonitorManager.initialize_monitors();
            binded = new BindElements(); 
            serviceControls = new List<serviceBoxControl>();
            startup = start;
            InitializeComponent();
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
        public void Closing(object sender, EventArgs e)
        {
            if (notifyIcon != null) notifyIcon.Visible = false;
            Program.forceTermination = true;

            foreach (Service service in ServiceManager.allServices)
            {
                service.stopService(true);
            }

            Timer timerClose = new Timer() { Enabled = true, Interval = 2000 };
            timerClose.Tick += (o, ea) => { Close(); };
        }

        private void RunTime()
        {
            //DateTime date = DateTime.UtcNow;
            //double difference = 9999999;
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            //string directory = Path.Combine(path, "StartUp_Time");
            //string file = Path.Combine(directory, "time.txt");
            //Thread.Sleep(400);
            //try
            // {
            //    Environment.SetEnvironmentVariable("Test1", "Value1");
            //   string start_time = Environment.GetEnvironmentVariable("StartTime");
            //start_time = "03/21/2022-16:01:28,08";
            //   DateTime start_datetime = new DateTime(
            //       Convert.ToUInt16(start_time.Substring(6, 4)),
            //       Convert.ToUInt16(start_time.Substring(0, 2)),
            //       Convert.ToUInt16(start_time.Substring(3, 2)),
            //       Convert.ToUInt16(start_time.Substring(11, 2)),
            //       Convert.ToUInt16(start_time.Substring(14, 2)),
            //       Convert.ToUInt16(start_time.Substring(17, 2))).ToUniversalTime();
            //string text = File.ReadAllText(file);
            //try { date = DateTime.FromFileTimeUtc(Convert.ToInt64(text)); } catch (Exception) { }
            //   difference = DateTime.Now.ToUniversalTime().Subtract(start_datetime).TotalSeconds;
            //Console.WriteLine("Date: " + date.ToLocalTime());
            //   Console.WriteLine("Difference: " + difference);
            //}
            //catch (Exception) { }


            //bool is_Startup = difference < 120;
            //MessageBox.Show(date + "    " + difference + "    " + is_Startup);

            // MessageBox.Show(startup + "   " + Properties.Settings.Default.startOnReboot);
            if(startup && Properties.Settings.Default.startOnReboot) Service_Start.SystemStart(true);
        }

        private void createServiceControls()
        {
            ServiceManager.createServices();
            foreach (Service service in ServiceManager.allServices)
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
                if (Program.forceTermination) return;
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

            notifyIcon.ContextMenuStrip.Items.Add("Sort Windows", Properties.Resources.managerPNG, Sort_Click);

            notifyIcon.ContextMenuStrip.Items.Add("Audio (Arduino)", Properties.Resources.managerPNG);
            notifyIcon.ContextMenuStrip.Items.Add("Start now!", Properties.Resources.managerPNG, Now_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Net Stats", Properties.Resources.managerPNG, Statistics_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", Properties.Resources.managerPNG, MenuExit_Click);


            ToolStripMenuItem audio = null, start = null, network = null;
            foreach (ToolStripMenuItem v in Home.notifyIcon.ContextMenuStrip.Items)
            {
                if (v.ToString() == "Audio (Arduino)") audio = v;
                else if (v.ToString() == "Start now!") start = v;
                else if (v.ToString() == "Net Stats") network = v;
            }
            audio.DropDownItems.Add("ON (keep)", Properties.Resources.managerPNG, turnAudioOnKeep_Click);
            audio.DropDownItems.Add("ON", Properties.Resources.managerPNG, turnAudioOn_Click);
            audio.DropDownItems.Add("OFF", Properties.Resources.managerPNG, turnAudioOff_Click);

            audio.Enabled = false;
            start.Enabled = false;
            network.Enabled = false;
        }
        private void Sort_Click(object sender, EventArgs e) { Service_Monitor.sort(); }
        private void turnLightOn_Click(object sender, EventArgs e) { Service_Arduino.turnLight(true); }
        private void turnLightOff_Click(object sender, EventArgs e) { Service_Arduino.turnLight(false); }
        private void turnAudioOnKeep_Click(object sender, EventArgs e) { Service_Arduino.turnAudio(true, true); }
        private void turnAudioOn_Click(object sender, EventArgs e) { Service_Arduino.turnAudio(true); }
        private void turnAudioOff_Click(object sender, EventArgs e) { Service_Arduino.turnAudio(false); }
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
                Closing(null, null);
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
