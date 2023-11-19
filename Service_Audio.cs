﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using static CyanSystemManager.Settings;
using static CyanSystemManager.Program;
using static CyanSystemManager.Utility;
using Timer = System.Threading.Timer;
using System.Security.Cryptography;
using NAudio.Wave;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
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
        public AudioDevice(AT category, string[] devices = null)
        {
            this.category = category;
            this.devices = devices == null? new string[] { } : devices;
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
        static public bool clear;
        static public bool suppressAllSounds;
        static public bool changingDevice;
        static public bool changedDevice;
        public static List<VolFormHelper> volForms;
        static public AudioInfo audioInfo;
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
            Console.WriteLine("playing " + status);
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
            Console.WriteLine("Changing AudioDevice to " + category);
            addCommand(AudioCom.SET_DEV, category);
            int iter = 0, sleep = 50, lim1 = (2*1000)/sleep, lim2 = (5*1000)/sleep;
            while (!changingDevice && iter < lim1) { Thread.Sleep(sleep); iter++; } // waits for a maximum of 2 seconds
            while (changingDevice && iter < lim2) { Thread.Sleep(sleep); iter++; }  // waits for a maximum of 5 seconds
            if (commands.Count() > 1) for (int i = commands.Count() - 2; i >= 0; i--) commands.RemoveAt(i);
            return changedDevice;
        }
        // //////////////
        static Timer audioThread;
        static IEnumerable<CoreAudioDevice> all_devices;
        public static void startService()
        {
            suppressAllSounds = false;
            changingDevice = false;
            changedDevice = false;
            volForms = new List<VolFormHelper>();
            audioInfo = new AudioInfo();

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
        public static void stopService(bool dispose) {
            Console.WriteLine("audioService stopped");
            status = State.OFF;
            changingDevice = false;
            changedDevice = false;
            Home.unregisterHotkeys(ST.Audio);
            commands.Clear();
            clear = true;
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
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }
                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        static MemoryStream ms = null, ms2 = null;
        static WaveStream ws = null, ws2 = null;
        static WaveOutEvent output = null, output2 = null;
        public static void Media_Ended()
        {
            if (ms != null)
            {
                ms.Close();
                ms.Flush();
                ms.Dispose();
            }
            if (ws != null)
            {
                ws.Close();
                ws.Dispose();
            }
            if (ms2 != null)
            {
                ms2.Close();
                ms2.Flush();
                ms2.Dispose();
            }
            if (ws2 != null)
            {
                ws2.Close();
                ws2.Dispose();
            }
            if (output != null)
            {
                output.Dispose();
            }
            if (output2 != null)
            {
                output2.Dispose();
            }
        }
        static void playTempAudio(object audio_args)
        {
            AudioArgs args = (AudioArgs)audio_args;
            void runOnDeviceAudio(UnmanagedMemoryStream audioFile, AudioDevice device)
            {
                if (status == State.OFF || tempAudioRunning) return;
                tempAudioRunning = true;
                int device_num = -1, main_dev_num = -1;
                string dev_name = "", main_dev_name = "";
                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    if (main_dev_num == -1)
                    {
                        WaveOutCapabilities WOC = WaveOut.GetCapabilities(i);
                        foreach (var dev_opt in audioInfo.audioDevice.devices)
                        {
                            if (dev_opt.Contains(WOC.ProductName)) { main_dev_num = i; main_dev_name = dev_opt; break; }
                        }
                    }
                }
                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    WaveOutCapabilities WOC = WaveOut.GetCapabilities(i);
                    foreach (var dev_opt in device.devices)
                    {
                        if (dev_opt.Contains(WOC.ProductName)) { device_num = i; dev_name = dev_opt; break; }
                    }
                }
                if (device_num == -1) return;

                double prev_vol = -1;
                CoreAudioDevice audio_device = null;
                CoreAudioDevice main_audio_device = null;
                foreach (var d in all_devices)
                {
                    if (d.FullName == dev_name) { audio_device = d; }
                    if (d.IsDefaultDevice) { main_audio_device = d; Console.WriteLine(main_audio_device); }
                }
                prev_vol = audio_device.Volume;
                audio_device.Volume = 20;
                try
                {
                    for (int i = 0; i < args.iterations; i++)
                    {
                        if (suppressAllSounds)
                        {
                            suppressAllSounds = false;
                            break;
                        }
                        if (main_audio_device != audio_device)
                        {
                            Console.WriteLine("CURRENT MAIN AUDIO --> " + main_dev_name);
                            ms = new MemoryStream(ReadToEnd(audioFile));
                            ws = new WaveFileReader(ms);
                            output = new WaveOutEvent();
                            output.DeviceNumber = main_dev_num;
                            output.Init(ws);
                            output.Play();
                        }

                        Console.WriteLine("PRIMARY AUDIO --> " + dev_name);
                        ms2 = new MemoryStream(ReadToEnd(audioFile));
                        ws2 = new WaveFileReader(ms2);
                        output2 = new WaveOutEvent();
                        output2.DeviceNumber = device_num;
                        output2.Init(ws2);
                        output2.Play();

                        if (output != null) while (output.PlaybackState != PlaybackState.Stopped) Thread.Sleep(50);
                        while (output2.PlaybackState != PlaybackState.Stopped) Thread.Sleep(50);
                        Media_Ended();
                    }
                    for (int i = commands.Count - 1; i >= 0; i--)
                        if (commands[i].type == AudioCom.TEMPAUDIO) commands.RemoveAt(i);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                if (prev_vol != -1) audio_device.Volume = prev_vol;
                tempAudioRunning = false;
            }
            new Thread(() => runOnDeviceAudio(args.file, args.device)).Start();
        }

        static void setDefaultDevice(AT category)
        {
            changingDevice = true;
            changedDevice = false;
            getAudioInfo();
            AudioDevice device = AudioDevices.getDevice(category);
            if (audioInfo.audioDevice == AudioDevices.getDevice(category))
            {
                changedDevice = true; Thread.Sleep(150);
                Console.WriteLine("  --> Current audio device: " + audioInfo.defaultDevice.FriendlyName);
                return;
            }

            string device_name = "";
            foreach (var dev in all_devices)
            {
                if (device_name == "")
                {
                    foreach (var dev_opt in device.devices)
                    {
                        if (dev.FullName == dev_opt)
                        {
                            device_name = dev.FullName;
                            dev.SetAsDefault();
                            break;
                        }
                    }
                }
            }

            if (device_name != "")
            {
                changedDevice = true;
            }
            else
            {
                Console.WriteLine("No device of category " + device.category + " has been found!");
            }
            getDefaultDevice();
            getAudioInfo();
            Console.WriteLine("  --> Current audio device: " + audioInfo.defaultDevice.FriendlyName);
            changingDevice = false;
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
            string name = audioInfo.defaultDevice.FriendlyName;
            foreach (AudioDevice device in audioDevices)
            {
                foreach (var dev_opt in device.devices)
                {
                    if (dev_opt == name) { audioInfo.audioDevice = device; break; }
                }
            }

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
        static int i = 0; static int i_to = (int)(1000 / msCycle);
        static int n_devices = 0;
        public static void audioRun(Object ob)
        {
            if (Program.forceTermination) return;
            if (status == State.OFF) { Thread.Sleep(50); audioThread.Change(msCycle, Timeout.Infinite); return; }
            try
            {
                if (i == i_to)
                {
                    i = 0;
                    int n = (new MMDeviceEnumerator()).EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Count;
                    if (n != n_devices) { n_devices = n; audioDeviceDiscovery(); }
                }
                else { i += 1; }
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
        public static void audioDeviceDiscovery()
        {
            try
            {
                all_devices = new CoreAudioController().GetPlaybackDevices();
                Console.WriteLine("New audio devices");
            }
            catch (Exception) { Console.WriteLine("Exception in audioDeviceDiscovery"); }
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
