using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public class Window
    {
        public bool validate = false;
        public IntPtr handle;
        public string name;
        public string class_name = "none";
        public int x;
        public int y;
        public int width;
        public int height;

        public Window(IntPtr handle, bool getText=true)
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

        public void FindWindow(IntPtr handle, bool getText=true)
        {
            this.handle = handle;
            StringBuilder lpString = new StringBuilder();
            if(getText) GetWindowText(handle, lpString, 100);
            this.name = lpString.ToString();
            //GetClassName(handle, lpString, 100);
            //this.class_name = lpString.ToString();
            Rectangle lpRect = new Rectangle();
            GetWindowRect(handle, ref lpRect);
            this.x = lpRect.Left;
            this.y = lpRect.Top;
            this.width = lpRect.Right - lpRect.Left - x;
            this.height = lpRect.Bottom - lpRect.Top - y;
            validate = true;
            
        }

        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name)
        {
            FindWindow(OpenWindows, partialname, class_name);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, Screen screen, int x, int y)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, partialname, class_name);
            Focus();
            SetPosition(screen, x, y);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, Screen screen, Point point)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, partialname, class_name);
            Focus();
            SetPosition(screen, point.X, point.Y);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, int width, int height)
        {
            FindWindow(OpenWindows, partialname, class_name);
            Focus();
            SetSize(width, height);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, 
                        Screen screen, Rectangle bounds, string[] exclusions = null)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, partialname, class_name, exclusions);
            Focus();
            SetPosition(screen, bounds.X, bounds.Y);
            SetSize(bounds.Width, bounds.Height);
        }
        public Window(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, Screen screen, int x, int y, int width, int height)
        {
            if (screen == null) return;
            FindWindow(OpenWindows, partialname, class_name);
            Focus();
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

        public void FindWindow(IDictionary<IntPtr, string> OpenWindows, string[] partialname, string class_name, string[] exclusions=null)
        {
            bool trovato = false;
            handle = IntPtr.Zero;
            foreach (KeyValuePair<IntPtr, string> window in OpenWindows)
            {
                bool containsall = true;
                foreach (string stringa in partialname) if (!window.Value.Contains(stringa)) containsall = false;
                if(exclusions != null)
                    foreach (string stringa in exclusions) if (window.Value.Contains(stringa)) containsall = false;
                if (containsall)
                {
                    StringBuilder lpString = new StringBuilder("Class not found");
                    if (class_name != "") GetClassName(window.Key, lpString, 300);
                    //Console.WriteLine(lpString);
                    if (lpString.ToString() == class_name || class_name == "")
                    {
                        handle = window.Key;
                        name = window.Value;
                        class_name = lpString.ToString();
                        trovato = true;
                    }
                }
            }
            if (!trovato) validate = false;
            this.class_name = class_name;
            Rectangle lpRect = new Rectangle();
            GetWindowRect(handle, ref lpRect);
            this.x = lpRect.Left;
            this.y = lpRect.Top;
            this.width = lpRect.Right - lpRect.Left - x;
            this.height = lpRect.Bottom - lpRect.Top - y;
            //Console.WriteLine("Window Name: " + name);
            //Console.WriteLine("Class -----> " + class_name);
            //Focus();
            validate = true;
            //Console.WriteLine(this.name + " -> " + this.handle);
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
    }
}
