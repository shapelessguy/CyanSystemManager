﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.IO;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public static class Service_Monitor
    {
        static public State status = State.OFF;

        public static void changeProfile(string conf) { addCommand(conf); }

        static List<Command> commands = new List<Command>();
        private static void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }

        public static void threadRun()
        {
            while (!timeToClose && status != State.OFF)
            {
                try
                {
                    Thread.Sleep(25);
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    if (command.type == MonitorCom.DEFAULT) Profile(Config.Default);
                    else if (command.type == MonitorCom.GAMING) Profile(Config.Gaming);
                    else if (command.type == MonitorCom.SOFTGAMING) Profile(Config.SoftGaming);
                }
                catch (Exception) { Console.WriteLine("Exception in monitorRun"); }
            }

        }
        public static void startService()
        {
            status = State.NEUTRAL;
            if (!File.Exists(variablePath.displayFusion)) MessageBox.Show(variablePath.displayFusion + " not found");
            Home.registerHotkeys(ST.Monitors); // register Hotkeys needed by Example_ activities
            Console.WriteLine("Starting monitorService..");

            new Thread(threadRun).Start();
            status = State.ON;
        }
        public static void stopService()
        {
            Console.WriteLine("monitorService stopped");
            status = State.OFF;
            Home.unregisterHotkeys(ST.Monitors);
            commands.Clear();
        }

        private static void Profile(object value)
        {
            Configuration configuration = Configuration.getConfiguration((Config)value);
            actualConfiguration = configuration;
            foreach (Monitor monit in monitors) monit.position = Configuration.getPosition((Config)value, monit.id);
            cmdAsync(variablePath.displayFusion, "-wallpaperloadprofile " + configuration.fusionCommand);
            Monitor monitor = MonitorManager.Ref(actualConfiguration.monitorList[0].id);
            string screenName = monitor.screen.DeviceName;
            MonitorChanger.SetAsPrimaryMonitor(monitor.screen.DeviceName);

            Thread.Sleep(4000); Service_Shortcut.KeyOrder();
            Thread.Sleep(4000); Service_Shortcut.KeyOrder();
            if (commands.Count > 1) for (int i = commands.Count - 1; i > 0; i--) commands.RemoveAt(i);
        }
    }
}
