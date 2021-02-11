using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using Parser.ParserModels;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = null;
            TimeSpan? delay = null;
            List<ParserListItem> parsers = new List<ParserListItem>();
            try
            {
                ParseArgs(args, ref directory, ref delay);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR PARSING ARGS\n{ex.Message}");
            }
            while (true)
            {
                var directoryInfo = new DirectoryInfo(directory);
                ParsingResults results = new ParsingResults();
                List<Models.Product> products;
                foreach (var parserFile in directoryInfo.GetFiles("*.json"))
                {
                    var parser = parsers.FirstOrDefault(x => x.Name == parserFile.Name);

                    if (parser != null && parser.LastUpdated < parserFile.LastWriteTime)
                        using (var sr = parserFile.OpenText())
                            parser.Parser = JsonConvert.DeserializeObject<ParserRules>(sr.ReadToEnd());
                    else if (parser == null)
                    {
                        using (var sr = parserFile.OpenText())
                            parsers.Add(new ParserListItem()
                            {
                                Name = parserFile.Name,
                                Parser = JsonConvert.DeserializeObject<ParserRules>(sr.ReadToEnd()),
                                LastUpdated = parserFile.LastWriteTime
                            });
                        parser = parsers.Last();
                    }

                    if (parser.LastRun + delay < DateTime.Now)
                    {
                        ParsingData data = new ParsingData(parser.Parser.HomePage, parser.Parser);
                        using (Models.SiteContext db = new Models.SiteContext())
                        {
                            products = db.Products.ToList();
                        }
                        using (WebClient client = new WebClient())
                        {
                            results.Add(data.Parse(client, products));
                            parser.LastRun = DateTime.Now;
                        }
                    }
                }
                using (Models.SiteContext db = new Models.SiteContext())
                {
                    db.Products.AddRange(results.AddedProducts);
                    db.Prices.AddRange(results.AddedPrices);
                    db.Pictures.AddRange(results.AddedPictures);
                    db.SaveChanges();
                }
            }
        }

        static void ParseArgs(string[] args, ref string directory, ref TimeSpan? delay)
        {
            int dI = Array.IndexOf(args, "-d");
            int tI = Array.IndexOf(args, "-t");
            directory = args[dI + 1];
            delay = TimeSpan.Parse(args[tI + 1]);
        }
    }
}
