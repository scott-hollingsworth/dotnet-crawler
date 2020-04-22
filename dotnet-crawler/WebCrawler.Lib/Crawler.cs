using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Buildit.Webcrawler.Models;

namespace WebCrawler.Lib {
    public class Crawler {
        private IHttpClientFactory factory;
        private IMapPrinter printer;
        private ConcurrentDictionary<string, byte> visitedPages;
        private ConcurrentQueue<Uri> pagesToVisit;
        private StringBuilder map;
        private string uri;
        private Page page;
        public Uri targetSite;

        public Crawler(string uri, IHttpClientFactory factory, IMapPrinter printer)
        {
            visitedPages = new ConcurrentDictionary<string, byte>();
            pagesToVisit = new ConcurrentQueue<Uri>();
            map = new StringBuilder();

            this.uri = uri;
            this.targetSite = new Uri(uri);
            this.factory = factory;
            this.printer = printer;
            this.page = new Page(uri);
        }

        public async Task<ConcurrentDictionary<string, byte>> Crawl() {
            ValidateUri();
            SeedQueue();

            do {
                await ProcessQueue();
            } while (SitesInQueue());

            return visitedPages;
        }

        private bool NotAlreadyVisited(Uri target) {
            return !visitedPages.ContainsKey(target.AbsoluteUri);
        }

        private void SeedQueue() {
            pagesToVisit.Enqueue(new Uri(uri));
        }

        public void ValidateUri() {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                throw new Exception("Invalid URL, Jerk!");
        }

        public async Task ProcessQueue() {
            bool success = pagesToVisit.TryDequeue(out Uri target);
            if (NotAlreadyVisited(target))
                await ProcessUri(target);
        }

        private async Task ProcessUri(Uri target) {
            visitedPages.TryAdd(target.AbsoluteUri, new byte());

            WebPage webPage = await BuildPage(target);
            AddWebPageToMap(webPage);
            AddInternalLinksToQueue(webPage);
        }

        private async Task<WebPage> BuildPage(Uri target) {
            var client = factory.CreateClient();
            string content = "<html><body><p>Empty</body></html>";
            try {
                content = await client.GetStringAsync(target);
            } catch (Exception e) {
                e.ToString();
            }

            return new WebPage(target, content);
        }

        private void AddWebPageToMap(WebPage webPage) {
            lock(map) {
                map.AppendLine(webPage.Print() ?? "");
            }
        }

        private void AddInternalLinksToQueue(WebPage webPage) {
            webPage.GetInternalLinks()
                .ToList()
                .ForEach(l => pagesToVisit.Enqueue(l));
        }

        private bool SitesInQueue() {
            return pagesToVisit.Count > 0;
        }

        public string Print() {
            return printer.Print(this);
        }

        public string GetMap() {
            return map.ToString();
        }

        public Page getPage()
        {
            return page;
        }
    }
}