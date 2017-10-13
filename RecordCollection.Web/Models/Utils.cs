using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Models
{
    public class ActionResponse
    {
        public string data { get; set; }
        public bool success { get; set; }
    }

    public class LastFM_Credentials
    {
        public string LastFM_ApiKey { get; set; }

        public string LastFM_SecretKey { get; set; }
    }
}
