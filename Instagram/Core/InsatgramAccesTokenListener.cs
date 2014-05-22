using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Margatsni.Instagram.Core
{
    public class InsatgramAccesTokenListener
    {
        static public string GetToken(Uri uri)
        {
            if (uri.Fragment.StartsWith("#access_token="))
            {
                return uri.Fragment.Substring(14);
            }
            return string.Empty;
        }
    }
}
