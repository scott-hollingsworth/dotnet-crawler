using System;
using HtmlAgilityPack;

namespace Buildit.Webcrawler.Models
{
    public interface WebDAO
    {
        HtmlDocument get(string url);
    }
}
