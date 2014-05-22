using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Margatsni.Instagram.Data
{
/*
    images: {
low_resolution: {
url: "http://photos-c.ak.instagram.com/hphotos-ak-frc/10261245_598573513571418_1756721484_a.jpg",
width: 306,
height: 306
},
thumbnail: {
url: "http://photos-c.ak.instagram.com/hphotos-ak-frc/10261245_598573513571418_1756721484_s.jpg",
width: 150,
height: 150
},
standard_resolution: {
url: "http://photos-c.ak.instagram.com/hphotos-ak-frc/10261245_598573513571418_1756721484_n.jpg",
width: 640,
height: 640
}
},
*/
    [DataContract]
    public class Images
    {
        [DataContract]
        public class Image
        {
            [DataMember]
            public string url { get; set; }
        }
        [DataMember]
        public Image low_resolution { get; set; }
        [DataMember]
        public Image thumbnail { get; set; }
        [DataMember]
        public Image standard_resolution { get; set; }
    }
}
