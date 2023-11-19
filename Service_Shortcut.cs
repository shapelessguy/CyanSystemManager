using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Windows.Forms;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    static public class Service_Shortcut
    {
        static public State status = State.OFF;
        static public bool clear;
        public static void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(ST.Shortcuts);
            Console.WriteLine("Starting shortcutService..");

            Thread audioThread = new Thread(shortcutRun);
            audioThread.Start();
            status = State.ON;
        }
        public static void stopService(bool dispose)
        {
            Console.WriteLine("shortcutService stopped");
            status = State.OFF;
            Home.unregisterHotkeys(ST.Shortcuts);
            commands.Clear();
            clear = true;
        }

        // //////////////   Functions of ShortcutService
        public static void TakeSnapshot() { addCommand(ShortcutCom.SNAPSHOT); }
        public static void UpSizing() { addCommand(ShortcutCom.UPSIZING); }
        public static void KeyPad(string command, KeyMode keymode = KeyMode.Normal) { addCommand(command, keymode); }

        // //////////////
        private static void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }

        static List<Command> commands = new List<Command>();
        public static void shortcutRun()
        {
            while (!Program.forceTermination && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(25);
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    int mode = 0;
                    if (command.type == ShortcutCom.A) A(mode);
                    else if (command.type == ShortcutCom.X) X(mode);
                    else if (command.type == ShortcutCom.Y) Y(mode);
                    else if (command.type == ShortcutCom.B) B(mode, (KeyMode)command.value);
                    else if (command.type == ShortcutCom.LTRIG) LTRIG(mode);
                    else if (command.type == ShortcutCom.RTRIG) RTRIG(mode);
                    else if (command.type == ShortcutCom.LPAD) LPAD(mode);
                    else if (command.type == ShortcutCom.RST) RST(mode);
                    else if (command.type == ShortcutCom.START) START(mode);
                    else if (command.type == ShortcutCom.SNAPSHOT) SNAPSHOT();
                    else if (command.type == ShortcutCom.UPSIZING) UPSIZING();
                }
                catch (Exception) { Console.WriteLine("Exception in shortcutRun"); }
            }

        }

        static int updatedThreadID = 0;
        static Point prevPoint;
        private static void A(int mode = 0)
        {
            if (mode == 0)
            {
                MonitorManager.GetMonitors();
                MyMonitor bigMonitor = MonitorManager.Ref(2);
                if (bigMonitor == null) return;
                Point centralPoint = new Point(bigMonitor.screen.Bounds.Location.X + bigMonitor.screen.Bounds.Width/2, 
                                                bigMonitor.screen.Bounds.Location.Y + bigMonitor.screen.Bounds.Height/2);
                if(updatedThreadID == 0) prevPoint = Cursor.Position;
                updatedThreadID++;
                Cursor.Show();
                for(int i=0; i<3; i++) Cursor.Position = centralPoint;
                Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)centralPoint.X, (uint)centralPoint.Y, 0, 0);
                Thread.Sleep(30);
                void afterWhile()
                {
                    int thisThreadID = updatedThreadID;
                    updatedThreadID = thisThreadID;
                    Thread.Sleep(150);
                    if (updatedThreadID!=thisThreadID) { return; }
                    for (int i = 0; i < 3; i++)
                        Program.home.Invoke((MethodInvoker)delegate { Cursor.Position = prevPoint; });
                    updatedThreadID = 0;
                }
                new Thread(afterWhile).Start();
                
            }
        }
        private static void X(int mode = 0)
        {
            if(mode == 0)
            {
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, 0, UIntPtr.Zero);
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
        }
        private static void Y(int mode = 0)
        {
            if(mode == 0) SendKeys.SendWait(" ");
        }
        private static void B(int mode = 0, KeyMode keymode = KeyMode.Normal)
        {
            if(mode == 0)
            {
                if (keymode == KeyMode.Normal) Service_Timer.ShortTimerPressed();
                else if (keymode == KeyMode.Hold) Service_Timer.LongTimerPressed();
            }
        }
        private static void LTRIG(int mode = 0)
        {
            MyMonitor bigM = MonitorManager.Ref(2);
            if (bigM == null) return;
            MyMonitor mediumM = MonitorManager.Ref(1);
            if(mediumM == null) mediumM = MonitorManager.Ref(3);
            if (mediumM == null) return;
            MyMonitor smallM = MonitorManager.Ref(3);

            Point centralBig = new Point(bigM.screen.Bounds.X + bigM.screen.Bounds.Width / 2,
                                            bigM.screen.Bounds.Y + bigM.screen.Bounds.Height / 2);
            Point centralMedium = new Point(mediumM.screen.Bounds.X + mediumM.screen.Bounds.Width / 2,
                                            mediumM.screen.Bounds.Y + mediumM.screen.Bounds.Height / 2);
            if (mediumM.screen.Bounds.Contains(Cursor.Position)) Cursor.Position = centralBig;
            else Cursor.Position = centralMedium;
        }
        private static void RTRIG(int mode = 0)
        {
            if(mode == 0)
            {
                //List<Rectangle> bounds = new List<Rectangle>();
                if (ScreenSaverForm.active) { ScreenSaverForm.active = false; return; }
                MonitorManager.GetMonitors();
                foreach (MyMonitor monitor in monitors) if (monitor.id != 2)
                    {
                        Program.home.Invoke((MethodInvoker)delegate {
                            ScreenSaverForm screenForm = new ScreenSaverForm() { Bounds = monitor.screen.Bounds};
                            screenForm.follow = monitor.screen;
                            //screenForm.Bounds = monitor.screen.Bounds;
                            screenForm.Show();
                        });
                    }
            }
        }
        private static void LPAD(int mode = 0)
        {

        }
        private static void RST(int mode = 0)
        {

        }
        private static void START(int mode = 0)
        {

        }

        private static void SNAPSHOT()
        {
            SendKeys.SendWait("%^+{S}");
        }
        private static void UPSIZING()
        {
            IntPtr hWnd = WindowFromPoint(Cursor.Position);
            Console.WriteLine("window -> " + hWnd);
            SortWindows.OpenWindows = WindowWrapper.GetOpenWindows();

            //Window win = new Window(hWnd);
            SetForegroundWindow(hWnd);
            
            MonitorManager.GetMonitors();
            Screen smallMonitor = MonitorManager.Ref(3).screen;
            Screen mediumMonitor = MonitorManager.Ref(1).screen;
            Screen bigMonitor = MonitorManager.Ref(2).screen;

            Rectangle lpRect = new Rectangle();
            Window.GetWindowRect(hWnd, ref lpRect);
            Point pnt = new Point(bigMonitor.Bounds.X + 100, bigMonitor.Bounds.Y + 100);
            Size size = new Size(bigMonitor.Bounds.Width-200, bigMonitor.Bounds.Height - 200);

            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, size.Width, size.Height, SWP_NOMOVE | 0x0004);
            SetWindowPos(hWnd, IntPtr.Zero, pnt.X, pnt.Y, size.Width, size.Height, SWP_NOSIZE | 0x0004);
            Thread.Sleep(100);
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, size.Width, size.Height, SWP_NOMOVE | 0x0004);
            SetWindowPos(hWnd, IntPtr.Zero, pnt.X, pnt.Y, size.Width, size.Height, SWP_NOSIZE | 0x0004);

            return;
            Window win = new Window(hWnd, false);
            win = new Window(hWnd, bigMonitor, new Rectangle(100,100,1000,1000));
            return;

            if (bigMonitor != null && bigMonitor.WorkingArea.Contains(new Point(win.x, win.y)))
            {
                Rectangle bigMonitorArea = WindowWrapper.addMarginsTo(bigMonitor.Bounds, -100, -100, -100, -100);
                win = new Window(hWnd, bigMonitor, bigMonitorArea);
            }
            

        }





    }
}
