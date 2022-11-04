using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WebCrawler
{
    internal class Program
    {
        private readonly List<Site> _newSites = new List<Site>();
        private readonly List<Site> _processedSites = new List<Site>();

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ArgumentOptions>(args)
                .WithParsed<ArgumentOptions>(Execute);

            // Block automatic close of window
            Console.ReadKey();
        }

        private bool CreateFolderStructure(string location)
        {
            location = Path.GetFullPath("C:\\" + location);
            if (!Directory.Exists(location))
            {
                try
                {
                    Directory.CreateDirectory(location);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        private static void Execute(ArgumentOptions opts)
        {
            var p = new Program();
            WebClient client = new WebClient();
            Console.WriteLine(p.CreateFolderStructure(opts.Location));
            p._newSites.Add(new Site(opts.Url));

            while (true)
            {
                if (p._newSites.Count == 0)
                {
                    break;
                }

                var site = p._newSites.First();

                var loc = String.Format("C:\\{0}/{1}.html", opts.Location, site.Name);
                client.DownloadFile(site.Path, Path.GetFullPath(loc));
                site.Analyze();
                site.InternalLinks.ForEach(link =>
                {
                    // TODO check if link is already processed
                    p._newSites.Add(new Site(link.Path));
                });
                p._newSites.Remove(site);
                p._processedSites.Add(site);
            }

            // TODO create statistic
            // TODO terminate program
        }
    }
}