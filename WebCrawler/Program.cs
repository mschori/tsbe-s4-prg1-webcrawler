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
            // Console.ReadKey();
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
            p.CreateFolderStructure(opts.Location);
            String htmlContent = client.DownloadString(opts.Url);
            p._newSites.Add(new Site("/", htmlContent));

            while (true)
            {
                if (p._newSites.Count == 0)
                {
                    break;
                }

                var site = p._newSites.First();

                String[] s = site.Path.Split('/');
                String last = site.Name;
                String newPath = String.Join("/", s.Take(s.Length - 1).ToArray());
                Console.WriteLine(opts.Location + newPath);
                p.CreateFolderStructure(opts.Location + newPath);

                Console.WriteLine(opts.Location + newPath);

                var loc = String.Format("C:/{0}/{1}.html", opts.Location + newPath, site.Name);
                Console.WriteLine(loc);
                Console.WriteLine(opts.Url + site.Path);
                client.DownloadFile(opts.Url + site.Path, Path.GetFullPath(loc));
                site.Analyze();
                List<String> processedLinks = new List<string>();
                site.InternalLinks.ForEach(link =>
                {
                    // TODO check if link is already processed
                    bool found = p._processedSites.Any(x => x.Path == link.Path);
                    bool found2 = processedLinks.Any(x => x == link.Path);
                    if (!found && !found2)
                    {
                        Console.WriteLine("Download File: {0}", opts.Url + link.Path);
                        try
                        {
                            htmlContent = client.DownloadString(opts.Url + link.Path);
                            // p._newSites.Add(new Site(link.Path, htmlContent));
                        }
                        catch (Exception e)
                        {
                            // TODO do nothing
                        }
                    }

                    processedLinks.Add(link.Path);
                });
                p._newSites.Remove(site);
                p._processedSites.Add(site);
            }

            // Save images
            foreach (Site site in p._processedSites)
            {
                foreach (Image image in site.Images)
                {
                    String[] s = image.Path.Split('/');
                    String last = s.Last();
                    String newPath = String.Join("/", s.Take(s.Length - 1).ToArray());
                    Console.WriteLine("Last Part: {0}", last);
                    Console.WriteLine("NewPath: {0}", newPath);
                    // TODO save image
                    try
                    {
                        p.CreateFolderStructure(opts.Location + "/" + newPath);
                        Console.WriteLine("Created FolderStrucute: {0}", opts.Location + "/" + newPath);
                        var loc = String.Format("C:\\{0}/{1}", opts.Location, image.Path);
                        client.DownloadFile(opts.Url + "/" + image.Path, Path.GetFullPath(loc));
                    }
                    catch (Exception e)
                    {
                        // TODO do nothing...
                        Console.WriteLine(e);
                    }

                }
            }

            // TODO create statistic
            // TODO terminate program
        }
    }
}