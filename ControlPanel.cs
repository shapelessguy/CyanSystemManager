using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Vanara.PInvoke;
using System.Configuration;  // Add a reference to System.Configuration.dll
using static CyanSystemManager.Settings;
using static System.Net.Mime.MediaTypeNames;
using static Vanara.PInvoke.Kernel32.RETRIEVAL_POINTERS_BUFFER;

namespace CyanSystemManager
{
    public class WinSet
    {
        public string name;
        public application app;
        public bool enabled = false;
        public Monitor monitor = null;
        public string monitor_placeholder = "null";
        public List<string> avail_monitors = new List<string>();
        public Point location = new Point(0, 0);
        public Size size = new Size(0, 0);
        public WinSet(string name, application app)
        {
            this.name = name;
            this.app = app;
            foreach(Monitor monitor in MonitorManager.allMonitors) { avail_monitors.Add(monitor.id); }
        }

        public void changeMonitor(string new_monitor)
        {
            foreach (Monitor monitor in MonitorManager.allMonitors)
            {
                if (monitor.id == new_monitor)
                {
                    this.monitor = monitor;
                    break;
                }
            }
        }

        public void overWrite(List<string> serial)
        {
            if (serial.Count > 1) { enabled = Convert.ToBoolean(serial[1]); }
            if (serial.Count > 2) {
                monitor_placeholder = serial[2];
                foreach (Monitor monitor in MonitorManager.allMonitors) { 
                    if (monitor.id == serial[2]) { 
                        this.monitor = monitor; 
                        break; 
                    } 
                } 
            }
            if (serial.Count > 4) { location = new Point(Convert.ToInt16(serial[3]), Convert.ToInt16(serial[4])); }
            if (serial.Count > 6) { size = new Size(Convert.ToInt16(serial[5]), Convert.ToInt16(serial[6])); }
        }

        public string stringify()
        {
            string out_ = "";
            out_ += name + ":";
            out_ += Convert.ToString(enabled) + ":";
            out_ += (monitor == null ? monitor_placeholder : monitor.id) + ":";
            out_ += location.X + ":" + location.Y + ":";
            out_ += size.Width + ":" + size.Height + ";";
            return out_;
        }
    }
    public class ControlPanel
    {
        public Home home;
        public string ctrlPanel;
        public static List<WinSet> winSets;
        public bool initializing;
        static public string settings_path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

