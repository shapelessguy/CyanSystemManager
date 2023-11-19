using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static CyanSystemManager.Utility;
using NAudio.CoreAudioApi;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;

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
            public static string pyStatScript = documents_path + @"\Workspace PyCharm\University-Basic Projects\Statistics - CyanFusion\Analysis.pyw";
            public static string notebookPath = @"C:\ProgramData\Cyan\Notebook";
            public static string networkPath = @"C:\ProgramData\Cyan\NetworkLogs";
            public static string multiMonitor = @"E:\Software\Controller\MultiMonitorTool\MultiMonitorTool.exe";
        }

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
                    false),
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
                    true, "", "#32770"
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
                    documents_path + "\\Outlook.lnk"
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
                    true, "", "Spotify"
                    ),
                new application(
                    "Microsoft Teams",
                    new string[] { "Microsoft Teams" }, "",
                    app_data_path + "\\Local\\Microsoft\\Teams\\Update.exe",
                    false,
                    " --processStart Teams.exe"
                    ),
                new application(
                    "WhatsApp",
                    new string[] { "WhatsApp" },
                    "WhatsApp",
                    @"",
                    false
                    ),
                new application(
                    "NordVPN",
                    new string[] { "NordVPN" },
                    "NordVPN",
                    @"",
                    false
                    ),
                new application(
                    "DeepL",
                    new string[] { "DeepL" },
                    "DeepL",
                    app_data_path + "\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\DeepL.lnk",
                    true,
                    "run --no-wait https://appdownload.deepl.com/windows/0install/deepl.xml"
                    ),
                new application(
                    "ChatGPT",
                    new string[] { "ChatGPT" },
                    "ChatGPT",
                    documents_path + "\\Workspace PyCharm\\miscellaneous\\OpenAI\\ChatGPT.lnk"
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
                    prog86_path + "\\DisplayFusion\\DisplayFusion.exe"
                    ),
                new application(
                    "Discord",
                    new string[] { "Discord" },
                    "discord",
                    app_data_path + "\\Local\\Discord\\Update.exe",
                    true
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

        public static List<AudioDevice> audioDevices = new List<AudioDevice>()
        {
            new AudioDevice(AT.None),
            new AudioDevice(AT.Primary, new string[] { "Speakers (Realtek(R) Audio)" }),
            new AudioDevice(AT.Headphones, new string[] { "Headphones (Soundcore Life Q30 Stereo)" }),
            new AudioDevice(AT.Secondary, new string[] { "SAMSUNG (NVIDIA High Definition Audio)" }),
            new AudioDevice(AT.Third, new string[] { "19LS4D-ZB" }),
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
            new MyMonitor(4, "SAM71B4"),
        };

        public static void apply()
        {
            for (int id = 0; id < activityList.Count; id++) activityList[id].id = id;
            if (!Directory.Exists(variablePath.notebookPath)) Directory.CreateDirectory(variablePath.notebookPath);
            if (!Directory.Exists(variablePath.networkPath)) Directory.CreateDirectory(variablePath.networkPath);
            if (!File.Exists(variablePath.python) || !File.Exists(variablePath.pyStatScript)) noStat = true;

            if (MonitorManager.allMonitors.Count == 0)
            {
                List<Monitor> all_monitors_ = MonitorManager.getMonitorConfiguration();
                if (MonitorManager.allMonitors.Count == 0) MonitorManager.allMonitors = all_monitors_;
            }

            Console.WriteLine("\nMonitors ->");
            foreach (var monitor in MonitorManager.allMonitors) { monitor.Print(); }

            Console.WriteLine("\nAudio devices ->");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)) Console.WriteLine(endpoint.FriendlyName);

            Console.WriteLine("\nSettings validation ->");

            if (!Directory.Exists(variablePath.networkPath)) Console.WriteLine("Directory " + variablePath.networkPath + " does not exist!");
            if (!Directory.Exists(variablePath.notebookPath)) Console.WriteLine("Directory " + variablePath.notebookPath + " does not exist!");
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
