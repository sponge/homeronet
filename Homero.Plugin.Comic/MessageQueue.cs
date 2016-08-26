using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// utility class for comic
namespace Homero.Plugin.Comic
{
    public class MessageQueue<T> : IEnumerable<T>
    {
        private int _limit;
        private List<T> _items;

        public MessageQueue(int limit)
        {
            _limit = limit;
            _items = new List<T>();
        }

        public void Add(T item)
        {
            if (_items.Count > _limit)
            {
                _items.RemoveAt(0);
            }
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
