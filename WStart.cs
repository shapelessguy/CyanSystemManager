using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static CyanSystemManager.Settings;
using HWND = System.IntPtr;

namespace CyanSystemManager
{
    class WStart
    {
        public static List<Window> Order_Win = new List<Window>();
        public static IntPtr[] win_id_calendar = new IntPtr[2];
        public static Dictionary<string[], Monitor> browsers = new Dictionary<string[], Monitor>();
        public static int tempo_avvio = 25000;
        public static Dictionary<float, float> AudioLevels = new Dictionary<float, float>();
        static string[] browserToLocate = null;


        public static int[] startUp()
        {
            int nOp = 0, nTot = 0;

            Dictionary<string, application> dict = App.getApplications();
            List<application> toStart = new List<application>();
            foreach (var app in dict.Values.ToArray())
                if (app.start) toStart.Add(app);

            foreach(var app in toStart) 
                nOp += TryToOpen(app.exe, app.proc_name, app.info); nTot += 1;

            return new int[] { nOp, nTot };
        }

        public static void defBrowsers()
        {
            browsers.Clear();
            browsers.Add(App.chrome.win, MonitorManager.Ref(2));
            browsers.Add(App.firefox.win, MonitorManager.Ref(3));
        }

        public static int TryToOpen(string path, string processName = "", string info="", bool admin = false)
        {
            try
            {
                if (processName != "" && Process.GetProcessesByName(processName).Length > 0) return 0;
                if (admin) RunAsAdmin.Start(path);
                else Process.Start(path, info);
                return 1;
            }
            catch (Exception) { return 0; }
        }
        private static void SetStartup()
        {
            //string AppName = "CyanSystemManager";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            //rk.SetValue(AppName, Application.ExecutablePath);
            //rk.DeleteValue(AppName, false);

        }
        public static void OrderWin(int nTimes=1) {for (int i = 0; i < nTimes; i++) OrderWin();}

        public static IDictionary<HWND, string> OpenWindows;
        public static Screen smallMonitor;
        public static Screen mediumMonitor;
        public static Screen bigMonitor;
        public static void OrderWin()
        {
            OpenWindows = WindowWrapper.GetOpenWindows();
            //foreach (var vari in OpenWindows) Console.WriteLine(vari.Value);
            Order_Win.Clear();
            MonitorManager.GetMonitors();
            smallMonitor = MonitorManager.Ref(3).screen;
            mediumMonitor = MonitorManager.Ref(1).screen;
            bigMonitor = MonitorManager.Ref(2).screen;

            defBrowsers();
            if(bigMonitor != null && mediumMonitor != null && smallMonitor != null)
            {
                if (bigMonitor.Bounds.Location.X == 0 && bigMonitor.Bounds.Location.Y == 0) order1();
                else order2();
            }
            else if (mediumMonitor != null && smallMonitor != null)
            {
                order2();
            }
        }
        private static void order0()
        {
            Console.WriteLine("___0");
            int verticalSet = 955;
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == smallMonitor) browserToLocate = key;
            if (browserToLocate != null)
                Order_Win.Add(new Window(OpenWindows, browserToLocate, "", smallMonitor, 0, 0, verticalSet, 739));
            Order_Win.Add(new Window(OpenWindows, App.moneyguardW.win, "", smallMonitor, verticalSet + 25, 2));

            //Order_Win.Add(new Window(OpenWindows, App.netmeterEvo.win, "", smallMonitor, verticalSet - 5, 128));

            Order_Win.Add(new Window(win_id_calendar[0], mediumMonitor, mediumMonitor.Bounds.Width - 315, 0));
            Order_Win.Add(new Window(win_id_calendar[1], mediumMonitor, mediumMonitor.Bounds.Width - 314, 240));
            Point clockLocation = new Point(smallMonitor.WorkingArea.Width - 180, 120);

            Rectangle HWBound = new Rectangle(verticalSet - 5 - 8, 317, smallMonitor.WorkingArea.Width - verticalSet + 20, 419);
            //Order_Win.Add(new Window(OpenWindows, App.HW2.win, "", smallMonitor, HWBound));
            //Order_Win.Add(new Window(OpenWindows, App.clockX.win, "#32770", smallMonitor, clockLocation));

            int[] verticalS = new int[] { 1360, 2600 };
            int[] horizontalS = new int[] { 800, 980, 980+376, 1200, 1200+0, 800+600};
            int width = bigMonitor.WorkingArea.Width;
            int height = bigMonitor.WorkingArea.Height;

