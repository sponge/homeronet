namespace Homero.Utility
{
    public class CustomEventHandlers
    {
        public delegate void EventHandler<TSender, TArgs>(TSender sender, TArgs e) where TArgs : System.EventArgs;
    }
}
