using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserRules parser = JsonConvert.DeserializeObject<ParserRules>(File.ReadAllText("parser.json"));
            ParsingData data = new ParsingData(parser.HomePage, parser);
            ParsingResults results;
            List<Models.Product> products;
            using (Models.SiteContext db=new Models.SiteContext())
            { 
                products = db.Products.ToList();
            }
            using (WebClient client = new WebClient())
            {
                results=data.Parse(client, products);
            }
            using (Models.SiteContext db=new Models.SiteContext())
            {
                db.Products.AddRange(results.AddedProducts);
                db.Prices.AddRange(results.AddedPrices);
                db.Pictures.AddRange(results.AddedPictures);
                db.SaveChanges();
            }
        }
    }
}
