using Newtonsoft.Json;
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
using System.Windows.Interop;
using Vanara.PInvoke;
using Windows.UI.Xaml.Media.Animation;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Vanara.PInvoke.User32;
using Timer = System.Windows.Forms.Timer;


namespace CyanSystemManager
{
    public partial class Home : Form
    {
        static readonly IContainer componentsNotify = new Container();
        static public NotifyIcon notifyIcon;
        static public BindElements binded;
        static public List<serviceBoxControl> serviceControls;
        static bool startup = false;
        static public Size winSize = new Size(100, 100);
        static System.Threading.Timer keepAlive;

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
            winSize = Size;
            new Thread(monitorMemConsumption).Start();
            // keepAlive = new System.Threading.Timer(Callback, null, 10000, Timeout.Infinite);
        }

        static async void Callback(Object state) {
            await sendHTTP("audio", "pingvol", false);
            keepAlive.Change(10000, Timeout.Infinite);
        }

        private async void monitorMemConsumption()
        {
            await sendHTTP("audio", "on");
            while (!Program.forceTermination)
            {
                Process currentProcess = Process.GetCurrentProcess();
                float totalBytesOfMemoryUsed = (float)(currentProcess.WorkingSet64 / 1024.0 / 1024.0);
                // Console.WriteLine($"Memory Used: {totalBytesOfMemoryUsed} MB");
                if (totalBytesOfMemoryUsed > 173)
                {
                    Program.restart = true;
                    Program.home.Invoke((MethodInvoker)delegate { SafeClose(null, null); });
                }
                Thread.Sleep(1000);
            }
        }

        private void LoadHome(object o, EventArgs e)
        {
            createNotify();
            createServiceControls();

            ServiceManager.loadActiveServices(true);
            if (startup && Properties.Settings.Default.startOnReboot)  new Thread(SystemStart).Start();
            ProcessStartInfo startInfo = new ProcessStartInfo(variablePath.chatbot)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            foreach(var p in Program.all_processes)
            {
                if (p.ProcessName == "CyanChatBot") p.Kill();
            }
            Process chatbot = Process.Start(startInfo);
            FirebaseClass.UploadIP();
        }
        public void SafeClose(object sender, EventArgs e)
        {
            if (notifyIcon != null) notifyIcon.Visible = false;
            Program.forceTermination = true;

            foreach (Service service in ServiceManager.allServices)
            {
                service.stopService(true);
            }

            keepAlive.Dispose();
            Timer timerClose = new Timer() { Enabled = true, Interval = 2000 };
            timerClose.Tick += (o, ea) => { Close(); };
        }

        private void SystemStart()
        {
            Service_Start.SystemStart(true);
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
        private void Sort_Click(object sender, EventArgs e) {Service_Monitor.sort(); }
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
        private static readonly HttpClient client = new HttpClient{ Timeout = TimeSpan.FromSeconds(1) };
        public static async Task<bool> sendHTTP(string topic, string arg, bool verbose=true)
        {
            try
            {
                var values = new Dictionary<string, string> {{ topic, arg }};
                var jsonContent = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");

                string server_port = "10004";
                if (verbose) Program.Log("http://" + FirebaseClass.serverIp + ":" + server_port + "/" + topic);
                var response = await client.PostAsync("http://" + FirebaseClass.serverIp + ":" + server_port + "/" + topic, jsonContent);

                using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1")))
                {
                    var responseString = sr.ReadToEnd();
                    if (verbose) Program.Log(responseString);
                }
            }
            catch (TaskCanceledException ex)
            {
                if (verbose) Program.Log("Request timed out: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Program.Log("HTTP request failed: " + ex.Message);
                return false;
            }
            return true;
        }

        public async void plant_leds_on_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("lights", "on");
            if (result) { Service_Display.ShowMsg(new MsgSettings("LIGHTS: ON")); }
        }

        public async void plant_leds_off_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("lights", "off");
            if (result) { Service_Display.ShowMsg(new MsgSettings("LIGHTS: OFF")); }
        }

        public async void plant_leds_auto_womsg_set(int hour_, int minute_)
        {
            string hour = hour_.ToString();
            string minute = minute_.ToString();
            hour = hour.Length == 1 ? "0" + hour : hour;
            minute = minute.Length == 1 ? "0" + minute : minute;
            string arg = "auto " + hour + ":" + minute;
            await sendHTTP("lights", arg);
        }
        public async void plant_leds_auto_set(int hour_, int minute_)
        {
            string hour = hour_.ToString();
            string minute = minute_.ToString();
            hour = hour.Length == 1 ? "0" + hour : hour;
            minute = minute.Length == 1 ? "0" + minute : minute;
            string arg = "auto " + hour + ":" + minute;
            var result = await sendHTTP("lights", arg);
            if (result) { Service_Display.ShowMsg(new MsgSettings("LIGHTS AUTO: " + "at " + hour + ":" + minute + " CONFIRMED")); }
        }

