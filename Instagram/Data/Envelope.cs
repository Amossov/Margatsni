using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Margatsni.Instagram.Data
{
    /*
       "meta": {
        "code": 200
    },
    "data": {
        ...
    },
    "pagination": {
        "next_url": "...",
        "next_max_id": "13872296"
    }
      */
    [DataContract]
    public class Envelope
    {
        [DataContract]
        public class Meta
        {
            [DataMember]
            public string code { get; set; }
        }
        [DataMember]
        public Meta meta { get; set; }
    }
}
