using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace Parser
{
    class ParsingData
    {
        private List<string> PagesLinks_;
        private List<string> ProductsLinks_;
        private ParserRules Rules_;
        private string DomainName_;

        public ParsingData(string HomePage, ParserRules rules) 
        {
            PagesLinks_ = new List<string>();
            ProductsLinks_ = new List<string>();
            PagesLinks_.Add(HomePage);
            Rules_ = rules;
            DomainName_ = HomePage.Split('/')[0] + "//" + HomePage.Split('/')[2].Split('/')[0];
        }
        public ParsingResults Parse(System.Net.WebClient client, List<Models.Product> ProductList)
        {
            HtmlDocument doc = new HtmlDocument();
            for (int i=0; i<PagesLinks_.Count;i++)
            {
                try
                {
                    string source = client.DownloadString(PagesLinks_[i]);

                    Console.Write(PagesLinks_[i] + " " + i.ToString() + "\n");
                    doc.LoadHtml(source);
                    HtmlNodeNavigator navigator = (HtmlNodeNavigator)doc.CreateNavigator();
                    var productNodes = navigator.Select(Rules_.DetailsXpath);
                    AddProducts(productNodes);
                    var pagesNodes = navigator.Select(Rules_.PaginationXpath);
                    AddPages(pagesNodes);
                }
                catch (System.Net.WebException) { }
            }
            ParsingResults res = new ParsingResults();
            for(int i=0; i<ProductsLinks_.Count;i++)
            {
                string source = client.DownloadString(ProductsLinks_[i]);
                Console.Write(ProductsLinks_[i] + " " + i.ToString() + "\n");
                doc.LoadHtml(source);
                HtmlNodeNavigator navigator = (HtmlNodeNavigator)doc.CreateNavigator();
                var product = IsAlreadyParsed(source, ProductList);
                if(product!=null)
                {
                    string priceValue = ConnectStrings(navigator.Select(Rules_.PriceXpath));
                    Models.Price price = new Models.Price() { Product = product, Date = DateTime.Now, PriceValue = priceValue };
                    product.Price = priceValue;
                    res.AddedPrices.Add(price);
                }
                else
                {
                    ParseNewProduct(navigator, product, res, ProductsLinks_[i]);
                }
            }
            return res;
        }
        private void ParseNewProduct(HtmlNodeNavigator navigator, Models.Product product, ParsingResults res, string link)
        {
            string header = ConnectStrings(navigator.Select(Rules_.HeaderXpath));
            string desctiption = ConnectStrings(navigator.Select(Rules_.DescriptionXpath));
            string priceValue = ConnectStrings(navigator.Select(Rules_.PriceXpath));
            List<string> pictures = ListValue(navigator.Select(Rules_.PicturesXpath));
            product = new Models.Product()
            {
                Description = desctiption,
                Name = header,
                Link = link,
                ImageUrl = (pictures.Count > 0 ? pictures[0] : ""),
                Price = priceValue
            };
            Models.Price price = new Models.Price() { Product = product, Date = DateTime.Now, PriceValue = priceValue };
            List<Models.Picture> thumbs = new List<Models.Picture>();
            foreach (string url in pictures)
            {
                thumbs.Add(new Models.Picture() { Product = product, PictureUrl = url });
            }
            res.AddedPictures.AddRange(thumbs);
            res.AddedPrices.Add(price);
            res.AddedProducts.Add(product);

        }
        private Models.Product IsAlreadyParsed(string link, List<Models.Product> products)
        {
            bool isParsed = false;
            int i;
            for(i=0;i<products.Count && !isParsed;i++) {
                isParsed = products[i].Link == link;
            }
            if (isParsed)
                return products[i - 1];
            else
                return null;
        }
        public bool AddPages(System.Xml.XPath.XPathNodeIterator newPages)
        {
            bool isAdded = false;
            foreach (System.Xml.XPath.XPathNavigator pageLink in newPages)
            {
                string link = (pageLink.Value.StartsWith("http://") || pageLink.Value.StartsWith("https://")
                    ? pageLink.Value : DomainName_ + pageLink.Value);
                System.Text.RegularExpressions.Regex rx =
                    new System.Text.RegularExpressions.Regex(Rules_.PaginationPattern + "\\d+");
                var matches = rx.Matches(link);
                bool isGood = true;
                for (int i = 0; isGood && i < matches.Count; i++)
                {
                    isGood=Int32.TryParse(matches[i].Groups[0].Value.Replace(Rules_.PaginationPattern, ""), out int j);
                    if (isGood)
                        isGood = j <= Rules_.MaxPageValue;
                }
                if (isGood && PagesLinks_.IndexOf(link)==-1)
                {
                    PagesLinks_.Add(link);
                    isAdded = true;
                }
            }
            return isAdded;
        }
        public bool AddProducts(System.Xml.XPath.XPathNodeIterator newProducts)
        {
            bool isAdded = false;
            foreach (System.Xml.XPath.XPathNavigator pageLink in newProducts)
            {
                string link = (pageLink.Value.StartsWith("http://") || pageLink.Value.StartsWith("https://")
                    ? pageLink.Value : DomainName_ + pageLink.Value);
                if (ProductsLinks_.IndexOf(link) == -1)
                {
                    ProductsLinks_.Add(link);
                    isAdded = true;
                }
            }
            return isAdded;
        }
        public string ConnectStrings(System.Xml.XPath.XPathNodeIterator values)
        {
            string res = "";
            foreach(System.Xml.XPath.XPathNavigator value in values)
            {
                res += value.Value.Replace("&nbsp;", " ");
            }
            return res;
        }
        public List<string> ListValue(System.Xml.XPath.XPathNodeIterator values)
        {
            List<string> res = new List<string>();
            foreach (System.Xml.XPath.XPathNavigator value in values)
            {
                res.Add((value.Value.StartsWith("http://") || value.Value.StartsWith("https://")
                    ? value.Value : DomainName_ + value.Value));
            }
            return res;

        }
    }
}
