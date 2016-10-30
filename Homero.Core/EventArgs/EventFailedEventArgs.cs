using System;

namespace Homero.Core.EventArgs
{
    public class EventFailedEventArgs : System.EventArgs
    {
        public System.EventArgs OriginalEventArgs { get; private set; }
        public Exception Exception { get; private set; }

        public EventFailedEventArgs(System.EventArgs originalArgs, Exception e)
        {
            OriginalEventArgs = originalArgs;
            Exception = e;
        }
    }
}