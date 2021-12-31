using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyanSystemManager
{
    public static class VolFormGiulietto
    {
        // Remember that you have a drawning space of size -> Size(820, 228)
        // The class VolSettings (in this istance 'set') has all attributes that you need:
        //      - float : volume
        //      - bool : mute
        //      - string : deviceName
        public static void Draw(Graphics g, VolSettings set)
        {
            // create background
            int volTop = 20;
            Rectangle BB = new Rectangle(0, 0, 60, 100+volTop);
            g.FillRectangle(Brushes.Black, BB);

            Rectangle carvingBounds = new Rectangle(BB.X+5, BB.Y- 3, BB.Width-10, BB.Height-6);
            g.FillRectangle(Brushes.DarkGray, carvingBounds);

            // create volume bar
            Rectangle volBounds = new Rectangle(0, (int)((1 - set.volume) * (BB.Height-volTop)), BB.Width, volTop);
            g.FillRectangle(Brushes.Red, volBounds);
        }
    }
}
