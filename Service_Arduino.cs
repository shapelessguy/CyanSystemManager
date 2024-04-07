using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using System.Windows.Forms;
using System.IO.Ports;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Interop;
using Newtonsoft.Json.Linq;

namespace CyanSystemManager
{
    class Session
    {

        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);


        string session_name = "DEFAULT";
        int auto_minutes = 0;
        public Session() { }
        public void changeSession(string session_name)
        {
            this.session_name = session_name;
            Service_Display.ShowMsg(new MsgSettings("Session: " + session_name));
            auto_minutes = 0;
        }
        public void on()
        {
            if (session_name == "DEFAULT")
            {
                home.audio_on_off_btn_Click(null, null);
            }
            else if (session_name == "LIGHTS UV")
            {
                home.plant_leds_on_btn_Click(null, null);
            }
            else if (session_name == "TV")
            {
                home.tv_on_btn_Click(null, null);
            }
        }
        public void off()
        {
            if (session_name == "DEFAULT")
            {
                home.audio_effect_btn_Click(null, null);
            }
            else if (session_name == "LIGHTS UV")
            {
                home.plant_leds_off_btn_Click(null, null);
            }
            else if (session_name == "TV")
            {
                home.tv_off_btn_Click(null, null);
            }
        }
        public void up()
        {
            if (session_name == "DEFAULT")
            {
                Service_Audio.MasterVolume(AudioCom.VOL_UP);
                Service_Audio.MasterVolume(AudioCom.VOL_UP);
                Service_Audio.MasterVolume(AudioCom.VOL_UP);
            }
            else if (session_name == "LIGHTS UV")
            {
                auto_minutes += 30;
                Service_Display.ShowMsg(new MsgSettings("AUTO: " + auto_minutes.ToString() + " min"));
            }
        }
        public void down()
        {
            if (session_name == "DEFAULT")
            {
                Service_Audio.MasterVolume(AudioCom.VOL_DOWN);
                Service_Audio.MasterVolume(AudioCom.VOL_DOWN);
                Service_Audio.MasterVolume(AudioCom.VOL_DOWN);
            }
            else if (session_name == "LIGHTS UV")
            {
                auto_minutes -= 30;
                Service_Display.ShowMsg(new MsgSettings("AUTO: " + auto_minutes.ToString() + " min"));
            }
        }
        public void ok()
        {
            if (session_name == "DEFAULT")
            {
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
                Service_Display.ShowMsg(new MsgSettings("PLAY / PAUSE"));
            }
            else if (session_name == "LIGHTS UV")
            {
                home.plants_leds_auto_in_set(auto_minutes);
            }
        }
    }
    static public class Service_Arduino
    {
        static public string title = "arduinoService";
        static public string serviceType = ST.Arduino;
        static public State status = State.OFF;
        static public SerialPort sp;
        static public bool connected;
        static public bool clear;
        static private Session cur_session = new Session();

        // Functions of Example_Service --> they should be called from outside the service

        // System is based on the interchange of messages
        static List<Command> commands = new List<Command>();
        static private void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }
        // run Example thread -> Interpret commands and call the appropriate functions inside the service
        static public void threadRun()
        {
            // List<string> values = new List<string>();
            long index = 0;
            long prev_value_idx = -1;

            while (!forceTermination && status != State.OFF)
            {
                try
                {
                    index += 1;
                    Thread.Sleep(25);
                    string value = readDevice();
                    if (value != null)
                    {
                        if (index - prev_value_idx <= 5)
                        {
                            continue;
                        }
                        else
                        {
                            handleValue(value);
                        }
                        prev_value_idx = index;
                    }
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Log("Exception in " + title); }
            }
        }
        static public void Tree(Command command)
        {
            // if (command.type == ArduinoCom.AUDIO_ON) sp.WriteLine("AH");
            // else if (command.type == ArduinoCom.AUDIO_ON_KEEP) sp.WriteLine("AK");
            // else if (command.type == ArduinoCom.AUDIO_OFF) sp.WriteLine("AL");
            // else if (command.type == ArduinoCom.LIGHT_ON) sp.WriteLine("LL");
            // else if (command.type == ArduinoCom.LIGHT_OFF) sp.WriteLine("LH");
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            InitializeSerialPortT();
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Log("Starting "+ title + "..");

            connected = false;

            beforeStart();
            new Thread(threadRun).Start();
        }
        static public void beforeStart()
        {
        }
        static public void stopService(bool dispose)
        {
            Log(title + " stopped");
            status = State.OFF;
            new Thread(ClosePort).Start();
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        // Inside functions
        static private string readDevice()
        {
            if (sp != null)
            {
                string msg = sp.ReadExisting().Replace("\n", "").Replace("\r", "");
                if (msg != null && msg != "") return msg;
            }
            return null;
        }
        static private void ClosePort()
        {
            if (sp != null)
            {
                Log("Closing port");
                try
                {
                    sp.Close();
                }
                catch (Exception) { }
                Log("Disposing port");
                try
                {
                    sp.Dispose();
                }
                catch (Exception) { }
                sp = null;
            }
        }
        static private bool ard_found = false;
        static private void OpenPortThread(object portNum)
        {
            portNum = (int)portNum;
            Log("Opening port: " + portNum);
            try
            {
                SerialPort port = new SerialPort("COM" + portNum, 9600, Parity.None, 8, StopBits.One);
                port.Open();
                Thread.Sleep(100);
                port.WriteLine("H");
                Thread.Sleep(100);
                string buf = "";
                int attempts = 10;
                for (int i=0; i < attempts; i++)
                {
                    buf += port.ReadExisting();
                    if (buf != "") break;
                    Thread.Sleep(100); // Give some time for data to arrive
                }
                string id_pass = buf.Replace(".", ",").Replace("\n", "").Replace("\r", "");
                if (id_pass == "H received")
                {
                    Log("| Connected to the port " + portNum + "!");
                    connected = true;
                    sp = port;
                    ard_found = true;
                }
                else
                {
                    port.Close();
                    Log("| Connection to the port " + portNum + " failed");
                    ard_found = false;
                }
            }
            catch (Exception ex)
            {
                Log("| Connection to the port " + portNum + " failed");
                Log(ex.Message);
                ard_found = false;
            }
        }
        static public bool OpenPort(int portNum)
        {
            ard_found = false;
            Thread thread = new Thread(new ParameterizedThreadStart(OpenPortThread));
            thread.Start(portNum);
            thread.Join(2000);
            thread.Abort();
            return ard_found;
        }

        static private void InitializeSerialPortT()
        {
            ClosePort();
            Log("Inizializing Port");
            connected = false;
            bool arduinoFound = false;
            for (int try_ = 0; try_ < 3; try_++)
            {
                arduinoFound = OpenPort(Properties.Settings.Default.arduinoDefPort);
                if (arduinoFound) break;
            }
            if (!arduinoFound)
            {
                for (int portNum = 0; portNum < 15; portNum++)
                {
                    if (portNum != Properties.Settings.Default.arduinoDefPort)
                    {
                        if (OpenPort(portNum)) { Properties.Settings.Default.arduinoDefPort = portNum; Properties.Settings.Default.Save(); break; }
                    }
                }
            }
            if (connected) status = State.ON;
            else stopService(false);
        }

        private static void handleValue(string value)
        {
            if (value == "1")
            {
                cur_session.changeSession("LIGHTS UV");
            }
            else if (value == "2")
            {

            }
            else if (value == "3")
            {
                cur_session.changeSession("TV");
            }
            else if (value == "4")
            {

            }
            else if (value == "5")
            {

            }
            else if (value == "6")
            {

            }
            else if (value == "7")
            {

            }
            else if (value == "8")
            {

            }
            else if (value == "9")
            {

            }
            else if (value == "*")
            {
                cur_session.off();
            }
            else if (value == "0")
            {
                cur_session.changeSession("DEFAULT");
            }
            else if (value == "#")
            {
                cur_session.on();
            }
            else if (value == "left")
            {

            }
            else if (value == "right")
            {

            }
            else if (value == "up")
            {
                cur_session.up();
            }
            else if (value == "down")
            {
                cur_session.down();
            }
            else if (value == "ok")
            {
                cur_session.ok();
            }
        }
        // //////////
    }
}
