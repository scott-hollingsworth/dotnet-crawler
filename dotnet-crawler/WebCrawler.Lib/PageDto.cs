using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace WebCrawler.Lib
{
    public class WebPageDto
    {
        public string Uri {get;}
        public List<string> InternalLinks {get;}
        public List<string> ExternalLinks {get;}
        public List<string> Images {get;}

        public WebPageDto(WebPage page) {
            Uri = page.Uri.AbsoluteUri;
            InternalLinks = page.GetInternalLinks()
                ?.Select(u => u.AbsoluteUri)
                ?.ToList() ?? new List<string>();
            ExternalLinks = page.GetExternalLinks()
                ?.Select(u => u.AbsoluteUri)
                ?.ToList() ?? new List<string>();
            Images = page.GetImages()
                ?.ToList() ?? new List<string>();
        }

        public string Print()
        {
            var options = new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
            };
            
            return JsonSerializer.Serialize(this, options);
        }
    }
}