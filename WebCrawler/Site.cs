using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebCrawler
{
    public class Site
    {
        public string Name { get; }
        public string Path { get; }
        private string Content { get; }
        public readonly List<Stylesheet> Stylesheets = new List<Stylesheet>();
        private readonly List<Link> _externalLinks = new List<Link>();
        public readonly List<Link> InternalLinks = new List<Link>();
        public readonly List<Image> Images = new List<Image>();

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
            return isInternal ? InternalLinks.Count : _externalLinks.Count;
        }

        public int GetAmountImages()
        {
            return Images.Count;
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
                    _externalLinks.Add(new Link(url));
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
                Console.WriteLine(e);
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
                Console.WriteLine(e);
            }
        }
    }
}