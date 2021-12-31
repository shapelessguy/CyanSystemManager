using System.Windows.Forms;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
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
        static public string SET_DEV = "SetDev";
        static public string SET_VOL = "SetVol";

        static public AT PRIMARY = AT.Primary;
        static public AT SECONDARY = AT.Secondary;
        static public AT HEADPHONES = AT.Headphones;
        static public AT THIRD = AT.Third;

        static public string TEMPAUDIO = "tempAudio";
    }
    public class ShortcutCom
    {
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
        static public string DEFAULT = "Default";
        static public string GAMING = "Gaming";
        static public string SOFTGAMING = "SoftGaming";
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
            string argument = (string)arg;
            if (timeToClose) return;
            // after a key is pressed and captured by IntPtr, it has to be specified the function associated to the first
            //if (name == "examp") ExampleService.FunctionFromOutside();

            if (name == "vol+") Service_Audio.MasterVolume(AudioCom.VOL_UP);
            else if (name == "vol-") Service_Audio.MasterVolume(AudioCom.VOL_DOWN);
            else if (name == "noVol") Service_Audio.MasterVolume(AudioCom.VOL_NULL);

            else if (name == "A") Service_Shortcut.KeyPad(ShortcutCom.A);
            else if (name == "B") Service_Shortcut.KeyPad(ShortcutCom.B, mode);
            else if (name == "X") Service_Shortcut.KeyPad(ShortcutCom.X);
            else if (name == "Y") Service_Shortcut.KeyPad(ShortcutCom.Y);
            else if (name == "Ltrigg") Service_Shortcut.KeyPad(ShortcutCom.LTRIG);
            else if (name == "Rtrigg") Service_Shortcut.KeyPad(ShortcutCom.RTRIG);
            else if (name == "LpadClick") Service_Shortcut.KeyPad(ShortcutCom.LPAD);
            else if (name == "reset") Service_Shortcut.KeyPad(ShortcutCom.RST);
            else if (name == "start") Service_Shortcut.KeyPad(ShortcutCom.START);
            else if (name == "order") Service_Shortcut.KeyOrder();
            else if (name == "snapshot") Service_Shortcut.TakeSnapshot();
            else if (name == "mediumUpSizing") Service_Shortcut.UpSizing();

            else if (name == "AudioPrimary") Service_Audio.SetDevice(AudioCom.PRIMARY);
            else if (name == "AudioSecondary") Service_Audio.SetDevice(AudioCom.SECONDARY);
            else if (name == "AudioHeadphones") Service_Audio.SetDevice(AudioCom.HEADPHONES);
            else if (name == "AudioThird") Service_Audio.SetDevice(AudioCom.THIRD);

            else if (name == "defaultProfile") Service_Monitor.changeProfile(MonitorCom.DEFAULT);
            else if (name == "gamingProfile") Service_Monitor.changeProfile(MonitorCom.GAMING);
            else if (name == "softgamingProfile") Service_Monitor.changeProfile(MonitorCom.SOFTGAMING);

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
