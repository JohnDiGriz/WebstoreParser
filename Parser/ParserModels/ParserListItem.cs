using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.ParserModels
{
    public class ParserListItem
    {
        public ParserRules Parser { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Name { get; set; }
    }
}
