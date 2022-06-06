using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public enum Config
    {
        Default = 0,
        Gaming = 1,
        SoftGaming = 2,
    }
    public enum AT
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Headphones = 3,
        Third = 4,
    }

    public class application
    {
        public string[] win;
        public string exe;
        public string info;
        public string proc_name;
        public bool start;
        public application(string[] win, string proc_name, string exe, string info="", bool start=false)
        {
            this.win = win;
            this.proc_name = proc_name;
            this.exe = exe;
            this.info = info;
            this.start = start;
        }
    }

    public class Settings
    {
        static public bool noStat = false;
        public class variablePath
        {
            static readonly string name = Environment.UserName;
            public static string adminNetPass = @"E:\DOCUMENTI\Workspace Visual Studio\CyanSystemManager\" +
                                                @"AdminNetPass\asjkdfhkalshljsjbdkjbayvelndkjhhka.txt";
            public static string localFileSite = "localFile.txt";

            public static string displayFusion = @"C:\Program Files (x86)\DisplayFusion\DisplayFusionCommand.exe";
            public static string python = @"C:\Users\"+name+@"\AppData\Local\Programs\Python\Python38\pythonw.exe";
            public static string pyStatScript = @"E:\DOCUMENTI\Workspace PyCharm\Local Codes\Statistics - CyanFusion\Analysis.pyw";
            public static string notebookPath = @"E:\DOCUMENTI\Varie\Notebook";
            public static string networkPath = @"E:\DOCUMENTI\Varie\NetworkLogs";
        }

        public static class App
        {
            static readonly string name = Environment.UserName;

            public static application HW1 = new application(new string[] { "HW Monitoring Charts(1)" }, "", "");

            public static application HW2 = new application(new string[] { "HW Monitoring Charts(2)" }, "", "");

            public static application firefox = new application(new string[] { "Mozilla Firefox" }, "firefox",
                "C:\\Users\\" + name + @"\\AppData\\Local\\Mozilla Firefox\\firefox.exe", "", true);

            public static application chrome = new application(new string[] { "Google Chrome" }, "chrome",
                "E:\\DOCUMENTI\\Macros\\Accensione\\Google.url", "", true);

            public static application moneyguardW = new application(new string[] { "WidgetMoneyguard" }, "Moneyguard",
                "C:\\Program Files (x86)\\Cyan\\MoneyGuard\\Moneyguard.exe", "", false);

            public static application netmeterEvo = new application(new string[] { "NetMeter Evo" }, "NetMeterEvo",
                "E:\\DOCUMENTI\\Installazioni\\Network\\NetMeterEvo.exe", "", false);

            public static application clockX = new application(new string[] { "ClocX" }, "ClocX",
                "C:\\Program Files (x86)\\ClocX\\ClocX.exe", "", false);

            public static application posta = new application(new string[] { "- Posta" }, "",
                "E:\\DOCUMENTI\\Varie\\.AppLinks\\Posta.lnk", "", false);

            public static application thunderbird = new application(new string[] { "- Mozilla Thunderbird" }, 
                "thunderbird", "C:\\Program Files (x86)\\Mozilla Thunderbird\\thunderbird.exe", "", true);

            public static application calendario = new application(new string[] { "- Calendar" }, "",
                "E:\\DOCUMENTI\\Varie\\.AppLinks\\Calendario.lnk", "", true);

            public static application spotify = new application(new string[] { "Spotify", "Free" }, "",
                @"C:\\Users\" + name + @"\AppData\\Roaming\\Spotify\\Spotify.exe", "", true);

            public static application teams = new application(new string[] { "Microsoft Teams" }, "",
                @"C:\Users\" + name + @"\AppData\Local\Microsoft\Teams\Update.exe", " --processStart Teams.exe", true);

            public static application whatsapp = new application(new string[] { "WhatsApp" }, "WhatsApp",
                @"C:\Users\" + name + @"\AppData\Local\WhatsApp\WhatsApp.exe", "", true);

            public static application telegram = new application(new string[] { "Telegram" }, "Telegram",
                @"C:\Users\" + name + @"\AppData\Roaming\Telegram Desktop\Telegram.exe", "", true);

            public static application cyanTabata = new application(new string[] { "Cyan Tabata" }, "",
                "E:\\DOCUMENTI\\Workspace Visual Studio\\CyanTabata\\CyanTabata\\bin\\Debug\\CyanTabata.exe");

            public static application msiG = new application(new string[] { "MSI Afterburner", "grafici" }, "MSIAfterburner",
                "C:\\Program Files (x86)\\MSI Afterburner\\MSIAfterburner.exe");

            public static application displFusion = new application(new string[] { "Display Fusion" }, "",
                "C:\\Program Files (x86)\\DisplayFusion\\DisplayFusion.exe", "", true);

            public static application xPadder = new application(new string[] { "Xpadder [5.7]" }, "Xpadder [5.7]",
                "E:\\DOCUMENTI\\Installazioni\\Controllo monitor, tastiera, gamepad\\Xpadder\\Xpadder 5.7\\Xpadder [5.7].exe",
                "", true);

            public static application ical = new application(new string[] { "Desktop iCalendar" }, "Desktop iCalendar",
                "C:\\Program Files\\desksware\\Desktop iCalendar\\Desktop iCalendar.exe");

            public static application g_drive = new application(new string[] { "" }, "googledrivesync",
                "C:\\Program Files\\Google\\Drive\\googledrivesync.exe", "", true);

            public static application discord = new application(new string[] { "Discord" }, "discord",
                @"C:\Users\" + name + @"\AppData\Local\Discord\Update.exe", "--processStart Discord.exe", true);

            public static application torrent = new application(new string[] { "uTorrent" }, "uTorrent",
                @"C:\\Users\\" + name + @"\\AppData\\Roaming\\uTorrent\\uTorrent.exe");

            public static application cyanVideos = new application(new string[] { "CyanVideos" }, "CyanVideos",
                @"E:\\DOCUMENTI\\Workspace Visual Studio\\CyanVideos\\CyanVideos\\bin\\Debug\\CyanVideos.exe");

            static public Dictionary<string, application> getApplications()
            {
                FieldInfo[] fields = typeof(App).GetFields(BindingFlags.Static | BindingFlags.Public);

                Dictionary<string, application> dict = new Dictionary<string, application>();
                foreach (FieldInfo fi in fields) dict[fi.Name] = (application)fi.GetValue(null);
                return dict;
            }
        }

        public static List<AudioDevice> audioDevices = new List<AudioDevice>()
        {
            new AudioDevice(AT.None, new string[] { "" }, ""),
            new AudioDevice(AT.Primary, new string[] { "Sound Blaster Audigy Fx" }, "AudioDevicePrimary"),
            new AudioDevice(AT.Secondary,
                new string[] { "AUKEY EP-B40S Stereo", "AUKEY EP-B40S Hands-Free AG Audio" }, "AudioDeviceSecondary"),
            new AudioDevice(AT.Headphones, new string[] { "Soundcore Life Q30 Stereo", 
                "Realtek High Definition Audio" }, "AudioDeviceHeadphones"),
            new AudioDevice(AT.Third, new string[] { "19LS4D-ZB", "LG 19''" }, "AudioDeviceThird"),
        };

        // This list is useful in order to bind a key to a specific activity
        public static List<BindDef> activityList = new List<BindDef>()
        {
            new BindDef(ST.Audio, "vol+", Keys.VolumeUp),
            new BindDef(ST.Audio, "vol-", Keys.VolumeDown),
            new BindDef(ST.Audio, "noVol", Keys.VolumeMute),
            new BindDef(ST.Audio, "AudioPrimary", Keys.F14),
            new BindDef(ST.Audio, "AudioSecondary", Keys.F14, KeyModifier.Alt),
            new BindDef(ST.Audio, "AudioHeadphones", Keys.F14, KeyModifier.Control),
            new BindDef(ST.Audio, "AudioThird", Keys.F14, KeyModifier.Shift),

            new BindDef(ST.Shortcuts, "A", Keys.F10, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "B", Keys.F12, KeyModifier.Control|KeyModifier.Alt, KeyMode.Hold, 1000, true),
            new BindDef(ST.Shortcuts, "X", Keys.F2, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "Y", Keys.F1, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "Ltrigg", Keys.F5, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "Rtrigg", Keys.F6, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "LpadClick", Keys.F7, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "reset", Keys.F8, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "start", Keys.F9, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "order", Keys.F15, KeyModifier.Alt),
            new BindDef(ST.Shortcuts, "snapshot", Keys.PrintScreen),
            new BindDef(ST.Shortcuts, "mediumUpSizing", Keys.F4, KeyModifier.Control|KeyModifier.Alt),
            new BindDef(ST.Timer, "timer", Keys.F15),

            new BindDef(ST.Notebook, "notebook", Keys.F10, KeyModifier.Control|KeyModifier.Alt),

            new BindDef(ST.Monitors, "defaultProfile", Keys.F13, KeyModifier.Control),
            new BindDef(ST.Monitors, "gamingProfile", Keys.F13, KeyModifier.Alt),
            new BindDef(ST.Monitors, "softgamingProfile", Keys.F13, KeyModifier.Control|KeyModifier.Alt),

        };



        public static List<Monitor> monitors = new List<Monitor>()
        {
            new Monitor(1, "19LS4D-ZB"),
            new Monitor(2, "LG TV"),
            new Monitor(3, "W1943"),
        };

        public static List<Configuration> configurations = new List<Configuration>()
        {
            new Configuration(Config.Default, new List<int>(){ 1,3,2},
                                new List<Position>(){
                                    new Position(3, new Point(1400, 0)),
                                    new Position(1, new Point(0, 0)),
                                    // new Position(2),
                                }, "Normal"),

            new Configuration(Config.Gaming, new List<int>(){ 2,1,3},
                                new List<Position>(){
                                    new Position(2),
                                    new Position(1, new Point(1500, 2160)),
                                    new Position(3, new Point(3840, 2160-600)),
                                }, "Gaming"),

            new Configuration(Config.SoftGaming, new List<int>(){ 1,2,3},
                                new List<Position>(){
                                    new Position(1),
                                    new Position(2, new Point(-1500,-2160)),
                                    new Position(3, new Point(3840 - 1500,-600)),
                                }, "SoftGaming"),
        };

        public static Configuration actualConfiguration = Configuration.getConfiguration(Config.Default);
        public static void apply() { 
            for (int id = 0; id < activityList.Count; id++) activityList[id].id = id;
            string document_path = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
            if (!Directory.Exists(variablePath.notebookPath))
            {
                string main_dir = Path.GetDirectoryName(variablePath.notebookPath);
                if (!Directory.Exists(main_dir))
                {
                    string notebook_path = Path.Combine(document_path, "CyanNotebook");
                    Console.WriteLine(notebook_path);
                    variablePath.notebookPath = notebook_path;
                }
                Directory.CreateDirectory(variablePath.notebookPath);
            }
            if (!Directory.Exists(variablePath.networkPath))
            {
                string main_dir = Path.GetDirectoryName(variablePath.networkPath);
                if (!Directory.Exists(main_dir))
                {
                    string network_path = Path.Combine(document_path, "CyanNetwork");
                    variablePath.networkPath = network_path;
                }
                Directory.CreateDirectory(variablePath.networkPath);
            }

            if (!File.Exists(variablePath.python) || !File.Exists(variablePath.pyStatScript)) noStat = true;
        }
    }
}
