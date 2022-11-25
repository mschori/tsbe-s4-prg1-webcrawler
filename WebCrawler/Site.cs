using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Analyze content of site-object.
        /// Extract all link and add them to external- or internal-links list.
        /// Extract all image-urls and add them to images-list.
        /// </summary>
        public void Analyze()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(Content);
            ExtractLinks(doc);
            ExtractImages(doc);
            ExtractStylesheets(doc);
        }

        /// <summary>
        /// Extract all links from given document.
        /// Links are identified with the HtmlAgilityPack
        /// </summary>
        /// <param name="doc">document to check for links</param>
        private void ExtractLinks(HtmlDocument doc)
        {
            foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                Console.WriteLine(link.Attributes["href"].Value);
                var url = link.Attributes["href"].Value;
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    ExternalLinks.Add(new Link(url));
                }
                else
                {
                    InternalLinks.Add(new Link(url));
                }
            }
        }

        private void ExtractImages(HtmlDocument doc)
        {
            foreach (var image in doc.DocumentNode.SelectNodes("//img[@src]"))
            {
                Console.WriteLine(image.Attributes["src"].Value);
                Images.Add(new Image(image.Attributes["src"].Value));
            }
        }

        private void ExtractStylesheets(HtmlDocument doc)
        {
            // TODO Extract Stylesheets
        }
    }
}