        CheckBox allowIdleBox, startBox, runBox;
        TextBox checkSite;
        Label checkSite_l;
        List<Object> allControls;
        public Color btnBackColor = Color.FromArgb(48, 48, 48);
        public Color btnSelectionColor = Color.FromArgb(90,90,90);
        public Color btnOver = Color.FromArgb(60,60,60);
        public Color btnClick = Color.FromArgb(90, 90, 90);
        public ControlPanel(Home home_)
        {
            ctrlPanel = "";
            winSets = new List<WinSet>();
            initializing = true;
            allControls = new List<object>();
            home = home_;

            runBox = initializeBox("runBox_pan1", "Run CyanSystemManager at startup", new Point(16, 14 + 0 * 30), new Size(200, 26));
            runBox.Checked = Properties.Settings.Default.runAtStartup;
            runBox.CheckedChanged += (o, e) => {
                try
                {
                    RunAtStartup(runBox.Checked);
                    Properties.Settings.Default.runAtStartup = runBox.Checked;
                    Properties.Settings.Default.Save();
                }
                catch (Exception) { MessageBox.Show("Unkwnown error."); }
            };
            allControls.Add(runBox);

            allowIdleBox = initializeBox("allowIdleBox_pan1", "Allow idle after 2 hours of inactivity and between 00:00 and 8:00", new Point(16, 14 + 1 * 30), new Size(200, 26));
            allowIdleBox.Checked = Properties.Settings.Default.allowIdle;
            allowIdleBox.CheckedChanged += (o, e) => {
                Properties.Settings.Default.allowIdle = allowIdleBox.Checked;
                Properties.Settings.Default.Save(); 
            };
            allControls.Add(allowIdleBox);

            startBox = initializeBox("startBox_pan1", "Execute applications at startup", new Point(16, 14 + 2 * 30), new Size(200, 26));
            startBox.Checked = Properties.Settings.Default.startOnReboot;
            startBox.CheckedChanged += (o, e) => {
                Properties.Settings.Default.startOnReboot = startBox.Checked;
                Properties.Settings.Default.Save();
            };
            allControls.Add(startBox);

            Label firebaseAPI_l = initializeLabel("firebaseAPI_l_pan1", "Firebase Key (Pub IP): ", new Point(16, 14 + 3 * 30), new Size(555, 26));
            allControls.Add(firebaseAPI_l);

            TextBox firebaseAPI = initializeTextBox("firebaseAPI_pan1", Properties.Settings.Default.firebaseApiKey, new Point(16 + firebaseAPI_l.Width, 14 + 3 * 30 + 1), new Size(664 - firebaseAPI_l.Width, 26));
            firebaseAPI.PasswordChar = '\u25CF';
            firebaseAPI.Font = new Font("Cambria", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            firebaseAPI.LostFocus += (o, e) => {
                Properties.Settings.Default.firebaseApiKey = firebaseAPI.Text;
                Properties.Settings.Default.Save();
            };
            allControls.Add(firebaseAPI);

            checkSite_l = initializeLabel("checkSite_l_pan1", "Check updates: ", new Point(16, 14 + 4 * 30), new Size(555, 26));
            allControls.Add(checkSite_l);

            checkSite = initializeTextBox("checkSite_pan1", Properties.Settings.Default.checkSite, new Point(16 + checkSite_l.Width, 14 + 4 * 30), new Size(664 - checkSite_l.Width, 26));
            checkSite.LostFocus += (o, e) => {
                File.Delete(CheckSiteUpdate.localFileSite);
                Properties.Settings.Default.checkSite = checkSite.Text;
                Properties.Settings.Default.Save();
            };
            allControls.Add(checkSite);

            Button exportConfig_btn = initializeButton("exportConfig_pan1", "Export", new Point(20, home.panel4.Height - 50), new Size(150, 35));
            exportConfig_btn.MouseClick += (o, e) => Export();
            allControls.Add(exportConfig_btn);

            Button importConfig_btn = initializeButton("importConfig_pan1", "Import", new Point(20 + 150 + 15, home.panel4.Height - 50), new Size(150, 35));
            importConfig_btn.MouseClick += (o, e) => Import();
            allControls.Add(importConfig_btn);

            Button getPosition_btn = initializeButton("get_position_pan2", "Get positions", new Point(300, 6), new Size(300, 35));
            getPosition_btn.MouseClick += (o, e) => getPositions();
            allControls.Add(getPosition_btn);

            loadSettings();
            int height_idx = 1;
            foreach (var item in App.getApplications())
            {
                WinSet cur_set = null;
                foreach (WinSet winset in winSets)
                    if (winset.name == item.Key.ToString())
                    {
                        cur_set = winset;
                        break;
                    }
                if (cur_set == null) continue;
                allControls.Add(initializeLabel(item.Key.ToString() + "_lbl_pan2", item.Key.ToString(), new Point(16, 14 + height_idx * 30), new Size(150, 26)));
                CheckBox enabling = initializeBox(item.Key.ToString() + "_enable_pan2", "Move", new Point(166, 14 + height_idx * 30), new Size(90, 26));
                ComboBox combo = initializeComboBox(item.Key.ToString() + "_screen_pan2", item.Key.ToString(), new Point(256, 14 + height_idx * 30), new Size(140, 24));
                TextBox x = initializeTextBox(item.Key.ToString() + "_x_pan2", "", new Point(400 + 70 * 0, 14 + height_idx * 30), new Size(66, 26));
                TextBox y = initializeTextBox(item.Key.ToString() + "_y_pan2", "", new Point(400 + 70 * 1, 14 + height_idx * 30), new Size(66, 26));
                TextBox width = initializeTextBox(item.Key.ToString() + "_width_pan2", "", new Point(400 + 70 * 2, 14 + height_idx * 30), new Size(66, 26));
                TextBox height = initializeTextBox(item.Key.ToString() + "_height_pan2", "", new Point(400 + 70 * 3, 14 + height_idx * 30), new Size(66, 26));
                combo.Enabled = false;
                x.Enabled = false;
                y.Enabled = false;
                width.Enabled = false;
                height.Enabled = false;

                enabling.CheckedChanged += (o, e) =>
                {
                    cur_set.enabled = enabling.Checked;
                    combo.Enabled = enabling.Checked;
                    x.Enabled = enabling.Checked;
                    y.Enabled = enabling.Checked;
                    width.Enabled = enabling.Checked;
                    height.Enabled = enabling.Checked;
                    saveWinSets();
                };
                combo.SelectedIndexChanged += (o, e) => {
                    if (initializing) return;
                    cur_set.changeMonitor(combo.SelectedItem.ToString());
                    saveWinSets();
                };
                x.TextChanged += (o, e) =>
                {
                    if (initializing) return;
                    cur_set.location = new Point(tryConvert(x), tryConvert(y));
                    saveWinSets();
                };
                y.TextChanged += (o, e) =>
                {
                    if (initializing) return;
                    cur_set.location = new Point(tryConvert(x), tryConvert(y));
                    saveWinSets();
                };
                width.TextChanged += (o, e) =>
                {
                    if (initializing) return;
                    cur_set.size = new Size(tryConvert(width), tryConvert(height));
                    saveWinSets();
                };
                height.TextChanged += (o, e) =>
                {
                    if (initializing) return;
                    cur_set.size = new Size(tryConvert(width), tryConvert(height));
                    saveWinSets();
                };
                allControls.Add(enabling);
                allControls.Add(combo);
                allControls.Add(x);
                allControls.Add(y);
                allControls.Add(width);
                allControls.Add(height);
                height_idx += 1;
            }
            assignOptions();

            foreach (Control ctrl in home.panel4.Controls) ctrl.Hide();
            foreach (Button btn in home.panel2.Controls.OfType<Button>())
            {
                btn.BackColor = btnBackColor; 
                btn.MouseClick += buttonClick;
                btn.FlatAppearance.MouseDownBackColor = btnClick;
                btn.FlatAppearance.MouseOverBackColor = btnOver;
            }
            buttonClick(home.generalBtn, null);
            initializing = false;
        }

        private void Export()
        {
            string dir_path = getDirName();
            if (dir_path == null || dir_path == "") return;
            if (Directory.Exists(dir_path))
            {
                File.Copy(settings_path, Path.Combine(dir_path, "CyanSystemManager_settings_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".config"));
            }
        }

        private void Import()
        {
            string file_name = getFileName();
            if (File.Exists(file_name))
            {
                File.Copy(file_name, settings_path, true);
                MessageBox.Show("Settings applied correctly! This application will restart now.");
                Program.restart = true;
                home.SafeClose(null, null);
            }
        }

        private string getDirName()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }
            return "";
        }

        private string getFileName()
        {
            OpenFileDialog op1 = new OpenFileDialog();
            op1.Filter = "User config|*.config";

            DialogResult result = op1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                return op1.FileName;
            }
            return "";
        }

