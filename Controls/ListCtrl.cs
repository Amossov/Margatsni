using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections;
//using Narr8.Phone.Common;

namespace Margatsni.Controls
{
    public class ListCtrlItem : ContentControl
    {
        public ListCtrlItem()
        {
            DefaultStyleKey = typeof(ListCtrlItem);
        }
        ~ListCtrlItem()
        {
            System.Diagnostics.Debug.WriteLine("~ListCtrlItem()");
        }
        public static readonly DependencyProperty IsSelectedProperty =
             DependencyProperty.Register(
                 "IsSelected",
                 typeof(bool),
                 typeof(ListCtrlItem),
                 new PropertyMetadata(false));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
    }
    public class ListCtrl:ItemsControl
    {
        static ListCtrl()
        {
  //          Microsoft.Phone.Controls.TiltEffect.TiltableItems.Add(typeof(ListCtrlItem));
        }
        public ListCtrl()
        {
            DefaultStyleKey = typeof(ListCtrl);
            Loaded += ListCtrl_Loaded;
        }

        void ListCtrl_Loaded(object sender, RoutedEventArgs e)
        {
    //        var sp = this.GetVisualChild<StackPanel>();
//            this.
        }
        ~ListCtrl()
        {
            System.Diagnostics.Debug.WriteLine("~ListCtrl()");
        }
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove || 
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (var d in e.OldItems)
                {
                    try
                    {
                        selected_items_.Remove(d);
                    }
                    catch
                    {
                    }
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                selected_items_.Clear();
            }
        }

        public static readonly DependencyProperty ItemsContainerStyleProperty =
             DependencyProperty.Register(
                 "ItemsContainerStyle",
                 typeof(Style),
                 typeof(ListCtrl),
                 new PropertyMetadata(null));

        public Style ItemsContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemsContainerStyleProperty);
            }
            set
            {
                SetValue(ItemsContainerStyleProperty, value);
            }
        }

        public static readonly DependencyProperty SelectionEnabledProperty =
             DependencyProperty.Register(
                 "SelectionEnabled",
                 typeof(bool),
                 typeof(ListCtrl),
                 new PropertyMetadata(false));

        public bool SelectionEnabled
        {
            get
            {
                return (bool)GetValue(SelectionEnabledProperty);
            }
            set
            {
                SetValue(SelectionEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty MultiSelectionEnabledProperty =
             DependencyProperty.Register(
                 "MultiSelectionEnabled",
                 typeof(bool),
                 typeof(ListCtrl),
                 new PropertyMetadata(false));

        public bool MultiSelectionEnabled
        {
            get
            {
                return (bool)GetValue(MultiSelectionEnabledProperty);
            }
            set
            {
                SetValue(MultiSelectionEnabledProperty, value);
            }
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListCtrlItem;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            ListCtrlItem balloon = new ListCtrlItem();

            if (this.ItemsContainerStyle != null)
                balloon.Style = this.ItemsContainerStyle;

            return balloon;
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var lbi = element as ListCtrlItem;
            if (!Object.ReferenceEquals(element, item) && lbi != null)
            {
                lbi.Tap += lbi_Tap;
                lbi.IsSelected = ItemContainerGenerator.IndexFromContainer(element) == selected_index_;
            }
        /*    Balloon balloon = element as Balloon;

            if (!Object.ReferenceEquals(element, item) && balloon != null)
            {
                if (this.HeaderTemplate != null)
                {
                    balloon.HeaderContent = item;
                    balloon.HeaderTemplate = this.HeaderTemplate;
                }
            }*/
        }
        public int SelectedIndex
        {
            get
            {
                return selected_index_;
            }
            set
            {
                if (selected_index_ == value)
                {
                    return;
                }
                if (selected_index_ != -1)
                {
                    var current_selected1 = ItemContainerGenerator.ContainerFromIndex(selected_index_);
                    var current_selected = current_selected1 as ListCtrlItem;
                    if (current_selected != null)
                    {
                        current_selected.IsSelected = false;
                    }
                }
                selected_index_ = value;
                if (selected_index_ != -1)
                {
                    var current_selected1 = ItemContainerGenerator.ContainerFromIndex(selected_index_);
                    var current_selected = current_selected1 as ListCtrlItem;
                    if (current_selected != null)
                    {
                        current_selected.IsSelected = true;
                    }
                }
            }
        }

        void lbi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var it = sender as ListCtrlItem;
            if (ItemClicked != null)
            {
                ItemClicked(this, new ItemClickEventArgs(it.Content));
            }
            if (!SelectionEnabled)
            {
                return;//-->
            }
            if (MultiSelectionEnabled)
            {
                if (it.IsSelected)
                {
                    selected_items_.Remove(it.Content);
                }
                it.IsSelected = !it.IsSelected;
                if (it.IsSelected)
                {
                    selected_items_.Add(it.Content);
                }
                if (it.IsSelected && ItemSelected != null)
                {
                    ItemSelected(this, new ItemClickEventArgs(it.Content));
                }

                return;
            }
            if (selected_index_ != -1)
            {
                var current_selected1 = ItemContainerGenerator.ContainerFromIndex(selected_index_);
                var current_selected = current_selected1 as ListCtrlItem;
                if (current_selected == it){
                    return;//-->
                }
                if (current_selected != null)
                {
                    current_selected.IsSelected = false;
                }
            }
            it.IsSelected = true;
            selected_index_ = ItemContainerGenerator.IndexFromContainer(it);
            if (ItemSelected != null)
            {
                ItemSelected(this, new ItemClickEventArgs(it.Content));
            }
        }

        public class ItemClickEventArgs : EventArgs
        {
            public ItemClickEventArgs(object clicked_item)
            {
                clicked_item_ = clicked_item;
            }
            public object ClickedItem
            {
                get
                {
                    return clicked_item_;
                }
            }
            object clicked_item_ = null;
        }
        public event EventHandler<ItemClickEventArgs> ItemClicked;
        public event EventHandler<ItemClickEventArgs> ItemSelected;

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            ListCtrlItem balloon = (ListCtrlItem)element;

            if (!balloon.Equals(item))
            {
                balloon.ClearValue(ListCtrlItem.ContentProperty);
                balloon.ClearValue(ListCtrlItem.ContentTemplateProperty);
                balloon.Tap -= lbi_Tap;
/*                balloon.ClearValue(Balloon.HeaderContentProperty);
                balloon.ClearValue(Balloon.HeaderTemplateProperty);*/
            }
        }

        public IEnumerable SelectedItems
        {
            get
            {
                return selected_items_;
            }
        }

        private ObservableCollection<object> selected_items_ = new ObservableCollection<object>();

        int selected_index_ = -1;
    }
}
