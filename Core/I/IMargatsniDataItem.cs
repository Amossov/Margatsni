using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Core.I
{
    public interface IMargatsniDataItem
    {
        string Id { get; }
        string ImageUrl { get; }
        int Sort { get; }
    }
/*    public interface IMargatsniDataSource:IEnumerable<IMargatsniDataItem>
    {

    }*/
}
