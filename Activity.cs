using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{

    public enum AT
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Headphones = 3,
        Third = 4,
    }
    public enum VT
    {
        Generic = 0,
        Primary = 1,
        Ausiliary1 = 2,
        Ausiliary2 = 3,
        Cinema = 4,
    }

    public class application
    {
        public string name;
        public string[] win;
        public string exe;
        public string info;
        public string proc_name;
        public bool start;
        public bool admin;
        /// <summary>
        /// Instantiate an application.
        /// </summary>
        /// <param name="win">Array containing substrings findable in the window's name.</param>
        /// <param name="proc_name">Name of the process.</param>
        /// <param name="exe">Exe file needed for executing the application.</param>
        /// <param name="start">If true, the application will be executed at the startup.</param>
        /// <param name="info">Optional arguments for the execution of the application.</param>
        public application(string name, string[] win, string proc_name, string exe, string info = "", bool start = true, bool admin = false)
        {
            this.name = name;
            this.win = win;
            this.proc_name = proc_name.Split(new string[] {"."}, System.StringSplitOptions.None)[0];
            string valid_exe = "";
            foreach (var value in exe.Split(new string[] { "\\" }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (value.Substring(0, 1) != "@")
                {
                    if (valid_exe.Length == 0) valid_exe = value;
                    else valid_exe += "\\" + value;
                }
                else
                {
                    string[] keys = value.Substring(1).Split(new string[] { "&&" }, System.StringSplitOptions.None);
                    foreach (var dir in Directory.GetDirectories(valid_exe).Reverse())
                    {
                        bool continue_ = false;
                        foreach (var key in keys) { if (!dir.Contains(key)) { continue_ = true; break; } }
                        if (continue_) continue;
                        valid_exe = dir;
                        break;
                    }
                }
            }
            Log(valid_exe);
            this.exe = valid_exe;
            this.info = info;
            this.start = start;
            this.admin = admin;
        }
    }

    // //////////// Example commands
    public class ExampleCom
    {
        static public string EX_COM1 = "COM1_Name";
        static public string EX_COM2 = "COM2_Name";
    }
    // ////////////////////////////////////////////////////
    public class AudioCom
    {
        static public string VOL_UP = "VOL+";
        static public string VOL_DOWN = "VOL-";
        static public string VOL_NULL = "VOLNULL";
        static public string PLAY_PAUSE = "PLAYPAUSE";
        static public string SET_DEV = "SetDev";
        static public string SET_VOL = "SetVol";

        static public AT PRIMARY = AT.Primary;
        static public AT SECONDARY = AT.Secondary;
        static public AT HEADPHONES = AT.Headphones;
        static public AT THIRD = AT.Third;

        static public string TEMPAUDIO = "tempAudio";
    }
    public class DisplayCom
    {
        static public string VOL_UP = "VOL+";
        static public string VOL_DOWN = "VOL-";
        static public string VOL_NULL = "VOLNULL";
        static public string SET_VOL = "SETVOL";
        static public string SHOW_VOL = "SHOWVOL";
        static public string SHOW = "SHOW";
        static public string SHOW_INDICATOR = "SHOWINDICATOR";
    }
    public class ShortcutCom
    {
        static public string SHOW_MENU = "SHOW_MENU";
        static public string ORDER = "orderWin";
        static public string A = "A";
        static public string X = "X";
        static public string Y = "Y";
        static public string B = "B";
        static public string LTRIG = "LTRIG";
        static public string RTRIG = "RTRIG";
        static public string LPAD = "LPAD";
        static public string RST = "RST";
        static public string START = "START";
        static public string SNAPSHOT = "SNAPSHOT";
        static public string UPSIZING = "UPSIZING";
    }
    public class MonitorCom
    {
        static public string CENTRALIZE = "centralize";
        static public string SORT = "sort";
        static public string TURN_ON_MONITORS = "TOM";
        static public string SHUT_DOWN_MONITORS = "SDM";
    }
    public class TimerCom
    {
        static public string TIMER = "FastTimer";
        static public string LONGPRESS = "longSystemTimer";
        static public string SHORTPRESS = "shortSystemTimer";
    }
    public class NotebookCom { static public string NOTEOPEN = "NoteOpen"; }
    public class StartCom { static public string START = "Start"; }

    public class Activity
    {
        public static void startActivity(string name, object arg, KeyMode mode)
        {
            if (Service_Shortcut.status != State.ON) return;
            string argument = (string)arg;
            if (forceTermination) return;
            // after a key is pressed and captured by IntPtr, it has to be specified the function associated to the first
            //if (name == "examp") ExampleService.FunctionFromOutside();

            if (name == "vol+") Service_Audio.MasterVolume(AudioCom.VOL_UP);
            else if (name == "vol-") Service_Audio.MasterVolume(AudioCom.VOL_DOWN);
            else if (name == "noVol") Service_Audio.MasterVolume(AudioCom.VOL_NULL);

            else if (name == "menu") Service_Shortcut.Show_Menu();
            else if (name == "A") Service_Shortcut.KeyPad(ShortcutCom.A);
            else if (name == "B") Service_Shortcut.KeyPad(ShortcutCom.B, mode);
            else if (name == "X") Service_Shortcut.KeyPad(ShortcutCom.X);
            else if (name == "Y") Service_Shortcut.KeyPad(ShortcutCom.Y);
            else if (name == "Ltrigg") Service_Shortcut.KeyPad(ShortcutCom.LTRIG);
            else if (name == "Rtrigg") Service_Shortcut.KeyPad(ShortcutCom.RTRIG);
            else if (name == "LpadClick") Service_Shortcut.KeyPad(ShortcutCom.LPAD);
            else if (name == "reset") Service_Shortcut.KeyPad(ShortcutCom.RST);
            else if (name == "start") Service_Shortcut.KeyPad(ShortcutCom.START);
            else if (name == "snapshot") Service_Shortcut.TakeSnapshot();
            else if (name == "mediumUpSizing") Service_Shortcut.UpSizing();

            else if (name == "AudioPrimary") Service_Audio.SetDevice(AudioCom.PRIMARY);
            else if (name == "AudioSecondary") Service_Audio.SetDevice(AudioCom.SECONDARY);
            else if (name == "AudioHeadphones") Service_Audio.SetDevice(AudioCom.HEADPHONES);
            else if (name == "AudioThird") Service_Audio.SetDevice(AudioCom.THIRD);

            else if (name == "centralize") Service_Monitor.function(MonitorCom.CENTRALIZE);
            else if (name == "TOM") Service_Monitor.function(MonitorCom.TURN_ON_MONITORS);
            else if (name == "SDM") Service_Monitor.function(MonitorCom.SHUT_DOWN_MONITORS);
            else if (name == "order") Service_Monitor.function(MonitorCom.SORT);

            else if (name == "timer") Service_Timer.TimerPressed();
            else if (name == "notebook") Service_Notebook.OpenOrClose();
        }
    }
    public class BindDef
    {
        public string service;
        public int id;
        public string name;
        public Keys key;
        public KeyModifier modifier;
        public KeyMode mode;
        public int trigger;
        public bool initialTrigger;

        public BindDef(string service, string name, Keys key, KeyModifier modifier = KeyModifier.None,
                                    KeyMode mode = KeyMode.Normal, int trigger = 0, bool initialTrigger = false)
        {
            this.service = service;
            this.name = name;
            this.key = key;
            this.modifier = modifier;
            this.mode = mode;
            this.trigger = trigger;
            this.initialTrigger = initialTrigger;
        }
    }
}
