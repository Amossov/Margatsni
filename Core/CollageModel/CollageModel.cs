using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
//using System.Windows.Media;
using System.Collections;//.Specialized;
using System.Collections.Specialized;

namespace Margatsni.Core.CollageModel
{
    public class CollageModel
    {
        public Data.Model SourceModel
        {
            get
            {
                return source_model_;
            }
            set
            {
                if (source_model_ != null)
                {
                    source_model_.Items.CollectionChanged -= Items_CollectionChanged;
                }
                source_model_ = value;
                UpdateModel(source_model_.Items);
                source_model_.Items.CollectionChanged += Items_CollectionChanged;
            }
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateModel(sender as IEnumerable);
        }
       
        public IEnumerable Items
        {
            get
            {
                return items_;
            }
        }
        public IEnumerable SourceItems
        {
            get
            {
                return source_items_;
            }
            set
            {
                if (source_items_ == value)
                {
                    return;
                }
                if (source_items_ != null)
                {
                    INotifyCollectionChanged ncs = source_items_ as INotifyCollectionChanged;
                    if (ncs != null)
                    {
                        ncs.CollectionChanged -= ncs_CollectionChanged;
                    }
                }
                source_items_ = value;
                if (source_items_ != null)
                {
                    UpdateModel(source_items_);
                    INotifyCollectionChanged ncs = source_items_ as INotifyCollectionChanged;
                    if (ncs != null)
                    {
                        ncs.CollectionChanged += ncs_CollectionChanged;
                    }
                }

            }
        }

        void ncs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateModel(sender as IEnumerable);
        }
        void UpdateModel(IEnumerable items)
        {
            items_.Clear();
            ICollection co = items as ICollection;
            int count = co.Count;
            int rows = (int)(Math.Ceiling(Math.Sqrt((double)count)));
            int cols = rows;
            int c = 0;
            double scale = 1.0 / (double)cols;
            foreach (var si in items)
            {
                CollageModelItem new_item = new CollageModelItem();
                new_item.Item = si as Data.Item;
                int row = c % rows;
                int col = c / rows;
                new_item.DX = (double)col / (double)cols;
                new_item.DY = (double)row / (double)rows;
                new_item.Scale = scale;
                items_.Add(new_item);
                c++;
            }
        }


        IEnumerable source_items_ = null;
        ObservableCollection<CollageModelItem> items_ = new ObservableCollection<CollageModelItem>();
        Data.Model source_model_ = null;
        
    }
}
