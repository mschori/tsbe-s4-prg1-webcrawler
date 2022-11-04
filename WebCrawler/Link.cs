using System;

namespace WebCrawler
{
    public enum Type
    {
        External,
        Internal
    }

    public class Link
    {
        public string Path { get; set; }
        private Type Type { get; set; }

        public Link(string path)
        {
            Path = path;
            // TODO Check type of link
            Type = Type.External;
        }
    }
}