using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public class ST
    {
        //static public string Example = "EXAMPLE"; // Example_ of service type
        static public string None = "NONE";
        static public string Start = "START";
        static public string Audio = "AUDIO";
        static public string Network = "NETWORK";
        static public string Notebook = "NOTEBOOK";
        static public string CheckSite = "CHECKSITE";
        static public string Shortcuts = "SHORTCUTS";
        static public string Monitors = "MONITORS";
        static public string Timer = "TIMER";
        static public string HW_Monitor = "HWMONITOR";
    }

    public static class ServiceManager
    {
        public static bool canSave = false;
        public static bool timeToSave = false;
        static public Service[] allServices = new Service[]
        {
            //new Service(ServiceType.Example, "Example Service tag"), // Example_ of service
            new Service(ST.Start, "Start Service"),
            new Service(ST.Audio, "Audio Service"),
            new Service(ST.Network, "Network Service"),
            new Service(ST.Notebook, "Notebook Service"),
            new Service(ST.CheckSite, "Check Site Service"),
            new Service(ST.Shortcuts, "Shortcuts Service"),
            new Service(ST.Monitors, "Monitors Service"),
            new Service(ST.Timer, "Timer Service"),
            //new Service(ST.HW_Monitor, "HW Monitor Service"),
        };

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
        public bool dataLoaded = false;
        public State status = State.OFF;
        public State statusFromBox = State.NEUTRAL;
        public string serviceType;
        public string friendlyName;
        public object args;

        public Service(string type, string friendlyName, object args = null)
        {
            this.serviceType = type;
            if (serviceType == ST.Start) status = Service_Start.status;
            else if (serviceType == ST.Audio) status = Service_Audio.status;
            else if (serviceType == ST.Network) status = Service_Network.status;
            else if (serviceType == ST.Notebook) status = Service_Notebook.status;
            else if (serviceType == ST.CheckSite) status = Service_CheckSite.status;
            else if (serviceType == ST.Shortcuts) status = Service_Shortcut.status;
            else if (serviceType == ST.Monitors) status = Service_Monitor.status;
            else if (serviceType == ST.Timer) status = Service_Timer.status;
            //else if (serviceType == ST.HW_Monitor) status = Service_HWMonitoring.status;
            this.friendlyName = friendlyName;
            this.args = args;
            new Thread(run).Start();
        }
        public void run()
        {
            int iterations = 0;
            while (!Program.timeToClose)
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
            if (serviceType == ST.Start) status = Service_Start.status;
            else if (serviceType == ST.Audio) status = Service_Audio.status;
            else if (serviceType == ST.Network) status = Service_Network.status;
            else if (serviceType == ST.Notebook) status = Service_Notebook.status;
            else if (serviceType == ST.CheckSite) status = Service_CheckSite.status;
            else if (serviceType == ST.Shortcuts) status = Service_Shortcut.status;
            else if (serviceType == ST.Monitors) status = Service_Monitor.status;
            else if (serviceType == ST.Timer) status = Service_Timer.status;
            //else if (serviceType == ST.HW_Monitor) status = Service_HWMonitoring.status; 
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
            if (serviceType == ST.Start) Service_Start.startService();
            else if (serviceType == ST.Audio) Service_Audio.startService();
            else if (serviceType == ST.Network) Service_Network.startService();
            else if (serviceType == ST.Notebook) Service_Notebook.startService();
            else if (serviceType == ST.CheckSite) Service_CheckSite.startService();
            else if (serviceType == ST.Shortcuts) Service_Shortcut.startService();
            else if (serviceType == ST.Monitors) Service_Monitor.startService();
            else if (serviceType == ST.Timer) Service_Timer.startService();
            //else if (serviceType == ST.HW_Monitor) Service_HWMonitoring.startService();
        }
        public void stopService()
        {
            if (status == State.OFF) return;
            status = State.OFF;
            //if (serviceType == ServiceType.Example) ExampleService.stopService(); // stop service inside Example_ service
            if (serviceType == ST.Start) Service_Start.stopService();
            else if (serviceType == ST.Audio) Service_Audio.stopService();
            else if (serviceType == ST.Network) Service_Network.stopService();
            else if (serviceType == ST.Notebook) Service_Notebook.stopService();
            else if (serviceType == ST.CheckSite) Service_CheckSite.stopService();
            else if (serviceType == ST.Shortcuts) Service_Shortcut.stopService();
            else if (serviceType == ST.Monitors) Service_Monitor.stopService();
            else if (serviceType == ST.Timer) Service_Timer.stopService();
            //else if (serviceType == ST.HW_Monitor) Service_HWMonitoring.stopService();
        }
    }
}
