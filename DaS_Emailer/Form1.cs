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
        private BackgroundWorker bg;// = new BackgroundWorker();
        private Generator.bg_params _params;

        public Form1()
        {
            InitializeComponent();

            bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(Generator.save);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Generator.completed);

            _params = new Generator.bg_params();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using ( var sfd = new SaveFileDialog() ) {
                sfd.Filter = "out data (*.html)|*.html|All files (*.*)|*.*";
                sfd.FileName = "result";
                sfd.DefaultExt = "html";
                if (sfd.ShowDialog() == DialogResult.OK){
                    toolStripStatusLabel1.Text = "Обработка";
                    _params.form = this;
                    _params.filename = sfd.FileName;
                    bg.RunWorkerAsync(_params);
                }
            }
        }
    }
}
