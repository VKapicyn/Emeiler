using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;

namespace DaS_Emailer
{
    public static class Sender
    {
        public static string email_text;
        public static string[] receiver;
        private static string sender, pass, smtp, port;

        public static void emailConfig(Form1 form)
        {
            StreamReader str = new StreamReader("config.txt");
            string[] config = str.ReadToEnd().Split(';');
            sender = config[0];
            pass = config[1];
            smtp = config[2];
            port = config[3];
            str.Close();
            form.label9.Text = sender;
        }

        public static void sendEmail(object s, DoWorkEventArgs e)
        {
            var form = ((Generator.bg_params)e.Argument).form;
            var eror = 0;
            var procent = receiver.Length / 100;

            form.BeginInvoke(new Action(() => form.toolStripStatusLabel1.Text = "Отправка..."));
            foreach(var rec in receiver){
                try
                { 
                    SmtpClient Smtp = new SmtpClient(smtp, Int32.Parse(port));
                    Smtp.Credentials = new NetworkCredential(sender, pass);
                    MailMessage Message = new MailMessage();
                    Message.From = new MailAddress(sender);
                    Message.To.Add(new MailAddress(rec));
                    Message.Subject = (string)form.Invoke(new Func<String>(() => { return form.textBox5.Text; }));
                    Message.Body = email_text;
                    Message.IsBodyHtml = true;
                    Smtp.Send(Message);
                }
                catch
                {
                    eror++;
                    continue;
                }
                form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
            }
            MessageBox.Show("Готово\nОтправлено: " + (receiver.Length - eror) + "\nОшибок: " + eror);
            e.Result = e.Argument;
        }

        public static void completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var _params = (Generator.bg_params)e.Result;
            _params.form.BeginInvoke(new Action(() => _params.form.progressBar1.Value = 0));
            _params.form.BeginInvoke(new Action(() => _params.form.toolStripStatusLabel1.Text = "Готово"));
        }
    }
}
