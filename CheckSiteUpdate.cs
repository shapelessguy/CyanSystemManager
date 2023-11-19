using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;

namespace CyanSystemManager
{
    public partial class CheckSiteUpdate : Form
    {
        Thread alarmThread;
        bool alarmBool = false;
        string site = "";
        bool verbose = true;
        public static string localFileSite = "localFile.txt";

        public CheckSiteUpdate()
        {
            InitializeComponent();

            System.Windows.Forms.Timer checking = new System.Windows.Forms.Timer() { Interval = 10000, Enabled = true };
            checking.Tick += Check;

            alarmThread = new Thread(Alarm);
            alarmThread.Start();

            WindowState = FormWindowState.Minimized;
        }

        private void Relocate()
        {
            Point centralPoint = new Point(Screen.PrimaryScreen.Bounds.X + Screen.PrimaryScreen.Bounds.Width / 2,
                  Screen.PrimaryScreen.Bounds.Y + Screen.PrimaryScreen.Bounds.Height / 2);
            Location = new Point(centralPoint.X - Width / 2, centralPoint.Y - Height / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            alarmBool = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(site);
        }

        private void Check(object o, EventArgs e)
        {
            if (CheckOp())
            {
                if (verbose) Console.WriteLine("CheckSite -> Alarm activated");
                alarmBool = true;
            }
        }

        private bool CheckOp()
        {
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                //client.DownloadFile(site, localFile);
                string htmlCode = "";
                if (site == "") return false;
                try
                {
                    htmlCode = client.DownloadString(site);
                }
                catch (Exception) { Console.WriteLine("Check site failed.");  return false; }

                if (File.Exists(localFileSite))
                {
                    if(compareStrings(htmlCode, File.ReadAllText(localFileSite)))
                    {
                        if (verbose) Console.WriteLine("CheckSite -> Same file");
                        Program.home.Invoke((MethodInvoker)delegate { ShowInTaskbar = false; });
                        return false;
                    }
                }
                else
                {
                    if (verbose) Console.WriteLine("CheckSite -> New file");
                    using (StreamWriter sw = new StreamWriter(localFileSite))
                    {
                        sw.Write(htmlCode);
                    }
                    return false;
                }

                
                if (WindowState != FormWindowState.Normal)
                    Program.home.Invoke((MethodInvoker)delegate { 
                        WindowState = FormWindowState.Normal;
                        Relocate();
                        if (!ShowInTaskbar) ShowInTaskbar = true;
                    });

                if (verbose) Console.WriteLine("CheckSite -> Updated file");
                using (StreamWriter sw = new StreamWriter(localFileSite))
                {
                    sw.Write(htmlCode);
                }
                return true;
            }
        }

        private bool compareStrings(string htmlCode, string local)
        {
            string key = "views-field views-field-entity-id";
            string[] array = htmlCode.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] array2 = local.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string result = "-", result2 = "-";
            foreach (string value in array)
            {
                if (value.Length >= key.Length && value.Contains(key)) result = value;
            }
            foreach (string value in array2)
            {
                if (value.Length >= key.Length && value.Contains(key)) result2 = value;
            }

            if (result != result2) return false;
            if (result == "-") Console.WriteLine("Warning - Can't find the key block into the HTML file!");
            return true;
            
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        bool prev_alarm = false;
        private void Alarm()
        {
            Thread.Sleep(5000);
            while (true)
            {
                if (alarmBool)
                {
                    if(!prev_alarm) Program.home.Invoke((MethodInvoker)delegate { SetForegroundWindow(this.Handle); }); 
                    Stream str = Properties.Resources.alarm;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                for (int i=0; i<20; i++)
                {
                    if (this.IsDisposed) return;
                    Thread.Sleep(200);
                    site = Properties.Settings.Default.checkSite;
                }
                prev_alarm = alarmBool;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            alarmBool = false;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }


        private void killMainProcess()
        {
            void kill()
            {
                Thread.Sleep(500);
                Close();
            }
            new Thread(kill).Start();
        }

        private static int WM_QUERYENDSESSION = 0x11;
        private static int WM_ENDSESSION = 0x16;
        public const uint SHUTDOWN_NORETRY = 0x00000001;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_QUERYENDSESSION) || m.Msg.Equals(WM_ENDSESSION) || m.Msg.Equals(SHUTDOWN_NORETRY))
            {
                killMainProcess();
            }
            base.WndProc(ref m);
        }
    }
}
