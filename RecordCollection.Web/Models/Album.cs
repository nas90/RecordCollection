using IF.Lastfm.Core.Objects;
using RecordCollection.Web.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        private LastAlbum _lastAlbum;

        [NotMapped]
        public LastAlbum LastAlbum {
            get
            {
                return _lastAlbum;
            }
            set
            {
                _lastAlbum = value;
            }
         }
    }
}
