using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class Charts : Form
    {
        static public List<Charts> chartForms = new List<Charts>();

        static public List<ChartClass> orderedList = new List<ChartClass>()
              { new ChartClass(ChartName.cpuTot, "CPU", "%", 100),
                new ChartClass(ChartName.cpuTempTot, "CPU temp", "°C", 110),
                new ChartClass(ChartName.memoryUsed, "RAM", "MB"),
                new ChartClass(ChartName.gpuTot, "GPU", "%", 100),
                new ChartClass(ChartName.gpuTemp, "GPU temp", "°C", 110),
                new ChartClass(ChartName.gpuVRAM, "VRAM", "MB", 16000),
                new ChartClass(ChartName.gpuClock, "GPU clock", "MHz"),
        };
        static private Timer updateTimer, tantumTimer;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem showBlock;

        private Timer updateSingleTimer;
        static public List<computerInfo> infos = new List<computerInfo>();
        static public List<string> allowList = new List<string>();
        static public List<string> blockList = new List<string>();
        public int id = -1;
        static public bool reallyToSave = false;
        private void CloseAll()
        {
            if (updateTimer != null) { updateTimer.Dispose(); updateTimer = null; }
            if (updateSingleTimer != null) { updateSingleTimer.Dispose(); updateSingleTimer = null; }
            Close();
        }
        public Charts()
        {
            InitializeComponent();
            contextMenuStrip = new ContextMenuStrip
            {
                ImageScalingSize = new Size(24, 24),
                Size = new Size(141, 34)
            };

            showBlock = new ToolStripMenuItem
            {
                Size = new System.Drawing.Size(140, 30),
                Text = "Show allow list"
            };
            showBlock.Click += new System.EventHandler(showBlock_Click);
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { showBlock });
            ContextMenuStrip = contextMenuStrip;

            allowList = deserialize(Properties.Settings.Default.allowChList);
            blockList = deserialize(Properties.Settings.Default.blockChList);
            if (updateTimer == null)
            {
                updateTimer = new Timer() { Enabled = true, Interval = 1000 };
                updateTimer.Tick += update;
                tantumTimer = new Timer() { Enabled = true, Interval = 5000 };
                tantumTimer.Tick += tantumUpdate;
            }
            if (updateSingleTimer == null)
            {
                updateSingleTimer = new Timer() { Enabled = true, Interval = 200 };
                updateSingleTimer.Tick += updateSingle;
            }
            FormClosing += (o, e) => { e.Cancel = true; Hide(); };
            id = chartForms.Count;
            Text = "HW Monitoring Charts("+(id+1)+")";
            Name = "ChartWin" + id;
            chartForms.Add(this); 
            while (infos.Count < Service_HWMonitoring.sampling) infos.Add(Service_HWMonitoring.PC_INFO);
            foreach (var item in orderedList) newChart(item.name);
            Show();
        }
        public void HideForm() { if (Visible) { Hide(); ShowInTaskbar = false; } }
        public void ShowForm() { if (!Visible) { Show(); } }
        private void update(object sender, EventArgs e)
        {
            if (Program.timeToClose) { CloseAll(); return; }
            if (Service_HWMonitoring.status == Utility.State.OFF) { HideForm(); return; }
            try { infos.Add(Service_HWMonitoring.PC_INFO.copy()); } catch (Exception) { }
            if (infos.Count > Service_HWMonitoring.sampling) infos.RemoveAt(0);
            

            if (reallyToSave)
            {
                reallyToSave = false;
                Properties.Settings.Default.allowChList = serialize(allowList);
                Properties.Settings.Default.blockChList = serialize(blockList);
                Properties.Settings.Default.Save();
            }
            foreach(Charts form in chartForms) form.Refresh();
        }
        private void tantumUpdate(object sender, EventArgs e)
        {
            List<string> fpsWin = new List<string>();
            foreach (var item in infos[infos.Count - 1].fps)
            {
                fpsWin.Add(item.Key);
                if (!allowList.Contains(item.Key) && !blockList.Contains(item.Key))
                { if (item.Value > 0) allowList.Add(item.Key);}
            }
            List<string> orderedStrings = new List<string>();
            foreach (var item in orderedList) orderedStrings.Add(item.name);
            if (infos.Count > 0)
                for (int i = allowList.Count - 1; i >= 0; i--)
                    if (!fpsWin.Contains(allowList[i]) && !orderedStrings.Contains(allowList[i])) allowList.RemoveAt(i);


            for(int i=fpsWin.Count-1; i>=0; i--)
            {
                int totVal = 0;
                for(int j= infos.Count - 1; j>= infos.Count - 10; j--)
                { if(infos[j].fps.ContainsKey(fpsWin[i])) totVal += infos[j].fps[fpsWin[i]];}
                if (totVal != 0) fpsWin.RemoveAt(i);
            }
            foreach (var str in fpsWin) { if (Charts.allowList.Contains(str)) Charts.allowList.Remove(str); }

        }
        private void newChart(string name)
        { if (!allowList.Contains(name) && !blockList.Contains(name)) { allowList.Add(name);} }

        private void updateSingle(object sender, EventArgs e)
        {
            List<Chart> charts = Controls.OfType<Chart>().ToList();
            List<string> actualcharts = new List<string>();
            for (int i = charts.Count - 1; i >= 0; i--)
            {
                actualcharts.Add(charts[i].Name);
                if (!allowList.Contains(charts[i].Name))
                    try { charts[i].CloseAll(); charts.RemoveAt(i);} catch (Exception) { }
            }
            foreach (string allowChart in allowList)
                if (!actualcharts.Contains(allowChart))
                { Chart new_chart = new Chart(allowChart, Name, Service_HWMonitoring.dims[id]); 
                    charts.Add(new_chart); Controls.Add(new_chart); }
            locate();
        }

        public void locate()
        {
            List<Chart> charts = Controls.OfType<Chart>().ToList();
            charts = orderCharts(charts);
            if (charts.Count == 0) return;
            int margin = 10;
            int maxCol = Width / (charts[0].Width + margin);
            int maxRow = (int)((double)charts.Count / maxCol + 0.99999);

            for (int row = 0; row<maxRow; row++)
            {
                for (int i = 0; i < maxCol; i++)
                {
                    int index = row * maxCol + i;
                    if (index >= charts.Count) return;
                    charts[index].Location = new Point(i * (charts[0].Width + margin), row * (charts[0].Height + margin));
                }
            }
        }

        private List<Chart> orderCharts(List<Chart> list)
        {
            List<Chart> newList = new List<Chart>();
            List<string> orderedStrings = new List<string>();
            foreach (var item in orderedList) orderedStrings.Add(item.name);
            foreach (string value in orderedStrings)
                    { Chart chart = select(value, list); if (chart != null) newList.Add(chart); }
            foreach (var item in list) if (!orderedStrings.Contains(item.Name)) newList.Add(item);
            return newList;
        }
        private Chart select(string name, List<Chart> list)
        {
            foreach (Chart chart in list) if (chart.Name == name) return chart;
            return null;
        }
        private string serialize(List<string> list)
        {
            string output = "";
            foreach (var elem in list) output += elem + ";";
            return output;
        }
        private List<string> deserialize(string listStr)
        {
            List<string> newList = listStr.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return newList;
        }

        static public ShowBlockListForm newForm;
        private void showBlock_Click(object sender, EventArgs e)
        {
            if (newForm != null && ShowBlockListForm.active) return;
            newForm = new ShowBlockListForm();
            newForm.Show();
        }

    }
    public class ChartName
    {
        static public string cpuTot = "cpuTot";
        static public string cpuTempTot = "cpuTempTot";
        static public string gpuClock = "gpuClock";
        static public string gpuTemp = "gpuTemp";
        static public string gpuTot = "gpuTot";
        static public string gpuVRAM = "gpuVRAM";
        static public string memoryUsed = "memoryUsed";
        static public string FPS = "fps";
    }
    public class ChartClass
    {
        public string name;
        public string title;
        public string unit;
        public int max;
        public ChartClass(string name, string title, string unit, int max=-1)
        {
            this.name = name;
            this.title = title;
            this.unit = unit;
            this.max = max;
        }
    }
}
