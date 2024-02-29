using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloOpenCvSharp
{
    public partial class AnnotationForm : Form
    {
        public AnnotationForm()
        {
            InitializeComponent();

            this.textBoxInput.KeyDown += new KeyEventHandler(textBoxInput_KeyDown);
        }

        public string AnnotationText => textBoxInput.Text;

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
                e.SuppressKeyPress = true; // 防止发出响声
                this.Close();
            }
        }
    }
}
