using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Margatsni.Instagram.Data.Templates;
namespace Margatsni.Instagram.Data.Response
{
    [DataContract]
    public class ImagesOnly
    {
        [DataMember]
        public Images images { get; set; }
        [DataContract]
        public class Likes
        {
            [DataMember]
            public int count { get; set; }
        }
        [DataMember]
        public Likes likes { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string id { get; set; }
    }

    [DataContract]
    public class ImagesOnlyCollectionResponse:CollectionData<ImagesOnly>
    {
    }
}
