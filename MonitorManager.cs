using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using static CyanSystemManager.Settings;

namespace CyanSystemManager
{
    public class Monitor
    {
        public int id;
        public string deviceName;
        public Screen screen;
        public Point position;
        public Monitor(int id, string deviceName, Point position) { SetMonitor(id, deviceName, position); }
        public Monitor(int id, string deviceName) { SetMonitor(id, deviceName, new Point(0, 0)); }
        private void SetMonitor(int id, string deviceName, Point position)
        {
            this.id = id;
            this.deviceName = deviceName;
            this.position = position;
            UpdateScreen();
        }
        public void UpdateScreen()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.DeviceFriendlyName() == deviceName) { this.screen = screen; return; }
            }
            screen = null;
        }
    }
    public class MonitorManager
    {
        public static void GetMonitors() 
            { foreach (Monitor monitor in actualConfiguration.monitorList) monitor.UpdateScreen(); }
        public static Monitor RefByScreenName(string screenName)
        {
            try
            {
                foreach (Monitor monitor in actualConfiguration.monitorList)
                    if (monitor.screen != null && monitor.screen.DeviceName == screenName) return monitor;
                return null;
            } catch { return null; }
        }

        public static Monitor Ref(int id)
        {
            foreach (Monitor monitor in actualConfiguration.monitorList) if (monitor.id == id) return monitor;
            return null;
        }
        public static Monitor Ref(string name)
        {
            foreach (Monitor monitor in actualConfiguration.monitorList) 
                if (monitor.deviceName == name) return monitor;
            return null;
        }

    }

    public class Position
    {
        public int id;
        public Point point = new Point(0, 0);
        public Position(int id, Point point)
        {
            this.id = id;
            this.point = point;
        }
        public Position(int id) {this.id = id;}
    }


    public class Configuration
    {
        public Config config;
        public List<Monitor> monitorList = new List<Monitor>();
        public List<Position> positions = new List<Position>();
        public string fusionCommand;
        public Configuration(Config conf, List<int> listMonitors, List<Position> pos, string fusionCommand)
        {
            this.config = conf;
            foreach (int id in listMonitors) foreach (Monitor monitor in monitors)
                    if (id == monitor.id) monitorList.Add(monitor);
            foreach (Position position in pos) positions.Add(position);
            this.fusionCommand = fusionCommand;
        }
        public static Configuration getConfiguration(Config conf)
        {
            foreach (Configuration configuration in configurations) if (configuration.config == conf) return configuration;
            return null;
        }
        public static Point getPosition(Config conf, int id)
        {
            foreach (Configuration configuration in configurations) if (configuration.config == conf)
                    foreach (Position pos in configuration.positions) if (pos.id == id) return pos.point;
            return new Point(0, 0);
        }
    }
}
