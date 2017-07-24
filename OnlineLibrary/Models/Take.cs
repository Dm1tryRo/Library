using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Models
{
    public class Take
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
        public bool isReturned { get; set; }
    }
}