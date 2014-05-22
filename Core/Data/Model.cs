using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Margatsni.Core.Data
{
    public class Model
    {
        private ObservableCollection<Item> items_ = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items
        {
            get
            {
                return items_;
            }
        }
    }
}
