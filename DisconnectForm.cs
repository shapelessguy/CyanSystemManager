using System.Drawing;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class DisconnectForm : Form
    {
        public DisconnectForm(Bitmap bitmap)
        {
            InitializeComponent();
            this.BackgroundImage = bitmap;
        }
    }
}
