using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace DaS_Emailer
{
    static class Generator
    {
        public static void save(Form1 form, string href)
        {
            StreamWriter sw = new StreamWriter(href);
            string value = createPage(form); 
            sw.WriteLine(value);
            sw.Close();
            MessageBox.Show("Готово!");
        }

        private static string createPage(Form1 form)
        {
            string page = "";

            //записывем заголовок
            StreamReader str = new StreamReader("head.txt");
            string head = str.ReadToEnd(); 
            page += head;

            //считываем параметры для парсинга и билдинга
            string date = (string)form.Invoke(new Func<String>(() => { return form.textBox1.Text; }));
            int from = int.Parse((string)form.Invoke(new Func<String>(() => { return form.textBox2.Text; })));
            int amount = int.Parse((string)form.Invoke(new Func<String>(() => { return form.textBox3.Text; })));

            page = page.Replace("_das_date", date);

            page += getData(from, amount);

            //записываем футер
            str = new StreamReader("footer.txt");
            string footer = str.ReadToEnd();

            page += footer;
            return page;
        }

        private static string getData(int from, int amount)
        {

            return "";
        }
    }
}
