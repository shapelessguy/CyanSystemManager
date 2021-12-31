using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public class KeyActivity
    {
        public string name = "";
        public int trigger;
        public bool initialTrigger;
        public KeyMode mode = KeyMode.Normal;
        public KeyMode actMode = KeyMode.Normal;
        private object arg = null;
        public Thread thread = null;
        public Thread activityThread = null;
        private bool feed = false;
        public KeyActivity(string name, KeyMode mode, int trigger, bool initialTrigger)
        {
            this.name = name;
            this.mode = mode;
            this.trigger = trigger;
            this.initialTrigger = initialTrigger;
        }
        public void call(object arg = null)
        {
            this.arg = arg;
            feed = true;
            if (thread == null)
            {
                thread = new Thread(delayActivity);
                thread.Start();
            }
        }
        private void activity()
        {
            Activity.startActivity(name, arg, actMode);
        }
        public void delayActivity()
        {
            if (Program.timeToClose) return;
            int iterations = 0;
            if(mode == KeyMode.Normal)
            {
                actMode = KeyMode.Normal;
                activityThread = new Thread(activity);
                activityThread.Start();
            }
            else if (mode == KeyMode.Hold)
            {
                while (feed)
                {
                    if(iterations == 0 && initialTrigger)
                    {
                        actMode = KeyMode.Normal;
                        activityThread = new Thread(activity);
                        activityThread.Start();
                    }
                    if (iterations == (trigger/80*3) / 4)
                    {
                        actMode = KeyMode.Hold;
                        activityThread = new Thread(activity);
                        activityThread.Start();
                    }
                    feed = false;
                    iterations += 1;
                    Thread.Sleep(80);
                }
            }
            else if(mode == KeyMode.onRelease)
            {
                while (feed)
                {
                    feed = false;
                    Thread.Sleep(30);
                }
                Thread.Sleep(500);
                while (feed)
                {
                    feed = false;
                    Thread.Sleep(30);
                }
                actMode = KeyMode.onRelease;
                activityThread = new Thread(activity);
                activityThread.Start();
            }

            thread = null;
        }
    }
    public class Key
    {
        public string service;
        public int id;
        public string name = "";
        public Keys key;
        public KeyModifier modifier;
        public KeyMode keyMode = KeyMode.Normal;
        public int trigger = 0;
        public bool initialTrigger;
        public Thread thread = null;
        public Key(string service, int id, string name, Keys actKey = Keys.None, KeyModifier actModifier = KeyModifier.None, 
                        KeyMode mode = KeyMode.Normal, int trigger = 0, bool initialTrigger = false) 
        {
            this.service = service;
            this.id = id;
            if (name == "") return;
            this.name = name;
            key = actKey;
            modifier = actModifier;
            keyMode = mode;
            this.trigger = trigger;
            this.initialTrigger = initialTrigger;
        }
    }
    public class Binded
    {
        public Key key;
        public KeyActivity activity;
        public Binded(Key key, KeyActivity activity)
        {
            this.key = key;
            this.activity = activity;
        }
    }
    public class BindElements
    {
        public List<Key> keyList = new List<Key>();
        public List<KeyActivity> activities = new List<KeyActivity>();

        public void makeConsistent()
        {
            activities.Clear();
            foreach (Key key in keyList)
            {
                bool contains = false;
                foreach (KeyActivity activity in activities) if (activity.name == key.name) contains = true;
                if (!contains) activities.Add(new KeyActivity(key.name, key.keyMode, key.trigger, key.initialTrigger));
            }
        }
        public KeyActivity findActivity(string name)
        {
            foreach (KeyActivity activity in activities) if (activity.name == name) return activity;
            return null;
        }
        public Binded findKey(Keys mkey, KeyModifier modifier)
        {
            foreach (Key key in keyList)
            {
                if (key.key == mkey && key.modifier == modifier) return new Binded(key, findActivity(key.name));
            }
            return null;
        }
    }
    public class Utility
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        public const int APPCOMMAND_VOLUME_UP = 0xA0000;
        public const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        public const int APPCOMMAND_PLAY_PAUSE = 0x140000;
        public const int WM_APPCOMMAND = 0x319;

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);


        public enum State
        {
            OFF = 0,
            ON = 1,
            NEUTRAL = 2,
        }
        public enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8,
        }
        public enum KeyMode
        {
            Normal = 1,
            Hold = 2,
            onRelease = 4,
        }

        //public IKeyboardMouseEvents m_GlobalHook;
        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);
        [DllImport("user32.dll")]
        static public extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static public extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static public extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const byte VK_MEDIA_PLAY_PAUSE = 0xB3;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pnt);
    }

}