        private void getPositions()
        {
            IDictionary<IntPtr, string> OpenWindows = WindowWrapper.GetOpenWindows();
            bool changed = false;
            foreach (WinSet winset in winSets)
            {
                if (winset.enabled)
                {
                    try
                    {
                        Window win = new Window(OpenWindows, winset.app);
                        int x = win.x;
                        int y = win.y;
                        int width = win.width;
                        int heigth = win.height;
                        int centerx = x + width / 2;
                        int centery = y + heigth / 2;
                        Monitor monitor = MonitorManager.allMonitors[0];
                        foreach (Monitor mon in MonitorManager.allMonitors)
                        {
                            Rectangle rect = new Rectangle(mon.x, mon.y, mon.width, mon.height);
                            if (rect.Contains(new Point(centerx, centery))) { monitor = mon; break; }
                        }
                        Point rel_location = new Point(x - monitor.x, y - monitor.y);
                        Size size = new Size(width, heigth);
                        winset.monitor = monitor;
                        winset.location = rel_location;
                        winset.size = size;
                        changed = true;
                    }
                    catch { }
                }
            }
            if (changed) { assignOptions(); }
        }

        private int tryConvert(TextBox txtBox)
        {
            try
            {
                return Convert.ToInt16(txtBox.Text);
            }
            catch { txtBox.Text = "0"; }
            return 0;
        }

