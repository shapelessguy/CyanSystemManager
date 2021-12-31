using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace CyanSystemManager
{
    static class Program
    {
        public static Home home;
        public static bool timeToClose = false;
        public static AutoResetEvent reset = new AutoResetEvent(false);

        [STAThread]
        static void Main()
        {
            /*ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject obj in searcher.Get())
            {
                Double temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                temperature = (temperature - 2732) / 10.0;
                Console.WriteLine(temperature);
            }

            return;*/


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(home = new Home());
            Console.WriteLine("Program terminated");
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
            new System.Threading.Thread(run).Start();
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
