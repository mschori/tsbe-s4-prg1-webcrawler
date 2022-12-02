using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            Name = path.Split('/').Last();
            if (Name.Length < 1)
            {
                Name = "index";
            }
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
                var url = link.Attributes["href"].Value;
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    ExternalLinks.Add(new Link(url));
                }
                else
                {
                    var re = new Regex(@"^/[a-zA-Z0-9\-\/]+$");
                    if (re.IsMatch(url))
                    {
                        InternalLinks.Add(new Link(url));
                    }
                }
            }
        }

        private void ExtractImages(HtmlDocument doc)
        {
            try
            {
                foreach (var image in doc.DocumentNode.SelectNodes("//img[@src]"))
                {
                    Images.Add(new Image(image.Attributes["src"].Value));
                }
            }
            catch (Exception e)
            {
                // TODO nothing
            }
        }

        private void ExtractStylesheets(HtmlDocument doc)
        {
            try
            {
                foreach (var cssFile in doc.DocumentNode.SelectNodes("//link[@href]"))
                {
                    Stylesheets.Add(new Stylesheet(cssFile.Attributes["href"].Value));
                }
            }
            catch (Exception e)
            {
                // TODO nothing
            }
        }
    }
}