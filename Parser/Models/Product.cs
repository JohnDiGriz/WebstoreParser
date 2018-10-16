using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parser.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<Picture> Pictures { get; set; }
    }
}