        public void saveWinSets()
        {
            if (initializing) return;
            string out_ = "";
            foreach (WinSet winset in winSets) out_ += winset.stringify();
            Properties.Settings.Default.winSet = out_;
            Properties.Settings.Default.Save();
        }

        public void loadSettings()
        {
            MonitorManager.GetMonitors();
            winSets.Clear();
            foreach (var item in App.getApplications())
            {
                winSets.Add(new WinSet(item.Key.ToString(), item.Value));
            }
            string set = Properties.Settings.Default.winSet;
            List<string> serial = new List<string>();
            int j = 0;
            for (int i = 0; i< set.Length; i++)
            {
                if (set.Substring(i, 1) == ":") { 
                    serial.Add(set.Substring(j, i - j)); 
                    j = i + 1; 
                }
                if (set.Substring(i, 1) == ";") { 
                    serial.Add(set.Substring(j, i - j));
                    foreach (WinSet winSet in winSets) 
                    {
                        if (winSet.name == serial[0])
                        {
                            winSet.overWrite(serial);
                            break;
                        }
                    }
                    serial = new List<string>();
                    j = i + 1;
                }
            }
        }

        private void assignOptions()
        {
            initializing= true;
            Dictionary<string, WinSet> dict = new Dictionary<string, WinSet>();
            foreach (WinSet winSet in winSets) dict[winSet.name] = winSet;
            foreach(Control ctrl in allControls)
            {
                string key = ctrl.Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (dict.ContainsKey(key))
                {
                    WinSet winSet = dict[key];
                    if (ctrl.Name.Contains("_enable_pan2"))
                    {
                        ((CheckBox)ctrl).Checked = winSet.enabled;
                    }
                    else if (ctrl.Name.Contains("_screen_pan2"))
                    {
                        string[] items = winSet.avail_monitors.ToArray();
                        if (((ComboBox)ctrl).DataSource == null) ((ComboBox)ctrl).DataSource = items;
                        ((ComboBox)ctrl).SelectedItem = winSet.monitor == null ? items[0] : winSet.monitor.id;
                        //Console.WriteLine(winSet.monitor.id + "   " + ((ComboBox)ctrl).SelectedItem);
                    }
                    else if (ctrl.Name.Contains("_x_pan2"))
                    {
                        ((TextBox)ctrl).Text = winSet.location.X.ToString();
                    }
                    else if (ctrl.Name.Contains("_y_pan2"))
                    {
                        ((TextBox)ctrl).Text = winSet.location.Y.ToString();
                    }
                    else if (ctrl.Name.Contains("_width_pan2"))
                    {
                        ((TextBox)ctrl).Text = winSet.size.Width.ToString();
                    }
                    else if (ctrl.Name.Contains("_height_pan2"))
                    {
                        ((TextBox)ctrl).Text = winSet.size.Height.ToString();
                    }
                }
            }
            initializing = false;
            saveWinSets();
        }

