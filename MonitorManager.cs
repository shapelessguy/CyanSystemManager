using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Tools;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using System.Windows.Forms.VisualStyles;
using Microsoft.Diagnostics.Symbols;
using System.Runtime.InteropServices;
using System.Text;
using Vanara.PInvoke;
using HWND = System.IntPtr;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static Vanara.PInvoke.User32;

namespace CyanSystemManager
{
    public class Monitor
    {
        public Screen screen;
        public string deviceName;
        public string id;
        public int width;
        public int height;
        public int x;
        public int y;
        public Monitor(Screen screen, string deviceName, int width, int height, int x, int y)
        {
            this.screen = screen;
            this.deviceName = deviceName;
            this.width = width;
            this.height = height;
            this.x = x;
            this.y = y;
        }
        public void Print()
        {
            Log(id + "  -> [" + width + " . " + height + "]   p=(" + x + ", " + y + ")");
        }
    }

    public class MyMonitor
    {
        public VT type;
        public string id;
        public Screen screen;
        public Point position;

        public MyMonitor(VT type, string id) { 
            SetMonitor(type, id, new Point(0, 0));
        }

        private void SetMonitor(VT type, string id, Point position)
        {
            this.type = type;
            this.id = id;
            this.position = position;
            UpdateScreen();
        }
        public void UpdateScreen()
        {
            if (MonitorManager.allMonitors.Count == 0)
            {
                List<Monitor> all_monitors_ = MonitorManager.getMonitorConfiguration();
                if (MonitorManager.allMonitors.Count == 0) MonitorManager.allMonitors = all_monitors_;
            }
            List<Screen> candidates = new List<Screen>();
            foreach (Monitor monitor in MonitorManager.allMonitors)
            {
                if (id == monitor.id) candidates.Add(monitor.screen);
            }
            if (candidates.Count == 1)
            {
                screen = candidates[0];
            }
            else
            {
                screen = null;
            }
        }
    }
    public static class MonitorManager
    {
        public static List<Monitor> allMonitors;

        public static void initialize_monitors()
        {
            allMonitors = new List<Monitor>();
        }
        public static void GetMonitors() {
            if (MonitorManager.allMonitors.Count == 0)
            {
                List<Monitor> all_monitors_ = MonitorManager.getMonitorConfiguration();
                if (MonitorManager.allMonitors.Count == 0) MonitorManager.allMonitors = all_monitors_;
            }
            foreach (MyMonitor monitor in monitors) monitor.UpdateScreen();
        }

        public static MyMonitor Ref(VT type)
        {
            foreach (MyMonitor monitor in monitors) { if (monitor.type == type) return monitor; }
            return null;
        }

