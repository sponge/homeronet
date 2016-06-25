using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Services
{
    public interface IStore
    {
        List<string> Keys { get; }
        void Remove(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value);

    }
}
