using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class ParserRules
    {
        public string HomePage { get; set; }
        public string DetailsXpath { get; set; }
        public string PaginationXpath { get; set; }
        public string PaginationPattern { get; set; }
        public int MaxPageValue { get; set; }

        public string HeaderXpath { get; set; }
        public string PriceXpath { get; set; }
        public string DescriptionXpath { get; set; }
        public string PicturesXpath { get; set; }
    }
}
