using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using AngleSharp.Parser.Html;
using System.Net;

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
            //узлы, которык необходимо извлечь с сайта
            List<string> header_text = new List<string>();
            List<string> header_link = new List<string>();
            List<string> date = new List<string>();
            List<string> img_link = new List<string>();
            List<string> text = new List<string>();
            List<string> button_link = new List<string>();

            var document = new HtmlParser().Parse(WebRequest.Create("http://da-strateg.ru/io/novosti-io/").GetResponse().GetResponseStream());

            #region парсинг DOM'а (Оптимзировать!!!)
            //парсинг заголовка и ссылки на него
            var i = 0;
            foreach (var element in document.QuerySelectorAll("article > header > h2 > a"))
            {
                i++;
                if (from <= i && (i - from) <= amount)
                {
                    header_link.Add(element.GetAttribute("href"));
                    header_text.Add(element.InnerHtml);
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
                    img_link.Add(element.GetAttribute("href"));
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
                    button_link.Add(element.GetAttribute("href"));
                else if ((i - from) > amount)
                    break;
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
    }
}
