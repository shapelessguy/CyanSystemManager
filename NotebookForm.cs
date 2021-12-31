using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using static CyanSystemManager.Utility;
using static CyanSystemManager.Settings;

namespace CyanSystemManager
{
    public partial class NotebookForm : Form
    {
        public static bool canSave = true;
        public static bool canResize = false;
        Screen actualScreen;
        public static bool palClicked = false;
        public static Color act_color = Color.Black;
        public static bool styClicked = false;
        public static FontStyle act_style;
        private Font selectionFont;
        string fileName = "";
        List<Color> palette = new List<Color>()
        {
            Color.Navy,
            Color.Blue,
            Color.Gray,
            Color.Green,
            Color.Aqua,
            Color.Maroon,
            Color.Purple,
            Color.Red,
            Color.Fuchsia,
            Color.Lime,
            Color.Yellow,
            Color.Olive,
            Color.Silver,
            Color.Teal,
            Color.White,
        };

        public NotebookForm()
        {
            InitializeComponent();
            InitializeControlsPanel();
            Timer checkClick = new Timer() { Interval = 10, Enabled = true, };
            int iterations = 0;
            checkClick.Tick += (o, e) => {
                iterations++;
                if (iterations >= 300) { //Save(); 
                    iterations = 0; }
                if (palClicked) { palClicked = false; PaletteClick(); }
                if (styClicked) { styClicked = false; ChangeStyle(act_style); }
            };
            Visible = true;
            Visible = false;
            canResize = true;
        }

        private void InitializeControlsPanel()
        {
            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                cmbFont.Items.Add(font.Name);
            }
            for (int i = 6; i < 120; i++) cmbSize.Items.Add(Convert.ToString(i));
            richTextBox1.HideSelection = false;
        }

        private void GetCurrentFile()
        {
            string infoFile = Path.Combine(variablePath.notebookPath, "_info_.txt");
            if (File.Exists(infoFile))
            {
                foreach (string line in File.ReadAllLines(infoFile))
                {
                    if (line.Split(':')[0] == "currentFile") { fileName = line.Split(':')[1]; }
                    //if (line.Split(':')[0] == "backColor") { richTextBox1.BackColor = Color.FromName(line.Split(':')[1]); }
                }
                if (!File.Exists(Path.Combine(variablePath.notebookPath, fileName))) fileName = "";
            }
        }
        private void LoadTabs()
        {
            cmbTabs.Items.Clear();
            foreach (string file in Directory.GetFiles(variablePath.notebookPath))
            {
                if (Path.GetFileName(file) == "_info_.txt" || Path.GetFileName(file) == "_temp_.txt") continue;
                if (Path.GetExtension(file) == ".txt")
                {
                    cmbTabs.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        private void Appunti_Load(object sender, EventArgs e)
        {
            int size = panel2.Controls[0].Width;
            foreach (Color color in palette)
            {
                panel2.Controls.Add(new NotebookColPal()
                {
                    BackColor = color,
                    BorderStyle = BorderStyle.FixedSingle,
                    Size = new Size(size, size),
                    Location = new Point(panel2.Controls[panel2.Controls.Count - 1].Location.X + size + 4, 4),
                });
            }
            LoadTabs();
            GetCurrentFile();
            LoadFile();
            richTextBox1.AutoWordSelection = false;
            ResizePalette();
        }

        private void ResizePalette()
        {
            int initial_space = panel2.Controls[0].Location.X;
            int space = panel2.Controls[1].Location.X - panel2.Controls[0].Location.X - panel2.Controls[0].Width;
            int total_space = space + panel2.Controls[0].Width;
            int space_available = wall_btn.Location.X - panel2.Location.X;
            int max_paletteNum = (int)((space_available - initial_space) / total_space);
            int paletteNum = panel2.Controls.Count;
            if (paletteNum > max_paletteNum) paletteNum = max_paletteNum;
            panel2.Size = new Size(initial_space + total_space * paletteNum, panel2.Height);
        }

        private void Appunti_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                if (!canResize) Bounds = new Rectangle(0, 0, 1, 1);
                else
                {
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (screen.Bounds.Contains(MousePosition)) actualScreen = screen;
                    }
                    Bounds = actualScreen.Bounds;
                    SetForegroundWindow(this.Handle);
                    ResizePalette();
                    Refresh();
                }
            }
            else Save();
        }

