using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
namespace Buildit.Webcrawler.Models
{
    public class Page
    {
        public string uri { get; set; }
        public ArrayList internalLinks { get; set; }
        public ArrayList externalLinks { get; set; }
        public ArrayList images { get; set; }

        public Page(string url) {
            uri = url;
            internalLinks = new ArrayList();
            externalLinks = new ArrayList();
            images = new ArrayList();
        }

        public String getUri()
        {
            return uri;
        }

        public ArrayList getInternalLinks()
        {
            return new ArrayList(internalLinks);
        }

        public ArrayList getExternalLinks()
        {
            return new ArrayList(externalLinks);
        }

        public ArrayList getImages()
        {
            return new ArrayList(images);
        }

        public void addInternalLink(String link)
        {
            internalLinks.Add(link);
        }

        public void addExternalLink(String link)
        {
            externalLinks.Add(link);
        }

        public void addImage(String link)
        {
            images.Add(link);
        }
    }
}
