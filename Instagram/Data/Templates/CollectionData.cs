using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace Margatsni.Instagram.Data.Templates
{
    [DataContract]
    public class CollectionData<T>:Envelope
    {
        [DataMember]
        public List<T> data { get; set; }
    }
}