        public void PaletteClick()
        {
            richTextBox1.SelectionColor = act_color;
            Save();
        }

        bool allowSave = true;
        private void Save()
        {
            if (!canSave) return;
            if ((!firstLoad && fileName != "") || !allowSave) return;
            string path = Path.Combine(variablePath.notebookPath, "_temp_.txt");
            if (!richTextBox1.Enabled) return;
            try
            {
                richTextBox1.SaveFile(path, RichTextBoxStreamType.RichText);
                RichTextBox tempRich = new RichTextBox();
                tempRich.LoadFile(path, RichTextBoxStreamType.RichText);
            }
            catch (Exception) { return; }

            path = Path.Combine(variablePath.notebookPath, fileName);
            richTextBox1.SaveFile(path, RichTextBoxStreamType.RichText);
            string backColor = "\nbackColor:" + richTextBox1.BackColor.ToArgb();
            File.AppendAllText(path, backColor);
            using (StreamWriter sw = new StreamWriter(Path.Combine(variablePath.notebookPath, "_info_.txt")))
            {
                sw.WriteLine("currentFile:" + fileName);
            }

        }
        bool firstLoad = false;
        private void LoadFile()
        {
            //Console.WriteLine(string.Join(",", new string[]{ "a", "b" }) );
            try
            {
                if (fileName != "") richTextBox1.Enabled = true;
                else { richTextBox1.Enabled = false; return; }
                string path = Path.Combine(variablePath.notebookPath, fileName);
                if (File.Exists(path))
                {
                    cmbTabs.Text = Path.GetFileNameWithoutExtension(path);
                    richTextBox1.LoadFile(path, RichTextBoxStreamType.RichText);
                    firstLoad = true;
                    foreach (string line in File.ReadAllLines(path))
                    {
                        if (line.Split(':')[0] == "backColor") { richTextBox1.BackColor = Color.FromArgb(Convert.ToInt32(line.Split(':')[1])); }
                    }
                }
                allowSave = true;
            }
            catch (Exception e) { canSave = false; MessageBox.Show("EXCEPTION IN LOADING THE FILE"); Console.WriteLine(e.Message); }
        }

