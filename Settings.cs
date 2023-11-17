using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static CyanSystemManager.Utility;
using NAudio.CoreAudioApi;

namespace CyanSystemManager
{
    public class Settings
    {
        static public bool noStat = false;
        public class variablePath
        {
            public static string localFileSite = "localFile.txt";
            static readonly string documents_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            static readonly string app_data_path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            static readonly string prog86_path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            public static string adminNetPass = documents_path + @"\Workspace Visual Studio\CyanSystemManager\AdminNetPass\asjkdfhkalshljsjbdkjbayvelndkjhhka.txt";

            public static string displayFusion = prog86_path + @"\DisplayFusion\DisplayFusionCommand.exe";
            public static string python = app_data_path + @"\Local\Programs\Python\Python311\pythonw.exe";
            public static string pyStatScript = documents_path + @"\Workspace PyCharm\University-Basic Projects\Statistics - CyanFusion\Analysis.pyw";
            public static string cyanPath = @"C:\ProgramData\Cyan";
            public static string notebookPath = cyanPath + @"\Notebook";
            public static string networkPath = cyanPath + @"\NetworkLogs";
            public static string multiMonitor = @"E:\Software\Controller\MultiMonitorTool\MultiMonitorTool.exe";
        }

        public static class App
        {
            static readonly string name = Environment.UserName;
            static readonly string documents_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            static readonly string app_data_path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            static readonly string drive_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).Substring(0, 2);
            static readonly string prog86_path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            public static application opera = new application(
                new string[] { "Opera" },
                "opera",
                app_data_path + "\\Local\\Programs\\Opera\\launcher.exe");

            public static application firefox = new application(
                new string[] { "Mozilla Firefox" },
                "firefox",
                drive_path + "\\Program Files\\Mozilla Firefox\\firefox.exe",
                false);

            public static application netmeterEvo = new application(
                new string[] { "NetMeter Evo" }, 
                "NetMeterEvo",
                "E:\\Software\\Network\\NetMeterEvo.exe");

            public static application clockX = new application(
                new string[] { "ClocX" }, 
                "ClocX",
                prog86_path + "\\ClocX\\ClocX.exe",
                true, "", "#32770");

            public static application outlook = new application(
                new string[] { "Claudio.Ciano@dlr.de - Outlook" },
                "OUTLOOK",
                prog86_path + "\\Microsoft Office\\root\\Office16\\OUTLOOK.exe");

            public static application outlookNew = new application(
                new string[] { "Claudio Ciano - Outlook" },
                "olk",
                documents_path + "\\Outlook.lnk");

            public static application skypeFB = new application(
                new string[] { "Skype for Business" },
                "lync",
                prog86_path + "\\Microsoft Office\\root\\Office16\\lync.exe");

            public static application spotify = new application(
                new string[] {}, 
                "Spotify",
                app_data_path + "\\Roaming\\Spotify\\Spotify.exe",
                true, "", "Spotify");

            public static application teams = new application(
                new string[] { "Microsoft Teams" }, "",
                app_data_path + "\\Local\\Microsoft\\Teams\\Update.exe",
                false, 
                " --processStart Teams.exe");

            public static application whatsapp = new application(
                new string[] { "WhatsApp" },
                "WhatsApp",
                @"",
                false);

            public static application nordvpn = new application(
                new string[] { "NordVPN" },
                "NordVPN",
                @"",
                false);

            public static application deepL = new application(
                new string[] { "DeepL" }, 
                "DeepL",
                app_data_path + "\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\DeepL.lnk", 
                true,
                "run --no-wait https://appdownload.deepl.com/windows/0install/deepl.xml");

            public static application chatGPT = new application(
                new string[] { "ChatGPT" }, 
                "ChatGPT",
                documents_path + "\\Workspace PyCharm\\miscellaneous\\OpenAI\\ChatGPT.lnk");

            public static application msiG = new application(
                new string[] { "MSI Afterburner", "hardware monitor" }, 
                "MSIAfterburner",
                prog86_path + "\\MSI Afterburner\\MSIAfterburner.exe");

            public static application displFusion = new application(
                new string[] { "Display Fusion" }, 
                "",
                prog86_path + "\\DisplayFusion\\DisplayFusion.exe");

            public static application discord = new application(
                new string[] { "Discord" },
                "discord",
                app_data_path + "\\Local\\Discord\\Update.exe",
                true);

            // public static application chrome = new application(new string[] { "Google Chrome" }, "chrome",
            //     "Path\\to\\google.exe", "", false);

            // public static application moneyguardW = new application(new string[] { "WidgetMoneyguard" }, "Moneyguard",
            //     "C:\\Program Files (x86)\\Cyan\\MoneyGuard\\Moneyguard.exe", "", false);

            // public static application posta = new application(new string[] { "- Posta" }, "",
            //     "E:\\DOCUMENTI\\Varie\\.AppLinks\\Posta.lnk", "", false);

            // public static application thunderbird = new application(new string[] { "- Mozilla Thunderbird" },
            //     "thunderbird", "C:\\Program Files\\Mozilla Thunderbird\\thunderbird.exe", "", true);

            // public static application calendario = new application(new string[] { "- Calendar" }, "",
            //     "E:\\DOCUMENTI\\Varie\\.AppLinks\\Calendario.lnk", "", false);

