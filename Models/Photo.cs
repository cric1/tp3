using JSON_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoManager.Models
{
    public class Photo : Record
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public bool Shared { get; set; }
    }
}