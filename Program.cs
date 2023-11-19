using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CyanSystemManager
{
    static class Program
    {
        public static Home home;
        public static bool forceTermination = false;
        public static bool restart = true;
        public static Process[] all_processes = null;
        public static string multimonitor_path;

        [STAThread]
        static void Main(string[] args)
        {
            bool startup = false;
            all_processes = Process.GetProcesses();
            multimonitor_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "MultiMonitorTool", "MultiMonitorTool.exe");
            foreach (string arg in args) if (arg == "startup") startup = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (restart)
            {
                restart = false;
                Application.Run(home = new Home(startup));
                startup = false;
                if (restart)
                {
                    ConfigurationManager.RefreshSection("appSettings");
                    ConfigurationManager.RefreshSection("configuration");
                    Properties.Settings.Default.Reload();
                    Thread.Sleep(1500);
                }
                forceTermination = false;
            }
            killMainProcess();
            Console.WriteLine("Program terminated");
        }

        static public void killMainProcess()
        {
            void kill()
            {
                foreach (Process clsProcess in Process.GetProcesses())
                    if (clsProcess.ProcessName == Application.ProductName)
                    { clsProcess.Kill(); Console.WriteLine("Process killed"); }
            }
            new Thread(kill).Start();
        }

        public static void cmd(string cmd, string args, bool isPath = false)
        {
            //Console.Write("Running cmd");
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.Arguments = args;
            if(isPath) start.Arguments = "\""+args+ "\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        public static void cmdAsync(string cmd, string args, bool isPath=false)
        {
            void run()
            {
                //Console.Write("Running cmd async");
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = cmd;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.Arguments = args;
                if (isPath) start.Arguments = "\"" + args + "\"";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                }
            }
            new Thread(run).Start();
        }
        public static DateTime WindowsStartTime()
        {
            DateTime dt = new DateTime();
            try
            {
                dt = DateTime.Now - new TimeSpan(0, 0, 0, 0, System.Environment.TickCount);
                return dt;
            }
            catch (Exception) {return dt; }
        }
    }
}
