using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static CyanSystemManager.Utility;
using NAudio.CoreAudioApi;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using static Vanara.PInvoke.Kernel32.FILE_REMOTE_PROTOCOL_INFO;

namespace CyanSystemManager
{
    public class Settings
    {
        static public bool noStat = false;
        public class variablePath
        {
            static readonly string documents_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            static readonly string app_data_path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            static readonly string prog86_path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            public static string python = app_data_path + @"\Local\Programs\Python\Python311\pythonw.exe";
            public static string pyStatScript = documents_path + @"\WorkPySpace\Python-Projects\StatisticsCyanFusion\Analysis.pyw";
            public static string pyWgScript = documents_path + @"\WorkPySpace\sharedCode\WG\cleaning_plan_sequence.pyw";
            public static string pyVacWgJson = documents_path + @"\WorkPySpace\sharedCode\WG\vacations.json";
            public static string pySwapWgJson = documents_path + @"\WorkPySpace\sharedCode\WG\swaps.json";
            public static string pyCalendarWg = documents_path + @"\WorkPySpace\sharedCode\WG\calendar.txt";
            public static string chatbot = documents_path + @"\WorkC#Space\ExeWrapper\ExeWrapper\bin\Debug\net6.0\CyanChatBot.exe";

            public static string notebookPath = @"C:\ProgramData\Cyan\Notebook";
            public static string networkPath = @"C:\ProgramData\Cyan\NetworkLogs";
        }

        public static List<AudioDevice> audioDevices = new List<AudioDevice>()
        {
            new AudioDevice(AT.None),
            new AudioDevice(AT.Primary, new string[] { "Speakers (Realtek(R) Audio)" }),
            new AudioDevice(AT.Headphones, new string[] { "Headphones (Soundcore Life Q30 Stereo)" }),
            new AudioDevice(AT.Secondary, new string[] { "SAMSUNG (NVIDIA High Definition Audio)" }),
            new AudioDevice(AT.Third, new string[] { "19LS4D-ZB" }),
        };

        public static List<MyMonitor> monitors = new List<MyMonitor>()
        {
            new MyMonitor(VT.Primary, "DEL41EE"),
            new MyMonitor(VT.Ausiliary1, "HKC2413"),
            new MyMonitor(VT.Ausiliary2, "HKC2413(2)"),
            new MyMonitor(VT.Cinema, "SAM71B4"),
        };

        public static class App
        {
            static readonly string name = Environment.UserName;
            static readonly string documents_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            static readonly string app_data_path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            static readonly string drive_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).Substring(0, 2);
            static readonly string prog86_path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            public static List<application> all_apps = new List<application>()
            {
                new application(
                    "Opera",
                    new string[] { "Opera" },
                    "opera",
                    app_data_path + "\\Local\\Programs\\Opera\\launcher.exe"
                    ),
                new application(
                    "Mozilla Firefox",
                    new string[] { "Mozilla Firefox" },
                    "firefox",
                    drive_path + "\\Program Files\\Mozilla Firefox\\firefox.exe",
                    "", false),
                new application(
                    "NetMeter Evo",
                    new string[] { "NetMeter Evo" },
                    "NetMeterEvo",
                    "E:\\Software\\Network\\NetMeterEvo.exe"
                    ),
                new application(
                    "ClocX",
                    new string[] { "ClocX" },
                    "ClocX",
                    prog86_path + "\\ClocX\\ClocX.exe",
                    "", true
                    ),
                new application(
                    "Outlook",
                    new string[] { "Claudio.Ciano@dlr.de - Outlook" },
                    "OUTLOOK",
                    prog86_path + "\\Microsoft Office\\root\\Office16\\OUTLOOK.exe"
                    ),
                new application(
                    "Outlook NEW",
                    new string[] { "Claudio Ciano - Outlook" },
                    "olk",
                    "C:\\Program Files\\WindowsApps" + "\\@OutlookForWindows&&x64" + "\\olk.exe"
                    ),
                new application(
                    "Skype for Business",
                    new string[] { "Skype for Business" },
                    "lync",
                    prog86_path + "\\Microsoft Office\\root\\Office16\\lync.exe"
                    ),
                new application(
                    "Spotify",
                    new string[] {},
                    "Spotify",
                    app_data_path + "\\Roaming\\Spotify\\Spotify.exe",
                    ""
                    ),
                new application(
                    "Microsoft Teams",
                    new string[] { "Microsoft Teams" }, "",
                    app_data_path + "\\Local\\Microsoft\\Teams\\Update.exe",
                    " --processStart Teams.exe",
                    false
                    ),
                new application(
                    "WhatsApp",
                    new string[] { "WhatsApp" },
                    "WhatsApp",
                    "",
                    "", false
                    ),
                new application(
                    "NordVPN",
                    new string[] { "NordVPN" },
                    "NordVPN",
                    @"",
                    "",
                    false
                    ),
                new application(
                    "DeepL",
                    new string[] { "DeepL" },
                    "DeepL",
                    app_data_path + "\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\DeepL.lnk",
                    "run --no-wait https://appdownload.deepl.com/windows/0install/deepl.xml",
                    true
                    ),
                new application(
                    "ChatGPT",
                    new string[] { "ChatGPT" },
                    "ChatGPT",
                    documents_path + "\\WorkPySpace\\miscellaneous\\OpenAI\\ChatGPT.lnk"
                    ),
                new application(
                    "MSI Afterburner",
                    new string[] { "MSI Afterburner", "hardware monitor" },
                    "MSIAfterburner",
                    prog86_path + "\\MSI Afterburner\\MSIAfterburner.exe"
                    ),
                new application(
                    "Display Fusion",
                    new string[] { "Display Fusion" },
                    "",
                    prog86_path + "\\DisplayFusion\\DisplayFusion.exe", "", true, true
                    ),
                new application(
                    "Discord",
                    new string[] { "Discord" },
                    "discord",
                    app_data_path + "\\Local\\Discord\\Update.exe",
                    ""
                    ),
            };

