namespace WebCrawler.Lib
{
    public class StringPrinter : IMapPrinter
    {
        public string Print(Crawler crawler) {
            return crawler.GetMap();
        }
    }
}