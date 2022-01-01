using System;
using System.Windows.Forms;

namespace CyanSystemManager
{
    public partial class NotebookStyleT : UserControl
    {
        public NotebookStyleT()
        {
            InitializeComponent();
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            BorderStyle = BorderStyle.Fixed3D;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            BorderStyle = BorderStyle.FixedSingle;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            NotebookForm.act_style = label1.Font.Style;
            NotebookForm.styClicked = true;
        }

        private void StyleT_Load(object sender, EventArgs e)
        {
            label1.Font = Font;
            label1.Text = Name.Substring(0, 1);
            label1.Size = Size;
        }
    }
}