        public static List<Monitor> getMonitorConfiguration()
        {
            Log("Get Monitor Configuration");
            for (int repeat=0; repeat<10; repeat++)
            {
                try
                {
                    string conf_path = Path.Combine(Path.GetDirectoryName(Program.multimonitor_path), "multimonitor.cfg");
                    string def_conf_path = Path.Combine(Path.GetDirectoryName(Program.multimonitor_path), "defConfig.cfg");
                    try
                    {
                        if (File.Exists(conf_path)) File.Delete(conf_path);
                        Thread.Sleep(100);
                    }
                    catch (Exception) { }

                    cmdAsync("\"" + Program.multimonitor_path + "\"", "/SaveConfig " + "\"" + conf_path + "\"");
                    for (int i = 0; i < 20; i++)
                    {
                        Thread.Sleep(100);
                        if (File.Exists(conf_path)) break;
                    }
                    Thread.Sleep(200);
                    if (File.Exists(conf_path))
                    {
                        try
                        {
                            File.Copy(conf_path, def_conf_path);
                        }
                        catch (IOException)
                        {
                            File.Copy(conf_path, def_conf_path, true);
                        }
                    }
                    else conf_path = def_conf_path;

                    List<List<string>> monitors = new List<List<string>>();
                    string[] lines = new string[] { };
                    for (int i = 0; i < 20; i++)
                    {
                        try { lines = File.ReadAllLines(conf_path); break; }
                        catch (Exception) { Thread.Sleep(100); }
                    }
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("[")) monitors.Add(new List<string>());
                        else
                        {
                            if (monitors.Count > 0) monitors[monitors.Count - 1].Add(line.Substring(line.IndexOf("=") + 1));
                        }
                    }
                    List<Monitor> temp_monitors = new List<Monitor>();
                    foreach (var monitor in monitors)
                    {
                        string deviceName = monitor[1].Split('\\')[1];
                        int width, height, x, y;
                        int.TryParse(monitor[3], out width);
                        int.TryParse(monitor[4], out height);
                        int.TryParse(monitor[8], out x);
                        int.TryParse(monitor[9], out y);
                        Screen screen = null;
                        foreach (var sc in Screen.AllScreens)
                        {
                            if (sc.DeviceName == monitor[0]) screen = sc;
                        }
                        temp_monitors.Add(new Monitor(screen, deviceName, width, height, x, y));
                    }
                    temp_monitors = temp_monitors.OrderBy(p => p.x).ToList();
                    Dictionary<string, int> name_freq = new Dictionary<string, int>();
                    foreach (Monitor monitor in temp_monitors)
                    {
                        if (!name_freq.ContainsKey(monitor.deviceName))
                        {
                            name_freq[monitor.deviceName] = 1;
                            monitor.id = monitor.deviceName;
                        }
                        else
                        {
                            name_freq[monitor.deviceName] += 1;
                            monitor.id = monitor.deviceName + "(" + name_freq[monitor.deviceName] + ")";
                        }
                    }
                    Service_Monitor.n_monitors = Screen.AllScreens.Length;
                    return temp_monitors;
                }
                catch (Exception) { continue; }
            }
            throw new Exception("Error while fetching monitor configuration.");
        }
    }

    public class Position
    {
        public int id;
        public Point point = new Point(0, 0);
        public Position(int id, Point point)
        {
            this.id = id;
            this.point = point;
        }
        public Position(int id) {this.id = id;}
    }


    public class Configuration
    {
        public List<MyMonitor> monitorList = new List<MyMonitor>();
        public List<Position> positions = new List<Position>();
        public string profile;
        public Configuration(string profile)
        {
            this.profile = profile;
        }
    }

    public class Window
    {
        public bool validate = false;
        public IntPtr handle;
        public int x;
        public int y;
        public int width;
        public int height;
        public static long last_update = 0;

        public Window(IntPtr handle, bool getText = true)
        {
            FindWindow(handle, getText);
        }
        public Window(IntPtr handle, Screen screen, int x, int y)
        {
            if (screen == null) return;
            FindWindow(handle);
            SetPosition(screen, x, y);
        }
        public Window(IntPtr handle, Screen screen, Point point)
        {
            if (screen == null) return;
            FindWindow(handle);
            SetPosition(screen, point.X, point.Y);
        }
        public Window(IntPtr handle, int width, int height)
        {
            FindWindow(handle);
            SetSize(width, height);
        }
        public Window(IntPtr handle, Size size)
        {
            FindWindow(handle);
            SetSize(size.Width, size.Height);
        }
        public Window(IntPtr handle, Screen screen, Rectangle bounds)
        {
            if (screen == null) return;
            FindWindow(handle);
            SetPosition(screen, bounds.X, bounds.Y);
            SetSize(bounds.Width, bounds.Height);
        }
        public Window(IntPtr handle, Screen screen, int x, int y, int width, int height)
        {
            if (screen == null) return;
            FindWindow(handle);
            SetPosition(screen, x, y);
            SetSize(width, height);
        }

        public void FindWindow(IntPtr handle, bool getText = true)
        {
            this.handle = handle;
            StringBuilder lpString = new StringBuilder();
            if (getText) GetWindowText(handle, lpString, 100);
            Rectangle lpRect = new Rectangle();
            GetWindowRect(handle, ref lpRect);
            this.x = lpRect.Left;
            this.y = lpRect.Top;
            this.width = lpRect.Right - lpRect.Left - x;
            this.height = lpRect.Bottom - lpRect.Top - y;
            validate = true;

        }

        public Window(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            FindWindow(OpenWindows, app);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, application app, Screen screen, int x, int y)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, app);
            Focus();
            SetPosition(screen, x, y);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, application app, Screen screen, Point point)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, app);
            Focus();
            SetPosition(screen, point.X, point.Y);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, application app, int width, int height)
        {
            FindWindow(OpenWindows, app);
            Focus();
            SetSize(width, height);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, application app, Screen screen, Rectangle bounds)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, app);
            Focus();
            SetPosition(screen, bounds.X, bounds.Y);
            SetSize(bounds.Width, bounds.Height);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, application app, Screen screen, int x, int y, int width, int height)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, app);
            ShowWindow(handle, 1);
            SetPosition(screen, x, y);
            SetSize(width, height);
        }
        public void Focus()
        {
            if (IsIconic(handle))
            {
                ShowWindow(handle, 1);
                SetForegroundWindow(handle);
            }
        }

        public void Minimize()
        {
            if (!IsIconic(handle))
            {
                ShowWindow(handle, 6);
            }
        }
        public void Normal()
        {
            if (!IsIconic(handle))
            {
                ShowWindow(handle, 1);
            }
        }

        public static HWND getHandleByWin(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            HWND handle_ = IntPtr.Zero;
            foreach (KeyValuePair<IntPtr, string> window in OpenWindows)
            {
                bool containsall = true;
                foreach (string stringa in app.win) if (!window.Value.Contains(stringa)) containsall = false;
                if (containsall)
                {
                    handle_ = window.Key;
                    break;
                }
            }
            return handle_;
        }

        public static HWND getHandleByProc(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            HWND handle_ = IntPtr.Zero;
            if ((DateTime.Now.Ticks - last_update) > 10000000 * 1)
            {
                last_update = DateTime.Now.Ticks;
                Program.all_processes = Process.GetProcesses();
            }
            foreach (Process pList in Program.all_processes)
            {
                if (pList.MainWindowTitle != "")
                {
                    if (pList.ProcessName == app.proc_name)
                    {
                        handle_ = pList.MainWindowHandle;
                        break;
                    }
                }
            }
            return handle_;
        }

        public static HWND getHandle(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            HWND handle_ = IntPtr.Zero;
            if (app.win.Length != 0)
            {
                Log("Searching " + app.name + " by windows");
                handle_ = getHandleByWin(OpenWindows, app);
            }
            if (handle_ == IntPtr.Zero)
            {
                Log("Searching " + app.name + " by processes");
                handle_ = getHandleByProc(OpenWindows, app);
            }
            return handle_;
        }

        public void FindWindow(IDictionary<IntPtr, string> OpenWindows, application app)
        {
            handle = getHandle(OpenWindows, app);
            Rectangle lpRect = new Rectangle();
            if (handle == IntPtr.Zero) { validate = false; return; }
            GetWindowRect(handle, ref lpRect);
            x = lpRect.Left;
            y = lpRect.Top;
            width = lpRect.Right - lpRect.Left - x;
            height = lpRect.Bottom - lpRect.Top - y;
            validate = true;
        }

        static public void FindAllWindows(IDictionary<IntPtr, string> OpenWindows, List<string[]> partialnames)
        {

        }

        public void SetPosition(Screen screen, int x, int y)
        {
            Normal();
            if (screen == null) return;
            if (handle == IntPtr.Zero) return;
            SetWindowPos(handle, IntPtr.Zero, screen.Bounds.X + x, screen.Bounds.Y + y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            this.x = x;
            this.y = y;
        }
        public void SetSize(int width, int height)
        {
            Normal();
            if (handle == IntPtr.Zero) return;
            SetWindowPos(handle, IntPtr.Zero, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
            this.width = width;
            this.height = height;
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("User32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_SHOWWINDOW = 0x0040;
        const uint SWP_NOMOVE = 0x0002;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImportAttribute("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImportAttribute("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
    }

    class WindowWrapper
    {

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_SHOWWINDOW = 0x0040;
        const uint SWP_NOMOVE = 0x0002;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;
        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_HIDE = 0;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        public static void SetWinOnTop(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
            SetWindowPos(win.handle, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void FocusWin(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
            SetForegroundWindow(win.handle);
        }
        public static void ShowNormalWin(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
        }
        public static void HideWin(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_HIDE);
        }
        public static void MaximizeWin(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_MAXIMIZE);
        }
        public static void MinimizeWin(application app)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_MINIMIZE);
        }
        public static void SetWinOnTop(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            SetWindowPos(win.handle, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void FocusWin(IDictionary<HWND, string> OpenWindows, application app, int delay = 0)
        {
            try
            {
                Window win = new Window(OpenWindows, app);
                if (IsIconic(win.handle))
                {
                    ShowWindow(win.handle, 1);
                }
                SetForegroundWindow(win.handle);
                if (delay != 0 && win.handle != IntPtr.Zero)
                {
                    int cycles = (int)(delay / 50);
                    for (int i = 0; i < cycles; i++)
                    {
                        System.Threading.Thread.Sleep(50);
                        SetForegroundWindow(win.handle);
                    }
                }
            }
            catch (Exception) { }
        }
        public static void ShowNormalWin(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            
            ShowWindow(win.handle, SW_SHOWNORMAL);
        }
        public static void MaximizeWin(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_MAXIMIZE);
        }
        public static void HideWin(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_HIDE);
        }
        public static void CloseWin(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            SendMessage(win.handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        public static void MinimizeWin(IDictionary<HWND, string> OpenWindows, application app)
        {
            Window win = new Window(OpenWindows, app);
            ShowWindow(win.handle, SW_MINIMIZE);
        }
        public static IDictionary<HWND, string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            foreach (var w in windows.Values)
            {
                Program.Log(w);
            }

            return windows;
        }

        public static Rectangle addMarginsTo(Rectangle rect, int lx, int rx, int uy, int dy)
        {
            rect = new Rectangle(rect.X - lx, rect.Y - uy, rect.Width + lx + rx, rect.Height + uy + dy);
            return rect;
        }
    }
}
