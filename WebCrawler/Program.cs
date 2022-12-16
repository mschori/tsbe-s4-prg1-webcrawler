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
        private readonly WebClient _client = new WebClient();
        private string _saveLocation;

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ArgumentOptions>(args)
                .WithParsed(Execute);
        }

        private void CreateFolderStructure(string location)
        {
            location = Path.GetFullPath(location);

            if (Directory.Exists(location))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(location);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SaveFile(string baseUrl, string fileWebLink, string saveLocation)
        {
            var split = fileWebLink.Split('/');
            var newPath = string.Join("/", split.Take(split.Length - 1).ToArray());
            try
            {
                CreateFolderStructure(saveLocation + "/" + newPath);
                // Check if fileWebLink has suspect characters after file-extension
                var idx = fileWebLink.IndexOf("?", StringComparison.Ordinal);
                if (idx > 0)
                {
                    fileWebLink = fileWebLink.Substring(0, idx);
                }

                var loc = $"{saveLocation}/{fileWebLink}";
                Console.WriteLine("Downloading file: {0}", fileWebLink);
                _client.DownloadFile(baseUrl + "/" + fileWebLink, Path.GetFullPath(loc));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Execute(ArgumentOptions opts)
        {
            var p = new Program();
            p._saveLocation = opts.Location;
            p.CreateFolderStructure(p._saveLocation);
            var htmlContent = p._client.DownloadString(opts.Url);
            p._newSites.Add(new Site("/", htmlContent));
            var processedLinks = new List<string>();

            while (true)
            {
                if (p._newSites.Count == 0)
                {
                    break;
                }

                var site = p._newSites.First();
                var siteSplit = site.Path.Split('/');
                var newPath = string.Join("/", siteSplit.Take(siteSplit.Length - 1).ToArray());
                p.CreateFolderStructure(opts.Location + newPath);
                var loc = $"{Path.Combine(p._saveLocation, newPath)}/{site.Name}.html";
                Console.WriteLine("Downloading page: {0}", opts.Url + site.Path);
                p._client.DownloadFile(opts.Url + site.Path, Path.GetFullPath(loc));
                site.Analyze();

                site.InternalLinks.ForEach(link =>
                {
                    var found = p._processedSites.Any(x => x.Path == link.Path);
                    var found2 = processedLinks.Any(x => x == link.Path);
                    if (!found && !found2)
                    {
                        Console.WriteLine("Downloading page: {0}", opts.Url + link.Path);
                        try
                        {
                            htmlContent = p._client.DownloadString(opts.Url + link.Path);
                            p._newSites.Add(new Site(link.Path, htmlContent));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        processedLinks.Add(link.Path);
                    }
                });
                p._newSites.Remove(site);
                p._processedSites.Add(site);
            }

            // Save images and stylesheets
            foreach (var site in p._processedSites)
            {
                foreach (var image in site.Images)
                {
                    p.SaveFile(opts.Url, image.Path, opts.Location);
                }

                foreach (var stylesheet in site.Stylesheets)
                {
                    p.SaveFile(opts.Url, stylesheet.Path, opts.Location);
                }
            }

            if (opts.Statistic)
            {
                var amountInternalLinks = 0;
                var amountExternalLinks = 0;
                var amountImages = 0;

                foreach (var site in p._processedSites)
                {
                    amountInternalLinks += site.GetAmountLinks(true);
                    amountExternalLinks += site.GetAmountLinks(false);
                    amountImages += site.GetAmountImages();
                }

                Console.WriteLine("\n----------- STATISTIC ----------");
                Console.WriteLine("Amount of internal links: {0}", amountInternalLinks);
                Console.WriteLine("Amount of external links: {0}", amountExternalLinks);
                Console.WriteLine("Amount of images: {0}", amountImages);
            }

            Console.WriteLine("\n-----------> Finished program. Please give a good note  :)");
        }
    }
}