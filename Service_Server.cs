using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Net;

namespace CyanSystemManager
{
    class SimpleHttpServer
    {
        private HttpListener listener;
        private Thread serverThread;
        private bool forceTermination = false;
        private string url = "http://localhost:8080/";

        public SimpleHttpServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
        }

        public void Start()
        {
            serverThread = new Thread(threadRun);
            serverThread.Start();
            new Thread(getServerIP).Start();
        }

        private void threadRun()
        {
            listener.Start();
            Program.Log("Server started. Listening on " + url);

            while (!forceTermination)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    if (request.HttpMethod == "POST")
                    {
                        using (System.IO.Stream body = request.InputStream)
                        {
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                            {
                                string postData = reader.ReadToEnd();
                                ProcessRequest(postData);
                                Program.Log($"Received POST data: {postData}");
                            }
                        }
                    }
                    else if (request.HttpMethod == "GET")
                    {
                        string message = request.QueryString["message"];
                        if (!string.IsNullOrEmpty(message))
                        {
                            Program.Log($"Received message: {message}");
                        }
                    }

                    string responseString = "OK";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                catch (Exception)
                {
                    // Console.WriteLine("Exception caught: " + ex.ToString());
                }
            }

            listener.Stop();
            Program.Log("Server stopped.");
        }

        private void getServerIP()
        {
            int server_ip_downloading_period = 60; // in seconds
            int index = 0;
            while (!forceTermination)
            {
                index += 1;
                if (index > 10 * server_ip_downloading_period)
                {
                    try
                    {
                        Console.WriteLine("Assigning");
                        FirebaseClass.AssignServerIP();
                    }
                    catch { }
                    index = 0;
                }
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            forceTermination = true; 
            listener.Stop();
            if (serverThread != null && serverThread.IsAlive)
            {
                serverThread.Join();
            }
        }

        private void ProcessRequest(string request)
        {
            if (request.Contains("UV_LIGHTS_AUTO_"))
            {
                var parts = request.Split('_');
                if (parts.Length > 3)
                {
                    string timePart = parts[3];
                    var timeParts = timePart.Split(':');
                    bool minuteParseSuccess = false;
                    if (timeParts.Length == 2)
                    {
                        bool hourParseSuccess = int.TryParse(timeParts[0], out int hour);
                        minuteParseSuccess = int.TryParse(timeParts[1], out int minute);
                        if (hourParseSuccess && minuteParseSuccess)
                        {
                            home.plant_leds_auto_set(hour, minute);
                            return;
                        }
                    }
                    timePart = parts[4];
                    minuteParseSuccess = int.TryParse(timePart, out int minutes);
                    if (minuteParseSuccess)
                    {
                        home.plants_leds_auto_in_set(minutes);
                        return;
                    }

                }
            }
            else if (request == "UV_LIGHTS_AUTO") home.plant_leds_auto_btn_Click(null, null);
            else if (request == "UV_LIGHTS_ON") home.plant_leds_on_btn_Click(null, null);
            else if (request == "UV_LIGHTS_OFF") home.plant_leds_off_btn_Click(null, null);
            else if (request.Contains("TIMER_"))
            {
                var parts = request.Split('_');
                string minutes = parts[1];
                string title = "";
                if (parts.Length > 2) title = parts[2].Replace("_", " ");
                int.TryParse(minutes, out int minutes_int);
                TimerArgs args = new TimerArgs(minutes_int, title);
                Service_Timer.createTimer(args);
            }
            else if (request == "TV_ON") home.tv_on_btn_Click(null, null);
            else if (request == "TV_OFF") home.tv_off_btn_Click(null, null);
            else if (request == "AUDIO_ON") home.audio_on_btn_Click(null, null);
            else if (request == "AUDIO_OFF") home.audio_off_btn_Click(null, null);
            else if (request == "START_WAITING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "END_WAITING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "START_THINKING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "END_THINKING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "START_SPEAKING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "END_SPEAKING") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "START_ERROR") Service_Display.ShowIndicator(new IndicatorSettings(request));
            else if (request == "END_ERROR") Service_Display.ShowIndicator(new IndicatorSettings(request));
        }
    }

    static public class Service_Server
    {
        static public string title = "ServerService";
        static public string serviceType = ST.None;
        static public State status = State.OFF;
        static public bool clear;
        static SimpleHttpServer server;

        // Functions of Example_Service --> they should be called from outside the service
        //static public void FunctionFromOutside() { addCommand(ExampleCom.EX_COM1); }

        // System is based on the interchange of messages
        static List<Command> commands = new List<Command>();
        static private void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }

        static public void threadRun()
        {
            server = new SimpleHttpServer();
            server.Start();
        }
        static public void Tree(Command command)
        {
            //if (command.type == ExampleCom.EX_COM1) InternalFunction1();
            //else if (command.type == ExampleCom.EX_COM2) InternalFunction2();
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Log("Starting "+ title + "..");

            beforeStart();
            new Thread(threadRun).Start();
            status = State.ON;
        }
        static public void beforeStart() { }
        static public void stopService(bool dispose)
        {
            Log(title + " stopped");
            status = State.OFF;
            server.Stop();
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        // Inside functions
        //static private void InternalFunction1() { Log("Do something"); }
        //static private void InternalFunction2() { Log("Do other stuff"); }
        // //////////
    }
}
