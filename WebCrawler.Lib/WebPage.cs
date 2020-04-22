using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler.Lib
{
    public class WebPage
    {
        private HtmlDocument Document;
        public Uri Uri;
        private Uri BaseUri;

        public WebPage(Uri uri, string html) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            Uri = uri;
            BaseUri = new Uri(uri.GetLeftPart(System.UriPartial.Authority));
        }

        public IEnumerable<Uri> GetLinks() {
            var links = Document.DocumentNode.SelectNodes("//a[@href]")
                    ?.Select(n => n.Attributes["href"].Value)
                    ?.Select(h => StripQueryString(h))
                    ?.Select(h => GetAbsoluteUriFromHref(h))
                    ?.Distinct()
                    ?.ToList();
            return links ?? new List<Uri>();
        }

        public IEnumerable<Uri> GetInternalLinks()
        {
            return GetLinks()?.Where(u => BaseUri.IsBaseOf(u)) ?? new List<Uri>();
        }

        public IEnumerable<Uri> GetExternalLinks()
        {
            return GetLinks()?.Where(u => !BaseUri.IsBaseOf(u)) ?? new List<Uri>();
        }

        public IEnumerable<string> GetImages()
        {
            var links = Document.DocumentNode.SelectNodes("//img[@src]")
                    ?.Select(n => n.Attributes["src"].Value)
                    ?.Distinct()
                    ?.ToList();
            
            return links ?? new List<string>();
        }

        private Uri GetAbsoluteUriFromHref(string href) {
            Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
            if (!uri.IsWellFormedOriginalString())
                return Uri;
            uri = uri.IsAbsoluteUri ? uri : new Uri(BaseUri, uri);
            
            return uri;
        }

        private static string StripQueryString(string href) {
            if (href.Contains('?'))
                return href.Split('?')[0];
            return href;
        }

        public string Print()
        {
            WebPageDto pageDto = new WebPageDto(this);
            return pageDto.Print();
        }
    }
}
