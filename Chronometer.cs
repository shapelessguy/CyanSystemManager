using System;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public partial class Chronometer : Form
    {
        public bool alert = false;
        Timer focusLostT, alarmT, recoverT, lastPositionT;
        int hours, minutes, seconds;

        private void DisposeAll()
        {
            try
            {
                if (focusLostT != null) focusLostT.Dispose();
                if (alarmT != null) alarmT.Dispose();
                if (recoverT != null) recoverT.Dispose();
                if (lastPositionT != null) lastPositionT.Dispose();
            }
            catch (Exception) { }
            alert = false;
        }
        public Chronometer()
        {
            InitializeComponent();
            Visible = true;
            initializeTimers();
            FormClosing += (o, e) => { DisposeAll(); };
        }

        void initializeTimers()
        {
            focusLostT = new Timer() { Enabled = true, Interval = 100, };
            alarmT = new Timer() { Enabled = true, Interval = 500, };
            recoverT = new Timer() { Enabled = true, Interval = 600, };
            lastPositionT = new Timer() { Enabled = true, Interval = 20, };

            SetForegroundWindow(this.Handle);
            this.TopMost = true;
            focusLostT.Tick += LostFocus_Timer;
            dateTimePicker1.KeyDown += KeyEnter;
        }

        void LostFocus_Timer(object sender, EventArgs e)
        {
            try
            {
                IntPtr win = GetForegroundWindow();
                if (IsDisposed || win != Handle) { Close(); }
            }
            catch (Exception) { }
        }
        void RealTimer(object sender, EventArgs e)
        {
            if (!alert)
            {
                IntPtr win = this.Handle;
                if (seconds == 0) { seconds = 59; minutes--; } else seconds--;
                if (minutes == -1) { minutes = 59; hours--; }
                if (hours == -1) { seconds = 0; minutes = 0; hours = 0; label1.Text = "ALERT"; Alarm(); }
            }
            if (alarmTime.Year > 2000) if((int)(DateTime.Now.Subtract(alarmTime).TotalSeconds+0.01f) > 2) Close();
            if (label1.Text != "ALERT") label1.Text = SetLabelText(hours, minutes, seconds);
        }

        void KeyEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                hours = dateTimePicker1.Value.Hour;
                minutes = dateTimePicker1.Value.Minute;
                seconds = dateTimePicker1.Value.Second;
                if (hours == 0 && minutes == 0 && seconds == 0) 
                    { SendKeys.Send("{RIGHT}"); lastPositionT.Tick += LastPosition; return; }
                focusLostT.Tick -= LostFocus_Timer;
                focusLostT.Interval = 1000;
                focusLostT.Tick += RealTimer;
                dateTimePicker1.Hide();
                label1.Text = SetLabelText(hours, minutes, seconds);
                label1.Show();
            }
        }
        void LastPosition(object sender, EventArgs e)
        {
            hours = dateTimePicker1.Value.Hour;
            minutes = dateTimePicker1.Value.Minute;
            seconds = dateTimePicker1.Value.Second;
            lastPositionT.Tick -= LastPosition;
            focusLostT.Tick -= LostFocus_Timer;
            focusLostT.Interval = 1000;
            focusLostT.Tick += RealTimer;
            dateTimePicker1.Hide();
            label1.Text = SetLabelText(hours, minutes, seconds);
            label1.Show();
        }

        string SetLabelText(int ore, int minuti, int secondi)
        {
            string aus1 = "", aus2 = "";
            if (minuti < 10) aus1 = "0";
            if (secondi < 10) aus2 = "0";
            string output = ore + ":" + aus1 + minuti + ":" + aus2 + secondi;
            return output;
        }

        DateTime alarmTime;
        void Alarm()
        {
            alert = true;
            alarmTime = DateTime.Now;
            AudioArgs args = new AudioArgs(Properties.Resources.Allarme, 3, AT.Primary, 0.4f);
            Service_Audio.tempAudio(args);
        }
    }
}