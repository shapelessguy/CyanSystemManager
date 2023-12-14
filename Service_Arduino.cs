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

namespace CyanSystemManager
{
    static public class Service_Arduino
    {
        static public string title = "arduinoService";
        static public string serviceType = ST.Arduino;
        static public State status = State.OFF;
        static public SerialPort sp;
        static public bool connected;
        static public bool clear;
        static private ArduinoMenu menu;

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
        static public void showMenu() { addCommand(ArduinoCom.SHOW_MENU); }

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

            while (!forceTermination && status != State.OFF)
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
                catch (Exception) { Log("Exception in " + title); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == ArduinoCom.AUDIO_ON) sp.WriteLine("AH");
            else if (command.type == ArduinoCom.AUDIO_ON_KEEP) sp.WriteLine("AK");
            else if (command.type == ArduinoCom.AUDIO_OFF) sp.WriteLine("AL");
            else if (command.type == ArduinoCom.LIGHT_ON) sp.WriteLine("LL");
            else if (command.type == ArduinoCom.LIGHT_OFF) sp.WriteLine("LH");
            else if (command.type == ArduinoCom.SHOW_MENU) ArdShowMenu();
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
            menu = new ArduinoMenu();
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
        static private void ArdShowMenu()
        {
            if (menu.Visible) Program.home.Invoke((MethodInvoker)delegate { menu.Hide(); });
            else Program.home.Invoke((MethodInvoker)delegate {
                try
                {
                    Screen s = Screen.FromPoint(Cursor.Position);
                    // float fill = 0.8f;
                    // Point location = new Point(s.Bounds.X + (int)(s.Bounds.Width * (1 - fill) / 2), s.Bounds.Y + (int)(s.Bounds.Height * (1 - fill) / 2));
                    // Size size = new Size((int)(s.Bounds.Width * fill), (int)(s.Bounds.Height * fill));
                    Size size = new Size(1297, 740);
                    Point location = new Point(s.Bounds.X + (int)((s.Bounds.Width - size.Width) / 2), s.Bounds.Y + (int)((s.Bounds.Height - size.Height) / 2));
                    menu.Show();
                    menu.Location = location;
                    menu.Size = size;
                    SetForegroundWindow(menu.Handle);
                }
                catch (Exception ex) { Log(ex.ToString()); }
            });
        }
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
                Log("Closing port");
                try
                {
                    sp.Close();
                    //Thread.Sleep(1500);
                }
                catch (Exception) { }
                Log("Disposing port");
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
            Log("Opening port: " + portNum);
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
                Log("---------------------\nArduino ID:\n" + id_pass + "\n---------------------");
                if (id_pass == "cyanSystemManager")
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
        // //////////
    }
}
