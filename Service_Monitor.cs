using System;
using System.Collections.Generic;
using System.Threading;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.IO;
using System.Windows.Forms;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using Vanara.PInvoke;

namespace CyanSystemManager
{
    public static class Service_Monitor
    {
        static public State status = State.OFF;
        static public int n_monitors = -1;

        public static void function(string conf) { addCommand(conf); }
        public static void sort() { addCommand(MonitorCom.SORT); }

        static List<Command> commands = new List<Command>();
        private static void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }

        public static void threadRun()
        {
            int counter = 0;
            TurnOn();
            while (!timeToClose && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(25);
                    counter++;
                    if (counter == 40) { 
                        counter = 0;
                        if (Screen.AllScreens.Length != n_monitors) { MonitorManager.getMonitorConfiguration(); }
                    }
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    if (command.type == MonitorCom.CENTRALIZE) Centralize();
                    else if (command.type == MonitorCom.SORT) Sort();
                    else if (command.type == MonitorCom.TURN_ON_MONITORS) TurnOn();
                    else if (command.type == MonitorCom.SHUT_DOWN_MONITORS) TurnOff();
                } 
                catch (Exception) { Console.WriteLine("Exception in monitorRun"); }
            }

        }
        public static void startService()
        {
            status = State.NEUTRAL;
            if (!File.Exists(variablePath.displayFusion)) MessageBox.Show(variablePath.displayFusion + " not found");
            Home.registerHotkeys(ST.Monitors); // register Hotkeys needed by Example_ activities
            Console.WriteLine("Starting monitorService..");
            MonitorManager.GetMonitors();

            new Thread(threadRun).Start();
            status = State.ON;
        }
        public static void stopService()
        {
            Console.WriteLine("monitorService stopped");
            status = State.OFF;
            Home.unregisterHotkeys(ST.Monitors);
            commands.Clear();
        }
        private static void execProfile(string command)
        {
            cmdAsync(variablePath.multiMonitor, command);
            Console.WriteLine(command);

            // Thread.Sleep(4000); Sort();
            if (commands.Count > 1) for (int i = commands.Count - 1; i > 0; i--) commands.RemoveAt(i);
        }
        private static void Centralize()
        {
            Console.WriteLine("1");
            string command = " /MoveWindow Primary All";
            execProfile(command);
        }
        private static void Sort()
        {
            SortWindows.OrderWin(1);
        }

        private static void TurnOn()
        {
            string command = " /TurnOn " + MonitorManager.Ref(1).screen.DeviceName + " "
                                          + MonitorManager.Ref(2).screen.DeviceName + " "
                                          + MonitorManager.Ref(3).screen.DeviceName + " ";
            execProfile(command);
        }

        private static void TurnOff()
        {
            string command = " /TurnOff " + MonitorManager.Ref(1).screen.DeviceName + " "
                                          + MonitorManager.Ref(2).screen.DeviceName + " "
                                          + MonitorManager.Ref(3).screen.DeviceName + " ";
            execProfile(command);
        }
    }

    class SortWindows
    {
        public static List<Window> Order_Win = new List<Window>();
        public static Dictionary<string[], MyMonitor> browsers = new Dictionary<string[], MyMonitor>();
        public static int tempo_avvio = 25000;
        public static Dictionary<float, float> AudioLevels = new Dictionary<float, float>();
        static string[] browserToLocate;

        public static void defBrowsers()
        {
            browsers.Clear();
            // browsers.Add(App.chrome.win, MonitorManager.Ref(2));
            browsers.Add(App.opera.win, MonitorManager.Ref(1));
        }

        public static void OrderWin(int nTimes = 1) { for (int i = 0; i < nTimes; i++) OrderWin(); }

        public static IDictionary<System.IntPtr, string> OpenWindows;
        public static Screen monitor1;
        public static Screen monitor2;
        public static Screen monitor3;
        public static Screen monitor_cin;
        public static void OrderWin()
        {
            OpenWindows = WindowWrapper.GetOpenWindows();
            Order_Win.Clear();

            monitor1 = MonitorManager.Ref(1).screen; // principal monitor
            monitor2 = MonitorManager.Ref(2).screen; // secondary left
            monitor3 = MonitorManager.Ref(3).screen; // secondary right
            monitor_cin = MonitorManager.Ref(4).screen; // cinema monitor

            Console.WriteLine("\nMonitors detected ->");
            if (monitor1 != null) Console.WriteLine("monitor 1 (" + MonitorManager.Ref(1).deviceName + "): " + monitor1.Bounds);
            else { Console.WriteLine("monitor1 not found"); }
            if (monitor2 != null) Console.WriteLine("monitor 2 (" + MonitorManager.Ref(2).deviceName + "): " + monitor2.Bounds);
            else { Console.WriteLine("monitor2 not found"); }
            if (monitor3 != null) Console.WriteLine("monitor 3 (" + MonitorManager.Ref(3).deviceName + "): " + monitor3.Bounds);
            else { Console.WriteLine("monitor3 not found"); }
            if (monitor_cin != null) Console.WriteLine("monitor 4 (" + MonitorManager.Ref(4).deviceName + "): " + monitor_cin.Bounds);
            else { Console.WriteLine("monitor4 not found"); }

            defBrowsers();
            bool primaryProfile = monitor1 != null && monitor2 != null && monitor3 != null;
            bool cinemaProfile = monitor1 == null && monitor2 == null && monitor3 == null && monitor_cin != null;

            if (primaryProfile) order_primary();
            else if (cinemaProfile) order_cinema();
        }
        private static void order_primary()
        {
            Console.WriteLine("Primary ordering!\n");
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == monitor1) browserToLocate = key;
            // if (browserToLocate != null) Order_Win.Add(new Window(OpenWindows, browserToLocate, "", monitor1, 150, 80, 1600, 1000));

            Order_Win.Add(new Window(OpenWindows, App.outlook.win, "", monitor2, 0, 0, 1112, 1040));
            Order_Win.Add(new Window(OpenWindows, new string[] { "MSI Afterburner", "hardware monitor" }, "", monitor2, 1105, -1, 822, 430));
            Order_Win.Add(new Window(OpenWindows, App.chatGPT.win, "", monitor2, 1105, 421, 822, 626));
            Order_Win.Add(new Window(OpenWindows, App.whatsapp.win, "", monitor3, -7, 0, 730, 1046));
            Order_Win.Add(new Window(OpenWindows, App.netmeterEvo.win, "", monitor3, 716, 0, 590, 220));
            Order_Win.Add(new Window(OpenWindows, new string[] { }, "Spotify", monitor3, 716, 220, 1, 820));
            Order_Win.Add(new Window(OpenWindows, App.clockX.win, "#32770", monitor3, 1306, 8, 200, 200));
            Order_Win.Add(new Window(OpenWindows, App.skype_fb.win, "", monitor3, 1517, 0, 1, 1040));
            WindowWrapper.CloseWin(OpenWindows, App.nordvpn.win, "");
            // WindowWrapper.FocusWin(OpenWindows, browserToLocate, "", 10);
        }
        private static void order_cinema()
        {
            Console.WriteLine("Cinema ordering!\n");
        }

    }
}
