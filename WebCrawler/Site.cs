using System;
using System.Collections.Generic;

namespace WebCrawler
{
    public class Site
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Stylesheet> Stylesheets = new List<Stylesheet>();
        public List<Link> ExternalLinks = new List<Link>();
        public List<Link> InternalLinks = new List<Link>();
        public List<Image> Images = new List<Image>();

        public Site(string path)
        {
            Path = path;
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
            Console.WriteLine("analyze...");

            // TODO analyze page

            // TODO extract links

            // TODO extract images
        }
    }
}