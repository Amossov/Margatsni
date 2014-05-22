using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Windows;
namespace Margatsni.Utils
{
    public class AsyncHelpers
    {
        public static void RunAsync(DispatchedHandler agileCallback)
        {
            Deployment.Current.Dispatcher.BeginInvoke(agileCallback);
        }

    }
}
