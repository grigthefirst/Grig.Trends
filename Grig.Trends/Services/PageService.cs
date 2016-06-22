using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Grig.Trends.Services
{
    public class PageService
    {
        public HtmlDocument Download(string uriStr)
        {
            Uri uri = new Uri(uriStr.IndexOf("http") == 0 ? uriStr : ("http://" + uriStr));
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(DownloadString(uri));
            return document;
        }

        private string DownloadString(Uri uri)
        {
            WebClient client = new WebClient();

            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return s;
        }
    }
}
