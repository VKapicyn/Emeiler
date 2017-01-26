using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaS_Emailer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using ( var sfd = new SaveFileDialog() ) {
                sfd.Filter = "out data (*.html)|*.html|All files (*.*)|*.*";
                sfd.FileName = "result";
                sfd.DefaultExt = "html";
                if (sfd.ShowDialog() == DialogResult.OK){
                    Generator.save(this, sfd.FileName);
                }
            }
        }

    }
}
