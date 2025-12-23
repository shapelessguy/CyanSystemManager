using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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
        public static string soundview_path;
        public static string log_path = "all_logs.txt";

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(log_path)) { File.WriteAllText(log_path, ""); }
                bool startup = false;
                all_processes = Process.GetProcesses();
                multimonitor_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "MultiMonitorTool", "MultiMonitorTool.exe");
                soundview_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "SoundVolumeView", "SoundVolumeView.exe");
                foreach (string arg in args) if (arg == "startup") startup = true;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                while (restart)
                {
                    restart = false;
                    try
                    {
                        Application.Run(home = new Home(startup));
                    }
                    catch (Exception e) { Log(e.ToString()); }
                    Thread.Sleep(10000);
                    startup = false;
                    forceTermination = false;
                    if (restart)
                    {
                        ConfigurationManager.RefreshSection("appSettings");
                        ConfigurationManager.RefreshSection("configuration");
                        Properties.Settings.Default.Reload();
                        Thread.Sleep(1500);
                    }
                }
                killMainProcess();
                Log("Program terminated");
            }
            catch (Exception) 
            {
                MessageBox.Show("Cyan Manager has encoutered a problem!");
            }
        }

        public class MultiTextWriter : TextWriter
        {
            private readonly TextWriter[] writers;

            public MultiTextWriter(params TextWriter[] writers)
            {
                this.writers = writers;
            }

            public override void Write(char value)
            {
                foreach (var writer in writers)
                {
                    writer.Write(value);
                }
            }

            public override void WriteLine(string value)
            {
                foreach (var writer in writers)
                {
                    writer.WriteLine(value);
                }
            }

            public override Encoding Encoding => Encoding.Default;
        }

        static public void Log(string text, string end=null)
        {
            if (end == null) end = Environment.NewLine;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] {text}";
            try
            {
                File.AppendAllText(log_path, logEntry + end);
                Console.Write(text + end);
            }
            catch { Console.Write("Error while saving logs.\n" + text + end); }
        }

        static public void killMainProcess()
        {
            void kill()
            {
                foreach (Process clsProcess in Process.GetProcesses())
                    if (clsProcess.ProcessName == Application.ProductName)
                    { clsProcess.Kill(); Log("Process killed"); }
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
                    Log(result, "");
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
                        Log(result, "");
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