            // public static application telegram = new application(new string[] { "Telegram" }, "Telegram",
            //      @"C:\Users\" + name + @"\AppData\Roaming\Telegram Desktop\Telegram.exe", "", true);

            // public static application cyanTabata = new application(new string[] { "Cyan Tabata" }, "",
            //     "E:\\DOCUMENTI\\Workspace Visual Studio\\CyanTabata\\CyanTabata\\bin\\Debug\\CyanTabata.exe");

            // public static application xPadder = new application(new string[] { "Xpadder [5.7]" }, "Xpadder [5.7]",
            //     "E:\\DOCUMENTI\\Installazioni\\Controller\\Xpadder\\Xpadder 5.7\\Xpadder [5.7].exe", "", false);

            static public Dictionary<string, application> getApplications()
            {
                FieldInfo[] fields = typeof(App).GetFields(BindingFlags.Static | BindingFlags.Public);

                Dictionary<string, application> dict = new Dictionary<string, application>();

                foreach (FieldInfo fi in fields)
                {
                    try
                    {
                        dict[fi.Name] = (application)fi.GetValue(null);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
                return dict;
            }
        }

        public static List<AudioDevice> audioDevices = new List<AudioDevice>()
        {
            new AudioDevice(AT.None),
            new AudioDevice(AT.Primary, "Speakers"),
            new AudioDevice(AT.Headphones, "Headphones", "Soundcore Life Q30 Stereo"),
            new AudioDevice(AT.Secondary, "SAMSUNG"),
            new AudioDevice(AT.Third, "19LS4D-ZB", "LG 19''"),
        };

        // This list is useful in order to bind a key to a specific activity
        public static List<BindDef> activityList = new List<BindDef>()
        {
            new BindDef(ST.Audio, "vol+", Keys.VolumeUp),
            new BindDef(ST.Audio, "vol-", Keys.VolumeDown),
            new BindDef(ST.Audio, "noVol", Keys.VolumeMute),

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

            new BindDef(ST.Notebook, "notebook", Keys.F14),
            new BindDef(ST.Audio, "AudioPrimary", Keys.F14, KeyModifier.Alt),
            new BindDef(ST.Audio, "AudioSecondary", Keys.F14, KeyModifier.Control),
            new BindDef(ST.Audio, "AudioHeadphones", Keys.F14, KeyModifier.Shift),
            // new BindDef(ST.Audio, "AudioThird", Keys.F14, KeyModifier.Shift),

            new BindDef(ST.Monitors, "centralize", Keys.F13),
            new BindDef(ST.Monitors, "TOM", Keys.F13, KeyModifier.Shift),
            new BindDef(ST.Monitors, "SDM", Keys.F13, KeyModifier.Control),
            new BindDef(ST.Monitors, "moveToMonitor", Keys.F15, KeyModifier.Shift),
        };



        public static List<MyMonitor> monitors = new List<MyMonitor>()
        {
            new MyMonitor(1, "DEL41EE"),
            new MyMonitor(2, "HKC2413", 0),
            new MyMonitor(3, "HKC2413", 1),
            new MyMonitor(4, "SAM71B5"),
        };

        public static void apply()
        {
            for (int id = 0; id < activityList.Count; id++) activityList[id].id = id;
            if (!Directory.Exists(variablePath.notebookPath)) Directory.CreateDirectory(variablePath.notebookPath);
            if (!Directory.Exists(variablePath.networkPath)) Directory.CreateDirectory(variablePath.networkPath);
            if (!File.Exists(variablePath.python) || !File.Exists(variablePath.pyStatScript)) noStat = true;
            if (MonitorManager.allMonitors.Count == 0) MonitorManager.getMonitorConfiguration();

            Console.WriteLine("\nMonitors ->");
            foreach (var monitor in MonitorManager.allMonitors) { monitor.Print(); }

            Console.WriteLine("\nAudio devices ->");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)) Console.WriteLine(endpoint.FriendlyName);

            Console.WriteLine("\nSettings validation ->");

            if (!Directory.Exists(variablePath.networkPath)) Console.WriteLine("Directory " + variablePath.networkPath + " does not exist!");
            if (!Directory.Exists(variablePath.notebookPath)) Console.WriteLine("Directory " + variablePath.notebookPath + " does not exist!");
            if (!File.Exists(variablePath.adminNetPass)) Console.WriteLine("File " + variablePath.adminNetPass + " does not exist!");
            if (!File.Exists(variablePath.displayFusion)) Console.WriteLine("File " + variablePath.displayFusion + " does not exist!");
            if (!File.Exists(variablePath.python)) Console.WriteLine("File " + variablePath.python + " does not exist!");
            if (!File.Exists(variablePath.pyStatScript)) Console.WriteLine("File " + variablePath.pyStatScript + " does not exist!");
            if (!File.Exists(variablePath.multiMonitor)) Console.WriteLine("File " + variablePath.multiMonitor + " does not exist!");
            Console.WriteLine("--------------");

            foreach(var item in App.getApplications())
            {
                if (item.Value.exe != "" && !File.Exists(item.Value.exe)) Console.WriteLine("File " + item.Value.exe + " does not exist!");
            }
            Console.WriteLine("");
        }
    }
}
