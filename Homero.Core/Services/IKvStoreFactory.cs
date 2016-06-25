using System;
using System.Collections.Generic;

namespace Homero.Core.Services
{
    public interface IKvStoreFactory
    {
        IStore GetKvStore(string ClassType);
    }

    public class KvStoreFactory : IKvStoreFactory
    {
        #region Singleton
        private static KvStoreFactory _instance;
        public static KvStoreFactory Instance
        {
            get { return _instance ?? (_instance = new KvStoreFactory()); }
        }

        #endregion

        private Dictionary<string, IStore> _backedKvStores; // Ensure only one instance of a configuration is grabbed from the factory.

        public KvStoreFactory()
        {
            if (_instance != null)
            {
                throw new Exception("DOUBLE STORE FACTORY OH NO");
            }
            _backedKvStores = new Dictionary<string, IStore>();
        }

        public IStore GetKvStore(string ClassType)
        {
            if (ClassType == null)
            {
                ClassType = "Core";
            }

            if (_backedKvStores.ContainsKey(ClassType))
            {
                return _backedKvStores[ClassType];
            }

            IStore KvStore = new SpeedyKvStore(ClassType);
            _backedKvStores.Add(ClassType, KvStore);
            return KvStore;
        }
    }
}
