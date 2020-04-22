using System;
using HtmlAgilityPack;

namespace Buildit.Webcrawler.Models
{
    public class WebDAOImpl : WebDAO
    {
        public WebDAOImpl()
        {
                
        }

        public HtmlDocument get(string url)
        {
            HtmlDocument response = new HtmlDocument();

                try
                {
                    response.LoadHtml(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception information: {0}", e);
                    response.LoadHtml(url);
                }

                return response;
        }
    }
}
