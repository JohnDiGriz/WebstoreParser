using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class ParsingResults
    {
        public List<Models.Product> AddedProducts = new List<Models.Product>();
        public List<Models.Price> AddedPrices = new List<Models.Price>();
        public List<Models.Picture> AddedPictures = new List<Models.Picture>();
    }
}
