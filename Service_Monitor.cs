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
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace CyanSystemManager
{
    public static class Service_Monitor
    {
        static public State status = State.OFF;
        static public bool clear;
        static public int n_monitors;

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
            n_monitors = -1;
            TurnOn();
            while (!forceTermination && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(25);
                    counter++;
                    if (counter == 40) { 
                        counter = 0;
                        if (Screen.AllScreens.Length != n_monitors)
                        {
                            MonitorManager.allMonitors = MonitorManager.getMonitorConfiguration();
                        }
                    }
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    if (command.type == MonitorCom.CENTRALIZE) Centralize();
                    else if (command.type == MonitorCom.SORT) Sort();
                    else if (command.type == MonitorCom.TURN_ON_MONITORS) TurnOn();
                    else if (command.type == MonitorCom.SHUT_DOWN_MONITORS) TurnOff();
                } 
                catch (Exception) { Log("Exception in monitorRun"); }
            }

        }
        public static void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(ST.Monitors); // register Hotkeys needed by Example_ activities
            Log("Starting monitorService..");
            MonitorManager.GetMonitors();

            new Thread(threadRun).Start();
            status = State.ON;
        }
        public static void stopService(bool dispose)
        {
            Log("monitorService stopped");
            status = State.OFF;
            Home.unregisterHotkeys(ST.Monitors);
            commands.Clear();
            clear = true;
        }
        private static void execProfile(string command)
        {
            cmdAsync(Program.multimonitor_path, command);
            if (commands.Count > 1) for (int i = commands.Count - 1; i > 0; i--) commands.RemoveAt(i);
        }
        private static void Centralize()
        {
            Log("1");
            string command = " /MoveWindow Primary All";
            execProfile(command);
        }
        private static void Sort()
        {
            SortWindows.OrderWin(1);
        }

        private static void TurnOn()
        {
            string command = " /TurnOn " + MonitorManager.Ref(VT.Primary).screen.DeviceName + " "
                                          + MonitorManager.Ref(VT.Ausiliary1).screen.DeviceName + " "
                                          + MonitorManager.Ref(VT.Ausiliary2).screen.DeviceName + " ";
            execProfile(command);
        }

        private static void TurnOff()
        {
            string command = " /TurnOff " + MonitorManager.Ref(VT.Primary).screen.DeviceName + " "
                                          + MonitorManager.Ref(VT.Ausiliary1).screen.DeviceName + " "
                                          + MonitorManager.Ref(VT.Ausiliary2).screen.DeviceName + " ";
            execProfile(command);
        }
    }

    class WinCol
    {
        List<Win> collection = new List<Win>();
        IDictionary<System.IntPtr, string> openWin = null;
        public WinCol(IDictionary<System.IntPtr, string> openWin) 
        {
            this.openWin = openWin;
        } 

        public void addWin(application application, Screen monitor, int x, int y, int width, int height)
        {
            this.collection.Add(new Win(application, monitor, x, y, width, height));
        }

        public void SortNow()
        {
            foreach (Win win in this.collection)
            {
                new Window(this.openWin, win.app, win.monitor, win.x, win.y, win.width, win.height);
            }
        }
    }

    class Win
    {
        public application app = null;
        public string[] win = null;
        public Screen monitor = null;
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;
        public Win(application application, Screen monitor, int x, int y, int width, int height)
        {
            this.app = application;
            this.win = application.win;
            this.monitor = monitor;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    class SortWindows
    {
        public static Dictionary<string[], MyMonitor> browsers = new Dictionary<string[], MyMonitor>();
        public static int tempo_avvio = 25000;
        public static Dictionary<float, float> AudioLevels = new Dictionary<float, float>();
        static string[] browserToLocate;

        public static void defBrowsers()
        {
            browsers.Clear();
            // browsers.Add(App.chrome.win, MonitorManager.Ref(VT.Ausiliary1));
            browsers.Add(App.getApp("Opera").win, MonitorManager.Ref(VT.Primary));
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

            monitor1 = MonitorManager.Ref(VT.Primary).screen; // principal monitor
            monitor2 = MonitorManager.Ref(VT.Ausiliary1).screen; // secondary left
            monitor3 = MonitorManager.Ref(VT.Ausiliary2).screen; // secondary right
            monitor_cin = MonitorManager.Ref(VT.Cinema).screen; // cinema monitor

            Log("\nMonitors detected ->");
            if (monitor1 != null) Log("monitor 1 (" + MonitorManager.Ref(VT.Primary).id + "): " + monitor1.Bounds);
            else { Log("monitor1 not found"); }
            if (monitor2 != null) Log("monitor 2 (" + MonitorManager.Ref(VT.Ausiliary1).id + "): " + monitor2.Bounds);
            else { Log("monitor2 not found"); }
            if (monitor3 != null) Log("monitor 3 (" + MonitorManager.Ref(VT.Ausiliary2).id + "): " + monitor3.Bounds);
            else { Log("monitor3 not found"); }
            if (monitor_cin != null) Log("monitor 4 (" + MonitorManager.Ref(VT.Cinema).id + "): " + monitor_cin.Bounds);
            else { Log("monitor4 not found"); }

            defBrowsers();
            bool primaryProfile = monitor1 != null && monitor2 != null && monitor3 != null;
            bool cinemaProfile = monitor1 == null && monitor2 == null && monitor3 == null && monitor_cin != null;

            if (primaryProfile) order_primary();
            else if (cinemaProfile) order_cinema();
        }
        private static void order_primary()
        {
            Log("Primary ordering!\n");
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == monitor1) browserToLocate = key;
            WinCol col = new WinCol(OpenWindows);

            foreach(WinSet winset in ControlPanel.winSets)
            {
                if (winset.monitor != null && winset.enabled)
                {
                    col.addWin(winset.app, winset.monitor.screen, winset.location.X, winset.location.Y, winset.size.Width, winset.size.Height);
                }
            }
            /*col.addWin(App.outlook_new, monitor2, 500, 0, 1112, 1040);
            col.addWin(App.outlook, monitor2, 0, 0, 1112, 1040);
            col.addWin(App.msiG, monitor2, 0, -1, 822, 430);
            col.addWin(App.chatGPT, monitor2, 1105, 421, 822, 626);
            col.addWin(App.whatsapp, monitor3, -7, 0, 730, 1046);
            col.addWin(App.netmeterEvo, monitor3, 716, 0, 590, 220);
            col.addWin(App.spotify, monitor3, 716, 220, 1, 820);
            col.addWin(App.clockX, monitor3, 1306, 8, 200, 200);
            col.addWin(App.skypeFB, monitor3, 1517, 0, 1, 1040); */
            col.SortNow();
            WindowWrapper.CloseWin(OpenWindows, App.getApp("NordVPN"));
            WindowWrapper.CloseWin(OpenWindows, App.getApp("DeepL"));
        }
        private static void order_cinema()
        {
            Log("Cinema ordering!\n");
        }

    }
}
