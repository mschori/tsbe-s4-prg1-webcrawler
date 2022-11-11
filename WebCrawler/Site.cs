using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class Site
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public List<Stylesheet> Stylesheets = new List<Stylesheet>();
        public List<Link> ExternalLinks = new List<Link>();
        public List<Link> InternalLinks = new List<Link>();
        public List<Image> Images = new List<Image>();

        public Site(string path, string content)
        {
            Path = path;
            Content = content;
            // TODO generate name
            Name = "index";
        }

        public int GetAmountLinks(bool isInternal)
        {
            return 2;
        }

        public int GetAmountImages()
        {
            return 2;
        }

        public void Analyze()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(Content);

            // Extract links
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                Console.WriteLine(link.Attributes["href"].Value);
                String url = link.Attributes["href"].Value;
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    ExternalLinks.Add(new Link(url));
                }
                else
                {
                    InternalLinks.Add(new Link(url));
                }
            }

            // Extract Images
            foreach (HtmlNode image in doc.DocumentNode.SelectNodes("//img[@src]"))
            {
                Console.WriteLine(image.Attributes["src"].Value);
                Images.Add(new Image(image.Attributes["src"].Value));
            }
            
            // TODO Extract Stylesheets
        }
    }
}