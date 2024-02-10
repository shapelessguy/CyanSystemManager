using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Vanara.PInvoke;
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

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm"; // Only use hours and minutes
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = new DateTime(2012, 05, 28, 22, 0, 0);
            Size = new Size(1276, 764);
        }
        private void LoadHome(object o, EventArgs e)
        {
            createNotify();
            createServiceControls();
            ServiceManager.loadActiveServices(true);

            runTimeSupport = new Thread(RunTime);
            runTimeSupport.Start();
        }
        public void SafeClose(object sender, EventArgs e)
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
                } };
        }
        void TrayMenuContext()
        {
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Sort Windows", Properties.Resources.managerPNG, Sort_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Start now!", Properties.Resources.managerPNG, Now_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Net Stats", Properties.Resources.managerPNG, Statistics_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", Properties.Resources.managerPNG, MenuExit_Click);


            ToolStripMenuItem start = null, network = null;
            foreach (ToolStripMenuItem v in Home.notifyIcon.ContextMenuStrip.Items)
            {
                if (v.ToString() == "Start now!") start = v;
                else if (v.ToString() == "Net Stats") network = v;
            }
            start.Enabled = false;
            network.Enabled = false;
        }
        private void Sort_Click(object sender, EventArgs e) { Service_Monitor.sort(); }
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
            SafeClose(null, null);
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
                SafeClose(null, null);
            }
            if (m.Msg == 0x0312)
            {
                // Note that the three lines below are not needed if you only want to register one hotkey.
                // The below lines are useful in case you want to register multiple keys, which you can use 
                // a switch with the id as argument, or if you want to know which key/modifier was pressed 
                // for some particular reason.
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);          // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
                writeKeyText(key, modifier == KeyModifier.Control, modifier == KeyModifier.Shift, modifier == KeyModifier.Alt);
                int id = m.WParam.ToInt32();                                // The id of the hotkey that was pressed.
                Binded bind = binded.findKey(key, modifier);
                if (bind.key.name == "") return;
                if (buttonPressed.Focused) { return; }
                bind.activity.call();

            }
            base.WndProc(ref m);
        }
        private void findKey(object o, KeyEventArgs e)
        {
            buttonPressed.Text = "";
            e.SuppressKeyPress = true;
            Keys key = e.KeyCode;
            writeKeyText(key, e.Control, e.Shift, e.Alt);
        }
        private void writeKeyText(Keys key, bool Ctrl, bool Shift, bool Alt)
        {
            if (!buttonPressed.Focused) buttonPressed.Text = "";
            else
            {
                string modifiers = "";
                if (Shift && key != Keys.ShiftKey) modifiers += "Shift +";
                if (Ctrl && key != Keys.ControlKey) modifiers += "Ctrl +";
                if (Alt && key != Keys.Menu) modifiers += "Alt +";
                modifiers += " ";
                string key_str = key.ToString();
                if (key_str.EndsWith("Key")) key_str = key_str.Substring(0, key_str.Length - 3);
                if (key_str == "Menu") key_str = "Alt";
                buttonPressed.Text = modifiers + key_str;
            }
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

        private void menu_btn_Click(object sender, EventArgs e)
        {
            menu_panel.BringToFront();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            icon_panel.BringToFront();
        }
        private static readonly HttpClient client = new HttpClient();
        private async Task<bool> sendHTTP(string topic, string arg)
        {
            try
            {
                var values = new Dictionary<string, string>
                  {
                      { topic, arg },
                  };

                var content = new FormUrlEncodedContent(values);

                Console.WriteLine("http://" + FirebaseClass.serverIp + ":10001/" + topic);
                var response = await client.PostAsync("http://" + FirebaseClass.serverIp + ":10001/" + topic, content);

                using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1")))
                {
                    var responseString = sr.ReadToEnd();
                    Program.Log(responseString);
                }
            }
            catch (Exception ex) { Program.Log(ex.Message); return false; }
            return true;
        }

        private void emitSound()
        {
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Default.wav"))
            {
                soundPlayer.Play();
            }
        }

        private async void plant_leds_on_btn_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "on");
        }

        private async void plant_leds_off_btn_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "off");
        }

        private async void plant_leds_auto_btn_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "auto");
        }

        private async void plant_leds_autoset_btn_Click(object sender, EventArgs e)
        {
            string hour = dateTimePicker1.Value.Hour.ToString();
            hour = hour.Length == 1 ? "0" + hour : hour;
            string minute = dateTimePicker1.Value.Minute.ToString();
            minute = minute.Length == 1 ? "0" + minute : minute;
            string arg = "auto " + hour + ":" + minute;
            emitSound();
            await sendHTTP("lights", arg);
        }

        private async void tv_on_btn_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("tv", "on");
        }

        private async void tv_off_btn_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("tv", "off");
        }
    }
}
