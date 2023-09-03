using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            while (!timeToClose && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(25);
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Console.WriteLine("Exception in " + title); }
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
            Console.WriteLine("Starting " + title + "..");

            beforeStart();
            new Thread(threadRun).Start();
            status = State.ON;
        }
        static public void beforeStart() { }
        static public void stopService()
        {
            Console.WriteLine(title + " stopped");
            status = State.OFF;
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
        }
        // Inside functions
        public static int[] startUp()
        {
            int nOp = 0, nTot = 0;

            Dictionary<string, application> dict = App.getApplications();
            List<application> toStart = new List<application>();
            foreach (var app in dict.Values.ToArray())
                if (app.start) toStart.Add(app);

            foreach (var app in toStart)
                nOp += TryToOpen(app.exe, app.proc_name, app.info); nTot += 1;

            return new int[] { nOp, nTot };
        }
        public static int TryToOpen(string path, string processName = "", string info = "", bool admin = false)
        {
            try
            {
                if (processName != "" && Process.GetProcessesByName(processName).Length > 0) return 0;
                if (admin) RunAsAdmin.Start(path, info);
                else Process.Start(path, info);
                return 1;
            }
            catch (Exception) { return 0; }
        }
        static private void Start(object args)
        {
            Console.WriteLine("Starting system initialization.");
            int[] done_over_all = startUp();
            if (done_over_all[0] < done_over_all[1]/2) return;
            Thread.Sleep(10 * 1000);
            Service_Audio.SetMasterVolume(0.18f);
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
            int wait = 100;
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
