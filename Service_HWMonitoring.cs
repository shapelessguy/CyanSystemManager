using System;
using System.Collections.Generic;
using System.Threading;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using OpenHardwareMonitor.Hardware;
using Microsoft.Diagnostics.Tracing.Session;
using System.Diagnostics;

namespace CyanSystemManager
{
    public class computerInfo
    {
        public int[] cpus = new int[0];
        public int[] cpusTemp = new int[0];
        public int gpuTemp = -1;
        public int gpuClock=-1;
        public int totalGpuMemory = -1;
        public int gpuVRAM = -1;
        public int gpuTot = -1;
        public int cpuTot = -1;
        public int cpuTempTot = -1;
        public int totalMemoryAvailable = -1;
        public int memoryUsed = -1;
        public Dictionary<string, int> fps = new Dictionary<string, int>();
        public computerInfo() { }
        public void print()
        {
            Program.Log("");
            Console.Write("CPU load: "); foreach (var value in cpus) Console.Write(value + "  "); Program.Log("");
            Log("CPU total load: " + cpuTot);
            Console.Write("CPU temp: "); foreach (var value in cpusTemp) Console.Write(value + "  "); Program.Log("");
            Log("CPU total temperature: " + cpuTempTot);
            Log("GPU total load: " + gpuTot);
            Log("GPU temp: : " + gpuTemp);
            Log("GPU clock: " + gpuClock);
            Log("GPU VRAM: " + gpuVRAM);
            Log("Memory used/available: " + memoryUsed+"/"+totalMemoryAvailable);
            Log("Processes/FPS: "); foreach (var item in fps) Log("  "+item.Key+": "+item.Value);
        }
        public computerInfo copy(computerInfo info)
        {
            computerInfo newInfo = new computerInfo();
            newInfo.cpus = new int[info.cpus.Length];
            newInfo.cpusTemp = new int[info.cpusTemp.Length];
            for (int i = 0; i < newInfo.cpus.Length; i++) newInfo.cpus[i] = info.cpus[i];
            for (int i = 0; i < newInfo.cpusTemp.Length; i++) newInfo.cpusTemp[i] = info.cpusTemp[i];
            newInfo.gpuTemp = info.gpuTemp;
            newInfo.gpuClock = info.gpuClock;
            newInfo.totalGpuMemory = info.totalGpuMemory;
            newInfo.gpuVRAM = info.gpuVRAM;
            newInfo.gpuTot = info.gpuTot;
            newInfo.cpuTot = info.cpuTot;
            newInfo.cpuTempTot = info.cpuTempTot;
            newInfo.totalMemoryAvailable = info.totalMemoryAvailable;
            newInfo.memoryUsed = info.memoryUsed;
            foreach (var item in info.fps) newInfo.fps[item.Key] = item.Value;
            return newInfo;
        }
        public computerInfo copy() {
            int iterations = 0;
            while (iterations < 3)
            {
                try { return copy(this); } catch (Exception) { Thread.Sleep(50); }
                iterations++;
            }
            return null;
        }
    }
    static public class Service_HWMonitoring
    {
        static public int monitoringTick = 1;
        static public int nWindows = 2;
        static public Dictionary<int, int> dims = new Dictionary<int, int>() { {0,195}, { 1, 128 }, { 2, 200 } };
        static public int sampling = 100;

        static public string title = "hwmonitoringService";
        static public string serviceType = ST.None;
        static public State status = State.OFF;
        static public bool clear;
        static Dictionary<string, int> fps_ = new Dictionary<string, int>();
        static public computerInfo PC_INFO = new computerInfo();

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
        static int iterations = 0;
        static int sleep = 25;
        static int condition;
        static public void threadRun()
        {
            GPUSession();
            while (!forceTermination && status != State.OFF)
            {
                condition = ((monitoringTick * 1000) / sleep);
                try
                {
                    iterations++;
                    if (iterations % condition == 0) { iterations = 0; getInformation(); }
                    Thread.Sleep(sleep);
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Log("Exception in " + title); }
            }
        }
        static public void Tree(Command command) { }
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
        static public void beforeStart() {
            bool onlyshow = false;
            if (Charts.chartForms.Count > 0) onlyshow = true;
            for(int i=0; i<nWindows; i++) { if (onlyshow) Charts.chartForms[i].ShowForm(); else new Charts(); }
        
                //Program.home.Invoke((MethodInvoker)delegate 
                //{ if (onlyshow) Charts.chartForms[i].ShowForm(); else new Charts(); }); 
        }
        static public void stopService()
        {
            Log(title + " stopped");
            status = State.OFF;
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        // Inside functions
        static private void getInformation()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer() {CPUEnabled = true, GPUEnabled = true, RAMEnabled = true};
            computer.Open();
            computer.Accept(updateVisitor);
            computerInfo INFO = new computerInfo();
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                //for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
               // {
                //    Console.Write(computer.Hardware[i].Sensors[j].Name + "  " + computer.Hardware[i].Sensors[j].SensorType + "  ");
                //    Console.Write(computer.Hardware[i].Sensors[j].Value);
                //    Log();
               // }

                List<int> values = new List<int>();

                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    values = new List<int>();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.cpusTemp = new int[values.Count-1];
                    for (int k = 0; k < INFO.cpusTemp.Length; k++) INFO.cpusTemp[k] = values[k];
                    INFO.cpuTempTot = values[values.Count - 1];

                    values = new List<int>();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.cpus = new int[values.Count - 1];
                    for (int k = 0; k < INFO.cpus.Length; k++) INFO.cpus[k] = values[k];
                    INFO.cpuTot = values[values.Count - 1];
                }


