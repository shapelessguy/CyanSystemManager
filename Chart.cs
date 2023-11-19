using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class Chart : UserControl
    {
        public static int xCursor = -5;
        public static string window = "";
        public bool allowGrip = false;
        public string parent = "";
        public Timer timerUpdate;
        private Size pSize;
        public Chart(string name, string parent, int dim)
        {
            InitializeComponent();
            Size = new Size(dim, (dim*3)/4+20);
            Name = name;
            this.parent = parent;
            button1.Name = name;
            title.Text = "";
            foreach (var chart in Charts.orderedList) if (Name == chart.name) title.Text = chart.title;
            if(title.Text=="") title.Text = name;
            pictureBox.MouseLeave += (o, e) => { xCursor = -5; };
            pictureBox.MouseMove += (o, e) => { xCursor = PointToClient(Cursor.Position).X; window = parent; };
            timerUpdate = new Timer() { Enabled = true, Interval = 30 };
            timerUpdate.Tick += update;
            pictureBox.Paint += PicturePaint;
            pSize = pictureBox.Size;
        }

        private int lastxCursor = -5;
        private void update(object sender, EventArgs e)
        {
            if (lastxCursor == xCursor) return;
            lastxCursor = xCursor;
            if (parent == window) pictureBox.Invalidate();
        }
        public void CloseAll()
        {
            Dispose();
        }

        protected void PicturePaint(object sender, PaintEventArgs e)
        {
            DrawSystem(e.Graphics);
            if (parent == window) DrawCursor(e.Graphics);
            DrawPoints(e.Graphics);
            //if (BackColor == Color.Red) BackColor = Color.Blue;
            //else BackColor = Color.Red;
            //BackColor = Color.Red;
        }
        private void DrawSystem(Graphics g)
        {
            g.DrawRectangle(new Pen(Color.White, 1), 0, 0, pictureBox.Width-1, pictureBox.Height-1);
        }

        public static int actPosition = -1;
        private void DrawPoints(Graphics g)
        {
            // Take the data
            List<int> values = new List<int>();
            int max = getData(values);
            if (values.Count == 0) return;
            while (values.Count < Service_HWMonitoring.sampling) values.Insert(0, -1);
            double factor = (double)(pSize.Height - 2) / max;
            if (factor < 0) factor = 0;


            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pen = new Pen(Color.White, 2) { DashCap = System.Drawing.Drawing2D.DashCap.Round };
            Pen redPen = new Pen(Color.Red, 4) { DashCap = System.Drawing.Drawing2D.DashCap.Round };

            int Rvalue = -1;
            int RactPosition = -1;
            for (int i= values.Count-2; i>=0; i--)
            {
                actPosition = (int)(pSize.Width * (i) / (double)values.Count);
                int prevPosition = (int)(pSize.Width * (i+1) / (double)values.Count);
                g.DrawLine(pen, (int)(pSize.Width * (i+1)/(double)values.Count), 
                    pSize.Height - (int)(values[i+1] * factor) -1, actPosition, pSize.Height - (int)(values[i] * factor)-1);
                
                if(actPosition<xCursor+2 && actPosition > xCursor - 2)
                { Rvalue = values[i]; RactPosition = actPosition; }
            }
            if (Rvalue != -1)
            {
                int width = 3;
                g.DrawEllipse(redPen, RactPosition - width, pSize.Height - (int)(Rvalue * factor) - 1 - 2, 2* width, 2 * width);
                //int strX = RactPosition + 5; if (strX > pSize.Width - 30) strX = pSize.Width - 30;
                //int strY = pSize.Height - (int)(Rprev_value * factor) - 10;
                //if (strY > pSize.Height - 25) strY = pSize.Height - 25;
                int strX = 1;
                int strY = 21;
                
                g.FillRectangle(Brushes.Black, strX, strY, 30, 20);
                g.DrawString(Rvalue.ToString(), new Font("Frankfurter", 12, FontStyle.Bold), Brushes.Red, strX, strY);
            }

            string unit = "fps";
            foreach (var chart in Charts.orderedList) if (Name == chart.name) unit = chart.unit;
            if (values.Count == 0) return;
            if (values[values.Count - 1] == -1) return;
            string lvalue = values[values.Count -1] + unit;
            g.FillRectangle(Brushes.Black, 2, 2, 60, 20);
            g.DrawString(lvalue, new Font("Frankfurter", 12, FontStyle.Bold), Brushes.White, 0, 0);
        }

        public static int[] maxima = new int[] { 1, 5, 10, 25, 50, 75, 100, 125, 150, 200, 250, 300, 400, 500 };
        private int getData(List<int> values)
        {
            int max = 0;
            for (int i = 0; i < Charts.infos.Count; i++)
            {
                if (Name == "cpuTot") values.Add(Charts.infos[i].cpuTot);
                else if (Name == "cpuTempTot") values.Add(Charts.infos[i].cpuTempTot);
                else if (Name == "gpuClock") values.Add(Charts.infos[i].gpuClock);
                else if (Name == "gpuTemp") values.Add(Charts.infos[i].gpuTemp);
                else if (Name == "gpuTot") values.Add(Charts.infos[i].gpuTot);
                else if (Name == "gpuVRAM") values.Add(Charts.infos[i].gpuVRAM);
                else if (Name == "memoryUsed") values.Add(Charts.infos[i].memoryUsed);

                //else if (Name == "totalGpuMemory") values.Add(Charts.infos[i].totalGpuMemory);
                //else if (Name == "totalMemoryAvailable") values.Add(Charts.infos[i].totalMemoryAvailable);
                else if (Charts.infos[i].fps.Keys.Contains(Name)) values.Add(Charts.infos[i].fps[Name]);
                else continue;
                if (values[values.Count - 1] > max) max = values[values.Count - 1];
            }
            int totMem = Charts.infos[Charts.infos.Count - 1].totalMemoryAvailable;
            int totgMem = Charts.infos[Charts.infos.Count - 1].totalGpuMemory;
            foreach (var chart in Charts.orderedList)
            {
                if (chart.name == "memoryUsed" && totMem>-1) chart.max = totMem;
                if (chart.name == "gpuVRAM" && totgMem > -1) chart.max = totgMem;
            }
            foreach (var chart in Charts.orderedList) if (Name == chart.name && chart.max > -1) max = chart.max;
            if(max == 0) foreach (var num in maxima) if (max > num) continue; else { max = num; break; }
            //if (Name == "cpuTot") Console.WriteLine(max);
            return max;
        }
        private void DrawCursor(Graphics g)
        {
            Pen pen = new Pen(Color.White, 1);
            pen.DashPattern = new float[] { 4.0F, 1.0F, 4.0F, 1.0F };
            g.DrawLine(pen, xCursor, 0, xCursor, pSize.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            Charts.reallyToSave = true;
            if(Charts.allowList.Contains(name)) Charts.allowList.Remove(name);
            if(!Charts.blockList.Contains(name)) Charts.blockList.Add(name);
        }
    }

}
