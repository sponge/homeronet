using System.Collections.Generic;

namespace Homero.Core.Services
{
    public interface IStore
    {
        List<string> Keys { get; }

        void Remove(string key);

        T Get<T>(string key);

        void Set<T>(string key, T value);
    }
}