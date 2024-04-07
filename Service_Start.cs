using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Threading;
using Vanara.PInvoke;
using static CyanSystemManager.Program;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    static public class Service_Start
    {
        static public string title = "startService";
        static public string serviceType = ST.Start;
        static public State status = State.OFF;
        static public bool clear;

        // Functions of Example_Service --> they should be called from outside the service
        static public void SystemStart(object args) { addCommand(StartCom.START, args); }

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
                    Thread.Sleep(25);
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception e) { Log("Exception in " + title); Log(e.Message); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == StartCom.START) Start(command.value);
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Log("Starting " + title + "..");

            beforeStart();
            new Thread(threadRun).Start();
            status = State.ON;
        }
        static public void beforeStart() { }
        static public void stopService(bool dispose)
        {
            Log(title + " stopped");
            status = State.OFF;
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        // Inside functions
        public static int[] startUp()
        {
            int nOp = 0, nTot = 0;
            Dictionary<string, application> dict = App.getApplications();
            List<application> toStart = new List<application>();
            IDictionary<IntPtr, string> OpenWindows = WindowWrapper.GetOpenWindows();
            foreach (var app in dict.Values.ToArray())
                if (app.start) toStart.Add(app);

            foreach (var app in toStart)
                nOp += TryToOpen(OpenWindows, app); nTot += 1;

            return new int[] { nOp, nTot };
        }
        public static int TryToOpen(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            try
            {
                if (app.proc_name != "" && Process.GetProcessesByName(app.proc_name).Length > 0)
                {
                    Log("Process '" + app.proc_name + "' already running.");
                    return 0;
                }
                if (Window.getHandle(OpenWindows, app) != IntPtr.Zero)
                {
                    Log("Process '" + app.proc_name + "' already running.");
                    return 0;
                }

                if (app.admin) {
                    try {
                        Log("Executing '" + app.exe + "' as admin user");
                        Process process = new Process()
                        {
                            StartInfo = new ProcessStartInfo(app.exe, app.info)
                            {
                                WindowStyle = ProcessWindowStyle.Normal,
                                WorkingDirectory = Path.GetDirectoryName(app.exe)
                            }
                        };
                        process.Start();
                    }
                    catch { }
                }
                else
                    try
                    {
                        if (Path.GetExtension(app.exe) == ".lnk") throw new Exception();
                        ProcessHelper.RunAsRestrictedUser(app.exe, app.info);
                        Log("'" + app.exe + "' executed as restricted user");
                    }
                    catch
                    {
                        Process.Start("explorer.exe", app.exe);
                        Log("'" + app.exe + "' executed from explorer.exe");
                    }
                return 1;
            }
            catch (Exception) {
                Log("Error while trying to open " + app.exe + ". Admin = " + app.admin);
                return 0; 
            }
        }
        static private void Start(object args)
        {
            Log("Starting system initialization.");
            int[] done_over_all = startUp();
            if (done_over_all[0] < done_over_all[1]/2) return;
            Thread.Sleep(10 * 1000);
            if (Service_Audio.audioInfo.audioDevice.category == AT.Primary) Service_Audio.SetMasterVolume(0.18f);
            Thread.Sleep(10 * 1000);
            SortWindows.OrderWin();
            Thread.Sleep(10 * 1000);
            SortWindows.OrderWin();
            fixSharpness();
            Thread.Sleep(2 * 1000);
            SortWindows.OrderWin();
            commands.Clear();
        }
        static private void fixSharpness()
        {
            IDictionary<IntPtr, string> OpenWindows = WindowWrapper.GetOpenWindows();
            // WindowWrapper.MaximizeWin(OpenWindows, new string[] { "Torrent" }, "");
            // WindowWrapper.MaximizeWin(OpenWindows, new string[] { "Outlook" }, "");
            // WindowWrapper.ShowNormalWin(OpenWindows, new string[] { "Steam" }, "");
            // WindowWrapper.ShowNormalWin(OpenWindows, new string[] { "Google Chrome" }, "");

            for (int i = 0; i < 3; i++) FocusWins(OpenWindows);

            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "CyanVideos" }, "");
            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "Outlook" }, "");
            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "Torrent" }, "");
            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "Discord" }, "");
            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "Teams" }, "");
            // WindowWrapper.MinimizeWin(OpenWindows, new string[] { "Steam" }, "");
        }
        private static void FocusWins(IDictionary<IntPtr, string> OpenWindows)
        {
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "CyanVideos" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "MSI Afterburner", "grafici" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Steam" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Torrent" }, "", wait);

            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Teams" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "WhatsApp" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Telegram" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Spotify" }, "", wait);
            //WindowWrapper.FocusWin(OpenWindows, new string[] { "Discord" }, "", wait);
        }
        // //////////
    }
}
