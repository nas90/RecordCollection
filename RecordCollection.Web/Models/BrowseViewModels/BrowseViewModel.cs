using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Models
{
    public class BrowseViewModel
    {
        public List<Album> Albums { get; set; }
        public Pager Pager { get; set; }

        public BrowseViewModel()
        {
            Albums = new List<Album>();
        }
    }
}
