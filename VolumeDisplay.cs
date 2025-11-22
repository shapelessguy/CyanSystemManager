using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static CyanSystemManager.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Vanara.PInvoke.Gdi32;

namespace CyanSystemManager
{
    public class VolSettings
    {
        public float volume;
        public bool mute;
        public string deviceName;
        public VolSettings(float volume, bool mute, string deviceName)
        {
            this.volume = volume;
            this.mute = mute;
            this.deviceName = deviceName;
        }
        public bool equalsTo(VolSettings set2)
        {
            if (set2 == null) return false;
            bool equalVol = false;
            if ((int)(volume * 100) == (int)(set2.volume * 100)) equalVol = true;
            return equalVol && mute == set2.mute && deviceName == set2.deviceName;
        }
        public void print() { Program.Log(deviceName + "  mute:" + mute + "   vol:" + volume); }
    }
    public class MsgSettings
    {
        public string message;
        public MsgSettings(string message)
        {
            this.message = message;
        }
        public void print() { Program.Log("Message: " + message); }
    }
    public class IndicatorSettings
    {
        public string type;
        public IndicatorSettings(string type)
        {
            this.type = type;
        }
        public void print() { Program.Log("Type: " + type); }
    }

    public partial class VolumeDisplay : Form
    {
        public bool dispose = false;
        public Screen screen;
        int cycle_index = 0;
        private List<Object> settingsQueue = new List<Object>();
        private List<string> indicators = new List<string>();

        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        public VolumeDisplay(Screen screen)
        {
            InitializeComponent();
            DoubleBuffered = true; // Enable double buffering to reduce flickering.
            this.screen = screen;
            Show();
            Location = screen.Bounds.Location;
            new Thread(handleVisibility).Start();
        }

        public void submit(Object set)
        {
            if (dispose) return;
            settingsQueue.Add(set); // Store the current settings for painting.
            cycle_index = 0;
        }

        private void handleVisibility()
        {
            int threshold = 20 * 2; // 2 secs of display
            while (!dispose)
            {
                cycle_index ++;
                if (cycle_index < threshold)
                {
                    if (settingsQueue.Count() > 0) { Invalidate(); cycle_index = 0; }
                }
                else if (cycle_index == threshold)
                {
                    Invalidate();
                }
                if (cycle_index > 10 * threshold)
                {
                    cycle_index = threshold;
                }
                Thread.Sleep(50);
            }
        }

