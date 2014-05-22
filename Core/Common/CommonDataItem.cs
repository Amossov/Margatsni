using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Core.Common
{
    public class CommonDataItem:I.IMargatsniDataItem
    {
        public string Id 
        { 
            get { return id_; } 
            set { id_ = value; } 
        }

        public string ImageUrl 
        {
            get { return image_url_; }
            set { image_url_ = value; }
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

        private string id_ = string.Empty;
        private string image_url_ = string.Empty;
        private int sort_ = 0;
    }
}
