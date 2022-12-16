using CommandLine;

namespace WebCrawler
{
    public class ArgumentOptions
    {
        [Option('u', "url", Required = true, HelpText = "Website to download - example: https://www.w3schools.com")]
        public string Url { get; set; }

        [Option('l', "location", Required = true, HelpText = "Location to save the website - example: c:\\mywebpage")]
        public string Location { get; set; }

        [Option('s', "statistic", Required = false, HelpText = "Print statistics after execution")]
        public bool Statistic { get; set; }
    }
}