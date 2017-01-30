using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DaS_Emailer
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bgGen, bgSend;
        private Generator.bg_params _params;

        public Form1()
        {
            InitializeComponent();
            Sender.emailConfig(this);

            bgGen = new BackgroundWorker();
            bgGen.DoWork += new DoWorkEventHandler(Generator.save);
            bgGen.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Generator.completed);

            bgSend = new BackgroundWorker();
            bgSend.DoWork += new DoWorkEventHandler(Sender.sendEmail);
            bgSend.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Sender.completed);//нет, это не опечатка

            _params = new Generator.bg_params();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using ( var sfd = new SaveFileDialog() ) {
                sfd.Filter = "out data (*.html)|*.html|All files (*.*)|*.*";
                sfd.FileName = "result";
                sfd.DefaultExt = "html";
                if (sfd.ShowDialog() == DialogResult.OK){
                    toolStripStatusLabel1.Text = "Обработка...";
                    _params.form = this;
                    _params.filename = sfd.FileName;
                    bgGen.RunWorkerAsync(_params);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "html files (*.html)|*.html|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Sender.email_text = File.ReadAllText(ofd.FileName);
                    label7.Text = "Выбрано";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Sender.receiver = File.ReadAllText(ofd.FileName).Split(',');
                    label6.Text = Sender.receiver.Length.ToString();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _params.form = this;
            bgSend.RunWorkerAsync(_params);
        }
    }
}