        private void RunAtStartup(bool active)
        {
            string vbs_script_model = Properties.Resources.vbs_script;
            string exe_folder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string vbs_script = vbs_script_model.Replace("RELEASEPATH", exe_folder);
            string file_location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), "CyanSystemManager.vbs");

            if (active)
            {
                using (StreamWriter sw = new StreamWriter(file_location)) sw.Write(vbs_script);
            }
            else
            {
                if (File.Exists(file_location)) File.Delete(file_location);
            }
        }

        public void objShow(Control obj, string identifier)
        {
            if (obj.Name.Contains("_" + identifier)) obj.Show();
        }

        public void changePanel()
        {
            foreach (Control ctrl in home.panel4.Controls) ctrl.Hide();
            string pan = "pan";
            if (ctrlPanel == "generalBtn")
            {
                pan = "pan1";
                foreach (Label obj in allControls.OfType<Label>()) objShow(obj, pan);
                foreach (CheckBox obj in allControls.OfType<CheckBox>()) objShow(obj, pan);
                foreach (TextBox obj in allControls.OfType<TextBox>()) objShow(obj, pan);
                foreach (Button obj in allControls.OfType<Button>()) objShow(obj, pan);
            }
            else if(ctrlPanel == "startBtn")
            {
                pan = "pan2";
                foreach (Label obj in allControls.OfType<Label>()) objShow(obj, pan);
                foreach (CheckBox obj in allControls.OfType<CheckBox>()) objShow(obj, pan);
                foreach (ComboBox obj in allControls.OfType<ComboBox>()) objShow(obj, pan);
                foreach (TextBox obj in allControls.OfType<TextBox>()) objShow(obj, pan);
                foreach (Button obj in allControls.OfType<Button>()) objShow(obj, pan);
            }
        }

        public void buttonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string name = button.Name;
            if (name == ctrlPanel) return;
            //Console.WriteLine(name);
            foreach (Button btn in home.panel2.Controls.OfType<Button>())
            {
                if (btn.Name != name) btn.BackColor = btnBackColor;
                else { btn.BackColor = btnSelectionColor; ctrlPanel = name; changePanel(); }
            }
        }

        private CheckBox initializeBox(string name, string text, Point location, Size size)
        {
            CheckBox Box = new CheckBox();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.Location = location;
            Box.Name = name;
            Box.Size = size;
            Box.Text = text;
            Box.UseVisualStyleBackColor = true;
            home.panel4.Controls.Add(Box);
            return Box;
        }
        private Label initializeLabel(string name, string text, Point location, Size size)
        {
            Label Box = new Label();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.Location = location;
            Box.Name = name;
            Box.Text = text;
            home.panel4.Controls.Add(Box);
            return Box;
        }
        private TextBox initializeTextBox(string name, string text, Point location, Size size)
        {
            TextBox Box = new TextBox();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.BackColor = btnBackColor;
            Box.Location = location;
            Box.Name = name;
            Box.Size = size;
            Box.Text = text;
            home.panel4.Controls.Add(Box);
            return Box;
        }
        private ComboBox initializeComboBox(string name, string text, Point location, Size size)
        {
            ComboBox Box = new ComboBox();
            Box.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.BackColor = btnBackColor;
            Box.Location = location;
            Box.Name = name;
            Box.Size = size;
            Box.MouseWheel += (o, e) => {
                ((HandledMouseEventArgs)e).Handled = true;
            };
            home.panel4.Controls.Add(Box);
            return Box;
        }

        private Button initializeButton(string name, string text, Point location, Size size)
        {
            Button Box = new Button();
            Box.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.BackColor = btnBackColor;
            Box.Location = location;
            Box.Name = name;
            Box.Text = text;
            Box.Size = size;
            home.panel4.Controls.Add(Box);
            return Box;
        }
    }
}
