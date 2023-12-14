using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Windows.Forms;

namespace CyanSystemManager
{
    static public class Service_Example
    {
        static public string title = "exampleService";
        static public string serviceType = ST.None;
        static public State status = State.OFF;
        static public bool clear;

        // Functions of Example_Service --> they should be called from outside the service
        static public void FunctionFromOutside() { addCommand(ExampleCom.EX_COM1); }

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
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Log("Exception in "+title); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == ExampleCom.EX_COM1) InternalFunction1();
            else if (command.type == ExampleCom.EX_COM2) InternalFunction2();
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Log("Starting "+ title + "..");

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
        static private void InternalFunction1() { Log("Do something"); }
        static private void InternalFunction2() { Log("Do other stuff"); }
        // //////////
    }
}
