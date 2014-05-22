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
    public class User
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string id { get; set; }
    }
    [DataContract]
    public class UserSearchCollectionResponse : CollectionData<User>
    {
    }
}
