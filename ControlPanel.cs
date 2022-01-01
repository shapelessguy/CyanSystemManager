using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public class ControlPanel
    {
        public Home home;
        public string ctrlPanel = "";

        CheckBox allowIdleBox, startBox, runBox;
        TextBox checkSite;
        Label checkSite_l;
        List<Object> allControls = new List<object>();
        public Color btnBackColor = Color.FromArgb(48, 48, 48);
        public Color btnSelectionColor = Color.FromArgb(90,90,90);
        public Color btnOver = Color.FromArgb(60,60,60);
        public Color btnClick = Color.FromArgb(90, 90, 90);
        public ControlPanel(Home home_)
        {
            home = home_;
            allowIdleBox = initializeBox("allowIdleBox", "Allow idle after 2 hours of inactivity and between 00:00 and 8:00");
            allowIdleBox.Checked = Properties.Settings.Default.allowIdle;
            allowIdleBox.CheckedChanged += (o, e) => {
                Properties.Settings.Default.allowIdle = allowIdleBox.Checked;
                Properties.Settings.Default.Save(); 
            };
            allControls.Add(allowIdleBox);

            runBox = initializeBox("runBox", "Run at startup");
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

            startBox = initializeBox("startBox", "Start at reboot");
            startBox.Checked = Properties.Settings.Default.startOnReboot;
            startBox.CheckedChanged += (o, e) => {
                Properties.Settings.Default.startOnReboot = startBox.Checked;
                Properties.Settings.Default.Save();
            };
            allControls.Add(startBox);

            checkSite_l = initializeLabel("checkSite", "Check updates: ");
            allControls.Add(checkSite_l);

            checkSite = initializeTextBox("checkSite", Properties.Settings.Default.checkSite, checkSite_l);
            checkSite.LostFocus += (o, e) => {
                File.Delete(Settings.variablePath.localFileSite);
                Properties.Settings.Default.checkSite = checkSite.Text;
                Properties.Settings.Default.Save();
            };
            allControls.Add(checkSite);



            foreach (Control ctrl in home.panel4.Controls) ctrl.Hide();
            foreach (Button btn in home.panel2.Controls.OfType<Button>())
            {
                btn.BackColor = btnBackColor; 
                btn.MouseClick += buttonClick;
                btn.FlatAppearance.MouseDownBackColor = btnClick;
                btn.FlatAppearance.MouseOverBackColor = btnOver;
            }
            buttonClick(home.generalBtn, null);
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

        public void changePanel()
        {
            foreach (Control ctrl in home.panel4.Controls) ctrl.Hide();
            if (ctrlPanel == "generalBtn")
            {
                foreach (Label obj in allControls.OfType<Label>()) obj.Show();
                foreach (CheckBox obj in allControls.OfType<CheckBox>()) obj.Show();
                foreach (TextBox obj in allControls.OfType<TextBox>()) obj.Show();
            }
            else if(ctrlPanel == "startBtn") 
            {

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

        private CheckBox initializeBox(string name, string text)
        {
            CheckBox Box = new CheckBox();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.Location = new Point(16, 14 + home.panel4.Controls.Count * 30);
            Box.Name = name;
            Box.Size = new Size(555, 26);
            Box.Text = text;
            Box.UseVisualStyleBackColor = true;
            home.panel4.Controls.Add(Box);
            return Box;
        }
        private Label initializeLabel(string name, string text)
        {
            Label Box = new Label();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.Location = new Point(16, 14 + home.panel4.Controls.Count * 30);
            Box.Name = name;
            Box.Size = new Size(555, 26);
            Box.Text = text;
            home.panel4.Controls.Add(Box);
            return Box;
        }
        private TextBox initializeTextBox(string name, string text, Label label=null)
        {
            TextBox Box = new TextBox();
            Box.AutoSize = true;
            Box.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Box.ForeColor = Color.White;
            Box.BackColor = btnBackColor;
            int x = 16;
            int y = 14 + home.panel4.Controls.Count * 30;
            if (label != null)
            {
                x = label.Location.X + label.Width;
                y = label.Location.Y;
            }
            Box.Location = new Point(x, y);
            Box.Name = name;
            Box.Size = new Size(680-x, 26);
            home.panel4.Controls.Add(Box);
            Box.Text = text;
            return Box;
        }
    }
}
