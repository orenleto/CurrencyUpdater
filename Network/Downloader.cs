using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using W1.CurrencyUpdater.Configuration;

namespace W1.CurrencyUpdater
{
    class Downloader
    {
        private HttpWebRequest Request { get; }
        public HttpWebResponse Response {
            get
            {
                try
                {
                    return Request.GetResponse() as HttpWebResponse;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public string Answer {
            get
            {
                using (var stream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8))
                {
                    return stream.ReadToEnd();
                }
            }
        }

        public Downloader(Exchanger exchanger)
        {
            Request = GetRequest(exchanger);
        }

        private HttpWebRequest GetRequest(Exchanger exchanger)
        {
            HttpWebRequest request = HttpWebRequest.Create(TransformUrl(exchanger.Url)) as HttpWebRequest;
            request.Method = exchanger.Method;
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            if (request.Method == "POST")
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(exchanger.Body);
                request.ContentLength = byteArray.Length;
                request.ContentType = exchanger.ContentType;

                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            return request as HttpWebRequest;
        }

        private Uri TransformUrl(string url)
        {
            InsertDateInUrl(ref url);
            return new Uri(url);
        }
        private void InsertDateInUrl(ref string url)
        {
            if (Regex.IsMatch(url, @"[yMd]{2,4}[-\./][yMd]{2}[-\./][yMd]{2,4}"))
            {
                var match = Regex.Match(url, @"[yMd]{2,4}[-\./][yMd]{2}[-\./][yMd]{2,4}");
                url = url.Replace(match.Value, DateTime.Now.AddDays(0.0).ToString(match.Value));
            }
        }
    }
}
