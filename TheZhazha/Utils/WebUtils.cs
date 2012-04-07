using System;
using System.IO;
using System.Net;
using System.Text;
using Shock.Logger;

namespace TheZhazha.Utils
{
    public static class WebUtils
    {
        public static HttpWebRequest PrepareRequest(string uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            return req;
        }

        public static string GetPageHtml(HttpWebRequest req)
        {
            string html = null;
            try
            {
                var stream = GetResponseStream(req);
                var reader = new StreamReader(stream, Encoding.UTF8);
                html = reader.ReadToEnd();
                stream.Close();
            }
            catch (Exception ex)
            {
                LoggerFacade.Log(ex.Message, Importance.Warning);
            }

            return html;
        }

        public static string GetPageHtml(string uri)
        {
            var req = PrepareRequest(uri);
            return GetPageHtml(req);
        }

        public static Stream GetResponseStream(HttpWebRequest req)
        {
            Stream stream = null;
            try
            {
                var resp = (HttpWebResponse)req.GetResponse();
                stream = resp.GetResponseStream();
            }
            catch (Exception ex)
            {
                LoggerFacade.Log(ex.Message, Importance.Error);
            }

            return stream;
        }
    }
}