        private void ChangeStyle(FontStyle style)
        {
            if (richTextBox1.SelectionFont == null)
            {
                if (richTextBox1.SelectionLength != 0)
                {
                    int sel_start = richTextBox1.SelectionStart;
                    int sel_fin = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    for (int i = sel_start; i < sel_fin; i++)
                    {
                        richTextBox1.Select(i, 1);
                        richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, style);
                    }
                    richTextBox1.Select(sel_start, sel_fin - sel_start);
                }
            }
            else
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, style);
            }
            Save();
        }
        private void ChangeSize(float size)
        {
            if (!CheckUniformFont())
            {
                if (richTextBox1.SelectionLength != 0)
                {
                    int sel_start = richTextBox1.SelectionStart;
                    int sel_fin = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    for (int i = sel_start; i < sel_fin; i++)
                    {
                        richTextBox1.Select(i, 1);
                        richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.Name, size);
                    }
                    richTextBox1.Select(sel_start, sel_fin - sel_start);
                }
            }
            else
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, size);
            }
            Save();
        }

        private void ChangeFontName(string name)
        {
            if (!CheckUniformFont())
            {
                if (richTextBox1.SelectionLength != 0)
                {
                    int sel_start = richTextBox1.SelectionStart;
                    int sel_fin = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    for (int i = sel_start; i < sel_fin; i++)
                    {
                        richTextBox1.Select(i, 1);
                        richTextBox1.SelectionFont = new Font(name, richTextBox1.SelectionFont.Size);
                    }
                    richTextBox1.Select(sel_start, sel_fin - sel_start);
                }
            }
            else
            {
                richTextBox1.SelectionFont = new Font(name, richTextBox1.SelectionFont.Size);
            }
            Save();
        }

        private bool CheckUniformFont()
        {
            if (richTextBox1.SelectionLength == 0) return true;
            else
            {
                block = true;
                List<string> names = new List<string>();
                List<float> sizes = new List<float>();
                int sel_start = richTextBox1.SelectionStart;
                int sel_fin = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                for (int i = sel_fin - 1; i >= sel_start; i--)
                {
                    richTextBox1.Select(i, 1);
                    if (!names.Contains(richTextBox1.SelectionFont.Name)) names.Add(richTextBox1.SelectionFont.Name);
                    if (!sizes.Contains(richTextBox1.SelectionFont.Size)) sizes.Add(richTextBox1.SelectionFont.Size);
                }
                richTextBox1.Select(sel_start, sel_fin - sel_start);
                block = false;
                if (names.Count == 1 && sizes.Count == 1) return true;
                else return false;
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            ChangeStyle(FontStyle.Bold);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ChangeStyle(FontStyle.Underline);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ChangeStyle(FontStyle.Italic);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            bool isLegit = (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || 
                (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) || e.KeyCode == Keys.Enter;
            if (!isLegit) { e.SuppressKeyPress = true; return; }
            if (e.KeyCode == Keys.Enter)
            {
                richTextBox1.Focus();
                // ChangeSize(Convert.ToInt16(textBox1.Text));
            }
        }

        private bool manualSelection = false;
        private void cmbFont_TextChanged(object sender, EventArgs e)
        {
            if (manualSelection) return;
            if (cmbFont.Items.Contains(cmbFont.Text)) ChangeFontName(cmbFont.Text);
        }
        private void cmbSize_TextChanged(object sender, EventArgs e)
        {
            if (manualSelection) return;
            try { Convert.ToInt16(cmbSize.Text); } catch (Exception) { return; }
            if (cmbSize.Items.Contains(cmbSize.Text)) ChangeSize((float)Convert.ToInt16(cmbSize.Text));
        }

        bool block = false;
        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            if (block) return;
            selectionFont = richTextBox1.SelectionFont;

            if (richTextBox1.SelectionFont == null)
            {
                if (richTextBox1.SelectionLength == 0) return;
                else
                {
                    manualSelection = true;
                    cmbFont.Text = "";
                    cmbSize.Text = "";
                    manualSelection = false;
                    return;
                }
            }
            manualSelection = true;
            cmbFont.Text = selectionFont.Name;
            cmbSize.Text = Convert.ToString(selectionFont.Size);
            manualSelection = false;
        }

        private void cmbTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Save();
            fileName = cmbTabs.Text + ".txt";
            LoadFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            for (; ;)
            {
                string name = Interaction.InputBox("Type name of the new tab.", "New Tab", "Notebook");

                if(fileName == "") firstLoad = true;
                name = Path.GetFileNameWithoutExtension(name);
                if (name == "") return;
                string pathName = Path.Combine(variablePath.notebookPath, name) + ".txt";
                if (File.Exists(pathName)) MessageBox.Show("This tab name already exists. Choose another one.");
                else
                {
                    richTextBox1.Text = "";
                    richTextBox1.BackColor = Color.White;
                    fileName = "";
                    fileName = Path.GetFileName(pathName);
                    LoadFile();
                    Save();
                    LoadTabs();
                    cmbTabs.Text = name;
                    return;
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this tab? Your changes will not be saved.", 
                "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    allowSave = false;
                    File.Delete(Path.Combine(variablePath.notebookPath, fileName));
                    richTextBox1.Text = "";
                    richTextBox1.BackColor = Color.White;
                    fileName = "";
                    LoadTabs();
                    if (cmbTabs.Items.Count == 0) richTextBox1.Enabled = false;
                    else
                    {
                        fileName = cmbTabs.Items[0] + ".txt";
                        LoadFile();
                    }
                }
                catch (Exception) { MessageBox.Show("Error in deleting the file."); allowSave = true; }
            }
            else return; ;
        }

        private void wall_btn_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowHelp = false;
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                richTextBox1.BackColor = colorDialog1.Color;
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}