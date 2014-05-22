using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
namespace Margatsni.Utils.Collections.Extensions
{
    public static class CollectonExtensions
    {
        public delegate bool SortPred<T>(T item1, T item);

        static public void AddSortedItem<T>(this Collection<T> childrens, T item, SortPred<T> pred)
        {
            try
            {
                if (pred != null)
                {
                    for (int i = 0; i < childrens.Count; ++i)
                    {
                        if (pred(item, childrens.ElementAt(i)))
                        {
                            childrens.Insert(i, item);
                            return;//-->
                        }
                    }
                }
                childrens.Add(item);
            }
            catch
            {
            }
        }
        static public void ResortChildrens<T>(this ObservableCollection<T> childrens, T item, SortPred<T> pred)
        {
            try
            {
                if (pred == null )
                {
                    return;//
                }
                int insind = -2;
                int episode_index = -1;
                for (int i = 0; i < childrens.Count; ++i)
                {
                    if (childrens.ElementAt(i).Equals(item))
                    {
                        episode_index = i;
                        if (insind != -2)
                        {
                            if (episode_index < insind) insind--;
                        }
                    }
                    //                if (group.Episodes.ElementAt(i).Item.Updated < field_val && insind == -2)
                    if (pred(item, childrens.ElementAt(i)) && insind == -2)
                    {
                        insind = i;
                        if (episode_index != -1)
                        {
                            if (episode_index < insind) insind--;
                        }
                    }
                }
                if (insind == -1)
                {
                    insind = 0;
                }
                if (episode_index == -1)
                {
                    return;//-->
                }
                if (insind == episode_index)
                {
                    return;//-->
                }

                if (insind == -2)
                {
                    if (episode_index == childrens.Count - 1)
                    {
                        return;//-->
                    }
                    try
                    {
                        childrens.RemoveAt(episode_index);
                        childrens.Add(item);
//                        childrens.Move(episode_index, childrens.Count - 1);
                    }
                    catch
                    {
                    }
                    return;//-->
                }
                try
                {
                    childrens.RemoveAt(episode_index);
                    childrens.Insert(insind, item);
                    //childrens.

//                    childrens.Move(episode_index, insind);
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

    }
}