                if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.gpuTemp = values[0];

                    values = new List<int>();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.gpuTot = values[0];

                    values = new List<int>();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Clock)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.gpuClock = values[0];
                    INFO.totalGpuMemory = values[1];

                    float load = 0;
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        { load = (float)computer.Hardware[i].Sensors[j].Value; }
                    }
                    INFO.gpuVRAM = (int)(load*INFO.totalGpuMemory*2) / 100;
                }

                if (computer.Hardware[i].HardwareType == HardwareType.RAM)
                {

                    float load = 0;
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        { load = (float)computer.Hardware[i].Sensors[j].Value; }
                    }

                    values = new List<int>();
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Data)
                        { values.Add((int)computer.Hardware[i].Sensors[j].Value); }
                    }
                    INFO.totalMemoryAvailable = (int)(values[0] + values[1]) * 1024;
                    INFO.memoryUsed = (int)(values[0]) * 1024;
                }







            }
            INFO.fps = fps_;
            PC_INFO = INFO;
            //PC_INFO.print();
            computer.Close();
        }

        public const int EventID_D3D9PresentStart = 1;
        public const int EventID_DxgiPresentStart = 42;

        //ETW provider codes
        public static readonly Guid DXGI_provider = Guid.Parse("{CA11C036-0102-4A2D-A6AD-F03CFED5D3C9}");
        public static readonly Guid D3D9_provider = Guid.Parse("{783ACA0A-790E-4D7F-8451-AA850511C6B9}");

        static TraceEventSession m_EtwSession;
        static Dictionary<int, TimestampCollection> frames = new Dictionary<int, TimestampCollection>();
        static Stopwatch watch = null;
        static object sync = new object(); 
        static void EtwThreadProc() {  m_EtwSession.Source.Process(); }

        static private void GPUSession (){

            m_EtwSession = new TraceEventSession("GpuInfoSession");
            m_EtwSession.StopOnDispose = true;
            m_EtwSession.EnableProvider("Microsoft-Windows-D3D9");
            m_EtwSession.EnableProvider("Microsoft-Windows-DXGI");

            //handle event
            m_EtwSession.Source.AllEvents += data =>
            {
                //filter out frame presentation events
                if (((int)data.ID == EventID_D3D9PresentStart && data.ProviderGuid == D3D9_provider) ||
                    ((int)data.ID == EventID_DxgiPresentStart && data.ProviderGuid == DXGI_provider))
                {
                    int pid = data.ProcessID;
                    long t;

                    lock (sync)
                    {
                        t = watch.ElapsedMilliseconds;

                        //if process is not yet in Dictionary, add it
                        if (!frames.ContainsKey(pid))
                        {
                            frames[pid] = new TimestampCollection();

                            string name = "";
                            var proc = Process.GetProcessById(pid);
                            if (proc != null)
                            {
                                using (proc)
                                {
                                    name = proc.ProcessName;
                                }
                            }
                            else name = pid.ToString();

                            frames[pid].Name = name;
                        }

                        //store frame timestamp in collection
                        frames[pid].Add(t);
                    }
                }
            };
            watch = new Stopwatch();
            watch.Start();

            Thread thETW = new Thread(EtwThreadProc);
            thETW.IsBackground = true;
            thETW.Start();

            Thread thOutput = new Thread(OutputThreadProc);
            thOutput.IsBackground = true;
            thOutput.Start();
        }
        static void OutputThreadProc()
        {
            while (!forceTermination && status != State.OFF)
            {
                long t1, t2;
                long dt = 2000;

                lock (sync)
                {
                    t2 = watch.ElapsedMilliseconds;
                    t1 = t2 - dt;

                    foreach (var x in frames.Values)
                    {
                        //get the number of frames
                        int count = x.QueryCount(t1, t2);

                        //calculate FPS
                        fps_[x.Name] = (int)((double)count / dt * 1000.0);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        // //////////
        public class TimestampCollection
        {
            const int MAXNUM = 1000;

            public string Name { get; set; }

            List<long> timestamps = new List<long>(MAXNUM + 1);
            object sync = new object();

            //add value to the collection
            public void Add(long timestamp)
            {
                lock (sync)
                {
                    timestamps.Add(timestamp);
                    if (timestamps.Count > MAXNUM) timestamps.RemoveAt(0);
                }
            }

            //get the number of timestamps withing interval
            public int QueryCount(long from, long to)
            {
                int c = 0;

                lock (sync)
                {
                    foreach (var ts in timestamps)
                    {
                        if (ts >= from && ts <= to) c++;
                    }
                }
                return c;
            }
        }
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
    }
}