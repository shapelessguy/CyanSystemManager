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

namespace CyanSystemManager
{
    static public class Service_Arduino
    {
        static public string title = "arduinoService";
        static public string serviceType = ST.None;
        static public State status = State.OFF;
        static public SerialPort sp;
        static public bool connected = false;

        // Functions of Example_Service --> they should be called from outside the service
        static public void turnAudio(bool on, bool keep=false) 
        { 
            if (on) 
            { 
                if (keep) addCommand(ArduinoCom.AUDIO_ON_KEEP); 
                else addCommand(ArduinoCom.AUDIO_ON);
            }
            else addCommand(ArduinoCom.AUDIO_OFF); 
        }
        static public void turnLight(bool on) { if (on) addCommand(ArduinoCom.LIGHT_ON); else addCommand(ArduinoCom.LIGHT_OFF); }

        // System is based on the interchange of messages
        static List<Command> commands = new List<Command>();
        static private void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }
        // run Example thread -> Interpret commands and call the appropriate functions inside the service
        static int ping_time = 2; // every 2 seconds
        static int ping = 40 * ping_time;
        static public void threadRun()
        {

            //Thread.Sleep(10000);
            //if (!active) { stopService(); }
            //status = State.ON;

            while (!timeToClose && status != State.OFF)
            {
                try
                {
                    ping -= 1;
                    Thread.Sleep(25);
                    if (ping < 0) { pingAudioDevice(); ping = 40 * ping_time; }
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Console.WriteLine("Exception in " + title); }
            }
        }
        static public void Tree(Command command)
        {
            Console.WriteLine(command.type);
            if (command.type == ArduinoCom.AUDIO_ON) sp.WriteLine("AH");
            else if (command.type == ArduinoCom.AUDIO_ON_KEEP) sp.WriteLine("AK");
            else if (command.type == ArduinoCom.AUDIO_OFF) sp.WriteLine("AL");
            else if (command.type == ArduinoCom.LIGHT_ON) sp.WriteLine("LL");
            else if (command.type == ArduinoCom.LIGHT_OFF) sp.WriteLine("LH");
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            InitializeSerialPortT();
            Home.registerHotkeys(serviceType); // register Hotkeys needed by Example_ activities
            Console.WriteLine("Starting "+ title + "..");

            connected = false;

            beforeStart();
            new Thread(threadRun).Start();
        }
        static public void beforeStart() 
        {
        }
        static public void stopService()
        {
            Console.WriteLine(title + " stopped");
            status = State.OFF;
            new Thread(ClosePort).Start();
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
        }
        // Inside functions
        static private bool getAudioState()
        {
            Thread.Sleep(200);
            sp.WriteLine("AS");
            Thread.Sleep(200);
            string state_str = sp.ReadExisting().Replace(".", ",").Replace("\n", "").Replace("\r", "");
            bool state = state_str[state_str.Length - 1] == '1' ? true : false;
            Properties.Settings.Default.audioStatus = state;
            return state;
        }
        static private void pingAudioDevice()
        {
            if (sp != null)
            {
                sp.WriteLine("p");
                Thread.Sleep(200);
            }
        }
        static private void ClosePort()
        {
            if (sp != null)
            {
                Console.WriteLine("Closing port");
                try
                {
                    sp.Close();
                    //Thread.Sleep(1500);
                }
                catch (Exception) { }
                Console.WriteLine("Disposing port");
                try
                {
                    sp.Dispose();
                    //Thread.Sleep(1500);
                }
                catch (Exception) { }
                sp = null;
            }
        }
        static private bool ard_found = false;
        static private void OpenPortThread(object portNum)
        {
            portNum = (int)portNum;
            Console.WriteLine("Opening port: " + portNum);
            try
            {
                SerialPort port = new SerialPort("COM" + portNum, 9600, Parity.None, 8, StopBits.One);
                port.Open();
                Thread.Sleep(200);
                port.WriteLine("Q");
                Thread.Sleep(300);
                string buf = port.ReadExisting();
                string id_pass = buf.Replace(".", ",").Replace("\n", "").Replace("\r", "");
                if (id_pass.Length > 17) id_pass = id_pass.Substring(id_pass.Length - 17, id_pass.Length);
                Console.WriteLine("---------------------\nArduino ID:\n" + id_pass + "\n---------------------");
                if (id_pass == "cyanSystemManager")
                {
                    Console.WriteLine("| Connected to the port " + portNum + "!");
                    connected = true;
                    sp = port;
                    ard_found = true;
                }
                else
                {
                    port.Close();
                    Console.WriteLine("| Connection to the port " + portNum + " failed");
                    ard_found = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("| Connection to the port " + portNum + " failed");
                Console.WriteLine(ex.Message);
                ard_found = false;
            }
        }
        static public bool OpenPort(int portNum)
        {
            ard_found = false;
            Thread thread = new Thread(new ParameterizedThreadStart(OpenPortThread));
            thread.Start(portNum);
            thread.Join(2000);
            return ard_found;
        }

        static private void InitializeSerialPortT()
        {
            ClosePort();
            Console.WriteLine("Inizializing Port");
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
            else stopService();
        }
        // //////////
    }
}
