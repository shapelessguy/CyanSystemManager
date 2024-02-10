using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;
using Tools;
using static CyanSystemManager.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CyanSystemManager
{
    public class VolFormHelper
    {
        static bool dispose;
        int timeout = 80; // proportional to the time needed for hiding Volume form
        int generalInterval = 5;
        int animationDelay = 3; // min 2

        public int screenId = -1;
        public static bool visible = false;
        public static System.Threading.Timer timerGeneralUpdate;
        public System.Threading.Timer timerUpdate;
        public static float[] volHist;
        public static List<VolSettings> messages;
        private VolForm volForm;
        int nScreen;
        bool destroyed = false;

        public VolFormHelper(int nScreen)
        {
            //Console.WriteLine("nre volForm");
            messages = new List<VolSettings>();
            this.nScreen = nScreen;
            if (timerGeneralUpdate == null)
                timerGeneralUpdate = new System.Threading.Timer(generalUpdate, null, generalInterval, Timeout.Infinite);
        }
        public void initializeForm(int screenId)
        {
            //Console.WriteLine("initializeForm  " + screenId);
            dispose = false;
            this.screenId = screenId;
            destroyed = false;
            if (volForm == null)
            {
                timerUpdate = new System.Threading.Timer(Update, null, generalInterval, Timeout.Infinite);
                Program.home.Invoke((MethodInvoker)delegate { volForm = new VolForm(nScreen);});
                Home.setTopAndTransparent(volForm.Handle);
                volForm.Show();
            }
        }
        public void tempShow() {countDownHide = timeout; visible = true; }
        public void Show()
        {
            if (volForm == null) return;
            try { Program.home.Invoke((MethodInvoker)delegate { volForm.ShowForm(); }); }
            catch (Exception) { }
        }
        public void Hide()
        {
            if (volForm == null) return;
            if (!volForm.Visible || volForm.Width < 5) return;
            try { Program.home.Invoke((MethodInvoker)delegate { volForm.HideForm(); }); }
            catch (Exception) { }
        }
        public void Dispose()
        {
            if (volForm == null) return;
            try { Program.home.Invoke((MethodInvoker)delegate { volForm.DisposeAll(); volForm = null; }); }
            catch (Exception) { }
        }

        int iteration = 0;
        float prev_volume = -1f;
        static DateTime prev;
        private void generalUpdate(Object state)
        {
            if (Program.forceTermination) { dispose = true; return; }
            if (Service_Audio.status==State.OFF) { timerGeneralUpdate.Change(100, Timeout.Infinite); return; }
            if (iteration % animationDelay == 0)
            {
                int n = messages.Count();
                if (n == 0) { timerGeneralUpdate.Change(generalInterval, Timeout.Infinite); return; }
                if (n > 5) for (int i = n-5-1; i >= 0; i--) messages.RemoveAt(i);

                if (prev == null) prev = DateTime.Now;
                long ms = (long)DateTime.Now.Subtract(prev).TotalMilliseconds;
                prev = DateTime.Now;
                iteration = 0;
                float act_volume = messages[0].volume;
                messages.RemoveAt(0);
                if (prev_volume < 0) prev_volume = act_volume;
                float gap = act_volume - prev_volume;
                volHist = new float[animationDelay];
                for (int i = 0; i < animationDelay; i++)
                {
                    if (ms > 100) volHist[i] = act_volume;
                    else volHist[i] = prev_volume + (i+1) * (gap / (animationDelay));
                }
                prev_volume = act_volume;
            }
            VolForm.actualSet = new VolSettings(volHist[iteration], Service_Audio.audioInfo.mute, Service_Audio.audioInfo.deviceName);
            iteration++;
            timerGeneralUpdate.Change(generalInterval, Timeout.Infinite);
        }

        int countDownHide = 0;
        private void Update(Object state)
        {
            if (dispose)
            {
                Dispose();
                timerUpdate.Dispose();
                timerGeneralUpdate = null; 
                volHist = null;
                return;
            }
            if (Program.forceTermination) return;
            if (Service_Audio.status == State.OFF || destroyed) 
                { Hide(); timerUpdate.Change(300, Timeout.Infinite); return; }
            if (volHist == null) { timerUpdate.Change(generalInterval, Timeout.Infinite); return; }

            if (volForm != null)
            {
                try
                {
                    if (visible) { Show(); countDownHide--; } else { countDownHide = 0; Hide(); }
                    if (countDownHide <= 0) { countDownHide = 0; visible = false; }
                    else visible = true;
                    if (visible) volForm.animate();
                }
                catch (Exception) { }
            }
            
            timerUpdate.Change(generalInterval, Timeout.Infinite);
        }
    }

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
}