            static public Dictionary<string, application> getApplications()
            {
                Dictionary<string, application> dict = new Dictionary<string, application>();
                foreach (var app in all_apps)
                {
                    dict[app.name] = app;
                }
                return dict;
            }

            static public application getApp(string name)
            {
                application out_ = null;
                foreach(var app in all_apps) { if (app.name== name) { out_ = app;break; } }
                return out_;
            }
        }

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
            new BindDef(ST.Shortcuts, "menu", Keys.F16),

            new BindDef(ST.Notebook, "notebook", Keys.F14),
            new BindDef(ST.Audio, "AudioPrimary", Keys.F14, KeyModifier.Alt),
            new BindDef(ST.Audio, "AudioSecondary", Keys.F14, KeyModifier.Control),
            new BindDef(ST.Audio, "AudioHeadphones", Keys.F14, KeyModifier.Shift),
            new BindDef(ST.Audio, "AudioThird", Keys.F14, KeyModifier.Shift),

            // new BindDef(ST.Monitors, "centralize", Keys.F13),
            new BindDef(ST.Monitors, "TOM", Keys.F13, KeyModifier.Shift),
            new BindDef(ST.Monitors, "SDM", Keys.F13, KeyModifier.Control),
            new BindDef(ST.Monitors, "moveToMonitor", Keys.F15, KeyModifier.Shift),
        };

        public static void apply()
        {
            for (int id = 0; id < activityList.Count; id++) activityList[id].id = id;
            if (!Directory.Exists(variablePath.notebookPath)) Directory.CreateDirectory(variablePath.notebookPath);
            if (!Directory.Exists(variablePath.networkPath)) Directory.CreateDirectory(variablePath.networkPath);
            if (File.Exists("ip_server.txt")) FirebaseClass.serverIp = File.ReadAllText("ip_server.txt");
            if (!File.Exists(variablePath.python) || !File.Exists(variablePath.pyStatScript)) noStat = true;

            if (MonitorManager.allMonitors.Count == 0)
            {
                List<Monitor> all_monitors_ = MonitorManager.getMonitorConfiguration();
                if (MonitorManager.allMonitors.Count == 0) MonitorManager.allMonitors = all_monitors_;
            }

            Program.Log("\nMonitors ->");
            foreach (var monitor in MonitorManager.allMonitors) { monitor.Print(); }

            Program.Log("\nAudio devices ->");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)) Program.Log(endpoint.FriendlyName);

            Program.Log("\nSettings validation ->");

            if (!Directory.Exists(variablePath.networkPath)) Program.Log("Directory " + variablePath.networkPath + " does not exist!");
            if (!Directory.Exists(variablePath.notebookPath)) Program.Log("Directory " + variablePath.notebookPath + " does not exist!");
            if (!File.Exists(variablePath.python)) Program.Log("File " + variablePath.python + " does not exist!");
            if (!File.Exists(variablePath.pyStatScript)) Program.Log("File " + variablePath.pyStatScript + " does not exist!");
            if (!File.Exists(variablePath.chatbot)) Program.Log("File " + variablePath.chatbot + " does not exist!");
            Program.Log("--------------");

            foreach(var item in App.getApplications())
            {
                if (item.Value.exe != "" && !File.Exists(item.Value.exe)) Program.Log("File " + item.Value.exe + " does not exist!");
            }
            Program.Log("");
        }
    }
}