            Rectangle bound1 = new Rectangle(0, 0, verticalS[0], horizontalS[0]);
            Rectangle bound1_2 = new Rectangle(0, horizontalS[0], verticalS[0], horizontalS[5] - horizontalS[0]);
            Rectangle bound2 = new Rectangle(0, horizontalS[5], verticalS[0], height - horizontalS[5]);
            Rectangle bound3 = new Rectangle(verticalS[0], 0, verticalS[1] - verticalS[0], horizontalS[1]);
            Rectangle bound3_4 = new Rectangle(verticalS[0], horizontalS[1], verticalS[1] - verticalS[0],
                                                horizontalS[2] - horizontalS[1]);
            Rectangle bound4 = new Rectangle(verticalS[0], horizontalS[2], verticalS[1] - verticalS[0],
                                                height - horizontalS[2]);
            Rectangle bound5 = new Rectangle(verticalS[1], 0, width - verticalS[1],
                                                horizontalS[3]);
            Rectangle bound5_6 = new Rectangle(verticalS[1], horizontalS[3], width - verticalS[1],
                                                horizontalS[4] - horizontalS[3]);
            Rectangle bound6 = new Rectangle(verticalS[1], horizontalS[4], width - verticalS[1],
                                                height - horizontalS[4]);
            Rectangle boundSpecial1 = new Rectangle(0, 0, verticalS[1], horizontalS[1]);
            Size size = new Size((int)(width * 0.6), (int)(height * 0.6));
            Rectangle boundSpecial2 = new Rectangle((width - size.Width) / 2, (height - size.Height) / 2,
                size.Width, size.Height);

            if (bigMonitor != null)
            {
                bound1 = WindowWrapper.addMarginsTo(bound1, 6, 6, 0, 7);
                bound1 = WindowWrapper.addMarginsTo(bound1, 0, 2, 0, 0);
                bound5 = WindowWrapper.addMarginsTo(bound5, 8, 8, 0, 8);
                Order_Win.Add(new Window(OpenWindows, App.posta.win, "", bigMonitor, bound1));
                Order_Win.Add(new Window(OpenWindows, App.calendario.win, "", bigMonitor, bound1));
                Order_Win.Add(new Window(OpenWindows, App.discord.win, "", bigMonitor, bound1_2));
                Order_Win.Add(new Window(OpenWindows, App.spotify.win, "", bigMonitor, bound4));
                Order_Win.Add(new Window(OpenWindows, App.teams.win, "", bigMonitor, bound3, new string[] { "Riunione" }));
                Order_Win.Add(new Window(OpenWindows, App.whatsapp.win, "", bigMonitor, bound2));
                Order_Win.Add(new Window(OpenWindows, new string[] { "Steam" }, "", bigMonitor, bound5));
                Order_Win.Add(new Window(OpenWindows, App.thunderbird.win, "", bigMonitor, bound5));
                bound3_4 = WindowWrapper.addMarginsTo(bound3_4, 8,8,0,8);
                Order_Win.Add(new Window(OpenWindows, new string[] { "MSI Afterburner", "grafici" }, "", bigMonitor, bound5_6));
                Order_Win.Add(new Window(OpenWindows, App.HW1.win, "", bigMonitor, bound3_4));
                Order_Win.Add(new Window(OpenWindows, App.telegram.win, "", bigMonitor, bound6));
                Order_Win.Add(new Window(OpenWindows, App.cyanTabata.win, "", bigMonitor, boundSpecial1));
                Order_Win.Add(new Window(OpenWindows, App.cyanVideos.win, "", bigMonitor, boundSpecial2));
            }
            WindowWrapper.ShowNormalWin(OpenWindows, new string[] { "MSI Afterburner", "grafici" }, "");
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == bigMonitor) browserToLocate = key;
            if (browserToLocate != null)
            {
                Window win = new Window(OpenWindows, browserToLocate, "");
                Rectangle rect = bigMonitor.WorkingArea;
                rect.Width -= 20; rect.Height -= 20; rect.X += 20; rect.Y += 20;
                if (rect.Contains(new Point(win.x, win.y)))
                {
                    WindowWrapper.ShowNormalWin(OpenWindows, browserToLocate, "");
                    bound4 = WindowWrapper.addMarginsTo(bound4, -20, -20, 180, -70);
                    Order_Win.Add(new Window(OpenWindows, browserToLocate, "", bigMonitor, bound4));
                    WindowWrapper.FocusWin(OpenWindows, browserToLocate, "", 10);
                }
            }