        public void plants_leds_auto_in_set(int auto_minutes)
        {
            string minutes = "NOW";
            DateTime currentTime = DateTime.Now;
            DateTime newTime = currentTime.AddMinutes(auto_minutes);
            if (auto_minutes == 0) plant_leds_auto();
            else plant_leds_auto_womsg_set(newTime.Hour, newTime.Minute);
            if (auto_minutes != 0) minutes = "at " + newTime.ToString("HH:mm");
            Service_Display.ShowMsg(new MsgSettings("LIGHTS AUTO: " + minutes + " CONFIRMED"));
        }

        public async void plant_leds_auto_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("lights", "auto");
            if (result) { Service_Display.ShowMsg(new MsgSettings("LIGHTS: AUTO")); }
        }
        public async void plant_leds_auto()
        {
            await sendHTTP("lights", "auto");
        }

        public async void plant_leds_autoset_btn_Click(object sender, EventArgs e)
        {
            string hour = dateTimePicker1.Value.Hour.ToString();
            hour = hour.Length == 1 ? "0" + hour : hour;
            string minute = dateTimePicker1.Value.Minute.ToString();
            minute = minute.Length == 1 ? "0" + minute : minute;
            string arg = "auto " + hour + ":" + minute;

            var result = await sendHTTP("lights", arg);
            if (result) { Service_Display.ShowMsg(new MsgSettings("LIGHTS: AUTO " + hour + ":" + minute)); }
        }

        public async void tv_on_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("tv", "on");
            if (result) { Service_Display.ShowMsg(new MsgSettings("TV: ON")); }
        }

        public async void tv_off_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("tv", "off");
            if (result) { Service_Display.ShowMsg(new MsgSettings("TV: OFF")); }
        }

        public async void audio_on_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "on");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: ON")); }
        }

        public async void audio_off_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "off");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: OFF")); }
        }

        public async void audio_on_off_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "on/off");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: ON/OFF")); }
        }

        private async void input_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "input");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: INPUT")); }
        }

        private async void level_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "level");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: LEVEL")); }
        }

        private async void minus_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "vol-");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: VOL -")); }
        }

        private async void plus_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "vol+");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: VOL +")); }
        }

        public async void audio_effect_btn_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("audio", "effect");
            if (result) { Service_Display.ShowMsg(new MsgSettings("AUDIO: EFFECT")); }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "toplight");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOPLIGHT")); }
        }

        private async void button7_Click_1(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topoff");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOPOFF")); }
        }

        public async void defaultColor(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topG4");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-G4")); }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topG4");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-G4")); }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topB3");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-B3")); }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topR0");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-R0")); }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topG0");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-G0")); }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topB0");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-B0")); }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "topR4");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: TOP-R4")); }
        }

        public async void button9_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "i4");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: I4")); }
        }

        public async void button10_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "i3");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: I3")); }
        }

        public async void button11_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "i2");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: I2")); }
        }

        public async void button12_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "i1");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: I1")); }
        }

        public async void minimumLights(object sender, EventArgs e)
        {
            var result = await sendHTTP("strip", "i0");
            if (result) { Service_Display.ShowMsg(new MsgSettings("STRIP: I0")); }
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            var result = await sendHTTP("announce", "performing");
            if (result) { Service_Display.ShowMsg(new MsgSettings("ANNOUNCE: PERFORMING")); }
        }


        public static void OpenFile(string filePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filePath, // Directly specify the file path
                UseShellExecute = true, // Let the system decide the application based on file association
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to open the file: {ex.Message}");
            }
        }

        private void wg_btn_Click(object sender, EventArgs e)
        {
            // Program.Log("Executing py script");
            // Program.cmd(variablePath.python, variablePath.pyWgScript, true);
            // string clipboardText = Clipboard.GetText(TextDataFormat.Text);
            // if (clipboardText.Substring(0, 10) == "Exception:")
            // {
            //     Clipboard.Clear();
            //     System.Windows.Forms.MessageBox.Show(clipboardText);
            // }
        }

        private void vac_wg_btn_Click(object sender, EventArgs e)
        {
            OpenFile(variablePath.pyVacWgJson);
        }

        private void swap_wg_btn_Click(object sender, EventArgs e)
        {
            OpenFile(variablePath.pySwapWgJson);
        }

        private void show_wg_btn_Click(object sender, EventArgs e)
        {
            // OpenFile(variablePath.pyCalendarWg);
        }
    }
}
