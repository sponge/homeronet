using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Utility
{
    public class CustomEventHandlers
    {
        public delegate void EventHandler<TSender, TArgs>(TSender sender, TArgs e) where TArgs : System.EventArgs;
    }
}
