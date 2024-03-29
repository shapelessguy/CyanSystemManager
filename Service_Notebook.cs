﻿using System;
using System.Collections.Generic;
using System.Threading;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Windows.Forms;
using System.Runtime.ConstrainedExecution;

namespace CyanSystemManager
{
    static public class Service_Notebook
    {
        static public string title = "notebookService";
        static public string serviceType = ST.Notebook;
        static public State status = State.OFF;
        static public bool clear;
        static private NotebookForm notebook;

        // Functions of Example_Service --> they should be called from outside the service
        static public void OpenOrClose() { addCommand(NotebookCom.NOTEOPEN); }

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
            Program.home.Invoke((MethodInvoker)delegate { notebook = new NotebookForm(); });
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
            if (command.type == NotebookCom.NOTEOPEN) CallNotebook();
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
            notebook = new NotebookForm();
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
        static private void CallNotebook()
        {
            if (!NotebookForm.canResize) return;
            if (notebook.Visible) Program.home.Invoke((MethodInvoker)delegate { notebook.Hide(); });
            else Program.home.Invoke((MethodInvoker)delegate { notebook.Show(); });
        }
        // //////////
    }
}
