using CommandLine;
using System;
using System.IO;

namespace WebCrawler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ArgumentOptions>(args)
                .WithParsed<ArgumentOptions>(Execute);

            // Block automatic close of window
            Console.ReadKey();

            Program p = new Program();
        }

        static void Execute(ArgumentOptions opts)
        {
            Console.WriteLine(opts.Url);
            Console.WriteLine(opts.Location);
            Boolean directoryExists = Directory.Exists(opts.Location);
            Console.WriteLine(directoryExists);
            Console.WriteLine(opts.Statistic);
        }
    }
}