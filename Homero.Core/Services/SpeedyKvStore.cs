using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Utility;
using Speedy;

namespace Homero.Core.Services
{
    public class SpeedyKvStore : IStore
    {
        private KeyValueRepository<object> _coreRepository;
        public SpeedyKvStore(string StoreName)
        {
            if (!Directory.Exists(Paths.DataDirectory))
            {
                Directory.CreateDirectory(Paths.DataDirectory);
            }
            _coreRepository = new KeyValueRepository<object>(Paths.DataDirectory, StoreName);
        }

        public List<string> Keys
        {
            get
            {
                return _coreRepository.ReadKeys().ToList();
            }
        }


        public void Remove(string key)
        {
            _coreRepository.Remove(key);
        }

        public T Get<T>(string key)
        {
            object result;
            if (!_coreRepository.TryRead(key, out result))
            {
                return default(T);
            }
            return (T) result;
        }

        public void Set<T>(string key, T value)
        {
            _coreRepository.Write(key, value);
            _coreRepository.Save();
            _coreRepository.Flush();
        }
    }
}