        private void DrawIndicator(PaintEventArgs e)
        {
            foreach (string indicator_type in indicators)
            {
                if (indicator_type == "START_WAITING" || 
                    indicator_type == "START_THINKING" || 
                    indicator_type == "START_SPEAKING" || 
                    indicator_type == "START_ERROR")
                {
                    Color color = Color.White;
                    if (indicator_type == "START_WAITING") color = Color.Green;
                    else if (indicator_type == "START_THINKING") color = Color.Orange;
                    else if (indicator_type == "START_SPEAKING") color = Color.Blue;
                    else if (indicator_type == "START_ERROR") color = Color.Red;
                    Brush redBrush = new SolidBrush(color);
                    float circleDiameter = 30.0f;
                    PointF circlePosition = new PointF(10, 10);
                    e.Graphics.FillEllipse(redBrush, circlePosition.X, circlePosition.Y, circleDiameter, circleDiameter);
                    redBrush.Dispose();
                }
                else if (indicator_type == "START_OUTLOOK")
                {
                    Image image = Properties.Resources.warning;
                    float imageWidth = 40f;
                    float imageHeight = 40f;
                    PointF imagePosition = new PointF(240, 50);
                    e.Graphics.DrawImage(image, imagePosition.X, imagePosition.Y, imageWidth, imageHeight);

                    Font textFont = new Font("Arial", 16, FontStyle.Bold);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    PointF textPosition = new PointF(290, 70);
                    PointF shadowPosition = new PointF(textPosition.X + 2, textPosition.Y + 2);
                    string text = "Password for Outlook needed";
                    e.Graphics.DrawString(text, textFont, Brushes.Black, shadowPosition, stringFormat);
                    e.Graphics.DrawString(text, textFont, Brushes.White, textPosition, stringFormat);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Location = screen.Bounds.Location;
            this.TopMost = true;
            this.TopLevel = true;
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            base.OnPaint(e);

            // Set high-quality rendering for smooth graphics.
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            if (settingsQueue.Count() > 0)
            {
                Object set_obj = settingsQueue[0];
                if (set_obj is VolSettings)
                {
                    VolSettings set = (VolSettings)set_obj;

                    // Define the overall dimensions for the volume display.
                    int diameter = 140; // Diameter of the circle.
                    int centerX = diameter / 2;
                    int centerY = diameter / 4;
                    Rectangle circleBounds = new Rectangle(centerX, centerY, diameter, diameter);

                    // Draw the background circle.
                    // e.Graphics.FillEllipse(Brushes.Black, circleBounds);

                    // Assuming the "carving" is a static visual element, like a shadow or a border.
                    e.Graphics.DrawEllipse(new Pen(Brushes.White, 34), circleBounds);

                    // Draw the volume indicator as an arc around the circle.
                    // The sweep angle is proportional to the current volume level (0 to 360 degrees).
                    float sweepAngle = (float)(360 * set.volume);
                    // You can adjust the pen thickness as needed.
                    Brush color = set.mute ? Brushes.DarkGray : Brushes.Red;
                    Pen volumePen = new Pen(color, 30);
                    // The arc is drawn just inside the circle's bounds.
                    e.Graphics.DrawArc(volumePen, circleBounds, -90, sweepAngle); // Start from the top (-90 degrees).

                    string volumeText = set.deviceName;
                    string volumeInt = set.mute ? "MUTE" : ((int)Math.Round(set.volume * 100)).ToString();

                    // Define the font and text formatting
                    Font textFont = new Font("Arial", 18, FontStyle.Bold);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near; // Align text to the near, so it starts from the specified point
                    stringFormat.LineAlignment = StringAlignment.Center; // Vertically align text to the center of the point

                    // Calculate the text position (to the right of the circle)
                    PointF textPosition = new PointF(centerX + diameter + 30, centerY + diameter / 2); // Adjust as needed
                    PointF shadowPosition = new PointF(textPosition.X + 2, textPosition.Y + 2); // Shadow position, slightly offset

                    // Draw the shadow text (black)
                    e.Graphics.DrawString(volumeText, textFont, Brushes.Black, shadowPosition, stringFormat);

                    // Draw the main text (white)
                    e.Graphics.DrawString(volumeText, textFont, Brushes.White, textPosition, stringFormat);

                    // Calculate the text position (to the right of the circle)
                    int offset = set.mute ? -40 : -20;
                    PointF VolPosition = new PointF(centerX + (int)(diameter / 2) + offset, centerY + diameter / 2); // Adjust as needed
                    PointF VolShadowPosition = new PointF(VolPosition.X + 2, VolPosition.Y + 2); // Shadow position, slightly offset

                    // Draw the shadow text (black)
                    e.Graphics.DrawString(volumeInt, textFont, Brushes.Black, VolPosition, stringFormat);

                    // Draw the main text (white)
                    e.Graphics.DrawString(volumeInt, textFont, Brushes.White, VolShadowPosition, stringFormat);
                }
                else if (set_obj is MsgSettings)
                {
                    MsgSettings set = (MsgSettings)set_obj;

                    Font textFont = new Font("Arial", 36, FontStyle.Bold);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Near;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Calculate the text position (to the right of the circle)
                    PointF VolPosition = new PointF(50, 30); // Adjust as needed
                    PointF VolShadowPosition = new PointF(VolPosition.X + 2, VolPosition.Y + 2); // Shadow position, slightly offset

                    // Draw the shadow text (black)
                    e.Graphics.DrawString(set.message, textFont, Brushes.Black, VolPosition, stringFormat);

                    // Draw the main text (white)
                    e.Graphics.DrawString(set.message, textFont, Brushes.White, VolShadowPosition, stringFormat);
                }
                else if (set_obj is IndicatorSettings)
                {
                    IndicatorSettings set = (IndicatorSettings)set_obj;

                    if (set.type == "START_WAITING") AddIfNotPresent(indicators, set.type);
                    else if (set.type == "END_WAITING") RemoveIfPresent(indicators, "START_WAITING");

                    else if (set.type == "START_THINKING") AddIfNotPresent(indicators, set.type);
                    else if (set.type == "END_THINKING") RemoveIfPresent(indicators, "START_THINKING");

                    else if (set.type == "START_SPEAKING") AddIfNotPresent(indicators, set.type);
                    else if (set.type == "END_SPEAKING") { RemoveIfPresent(indicators, "START_SPEAKING"); }

                    else if (set.type == "START_ERROR") AddIfNotPresent(indicators, set.type);
                    else if (set.type == "END_ERROR") { RemoveIfPresent(indicators, "START_ERROR"); }

                    else if (set.type == "START_OUTLOOK") AddIfNotPresent(indicators, set.type);
                    else if (set.type == "END_OUTLOOK") RemoveIfPresent(indicators, "START_OUTLOOK");

                }
                settingsQueue.RemoveAt(0);
            }
            DrawIndicator(e);
        }
        public static void AddIfNotPresent(List<string> list, string item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
        public static void RemoveAllStarts(List<string> list)
        {
            for (int i = list.Count- 1; i >= 0; i--)
            {
                if (list[i].Substring(0, 5) == "START") { list.RemoveAt(i); }
            }
        }
        public static void RemoveIfPresent(List<string> list, string item)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
                return cp;
            }
        }

        public void DisposeAll()
        {
            dispose = true;
            Dispose();
            Program.Log("Disposing");
        }
    }
}
