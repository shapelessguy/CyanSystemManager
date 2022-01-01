using System;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class NotebookColPal : UserControl
    {
        public NotebookColPal()
        {
            InitializeComponent();
        }

        private void ColPal_MouseEnter(object sender, EventArgs e)
        {
            BorderStyle = BorderStyle.Fixed3D;
        }

        private void ColPal_MouseLeave(object sender, EventArgs e)
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        private void ColPal_Click(object sender, EventArgs e)
        {
            NotebookForm.act_color = BackColor;
            NotebookForm.palClicked = true;
        }
    }
}
