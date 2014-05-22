using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Core.Data
{
    public class Item
    {
        public string ImageUrl
        {
            get
            {
                return image_url_;
            }
            set
            {
                image_url_ = value;
            }
        }
        public string Id
        {
            get
            {
                return id_;
            }
            set
            {
                id_ = value;
            }
        }
        public int Priority
        {
            get
            {
                return priority_;
            }
            set
            {
                priority_ = value;
            }
        }
        public int Sort
        {
            get
            {
                return sort_;
            }
            set
            {
                sort_ = value;
            }
        }
        int priority_ = 10;
        int sort_ = 0;
        private string image_url_ = string.Empty;
        private string id_ = string.Empty;
    }
}
