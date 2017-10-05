using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Models
{
    public class Album
    {
        [Key]
        public int ID { get; set; }
        public string LastFM_ID { get; set; }
        public int CollectionID { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string ArtistName { get; set; }
    }
}
