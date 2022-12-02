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

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ArgumentOptions>(args)
                .WithParsed(Execute);
        }

        private void CreateFolderStructure(string location)
        {
            location = Path.GetFullPath("C:\\" + location);

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
            var s = fileWebLink.Split('/');
            var last = s.Last();
            var newPath = string.Join("/", s.Take(s.Length - 1).ToArray());
            Console.WriteLine("Last Part: {0}", last);
            Console.WriteLine("NewPath: {0}", newPath);
            try
            {
                CreateFolderStructure(saveLocation + "/" + newPath);
                var idx = fileWebLink.IndexOf("?");
                if (idx > 0)
                {
                    fileWebLink = fileWebLink.Substring(0, idx);
                }

                var loc = string.Format("C:\\{0}/{1}", saveLocation, fileWebLink);
                _client.DownloadFile(baseUrl + "/" + fileWebLink, Path.GetFullPath(loc));
            }
            catch (Exception e)
            {
                // TODO do nothing...
                Console.WriteLine(e);
            }
        }

        private static void Execute(ArgumentOptions opts)
        {
            var p = new Program();
            p.CreateFolderStructure(opts.Location);
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
                var s = site.Path.Split('/');
                var newPath = string.Join("/", s.Take(s.Length - 1).ToArray());
                p.CreateFolderStructure(opts.Location + newPath);
                var loc = string.Format("C:/{0}/{1}.html", opts.Location + newPath, site.Name);
                p._client.DownloadFile(opts.Url + site.Path, Path.GetFullPath(loc));

                site.Analyze();

                site.InternalLinks.ForEach(link =>
                {
                    var found = p._processedSites.Any(x => x.Path == link.Path);
                    var found2 = processedLinks.Any(x => x == link.Path);
                    processedLinks.ForEach(Console.WriteLine);
                    if (!found && !found2)
                    {
                        Console.WriteLine("Download File: {0}", opts.Url + link.Path);
                        try
                        {
                            htmlContent = p._client.DownloadString(opts.Url + link.Path);
                            p._newSites.Add(new Site(link.Path, htmlContent));
                        }
                        catch (Exception e)
                        {
                            // TODO do nothing
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

            // TODO create statistic
            // TODO terminate program
        }
    }
}