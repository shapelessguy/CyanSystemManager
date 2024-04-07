using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Media;

namespace CyanSystemManager
{

    static public class Service_Display
    {
        static public string title = "displayService";
        static public string serviceType = ST.Display;
        static public State status = State.OFF;
        static public bool clear;
        static public int n_screens = 0;
        static public List<VolumeDisplay> vol_forms = new List<VolumeDisplay>();

        static public void IncreaseVol() { addCommand(DisplayCom.VOL_UP); }
        static public void DecreaseVol() { addCommand(DisplayCom.VOL_DOWN); }
        static public void SetVolNull() { addCommand(DisplayCom.VOL_NULL); }
        static public void SetVol(float vol) { addCommand(DisplayCom.SET_VOL, vol); }
        static public void ShowVol(VolSettings vol_settings) { addCommand(DisplayCom.SHOW_VOL, vol_settings); }
        static public void ShowMsg(MsgSettings msg) { addCommand(DisplayCom.SHOW, msg); }
        static public void ShowIndicator(IndicatorSettings indicator) { addCommand(DisplayCom.SHOW_INDICATOR, indicator); }

        // System is based on the interchange of messages
        static List<Command> commands = new List<Command>();
        static private void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }
        // run Example thread -> Interpret commands and call the appropriate functions inside the service
        static public void threadRun()
        {
            while (!forceTermination && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(100); // 10 fps
                    if (commands.Count == 0) continue;
                    Command command = commands[commands.Count - 1];
                    commands.Clear();
                    Tree(command);
                }
                catch (Exception ex) { Log("Exception in " + title); Log(ex.Message); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == DisplayCom.VOL_UP) ShowVolUp();
            else if (command.type == DisplayCom.VOL_DOWN) ShowVolDown();
            else if (command.type == DisplayCom.VOL_NULL) ShowVolNull();
            else if (command.type == DisplayCom.SET_VOL) ShowSetVol((float)command.value);
            else if (command.type == DisplayCom.SHOW_VOL) ShowVol_((VolSettings)command.value);
            else if (command.type == DisplayCom.SHOW) ShowMessage((MsgSettings)command.value);
            else if (command.type == DisplayCom.SHOW_INDICATOR) Show_Indicator((IndicatorSettings)command.value);
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Log("Starting " + title + "..");

            beforeStart();
            n_screens = 0;
            new Thread(threadRun).Start();
            new Thread(createDeleteForms).Start();
            status = State.ON;
        }
        static public void beforeStart() { }
        static public void stopService(bool dispose)
        {
            Log(title + " stopped");
            status = State.OFF;
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            Program.home.Invoke((MethodInvoker)delegate {
                foreach (var vol_form in vol_forms) { vol_form.DisposeAll(); }
            });
            vol_forms.Clear();
            clear = true;
        }
        // Inside functions
        static private void ShowVolUp() { Log("Do something"); }
        static private void ShowVolDown() { Log("Do something"); }
        static private void ShowVolNull() { Log("Do something"); }
        static private void ShowSetVol(float vol) { Log("Do something"); }
        static private void ShowVol_(VolSettings vol_settings) 
        {
            foreach (var form in vol_forms)
            {
                Program.home.Invoke((MethodInvoker)delegate { form.submit(vol_settings); });
            }
        }
        static private void emitSound()
        {
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Default.wav"))
            {
                soundPlayer.Play();
            }
        }

        static private void ShowMessage(MsgSettings message)
        {
            foreach (var form in vol_forms)
            {
                Program.home.Invoke((MethodInvoker)delegate { form.submit(message); });
            }
            emitSound();
        }
        static private void Show_Indicator(IndicatorSettings indicator)
        {
            foreach (var form in vol_forms)
            {
                Program.home.Invoke((MethodInvoker)delegate { form.submit(indicator); });
            }
            if (indicator.type == "START_WAITING") emitSound();
        }
        // //////////

        static private void createDeleteForms()
        {
            while (!forceTermination && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(100);
                    int n_screens_ = Screen.AllScreens.Length;
                    if (n_screens_ != n_screens)
                    {
                        n_screens = n_screens_;
                        Program.home.Invoke((MethodInvoker)delegate {
                            foreach (var vol_form in vol_forms) vol_form.DisposeAll();
                            vol_forms.Clear();
                            foreach (var screen in Screen.AllScreens) vol_forms.Add(new VolumeDisplay(screen));
                        });
                    }
                }
                catch (Exception ex) { Log("Exception in " + title); Log(ex.ToString()); Log(ex.Message); }
            }
        }
    }
}
