using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using Timer = System.Threading.Timer;
using System.Management;

namespace CyanSystemManager
{
    public class AudioDevices
    {
        public static AudioDevice getDevice(AT category)
        {
            foreach (AudioDevice device in audioDevices) if (device.category == category) return device;
            return null;
        }
    }
    public class AudioArgs
    {
        public UnmanagedMemoryStream file;
        public int iterations;
        public AudioDevice device;
        public float volume;
        public AudioArgs(UnmanagedMemoryStream audioFile, int iterations, AT category, float volume)
        {
            this.file = audioFile;
            this.iterations = iterations;
            this.device = AudioDevices.getDevice(category);
            this.volume = volume;
        }
    }
    public class AudioDevice
    {
        public AT category;
        public string[] devices;
        public string fusionCommand;
        public AudioDevice(AT category, string[] devices, string fusionCommand)
        {
            this.category = category;
            this.devices = devices;
            this.fusionCommand = fusionCommand;
        }
    }
    public class Command
    {
        public string type = "";
        public object value = null;
        public Command(string type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
    public class AudioInfo
    {
        public int n = 0;
        public MMDevice defaultDevice;
        public AudioEndpointVolumeChannel[] channels;
        public AudioDevice audioDevice = AudioDevices.getDevice(AT.None);
        public float masterVolume;
        public string deviceName = "";
        public bool mute;
        public bool validated = false;
        public bool newDevice = false;
        public AudioInfo() { }
        public AudioInfo(int count, MMDevice device, AudioEndpointVolumeChannel[] channels, float masterVolume, bool mute)
        {
            n = count;
            defaultDevice = device;
            this.channels = channels;
            this.masterVolume = masterVolume;
            this.mute = mute;
        }
    }

    public static class Service_Audio
    {
        static public State status = State.OFF;
        static public bool suppressAllSounds = false;
        static public bool changingDevice = false;
        static public bool changedDevice = false;
        public static List<VolFormHelper> volForms = new List<VolFormHelper>();
        static public AudioInfo audioInfo = new AudioInfo();
        static MMDeviceEnumerator devEnum;
        static private int msCycle = 20;
        static private float baseSpeed = 0.01f;
        static private float fastSpeed = 0.02f;
        static private float limitInc = 0.4f;

        // //////////////   Functions of AudioService
        public static void MasterVolume(string type) { addCommand(type, baseSpeed); }
        public static float GetMasterVolume() { if (status == State.OFF) return -1f; return audioInfo.masterVolume; }
        public static void SetMasterVolume(float volume) { addCommand(AudioCom.SET_VOL, volume); }
        public static AudioDevice GetDevice() { if (status == State.OFF) return AudioDevices.getDevice(AT.None); 
                                                return audioInfo.audioDevice; }
        public static void tempAudio(object args)
        {
            suppressAllSounds = false;
            if (status == State.OFF) {
                void playSound()
                {
                    tempAudioRunning = true;
                    AudioArgs audio = (AudioArgs)args;
                    SoundPlayer sound = new SoundPlayer(audio.file);
                    for (int i = 0; i < audio.iterations; i++) 
                        {  if (suppressAllSounds) { suppressAllSounds = false; break; } sound.PlaySync(); }
                    tempAudioRunning = false;
                }
                if(!tempAudioRunning) new Thread(playSound).Start();
            } 
            else addCommand(AudioCom.TEMPAUDIO, args);
        }
        public static bool SetDevice(AT category) // blocking function!!
        {
            if (status == State.OFF) return false;
            AudioDevice act_device = GetDevice();
            if (category == act_device.category) { changedDevice = true; return true; }
            Console.WriteLine("Changing AudioDevice to "+category);
            addCommand(AudioCom.SET_DEV, category);
            int iter = 0, sleep = 50, lim1 = (2*1000)/sleep, lim2 = (5*1000)/sleep;
            while (!changingDevice && iter < lim1) { Thread.Sleep(sleep); iter++; } // waits for a maximum of 2 seconds
            while (changingDevice && iter < lim2) { Thread.Sleep(sleep); iter++; }  // waits for a maximum of 5 seconds
            if (commands.Count() > 1) for (int i = commands.Count() - 2; i >= 0; i--) commands.RemoveAt(i);
            return changedDevice;
        }
        // //////////////
        static Timer audioThread;
        public static void startService()
        {
            status = State.NEUTRAL;
            Console.WriteLine("Starting audioService..");
            Home.registerHotkeys(ST.Audio);
            if (volForms.Count == 0)
            {
                createVolForms();
                activateAllForms();
                getAudioInfo();

                audioThread = new Timer(audioRun, null, msCycle, Timeout.Infinite);
            }
            status = State.ON;
        }
        public static void stopService() {
            Console.WriteLine("audioService stopped");
            status = State.OFF;
            changingDevice = false;
            changedDevice = false;
            Home.unregisterHotkeys(ST.Audio);
            commands.Clear();
        }
        static void createVolForms() { for (int i = 0; i < 10; i++) volForms.Add(new VolFormHelper(i)); }
        public static void activateAllForms()
        {
            if (status == State.OFF) return;
            for (int i = 0; i < 3; i++) volForms[i].initializeForm(i);
            activateForms();
        }
        static void activateForms()
        {
            if (status == State.OFF) return;
            int nScreens = Screen.AllScreens.Count();
            for (int i = 0; i < nScreens; i++) volForms[i].initializeForm(i);
            for (int i = nScreens; i < 3; i++) volForms[i].destroyForm();
        }

        public static bool tempAudioRunning = false;
        static void playTempAudio(object audio_args)
        {
            AudioArgs args = (AudioArgs)audio_args;
            void runOnDeviceAudio(UnmanagedMemoryStream audioFile)
            {
                if (status == State.OFF || tempAudioRunning) return;
                tempAudioRunning = true;
                try
                {
                    AudioDevice act_device = GetDevice();
                    if (SetDevice(args.device.category))
                    {
                        float vol = GetMasterVolume();
                        SetMasterVolume(args.volume);
                        SoundPlayer sound = new SoundPlayer(audioFile);
                        for (int i = 0; i < args.iterations; i++)
                            { if (suppressAllSounds) { suppressAllSounds = false; break; } sound.PlaySync(); }
                        SetMasterVolume(vol);
                        SetDevice(act_device.category);
                        for (int i = commands.Count - 1; i >= 0; i--)
                            if (commands[i].type == AudioCom.TEMPAUDIO) commands.RemoveAt(i);
                    }
                }
                catch (Exception) { }
                tempAudioRunning = false;
            }
            new Thread(() => runOnDeviceAudio(args.file)).Start();
        }

        static void setDefaultDevice(AT category)
        {
            changingDevice = true;
            changedDevice = false;
            getAudioInfo();
            AudioDevice device = AudioDevices.getDevice(category);
            if (audioInfo.audioDevice == AudioDevices.getDevice(category)){ 
                changedDevice = true; Thread.Sleep(150);
                Console.WriteLine("  --> Actual audio device: " + audioInfo.defaultDevice.FriendlyName); 
                return; 
            }
            cmdAsync(variablePath.displayFusion, "-functionrun " + device.fusionCommand);

            Thread.Sleep(1500);
            for (int i = 0; i < 10; i++)
            {
                getDefaultDevice();
                if (device.devices.Contains(audioInfo.defaultDevice.DeviceFriendlyName)) { changedDevice = true; break; }
                Thread.Sleep(300);
            }
            getAudioInfo();
            changingDevice = false;
            Console.WriteLine("  --> Actual audio device: " + audioInfo.defaultDevice.FriendlyName);
        }

        static Timer timerSeek;
        static bool canSeek = true;
        static void Callback(Object state) { canSeek = true; timerSeek.Dispose(); }
        static void getAudioInfo()
        {
            if(timerSeek!= null) timerSeek.Dispose();
            timerSeek = new Timer(Callback, null, 1000, Timeout.Infinite);
            if (!canSeek) return;
            canSeek = false;
            getDefaultDevice();
            fillInfo();
            audioInfo.validated = true;
            status = State.ON;
        }

        static void getDefaultDevice()
        {
            MMDevice prev_Device = null;
            if(audioInfo!=null && audioInfo.defaultDevice!= null) prev_Device = audioInfo.defaultDevice;
            devEnum = new MMDeviceEnumerator();
            audioInfo = new AudioInfo();
            audioInfo.defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (prev_Device != audioInfo.defaultDevice) audioInfo.newDevice = true;
        }
        static void fillInfo()
        {
            int n = audioInfo.defaultDevice.AudioEndpointVolume.Channels.Count;
            AudioEndpointVolumeChannel[] channels = new AudioEndpointVolumeChannel[n];
            for (int i = 0; i < channels.Length; i++) channels[i] = audioInfo.defaultDevice.AudioEndpointVolume.Channels[i];
            float defaultVolume = getMasterVolume(channels, n);
            audioInfo.masterVolume = defaultVolume;

            bool mute = audioInfo.defaultDevice.AudioEndpointVolume.Mute;
            audioInfo = new AudioInfo(n, audioInfo.defaultDevice, channels, defaultVolume, mute);

            string[] pool = new string[] { "" };
            string name = audioInfo.defaultDevice.DeviceFriendlyName;
            foreach(AudioDevice device in audioDevices)
                if (device.devices.Contains(name)) audioInfo.audioDevice = device;

            audioInfo.newDevice = false;
            audioInfo.deviceName = audioInfo.defaultDevice.FriendlyName;
        }

        static void mapVolume(float masterVolume, AudioEndpointVolumeChannel[] channels, int nChan)
        {
            float fullVol = (float)((int)((masterVolume+0.005f) * 100))/100;
            float halfVol = (float)((int)((masterVolume + 0.005f) * 50))/100;
            if (nChan > 4)
            {
                for (int i = 0; i < 4; i++) channels[i].VolumeLevelScalar = (0.49f*fullVol + 0.5f)*fullVol;
                for (int i = 4; i < nChan; i++) channels[i].VolumeLevelScalar = fullVol;
            }
            else foreach (var chan in channels) chan.VolumeLevelScalar = fullVol;
        }
        static float getMasterVolume(AudioEndpointVolumeChannel[] channels, int nChan)
        {
            float output = channels[nChan-1].VolumeLevelScalar;
            return output;
        }
        private static void printVolumeLevels(AudioEndpointVolumeChannel[] channels)
        {
            string levels = "Levels: ";
            foreach (var channel in channels) levels += (int)(channel.VolumeLevelScalar*100) + " ";
            levels += "  - Master: " + (int)(audioInfo.defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar*100);
            levels += "   .. Device: " + audioInfo.defaultDevice.DeviceFriendlyName;
            Console.WriteLine(levels);
        }

        static DateTime previousCom = DateTime.Now;
        static void Vol(string arg, object value)
        {
            activateForms();
            getAudioInfo();
            float val = (float)value;
            DateTime now = DateTime.Now;
            double totalTime = now.Subtract(previousCom).TotalMilliseconds;
            previousCom = now;
            if (totalTime < 100) val = fastSpeed;

            float prev_masterVol = audioInfo.masterVolume;
            if (arg == AudioCom.VOL_UP) audioInfo.masterVolume += val;
            else if (arg == AudioCom.VOL_DOWN) audioInfo.masterVolume -= val;

            if (audioInfo.masterVolume < 0f) audioInfo.masterVolume = 0f;
            else if (audioInfo.masterVolume > 1f) audioInfo.masterVolume = 1f;

            bool change = audioInfo.masterVolume - prev_masterVol != 0;
            if (change) { audioInfo.defaultDevice.AudioEndpointVolume.Mute = false; audioInfo.mute = false; }

            SetVolume(audioInfo.masterVolume);
        }
        static void noVol()
        {
            getAudioInfo();
            audioInfo.mute = !audioInfo.mute;
            audioInfo.defaultDevice.AudioEndpointVolume.Mute = audioInfo.mute;
            submit();
        }
        static void SetVolume(float volume)
        {
            if(audioInfo == null || audioInfo.channels == null || audioInfo.newDevice) getAudioInfo();
            if (volume < 0f) volume = 0f; else if (volume > 1f) volume = 1f;
            mapVolume(volume, audioInfo.channels, audioInfo.n);
            audioInfo.masterVolume = volume;
            submit();
            //printVolumeLevels(audioInfo.channels);
        }
        static void submit()
        {
            VolFormHelper.messages.Add(new VolSettings(audioInfo.masterVolume, audioInfo.mute, audioInfo.deviceName));
            foreach (var form in volForms) form.tempShow();
        }

        static void addCommand(string type, object value)
        {
            if (status == State.OFF) { return; }
            bool try_ = false;
            if (commands.Count>2 && commands[commands.Count - 1].type == type) 
                try_ = tryToAggregate(commands, value);
            if(!try_) commands.Add(new Command(type, value));
        }

        static bool tryToAggregate(List<Command> commands, object value)
        {
            string type = commands[commands.Count - 1].type;
            float comValue;
            float thisValue;
            try { comValue = (float)commands[commands.Count-1].value; } catch (Exception) { return false; }
            try { thisValue = (float)value*2; } catch (Exception) { return false; }
            List<float> values = new List<float>();
            for (int i = commands.Count - 1; i >= 0; i--)
                if (commands[i].type == type) values.Add((float)commands[i].value); else break;
            int n = values.Count - 1;
            float atom = 2 * thisValue / (n * n);
            int fract = n;
            for (int i = commands.Count - 1; i >= commands.Count - n; i--)
                { 
                commands[i].value = (float)commands[i].value + atom * fract;
                fract--;
                if ((float)commands[i].value > limitInc) commands[i].value = limitInc;
                if ((float)commands[i].value < -limitInc) commands[i].value = -limitInc;
            }
            return true;
        }

        static DateTime prev;
        static List<Command> commands = new List<Command>();
        public static void audioRun(Object ob)
        {
            if (Program.timeToClose) return;
            if (status == State.OFF) { Thread.Sleep(50); audioThread.Change(msCycle, Timeout.Infinite); return; }
            try
            {
                if (commands.Count == 0) { audioThread.Change(msCycle, Timeout.Infinite); return; }
                activateForms();
                Stabilize();
                Command command = commands[0];
                commands.RemoveAt(0);
                if (command.type == AudioCom.SET_VOL) SetVolume((float)command.value);
                else if (command.type == AudioCom.SET_DEV) setDefaultDevice((AT)command.value);
                else if (command.type == AudioCom.VOL_UP || command.type == AudioCom.VOL_DOWN)
                    Vol(command.type, command.value);
                else if (command.type == AudioCom.VOL_NULL) noVol();
                else if (command.type == AudioCom.TEMPAUDIO) playTempAudio(command.value);
            }
            catch (Exception) { Console.WriteLine("Exception in audioRun"); }
            audioThread.Change(msCycle, Timeout.Infinite);
        }

        private static void Stabilize()
        {
            if (prev == null) prev = DateTime.Now;
            long now = (long)DateTime.Now.Subtract(prev).TotalMilliseconds;
            while (now < msCycle * 2 - 10)
            {
                Thread.Sleep((int)(msCycle * 2 - now) - 10);
                now = (long)DateTime.Now.Subtract(prev).TotalMilliseconds;
            }
            prev = DateTime.Now;
        }

    }

}
