using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public int ProductId { get; set; }
        //public Product Product { get; set; }
    }
}