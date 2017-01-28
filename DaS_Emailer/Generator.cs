using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using AngleSharp.Parser.Html;
using System.Net;
using System.ComponentModel;

namespace DaS_Emailer
{
    static class Generator
    {
        public struct bg_params
        {
            public Form1 form;
            public string filename;
        }

        public static void save(object sender, DoWorkEventArgs e)//Form1 form, string href)
        {
            var _params = (bg_params)e.Argument;

            StreamWriter sw = new StreamWriter(_params.filename);
            string value = createPage(_params.form); 
            sw.WriteLine(value);
            sw.Close();
            e.Result = _params;
            //MessageBox.Show("Готово!");
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

            page += getData(from, amount, form);

            //записываем футер
            str = new StreamReader("footer.txt");
            string footer = str.ReadToEnd();

            page += footer;
            return page;
        }

        private static string getData(int from, int amount, Form1 form)
        {
            //узлы, которык необходимо извлечь с сайта
            List<string> header_text = new List<string>();
            List<string> header_link = new List<string>();
            List<string> date = new List<string>();
            List<string> img_link = new List<string>();
            List<string> text = new List<string>();
            List<string> button_link = new List<string>();

            var document = new HtmlParser().Parse(WebRequest.Create("http://da-strateg.ru/io/novosti-io/").GetResponse().GetResponseStream());

            #region парсинг DOM'а (Оптимзировать!!!)
            int procent = 20/amount;
            //парсинг заголовка и ссылки на него
            try
            {
                var i = 0;
                foreach (var element in document.QuerySelectorAll("article > header > h2 > a"))
                {
                    i++;
                    if (from <= i && (i - from) <= amount)
                    {
                        header_link.Add(element.GetAttribute("href"));
                        header_text.Add(element.InnerHtml);
                        form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
                    }
                    else if ((i - from) > amount)
                        break;
                }

                //парсинг даты
                i = 0;
                foreach (var element in document.QuerySelectorAll("article > div > p > strong > em"))
                {
                    i++;
                    if (from <= i && (i - from) <= amount)
                    {
                        var res = element.InnerHtml.Replace(element.QuerySelector("a").OuterHtml, "");
                        date.Add(res);
                        form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
                    }
                    else if ((i - from) > amount)
                        break;
                }

                //парсинг ссылки на изображение
                i = 0;
                foreach (var element in document.QuerySelectorAll("article > div > p > strong > em > a"))
                {
                    i++;
                    if (from <= i && (i - from) <= amount)
                    {
                        img_link.Add(element.GetAttribute("href"));
                        form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
                    }
                    else if ((i - from) > amount)
                        break;
                }

                //парсинг текста
                i = 0;
                foreach (var element in document.QuerySelectorAll("article > div"))
                {
                    i++;
                    if (from <= i && (i - from) <= amount)
                    {
                        var res = element.InnerHtml.Replace(element.QuerySelector("p").OuterHtml, "");
                        text.Add(res);
                        form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
                    }
                    else if ((i - from) > amount)
                        break;
                }

                //парсинг ссылки для кнопки
                i = 0;
                foreach (var element in document.QuerySelectorAll("article > div > p > a"))
                {
                    i++;
                    if (from <= i && (i - from) <= amount)
                    {
                        button_link.Add(element.GetAttribute("href"));
                        form.BeginInvoke(new Action(() => form.progressBar1.Value += procent));
                    }
                    else if ((i - from) > amount)
                        break;
                }
            }
            catch {
                MessageBox.Show("Неудалось сформировать страницу. Так как новости не в типовом формате!");
                return "";
            }

            #endregion

            var response = makeNews(header_text, header_link, date, img_link, text, button_link);

            return response;
        }

        private static string makeNews(List<string> header_text, List<string> header_link, List<string> date, List<string> img_link, List<string> text, List<string>  button_link)
        {
            StreamReader str = new StreamReader("news.txt");
            string news = str.ReadToEnd();

            string response = "";

            for(var i=0; i < header_text.Count; i++)
            {
                response += news.Replace("_news_header_text", header_text[i]).
                    Replace("_news_header_link", header_link[i]).
                    Replace("_news_date", date[i]).
                    Replace("_news_img_link", img_link[i]).
                    Replace("_news_button_link", button_link[i]).
                    Replace("_news_text", text[i]);
            }

            return response;
        }

        public static void completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var _params = (bg_params)e.Result;
            _params.form.BeginInvoke(new Action(() => _params.form.toolStripStatusLabel1.Text = "Готово"));
            MessageBox.Show("Готово!");
            _params.form.BeginInvoke(new Action(() => _params.form.progressBar1.Value = 0));
        }
    }
}
