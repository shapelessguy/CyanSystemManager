﻿using NAudio.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public class ST
    {
        //static public string Example = "EXAMPLE"; // Example_ of service type
        static public string None = "NONE";
        static public string Display = "DISPLAY";
        static public string Start = "START";
        static public string Audio = "AUDIO";
        static public string Network = "NETWORK";
        static public string Notebook = "NOTEBOOK";
        static public string CheckSite = "CHECKSITE";
        static public string Shortcuts = "SHORTCUTS";
        static public string Monitors = "MONITORS";
        static public string Timer = "TIMER";
        static public string Arduino = "ARDUINO";
        static public string HW_Monitor = "HWMONITOR";
        static public string Server = "SERVER";
    }
    public class Command
    {
        public string type = "";
        public object value = null;
        public Command(string type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }

    public static class ServiceManager
    {
        public static int generation = 0;
        public static bool canSave = false;
        public static bool timeToSave = false;

        static public Service[] allServices;

        static public void createServices()
        {
            generation += 1;
            allServices = new Service[]{
            //new Service(ServiceType.Example, "Example Service tag"), // Example_ of service
            new Service(ST.Display, "Display Service"),
            new Service(ST.Start, "Start Service"),
            new Service(ST.Audio, "Audio Service"),
            new Service(ST.Network, "Network Service"),
            new Service(ST.Notebook, "Notebook Service"),
            new Service(ST.CheckSite, "Check Site Service"),
            new Service(ST.Shortcuts, "Shortcuts Service"),
            new Service(ST.Monitors, "Monitors Service"),
            new Service(ST.Timer, "Timer Service"),
            new Service(ST.Arduino, "Arduino Service"),
            new Service(ST.Server, "Server Service")};
    }

        static public void loadActiveServices(bool andStart = false)
        {
            bool[] values = deserialize(Properties.Settings.Default.activeServices);
            for (int i = 0; i < allServices.Count(); i++)
            {
                allServices[i].startup = values[i];
                allServices[i].dataLoaded = true;
                if (andStart) allServices[i].startServiceAsWish();
            }
            System.Windows.Forms.Timer initialize = new System.Windows.Forms.Timer() { Enabled = true, Interval = 1000 };
            initialize.Tick += (o, e) => { canSave = true; initialize.Dispose(); };
        }
        static public void saveActiveServices()
        {
            if (!canSave) return;
            if (!timeToSave) return;
            timeToSave = false;
            bool[] values = new bool[allServices.Count()];
            for (int i = 0; i < allServices.Count(); i++) values[i] = allServices[i].status!=State.OFF;
            Properties.Settings.Default.activeServices = serialize(values);
            Properties.Settings.Default.Save();
        }
        static public string serialize(bool[] boolArray)
        {
            string output = "";
            foreach (bool val in boolArray)
            {
                string newVal = "";
                if (val) newVal = "1"; else newVal = "0";
                output += newVal + ";";
            }
            return output;
        }
        static public bool[] deserialize(string charSeq)
        {
            bool[] output = new bool[allServices.Count()]; 
            for (int i = 0; i < output.Length; i++) output[i] = false;

            bool[] boolArray = new bool[charSeq.Length/2];
            for (int i = 0; i < charSeq.Length / 2; i++)
            {
                bool val = false;
                if (charSeq.Substring(i * 2, 1) == "1") val = true;
                boolArray[i] = val;
            }
            for (int i = 0; i < output.Length; i++) if (boolArray.Length > i) output[i] = boolArray[i];
            return output;
        }
    }
    public class Service
    {
        public bool startup = false;
        public int generation;
        public bool dataLoaded = false;
        public State status = State.OFF;
        public bool clear;
        public State statusFromBox = State.NEUTRAL;
        public string serviceType;
        public string friendlyName;
        public object args;

        public Service(string type, string friendlyName, object args = null)
        {

            generation = ServiceManager.generation;
            this.serviceType = type;
            if (serviceType == ST.Display) { status = Service_Display.status; clear = Service_Display.clear; }
            else if (serviceType == ST.Start) { status = Service_Start.status; clear = Service_Start.clear; }
            else if (serviceType == ST.Audio) { status = Service_Audio.status; clear = Service_Audio.clear; }
            else if (serviceType == ST.Network) { status = Service_Network.status; clear = Service_Network.clear; }
            else if (serviceType == ST.Notebook) { status = Service_Notebook.status; clear = Service_Notebook.clear; }
            else if (serviceType == ST.CheckSite) { status = Service_CheckSite.status; clear = Service_CheckSite.clear; }
            else if (serviceType == ST.Shortcuts) { status = Service_Shortcut.status; clear = Service_Shortcut.clear; }
            else if (serviceType == ST.Monitors) { status = Service_Monitor.status; clear = Service_Monitor.clear; }
            else if (serviceType == ST.Timer) { status = Service_Timer.status; clear = Service_Timer.clear; }
            else if (serviceType == ST.Arduino) { status = Service_Arduino.status; clear = Service_Arduino.clear; }
            else if (serviceType == ST.Server) { status = Service_Server.status; clear = Service_Server.clear; }
            this.friendlyName = friendlyName;
            this.args = args;
            new Thread(run).Start();
        }
        public void run()
        {
            int iterations = 0;
            while (!Program.forceTermination)
            {
                iterations++;
                Thread.Sleep(200);
                getStatus();
                if (iterations == 1000/200 * 3) { ServiceManager.saveActiveServices(); iterations = 0; }
            }
        }
        public void getStatus()
        {
            status = State.OFF;
            //if (serviceType == ServiceType.Example) status = ExampleService.status; // get status from Example_ static class!
            if (serviceType == ST.Display) status = Service_Display.status;
            else if (serviceType == ST.Start) status = Service_Start.status;
            else if (serviceType == ST.Audio) status = Service_Audio.status;
            else if (serviceType == ST.Network) status = Service_Network.status;
            else if (serviceType == ST.Notebook) status = Service_Notebook.status;
            else if (serviceType == ST.CheckSite) status = Service_CheckSite.status;
            else if (serviceType == ST.Shortcuts) status = Service_Shortcut.status;
            else if (serviceType == ST.Monitors) status = Service_Monitor.status;
            else if (serviceType == ST.Timer) status = Service_Timer.status;
            else if (serviceType == ST.Arduino) status = Service_Arduino.status;
            else if (serviceType == ST.Server) status = Service_Server.status; 
            if (statusFromBox != State.NEUTRAL)
            {
                if (statusFromBox == State.ON) Program.home.Invoke((MethodInvoker)delegate { startService(); }); 
                else Program.home.Invoke((MethodInvoker)delegate { stopService(); });
            }
            statusFromBox = State.NEUTRAL;
        }
        public void startServiceAsWish() { if (startup) { startService();} }
        public void startService()
        {
            if (status != State.OFF) return;
            status = State.NEUTRAL;
            //if (serviceType == ServiceType.Example) ExampleService.startService(); // start service inside Example_ service
            if (serviceType == ST.Display) Service_Display.startService();
            else if (serviceType == ST.Start) Service_Start.startService();
            else if (serviceType == ST.Audio) Service_Audio.startService();
            else if (serviceType == ST.Network) Service_Network.startService();
            else if (serviceType == ST.Notebook) Service_Notebook.startService();
            else if (serviceType == ST.CheckSite) Service_CheckSite.startService();
            else if (serviceType == ST.Shortcuts) Service_Shortcut.startService();
            else if (serviceType == ST.Monitors) Service_Monitor.startService();
            else if (serviceType == ST.Timer) Service_Timer.startService();
            else if (serviceType == ST.Arduino) Service_Arduino.startService();
            else if (serviceType == ST.Server) Service_Server.startService();
        }
        public void stopService(bool dispose = false)
        {
            if (status == State.OFF) return;
            status = State.OFF;
            //if (serviceType == ServiceType.Example) ExampleService.stopService(); // stop service inside Example_ service
            if (serviceType == ST.Display) Service_Display.stopService(dispose);
            else if (serviceType == ST.Start) Service_Start.stopService(dispose);
            else if (serviceType == ST.Audio) Service_Audio.stopService(dispose);
            else if (serviceType == ST.Network) Service_Network.stopService(dispose);
            else if (serviceType == ST.Notebook) Service_Notebook.stopService(dispose);
            else if (serviceType == ST.CheckSite) Service_CheckSite.stopService(dispose);
            else if (serviceType == ST.Shortcuts) Service_Shortcut.stopService(dispose);
            else if (serviceType == ST.Monitors) Service_Monitor.stopService(dispose);
            else if (serviceType == ST.Timer) Service_Timer.stopService(dispose);
            else if (serviceType == ST.Arduino) Service_Arduino.stopService(dispose);
            else if (serviceType == ST.Server) Service_Server.stopService(dispose);
        }
    }
}
