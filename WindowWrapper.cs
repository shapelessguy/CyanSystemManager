using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HWND = System.IntPtr;

namespace CyanSystemManager
{
    class WindowWrapper
    {
        public static IntPtr[] win_id_calendar = new IntPtr[2];

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
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        public static void SetWinOnTop(string[] partials, string class_name)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, partials, class_name);
            SetWindowPos(win.handle, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void FocusWin(string[] partials, string class_name)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, partials, class_name);
            SetForegroundWindow(win.handle);
        }
        public static void ShowNormalWin(string[] partials, string class_name)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, partials, class_name);
            ShowWindow(win.handle, SW_SHOWNORMAL);
        }
        public static void MaximizeWin(string[] partials, string class_name)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, partials, class_name);
            ShowWindow(win.handle, SW_MAXIMIZE);
        }
        public static void MinimizeWin(string[] partials, string class_name)
        {
            IDictionary<HWND, string> OpenWindows = GetOpenWindows();
            Window win = new Window(OpenWindows, partials, class_name);
            ShowWindow(win.handle, SW_MINIMIZE);
        }
        public static void SetWinOnTop(IDictionary<HWND, string> OpenWindows, string[] partials, string class_name)
        {
            Window win = new Window(OpenWindows, partials, class_name);
            SetWindowPos(win.handle, IntPtr.Zero, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void FocusWin(IDictionary<HWND, string> OpenWindows, string[] partials, string class_name, int delay = 0)
        {
            try
            {
                Window win = new Window(OpenWindows, partials, class_name);
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
        public static void ShowNormalWin(IDictionary<HWND, string> OpenWindows, string[] partials, string class_name)
        {
            Window win = new Window(OpenWindows, partials, class_name);
            ShowWindow(win.handle, SW_SHOWNORMAL);
        }
        public static void MaximizeWin(IDictionary<HWND, string> OpenWindows, string[] partials, string class_name)
        {
            Window win = new Window(OpenWindows, partials, class_name);
            ShowWindow(win.handle, SW_MAXIMIZE);
        }
        public static void MinimizeWin(IDictionary<HWND, string> OpenWindows, string[] partials, string class_name)
        {
            Window win = new Window(OpenWindows, partials, class_name);
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
                if (length == 0)
                {
                    Rectangle lpRect = new Rectangle();
                    GetWindowRect(hWnd, ref lpRect);
                    if (lpRect.Right - 2 * lpRect.Left == 315) win_id_calendar[0] = hWnd;
                    else if (lpRect.Right - 2 * lpRect.Left == 313) win_id_calendar[1] = hWnd;
                    else return true;
                }

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }
        public static Rectangle addMarginsTo(Rectangle rect, int lx, int rx, int uy, int dy)
        {
            rect = new Rectangle(rect.X - lx, rect.Y-uy, rect.Width + lx+rx, rect.Height + uy+dy);
            return rect;
        }
    }
}
