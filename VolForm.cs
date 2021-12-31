using NAudio.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public partial class VolForm : Form
    {
        private static Size defaultSize;
        private static Point defaultToppinLocation;
        private const Style defaultStyle = Style.Classic;  // change this defaultStyle in order to effectively 
                                                           // assign your custom style to the form
        private static Style style;
        private int nScreen;
        static bool imageThreadCreated = false;
        static private VolSettings previousSet;
        static public VolSettings actualSet;
        static private Image actualVolImage;
        public enum Style
        {
            Classic = 0,
            GiuloppisStyle = 1,  // you should rename this state of default
        }

        public VolForm(int nScreen, Style style_ = defaultStyle)
        {
            this.nScreen = nScreen;
            InitializeComponent();
            defaultSize = Size;
            defaultToppinLocation = terminal.Location;
            terminal.BringToFront();
            style = style_;
            if (!imageThreadCreated) new Thread(createImage).Start();
            if (style != Style.Classic)
            {
                textDevice.Visible = false;
                volValue.Visible = false;
                container.Visible = false;
            }
            else pictureBox.Visible = false;
            this.Load += new EventHandler(FormLoad);
        }
        void FormLoad(object sender, EventArgs e) { locate(); Refresh(); HideForm(); }

        public void animate(float volume, AudioInfo audioInfo)
        {
            if (!audioInfo.validated) return;
            if (this == null) return;
            Invoke((MethodInvoker)delegate {
                actualSet = new VolSettings(volume, audioInfo.mute, audioInfo.deviceName);
                Point location = locate();
                if (style != Style.Classic) pictureBox.Image = actualVolImage;
                else animate(actualSet);
            });
        }
        public void animate()
        {
            if (this == null) return;
            Invoke((MethodInvoker)delegate {
                Point location = locate();
                try
                {
                    if (style != Style.Classic) pictureBox.Image = actualVolImage;
                    else animate(actualSet);
                }
                catch (Exception) { }
            });
        }
        private Point locate()
        {
            Point location = new Point();
            if (Screen.AllScreens.Count() > nScreen)
            {
                Screen screen = Screen.AllScreens[nScreen];
                location = new Point(screen.Bounds.Location.X + 20, screen.Bounds.Location.Y + 15);
                Location = location;
            }
            return location;
        }
        public void ShowForm() { 
            try
            {
                if (this.Handle != null)
                {
                    if(Width<10) Invoke((MethodInvoker)delegate { Home.setTopAndTransparent(Handle); Size = defaultSize;  });
                    Invoke((MethodInvoker)delegate { Show(); Refresh(); });
                }
            }
            catch (Exception) { }
        }
        public void HideForm()
        {
            try {  if (this.Handle != null) Invoke((MethodInvoker)delegate { Size = new Size(0, 0); }); }
            catch (Exception) { }
        }

        private void animate(VolSettings settings)
        {
            volBar.Size = container.Size;
            int Y = (int)((container.Size.Height - 4 - terminal.Height) * (1 - settings.volume));
            string text;
            if (!settings.mute)
            {
                volBar.Location = new Point(-2, Y);
                terminal.Location = new Point(defaultToppinLocation.X, Y + defaultToppinLocation.Y);
                text = ((int)((settings.volume + 0.005) * 100)).ToString();
            }
            else
            {
                volBar.Location = new Point(0, container.Size.Height + 50);
                text = "Mute";
            }

            try { if (settings.deviceName != "") textDevice.Text = settings.deviceName; }
            catch (Exception) { }
            volValue.Text = text;
        }

        private void createImage()
        {
            imageThreadCreated = true;
            while (!Program.timeToClose)
            {
                if (Service_Audio.status == State.OFF || style == Style.Classic) { Thread.Sleep(100); continue; }

                if (actualSet!= null && !actualSet.equalsTo(previousSet))
                {
                    actualVolImage = new Bitmap(pictureBox.Width, pictureBox.Height);
                    Graphics g = Graphics.FromImage(actualVolImage);
                    if (style == Style.GiuloppisStyle) VolFormGiulietto.Draw(g, actualSet);
                    previousSet = actualSet;
                }
                Thread.Sleep(5);
            }
        }

        private void killMainProcess()
        {
            void kill()
            {
                Thread.Sleep(1000);
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
