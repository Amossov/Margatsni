using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Margatsni.Core.CollageModel
{
    public class CollageModelItem
    {
        public Data.Item Item
        {
            get
            {
                return item_;
            }
            set
            {
                item_ = value;
            }
        }
        public double DX
        {
            get
            {
                return dx_;
            }
            set
            {
                dx_ = value;
            }
        }
        public double DY
        {
            get
            {
                return dy_;
            }
            set
            {
                dy_ = value;
            }
        }
        public double Scale
        {
            get
            {
                return scale_;
            }
            set
            {
                scale_ = value;
            }
        }

        Data.Item item_ = null;
        double dx_ = 0;
        double dy_ = 0;
        double scale_ = 1;
    }
}
