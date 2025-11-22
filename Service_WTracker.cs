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
using System.Drawing;
using Vanara.PInvoke;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using static Vanara.PInvoke.Gdi32;
using System.Text.RegularExpressions;

namespace CyanSystemManager
{
    static public class Service_WTracker
    {
        static public string title = "wTrackerService";
        static public string serviceType = ST.None;
        static public State status = State.OFF;
        static public bool clear;
        static Bitmap bitmap;
        static Bitmap croppedBitmap;

        // Functions of WTracker_Service --> they should be called from outside the service
        // static public void FunctionFromOutside() { addCommand(WTrackerCom.EX_COM1); }

        // System is based on the interchange of messages
        static List<Command> commands = new List<Command>();
        static private void addCommand(string type, object value = null)
        {
            if (status == State.OFF) { return; }
            commands.Add(new Command(type, value));
        }
        // run WTracker thread -> Interpret commands and call the appropriate functions inside the service
        static public void threadRun()
        {
            int iter = 0;
            while (!forceTermination && status != State.OFF)
            {
                try
                {
                    iter++;
                    Thread.Sleep(25);
                    if (iter == 120)
                    {
                        commands.Add(new Command(WTrackerCom.CHECKWIN, null));
                        iter = 0;
                    }
                    if (commands.Count == 0) continue;
                    Command command = commands[0];
                    commands.RemoveAt(0);
                    Tree(command);
                }
                catch (Exception) { Log("Exception in "+title); }
            }
        }
        static public void Tree(Command command)
        {
            if (command.type == WTrackerCom.CHECKWIN) CheckWindows();
        }
        // /////////////
        static public void startService()
        {
            status = State.NEUTRAL;
            Home.registerHotkeys(serviceType); // register Hotkeys needed by WTracker_ activities
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
            Home.unregisterHotkeys(serviceType);
            commands.Clear();
            clear = true;
        }
        // Inside functions

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        static private void CheckWindows()
        {
            bool match = false;
            try
            {
                IntPtr hWnd = Window.getHandle(WindowWrapper.GetOpenWindows(), App.getApplications()["Outlook"]);
                if (hWnd != IntPtr.Zero)
                {
                    Rectangle lpRect = new Rectangle();
                    Window.GetWindowRect(hWnd, ref lpRect);
                    int x = lpRect.Left, y = lpRect.Top, width = lpRect.Width - x, height = lpRect.Height - y;
                    bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        IntPtr hDC = graphics.GetHdc();
                        try
                        {
                            PrintWindow(hWnd, hDC, 0);
                        }
                        finally
                        {
                            graphics.ReleaseHdc(hDC);
                        }
                    }
                    int portion_height = 20;
                    int pass_length = 120;
                    try
                    {
                        /*
                        for (int i=0; i < 300; i++)
                        {
                            if (ComparePasswordOutlook(bitmap, Properties.Resources.passPlaceholder, width, height, pass_length, portion_height, i))
                            {
                                Console.WriteLine(i.ToString());
                            }
                        }
                        */
                        if (ComparePasswordOutlook(bitmap, width, height, pass_length, portion_height, 13)) match = true;
                        else if (ComparePasswordOutlook(bitmap, width, height, pass_length, portion_height, 101)) match = true;
                        else if (ComparePasswordOutlook(bitmap, width, height, pass_length, portion_height, 280)) match = true;
                    }
                    catch {}
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

            GC.Collect();
            if (match) Service_Display.ShowIndicator(new IndicatorSettings("START_OUTLOOK"));
            else Service_Display.ShowIndicator(new IndicatorSettings("END_OUTLOOK"));
        }

        static bool ComparePasswordOutlook(Bitmap parent_image, int w_width, int w_height, int pass_length, int portion_height, int right_offset)
        {
            int portion_width = pass_length + right_offset;
            Rectangle cropArea = new Rectangle(w_width - portion_width, w_height - portion_height, pass_length, portion_height - 3);
            croppedBitmap = parent_image.Clone(cropArea, parent_image.PixelFormat);
            // croppedBitmap.Save("pass_images//pass" + right_offset.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            // ShowCapturedImage(croppedBitmap);
            if (CompareBitmapsFast(croppedBitmap, Properties.Resources.passPlaceholder)) return true;
            return false;
        }

        static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
            {
                return false; // Different sizes mean they are not identical
            }

            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        return false; // Found a pixel that is not the same
                }
            }
            return true; // No pixels were different
        }

        static void ShowCapturedImage(Bitmap image)
        {
            // Display the captured image in a PictureBox
            Form form = new Form();
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.Dock = DockStyle.Fill;
            form.Controls.Add(pictureBox);
            form.ShowDialog();
        }
        // //////////
    }
}
