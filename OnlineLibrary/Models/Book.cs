using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public List<Author> Authors { get; set; }
    }
}