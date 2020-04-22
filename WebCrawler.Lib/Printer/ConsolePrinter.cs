using System;

namespace WebCrawler.Lib
{
    public class ConsolePrinter : IMapPrinter
    {
        public string Print(Crawler crawler) {
            string mapOutput = crawler.GetMap();
            Console.WriteLine(mapOutput);
            return crawler.targetSite.AbsoluteUri;
        }
    }
}