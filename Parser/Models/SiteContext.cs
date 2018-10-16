using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Parser.Models
{
    public class SiteContext : DbContext
    {
        public SiteContext() : base("DbConnection") { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Picture> Pictures { get; set; }
    }
}