using System;
using System.Collections.Generic;
using System.Threading;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Configuration;

namespace CyanSystemManager
{
    public class TimerArgs
    {
        public int hours = 0;
        public int minutes = 0;
        public string title = null;
        public TimerArgs(int minutes, string title=null) 
        {
            this.hours = minutes / 60;
            this.minutes = minutes % 60;
            this.title = title;
        }
    }
    static public class Service_Timer
    {
        static public string title = "timerService";
        static public string serviceType = ST.Timer;
        static public State status = State.OFF;
        static public bool clear;

        // Functions of Example_Service --> they should be called from outside the service
        static public void TimerPressed() { addCommand(TimerCom.TIMER); }
        static public void createTimer(TimerArgs args) { addCommand(TimerCom.TIMER, args); }
        static public void ShortTimerPressed() { addCommand(TimerCom.SHORTPRESS); }
        static public void LongTimerPressed() { addCommand(TimerCom.LONGPRESS); }

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
                    updateMousePosition();
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Log("Exception in " + title); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == TimerCom.TIMER) fastTimer(command.value);
            else if (command.type == TimerCom.SHORTPRESS) shortSystemTimer();
            else if (command.type == TimerCom.LONGPRESS) longSystemTimer();
        }
        static Point mousePosition = new Point(0, 0);
        static DateTime prevTime = DateTime.Now;
        static int iterations = 0;
        static private void updateMousePosition()
        {
            iterations++;
            if (Cursor.Position != mousePosition) { mousePosition = Cursor.Position; prevTime = DateTime.Now; }
            else
            {
                if (iterations%10 == 0) { if(iterations>1000) iterations = 0; } else return;
                if (InactivityForm.active || !Properties.Settings.Default.allowIdle) return;
                if (DateTime.Now.Hour < 2 || DateTime.Now.Hour > 8) return;
                if (prevTime.Year > 2000 && DateTime.Now.Subtract(prevTime).TotalMinutes > 120)
                    Program.home.Invoke((MethodInvoker)delegate { 
                        foreach(Screen screen in Screen.AllScreens)
                        {
                            InactivityForm form = new InactivityForm();
                            form.locate(screen);
                        }
                });
                //prevTime = DateTime.Now;
                //if (iterations % 50 == 0) { getTemp(); Log(DateTime.Now.Subtract(prevTime).TotalMinutes); }
            }
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
        static private void shortSystemTimer()
        {
            Log("normal");
        }
        static private void longSystemTimer()
        {
            Log("hold");
        }
        static private void fastTimer(Object args=null)
        {
            TimerArgs args_ = null;
            if (args != null) args_ = (TimerArgs)args;
            Program.home.Invoke((MethodInvoker)delegate { new Chronometer(args_); });
        }
        // //////////
    }
}