            WindowWrapper.FocusWin(OpenWindows, App.cyanTabata.win, "", 10);
            WindowWrapper.FocusWin(OpenWindows, App.cyanVideos.win, "", 10);
        }
        private static void order1()
        {
            Console.WriteLine("___1");
            int verticalSet = 955;
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == smallMonitor) browserToLocate = key;
            if (browserToLocate != null)
                Order_Win.Add(new Window(OpenWindows, browserToLocate, "", smallMonitor, 0, 0, verticalSet, 739));

            //Order_Win.Add(new Window(OpenWindows, new string[] { "WidgetMoneyguard" }, "", smallMonitor, verticalSet + 25, 2));

            //Order_Win.Add(new Window(OpenWindows, new string[] { "NetMeter Evo" }, "", smallMonitor, verticalSet - 5, 128));

            Rectangle afterB = new Rectangle(verticalSet - 5 - 8, 317, smallMonitor.WorkingArea.Width - verticalSet + 20, 419);
            //Order_Win.Add(new Window(OpenWindows, new string[] { "MSI Afterburner", "grafici" }, "", smallMonitor,afterB));
            Rectangle HWBound = new Rectangle(verticalSet - 5 - 8, 317, smallMonitor.WorkingArea.Width - verticalSet + 20, 419);
            //Order_Win.Add(new Window(OpenWindows, new string[] { "HW Monitoring Charts(2)" }, "", smallMonitor, HWBound));

            //Order_Win.Add(new Window(OpenWindows, new string[] { "FormWeather" }, "", smallMonitor, verticalSet + 1, 128));
            Order_Win.Add(new Window(win_id_calendar[0], mediumMonitor, mediumMonitor.Bounds.Width - 315, 0));
            Order_Win.Add(new Window(win_id_calendar[1], mediumMonitor, mediumMonitor.Bounds.Width - 314, 240));
            Point clockLocation = new Point(smallMonitor.WorkingArea.Width - 180, 120);
            Order_Win.Add(new Window(OpenWindows, new string[] { "ClocX" }, "#32770", smallMonitor, clockLocation));

            float p_wa = 0.55f;
            Rectangle whatsAppBounds = new Rectangle(0,0,(int)(mediumMonitor.WorkingArea.Width * p_wa), mediumMonitor.WorkingArea.Height);
            Order_Win.Add(new Window(OpenWindows, new string[] { "WhatsApp" }, "", smallMonitor, whatsAppBounds));

            Rectangle telegramBounds = new Rectangle(whatsAppBounds.Width, 0,
                                                        (int)(mediumMonitor.WorkingArea.Width * (1-p_wa)), mediumMonitor.WorkingArea.Height);
            Order_Win.Add(new Window(OpenWindows, new string[] { "Telegram" }, "", smallMonitor, telegramBounds));

            int[] verticalS = new int[] { 1300, 2600 };
            int[] horizontalS = new int[] { 1150, 1300, 900, 900+294, 1150+376 };

            if (bigMonitor != null)
            {
                Rectangle bound1 = new Rectangle(0, 0, verticalS[0], horizontalS[0]);
                Rectangle bound1_2 = new Rectangle(0, horizontalS[0], verticalS[0], horizontalS[5] - horizontalS[0]);
                Rectangle bound2 = new Rectangle(0, horizontalS[5], verticalS[0], bigMonitor.WorkingArea.Height - horizontalS[5]);
                Rectangle bound3 = new Rectangle(verticalS[0], 0, verticalS[1] - verticalS[0], horizontalS[1]);
                Rectangle bound3_4 = new Rectangle(verticalS[0], 0, verticalS[1] - verticalS[0], horizontalS[1]);
                Rectangle bound4 = new Rectangle(verticalS[0], horizontalS[1], verticalS[1] - verticalS[0],
                                                    bigMonitor.WorkingArea.Height - horizontalS[1]);
                Rectangle bound5 = new Rectangle(verticalS[1], 0, bigMonitor.WorkingArea.Width - verticalS[1],
                                                    horizontalS[2]);
                Rectangle bound6 = new Rectangle(verticalS[1], horizontalS[2], bigMonitor.WorkingArea.Width - verticalS[1],
                                                    bigMonitor.WorkingArea.Height - horizontalS[2]);
                Rectangle boundSpecial1 = new Rectangle(0, 0, verticalS[1], horizontalS[1]);


                bound1 = WindowWrapper.addMarginsTo(bound1, 6, 6, 0, 6);
                bound5 = WindowWrapper.addMarginsTo(bound5, 8, 8, 0, 8);
                Order_Win.Add(new Window(OpenWindows, new string[] { "- Posta" }, "", bigMonitor, bound1));
                Order_Win.Add(new Window(OpenWindows, new string[] { "Spotify", "Free" }, "", bigMonitor, bound2));
                Order_Win.Add(new Window(OpenWindows, new string[] { "Microsoft Teams" }, "", bigMonitor, bound3));
                //Order_Win.Add(new Window(OpenWindows, new string[] { "Steam" }, "", bigMonitor, bound5));
                Order_Win.Add(new Window(OpenWindows, new string[] { "- Calendario" }, "", bigMonitor, bound5));
                Order_Win.Add(new Window(OpenWindows, new string[] { "Cyan Tabata" }, "", bigMonitor, boundSpecial1));
            }

            WindowWrapper.ShowNormalWin(OpenWindows, new string[] { "MSI Afterburner", "grafici" }, "");

            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == bigMonitor) browserToLocate = key;
            if (browserToLocate != null) WindowWrapper.ShowNormalWin(OpenWindows, browserToLocate, "");

            WindowWrapper.FocusWin(OpenWindows, new string[] { "Cyan Tabata" }, "", 10);
        }

        private static void order2()
        {
            Console.WriteLine("___2");
            //int verticalSet = 955;
            browserToLocate = null;
            foreach (string[] key in browsers.Keys) if (browsers[key].screen == smallMonitor) browserToLocate = key;

            if (browserToLocate != null)
                Order_Win.Add(new Window(OpenWindows, browserToLocate, "", mediumMonitor, 0, 0, mediumMonitor.WorkingArea.Width, mediumMonitor.WorkingArea.Height));

            //Order_Win.Add(new Window(OpenWindows, App.moneyguardW.win, "", smallMonitor, verticalSet + 25, 2));

            //Order_Win.Add(new Window(OpenWindows, App.netmeterEvo.win, "", smallMonitor, verticalSet - 5, 0));

            //Rectangle afterB = new Rectangle(verticalSet - 5 - 8, 317, smallMonitor.WorkingArea.Width - verticalSet + 20, 419);
            //Rectangle HWBound = new Rectangle(verticalSet - 5 - 8, 190, smallMonitor.WorkingArea.Width - verticalSet + 20, 419 + 120);
            //Order_Win.Add(new Window(OpenWindows, App.HW2.win, "", smallMonitor, HWBound));

            //Order_Win.Add(new Window(win_id_calendar[0], mediumMonitor, mediumMonitor.Bounds.Width - 315, 0));

            //Order_Win.Add(new Window(win_id_calendar[1], mediumMonitor, mediumMonitor.Bounds.Width - 314, 240));

            //Point clockLocation = new Point(smallMonitor.WorkingArea.Width - 180, 10);
            //Order_Win.Add(new Window(OpenWindows, App.clockX.win, "#32770", smallMonitor, clockLocation));


            float p_wa = 0.56f;
            Rectangle whatsAppBounds = new Rectangle(0, 0, (int)(smallMonitor.WorkingArea.Width * p_wa), smallMonitor.WorkingArea.Height);
            Order_Win.Add(new Window(OpenWindows, App.whatsapp.win, "", smallMonitor, whatsAppBounds));

            Rectangle telegramBounds = new Rectangle(whatsAppBounds.Width, 0,
                                                        (int)(smallMonitor.WorkingArea.Width * (1 - p_wa)) + 1, smallMonitor.WorkingArea.Height);
            Order_Win.Add(new Window(OpenWindows, App.telegram.win, "", smallMonitor, telegramBounds));

            int[] verticalS = new int[] { 1300, 2600 };
            int[] horizontalS = new int[] { 1150, 1300, 900, 900 + 294, 1150 + 376 };

            if (browserToLocate != null) WindowWrapper.MaximizeWin(OpenWindows, browserToLocate, "");

            WindowWrapper.FocusWin(OpenWindows, App.cyanTabata.win, "", 10);
        }

    }
}
