﻿using System;
using System.Collections.Generic;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Threading;
using System.Windows.Forms;

namespace CyanSystemManager
{
    class Service_CheckSite
    {
        static public string title = "checkSiteService";
        static public string serviceType = ST.CheckSite;
        static public State status = State.OFF;
        static public bool clear;
        static private CheckSiteUpdate checkform;

        // Functions of Example_Service --> they should be called from outside the service


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
            Program.home.Invoke((MethodInvoker)delegate
            {
                checkform = new CheckSiteUpdate();
                checkform.Show();
            });
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
                catch (Exception) { Log("Exception in " + title); }
            }
        }
        static public void Tree(Command command)
        {

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
        static public void beforeStart()
        {
        }
        static public void stopService(bool dispose)
        {
            Log(title + " stopped");
            status = State.OFF;
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        // Inside functions

        // //////////
    }
}
