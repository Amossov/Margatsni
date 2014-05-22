using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Core.I
{
    public class ItemUpdateEventArgs : EventArgs
    {
        public ItemUpdateEventArgs(IMargatsniDataItem item)
        {
            item_ = item;
        }
        public IMargatsniDataItem Item
        {
            get
            {
                return item_;
            }
        }
        IMargatsniDataItem item_ = null;
    }

    public interface IMargatsniPushSource
    {
        event EventHandler<ItemUpdateEventArgs> UpdateItem;
        event EventHandler Reseted;
    }
}
