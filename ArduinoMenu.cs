using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class ArduinoMenu : Form
    {
        Size defSize = new Size(0, 0);
        Size defBtnSize = new Size(0, 0);
        private static readonly HttpClient client = new HttpClient();
        public ArduinoMenu()
        {
            InitializeComponent();
            button1.BackgroundImage = Properties.Resources.neon_on;
            button2.BackgroundImage = Properties.Resources.neon_off;
            button4.BackgroundImage = Properties.Resources.auto;
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button4.BackgroundImageLayout = ImageLayout.Stretch;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm"; // Only use hours and minutes
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = new DateTime(2012, 05, 28, 22, 0, 0);
            defSize = Size;
            defBtnSize = panel1.Size;
        }

        private void ArduinoMenu_Load(object sender, EventArgs e)
        {

        }

        private async Task<bool> sendHTTP(string topic, string arg)
        {
            try
            {
                var values = new Dictionary<string, string>
                  {
                      { topic, arg },
                  };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://" + FirebaseClass.serverIp + ":10001/" + topic, content);

                using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1")))
                {
                    var responseString = sr.ReadToEnd();
                    Program.Log(responseString);
                }
            }
            catch (Exception ex) { Program.Log(ex.Message); return false; }
            return true;
        }

        private void emitSound()
        {
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Default.wav"))
            {
                soundPlayer.Play();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "on");
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "off");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            emitSound();
            await sendHTTP("lights", "auto"); 
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string hour = dateTimePicker1.Value.Hour.ToString();
            hour = hour.Length == 1 ? "0" + hour : hour;
            string minute = dateTimePicker1.Value.Minute.ToString();
            minute = minute.Length == 1 ? "0" + minute : minute;
            string arg = "auto " + hour + ":" + minute;
            emitSound();
            await sendHTTP("lights", arg);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
