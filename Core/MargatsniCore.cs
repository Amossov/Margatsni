using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows;
using Margatsni.Utils.Collections.Extensions;
namespace Margatsni.Core
{
    public class MargatsniCore
    {

        static public MargatsniCore Instance
        {
            get
            {
                return core_;
            }
        }

        static MargatsniCore core_ = new MargatsniCore();

        public bool RegisterSource(object source, int priority)
        {
            IEnumerable<object> d = source as IEnumerable<object>;
            I.IMargatsniPushSource push_source = source as I.IMargatsniPushSource;
            if (d != null)
            {
                UpdateModel(d, priority);
                INotifyCollectionChanged d1 = source as INotifyCollectionChanged;
                if (d1 != null)
                {
                    prioritys_.Add(source, priority);
                    d1.CollectionChanged += d1_CollectionChanged;
                }
                return true;
            }
            if (push_source != null)
            {
                prioritys_.Add(source, priority);
                push_source.UpdateItem += push_source_UpdateItem;
                push_source.Reseted += push_source_Reseted;
                return true;
            }
            return false;
        }

        void push_source_Reseted(object sender, EventArgs e)
        {
            var pri = prioritys_[sender];
            for (int i = model_.Items.Count - 1; i >= 0; --i)
            {

                if (model_.Items.ElementAt(i).Priority == pri)
                {
                    model_.Items.RemoveAt(i);
                }
            }
        }

        void push_source_UpdateItem(object sender, I.ItemUpdateEventArgs e)
        {
            UpdateModel(new List<I.IMargatsniDataItem>() { e.Item }, prioritys_[sender]);
        }

        void d1_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                UpdateModel(e.NewItems.Cast<object>(), prioritys_[sender]);
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var pri = prioritys_[sender];
                for (int i = model_.Items.Count-1; i >= 0; --i)
                {

                    if (model_.Items.ElementAt(i).Priority == pri)
                    {
                        model_.Items.RemoveAt(i);
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var pri = prioritys_[sender];
                foreach (var t in e.OldItems)
                {
                    I.IMargatsniDataItem it = t as I.IMargatsniDataItem;
                    if (it != null)
                    {
                        RemoveItemFromModel(it, (a, b) => a.Id == b.Id, pri);
                    }else{
                        string ass = t.ToString();
                        if (!string.IsNullOrEmpty(ass)){
                            RemoveItemFromModel(ass, (a, b) => a == b.Id, pri);
                        }
                    }
                }
            }
        }
        private void RemoveItemFromModel<T>(T item_data, Func<T, Data.Item, bool> remo, int pri)
        {
            for (int i = model_.Items.Count - 1; i >= 0; --i)
            {
                var mi = model_.Items.ElementAt(i);
                if (remo(item_data, mi))
                {
                    if (mi.Priority <= pri)
                    {
                        model_.Items.RemoveAt(i);
                    }
                    i = -1;
                }
            }
        }

        public Data.Model Model
        {
            get
            {
                return model_;
            }
        }



        private void UpdateModel(IEnumerable<object> data, int p)
        {
            foreach(var d in data){
                var it = d as I.IMargatsniDataItem;
                if (it != null)
                {
                    UpdateModelItem<I.IMargatsniDataItem>(it, (di, mdi) => { return di.Id == mdi.Id; }, CreateItem, UpdateItem, p);
                }
                else
                {
                    var as_string = d.ToString();
                    if (string.IsNullOrEmpty(as_string))
                    {
                        return;
                    }
                    UpdateModelItem<string>(as_string, (di, mdi) => { return di.Id == mdi; }, CreateItem, UpdateItem, p);
                }
            }
        }
        private void UpdateModelItem<T>(T data_item, Func<Data.Item, T, bool> f,Func<T, int, Data.Item> Create, Func<Data.Item, T, int, bool> Update,  int p) 
        {
            var current = model_.Items.FirstOrDefault(it1 => f(it1, data_item));
            if (current != null)
            {
                if (current.Priority <= p)
                {
                    Update(current, data_item, p);
                    model_.Items.ResortChildrens(current, (a, b) => a.Sort < b.Sort);
                }
            }
            else
            {
                model_.Items.AddSortedItem(Create(data_item, p), (a, b) => a.Sort < b.Sort);
              //  model_.Items.Add(Create(data_item, p));
            }
        }
        private Data.Item CreateItem(I.IMargatsniDataItem item, int p)
        {
            return new Data.Item() { Id = item.Id, ImageUrl = item.ImageUrl, Priority = p , Sort = item.Sort};
        }
        private Data.Item CreateItem(string item, int p)
        {
            return new Data.Item() { Id = item, ImageUrl = item, Priority = p , Sort = 0};
        }
        private bool UpdateItem(Data.Item current, string item, int p)
        {
            current.ImageUrl = item;
            current.Priority = p;
            return true;
        }
        private bool UpdateItem(Data.Item current, I.IMargatsniDataItem item, int p)
        {
            current.ImageUrl = item.ImageUrl;
            current.Priority = p;
            current.Sort = item.Sort;
            return true;
        }

        private static DependencyProperty PriorityProperty =
            DependencyProperty.RegisterAttached("Priority", typeof(int), typeof(MargatsniCore), null);
        public static void SetTypeName(DependencyObject element, int value)
        {
            element.SetValue(PriorityProperty, value);
        }

        public static int GetTypeName(DependencyObject element)
        {
            return (int)element.GetValue(PriorityProperty);
        }


        Data.Model model_ = new Data.Model();
        Dictionary<object, int> prioritys_ = new Dictionary<object, int>();
    }
}
