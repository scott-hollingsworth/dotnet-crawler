using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Buildit.Webcrawler.Models;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.Lib;
using HtmlAgilityPack;
using System.Collections;


namespace Buildit.Webcrawler.Controllers
{
    [Route("crawl")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IHttpClientFactory factory;

        [HttpGet]
        public async Task<Page> GetLinksAsync(string url)
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<IMapPrinter, ConsolePrinter>()
                .BuildServiceProvider();
            factory = serviceProvider.GetService<IHttpClientFactory>();
            
            return await crawlAsync(url);
        }

        private async Task<Page> crawlAsync(string url)
        {
            Uri uri = new Uri(url);
            String host = uri.Host;
            HashSet<string> visited = new HashSet<string>();
            Queue<String> toVisit = new Queue<string>();
            Page page = new Page(url);
            WebDAO webDao = new WebDAOImpl();

            toVisit.Enqueue(url);
            while (toVisit.Count != 0)
            {

                string currentLink = formatLink(toVisit.Dequeue());
                visited.Add(currentLink);
                var client = factory.CreateClient();
                string content = "<html><body><p>Empty</body></html>";
                try
                {
                    content = await client.GetStringAsync(currentLink);
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                HtmlDocument document = webDao.get(content);
                string href = "";
                if (document != null)
                {
                    foreach (Uri link in GetLinks(document, new Uri(new Uri(currentLink).GetLeftPart(System.UriPartial.Authority))))
                    {
                        if (link != null)
                            href = link.AbsoluteUri ?? "";
                        else href = "";

                        if (!visited.Contains(href) && !href.Equals(""))
                        {
                            if (host.Equals(new Uri(href).Host))
                            {
                                page.addInternalLink(href);
                                toVisit.Enqueue(href);
                            }
                            else
                            {
                                page.addExternalLink(href);
                                visited.Add(href);
                            }
                        }

                        foreach (string image in GetImages(document))
                        {
                            ArrayList pageImages = page.getImages();

                            if (!pageImages.Contains(image))
                            {
                                page.addImage(image);

                            }
                        }
                    }
                }
            }

            foreach (string visit in visited)
                Console.WriteLine("Visited: {0}", visit.ToString());

            return page;
        }


        private String formatLink(string linkToFormat)
        {
            Uri link = new Uri(linkToFormat.Replace(" ", "%20"));

            return link.Scheme + "://" + link.Host + link.AbsolutePath;
        }

        private IEnumerable<Uri> GetLinks(HtmlDocument document, Uri baseUri)
        {
            var links = document.DocumentNode.SelectNodes("//a[@href]")
                    ?.Select(n => n.Attributes["href"].Value)
                    ?.Select(h => StripQueryString(h))
                    ?.Select(h => GetAbsoluteUriFromHref(h, baseUri))
                    ?.Distinct()
                    ?.ToList();
            return links ?? new List<Uri>();
        }

        public IEnumerable<string> GetImages(HtmlDocument document)
        {
            var links = document.DocumentNode.SelectNodes("//img[@src]")
                    ?.Select(n => n.Attributes["src"].Value)
                    ?.Distinct()
                    ?.ToList();

            return links ?? new List<string>();
        }

        private static string StripQueryString(string href)
        {
            if (href.Contains('?'))
                return href.Split('?')[0];
            return href;
        }

        private Uri GetAbsoluteUriFromHref(string href, Uri baseUri)
        {
            Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
            if (!uri.IsWellFormedOriginalString())
                return null;
            uri = uri.IsAbsoluteUri ? uri : new Uri(new Uri(baseUri.GetLeftPart(System.UriPartial.Authority)), uri);

            return uri;
        }
    }
}