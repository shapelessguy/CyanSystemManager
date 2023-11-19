using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Utility;

namespace CyanSystemManager
{
    public class Service_Network
    {
        static public State status = State.OFF;
        static public DisconnectForm disc_modem;
        static public DisconnectForm disc_wan;
        static DateTime now = DateTime.Now;
        static int[] prev_data = new int[] { now.Year, now.Month, now.Day, now.Hour, now.Minute };
        static int iter_UpdateIP = 61;
        static string pathLogs = Path.Combine(variablePath.networkPath, "Logs.txt");
        static string pathLogsGar = Path.Combine(variablePath.networkPath, "Logs_gar.txt");
        public static void startService()
        {
            Console.WriteLine("Starting netService..");
            listIP = ips.Values.ToList();
            status = State.NEUTRAL;
            disc_modem = new DisconnectForm(Properties.Resources.forbidden);
            disc_wan = new DisconnectForm(Properties.Resources.global);
            Home.setTopAndTransparent(disc_modem.Handle);
            Home.setTopAndTransparent(disc_wan.Handle);

            Thread pingThread = new Thread(PingTimer);
            pingThread.Start();
        }
        public static void stopService() 
        { 
            Console.WriteLine("netService Stopped");
            HideDisconnect();
            status = State.OFF; 
        }

        private static readonly Dictionary<string, string> ips = new Dictionary<string, string>(){
        { "google", "8.8.8.8" },
        { "google_2", "8.8.4.4" },
        { "cisco", "208.67.222.222" },
        { "cisco_2", "208.67.220.220" },
        { "quad9", "9.9.9.9" },
        { "quad9_2", "149.112.112.112" },
        };
        private static List<string> listIP;

        private static void PingTimer()
        {
            int iter = 0;
            Status prev_status = new Status(false, false);
            Status act_status = new Status(false, false);
            Status last_persistentStatus = new Status();
            byte[] buffer = new byte[650];
            bool initialization = true;

            while (!Program.timeToClose)
            {
                if (status == State.OFF) { Thread.Sleep(100); return; }
                iter++;
                Thread.Sleep(150);
                now = DateTime.Now;
                int[] act_data = new int[] { now.Year, now.Month, now.Day, now.Hour, now.Minute };

                if (!act_data.SequenceEqual(prev_data))
                {
                    iter_UpdateIP++;
                    if (iter_UpdateIP > 60) { iter_UpdateIP = 0; FirebaseClass.UploadIP(); }  
                    // upload ip ogni ora ... oppure ogni disconnessione (guarda sotto)
                    prev_data = new int[] { act_data[0], act_data[1], act_data[2], act_data[3], act_data[4] };
                    SaveGarancy(Fill0(act_data[0], 4) + Fill0(act_data[1], 2) + Fill0(act_data[2], 2) + Fill0(act_data[3], 2) + Fill0(act_data[4], 2));
                }
                act_status = new Status(false, false);
                try
                {
                    Ping p1 = new Ping(), p2 = new Ping(); ;
                    bool bP1 = false, bP2 = false;
                    DateTime localDate = DateTime.Now;
                    try
                    {
                        PingReply PR1 = p1.Send(getIP(), 50, buffer, new PingOptions(30, true));
                        bP1 = PR1.Status.ToString() == "Success";
                        p1.Dispose();
                    } catch (Exception) { p1.Dispose(); if (initialization) ShowDisconnect(disc_modem); initialization = false; }
                    try
                    {
                        PingReply PR2 = p2.Send("192.168.1.1", 50);
                        bP2 = PR2.Status.ToString() == "Success";
                        p2.Dispose();
                    } catch (Exception) { p2.Dispose(); if (initialization) ShowDisconnect(disc_wan); }
                    initialization = false;
                    act_status = new Status(bP2, bP1);

                    if (act_status.modem == true && act_status.wan == true) { HideDisconnect(); }
                    if (act_status.modem == prev_status.modem && act_status.wan == prev_status.wan) { act_status = prev_status; act_status.IncPers(); }


                    if (iter == 1) { act_status.print(); iter = 0; }
                    if (act_status.isOnEdge() && (act_status.modem != last_persistentStatus.modem || act_status.wan != last_persistentStatus.wan))
                    {
                        last_persistentStatus = act_status;
                        last_persistentStatus.data = DateTime.Now;
                        SaveLog(last_persistentStatus.verbose());
                        act_status.print();
                        if (act_status.modem == false && act_status.wan == false)
                        {
                            ShowDisconnect(disc_modem);
                            if (disc_wan.Visible) Program.home.Invoke((MethodInvoker)delegate { disc_wan.Hide(); });
                        }
                        else if (act_status.modem == true && act_status.wan == false)
                        {
                            ShowDisconnect(disc_wan);
                            if (disc_modem.Visible) Program.home.Invoke((MethodInvoker)delegate { disc_modem.Hide(); });
                        }
                        else
                        {
                            HideDisconnect();
                            FirebaseClass.UploadIP();
                        }
                    }

                    prev_status = act_status;
                    if (status == State.OFF) { Thread.Sleep(100); continue; }
                    status = State.ON;
                    continue;
                }
                catch (Exception) { Console.WriteLine("Exception"); }
            }
            Program.home.Invoke((MethodInvoker)delegate { disc_wan.Hide(); });
            Program.home.Invoke((MethodInvoker)delegate { disc_modem.Hide(); });
        }

