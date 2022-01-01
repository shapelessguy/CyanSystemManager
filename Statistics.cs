using System;
using System.Collections.Generic;
using System.IO;
using static CyanSystemManager.Settings;

namespace CyanSystemManager
{
    public class Statistics
    {
        public List<Status> status = new List<Status>();
        public List<DateTime> garancies = new List<DateTime>();
        static string pathLogs = Path.Combine(variablePath.networkPath, "Logs.txt");
        static string pathLogsGar = Path.Combine(variablePath.networkPath, "Logs_gar.txt");
        static string pathLogsCSV = Path.Combine(variablePath.networkPath, "Logs.csv");
        static string pathLogsGarCSV = Path.Combine(variablePath.networkPath, "Logs_gar.csv");

        public Statistics()
        {
            System.Threading.Thread thread = new System.Threading.Thread(Stat);
            thread.Start();
        }
        private void Stat()
        {
            Statistics_Load();
            Program.cmd(variablePath.python, variablePath.pyStatScript, true);
        }
        private void Statistics_Load()
        {
            Console.WriteLine("Start loading");
            if (!File.Exists(pathLogsCSV)) File.CreateText(pathLogsCSV);
            if (File.Exists(pathLogs))
            {
                foreach (string line in File.ReadLines(pathLogs))
                {
                    status.Add(Status.LoadStatus(line));
                }
            }
            Console.WriteLine("Loaded data from logs.txt");
            Console.WriteLine("Start writing on logs.csv");
            using (StreamWriter sw = new StreamWriter(pathLogsCSV))
            {
                sw.WriteLine("Date,modem,wan,persistance");
                foreach (Status stat in status)
                {
                    string stringa = "";
                    stringa += stat.data.ToString() + ",";
                    stringa += stat.modem.ToString() + ",";
                    stringa += stat.wan.ToString() + ",";
                    stringa += stat.persistance.ToString();
                    sw.WriteLine(stringa);
                }
            }
            Console.WriteLine("Wrote on file");

            Console.WriteLine("Loading data from logs_gar");
            if (File.Exists(pathLogsGar))
            {
                foreach (string line in File.ReadLines(pathLogsGar))
                {
                    garancies.Add(new DateTime(
                        Convert.ToInt32(line.Substring(0, 4)),
                        Convert.ToInt32(line.Substring(4, 2)),
                        Convert.ToInt32(line.Substring(6, 2)),
                        Convert.ToInt32(line.Substring(8, 2)),
                        Convert.ToInt32(line.Substring(10, 2)),
                        0
                        ));
                }
            }
            Console.WriteLine("Loaded data");
            Console.WriteLine("Start writing on logs_gar.csv");
            bool Errors = true;
            while (Errors)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(pathLogsGarCSV))
                    {
                        sw.WriteLine("Date");
                        foreach (DateTime time in garancies)
                        {
                            string stringa = time.ToString();
                            sw.WriteLine(stringa);
                        }
                    }

                    Errors = false;
                }
                catch (Exception) { System.Threading.Thread.Sleep(100); }
            }
            Console.WriteLine("Wrote on file logs_gar.csv");
        }
    }
}
