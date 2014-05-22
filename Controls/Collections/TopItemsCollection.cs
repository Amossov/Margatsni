using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace Margatsni.Controls.Collections
{
    public class TopItemsCollection<T>
    {
        public TopItemsCollection(int max){
            max_ = max;
        }
        public TopItemsCollection(){
        }


        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < max_)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > max_)
                        {
                            TopItems.RemoveAt(max_);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < max_ && e.NewStartingIndex < max_)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < max_)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[max_-1]);
                    }
                    else if (e.NewStartingIndex < max_)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(max_);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < max_)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= max_)
                        {
                            TopItems.Add(Items[max_-1]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < max_)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < max_)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }

        }
        public ObservableCollection<T> Source
        {
            get
            {
                return Items;
            }
            set
            {
                if (Items != null)
                {
                    Items.CollectionChanged -= ItemsCollectionChanged;
                    TopItems.Clear();
                }
                Items = value;
                if (Items != null)
                {
                    while (TopItems.Count < Items.Count && TopItems.Count < max_)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    Items.CollectionChanged += ItemsCollectionChanged;
                }
            }
        }

        public ObservableCollection<T> TopItems
        {
            get
            {
                return top_items_;
            }
        }
        private int max_ = 12;
        ObservableCollection<T> Items = null;
        ObservableCollection<T> top_items_ =new ObservableCollection<T>();
    }

}