        private static string Fill0(int input, int num)
        {
            string stringa = Convert.ToString(input);
            int num_zeros = num - stringa.Length;
            for (int i = 0; i < num_zeros; i++) stringa = "0" + stringa;
            return stringa;
        }

        private static void HideDisconnect()
        {
            if (disc_modem.Visible) Program.home.Invoke((MethodInvoker)delegate { disc_modem.Hide(); });
            if (disc_wan.Visible) Program.home.Invoke((MethodInvoker)delegate { disc_wan.Hide(); });
        }

        private static void ShowDisconnect(DisconnectForm form)
        {
            if (!form.Visible)
            {
                Program.home.Invoke((MethodInvoker)delegate
                {
                    form.Show();
                    form.Location = new System.Drawing.Point(
                        Screen.PrimaryScreen.WorkingArea.Width - 100 + Screen.PrimaryScreen.WorkingArea.X,
                        Screen.PrimaryScreen.WorkingArea.Height - 100 + Screen.PrimaryScreen.WorkingArea.Y);
                    form.Size = new System.Drawing.Size(100, 100);
                });
            }
        }
        private static void SaveLog(string stringa)
        {
            //Console.WriteLine("Saving in Logs.txt");
            if (!File.Exists(pathLogs)) File.CreateText(pathLogs);
            File.AppendAllText(pathLogs, stringa + Environment.NewLine);
        }
        private static void SaveGarancy(string stringa)
        {
            if (!File.Exists(pathLogsGar)) File.CreateText(pathLogsGar);
            try { File.AppendAllText(pathLogsGar, stringa + Environment.NewLine); } catch (Exception) { }

        }

        static int index = 0;
        private static string getIP()
        {
            if (index + 1 < listIP.Count) index += 1;
            else index = 0;
            //foreach (var value in listIP) Console.WriteLine(value);
            return listIP[index];
        }
    }



    public class Status
    {
        public bool modem = false;
        public bool wan = false;
        public DateTime data;
        public int persistance = 0;

        public bool canPrint = false;
        int threshold = 5;
        public bool final = false;
        public Status() { }
        public Status(bool modem, bool wan, DateTime data = new DateTime(), int persistance = 0)
        {
            this.modem = modem;
            this.wan = wan;
            if (data.Year == 1) data = DateTime.Now;
            this.data = data;
            this.persistance = persistance;
        }
        public void print()
        {
            if (canPrint) Console.WriteLine(verbose());
        }
        public string verbose()
        {
            string modem_str = "OFF", wan_str = "OFF";
            if (modem) modem_str = "ON";
            if (wan) wan_str = "ON";
            return "Modem: " + modem_str + " WAN: " + wan_str + "  -> time: " + data + ", persistance = " + persistance;
        }
        public static Status LoadStatus(string line)
        {
            line = line.Split(new string[] { "Modem: ", }, StringSplitOptions.RemoveEmptyEntries)[0];
            string modem_str = line.Split(new string[] { " WAN: ", }, StringSplitOptions.RemoveEmptyEntries)[0];
            line = line.Split(new string[] { " WAN: ", }, StringSplitOptions.RemoveEmptyEntries)[1];
            string wan_str = line.Split(new string[] { "  -> time: ", }, StringSplitOptions.RemoveEmptyEntries)[0];
            line = line.Split(new string[] { "  -> time: ", }, StringSplitOptions.RemoveEmptyEntries)[1];
            string data_str = line.Split(new string[] { ", persistance = ", }, StringSplitOptions.RemoveEmptyEntries)[0];
            string persistance_str = line.Split(new string[] { ", persistance = ", }, StringSplitOptions.RemoveEmptyEntries)[1];

            bool modem = modem_str == "ON";
            bool wan = wan_str == "ON";
            DateTime data = DateTime.Parse(data_str);
            int persistance = Convert.ToInt32(persistance_str);
            return new Status(modem, wan, data, persistance);
        }
        public void IncPers()
        {
            persistance++;
            if (persistance > 2 * threshold) { persistance = threshold + 1; final = true; }
        }
        public bool isOnEdge()
        {
            return (persistance == threshold || (modem && wan));
        }

    }
}
