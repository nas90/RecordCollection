using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Models
{
    public class HomeViewModel
    {
        public List<Album> Albums { get; set; }

        public HomeViewModel()
        {
            Albums = new List<Album>();
        }
    }
}
